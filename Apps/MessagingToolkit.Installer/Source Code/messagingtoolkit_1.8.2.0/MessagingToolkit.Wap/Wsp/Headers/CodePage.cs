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
using System.Reflection;
using System.IO;

namespace MessagingToolkit.Wap.Wsp.Headers
{

    /// <summary>
    /// This class represents a Header Codepage.
    /// </summary>
    public abstract class CodePage
    {
        private static readonly Type[] EncSig = new Type[] { typeof(Stream), Type.GetType("System.Int16"), typeof(string) };
        private static readonly Type[] DecSig = new Type[] { typeof(byte[]) };

        private int pageCode;
        private string pageName;
        private bool shortCut;

        virtual public int PageCode
        {
            get
            {
                return pageCode;
            }

        }
        virtual public string PageName
        {
            get
            {
                return pageName;
            }

        }
        virtual public bool ShortCut
        {
            get
            {
                return shortCut;
            }

        }
        virtual public byte[] Bytes
        {
            /*
            * Get the Shift sequence for this code page
            */
            get
            {
                byte[] b = null;

                if (ShortCut)
                {
                    b = new byte[1];
                    b[0] = (byte)PageCode;
                }
                else
                {
                    b = new byte[2];
                    b[0] = 127;
                    b[1] = (byte)PageCode;
                }
                return b;
            }
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="CodePage"/> class.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="shortCut">if set to <c>true</c> [short cut].</param>
        /// <param name="name">The name.</param>
        protected internal CodePage(int code, bool shortCut, string name)
        {
            if ((code < 1) || (code > 255))
            {
                throw new ArgumentException(code + ": Illegal page code.");
            }

            if (shortCut && (code > 31))
            {
                throw new ArgumentException(code + ": Illegal shortcut page code.");
            }

            this.shortCut = shortCut;
            pageCode = code;
            pageName = name;
        }

        /// <summary>
        /// Encode a header.
        /// </summary>
        /// <param name="key">the header name</param>
        /// <param name="value"></param>
        /// <returns>the header encoded as byte array</returns>
        /// <throws>  HeaderParseException if the header cannot be encoded </throws>
        public abstract byte[] Encode(string key, string value);

        /// <summary> Encode a date header.</summary>
        /// <param name="key">the header name
        /// </param>
        /// <param name="value">the value
        /// </param>
        /// <returns> the header encoded as byte array
        /// </returns>
        /// <throws>  HeaderParseException if the header cannot be encoded </throws>
         public abstract byte[] Encode(string key, ref DateTime value);

        /// <summary>
        /// Encode a long header.
        /// </summary>
        /// <param name="key">the header name</param>
        /// <param name="value_Renamed"></param>
        /// <returns>the header encoded as byte array</returns>
        /// <throws>  HeaderParseException if the header cannot be encoded </throws>
        public abstract byte[] Encode(string key, long value);

        /// <summary> Convert (decode) a byte array containing a Header.</summary>
        /// <param name="data">the data to decode
        /// </param>
        /// <returns> a Header object
        /// </returns>
        /// <throws>  HeaderParseException if the data cannot be decoded </throws>
        public abstract Header Decode(byte[] data);

        /// <summary>
        /// Encodes the header.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="output">The output.</param>
        /// <param name="wk">The wk.</param>
        /// <param name="value">The value.</param>
        protected internal virtual void EncodeHeader(string header, Stream output, short wk, string value)
        {
            // Method must be named "handleHeaderName()
            StringBuilder mname = new StringBuilder("Encode");
            CamelCase(mname, header);

            MethodInfo m = GetType().GetMethod(mname.ToString(), (EncSig == null) ? new Type[0] : (Type[])EncSig);
            Object[] args = new Object[] { output, (short)wk, value };

            try
            {
                if (m != null)
                    m.Invoke(this, (Object[])args);
            }
            catch (TargetInvocationException ite)
            {
                Exception t = ite.GetBaseException();

                if (t is Exception)
                {
                    throw (Exception)t;
                }
                else if (t is IOException)
                {
                    throw (IOException)t;
                }
                else
                {
                    throw ite;
                }
            }
        }

        /// <summary>
        /// Decodes the header field.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected internal virtual string DecodeHeaderField(string header, byte[] value)
        {
            StringBuilder mname = new StringBuilder("Decode");
            CamelCase(mname, header);
            MethodInfo m = GetType().GetMethod(mname.ToString(), (DecSig == null) ? new System.Type[0] : (System.Type[])DecSig);
            object[] args = new object[] { value };
            if (m != null)
            {
                return (string)m.Invoke(this, (object[])args);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Camels the case.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="name">The name.</param>
        private void CamelCase(StringBuilder sb, string name)
        {
            Tokenizer strtok = new Tokenizer(name, "-");

            while (strtok.HasMoreTokens())
            {
                string p = strtok.NextToken().ToLower();
                char fc = p[0];
                sb.Append(System.Char.ToUpper(fc));

                if (p.Length > 1)
                {
                    sb.Append(p.Substring(1));
                }
            }
        }
    }
}
