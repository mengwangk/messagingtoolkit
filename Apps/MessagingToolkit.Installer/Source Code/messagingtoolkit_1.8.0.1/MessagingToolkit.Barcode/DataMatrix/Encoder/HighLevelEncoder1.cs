using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    /// <summary>
    /// DataMatrix ECC 200 data encoder following the algorithm described in ISO/IEC 16022:200(E) in
    /// annex S.
    /// </summary>
    public class HighLevelEncoder : Constants
    {

        private const int ASCII_ENCODATION = 0;
        private const int C40_ENCODATION = 1;
        private const int TEXT_ENCODATION = 2;
        private const int X12_ENCODATION = 3;
        private const int EDIFACT_ENCODATION = 4;
        private const int BASE256_ENCODATION = 5;

#if (WINDOWS_PHONE || SILVERLIGHT || NETFX_CORE)
        public const string DefaultEncoding = "UTF-8";
#else
        public const string DefaultEncoding = "ISO-8859-1";
#endif


        private const string URL_START = "url(";
        private const string URL_END = ")";

        /// <summary>
        /// Converts the message to a byte array using the default encoding (cp437) as defined by the
        /// specification
        /// </summary>
        /// <param name="msg">the message</param>
        /// <returns>
        /// the byte array of the message
        /// </returns>
        public static byte[] GetBytesForMessage(string msg)
        {
            const string charset = "cp437"; //See 4.4.3 and annex B of ISO/IEC 15438:2001(E)
            try
            {
                return Encoding.GetEncoding(charset).GetBytes(msg);
            }
            catch (ArgumentException e)
            {
                throw new NotSupportedException("Incompatible environment. The '" + charset + "' charset is not available!");
            }
        }

        private static char Randomize253State(char ch, int codewordPosition)
        {
            int pseudoRandom = ((149 * codewordPosition) % 253) + 1;
            int tempVariable = ch + pseudoRandom;
            if (tempVariable <= 254)
            {
                return (char)tempVariable;
            }
            else
            {
                return (char)(tempVariable - 254);
            }
        }

        private static char Randomize255State(char ch, int codewordPosition)
        {
            int pseudoRandom = ((149 * codewordPosition) % 255) + 1;
            int tempVariable = ch + pseudoRandom;
            if (tempVariable <= 255)
            {
                return (char)tempVariable;
            }
            else
            {
                return (char)(tempVariable - 256);
            }
        }

        /// <summary>
        /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
        /// of ISO/IEC 16022:2000(E). </summary>
        /// <param name="msg"> the message </param>
        /// <returns> the encoded message (the char values range from 0 to 255) </returns>
        public static string EncodeHighLevel(string msg)
        {
            return EncodeHighLevel(msg, SymbolShapeHint.ForceNone, null, null, null);
        }

        /// <summary>
        /// Performs message encoding of a DataMatrix message using the algorithm described in annex P
        /// of ISO/IEC 16022:2000(E).
        /// </summary>
        /// <param name="msg">the message</param>
        /// <param name="shape">requested shape. May be <code>SymbolShapeHint.FORCE_NONE</code>,
        /// <code>SymbolShapeHint.FORCE_SQUARE</code> or <code>SymbolShapeHint.FORCE_RECTANGLE</code>.</param>
        /// <param name="minSize">the minimum symbol size constraint or null for no constraint</param>
        /// <param name="maxSize">the maximum symbol size constraint or null for no constraint</param>
        /// <returns>
        /// the encoded message (the char values range from 0 to 255)
        /// </returns>
        public static string EncodeHighLevel(string msg, SymbolShapeHint shape, Dimensions minSize, Dimensions maxSize, Dictionary<EncodeOptions, object> encodingOptions)
        {
            //the codewords 0..255 are encoded as Unicode characters
            Encoder[] encoders = new Encoder[] { new ASCIIEncoder(), new C40Encoder(), new TextEncoder(), new X12Encoder(), new EdifactEncoder(), new Base256Encoder() };

            int encodingMode = ASCII_ENCODATION; //Default mode
            EncoderContext context = CreateEncoderContext(msg);
            context.SymbolShape = shape;
            context.SetSizeConstraints(minSize, maxSize);

            if (msg.StartsWith(MACRO_05_HEADER) && msg.EndsWith(MACRO_TRAILER))
            {
                context.WriteCodeword(MACRO_05);
                context.SkipAtEnd = 2;
                context.pos += MACRO_05_HEADER.Length;
            }
            else if (msg.StartsWith(MACRO_06_HEADER) && msg.EndsWith(MACRO_TRAILER))
            {
                context.WriteCodeword(MACRO_06);
                context.SkipAtEnd = 2;
                context.pos += MACRO_06_HEADER.Length;
            }

            while (context.HasMoreCharacters())
            {
                encoders[encodingMode].Encode(context);
                if (context.newEncoding >= 0)
                {
                    encodingMode = context.newEncoding;
                    context.ResetEncoderSignal();
                }
            }

            int len = context.codewords.Length;
            context.UpdateSymbolInfo();
            int capacity = context.symbolInfo.dataCapacity;
            if (len < capacity)
            {
                if (encodingMode != ASCII_ENCODATION && encodingMode != BASE256_ENCODATION)
                {
                    context.WriteCodeword('\u00fe'); //Unlatch (254)
                }
            }
            //Padding
            StringBuilder codewords = context.codewords;
            if (codewords.Length < capacity)
            {
                codewords.Append(Constants.PAD);
            }
            while (codewords.Length < capacity)
            {
                codewords.Append(Randomize253State(Constants.PAD, codewords.Length + 1));
            }

            return context.codewords.ToString();
        }

        private static EncoderContext CreateEncoderContext(string msg)
        {
            if (msg.StartsWith(URL_START) && msg.EndsWith(URL_END))
            {
                // URL processing
                string url = msg.Substring(URL_START.Length, msg.Length - URL_END.Length - (URL_START.Length));
                byte[] data;
                data = GetData(url, DefaultEncoding);
                return new EncoderContext(data);
            }
            else
            {
                return new EncoderContext(msg);
            }
        }

        private const string DataProtocol = "data:";

        /// <summary>
        /// Returns the data pointed at by a URL as a byte array. </summary>
        /// <param name="url"> the URL </param>
        /// <param name="encoding"> the encoding to use for converting text content to binary content </param>
        /// <returns> the data as a byte array </returns>
        public static byte[] GetData(string url, string encoding)
        {
            if (url.StartsWith(DataProtocol))
            {
                return ParseDataURL(url, encoding);
            }
            else
            {
#if (!SILVERLIGHT && !NETFX_CORE)

                WebClient webClient = new WebClient();
                return webClient.DownloadData(url);
#else
                return ParseDataURL(url, encoding);
#endif
            }
        }

        private static byte[] ParseDataURL(string url, string encoding)
        {
            int commaPos = url.IndexOf(',');
            // header is of the form data:[<mediatype>][;base64]
            string header = url.Substring(0, commaPos);
            string data = url.Substring(commaPos + 1);
            if (header.EndsWith(";base64"))
            {
                Base64InputStream input = new Base64InputStream(new StringReader(data));
                using (MemoryStream ms = new MemoryStream())
                {
                    int buf;
                    while ((buf = input.Read()) != -1)
                    {
                        ms.WriteByte((byte)buf);
                    }

                    try
                    {
                        input.Close();
                    }
                    catch { }

                    return ms.ToArray();
                }
            }
            else
            {
                string urlEncoding = "US-ASCII";
                int charsetpos = header.IndexOf(";charset=");
                if (charsetpos > 0)
                {
                    urlEncoding = header.Substring(charsetpos + 9);
                    int pos = urlEncoding.IndexOf(';');
                    if (pos > 0)
                    {
                        urlEncoding = urlEncoding.Substring(0, pos);
                    }
                }
                string unescapedString = Uri.UnescapeDataString(data);
                byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(unescapedString);
                return bytes;
            }
        }


        private class EncoderContext
        {

            internal string msg;
            internal SymbolShapeHint shape = SymbolShapeHint.ForceNone;
            internal Dimensions minSize;
            internal Dimensions maxSize;
            internal StringBuilder codewords;
            internal int pos = 0;
            internal int newEncoding = -1;
            internal SymbolInfo symbolInfo;
            internal int skipAtEnd = 0;

            public EncoderContext(string msg)
            {
                // From this point on Strings are not Unicode anymore!
                byte[] msgBinary;
                try
                {
                    msgBinary = Encoding.GetEncoding(DefaultEncoding).GetBytes(msg);
                }
                catch (Exception e)
                {
                    throw new System.NotSupportedException("Unsupported encoding: " + e.Message);
                }
                StringBuilder sb = new StringBuilder(msgBinary.Length);
                for (int i = 0, c = msgBinary.Length; i < c; i++)
                {
                    char ch = (char)(msgBinary[i] & 0xff);
                    if (ch == '?' && msg[i] != '?')
                    {
                        throw new ArgumentException("Message contains characters outside " + DefaultEncoding + " encoding.");
                    }
                    sb.Append(ch);
                }
                this.msg = sb.ToString(); //Not Unicode here!
                this.codewords = new StringBuilder(msg.Length);
            }

            public EncoderContext(byte[] data)
            {
                //From this point on Strings are not Unicode anymore!
                StringBuilder sb = new StringBuilder(data.Length);
                for (int i = 0, c = data.Length; i < c; i++)
                {
                    char ch = (char)(data[i] & 0xff);
                    sb.Append(ch);
                }
                this.msg = sb.ToString(); //Not Unicode here!
                this.codewords = new StringBuilder(msg.Length);
            }

            public virtual SymbolShapeHint SymbolShape
            {
                set
                {
                    this.shape = value;
                }
            }

            public virtual void SetSizeConstraints(Dimensions minSize, Dimensions maxSize)
            {
                this.minSize = minSize;
                this.maxSize = maxSize;
            }

            public virtual string Message
            {
                get
                {
                    return this.msg;
                }
            }

            public virtual int SkipAtEnd
            {
                set
                {
                    this.skipAtEnd = value;
                }
            }

            public virtual char CurrentChar
            {
                get
                {
                    return msg[pos];
                }
            }

            public virtual char Current
            {
                get
                {
                    return msg[pos];
                }
            }

            public virtual void WriteCodewords(string codewords)
            {
                this.codewords.Append(codewords);
            }

            public virtual void WriteCodeword(char codeword)
            {
                this.codewords.Append(codeword);
            }

            public virtual int CodewordCount
            {
                get
                {
                    return this.codewords.Length;
                }
            }

            public virtual void SignalEncoderChange(int encoding)
            {
                this.newEncoding = encoding;
            }

            public virtual void ResetEncoderSignal()
            {
                this.newEncoding = -1;
            }

            public virtual bool HasMoreCharacters()
            {
                return pos < TotalMessageCharCount;
            }

            private int TotalMessageCharCount
            {
                get
                {
                    return msg.Length - skipAtEnd;
                }
            }

            public virtual int RemainingCharacters
            {
                get
                {
                    return TotalMessageCharCount - pos;
                }
            }

            public virtual void UpdateSymbolInfo()
            {
                UpdateSymbolInfo(CodewordCount);
            }

            public virtual void UpdateSymbolInfo(int len)
            {
                if (this.symbolInfo == null || len > this.symbolInfo.dataCapacity)
                {
                    this.symbolInfo = SymbolInfo.Lookup(len, shape, minSize, maxSize, true);
                }
            }

            public virtual void ResetSymbolInfo()
            {
                this.symbolInfo = null;
            }
        }

        private interface Encoder
        {
            int EncodingMode { get; }
            void Encode(EncoderContext context);
        }

        private class ASCIIEncoder : Encoder
        {

            public virtual int EncodingMode
            {
                get
                {
                    return ASCII_ENCODATION;
                }
            }

            public virtual void Encode(EncoderContext context)
            {
                //step B
                int n = DetermineConsecutiveDigitCount(context.msg, context.pos);
                if (n >= 2)
                {
                    context.WriteCodeword(EncodeASCIIDigits(context.msg[context.pos], context.msg[context.pos + 1]));
                    context.pos += 2;
                }
                else
                {
                    char c = context.CurrentChar;
                    int newMode = LookAheadTest(context.msg, context.pos, EncodingMode);
                    if (newMode != EncodingMode)
                    {
                        switch (newMode)
                        {
                            case BASE256_ENCODATION:
                                context.WriteCodeword(LATCH_TO_BASE256);
                                context.SignalEncoderChange(BASE256_ENCODATION);
                                return;
                            case C40_ENCODATION:
                                context.WriteCodeword(LATCH_TO_C40);
                                context.SignalEncoderChange(C40_ENCODATION);
                                return;
                            case X12_ENCODATION:
                                context.WriteCodeword(LATCH_TO_ANSIX12);
                                context.SignalEncoderChange(X12_ENCODATION);
                                break;
                            case TEXT_ENCODATION:
                                context.WriteCodeword(LATCH_TO_TEXT);
                                context.SignalEncoderChange(TEXT_ENCODATION);
                                break;
                            case EDIFACT_ENCODATION:
                                context.WriteCodeword(LATCH_TO_EDIFACT);
                                context.SignalEncoderChange(EDIFACT_ENCODATION);
                                break;
                            default:
                                throw new ArgumentException("Illegal mode: " + newMode);
                        }
                    }
                    else if (IsExtendedASCII(c))
                    {
                        context.WriteCodeword(UPPER_SHIFT);
                        context.WriteCodeword((char)(c - 128 + 1));
                        context.pos++;
                    }
                    else
                    {
                        context.WriteCodeword((char)(c + 1));
                        context.pos++;
                    }

                }
            }

        }

        private class C40Encoder : Encoder
        {

            public virtual int EncodingMode
            {
                get
                {
                    return C40_ENCODATION;
                }
            }

            public virtual void Encode(EncoderContext context)
            {
                //step C
                int lastCharSize = -1;
                StringBuilder buffer = new StringBuilder();
            //outerloop:
                while (context.HasMoreCharacters())
                {
                    char c = context.CurrentChar;
                    context.pos++;

                    lastCharSize = EncodeChar(c, buffer);

                    int unwritten = (buffer.Length / 3) * 2;

                    int curCodewordCount = context.CodewordCount + unwritten;
                    context.UpdateSymbolInfo(curCodewordCount);
                    int available = context.symbolInfo.dataCapacity - curCodewordCount;

                    if (!context.HasMoreCharacters())
                    {
                        //Avoid having a single C40 value in the last triplet
                        StringBuilder removed = new StringBuilder();
                        if ((buffer.Length % 3) == 2)
                        {
                            if (available < 2 || available > 2)
                            {
                                lastCharSize = BacktrackOneCharacter(context, buffer, removed, lastCharSize);
                            }
                        }
                        while ((buffer.Length % 3) == 1 && ((lastCharSize <= 3 && available != 1) || lastCharSize > 3))
                        {
                            lastCharSize = BacktrackOneCharacter(context, buffer, removed, lastCharSize);
                        }
                        break;
                    }

                    int count = buffer.Length;
                    if ((count % 3) == 0)
                    {
                        int newMode = LookAheadTest(context.msg, context.pos, EncodingMode);
                        if (newMode != EncodingMode)
                        {
                            context.SignalEncoderChange(newMode);
                            break;
                        }
                    }
                }
                HandleEOD(context, buffer, lastCharSize);
            }

            private int BacktrackOneCharacter(EncoderContext context, StringBuilder buffer, StringBuilder removed, int lastCharSize)
            {
                int count = buffer.Length;
                //buffer.Remove(count - lastCharSize, count);
                buffer.Remove(count - lastCharSize, lastCharSize);
                context.pos--;
                char c = context.CurrentChar;
                lastCharSize = EncodeChar(c, removed);
                context.ResetSymbolInfo(); //Deal with possible reduction in symbol size
                return lastCharSize;
            }

            protected internal virtual void WriteNextTriplet(EncoderContext context, StringBuilder buffer)
            {
                context.WriteCodewords(EncodeToCodewords(buffer, 0));
                buffer.Remove(0, 3);
            }

            /// <summary>
            /// Handle "end of data" situations </summary>
            /// <param name="context"> the encoder context </param>
            /// <param name="buffer"> the buffer with the remaining encoded characters </param>
            protected internal virtual void HandleEOD(EncoderContext context, StringBuilder buffer, int lastCharSize)
            {
                int unwritten = (buffer.Length / 3) * 2;
                int rest = buffer.Length % 3;

                int curCodewordCount = context.CodewordCount + unwritten;
                context.UpdateSymbolInfo(curCodewordCount);
                int available = context.symbolInfo.dataCapacity - curCodewordCount;

                if (rest == 2)
                {
                    buffer.Append('\0'); //Shift 1
                    while (buffer.Length >= 3)
                    {
                        WriteNextTriplet(context, buffer);
                    }
                    if (context.HasMoreCharacters())
                    {
                        context.WriteCodeword(C40_UNLATCH);
                    }
                }
                else if (available == 1 && rest == 1)
                {
                    while (buffer.Length >= 3)
                    {
                        WriteNextTriplet(context, buffer);
                    }
                    if (context.HasMoreCharacters())
                    {
                        context.WriteCodeword(C40_UNLATCH);
                    }
                    else
                    {
                        //No unlatch
                    }
                    context.pos--;
                }
                else if (rest == 0)
                {
                    while (buffer.Length >= 3)
                    {
                        WriteNextTriplet(context, buffer);
                    }
                    if (available > 0 || context.HasMoreCharacters())
                    {
                        context.WriteCodeword(C40_UNLATCH);
                    }
                }
                else
                {
                    throw new ArgumentException("Unexpected case. Please report!");
                }
                context.SignalEncoderChange(ASCII_ENCODATION);
            }

            protected internal virtual int EncodeChar(char c, StringBuilder sb)
            {
                if (c == ' ')
                {
                    sb.Append((char)3);
                    return 1;
                }
                else if (c >= '0' && c <= '9')
                {
                    sb.Append((char)(c - 48 + 4));
                    return 1;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    sb.Append((char)(c - 65 + 14));
                    return 1;
                }
                else if (c >= '\0' && c <= '\u001f')
                {
                    sb.Append('\0'); //Shift 1 Set
                    sb.Append(c);
                    return 2;
                }
                else if (c >= '!' && c <= '/')
                {
                    sb.Append((char)1); //Shift 2 Set
                    sb.Append((char)(c - 33));
                    return 2;
                }
                else if (c >= ':' && c <= '@')
                {
                    sb.Append((char)1); //Shift 2 Set
                    sb.Append((char)(c - 58 + 15));
                    return 2;
                }
                else if (c >= '[' && c <= '_')
                {
                    sb.Append((char)1); //Shift 2 Set
                    sb.Append((char)(c - 91 + 22));
                    return 2;
                }
                else if (c >= '\u0060' && c <= '\u007f')
                {
                    sb.Append((char)2); //Shift 3 Set
                    sb.Append((char)(c - 96));
                    return 2;
                }
                else if (c >= '\u0080')
                {
                    sb.Append((char)1);
                    sb.Append('\u001e'); //Shift 2, Upper Shift
                    int len = 2;
                    len += EncodeChar((char)(c - 128), sb);
                    return len;
                }
                else
                {
                    throw new ArgumentException("Illegal character: " + c);
                }
            }

            protected internal virtual string EncodeToCodewords(StringBuilder sb, int startPos)
            {
                char c1 = sb[startPos];
                char c2 = sb[startPos + 1];
                char c3 = sb[startPos + 2];
                int v = (1600 * c1) + (40 * c2) + c3 + 1;
                char cw1 = (char)(v / 256);
                char cw2 = (char)(v % 256);
                return "" + cw1 + cw2;
            }

        }

        private class TextEncoder : C40Encoder
        {

            public override int EncodingMode
            {
                get
                {
                    return TEXT_ENCODATION;
                }
            }

            protected internal override int EncodeChar(char c, StringBuilder sb)
            {
                if (c == ' ')
                {
                    sb.Append((char)3);
                    return 1;
                }
                else if (c >= '0' && c <= '9')
                {
                    sb.Append((char)(c - 48 + 4));
                    return 1;
                }
                else if (c >= 'a' && c <= 'z')
                {
                    sb.Append((char)(c - 97 + 14));
                    return 1;
                }
                else if (c >= '\0' && c <= '\u001f')
                {
                    sb.Append('\0'); //Shift 1 Set
                    sb.Append(c);
                    return 2;
                }
                else if (c >= '!' && c <= '/')
                {
                    sb.Append((char)1); //Shift 2 Set
                    sb.Append((char)(c - 33));
                    return 2;
                }
                else if (c >= ':' && c <= '@')
                {
                    sb.Append((char)1); //Shift 2 Set
                    sb.Append((char)(c - 58 + 15));
                    return 2;
                }
                else if (c >= '[' && c <= '_')
                {
                    sb.Append((char)1); //Shift 2 Set
                    sb.Append((char)(c - 91 + 22));
                    return 2;
                }
                else if (c == '\u0060')
                {
                    sb.Append((char)2); //Shift 3 Set
                    sb.Append((char)(c - 96));
                    return 2;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    sb.Append((char)2); //Shift 3 Set
                    sb.Append((char)(c - 65 + 1));
                    return 2;
                }
                else if (c >= '{' && c <= '\u007f')
                {
                    sb.Append((char)2); //Shift 3 Set
                    sb.Append((char)(c - 123 + 27));
                    return 2;
                }
                else if (c >= '\u0080')
                {
                    sb.Append((char)1); //Shift 2, Upper Shift
                    sb.Append((char)'\u001e'); //Shift 2, Upper Shift
                    int len = 2;
                    len += EncodeChar((char)(c - 128), sb);
                    return len;
                }
                else
                {
                    IllegalCharacter(c);
                    return -1;
                }
            }

        }

        private class X12Encoder : C40Encoder
        {

            public override int EncodingMode
            {
                get
                {
                    return X12_ENCODATION;
                }
            }

            public override void Encode(EncoderContext context)
            {
                //step C
                StringBuilder buffer = new StringBuilder();
                while (context.HasMoreCharacters())
                {
                    char c = context.CurrentChar;
                    context.pos++;

                    EncodeChar(c, buffer);

                    int count = buffer.Length;
                    if ((count % 3) == 0)
                    {
                        WriteNextTriplet(context, buffer);

                        int newMode = LookAheadTest(context.msg, context.pos, EncodingMode);
                        if (newMode != EncodingMode)
                        {
                            context.SignalEncoderChange(newMode);
                            break;
                        }
                    }
                }
                HandleEOD(context, buffer);
            }

            protected internal override int EncodeChar(char c, StringBuilder sb)
            {
                if (c == '\r')
                {
                    sb.Append('\0');
                }
                else if (c == '*')
                {
                    sb.Append((char)1);
                }
                else if (c == '>')
                {
                    sb.Append((char)2);
                }
                else if (c == ' ')
                {
                    sb.Append((char)3);
                }
                else if (c >= '0' && c <= '9')
                {
                    sb.Append((char)(c - 48 + 4));
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    sb.Append((char)(c - 65 + 14));
                }
                else
                {
                    IllegalCharacter(c);
                }
                return 1;
            }

            protected internal virtual void HandleEOD(EncoderContext context, StringBuilder buffer)
            {
                context.UpdateSymbolInfo();
                int available = context.symbolInfo.dataCapacity - context.CodewordCount;
                int count = buffer.Length;
                if (count == 2)
                {
                    context.WriteCodeword(X12_UNLATCH);
                    context.pos -= 2;
                    context.SignalEncoderChange(ASCII_ENCODATION);
                }
                else if (count == 1)
                {
                    context.pos--;
                    if (available > 1)
                    {
                        context.WriteCodeword(X12_UNLATCH);
                    }
                    else
                    {
                        //NOP - No unlatch necessary
                    }
                    context.SignalEncoderChange(ASCII_ENCODATION);
                }
            }
        }

        private class EdifactEncoder : Encoder
        {

            public virtual int EncodingMode
            {
                get
                {
                    return EDIFACT_ENCODATION;
                }
            }

            public virtual void Encode(EncoderContext context)
            {
                //step F
                StringBuilder buffer = new StringBuilder();
                while (context.HasMoreCharacters())
                {
                    char c = context.CurrentChar;
                    EncodeChar(c, buffer);
                    context.pos++;

                    int count = buffer.Length;
                    if (count >= 4)
                    {
                        context.WriteCodewords(EncodeToCodewords(buffer, 0));
                        buffer.Remove(0, 4);

                        int newMode = LookAheadTest(context.msg, context.pos, EncodingMode);
                        if (newMode != EncodingMode)
                        {
                            context.SignalEncoderChange(ASCII_ENCODATION);
                            break;
                        }
                    }
                }
                buffer.Append((char)31); //Unlatch
                HandleEOD(context, buffer);
            }

            /// <summary>
            /// Handle "end of data" situations </summary>
            /// <param name="context"> the encoder context </param>
            /// <param name="buffer"> the buffer with the remaining encoded characters </param>
            protected internal virtual void HandleEOD(EncoderContext context, StringBuilder buffer)
            {
                try
                {
                    int count = buffer.Length;
                    if (count == 0)
                    {
                        return; //Already finished
                    }
                    else if (count == 1)
                    {
                        //Only an unlatch at the end
                        context.UpdateSymbolInfo();
                        int avail = context.symbolInfo.dataCapacity - context.CodewordCount;
                        int remaining = context.RemainingCharacters;
                        if (remaining == 0 && avail <= 2)
                        {
                            return; //No unlatch
                        }
                    }

                    if (count > 4)
                    {
                        throw new ArgumentException("Count must not exceed 4");
                    }
                    int restChars = count - 1;
                    string encoded = EncodeToCodewords(buffer, 0);
                    bool endOfSymbolReached = !context.HasMoreCharacters();
                    bool restInAscii = endOfSymbolReached && restChars <= 2;

                    int available;
                    if (restChars <= 2)
                    {
                        context.UpdateSymbolInfo(context.CodewordCount + restChars);
                        available = context.symbolInfo.dataCapacity - context.CodewordCount;
                        if (available >= 3)
                        {
                            restInAscii = false;
                            context.UpdateSymbolInfo(context.CodewordCount + encoded.Length);
                            available = context.symbolInfo.dataCapacity - context.CodewordCount;
                        }
                    }

                    if (restInAscii)
                    {
                        context.ResetSymbolInfo();
                        context.pos -= restChars;
                    }
                    else
                    {
                        context.WriteCodewords(encoded);
                    }
                }
                finally
                {
                    context.SignalEncoderChange(ASCII_ENCODATION);
                }
            }

            protected internal virtual void EncodeChar(char c, StringBuilder sb)
            {
                if (c >= ' ' && c <= '?')
                {
                    sb.Append(c);
                }
                else if (c >= '@' && c <= '^')
                {
                    sb.Append((char)(c - 64));
                }
                else
                {
                    IllegalCharacter(c);
                }
            }

            protected internal virtual string EncodeToCodewords(StringBuilder sb, int startPos)
            {
                int len = sb.Length - startPos;
                if (len == 0)
                {
                    throw new ArgumentException("StringBuffer must not be empty");
                }
                char c1 = sb[startPos];
                char c2 = (char)(len >= 2 ? sb[startPos + 1] : 0);
                char c3 = (char)(len >= 3 ? sb[startPos + 2] : 0);
                char c4 = (char)(len >= 4 ? sb[startPos + 3] : 0);

                int v = (c1 << 18) + (c2 << 12) + (c3 << 6) + c4;
                char cw1 = (char)((v >> 16) & 255);
                char cw2 = (char)((v >> 8) & 255);
                char cw3 = (char)(v & 255);
                StringBuilder res = new StringBuilder(3);
                res.Append(cw1);
                if (len >= 2)
                {
                    res.Append(cw2);
                }
                if (len >= 3)
                {
                    res.Append(cw3);
                }
                return res.ToString();
            }

        }

        private class Base256Encoder : Encoder
        {

            public virtual int EncodingMode
            {
                get
                {
                    return BASE256_ENCODATION;
                }
            }

            public virtual void Encode(EncoderContext context)
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append('\0'); //Initialize length field
                while (context.HasMoreCharacters())
                {
                    char c = context.CurrentChar;
                    buffer.Append(c);

                    context.pos++;

                    int newMode = LookAheadTest(context.msg, context.pos, EncodingMode);
                    if (newMode != EncodingMode)
                    {
                        context.SignalEncoderChange(newMode);
                        break;
                    }
                }
                int dataCount = buffer.Length - 1;
                int lengthFieldSize = 1;
                int currentSize = (context.CodewordCount + dataCount + lengthFieldSize);
                context.UpdateSymbolInfo(currentSize);
                bool mustPad = ((context.symbolInfo.dataCapacity - currentSize) > 0);
                if (context.HasMoreCharacters() || mustPad)
                {
                    if (dataCount <= 249)
                    {
                        buffer[0] = (char)dataCount;
                    }
                    else if (dataCount > 249 && dataCount <= 1555)
                    {
                        buffer[0] = (char)((dataCount / 250) + 249);
                        buffer.Insert(1, Convert.ToString(dataCount % 250));
                    }
                    else
                    {
                        throw new ArgumentException("Message length not in valid ranges: " + dataCount);
                    }
                }
                for (int i = 0, c = buffer.Length; i < c; i++)
                {
                    context.WriteCodeword(Randomize255State(buffer[i], context.CodewordCount + 1));
                }
            }

        }

        private static char EncodeASCIIDigits(char digit1, char digit2)
        {
            if (IsDigit(digit1) && IsDigit(digit2))
            {
                int num = (digit1 - 48) * 10 + (digit2 - 48);
                return (char)(num + 130);
            }
            else
            {
                throw new ArgumentException("not digits: " + digit1 + digit2);
            }
        }

        private static int LookAheadTest(string msg, int startpos, int currentMode)
        {
            if (startpos >= msg.Length)
            {
                return currentMode;
            }
            float[] charCounts;
            //step 
            if (currentMode == ASCII_ENCODATION)
            {
                charCounts = new float[] { 0, 1, 1, 1, 1, 1.25f };
            }
            else
            {
                charCounts = new float[] { 1, 2, 2, 2, 2, 2.25f };
                charCounts[currentMode] = 0;
            }

            int charsProcessed = 0;
            while (true)
            {
                //step K
                if ((startpos + charsProcessed) == msg.Length)
                {
                    int min = int.MaxValue;
                    sbyte[] mins = new sbyte[6];
                    int[] intCharCounts = new int[6];
                    min = FindMinimums(charCounts, intCharCounts, min, mins);
                    int minCount = GetMinimumCount(mins);

                    if (intCharCounts[ASCII_ENCODATION] == min)
                    {
                        return ASCII_ENCODATION;
                    }
                    else if (minCount == 1 && mins[BASE256_ENCODATION] > 0)
                    {
                        return BASE256_ENCODATION;
                    }
                    else if (minCount == 1 && mins[EDIFACT_ENCODATION] > 0)
                    {
                        return EDIFACT_ENCODATION;
                    }
                    else if (minCount == 1 && mins[TEXT_ENCODATION] > 0)
                    {
                        return TEXT_ENCODATION;
                    }
                    else if (minCount == 1 && mins[X12_ENCODATION] > 0)
                    {
                        return X12_ENCODATION;
                    }
                    else
                    {
                        return C40_ENCODATION;
                    }
                }

                char c = msg[startpos + charsProcessed];
                charsProcessed++;

                //step L
                if (IsDigit(c))
                {
                    charCounts[ASCII_ENCODATION] += 0.5f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[ASCII_ENCODATION] = (int)Math.Ceiling(charCounts[ASCII_ENCODATION]);
                    charCounts[ASCII_ENCODATION] += 2;
                }
                else
                {
                    charCounts[ASCII_ENCODATION] = (int)Math.Ceiling(charCounts[ASCII_ENCODATION]);
                    charCounts[ASCII_ENCODATION] += 1;
                }

                //step M
                if (IsNativeC40(c))
                {
                    charCounts[C40_ENCODATION] += 2f / 3f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[C40_ENCODATION] += 8f / 3f;
                }
                else
                {
                    charCounts[C40_ENCODATION] += 4f / 3f;
                }

                //step N
                if (IsNativeText(c))
                {
                    charCounts[TEXT_ENCODATION] += 2f / 3f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[TEXT_ENCODATION] += 8f / 3f;
                }
                else
                {
                    charCounts[TEXT_ENCODATION] += 4f / 3f;
                }

                //step O
                if (IsNativeX12(c))
                {
                    charCounts[X12_ENCODATION] += 2f / 3f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[X12_ENCODATION] += 13f / 3f;
                }
                else
                {
                    charCounts[X12_ENCODATION] += 10f / 3f;
                }

                //step P
                if (IsNativeEDIFACT(c))
                {
                    charCounts[EDIFACT_ENCODATION] += 3f / 4f;
                }
                else if (IsExtendedASCII(c))
                {
                    charCounts[EDIFACT_ENCODATION] += 17f / 4f;
                }
                else
                {
                    charCounts[EDIFACT_ENCODATION] += 13f / 4f;
                }

                // step Q
                if (IsSpecialB256(c))
                {
                    charCounts[BASE256_ENCODATION] += 4;
                }
                else
                {
                    charCounts[BASE256_ENCODATION] += 1;
                }

                //step R
                if (charsProcessed >= 4)
                {
                    int min = int.MaxValue;
                    int[] intCharCounts = new int[6];
                    sbyte[] mins = new sbyte[6];
                    min = FindMinimums(charCounts, intCharCounts, min, mins);
                    int minCount = GetMinimumCount(mins);

                    if (intCharCounts[ASCII_ENCODATION] + 1 <= intCharCounts[BASE256_ENCODATION] && intCharCounts[ASCII_ENCODATION] + 1 <= intCharCounts[C40_ENCODATION] && intCharCounts[ASCII_ENCODATION] + 1 <= intCharCounts[TEXT_ENCODATION] && intCharCounts[ASCII_ENCODATION] + 1 <= intCharCounts[X12_ENCODATION] && intCharCounts[ASCII_ENCODATION] + 1 <= intCharCounts[EDIFACT_ENCODATION])
                    {
                        return ASCII_ENCODATION;
                    }
                    else if (intCharCounts[BASE256_ENCODATION] + 1 <= intCharCounts[ASCII_ENCODATION] || (mins[C40_ENCODATION] + mins[TEXT_ENCODATION] + mins[X12_ENCODATION] + mins[EDIFACT_ENCODATION]) == 0)
                    {
                        return BASE256_ENCODATION;
                    }
                    else if (minCount == 1 && mins[EDIFACT_ENCODATION] > 0)
                    {
                        return EDIFACT_ENCODATION;
                    }
                    else if (minCount == 1 && mins[TEXT_ENCODATION] > 0)
                    {
                        return TEXT_ENCODATION;
                    }
                    else if (minCount == 1 && mins[X12_ENCODATION] > 0)
                    {
                        return X12_ENCODATION;
                    }
                    else if (intCharCounts[C40_ENCODATION] + 1 < intCharCounts[ASCII_ENCODATION] && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[BASE256_ENCODATION] && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[EDIFACT_ENCODATION] && intCharCounts[C40_ENCODATION] + 1 < intCharCounts[TEXT_ENCODATION])
                    {
                        if (intCharCounts[C40_ENCODATION] < intCharCounts[X12_ENCODATION])
                        {
                            return C40_ENCODATION;
                        }
                        else if (intCharCounts[C40_ENCODATION] == intCharCounts[X12_ENCODATION])
                        {
                            int p = startpos + charsProcessed + 1;
                            while (p < msg.Length)
                            {
                                char tc = msg[p];
                                if (IsX12TermSep(tc))
                                {
                                    return X12_ENCODATION;
                                }
                                else if (!IsNativeX12(tc))
                                {
                                    break;
                                }
                                p++;
                            }
                            return C40_ENCODATION;
                        }
                    }
                }
            }
        }

        private static int FindMinimums(float[] charCounts, int[] intCharCounts, int min, sbyte[] mins)
        {
            for (int i = 0; i < mins.Length; i++)
                mins[i] = 0;
            for (int i = 0; i < 6; i++)
            {
                intCharCounts[i] = (int)Math.Ceiling(charCounts[i]);
                int current = intCharCounts[i];
                if (min > current)
                {
                    min = current;
                    for (int j = 0; j < mins.Length; j++)
                        mins[j] = 0;
                }
                if (min == current)
                {
                    mins[i]++;

                }
            }
            return min;
        }

        private static int GetMinimumCount(sbyte[] mins)
        {
            int minCount = 0;
            for (int i = 0; i < 6; i++)
            {
                minCount += mins[i];
            }
            return minCount;
        }

        private static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private static bool IsExtendedASCII(char ch)
        {
            return (ch >= 128 && ch <= 255);
        }

        private static bool IsASCII7(char ch)
        {
            return (ch >= 0 && ch <= 127);
        }

        private static bool IsNativeC40(char ch)
        {
            return (ch == 32) || (ch >= 48 && ch <= 57) || (ch >= 65 && ch <= 90); //A..Z - 0..9
        }

        private static bool IsNativeText(char ch)
        {
            return (ch == 32) || (ch >= 48 && ch <= 57) || (ch >= 97 && ch <= 122); //a..z - 0..9
        }

        private static bool IsNativeX12(char ch)
        {
            return IsX12TermSep(ch) || (ch == 32) || (ch >= 48 && ch <= 57) || (ch >= 65 && ch <= 90); //SPACE
        }

        private static bool IsX12TermSep(char ch)
        {
            return (ch == 13) || (ch == 42) || (ch == 62); //">" - "*" - CR
        }

        private static bool IsNativeEDIFACT(char ch)
        {
            return (ch >= 32 && ch <= 94);
        }

        private static bool IsSpecialB256(char ch)
        {
            return false; //TODO NOT IMPLEMENTED YET!!!
        }

        /// <summary>
        /// Determines the number of consecutive characters that are encodable using numeric compaction. </summary>
        /// <param name="msg"> the message </param>
        /// <param name="startpos"> the start position within the message </param>
        /// <returns> the requested character count </returns>
        public static int DetermineConsecutiveDigitCount(string msg, int startpos)
        {
            int count = 0;
            int len = msg.Length;
            int idx = startpos;
            if (idx < len)
            {
                char ch = msg[idx];
                while (IsDigit(ch) && idx < len)
                {
                    count++;
                    idx++;
                    if (idx < len)
                    {
                        ch = msg[idx];
                    }
                }
            }
            return count;
        }

        private static void IllegalCharacter(char c)
        {
            string hex = (Convert.ToInt32(c)).ToString("X");
            const string padding = "0000";
            hex = padding.Substring(0, 4 - hex.Length) + hex;
            throw new System.ArgumentException("Illegal character: " + c + " (0x" + hex + ")");
        }

    }
}
