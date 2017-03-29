using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAAR
{
	public partial class VersionInfo : Form
	{
		private string Ver;

		public VersionInfo(string _ver)
		{
			InitializeComponent();

			Ver = _ver.Split(new char[] { ' ' })[1];
		}

		private void VersionInfo_Load(object sender, EventArgs e)
		{
			label1.Text = Ver;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			//ブラウザで開く
			System.Diagnostics.Process.Start("http://homepage3.nifty.com/rerebo/othersoft/SAAR/SAAR.html");
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}
	}
}
