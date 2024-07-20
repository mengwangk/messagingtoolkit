using System;
using System.IO;
using System.Text;

using ResultParser = MessagingToolkit.Barcode.Client.Results.ResultParser;

namespace MessagingToolkit.Barcode.Client.Results.Optional
{
	
	/// <summary> 
    /// Superclass for classes encapsulating results in the NDEF format.
	/// See <a href="http://www.nfc-forum.org/specs/">http://www.nfc-forum.org/specs/</a>.
    /// 
   	/// This code supports a limited subset of NDEF messages, ones that are plausibly
	/// useful in 2D barcode formats. This generally includes 1-record messages, no chunking,
	/// "short record" syntax, no ID field.
	/// </summary>
	abstract class AbstractNDEFResultParser:ResultParser
	{

        /// <summary>
        /// Byteses to string.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
		internal static string BytesToString(byte[] bytes, int offset, int length, string encoding)
		{
			try
			{
				string tempStr;
				tempStr = Encoding.GetEncoding(encoding).GetString(bytes);
				return new string(tempStr.ToCharArray(), offset, length);
			}
			catch (IOException uee)
			{
				// This should only be used when 'encoding' is an encoding that must necessarily
				// be supported by the JVM, like UTF-8
				throw new SystemException("Platform does not support required encoding: " + uee);
			}
		}
	}
}