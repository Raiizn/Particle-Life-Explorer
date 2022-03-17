namespace Particle_Life_Explorer
{
    partial class MainForm
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.openGL_control = new OpenGL.GlControl();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 52);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 52);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(146, 398);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.AutoSize = true;
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Controls.Add(this.openGL_control);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(146, 52);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(654, 398);
            this.panel3.TabIndex = 2;
            // 
            // openGL_control
            // 
            this.openGL_control.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.openGL_control.Animation = true;
            this.openGL_control.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.openGL_control.ColorBits = ((uint)(24u));
            this.openGL_control.DepthBits = ((uint)(0u));
            this.openGL_control.Location = new System.Drawing.Point(0, 0);
            this.openGL_control.MultisampleBits = ((uint)(0u));
            this.openGL_control.Name = "openGL_control";
            this.openGL_control.Size = new System.Drawing.Size(654, 398);
            this.openGL_control.StencilBits = ((uint)(0u));
            this.openGL_control.TabIndex = 0;
            this.openGL_control.ContextCreated += new System.EventHandler<OpenGL.GlControlEventArgs>(this.openGL_control_ContextCreated);
            this.openGL_control.ContextDestroying += new System.EventHandler<OpenGL.GlControlEventArgs>(this.openGL_control_ContextDestroying);
            this.openGL_control.Render += new System.EventHandler<OpenGL.GlControlEventArgs>(this.openGL_control_Render);
            this.openGL_control.ContextUpdate += new System.EventHandler<OpenGL.GlControlEventArgs>(this.openGL_control_ContextUpdate);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private OpenGL.GlControl openGL_control;
    }
}