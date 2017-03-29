namespace SAAR
{
	partial class FormListViewMemoEdit
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
			this.buttonMemoEditOK = new System.Windows.Forms.Button();
			this.buttonMemoEditCancel = new System.Windows.Forms.Button();
			this.textBoxMemoEdit = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonMemoEditOK
			// 
			this.buttonMemoEditOK.Location = new System.Drawing.Point(46, 38);
			this.buttonMemoEditOK.Name = "buttonMemoEditOK";
			this.buttonMemoEditOK.Size = new System.Drawing.Size(75, 23);
			this.buttonMemoEditOK.TabIndex = 0;
			this.buttonMemoEditOK.Text = "OK";
			this.buttonMemoEditOK.UseVisualStyleBackColor = true;
			this.buttonMemoEditOK.Click += new System.EventHandler(this.buttonMemoEditOK_Click);
			// 
			// buttonMemoEditCancel
			// 
			this.buttonMemoEditCancel.Location = new System.Drawing.Point(127, 38);
			this.buttonMemoEditCancel.Name = "buttonMemoEditCancel";
			this.buttonMemoEditCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonMemoEditCancel.TabIndex = 1;
			this.buttonMemoEditCancel.Text = "キャンセル";
			this.buttonMemoEditCancel.UseVisualStyleBackColor = true;
			this.buttonMemoEditCancel.Click += new System.EventHandler(this.buttonMemoEditCancel_Click);
			// 
			// textBoxMemoEdit
			// 
			this.textBoxMemoEdit.Location = new System.Drawing.Point(14, 13);
			this.textBoxMemoEdit.Name = "textBoxMemoEdit";
			this.textBoxMemoEdit.Size = new System.Drawing.Size(188, 19);
			this.textBoxMemoEdit.TabIndex = 2;
			this.textBoxMemoEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxMemoEdit_KeyDown);
			// 
			// FormListViewMemoEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(216, 74);
			this.Controls.Add(this.textBoxMemoEdit);
			this.Controls.Add(this.buttonMemoEditCancel);
			this.Controls.Add(this.buttonMemoEditOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormListViewMemoEdit";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Memoの編集";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonMemoEditOK;
		private System.Windows.Forms.Button buttonMemoEditCancel;
		public System.Windows.Forms.TextBox textBoxMemoEdit;
	}
}