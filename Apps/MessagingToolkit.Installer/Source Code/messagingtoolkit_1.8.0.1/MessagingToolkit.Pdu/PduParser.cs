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
using System.Globalization;

using MessagingToolkit.Pdu.Ie;

namespace MessagingToolkit.Pdu
{
    public class PduParser
    {
        // ==================================================
        // RAW PDU PARSER
        // ==================================================
        // increments as methods are called
        private int position;
        private byte[] pduByteArray;
        /*
        private string[] DateFormats = new string[]{
                                   "M/d/yyyy H:m:s zzz",
                                   "M/d/yyyy h:m:s zzz",
                                   "M/d/yy H:m:s zzz",
                                   "M/d/yy s:m:s zzz"
                               };
        */
        private string[] DateFormats = new string[]{
                                   "M/d/yyyy H:m:s",
                                   "M/d/yyyy h:m:s",
                                   "M/d/yy H:m:s",
                                   "M/d/yy s:m:s"
                               };

        // possible types of data
        // BCD digits
        // byte
        // gsm-septets 
        // timestamp info 
        private int ReadByte()
        {
            // read 8-bits forward
            int retVal = pduByteArray[position] & 0xFF;
            position++;
            return retVal;
        }

        private int ReadSwappedNibbleBCDByte()
        {
            // read 8-bits forward, swap the nibbles
            int data = ReadByte();
            data = PduUtils.SwapNibbles((byte)data);
            int retVal = 0;
            retVal += ((PduUtils.URShift(data, 4)) & 0xF) * 10;
            retVal += ((data & 0xF));
            return retVal;
        }

