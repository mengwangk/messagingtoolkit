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

using MessagingToolkit.Core.Helper;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Operator logo
    /// </summary>
    public class OperatorLogo: Sms
    {

        #region =============================================== Constants =====================================================
           

        /// <summary>
        /// Operator logo destination port
        /// </summary>
        private const int OperatorLogoDestinationPort = 5506;

        /// <summary>
        /// Operator logo source port
        /// </summary>
        private const int OperatorLogoSourcePort = 0;


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
        /// <param name="bitmap"></param>
        /// <param name="mobileCountryCode"></param>
        /// <param name="mobileNetworkCode"></param>
        public OperatorLogo(Bitmap bitmap, string mobileCountryCode, string mobileNetworkCode)
            : base()
        {
            this.SourcePort = OperatorLogoSourcePort;
            this.DestinationPort = OperatorLogoDestinationPort;
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            this.userDataHeaderIndicator = 1;
            
            this.bitmap = BitmapToOperatorLogo(bitmap);
            this.MobileCountryCode = mobileCountryCode;
            this.MobileNetworkCode = mobileNetworkCode;
        }

       
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operatorLogo"></param>
        /// <param name="mobileCountryCode"></param>
        /// <param name="mobileNetworkCode"></param>
        public OperatorLogo(byte[] operatorLogo, string mobileCountryCode, string mobileNetworkCode)
            : base()
        {
            this.SourcePort = OperatorLogoSourcePort;
            this.DestinationPort = OperatorLogoDestinationPort;
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            this.userDataHeaderIndicator = 1;

            if (operatorLogo == null)
            {
                throw new ArgumentException("OperatorLogo is blank");
            }
            int num = 0;
            this.infoField = operatorLogo[num++];
            this.width = operatorLogo[num++];
            this.height = operatorLogo[num++];
            this.grayscales = operatorLogo[num++];
            this.dataStart = num;
            this.dataLen = operatorLogo.Length - num;
            this.bitmap = new byte[operatorLogo.Length];
            operatorLogo.CopyTo(this.bitmap, 0);
            this.MobileCountryCode = mobileCountryCode;
            this.MobileNetworkCode = mobileNetworkCode;
        }

        #endregion ============================================================================================================

        #region ============================================== Private Static Method ==========================================

        /// <summary>
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private static byte[] BitmapToOperatorLogo(Bitmap bitmap)
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
        /// <param name="OperatorLogo"></param>
        /// <returns></returns>
        private static Bitmap OperatorLogoToBitmap(byte[] OperatorLogo)
        {
            if (OperatorLogo == null)
            {
                return null;
            }
            int num = 0;
            byte num1 = OperatorLogo[num++];
            byte width = OperatorLogo[num++];
            byte height = OperatorLogo[num++];
            byte num9 = OperatorLogo[num++];
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
                        num5 = OperatorLogo[num++];
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


        /// <summary>
        /// Creates the operator logo.
        /// </summary>
        /// <param name="otaBitmap">The ota bitmap.</param>
        /// <param name="mobileCountryCode">The mobile country code.</param>
        /// <param name="mobileNetworkCode">The mobile network code.</param>
        /// <returns></returns>
        public static byte[] CreateOperatorLogo(OperatorLogo otaBitmap, string mobileCountryCode, string mobileNetworkCode)
        {
            if (otaBitmap == null)
            {
                throw new ArgumentNullException("otaBitmap");
            }
            if ((otaBitmap.Width > 0x48) || (otaBitmap.Height > 14))
            {
                string[] strArray = new string[] { "Bitmaps used as operator logos must not be larger than ", 0x48.ToString(), "x", 14.ToString(), " pixels." };
                throw new ArgumentException(string.Concat(strArray));
            }
            if (mobileCountryCode.Length != 3)
            {
                throw new ArgumentException("mobileCountryCode must be 3 digits long.", "mobileCountryCode");
            }
            if (mobileNetworkCode.Length != 2)
            {
                throw new ArgumentException("mobileNetworkCode must be 2 digits long.", "mobileNetworkCode");
            }
            byte num = 0x30;
            byte[] buffer = GatewayHelper.HexToInt(GatewayHelper.EncodeSemiOctets(mobileCountryCode.PadRight(4, 'F')));
            byte[] buffer2 = GatewayHelper.HexToInt(GatewayHelper.EncodeSemiOctets(mobileNetworkCode.PadRight(2, 'F')));
            int num2 = ((1 + buffer.Length) + buffer2.Length) + 1;
            int index = 0;
            byte[] array = new byte[num2];
            array[index++] = num;
            buffer.CopyTo(array, index);
            index += buffer.Length;
            buffer2.CopyTo(array, index);
            index += buffer2.Length;
            array[index++] = 10;
            byte[] buffer4 = otaBitmap.ToByteArray();
            byte[] buffer5 = new byte[array.Length + buffer4.Length];
            array.CopyTo(buffer5, 0);
            buffer4.CopyTo(buffer5, index);
            return buffer5;
        }

       

        #endregion ============================================================================================================

        #region ============================================== Public Static Method ===========================================

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static explicit operator Bitmap(OperatorLogo b)
        {
            return b.ToBitmap();
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static explicit operator OperatorLogo(byte[] b)
        {
            return new OperatorLogo(b, string.Empty, string.Empty);
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static explicit operator OperatorLogo(Bitmap b)
        {
            return new OperatorLogo(b, string.Empty, string.Empty);
        }

        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static implicit operator byte[](OperatorLogo b)
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
            return OperatorLogoToBitmap(this.bitmap);
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

        /// <summary>
        /// Gets or sets the mobile country code.
        /// </summary>
        /// <value>The mobile country code.</value>
        public string MobileCountryCode 
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mobile network code.
        /// </summary>
        /// <value>The mobile network code.</value>
        public string MobileNetworkCode 
        {
            get;
            set;
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
            //return ToByteArray();
            return CreateOperatorLogo(this, this.MobileCountryCode, this.MobileNetworkCode);
        }

        #endregion ============================================================================================================


        #region ============== Factory method   ===============================================================================


        /// <summary>
        /// Static factory to create the OperatorLogo instance
        /// </summary>
        /// <param name="bitmap">Bitmap object</param>
        /// <param name="mobileCountryCode">The mobile country code.</param>
        /// <param name="mobileNetworkCode">The mobile network code.</param>
        /// <returns>
        /// A new instance of the OperatorLogo object
        /// </returns>
        public static OperatorLogo NewInstance(Bitmap bitmap, string mobileCountryCode, string mobileNetworkCode)
        {
            return new OperatorLogo(bitmap, mobileCountryCode, mobileNetworkCode);
        }

        /// <summary>
        /// Static factory to create the OperatorLogo instance
        /// </summary>
        /// <param name="OperatorLogo">Bitmap byte array</param>
        /// <param name="mobileCountryCode">The mobile country code.</param>
        /// <param name="mobileNetworkCode">The mobile network code.</param>
        /// <returns>
        /// A new instance of the OperatorLogo object
        /// </returns>
        public static OperatorLogo NewInstance(byte[] OperatorLogo, string mobileCountryCode, string mobileNetworkCode)
        {
            return new OperatorLogo(OperatorLogo, mobileCountryCode, mobileNetworkCode);
        }

        #endregion ============================================================================================================
    }
}

