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
using System.Net;

using MessagingToolkit.Wap.Wtp.Pdu;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wtp
{
    /// <summary>
    /// This class represents a service primitive used to communicate between WTP
    /// layer and the layer above (e.g. WSP). Generally are four types of
    /// service primitives:
    /// <ul>
    /// 		<li>req (upper -&gt; WTP) - Anounce a request</li>
    /// 		<li>ind (WTP -&gt; upper) - Indicate a request by a remote stack</li>
    /// 		<li>res (upper -&gt; WTP) - Answer a indication</li>
    /// 		<li>cnf (wtp -&gt; upper) - Answer a request</li>
    /// 	</ul>
    /// The wireless transaction protocol provides the following services
    /// according tp section 5.3 of the specification.
    /// <ul>
    /// 		<li>TR-INVOKE - Used to initiate a new transaction (req, ind, res, cnf) </li>
    /// 		<li>TR-RESULT - Used to send back a result of a previously initiated transaction</li>
    /// 		<li>TR-ABORT - Used to abort an existing transaction.</li>
    /// 	</ul>
    /// TR-INVOKE and TR-RESULT are modeled by this class. TR-ABORT is a call of
    /// <code>abort(abordCode)</code> on the corresponding transaction.
    /// Service primitive can be processed by CWTPSocket, that implements the state
    /// machine of the WTP layer or they are used to inform IWTPListeners by the WTP layer.
    /// </summary>
    public class CWTPEvent
    {
        // use these constants to specify the service primitive type
        public const byte TrInvokeReq = (0x00);
        public const byte TrInvokeInd = (0x01);
        public const byte TrInvokeRes = (0x02);
        public const byte TrInvokeCnf = (0x03);
        public const byte TrResultReq = (0x04);
        public const byte TrResultInd = (0x05);
        public const byte TrResultRes = (0x06);
        public const byte TrResultCnf = (0x07);
        public const byte TrInvokeDataReq = (0x08);
        public const byte TrInvokeDataInd = (0x09);
        public const byte TrInvokeDataRes = (0x0A);
        public const byte TrInvokeDataCnf = (0x0B);
        public const byte TrResultDataReq = (0x0C);
        public const byte TrResultDataInd = (0x0D);
        public const byte TrResultDataRes = (0x0E);
        public const byte TrResultDataCnf = (0x0F);
        public static readonly string[] types = new string[] { "TR_INVOKE_REQ", "TR_INVOKE_IND", "TR_INVOKE_RES", "TR_INVOKE_CNF", "TR_RESULT_REQ", "TR_RESULT_IND", "TR_RESULT_RES", "TR_RESULT_CNF", "TR_INVOKEDATA_REQ", "TR_INVOKEDATA_IND", "TR_INVOKEDATA_RES", "TR_INVOKEDATA_CNF", "TR_RESULTDATA_REQ", "TR_RESULTDATA_IND", "TR_RESULTDATA_RES", "TR_RESULTDATA_CNF" };
      
        /// <summary> 
        /// the service primitive type
        /// </summary>
        private byte type;
                
        // Fields used in a TR-Invoke oder TR-Result according to the spec

        /// <summary> 
        /// Section 5.3.1.6 User Data
        /// The user data carried by the  WTP protocol (payload).
        /// WTP layer will submit this data to the destination without changing content.
        /// </summary>
        private byte[] userData;

        /// <summary> 
        /// Section 5.3.1.8 Exit Info
        /// additional user data to be sent to the originator on transaction completion.
        /// moredata has to be false and classtype of transacition has to be 1.
        /// </summary>
        private byte[] exitInfo = new byte[0];

        /// <summary> 
        /// Section 5.3.1.9 More Data
        /// Will there be more invocations of this primitive for the same transaction?
        /// ext. segmentation and re-assambly has to be used!
        /// </summary>
        private bool moreData;

        /// <summary> 
        /// Section 5.3.1.10 Frame Boundary
        /// Is this user data the beginning of a new user defined frame?
        /// ext. segmentation and re-assambly has to be used!
        /// </summary>
        private bool frameBoundary;
        private IWTPTransaction transaction;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        virtual public byte Type
        {         
            get
            {
                return type;
            }

            set
            {
                this.type = value;
            }

        }
        /// <summary>
        /// Gets or sets the user data.
        /// </summary>
        /// <value>The user data.</value>
        virtual public byte[] UserData
        {
            get
            {
                return userData;
            }

            set
            {
                this.userData = value;
            }

        }
        /// <summary>
        /// Gets or sets the exit info.
        /// </summary>
        /// <value>The exit info.</value>
        virtual public byte[] ExitInfo
        {
            get
            {
                return exitInfo;
            }

            set
            {
                this.exitInfo = value;
            }

        }
        /// <summary>
        /// Gets or sets a value indicating whether [more data].
        /// </summary>
        /// <value><c>true</c> if [more data]; otherwise, <c>false</c>.</value>
        virtual public bool MoreData
        {
            get
            {
                return moreData;
            }

            set
            {
                this.moreData = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [frame boundary].
        /// </summary>
        /// <value><c>true</c> if [frame boundary]; otherwise, <c>false</c>.</value>
        virtual public bool FrameBoundary
        {
            get
            {
                return frameBoundary;
            }

            set
            {
                this.frameBoundary = value;
            }

        }
        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        virtual public IWTPTransaction Transaction
        {
            get
            {
                return transaction;
            }

            set
            {
                this.transaction = value;
            }

        }


        /// <summary>
        /// Constructor to construct tr-invoke and tr-result
        /// construct Aborts by calling abort(code) on the transaction.
        /// After this the transaction throws abortedExceptions on calling anything on it.)
        /// </summary>
        /// <param name="userData">Payload/bytes of the upper layer</param>
        /// <param name="exitInfo">Will there be more invocations of this primitive for the
        /// same transaction? (available if ext. segmentation and re-assambly used)</param>
        /// <param name="moreData">Additional invocations for the same transaction follow
        /// (available if ext. segmentation and re-assambly used)</param>
        /// <param name="frameBoundary">specifies a new user defined data frame
        /// (available if ext. segmentation and re-assambly used)</param>
        /// <param name="type">the type of this service primitive
        /// (use constants defined in the class)</param>
        public CWTPEvent(byte[] userData, byte[] exitInfo, bool moreData, bool frameBoundary, byte type)
        {
            this.userData = userData;
            this.exitInfo = exitInfo;
            this.moreData = moreData;
            this.frameBoundary = frameBoundary;
            this.type = type;

        }

        /// <summary>
        /// Constructor to construct tr-invoke and tr-result
        /// defaults: moreData = false, no exitInfo, frameBoundary = false
        /// </summary>
        /// <param name="userData">Payload/bytes of the upper layer</param>
        /// <param name="type">the type of this service primitive
        /// (use constants defined in the class)</param>
        public CWTPEvent(byte[] userData, byte type)
        {
            this.userData = userData;
            this.moreData = false;
            this.frameBoundary = false;
            this.type = type;
        }        
    }
}
