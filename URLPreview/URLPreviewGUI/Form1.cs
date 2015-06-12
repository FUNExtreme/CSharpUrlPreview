using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using URLPreviewLib;

namespace URLPreviewGUI
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void buttonPreview_Click(object sender, EventArgs e)
		{
			URLPreview result = URLPreviewGenerator.CreatePreview(this.textBox1.Text);
			this.labelTitle.Text = result.Title;
			this.labelDescription.Text = (result.Description != null) ? result.Description : "Skipped";
			this.pictureBoxPreview.Load(result.ImageUrl);
		}
	}
}
