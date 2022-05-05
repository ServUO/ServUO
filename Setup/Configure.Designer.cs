
namespace Setup
{
	partial class Configure
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Global = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// Global
			// 
			this.Global.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Global.Location = new System.Drawing.Point(0, 0);
			this.Global.Name = "Global";
			this.Global.Size = new System.Drawing.Size(800, 450);
			this.Global.TabIndex = 0;
			// 
			// Configure
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.Global);
			this.Name = "Configure";
			this.Text = "ServUO Configuration";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid Global;
	}
}

