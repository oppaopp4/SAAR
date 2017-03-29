using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SAAR
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main()
		{
			//多重起動防止
			string stThisProcess = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

			//同名のプロセスが他に存在した場合は既に起動していると判断する
			if (System.Diagnostics.Process.GetProcessesByName(stThisProcess).Length > 1)
			{
				MessageBox.Show("SAARは既に起動しています");
				Application.Exit();
			}
			else
			{
				//通常のアプリケーション開始
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new FormSAAR());
			}
		}
	}
}
