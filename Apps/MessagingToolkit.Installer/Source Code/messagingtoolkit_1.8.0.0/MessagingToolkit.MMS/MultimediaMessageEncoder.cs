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
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace MessagingToolkit.MMS
{

    /// <summary>
    /// The MultimediaMessageEncoder class encodes Multimedia Message object (MultimediaMessage)
    /// into an array of bytes according to the specification WAP-209-MMSEncapsulation
    /// (WAP Forum).
    /// </summary>
    public class MultimediaMessageEncoder : MultimediaMessageConstants
    {

        private MultimediaMessage m_Message;
        private bool m_bMessageAvailable;
        private bool m_bMultipartRelated;
        private bool m_bMessageEcoded;
        private MemoryStream m_Out;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaMessageEncoder"/> class.
        /// </summary>
        public MultimediaMessageEncoder()
            : base()
        {
            Reset();
        }

        /// <summary>
        /// Resets the Decoder object.
        /// </summary>
        public virtual void Reset()
        {
            m_Message = null;
            m_bMultipartRelated = false;
            m_bMessageAvailable = false;
            m_bMessageEcoded = false;
            m_Out = null;
        }

        /// <summary>
        /// Sets the Multimedia Message to be encoded.
        /// </summary>
        /// <param name="msg">Multimedia message.</param>
        public virtual void SetMessage(MultimediaMessage msg)
        {
            m_Message = msg;
            m_bMessageAvailable = true;
        }

        /// <summary>
        /// Retrieve the buffer of byte representing the encoded Multimedia Message.
        /// This method has to be called after the calling to encodeMessasge()
        /// </summary>
        /// <returns>
        /// the array of bytes representing the Multmedia Message
        /// </returns>
        public virtual byte[] GetMessage()
        {
            if (m_bMessageEcoded)
            {
                return m_Out.ToArray();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Encode known content type assignments.
        /// List of the content type assignments can be found from WAP-203-WSP, Table 40
        /// This version is compliant with Approved version 4-May-2000
        /// </summary>
        /// <param name="sContentType">Type of the s content.</param>
        /// <returns>assigned number</returns>
        private byte EncodeContentType(string sContentType)
        {
            if (sContentType.ToUpper().Equals("*/*".ToUpper()))
                return (0x00);
            else if (sContentType.ToUpper().Equals("text/*".ToUpper()))
                return (0x01);
            else if (sContentType.ToUpper().Equals("text/html".ToUpper()))
                return (0x02);
            else if (sContentType.ToUpper().Equals("text/plain".ToUpper()))
                return (0x03);
            else if (sContentType.ToUpper().Equals("text/x-hdml".ToUpper()))
                return (0x04);
            else if (sContentType.ToUpper().Equals("text/x-ttml".ToUpper()))
                return (0x05);
            else if (sContentType.ToUpper().Equals("text/x-vCalendar".ToUpper()))
                return (0x06);
            else if (sContentType.ToUpper().Equals("text/x-vCard".ToUpper()))
                return (0x07);
            else if (sContentType.ToUpper().Equals("text/vnd.wap.wml".ToUpper()))
                return (0x08);
            else if (sContentType.ToUpper().Equals("text/vnd.wap.wmlscript".ToUpper()))
                return (0x09);
            else if (sContentType.ToUpper().Equals("text/vnd.wap.channel".ToUpper()))
                return (0x0A);
            else if (sContentType.ToUpper().Equals("multipart/*".ToUpper()))
                return (0x0B);
            else if (sContentType.ToUpper().Equals("multipart/mixed".ToUpper()))
                return (0x0C);
            else if (sContentType.ToUpper().Equals("multipart/form-data".ToUpper()))
                return (0x0D);
            else if (sContentType.ToUpper().Equals("multipart/byteranges".ToUpper()))
                return (0x0E);
            else if (sContentType.ToUpper().Equals("multipart/alternative".ToUpper()))
                return (0x0F);
            else if (sContentType.ToUpper().Equals("application/*".ToUpper()))
                return (0x10);
            else if (sContentType.ToUpper().Equals("application/java-vm".ToUpper()))
                return (0x11);
            else if (sContentType.ToUpper().Equals("application/x-www-form-urlencoded".ToUpper()))
                return (0x12);
            else if (sContentType.ToUpper().Equals("application/x-hdmlc".ToUpper()))
                return (0x13);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.wmlc".ToUpper()))
                return (0x14);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.wmlscriptc".ToUpper()))
                return (0x15);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.channelc".ToUpper()))
                return (0x16);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.uaprof".ToUpper()))
                return (0x17);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.wtls-ca-certificate".ToUpper()))
                return (0x18);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.wtls-user-certificate".ToUpper()))
                return (0x19);
            else if (sContentType.ToUpper().Equals("application/x-x509-ca-cert".ToUpper()))
                return (0x1A);
            else if (sContentType.ToUpper().Equals("application/x-x509-user-cert".ToUpper()))
                return (0x1B);
            else if (sContentType.ToUpper().Equals("image/*".ToUpper()))
                return (0x1C);
            else if (sContentType.ToUpper().Equals("image/gif".ToUpper()))
                return (0x1D);
            else if (sContentType.ToUpper().Equals("image/jpeg".ToUpper()))
                return (0x1E);
            else if (sContentType.ToUpper().Equals("image/tiff".ToUpper()))
                return (0x1F);
            else if (sContentType.ToUpper().Equals("image/png".ToUpper()))
                return (0x20);
            else if (sContentType.ToUpper().Equals("image/vnd.wap.wbmp".ToUpper()))
                return (0x21);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.multipart.*".ToUpper()))
                return (0x22);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.multipart.mixed".ToUpper()))
                return (0x23);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.multipart.form-data".ToUpper()))
                return (0x24);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.multipart.byteranges".ToUpper()))
                return (0x25);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.multipart.alternative".ToUpper()))
                return (0x26);
            else if (sContentType.ToUpper().Equals("application/xml".ToUpper()))
                return (0x27);
            else if (sContentType.ToUpper().Equals("text/xml".ToUpper()))
                return (0x28);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.wbxml".ToUpper()))
                return (0x29);
            else if (sContentType.ToUpper().Equals("application/x-x968-cross-cert".ToUpper()))
                return (0x2A);
            else if (sContentType.ToUpper().Equals("application/x-x968-ca-cert".ToUpper()))
                return (0x2B);
            else if (sContentType.ToUpper().Equals("application/x-x968-user-cert".ToUpper()))
                return (0x2C);
            else if (sContentType.ToUpper().Equals("text/vnd.wap.si".ToUpper()))
                return (0x2D);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.sic".ToUpper()))
                return (0x2E);
            else if (sContentType.ToUpper().Equals("text/vnd.wap.sl".ToUpper()))
                return (0x2F);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.slc".ToUpper()))
                return (0x30);
            else if (sContentType.ToUpper().Equals("text/vnd.wap.co".ToUpper()))
                return (0x31);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.coc".ToUpper()))
                return (0x32);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.multipart.related".ToUpper()))
                return (0x33);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.sia".ToUpper()))
                return (0x34);
            else if (sContentType.ToUpper().Equals("text/vnd.wap.connectivity-xml".ToUpper()))
                return (0x35);
            else if (sContentType.ToUpper().Equals("application/vnd.wap.connectivity-wbxml".ToUpper()))
                return (0x36);
            else
                return 0;
        }

      
        private void WriteValueLength(long value)
        {
            if (value <= 30)
                m_Out.WriteByte((byte)value);
            else
            {
                m_Out.WriteByte((byte)31);
                int[] data = EncodeUintvarNumber(value);
                int numValue;
                for (int i = 1; i <= data[0]; i++)
                {
                    numValue = data[i];
                    m_Out.WriteByte((byte)numValue);
                }
            }
        }

        private void WriteUintVar(long value)
        {
            int[] data = EncodeUintvarNumber(value);
            int numValue;
            for (int i = 1; i <= data[0]; i++)
            {
                numValue = data[i];
                m_Out.WriteByte((byte)numValue);
            }
        }

        /// <summary>
        /// Encodes the Multimedia Message set by calling SetMessage(MultimediaMessage msg)
        /// </summary>
        public virtual void EncodeMessage()
        {
            int numValue;
            string strValue;
            m_bMessageEcoded = false;
            m_bMultipartRelated = false;

            if (!m_bMessageAvailable)
                throw new MultimediaMessageEncoderException("No Multimedia Messages set in the encoder");

            try
            {
                m_Out = new MemoryStream();

                if (!m_Message.MessageTypeAvailable)
                {
                    m_Out.Close();
                    throw new MultimediaMessageEncoderException("Invalid Multimedia Message format.");
                }

                byte nMessageType = m_Message.MessageType;

                switch (nMessageType)
                {

                    default:
                        {
                            m_Out.Close();
                            throw new MultimediaMessageEncoderException("Invalid Multimedia Message format.");
                        }
                    case MultimediaMessageConstants.MessageTypeMDeliveryInd:  // ---------------------------- m-delivery-ind 

                        // ------------------- MESSAGE TYPE -------- 
                        m_Out.WriteByte((MultimediaMessageConstants.FieldNameMessageType + 0x80));
                        m_Out.WriteByte(nMessageType);

                        // ------------------- MESSAGE ID ------ 
                        if (m_Message.MessageIdAvailable)
                        {
                            m_Out.WriteByte((MultimediaMessageConstants.FieldNameMessageId + 0x80));
                            byte[] byteArray;
                            byteArray = MultimediaMessageHelper.GetBytes(m_Message.MessageId);
                            m_Out.Write(byteArray, 0, byteArray.Length);
                            m_Out.WriteByte((byte)0x00);
                        }
                        else
                        {
                            m_Out.Close();
                            throw new MultimediaMessageEncoderException("The field Message-ID of the Multimedia Message is null");
                        }

                        // ------------------- VERSION ------------- 
                        m_Out.WriteByte((MultimediaMessageConstants.FieldNameMmsVersion + 0x80));
                        if (!m_Message.VersionAvailable)
                        {
                            numValue = MultimediaMessageConstants.MmsVersion10;
                        }
                        else
                        {
                            numValue = m_Message.Version;
                        }
                        m_Out.WriteByte((byte)numValue);

                        // ------------------- DATE ---------------- 
                        if (m_Message.DateAvailable)
                        {
                            long secs = (m_Message.Date).Ticks / 1000;
                            int[] data = EncodeMultiByteNumber(secs);
                            if (data == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("An error occurred encoding the sending date of the Multimedia Message");
                            }
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameDate + 0x80));
                            int nCount = data[0];
                            m_Out.WriteByte((byte)nCount);
                            for (int i = 1; i <= nCount; i++)
                            {
                                m_Out.WriteByte((byte)data[i]);
                            }
                        }

                        // ------------------- TO ------------------ 
                        if (m_Message.ToAvailable)
                        {
                            List<MultimediaMessageAddress> sAddress = m_Message.To;
                            int nAddressCount = sAddress.Count;
                            if (sAddress == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("The field TO of the Multimedia Message is set to null.");
                            }
                            for (int i = 0; i < nAddressCount; i++)
                            {
                                strValue = ((MultimediaMessageAddress)sAddress[i]).FullAddress;
                                if (strValue != null)
                                {
                                    m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameTo + 0x80));
                                    byte[] byteArray;
                                    byteArray = MultimediaMessageHelper.GetBytes(strValue);
                                    m_Out.Write(byteArray, 0, byteArray.Length);
                                    m_Out.WriteByte((byte)0x00);
                                }
                            }
                        }
                        else
                        {
                            m_Out.Close();
                            throw new MultimediaMessageEncoderException("No recipient specified in the Multimedia Message.");
                        }

                        // ------------------- MESSAGE-STATUS ---------------- 
                        if (m_Message.StatusAvailable)
                        {
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameStatus + 0x80));
                            m_Out.WriteByte((byte)m_Message.MessageStatus);
                        }
                        else
                        {
                            m_Out.Close();
                            throw new MultimediaMessageEncoderException("The field Message-ID of the Multimedia Message is null");
                        }

                        break;


                    case MultimediaMessageConstants.MessageTypeMSendReq:  // ---------------------------- m-send-req 
                    case MultimediaMessageConstants.MessageTypeMReceived:

                        // ------------------- MESSAGE TYPE -------- 
                        m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameMessageType + 0x80));
                        m_Out.WriteByte((byte)nMessageType);

                        // ------------------- TRANSACTION ID ------ 
                        if (m_Message.TransactionIdAvailable)
                        {
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameTransactionId + 0x80));
                            byte[] byteArray;
                            byteArray = MultimediaMessageHelper.GetBytes(m_Message.TransactionId);
                            m_Out.Write(byteArray, 0, byteArray.Length);
                            m_Out.WriteByte((byte)0x00);
                        }

                        // ------------------- VERSION ------------- 
                        m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameMmsVersion + 0x80));
                        if (!m_Message.VersionAvailable)
                        {
                            numValue = MultimediaMessageConstants.MmsVersion10;
                        }
                        else
                        {
                            numValue = m_Message.Version;
                        }
                        m_Out.WriteByte((byte)numValue);

                        // ------------------- DATE ---------------- 
                        if (m_Message.DateAvailable)
                        {
                            long secs = (m_Message.Date).Ticks / 1000;
                            int[] data = EncodeMultiByteNumber(secs);
                            if (data == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("An error occurred encoding the sending date of the Multimedia Message");
                            }
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameDate + 0x80));
                            int nCount = data[0];
                            m_Out.WriteByte((byte)nCount);
                            for (int i = 1; i <= nCount; i++)
                            {
                                m_Out.WriteByte((byte)data[i]);
                            }
                        }


                        // ------------------- FROM ---------------- 
                        if (m_Message.FromAvailable)
                        {
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameFrom + 0x80));

                            strValue = (m_Message.GetFrom()).FullAddress;
                            if (strValue == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("The field from is assigned to null");
                            }

                            // Value-length 
                            WriteValueLength(strValue.Length + 2);
                            // Address-present-token 
                            m_Out.WriteByte((byte)0x80);

                            // Encoded-string-value 
                            byte[] byteArray;
                            byteArray = MultimediaMessageHelper.GetBytes(strValue);
                            m_Out.Write(byteArray, 0, byteArray.Length);
                            m_Out.WriteByte((byte)0x00);
                        }
                        else
                        {
                            // Value-length 
                            m_Out.WriteByte((byte)1);
                            m_Out.WriteByte((byte)0x81);
                        }

                        // ------------------- TO ------------------ 
                        if (m_Message.ToAvailable)
                        {
                            List<MultimediaMessageAddress> sAddress = m_Message.To;
                            int nAddressCount = sAddress.Count;
                            if (sAddress == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("The field TO of the Multimedia Message is set to null.");
                            }
                            for (int i = 0; i < nAddressCount; i++)
                            {
                                strValue = ((MultimediaMessageAddress)sAddress[i]).FullAddress;
                                if (strValue != null)
                                {
                                    m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameTo + 0x80));
                                    byte[] byteArray;
                                    byteArray = MultimediaMessageHelper.GetBytes(strValue);
                                    m_Out.Write(byteArray, 0, byteArray.Length);
                                    m_Out.WriteByte((byte)0x00);
                                }
                            }
                        }

                        // ------------------- CC ------------------ 
                        if (m_Message.CcAvailable)
                        {
                            List<MultimediaMessageAddress> sAddress = m_Message.Cc;
                            int nAddressCount = sAddress.Count;

                            if (sAddress == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("The field CC of the Multimedia Message is set to null.");
                            }

                            for (int i = 0; i < nAddressCount; i++)
                            {
                                strValue = ((MultimediaMessageAddress)sAddress[i]).FullAddress;
                                if (strValue != null)
                                {
                                    m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameCc + 0x80));
                                    byte[] byteArray;
                                    byteArray = MultimediaMessageHelper.GetBytes(strValue);
                                    m_Out.Write(byteArray, 0, byteArray.Length);
                                    m_Out.WriteByte((byte)0x00);
                                }
                            }
                        }

                        // ------------------- BCC ------------------ 
                        if (m_Message.BccAvailable)
                        {
                            List<MultimediaMessageAddress> sAddress = m_Message.Bcc;
                            int nAddressCount = sAddress.Count;

                            if (sAddress == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("The field BCC of the Multimedia Message is set to null.");
                            }

                            for (int i = 0; i < nAddressCount; i++)
                            {
                                strValue = ((MultimediaMessageAddress)sAddress[i]).FullAddress;
                                if (strValue != null)
                                {
                                    m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameBcc + 0x80));
                                    byte[] byteArray;
                                    byteArray = MultimediaMessageHelper.GetBytes(strValue);
                                    m_Out.Write(byteArray, 0, byteArray.Length);
                                    m_Out.WriteByte((byte)0x00);
                                }
                            }
                        }

                        if (!(m_Message.ToAvailable || m_Message.CcAvailable || m_Message.BccAvailable))
                        {
                            m_Out.Close();
                            throw new MultimediaMessageEncoderException("No recipient specified in the Multimedia Message.");
                        }

                        // ---------------- SUBJECT  -------------- 
                        if (m_Message.SubjectAvailable)
                        {
                            bool isUnicode = MultimediaMessageHelper.IsUnicode(m_Message.Subject);   

                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameSubject + 0x80));
                            byte[] byteArray;
                            byteArray = MultimediaMessageHelper.GetBytes(m_Message.Subject);
                            
                            if (isUnicode)
                            {
                                //if (byteArray.Length > 30)
                                //m_Out.WriteByte((byte)0x1F);
                                m_Out.WriteByte((byte)(byteArray.Length + 3));
                                m_Out.WriteByte((byte)0xEA); // UTF-8  
                                //m_Out.WriteByte((byte)0x7A);  
                                m_Out.WriteByte((byte)0x7F); 
                                
                            }
                            m_Out.Write(byteArray, 0, byteArray.Length);
                            m_Out.WriteByte((byte)0x00);                               
                        }

                        // ------------------- DELIVERY-REPORT ---------------- 
                        if (m_Message.DeliveryReportAvailable)
                        {
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameDeliveryReport + 0x80));
                            if (m_Message.DeliveryReport == true)
                                m_Out.WriteByte((byte)0x80);
                            else
                                m_Out.WriteByte((byte)0x81);
                        }

                        // ------------------- SENDER-VISIBILITY ---------------- 
                        if (m_Message.SenderVisibilityAvailable)
                        {
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameSenderVisibility + 0x80));
                            m_Out.WriteByte((byte)m_Message.SenderVisibility);
                        }

                        // ------------------- READ-REPLY ---------------- 
                        if (m_Message.ReadReplyAvailable)
                        {
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldReadReply + 0x80));
                            if (m_Message.ReadReply == true)
                                m_Out.WriteByte((byte)0x80);
                            else
                                m_Out.WriteByte((byte)0x81);
                        }

                        // ---------------- MESSAGE CLASS --------- 
                        if (m_Message.MessageClassAvailable)
                        {
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameMessageClass + 0x80));
                            m_Out.WriteByte((byte)m_Message.MessageClass);
                        }

                        // ---------------- EXPIRY ---------------- 
                        if (m_Message.ExpiryAvailable)
                        {
                            long secs = (m_Message.Expiry).Ticks / 1000;
                            int[] data = EncodeMultiByteNumber(secs);
                            if (data == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("An error occurred encoding the EXPIRY field of the Multimedia Message. The field is set to null");
                            }
                            int nCount = data[0];

                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameExpiry + 0x80));

                            // Value-length 
                            WriteValueLength(nCount + 2);

                            if (m_Message.ExpiryAbsolute)
                            {
                                // Absolute-token 
                                m_Out.WriteByte((byte)0x80);
                            }
                            else
                            {
                                // Relative-token 
                                m_Out.WriteByte((byte)0x81);
                            }

                            // Date-value or Delta-seconds-value 
                            for (int i = 0; i <= nCount; i++)
                            {
                                m_Out.WriteByte((byte)data[i]);
                            }
                        }

                        // ---------------- DELIVERY TIME ---------------- 
                        if (m_Message.DeliveryTimeAvailable)
                        {
                            long secs = (m_Message.DeliveryTime).Ticks / 1000;
                            int[] data = EncodeMultiByteNumber(secs);
                            if (data == null)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("The field DELIVERY TIME of the Multimedia Message is set to null.");
                            }
                            int nCount = data[0];

                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameDeliveryTime + 0x80));

                            // Value-length 
                            WriteValueLength(nCount + 2);

                            if (m_Message.DeliveryTimeAbsolute)
                            {
                                // Absolute-token 
                                m_Out.WriteByte((byte)0x80);
                            }
                            else
                            {
                                // Relative-token 
                                m_Out.WriteByte((byte)0x81);
                            }

                            // Date-value or Delta-seconds-value 
                            for (int i = 0; i <= nCount; i++)
                            {
                                m_Out.WriteByte((byte)data[i]);
                            }
                        }

                        // ---------------- PRIORITY ---------------- 
                        if (m_Message.PriorityAvailable)
                        {
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNamePriority + 0x80));
                            m_Out.WriteByte((byte)m_Message.Priority);
                        }

                        // ---------------- CONTENT TYPE ---------------- 
                        if (m_Message.ContentTypeAvailable)
                        {
                            m_bMultipartRelated = false;
                            m_Out.WriteByte((byte)(MultimediaMessageConstants.FieldNameContentType + 0x80));

                            byte ctype = EncodeContentType(m_Message.ContentType);

                            if (ctype == 0x33)
                            {
                                // application/vnd.wap.multipart.related 
                                m_bMultipartRelated = true;
                                int valueLength = 1;
                                string mprt = m_Message.MultipartRelatedType;
                                if (string.IsNullOrEmpty(mprt))
                                {
                                    mprt = MultimediaMessageConstants.ContentTypeApplicationSmil;
                                }
                                valueLength += mprt.Length + 2;
                                string start = m_Message.PresentationId;
                                if (string.IsNullOrEmpty(start))
                                {
                                    start = string.Empty;
                                }
                                valueLength += start.Length + 2;
                                // Value-length 
                                WriteValueLength(valueLength);
                                // Well-known-media 
                                m_Out.WriteByte((byte)(0x33 + 0x80));
                                // Parameters 
                                // Type 
                                m_Out.WriteByte((byte)(0x09 + 0x80));
                                byte[] byteArray;
                                byteArray = MultimediaMessageHelper.GetBytes(mprt);
                                m_Out.Write(byteArray, 0, byteArray.Length);
                                m_Out.WriteByte((byte)0x00);
                                // Start 
                                m_Out.WriteByte((byte)(0x0A + 0x80));
                                byteArray = MultimediaMessageHelper.GetBytes(start);
                                m_Out.Write(byteArray, 0, byteArray.Length);
                                m_Out.WriteByte((byte)0x00);
                            }
                            else
                            {
                                if (ctype > 0x00)
                                    m_Out.WriteByte((byte)(ctype + 0x80));
                                else
                                {
                                    byte[] byteArray;
                                    byteArray = MultimediaMessageHelper.GetBytes(m_Message.ContentType);
                                    m_Out.Write(byteArray, 0, byteArray.Length);
                                    m_Out.WriteByte((byte)0x00);
                                }
                            }
                        }
                        else
                        {
                            m_Out.Close();
                            throw new MultimediaMessageEncoderException("The field CONTENT TYPE of the Multimedia Message is not specified.");
                        }

                        // -------------------------- BODY ------------- 
                        int nPartsCount = m_Message.NumContents;
                        m_Out.WriteByte((byte)nPartsCount);
                        MultimediaMessageContent part = null;
                        for (int i = 0; i < nPartsCount; i++)
                        {
                            part = m_Message.GetContent(i);
                            bool bRetVal = EncodePart(part);
                            if (!bRetVal)
                            {
                                m_Out.Close();
                                throw new MultimediaMessageEncoderException("The entry having Content-id = " + part.ContentId + " cannot be encoded.");
                            }
                        }

                        break;
                }

                m_Out.Close();
                m_bMessageEcoded = true;
            }
            catch (IOException e)
            {
                throw new MultimediaMessageEncoderException("An IO error occurred encoding the Multimedia Message.");
            }
        }

        private int[] EncodeMultiByteNumber(long lData)
        {

            int[] data = new int[32];
            long lDivider = 1L;
            int nSize = 0;
            long lNumber = lData;

            for (int i = 0; i < 32; i++)
                data[i] = 0;

            for (int i = 4; i >= 0; i--)
            {
                lDivider = 1L;
                for (int j = 0; j < i; j++)
                    lDivider *= 256L;

                int q = (int)(lNumber / lDivider);

                if (q != 0 || nSize != 0)
                {
                    int r = (int)(lNumber % lDivider);
                    data[nSize + 1] = q;
                    lNumber = r;
                    nSize++;
                }
            }

            data[0] = nSize;
            return data;
        }

        private int[] EncodeUintvarNumber(long lData)
        {
            int[] data = new int[32];
            long lDivider = 1L;
            int nSize = 0;
            long lNumber = lData;

            for (int i = 0; i < 32; i++)
                data[i] = 0;

            for (int i = 4; i >= 0; i--)
            {
                lDivider = 1L;
                for (int j = 0; j < i; j++)
                    lDivider *= 128L;

                int q = (int)(lNumber / lDivider);
                if (q != 0 || nSize != 0)
                {
                    int r = (int)(lNumber % lDivider);
                    data[nSize + 1] = q;
                    if (i != 0)
                        data[nSize + 1] += 128;
                    lNumber = r;
                    nSize++;
                }
            }

            data[0] = nSize;
            return data;
        }

        private bool EncodePart(MultimediaMessageContent part)
        {

            if (part == null)
                return false;

            int nHeadersLen = 0; // nHeadersLen = nLengthOfContentType + nLengthOfHeaders 
            int nContentType = 0;

            int nLengthOfHeaders = 0;
            int nLengthOfContentType = 0;


            // -------- HeadersLen = ContentType + Headers fields --------- 
            if ((part.ContentId.Length > 0) && (m_bMultipartRelated))
            {
                if (MultimediaMessageHelper.GetBytes(part.ContentId)[0] == '<')
                {
                    nLengthOfHeaders = 2 + (part.ContentId).Length + 1;
                    // 2 = 0xC0 (Content-ID) + 0x22 (quotes) 
                    // 1 = 0x00 (at the end of the contentID) 
                }
                else
                {
                    nLengthOfHeaders = 1 + (part.ContentId).Length + 1;
                    // 1 = 0x8E (Content-Location) 
                    // 1 = 0x00 (end string) 
                }
            }

            // -------- DataLen ------------- 
            long lDataLen = part.Length;

            // -------- ContentType --------- 
            nContentType = EncodeContentType(part.Type) + 128;

            if (nContentType > 0x80)
            {
                // ---------- Well Known Content Types ------------------------------ 
                if (nContentType == 0x83)
                {
                    // text/plain 
                    nLengthOfContentType = 4;
                    // 4 = 0x03 (Value Length)+ 0x83(text/plain) + 0x81 (Charset) + 0x83 (us-ascii code) 

                    nHeadersLen = nLengthOfContentType + nLengthOfHeaders;

                    // write HeadersLen 
                    WriteUintVar(nHeadersLen);

                    // write DataLen 
                    WriteUintVar(lDataLen);

                    // write ContentType 
                    m_Out.WriteByte((byte)0x03); // length of content type 
                    m_Out.WriteByte((byte)nContentType);
                    m_Out.WriteByte((byte)0x81); // charset parameter 

                    if (!MultimediaMessageHelper.IsUnicode(part.ContentAsString))
                        m_Out.WriteByte((byte)0x83);    // us-ascii code 
                    else
                        m_Out.WriteByte((byte)0xEA);    // UTF-8
                }
                else
                {
                    nLengthOfContentType = 1;
                    nHeadersLen = nLengthOfContentType + nLengthOfHeaders;
                    // write HeadersLen 
                    WriteUintVar(nHeadersLen);
                    // write DataLen 
                    WriteUintVar(lDataLen);
                    // write ContentType 
                    m_Out.WriteByte((byte)nContentType);
                }
            }
            else
            {
                // ----------- Don't known Content Type 
                if (part.Type.ToUpper().Equals(MultimediaMessageConstants.ContentTypeApplicationSmil.ToUpper()))
                {
                    nLengthOfContentType = 1 + part.Type.Length + 3;
                    // 1 = 0x13 (Value Length) 
                    // 3 = 0x00 + 0x81 (Charset) + 0x83 (us-ascii code) 

                    nHeadersLen = nLengthOfContentType + nLengthOfHeaders;

                    // write HeadersLen 
                    WriteUintVar(nHeadersLen);
                    // write DataLen 
                    WriteUintVar(lDataLen);

                    // write ContentType 
                    m_Out.WriteByte((byte)0x13); //13 characters, actually part.getType().length()+1+1+1 
                    byte[] byteArray;
                    byteArray = MultimediaMessageHelper.GetBytes(part.Type);
                    m_Out.Write(byteArray, 0, byteArray.Length);
                    m_Out.WriteByte((byte)0x00);
                    m_Out.WriteByte((byte)0x81); // charset parameter 

                    if (!MultimediaMessageHelper.IsUnicode(part.ContentAsString))
                        m_Out.WriteByte((byte)0x83); // ascii-code 
                    else
                        m_Out.WriteByte((byte)0xEA); // UTF-8
                }
                else
                {
                    nLengthOfContentType = part.Type.Length + 1;
                    // 1 = 0x00 

                    nHeadersLen = nLengthOfContentType + nLengthOfHeaders;

                    // write HeadersLen 
                    WriteUintVar(nHeadersLen);
                    // write DataLen 
                    WriteUintVar(lDataLen);
                    // write ContentType 
                    byte[] byteArray;
                    byteArray = MultimediaMessageHelper.GetBytes(part.Type);
                    m_Out.Write(byteArray, 0, byteArray.Length);
                    m_Out.WriteByte((byte)0x00);
                }
            }

            // writes the Content ID or the Content Location 
            if ((part.ContentId.Length > 0) && (m_bMultipartRelated))
            {
                if (MultimediaMessageHelper.GetBytes(part.ContentId)[0] == '<')
                {
                    Console.Out.WriteLine("--->QUOTED!!");
                    m_Out.WriteByte((byte)0xC0);
                    m_Out.WriteByte((byte)0x22);
                    byte[] byteArray;
                    byteArray = MultimediaMessageHelper.GetBytes(part.ContentId);
                    m_Out.Write(byteArray, 0, byteArray.Length);
                    m_Out.WriteByte((byte)0x00);
                }
                else
                {
                    // content id 
                    m_Out.WriteByte((byte)0x8E);
                    byte[] byteArray;
                    byteArray = MultimediaMessageHelper.GetBytes(part.ContentId);
                    m_Out.Write(byteArray, 0, byteArray.Length);
                    m_Out.WriteByte((byte)0x00);
                }
            }

            // ----------- Data -------------- 
            byte[] data = part.GetContent();        
            m_Out.Write(data, 0, data.Length);

            return true;
        }

        /// <summary>
        /// Saves to file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public virtual void SaveToFile(string fileName)
        {
            try
            {
                FileInfo f = new FileInfo(fileName);
                FileStream outputFile = new FileStream(f.FullName, FileMode.Create);
                outputFile.Write(m_Out.ToArray(), 0, m_Out.ToArray().Length);
                outputFile.Close();
            }
            catch (Exception e)
            {
                throw new MultimediaMessageEncoderException(e.Message);
            }
        }
    }
}