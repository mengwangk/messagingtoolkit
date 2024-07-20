using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// This class renders CodaBar as bool[].
    /// 
    /// Modified: April 17 2012
    /// </summary>
    public sealed class CodaBarEncoder : OneDEncoder
    {
        private static readonly char[] START_END_CHARS = { 'A', 'B', 'C', 'D' };
        private static readonly char[] ALT_START_END_CHARS = { 'T', 'N', '*', 'E' };

        override public bool[] Encode(String contents)
        {

            if (contents.Length < 2)
            {
                throw new ArgumentException("Codabar should start/end with start/stop symbols");
            }

            // Verify input and calculate decoded length.
            char firstChar = Char.ToUpper(contents[0]);
            char lastChar = Char.ToUpper(contents[contents.Length - 1]);
            bool startsEndsNormal = CodaBarDecoder.ArrayContains(START_END_CHARS, firstChar) && CodaBarDecoder.ArrayContains(START_END_CHARS, lastChar);
            bool startsEndsAlt = CodaBarDecoder.ArrayContains(ALT_START_END_CHARS, firstChar) && CodaBarDecoder.ArrayContains(ALT_START_END_CHARS, lastChar);
            if (!(startsEndsNormal || startsEndsAlt))
            {
                throw new ArgumentException(
                     "Codabar should start/end with " + BarcodeHelper.Join(", ", START_END_CHARS) +
                     ", or start/end with " + BarcodeHelper.Join(", ", ALT_START_END_CHARS));
            }

            // The start character and the end character are decoded to 10 length each.
            int resultLength = 20;
            char[] charsWhichAreTenLengthEachAfterDecoded = { '/', ':', '+', '.' };
            for (int i = 1; i < contents.Length - 1; i++)
            {
                if (Char.IsDigit(contents[i]) || contents[i] == '-'
                    || contents[i] == '$')
                {
                    resultLength += 9;
                }
                else if (CodaBarDecoder.ArrayContains(charsWhichAreTenLengthEachAfterDecoded, contents[i]))
                {
                    resultLength += 10;
                }
                else
                {
                    throw new ArgumentException("Cannot Encode : '" + contents[i] + '\'');
                }
            }
            // A blank is placed between each character.
            resultLength += contents.Length - 1;

            bool[] result = new bool[resultLength];
            int position = 0;
            for (int index = 0; index < contents.Length; index++)
            {
                char c = Char.ToUpper(contents[index]);
                if (index == contents.Length - 1)
                {
                    // The end chars are not in the CodaBarReader.ALPHABET.
                    switch (c)
                    {
                        case 'T':
                            c = 'A';
                            break;
                        case 'N':
                            c = 'B';
                            break;
                        case '*':
                            c = 'C';
                            break;
                        case 'E':
                            c = 'D';
                            break;
                    }
                }
                int code = 0;
                for (int i = 0; i < CodaBarDecoder.ALPHABET.Length; i++)
                {
                    // Found any, because I checked above.
                    if (c == CodaBarDecoder.ALPHABET[i])
                    {
                        code = CodaBarDecoder.CHARACTER_ENCODINGS[i];
                        break;
                    }
                }
                bool color = true;
                int counter = 0;
                int bit = 0;
                while (bit < 7)
                {
                    // A character consists of 7 digit.
                    result[position] = color;
                    position++;
                    if (((code >> (6 - bit)) & 1) == 0 || counter == 1)
                    {
                        color = !color; // Flip the color.
                        bit++;
                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                }
                if (index < contents.Length - 1)
                {
                    result[position] = false;
                    position++;
                }
            }
            return result;
        }
    }
}

