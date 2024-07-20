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
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Collections;

using MessagingToolkit.Core.Helper;
using MessagingToolkit.Pdu;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Picture SMS
    /// </summary>
    public class PictureSms: Sms
    {
        #region =============================================== Constants =====================================================

        /// <summary>
        /// Destination port
        /// </summary>
        protected const int PictureSmsDestinationPort = 5514;

        /// <summary>
        /// Source port
        /// </summary>
        protected const int PictureSmsSourcePort = 0;

        #endregion ============================================================================================================

        #region =============================================== Private Variables =====================================================


        protected byte[] bitmap = null;
        protected string message = string.Empty;

        #endregion ============================================================================================================


        #region =============================================== Constructor =====================================================


        /// <summary>
        /// Initializes a new instance of the <see cref="PictureSms"/> class.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="message">The message.</param>
        public PictureSms(Bitmap bitmap, string message)
            : base()
        {
            this.SourcePort = PictureSmsSourcePort;
            this.DestinationPort = PictureSmsDestinationPort;
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            //this.DcsMessageClass = MessageClasses.Sim;
            this.userDataHeaderIndicator = 1;
            
            if (bitmap != null)
                this.bitmap = BitmapToOtaBitmap(bitmap);
            this.message = message;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="message">The message.</param>
        public PictureSms(byte[] data, string message)
            : base()
        {
            this.SourcePort = PictureSmsSourcePort;
            this.DestinationPort = PictureSmsDestinationPort;
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            //this.DcsMessageClass = MessageClasses.Sim;
            this.userDataHeaderIndicator = 1;

            this.bitmap = new byte[data.Length];
            data.CopyTo(this.bitmap, 0);
            this.message = message;          
        }

        #endregion ============================================================================================================



        #region =============================================== Factory Methods ================================================

        /// <summary>
        /// News the instance.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static PictureSms NewInstance(Bitmap bitmap, string message)
        {
            return new PictureSms(bitmap, message);
        }


        /// <summary>
        /// News the instance.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public static PictureSms NewInstance(byte[] picture, string message)
        {
            return new PictureSms(picture, message);
        }

        #endregion ============================================================================================================


        #region =============================================== Public Static Methods ================================================

        /// <summary>
        /// Creates the picture SMS.
        /// </summary>
        /// <param name="pictureSms">Picture SMS</param>
        /// <returns></returns>
        public static byte[] CreatePictureSms(PictureSms pictureSms)
        {
            if (pictureSms.bitmap == null && string.IsNullOrEmpty(pictureSms.message))
            {
                throw new ArgumentNullException("Image and message are not provided");
            }
          
            MemoryStream stream = new MemoryStream();
            stream.WriteByte(0x30);  // version

            if (!string.IsNullOrEmpty(pictureSms.message))
            {
                stream.WriteByte(0x00);
                byte[] bytes = BitConverter.GetBytes(pictureSms.message.Length);
                stream.WriteByte((byte)bytes[1]);
                stream.WriteByte((byte)bytes[0]);
                bytes = PduUtils.Encode8bitUserData(pictureSms.message);
                stream.Write(bytes, 0, bytes.Length);
            }

            if (pictureSms.bitmap != null)
            {
                stream.WriteByte(0x02);

                /*
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Jpeg);               
                byte[] bmpBytes = ms.GetBuffer();               
                ms.Close();
                */
                
                byte[] bytes =  BitConverter.GetBytes(pictureSms.bitmap.Length);
                stream.WriteByte((byte)bytes[1]);
                stream.WriteByte((byte)bytes[0]);
                stream.Write(pictureSms.bitmap, 0, pictureSms.bitmap.Length);

            }
            byte[] messageBytes = stream.ToArray();
            stream.Close();
            return messageBytes;
        }

        #endregion ============================================================================================================


        #region =============================================== Private Static Methods ================================================

        /// <summary>
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        protected static byte[] BitmapToOtaBitmap(Bitmap bitmap)
        {
            byte[] buffer = null;
            byte[] array = null;
            if (bitmap != null)
            {               
                if (!GatewayHelper.IsBlackAndWhite(bitmap))
                {
                    bitmap = GatewayHelper.ConvertBlackAndWhite(bitmap);
                }
                
                if (((bitmap.Height < 1) || (bitmap.Width < 1)) )
                {
                    throw new ArgumentException("Invalid bitmap dimensions. Maximum size is 255x255, minimum size is 1x1 pixels.");
                }

                if (bitmap.Height > 0xff || bitmap.Width > 0xff)
                {
                    bitmap = GatewayHelper.ResizeImage(bitmap, 255, 255);
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

        #endregion ============================================================================================================


        /// <summary>
        /// Encode the vCard content
        /// </summary>
        /// <returns>Encoded vCard content</returns>
        internal override byte[] GetPdu()
        {
            return CreatePictureSms(this);
        }

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
            return str;
        }

    }
}
