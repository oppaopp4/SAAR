using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SAAR
{
	public static class IDCorrespondence
	{
		public static Dictionary<string, string> DicMap;
		public static Dictionary<string, string> DicMainArms;
		public static Dictionary<string, string> DicSubArms;
		public static Dictionary<string, string> DicKnife;
		public static Dictionary<string, string> DicOther;

		/// <summary>
		/// 初期化読み込み
		/// </summary>
		public static void IDCorrespondenceInit()
		{
			DicMap = new Dictionary<string, string>();
			DicMainArms = new Dictionary<string, string>();
			DicSubArms = new Dictionary<string, string>();
			DicKnife = new Dictionary<string, string>();
			DicOther = new Dictionary<string, string>();

			//iniファイルからそれぞれ読み込み
			if (File.Exists(@"ini\Map.ini"))
			{
				StreamReader sr = new StreamReader(@"ini\Map.ini", Encoding.GetEncoding(932));

				//内容を一行ずつ読み込む
				while (sr.Peek() > -1)
				{
					string strReadLine = sr.ReadLine();

					string[] strcmp = strReadLine.Split(new char[] { '=' });

					if (strcmp[0] != "") DicMap[strcmp[0]] = strcmp[1];
				}

				sr.Close();
			}

			if (File.Exists(@"ini\MainArms.ini"))
			{
				StreamReader sr = new StreamReader(@"ini\MainArms.ini", Encoding.GetEncoding(932));

				//内容を一行ずつ読み込む
				while (sr.Peek() > -1)
				{
					string strReadLine = sr.ReadLine();

					string[] strcmp = strReadLine.Split(new char[] { '=' });

					if (strcmp[0] != "") DicMainArms[strcmp[0]] = strcmp[1];
				}

				sr.Close();
			}

			if (File.Exists(@"ini\SubArms.ini"))
			{
				StreamReader sr = new StreamReader(@"ini\SubArms.ini", Encoding.GetEncoding(932));

				//内容を一行ずつ読み込む
				while (sr.Peek() > -1)
				{
					string strReadLine = sr.ReadLine();

					string[] strcmp = strReadLine.Split(new char[] { '=' });

					if (strcmp[0] != "") DicSubArms[strcmp[0]] = strcmp[1];
				}

				sr.Close();
			}

			if (File.Exists(@"ini\Knife.ini"))
			{
				StreamReader sr = new StreamReader(@"ini\Knife.ini", Encoding.GetEncoding(932));

				//内容を一行ずつ読み込む
				while (sr.Peek() > -1)
				{
					string strReadLine = sr.ReadLine();

					string[] strcmp = strReadLine.Split(new char[] { '=' });

					if (strcmp[0] != "") DicKnife[strcmp[0]] = strcmp[1];
				}

				sr.Close();
			}

			if (File.Exists(@"ini\Other.ini"))
			{
				StreamReader sr = new StreamReader(@"ini\Other.ini", Encoding.GetEncoding(932));

				//内容を一行ずつ読み込む
				while (sr.Peek() > -1)
				{
					string strReadLine = sr.ReadLine();

					string[] strcmp = strReadLine.Split(new char[] { '=' });

					if (strcmp[0] != "") DicOther[strcmp[0]] = strcmp[1];
				}

				sr.Close();
			}
		}
	}
}
