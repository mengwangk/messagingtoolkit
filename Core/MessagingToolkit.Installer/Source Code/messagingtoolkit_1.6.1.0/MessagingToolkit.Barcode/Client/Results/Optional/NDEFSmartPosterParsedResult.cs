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
using ParsedResult = MessagingToolkit.Barcode.Client.Results.ParsedResult;
using ParsedResultType = MessagingToolkit.Barcode.Client.Results.ParsedResultType;


namespace MessagingToolkit.Barcode.Client.Results.Optional
{
    /// <summary>
    /// 
    /// </summary>
	public sealed class NDEFSmartPosterParsedResult:ParsedResult
	{
        public const int ActionUnspecified = -1;
        public const int ActionDo = 0;
        public const int ActionSave = 1;
        public const int ActionOpen = 2;

        private string title;
        private string uri;
        private int action;
	
    
        public string Title
		{
			get
			{
				return title;
			}
			
		}
		public string URI
		{
			get
			{
				return uri;
			}
			
		}
		public int Action
		{
			get
			{
				return action;
			}
			
		}
		override public string DisplayResult
		{
			get
			{
				if (title == null)
				{
					return uri;
				}
				else
				{
					return title + '\n' + uri;
				}
			}			
		}
		
		internal NDEFSmartPosterParsedResult(int action, string uri, string title):base(ParsedResultType.NDefSmartPoster)
		{
			this.action = action;
			this.uri = uri;
			this.title = title;
		}
	}
}