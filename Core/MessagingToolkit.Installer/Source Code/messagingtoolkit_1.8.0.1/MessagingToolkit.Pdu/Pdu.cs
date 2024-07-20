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

using MessagingToolkit.Pdu.Ie;
using MessagingToolkit.Pdu.WapPush;

namespace MessagingToolkit.Pdu
{
    public abstract class Pdu
    {
        // UDH portion of UD if UDHI is present
        // only Concat and Port info will be treated specially
        // other IEs will have to get extracted from the map manually and parsed
        private Dictionary<int, InformationElement> ieMap = new Dictionary<int, InformationElement>();

        private List<InformationElement> ieList = new List<InformationElement>();

        // PDU class
        // this class holds directly usable data only
        // - all lengths are ints
        // - dates and strings are already decoded
        // - byte[] for binary data that can interpreted later
        // an object of this type is created via a PduParser
        // or created raw, has its field set and supplied to a PduGenerator
        // ==================================================
        // SMSC INFO
        // ==================================================
        private int smscInfoLength;

        private int smscAddressType;

        private string smscAddress;

        // ==================================================
        // FIRST OCTET
        // ==================================================
        private int firstOctet = 0;

        // ==================================================
        // NON-UDH DATA
        // ==================================================
        // UD minus the UDH portion, same as userData if
        // no UDH
        // these fields store data for the generation step
        private string decodedText;

        private byte[] dataBytes;

        // PDU MANAGEMENT
        private string rawPdu;


        // ==================================================
        // PROTOCOL IDENTIFIER
        // ==================================================
        // usually just 0x00 for regular SMS
        private int protocolIdentifier = 0x00;

        // ==================================================
        // DATA CODING SCHEME
        // ==================================================
        // usually just 0x00 for default GSM alphabet, phase 2
        private int dataCodingScheme = 0x00;

        // ==================================================
        // TYPE-OF-ADDRESS
        // ==================================================
        private int addressType;

        // ==================================================
        // ADDRESS
        // ==================================================
        // swapped BCD-format for numbers
        // 7-bit GSM string for alphanumeric
        private string address;

        // ==================================================
        // USER DATA SECTION
        // ==================================================
        // this is still needs to be stored since it does not always represent 
        // length in octets, for 7-bit encoding this is length in SEPTETS
        // NOTE: udData.length may not equal udLength if 7-bit encoding is used
        private int udLength;

        private byte[] udData;

        // ==================================================
        // USER DATA HEADER
        // ==================================================
        // all methods accessing UDH specific methods require the UDHI to be set
        // or else an exception will result
        private const int UdhCheckModeAddIfNone = 0;

        private const int UdhCheckModeExceptionIfNone = 1;

        private const int UdhCheckModeIgnoreIfNone = 2;

        /// <summary>
        /// Constructor
        /// </summary>
        public Pdu()
        {

        }

        virtual public int SmscInfoLength
        {
            get
            {
                return smscInfoLength;
            }

            set
            {
                this.smscInfoLength = value;
            }

        }
        virtual public int SmscAddressType
        {
            get
            {
                return smscAddressType;
            }

            set
            {
                this.smscAddressType = PduUtils.CreateAddressType(value);
            }

        }
        virtual public string SmscAddress
        {
            get
            {
                return smscAddress;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.smscAddress = null;
                    smscAddressType = 0;
                    smscInfoLength = 0;
                    return;
                }
                // strip the + since it is not needed
                if (value.StartsWith("+"))
                {
                    this.smscAddress = value.Substring(1);
                }
                else
                {
                    this.smscAddress = value;
                }
            }

        }
        virtual public int FirstOctet
        {
            get
            {
                return firstOctet;
            }

            set
            {
                firstOctet = value;
            }

        }
        virtual public int TpMti
        {
            get
            {
                return GetFirstOctetField(PduUtils.TpMtiMask);
            }

            set
            {
                SetFirstOctetField(PduUtils.TpMtiMask, value, new int[] { PduUtils.TpMtiSmsDeliver, PduUtils.TpMtiSmsStatusReport, PduUtils.TpMtiSmsSubmit });
            }

        }


