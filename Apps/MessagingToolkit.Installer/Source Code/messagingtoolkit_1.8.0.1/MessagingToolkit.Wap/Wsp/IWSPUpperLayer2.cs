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

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp.Pdu;


namespace MessagingToolkit.Wap.Wsp
{		
	public interface IWSPUpperLayer2:IWSPUpperLayer
	{
		new void  ConnectCnf();
		
		new void  SuspendInd(short reason);
		
		new void  ResumeCnf();
		
		new void  DisconnectInd(short reason);
		
		new void  DisconnectInd(System.Net.IPAddress[] redirectInfo);
		
		void  DisconnectInd(CWSPSocketAddress[] redirectInfo);
		
		void  MethodResultInd(CWSPResult result);
	}
}