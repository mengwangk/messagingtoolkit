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

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Send DTMF
    /// </summary>
    internal sealed class SendDtmfFeature: BaseFeature<SendDtmfFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

        /// <summary>
        /// Command to set DTMF duration
        /// </summary>
        private const string SetDurationCommand = "AT+VTD={0}";

        /// <summary>
        /// Command to send DTMF
        /// </summary>
        private const string SendDtmfCommand = "AT+VTS={0}";
               

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private SendDtmfFeature()
            : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(SetDurationCommand, Response.Ok, SetCommandDuration, DoNothing, true));
            AddCommand(Command.NewInstance(SendDtmfCommand, Response.Ok, Preprocess, DoNothing));

            // 0 - default value
            this.Duration = 0;

            this.Tones = string.Empty;
        }


        #endregion ==========================================================================================


        #region =========== Public Properties=================================================================

        /// <summary>
        /// Gets or sets the duration in milliseconds
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public int Duration
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the tones.
        /// </summary>
        /// <value>
        /// The tones.
        /// </value>
        public string Tones
        {
            get;
            set;
        }

        #endregion ===========================================================================================


        #region =========== Private Methods =================================================================

        /// <summary>
        /// Preprocess the command
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool SetCommandDuration(IContext context, ICommand command)
        {
            command.Request = string.Format(SetDurationCommand, this.Duration / 100);
            return true;
        }
        

        /// <summary>
        /// Preprocesses the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private bool Preprocess(IContext context, ICommand command)
        {
            if (string.IsNullOrEmpty(this.Tones)) return false;

            // AT+VTS=1;+VTS=3;+VTS=#
            string request = "AT";
            bool first = true;
            foreach (char c in this.Tones.ToCharArray())
            {
                if (first)
                {
                    request += "+VTS=" + c;
                    first = false;
                }
                else
                {
                    request += ";+VTS=" + c;
                }
            }
            command.Request = request;
            return true;
        }

        #endregion ===========================================================================================

      
        #region =========== Public Methods ===================================================================

       
        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "SendDtmfFeature: Send DTMF tones";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of SendDtmfFeature
        /// </summary>
        /// <returns>SendDtmfFeature instance</returns>
        public static SendDtmfFeature NewInstance()
        {
            return new SendDtmfFeature();
        }

        #endregion ===========================================================================================
       
    }
}
