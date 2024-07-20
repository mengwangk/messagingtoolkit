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
using System.IO;
using System.Security.Cryptography;

using MessagingToolkit.Core.Log;
using MessagingToolkit.Core;
using MessagingToolkit.Core.Service;
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Base
{
    /// <summary>
    /// Base class for all gateways
    /// </summary>
    /// <typeparam name="T">Derived gateway</typeparam>
    public abstract class BaseGateway<T>
    {
        //private const string LicenseFile = "license.lic";

        #region =========== Protected Variables ============================================================

        /// <summary>
        /// Store the last exception encountered
        /// </summary>
        protected Exception exception;
             
        #endregion ========================================================================================


        #region ================== Constructor ============================================================

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseGateway()
        {
            //CheckLicense();
        }

        #endregion ========================================================================================


        #region =========== Public Properties =============================================================

        /// <summary>
        /// Gateway id
        /// </summary>
        /// <value>gateway id</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gateway status
        /// </summary>
        /// <value>Gateway status</value>
        public GatewayStatus Status
        {
            get;
            set;
        }


        /// <summary>
        /// Return the last exception encountered
        /// </summary>
        /// <value>Exception</value>
        public Exception LastError
        {
            get
            {
                //if (exception != null)
                return exception;

                //return new Exception(Resources.MessageNoException);
            }
        }      

        /// <summary>
        /// Set the logging level.
        /// </summary>
        /// <value>LogLevel enum. See <see cref="LogLevel"/></value>
        public virtual LogLevel LogLevel
        {
            get
            {
                return Logger.LogLevel;
            }
            set
            {
                Logger.LogLevel = value;
            }
        }

        /// <summary>
        /// Log destination
        /// </summary>
        /// <value>Log destination. See <see cref="LogDestination"/></value>
        public virtual LogDestination LogDestination
        {
            get
            {
                return Logger.LogWhere;
            }
            set
            {
                Logger.LogWhere = value;
            }
        }

        /// <summary>
        /// Gets the log file.
        /// </summary>
        /// <value>The log file.</value>
        public virtual string LogFile
        {
            get
            {
                return Logger.LogPath;
            } 
        }
              
        
        #endregion ===========================================================================================


        #region =========== Public Method  ===================================================================

        /// <summary>
        /// Reset the exception
        /// </summary>
        public void ClearError()
        {
            exception = null;
        }

        /// <summary>
        /// Clears the log file content
        /// </summary>
        public void ClearLog()
        {
            Logger.ClearLog();
        }

        #endregion ===========================================================================================

        #region =========== Private Method  ===================================================================

        /*
        private void CheckLicense()
        {
            this.isLicensed = false;
            byte[] licenseKey = new byte[20] { 150, 10, 202, 111, 151, 16, 250, 55, 191, 200, 228, 70, 83, 84, 249, 248, 52, 168, 134, 191 };

            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string licenseFile = currentDirectory + Path.DirectorySeparatorChar + LicenseFile;
            if (File.Exists(licenseFile))
            {
                string fileContent = File.ReadAllText(licenseFile, Encoding.ASCII);
                fileContent = fileContent.Trim();
                HashAlgorithm sha = new SHA1CryptoServiceProvider();
                byte[] dataArray = Encoding.ASCII.GetBytes(fileContent);
                byte[] result = sha.ComputeHash(dataArray);
                if (result.Count() != licenseKey.Count()) return;
                for (int i = 0; i < licenseKey.Count(); i++) 
                {
                    if (result[i] != licenseKey[i]) return;
                }
                isLicensed = true;
            }            
        }
        */

        #endregion ===========================================================================================
    }
}
