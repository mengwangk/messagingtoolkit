using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Extension
{

    /// <summary>
    /// Wavecomm device with SIM Toolkit support.
    /// </summary>
    internal sealed class WavecommStk : WaveComGateway
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WavecommStk(MobileGatewayConfiguration config)
            : base(config)
        {
            messageStorage = MessageStorage.SmSr;
        }
    }
}
