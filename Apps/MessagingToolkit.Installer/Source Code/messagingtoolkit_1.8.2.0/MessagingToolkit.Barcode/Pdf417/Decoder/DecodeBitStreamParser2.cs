using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class DecodeBitStreamParser2
    {
        /*****
         private const int TEXT_COMPACTION_MODE_LATCH = 900;
    private const int BYTE_COMPACTION_MODE_LATCH = 901;
    private const int NUMERIC_COMPACTION_MODE_LATCH = 902;
    private const int BYTE_COMPACTION_MODE_LATCH_6 = 924;
    private const int BEGIN_MACRO_PDF417_CONTROL_BLOCK = 928;
    private const int BEGIN_MACRO_PDF417_OPTIONAL_FIELD = 923;
    private const int MACRO_PDF417_TERMINATOR = 922;
    private const int MODE_SHIFT_TO_BYTE_COMPACTION_MODE = 913;
    private const int MAX_NUMERIC_CODEWORDS = 15;

    private const int ALPHA = 0;
    private const int LOWER = 1;
    private const int MIXED = 2;
    private const int PUNCT = 3;
    private const int PUNCT_SHIFT = 4;

    private const int PL = 25;
    private const int LL = 27;
    private const int AS = 27;
    private const int ML = 28;
    private const int AL = 28;
    private const int PS = 29;
    private const int PAL = 29;

    private const char[] PUNCT_CHARS ={ ';', '<', '>', '@', '[', '\\', '}', '_', '`', '~', '!', '\r', '\t', ',', ':', '\n', '-', '.', '$', '/', '"', '|', '*', '(', ')', '?', '{', '}', '\'' };

    private const char[] MIXED_CHARS = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '&', '\r', '\t', ',', ':', '#', '-', '.', '$', '/', '+', '%', '*', '=', '^' };

     private const string[] EXP900 = { 
        "000000000000000000000000000000000000000000001",
        "000000000000000000000000000000000000000000900",
        "000000000000000000000000000000000000000810000",
        "000000000000000000000000000000000000729000000",
        "000000000000000000000000000000000656100000000",
        "000000000000000000000000000000590490000000000",
        "000000000000000000000000000531441000000000000",
        "000000000000000000000000478296900000000000000",
        "000000000000000000000430467210000000000000000",
        "000000000000000000387420489000000000000000000",
        "000000000000000348678440100000000000000000000",
        "000000000000313810596090000000000000000000000",
        "000000000282429536481000000000000000000000000",
        "000000254186582832900000000000000000000000000",
        "000228767924549610000000000000000000000000000",
        "205891132094649000000000000000000000000000000"};
            
        private const string ASCII = "ASCII";
        private const string ISO88591 = "ISO-8859-1";
        private const string UTF8 = "UTF-8";
        private const string SHIFT_JIS = "SHIFT_JIS";
        private const string EUC_JP = "EUC-JP";

        private void append(string result, const unsigned char *bufIn, size_t nIn, const char *src) {
#ifndef NO_ICONV
            if (nIn == 0) {
                return;
            }
            
            iconv_t cd = iconv_open(UTF8, src);
            const int maxOut = 4 * nIn + 1;
            unsigned char* bufOut = new unsigned char[maxOut];
            
            ICONV_CONST char *fromPtr = (ICONV_CONST char *)bufIn;
            size_t nFrom = nIn;
            char *toPtr = (char *)bufOut;
            size_t nTo = maxOut;
            
            while (nFrom > 0) {
                size_t oneway = iconv(cd, &fromPtr, &nFrom, &toPtr, &nTo);
                if (oneway == (size_t)(-1)) {
                    iconv_close(cd);
                    delete[] bufOut;
                    throw ReaderException("error converting characters");
                }
            }
            iconv_close(cd);
            
            int nResult = maxOut - nTo;
            bufOut[nResult] = '\0';
            result.append((const char *)bufOut);
            delete[] bufOut;
#else
            result.append((const char *)bufIn, nIn);
#endif
        }
        
        public string Decode(List<int> codewords) {
            string result;
            // Get compaction mode
            int codeIndex = 1;
            int code = codewords[codeIndex++];
            while (codeIndex < codewords[0]) {
                switch (code) {
                    case TEXT_COMPACTION_MODE_LATCH: {
                        codeIndex = textCompaction(codewords, codeIndex, result);
                        break;
                    }
                    case BYTE_COMPACTION_MODE_LATCH: {
                        codeIndex = byteCompaction(code, codewords, codeIndex, result);
                        break;
                    }
                    case NUMERIC_COMPACTION_MODE_LATCH: {
                        codeIndex = numericCompaction(codewords, codeIndex, result);
                        break;
                    }
                    case MODE_SHIFT_TO_BYTE_COMPACTION_MODE: {
                        codeIndex = byteCompaction(code, codewords, codeIndex, result);
                        break;
                    }
                    case BYTE_COMPACTION_MODE_LATCH_6: {
                        codeIndex = byteCompaction(code, codewords, codeIndex, result);
                        break;
                    }
                    default: {
                        // Default to text compaction. During testing numerous barcodes
                        // appeared to be missing the starting mode. In these cases defaulting
                        // to text compaction seems to work.
                        codeIndex--;
                        codeIndex = textCompaction(codewords, codeIndex, result);
                        break;
                    }
                }
                if (codeIndex < codewords->size()) {
                    code = codewords[codeIndex++];
                } else {
                    throw ReaderException("Invalid index");
                }
            }
            
            return result;
        }
        
        int textCompaction(ArrayRef<int> codewords, int codeIndex, std::string &result) {
            // 2 character per codeword
            ArrayRef<int> textCompactionData(codewords[0] << 1);
            // Used to hold the byte compaction value if there is a mode shift
            ArrayRef<int> byteCompactionData(codewords[0] << 1);
            
            int index = 0;
            bool end = false;
            while ((codeIndex < codewords[0]) && !end) {
                int code = codewords[codeIndex++];
                if (code < TEXT_COMPACTION_MODE_LATCH) {
                    textCompactionData[index] = code / 30;
                    textCompactionData[index + 1] = code % 30;
                    index += 2;
                } else {
                    switch (code) {
                        case TEXT_COMPACTION_MODE_LATCH: {
                            codeIndex--;
                            end = true;
                            break;
                        }
                        case BYTE_COMPACTION_MODE_LATCH: {
                            codeIndex--;
                            end = true;
                            break;
                        }
                        case NUMERIC_COMPACTION_MODE_LATCH: {
                            codeIndex--;
                            end = true;
                            break;
                        }
                        case MODE_SHIFT_TO_BYTE_COMPACTION_MODE: {
                            // The Mode Shift codeword 913 shall cause a temporary
                            // switch from Text Compaction mode to Byte Compaction mode.
                            // This switch shall be in effect for only the next codeword,
                            // after which the mode shall revert to the prevailing sub-mode
                            // of the Text Compaction mode. Codeword 913 is only available
                            // in Text Compaction mode; its use is described in 5.4.2.4.
                            textCompactionData[index] = MODE_SHIFT_TO_BYTE_COMPACTION_MODE;
                            byteCompactionData[index] = code; //Integer.toHexString(code);
                            index++;
                            break;
                        }
                        case BYTE_COMPACTION_MODE_LATCH_6: {
                            codeIndex--;
                            end = true;
                            break;
                        }
                    }
                }
            }
            decodeTextCompaction(textCompactionData, byteCompactionData, index, result);
            return codeIndex;
        }
        
        void decodeTextCompaction(ArrayRef<int> textCompactionData, ArrayRef<int> byteCompactionData, int length, std::string &result) {
            // Beginning from an initial state of the Alpha sub-mode
            // The default compaction mode for PDF417 in effect at the start of each symbol shall always be Text
            // Compaction mode Alpha sub-mode (uppercase alphabetic). A latch codeword from another mode to the Text
            // Compaction mode shall always switch to the Text Compaction Alpha sub-mode.
            int subMode = ALPHA;
            int priorToShiftMode = ALPHA;
            int i = 0;
            while (i < length) {
                int subModeCh = textCompactionData[i];
                if(subModeCh<0) { subModeCh = 0; } //Hack to fix when smaller than 0 number comes.
                unsigned char ch = 0;
                switch (subMode) {
                    case ALPHA:
                        // Alpha (uppercase alphabetic)
                        if (subModeCh < 26) {
                            // Upper case Alpha Character
                            ch = (char) ('A' + subModeCh);
                        } else {
                            if (subModeCh == 26) {
                                ch = ' ';
                            } else if (subModeCh == LL) {
                                subMode = LOWER;
                            } else if (subModeCh == ML) {
                                subMode = MIXED;
                            } else if (subModeCh == PS) {
                                // Shift to punctuation
                                priorToShiftMode = subMode;
                                subMode = PUNCT_SHIFT;
                            } else if (subModeCh == MODE_SHIFT_TO_BYTE_COMPACTION_MODE) {
                                append(result, (unsigned char*) &byteCompactionData[i], 1, ASCII);
                            }
                        }
                        break;
                        
                    case LOWER:
                        // Lower (lowercase alphabetic)
                        if (subModeCh < 26) {
                            ch = (char) ('a' + subModeCh);
                        } else {
                            if (subModeCh == 26) {
                                ch = ' ';
                            } else if (subModeCh == AL) {
                                subMode = ALPHA;
                            } else if (subModeCh == ML) {
                                subMode = MIXED;
                            } else if (subModeCh == PS) {
                                // Shift to punctuation
                                priorToShiftMode = subMode;
                                subMode = PUNCT_SHIFT;
                            } else if (subModeCh == MODE_SHIFT_TO_BYTE_COMPACTION_MODE) {
                                append(result, (unsigned char*) &byteCompactionData[i], 1, ASCII);
                            }
                        }
                        break;
                        
                    case MIXED:
                        // Mixed (numeric and some punctuation)
                        if (subModeCh < PL) {
                            ch = MIXED_CHARS[subModeCh];
                        } else {
                            if (subModeCh == PL) {
                                subMode = PUNCT;
                            } else if (subModeCh == 26) {
                                ch = ' ';
                            } else if (subModeCh == AS) {
                                //mode_change = true;
                            } else if (subModeCh == AL) {
                                subMode = ALPHA;
                            } else if (subModeCh == PS) {
                                // Shift to punctuation
                                priorToShiftMode = subMode;
                                subMode = PUNCT_SHIFT;
                            } else if (subModeCh == MODE_SHIFT_TO_BYTE_COMPACTION_MODE) {
                                append(result, (unsigned char*) &byteCompactionData[i], 1, ASCII);
                            }
                        }
                        break;
                        
                    case PUNCT:
                        // Punctuation
                        if (subModeCh < PS) {
                            ch = PUNCT_CHARS[subModeCh];
                        } else {
                            if (subModeCh == PAL) {
                                subMode = ALPHA;
                            } else if (subModeCh == MODE_SHIFT_TO_BYTE_COMPACTION_MODE) {
                                append(result, (unsigned char*) &byteCompactionData[i], 1, ASCII);
                            }
                        }
                        break;
                        
                    case PUNCT_SHIFT:
                        // Restore sub-mode
                        subMode = priorToShiftMode;
                        if (subModeCh < PS) {
                            ch = PUNCT_CHARS[subModeCh];
                        } else {
                            if (subModeCh == PAL) {
                                subMode = ALPHA;
                            }
                        }
                        break;
                }
                if (ch != 0) {
                    // Append decoded character to result
                    //result += ch;
                    append(result, &ch, 1, ASCII);
                }
                i++;
            }
        }
        
        int byteCompaction(int mode, ArrayRef<int> codewords, int codeIndex, std::string &result) {
            if (mode == BYTE_COMPACTION_MODE_LATCH) {
                // Total number of Byte Compaction characters to be encoded
                // is not a multiple of 6
                int count = 0;
                long long value = 0;
                unsigned char decodedData[6];
                int byteCompactedCodewords[6];
                for(int i = 0; i< 6; i++){
                    byteCompactedCodewords[i] = 0;
                }
                bool end = false;
                while ((codeIndex < codewords[0]) && !end) {
                    int code = codewords[codeIndex++];
                    if (code < TEXT_COMPACTION_MODE_LATCH) {
                        byteCompactedCodewords[count] = code;
                        count++;
                        // Base 900
                        value = 900 * value + code;
                    } else {
                        if (code == TEXT_COMPACTION_MODE_LATCH ||
                            code == BYTE_COMPACTION_MODE_LATCH ||
                            code == NUMERIC_COMPACTION_MODE_LATCH ||
                            code == BYTE_COMPACTION_MODE_LATCH_6 ||
                            code == BEGIN_MACRO_PDF417_CONTROL_BLOCK ||
                            code == BEGIN_MACRO_PDF417_OPTIONAL_FIELD ||
                            code == MACRO_PDF417_TERMINATOR) {
                            codeIndex--;
                            end = true;
                        }
                    }
                    if ((count % 5 == 0) && (count > 0)) {
                        // Decode every 5 codewords
                        // Convert to Base 256
                        for (int j = 0; j < 6; ++j) {
                            decodedData[5 - j] = (char) (value % 256);
                            value >>= 8;
                        }
                        const char *encoding = guessEncoding(decodedData, 6);
                        append(result, decodedData, 6, encoding);
                        count = 0;
                    }
                }
                // If Byte Compaction mode is invoked with codeword 901,
                // the final group of codewords is interpreted directly
                // as one byte per codeword, without compaction.
                for (int i = ((count / 5) * 5); i < count; i++) {
                    const char *encoding = guessEncoding((unsigned char*)&byteCompactedCodewords[i], 1);
                    append(result, (unsigned char*)&byteCompactedCodewords[i], 1, encoding);
                }
                
            } else if (mode == BYTE_COMPACTION_MODE_LATCH_6) {
                // Total number of Byte Compaction characters to be encoded
                // is an integer multiple of 6
                int count = 0;
                long long value = 0;
                bool end = false;
                while (codeIndex < codewords[0] && !end) {
                    int code = codewords[codeIndex++];
                    if (code < TEXT_COMPACTION_MODE_LATCH) {
                        count++;
                        // Base 900
                        value = 900 * value + code;
                    } else {
                        if (code == TEXT_COMPACTION_MODE_LATCH ||
                            code == BYTE_COMPACTION_MODE_LATCH ||
                            code == NUMERIC_COMPACTION_MODE_LATCH ||
                            code == BYTE_COMPACTION_MODE_LATCH_6 ||
                            code == BEGIN_MACRO_PDF417_CONTROL_BLOCK ||
                            code == BEGIN_MACRO_PDF417_OPTIONAL_FIELD ||
                            code == MACRO_PDF417_TERMINATOR) {
                            codeIndex--;
                            end = true;
                        }
                    }
                    if ((count % 5 == 0) && (count > 0)) {
                        // Decode every 5 codewords
                        // Convert to Base 256
                        unsigned char decodedData[6];
                        for (int j = 0; j < 6; ++j) {
                            decodedData[5 - j] = (char) (value & 0xFF);
                            value >>= 8;
                        }
                        const char *encoding = guessEncoding(decodedData, 6);
                        append(result, decodedData, 6, encoding);
                    }
                }
            }
            return codeIndex;
        }
        
        int numericCompaction(ArrayRef<int> codewords, int codeIndex, std::string &result) {
            int count = 0;
            bool end = false;
            
            ArrayRef<int> numericCodewords(MAX_NUMERIC_CODEWORDS);
            
            while (codeIndex < codewords[0] && !end) {
                int code = codewords[codeIndex++];
                if (codeIndex == codewords[0]) {
                    end = true;
                }
                if (code < TEXT_COMPACTION_MODE_LATCH) {
                    numericCodewords[count] = code;
                    count++;
                } else {
                    if (code == TEXT_COMPACTION_MODE_LATCH ||
                        code == BYTE_COMPACTION_MODE_LATCH ||
                        code == BYTE_COMPACTION_MODE_LATCH_6 ||
                        code == BEGIN_MACRO_PDF417_CONTROL_BLOCK ||
                        code == BEGIN_MACRO_PDF417_OPTIONAL_FIELD ||
                        code == MACRO_PDF417_TERMINATOR) {
                        codeIndex--;
                        end = true;          
                    }
                }
                if (count % MAX_NUMERIC_CODEWORDS == 0 ||
                    code == NUMERIC_COMPACTION_MODE_LATCH ||
                    end) {
                    // Re-invoking Numeric Compaction mode (by using codeword 902
                    // while in Numeric Compaction mode) serves  to terminate the
                    // current Numeric Compaction mode grouping as described in 5.4.4.2,
                    // and then to start a new one grouping.
                    
                    result.append(decodeBase900toBase10(numericCodewords, count));
                    //append(result, decodedData, 6, encoding);
                    count = 0;
                }
            }
            return codeIndex;
        }

        
        std::string decodeBase900toBase10(ArrayRef<int> codewords, int count) {
            std::string result;
            std::string accum;
            bool first = true;
            for (int i = 0; i < count; i++) {
                std::string value;
                const char * exp = EXP900[count - i - 1];
                std::string expString;
                expString.assign(exp);
                value = multiply(expString, codewords[i]);
                if (first) {
                    // First time in accum=0
                    accum.assign(value);
                    first = false;
                } else {
                    accum = add(accum, value);
                }
            }

            for (int i = 0; i < accum.length(); i++) {
                if (accum.at(i) == '1') {
                    result.assign(accum.substr(i+1, accum.length()-i-1));
                    break;
                }
            }
            return result;
        }
        
        std::string multiply(std::string &value1, int value2) {
            
            std::string result(value1.length(), '0');

            int hundreds = value2 / 100;
            int tens = (value2 / 10) % 10;
            int ones = value2 % 10;
            // Multiply by ones
            for (int j = 0; j < ones; j++) {
                result = add(result, value1);
            }
            // Multiply by tens
            
            std::string shifted(value1);
            shifted.erase(shifted.begin());
            shifted.push_back('0');
            
            for (int j = 0; j < tens; j++) {
                result = add(result, shifted);
            }
            shifted.erase(shifted.begin());
            shifted.push_back('0');

            // Multiply by hundreds
            for (int j = 0; j < hundreds; j++) {
                result = add(result, shifted);
            }
            return result;
        }

        
        std::string add(std::string &value1, std::string &value2) {
            string temp1;
            string temp2;
            
            std::string result(value1.length(), '0');
            
            int carry = 0;
            for (int i = value1.length() - 3; i > -1; i -= 3) {
                temp1.clear();
                temp1 += value1[i];
                temp1 += value1[i + 1];
                temp1 += value1[i + 2];
                temp2.clear();                
                temp2 += value2[i];
                temp2 += value2[i + 1];
                temp2 += value2[i + 2];
                
                int intValue1 = atoi(temp1.c_str());
                int intValue2 = atoi(temp2.c_str());
                
                int sumval = (intValue1 + intValue2 + carry) % 1000;
                carry = (intValue1 + intValue2 + carry) / 1000;
                
                result[i + 2] =  (char) ((sumval % 10) + '0');
                result[i + 1] =  (char) (((sumval / 10) % 10) + '0');
                result[i] = (char) ((sumval / 100) + '0');
            }
            return result;
        }

        const char *
        guessEncoding(unsigned char *bytes, int length) {
            const bool ASSUME_SHIFT_JIS = false;
            char const* const PLATFORM_DEFAULT_ENCODING="UTF-8";
            
            // Does it start with the UTF-8 byte order mark? then guess it's UTF-8
            if (length > 3 && bytes[0] == (unsigned char)0xEF && bytes[1] == (unsigned char)0xBB && bytes[2]
                == (unsigned char)0xBF) {
                return UTF8;
            }
            // For now, merely tries to distinguish ISO-8859-1, UTF-8 and Shift_JIS,
            // which should be by far the most common encodings. ISO-8859-1
            // should not have bytes in the 0x80 - 0x9F range, while Shift_JIS
            // uses this as a first byte of a two-byte character. If we see this
            // followed by a valid second byte in Shift_JIS, assume it is Shift_JIS.
            // If we see something else in that second byte, we'll make the risky guess
            // that it's UTF-8.
            bool canBeISO88591 = true;
            bool canBeShiftJIS = true;
            bool canBeUTF8 = true;
            int utf8BytesLeft = 0;
            int maybeDoubleByteCount = 0;
            int maybeSingleByteKatakanaCount = 0;
            bool sawLatin1Supplement = false;
            bool sawUTF8Start = false;
            bool lastWasPossibleDoubleByteStart = false;
            for (int i = 0;
                 i < length && (canBeISO88591 || canBeShiftJIS || canBeUTF8);
                 i++) {
                int value = bytes[i] & 0xFF;
                
                // UTF-8 stuff
                if (value >= 0x80 && value <= 0xBF) {
                    if (utf8BytesLeft > 0) {
                        utf8BytesLeft--;
                    }
                } else {
                    if (utf8BytesLeft > 0) {
                        canBeUTF8 = false;
                    }
                    if (value >= 0xC0 && value <= 0xFD) {
                        sawUTF8Start = true;
                        int valueCopy = value;
                        while ((valueCopy & 0x40) != 0) {
                            utf8BytesLeft++;
                            valueCopy <<= 1;
                        }
                    }
                }
                
                // Shift_JIS stuff
                
                if (value >= 0xA1 && value <= 0xDF) {
                    // count the number of characters that might be a Shift_JIS single-byte Katakana character
                    if (!lastWasPossibleDoubleByteStart) {
                        maybeSingleByteKatakanaCount++;
                    }
                }
                if (!lastWasPossibleDoubleByteStart &&
                    ((value >= 0xF0 && value <= 0xFF) || value == 0x80 || value == 0xA0)) {
                    canBeShiftJIS = false;
                }
                if (((value >= 0x81 && value <= 0x9F) || (value >= 0xE0 && value <= 0xEF))) {
                    // These start double-byte characters in Shift_JIS. Let's see if it's followed by a valid
                    // second byte.
                    if (lastWasPossibleDoubleByteStart) {
                        // If we just checked this and the last byte for being a valid double-byte
                        // char, don't check starting on this byte. If this and the last byte
                        // formed a valid pair, then this shouldn't be checked to see if it starts
                        // a double byte pair of course.
                        lastWasPossibleDoubleByteStart = false;
                    } else {
                        // ... otherwise do check to see if this plus the next byte form a valid
                        // double byte pair encoding a character.
                        lastWasPossibleDoubleByteStart = true;
                        if (i >= length - 1) {
                            canBeShiftJIS = false;
                        } else {
                            int nextValue = bytes[i + 1] & 0xFF;
                            if (nextValue < 0x40 || nextValue > 0xFC) {
                                canBeShiftJIS = false;
                            } else {
                                maybeDoubleByteCount++;
                            }
                            // There is some conflicting information out there about which bytes can follow which in
                            // double-byte Shift_JIS characters. The rule above seems to be the one that matches practice.
                        }
                    }
                } else {
                    lastWasPossibleDoubleByteStart = false;
                }
            }
            if (utf8BytesLeft > 0) {
                canBeUTF8 = false;
            }
            
            // Easy -- if assuming Shift_JIS and no evidence it can't be, done
            if (canBeShiftJIS && ASSUME_SHIFT_JIS) {
                return SHIFT_JIS;
            }
            if (canBeUTF8 && sawUTF8Start) {
                return UTF8;
            }
            // Distinguishing Shift_JIS and ISO-8859-1 can be a little tough. The crude heuristic is:
            // - If we saw
            //   - at least 3 bytes that starts a double-byte value (bytes that are rare in ISO-8859-1), or
            //   - over 5% of bytes could be single-byte Katakana (also rare in ISO-8859-1),
            // - and, saw no sequences that are invalid in Shift_JIS, then we conclude Shift_JIS
            if (canBeShiftJIS && (maybeDoubleByteCount >= 3 || 20 * maybeSingleByteKatakanaCount > length)) {
                return SHIFT_JIS;
            }
            // Otherwise, we default to ISO-8859-1 unless we know it can't be
            if (!sawLatin1Supplement && canBeISO88591) {
                return ISO88591;
            }
            // Otherwise, we take a wild guess with platform encoding
            return PLATFORM_DEFAULT_ENCODING;
        }
        ***/

    }
}
