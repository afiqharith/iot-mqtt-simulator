namespace ModelInterface
{
    partial class ListenerWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.DataGridViewBitSet = new System.Windows.Forms.DataGridView();
            this.hardwareControllerFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxUnitLamp1 = new System.Windows.Forms.CheckBox();
            this.checkBoxUnitFan1 = new System.Windows.Forms.CheckBox();
            this.checkBoxLocGroup1 = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBoxLocGroup2 = new System.Windows.Forms.CheckBox();
            this.checkBoxUnitFan2 = new System.Windows.Forms.CheckBox();
            this.checkBoxUnitLamp2 = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.checkBoxLocGroup3 = new System.Windows.Forms.CheckBox();
            this.checkBoxUnitFan3 = new System.Windows.Forms.CheckBox();
            this.checkBoxUnitLamp3 = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.checkBoxLocGroup4 = new System.Windows.Forms.CheckBox();
            this.checkBoxUnitFan4 = new System.Windows.Forms.CheckBox();
            this.checkBoxUnitLamp4 = new System.Windows.Forms.CheckBox();
            this.checkBoxShutdownAll = new System.Windows.Forms.CheckBox();
            this.hardwareViewerFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewBitSet)).BeginInit();
            this.hardwareControllerFlowLayoutPanel.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.hardwareControllerFlowLayoutPanel);
            this.panel1.Controls.Add(this.hardwareViewerFlowLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(558, 454);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(9, 124);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(540, 221);
            this.tabControl1.TabIndex = 34;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.richTextBox2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(532, 195);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Controller";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // richTextBox2
            // 
            this.richTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox2.Location = new System.Drawing.Point(5, 5);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(522, 184);
            this.richTextBox2.TabIndex = 33;
            this.richTextBox2.Text = "";
            this.richTextBox2.WordWrap = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBox1);
            this.tabPage2.Controls.Add(this.DataGridViewBitSet);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(532, 195);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Listener";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(4, 60);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(522, 129);
            this.richTextBox1.TabIndex = 14;
            this.richTextBox1.Text = "";
            // 
            // DataGridViewBitSet
            // 
            this.DataGridViewBitSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridViewBitSet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DataGridViewBitSet.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridViewBitSet.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridViewBitSet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridViewBitSet.Enabled = false;
            this.DataGridViewBitSet.Location = new System.Drawing.Point(5, 6);
            this.DataGridViewBitSet.Name = "DataGridViewBitSet";
            this.DataGridViewBitSet.RowHeadersVisible = false;
            this.DataGridViewBitSet.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.DataGridViewBitSet.Size = new System.Drawing.Size(522, 48);
            this.DataGridViewBitSet.TabIndex = 15;
            // 
            // hardwareControllerFlowLayoutPanel
            // 
            this.hardwareControllerFlowLayoutPanel.AutoScroll = true;
            this.hardwareControllerFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hardwareControllerFlowLayoutPanel.Controls.Add(this.groupBox5);
            this.hardwareControllerFlowLayoutPanel.Controls.Add(this.groupBox6);
            this.hardwareControllerFlowLayoutPanel.Controls.Add(this.groupBox7);
            this.hardwareControllerFlowLayoutPanel.Controls.Add(this.groupBox8);
            this.hardwareControllerFlowLayoutPanel.Controls.Add(this.checkBoxShutdownAll);
            this.hardwareControllerFlowLayoutPanel.Location = new System.Drawing.Point(9, 351);
            this.hardwareControllerFlowLayoutPanel.Name = "hardwareControllerFlowLayoutPanel";
            this.hardwareControllerFlowLayoutPanel.Size = new System.Drawing.Size(536, 97);
            this.hardwareControllerFlowLayoutPanel.TabIndex = 32;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxUnitLamp1);
            this.groupBox5.Controls.Add(this.checkBoxUnitFan1);
            this.groupBox5.Controls.Add(this.checkBoxLocGroup1);
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(92, 91);
            this.groupBox5.TabIndex = 28;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Group Loc1";
            // 
            // checkBoxUnitLamp1
            // 
            this.checkBoxUnitLamp1.AutoSize = true;
            this.checkBoxUnitLamp1.Location = new System.Drawing.Point(6, 42);
            this.checkBoxUnitLamp1.Name = "checkBoxUnitLamp1";
            this.checkBoxUnitLamp1.Size = new System.Drawing.Size(75, 17);
            this.checkBoxUnitLamp1.TabIndex = 15;
            this.checkBoxUnitLamp1.Text = "Lamp: ID1";
            this.checkBoxUnitLamp1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxUnitLamp1.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnitFan1
            // 
            this.checkBoxUnitFan1.AutoSize = true;
            this.checkBoxUnitFan1.Location = new System.Drawing.Point(6, 65);
            this.checkBoxUnitFan1.Name = "checkBoxUnitFan1";
            this.checkBoxUnitFan1.Size = new System.Drawing.Size(67, 17);
            this.checkBoxUnitFan1.TabIndex = 16;
            this.checkBoxUnitFan1.Text = "Fan: ID1";
            this.checkBoxUnitFan1.UseVisualStyleBackColor = true;
            // 
            // checkBoxLocGroup1
            // 
            this.checkBoxLocGroup1.AutoSize = true;
            this.checkBoxLocGroup1.Location = new System.Drawing.Point(6, 19);
            this.checkBoxLocGroup1.Name = "checkBoxLocGroup1";
            this.checkBoxLocGroup1.Size = new System.Drawing.Size(48, 17);
            this.checkBoxLocGroup1.TabIndex = 23;
            this.checkBoxLocGroup1.Text = "Both";
            this.checkBoxLocGroup1.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBoxLocGroup2);
            this.groupBox6.Controls.Add(this.checkBoxUnitFan2);
            this.groupBox6.Controls.Add(this.checkBoxUnitLamp2);
            this.groupBox6.Location = new System.Drawing.Point(101, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(92, 91);
            this.groupBox6.TabIndex = 29;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Group Loc2";
            // 
            // checkBoxLocGroup2
            // 
            this.checkBoxLocGroup2.AutoSize = true;
            this.checkBoxLocGroup2.Location = new System.Drawing.Point(6, 19);
            this.checkBoxLocGroup2.Name = "checkBoxLocGroup2";
            this.checkBoxLocGroup2.Size = new System.Drawing.Size(48, 17);
            this.checkBoxLocGroup2.TabIndex = 25;
            this.checkBoxLocGroup2.Text = "Both";
            this.checkBoxLocGroup2.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnitFan2
            // 
            this.checkBoxUnitFan2.AutoSize = true;
            this.checkBoxUnitFan2.Location = new System.Drawing.Point(6, 65);
            this.checkBoxUnitFan2.Name = "checkBoxUnitFan2";
            this.checkBoxUnitFan2.Size = new System.Drawing.Size(67, 17);
            this.checkBoxUnitFan2.TabIndex = 18;
            this.checkBoxUnitFan2.Text = "Fan: ID2";
            this.checkBoxUnitFan2.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnitLamp2
            // 
            this.checkBoxUnitLamp2.AutoSize = true;
            this.checkBoxUnitLamp2.Location = new System.Drawing.Point(6, 42);
            this.checkBoxUnitLamp2.Name = "checkBoxUnitLamp2";
            this.checkBoxUnitLamp2.Size = new System.Drawing.Size(75, 17);
            this.checkBoxUnitLamp2.TabIndex = 17;
            this.checkBoxUnitLamp2.Text = "Lamp: ID2";
            this.checkBoxUnitLamp2.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.checkBoxLocGroup3);
            this.groupBox7.Controls.Add(this.checkBoxUnitFan3);
            this.groupBox7.Controls.Add(this.checkBoxUnitLamp3);
            this.groupBox7.Location = new System.Drawing.Point(199, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(92, 91);
            this.groupBox7.TabIndex = 30;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Group Loc3";
            // 
            // checkBoxLocGroup3
            // 
            this.checkBoxLocGroup3.AutoSize = true;
            this.checkBoxLocGroup3.Location = new System.Drawing.Point(6, 20);
            this.checkBoxLocGroup3.Name = "checkBoxLocGroup3";
            this.checkBoxLocGroup3.Size = new System.Drawing.Size(48, 17);
            this.checkBoxLocGroup3.TabIndex = 26;
            this.checkBoxLocGroup3.Text = "Both";
            this.checkBoxLocGroup3.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnitFan3
            // 
            this.checkBoxUnitFan3.AutoSize = true;
            this.checkBoxUnitFan3.Location = new System.Drawing.Point(6, 66);
            this.checkBoxUnitFan3.Name = "checkBoxUnitFan3";
            this.checkBoxUnitFan3.Size = new System.Drawing.Size(67, 17);
            this.checkBoxUnitFan3.TabIndex = 22;
            this.checkBoxUnitFan3.Text = "Fan: ID3";
            this.checkBoxUnitFan3.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnitLamp3
            // 
            this.checkBoxUnitLamp3.AutoSize = true;
            this.checkBoxUnitLamp3.Location = new System.Drawing.Point(6, 43);
            this.checkBoxUnitLamp3.Name = "checkBoxUnitLamp3";
            this.checkBoxUnitLamp3.Size = new System.Drawing.Size(75, 17);
            this.checkBoxUnitLamp3.TabIndex = 21;
            this.checkBoxUnitLamp3.Text = "Lamp: ID3";
            this.checkBoxUnitLamp3.UseVisualStyleBackColor = true;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.checkBoxLocGroup4);
            this.groupBox8.Controls.Add(this.checkBoxUnitFan4);
            this.groupBox8.Controls.Add(this.checkBoxUnitLamp4);
            this.groupBox8.Location = new System.Drawing.Point(297, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(92, 91);
            this.groupBox8.TabIndex = 30;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Group Loc4";
            // 
            // checkBoxLocGroup4
            // 
            this.checkBoxLocGroup4.AutoSize = true;
            this.checkBoxLocGroup4.Location = new System.Drawing.Point(6, 21);
            this.checkBoxLocGroup4.Name = "checkBoxLocGroup4";
            this.checkBoxLocGroup4.Size = new System.Drawing.Size(48, 17);
            this.checkBoxLocGroup4.TabIndex = 24;
            this.checkBoxLocGroup4.Text = "Both";
            this.checkBoxLocGroup4.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnitFan4
            // 
            this.checkBoxUnitFan4.AutoSize = true;
            this.checkBoxUnitFan4.Location = new System.Drawing.Point(6, 67);
            this.checkBoxUnitFan4.Name = "checkBoxUnitFan4";
            this.checkBoxUnitFan4.Size = new System.Drawing.Size(67, 17);
            this.checkBoxUnitFan4.TabIndex = 20;
            this.checkBoxUnitFan4.Text = "Fan: ID4";
            this.checkBoxUnitFan4.UseVisualStyleBackColor = true;
            // 
            // checkBoxUnitLamp4
            // 
            this.checkBoxUnitLamp4.AutoSize = true;
            this.checkBoxUnitLamp4.Location = new System.Drawing.Point(6, 44);
            this.checkBoxUnitLamp4.Name = "checkBoxUnitLamp4";
            this.checkBoxUnitLamp4.Size = new System.Drawing.Size(75, 17);
            this.checkBoxUnitLamp4.TabIndex = 19;
            this.checkBoxUnitLamp4.Text = "Lamp: ID4";
            this.checkBoxUnitLamp4.UseVisualStyleBackColor = true;
            // 
            // checkBoxShutdownAll
            // 
            this.checkBoxShutdownAll.AutoSize = true;
            this.checkBoxShutdownAll.Location = new System.Drawing.Point(395, 3);
            this.checkBoxShutdownAll.Name = "checkBoxShutdownAll";
            this.checkBoxShutdownAll.Size = new System.Drawing.Size(37, 17);
            this.checkBoxShutdownAll.TabIndex = 27;
            this.checkBoxShutdownAll.Text = "All";
            this.checkBoxShutdownAll.UseVisualStyleBackColor = true;
            // 
            // hardwareViewerFlowLayoutPanel
            // 
            this.hardwareViewerFlowLayoutPanel.AutoScroll = true;
            this.hardwareViewerFlowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hardwareViewerFlowLayoutPanel.Location = new System.Drawing.Point(9, 12);
            this.hardwareViewerFlowLayoutPanel.Name = "hardwareViewerFlowLayoutPanel";
            this.hardwareViewerFlowLayoutPanel.Size = new System.Drawing.Size(540, 106);
            this.hardwareViewerFlowLayoutPanel.TabIndex = 19;
            // 
            // ListenerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 454);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ListenerWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simulation Overview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridViewBitSet)).EndInit();
            this.hardwareControllerFlowLayoutPanel.ResumeLayout(false);
            this.hardwareControllerFlowLayoutPanel.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.DataGridView DataGridViewBitSet;
        private System.Windows.Forms.FlowLayoutPanel hardwareViewerFlowLayoutPanel;
        private System.Windows.Forms.FlowLayoutPanel hardwareControllerFlowLayoutPanel;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBoxUnitLamp1;
        private System.Windows.Forms.CheckBox checkBoxUnitFan1;
        private System.Windows.Forms.CheckBox checkBoxLocGroup1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox checkBoxLocGroup2;
        private System.Windows.Forms.CheckBox checkBoxUnitFan2;
        private System.Windows.Forms.CheckBox checkBoxUnitLamp2;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox checkBoxLocGroup3;
        private System.Windows.Forms.CheckBox checkBoxUnitFan3;
        private System.Windows.Forms.CheckBox checkBoxUnitLamp3;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox checkBoxLocGroup4;
        private System.Windows.Forms.CheckBox checkBoxUnitFan4;
        private System.Windows.Forms.CheckBox checkBoxUnitLamp4;
        private System.Windows.Forms.CheckBox checkBoxShutdownAll;
        private System.Windows.Forms.RichTextBox richTextBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}