        private DateTime ReadTimeStamp(out string tz)
        {
            // reads timestamp info
            // 7 bytes in semi-octet(BCD) style
            int year = ReadSwappedNibbleBCDByte();
            int month = ReadSwappedNibbleBCDByte();
            int day = ReadSwappedNibbleBCDByte();
            int hour = ReadSwappedNibbleBCDByte();
            int minute = ReadSwappedNibbleBCDByte();
            int second = ReadSwappedNibbleBCDByte();
            int timezone = ReadSwappedNibbleBCDByte();
            string tzString = string.Empty;
            if (timezone >= 128)
            {
                // bit 3 of unswapped value represents the sign (1 == negative, 0 == positive)
                // when swapped this will now be bit 7 (128)
                timezone = timezone - 128;
                int totalMinutes = timezone * 15;
                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;

                tzString = "-" + hours + ":" + (minutes < 10 ? "0" : "") + minutes;
            }
            else
            {
                int totalMinutes = timezone * 15;
                int hours = totalMinutes / 60;
                int minutes = totalMinutes % 60;

                tzString = "+" + hours + ":" + (minutes < 10 ? "0" : "") + minutes;
            }
            tz = tzString;
            string dateString = month + "/" + day + "/" + (year + 2000) + " " + hour + ":" + minute +
                                        ":" + second; // +" " + tzString;
            try
            {
                return DateTime.ParseExact(dateString, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        private string ReadAddress(int addressLength, int addressType)
        {
            // NOTE: the max number of octets on an address is 12 octets 
            //       this means that an address field need only be 12 octets long
            //       what about for 7-bit?  This would be 13 chars at 12 octets?
            if (addressLength > 0)
            {
                // length is a semi-octet count
                int addressDataOctetLength = addressLength / 2 + ((addressLength % 2 == 1) ? 1 : 0);
                // extract data and increment position
                byte[] addressData = new byte[addressDataOctetLength];
                Array.Copy(pduByteArray, position, addressData, 0, addressDataOctetLength);
                position = position + addressDataOctetLength;
                switch (PduUtils.ExtractAddressType(addressType))
                {

                    case PduUtils.AddressTypeAlphanumeric:
                        // extract and process encoded bytes 
                        byte[] uncompressed = PduUtils.EncodedSeptetsToUnencodedSeptets(addressData);
                        int septets = addressLength * 4 / 7;
                        byte[] choppedAddressData = new byte[septets];
                        Array.Copy(uncompressed, 0, choppedAddressData, 0, septets);
                        return PduUtils.UnencodedSeptetsToString(choppedAddressData);
                    default:
                        // process BCD style data any other
                        return PduUtils.ReadBcdNumbers(addressLength, addressData);

                }
            }
            return null;
        }

        private int ReadValidityPeriodInt()
        {
            // this will convert the VP to #MINUTES
            int validity = ReadByte();
            int minutes = 0;
            if ((validity > 0) && (validity <= 143))
            {
                // groups of 5 min
                minutes = (validity + 1) * 5;
            }
            else if ((validity > 143) && (validity <= 167))
            {
                // groups of 30 min + 12 hrs
                minutes = (12 * 60) + (validity - 143) * 30;
            }
            else if ((validity > 167) && (validity <= 196))
            {
                // days
                minutes = (validity - 166) * 24 * 60;
            }
            else if ((validity > 197) && (validity <= 255))
            {
                // weeks
                minutes = (validity - 192) * 7 * 24 * 60;
            }
            return minutes;
        }

        public virtual Pdu ParsePdu(string rawPdu)
        {
            // encode pdu to byte[] for easier processing
            pduByteArray = PduUtils.PduToBytes(rawPdu);

            try
            {
                position = 0;
                // parse start and determine what type of pdu it is
                Pdu pdu = ParseStart();

                if (pdu.SmscAddressType != PduUtils.AddressTypeInternationFormat &&
                    pdu.SmscAddressType != PduUtils.AddressTypeDomesticFormat &&
                    pdu.SmscAddressType != PduUtils.AddressTypeUnknown)
                {
                    position = 0;
                    // parse start and determine what type of pdu it is
                    pdu = ParseStartWithSmscHeader();
                    pdu.RawPdu = rawPdu;
                    return ParsePduDetails(pdu);
                }
                else
                {
                    pdu.RawPdu = rawPdu;
                    return ParsePduDetails(pdu);
                }

            }
            catch (Exception)
            {
                position = 0;
                // parse start and determine what type of pdu it is
                Pdu pdu = ParseStartWithSmscHeader();
                pdu.RawPdu = rawPdu;
                return ParsePduDetails(pdu);
            }
        }

        private Pdu ParsePduDetails(Pdu pdu)
        {
            // parse depending on the pdu type
            switch (pdu.TpMti)
            {

                case PduUtils.TpMtiSmsDeliver:
                    ParseSmsDeliverMessage((SmsDeliveryPdu)pdu);
                    break;

                case PduUtils.TpMtiSmsSubmit:
                    //ParseSmsStatusReportMessage((SmsStatusReportPdu)pdu);
                    ParseSmsSubmitMessage((SmsSubmitPdu)pdu);
                    break;

                case PduUtils.TpMtiSmsStatusReport:
                    ParseSmsStatusReportMessage((SmsStatusReportPdu)pdu);
                    break;
            }
            return pdu;
        }


        private Pdu ParseStart()
        {
            // SMSC info
            // length
            // address type
            // smsc data
            int addressLength = ReadByte();
            Pdu pdu = null;
            if (addressLength > 0)
            {
                int addressType = ReadByte();
                string smscAddress = ReadAddress((addressLength - 1) * 2, addressType);
                // first octet - determine how to parse and how to store
                int firstOctet = ReadByte();
                pdu = PduFactory.CreatePdu(firstOctet);
                // generic methods
                pdu.SmscAddressType = addressType;
                pdu.SmscAddress = smscAddress;
                pdu.SmscInfoLength = addressLength;
            }
            else
            {
                // first octet - determine how to parse and how to store
                int firstOctet = ReadByte();
                pdu = PduFactory.CreatePdu(firstOctet);
            }
            return pdu;
        }

        private Pdu ParseStartWithSmscHeader()
        {
            Pdu pdu = null;
            // first octet - determine how to parse and how to store
            int firstOctet = ReadByte();
            pdu = PduFactory.CreatePdu(firstOctet);
            return pdu;
        }

        private void ParseUserData(Pdu pdu)
        {
            // ud length
            // NOTE: - the udLength value is just stored, it is not used to determine the length 
            //         of the remaining data (it may be a septet length not an octet length)            
            //       - parser just assumes that the remaining PDU data is for the User-Data field 
            int udLength = ReadByte();
            pdu.UdLength = udLength;
            // user data
            // NOTE: UD Data does not contain the length octet
            int udOctetLength = pduByteArray.Length - position;
            byte[] udData = new byte[udOctetLength];
            Array.Copy(pduByteArray, position, udData, 0, udOctetLength);

            // save the UD data
            pdu.UdData = udData;
            // user data header (if present)
            // position is still at the start of the UD
            if (pdu.HasTpUdhi())
            {
                // udh length
                int udhLength = ReadByte();
                // udh data (iterate till udh is consumed)
                // iei id
                // iei data length
                // iei data
                int endUdh = position + udhLength;
                while (position < endUdh)
                {
                    int iei = ReadByte();
                    int iedl = ReadByte();
                    byte[] ieData = new byte[iedl];
                    Array.Copy(pduByteArray, position, ieData, 0, iedl);
                    InformationElement ie = InformationElementFactory.CreateInformationElement(iei, ieData);
                    pdu.AddInformationElement(ie);
                    position = position + iedl;
                    if (position > endUdh)
                    {
                        // at the end, position after adding should be exactly at endUdh
                        throw new Exception("UDH is shorter than expected endUdh=" + endUdh + ", position=" + position);
                    }
                }
            }
        }

        private void ParseSmsDeliverMessage(SmsDeliveryPdu pdu)
        {
            // originator address info
            // address length
            // type of address
            // address data
            int addressLength = ReadByte();
            int addressType = ReadByte();
            string originatorAddress = ReadAddress(addressLength, addressType);
            pdu.Address = originatorAddress;
            pdu.AddressType = addressType;
            // protocol id
            int protocolId = ReadByte();
            pdu.ProtocolIdentifier = protocolId;
            // data coding scheme
            int dcs = ReadByte();
            pdu.DataCodingScheme = dcs;
            // timestamp
            string tz = string.Empty;
            DateTime timestamp = ReadTimeStamp(out tz);
            pdu.Timestamp = timestamp;
            pdu.Timezone = tz;
            // user data
            ParseUserData(pdu);
        }

        private void ParseSmsStatusReportMessage(SmsStatusReportPdu pdu)
        {
            // message reference
            int messageReference = ReadByte();
            pdu.MessageReference = messageReference;
            // destination address info
            int addressLength = ReadByte();
            int addressType = ReadByte();
            string destinationAddress = ReadAddress(addressLength, addressType);
            pdu.Address = destinationAddress;
            pdu.AddressType = addressType;
            // timestamp
            string tz = string.Empty;
            DateTime timestamp = ReadTimeStamp(out tz);
            pdu.Timestamp = timestamp;
            pdu.Timezone = tz;
            // discharge time(timestamp)
            DateTime timestamp2 = ReadTimeStamp(out tz);
            pdu.DischargeTime = timestamp2;
            // status
            int status = ReadByte();
            pdu.Status = status;
        }

        // NOTE: the following is just for validation of the PduGenerator
        //       - there is no normal scenario where this is used
        private void ParseSmsSubmitMessage(SmsSubmitPdu pdu)
        {
            // message reference
            int messageReference = ReadByte();
            pdu.MessageReference = messageReference;
            // destination address info
            int addressLength = ReadByte();
            int addressType = ReadByte();
            string destinationAddress = ReadAddress(addressLength, addressType);
            pdu.Address = destinationAddress;
            pdu.AddressType = addressType;
            // protocol id
            int protocolId = ReadByte();
            pdu.ProtocolIdentifier = protocolId;
            // data coding scheme
            int dcs = ReadByte();
            pdu.DataCodingScheme = dcs;
            // validity period
            switch (pdu.TpVpf)
            {

                case PduUtils.TpVpfNone:
                    break;

                case PduUtils.TpVpfInteger:
                    int validityInt = ReadValidityPeriodInt();
                    pdu.ValidityPeriod = validityInt / 60.0; // pdu assumes hours
                    break;

                case PduUtils.TpVpfTimestamp:
                    string tz = string.Empty;
                    DateTime validityDate = ReadTimeStamp(out tz);
                    pdu.ValidityTimestamp = validityDate;
                    pdu.Timezone = tz;
                    break;
            }
            ParseUserData(pdu);
        }


        /// <summary>
        /// Strip fill bits existing in the address field being interpreted as part
        /// of the actual address string. Note that this is only needed for
        /// alphanumeric sender addresses which will be encoded in the default 7-bit
        /// GSM alphabet.
        /// </summary>
        /// <param name="validSemiOctetNumber">the number of valid semi octets as given in the address length field</param>
        /// <param name="addressFieldString">the address string as per interpretation of whole field</param>
        /// <returns>
        /// the actual address string without possibly existing characters from interpreting fill bits
        /// </returns>
        private string StripAddressFieldPadding(int validSemiOctetNumber, string addressFieldString)
        {
            int validBitNumber = validSemiOctetNumber * 4;
            int valid7BitCharNumber = validBitNumber / 7;
            return addressFieldString.Substring(0, valid7BitCharNumber);
        }

    }
}