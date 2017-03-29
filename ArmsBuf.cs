using System;
using System.Collections.Generic;
using System.Text;

namespace SAAR
{
	class ArmsBuf
	{
		public ArmsBuf()
		{
			MainArms = "";
			SubArms = "";
			Knife = "";
			Other = "";
		}

		public string MainArms { get; set; }
		public string SubArms { get; set; }
		public string Knife { get; set; }
		public string Other { get; set; }
	}
}
