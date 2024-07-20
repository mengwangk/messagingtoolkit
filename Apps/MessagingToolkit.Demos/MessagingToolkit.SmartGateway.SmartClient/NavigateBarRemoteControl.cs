using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using MessagingToolkit.UI;

namespace MessagingToolkit.SmartGateway.SmartClient
{
    public partial class NavigateBarRemoteControl : UserControl
    {

        NavigateBar navigateBar;
        NavigateBarColorTable colorTable;

        public NavigateBarRemoteControl(NavigateBar tNavigatebar)
        {
            InitializeComponent();

            navigateBar = tNavigatebar;

            // Combo Theme

            cboTheme.Items.Add("System Colors");
            cboTheme.Items.Add("Office 2003 Style Blue");
            cboTheme.Items.Add("Office 2003 Style Silver");
            cboTheme.Items.Add("Office 2003 Style Olive");
            cboTheme.Items.Add("Office 2007 Style Blue");
            cboTheme.Items.Add("Office 2007 Style Black");
            cboTheme.Items.Add("Office 2007 Style Silver");

            // Combo Select Button

            foreach (NavigateBarButton nvb in navigateBar.NavigateBarButtons)
                cboSelect.Items.Add(nvb.ToString());

            // Property Grid
            colorTable = navigateBar.NavigateBarColorTable;
            pgColorTable.SelectedObject = colorTable;
        }

        /// <summary>
        /// Change buttons height
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudButtonHeight_ValueChanged(object sender, EventArgs e)
        {
            navigateBar.NavigateBarButtonHeight = (int)nudButtonHeight.Value;
        }

        /// <summary>
        /// Programatic change collapse mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCollapseMode_Click(object sender, EventArgs e)
        {
            if (navigateBar.IsCollapsedMode)
                navigateBar.ChangeCollapseMode(false);
            else
                navigateBar.ChangeCollapseMode(true);
        }

        /// <summary>
        /// Run navigate pane menu options dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMenuOption_Click(object sender, EventArgs e)
        {
            navigateBar.RunMenuOptionsDialog();
        }

        /// <summary>
        /// Apply new color table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColorTable_Click(object sender, EventArgs e)
        {
            navigateBar.NavigateBarColorTable = colorTable;
        }

        /// <summary>
        /// Change enabled state "Tasks" button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnableDisableTasks_Click(object sender, EventArgs e)
        {
            NavigateBarButton nvb = navigateBar.NavigateBarButtons.FindByKey("TASKS");
            if (nvb != null)
                nvb.Enabled = !nvb.Enabled;

            // OR
            //nvbTasks.Enabled = !nvbTasks.Enabled;
        }

        /// <summary>
        /// Programatic change selected button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (NavigateBarButton nvb in navigateBar.NavigateBarButtons)
            {
                if (nvb.Caption.Equals(cboSelect.SelectedItem.ToString()))
                {
                    navigateBar.SelectedButton = nvb;
                    break;
                }
            }

            // OR
            // navigateBar.SelectedButton = nvbTasks;
        }

        /// <summary>
        /// Change color theme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cboTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboTheme.SelectedIndex)
            {
                case 0:
                    {
                        navigateBar.NavigateBarColorTable = NavigateBarColorTable.SystemColor;
                        break;
                    }
                case 1:
                    {
                        navigateBar.NavigateBarColorTable = NavigateBarColorTable.Office2003Blue;
                        break;
                    }
                case 2:
                    {
                        navigateBar.NavigateBarColorTable = NavigateBarColorTable.Office2003Silver;
                        break;
                    }
                case 3:
                    {
                        navigateBar.NavigateBarColorTable = NavigateBarColorTable.Office2003Olive;
                        break;
                    }
                case 4:
                    {
                        navigateBar.NavigateBarColorTable = NavigateBarColorTable.Office2007Blue;
                        break;
                    }
                case 5:
                    {
                        navigateBar.NavigateBarColorTable = NavigateBarColorTable.Office2007Black;
                        break;
                    }
                case 6:
                    {
                        navigateBar.NavigateBarColorTable = NavigateBarColorTable.Office2007Silver;
                        break;
                    }
            }

