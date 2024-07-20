using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Diagnostics
{
    /// <summary>
    /// Diagnostics command class
    /// </summary>
    public static class DiagnosticsCommand
    {
        /// <summary>
        /// Send message
        /// </summary>
        public static string SendMessage = "AT+CMGS=?";

        /// <summary>
        /// List message
        /// </summary>
        public static string ListMessage = "AT+CMGL=?";


        /// <summary>
        /// Read message
        /// </summary>
        public static string ReadMessage = "AT+CMGR=?";       

    }
}
