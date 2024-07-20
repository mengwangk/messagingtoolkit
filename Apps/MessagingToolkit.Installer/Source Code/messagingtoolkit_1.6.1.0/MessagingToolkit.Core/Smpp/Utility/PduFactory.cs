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
using System.Collections;
using MessagingToolkit.Core.Smpp;
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.Packet.Request;
using MessagingToolkit.Core.Smpp.Packet.Response;

namespace MessagingToolkit.Core.Smpp.Utility
{
	/// <summary>
	/// Takes incoming packets from an input stream and generates
	/// PDUs based on the command field.
	/// </summary>
	public class PduFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PduFactory()
		{
		}
		
		/// <summary>
		/// Factory method to generate the PDU.
		/// </summary>
		/// <param name="incomingPDUs">The SMSC response.</param>
		/// <returns>The PDU.</returns>
		public Queue GetPduQueue(byte[] incomingPDUs)
		{
			Queue packetQueue = new Queue();
			//get the first packet
			byte[] response = null;
            PDU packet = null;
			int newLength = 0;
			//this needs to start at zero
			uint CommandLength = 0;
			
			//look for multiple PDUs in the response
			while(incomingPDUs.Length > 0)
			{
				//determine if we have another response PDU after this one
				newLength =(int)(incomingPDUs.Length - CommandLength);
				//could be empty data or it could be a PDU
				if(newLength > 0)
				{
					//get the next PDU
                    response = PDU.TrimResponsePdu(incomingPDUs);
					//there could be none...
					if(response.Length > 0)
					{
						//get the command length and command ID
                        CommandLength = PDU.DecodeCommandLength(response);
						//trim the packet down so we can look for more PDUs
						long length = incomingPDUs.Length - CommandLength;
						byte[] newRemainder = new byte[length];
						Array.Copy(incomingPDUs, CommandLength, newRemainder, 0, length);
						incomingPDUs = newRemainder;
						newRemainder = null;
						if(CommandLength > 0)
						{
							//process
							packet = GetPDU(response);
							if(packet != null)
								packetQueue.Enqueue(packet);
						}
					}
					else
					{
						//kill it off and return
						incomingPDUs = new Byte[0];
					}
				}
			}
			
			return packetQueue;
		}
		
		/// <summary>
		/// Gets a single PDU based on the response bytes.
		/// </summary>
		/// <param name="response">The SMSC response.</param>
		/// <returns>The PDU corresponding to the bytes.</returns>
        private PDU GetPDU(byte[] response)
		{
            PDU.CommandIdType commandID = PDU.DecodeCommandId(response);
            PDU packet;
			switch(commandID)
			{
                case PDU.CommandIdType.AlertNotification:
					packet = new SmppAlertNotification(response);
					break;
                case PDU.CommandIdType.BindReceiverResp:
                case PDU.CommandIdType.BindTransceiverResp:
                case PDU.CommandIdType.BindTransmitterResp:
					packet = new SmppBindResp(response);
					break;
                case PDU.CommandIdType.CancelSmResp:
					packet = new SmppCancelSmResp(response);
					break;
                case PDU.CommandIdType.DataSmResp:
					packet = new SmppDataSmResp(response);
					break;
                case PDU.CommandIdType.DeliverSm:
					packet = new SmppDeliverSm(response);
					break;
                case PDU.CommandIdType.EnquireLinkResp:
					packet = new SmppEnquireLinkResp(response);
					break;
                case PDU.CommandIdType.Outbind:
					packet = new SmppOutbind(response);
					break;
                case PDU.CommandIdType.QuerySmResp:
					packet = new SmppQuerySmResp(response);
					break;
                case PDU.CommandIdType.ReplaceSmResp:
					packet = new SmppReplaceSmResp(response);
					break;
                case PDU.CommandIdType.SubmitMultiResp:
					packet = new SmppSubmitMultiResp(response);
					break;
                case PDU.CommandIdType.SubmitSmResp:
					packet = new SmppSubmitSmResp(response);
					break;
                case PDU.CommandIdType.UnbindResp:
					packet = new SmppUnbindResp(response);
					break;
				default:
					packet = null;
					break;
			}

			return packet;
		}
	}
}
