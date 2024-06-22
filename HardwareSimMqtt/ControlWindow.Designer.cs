namespace HardwareSimMqtt
{
    partial class ControlWindow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBoxLoc3 = new System.Windows.Forms.CheckBox();
            this.checkBoxFan3 = new System.Windows.Forms.CheckBox();
            this.checkBoxLamp3 = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxLoc4 = new System.Windows.Forms.CheckBox();
            this.checkBoxFan4 = new System.Windows.Forms.CheckBox();
            this.checkBoxLamp4 = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxLoc2 = new System.Windows.Forms.CheckBox();
            this.checkBoxFan2 = new System.Windows.Forms.CheckBox();
            this.checkBoxLamp2 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxLamp1 = new System.Windows.Forms.CheckBox();
            this.checkBoxFan1 = new System.Windows.Forms.CheckBox();
            this.checkBoxLoc1 = new System.Windows.Forms.CheckBox();
            this.checkBoxShutdownAll = new System.Windows.Forms.CheckBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.checkBoxShutdownAll);
            this.panel1.Controls.Add(this.richTextBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(457, 213);
            this.panel1.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBoxLoc3);
            this.groupBox3.Controls.Add(this.checkBoxFan3);
            this.groupBox3.Controls.Add(this.checkBoxLamp3);
            this.groupBox3.Location = new System.Drawing.Point(214, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(92, 91);
            this.groupBox3.TabIndex = 30;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Group Loc3";
            // 
            // checkBoxLoc3
            // 
            this.checkBoxLoc3.AutoSize = true;
            this.checkBoxLoc3.Location = new System.Drawing.Point(6, 20);
            this.checkBoxLoc3.Name = "checkBoxLoc3";
            this.checkBoxLoc3.Size = new System.Drawing.Size(48, 17);
            this.checkBoxLoc3.TabIndex = 26;
            this.checkBoxLoc3.Text = "Both";
            this.checkBoxLoc3.UseVisualStyleBackColor = true;
            this.checkBoxLoc3.CheckStateChanged += new System.EventHandler(this.CheckboxLoc_CheckStateChanged);
            // 
            // checkBoxFan3
            // 
            this.checkBoxFan3.AutoSize = true;
            this.checkBoxFan3.Location = new System.Drawing.Point(6, 66);
            this.checkBoxFan3.Name = "checkBoxFan3";
            this.checkBoxFan3.Size = new System.Drawing.Size(67, 17);
            this.checkBoxFan3.TabIndex = 22;
            this.checkBoxFan3.Text = "Fan: ID3";
            this.checkBoxFan3.UseVisualStyleBackColor = true;
            // 
            // checkBoxLamp3
            // 
            this.checkBoxLamp3.AutoSize = true;
            this.checkBoxLamp3.Location = new System.Drawing.Point(6, 43);
            this.checkBoxLamp3.Name = "checkBoxLamp3";
            this.checkBoxLamp3.Size = new System.Drawing.Size(75, 17);
            this.checkBoxLamp3.TabIndex = 21;
            this.checkBoxLamp3.Text = "Lamp: ID3";
            this.checkBoxLamp3.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxLoc4);
            this.groupBox4.Controls.Add(this.checkBoxFan4);
            this.groupBox4.Controls.Add(this.checkBoxLamp4);
            this.groupBox4.Location = new System.Drawing.Point(315, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(92, 91);
            this.groupBox4.TabIndex = 30;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Group Loc4";
            // 
            // checkBoxLoc4
            // 
            this.checkBoxLoc4.AutoSize = true;
            this.checkBoxLoc4.Location = new System.Drawing.Point(6, 21);
            this.checkBoxLoc4.Name = "checkBoxLoc4";
            this.checkBoxLoc4.Size = new System.Drawing.Size(48, 17);
            this.checkBoxLoc4.TabIndex = 24;
            this.checkBoxLoc4.Text = "Both";
            this.checkBoxLoc4.UseVisualStyleBackColor = true;
            this.checkBoxLoc4.CheckStateChanged += new System.EventHandler(this.CheckboxLoc_CheckStateChanged);
            // 
            // checkBoxFan4
            // 
            this.checkBoxFan4.AutoSize = true;
            this.checkBoxFan4.Location = new System.Drawing.Point(6, 67);
            this.checkBoxFan4.Name = "checkBoxFan4";
            this.checkBoxFan4.Size = new System.Drawing.Size(67, 17);
            this.checkBoxFan4.TabIndex = 20;
            this.checkBoxFan4.Text = "Fan: ID4";
            this.checkBoxFan4.UseVisualStyleBackColor = true;
            // 
            // checkBoxLamp4
            // 
            this.checkBoxLamp4.AutoSize = true;
            this.checkBoxLamp4.Location = new System.Drawing.Point(6, 44);
            this.checkBoxLamp4.Name = "checkBoxLamp4";
            this.checkBoxLamp4.Size = new System.Drawing.Size(75, 17);
            this.checkBoxLamp4.TabIndex = 19;
            this.checkBoxLamp4.Text = "Lamp: ID4";
            this.checkBoxLamp4.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxLoc2);
            this.groupBox2.Controls.Add(this.checkBoxFan2);
            this.groupBox2.Controls.Add(this.checkBoxLamp2);
            this.groupBox2.Location = new System.Drawing.Point(113, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(92, 91);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Group Loc2";
            // 
            // checkBoxLoc2
            // 
            this.checkBoxLoc2.AutoSize = true;
            this.checkBoxLoc2.Location = new System.Drawing.Point(6, 19);
            this.checkBoxLoc2.Name = "checkBoxLoc2";
            this.checkBoxLoc2.Size = new System.Drawing.Size(48, 17);
            this.checkBoxLoc2.TabIndex = 25;
            this.checkBoxLoc2.Text = "Both";
            this.checkBoxLoc2.UseVisualStyleBackColor = true;
            this.checkBoxLoc2.CheckStateChanged += new System.EventHandler(this.CheckboxLoc_CheckStateChanged);
            // 
            // checkBoxFan2
            // 
            this.checkBoxFan2.AutoSize = true;
            this.checkBoxFan2.Location = new System.Drawing.Point(6, 65);
            this.checkBoxFan2.Name = "checkBoxFan2";
            this.checkBoxFan2.Size = new System.Drawing.Size(67, 17);
            this.checkBoxFan2.TabIndex = 18;
            this.checkBoxFan2.Text = "Fan: ID2";
            this.checkBoxFan2.UseVisualStyleBackColor = true;
            // 
            // checkBoxLamp2
            // 
            this.checkBoxLamp2.AutoSize = true;
            this.checkBoxLamp2.Location = new System.Drawing.Point(6, 42);
            this.checkBoxLamp2.Name = "checkBoxLamp2";
            this.checkBoxLamp2.Size = new System.Drawing.Size(75, 17);
            this.checkBoxLamp2.TabIndex = 17;
            this.checkBoxLamp2.Text = "Lamp: ID2";
            this.checkBoxLamp2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxLamp1);
            this.groupBox1.Controls.Add(this.checkBoxFan1);
            this.groupBox1.Controls.Add(this.checkBoxLoc1);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(92, 91);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Group Loc1";
            // 
            // checkBoxLamp1
            // 
            this.checkBoxLamp1.AutoSize = true;
            this.checkBoxLamp1.Location = new System.Drawing.Point(6, 42);
            this.checkBoxLamp1.Name = "checkBoxLamp1";
            this.checkBoxLamp1.Size = new System.Drawing.Size(75, 17);
            this.checkBoxLamp1.TabIndex = 15;
            this.checkBoxLamp1.Text = "Lamp: ID1";
            this.checkBoxLamp1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxLamp1.UseVisualStyleBackColor = true;
            // 
            // checkBoxFan1
            // 
            this.checkBoxFan1.AutoSize = true;
            this.checkBoxFan1.Location = new System.Drawing.Point(6, 65);
            this.checkBoxFan1.Name = "checkBoxFan1";
            this.checkBoxFan1.Size = new System.Drawing.Size(67, 17);
            this.checkBoxFan1.TabIndex = 16;
            this.checkBoxFan1.Text = "Fan: ID1";
            this.checkBoxFan1.UseVisualStyleBackColor = true;
            // 
            // checkBoxLoc1
            // 
            this.checkBoxLoc1.AutoSize = true;
            this.checkBoxLoc1.Location = new System.Drawing.Point(6, 19);
            this.checkBoxLoc1.Name = "checkBoxLoc1";
            this.checkBoxLoc1.Size = new System.Drawing.Size(48, 17);
            this.checkBoxLoc1.TabIndex = 23;
            this.checkBoxLoc1.Text = "Both";
            this.checkBoxLoc1.UseVisualStyleBackColor = true;
            this.checkBoxLoc1.CheckStateChanged += new System.EventHandler(this.CheckboxLoc_CheckStateChanged);
            // 
            // checkBoxShutdownAll
            // 
            this.checkBoxShutdownAll.AutoSize = true;
            this.checkBoxShutdownAll.Location = new System.Drawing.Point(413, 83);
            this.checkBoxShutdownAll.Name = "checkBoxShutdownAll";
            this.checkBoxShutdownAll.Size = new System.Drawing.Size(37, 17);
            this.checkBoxShutdownAll.TabIndex = 27;
            this.checkBoxShutdownAll.Text = "All";
            this.checkBoxShutdownAll.UseVisualStyleBackColor = true;
            this.checkBoxShutdownAll.CheckStateChanged += new System.EventHandler(this.CheckboxLoc_CheckStateChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 106);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(435, 99);
            this.richTextBox1.TabIndex = 14;
            this.richTextBox1.Text = "";
            // 
            // ControlWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 213);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ControlWindow";
            this.Text = "Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlWindow_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxFan3;
        private System.Windows.Forms.CheckBox checkBoxLamp3;
        private System.Windows.Forms.CheckBox checkBoxFan4;
        private System.Windows.Forms.CheckBox checkBoxLamp4;
        private System.Windows.Forms.CheckBox checkBoxFan2;
        private System.Windows.Forms.CheckBox checkBoxLamp2;
        private System.Windows.Forms.CheckBox checkBoxFan1;
        private System.Windows.Forms.CheckBox checkBoxLamp1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.CheckBox checkBoxLoc3;
        private System.Windows.Forms.CheckBox checkBoxLoc2;
        private System.Windows.Forms.CheckBox checkBoxLoc4;
        private System.Windows.Forms.CheckBox checkBoxLoc1;
        private System.Windows.Forms.CheckBox checkBoxShutdownAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}