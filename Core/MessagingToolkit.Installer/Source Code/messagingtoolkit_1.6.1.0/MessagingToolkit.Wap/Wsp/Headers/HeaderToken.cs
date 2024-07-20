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
using System.Collections;
using System.Collections.Specialized;

namespace MessagingToolkit.Wap.Wsp.Headers
{
    /// <summary> 
    /// Helper class for tokenizing header fields. A HeaderToken consists of a token value
    /// and optional parameters.
    /// </summary> 
    public class HeaderToken
    {
        private static readonly IEnumerator EmptyEnum = ArrayList.Synchronized(new ArrayList(10)).GetEnumerator();
        private string token;
        private NameValueCollection parameters;     

        private HeaderToken(string token)
        {
            parameters = new NameValueCollection();
            this.token = token.Trim();
        }
        
        virtual public string Token
        {
            get
            {
                return token;
            }

        }

        /// <summary>
        /// Returns an enumeration of HeaderToken objects by tokenizing the specified
        /// String.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>an enumeration of HeaderToken's</returns>
        public static IEnumerator Tokenize(string value)
        {
            if (value == null)
            {
                return EmptyEnum;
            }
            value = value.Trim();
            ArrayList v = ArrayList.Synchronized(new ArrayList(10));

            if (value.Equals(""))
            {
                v.Add(new HeaderToken(""));
                return v.GetEnumerator();
            }

            Tokenizer strtok = new Tokenizer(value, ",");

            while (strtok.HasMoreTokens())
            {
                string token = strtok.NextToken();
                Tokenizer st2 = new Tokenizer(token, ";");
                token = st2.NextToken();

                HeaderToken ht = new HeaderToken(token);
                v.Add(ht);

                while (st2.HasMoreTokens())
                {
                    string param = st2.NextToken();
                    string key;
                    string val;
                    int p = param.IndexOf("=");

                    if (p > 0)
                    {
                        key = param.Substring(0, (p) - (0));
                        val = param.Substring(p + 1);
                    }
                    else
                    {
                        key = param;
                        val = "";
                    }

                    ht.AddParameter(key.ToLower(), val);
                }
            }

            return v.GetEnumerator();
        }

        public virtual string GetParameter(string key)
        {
            return parameters.Get(key);
        }

        public virtual bool HasParameters()
        {
            return parameters.Count > 0;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(token).Append("=").Append(parameters);

            return sb.ToString();
        }

        private void AddParameter(string key, string value)
        {
            parameters[(string)key.Trim()] = (string)value.Trim();
        }
    }
}
