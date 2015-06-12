namespace URLPreviewGUI
{
	partial class Form1
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
			if(disposing && (components != null))
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.buttonPreview = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.linkLabelUrl = new System.Windows.Forms.LinkLabel();
			this.labelTitle = new System.Windows.Forms.Label();
			this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(13, 13);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(275, 20);
			this.textBox1.TabIndex = 0;
			// 
			// buttonPreview
			// 
			this.buttonPreview.Location = new System.Drawing.Point(294, 13);
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.Size = new System.Drawing.Size(57, 23);
			this.buttonPreview.TabIndex = 1;
			this.buttonPreview.Text = "Preview";
			this.buttonPreview.UseVisualStyleBackColor = true;
			this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelDescription);
			this.groupBox1.Controls.Add(this.linkLabelUrl);
			this.groupBox1.Controls.Add(this.labelTitle);
			this.groupBox1.Controls.Add(this.pictureBoxPreview);
			this.groupBox1.Location = new System.Drawing.Point(13, 42);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(338, 206);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Preview";
			// 
			// linkLabelUrl
			// 
			this.linkLabelUrl.AutoSize = true;
			this.linkLabelUrl.Location = new System.Drawing.Point(16, 181);
			this.linkLabelUrl.Name = "linkLabelUrl";
			this.linkLabelUrl.Size = new System.Drawing.Size(55, 13);
			this.linkLabelUrl.TabIndex = 3;
			this.linkLabelUrl.TabStop = true;
			this.linkLabelUrl.Text = "linkLabel1";
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.Location = new System.Drawing.Point(16, 125);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(35, 13);
			this.labelTitle.TabIndex = 1;
			this.labelTitle.Text = "label1";
			// 
			// pictureBoxPreview
			// 
			this.pictureBoxPreview.Location = new System.Drawing.Point(16, 20);
			this.pictureBoxPreview.Name = "pictureBoxPreview";
			this.pictureBoxPreview.Size = new System.Drawing.Size(301, 98);
			this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxPreview.TabIndex = 0;
			this.pictureBoxPreview.TabStop = false;
			// 
			// labelDescription
			// 
			this.labelDescription.AutoSize = true;
			this.labelDescription.Location = new System.Drawing.Point(19, 142);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(35, 13);
			this.labelDescription.TabIndex = 4;
			this.labelDescription.Text = "label1";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(360, 257);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonPreview);
			this.Controls.Add(this.textBox1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button buttonPreview;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.LinkLabel linkLabelUrl;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.PictureBox pictureBoxPreview;
		private System.Windows.Forms.Label labelDescription;
	}
}

