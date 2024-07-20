﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Parses a WIFI configuration string. Strings will be of the form:
    /// <p>{@code WIFI:T:[network type];S:[network SSID];P:[network password];H:[hidden?];;}</p>
    /// <p>The fields can appear in any order. Only "S:" is required.</p>
    /// 
    /// Modified: May 18 2012
    /// </summary>
    internal sealed class WifiResultParser : ResultParser
    {

        public override ParsedResult Parse(Result result)
        {
            String rawText = GetMassagedText(result);
            if (!rawText.StartsWith("WIFI:"))
            {
                return null;
            }
            // Don't remove leading or trailing whitespace
            String ssid = MatchSinglePrefixedField("S:", rawText, ';', false);
            if (ssid == null || ssid.Length == 0)
            {
                return null;
            }
            String pass = MatchSinglePrefixedField("P:", rawText, ';', false);
            String type = MatchSinglePrefixedField("T:", rawText, ';', false);
            if (type == null)
            {
                type = "nopass";
            }

            bool hidden;
            Boolean.TryParse(MatchSinglePrefixedField("B:", rawText, ';', false), out hidden);
            return new WifiParsedResult(type, ssid, pass, hidden);
        }
    }
}
