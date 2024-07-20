using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Modified: May 18 2012
    /// </summary>
    public sealed class WifiParsedResult : ParsedResult
    {
        private readonly String ssid;
        private readonly bool hidden;
        private readonly String networkEncryption;
        private readonly String password;

        public WifiParsedResult(String networkEncryption, String ssid, String password)
            : this(networkEncryption, ssid, password, false)
        {

        }

        public WifiParsedResult(String networkEncryption, String ssid, String password, bool hidden)
            : base(ParsedResultType.Wifi)
        {
            this.ssid = ssid;
            this.networkEncryption = networkEncryption;
            this.password = password;
            this.hidden = hidden;
        }

        public String GetSsid()
        {
            return ssid;
        }

        public String GetNetworkEncryption()
        {
            return networkEncryption;
        }

        public String GetPassword()
        {
            return password;
        }

        public bool IsHidden()
        {
            return hidden;
        }
        public override String DisplayResult
        {
            get
            {
                StringBuilder result = new StringBuilder(80);
                MaybeAppend(ssid, result);
                MaybeAppend(networkEncryption, result);
                MaybeAppend(password, result);
                MaybeAppend(hidden.ToString(), result);
                return result.ToString();
            }
        }
    }
}