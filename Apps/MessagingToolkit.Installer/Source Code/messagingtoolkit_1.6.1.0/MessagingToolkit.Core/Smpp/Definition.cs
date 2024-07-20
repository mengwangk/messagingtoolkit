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

using MessagingToolkit.Core.Smpp.Packet.Request;
using MessagingToolkit.Core.Smpp.Utility;
using MessagingToolkit.Core.Smpp.Packet.Response;
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.EventObjects;

namespace MessagingToolkit.Core.Smpp
{

    /// <summary>
    /// Binding types for the SMPP bind request.
    /// </summary>
    public enum BindingType : uint
    {
        /// <summary>
        /// BindAsReceiver
        /// </summary>
        BindAsReceiver = 1,
        /// <summary>
        /// BindAsTransmitter
        /// </summary>
        BindAsTransmitter = 2,
        /// <summary>
        /// BindAsTransceiver
        /// </summary>
        BindAsTransceiver = 9
    }

    /// <summary>
    /// Enumerates the number plan indicator types that can be used for the 
    /// SMSC
    /// message sending.
    /// </summary>
    public enum NpiType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0x00,
        /// <summary>
        /// ISDN
        /// </summary>
        ISDN = 0x01,
        /// <summary>
        /// Data
        /// </summary>
        Data = 0x03,
        /// <summary>
        /// Telex
        /// </summary>
        Telex = 0x04,
        /// <summary>
        /// Land mobile
        /// </summary>
        LandMobile = 0x06,
        /// <summary>
        /// National
        /// </summary>
        National = 0x08,
        /// <summary>
        /// Private
        /// </summary>
        Private = 0x09,
        /// <summary>
        /// ERMES
        /// </summary>
        ERMES = 0x0A,
        /// <summary>
        /// Internet
        /// </summary>
        Internet = 0x0E
    }

    /// <summary>
    /// Enumerates the type of number types that can be used for the SMSC 
    /// message
    /// sending.
    /// </summary>
    public enum TonType : byte
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0x00,
        /// <summary>
        /// International
        /// </summary>
        International = 0x01,
        /// <summary>
        /// National
        /// </summary>
        National = 0x02,
        /// <summary>
        /// Network specific
        /// </summary>
        NetworkSpecific = 0x03,
        /// <summary>
        /// Subscriber number
        /// </summary>
        SubscriberNumber = 0x04,
        /// <summary>
        /// Alphanumeric
        /// </summary>
        Alphanumeric = 0x05,
        /// <summary>
        /// Abbreviated
        /// </summary>
        Abbreviated = 0x06
    }


    /// <summary>
    /// SMPP version type.
    /// </summary>
    public enum SmppVersionType : byte
    {
        /// <summary>
        /// Version 3.3 of the SMPP spec.
        /// </summary>
        Version3_3 = 0x33,
        /// <summary>
        /// Version 3.4 of the SMPP spec.
        /// </summary>
        Version3_4 = 0x34
    }


    #region delegates

    /// <summary>
    /// Delegate to handle binding responses of the gateway.
    /// </summary>
    public delegate void BindRespEventHandler(object source, BindRespEventArgs e);
    /// <summary>
    /// Delegate to handle any errors that come up.
    /// </summary>
    public delegate void ErrorEventHandler(object source, CommonErrorEventArgs e);
    /// <summary>
    /// Delegate to handle the unbind_resp.
    /// </summary>
    public delegate void UnbindRespEventHandler(object source, UnbindRespEventArgs e);
    /// <summary>
    /// Delegate to handle closing of the connection.
    /// </summary>
    public delegate void ClosingEventHandler(object source, EventArgs e);
    /// <summary>
    /// Delegate to handle alert_notification events.
    /// </summary>
    public delegate void AlertEventHandler(object source, AlertEventArgs e);
    /// <summary>
    /// Delegate to handle a submit_sm_resp
    /// </summary>
    public delegate void SubmitSmRespEventHandler(object source, SubmitSmRespEventArgs e);
    /// <summary>
    /// Delegate to handle the enquire_link response.
    /// </summary>
    public delegate void EnquireLinkRespEventHandler(object source, EnquireLinkRespEventArgs e);
    /// <summary>
    /// Delegate to handle the submit_sm.
    /// </summary>
    public delegate void SubmitSmEventHandler(object source, SubmitSmEventArgs e);
    /// <summary>
    /// Delegate to handle the query_sm.
    /// </summary>
    public delegate void QuerySmEventHandler(object source, QuerySmEventArgs e);
    /// <summary>
    /// Delegate to handle generic_nack.
    /// </summary>
    public delegate void GenericNackEventHandler(object source, GenericNackEventArgs e);
    /// <summary>
    /// Delegate to handle the enquire_link.
    /// </summary>
    public delegate void EnquireLinkEventHandler(object source, EnquireLinkEventArgs e);
    /// <summary>
    /// Delegate to handle the unbind message.
    /// </summary>
    public delegate void UnbindEventHandler(object source, UnbindEventArgs e);
    /// <summary>
    /// Delegate to handle requests for binding of the gateway.
    /// </summary>
    public delegate void BindEventHandler(object source, BindEventArgs e);
    /// <summary>
    /// Delegate to handle cancel_sm.
    /// </summary>
    public delegate void CancelSmEventHandler(object source, CancelSmEventArgs e);
    /// <summary>
    /// Delegate to handle cancel_sm_resp.
    /// </summary>
    public delegate void CancelSmRespEventHandler(object source, CancelSmRespEventArgs e);
    /// <summary>
    /// Delegate to handle query_sm_resp.
    /// </summary>
    public delegate void QuerySmRespEventHandler(object source, QuerySmRespEventArgs e);
    /// <summary>
    /// Delegate to handle data_sm.
    /// </summary>
    public delegate void DataSmEventHandler(object source, DataSmEventArgs e);
    /// <summary>
    /// Delegate to handle data_sm_resp.
    /// </summary>
    public delegate void DataSmRespEventHandler(object source, DataSmRespEventArgs e);
    /// <summary>
    /// Delegate to handle deliver_sm.
    /// </summary>
    public delegate void DeliverSmEventHandler(object source, DeliverSmEventArgs e);
    /// <summary>
    /// Delegate to handle deliver_sm_resp.
    /// </summary>
    public delegate void DeliverSmRespEventHandler(object source, DeliverSmRespEventArgs e);
    /// <summary>
    /// Delegate to handle replace_sm.
    /// </summary>
    public delegate void ReplaceSmEventHandler(object source, ReplaceSmEventArgs e);
    /// <summary>
    /// Delegate to handle replace_sm_resp.
    /// </summary>
    public delegate void ReplaceSmRespEventHandler(object source, ReplaceSmRespEventArgs e);
    /// <summary>
    /// Delegate to handle submit_multi.
    /// </summary>
    public delegate void SubmitMultiEventHandler(object source, SubmitMultiEventArgs e);
    /// <summary>
    /// Delegate to handle submit_multi_resp.
    /// </summary>
    public delegate void SubmitMultiRespEventHandler(object source, SubmitMultiRespEventArgs e);

    #endregion delegates
}
