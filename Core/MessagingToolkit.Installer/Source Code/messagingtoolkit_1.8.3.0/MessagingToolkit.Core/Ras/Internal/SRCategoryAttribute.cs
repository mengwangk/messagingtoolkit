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

namespace MessagingToolkit.Core.Ras.Internal
{
    using System;
    using System.ComponentModel;
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Specifies the name of the category in which to group the property or event based on the string resource specified. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRCategoryAttribute : CategoryAttribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SRCategoryAttribute"/> class.
        /// </summary>
        /// <param name="resource">The name of the resource.</param>
        public SRCategoryAttribute(string resource)
            : base(resource)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Overridden. Looks up the name of the category from the resource manager.
        /// </summary>
        /// <param name="value">The string resource containing the category name.</param>
        /// <returns>The category name.</returns>
        protected override string GetLocalizedString(string value)
        {
            string retval = Resources.ResourceManager.GetString(value);
            if (string.IsNullOrEmpty(retval))
            {
                retval = value;
            }

            return retval;
        }

        #endregion
    }
}