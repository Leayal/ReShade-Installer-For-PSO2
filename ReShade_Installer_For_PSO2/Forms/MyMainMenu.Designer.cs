namespace ReShade_Installer_For_PSO2.Forms
{
    partial class MyMainMenu
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
            this.q_usingArksLayerPlugin_no = new System.Windows.Forms.RadioButton();
            this.q_usingArksLayerPlugin_yes = new System.Windows.Forms.RadioButton();
            this.q_usingArksLayerPlugin = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.reshade = new System.Windows.Forms.RadioButton();
            this.swfxv2 = new System.Windows.Forms.RadioButton();
            this.button_Install = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button_browse = new System.Windows.Forms.Button();
            this.swfx2_source = new System.Windows.Forms.LinkLabel();
            this.reshade_source = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.q_usingArksLayerPlugin_no);
            this.panel1.Controls.Add(this.q_usingArksLayerPlugin_yes);
            this.panel1.Location = new System.Drawing.Point(12, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 24);
            this.panel1.TabIndex = 1;
            // 
            // q_usingArksLayerPlugin_no
            // 
            this.q_usingArksLayerPlugin_no.AutoSize = true;
            this.q_usingArksLayerPlugin_no.Location = new System.Drawing.Point(185, 3);
            this.q_usingArksLayerPlugin_no.Name = "q_usingArksLayerPlugin_no";
            this.q_usingArksLayerPlugin_no.Size = new System.Drawing.Size(39, 17);
            this.q_usingArksLayerPlugin_no.TabIndex = 1;
            this.q_usingArksLayerPlugin_no.TabStop = true;
            this.q_usingArksLayerPlugin_no.Text = "No";
            this.q_usingArksLayerPlugin_no.UseVisualStyleBackColor = true;
            // 
            // q_usingArksLayerPlugin_yes
            // 
            this.q_usingArksLayerPlugin_yes.AutoSize = true;
            this.q_usingArksLayerPlugin_yes.Location = new System.Drawing.Point(24, 3);
            this.q_usingArksLayerPlugin_yes.Name = "q_usingArksLayerPlugin_yes";
            this.q_usingArksLayerPlugin_yes.Size = new System.Drawing.Size(43, 17);
            this.q_usingArksLayerPlugin_yes.TabIndex = 0;
            this.q_usingArksLayerPlugin_yes.TabStop = true;
            this.q_usingArksLayerPlugin_yes.Text = "Yes";
            this.q_usingArksLayerPlugin_yes.UseVisualStyleBackColor = true;
            // 
            // q_usingArksLayerPlugin
            // 
            this.q_usingArksLayerPlugin.AutoSize = true;
            this.q_usingArksLayerPlugin.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.q_usingArksLayerPlugin.LinkArea = new System.Windows.Forms.LinkArea(14, 31);
            this.q_usingArksLayerPlugin.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.q_usingArksLayerPlugin.Location = new System.Drawing.Point(16, 9);
            this.q_usingArksLayerPlugin.Name = "q_usingArksLayerPlugin";
            this.q_usingArksLayerPlugin.Size = new System.Drawing.Size(254, 17);
            this.q_usingArksLayerPlugin.TabIndex = 2;
            this.q_usingArksLayerPlugin.TabStop = true;
            this.q_usingArksLayerPlugin.Text = "Are you using Arks-Layer\'s PSO2 Plugin System?";
            this.q_usingArksLayerPlugin.UseCompatibleTextRendering = true;
            this.q_usingArksLayerPlugin.VisitedLinkColor = System.Drawing.Color.Blue;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.reshade_source);
            this.groupBox1.Controls.Add(this.swfx2_source);
            this.groupBox1.Controls.Add(this.reshade);
            this.groupBox1.Controls.Add(this.swfxv2);
            this.groupBox1.Location = new System.Drawing.Point(11, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 64);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shaders";
            // 
            // reshade
            // 
            this.reshade.AutoSize = true;
            this.reshade.Location = new System.Drawing.Point(6, 39);
            this.reshade.Name = "reshade";
            this.reshade.Size = new System.Drawing.Size(88, 17);
            this.reshade.TabIndex = 3;
            this.reshade.TabStop = true;
            this.reshade.Text = "ReShade 3.0";
            this.reshade.UseVisualStyleBackColor = true;
            // 
            // swfxv2
            // 
            this.swfxv2.AutoSize = true;
            this.swfxv2.Location = new System.Drawing.Point(6, 19);
            this.swfxv2.Name = "swfxv2";
            this.swfxv2.Size = new System.Drawing.Size(183, 17);
            this.swfxv2.TabIndex = 2;
            this.swfxv2.TabStop = true;
            this.swfxv2.Text = "ReShade 3.0 with SweetFX 2.0.8";
            this.swfxv2.UseVisualStyleBackColor = true;
            // 
            // button_Install
            // 
            this.button_Install.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.button_Install.Location = new System.Drawing.Point(11, 162);
            this.button_Install.Name = "button_Install";
            this.button_Install.Size = new System.Drawing.Size(260, 36);
            this.button_Install.TabIndex = 4;
            this.button_Install.Text = "Install";
            this.button_Install.UseVisualStyleBackColor = true;
            this.button_Install.Click += new System.EventHandler(this.button_Install_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Game Directory:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(11, 136);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(226, 20);
            this.textBox1.TabIndex = 6;
            // 
            // button_browse
            // 
            this.button_browse.Location = new System.Drawing.Point(241, 136);
            this.button_browse.Name = "button_browse";
            this.button_browse.Size = new System.Drawing.Size(30, 20);
            this.button_browse.TabIndex = 7;
            this.button_browse.Text = "...";
            this.button_browse.UseVisualStyleBackColor = true;
            this.button_browse.Click += new System.EventHandler(this.button_browse_Click);
            // 
            // swfx2_source
            // 
            this.swfx2_source.AutoSize = true;
            this.swfx2_source.Location = new System.Drawing.Point(183, 20);
            this.swfx2_source.Name = "swfx2_source";
            this.swfx2_source.Size = new System.Drawing.Size(75, 13);
            this.swfx2_source.TabIndex = 4;
            this.swfx2_source.TabStop = true;
            this.swfx2_source.Text = "(Discontinued)";
            this.swfx2_source.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.swfx2_source_LinkClicked);
            // 
            // reshade_source
            // 
            this.reshade_source.AutoSize = true;
            this.reshade_source.Location = new System.Drawing.Point(90, 40);
            this.reshade_source.Name = "reshade_source";
            this.reshade_source.Size = new System.Drawing.Size(47, 13);
            this.reshade_source.TabIndex = 5;
            this.reshade_source.TabStop = true;
            this.reshade_source.Text = "(Source)";
            this.reshade_source.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.reshade_source_LinkClicked);
            // 
            // MyMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 210);
            this.Controls.Add(this.button_browse);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Install);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.q_usingArksLayerPlugin);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MyMainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReShade Installer for PSO2";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton q_usingArksLayerPlugin_no;
        private System.Windows.Forms.RadioButton q_usingArksLayerPlugin_yes;
        private System.Windows.Forms.LinkLabel q_usingArksLayerPlugin;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton swfxv2;
        private System.Windows.Forms.RadioButton reshade;
        private System.Windows.Forms.Button button_Install;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button_browse;
        private System.Windows.Forms.LinkLabel reshade_source;
        private System.Windows.Forms.LinkLabel swfx2_source;
    }
}