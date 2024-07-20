//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Specialized;

using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Wsp;

namespace MessagingToolkit.Wap.Wsp.Headers
{

    /// <summary>
    /// This class implements the WAP default codepage for Header encoding/decoding
    /// </summary>
    public class WAPCodePage : CodePage
    {
        private const string TtCtEnc = "content-encoding";
        private const string TtWkParams = "wk-params-1.3";
        private const string TtCharsets = "charsets";
        private const string TtLng = "languages";
        private const string TtCControl = "cache-control";
        private const string TtCTypes = "content-types-1.3";

        //private static SimpleDateFormat fmt = new SimpleDateFormat("EEE, d MMM yyyy HH:mm:ss zzz");

        private TransTable transTable;

        /// <summary>
        /// Construct a new WAP Code Page. Encoding is done according to WAP encoding
        /// version 1.1
        /// </summary>
        /// <throws>IOException if the code-page translation table cannot be found </throws>
        protected internal WAPCodePage():this(1, 5)
        {
        }

        /// <summary>
        /// Construct a new WAP Code Page
        /// </summary>
        /// <param name="major">WAP major version</param>
        /// <param name="minor">WAP minor version</param>
        /// <throws>IOException if the code-page translation table cannot be found </throws>
        protected internal WAPCodePage(int major, int minor) : base(1, true, "default")
        {
            StringBuilder rn = new StringBuilder("wsp-headers-").Append(major).Append(".").Append(minor);
            transTable = TransTable.GetTable(rn.ToString());
        }

        /// <summary>
        /// Returns an instance of the WAP Codepage. Encoding is done according
        /// to encoding version 1.1
        /// </summary>
        /// <returns>WAP code page instance</returns>
        public static WAPCodePage GetInstance()
        {
            try
            {
                return new WAPCodePage(1, 5);
            }
            catch (IOException e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Returns an instance of the WAP Codepage. Encoding is done according
        /// to the specified encoding version.
        /// </summary>
        /// <param name="major">the major encoding version</param>
        /// <param name="minor">the minor encoding version</param>
        /// <returns></returns>
        /// <throws>  IllegalArgumentException if no encoding exists for the </throws>
        public static WAPCodePage GetInstance(int major, int minor)
        {
            try
            {
                return new WAPCodePage(major, minor);
            }
            catch (IOException e)
            {
                throw new ArgumentException(major + "." + minor + ": No encoding available");
            }
        }

        public override byte[] Encode(string key, string value)
        {
            string lowKey = key.ToLower().Trim();
            int o = transTable.Str2Code(lowKey);
                        
            //Logger.LogThis("Encode: '" + key + "' -> " + o, LogLevel.Verbose);

            // Not a Well-Known value? 
            if (o == -1)
            {
               
                Logger.LogThis(key + ": Not a well known value, using text-string encoding", LogLevel.Verbose);
                return Wsp.Headers.Encoding.EncodeHeader(key, Wsp.Headers.Encoding.TextString(value));
            }

            MemoryStream output = new MemoryStream();
            short wk = (short)(o);

            try
            {
                if ("content-base".Equals(lowKey))
                {
                    throw new HeaderParseException(key + ": Deprecated Header field");
                }

                try
                {
                    // Invoke handler using reflection..
                    EncodeHeader(lowKey, output, wk, value);
                }
                catch (MethodAccessException nsme)
                {
                    Logger.LogThis(key + ": No handler for this header, using text-string encoding", LogLevel.Warn);
                    byte[] ba;
                    ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(value));
                    output.Write(ba, 0, ba.Length);
                }
            }
            catch (HeaderParseException hpe)
            {
                throw hpe;
            }
            catch (IOException ex)
            {
                throw new HeaderParseException("I/O Error while encoding.", ex);
            }
            catch (Exception uex)
            {
                Logger.LogThis("Unexpected exception while encoding " + key + "=" + value, LogLevel.Warn);
                Logger.LogThis(uex.StackTrace, LogLevel.Warn);
                throw new HeaderParseException("Unexpected exception while encoding.", uex);
            }

            return output.ToArray();
        }

        public override byte[] Encode(string key, ref DateTime value)
        {
            int o = transTable.Str2Code(key.Trim());
            long secs = (value == DateTime.MinValue) ? 0 : (value.Ticks / 1000);
            byte[] hv = Wsp.Headers.Encoding.LongInteger(secs);

            if (o == -1)
            {
                return Wsp.Headers.Encoding.EncodeHeader(key, hv);
            }
            else
            {
                return Wsp.Headers.Encoding.EncodeHeader((short)((Int32)o), hv);
            }
        }

        public override byte[] Encode(string key, long value)
        {
            int o = transTable.Str2Code(key.Trim());
            short wk = ((o == -1) ? (short)(-1) : (short)(o));

            if (value < 0)
            {
                throw new HeaderParseException(value + ": negative integer values not accepted");
            }

            byte[] hv = Wsp.Headers.Encoding.IntegerValue(value);

            if (o == -1)
            {
                return Wsp.Headers.Encoding.EncodeHeader(key, hv);
            }
            else
            {
                return Wsp.Headers.Encoding.EncodeHeader(wk, hv);
            }
        }

        public virtual void EncodeAccept(Stream hdrs, short wk, string value)
        {
            for (IEnumerator e = HeaderToken.Tokenize(value); e.MoveNext(); )
            {
                HeaderToken ht = (HeaderToken)e.Current;
                string token = ht.Token;

                int code = TransTable.GetTable(TtCTypes).Str2Code(token);

                if (code == -1)
                {
                    byte[] ba;
                    ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(token));
                    hdrs.Write(ba, 0, ba.Length);
                }
                else
                {
                    byte[] ba2;
                    ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.ShortInteger((short)code));
                    hdrs.Write(ba2, 0, ba2.Length);
                }
            }
        }

