using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{

    /// <summary>
    /// Base64-implementation as an Stream reading Base64-encoded data
    /// from a Reader
    /// </summary>
    internal class Base64InputStream 
    {

        private const int EOF = -1;

        private static readonly sbyte[] Lookup = new sbyte[128];

        static Base64InputStream()
        {
            for (int i = 0; i < Lookup.Length; i++)
            {
                Lookup[i] = -1;
            }
            int idx = 0;
            for (char ch = 'A'; ch <= 'Z'; ch++)
            {
                Lookup[ch] = (sbyte)idx++;
            }
            for (char ch = 'a'; ch <= 'z'; ch++)
            {
                Lookup[ch] = (sbyte)idx++;
            }
            for (char ch = '0'; ch <= '9'; ch++)
            {
                Lookup[ch] = (sbyte)idx++;
            }
            Lookup['-'] = (sbyte)idx; //URL- & filename-safe
            Lookup['+'] = (sbyte)idx++;

            Lookup['_'] = (sbyte)idx; //URL- & filename-safe
            Lookup['/'] = (sbyte)idx++;
        }

        private StringReader source;

        private char[] quadBuffer = new char[4];

        private sbyte[] triple = new sbyte[3];
        private int tripleIndex = 4;
        private int tripleFilled;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="source">the Reader to read the Base64-encoded data from</param>
        /// <exception cref="System.ArgumentException">source must not be null</exception>
        public Base64InputStream(StringReader source)
        {
            if (source == null)
            {
                throw new ArgumentException("source must not be null");
            }
            this.source = source;
        }

        public virtual int Read()
        {
            CheckOpen();
            if (tripleIndex >= tripleFilled)
            {
                if (!ReadNextTriple())
                {
                    return EOF;
                }
            }
            return triple[tripleIndex++];
        }

        private bool ReadNextTriple()
        {
            int offset = 0;
            while (offset < 4)
            {
                int ch = source.Read();
                if (ch < 0)
                {
                    return false;
                }
                else if (ch == '\r' || ch == '\n' || ch == ' ')
                {
                    continue;
                }
                quadBuffer[offset++] = (char)ch;
            }
            int quad = 0;
            tripleFilled = 3;
            for (int i = 0; i < 4; i++)
            {
                sbyte b = -1;
                char ch = quadBuffer[i];
                if ('=' == ch)
                {
                    if (i < 2)
                    {
                        throw new IOException("Padding character at invalid position");
                    }
                    else
                    {
                        tripleFilled = Math.Min(i - 1, tripleFilled);
                        break;
                    }
                }
                if (ch < 128)
                {
                    b = Lookup[ch];
                }
                if (b < 0)
                {
                    throw new IOException("Illegal Base64 character encountered: " + ch);
                }
                quad |= b << ((3 - i) * 6);
            }
            triple[0] = (sbyte)((quad & 0xFF0000) >> 16);
            triple[1] = (sbyte)((quad & 0xFF00) >> 8);
            triple[2] = (sbyte)(quad & 0xFF);
            tripleIndex = 0;
            return true;
        }


        private void CheckOpen()
        {
            if (this.source == null)
            {
                throw new IOException("Stream is already closed");
            }
        }


        public void Close()
        {
            this.source.Dispose();
            this.source = null;
        }       
    }
}
