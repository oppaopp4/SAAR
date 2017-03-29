using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAAR
{
	public partial class FormListViewMemoEdit : Form
	{
		public bool OK { get; set; }

		public FormListViewMemoEdit()
		{
			InitializeComponent();

			OK = false;
		}

		private void buttonMemoEditOK_Click(object sender, EventArgs e)
		{
			OK = true;
			this.Close();
		}

		private void buttonMemoEditCancel_Click(object sender, EventArgs e)
		{
			OK = false;
			this.Close();
		}

		private void textBoxMemoEdit_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				OK = true;
				this.Close();
			}
		}

	}
}
