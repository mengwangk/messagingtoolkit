using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal sealed class EncoderContext
    {

#if (SILVERLIGHT || NETFX_CORE || PORTABLE)
        public const string DefaultEncoding = "UTF-8";
#else
        public const string DefaultEncoding = "ISO-8859-1";
#endif


        private string msg;
        private SymbolShapeHint shape;
        private Dimension minSize;
        private Dimension maxSize;
        private StringBuilder codewords;
        internal int pos;
        private int newEncoding;
        private SymbolInfo symbolInfo;
        private int skipAtEnd;

        internal EncoderContext(string msg)
        {
            // From this point on Strings are not Unicode anymore!
            byte[] msgBinary;
            try
            {
                msgBinary = Encoding.GetEncoding(DefaultEncoding).GetBytes(msg);
            }
            catch (Exception e)
            {
                throw new NotSupportedException("Unsupported encoding: " + e.Message);
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
            this.msg = sb.ToString(); // Not Unicode here!
            shape = SymbolShapeHint.ForceNone;
            this.codewords = new StringBuilder(msg.Length);
            newEncoding = -1;
        }

        public EncoderContext(byte[] data)
        {
            // From this point on Strings are not Unicode anymore!
            StringBuilder sb = new StringBuilder(data.Length);
            for (int i = 0, c = data.Length; i < c; i++)
            {
                char ch = (char)(data[i] & 0xff);
                sb.Append(ch);
            }
            this.msg = sb.ToString(); // Not Unicode here!
            this.codewords = new StringBuilder(msg.Length);
        }

        public SymbolShapeHint SymbolShape
        {
            set
            {
                this.shape = value;
            }
        }

        public void SetSizeConstraints(Dimension minSize, Dimension maxSize)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;
        }

        public string Message
        {
            get
            {
                return this.msg;
            }
        }

        public int SkipAtEnd
        {
            set
            {
                this.skipAtEnd = value;
            }
        }

        public char CurrentChar
        {
            get
            {
                return msg[pos];
            }
        }

        public char Current
        {
            get
            {
                return msg[pos];
            }
        }

        public StringBuilder Codewords
        {
            get
            {
                return codewords;
            }
        }

        public void WriteCodewords(string codewords)
        {
            this.codewords.Append(codewords);
        }

        public void WriteCodeword(char codeword)
        {
            this.codewords.Append(codeword);
        }

        public int CodewordCount
        {
            get
            {
                return this.codewords.Length;
            }
        }

        public int NewEncoding
        {
            get
            {
                return newEncoding;
            }
        }

        public int Position
        {
            get
            {
                return this.pos;
            }
        }

        public void SignalEncoderChange(int encoding)
        {
            this.newEncoding = encoding;
        }

        public void ResetEncoderSignal()
        {
            this.newEncoding = -1;
        }

        public bool HasMoreCharacters()
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

        public int RemainingCharacters
        {
            get
            {
                return TotalMessageCharCount - pos;
            }
        }

        public SymbolInfo SymbolInfo
        {
            get
            {
                return symbolInfo;
            }
        }

        public void UpdateSymbolInfo()
        {
            UpdateSymbolInfo(CodewordCount);
        }

        public void UpdateSymbolInfo(int len)
        {
            if (this.symbolInfo == null || len > this.symbolInfo.DataCapacity)
            {
                this.symbolInfo = SymbolInfo.Lookup(len, shape, minSize, maxSize, true);
            }
        }

        public void ResetSymbolInfo()
        {
            this.symbolInfo = null;
        }
    }
}
