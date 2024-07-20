using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingToolkit.Pdu
{

    /// <summary>
    /// WAP signal strength enumeration
    /// </summary>
    public enum WapSignalStrength
    {
        WapSignalNone = 0x05,
        WapSignalLow = 0x06,
        WapSignalMedium = 0x07,
        WapSignalHigh = 0x08,
        WapSignalDelete = 0x09
    }

    /// <summary>
    /// ==================================================
    /// FIRST OCTET CONSTANTS
    /// ==================================================
    /// to add, use the & with MASK to clear bits on original value
    ///         and | this cleared value with constant specified 
    /// TP-MTI   xxxxxx00 = SMS-DELIVER
    ///          xxxxxx10 = SMS-STATUS-REPORT
    ///          xxxxxx01 = SMS-SUBMIT
    /// </summary>
    public enum MessageTypeIndicator
    {
        TpMtiMask = 0xFC,
        TpMtiSmsDeliver = 0x00,
        TpMtiSmsSubmit = 0x01,
        TpMtiSmsStatusReport = 0x02
    }

    /// <summary>
    /// TP-RD   xxxxx0xx = accept duplicate messages
    /// xxxxx1xx = reject duplicate messages
    /// for SMS-SUBMIT only
    /// </summary>
    public enum RejectDuplicates
    {
        TpRdMask = 0xFB,
        TpRdAcceptDuplicates = 0x00,
        TpRdRejectDuplicates = 0x04
    }

    /// <summary>
    // TP-MMS   xxxxx0xx = more messages for the MS
    ///         xxxxx1xx = no more messages for the MS
    ///         for SMS-DELIVER and SMS-STATUS-REPORT only
    /// </summary>
    public enum MoreMessagesToSend
    {
        TpMmsMask = 0xFB,
        TpMmsNoMessages = 0x00,
        TpMmsMoreMessages = 0x04
    }


    /// <summary>
    // TP-VPF   xxx00xxx = no validity period
    ///         xxx10xxx = validity period integer-representation
    ///         xxx11xxx = validity period timestamp-representation  
    /// </summary>
    public enum ValidityPeriodFormat
    {
        TpVpfMask = 0xE7,
        TpVpfNone = 0x00,
        TpVpfInteger = 0x10,
        TpVpfTimestamp = 0x18
    }

    /// <summary>
    /// TP-SRI   xx0xxxxx = no status report to SME (for SMS-DELIVER only)
    ///          xx1xxxxx = status report to SME
    /// </summary>
    public enum StatusReportIndicator
    {
        TpSriMask = 0xDF,
        TpSriNoReport = 0x00,
        TpSriReport = 0x20
    }

    /// <summary>
    /// TP-SRR   xx0xxxxx = no status report (for SMS-SUBMIT only)
    ///          xx1xxxxx = status report
    /// </summary>
    public enum StatusReportRequest
    {
        TpSrrMask = 0xDF,
        TpSrrNoReport = 0x00,
        TpSrrReport = 0x20
    }

    /// <summary>
    /// TP-UDHI  x0xxxxxx = no UDH
    ///          x1xxxxxx = UDH present
    /// </summary>
    public enum UserDataHeaderIndicator
    {
        TpUdhiMask = 0xBF,
        TpUdhiNoUdh = 0x00,
        TpUdhiWithUdh = 0x40
    }

    /// <summary>
    /// TP-RP reply path - 1xxxxxxx = reply path exist
    ///                    0xxxxxxx = no reply path exist
    /// </summary>
    public enum ReplyPath
    {
        TpRpMask = 0x7F,
        TpRpNoRp = 0x00,
        TpRpWithRp = 0x80
    }

    /// <summary>
    /// ==================================================
    /// ADDRESS-TYPE CONSTANTS 
    /// ==================================================
    /// some typical ones used for sending, though receiving may get other types
    /// usually 1 001 0001 (0x91) international format 
    ///         1 000 0001 (0x81) (unknown) short number (e.g. access codes)
    ///         1 101 0000 (0xD0) alphanumeric (e.g. access code names like PasaLoad)
    /// </summary>
    public enum AddressNumberPlan
    {
        AddressNumberPlanIdMask = 0x0F,
        AddressNumberPlanIdUnknown = 0x00,
        AddressNumberPlanIdTelephone = 0x01
    }

    public enum AddressType
    {
        AddressTypeMask = 0x70,
        AddressTypeUnknown = 0x00,
        AddressTypeInternational = 0x10,
        AddressTypeAlphanumeric = 0x50,
        AddressTypeInternationFormat = 0x91,
        AddressTypeDomesticFormat = 0x81
    }

    /// <summary>
    /// ==================================================
    /// DCS ENCODING CONSTANTS 
    /// ==================================================
    /// </summary>
    public enum DcsCodingGroup
    {
        DcsCodingGroupMask = 0x0F,
        DcsCodingGroupData = 0xF0,
        DcsCodingGroupGeneral = 0xC0
    }

    public enum DcsEncoding
    {
        DcsEncodingMask = 0xF3,
        DcsEncoding7Bit = 0x00,
        DcsEncoding8Bit = 0x04,
        DcsEncodingUcs2 = 0x08
    }

    public enum DcsMessageClass
    {
        DcsMessageClassMask = 0xEC,
        DcsMessageClassFlash = 0x10,
        DcsMessageClassMe = 0x11,
        DcsMessageClassSim = 0x12,
        DcsMessageClassTe = 0x13
    }


    /// <summary> 
    /// Enumeration representing available message encodings.
    /// </summary>
    public enum MessageEncodings
    {
        /// <summary>
        /// 7 bit encoding - standard GSM alphabet.
        /// </summary>
        Enc7Bit,
        /// <summary>
        /// 8 bit encoding.
        /// </summary>
        Enc8Bit,
        /// <summary>
        /// UCS2 (Unicode) encoding.
        /// </summary>
        EncUcs2,
        /// <summary>
        /// Custom encoding. Currently just defaults to 7-bit.
        /// </summary>
        EncCustom
    }

    /// <summary>
    /// Enumeration representing the different types of messages.
    /// </summary>
    public enum MessageTypes
    {
        /// <summary>
        /// Inbound message.
        /// </summary>
        Inbound,
        /// <summary>
        /// Outbound message.
        /// </summary>
        Outbound,
        /// <summary>
        /// Status (delivery) report message
        /// </summary>
        StatusReport,
        /// <summary>
        /// Outbound WAP SI message.
        /// </summary>
        WapSi,
        /// <summary>
        /// Unknown message.
        /// </summary>
        Unknown
    }
}
