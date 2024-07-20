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
using System.Security.Cryptography;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core
{
    /// <summary>
    /// Contains the license information for the library
    /// </summary>
    public sealed class License
    {
        /// <summary>
        /// License key
        /// </summary>
        private string licenseKey;

        /// <summary>
        /// Flag to indicate if the license is valid
        /// </summary>
        private bool isValid;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        public License(IConfiguration configuration)
        {
            this.licenseKey = configuration.LicenseKey;
            CheckLicense();
        }
        /// <summary>
        /// Return the copyright information
        /// </summary>
        /// <value>Copyright information</value>
        public string CopyRight
        {
            get
            {
                return Resources.LicenseCopyright;
            }
        }

        /// <summary>
        /// License information
        /// </summary>
        /// <value>License information</value>
        public string Information
        {
            get
            {
                if (isValid)
                {
                    return "Licensed Copy";
                }
                else
                {
                    return "Community Copy";
                }
            }
        }

        /// <summary>
        /// Indicate if the license is valid
        /// </summary>
        /// <value>License status indicator</value>
        public bool Valid
        {
            get
            {
                return this.isValid;
            }
        }

        /// <summary>
        /// Check the license key
        /// </summary>
        private void CheckLicense()
        {
            if (string.IsNullOrEmpty(this.licenseKey))
            {
                this.isValid = false;
                return;
            }
            byte[] license = new byte[20] { 150, 10, 202, 111, 151, 16, 250, 55, 191, 200, 228, 70, 83, 84, 249, 248, 52, 168, 134, 191 };
            byte[] alternateLicense = new byte[20] {85,162,185,246,10,33,66,117,194,73,129,170,196,84,161,81,42,145,70,24 };

            HashAlgorithm sha = new SHA1CryptoServiceProvider();
            byte[] dataArray = Encoding.ASCII.GetBytes(this.licenseKey);
            byte[] result = sha.ComputeHash(dataArray);

            if (!CompareLicense(license, result) && !CompareLicense(alternateLicense, result)) 
            {
                this.isValid = false;
                return;
            }
            this.isValid = true;          
        }

        /// <summary>
        /// Compare license key
        /// </summary>
        /// <param name="license"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool CompareLicense(byte[] license, byte[] result)
        {
            if (result.Count() != license.Count())
            {                
                return false;
            }
            for (int i = 0; i < license.Count(); i++)
            {
                if (result[i] != license[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
