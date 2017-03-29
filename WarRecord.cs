using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SAAR
{
	class WarRecord
	{
		private string PlayerName;
		private string Path;

		private StreamReader sr;

		private ArmsBuf armsbuf;

		private Record rec;

		private List<Record> ReturnRec;

		private Regex regMap;
		private Regex regWinTeam;
		private Regex regTKD;
		private Regex regMSKO;
		private Regex regAddMainArms;
		private Regex regAddSubArms;
		private Regex regAddKnife;
		private Regex regAddOther;
		private Regex regClanMatch;
		private Regex regNMainArms;
		private Regex regNSubArms;
		private Regex regNKnife;
		private Regex regNOther;
		private Regex regGameType;
		private Regex regExp;
		private Regex regMoney;
		private Regex regHighlight;

		public WarRecord(string Pname, string Pth)
		{
			PlayerName = Pname;

			//ユーザ名に特殊記号があったらエスケープする
			//. ^ $ [ ] * + ? | ( ) \
			PlayerName = PlayerName.Replace("\\", @"\\");
			PlayerName = PlayerName.Replace(".", @"\.");
			PlayerName = PlayerName.Replace("^", @"\^");
			PlayerName = PlayerName.Replace("$", @"\$");
			PlayerName = PlayerName.Replace("[", @"\[");
			PlayerName = PlayerName.Replace("]", @"\]");
			PlayerName = PlayerName.Replace("*", @"\*");
			PlayerName = PlayerName.Replace("+", @"\+");
			PlayerName = PlayerName.Replace("?", @"\?");
			PlayerName = PlayerName.Replace("|", @"\|");
			PlayerName = PlayerName.Replace("(", @"\(");
			PlayerName = PlayerName.Replace(")", @"\)");

			Path = Pth;

			armsbuf = new ArmsBuf();

			rec = new Record();

			ReturnRec = new List<Record>();

			regMap = new Regex(@"Loading world: SA_Worlds\\Project\\(?<Map>.*?)$");
			regWinTeam = new Regex(@"\[NETWORK\] Recv GameResultTeam : T\((?<WinTeam>.*?)\).*");
			regTKD = new Regex(PlayerName + @" T:(?<Team>.*?) .*K:(?<Kill>.*?) D:(?<Death>.*?) .*");
			regMSKO = new Regex(@"\[NETWORK\] Recv Equip \(.*,.*,(?<MainArms>.*?),(?<SubArms>.*?),(?<Knife>.*?),(?<Other>.*?)\)$");
			regAddMainArms = new Regex(@"\[NETWORK\] Send AddEquip W\|PW Idx\((?<AddMainArms>.*?)\).*");
			regAddSubArms = new Regex(@"\[NETWORK\] Send AddEquip W\|SW Idx\((?<AddSubArms>.*?)\).*");
			regAddKnife = new Regex(@"\[NETWORK\] Send AddEquip W\|KN Idx\((?<AddKnife>.*?)\).*");
			regAddOther = new Regex(@"\[NETWORK\] Send AddEquip W\|ET Idx\((?<AddOther>.*?)\).*");
			regClanMatch = new Regex(@".*CLAN MATCH: (?<ClanMatch>.*?)$");
			regNMainArms = new Regex(@"\[NETWORK\] Send ChangeEquipment\(.*\) W\|PW\|(?<NMainArms>.*?)$");
			regNSubArms = new Regex(@"\[NETWORK\] Send ChangeEquipment\(.*\) W\|SW\|(?<NSubArms>.*?)$");
			regNKnife = new Regex(@"\[NETWORK\] Send ChangeEquipment\(.*\) W\|KN\|(?<NKnife>.*?)$");
			regNOther = new Regex(@"\[NETWORK\] Send ChangeEquipment\(.*\) W\|ET\|(?<NOther>.*?)$");
			regGameType = new Regex(@".*GAME TYPE: (?<GameType>.*?)$");
			regExp = new Regex(@".*Exp: (?<Exp>.*?)$");
			regMoney = new Regex(@".*Money: (?<Money>.*?)$");
			regHighlight = new Regex(@" T:(?<Team>.*?) .*K:(?<Kill>.*?) D:(?<Death>.*?) .*");
		}

		public void SearchRecord()
		{
			sr = new StreamReader(Path, Encoding.GetEncoding(932));

			int KillToptmp = 0;
			int DeathToptmp = 0;
			int KillBottomtmp = 9999;
			int DeathBottomtmp = 9999;
			float KDToptmp = 0.0f;
			float KDBottomtmp = 101.0f;

			//内容を一行ずつ読み込む
			while (sr.Peek() > -1)
			{
				string strReadLine = sr.ReadLine();

				//基本となるMSKO
				for (Match m = regMSKO.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.MainArms = m.Groups["MainArms"].Value;
					armsbuf.SubArms = m.Groups["SubArms"].Value;
					armsbuf.Knife = m.Groups["Knife"].Value;
					armsbuf.Other = m.Groups["Other"].Value;
				}

				//Map検出
				for (Match m = regMap.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					rec.Map = m.Groups["Map"].Value;
				}

				//WinTeam検出
				for (Match m = regWinTeam.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					rec.WinTeam = m.Groups["WinTeam"].Value;
				}

				//Add検出
				for (Match m = regAddMainArms.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.MainArms = m.Groups["AddMainArms"].Value;
				}
				for (Match m = regAddSubArms.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.SubArms = m.Groups["AddSubArms"].Value;
				}
				for (Match m = regAddKnife.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.Knife = m.Groups["AddKnife"].Value;
				}
				for (Match m = regAddOther.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.Other = m.Groups["AddOther"].Value;
				}

				//持ち替え検出
				for (Match m = regNMainArms.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.MainArms = m.Groups["NMainArms"].Value;
				}
				for (Match m = regNSubArms.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.SubArms = m.Groups["NSubArms"].Value;
				}
				for (Match m = regNKnife.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.Knife = m.Groups["NKnife"].Value;
				}
				for (Match m = regNOther.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					armsbuf.Other = m.Groups["NOther"].Value;
				}

				//CLAN MATCH検出
				for (Match m = regClanMatch.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					rec.ClanMatch = m.Groups["ClanMatch"].Value;
				}

				//GAME TYPE検出
				for (Match m = regGameType.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					rec.GameType = m.Groups["GameType"].Value;
				}

				//TKD検出
				for (Match m = regTKD.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					rec.Team = m.Groups["Team"].Value;
					rec.Kill = m.Groups["Kill"].Value;
					rec.Death = m.Groups["Death"].Value;
				}

				//ハイライト検出
				for (Match m = regHighlight.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					int tk = int.Parse(m.Groups["Kill"].Value);
					int td = int.Parse(m.Groups["Death"].Value);
					//KillTop
					if (tk >= KillToptmp)
					{
						KillToptmp = tk;
					}
					//DeathTop
					if (td >= DeathToptmp)
					{
						DeathToptmp = td;
					}
					//KillBottom
					if (tk <= KillBottomtmp)
					{
						KillBottomtmp = tk;
					}
					//DeathBottom
					if (td <= DeathBottomtmp)
					{
						DeathBottomtmp = td;
					}
					//K/DTop,K/DBottom
					float tkp = 0.0f;
					if (tk + td != 0)
					{
						tkp = (float)(tk) / ((float)(tk) + (float)(td));
					}
					//K/DTop
					if (tkp >= KDToptmp)
					{
						KDToptmp = tkp;
					}
					//K/DBottom
					if (tkp <= KDBottomtmp)
					{
						KDBottomtmp = tkp;
					}
				}

				//Exp検出
				for (Match m = regExp.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					rec.Exp = m.Groups["Exp"].Value.Trim();

					if (rec.PreExp == "")
					{
						rec.PreExp = m.Groups["Exp"].Value.Trim();
					}
				}

				//Money検出
				for (Match m = regMoney.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					rec.Money = m.Groups["Money"].Value.Trim();

					if (rec.PreMoney == "")
					{
						rec.PreMoney = m.Groups["Money"].Value.Trim();
					}

					//Moneyを検出し、KDやmapも検出出来ていたらたらListに追加
					if (rec.Kill != "" && rec.Map != "")
					{
						if (rec.PreExp == rec.Exp) rec.PreExp = "";
						if (rec.PreMoney == rec.Money) rec.PreMoney = "";

						rec.MainArms = armsbuf.MainArms;
						rec.SubArms = armsbuf.SubArms;
						rec.Knife = armsbuf.Knife;
						rec.Other = armsbuf.Other;

						DateTime dt = File.GetLastWriteTime(Path);
						rec.Date = dt.ToString();

						//KillTop
						if (int.Parse(rec.Kill) >= KillToptmp)
						{
							rec.KillTop = true;
						}
						else
						{
							rec.KillTop = false;
						}
						KillToptmp = 0;
						//DeathTop
						if (int.Parse(rec.Death) >= DeathToptmp)
						{
							rec.DeathTop = true;
						}
						else
						{
							rec.DeathTop = false;
						}
						DeathToptmp = 0;
						//KillBottom
						if (int.Parse(rec.Kill) <= KillBottomtmp)
						{
							rec.KillBottom = true;
						}
						else
						{
							rec.KillBottom = false;
						}
						KillBottomtmp = 9999;
						//DeathBottom
						if (int.Parse(rec.Death) <= DeathBottomtmp)
						{
							rec.DeathBottom = true;
						}
						else
						{
							rec.DeathBottom = false;
						}
						DeathBottomtmp = 9999;
						//K/D
						float kdper = 0.0f;
						if (int.Parse(rec.Kill) + int.Parse(rec.Death) != 0)
						{
							kdper = float.Parse(rec.Kill) / (float.Parse(rec.Kill) + float.Parse(rec.Death));
						}
						//K/DTop
						if (kdper >= KDToptmp)
						{
							rec.KDTop = true;
						}
						else
						{
							rec.KDTop = false;
						}
						KDToptmp = 0.0f;
						//K/DBottom
						if (kdper <= KDBottomtmp)
						{
							rec.KDBottom = true;
						}
						else
						{
							rec.KDBottom = false;
						}
						KDBottomtmp = 101.0f;

						ReturnRec.Add(rec);
						rec = new Record();
					}
				}

			}

			sr.Close();
		}

		/// <summary>
		/// レコードを取得
		/// </summary>
		/// <returns></returns>
		public List<Record> GetRecord()
		{
			return ReturnRec;
		}

		/// <summary>
		/// 総Killを取得
		/// </summary>
		private string strTK;
		public string GetTotalKill()
		{
			StreamReader TotalKillsr = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regTotalKill = new Regex(@".*Kill: (?<TotalKill>.*?)$");
			Regex regPlayerName = new Regex(@".*Nick: " + PlayerName + @"$");
			bool NameExists = false;

			while (TotalKillsr.Peek() > -1)
			{
				string strReadLine = TotalKillsr.ReadLine();

				for (Match m = regTotalKill.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strTK = m.Groups["TotalKill"].Value;
				}

				for (Match m = regPlayerName.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					NameExists = true;
				}
			}

			TotalKillsr.Close();

			if (NameExists)
			{
				return strTK;
			}
			else
			{
				return "NameNotExists";
			}
		}

		/// <summary>
		/// 総Deathを取得
		/// </summary>
		private string strTD;
		public string GetTotalDeath()
		{
			StreamReader TotalDeathsr = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regTotalDeath = new Regex(@".*Death: (?<TotalDeath>.*?)$");
			Regex regPlayerName = new Regex(@".*Nick: " + PlayerName + @"$");
			bool NameExists = false;

			while (TotalDeathsr.Peek() > -1)
			{
				string strReadLine = TotalDeathsr.ReadLine();

				for (Match m = regTotalDeath.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strTD = m.Groups["TotalDeath"].Value;
				}

				for (Match m = regPlayerName.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					NameExists = true;
				}
			}

			TotalDeathsr.Close();

			if (NameExists)
			{
				return strTD;
			}
			else
			{
				return "NameNotExists";
			}
		}

		/// <summary>
		/// 総Winを取得
		/// </summary>
		private string strTW;
		public string GetTotalWin()
		{
			StreamReader TotalWinsr = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regTotalWin = new Regex(@".*Win: (?<TotalWin>.*?)$");
			Regex regPlayerName = new Regex(@".*Nick: " + PlayerName + @"$");
			bool NameExists = false;

			while (TotalWinsr.Peek() > -1)
			{
				string strReadLine = TotalWinsr.ReadLine();

				for (Match m = regTotalWin.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strTW = m.Groups["TotalWin"].Value;
				}

				for (Match m = regPlayerName.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					NameExists = true;
				}
			}

			TotalWinsr.Close();

			if (NameExists)
			{
				return strTW;
			}
			else
			{
				return "NameNotExists";
			}
		}

		/// <summary>
		/// 総Loseを取得
		/// </summary>
		private string strTL;
		public string GetTotalLose()
		{
			StreamReader TotalLosesr = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regTotalLose = new Regex(@".*Lose: (?<TotalLose>.*?)$");
			Regex regPlayerName = new Regex(@".*Nick: " + PlayerName + @"$");
			bool NameExists = false;

			while (TotalLosesr.Peek() > -1)
			{
				string strReadLine = TotalLosesr.ReadLine();

				for (Match m = regTotalLose.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strTL = m.Groups["TotalLose"].Value;
				}

				for (Match m = regPlayerName.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					NameExists = true;
				}
			}

			TotalLosesr.Close();

			if (NameExists)
			{
				return strTL;
			}
			else
			{
				return "NameNotExists";
			}
		}

		/// <summary>
		/// 総Drawを取得
		/// </summary>
		private string strTDraw;
		public string GetTotalDraw()
		{
			StreamReader TotalDrawsr = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regTotalDraw = new Regex(@".*Draw: (?<TotalDraw>.*?)$");
			Regex regPlayerName = new Regex(@".*Nick: " + PlayerName + @"$");
			bool NameExists = false;

			while (TotalDrawsr.Peek() > -1)
			{
				string strReadLine = TotalDrawsr.ReadLine();

				for (Match m = regTotalDraw.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strTDraw = m.Groups["TotalDraw"].Value;
				}

				for (Match m = regPlayerName.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					NameExists = true;
				}
			}

			TotalDrawsr.Close();

			if (NameExists)
			{
				return strTDraw;
			}
			else
			{
				return "NameNotExists";
			}
		}

		/// <summary>
		/// Moneyを取得
		/// </summary>
		private string strMN;
		public string GetMoney()
		{
			StreamReader Money = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regMoney = new Regex(@".*Money: (?<Money>.*?)$");
			Regex regPlayerName = new Regex(@".*Nick: " + PlayerName + @"$");
			bool NameExists = false;

			while (Money.Peek() > -1)
			{
				string strReadLine = Money.ReadLine();

				for (Match m = regMoney.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strMN = m.Groups["Money"].Value;
				}

				for (Match m = regPlayerName.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					NameExists = true;
				}
			}

			Money.Close();

			if (NameExists)
			{
				strMN = strMN.Replace(" ", "");
				return strMN;
			}
			else
			{
				return "NameNotExists";
			}
		}

		/// <summary>
		/// 経験値を取得
		/// </summary>
		private string strExp;
		public string GetExp()
		{
			StreamReader Exp = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regExp = new Regex(@".*Exp: (?<Exp>.*?)$");
			Regex regPlayerName = new Regex(@".*Nick: " + PlayerName + @"$");
			bool NameExists = false;

			while (Exp.Peek() > -1)
			{
				string strReadLine = Exp.ReadLine();

				for (Match m = regExp.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strExp = m.Groups["Exp"].Value;
				}

				for (Match m = regPlayerName.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					NameExists = true;
				}
			}

			Exp.Close();

			if (NameExists)
			{
				strExp = strExp.Replace(" ", "");
				return strExp;
			}
			else
			{
				return "NameNotExists";
			}
		}

		/// <summary>
		/// 階級を取得
		/// </summary>
		private string strGrade;
		public string GetGrade()
		{
			StreamReader Grade = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regGrade = new Regex(@".*Grade: (?<Grade>.*?)$");
			Regex regPlayerName = new Regex(@".*Nick: " + PlayerName + @"$");
			bool NameExists = false;

			while (Grade.Peek() > -1)
			{
				string strReadLine = Grade.ReadLine();

				for (Match m = regGrade.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strGrade = m.Groups["Grade"].Value;
				}

				for (Match m = regPlayerName.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					NameExists = true;
				}
			}

			Grade.Close();

			if (NameExists)
			{
				strGrade = strGrade.Replace(" ", "");
				return strGrade;
			}
			else
			{
				return "NameNotExists";
			}
		}

		/// <summary>
		/// 今日以前のExpを取得
		/// </summary>
		/// <returns></returns>
		private string strPE;
		public string GetPreviousExp()
		{
			StreamReader srPE = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regPE = new Regex(@".*Exp: (?<PreExp>.*?)$");

			bool Got = false;
			while (srPE.Peek() > -1)
			{
				string strReadLine = srPE.ReadLine();

				for (Match m = regPE.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strPE = m.Groups["PreExp"].Value;
					Got = true;
				}
				if (Got) break;
			}

			srPE.Close();

			if (Got)
			{
				strPE = strPE.Replace(" ", "");
				return strPE;
			}
			else
			{
				return "-1";
			}
		}

		/// <summary>
		/// 今日以前のMoneyを取得
		/// </summary>
		/// <returns></returns>
		private string strPM;
		public string GetPreviousMoney()
		{
			StreamReader srPM = new StreamReader(Path, Encoding.GetEncoding(932));

			Regex regPM = new Regex(@".*Money: (?<PreMoney>.*?)$");

			bool Got = false;
			while (srPM.Peek() > -1)
			{
				string strReadLine = srPM.ReadLine();

				for (Match m = regPM.Match(strReadLine); m.Success; m = m.NextMatch())
				{
					strPM = m.Groups["PreMoney"].Value;
					Got = true;
				}
				if (Got) break;
			}

			srPM.Close();

			if (Got)
			{
				strPM = strPM.Replace(" ", "");
				return strPM;
			}
			else
			{
				return "-1";
			}
		}
	}
}
