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
    /// Specifies a description for a property event based on the string resource specified. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class SRDescriptionAttribute : DescriptionAttribute
    {
        #region Fields

        private bool replaced;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SRDescriptionAttribute"/> class.
        /// </summary>
        /// <param name="resource">The name of the resource.</param>
        public SRDescriptionAttribute(string resource)
            : base(resource)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the description stored in this attribute.
        /// </summary>
        public override string Description
        {
            get
            {
                if (!this.replaced)
                {
                    this.replaced = true;
                    this.DescriptionValue = Resources.ResourceManager.GetString(base.Description);
                }

                return base.Description;
            }
        }

        #endregion
    }
}