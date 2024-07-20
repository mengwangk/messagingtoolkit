//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright � TWIT88.COM.  All rights reserved.
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
using System.Xml;


#if NETFX_CORE

using Windows.Storage;
using Windows.Storage.Streams;

#endif

namespace MessagingToolkit.MMS
{

    /// <summary>
    /// The MultimediaMessageContent class represent a generic entry of a Multimedia Message.
    /// </summary>
#if !NETFX_CORE
    [Serializable]
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = false)]
#else
    [System.Runtime.Serialization.DataContract]
#endif
	public class MultimediaMessageContent
	{
        private string m_sType = string.Empty;
        private string m_sContentId = string.Empty;
        private MemoryStream m_byteArray;
        private int m_iLength = 0;


        /// <summary>
        /// Constructor. Creates the object representing the content.
        /// </summary>
        public MultimediaMessageContent()
        {
        }


        /// <summary>
        /// Retrieves the type of the entry. Examples are: text/plain, image/jpeg, image/gif, etc.
        /// return the type of the entry.
        /// </summary>
        /// <value>
        /// It's a valid content types. See WAP-203-WSP (WAP Forum)
        /// Examples are: text/plain, image/jpeg, image/gif, etc.
        /// You can use also some constants like:
        /// CONTENT_TYPE_TEXT_HTML, CONTENT_TYPE_TEXT_PLAIN, CONTENT_TYPE_TEXT_WML,
        /// CONTENT_TYPE_IMAGE_GIF, CONTENT_TYPE_IMAGE_JPEG, CONTENT_TYPE_IMAGE_WBMP,
        /// CONTENT_TYPE_APPLICATION_SMIL, etc.
        /// (these constants are defined in the interface MultimediaMessageConstants)
        /// </value>
	    virtual public string Type
		{
			get
			{
				return m_sType;
			}
			
			set
			{
				m_sType = value;
			}
			
		}

        /// <summary>
        /// Retrieves or sets the ID of the entry.
        /// </summary>
        /// <value>The content id.</value>
		virtual public string ContentId
		{
			get
			{
				return m_sContentId;
			}
			
			set
			{
				m_sContentId = value;
			}
			
		}
        /// <summary>
        /// Retrieves the length in bytes of the entry.
        /// </summary>
        /// <value>The length.</value>
		virtual public int Length
		{
			get
			{
				return m_iLength;
			}
			
		}
        /// <summary>
        /// Retrieves the String representing the entry.
        /// </summary>
        /// <value>The content as string.</value>
		virtual public string ContentAsString
		{
			get
			{
				//char[] tmpChar;
				byte[] tmpByte;
				tmpByte = m_byteArray.GetBuffer();

                /*
                char[] tmpChar = new char[m_byteArray.Length];
                Array.Copy(tmpByte, 0, tmpChar, 0, tmpChar.Length);
                string val = new string(tmpChar);
                Console.Write(val);
                */

                Encoding enc = Encoding.UTF8; //MultimediaMessageHelper.DetectEncoding(tmpByte);
                string value = enc.GetString(tmpByte);
                return value;

                /*
                string val1 = Encoding.ASCII.GetString(tmpByte);
                string val2 = Encoding.UTF8.GetString(tmpByte);
                if (!val1.Equals(val2)) return val2;

                return val1;   
                */
				
			}
			
		}

        /// <summary>
        /// Writes <code>len</code> bytes from the specified byte array starting at offset off.
        /// </summary>
        /// <param name="buf">the data</param>
        /// <param name="off">the start offset in the data</param>
        /// <param name="len">the number of bytes to write</param>
        public virtual void  SetContent(byte[] buf, int off, int len)
		{
			m_iLength = len;
			m_byteArray = new MemoryStream(len);
			m_byteArray.Write(buf, off, len);
		}
		
        /// <summary>
        /// Sets the content from file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
#if !NETFX_CORE
        public virtual void SetContent(string fileName)
#else
        public async virtual void SetContent(string fileName)
#endif
        {
            int fileSize = 0;
#if !NETFX_CORE
            FileStream fileHandler = null;
            // Opens the file for reading.
            try
            {
                fileHandler = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                fileSize = (int)fileHandler.Length;
            }
            catch (IOException ioErr)
            {
                throw new MultimediaMessageEncoderException(ioErr.Message);
            }
            byte[] buf = new byte[fileSize];
            fileHandler.Read(buf, 0, fileSize);
#else
            StorageFile storageFile = await StorageFile.GetFileFromPathAsync(fileName);
            Stream stream = await storageFile.OpenStreamForReadAsync();
            fileSize = (int)stream.Length;
            byte[] buf = new byte[fileSize];
            stream.Read(buf, 0, fileSize);
#endif
            m_iLength = fileSize;
            m_byteArray = new MemoryStream(m_iLength);
            m_byteArray.Write(buf, 0, m_iLength);
        }


        /// <summary>
        /// Retrieves the array of byte representing the entry.
        /// </summary>
        /// <returns>Byte array content</returns>
        public virtual byte[] GetContent()
        {
            return m_byteArray.ToArray();
        }

        /// <summary>
        /// Saves to file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public virtual void SaveToFile(string fileName)
        {
#if !NETFX_CORE
            FileStream fileoutputstream = null;
            try
            {
                fileoutputstream = new FileStream(fileName, FileMode.Create);
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                throw new MultimediaMessageEncoderException(fileNotFoundException.Message);
            }
            m_byteArray.WriteTo(fileoutputstream);
#else

            // @TODO
            StorageFolder folder = StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(fileName));
            StorageFile file = await folder.CreateFileAsync();
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (IOutputStream outputStream = fileStream.GetOutputStreamAt(0))
                {
                    using (DataWriter dataWriter = new DataWriter(outputStream))
                    {
                        //TODO: Replace "Bytes" with the type you want to write.
                        dataWriter.WriteBytes(bytes);
                        await dataWriter.StoreAsync();
                        dataWriter.DetachStream();
                    }

                    await outputStream.FlushAsync();
                }
            }
#endif
        }



        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy;
            System.Reflection.PropertyInfo[] infos = this.GetType().GetProperties(flags);

            StringBuilder sb = new StringBuilder();

            string typeName = this.GetType().Name;
            sb.AppendLine(typeName);
            sb.AppendLine(string.Empty.PadRight(typeName.Length + 5, '='));

            foreach (var info in infos)
            {
                if (!info.PropertyType.Name.StartsWith("List"))
                {
                    object value = info.GetValue(this, null);
                    sb.AppendFormat("{0}: {1}{2}", info.Name, value != null ? value : string.Empty, Environment.NewLine);
                }
            }

            return sb.ToString();
        }
		
	}
}