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

using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Helper;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// MMS slide. A MMS message consists of at least one MMS slide.
    /// </summary>
    [global::System.Serializable]
    public class MmsSlide
    {
        #region ========================= Internal class   =======================================================

      
        #endregion ========================= End Internal Class ==================================================

        #region ========================= Private Constants   ====================================================

        /// <summary>
        /// Slide duration default value. Default to 10 seconds
        /// </summary>
        private const int DefaultDuration = 10;

        #endregion ========================= End Private Constants   ==============================================


        #region ========================= Private Variables   ====================================================

        /// <summary>
        /// List of attachments
        /// </summary>
        private List<MmsAttachment> attachments;

       
        #endregion ========================= End Private Variable   ==============================================


         #region ========================= Protected Constructor===================================================

        /// <summary>
        /// Constructor
        /// </summary>
        protected MmsSlide()
        {
            // Default to 10 seconds
            Duration = DefaultDuration;

            // Initialize the lists
            attachments = new List<MmsAttachment>(1);           
        }

        #endregion ================================================================================================


        #region ========================= Public Properties   =====================================================

        /// <summary>
        /// Gets or sets the id. The id is used to uniquely 
        /// identify the MMS slide
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public int Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the last error.
        /// </summary>
        /// <value>The last error.</value>
        public Exception LastError
        {
            get;
            private set;
        }       

        /// <summary>
        /// Gets the attachments.
        /// </summary>
        /// <value>The attachments.</value>
        public List<MmsAttachment> Attachments
        {
            get
            {
                return this.attachments;
            }
        }

        #endregion ========================= End Public Properties   ================================================


        #region ========================= Public Functions   =====================================================

        /// <summary>
        /// This function resets all Properties to the initial, default values.
        /// </summary>
        /// <returns>
        /// true if successful, false then check <b>LastError</b> property
        /// </returns>
        public bool Clear()
        {
            // Reset the properties
            Duration = DefaultDuration;
            attachments.Clear();          
            return true;
        }

        /// <summary>
        /// Adds the attachment.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="attachmentType">Type of the attachment.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        /// true if successful, false then check <b>LastError</b> property
        /// </returns>
        public bool AddAttachment(string fileName, AttachmentType attachmentType, ContentType contentType)
        {
            if (!File.Exists(fileName))
            {
                LastError = new GatewayException(string.Format(Resources.MmsSlideFileDoesNotExist, fileName));
                return false;
            }
            try
            {
                byte[] data = File.ReadAllBytes(fileName);
                attachments.Add(new MmsAttachment(System.IO.Path.GetFileName(fileName), data, attachmentType, contentType));
            }
            catch (Exception e)
            {
                LastError = new GatewayException(e.Message, e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds the attachment.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="data">The data.</param>
        /// <param name="attachmentType">Type of the attachment.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>
        /// true if successful, false then check <b>LastError</b> property
        /// </returns>
        public bool AddAttachment(string name, byte[] data, AttachmentType attachmentType, ContentType contentType)
        {
            if (data == null || data.Length == 0)
            {
                LastError = new GatewayException(string.Format(Resources.MmsSlideDataNotValid, name));
                return false;
            }
            attachments.Add(new MmsAttachment(name, data, attachmentType, contentType));
            return true;
        }

        /// <summary>
        /// Adds the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// true if successful, false then check <b>LastError</b> property
        /// </returns>
        public bool AddText(string text)
        {
            byte[] bytes = null;
            if (IsUnicode(text))
            {
                bytes = Encoding.UTF8.GetBytes(text);
            }
            else
            {
                bytes = Encoding.ASCII.GetBytes(text);
            }
            attachments.Add(new MmsAttachment(GatewayHelper.GenerateRandomId(), bytes, AttachmentType.Text, ContentType.TextPlain));
            return true;
        }

        #endregion ========================= End Public Functions ==================================================


        #region =========== Private Methods ===============================================================================

        /// <summary>
        /// Determines whether the specified text is Unicode.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text is Unicode; otherwise, <c>false</c>.
        /// </returns>
        private bool IsUnicode(string text)
        {
            int i = 0;
            for (i = 1; i <= text.Length; i++)
            {
                int code = Convert.ToInt32(Convert.ToChar(text.Substring(i - 1, 1)));
                if (code < 0 || code > 255)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion ========================================================================================================

       


        #region ============== Factory method   ====================================================================


        /// <summary>
        /// Static factory to create the MmsSlide object
        /// </summary>
        /// <returns>A new instance of the MmsSlide object</returns>
        public static MmsSlide NewInstance()
        {
            return new MmsSlide();
        }

        #endregion =================================================================================================
    }
}
