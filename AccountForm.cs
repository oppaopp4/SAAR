using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAAR
{
	public partial class FormAccount : Form
	{
		public string UserName { get; set; }
		public string TargetKD { get; set; }
		public string TargetHS { get; set; }
		public string ID { get; set; }
		public string Pass { get; set; }
		public string ClanName { get; set; }
		public string LoginSite { get; set; }

		public bool OK { get; set; }

		public FormAccount()
		{
			InitializeComponent();

			OK = false;

			textBoxAccountID.ShortcutsEnabled = false;

			comboBoxLoginSite.SelectedIndex = 0;
		}

		public FormAccount(string un, string tkd, string ths, string id, string ps, string cn, int ls)
		{
			InitializeComponent();

			OK = false;

			textBoxAccountID.ShortcutsEnabled = false;

			textBoxAccountUserName.Text = un;
			textBoxAccountTargetKD.Text = tkd;
			textBoxAccountTargetHS.Text = ths;
			textBoxAccountID.Text = id;
			textBoxAccountPW.Text = ps;
			textBoxAccountClanName.Text = cn;
			comboBoxLoginSite.SelectedIndex = ls;
		}

		/// <summary>
		/// OKボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAccountOK_Click(object sender, EventArgs e)
		{
			ReturnOK();
		}

		/// <summary>
		/// EnterでもOK
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Enter_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				ReturnOK();
			}
		}

		/// <summary>
		/// OK
		/// </summary>
		private void ReturnOK()
		{
			if (textBoxAccountTargetKD.Text != "")
			{
				if (double.Parse(textBoxAccountTargetKD.Text) - 100.0 >= 0 || double.Parse(textBoxAccountTargetKD.Text) <= 0)
				{
					textBoxAccountTargetKD.Text = "99";
				}
			}

			if (textBoxAccountTargetHS.Text != "")
			{
				if (double.Parse(textBoxAccountTargetHS.Text) - 100.0 >= 0 || double.Parse(textBoxAccountTargetHS.Text) <= 0)
				{
					textBoxAccountTargetHS.Text = "99";
				}
			}

			UserName = textBoxAccountUserName.Text;
			TargetKD = textBoxAccountTargetKD.Text;
			TargetHS = textBoxAccountTargetHS.Text;
			ID = textBoxAccountID.Text;
			Pass = textBoxAccountPW.Text;
			ClanName = textBoxAccountClanName.Text;
			switch (comboBoxLoginSite.SelectedIndex)
			{
				case 0:
					LoginSite = "NEXON";
					break;
				case 1:
					LoginSite = "gameyarou";
					break;
				default:
					break;
			}

			OK = true;
			this.Close();
		}

		private void buttonAccountCancel_Click(object sender, EventArgs e)
		{
			OK = false;
			this.Close();
		}

		/// <summary>
		/// 数字か . のみ入力できるようにする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textboxNum_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
			{
				e.Handled = true;
			}
		}

		/// <summary>
		/// \/:*?"<>|以外入力できるようにする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textboxID_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char)Keys.ControlKey || e.KeyChar == '\\' || e.KeyChar == '/' || e.KeyChar == ':' || e.KeyChar == '*' || e.KeyChar == '?'
				|| e.KeyChar == '"' || e.KeyChar == '<' || e.KeyChar == '>' || e.KeyChar == '|')
			{
				e.Handled = true;
			}
		}
	}
}