        virtual public int TpUdhi
        {
            set
            {
                SetFirstOctetField(PduUtils.TpUdhiMask, value, new int[] { PduUtils.TpUdhiNoUdh, PduUtils.TpUdhiWithUdh });
            }

        }
        virtual public int ProtocolIdentifier
        {
            get
            {
                return protocolIdentifier;
            }

            set
            {
                this.protocolIdentifier = value;
            }

        }
        virtual public int DataCodingScheme
        {
            get
            {
                return dataCodingScheme;
            }

            set
            {
                switch (value & ~PduUtils.DcsEncodingMask)
                {

                    case PduUtils.DcsEncoding7Bit:
                    case PduUtils.DcsEncoding8Bit:
                    case PduUtils.DcsEncodingUcs2:
                        break;
                    default:
                        throw new Exception("Invalid encoding value: " + PduUtils.ByteToPdu(value));

                }
                dataCodingScheme = value;
            }

        }
        virtual public int AddressType
        {
            get
            {
                return addressType;
            }

            set
            {
                // insure last bit is always set
                this.addressType = PduUtils.CreateAddressType(value);
            }
        }

        virtual public string Address
        {
            get
            {
                return address;
            }

            set
            {
                if (value != null)
                {
                    // eliminate the + since this does not do anything
                    if (value.StartsWith("+"))
                    {
                        this.address = value.Substring(1);
                    }
                    else
                    {
                        this.address = value;
                    }
                    // pass original address with the +, if any
                    // to determine if international format or not
                    AddressType = PduUtils.GetAddressTypeFor(value);
                }
                else
                {
                    this.address = value;
                }
            }

        }
        virtual public int UdLength
        {
            get
            {
                return udLength;
            }

            set
            {
                this.udLength = value;
            }

        }
        virtual public byte[] UdData
        {
            get
            {
                return udData;
            }

            // NOTE: udData DOES NOT include the octet with the length			
            set
            {
                this.udData = value;
            }

        }
        virtual public int TotalUdhLength
        {
            get
            {
                int udhLength = UdhLength;
                if (udhLength == 0)
                    return 0;
                // also takes into account the field holding the length
                // it self
                return udhLength + 1;
            }

        }
        virtual public int UdhLength
        {
            get
            {
                // compute based on the IEs
                int udhLength = 0;
                foreach (InformationElement ie in ieMap.Values)
                {
                    // length + 2 to account for the octet holding the IE length and id
                    udhLength = udhLength + ie.Length + 2;
                }
                return udhLength;
            }

        }
        virtual public byte[] UdhData
        {
            get
            {
                CheckForUdhi(UdhCheckModeIgnoreIfNone);
                int totalUdhLength = TotalUdhLength;
                if (totalUdhLength == 0)
                    return null;
                byte[] retVal = new byte[totalUdhLength];
                Array.Copy(udData, 0, retVal, 0, totalUdhLength);
                return retVal;
            }

        }
        virtual public bool ConcatMessage
        {
            // ==================================================
            // CONCAT INFO
            // ==================================================

            get
            {
                // check if iei 0x00 or 0x08 is present
                return (ConcatInfo != null);
            }
        }

        virtual public ConcatInformationElement ConcatInfo
        {
            get
            {
                CheckForUdhi(UdhCheckModeIgnoreIfNone);
                ConcatInformationElement concat = (ConcatInformationElement)GetInformationElement(ConcatInformationElement.Concat8BitRef);
                if (concat == null)
                {
                    concat = (ConcatInformationElement)GetInformationElement(ConcatInformationElement.Concat16BitRef);
                }
                return concat;
            }
        }

