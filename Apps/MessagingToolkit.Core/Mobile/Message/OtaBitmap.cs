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
using System.Drawing;
using System.Collections;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Represent an OTA bitmap
    /// </summary>
    public class OtaBitmap: Sms
    {

        #region =============================================== Constants =====================================================
        
        // 5506 - operator logo

        /// <summary>
        /// OTA destination port
        /// </summary>
        protected const int OtaBitmapDestinationPort = 5507;

        /// <summary>
        /// OTA source port
        /// </summary>
        protected const int OtaBitmapSourcePort = 0;


        #endregion ============================================================================================================

        #region ============================================== Private Variables ==============================================

        private byte[] bitmap;
        private int dataLen;
        private int dataStart;
        private byte grayscales;
        private byte height;
        private byte infoField;
        private byte width;

        #endregion ============================================================================================================

        #region ============================================== Constructor ====================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public OtaBitmap(Bitmap bitmap): base()
        {
            this.SourcePort = OtaBitmapSourcePort;
            this.DestinationPort = OtaBitmapDestinationPort;
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            this.userDataHeaderIndicator = 1;
            
            this.bitmap = BitmapToOtaBitmap(bitmap);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="otaBitmap">The ota bitmap.</param>
        public OtaBitmap(byte[] otaBitmap): base()
        {
            this.SourcePort = OtaBitmapSourcePort;
            this.DestinationPort = OtaBitmapDestinationPort;
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            this.userDataHeaderIndicator = 1;

            if (otaBitmap == null)
            {
                throw new ArgumentException("OtaBitmap is blank");
            }
            int num = 0;
            this.infoField = otaBitmap[num++];
            this.width = otaBitmap[num++];
            this.height = otaBitmap[num++];
            this.grayscales = otaBitmap[num++];
            this.dataStart = num;
            this.dataLen = otaBitmap.Length - num;
            this.bitmap = new byte[otaBitmap.Length];
            otaBitmap.CopyTo(this.bitmap, 0);
        }

        #endregion ============================================================================================================

        #region ============================================== Private Static Method ==========================================

        /// <summary>
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private static byte[] BitmapToOtaBitmap(Bitmap bitmap)
        {            
            byte[] buffer = null;
            byte[] array = null;
            if (bitmap != null)
            {
                if (((bitmap.Height < 1) || (bitmap.Width < 1)) || ((bitmap.Height > 0xff) || (bitmap.Width > 0xff)))
                {
                    throw new ArgumentException("Invalid bitmap dimensions. Maximum size is 255x255, minimum size is 1x1 pixels.");
                }
                int num = 7;
                byte num2 = 0;
                ArrayList list = new ArrayList();
                for (int i = 0; i < bitmap.Height; i++)
                {
                    for (int j = 0; j < bitmap.Width; j++)
                    {
                        byte num5 = (byte)Math.Pow(2.0, (double)num);
                        if (bitmap.GetPixel(j, i).ToArgb() == Color.Black.ToArgb())
                        {
                            num2 = (byte)(num2 | num5);
                        }
                        if (num == 0)
                        {
                            list.Add(num2);
                            num2 = 0;
                            num = 7;
                        }
                        else
                        {
                            num--;
                        }
                    }
                }
                if (num < 7)
                {
                    list.Add(num2);
                }
                array = new byte[list.Count];
                list.CopyTo(array);
                byte[] buffer4 = new byte[4];
                buffer4[1] = (byte)bitmap.Width;
                buffer4[2] = (byte)bitmap.Height;
                buffer4[3] = 1;
                buffer = buffer4;
            }
            else
            {
                array = new byte[0];
                byte[] buffer5 = new byte[4];
                buffer5[3] = 1;
                buffer = buffer5;
            }
            byte[] buffer3 = new byte[buffer.Length + array.Length];
            buffer.CopyTo(buffer3, 0);
            array.CopyTo(buffer3, buffer.Length);
            return buffer3;
        }

        /// <summary>
        /// </summary>
        /// <param name="otaBitmap"></param>
        /// <returns></returns>
        private static Bitmap OtaBitmapToBitmap(byte[] otaBitmap)
        {
            if (otaBitmap == null)
            {
                return null;
            }
            int num = 0;
            byte num1 = otaBitmap[num++];
            byte width = otaBitmap[num++];
            byte height = otaBitmap[num++];
            byte num9 = otaBitmap[num++];
            if ((width == 0) || (height == 0))
            {
                return null;
            }
            Bitmap bitmap = new Bitmap(width, height);
            int num4 = 0;
            byte num5 = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (num4 == 0)
                    {
                        num5 = otaBitmap[num++];
                        num4 = 7;
                    }
                    else
                    {
                        num4--;
                    }
                    byte num8 = (byte)Math.Pow(2.0, (double)num4);
                    bitmap.SetPixel(j, i, ((num5 & num8) > 0) ? Color.Black : Color.White);
                }
            }
            return bitmap;
        }

        #endregion ============================================================================================================

        #region ============================================== Public Static Method ===========================================

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static explicit operator Bitmap(OtaBitmap b)
        {
            return b.ToBitmap();
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static explicit operator OtaBitmap(byte[] b)
        {
            return new OtaBitmap(b);
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static explicit operator OtaBitmap(Bitmap b)
        {
            return new OtaBitmap(b);
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static implicit operator byte[](OtaBitmap b)
        {
            return b.ToByteArray();
        }

        #endregion ============================================================================================================

        #region ============================================== Public Method ==================================================

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public Bitmap ToBitmap()
        {
            return OtaBitmapToBitmap(this.bitmap);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            byte[] array = new byte[this.bitmap.Length];
            this.bitmap.CopyTo(array, 0);
            return array;
        }

        #endregion ============================================================================================================

        #region ============================================== Public Property ================================================

        /// <summary>
        /// </summary>
        /// <value></value>
        public byte[] Data
        {
            get
            {
                byte[] destinationArray = new byte[this.dataLen];
                Array.Copy(this.bitmap, this.dataStart, destinationArray, 0, this.dataLen);
                return destinationArray;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public byte Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public byte InfoField
        {
            get
            {
                return this.infoField;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public byte NumGrayscales
        {
            get
            {
                return this.grayscales;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public byte Width
        {
            get
            {
                return this.width;
            }
        }

        #endregion ============================================================================================================


       

        #region ============================================== Internal Method =================================================

        /// <summary>
        /// Encode the vCard content
        /// </summary>
        /// <returns>Encoded vCard content</returns>
        internal override byte[] GetPdu()
        {           
            /*
            char[] hexDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            byte[] bytes = ToByteArray();
            char[] chArray = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int num2 = bytes[i];
                chArray[i * 2] = hexDigits[num2 >> 4];
                chArray[(i * 2) + 1] = hexDigits[num2 & 15];
            }

            string pdu = new string(chArray);
            return pdu;
            */
            return ToByteArray();
        }

        #endregion ============================================================================================================


        #region ============== Factory method   ===============================================================================


        /// <summary>
        /// Static factory to create the OtaBitmap instance
        /// </summary>
        /// <param name="bitmap">Bitmap object</param>
        /// <returns>A new instance of the OtaBitmap object</returns>
        public static OtaBitmap NewInstance(Bitmap bitmap)
        {
            return new OtaBitmap(bitmap);
        }

        /// <summary>
        /// Static factory to create the OtaBitmap instance
        /// </summary>
        /// <param name="otaBitmap">Bitmap byte array</param>
        /// <returns>A new instance of the OtaBitmap object</returns>
        public static OtaBitmap NewInstance(byte[] otaBitmap)
        {
            return new OtaBitmap(otaBitmap);
        }


        #endregion ============================================================================================================

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "DestinationAddress = ", DestinationAddress, "\r\n");
            str = String.Concat(str, "Protocol = ", Protocol, "\r\n");
            str = String.Concat(str, "Flash = ", Flash, "\r\n");
            str = String.Concat(str, "SourcePort = ", SourcePort, "\r\n");
            str = String.Concat(str, "DestinationPort = ", DestinationPort, "\r\n");
            str = String.Concat(str, "ReferenceNo = ", ReferenceNo, "\r\n");
            str = String.Concat(str, "Indexes = ", Indexes, "\r\n");
            str = String.Concat(str, "SaveSentMessage = ", SaveSentMessage, "\r\n");
            str = String.Concat(str, "RawMessage = ", RawMessage, "\r\n");
            str = String.Concat(str, "LongMessageOption = ", LongMessageOption, "\r\n");
            str = String.Concat(str, "ReplyPath = ", ReplyPath, "\r\n");
            str = String.Concat(str, "Data = ", Data, "\r\n");
            str = String.Concat(str, "Height = ", Height, "\r\n");
            str = String.Concat(str, "InfoField = ", InfoField, "\r\n");
            str = String.Concat(str, "NumGrayscales = ", NumGrayscales, "\r\n");
            str = String.Concat(str, "Width = ", Width, "\r\n");
            return str;
        }
    }
}
