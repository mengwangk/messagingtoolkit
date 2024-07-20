namespace MessagingToolkit.SmartGateway.SmartClient
{
    partial class NavigateBarRemoteControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlCaption = new System.Windows.Forms.Panel();
            this.lnlCaption = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cboTheme = new System.Windows.Forms.ComboBox();
            this.btnEnableDisableTasks = new System.Windows.Forms.Button();
            this.btnCollapseMode = new System.Windows.Forms.Button();
            this.pgColorTable = new System.Windows.Forms.PropertyGrid();
            this.btnColorTable = new System.Windows.Forms.Button();
            this.btnShowHideCollapseButton = new System.Windows.Forms.Button();
            this.chkIsCollapsible = new System.Windows.Forms.CheckBox();
            this.btnShowHideCollapseScreen = new System.Windows.Forms.Button();
            this.chkShowOnClick = new System.Windows.Forms.CheckBox();
            this.tcNvBar = new System.Windows.Forms.TabControl();
            this.tpButtons = new System.Windows.Forms.TabPage();
            this.btnRunMailButtonClick = new System.Windows.Forms.Button();
            this.btnMenuOption = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.nudMaxDispButtonCountInPane = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nudPosition = new System.Windows.Forms.NumericUpDown();
            this.cboSelect = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudButtonCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.nudOFPHeight = new System.Windows.Forms.NumericUpDown();
            this.lblButtonHeight = new System.Windows.Forms.Label();
            this.nudButtonHeight = new System.Windows.Forms.NumericUpDown();
            this.tpCollapse = new System.Windows.Forms.TabPage();
            this.chkShowColPnlCap = new System.Windows.Forms.CheckBox();
            this.tpTheme = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.nudPaintAngle = new System.Windows.Forms.NumericUpDown();
            this.tpRTL = new System.Windows.Forms.TabPage();
            this.chkUseRTLMenu = new System.Windows.Forms.CheckBox();
            this.chkUseRTLButton = new System.Windows.Forms.CheckBox();
            this.btnRightToLeft = new System.Windows.Forms.Button();
            this.pnlCaption.SuspendLayout();
            this.tcNvBar.SuspendLayout();
            this.tpButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxDispButtonCountInPane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPosition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudButtonCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOFPHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudButtonHeight)).BeginInit();
            this.tpCollapse.SuspendLayout();
            this.tpTheme.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPaintAngle)).BeginInit();
            this.tpRTL.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCaption
            // 
            this.pnlCaption.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlCaption.Controls.Add(this.lnlCaption);
            this.pnlCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCaption.Location = new System.Drawing.Point(0, 0);
            this.pnlCaption.Name = "pnlCaption";
            this.pnlCaption.Size = new System.Drawing.Size(252, 24);
            this.pnlCaption.TabIndex = 0;
            // 
            // lnlCaption
            // 
            this.lnlCaption.AutoSize = true;
            this.lnlCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lnlCaption.Location = new System.Drawing.Point(45, 5);
            this.lnlCaption.Name = "lnlCaption";
            this.lnlCaption.Size = new System.Drawing.Size(162, 15);
            this.lnlCaption.TabIndex = 0;
            this.lnlCaption.Text = "Navigation Pane Control";
            this.lnlCaption.Click += new System.EventHandler(this.lnlCaption_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Color Theme";
            // 
            // cboTheme
            // 
            this.cboTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTheme.FormattingEnabled = true;
            this.cboTheme.Location = new System.Drawing.Point(101, 17);
            this.cboTheme.Name = "cboTheme";
            this.cboTheme.Size = new System.Drawing.Size(128, 21);
            this.cboTheme.TabIndex = 1;
            this.cboTheme.SelectedIndexChanged += new System.EventHandler(this.cboTheme_SelectedIndexChanged);
            // 
            // btnEnableDisableTasks
            // 
            this.btnEnableDisableTasks.Location = new System.Drawing.Point(6, 228);
            this.btnEnableDisableTasks.Name = "btnEnableDisableTasks";
            this.btnEnableDisableTasks.Size = new System.Drawing.Size(217, 31);
            this.btnEnableDisableTasks.TabIndex = 12;
            this.btnEnableDisableTasks.Text = "Enable / Disable \"Tasks\" Button";
            this.btnEnableDisableTasks.UseVisualStyleBackColor = true;
            this.btnEnableDisableTasks.Click += new System.EventHandler(this.btnEnableDisableTasks_Click);
            // 
            // btnCollapseMode
            // 
            this.btnCollapseMode.Location = new System.Drawing.Point(6, 56);
            this.btnCollapseMode.Name = "btnCollapseMode";
            this.btnCollapseMode.Size = new System.Drawing.Size(217, 31);
            this.btnCollapseMode.TabIndex = 1;
            this.btnCollapseMode.Text = "Change Collapse Mode Navigation Pane";
            this.btnCollapseMode.UseVisualStyleBackColor = true;
            this.btnCollapseMode.Click += new System.EventHandler(this.btnCollapseMode_Click);
            // 
            // pgColorTable
            // 
            this.pgColorTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pgColorTable.Location = new System.Drawing.Point(7, 81);
            this.pgColorTable.Name = "pgColorTable";
            this.pgColorTable.Size = new System.Drawing.Size(219, 354);
            this.pgColorTable.TabIndex = 4;
            // 
            // btnColorTable
            // 
            this.btnColorTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnColorTable.Location = new System.Drawing.Point(7, 441);
            this.btnColorTable.Name = "btnColorTable";
            this.btnColorTable.Size = new System.Drawing.Size(219, 24);
            this.btnColorTable.TabIndex = 5;
            this.btnColorTable.Text = "Apply New Color Table";
            this.btnColorTable.UseVisualStyleBackColor = true;
            this.btnColorTable.Click += new System.EventHandler(this.btnColorTable_Click);
            // 
            // btnShowHideCollapseButton
            // 
            this.btnShowHideCollapseButton.Location = new System.Drawing.Point(6, 93);
            this.btnShowHideCollapseButton.Name = "btnShowHideCollapseButton";
            this.btnShowHideCollapseButton.Size = new System.Drawing.Size(217, 31);
            this.btnShowHideCollapseButton.TabIndex = 2;
            this.btnShowHideCollapseButton.Text = "Show / Hide Collapse Button";
            this.btnShowHideCollapseButton.UseVisualStyleBackColor = true;
            this.btnShowHideCollapseButton.Click += new System.EventHandler(this.btnShowHideCollapseButton_Click);
            // 
            // chkIsCollapsible
            // 
            this.chkIsCollapsible.AutoSize = true;
            this.chkIsCollapsible.Checked = true;
            this.chkIsCollapsible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsCollapsible.Location = new System.Drawing.Point(6, 20);
            this.chkIsCollapsible.Name = "chkIsCollapsible";
            this.chkIsCollapsible.Size = new System.Drawing.Size(104, 17);
            this.chkIsCollapsible.TabIndex = 0;
            this.chkIsCollapsible.Text = "Collapsible Pane";
            this.chkIsCollapsible.UseVisualStyleBackColor = true;
            this.chkIsCollapsible.CheckedChanged += new System.EventHandler(this.chkIsCollapsible_CheckedChanged);
            // 
            // btnShowHideCollapseScreen
            // 
            this.btnShowHideCollapseScreen.Location = new System.Drawing.Point(6, 130);
            this.btnShowHideCollapseScreen.Name = "btnShowHideCollapseScreen";
            this.btnShowHideCollapseScreen.Size = new System.Drawing.Size(217, 31);
            this.btnShowHideCollapseScreen.TabIndex = 3;
            this.btnShowHideCollapseScreen.Text = "Show and Hide Collapse Screen";
            this.btnShowHideCollapseScreen.UseVisualStyleBackColor = true;
            this.btnShowHideCollapseScreen.Click += new System.EventHandler(this.btnShowHideCollapseScreen_Click);
            // 
            // chkShowOnClick
            // 
            this.chkShowOnClick.AutoSize = true;
            this.chkShowOnClick.Location = new System.Drawing.Point(6, 196);
            this.chkShowOnClick.Name = "chkShowOnClick";
            this.chkShowOnClick.Size = new System.Drawing.Size(93, 17);
            this.chkShowOnClick.TabIndex = 4;
            this.chkShowOnClick.Text = "Show on click";
            this.chkShowOnClick.UseVisualStyleBackColor = true;
            this.chkShowOnClick.CheckedChanged += new System.EventHandler(this.chkShowOnClick_CheckedChanged);
            // 
            // tcNvBar
            // 
            this.tcNvBar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tcNvBar.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tcNvBar.Controls.Add(this.tpButtons);
            this.tcNvBar.Controls.Add(this.tpCollapse);
            this.tcNvBar.Controls.Add(this.tpTheme);
            this.tcNvBar.Controls.Add(this.tpRTL);
            this.tcNvBar.HotTrack = true;
            this.tcNvBar.Location = new System.Drawing.Point(8, 30);
            this.tcNvBar.Multiline = true;
            this.tcNvBar.Name = "tcNvBar";
            this.tcNvBar.SelectedIndex = 0;
            this.tcNvBar.Size = new System.Drawing.Size(237, 499);
            this.tcNvBar.TabIndex = 0;
            // 
            // tpButtons
            // 
            this.tpButtons.Controls.Add(this.btnRunMailButtonClick);
            this.tpButtons.Controls.Add(this.btnMenuOption);
            this.tpButtons.Controls.Add(this.label7);
            this.tpButtons.Controls.Add(this.btnEnableDisableTasks);
            this.tpButtons.Controls.Add(this.nudMaxDispButtonCountInPane);
            this.tpButtons.Controls.Add(this.label6);
            this.tpButtons.Controls.Add(this.nudPosition);
            this.tpButtons.Controls.Add(this.cboSelect);
            this.tpButtons.Controls.Add(this.label5);
            this.tpButtons.Controls.Add(this.label2);
            this.tpButtons.Controls.Add(this.nudButtonCount);
            this.tpButtons.Controls.Add(this.label1);
            this.tpButtons.Controls.Add(this.nudOFPHeight);
            this.tpButtons.Controls.Add(this.lblButtonHeight);
            this.tpButtons.Controls.Add(this.nudButtonHeight);
            this.tpButtons.Location = new System.Drawing.Point(4, 25);
            this.tpButtons.Name = "tpButtons";
            this.tpButtons.Padding = new System.Windows.Forms.Padding(3);
            this.tpButtons.Size = new System.Drawing.Size(229, 470);
            this.tpButtons.TabIndex = 0;
            this.tpButtons.Text = "Buttons";
            this.tpButtons.UseVisualStyleBackColor = true;
            // 
            // btnRunMailButtonClick
            // 
            this.btnRunMailButtonClick.Location = new System.Drawing.Point(6, 299);
            this.btnRunMailButtonClick.Name = "btnRunMailButtonClick";
            this.btnRunMailButtonClick.Size = new System.Drawing.Size(217, 31);
            this.btnRunMailButtonClick.TabIndex = 14;
            this.btnRunMailButtonClick.Text = "Run Mail Button Click";
            this.btnRunMailButtonClick.UseVisualStyleBackColor = true;
            this.btnRunMailButtonClick.Click += new System.EventHandler(this.btnRunMailButtonClick_Click);
            // 
            // btnMenuOption
            // 
            this.btnMenuOption.Location = new System.Drawing.Point(6, 262);
            this.btnMenuOption.Name = "btnMenuOption";
            this.btnMenuOption.Size = new System.Drawing.Size(217, 31);
            this.btnMenuOption.TabIndex = 13;
            this.btnMenuOption.Text = "Run Navigation Pane Option Dialog";
            this.btnMenuOption.UseVisualStyleBackColor = true;
            this.btnMenuOption.Click += new System.EventHandler(this.btnMenuOption_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 63);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(159, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Max Disp. Button Count In Pane";
            // 
            // nudMaxDispButtonCountInPane
            // 
            this.nudMaxDispButtonCountInPane.Location = new System.Drawing.Point(168, 61);
            this.nudMaxDispButtonCountInPane.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudMaxDispButtonCountInPane.Name = "nudMaxDispButtonCountInPane";
            this.nudMaxDispButtonCountInPane.Size = new System.Drawing.Size(58, 20);
            this.nudMaxDispButtonCountInPane.TabIndex = 5;
            this.nudMaxDispButtonCountInPane.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nudMaxDispButtonCountInPane.ValueChanged += new System.EventHandler(this.nudMaxDispButtonCountInPane_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 188);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Change Button Position";
            // 
            // nudPosition
            // 
            this.nudPosition.Location = new System.Drawing.Point(161, 186);
            this.nudPosition.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPosition.Name = "nudPosition";
            this.nudPosition.Size = new System.Drawing.Size(65, 20);
            this.nudPosition.TabIndex = 11;
            this.nudPosition.ValueChanged += new System.EventHandler(this.nudPosition_ValueChanged);
            // 
            // cboSelect
            // 
            this.cboSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSelect.FormattingEnabled = true;
            this.cboSelect.Location = new System.Drawing.Point(98, 159);
            this.cboSelect.Name = "cboSelect";
            this.cboSelect.Size = new System.Drawing.Size(128, 21);
            this.cboSelect.TabIndex = 9;
            this.cboSelect.SelectedIndexChanged += new System.EventHandler(this.cboSelect_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Select Button";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Displayed Button Count";
            // 
            // nudButtonCount
            // 
            this.nudButtonCount.Location = new System.Drawing.Point(168, 85);
            this.nudButtonCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudButtonCount.Name = "nudButtonCount";
            this.nudButtonCount.Size = new System.Drawing.Size(58, 20);
            this.nudButtonCount.TabIndex = 7;
            this.nudButtonCount.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.nudButtonCount.ValueChanged += new System.EventHandler(this.nudButtonCount_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Over Flow Panel Height";
            // 
            // nudOFPHeight
            // 
            this.nudOFPHeight.Location = new System.Drawing.Point(168, 37);
            this.nudOFPHeight.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudOFPHeight.Name = "nudOFPHeight";
            this.nudOFPHeight.Size = new System.Drawing.Size(58, 20);
            this.nudOFPHeight.TabIndex = 3;
            this.nudOFPHeight.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudOFPHeight.ValueChanged += new System.EventHandler(this.nudOFPHeight_ValueChanged);
            // 
            // lblButtonHeight
            // 
            this.lblButtonHeight.AutoSize = true;
            this.lblButtonHeight.Location = new System.Drawing.Point(5, 17);
            this.lblButtonHeight.Name = "lblButtonHeight";
            this.lblButtonHeight.Size = new System.Drawing.Size(72, 13);
            this.lblButtonHeight.TabIndex = 0;
            this.lblButtonHeight.Text = "Button Height";
            // 
            // nudButtonHeight
            // 
            this.nudButtonHeight.Location = new System.Drawing.Point(168, 13);
            this.nudButtonHeight.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudButtonHeight.Name = "nudButtonHeight";
            this.nudButtonHeight.Size = new System.Drawing.Size(58, 20);
            this.nudButtonHeight.TabIndex = 1;
            this.nudButtonHeight.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.nudButtonHeight.ValueChanged += new System.EventHandler(this.nudButtonHeight_ValueChanged);
            // 
            // tpCollapse
            // 
            this.tpCollapse.Controls.Add(this.chkShowColPnlCap);
            this.tpCollapse.Controls.Add(this.btnShowHideCollapseButton);
            this.tpCollapse.Controls.Add(this.chkShowOnClick);
            this.tpCollapse.Controls.Add(this.btnCollapseMode);
            this.tpCollapse.Controls.Add(this.btnShowHideCollapseScreen);
            this.tpCollapse.Controls.Add(this.chkIsCollapsible);
            this.tpCollapse.Location = new System.Drawing.Point(4, 25);
            this.tpCollapse.Name = "tpCollapse";
            this.tpCollapse.Padding = new System.Windows.Forms.Padding(3);
            this.tpCollapse.Size = new System.Drawing.Size(229, 470);
            this.tpCollapse.TabIndex = 1;
            this.tpCollapse.Text = "Collapse";
            this.tpCollapse.UseVisualStyleBackColor = true;
            // 
            // chkShowColPnlCap
            // 
            this.chkShowColPnlCap.AutoSize = true;
            this.chkShowColPnlCap.Checked = true;
            this.chkShowColPnlCap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowColPnlCap.Location = new System.Drawing.Point(6, 173);
            this.chkShowColPnlCap.Name = "chkShowColPnlCap";
            this.chkShowColPnlCap.Size = new System.Drawing.Size(162, 17);
            this.chkShowColPnlCap.TabIndex = 5;
            this.chkShowColPnlCap.Text = "Show collapse panel caption";
            this.chkShowColPnlCap.UseVisualStyleBackColor = true;
            this.chkShowColPnlCap.CheckedChanged += new System.EventHandler(this.chkShowColPnlCap_CheckedChanged);
            // 
            // tpTheme
            // 
            this.tpTheme.Controls.Add(this.label3);
            this.tpTheme.Controls.Add(this.nudPaintAngle);
            this.tpTheme.Controls.Add(this.cboTheme);
            this.tpTheme.Controls.Add(this.label4);
            this.tpTheme.Controls.Add(this.pgColorTable);
            this.tpTheme.Controls.Add(this.btnColorTable);
            this.tpTheme.Location = new System.Drawing.Point(4, 25);
            this.tpTheme.Name = "tpTheme";
            this.tpTheme.Size = new System.Drawing.Size(229, 470);
            this.tpTheme.TabIndex = 2;
            this.tpTheme.Text = "Theme";
            this.tpTheme.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Paint Angle";
            // 
            // nudPaintAngle
            // 
            this.nudPaintAngle.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudPaintAngle.Location = new System.Drawing.Point(168, 44);
            this.nudPaintAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.nudPaintAngle.Name = "nudPaintAngle";
            this.nudPaintAngle.Size = new System.Drawing.Size(58, 20);
            this.nudPaintAngle.TabIndex = 3;
            this.nudPaintAngle.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.nudPaintAngle.ValueChanged += new System.EventHandler(this.nudPaintAngle_ValueChanged);
            // 
            // tpRTL
            // 
            this.tpRTL.Controls.Add(this.chkUseRTLMenu);
            this.tpRTL.Controls.Add(this.chkUseRTLButton);
            this.tpRTL.Controls.Add(this.btnRightToLeft);
            this.tpRTL.Location = new System.Drawing.Point(4, 25);
            this.tpRTL.Name = "tpRTL";
            this.tpRTL.Padding = new System.Windows.Forms.Padding(3);
            this.tpRTL.Size = new System.Drawing.Size(229, 470);
            this.tpRTL.TabIndex = 3;
            this.tpRTL.Text = "RTL";
            this.tpRTL.UseVisualStyleBackColor = true;
            // 
            // chkUseRTLMenu
            // 
            this.chkUseRTLMenu.AutoSize = true;
            this.chkUseRTLMenu.Checked = true;
            this.chkUseRTLMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseRTLMenu.Location = new System.Drawing.Point(8, 66);
            this.chkUseRTLMenu.Name = "chkUseRTLMenu";
            this.chkUseRTLMenu.Size = new System.Drawing.Size(118, 17);
            this.chkUseRTLMenu.TabIndex = 18;
            this.chkUseRTLMenu.Text = "Use RTL for menus";
            this.chkUseRTLMenu.UseVisualStyleBackColor = true;
            this.chkUseRTLMenu.CheckedChanged += new System.EventHandler(this.chkUseRTLMenu_CheckedChanged);
            // 
            // chkUseRTLButton
            // 
            this.chkUseRTLButton.AutoSize = true;
            this.chkUseRTLButton.Checked = true;
            this.chkUseRTLButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseRTLButton.Location = new System.Drawing.Point(8, 43);
            this.chkUseRTLButton.Name = "chkUseRTLButton";
            this.chkUseRTLButton.Size = new System.Drawing.Size(122, 17);
            this.chkUseRTLButton.TabIndex = 17;
            this.chkUseRTLButton.Text = "Use RTL for buttons";
            this.chkUseRTLButton.UseVisualStyleBackColor = true;
            this.chkUseRTLButton.CheckedChanged += new System.EventHandler(this.chkUseRTLButton_CheckedChanged);
            // 
            // btnRightToLeft
            // 
            this.btnRightToLeft.Location = new System.Drawing.Point(6, 6);
            this.btnRightToLeft.Name = "btnRightToLeft";
            this.btnRightToLeft.Size = new System.Drawing.Size(217, 31);
            this.btnRightToLeft.TabIndex = 16;
            this.btnRightToLeft.Text = "Right To Left";
            this.btnRightToLeft.UseVisualStyleBackColor = true;
            this.btnRightToLeft.Click += new System.EventHandler(this.btnRightToLeft_Click);
            // 
            // NavigateBarRemoteControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcNvBar);
            this.Controls.Add(this.pnlCaption);
            this.Name = "NavigateBarRemoteControl";
            this.Size = new System.Drawing.Size(252, 532);
            this.pnlCaption.ResumeLayout(false);
            this.pnlCaption.PerformLayout();
            this.tcNvBar.ResumeLayout(false);
            this.tpButtons.ResumeLayout(false);
            this.tpButtons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxDispButtonCountInPane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPosition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudButtonCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudOFPHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudButtonHeight)).EndInit();
            this.tpCollapse.ResumeLayout(false);
            this.tpCollapse.PerformLayout();
            this.tpTheme.ResumeLayout(false);
            this.tpTheme.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPaintAngle)).EndInit();
            this.tpRTL.ResumeLayout(false);
            this.tpRTL.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlCaption;
        private System.Windows.Forms.Label lnlCaption;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboTheme;
        private System.Windows.Forms.Button btnEnableDisableTasks;
        private System.Windows.Forms.Button btnCollapseMode;
        private System.Windows.Forms.PropertyGrid pgColorTable;
        private System.Windows.Forms.Button btnColorTable;
        private System.Windows.Forms.Button btnShowHideCollapseButton;
        private System.Windows.Forms.CheckBox chkIsCollapsible;
        private System.Windows.Forms.Button btnShowHideCollapseScreen;
        private System.Windows.Forms.CheckBox chkShowOnClick;
        private System.Windows.Forms.TabControl tcNvBar;
        private System.Windows.Forms.TabPage tpButtons;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.NumericUpDown nudMaxDispButtonCountInPane;
        private System.Windows.Forms.Label label6;
        internal System.Windows.Forms.NumericUpDown nudPosition;
        private System.Windows.Forms.ComboBox cboSelect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.NumericUpDown nudButtonCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudOFPHeight;
        private System.Windows.Forms.Label lblButtonHeight;
        private System.Windows.Forms.NumericUpDown nudButtonHeight;
        private System.Windows.Forms.TabPage tpCollapse;
        private System.Windows.Forms.TabPage tpTheme;
        private System.Windows.Forms.Button btnMenuOption;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudPaintAngle;
        private System.Windows.Forms.CheckBox chkShowColPnlCap;
        private System.Windows.Forms.TabPage tpRTL;
        private System.Windows.Forms.CheckBox chkUseRTLMenu;
        private System.Windows.Forms.CheckBox chkUseRTLButton;
        private System.Windows.Forms.Button btnRightToLeft;
        private System.Windows.Forms.Button btnRunMailButtonClick;
    }
}