        virtual public int MpRefNo
        {
            get
            {
                ConcatInformationElement concat = ConcatInfo;
                if (concat != null)
                    return concat.MpRefNo;
                return 0;
            }

        }
        virtual public int MpMaxNo
        {
            get
            {
                ConcatInformationElement concat = ConcatInfo;
                if (concat != null)
                    return concat.MpMaxNo;
                return 1;
            }
        }

        virtual public int MpSeqNo
        {
            get
            {
                ConcatInformationElement concat = ConcatInfo;
                if (concat != null)
                    return concat.MpSeqNo;
                return 0;
            }
        }

        virtual public bool PortedMessage
        {
            // ==================================================
            // PORT DATA
            // ==================================================			
            get
            {
                // check if iei 0x05 is present
                return (PortInfo != null);
            }
        }

        private PortInformationElement PortInfo
        {
            get
            {
                CheckForUdhi(UdhCheckModeIgnoreIfNone);
                return (PortInformationElement)GetInformationElement(PortInformationElement.Port16Bit);
            }

        }
        virtual public int DestinationPort
        {
            get
            {
                PortInformationElement portIe = PortInfo;
                if (portIe == null)
                    return -1;
                return portIe.DestinationPort;
            }
        }

        virtual public int SourcePort
        {
            get
            {
                PortInformationElement portIe = PortInfo;
                if (portIe == null)
                    return -1;
                return portIe.SourcePort;
            }
        }

        virtual public bool Binary
        {
            get
            {
                // use the DCS coding group or 8bit encoding				
                // return ((this.dataCodingScheme & PduUtils.DCS_CODING_GROUP_DATA) == PduUtils.DCS_CODING_GROUP_DATA || (this.dataCodingScheme & PduUtils.DCS_ENCODING_8BIT) == PduUtils.DCS_ENCODING_8BIT);
                if ((this.dataCodingScheme & PduUtils.DcsCodingGroupData) == PduUtils.DcsCodingGroupData || (this.dataCodingScheme & PduUtils.DcsEncoding8Bit) == PduUtils.DcsEncoding8Bit)
                {
                    if ((this.dataCodingScheme & PduUtils.DcsEncoding8Bit) == PduUtils.DcsEncoding8Bit)
                        return (true);
                }
                return (false);
            }
        }

