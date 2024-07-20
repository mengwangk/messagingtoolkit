using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MessagingToolkit.SmartGateway.SmartClient
{
    public partial class Calendar : ScrollableControl
    {
        public Calendar()
        {
            InitializeComponent();

            this.AutoScroll = true;
            this.AutoScrollMinSize = this.Size;
        }
    }
}
