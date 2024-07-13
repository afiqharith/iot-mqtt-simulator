﻿namespace HardwareSimMqtt.UIComponent
{
    partial class HardwareViewerGroup
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
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.LabelLampId = new System.Windows.Forms.Label();
            this.LabelFanId = new System.Windows.Forms.Label();
            this.GroupBoxLoc.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBoxLoc
            // 
            this.GroupBoxLoc.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GroupBoxLoc.Controls.Add(this.splitContainerMain);
            this.GroupBoxLoc.Location = new System.Drawing.Point(3, 3);
            this.GroupBoxLoc.Name = "GroupBoxLoc";
            this.GroupBoxLoc.Size = new System.Drawing.Size(105, 89);
            this.GroupBoxLoc.TabIndex = 0;
            this.GroupBoxLoc.TabStop = false;
            this.GroupBoxLoc.Text = "groupBox1";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainerMain.IsSplitterFixed = true;
            this.splitContainerMain.Location = new System.Drawing.Point(9, 20);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.LabelLampId);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.AccessibleName = "";
            this.splitContainerMain.Panel2.Controls.Add(this.LabelFanId);
            this.splitContainerMain.Size = new System.Drawing.Size(88, 61);
            this.splitContainerMain.SplitterDistance = 30;
            this.splitContainerMain.TabIndex = 2;
            // 
            // LabelLampId
            // 
            this.LabelLampId.AutoSize = true;
            this.LabelLampId.Location = new System.Drawing.Point(16, 7);
            this.LabelLampId.Name = "LabelLampId";
            this.LabelLampId.Size = new System.Drawing.Size(53, 13);
            this.LabelLampId.TabIndex = 7;
            this.LabelLampId.Text = "Lamp ID1";
            // 
            // LabelFanId
            // 
            this.LabelFanId.AutoSize = true;
            this.LabelFanId.Location = new System.Drawing.Point(20, 5);
            this.LabelFanId.Name = "LabelFanId";
            this.LabelFanId.Size = new System.Drawing.Size(45, 13);
            this.LabelFanId.TabIndex = 8;
            this.LabelFanId.Text = "Fan ID1";
            // 
            // HardwareViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.GroupBoxLoc);
            this.Name = "HardwareViewer";
            this.Size = new System.Drawing.Size(111, 95);
            this.GroupBoxLoc.ResumeLayout(false);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel1.PerformLayout();
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox GroupBoxLoc;
        private System.Windows.Forms.Label LabelLampId;
        private System.Windows.Forms.Label LabelFanId;
        public System.Windows.Forms.SplitContainer splitContainerMain;
    }
}
