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
using System.Reflection;
using System.IO;

namespace MessagingToolkit.Wap.Helper.WbXml
{

    /// <summary>
    /// Global Tokens are common across all document types and are present in all code spaces and all code pages.
    /// All tokens have hexadecimal values.
    /// </summary>
	public abstract class GlobalTokens
    {
        public readonly static byte SwitchPage = 0x00;
		public readonly static byte End = 0x01;
		public readonly static byte Entity = 0x02;
		public readonly static byte StrIStrI  = 0x03;
		public readonly static byte Literal = 0x04;
		public readonly static byte ExtI0 = 0x40;
		public readonly static byte ExtI1 = 0x41;
		public readonly static byte ExtI2 = 0x42;
		public readonly static byte Pi = 0x43;
		public readonly static byte LiteralC = 0x44;
		public readonly static byte ExtT0 = 0x80;
		public readonly static byte ExtT1 = 0x81;
		public readonly static byte ExtT2 = 0x82;
		public readonly static byte StrT = 0x83;
		public readonly static byte LITERAL_A = 0x84;
		public readonly static byte Ext0 = 0xC0;
		public readonly static byte Ext1  = 0xC1;
		public readonly static byte Ext2  = 0xC2;
		public readonly static byte Opaque  = 0xC3;
		public readonly static byte LiteralAc  = 0xC4;	
	}
	
}