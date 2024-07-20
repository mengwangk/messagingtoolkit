using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal sealed class TextEncoder : C40Encoder
    {

        public override int EncodingMode
        {
            get
            {
                return HighLevelEncoder.TEXT_ENCODATION;
            }
        }

        internal override int EncodeChar(char c, StringBuilder sb)
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
                HighLevelEncoder.IllegalCharacter(c);
                return -1;
            }
        }
    }
}
