namespace SAAR
{
	partial class FormAccount
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
			this.components = new System.ComponentModel.Container();
			this.buttonAccountOK = new System.Windows.Forms.Button();
			this.label13 = new System.Windows.Forms.Label();
			this.textBoxAccountID = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.textBoxAccountClanName = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textBoxAccountPW = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxAccountTargetKD = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.textBoxAccountTargetHS = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textBoxAccountUserName = new System.Windows.Forms.TextBox();
			this.buttonAccountCancel = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxLoginSite = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// buttonAccountOK
			// 
			this.buttonAccountOK.Location = new System.Drawing.Point(128, 233);
			this.buttonAccountOK.Name = "buttonAccountOK";
			this.buttonAccountOK.Size = new System.Drawing.Size(75, 23);
			this.buttonAccountOK.TabIndex = 22;
			this.buttonAccountOK.Text = "OK";
			this.buttonAccountOK.UseVisualStyleBackColor = true;
			this.buttonAccountOK.Click += new System.EventHandler(this.buttonAccountOK_Click);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(31, 104);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(18, 12);
			this.label13.TabIndex = 25;
			this.label13.Text = "ID:";
			this.toolTip1.SetToolTip(this.label13, "入力必須\r\nオートログイン機能を使用する場合はSAのユーザIDを入力してください\r\nオートログイン機能を使用しない場合はユニークな名前を入力してください\r\nユーザ" +
					"の識別にも使用します");
			// 
			// textBoxAccountID
			// 
			this.textBoxAccountID.Location = new System.Drawing.Point(128, 101);
			this.textBoxAccountID.Name = "textBoxAccountID";
			this.textBoxAccountID.Size = new System.Drawing.Size(133, 19);
			this.textBoxAccountID.TabIndex = 3;
			this.toolTip1.SetToolTip(this.textBoxAccountID, "空欄またはユニーク入力\r\nSAのユーザIDを入力してください\r\nアカウント自動入力機能を使用しない場合は必要ありません");
			this.textBoxAccountID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
			this.textBoxAccountID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxID_KeyPress);
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(31, 162);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(44, 12);
			this.label18.TabIndex = 33;
			this.label18.Text = "クラン名:";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(31, 133);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(32, 12);
			this.label14.TabIndex = 26;
			this.label14.Text = "Pass:";
			// 
			// textBoxAccountClanName
			// 
			this.textBoxAccountClanName.Location = new System.Drawing.Point(128, 159);
			this.textBoxAccountClanName.Name = "textBoxAccountClanName";
			this.textBoxAccountClanName.Size = new System.Drawing.Size(133, 19);
			this.textBoxAccountClanName.TabIndex = 5;
			this.toolTip1.SetToolTip(this.textBoxAccountClanName, "空欄可\r\nクラン名を入力してください\r\nランキング機能で使用します");
			this.textBoxAccountClanName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(178, 46);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(11, 12);
			this.label9.TabIndex = 29;
			this.label9.Text = "%";
			// 
			// textBoxAccountPW
			// 
			this.textBoxAccountPW.Location = new System.Drawing.Point(128, 130);
			this.textBoxAccountPW.Name = "textBoxAccountPW";
			this.textBoxAccountPW.PasswordChar = '*';
			this.textBoxAccountPW.Size = new System.Drawing.Size(133, 19);
			this.textBoxAccountPW.TabIndex = 4;
			this.toolTip1.SetToolTip(this.textBoxAccountPW, "空欄可\r\nSAのログインパスワードを入力してください\r\nアカウント自動入力機能を使用しない場合は必要ありません");
			this.textBoxAccountPW.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(31, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 12);
			this.label1.TabIndex = 27;
			this.label1.Text = "目標KD率:";
			// 
			// textBoxAccountTargetKD
			// 
			this.textBoxAccountTargetKD.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.textBoxAccountTargetKD.Location = new System.Drawing.Point(128, 43);
			this.textBoxAccountTargetKD.Name = "textBoxAccountTargetKD";
			this.textBoxAccountTargetKD.Size = new System.Drawing.Size(44, 19);
			this.textBoxAccountTargetKD.TabIndex = 1;
			this.textBoxAccountTargetKD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.textBoxAccountTargetKD, "空欄可\r\n目標KD率を入力してください\r\n小数点入力もできます");
			this.textBoxAccountTargetKD.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
			this.textBoxAccountTargetKD.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxNum_KeyPress);
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(178, 75);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(11, 12);
			this.label16.TabIndex = 32;
			this.label16.Text = "%";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(31, 75);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(58, 12);
			this.label17.TabIndex = 30;
			this.label17.Text = "目標HS率:";
			// 
			// textBoxAccountTargetHS
			// 
			this.textBoxAccountTargetHS.ImeMode = System.Windows.Forms.ImeMode.Disable;
			this.textBoxAccountTargetHS.Location = new System.Drawing.Point(128, 72);
			this.textBoxAccountTargetHS.Name = "textBoxAccountTargetHS";
			this.textBoxAccountTargetHS.Size = new System.Drawing.Size(44, 19);
			this.textBoxAccountTargetHS.TabIndex = 2;
			this.textBoxAccountTargetHS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip1.SetToolTip(this.textBoxAccountTargetHS, "空欄可\r\n目標HS率を入力してください\r\n小数点入力もできます");
			this.textBoxAccountTargetHS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
			this.textBoxAccountTargetHS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxNum_KeyPress);
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(31, 17);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(91, 12);
			this.label8.TabIndex = 23;
			this.label8.Text = "ゲーム内ユーザ名:";
			// 
			// textBoxAccountUserName
			// 
			this.textBoxAccountUserName.Location = new System.Drawing.Point(128, 14);
			this.textBoxAccountUserName.Name = "textBoxAccountUserName";
			this.textBoxAccountUserName.Size = new System.Drawing.Size(133, 19);
			this.textBoxAccountUserName.TabIndex = 0;
			this.toolTip1.SetToolTip(this.textBoxAccountUserName, "ユニーク入力必須\r\nゲーム内のキャラクター名を入力してください\r\n戦績の記録やユーザの識別で使用します");
			this.textBoxAccountUserName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
			// 
			// buttonAccountCancel
			// 
			this.buttonAccountCancel.Location = new System.Drawing.Point(211, 233);
			this.buttonAccountCancel.Name = "buttonAccountCancel";
			this.buttonAccountCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonAccountCancel.TabIndex = 35;
			this.buttonAccountCancel.Text = "キャンセル";
			this.buttonAccountCancel.UseVisualStyleBackColor = true;
			this.buttonAccountCancel.Click += new System.EventHandler(this.buttonAccountCancel_Click);
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 0;
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 0;
			this.toolTip1.ReshowDelay = 0;
			this.toolTip1.UseAnimation = false;
			this.toolTip1.UseFading = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(31, 191);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 12);
			this.label2.TabIndex = 36;
			this.label2.Text = "ログインサイト:";
			// 
			// comboBoxLoginSite
			// 
			this.comboBoxLoginSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxLoginSite.FormattingEnabled = true;
			this.comboBoxLoginSite.Items.AddRange(new object[] {
            "NEXON",
            "ゲームヤロウ"});
			this.comboBoxLoginSite.Location = new System.Drawing.Point(128, 188);
			this.comboBoxLoginSite.Name = "comboBoxLoginSite";
			this.comboBoxLoginSite.Size = new System.Drawing.Size(121, 20);
			this.comboBoxLoginSite.TabIndex = 37;
			// 
			// FormAccount
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(299, 270);
			this.Controls.Add(this.comboBoxLoginSite);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonAccountCancel);
			this.Controls.Add(this.buttonAccountOK);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textBoxAccountID);
			this.Controls.Add(this.label18);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.textBoxAccountClanName);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textBoxAccountPW);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxAccountTargetKD);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.textBoxAccountTargetHS);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textBoxAccountUserName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAccount";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "アカウント管理";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonAccountOK;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textBoxAccountID;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textBoxAccountClanName;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textBoxAccountPW;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxAccountTargetKD;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.TextBox textBoxAccountTargetHS;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textBoxAccountUserName;
		private System.Windows.Forms.Button buttonAccountCancel;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxLoginSite;
	}
}