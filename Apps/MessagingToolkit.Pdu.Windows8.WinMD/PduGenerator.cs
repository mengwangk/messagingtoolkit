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
using System.Collections;
using System.IO;
using System.Globalization;

using MessagingToolkit.Pdu.Ie;

namespace MessagingToolkit.Pdu
{
    /// <summary>
    /// PDU generator
    /// </summary>
    public sealed class PduGenerator
    {
        private MemoryStream baos;
        private int firstOctetPosition = -1;
        private bool updateFirstOctet = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PduGenerator()
        {
        }

        protected internal virtual void WriteSmscInfo(Pdu pdu)
        {
            if (!string.IsNullOrEmpty(pdu.SmscAddress))
            {
                WriteBcdAddress(pdu.SmscAddress, pdu.SmscAddressType, pdu.SmscInfoLength);
            }
            else
            {
                WriteByte(0);
            }
        }

        protected internal virtual void WriteFirstOctet(Pdu pdu)
        {
            // store the position in case it will need to be updated later
            firstOctetPosition = pdu.SmscInfoLength + 1;
            WriteByte(pdu.FirstOctet);
        }

        // validity period conversion from hours to the proper integer
        protected internal virtual void WriteValidityPeriodInteger(double validityPeriod)
        {
            if (validityPeriod == -1)
            {
                baos.WriteByte((byte)0xFF);
            }
            else
            {
                double validityInt;
                if (validityPeriod < 1) // In minutes
                    validityInt = (validityPeriod * 60) / 5 - 1;
                else if (validityPeriod <= 12)
                    validityInt = (validityPeriod * 12) - 1;
                else if (validityPeriod <= 24)
                    validityInt = (((validityPeriod - 12) * 2) + 143);
                else if (validityPeriod <= 720)
                    validityInt = (validityPeriod / 24) + 166;
                else
                    validityInt = (validityPeriod / 168) + 192;
                baos.WriteByte((byte)validityInt);
            }
        }

        protected internal virtual void WriteTimeStampStringForDate(ref DateTimeOffset timestamp)
        {
            int year = timestamp.Year - 2000;
            int month = timestamp.Month;
            int dayOfMonth = timestamp.Day;
            int hourOfDay = timestamp.Hour;
            int minute = timestamp.Minute;
            int sec = timestamp.Second;
#if !NETFX_CORE && !PORTABLE
            TimeZone tz = TimeZone.CurrentTimeZone;
            int offset = tz.GetUtcOffset(timestamp).Milliseconds;
#else
            TimeZoneInfo tzi = TimeZoneInfo.Local;
            int offset = tzi.GetUtcOffset(timestamp).Milliseconds;
#endif
            int minOffset = offset / 60000;
            int tzValue = minOffset / 15;
            // for negative offsets, add 128 to the absolute value
            if (tzValue < 0)
            {
                tzValue = 128 - tzValue;
            }
            // note: the nibbles are written as BCD style
            baos.WriteByte((byte)PduUtils.CreateSwappedBcd(year));
            baos.WriteByte((byte)PduUtils.CreateSwappedBcd(month));
            baos.WriteByte((byte)PduUtils.CreateSwappedBcd(dayOfMonth));
            baos.WriteByte((byte)PduUtils.CreateSwappedBcd(hourOfDay));
            baos.WriteByte((byte)PduUtils.CreateSwappedBcd(minute));
            baos.WriteByte((byte)PduUtils.CreateSwappedBcd(sec));
            baos.WriteByte((byte)PduUtils.CreateSwappedBcd(tzValue));
        }

        protected internal virtual void WriteAddress(string address, int addressType, int addressLength)
        {
            switch (PduUtils.ExtractAddressType(addressType))
            {

                case (int)AddressType.AddressTypeAlphanumeric:
                    byte[] textSeptets = PduUtils.StringToUnencodedSeptets(address);
                    byte[] alphaNumBytes = PduUtils.Encode7bitUserData(null, textSeptets);
                    // ADDRESS LENGTH - should be the semi-octet count
                    //                - this type is not used for SMSCInfo
                    baos.WriteByte((byte)(alphaNumBytes.Length * 2));
                    // ADDRESS TYPE
                    baos.WriteByte((byte)addressType);
                    // ADDRESS TEXT					
                    baos.Write(alphaNumBytes, 0, alphaNumBytes.Length);
                    break;
                default:
                    // BCD-style
                    WriteBcdAddress(address, addressType, addressLength);
                    break;

            }
        }

