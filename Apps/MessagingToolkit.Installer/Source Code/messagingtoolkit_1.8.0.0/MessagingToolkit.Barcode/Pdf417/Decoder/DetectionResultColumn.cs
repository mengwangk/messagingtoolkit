using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal class DetectionResultColumn
    {

        private const int MAX_NEARBY_DISTANCE = 5;

        private readonly BoundingBox boundingBox;
        private readonly Codeword[] codewords;

        internal DetectionResultColumn(BoundingBox boundingBox)
        {
            this.boundingBox = new BoundingBox(boundingBox);
            codewords = new Codeword[boundingBox.MaxY - boundingBox.MinY + 1];
        }

        internal Codeword GetCodewordNearby(int imageRow)
        {
            Codeword codeword = GetCodeword(imageRow);
            if (codeword != null)
            {
                return codeword;
            }
            for (int i = 1; i < MAX_NEARBY_DISTANCE; i++)
            {
                int nearImageRow = ImageRowToCodewordIndex(imageRow) - i;
                if (nearImageRow >= 0)
                {
                    codeword = codewords[nearImageRow];
                    if (codeword != null)
                    {
                        return codeword;
                    }
                }
                nearImageRow = ImageRowToCodewordIndex(imageRow) + i;
                if (nearImageRow < codewords.Length)
                {
                    codeword = codewords[nearImageRow];
                    if (codeword != null)
                    {
                        return codeword;
                    }
                }
            }
            return null;
        }

        internal int ImageRowToCodewordIndex(int imageRow)
        {
            return imageRow - boundingBox.MinY;
        }

        internal int CodewordIndexToImageRow(int codewordIndex)
        {
            return boundingBox.MinY + codewordIndex;
        }

        internal void SetCodeword(int imageRow, Codeword codeword)
        {
            codewords[ImageRowToCodewordIndex(imageRow)] = codeword;
        }

        internal Codeword GetCodeword(int imageRow)
        {
            return codewords[ImageRowToCodewordIndex(imageRow)];
        }

        internal BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }

        internal Codeword[] Codewords
        {
            get
            {
                return codewords;
            }
        }

        public override string ToString()
        {
            StringBuilder formatter = new StringBuilder();
            int row = 0;
            foreach (Codeword codeword in codewords)
            {
                if (codeword == null)
                {
                    formatter.Append(String.Format("%3d:    |   \n", row++));
                    continue;
                }
                formatter.Append(String.Format("%3d: %3d|%3d\n", row++, codeword.RowNumber, codeword.Value));
            }
            string result = formatter.ToString();
            return result;
        }

    }
}