        public virtual void EncodeAcceptCharset(Stream hdrs, short wk, string value)
        {
            for (IEnumerator e = HeaderToken.Tokenize(value); e.MoveNext(); )
            {
                HeaderToken ht = (HeaderToken)e.Current;
                string token = ht.Token;

                if ("*".Equals(token))
                {
                    byte[] ba;
                    ba = new byte[] { (byte)wk, (byte)(0xff) };
                    hdrs.Write(ba, 0, ba.Length);

                    continue;
                }

                int wkl = TransTable.GetTable(TtCharsets).Str2Code(token);
                string qf = ht.GetParameter("q");

                if (qf == null)
                {
                    if (wkl != -1)
                    {
                        byte[] ba2;
                        ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.IntegerValue((long)wkl));
                        hdrs.Write(ba2, 0, ba2.Length);
                    }
                    else
                    {
                        byte[] ba3;
                        ba3 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.ExtensionMedia(token));
                        hdrs.Write(ba3, 0, ba3.Length);
                    }
                }
                else
                {
                    float q = Single.Parse(qf);
                    byte[] qb = Wsp.Headers.Encoding.QualityFactor(q);
                    byte[] lng = null;

                    if (wkl != -1)
                    {
                        lng = Wsp.Headers.Encoding.ShortInteger((short)wkl);
                    }
                    else
                    {
                        lng = Wsp.Headers.Encoding.TextString(value);
                    }

                    byte[] dt = new byte[qb.Length + lng.Length + 1];
                    dt[0] = (byte)(qb.Length + lng.Length);
                    Array.Copy(lng, 0, dt, 1, lng.Length);
                    Array.Copy(qb, 0, dt, lng.Length + 1, qb.Length);
                    byte[] ba4;
                    ba4 = Wsp.Headers.Encoding.EncodeHeader(wk, dt);
                    hdrs.Write(ba4, 0, ba4.Length);
                }
            }
        }

        public virtual void EncodeAcceptEncoding(Stream hdrs, short wk, string value)
        {
            for (IEnumerator e = HeaderToken.Tokenize(value); e.MoveNext(); )
            {
                HeaderToken ht = (HeaderToken)e.Current;
                string token = ht.Token;
                byte[] encoding = null;
                byte[] param = null;
                                
                int code = TransTable.GetTable(TtCtEnc).Str2Code(token);
    
                if (code != -1)
                {
                    encoding = new byte[] { (byte)(code & 0xff) };
                }
                else
                {
                    encoding = Wsp.Headers.Encoding.TextString(token);
                }

                // Q-Factor?
                string qFactor = ht.GetParameter("q");

                if (qFactor != null)
                {
                    float qf = Single.Parse(qFactor);
                    param = Wsp.Headers.Encoding.QualityFactor(qf);
                }

                byte[] ba;
                ba = Wsp.Headers.Encoding.ShortInteger(wk);
                hdrs.Write(ba, 0, ba.Length);

                if ((param != null) || "*".Equals(token))
                {
                    byte[] ba2;
                    ba2 = Wsp.Headers.Encoding.ValueLength(encoding.Length + ((param == null) ? 0 : param.Length));
                    hdrs.Write(ba2, 0, ba2.Length);
                }

                byte[] ba3;
                ba3 = encoding;
                hdrs.Write(ba3, 0, ba3.Length);

                if (param != null)
                {
                    byte[] ba4;
                    ba4 = param;
                    hdrs.Write(ba4, 0, ba4.Length);
                }
            }
        }

        public virtual void EncodeAcceptLanguage(Stream hdrs, short wk, string value)
        {          
            for (IEnumerator e = HeaderToken.Tokenize(value); e.MoveNext(); )
            {               
                HeaderToken ht = (HeaderToken)e.Current;
                string token = ht.Token;

                if ("*".Equals(token))
                {
                    byte[] ba;
                    ba = new byte[] { (byte)wk, (byte)0xff };
                    hdrs.Write(ba, 0, ba.Length);
                    continue;
                }

                int wkl = TransTable.GetTable(TtLng).Str2Code(token);
                string qf = ht.GetParameter("q");

                if (qf == null)
                {
                    if (wkl != -1)
                    {
                        byte[] ba2;
                        ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.ShortInteger((short)wkl));
                        hdrs.Write(ba2, 0, ba2.Length);
                    }
                    else
                    {
                        byte[] ba3;
                        ba3 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.ExtensionMedia(token));
                        hdrs.Write(ba3, 0, ba3.Length);                        
                    }
                }
                else
                {
                    float q = Single.Parse(qf);
                    byte[] qb = Wsp.Headers.Encoding.QualityFactor(q);
                    byte[] lng = null;

                    if (wkl != -1)
                    {
                        lng = Wsp.Headers.Encoding.ShortInteger((short)wkl);
                    }
                    else
                    {
                        lng = Wsp.Headers.Encoding.TextString(value);
                    }

                    byte[] dt = new byte[qb.Length + lng.Length + 1];
                    dt[0] = (byte)(qb.Length + lng.Length);
                    Array.Copy(lng, 0, dt, 1, lng.Length);
                    Array.Copy(qb, 0, dt, lng.Length + 1, qb.Length);
                    byte[] ba4;
                    ba4 = Wsp.Headers.Encoding.EncodeHeader(wk, dt);
                    hdrs.Write(ba4, 0, ba4.Length);
                }
            }
        }

        public virtual void EncodeAcceptRanges(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            string nv = value.ToLower().Trim();

            if ("none".Equals(nv))
            {
                byte[] ba;
                ba = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)128 });
                hdrs.Write(ba, 0, ba.Length);
            }
            else if ("bytes".Equals(nv))
            {
                byte[] ba2;
                ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)129 });
                hdrs.Write(ba2, 0, ba2.Length);
            }
            else
            {
                byte[] ba3;
                ba3 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TokenText(value));
                hdrs.Write(ba3, 0, ba3.Length);
            }
        }

        public virtual void EncodeCacheControl(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }
            value = value.Trim();
            if ("".Equals(value))
            {
                return;
            }
            string nv = value.ToLower();

            int epos = nv.IndexOf('=');
            string arg = null;
            if (epos > 0)
            {
                arg = nv.Substring(epos + 1);
                nv = nv.Substring(0, (epos) - (0));
            }
            if ("no-cache".Equals(nv))
            {
                byte[] ba;
                ba = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)128 });
                hdrs.Write(ba, 0, ba.Length);
            }
            else if ("no-store".Equals(nv))
            {
                byte[] ba2;
                ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)129 });
                hdrs.Write(ba2, 0, ba2.Length);
            }
            else if ("only-if-cached".Equals(nv))
            {
                byte[] ba3;
                ba3 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)133 });
                hdrs.Write(ba3, 0, ba3.Length);
            }
            else if ("public".Equals(nv))
            {
                byte[] ba4;
                ba4 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)134 });
                hdrs.Write(ba4, 0, ba4.Length);
            }
            else if ("private".Equals(nv))
            {
                byte[] ba5;
                ba5 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)135 });
                hdrs.Write(ba5, 0, ba5.Length);
            }
            else if ("no-transform".Equals(nv))
            {
                byte[] ba6;
                ba6 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)136 });
                hdrs.Write(ba6, 0, ba6.Length);
            }
            else if ("must-revalidate".Equals(nv))
            {
                byte[] ba7;
                ba7 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)137 });
                hdrs.Write(ba7, 0, ba7.Length);
            }
            else if ("proxy-revalidate".Equals(nv))
            {
                byte[] ba8;
                ba8 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)138 });
                hdrs.Write(ba8, 0, ba8.Length);
            }
            else if ("max-age".Equals(nv))
            {
                byte[] ba9;
                ba9 = Wsp.Headers.Encoding.EncodeHeader(wk, DeltaSeconds((byte)130, arg));
                hdrs.Write(ba9, 0, ba9.Length);
            }
            else if ("max-stale".Equals(nv))
            {
                byte[] ba10;
                ba10 = Wsp.Headers.Encoding.EncodeHeader(wk, DeltaSeconds((byte)131, arg));
                hdrs.Write(ba10, 0, ba10.Length);
            }
            else if ("min-fresh".Equals(nv))
            {
                byte[] ba11;
                ba11 = Wsp.Headers.Encoding.EncodeHeader(wk, DeltaSeconds((byte)132, arg));
                hdrs.Write(ba11, 0, ba11.Length);
            }
            else if ("s-maxage".Equals(nv))
            {
                byte[] ba12;
                ba12 = Wsp.Headers.Encoding.EncodeHeader(wk, DeltaSeconds((byte)139, arg));
                hdrs.Write(ba12, 0, ba12.Length);
            }
            else
            {
                byte[] ba13;
                ba13 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TokenText(value));
                hdrs.Write(ba13, 0, ba13.Length);
            }
        }

        private byte[] DeltaSeconds(byte token, string arg)
        {
            long ds = Int64.Parse(arg);
            byte[] iv = Wsp.Headers.Encoding.IntegerValue(ds);
            byte[] bytes = new byte[iv.Length + 1];
            Array.Copy(bytes, 0, bytes, 1, iv.Length);
            bytes[0] = token;
            return bytes;
        }

        public virtual void EncodeConnection(Stream hdrs, short wk, string value)
        {
            if (value != null && "CLOSE".ToUpper().Equals(value.Trim().ToUpper()))
            {
                byte[] ba;
                ba = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)128 });
                hdrs.Write(ba, 0, ba.Length);
            }
            else
            {
                byte[] ba2;
                ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TokenText(value));
                hdrs.Write(ba2, 0, ba2.Length);
            }
        }

        public virtual void EncodeContentEncoding(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            int code = TransTable.GetTable(TtCtEnc).Str2Code(value.Trim());

            if (code != -1)
            {
                byte[] ba;
                ba = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { (byte)(code & 0xff) });
                hdrs.Write(ba, 0, ba.Length);
            }
            else
            {
                byte[] ba2;
                ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TokenText(value));
                hdrs.Write(ba2, 0, ba2.Length);
            }
        }


        public virtual void EncodeContentId(Stream hdrs, short wk, string value)
        {
            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.QuotedString(value));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeContentLanguage(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            int code = TransTable.GetTable(TtLng).Str2Code(value.Trim());

            if (code != -1)
            {
                byte[] ba;
                ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.ShortInteger((short)code));
                hdrs.Write(ba, 0, ba.Length);
            }
            else
            {
                byte[] ba2;
                ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TokenText(value));
                hdrs.Write(ba2, 0, ba2.Length);
            }
        }

        public virtual void EncodeContentLength(Stream hdrs, short wk, string value)
        {
            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.IntegerValue(Int64.Parse(value)));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeContentLocation(Stream hdrs, short wk, string value)
        {
            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(value));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeContentType(Stream hdrs, short wk, string value)
        {
            TransTable wkp = TransTable.GetTable(TtWkParams);
            // tokenize header-value
            IEnumerator e = HeaderToken.Tokenize(value);
            e.MoveNext();
            HeaderToken token = (HeaderToken)e.Current;
            string contentType = token.Token;
            int code = TransTable.GetTable(TtCTypes).Str2Code(contentType);

            // set primary value
            byte[] ctv = null;

            if (code == -1)
            {
                ctv = Wsp.Headers.Encoding.TextString(contentType);
            }
            else
            {
                ctv = Wsp.Headers.Encoding.ShortInteger((short)code);
            }

            // handle parameters
            MemoryStream outputStream = new MemoryStream();
            string type = token.GetParameter("type");
            if (type == null)
            {
                type = token.GetParameter("type[mpr]");
            }

            if (type != null)
            {
                // If the content-type is application/vnd.wap.multipart.related,
                // type encoding differs...
                StringBuilder sb = new StringBuilder("type");
                if ("application/vnd.wap.multipart.related".ToUpper().Equals(contentType.ToUpper()))
                {
                    sb.Append("[MPR]");
                }
                // TODO: contentType parameters are not always encoded as String 
                // (might be Constrained-encoding)
                byte[] ba;
                ba = Wsp.Headers.Encoding.EncodeHeader((short)wkp.Str2Code(sb.ToString()), Wsp.Headers.Encoding.TextString(type));
                outputStream.Write(ba, 0, ba.Length);
            }

            string start = token.GetParameter("start");

            if (start != null)
            {
                byte[] ba2;
                ba2 = Wsp.Headers.Encoding.EncodeHeader((short)wkp.Str2Code("start"), Wsp.Headers.Encoding.TextString(start));
                outputStream.Write(ba2, 0, ba2.Length);
            }

            string name = token.GetParameter("name");

            if (name != null)
            {
                byte[] ba3;
                ba3 = Wsp.Headers.Encoding.EncodeHeader((short)wkp.Str2Code("name"), Wsp.Headers.Encoding.TextString(name));
                outputStream.Write(ba3, 0, ba3.Length);
            }

            string charset = token.GetParameter("charset");

            if (charset != null)
            {
                // Replace _ with -
                string cset = charset.Replace('_', '-');
                int wkl = TransTable.GetTable(TtCharsets).Str2Code(cset);
                short cwk = (short)wkp.Str2Code("charset");
                if (wkl != -1)
                {
                    byte[] ba4;
                    ba4 = Wsp.Headers.Encoding.EncodeHeader(cwk, Wsp.Headers.Encoding.ShortInteger((short)wkl));
                    outputStream.Write(ba4, 0, ba4.Length);
                }
                else
                {
                    Logger.LogThis(charset + ": Ignoring unknown charset", LogLevel.Warn);
                }
            }

            // create header
            byte[] parameters = outputStream.ToArray();

            if (parameters.Length == 0)
            {
                byte[] ba5;
                ba5 = Wsp.Headers.Encoding.EncodeHeader(wk, ctv);
                hdrs.Write(ba5, 0, ba5.Length);
            }
            else
            {
                byte[] length = Wsp.Headers.Encoding.UIntVar(parameters.Length + ctv.Length);
                byte[] head = new byte[length.Length + parameters.Length + ctv.Length];

                Array.Copy(length, 0, head, 0, length.Length);
                Array.Copy(ctv, 0, head, length.Length, ctv.Length);
                Array.Copy(parameters, 0, head, (ctv.Length + length.Length), parameters.Length);

                byte[] ba6;
                ba6 = Wsp.Headers.Encoding.EncodeHeader(wk, head);
                hdrs.Write(ba6, 0, ba6.Length);
            }
        }

        public virtual void EncodeContentDisposition(Stream hdrs, short wk, string value)
        {
            if (value != null)
            {
                if (value.ToUpper().Equals("FORM-DATA"))
                {
                    byte[] ba;
                    ba = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { 1, (byte)128 });
                    hdrs.Write(ba, 0, ba.Length);
                }
                else if (value.ToUpper().Equals("ATTACHMENT"))
                {
                    byte[] ba2;
                    ba2 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { 1, (byte)129 });
                    hdrs.Write(ba2, 0, ba2.Length);
                }
                else if (value.ToUpper().Equals("INLINE"))
                {
                    byte[] ba3;
                    ba3 = Wsp.Headers.Encoding.EncodeHeader(wk, new byte[] { 1, (byte)130 });
                    hdrs.Write(ba3, 0, ba3.Length);
                }
                else
                {
                    // Token-Text
                    byte[] tt = Wsp.Headers.Encoding.TokenText(value);
                    byte[] ln = Wsp.Headers.Encoding.ValueLength(tt);
                    byte[] lt = new byte[ln.Length + tt.Length];
                    Array.Copy(ln, 0, lt, 0, ln.Length);
                    Array.Copy(tt, 0, lt, ln.Length, tt.Length);

                    byte[] ba4;
                    ba4 = Wsp.Headers.Encoding.EncodeHeader(wk, lt);
                    hdrs.Write(ba4, 0, ba4.Length);
                }
            }
        }

        public virtual void EncodeFrom(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(value.Trim()));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeHost(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(value.Trim()));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeIfMatch(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(value.Trim()));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeIfNonMatch(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(value.Trim()));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeIfRange(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(value.Trim()));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeMaxForwards(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.IntegerValue(Int64.Parse(value)));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeReferer(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.UriValue(value.Trim()));
            hdrs.Write(ba, 0, ba.Length);
        }

        public virtual void EncodeUserAgent(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.TextString(value.Trim()));
            hdrs.Write(ba, 0, ba.Length);
        }


        public virtual void EncodeProfile(Stream hdrs, short wk, string value)
        {
            if (value == null)
            {
                return;
            }

            byte[] ba;
            ba = Wsp.Headers.Encoding.EncodeHeader(wk, Wsp.Headers.Encoding.UriValue(value.Trim()));
            hdrs.Write(ba, 0, ba.Length);
        }

        public override Header Decode(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            int c1 = d.UInt8;
            string key = null;
            string val = null;

            if ((c1 & 0x80) != 0)
            {
                c1 = c1 & 0x7f;
                key = transTable.Code2Str(c1);
               
                //Logger.LogThis("code2str(0x" + Convert.ToString(c1, 16) + ")=" + key, LogLevel.Verbose);
                
                byte[] fieldValue = d.GetBytes(data.Length - 1);

                if (key == null)
                {
                    key = "0x" + Convert.ToString(c1, 16);
                    Logger.LogThis(key + ": unknown header", LogLevel.Warn);
                }
                else
                {
                    try
                    {
                        val = DecodeHeaderField(key, fieldValue);
                    }
                    catch (MethodAccessException nsme)
                    {
                        Logger.LogThis("'" + key + "': Header decoding not yet implemented :-(", LogLevel.Warn);
                    }
                    catch (Exception e)
                    {
                        Logger.LogThis("Unable to decode header " + key, LogLevel.Warn);
                        Logger.LogThis(e.Message, LogLevel.Warn);
                    }
                }
            }
            else
            {
                d.Seek(-1);
                key = d.CString;
                val = d.CString;
            }

            return new Header(key, val);
        }

        public virtual string DecodeAcceptLanguage(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            string retval = null;

            int c1 = d.UInt8;

            if (c1 > 31)
            {
                // Constrained encoding
                if ((c1 & 0x80) != 0)
                {
                    // Short integer
                    c1 = c1 & 0x7f;
                    retval = TransTable.GetTable(TtLng).Code2Str(c1);
                }
                else
                {
                    d.Seek(-1);
                    retval = d.CString;
                }
            }
            else
            {
                // General form 
            }

            return retval;
        }

        public virtual string DecodeDate(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            DateTime dt = d.DateValue;
            //return SupportClass.FormatDateTime(fmt, dt);
            // RFC1123Pattern - http://book.javanb.com/NET-For-Java-Developers-Migrating-To-Csharp/0672324024_ch13lev1sec3.html
            return dt.ToString("R");    
        }

        public virtual string DecodeServer(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            return d.TextString;
        }

        public virtual string DecodeContentId(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            // remove quote
            d.Seek(1);
            return d.CString;
        }

        public virtual string DecodeContentDisposition(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            WSPDecoder d = new WSPDecoder(data);
            long l = d.ValueLength;
            int o = d.UInt8;
            switch (o)
            {

                case 128:
                    sb.Append("Form-Data");
                    break;

                case 129:
                    sb.Append("Attachment");
                    break;

                case 130:
                    sb.Append("Inline");
                    break;

                default:
                    d.Seek(-1);
                    sb.Append(d.TextString);
                    break;

            }
            // TODO Parse parameters...
            byte[] parms = d.GetBytes(d.RemainingOctets);
            return sb.ToString();
        }

        public virtual string DecodeContentLength(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            return Convert.ToString(d.IntegerValue);
        }

        public virtual string DecodeContentLocation(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            return d.CString;
        }

        public virtual string DecodeContentType(byte[] data)
        {
            string retval = null;
            WSPDecoder d = new WSPDecoder(data);
            int o = d.UInt8;
            if (o <= 31)
            {
                // General Form
                d.Seek(-1);
                long len = d.ValueLength;
                if (len == 0)
                {
                    return "";
                }
                o = d.UInt8;
                if ((o & 0x80) != 0)
                {
                    // Short-Integer
                    short wk = (short)(o & 0x7f);
                    retval = TransTable.GetTable(TtCTypes).Code2Str(wk);
                }
                else if (o <= 30)
                {
                    // long-integer
                    d.Seek(-1);
                    int wk = (int)d.LongInteger;
                    retval = TransTable.GetTable(TtCTypes).Code2Str(wk);
                }
                else
                {
                    // *TEXT EOF
                    d.Seek(-1);
                    retval = d.TextString;
                }
                byte[] parameters = d.GetBytes(d.RemainingOctets);
                if (parameters.Length > 0)
                {
                    WSPDecoder param = new WSPDecoder(parameters);
                    StringBuilder buf = new StringBuilder();
                    buf.Append(retval);
                    while (!param.EOF)
                    {
                        string code = null;
                        try
                        {
                            int c = (int)param.IntegerValue;
                            TransTable wkp = TransTable.GetTable(TtWkParams);
                            code = wkp.Code2Str((int)c);
                            string val = null;
                            switch (c)
                            {

                                case 0x01:  // charset
                                    int charset = param.UInt8;
                                    if ((charset & 0x80) != 0)
                                    {
                                        // Short integer
                                        charset = charset & 0x7f;
                                        // Lookup charset
                                        val = TransTable.GetTable(TtCharsets).Code2Str(charset);
                                    }
                                    break;

                                case 0x05:
                                // name version 1.1
                                case 0x06:
                                // fileName version 1.1
                                case 0x07:
                                // name version 1.4
                                case 0x08:
                                // fileName version 1.4
                                case 0x09:
                                // type
                                case 0x0A:
                                // start
                                case 0x0B:
                                // start info
                                case 0x0C:
                                // comment
                                case 0x0D:
                                // domain
                                case 0x0F:
                                // path
                                case 0x12:  // MAC
                                    val = param.CString;
                                    break;
                            }
                            if (val != null)
                            {
                                buf.Append("; ").Append(code).Append("=").Append(val);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogThis("DecodeContentType: parameter decoding failed, ignoring parameter " + code, LogLevel.Info);
                            Logger.LogThis(e.Message, LogLevel.Info);
                        }
                    }
                    retval = buf.ToString();
                }
            }
            else
            {
                // Constrained encoding
                if ((o & 0x80) != 0)
                {
                    // Short integer
                    short wk = (short)(o & 0x7f);
                    retval = TransTable.GetTable(TtCTypes).Code2Str(wk);
                }
                else
                {
                    // *Text EOF 
                    d.Seek(-1);
                    retval = d.TextString;
                }
            }
            return retval;
        }

        public virtual string DecodeConnection(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            int o = d.UInt8;
            if (o == 128)
            {
                return "CLOSE";
            }
            d.Seek(-1);
            return d.CString;
        }

        public virtual string DecodeVia(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            return d.TextString;
        }

        public virtual string DecodeWarning(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            int o = d.UInt8, code = 0;
            string agent = null, txt = null;

            if ((o & 0x80) != 0)
            {
                // Warn-Code
                code = o & 0x7f;
            }
            else
            {
                long generatedAux = d.ValueLength;
                code = d.GetShortInteger();
                agent = d.TextString;
                txt = d.TextString;
            }
            string cTxt = null;
            switch (code)
            {

                case 10: cTxt = "110 Response is stale"; break;

                case 11: cTxt = "111 Revalidation failed"; break;

                case 12: cTxt = "112 Disconnected operation"; break;

                case 13: cTxt = "113 Heuristic expiration"; break;

                case 99: cTxt = "199 Miscellaneous (persistent) warning"; break;

                case 14: cTxt = "214 Transformation applied"; break;

                default: cTxt = Convert.ToString(code);
                    break;

            }
            StringBuilder sb = new StringBuilder(cTxt);
            if (agent != null)
            {
                sb.Append("; agent=\"").Append(agent).Append('"');
            }
            if (txt != null)
            {
                sb.Append("; text=").Append(txt);
            }
            return sb.ToString();
        }

        public virtual string DecodeExpires(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            DateTime dt = d.DateValue;
            //return SupportClass.FormatDateTime(fmt, dt);
            return dt.ToString("R");
        }

        public virtual string DecodeCacheControl(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            string ret = null;

            int o = d.UInt8;
            if (o <= 31)
            {
                // value-length cache-directive
                Logger.LogThis("decoding cache-control (value-length cache-directive) not yet implemented", LogLevel.Warn);
            }
            else if ((o & 0x80) != 0)
            {
                ret = TransTable.GetTable(TtCControl).Code2Str(o);
            }
            else
            {
                // Token-Text
                ret = d.CString;
            }
            return ret;
        }

        public virtual string DecodeLocation(byte[] data)
        {
            WSPDecoder d = new WSPDecoder(data);
            return d.TextString;
        }      
    }
}
