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

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp.Pdu;

namespace MessagingToolkit.Wap.Wsp
{

    /// <summary>
    /// WSP push manager
    /// </summary>
	public class CWSPPushManager : IWTPUpperLayer
	{
		private CWSPSession session;

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPPushManager"/> class.
        /// </summary>
        /// <param name="initpdu">The initpdu.</param>
        /// <param name="session">The session.</param>
		public CWSPPushManager(CWSPPDU initpdu, CWSPSession session)
		{
			this.session = session;
		}

        /// <summary>
        /// used by *WSP* layer
        /// </summary>
        /// <param name="reason">The reason.</param>
		public virtual void  Abort(short reason)
		{
			/** @todo abort by wsp */
		}

        /// <summary>
        /// S-ConfirmedPush.res
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="exitInfoAckHeaders">The exit info ack headers.</param>
		public virtual void  SConfirmedPush(sbyte[] data, string contentType, CWSPHeaders exitInfoAckHeaders)
		{
			lock (this)
			{
				if (session.GetState() == CWSPSession.StateConnectetd)
				{
					/** @todo implement s-confirmedPush.res */
				}
			}
		}

        /// <summary>
        /// S-PushAbort.req
        /// </summary>
        /// <param name="reason">Abort reason defined in class CWTPAbort</param>
        /// <returns>S-PushAbort.ind</returns>
		public virtual bool SPushAbort(short reason)
		{
			lock (this)
			{
				if (session.GetState() == CWSPSession.StateConnectetd)
				{
				}
				
				return false;
			}
		}
		
		public virtual void  TrProcess(CWTPEvent p)
		{
			throw new NotSupportedException("Methode Trrocess() is not implemented.");
		}

        /// <summary>
        /// used by *WTP* layer
        /// </summary>
        /// <param name="abortReason">The abort reason.</param>
		public virtual void  TrAbort(short abortReason)
		{			
			throw new NotSupportedException("Methode Abort() is not implemented.");
		}
	}
}