        protected internal virtual void WriteBcdAddress(string address, int addressType, int addressLength)
        {
            // BCD-style
            // ADDRESS LENGTH - either an octet count or semi-octet count
            baos.WriteByte((byte)addressLength);
            // ADDRESS TYPE
            baos.WriteByte((byte)addressType);
            // ADDRESS NUMBERS
            // if address.length is not even, pad the string an with F at the end
            if (address.Length % 2 == 1)
            {
                address = address + "F";
            }
            int digit = 0;
            for (int i = 0; i < address.Length; i++)
            {
                char c = address[i];
                if (i % 2 == 1)
                {
                    digit |= ((Convert.ToInt32(Convert.ToString(c), 16)) << 4);
                    baos.WriteByte((byte)digit);
                    // clear it
                    digit = 0;
                }
                else
                {
                    digit |= (Convert.ToInt32(Convert.ToString(c), 16) & 0x0F);
                }
            }
        }

        protected internal virtual void WriteUdData(Pdu pdu, int mpRefNo, int partNo)
        {
            int dcs = pdu.DataCodingScheme;
            try
            {
                switch (PduUtils.ExtractDcsEncoding(dcs))
                {

                    case (int)DcsEncoding.DcsEncoding7Bit:
                        WriteUdData7bit(pdu, mpRefNo, partNo);
                        break;

                    case (int)DcsEncoding.DcsEncoding8Bit:
                        WriteUdData8bit(pdu, mpRefNo, partNo);
                        break;

                    case (int)DcsEncoding.DcsEncodingUcs2:
                        WriteUdDataUcs2(pdu, mpRefNo, partNo);
                        break;

                    default:
                        throw new Exception("Invalid DCS encoding: " + PduUtils.ExtractDcsEncoding(dcs));

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        protected internal virtual void WriteUdh(Pdu pdu)
        {
            // stream directly into the internal baos
            WriteUdh(pdu, baos);
        }

        protected internal virtual void WriteUdh(Pdu pdu, MemoryStream udhBaos)
        {
            // need to insure that proper concat info is inserted
            // before writing if needed
            // i.e. the reference number, maxseq and seq have to be set from
            // outside (OutboundMessage)
            udhBaos.WriteByte((byte)pdu.UdhLength);
            foreach (InformationElement ie in pdu.GetInformationElements())
            {
                udhBaos.WriteByte((byte)ie.Identifier);
                udhBaos.WriteByte((byte)ie.Length);
                udhBaos.Write(ie.Data, 0, ie.Data.Count());
            }
        }

        protected internal virtual int ComputeOffset(Pdu pdu, int maxMessageLength, int partNo)
        {
            // computes offset to which part of the string is to be encoded into the PDU
            // also sets the MpMaxNo field of the concatInfo if message is multi-part
            int offset;
            int maxParts = 1;
            if (!pdu.Binary)
            {
                maxParts = pdu.DecodedText.Length / maxMessageLength + 1;
            }
            else
            {
                maxParts = pdu.GetDataBytes().Length / maxMessageLength + 1;
            }
            if (pdu.HasTpUdhi())
            {
                if (pdu.ConcatInfo != null)
                {
                    if (partNo > 0)
                    {
                        pdu.ConcatInfo.MpMaxNo = maxParts;
                    }
                }
            }
            if ((maxParts > 1) && (partNo > 0))
            {
                //      - if partNo > maxParts
                //          - error
                if (partNo > maxParts)
                {
                    throw new Exception("Invalid partNo: " + partNo + ", maxParts=" + maxParts);
                }
                offset = ((partNo - 1) * maxMessageLength);
            }
            else
            {
                // just get from the start
                offset = 0;
            }
            return offset;
        }

        protected internal virtual void CheckForConcat(Pdu pdu, int lengthOfText, int maxLength, int maxLengthWithUdh, int mpRefNo, int partNo)
        {
            if ((lengthOfText <= maxLengthWithUdh) || ((lengthOfText > maxLengthWithUdh) && (lengthOfText <= maxLength)))
            {
                // nothing needed
            }
            else
            {
                // need concat
                if (pdu.ConcatInfo != null)
                {
                    // if concatInfo is already present then just replace the values with the supplied
                    pdu.ConcatInfo.MpRefNo = mpRefNo;
                    pdu.ConcatInfo.MpSeqNo = partNo;
                }
                else
                {
                    // add concat info with the specified mpRefNo, bogus maxSeqNo, and partNo
                    // bogus maxSeqNo will be replaced once it is known in the later steps
                    // this just needs to be added since its presence is needed to compute
                    // the UDH length
                    ConcatInformationElement concatInfo = InformationElementFactory.GenerateConcatInfo(mpRefNo, partNo);
                    pdu.AddInformationElement(concatInfo);
                    updateFirstOctet = true;
                }
            }
        }

        protected internal virtual int ComputePotentialUdhLength(Pdu pdu)
        {
            int currentUdhLength = pdu.TotalUdhLength;
            if (currentUdhLength == 0)
            {
                // add 1 for the UDH Length field
                return ConcatInformationElement.DefaultConcatLength + 1;
            }
            else
            {
                // this already has the UDH Length field, no need to add 1
                return currentUdhLength + ConcatInformationElement.DefaultConcatLength;
            }
        }

        protected internal virtual void WriteUdData7bit(Pdu pdu, int mpRefNo, int partNo)
        {
            string decodedText = pdu.DecodedText;
            // partNo states what part of the unencoded text will be used
            //      - max length is based on the size of the UDH
            //        for 7bit => maxLength = 160 - total UDH septets
            // check if this message needs a concat
            byte[] textSeptetsForDecodedText = PduUtils.StringToUnencodedSeptets(decodedText);
            int potentialUdhLength = PduUtils.GetNumSeptetsForOctets(ComputePotentialUdhLength(pdu));

            CheckForConcat(pdu, textSeptetsForDecodedText.Length, 160 - PduUtils.GetNumSeptetsForOctets(pdu.TotalUdhLength), 160 - potentialUdhLength, mpRefNo, partNo);

            // given the IEs in the pdu derive the max message body length
            // this length will include the potential concat added in the previous step
            int totalUDHLength = pdu.TotalUdhLength;
            int maxMessageLength = 160 - PduUtils.GetNumSeptetsForOctets(totalUDHLength);

            // get septets for part
            byte[] textSeptets = GetUnencodedSeptetsForPart(pdu, maxMessageLength, partNo);

            // udlength is the sum of udh septet length and the text septet length
            int udLength = PduUtils.GetNumSeptetsForOctets(totalUDHLength) + textSeptets.Length;
            baos.WriteByte((byte)udLength);
            // generate UDH byte[]
            // UDHL (sum of all IE lengths)
            // IE list        
            byte[] udhBytes = null;
            if (pdu.HasTpUdhi())
            {
                MemoryStream udhBaos = new MemoryStream();
                WriteUdh(pdu, udhBaos);
                // buffer the udh since this needs to be 7-bit encoded with the text
                udhBytes = udhBaos.ToArray();
#if !NETFX_CORE && !PORTABLE
                udhBaos.Close();
#endif
                udhBaos.Dispose();
            }
            // encode both as one unit        
            byte[] udBytes = PduUtils.Encode7bitUserData(udhBytes, textSeptets);
            // write combined encoded array			
            baos.Write(udBytes, 0, udBytes.Length);
        }

        private byte[] GetUnencodedSeptetsForPart(Pdu pdu, int maxMessageLength, int partNo)
        {
            // computes offset to which part of the string is to be encoded into the PDU
            // also sets the MpMaxNo field of the concatInfo if message is multi-part
            int offset;
            int maxParts = 1;

            // must use the unencoded septets not the actual string since
            // it is possible that some special characters in string are multi-septet
            byte[] unencodedSeptets = PduUtils.StringToUnencodedSeptets(pdu.DecodedText);

            maxParts = (unencodedSeptets.Length / maxMessageLength) + 1;

            if (pdu.HasTpUdhi())
            {
                if (pdu.ConcatInfo != null)
                {
                    if (partNo > 0)
                    {
                        pdu.ConcatInfo.MpMaxNo = maxParts;
                    }
                }
            }
            if ((maxParts > 1) && (partNo > 0))
            {
                //      - if partNo > maxParts
                //          - error
                if (partNo > maxParts)
                {
                    throw new Exception("Invalid partNo: " + partNo + ", maxParts=" + maxParts);
                }
                offset = ((partNo - 1) * maxMessageLength);
            }
            else
            {
                // just get from the start
                offset = 0;
            }

            // copy the portion of the full unencoded septet array for this part
            byte[] septetsForPart = new byte[Math.Min(maxMessageLength, unencodedSeptets.Length - offset)];
            Array.Copy(unencodedSeptets, offset, septetsForPart, 0, septetsForPart.Length);

            return septetsForPart;
        }

        protected internal virtual void WriteUdData8bit(Pdu pdu, int mpRefNo, int partNo)
        {
            // NOTE: binary messages are also handled here
            byte[] data;

            if (pdu.Binary)
            {
                // use the supplied bytes
                data = pdu.GetDataBytes();
            }
            else
            {
                // encode the text
                data = PduUtils.Encode8bitUserData(pdu.DecodedText);
            }
            // partNo states what part of the unencoded text will be used
            //      - max length is based on the size of the UDH
            //        for 8bit => maxLength = 140 - the total UDH bytes
            // check if this message needs a concat
            int potentialUdhLength = ComputePotentialUdhLength(pdu);

            CheckForConcat(pdu, data.Length, 140 - pdu.TotalUdhLength, 140 - potentialUdhLength, mpRefNo, partNo);

            // given the IEs in the pdu derive the max message body length
            // this length will include the potential concat added in the previous step        
            int totalUDHLength = pdu.TotalUdhLength;
            int maxMessageLength = 140 - totalUDHLength;
            // compute which portion of the message will be part of the message
            int offset = ComputeOffset(pdu, maxMessageLength, partNo);
            byte[] dataToWrite = new byte[Math.Min(maxMessageLength, data.Length - offset)];
            Array.Copy(data, offset, dataToWrite, 0, dataToWrite.Length);
            // generate udlength
            // based on partNo
            // udLength is an octet count for 8bit/ucs2
            int udLength = totalUDHLength + dataToWrite.Length;
            // write udlength
            baos.WriteByte((byte)udLength);
            // write UDH to the stream directly
            if (pdu.HasTpUdhi())
            {
                WriteUdh(pdu, baos);
            }
            // write data			
            baos.Write(dataToWrite, 0, dataToWrite.Length);
        }

        protected internal virtual void WriteUdDataUcs2(Pdu pdu, int mpRefNo, int partNo)
        {
            string decodedText = pdu.DecodedText;
            // partNo states what part of the unencoded text will be used
            //      - max length is based on the size of the UDH
            //        for ucs2 => maxLength = (140 - the total UDH bytes)/2
            // check if this message needs a concat
            int potentialUdhLength = ComputePotentialUdhLength(pdu);

            CheckForConcat(pdu, decodedText.Length, (140 - pdu.TotalUdhLength) / 2, (140 - potentialUdhLength) / 2, mpRefNo, partNo);

            // given the IEs in the pdu derive the max message body length
            // this length will include the potential concat added in the previous step        
            int totalUDHLength = pdu.TotalUdhLength;
            int maxMessageLength = (140 - totalUDHLength) / 2;
            // compute which portion of the message will be part of the message
            int offset = ComputeOffset(pdu, maxMessageLength, partNo);
            string textToEncode = decodedText.Substring(offset, (Math.Min(offset + maxMessageLength, decodedText.Length)) - (offset));
            // generate udlength
            // based on partNo
            // udLength is an octet count for 8bit/ucs2
            int udLength = totalUDHLength + (textToEncode.Length * 2);

            // write udlength         
            //byte[] val = BitConverter.GetBytes(udLength);
            baos.WriteByte((byte)udLength);
            // write UDH to the stream directly
            if (pdu.HasTpUdhi())
            {
                WriteUdh(pdu, baos);
            }
            // write encoded text
            byte[] tempSbyteArray;
            tempSbyteArray = PduUtils.EncodeUcs2UserData(textToEncode);
            baos.Write(tempSbyteArray, 0, tempSbyteArray.Length);
        }

        protected internal virtual void WriteByte(int value)
        {
            /*
            byte[] bytes = System.BitConverter.GetBytes(value);            
            if (BitConverter.IsLittleEndian)
                 Array.Reverse(bytes);

            for (int i=0; i < bytes.Length; i++) 
            {
                if (bytes[i] != 0) 
                {
                    baos.WriteByte(bytes[i]);
                }
            }
            */
            //baos.Write(bytes, 0, bytes.Length);

            /*
            byte[] firstOctet = new byte[2];
            firstOctet[0] = bytes[1];
            firstOctet[1] = bytes[0];

            byte[] secondOctet = new byte[2];
            secondOctet[0] = bytes[3];
            secondOctet[1] = bytes[2];
            */

            //baos.Write(firstOctet,0, firstOctet.Length);
            //baos.Write(secondOctet, 0, secondOctet.Length); 

            /*
            BinaryWriter binaryWriter = new BinaryWriter(baos);
            binaryWriter.Write(value);
            binaryWriter.Close();
            */
            baos.WriteByte((byte)value);

        }

        protected internal virtual void WriteBytes(byte[] b)
        {
            baos.Write(b, 0, b.Length);
        }

        internal List<string> GeneratePduList(Pdu pdu, int mpRefNo)
        {
            // generate all required PDUs for a given message
            // mpRefNo comes from the ModemGateway
            List<String> pduList = new List<String>();
            for (int i = 1; i <= pdu.MpMaxNo; i++)
            {
                string pduString = GeneratePduString(pdu, mpRefNo, i);
                pduList.Add(pduString);
            }
            return pduList;
        }

        public virtual string GeneratePduString(Pdu pdu)
        {
            return GeneratePduString(pdu, -1, -1);
        }

        // NOTE: partNo indicates which part of a multipart message to generate
        //       assuming that the message is multipart, this will be ignored if the
        //       message is not a concat message
        public virtual string GeneratePduString(Pdu pdu, int mpRefNo, int partNo)
        {
            baos = null;
            try
            {
                baos = new MemoryStream();
                firstOctetPosition = -1;
                updateFirstOctet = false;
                // process the PDU
                switch (pdu.TpMti)
                {

                    case (int)MessageTypeIndicator.TpMtiSmsDeliver:
                        GenerateSmsDeliverPduString((SmsDeliveryPdu)pdu, mpRefNo, partNo);
                        break;

                    case (int)MessageTypeIndicator.TpMtiSmsSubmit:
                        GenerateSmsSubmitPduString((SmsSubmitPdu)pdu, mpRefNo, partNo);
                        break;

                    case (int)MessageTypeIndicator.TpMtiSmsStatusReport:
                        GenerateSmsStatusReportPduString((SmsStatusReportPdu)pdu);
                        break;
                }
                // in case concat is detected in the writeUD() method
                // and there was no UDHI at the time of detection
                // the old firstOctet must be overwritten with the new value
                byte[] pduBytes = baos.ToArray();
                if (updateFirstOctet)
                {
                    pduBytes[firstOctetPosition] = (byte)(pdu.FirstOctet & 0xFF);
                }
                return PduUtils.BytesToPdu(pduBytes);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (baos != null)
                {
#if !NETFX_CORE && !PORTABLE
                    baos.Close();
#endif
                    baos.Dispose();
                }
            }
        }

        protected internal virtual void GenerateSmsSubmitPduString(SmsSubmitPdu pdu, int mpRefNo, int partNo)
        {
            // SMSC address info
            WriteSmscInfo(pdu);
            // first octet
            WriteFirstOctet(pdu);
            // message reference
            WriteByte(pdu.MessageReference);
            // destination address info
            WriteAddress(pdu.Address, pdu.AddressType, pdu.Address.Length);
            // protocol id
            WriteByte(pdu.ProtocolIdentifier);
            // data coding scheme
            WriteByte(pdu.DataCodingScheme);
            // validity period
            switch (pdu.TpVpf)
            {

                case (int)ValidityPeriodFormat.TpVpfInteger:
                    WriteValidityPeriodInteger(pdu.ValidityPeriod);
                    break;

                case (int)ValidityPeriodFormat.TpVpfTimestamp:
                    DateTimeOffset tempAux = pdu.ValidityTimestamp;
                    WriteTimeStampStringForDate(ref tempAux);
                    break;
            }
            // user data
            // headers        
            WriteUdData(pdu, mpRefNo, partNo);
        }

        // NOTE: the following are just for validation of the PduParser
        //       - there is no normal scenario where these are used
        protected internal virtual void GenerateSmsDeliverPduString(SmsDeliveryPdu pdu, int mpRefNo, int partNo)
        {
            // SMSC address info
            WriteSmscInfo(pdu);
            // first octet
            WriteFirstOctet(pdu);
            // originator address info
            WriteAddress(pdu.Address, pdu.AddressType, pdu.Address.Length);
            // protocol id
            WriteByte(pdu.ProtocolIdentifier);
            // data coding scheme
            WriteByte(pdu.DataCodingScheme);
            // timestamp
            DateTimeOffset tempAux = pdu.Timestamp;
            WriteTimeStampStringForDate(ref tempAux);
            // user data
            // headers
            WriteUdData(pdu, mpRefNo, partNo);
        }

        protected internal virtual void GenerateSmsStatusReportPduString(SmsStatusReportPdu pdu)
        {
            // SMSC address info
            WriteSmscInfo(pdu);
            // first octet
            WriteFirstOctet(pdu);
            // message reference
            WriteByte(pdu.MessageReference);
            // destination address info
            WriteAddress(pdu.Address, pdu.AddressType, pdu.Address.Length);
            // timestamp
            DateTimeOffset tempAux = pdu.Timestamp;
            WriteTimeStampStringForDate(ref tempAux);
            // discharge time(timestamp)
            DateTimeOffset tempAux2 = pdu.DischargeTime;
            WriteTimeStampStringForDate(ref tempAux2);
            // status
            WriteByte(pdu.Status);
        }
    }
}