        virtual public string DecodedText
        {
            get
            {
                // this should be try-catched in case the ud data is 
                // actually binary and might cause a decoding exception
                if (decodedText != null)
                    return decodedText;
                if (udData == null)
                {
                    // throw new NullReferenceException("No udData to decode");
                    return string.Empty;
                }
                try
                {
                    return DecodeNonUdhDataAsString();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            set
            {
                this.decodedText = value;
                this.dataBytes = null;
                // check if existing DCS indicates a flash message
                bool flash = false;
                if (PduUtils.ExtractDcsFlash(dataCodingScheme) == PduUtils.DcsMessageClassFlash)
                    flash = true;
                // clears the coding group to be text again in case it was originally binary
                this.dataCodingScheme &= PduUtils.DcsCodingGroupMask;
                // set the flash bit back since the above would clear it
                if (flash)
                    dataCodingScheme = dataCodingScheme | PduUtils.DcsMessageClassFlash;
            }
        }

        virtual public byte[] UserDataAsBytes
        {
            get
            {
                int remainingLength = udData.Length - (TotalUdhLength);
                byte[] retVal = new byte[remainingLength];
                Array.Copy(udData, TotalUdhLength, retVal, 0, remainingLength);
                return retVal;
            }

        }
        virtual public string RawPdu
        {
            get
            {
                return rawPdu;
            }

            set
            {
                this.rawPdu = value;
            }

        }


        protected internal virtual void SetFirstOctetField(int fieldName, int fieldValue, int[] allowedValues)
        {
            foreach (int value in allowedValues)
            {
                if (value == fieldValue)
                {
                    // clear the bits for this field
                    firstOctet &= fieldName;
                    // copy the new bits on to it
                    firstOctet |= fieldValue;
                    return;
                }
            }
            throw new Exception("Invalid value for fieldName.");
        }

        protected internal virtual int GetFirstOctetField(int fieldName)
        {
            return firstOctet & ~fieldName;
        }

        // ==================================================
        // FIRST OCTET UTILITIES
        // ==================================================
        protected internal virtual void CheckTpMti(int allowedType)
        {
            int tpMti = TpMti;
            if (tpMti != allowedType)
            {
                throw new Exception("Invalid message type : " + TpMti);
            }
        }

        protected internal virtual void CheckTpMti(int[] allowedTypes)
        {
            int tpMti = TpMti;
            foreach (int type in allowedTypes)
            {
                if (tpMti == type)
                {
                    return;
                }
            }
            throw new Exception("Invalid message type : " + TpMti);
        }

        public virtual bool HasTpUdhi()
        {
            return GetFirstOctetField(PduUtils.TpUdhiMask) == PduUtils.TpUdhiWithUdh;
        }


        private void CheckForUdhi(int udhCheckMode)
        {
            if (!HasTpUdhi())
            {
                switch (udhCheckMode)
                {

                    case UdhCheckModeExceptionIfNone:
                        throw new Exception("PDU does not have a UDHI in the first octet");

                    case UdhCheckModeAddIfNone:
                        TpUdhi = PduUtils.TpUdhiWithUdh;
                        break;

                    case UdhCheckModeIgnoreIfNone:
                        break;

                    default:
                        throw new Exception("Invalid UDH check mode");

                }
            }
        }


        public virtual void AddInformationElement(InformationElement ie)
        {
            CheckForUdhi(UdhCheckModeAddIfNone);
            ieMap.Add(ie.Identifier, ie);
            ieList.Add(ie);
        }

        public virtual InformationElement GetInformationElement(int iei)
        {
            CheckForUdhi(UdhCheckModeIgnoreIfNone);
            InformationElement ie;
            if (ieMap.TryGetValue(iei, out ie))
            {
                return ie;
            }
            else
            {
                return null;
            }
        }

        // this is only used in the parser generator
        public List<InformationElement> GetInformationElements()
        {
            CheckForUdhi(UdhCheckModeIgnoreIfNone);
            return ieList;
        }

        public virtual byte[] DataBytes
        {
            get
            {
                return dataBytes;
            }
            set
            {
                this.dataBytes = value;
                this.decodedText = null;
                // clear the encoding bits for this field 8-bit/data
                this.dataCodingScheme &= PduUtils.DcsEncodingMask;
                this.dataCodingScheme |= PduUtils.DcsEncoding8Bit;
                this.dataCodingScheme |= PduUtils.DcsCodingGroupData;
            }
        }



        public virtual void SetDataBytes(byte[] dataBytes)
        {
            this.dataBytes = dataBytes;
            this.decodedText = null;
            // clear the encoding bits for this field 8-bit/data
            this.dataCodingScheme &= PduUtils.DcsEncodingMask;
            this.dataCodingScheme |= PduUtils.DcsEncoding8Bit;
            this.dataCodingScheme |= PduUtils.DcsCodingGroupData;
        }

        public virtual byte[] GetDataBytes()
        {
            return dataBytes;
        }

        private string DecodeNonUdhDataAsString()
        {
            // convert PDU to text depending on the encoding
            // must also take into account the octet holding the length
            switch (PduUtils.ExtractDcsEncoding(DataCodingScheme))
            {

                case PduUtils.DcsEncoding7Bit:
                    // unpack all septets to octets with MSB holes
                    byte[] septets = PduUtils.EncodedSeptetsToUnencodedSeptets(udData);
                    int septetUDHLength = 0;
                    if (this.UdhData != null)
                    {
                        // work out how much of the UD is UDH
                        septetUDHLength = this.UdhData.Length * 8 / 7;
                        if (this.UdhData.Length * 8 % 7 > 0)
                        {
                            septetUDHLength++;
                        }
                    }
                    byte[] septetsNoUDH = new byte[udLength - septetUDHLength];
                    // src, srcStart, dest, destStart, length
                    Array.Copy(septets, septetUDHLength, septetsNoUDH, 0, septetsNoUDH.Length);
                    return PduUtils.UnencodedSeptetsToString(septetsNoUDH);

                case PduUtils.DcsEncoding8Bit:
                    return PduUtils.Decode8bitEncoding(UdhData, udData);

                case PduUtils.DcsEncodingUcs2:
                    return PduUtils.DecodeUcs2Encoding(UdhData, udData);
            }
            throw new Exception("Invalid dataCodingScheme: " + DataCodingScheme);
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<< " + GetType().Name + " >>");
            sb.Append("\r\n");
            sb.Append("Raw Pdu: ");
            sb.Append(rawPdu);
            sb.Append("\r\n");
            sb.Append("\r\n");
            // smsc info        
            // first octet
            if (smscAddress != null)
            {
                sb.Append("SMSC Address: [Length: " + SmscInfoLength + " (" + PduUtils.ByteToPdu((byte)SmscInfoLength) + ") octets");
                sb.Append(", Type: " + PduUtils.ByteToPdu(smscAddressType) + " (" + PduUtils.ByteToBits((byte)smscAddressType) + ")");
                sb.Append(", Address: " + smscAddress);
                sb.Append("]");
            }
            else
            {
                sb.Append("SMSC Address: [Length: 0 octets]");
            }
            sb.Append("\r\n\r\n");
            sb.Append(PduUtils.DecodeFirstOctet(this));
            string subclassInfo = PduSubclassInfo();
            if (subclassInfo != null)
            {
                sb.Append(subclassInfo);
            }
            sb.Append("\r\n");
            // ud, only for Submit and Delivery, Status Reports have no UD       
            if (udData != null)
            {
                switch (PduUtils.ExtractDcsEncoding(DataCodingScheme))
                {

                    case PduUtils.DcsEncoding7Bit:
                        sb.Append("User Data Length: " + UdLength + " (" + PduUtils.ByteToPdu(UdLength) + ") septets");
                        sb.Append("\r\n");
                        break;

                    case PduUtils.DcsEncoding8Bit:
                    case PduUtils.DcsEncodingUcs2:
                        sb.Append("User Data Length: " + UdLength + " (" + PduUtils.ByteToPdu(UdLength) + ") octets");
                        sb.Append("\r\n");
                        break;
                }
                sb.Append("User Data (pdu) : " + PduUtils.BytesToPdu(UdData));
                sb.Append("\r\n");
                if (HasTpUdhi())
                {
                    // raw udh
                    sb.Append("User Data Header (pdu) : " + PduUtils.BytesToPdu(UdhData));
                    sb.Append("\r\n");
                    int udhLength = UdhLength;
                    sb.Append("User Data Header Length: " + udhLength + " (" + PduUtils.ByteToPdu(udhLength) + ") octets");
                    sb.Append("\r\n");
                    sb.Append("\r\n");
                    // information elements
                    sb.Append("UDH Information Elements:\r\n");
                    foreach (InformationElement ie in ieMap.Values)
                    {
                        sb.Append(ie.ToString());
                        sb.Append("\r\n");
                    }
                    // decoded text
                    // raw binary (as pdu)
                    sb.Append("\r\n");
                    sb.Append("Non UDH Data (pdu)    : " + PduUtils.BytesToPdu(UserDataAsBytes));
                    sb.Append("\r\n");
                    if (!Binary)
                    {
                        sb.Append("Non UDH Data (decoded): [" + DecodedText + "]");
                        sb.Append("\r\n");
                    }
                }
                else
                {
                    if (!Binary)
                    {
                        sb.Append("User Data (decoded): [" + DecodedText + "]");
                        sb.Append("\r\n");
                    }
                }
            }
            return sb.ToString();
        }

        protected internal virtual string PduSubclassInfo()
        {
            return null;
        }
    }
}