using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections;

namespace SAAR
{
	[Serializable()]
	class Settings
	{
		//設定を保存するフィールド
		public string FileLastWriteTime { get; set; }

		public int ActiveUser { get; set; }
		public bool TitleIDDraw { get; set; }
		public string ErrorLogPath { get; set; }

		public bool GraphDaycountCheck { get; set; }

		public bool GetRankCheck { get; set; }

		public int WindowWidth { get; set; }
		public int WindowHeight { get; set; }
		public bool WindowMax { get; set; }

		public int SplitterDistance1 { get; set; }
		public int SplitterDistance2 { get; set; }
		public int SplitterDistance3 { get; set; }

		public bool LoginMinCheck { get; set; }

		public bool ColorSet { get; set; }
		public byte MGColor_R { get; set; }
		public byte MGColor_G { get; set; }
		public byte MGColor_B { get; set; }
		public byte MGColor2_R { get; set; }
		public byte MGColor2_G { get; set; }
		public byte MGColor2_B { get; set; }
		public byte RGColor_R { get; set; }
		public byte RGColor_G { get; set; }
		public byte RGColor_B { get; set; }
		public byte CGColor_R { get; set; }
		public byte CGColor_G { get; set; }
		public byte CGColor_B { get; set; }
		public byte KTColor_R { get; set; }
		public byte KTColor_G { get; set; }
		public byte KTColor_B { get; set; }
		public byte DTColor_R { get; set; }
		public byte DTColor_G { get; set; }
		public byte DTColor_B { get; set; }

		public int LoginLeftFormCheck { get; set; }
		public int PreExe { get; set; }
		public int getLinkGameExecute { get; set; }
		public int AutoClose { get; set; }

		public bool rbSALogin1x { get; set; }
		public bool rbSALogin2x { get; set; }

		public bool GraphXLineClear { get; set; }

		public bool UserWinMax { get; set; }
		public int UserSizeWinX { get; set; }
		public int UserSizeWinY { get; set; }
		public int UserSizedis1 { get; set; }
		public int UserSizedis2 { get; set; }
		public int UserSizedis3 { get; set; }

		public int GraphDayUnit { get; set; }

		//ハイライト設定(0=none 1=Top 2=Bottom)
		public int HighlightKill { get; set; }
		public int HighlightDeath { get; set; }
		public int HighlightKD { get; set; }

		//Settingsクラスのただ一つのインスタンス
		[NonSerialized()]
		private static Settings _instance;
		[System.Xml.Serialization.XmlIgnore]
		public static Settings Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Settings();
				return _instance;
			}
			set { _instance = value; }
		}

		/// <summary>
		/// 設定をバイナリファイルから読み込み復元する
		/// </summary>
		public static void LoadFromBinaryFile()
		{
			string path = GetSettingPath();

			if (File.Exists(path) == false)
			{
				return;
			}

			FileStream fs = new FileStream(path,
				FileMode.Open,
				FileAccess.Read);
			BinaryFormatter bf = new BinaryFormatter();
			//読み込んで逆シリアル化する
			object obj = bf.Deserialize(fs);
			fs.Close();

			Instance = (Settings)obj;
		}

		/// <summary>
		/// 現在の設定をバイナリファイルに保存する
		/// </summary>
		public static void SaveToBinaryFile()
		{
			string path = GetSettingPath();

			if (Directory.Exists(path) == false)
			{
				Directory.CreateDirectory(GetAppPath() + "\\config");
			}

			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				BinaryFormatter bf = new BinaryFormatter();
				//シリアル化して書き込む
				bf.Serialize(fs, Instance);
				fs.Close();
			}
		}

		private static string GetSettingPath()
		{
			string dir = GetAppPath();
			string path = dir + "\\config\\app.config";
			return path;
		}

		/// <summary>
		/// 実行ファイルのあるディレクトリパス取得
		/// </summary>
		/// <returns></returns>
		private static string GetAppPath()
		{
			//return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			return System.Windows.Forms.Application.StartupPath;
		}
	}
}
