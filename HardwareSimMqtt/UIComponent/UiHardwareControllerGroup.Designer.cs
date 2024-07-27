namespace HardwareSimMqtt.UIComponent
{
    partial class UiHardwareControllerGroup
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
            this.GroupBoxLoc = new System.Windows.Forms.GroupBox();
            this.CheckBoxFan = new System.Windows.Forms.CheckBox();
            this.CheckBoxLamp = new System.Windows.Forms.CheckBox();
            this.CheckBoxBoth = new System.Windows.Forms.CheckBox();
            this.GroupBoxLoc.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBoxLoc
            // 
            this.GroupBoxLoc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GroupBoxLoc.Controls.Add(this.CheckBoxFan);
            this.GroupBoxLoc.Controls.Add(this.CheckBoxLamp);
            this.GroupBoxLoc.Controls.Add(this.CheckBoxBoth);
            this.GroupBoxLoc.Location = new System.Drawing.Point(3, 3);
            this.GroupBoxLoc.Name = "GroupBoxLoc";
            this.GroupBoxLoc.Size = new System.Drawing.Size(94, 92);
            this.GroupBoxLoc.TabIndex = 0;
            this.GroupBoxLoc.TabStop = false;
            this.GroupBoxLoc.Text = "groupBox1";
            // 
            // CheckBoxFan
            // 
            this.CheckBoxFan.AutoSize = true;
            this.CheckBoxFan.Location = new System.Drawing.Point(6, 65);
            this.CheckBoxFan.Name = "CheckBoxFan";
            this.CheckBoxFan.Size = new System.Drawing.Size(80, 17);
            this.CheckBoxFan.TabIndex = 2;
            this.CheckBoxFan.Text = "checkBox3";
            this.CheckBoxFan.UseVisualStyleBackColor = true;
            // 
            // CheckBoxLamp
            // 
            this.CheckBoxLamp.AutoSize = true;
            this.CheckBoxLamp.Location = new System.Drawing.Point(6, 42);
            this.CheckBoxLamp.Name = "CheckBoxLamp";
            this.CheckBoxLamp.Size = new System.Drawing.Size(80, 17);
            this.CheckBoxLamp.TabIndex = 1;
            this.CheckBoxLamp.Text = "checkBox2";
            this.CheckBoxLamp.UseVisualStyleBackColor = true;
            // 
            // CheckBoxBoth
            // 
            this.CheckBoxBoth.AutoSize = true;
            this.CheckBoxBoth.Location = new System.Drawing.Point(6, 19);
            this.CheckBoxBoth.Name = "CheckBoxBoth";
            this.CheckBoxBoth.Size = new System.Drawing.Size(80, 17);
            this.CheckBoxBoth.TabIndex = 0;
            this.CheckBoxBoth.Text = "checkBox1";
            this.CheckBoxBoth.UseVisualStyleBackColor = true;
            // 
            // HardwareControllerGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.GroupBoxLoc);
            this.Name = "HardwareControllerGroup";
            this.Size = new System.Drawing.Size(100, 98);
            this.GroupBoxLoc.ResumeLayout(false);
            this.GroupBoxLoc.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBoxLoc;
        public System.Windows.Forms.CheckBox CheckBoxFan;
        public System.Windows.Forms.CheckBox CheckBoxLamp;
        public System.Windows.Forms.CheckBox CheckBoxBoth;
    }
}
