using System;
using System.Collections.Generic;
using System.Text;

namespace SAAR
{
	public class Record
	{
		public Record()
		{
			MainArms = "";
			SubArms = "";
			Knife = "";
			Other = "";

			Map = "";
			Team = "";
			Kill = "";
			Death = "";
			WinTeam = "";
			ClanMatch = "";
			GameType = "";

			Date = "";

			Exp = "";
			Money = "";
			PreExp = "";
			PreMoney = "";

			KillTop = false;
			DeathTop = false;
		}

		public string MainArms { get; set; }
		public string SubArms { get; set; }
		public string Knife { get; set; }
		public string Other { get; set; }

		public string Map { get; set; }
		public string Team { get; set; }
		public string Kill { get; set; }
		public string Death { get; set; }
		public string WinTeam { get; set; }
		public string ClanMatch { get; set; }
		public string GameType { get; set; }

		public string Date { get; set; }

		public string Exp { get; set; }
		public string Money { get; set; }
		public string PreExp { get; set; }
		public string PreMoney { get; set; }

		public bool KillTop { get; set; }
		public bool DeathTop { get; set; }
		public bool KillBottom { get; set; }
		public bool DeathBottom { get; set; }
		public bool KDTop { get; set; }
		public bool KDBottom { get; set; }
	}
}
