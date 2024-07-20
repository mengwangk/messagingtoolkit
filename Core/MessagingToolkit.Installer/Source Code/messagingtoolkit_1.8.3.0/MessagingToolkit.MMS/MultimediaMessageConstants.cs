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

namespace MessagingToolkit.MMS
{

    /// <summary>
    /// The MultimediaMessageConstants interface contains
    /// all the constants that are useful
    /// for the management of a Multimedia Message.
    /// </summary>
#if !NETFX_CORE
    [Serializable]
#else
    [System.Runtime.Serialization.DataContract]
#endif
	public class MultimediaMessageConstants 
    { 
        /// <summary>
        /// MMS version 1.0
        /// </summary>
		public const byte MmsVersion10 = 0x90;

        /// <summary>
        /// MMS version 1.1
        /// </summary>
        public const byte MmsVersion11 = 0x91;

        /// <summary>
        /// MMS version 1.2
        /// </summary>
        public const byte MmsVersion12 = 0x92;

        /// <summary>
        /// BCC field name
        /// </summary>
		public const byte FieldNameBcc = 0x01;

        /// <summary>
        /// CC field name
        /// </summary>
        public const byte FieldNameCc = 0x02;

        /// <summary>
        /// Content type field name
        /// </summary>
        public const byte FieldNameContentType = 0x04;

        /// <summary>
        /// Date field name
        /// </summary>
        public const byte FieldNameDate = 0x05;

        /// <summary>
        /// Delivery report field name
        /// </summary>
        public const byte FieldNameDeliveryReport = 0x06;

        /// <summary>
        /// Delivery time field name
        /// </summary>
        public const byte FieldNameDeliveryTime = 0x07;

        /// <summary>
        /// Expiry field name
        /// </summary>
        public const byte FieldNameExpiry = 0x08;

        /// <summary>
        /// From field name
        /// </summary>
        public const byte FieldNameFrom = 0x09;

        /// <summary>
        /// Message class field name
        /// </summary>
        public const byte FieldNameMessageClass = 0x0A;

        /// <summary>
        /// Message ID field name
        /// </summary>
        public const byte FieldNameMessageId = 0x0B;

        /// <summary>
        /// Message type field name
        /// </summary>
        public const byte FieldNameMessageType = 0x0C;

        /// <summary>
        /// MMS version field name
        /// </summary>
        public const byte FieldNameMmsVersion = 0x0D;

        /// <summary>
        /// Priority field name
        /// </summary>
        public const byte FieldNamePriority = 0x0F;

        /// <summary>
        /// Read reply field name
        /// </summary>
        public const byte FieldReadReply = 0x10;

        /// <summary>
        /// Sender visibility field name
        /// </summary>
        public const byte FieldNameSenderVisibility = 0x14;

        /// <summary>
        /// Status field name
        /// </summary>
        public const byte FieldNameStatus = 0x15;

        /// <summary>
        /// Subject field name
        /// </summary>
        public const byte FieldNameSubject = 0x16;

        /// <summary>
        /// To field name
        /// </summary>
        public const byte FieldNameTo = 0x17;

        /// <summary>
        /// Transaction id field name
        /// </summary>
        public const byte FieldNameTransactionId = 0x18;

        /// <summary>
        /// Unknown address type
        /// </summary>
		public const byte AddressTypeUnknown = 0;

        /// <summary>
        /// PLMN address type
        /// </summary>
        public const byte AddressTypePlmn = 1;

        /// <summary>
        /// IPV4 address type
        /// </summary>
        public const byte AddressTypeIpv4 = 2;

        /// <summary>
        /// IPV6 address type
        /// </summary>
        public const byte AddressTypeIpv6 = 3;

        /// <summary>
        /// Email address type
        /// </summary>
        public const byte AddressTypeEmail = 4;

        /// <summary>
        /// Send request message type
        /// </summary>
        public const byte MessageTypeMSendReq = 0x80;

        /// <summary>
        /// Received message type
        /// </summary>
        public const byte MessageTypeMReceived = 0x84;

        /// <summary>
        /// Delivery indication message type
        /// </summary>
        public const byte MessageTypeMDeliveryInd = 0x86;


        /// <summary>
        /// Expired message status
        /// </summary>
		public const byte MessageStatusExpired = 0x80;

        /// <summary>
        /// Retrieved message status
        /// </summary>
        public const byte MessageStatusRetrieved = 0x81;

        /// <summary>
        /// Rejected message status
        /// </summary>
        public const byte MessageStatusRejected = 0x82;

        /// <summary>
        /// Deferred message status
        /// </summary>
        public const byte MessageStatusDeferred = 0x83;

        /// <summary>
        /// Unrecognised message status
        /// </summary>
        public const byte MessageStatusUnrecognised = 0x84;

