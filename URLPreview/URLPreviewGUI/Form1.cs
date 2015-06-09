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
			Stopwatch swWatch = Stopwatch.StartNew();
			Preview result = URLPreviewGenerator.CreatePreview(this.textBox1.Text);
			swWatch.Stop();
			this.labelTitle.Text = swWatch.ElapsedMilliseconds.ToString();
		}
	}
}
