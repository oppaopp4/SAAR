using System;
using System.Collections.Generic;
using System.Text;

namespace SAAR
{
	public class Data
	{
		public string IndexNo { get; set; }

		public string Date { get; set; }

		private string _Map;
		public string Map
		{
			get
			{
				return _Map;
			}
			set
			{
				_Map = MapString(value);
			}
		}

		private string _GameType;
		public string GameType {
			get
			{
				return _GameType;
			}
			set
			{
				switch (value)
				{
					case "3":
						_GameType = "デスマッチ";
						break;
					case "5":
						_GameType = "爆破ミッション";
						break;
					case "6":
						_GameType = "奪取ミッション";
						break;
					case "7":
						_GameType = "占領ミッション";
						break;
					case "8":
						_GameType = "サブミッション";
						break;
					default:
						_GameType = value;
						break;
				}
			}
		}

		private string _Team;
		public string Team
		{
			get
			{
				return _Team;
			}
			set
			{
				if (value == "0") _Team = "RED";
				if (value == "1") _Team = "BLUE";
				if ((value != "0") && (value != "1")) _Team = value;
				
			}
		}

		private string _Mainarms;
		public string Mainarms
		{
			get
			{
				return _Mainarms;
			}
			set
			{
				_Mainarms = MainArmsString(value);
			}
		}

		private string _Subarms;
		public string Subarms
		{
			get
			{
				return _Subarms;
			}
			set
			{
				_Subarms = SubArmsString(value);
			}
		}

		private string _Knife;
		public string Knife
		{
			get
			{
				return _Knife;
			}
			set
			{
				_Knife = KnifeString(value);
			}
		}

		private string _Other;
		public string Other
		{
			get
			{
				return _Other;
			}
			set
			{
				_Other = OtherString(value);
			}
		}

		public string Kill { get; set; }

		public string Death { get; set; }

		public string Memo { get; set; }

		public string WinLose { get; set; }

		public string WinTeam
		{
			set
			{
				string winteam = "";
				switch (value)
				{
					case "0":
						winteam = "RED";
						break;
					case "1":
						winteam = "BLUE";
						break;
					default:
						winteam = "引き分け";
						break;
				}

				if (winteam == "引き分け")
				{
					WinLose = "引き分け";
				}
				else
				{
					if (Team == winteam)
					{
						WinLose = "勝ち";
					}
					else
					{
						WinLose = "負け";
					}
				}
			}
		}

		public string Exp { get; set; }
		public string Money { get; set; }
		public string PreExp { get; set; }
		public string PreMoney { get; set; }

		public bool KillTop { get; set; }
		public bool KillBottom { get; set; }
		public bool DeathTop { get; set; }
		public bool DeathBottom { get; set; }
		public bool KDTop { get; set; }
		public bool KDBottom { get; set; }

		#region ID変換
		/// <summary>
		/// mapを文字列にする
		/// </summary>
		/// <param name="strmap"></param>
		/// <returns></returns>
		public string MapString(string mapID)
		{
			if (mapID == null) return "null mapID";
			if (IDCorrespondence.DicMap.ContainsKey(mapID))
			{
				string str = IDCorrespondence.DicMap[mapID];
				if (str != null)
				{
					return str;
				}
				else
				{
					return "";
				}
			}
			else
			{
				if (mapID == null)
				{
					return "";
				}
				else
				{
					return mapID;
				}
			}
		}

		/// <summary>
		/// メイン武器を文字列にする
		/// </summary>
		/// <param name="mainarmsID"></param>
		/// <returns></returns>
		public string MainArmsString(string mainarmsID)
		{
			if (mainarmsID == null) return "null mainarmsID";
			if (IDCorrespondence.DicMainArms.ContainsKey(mainarmsID))
			{
				return IDCorrespondence.DicMainArms[mainarmsID];
			}
			else
			{
				if (mainarmsID == null)
				{
					return "";
				}
				else
				{
					return mainarmsID;
				}
			}
		}

		/// <summary>
		/// サブ武器を文字列にする
		/// </summary>
		/// <param name="subarmsID"></param>
		/// <returns></returns>
		public string SubArmsString(string subarmsID)
		{
			if (subarmsID == null) return "null subarmsID";
			if (IDCorrespondence.DicSubArms.ContainsKey(subarmsID))
			{
				return IDCorrespondence.DicSubArms[subarmsID];
			}
			else
			{
				if (subarmsID == null)
				{
					return "";
				}
				else
				{
					return subarmsID;
				}
			}
		}

		/// <summary>
		/// ナイフを文字列にする
		/// </summary>
		/// <param name="knifeID"></param>
		/// <returns></returns>
		public string KnifeString(string knifeID)
		{
			if (knifeID == null) return "null knifeID";
			if (IDCorrespondence.DicKnife.ContainsKey(knifeID))
			{
				return IDCorrespondence.DicKnife[knifeID];
			}
			else
			{
				if (knifeID == null)
				{
					return "";
				}
				else
				{
					return knifeID;
				}
			}
		}

		/// <summary>
		/// その他を文字列にする
		/// </summary>
		/// <param name="otherID"></param>
		/// <returns></returns>
		public string OtherString(string otherID)
		{
			if (otherID == null) return "null otherID";
			if (IDCorrespondence.DicOther.ContainsKey(otherID))
			{
				return IDCorrespondence.DicOther[otherID];
			}
			else
			{
				if (otherID == null)
				{
					return "";
				}
				else
				{
					return otherID;
				}
			}
		}

		#endregion
	}

	public class RankData
	{
		public string Date { get; set; }
		public string TotalRank { get; set; }
		public string HS { get; set; }
		public string HSRank { get; set; }
		public string Kill { get; set; }
		public string Death { get; set; }
		public string KillRank { get; set; }
		public string ClanName { get; set; }
		public string ClanRank { get; set; }
		public string ClanWin { get; set; }
		public string ClanLose { get; set; }
		public string ClanDraw { get; set; }

		public RankData()
		{
			Clear();
		}

		public void Clear()
		{
			Date = "-1";
			TotalRank = "-1";
			HS = "-1";
			HSRank = "-1";
			Kill = "-1";
			Death = "-1";
			KillRank = "-1";
			ClanName = "-1";
			ClanRank = "-1";
			ClanWin = "-1";
			ClanLose = "-1";
			ClanDraw = "-1";
		}
	}

	public class TotalData
	{
		public string TotalKill { get; set; }
		public string TotalDeath { get; set; }
		public string TotalWin { get; set; }
		public string TotalLose { get; set; }
		public string TotalDraw { get; set; }
		public string OldMoney { get; set; }
		public string NewMoney { get; set; }
		public string OldExp { get; set; }
		public string NewExp { get; set; }
		public string Grade { get; set; }
		public string LastDate { get; set; }
		public string PreviousExp { get; set; }
		public string PreviousMoney { get; set; }

		public TotalData()
		{
			TotalKill = "-1";
			TotalDeath = "-1";
			TotalWin = "-1";
			TotalLose = "-1";
			TotalDraw = "-1";
			OldMoney = "-1";
			NewMoney = "-1";
			OldExp = "-1";
			NewExp = "-1";
			Grade = "-1";
			LastDate = "-1";
			PreviousExp = "-1";
			PreviousMoney = "-1";
		}
	}
}
