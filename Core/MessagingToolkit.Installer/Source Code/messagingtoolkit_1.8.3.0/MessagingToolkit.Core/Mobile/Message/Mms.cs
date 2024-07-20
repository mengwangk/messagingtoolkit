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
using System.IO;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Properties;
using MessagingToolkit.MMS;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.Core.Service;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Multimedia Message (MMS) class
    /// </summary>
    [global::System.Serializable]
    public class Mms : BaseMms, IMessage
    {        
        #region ========================= Private Constants   ====================================================

        /// <summary>
        /// Default SMIL layout
        /// </summary>
        private const string DefaultSmilLayout = @"<smil><head><meta name=""title"" content=""{0}"" /><meta name=""author"" content=""MessagingToolkit"" /><layout><root-layout width=""160"" height=""120""/><region id=""Image"" width=""100%"" height=""80"" left=""0"" top=""0"" /><region id=""Video"" width=""100%"" height=""80"" left=""0"" top=""0"" /><region id=""Text"" width=""100%"" height=""40"" left=""0"" top=""80"" /></layout></head><body>{1}</body></smil>";

        #endregion ========================= End Private Constants   ==============================================


        #region ========================= Private Variables   ====================================================

        /// <summary>
        /// List of MMS slides
        /// </summary>
        private List<MmsSlide> slides;
     

        #endregion ========================= End Private Variable   ==============================================

        #region ========================= Protected Constructor===================================================

        /// <summary>
        /// Constructor
        /// </summary>
        protected Mms():base()
        {
            // Initialize the lists
            slides = new List<MmsSlide>(1);

            // Set custom SMIL layout to the default 
            CustomSmilLayout = DefaultSmilLayout;

            // Default to false
            IsSmilContentComposed = false;
        }

        #endregion ================================================================================================


        #region ========================= Public Properties   =====================================================

        /// <summary>
        /// Gets or sets the custom SMIL layout.
        /// </summary>
        /// <value>The custom SMIL layout.</value>
        public virtual string CustomSmilLayout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the slides.
        /// </summary>
        /// <value>The slides.</value>
        public virtual List<MmsSlide> Slides
        {
            get
            {
                return slides;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the SMIL content is composed
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is SMIL content composed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsSmilContentComposed
        {
            get;
            set;
        }
     
        #endregion ========================= End Public Properties   ================================================


        #region ========================= Public Functions   =====================================================

        /// <summary>
        /// This function resets all properties to the initial, default values
        /// </summary>
        /// <returns>
        /// true if successful, false then check <b>LastError</b> property
        /// </returns>
        public bool Clear()
        {
            slides.Clear();
            tableOfContents.Clear();
            PresentationId = string.Empty;
            Subject = string.Empty;
            hFrom = null;
            hTo.Clear();
            hCc.Clear();
            hBcc.Clear();
            IsSmilContentComposed = false;
            return true;
        }


        /// <summary>
        /// Add a new slide to the message
        /// </summary>
        /// <param name="slide">The slide.</param>
        /// <returns>
        /// true if successful, false then check <b>LastError</b> property
        /// </returns>
        public bool AddSlide(MmsSlide slide) 
        {            
            slides.Add(slide);
            return true;

        }


        /// <summary>
        /// Adds a new receiver of the Multimedia Message. The message can have
        /// more than one receiver but at least one.
        /// param value is the string representing the address of the receiver. It has
        /// to be specified in the full format i.e.: +358990000005/TYPE=PLMN or
        /// joe@user.org or 1.2.3.4/TYPE=IPv4.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="addressType">Address type.</param>
        public virtual void AddToAddress(string value, MmsAddressType addressType)
        {
            base.AddToAddress(FormatAddress(value, addressType));
        }

        /// <summary>
        /// Adds a new receiver in the CC (Carbon Copy) field of the Multimedia Message. The message can have
        /// more than one receiver in the CC field.
        /// param value is the string representing the address of the CC receiver. It has
        /// to be specified in the full format i.e.: +358990000005/TYPE=PLMN or
        /// joe@user.org or 1.2.3.4/TYPE=IPv4.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="addressType">Type of the address.</param>
        public virtual void AddCcAddress(string value,MmsAddressType addressType)
        {
            base.AddCcAddress(FormatAddress(value, addressType));
        }

        /// <summary>
        /// Adds a new receiver in the BCC (Blind Carbon Copy) field of the Multimedia Message. The message can have
        /// more than one receiver in the BCC field.
        /// </summary>
        /// <param name="value">value is the string representing the address of the BCC receiver. It has
        /// to be specified in the full format i.e.: +358990000005/TYPE=PLMN or
        /// joe@user.org or 1.2.3.4/TYPE=IPv4.</param>
        /// <param name="addressType">Address type.</param>
        public virtual void AddBccAddress(string value, MmsAddressType addressType)
        {
            base.AddBccAddress(FormatAddress(value, addressType));
        }


        /// <summary>
        /// Load a MMS message from file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The MMS instance</returns>
        public static Mms LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new GatewayException(string.Format(Resources.MmsFileNotExist, fileName));               
            }

            try
            {
                MultimediaMessageDecoder decoder = new MultimediaMessageDecoder(File.ReadAllBytes(fileName));
                decoder.DecodeMessage();
                MultimediaMessage mms = decoder.GetMessage();                
                Mms message = new Mms();
                GatewayHelper.SetFields<Mms, MultimediaMessage>(mms, message);
                return message;
            }
            catch (Exception e)
            {
                throw new GatewayException(string.Format(Resources.MmsLoadException, fileName, e.Message), e);
            }           
        }

        /// <summary>
        /// Save the message into a single file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// true if successful, false then check <b>LastError</b> property
        /// </returns>
        public bool SaveToFile(string fileName)
        {
            try
            {
                // Create a new instance
                Mms newInstance = ObjectClone.DeepCopy<Mms>(this);
                newInstance.ComposeContentFromSlide();

                MultimediaMessage mms = new MultimediaMessage();
                // Assign the values from current instance to the MMS message
                GatewayHelper.SetFields<MultimediaMessage, Mms>(newInstance, mms);
                MultimediaMessageEncoder encoder = new MultimediaMessageEncoder();
                encoder.SetMessage(mms);
                encoder.EncodeMessage();
                byte[] output = encoder.GetMessage();
                encoder.SaveToFile(fileName);
                return true;
            }
            catch (Exception e)
            {
                LastError = new GatewayException(string.Format(Resources.MmsSaveException, fileName, e.Message), e);
            }
            return false;
        }


        #endregion ========================= End Public Functions ==================================================


        #region ============== Private Functions   ====================================================================

        /// <summary>
        /// Formats the address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="addressType">Type of the address.</param>
        /// <returns></returns>
        private string FormatAddress(string address, MmsAddressType addressType)
        {
            string formattedAddress = address;
            if (addressType == MmsAddressType.PhoneNumber)
            {
                formattedAddress += MmsConstants.PLMNSuffix;
            }
            else if (addressType == MmsAddressType.IPv4)
            {
                formattedAddress +=  MmsConstants.IPv4Suffix;

            }
            else if (addressType == MmsAddressType.IPv6)
            {
                formattedAddress += MmsConstants.IPv6Suffix;
            }
            return formattedAddress;
        }

        #endregion ========================= End Private Functions ==================================================

        #region ============== Internal Functions   =================================================================

        /// <summary>
        /// Composes the content from slides
        /// </summary>
        internal void ComposeContentFromSlide()
        {
            if (IsSmilContentComposed) return;

            // Translate the MMS slides to actual content
            string content = string.Empty;
            if (this.Slides.Count > 0)
            {
                string smilContent = this.CustomSmilLayout;
                foreach (MmsSlide slide in this.Slides)
                {
                    List<MmsAttachment> attachments = slide.Attachments;
                    if (attachments.Count > 0)
                    {
                        content += "<par dur=\"" + slide.Duration + "\">";
                        foreach (MmsAttachment attachment in attachments)
                        {

                            string contentId = attachment.Name;
                            MultimediaMessageContent multimediaMessageContent = new MultimediaMessageContent();
                            multimediaMessageContent.SetContent(attachment.Data, 0, attachment.Data.Length);
                            multimediaMessageContent.ContentId = contentId;
                            multimediaMessageContent.Type = StringEnum.GetStringValue(attachment.ContentType);
                            this.AddContent(multimediaMessageContent);

                            if (attachment.AttachmentType == AttachmentType.Image)
                            {
                                content += "<img src=\"" + contentId + "\" region=\"Image\" />";
                            }
                            else if (attachment.AttachmentType == AttachmentType.Text)
                            {
                                content += "<text src=\"" + contentId + "\" region=\"Text\" />";
                            }
                            else if (attachment.AttachmentType == AttachmentType.Audio)
                            {
                                content += "<audio src=\"" + contentId + "\"/>";
                            } else if (attachment.AttachmentType == AttachmentType.Video)
                            {
                                // Added June 23rd 2016
                                content += "<video src=\"" + contentId + "\" region=\"Video\" />";
                            }
                        }
                        content += "</par>";
                    }
                }

                string smilPresentationId = GatewayHelper.GenerateRandomId();
                smilContent = string.Format(smilContent, this.Subject, content);

                // Add the SMIL content
                MultimediaMessageContent multimediaSmilContent = new MultimediaMessageContent();
                byte[] smilBytes = Encoding.ASCII.GetBytes(smilContent);
                multimediaSmilContent.SetContent(smilBytes, 0, smilBytes.Length);
                multimediaSmilContent.ContentId = smilPresentationId;
                multimediaSmilContent.Type = Mms.ContentTypeApplicationSmil;
                this.AddContent(multimediaSmilContent);
                this.PresentationId = smilPresentationId;
                this.TransactionId = smilPresentationId;
                IsSmilContentComposed = true;
            } 
        }

        #endregion ========================= End Internal Functions ==================================================

        #region ============== Factory method   ====================================================================


        /// <summary>
        /// Static factory to create the Mms object
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="from">From</param>
        /// <returns>A new instance of the Mms object</returns>
        public static Mms NewInstance(string subject, string from)
        {
            Mms mms = new Mms();
            mms.Subject = subject;
            mms.SetFrom(from + MmsConstants.PLMNSuffix);
            mms.MessageType = MultimediaMessageConstants.MessageTypeMSendReq;
            mms.Version = MultimediaMessageConstants.MmsVersion10;

            // ContentType is mandatory, and must be last header!  These last 3 lines set the ContentType to
            // application/vnd.wml.multipart.related;type="application/smil";start="<0000>"
            // In case of multipart.mixed, only the first line is needed (and change the constant)

            // Any string will do for the Content-ID, but it must match that used for the presentation part,
            // and it must be Content-ID, it cannot be Content-Location.
            mms.ContentType =  MultimediaMessageConstants.ContentTypeApplicationMultipartRelated;
            
            
            // presentation part is written in SMIL
            mms.MultipartRelatedType = MultimediaMessageConstants.ContentTypeApplicationSmil;

            return mms;
        }
        
        #endregion =================================================================================================

        /// <summary>
        /// ToString method
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "GatewayId = ", GatewayId, "\r\n");
            str = String.Concat(str, "LastError = ", LastError, "\r\n");
            str = String.Concat(str, "QueuePriority = ", QueuePriority, "\r\n");
            str = String.Concat(str, "Identifier = ", Identifier, "\r\n");
            str = String.Concat(str, "ScheduledDeliveryDate = ", ScheduledDeliveryDate, "\r\n");
            str = String.Concat(str, "Persisted = ", Persisted, "\r\n");
            str = String.Concat(str, "CustomSmilLayout = ", CustomSmilLayout, "\r\n");
            str = String.Concat(str, "Slides = ", Slides, "\r\n");
            str = String.Concat(str, "IsSmilContentComposed = ", IsSmilContentComposed, "\r\n");
            return str;
        }
    
    }
    
}
