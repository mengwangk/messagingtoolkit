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
    /// This Interface should be implemented by the state machine of a upper
    /// protocol layer (e.g. WSP) to be informed, if there are service primitives
    /// to be processed.
    /// </summary>
    public interface IWTPUpperLayer
    {
        /// <summary>
        /// Receive service primitives ind and cnf by the WTP layer.
        /// </summary>
        /// <param name="wtpEvent">The WTP event.</param>
        void TrProcess(CWTPEvent wtpEvent);

        /// <summary>
        /// Service primitive TR-Abort.ind
        /// </summary>
        /// <param name="abortReason">The abort reason.</param>
        void TrAbort(short abortReason);
    }
}
