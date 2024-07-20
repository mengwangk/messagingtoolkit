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

namespace MessagingToolkit.Core.Ras.Design
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Specifies the base class used for displaying remote access service (RAS) dialog boxes on the screen. This class must be inherited.
    /// </summary>
    [ToolboxItem(false)]
    public abstract class RasCommonDialog : CommonDialog
    {
        #region Fields

        private Point _location;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.Design.RasCommonDialog"/> class.
        /// </summary>
        protected RasCommonDialog()
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the dialog has encountered an error.
        /// </summary>
        [SRDescription("RCDErrorDesc")]
        public event EventHandler<RasErrorEventArgs> Error;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the coordinates of the upper-left corner of the dialog box.
        /// </summary>
        [DefaultValue(typeof(Point), "0,0")]
        [SRCategory("CatLayout")]
        [SRDescription("RCDLocationDesc")]
        public Point Location
        {
            get { return this._location; }
            set { this._location = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets all <see cref="RasCommonDialog"/> properties to their default values.
        /// </summary>
        public override void Reset()
        {
            this.Location = Point.Empty;
        }

        /// <summary>
        /// Raises the <see cref="Error"/> event.
        /// </summary>
        /// <param name="e">An <see cref="MessagingToolkit.Core.Ras.RasErrorEventArgs"/> containing event data.</param>
        protected void OnError(RasErrorEventArgs e)
        {
            if (this.Error != null)
            {
                this.Error(this, e);
            }
        }

        #endregion
    }
}