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
    using System.IO;
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Provides the base implementation for remote access service (RAS) components. This class must be inherited. 
    /// </summary>
    [ToolboxItem(false)]
    public abstract class RasComponent : Component
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.Design.RasComponent"/> class.
        /// </summary>
        protected RasComponent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.Design.RasComponent"/> class.
        /// </summary>
        /// <param name="container">An <see cref="System.ComponentModel.IContainer"/> that will contain this component.</param>
        protected RasComponent(IContainer container)
        {
            if (container != null)
            {
                container.Add(this);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the component has encountered an error.
        /// </summary>
        [SRDescription("RCErrorDesc")]
        public event EventHandler<ErrorEventArgs> Error;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls that are issued by the component.
        /// </summary>
        /// <remarks>This property is only required if you need to marshal events raised by the component back to another thread. Typically this is only needed if you're using a user interface, applications like Windows services do not require any thread marshalling.</remarks>
        [DefaultValue(null)]
        [SRDescription("RCSyncObjectDesc")]
        public ISynchronizeInvoke SynchronizingObject
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the component.
        /// </summary>
        protected virtual void InitializeComponent()
        {
        }

        /// <summary>
        /// Raises the <see cref="Error"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.IO.ErrorEventArgs"/> containing event data.</param>
        protected void OnError(ErrorEventArgs e)
        {
            this.RaiseEvent<ErrorEventArgs>(this.Error, e);
        }

        /// <summary>
        /// Raises the event specified by <paramref name="method"/> with the event data provided. 
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="System.EventArgs"/> used by the event delegate.</typeparam>
        /// <param name="method">The event delegate being raised.</param>
        /// <param name="e">An <typeparamref name="TEventArgs"/> containing event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "The design is ok. This method is used to raise events on multi-threaded components.")]
        protected void RaiseEvent<TEventArgs>(EventHandler<TEventArgs> method, TEventArgs e) where TEventArgs : EventArgs
        {
            if (method != null && this.CanRaiseEvents)
            {
                lock (method)
                {
                    if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
                    {
                        this.SynchronizingObject.Invoke(method, new object[] { this, e });
                    }
                    else
                    {
                        method(this, e);
                    }
                }
            }
        }

        #endregion
    }
}