using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SAAR
{
	public class AccountData
	{
		public AccountData()
		{
			ID = "";
			Pass = "";
			UserName = "";
			ClanName = "";
			TargetKD = "";
			TargetHS = "";
			LoginSite = "NEXON";
		}

		public string ID { get; set; }
		public string Pass { get; set; }
		public string UserName { get; set; }
		public string ClanName { get; set; }
		public string TargetKD { get; set; }
		public string TargetHS { get; set; }
		public string LoginSite { get; set; }
	}

	public class AccountDataArray
	{
		[System.Xml.Serialization.XmlArrayItem(typeof(AccountData))]
		public ArrayList DataList = new ArrayList();
	}
}