            colorTable = navigateBar.NavigateBarColorTable;
            pgColorTable.SelectedObject = colorTable;

        }

        /// <summary>
        /// Change paint angle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudPaintAngle_ValueChanged(object sender, EventArgs e)
        {
            navigateBar.NavigateBarColorTable.PaintAngle = (float)nudPaintAngle.Value;
            navigateBar.Invalidate();
        }

        /// <summary>
        /// Change displayed button count in pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudButtonCount_ValueChanged(object sender, EventArgs e)
        {
            navigateBar.DisplayedButtonCount = (int)nudButtonCount.Value;
        }

        /// <summary>
        /// Change overflow panel height
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudOFPHeight_ValueChanged(object sender, EventArgs e)
        {
            navigateBar.OverFlowPanelHeight = (int)nudOFPHeight.Value;
        }

        /// <summary>
        /// Change button postion in navigation pane and buttons collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudPosition_ValueChanged(object sender, EventArgs e)
        {
            navigateBar.ChangeButtonPosition(navigateBar.SelectedButton, (int)nudPosition.Value);
        }

        /// <summary>
        /// Hide collapse button on caption band
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowHideCollapseButton_Click(object sender, EventArgs e)
        {
            navigateBar.IsShowCollapseButton = !navigateBar.IsShowCollapseButton;
        }

        /// <summary>
        /// Use (or don't) collapsible mode navigation pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIsCollapsible_CheckedChanged(object sender, EventArgs e)
        {
            navigateBar.IsCollapsible = chkIsCollapsible.Checked;
            btnCollapseMode.Enabled = chkIsCollapsible.Checked;
            btnShowHideCollapseButton.Enabled = chkIsCollapsible.Checked;
            btnShowHideCollapseScreen.Enabled = chkIsCollapsible.Checked;
            btnCollapseMode.Enabled = chkIsCollapsible.Checked;
            chkShowOnClick.Enabled = chkIsCollapsible.Checked;
            chkShowColPnlCap.Enabled = chkIsCollapsible.Checked;
        }

        /// <summary>
        /// Programatic hide / show collapse screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowHideCollapseScreen_Click(object sender, EventArgs e)
        {

            if (!navigateBar.IsCollapsedMode)
                return;


            if (!navigateBar.IsCollapsibleScreenShowNow)
                navigateBar.ShowCollapseScreen();

            System.Threading.Thread.Sleep(4);

            if (navigateBar.IsCollapsibleScreenShowNow)
                navigateBar.HideCollapseScreen();


        }

        /// <summary>
        /// If collapse mode then show collapse screen each click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowOnClick_CheckedChanged(object sender, EventArgs e)
        {
            navigateBar.IsCollapseScreenShowOnButtonSelect = chkShowOnClick.Checked;
        }

        /// <summary>
        /// Max displayed button count in navigation pane
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudMaxDispButtonCountInPane_ValueChanged(object sender, EventArgs e)
        {
            navigateBar.NavigateBarDisplayedButtonCount = (int)nudMaxDispButtonCountInPane.Value;
        }

        /// <summary>
        /// Show / hide collapse panel screen caption
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowColPnlCap_CheckedChanged(object sender, EventArgs e)
        {
            foreach (NavigateBarButton nvb in navigateBar.NavigateBarButtons)
                nvb.IsShowCollapseScreenCaption = chkShowColPnlCap.Checked;
        }

        /// <summary>
        /// Change RightToLeft Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRightToLeft_Click(object sender, EventArgs e)
        {
            if (navigateBar.RightToLeft != RightToLeft.Yes)
                navigateBar.RightToLeft = RightToLeft.Yes;
            else
                navigateBar.RightToLeft = RightToLeft.Inherit;
        }

        /// <summary>
        /// Use RightToLeft for button's
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseRTLButton_CheckedChanged(object sender, EventArgs e)
        {
            navigateBar.IsUseRTLforButtons = chkUseRTLButton.Checked;
        }

        /// <summary>
        /// Use RTL for menu's
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkUseRTLMenu_CheckedChanged(object sender, EventArgs e)
        {
            navigateBar.IsUseRTLforMenus = chkUseRTLMenu.Checked;
        }

        /// <summary>
        /// Run mail button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRunMailButtonClick_Click(object sender, EventArgs e)
        {
            NavigateBarButton nvbMail = navigateBar.NavigateBarButtons.FindByKey("MAIL");

            if (nvbMail == null) 
                return;

            nvbMail.PerformClick();

        }

        private void lnlCaption_Click(object sender, EventArgs e)
        {

        }

    }
}
