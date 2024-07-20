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
using System.Drawing;

using MessagingToolkit.Barcode;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Picture SMS
    /// </summary>
    public class QRCodeSms : PictureSms
    {
        #region =============================================== Constants =====================================================

       

        #endregion ============================================================================================================

        #region =============================================== Private Variables =====================================================


        #endregion ============================================================================================================


        #region =============================================== Constructor =====================================================


        /// <summary>
        /// Initializes a new instance of the <see cref="PictureSms"/> class.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        public QRCodeSms(Bitmap bitmap)
            : base(bitmap, string.Empty)
        {
           
        }

      

        #endregion ============================================================================================================



        #region =============================================== Factory Methods ================================================

        /// <summary>
        /// News the instance.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static QRCodeSms NewInstance(string message, int width,int height)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("Message is not provided");
            }
            BarcodeEncoder encoder = new BarcodeEncoder();
            if (width > 0)
                encoder.Width = width;
            if (height > 0)
                encoder.Height = height;
            Bitmap bitmap = (Bitmap)encoder.Encode(BarcodeFormat.QRCode, message);
            return new QRCodeSms(bitmap);
        }


       
        #endregion ============================================================================================================


        #region =============================================== Public Static Methods ================================================

       
        #endregion ============================================================================================================


        #region =============================================== Private Static Methods ================================================


        #endregion ============================================================================================================

    }
}

