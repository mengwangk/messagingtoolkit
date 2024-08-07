//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright � TWIT88.COM.  All rights reserved.
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

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// See
    /// <a href="http://www.nttdocomo.co.jp/english/service/imode/make/content/barcode/about/s2.html">
    /// DoCoMo's documentation</a> about the result types represented by subclasses of this class.
    /// 
    /// Modified: May 10 2012
    /// </summary>
	public abstract class AbstractDoCoMoResultParser:ResultParser
	{


        internal static string[] MatchDoCoMoPrefixedField(string prefix, string rawText, bool trim)
        {
            return MatchPrefixedField(prefix, rawText, ';', trim);
        }

        internal static string MatchSingleDoCoMoPrefixedField(string prefix, string rawText, bool trim)
        {
            return MatchSinglePrefixedField(prefix, rawText, ';', trim);
        }

	}
}