        /// <summary>
        /// Hide sender visibility
        /// </summary>
        public const byte SenderVisibilityHide = 0x80;

        /// <summary>
        /// Show sender visibility
        /// </summary>
        public const byte SenderVisibilityShow = 0x81;

        /// <summary>
        /// Personal message class
        /// </summary>
        public const byte MessageClassPersonal = 0x80;

        /// <summary>
        /// Advertisement message class
        /// </summary>
        public const byte MessageClassAdvertisement = 0x81;

        /// <summary>
        /// Informational message class
        /// </summary>
        public const byte MessageClassInformational = 0x82;

        /// <summary>
        /// Auto message class
        /// </summary>
        public const byte MessageClassAuto = 0x83;

        /// <summary>
        /// Low priority
        /// </summary>
        public const byte PriorityLow = 0x80;

        /// <summary>
        /// Normal priority
        /// </summary>
        public const byte PriorityNormal = 0x81;

        /// <summary>
        /// High priority
        /// </summary>
        public const byte PriorityHigh = 0x82;

        
        /// <summary>
        /// Content location header field name
        /// </summary>
        public const byte HeaderFieldNameContentLocation = 0x0E;

        /// <summary>
        /// Content id header field name
        /// </summary>
        public const byte HeaderFieldNameContentId = 0x40;
		
      

        /// <summary>
        /// Well known parameter type
        /// </summary>
        public const byte WkpaType = 0x09;

        /// <summary>
        /// Well known parameter start
        /// </summary>
        public const byte WkpaStart = 0x0A;




        /// <summary>
        /// HTML text content type
        /// </summary>
        public const string ContentTypeTextHtml = "text/html";

        /// <summary>
        /// Plain text content type
        /// </summary>
        public const string ContentTypeTextPlain = "text/plain";

        /// <summary>
        /// vCalendar content ype
        /// </summary>
        public const string ContentTypevCalendar = "text/x-vCalendar";

        /// <summary>
        /// vCard content ype
        /// </summary>
        public const string ContentTypevCard = "text/x-vCard";

        /// <summary>
        /// MPEG video
        /// </summary>
        public const string ContentTypeMpeg = "video/mpeg";

        /// <summary>
        /// AVI
        /// </summary>
        public const string ContentTypeAvi = "video/x-msvideo";


        /// <summary>
        /// Quicktime
        /// </summary>
        public const string ContentTypeQuicktime = "video/quicktime";

        /// <summary>
        /// WAV
        /// </summary>
        public const string ContentTypeWav = "audio/x-wav";

        /// <summary>
        /// WAV
        /// </summary>
        public const string ContentTypeAu = "audio/basic";

        /// <summary>
        /// AIFF
        /// </summary>
        public const string ContentTypeAiff = "audio/x-aiff";

        /// <summary>
        /// MP3
        /// </summary>
        public const string ContentTypeMp3 = "audio/mpeg";

        /// <summary>
        /// OGG
        /// </summary>
        public const string ContentTypeOgg = "audio/ogg";

        /// <summary>
        /// WML text content type
        /// </summary>
        public const string ContentTypeTextWml = "text/vnd.wap.wml";

        /// <summary>
        /// GIF image content type
        /// </summary>
        public const string ContentTypeImageGif = "image/gif";

        /// <summary>
        /// JPEG image content type
        /// </summary>
        public const string ContentTypeImageJpeg = "image/jpeg";

        /// <summary>
        /// TIFF image content type
        /// </summary>
        public const string ContentTypeImageTiff = "image/tiff";

        /// <summary>
        /// PNG image content type
        /// </summary>
        public const string ContentTypeImagePng = "image/png";

        /// <summary>
        /// WBMP image content type
        /// </summary>
        public const string ContentTypeImageWbmp = "image/vnd.wap.wbmp";

        /// <summary>
        /// AMR audio content type
        /// </summary>
        public const string ContentTypeAudioAmr = "audio/amr";

        /// <summary>
        /// IMelody content type
        /// </summary>
        public const string ContentTypeaAudioIMelody = "text/x-imelody";


        /// <summary>
        /// MIDI content type
        /// </summary>
        public const string ContentTypeAudioMidi = "audio/midi";


        /// <summary>
        /// Multipart mixed application content type
        /// </summary>
		public const string ContentTypeApplicationMultipartMixed = "application/vnd.wap.multipart.mixed";

        /// <summary>
        /// Multipart related application content type
        /// </summary>
        public const string ContentTypeApplicationMultipartRelated = "application/vnd.wap.multipart.related";
        
        /// <summary>
        /// SMIL application content type
        /// </summary>
		public const string ContentTypeApplicationSmil = "application/smil";


        /// <summary>
        /// Ticks for 1970
        /// </summary>
        public const long Ticks1970 = 621355968000000000; // .NET ticks for 1970
	}
	
}