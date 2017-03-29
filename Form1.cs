using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Web;
using System.Text.RegularExpressions;

namespace SAAR
{
	public partial class FormSAAR : Form
	{
		public FormSAAR()
		{
			InitializeComponent();
		}

		private string Title = "SAAR ver0.971";

		private void Form1_Load(object sender, EventArgs e)
		{
			Directory.SetCurrentDirectory(Application.StartupPath);

			//設定を読み込む
			Settings.LoadFromBinaryFile();

			//ウィンドウのサイズを読み込む
			if (Settings.Instance.WindowWidth != 0 && Settings.Instance.WindowHeight != 0)
			{
				this.Size = new Size(Settings.Instance.WindowWidth, Settings.Instance.WindowHeight);
			}
			if (Settings.Instance.WindowMax) this.WindowState = FormWindowState.Maximized;

			//SplitterDistanceを復元
			if (Settings.Instance.SplitterDistance1 != 0)
			{
				splitContainer1.SplitterDistance = Settings.Instance.SplitterDistance1;
			}
			else
			{
				splitContainer1.SplitterDistance = 71;
			}
			if (Settings.Instance.SplitterDistance2 != 0)
			{
				splitContainer2.SplitterDistance = Settings.Instance.SplitterDistance2;
			}
			else
			{
				splitContainer2.SplitterDistance = 149;
			}
			if (Settings.Instance.SplitterDistance3 != 0)
			{
				splitContainer3.SplitterDistance = Settings.Instance.SplitterDistance3;
			}
			else
			{
				splitContainer3.SplitterDistance = 180;
			}

			//ID対応クラス初期化
			IDCorrespondence.IDCorrespondenceInit();

			//アカウント情報をadaに読み込む
			LoadAccount();
			//アクティブユーザコンボボックス更新
			RefreshActiveUserBox();

			///comboBoxActiveUser_SelectedIndexChanged()が呼ばれるので以下省略
			//データをdata.xmlからArrayListに読み込む
			//LoadDataXml();
			//TotalData.xmlをtotaldataに読み込み
			//LoadTotalData();
			//ランキングデータをRankData.xmlからArrayListに読み込む
			//LoadRankDataXml();

			if (ada.DataList.Count > 0)
			{
				//アクティブユーザを読み込む
				try
				{
					comboBoxActiveUser.SelectedIndex = Settings.Instance.ActiveUser;
				}
				catch
				{
					comboBoxActiveUser.SelectedIndex = -1;
				}
			}
			//タイトルバーにアクティブユーザ表示
			TitleDraw();

			//色設定が無い場合はデフォルト
			if (Settings.Instance.ColorSet == false)
			{
				ColorReset();
			}

			//ステータスを読み込む
			GetState();

			//TextboxMainInfo表示
			//PrintTextboxMainInfo();

			//pictureBoxGraphを描画しなおす
			//pictureBoxGraph.Invalidate();

			//ListView表示
			//ListViewItemAdd();

			//ListViewRanking表示
			//RankingListViewItemAdd();

			//集計カテゴリページのコンボボックス
			LoadCategoryCombobox();

			//ランキングブラウザ初期化
			WbRankIni();

			//フォーカス出来ないバグ対策？
			buttonREC.Focus();

			//メニューストリップのアクティブユーザメニュー更新
			//MenuRefresh();
		}

		private void TitleDraw()
		{
			if (Settings.Instance.TitleIDDraw)
			{
				this.Text = Title + " (" + comboBoxActiveUser.Text + ")";
			}
			else
			{
				this.Text = Title;
			}
		}

		/// <summary>
		/// フォームが閉じられる時
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (WindowState == FormWindowState.Maximized)
			{
				Settings.Instance.WindowMax = true;
			}
			else
			{
				Settings.Instance.WindowMax = false;
				if (WindowState != FormWindowState.Minimized)
				{
					Settings.Instance.WindowWidth = this.Width;
					Settings.Instance.WindowHeight = this.Height;
				}
			}

			Settings.Instance.ActiveUser = comboBoxActiveUser.SelectedIndex;

			Settings.Instance.SplitterDistance1 = splitContainer1.SplitterDistance;
			Settings.Instance.SplitterDistance2 = splitContainer2.SplitterDistance;
			Settings.Instance.SplitterDistance3 = splitContainer3.SplitterDistance;

			Settings.SaveToBinaryFile();

			//アカウント情報をXMLに保存
			SaveAccount();
		}

		private string Path;

		private ArrayList DataList = new ArrayList();

		private TotalData totaldata = new TotalData();

		private void button1_Click(object sender, EventArgs e)
		{
			//ランキング取得
			if (Settings.Instance.GetRankCheck)
			{
				GetRanking();
			}

			if (GetRec())
			{
				toolStripProgressBarData.PerformStep();

				//data.xmlに保存
				bool bl_savexml;
				bl_savexml = SaveXml_TotalKDWL();

				//カレンダーセレクト
				_SLI = SelectedListItem.CalendarSelect;

				//TextboxMainInfo表示
				PrintTextboxMainInfo();

				//pictureBoxGraphを描画しなおす
				pictureBoxGraph.Invalidate();

				//ListView表示
				ListViewItemAdd();

				//集計カテゴリページのコンボボックス
				LoadCategoryCombobox();

				toolStripProgressBarData.PerformStep();
				toolStripProgressBarData.Value = 0;

				if (bl_savexml)
				{
					System.Media.SystemSounds.Asterisk.Play();
					//MessageBox.Show("戦績を記録しました");
				}
			}
		}

		/// <summary>
		/// ステータスを読み込む
		/// </summary>
		private void GetState()
		{
			//PlayerName = Settings.Instance.UserName;

			//Path = @"../../test6.txt";
			//Path = @"C:\Program Files\RedBanana\SuddenAttack\error.log";
			Path = Settings.Instance.ErrorLogPath;
			if (Path == null || Path == "")
			{
				using (Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Nexon\SuddenAttack", false))
				{
					if (regkey == null)
					{
						Path = "regkey null";
					}
					else
					{
						Path = (string)regkey.GetValue("RootPath", "regkey GetValue null");
						Path += @"\error.log";
					}
				}

				Settings.Instance.ErrorLogPath = Path;
				Settings.SaveToBinaryFile();

				textBoxerrorlogPath.Text = Path;
			}
		}

		/// <summary>
		/// data.xmlに記録
		/// </summary>
		private bool SaveDataXML()
		{
			string createfilename = "";
			foreach (AccountData item in ada.DataList)
			{
				if (comboBoxActiveUser.Text == item.UserName)
				{
					createfilename = item.ID;
				}
			}

			string datapath = GetAppPath() + "\\data\\" + createfilename;
			if (Directory.Exists(datapath) == false)
			{
				Directory.CreateDirectory(datapath);
			}

			Type[] et = new Type[] { typeof(Data) };

			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList), et);
			try
			{
				using (System.IO.FileStream fs = new System.IO.FileStream(datapath + "\\data.xml", System.IO.FileMode.Create))
				{
					serializer.Serialize(fs, DataList);
				}
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.ToString(), "data.xml保存失敗");
				return false;
			}

			return true;
		}

		/// <summary>
		/// TotalData.xmlをtotaldataに読み込み
		/// </summary>
		private void LoadTotalData()
		{
			totaldata.TotalKill = totaldata.TotalDeath = totaldata.TotalWin = totaldata.TotalLose = totaldata.TotalDraw
			= totaldata.OldMoney = totaldata.NewMoney = totaldata.OldExp = totaldata.NewExp = totaldata.Grade = "0";

			string createfilename = "";
			foreach (AccountData item in ada.DataList)
			{
				if (comboBoxActiveUser.Text == item.UserName)
				{
					createfilename = item.ID;
				}
			}

			string tdpath = GetAppPath() + "\\data\\" + createfilename;
			if (Directory.Exists(tdpath) == false)
			{
				Directory.CreateDirectory(tdpath);
			}

			//XmlSerializerオブジェクトの作成
			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TotalData));

			try
			{
				//ファイルを開く
				using (System.IO.FileStream fs = new System.IO.FileStream(tdpath + "\\TotalData.xml", System.IO.FileMode.Open))
				{
					//XMLファイルから読み込み、逆シリアル化する
					totaldata = (TotalData)serializer.Deserialize(fs);
				}

			}
			catch
			{
			}
		}

		/// <summary>
		/// data.xmlと設定ファイルに保存
		/// </summary>
		/// <returns></returns>
		private bool SaveXml_TotalKDWL()
		{
			//data.xmlに記録
			SaveDataXML();

			//設定ファイルにTotalKDWLを記録
			WarRecord wr = new WarRecord(comboBoxActiveUser.Text, Path);
			string tmpget = "";

			tmpget = wr.GetTotalKill();
			if (tmpget != "NameNotExists")
			{
				totaldata.TotalKill = tmpget;
				//Settings.Instance.TotalKill = tmpget;
			}
			toolStripProgressBarData.PerformStep();

			tmpget = wr.GetTotalDeath();
			if (tmpget != "NameNotExists")
			{
				totaldata.TotalDeath = tmpget;
				//Settings.Instance.TotalDeath = tmpget;
			}
			toolStripProgressBarData.PerformStep();

			tmpget = wr.GetTotalWin();
			if (tmpget != "NameNotExists")
			{
				totaldata.TotalWin = tmpget;
			}
			toolStripProgressBarData.PerformStep();

			tmpget = wr.GetTotalLose();
			if (tmpget != "NameNotExists")
			{
				totaldata.TotalLose = tmpget;
			}
			toolStripProgressBarData.PerformStep();

			tmpget = wr.GetTotalDraw();
			if (tmpget != "NameNotExists")
			{
				totaldata.TotalDraw = tmpget;
			}
			toolStripProgressBarData.PerformStep();

			tmpget = wr.GetMoney();
			if (tmpget != "NameNotExists")
			{
				totaldata.OldMoney = totaldata.NewMoney;
				totaldata.NewMoney = tmpget;
			}
			toolStripProgressBarData.PerformStep();

			tmpget = wr.GetExp();
			if (tmpget != "NameNotExists")
			{
				totaldata.OldExp = totaldata.NewExp;
				totaldata.NewExp = tmpget;
			}
			toolStripProgressBarData.PerformStep();

			tmpget = wr.GetGrade();
			if (tmpget != "NameNotExists")
			{
				totaldata.Grade = tmpget;
			}
			toolStripProgressBarData.PerformStep();

			//LastDateと今日の日付を比較
			if (totaldata.LastDate.Split(new char[] { ' ' })[0] != DateTime.Today.ToString().Split(new char[] { ' ' })[0])
			{
				//LastDateを更新
				totaldata.LastDate = DateTime.Today.ToString().Split(new char[] { ' ' })[0];

				//今日以前のExpとMoneyを記録
				totaldata.PreviousExp = wr.GetPreviousExp();
				totaldata.PreviousMoney = wr.GetPreviousMoney();
			}
			toolStripProgressBarData.PerformStep();

			if (tmpget == "NameNotExists")
			{
				MessageBox.Show(comboBoxActiveUser.Text + " の記録はありません");

				return false;
			}
			else
			{
				//設定ファイルにファイル時間を記録
				Settings.Instance.FileLastWriteTime = File.GetLastWriteTime(Path).ToString();

				Settings.SaveToBinaryFile();

				//TotalData.xmlに記録
				string createfilename = "";
				foreach (AccountData item in ada.DataList)
				{
					if (comboBoxActiveUser.Text == item.UserName)
					{
						createfilename = item.ID;
					}
				}

				string datapath = GetAppPath() + "\\data\\" + createfilename;
				if (Directory.Exists(datapath) == false)
				{
					Directory.CreateDirectory(datapath);
				}

				//XmlSerializerオブジェクトを作成
				//書き込むオブジェクトの型を指定する
				System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(TotalData));

				try
				{
					using (System.IO.FileStream fs = new System.IO.FileStream(datapath + "\\TotalData.xml", System.IO.FileMode.Create))
					{
						serializer.Serialize(fs, totaldata);
					}
				}
				catch (System.Exception e)
				{
					MessageBox.Show(e.ToString(), "TotalData.xml保存失敗");
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Data.xmlを読み込む
		/// </summary>
		/// <returns></returns>
		private bool LoadDataXml()
		{
			DataList.Clear();

			string createfilename = "";
			foreach (AccountData item in ada.DataList)
			{
				if (comboBoxActiveUser.Text == item.UserName)
				{
					createfilename = item.ID;
				}
			}

			string loadpath = GetAppPath() + "\\data\\" + createfilename + "\\data.xml";
			if (File.Exists(loadpath) == true)
			{
				Type[] et = new Type[] { typeof(Data) };

				System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList), et);

				try
				{
					using (System.IO.FileStream fs = new System.IO.FileStream(loadpath, System.IO.FileMode.Open))
					{
						DataList = (ArrayList)serializer.Deserialize(fs);
					}
				}
				catch (System.Exception e)
				{
					MessageBox.Show(e.ToString(), "data.xml読み込み失敗");
					return false;
				}
			}

			//IndexNoのないもの(ver0.4以前のデータ)は暫定的にindexを付ける
			int indexcnt = 0;
			foreach (Data item in DataList)
			{
				if ((item.IndexNo == null) || (item.IndexNo == ""))
				{
					item.IndexNo = indexcnt.ToString();
					indexcnt++;
				}
			}

			//GameTypeのないもの(ver0.8以前のデータ)にGameTypeを付ける
			foreach (Data item in DataList)
			{
				if (item.GameType == null)
				{
					if (item.Map == "ゲートウェイ" || item.Map == "シャークテール"
									|| item.Map == "パワープラント" || item.Map == "ウェアハウス"
									|| item.Map == "潜水艦基地" || item.Map == "バーニングリバー"
									|| item.Map == "ジー・キューブ")
					{
						item.GameType = "デスマッチ";
					}

					if (item.Map == "ポセイドン" || item.Map == "プロバンス" || item.Map == "エンブル研究所"
									|| item.Map == "オールドタウン" || item.Map == "海洋中継所" || item.Map == "フローズンシティ"
									|| item.Map == "エアポート" || item.Map == "ドラゴンロード" || item.Map == "第3補給倉庫")
					{
						item.GameType = "爆破ミッション";
					}

					if (item.Map == "ゴールデンアイ" || item.Map == "クラブナイト")
					{
						item.GameType = "奪取ミッション";
					}

					if (item.Map == "ストームビル")
					{
						item.GameType = "占領ミッション";
					}

					if (item.Map == "マッドケージ")
					{
						item.GameType = "サブミッション";
					}
				}
			}

			return true;
		}

		/// <summary>
		/// 戦績をDataListに取得する
		/// </summary>
		private bool GetRec()
		{
			//logファイルが無ければ記録しない
			if (!File.Exists(Path))
			{
				MessageBox.Show("logファイルがありません");
				return false;
			}

			//ファイル時間が同じなら保存しない
			if (Settings.Instance.FileLastWriteTime == File.GetLastWriteTime(Path).ToString())
			{
				MessageBox.Show("新しいデータはありません");
				return false;
			}

			if (comboBoxActiveUser.Items.Count == 0)
			{
				MessageBox.Show("ユーザ登録をしてください");
				return false;
			}
			if (comboBoxActiveUser.SelectedIndex == -1)
			{
				MessageBox.Show("アクティブユーザを選択してください");
				return false;
			}

			List<Record> RecList = new List<Record>();

			WarRecord wr;

			try
			{
				wr = new WarRecord(comboBoxActiveUser.Text, Path);
				wr.SearchRecord();
			}
			catch
			{
				return false;
			}

			RecList = wr.GetRecord();

			int indexcnt = 0;
			foreach (var item in RecList)
			{
				Data data = new Data();

				data.IndexNo = indexcnt.ToString();
				indexcnt++;
				data.Date = item.Date;
				data.Map = item.Map;
				data.Team = item.Team;
				data.WinTeam = item.WinTeam;//勝敗を先に判定
				if (item.ClanMatch == "1") data.Team = "クラン戦";
				data.Mainarms = item.MainArms;
				data.Subarms = item.SubArms;
				data.Knife = item.Knife;
				data.Other = item.Other;
				data.Kill = item.Kill;
				data.Death = item.Death;
				data.GameType = item.GameType;
				data.Exp = item.Exp;
				data.Money = item.Money;
				data.PreExp = item.PreExp;
				data.PreMoney = item.PreMoney;
				data.KillTop = item.KillTop;
				data.DeathTop = item.DeathTop;
				data.KillBottom = item.KillBottom;
				data.DeathBottom = item.DeathBottom;
				data.KDTop = item.KDTop;
				data.KDBottom = item.KDBottom;

				DataList.Add(data);
			}

			return true;
		}

		/// <summary>
		/// listViewMainに追加するアイテム作成
		/// </summary>
		/// <param name="reclist"></param>
		/// <returns></returns>
		private ListViewItem CreateListViewItem(Data reclist)
		{
			//KD%計算(Kill:0 Death:0の場合はKD%:0)
			float kdper = 0;
			if ((float.Parse(reclist.Kill) + float.Parse(reclist.Death)) != 0)
			{
				kdper = float.Parse(reclist.Kill) / (float.Parse(reclist.Kill) + float.Parse(reclist.Death)) * 100;
			}

			ListViewItem itemx = new ListViewItem();
			itemx.UseItemStyleForSubItems = false;
			Font f = new Font(itemx.Font.Name, itemx.Font.Size, itemx.Font.Style);

			itemx.Text = reclist.Date;

			//Killハイライト
			if (Settings.Instance.HighlightKill == 1 && reclist.KillTop)
			{
				itemx.SubItems.Add(reclist.Kill, Color.Black, Color.Salmon, f);
			}
			else if (Settings.Instance.HighlightKill == -1 && reclist.KillBottom)
			{
				itemx.SubItems.Add(reclist.Kill, Color.Black, Color.LightSteelBlue, f);
			}
			else
			{
				itemx.SubItems.Add(reclist.Kill);
			}
			//Deathハイライト
			if (Settings.Instance.HighlightDeath == 1 && reclist.DeathTop)
			{
				itemx.SubItems.Add(reclist.Death, Color.Black, Color.Salmon, f);
			}
			else if (Settings.Instance.HighlightDeath == -1 && reclist.DeathBottom)
			{
				itemx.SubItems.Add(reclist.Death, Color.Black, Color.LightSteelBlue, f);
			}
			else
			{
				itemx.SubItems.Add(reclist.Death);
			}
			//K/Dハイライト
			if (Settings.Instance.HighlightKD == 1 && reclist.KDTop)
			{
				itemx.SubItems.Add(kdper.ToString(), Color.Black, Color.Salmon, f);
			}
			else if (Settings.Instance.HighlightKD == -1 && reclist.KDBottom)
			{
				itemx.SubItems.Add(kdper.ToString(), Color.Black, Color.LightSteelBlue, f);
			}
			else
			{
				itemx.SubItems.Add(kdper.ToString());
			}

			itemx.SubItems.Add(reclist.Map);
			itemx.SubItems.Add(reclist.Team);
			itemx.SubItems.Add(reclist.WinLose);
			itemx.SubItems.Add(reclist.Mainarms);
			itemx.SubItems.Add(reclist.Subarms);
			itemx.SubItems.Add(reclist.Knife);
			itemx.SubItems.Add(reclist.Other);
			itemx.SubItems.Add(reclist.Memo);
			//itemx.SubItems.Add(reclist.IndexNo);
			itemx.Tag = reclist.IndexNo;
			itemx.SubItems.Add(reclist.Exp);
			itemx.SubItems.Add(reclist.Money);

			return itemx;
		}

		/// <summary>
		/// リストビューにアイテムを追加
		/// </summary>
		private void ListViewItemAdd()
		{
			listViewMain.Items.Clear();

			//選択された日数を取得
			int SelectDays = (monthCalendar1.SelectionRange.End.Subtract(monthCalendar1.SelectionStart)).Days + 1;

			string[] item = new string[listViewMain.Columns.Count];

			//区間内KD用
			int SectionK = 0;
			int SectionD = 0;

			//区間内WL用
			int SectionW = 0;
			int SectionL = 0;
			int SectionDr = 0;

			//listViewMainに描画抑制
			listViewMain.BeginUpdate();

			foreach (Data reclist in DataList)
			{
				for (int j = 0; j < SelectDays; j++)
				{
					string strdate = reclist.Date;
					string[] strcmp = strdate.Split(new char[] { ' ' });

					//int comptmp = monthCalendar1.SelectionRange.Start.AddDays(j).ToString("d").CompareTo(strcmp[0]);

					//データの方をyyyyMMdd形式にし、カルチャ非依存で比較
					strcmp[0] = strcmp[0].Replace("/", "");
					string strdateFormat = monthCalendar1.SelectionRange.Start.AddDays(j).ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
					int comptmp = strdateFormat.CompareTo(strcmp[0]);

					//日付が一致するもののみ表示
					if (comptmp == 0)
					{
						//リストビューにアイテム追加
						listViewMain.Items.Add(CreateListViewItem(reclist));

						//区間内のKDを加算
						SectionK += int.Parse(reclist.Kill);
						SectionD += int.Parse(reclist.Death);

						//区間内のWLを加算
						if (reclist.WinLose == "勝ち") SectionW++;
						if (reclist.WinLose == "負け") SectionL++;
						if (reclist.WinLose == "引き分け") SectionDr++;
					}
				}
			}

			//textBoxSectionKD表示
			PrintSectionKD(SectionK, SectionD, SectionW, SectionL, SectionDr);

			//listViewMainに描画
			listViewMain.EndUpdate();
		}

		/// <summary>
		/// textBoxSectionKD表示
		/// </summary>
		/// <param name="sectionK"></param>
		/// <param name="sectionD"></param>
		private void PrintSectionKD(int sectionK, int sectionD, int sectionW, int sectionL, int sectionDr)
		{
			float SectionKD = 0;
			if ((sectionK + sectionD) != 0)
			{
				SectionKD = ((float)sectionK / (float)(sectionK + sectionD)) * 100;
				//SectionKD = (float)ToHalfAdjust(SectionKD, 2);
			}

			float SectionWL = 0;
			if ((sectionW + sectionL + sectionDr) != 0)
			{
				SectionWL = ((float)sectionW / (float)(sectionW + sectionL + sectionDr)) * 100;
				//SectionWL = (float)ToHalfAdjust(SectionWL, 2);
			}

			//区間内のKDを計算して表示
			if (listViewMain.Items.Count == 0)
			{
				textBoxSectionKD.Text = "K/D:--% (K:-- D:--) W/L:--% (W:-- L:-- D:--)";
			}
			else
			{
				textBoxSectionKD.Text = "K/D: " + SectionKD.ToString()
					+ "%(K: " + sectionK.ToString() + " D: " + sectionD.ToString()
					+ ") W/L: " + SectionWL.ToString() + "%(W: " + sectionW.ToString() + " L: " + sectionL.ToString()
					+ " D: " + sectionDr.ToString() + ")";
			}
		}

		/// <summary>
		/// textboxMainInfo表示
		/// </summary>
		private void PrintTextboxMainInfo()
		{
			//今日のペースであと何日か計算
			string pace = CalPaceKill();

			string tkd = "", ths = "";
			foreach (AccountData item in ada.DataList)
			{
				if (item.UserName == comboBoxActiveUser.Text)
				{
					tkd = item.TargetKD;
					ths = item.TargetHS;
				}
			}

			float totalKDper = CalTotalKDper(totaldata.TotalKill, totaldata.TotalDeath);
			float totalWLper = CalTotalWLper(totaldata.TotalWin, totaldata.TotalLose, totaldata.TotalDraw);
			double targetk = CalTargetKD(tkd, totaldata.TotalKill, totaldata.TotalDeath);

			//目標KDまでプラスの場合
			string strmaininfo = "";

			if (targetk >= 0)
			{
				strmaininfo = "現在のKD率: " + totalKDper + "% (Kill: " + totaldata.TotalKill + " Death: " + totaldata.TotalDeath + ")\r\n"
								   + "現在のWL率: " + totalWLper + "% (Win: " + totaldata.TotalWin + " Lose: " + totaldata.TotalLose + " Draw: " + totaldata.TotalDraw + ")\r\n\r\n"
								   + "目標KD率" + tkd + "% まで: " + targetk + " Kill " + pace + "\r\n";
			}
			//目標KDまでマイナスの場合
			else
			{
				targetk = CalTargetDown(tkd, totaldata.TotalKill, totaldata.TotalDeath);
				strmaininfo = "現在のKD率: " + totalKDper + "% (Kill: " + totaldata.TotalKill + " Death: " + totaldata.TotalDeath + ")\r\n"
								   + "現在のWL率: " + totalWLper + "% (Win: " + totaldata.TotalWin + " Lose: " + totaldata.TotalLose + " Draw: " + totaldata.TotalDraw + ")\r\n\r\n"
								   + "目標KD率" + tkd + "% まで: " + targetk + " Death\r\n";
			}

			//階級表示
			if (totaldata.Grade != "-1")
			{
				string strgrade = GetGradeTable(totaldata.Grade);
				strmaininfo += "\r\nGrade: " + strgrade;

				//%計算
				if (totaldata.NewExp != "-1")
				{
					double per = GetExpPer(totaldata.Grade, totaldata.NewExp);
					if (per != -1)
					{
						strmaininfo += " [" + per.ToString() + "%]";
					}
				}
			}

			//経験値表示
			if (totaldata.NewExp != "-1")
			{
				strmaininfo += "\r\nExp: " + totaldata.NewExp;
			}
			if (totaldata.OldExp != "-1" && totaldata.PreviousExp != "-1" && totaldata.PreviousExp != null)
			{
				int inter = int.Parse(totaldata.NewExp) - int.Parse(totaldata.OldExp);
				if (inter > 0)
				{
					strmaininfo += " (前回記録時から: +" + inter;
				}

				if (CalUpExp() > 0)
				{
					strmaininfo += "  今日: +" + CalUpExp() + ")";
				}
				else
				{
					if (inter > 0) strmaininfo += ")";
				}
			}

			//Money表示
			if (totaldata.NewMoney != "-1")
			{
				strmaininfo += "\r\nPoint: " + totaldata.NewMoney;
			}
			if (totaldata.OldMoney != "-1")
			{
				int inter = int.Parse(totaldata.NewMoney) - int.Parse(totaldata.OldMoney);
				int todayup = -99;
				string todayup_s = CalUpMoney();
				if (todayup_s != "err")
				{
					todayup = int.Parse(todayup_s);
				}

				if (inter > 0)
				{
					strmaininfo += " (前回記録時から: +" + inter;
				}
				else
				{
					strmaininfo += " (前回記録時から: " + inter;
				}

				if (todayup_s != "err")
				{
					if (todayup > 0)
					{
						strmaininfo += "  今日: +" + todayup + ")";
					}
					else
					{
						strmaininfo += "  今日: " + todayup + ")";
					}
				}
				else
				{
					if (inter > 0) strmaininfo += ")";
				}
			}

			//次の階級まで表示
			if (totaldata.NewExp != "-1" && int.Parse(totaldata.Grade) < 57)
			{
				int nextgrade = int.Parse(totaldata.Grade) + 1;

				string strnextgrade = GetGradeTable(nextgrade.ToString());
				int NeceExp = GetNeceExp(totaldata.Grade, nextgrade.ToString());

				if (NeceExp != -1)
				{
					string paceexp = CalPaceExp(NeceExp);
					strmaininfo += "\r\n\r\n" + strnextgrade + "まで: " + NeceExp + " Exp " + paceexp;
				}
			}

			string rankdate = "";
			string rankkill = "";
			string rankhs = "";
			foreach (RankData item in RankList)
			{
				rankdate = item.Date;
				rankkill = item.Kill;
				rankhs = item.HS;
			}

			if (rankdate != "")
			{
				float hsrate = 0.0f;
				double targeths = 0.0f;
				if (rankkill != "")
				{
					if (float.Parse(rankkill) != 0)
					{
						hsrate = float.Parse(rankhs) / float.Parse(rankkill) * 100;
						targeths = CalTargetHS(ths, rankhs, rankkill);
					}
				}

				strmaininfo += "\r\n\r\n" + rankdate + " UpDate"
					+ "\r\n現在のHS率: " + hsrate + "% (HS: " + rankhs + ")\r\n\r\n" + "目標HS率" + ths + "% まで: " + targeths + " HS";
			}

			textBoxMainInfo.Text = strmaininfo;
		}

		/// <summary>
		/// 経験値%計算
		/// </summary>
		/// <param name="grade">階級</param>
		/// <param name="exp">経験値</param>
		/// <returns>不正 = -1</returns>
		private double GetExpPer(string grade, string exp)
		{
			int Grade, Exp;
			bool resg, rese;
			resg = int.TryParse(exp, out Exp);
			rese = int.TryParse(grade, out Grade);

			int min_exp = 0;
			int max_exp = 0;

			if (resg && rese)
			{
				#region 階級別経験値
				switch (Grade)
				{
					case 0:
						min_exp = 0;
						max_exp = 2999;
						break;
					case 1:
						min_exp = 3000;
						max_exp = 8999;
						break;
					case 2:
						min_exp = 9000;
						max_exp = 17999;
						break;
					case 3:
						min_exp = 18000;
						max_exp = 29999;
						break;
					case 4:
						min_exp = 30000;
						max_exp = 44999;
						break;
					case 5:
						min_exp = 45000;
						max_exp = 64999;
						break;
					case 6:
						min_exp = 65000;
						max_exp = 84999;
						break;
					case 7:
						min_exp = 85000;
						max_exp = 104999;
						break;
					case 8:
						min_exp = 105000;
						max_exp = 134999;
						break;
					case 9:
						min_exp = 135000;
						max_exp = 164999;
						break;
					case 10:
						min_exp = 165000;
						max_exp = 194999;
						break;
					case 11:
						min_exp = 195000;
						max_exp = 224999;
						break;
					case 12:
						min_exp = 225000;
						max_exp = 274999;
						break;
					case 13:
						min_exp = 275000;
						max_exp = 324999;
						break;
					case 14:
						min_exp = 325000;
						max_exp = 374999;
						break;
					case 15:
						min_exp = 375000;
						max_exp = 424999;
						break;
					case 16:
						min_exp = 425000;
						max_exp = 474999;
						break;
					case 17:
						min_exp = 475000;
						max_exp = 574999;
						break;
					case 18:
						min_exp = 575000;
						max_exp = 674999;
						break;
					case 19:
						min_exp = 675000;
						max_exp = 774999;
						break;
					case 20:
						min_exp = 775000;
						max_exp = 874999;
						break;
					case 21:
						min_exp = 875000;
						max_exp = 974999;
						break;
					case 22:
						min_exp = 975000;
						max_exp = 1074999;
						break;
					case 23:
						min_exp = 1075000;
						max_exp = 1224999;
						break;
					case 24:
						min_exp = 1225000;
						max_exp = 1374999;
						break;
					case 25:
						min_exp = 1375000;
						max_exp = 1524999;
						break;
					case 26:
						min_exp = 1525000;
						max_exp = 1674999;
						break;
					case 27:
						min_exp = 1675000;
						max_exp = 1824999;
						break;
					case 28:
						min_exp = 1825000;
						max_exp = 1974999;
						break;
					case 29:
						min_exp = 1975000;
						max_exp = 2174999;
						break;
					case 30:
						min_exp = 2175000;
						max_exp = 2374999;
						break;
					case 31:
						min_exp = 2375000;
						max_exp = 2574999;
						break;
					case 32:
						min_exp = 2575000;
						max_exp = 2774999;
						break;
					case 33:
						min_exp = 2775000;
						max_exp = 2974999;
						break;
					case 34:
						min_exp = 2975000;
						max_exp = 3174999;
						break;
					case 35:
						min_exp = 3175000;
						max_exp = 3474999;
						break;
					case 36:
						min_exp = 3475000;
						max_exp = 3774999;
						break;
					case 37:
						min_exp = 3775000;
						max_exp = 4074999;
						break;
					case 38:
						min_exp = 4075000;
						max_exp = 4374999;
						break;
					case 39:
						min_exp = 4375000;
						max_exp = 4674999;
						break;
					case 40:
						min_exp = 4675000;
						max_exp = 4974999;
						break;
					case 41:
						min_exp = 4975000;
						max_exp = 5374999;
						break;
					case 42:
						min_exp = 5375000;
						max_exp = 5774999;
						break;
					case 43:
						min_exp = 5775000;
						max_exp = 6174999;
						break;
					case 44:
						min_exp = 6175000;
						max_exp = 6574999;
						break;
					case 45:
						min_exp = 6575000;
						max_exp = 6974999;
						break;
					case 46:
						min_exp = 6975000;
						max_exp = 7374999;
						break;
					case 47:
						min_exp = 7375000;
						max_exp = 7874999;
						break;
					case 48:
						min_exp = 7875000;
						max_exp = 8374999;
						break;
					case 49:
						min_exp = 8375000;
						max_exp = 8874999;
						break;
					case 50:
						min_exp = 8875000;
						max_exp = 9374999;
						break;
					case 51:
						min_exp = 9375000;
						max_exp = 9874999;
						break;
					case 52:
						min_exp = 9875000;
						max_exp = 10374999;
						break;
					case 53:
						min_exp = 10375000;
						max_exp = 11374999;
						break;
					case 54:
						min_exp = 11375000;
						max_exp = 13374999;
						break;
					case 55:
						min_exp = 13375000;
						max_exp = 16374999;
						break;
					case 56:
						min_exp = 16375000;
						max_exp = 20374999;
						break;
					case 57:
						min_exp = 20375000;
						max_exp = -1;
						break;
					default:
						min_exp = -1;
						max_exp = -1;
						break;
				}
				#endregion

				if (min_exp == -1 || max_exp == -1)
				{
					return -1;
				}

				return ToHalfAdjust((((double)Exp - (double)min_exp) / ((double)max_exp - (double)min_exp)) * 100, 1);
			}
			else
			{
				return -1;
			}
		}

		/// <summary>
		/// 目標階級までの必要経験値計算
		/// </summary>
		/// <param name="grade">現在の階級</param>
		/// <param name="targetgrade">目標階級</param>
		/// <returns>不正: -1</returns>
		private int GetNeceExp(string grade, string targetgrade)
		{
			if (totaldata.NewExp == "-1" || totaldata.NewExp == null || totaldata.NewExp == "") return -1;

			int NowExp = int.Parse(totaldata.NewExp);
			int TargetGrade = int.Parse(targetgrade);
			int neceExp = 0;
			int result = 0;

			#region 階級別必要経験値
			switch (TargetGrade)
			{
				case 0:
					neceExp = 0;
					break;
				case 1:
					neceExp = 3000;
					break;
				case 2:
					neceExp = 9000;
					break;
				case 3:
					neceExp = 18000;
					break;
				case 4:
					neceExp = 30000;
					break;
				case 5:
					neceExp = 45000;
					break;
				case 6:
					neceExp = 65000;
					break;
				case 7:
					neceExp = 85000;
					break;
				case 8:
					neceExp = 105000;
					break;
				case 9:
					neceExp = 135000;
					break;
				case 10:
					neceExp = 165000;
					break;
				case 11:
					neceExp = 195000;
					break;
				case 12:
					neceExp = 225000;
					break;
				case 13:
					neceExp = 275000;
					break;
				case 14:
					neceExp = 325000;
					break;
				case 15:
					neceExp = 375000;
					break;
				case 16:
					neceExp = 425000;
					break;
				case 17:
					neceExp = 475000;
					break;
				case 18:
					neceExp = 575000;
					break;
				case 19:
					neceExp = 675000;
					break;
				case 20:
					neceExp = 775000;
					break;
				case 21:
					neceExp = 875000;
					break;
				case 22:
					neceExp = 975000;
					break;
				case 23:
					neceExp = 1075000;
					break;
				case 24:
					neceExp = 1225000;
					break;
				case 25:
					neceExp = 1375000;
					break;
				case 26:
					neceExp = 1525000;
					break;
				case 27:
					neceExp = 1675000;
					break;
				case 28:
					neceExp = 1825000;
					break;
				case 29:
					neceExp = 1975000;
					break;
				case 30:
					neceExp = 2175000;
					break;
				case 31:
					neceExp = 2375000;
					break;
				case 32:
					neceExp = 2575000;
					break;
				case 33:
					neceExp = 2775000;
					break;
				case 34:
					neceExp = 2975000;
					break;
				case 35:
					neceExp = 3175000;
					break;
				case 36:
					neceExp = 3475000;
					break;
				case 37:
					neceExp = 3775000;
					break;
				case 38:
					neceExp = 4075000;
					break;
				case 39:
					neceExp = 4375000;
					break;
				case 40:
					neceExp = 4675000;
					break;
				case 41:
					neceExp = 4975000;
					break;
				case 42:
					neceExp = 5375000;
					break;
				case 43:
					neceExp = 5775000;
					break;
				case 44:
					neceExp = 6175000;
					break;
				case 45:
					neceExp = 6575000;
					break;
				case 46:
					neceExp = 6975000;
					break;
				case 47:
					neceExp = 7375000;
					break;
				case 48:
					neceExp = 7875000;
					break;
				case 49:
					neceExp = 8375000;
					break;
				case 50:
					neceExp = 8875000;
					break;
				case 51:
					neceExp = 9375000;
					break;
				case 52:
					neceExp = 9875000;
					break;
				case 53:
					neceExp = 10375000;
					break;
				case 54:
					neceExp = 11375000;
					break;
				case 55:
					neceExp = 13375000;
					break;
				case 56:
					neceExp = 16375000;
					break;
				case 57:
					neceExp = 20375000;
					break;
				default:
					neceExp = -1;
					break;
			}
			#endregion

			result = neceExp - NowExp;
			if (result <= 0)
			{
				return -1;
			}
			else
			{
				return result;
			}
		}

		/// <summary>
		/// 階級テーブル
		/// </summary>
		/// <param name="gradenum"></param>
		/// <returns></returns>
		private string GetGradeTable(string gradenum)
		{
			string restrgrade = "???";

			switch (gradenum)
			{
				case "0":
					restrgrade = "訓練兵";
					break;
				case "1":
					restrgrade = "二等兵";
					break;
				case "2":
					restrgrade = "一等兵";
					break;
				case "3":
					restrgrade = "上等兵";
					break;
				case "4":
					restrgrade = "兵長";
					break;
				case "5":
					restrgrade = "伍長1";
					break;
				case "6":
					restrgrade = "伍長2";
					break;
				case "7":
					restrgrade = "伍長3";
					break;
				case "8":
					restrgrade = "軍曹1";
					break;
				case "9":
					restrgrade = "軍曹2";
					break;
				case "10":
					restrgrade = "軍曹3";
					break;
				case "11":
					restrgrade = "軍曹4";
					break;
				case "12":
					restrgrade = "曹長1";
					break;
				case "13":
					restrgrade = "曹長2";
					break;
				case "14":
					restrgrade = "曹長3";
					break;
				case "15":
					restrgrade = "曹長4";
					break;
				case "16":
					restrgrade = "曹長5";
					break;
				case "17":
					restrgrade = "少尉1";
					break;
				case "18":
					restrgrade = "少尉2";
					break;
				case "19":
					restrgrade = "少尉3";
					break;
				case "20":
					restrgrade = "少尉4";
					break;
				case "21":
					restrgrade = "少尉5";
					break;
				case "22":
					restrgrade = "少尉6";
					break;
				case "23":
					restrgrade = "中尉1";
					break;
				case "24":
					restrgrade = "中尉2";
					break;
				case "25":
					restrgrade = "中尉3";
					break;
				case "26":
					restrgrade = "中尉4";
					break;
				case "27":
					restrgrade = "中尉5";
					break;
				case "28":
					restrgrade = "中尉6";
					break;
				case "29":
					restrgrade = "大尉1";
					break;
				case "30":
					restrgrade = "大尉2";
					break;
				case "31":
					restrgrade = "大尉3";
					break;
				case "32":
					restrgrade = "大尉4";
					break;
				case "33":
					restrgrade = "大尉5";
					break;
				case "34":
					restrgrade = "大尉6";
					break;
				case "35":
					restrgrade = "少佐1";
					break;
				case "36":
					restrgrade = "少佐2";
					break;
				case "37":
					restrgrade = "少佐3";
					break;
				case "38":
					restrgrade = "少佐4";
					break;
				case "39":
					restrgrade = "少佐5";
					break;
				case "40":
					restrgrade = "少佐6";
					break;
				case "41":
					restrgrade = "中佐1";
					break;
				case "42":
					restrgrade = "中佐2";
					break;
				case "43":
					restrgrade = "中佐3";
					break;
				case "44":
					restrgrade = "中佐4";
					break;
				case "45":
					restrgrade = "中佐5";
					break;
				case "46":
					restrgrade = "中佐6";
					break;
				case "47":
					restrgrade = "大佐1";
					break;
				case "48":
					restrgrade = "大佐2";
					break;
				case "49":
					restrgrade = "大佐3";
					break;
				case "50":
					restrgrade = "大佐4";
					break;
				case "51":
					restrgrade = "大佐5";
					break;
				case "52":
					restrgrade = "大佐6";
					break;
				case "53":
					restrgrade = "准将";
					break;
				case "54":
					restrgrade = "少将";
					break;
				case "55":
					restrgrade = "中将";
					break;
				case "56":
					restrgrade = "大将";
					break;
				case "57":
					restrgrade = "元帥";
					break;
				default:
					break;
			}

			return restrgrade;
		}

		/// <summary>
		/// トータルKD率計算
		/// </summary>
		/// <param name="totalkill">トータルKill</param>
		/// <param name="totaldeath">トータルDeath</param>
		/// <returns>トータルKD率</returns>
		private float CalTotalKDper(string totalkill, string totaldeath)
		{
			if ((totalkill == null) || (float.Parse(totalkill) <= 0)) return 0.0f;

			return float.Parse(totalkill) / (float.Parse(totalkill) + float.Parse(totaldeath)) * 100;
		}

		/// <summary>
		/// トータルWL率計算
		/// </summary>
		/// <param name="totalkill">トータルWin</param>
		/// <param name="totaldeath">トータルLose</param>
		/// <returns>トータルWL率</returns>
		private float CalTotalWLper(string totalwin, string totallose, string totaldraw)
		{
			if ((totalwin == null) || (float.Parse(totalwin) <= 0)) return 0.0f;
			if (totallose == null) totallose = "0";
			if (totaldraw == null) totaldraw = "0";

			return float.Parse(totalwin) / (float.Parse(totalwin) + float.Parse(totallose) + float.Parse(totaldraw)) * 100;
		}

		/// <summary>
		/// 目標KDまでのKill数計算
		/// </summary>
		/// <param name="targetKD">目標KD率</param>
		/// <param name="totalkill">トータルKill</param>
		/// <param name="totaldeath">トータルDeath</param>
		/// <returns>目標KDまでのKill数</returns>
		private double CalTargetKD(string targetKD, string totalkill, string totaldeath)
		{
			if ((targetKD == null) || (targetKD == "") || (totalkill == null) || (totalkill == "")) return 0.0f;

			double tmp = 0.0;
			double k = 0.0;
			double target_kd = 0.0;

			//. が２つ以上の場合例外
			try
			{
				target_kd = (double.Parse(targetKD)) / 100;
			}
			catch
			{
				return 0.0;
			}
			double kill = double.Parse(totalkill);
			double death = double.Parse(totaldeath);

			//目標KDまでの計算
			//(n/(1-n))*D-K
			tmp = (target_kd / (1.0 - target_kd));
			tmp = tmp * death;
			k = ToRoundUp(tmp - kill);

			return k;
		}
		/// <summary>
		/// 目標KDまでのDeath計算
		/// </summary>
		/// <param name="targetKD"></param>
		/// <param name="totalkill"></param>
		/// <param name="totaldeath"></param>
		/// <returns></returns>
		private double CalTargetDown(string targetKD, string totalkill, string totaldeath)
		{
			if ((targetKD == null) || (targetKD == "") || (totalkill == null) || (totalkill == "")) return 0.0f;

			double tmp = 0.0;
			double k = 0.0;
			double target_kd = 0.0;

			//. が２つ以上の場合例外
			try
			{
				target_kd = (double.Parse(targetKD)) / 100;
			}
			catch
			{
				return 0.0;
			}
			double kill = double.Parse(totalkill);
			double death = double.Parse(totaldeath);

			//目標KDまでの計算
			//1/(n/(1-n))*K-D
			tmp = kill / (target_kd / (1.0 - target_kd)) - death;
			k = ToRoundUp(tmp);

			return k;
		}

		/// <summary>
		/// 目標HSまでのHS数計算
		/// </summary>
		/// <param name="targetKD">目標HS率</param>
		/// <param name="totalkill">トータルHS</param>
		/// <param name="totaldeath">トータルKill</param>
		/// <returns>目標KDまでのKill数</returns>
		private double CalTargetHS(string targetHS, string totalhs, string totalkill)
		{
			if ((targetHS == null) || (targetHS == "") || (totalhs == null) || (totalhs == "")) return 0.0f;

			double tmp = 0.0;
			double k = 0.0;
			double target_hs = 0.0;

			//. が２つ以上の場合例外
			try
			{
				target_hs = (double.Parse(targetHS)) / 100;
			}
			catch
			{
				return 0.0;
			}
			double hs = double.Parse(totalhs);
			double kill = double.Parse(totalkill);

			tmp = (target_hs * kill - hs) / (1 - target_hs);
			k = ToRoundUp(tmp);

			return k;
		}
		/// <summary>
		///     指定した精度の数値に四捨五入します。</summary>
		/// <param name="dValue">
		///     丸め対象の倍精度浮動小数点数。</param>
		/// <param name="iDigits">
		///     戻り値の有効桁数の精度。</param>
		/// <returns>
		///     iDigits に等しい精度の数値に四捨五入された数値。</returns>
		public static double ToHalfAdjust(double dValue, int iDigits)
		{
			double dCoef = System.Math.Pow(10, iDigits);

			return dValue > 0 ? System.Math.Floor((dValue * dCoef) + 0.5) / dCoef :
								System.Math.Ceiling((dValue * dCoef) - 0.5) / dCoef;
		}
		/// <summary>
		/// 小数点第三位切り上げ
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static double ToRoundUp(double a)
		{
			decimal dotAlign = (decimal)a;
			dotAlign *= 1000;
			dotAlign = Math.Ceiling(dotAlign);
			dotAlign /= 1000;

			return (double)dotAlign;
		}

		/// <summary>
		/// 今日のペースであと何日かを計算(Kill)
		/// </summary>
		/// <returns>今日のペースの結果文字列</returns>
		public string CalPaceKill()
		{
			try
			{
				string resultstr = "";

				string targetKDstr = "";
				foreach (AccountData item in ada.DataList)
				{
					if (item.UserName == comboBoxActiveUser.Text)
					{
						targetKDstr = item.TargetKD;
					}
				}
				if (targetKDstr == "" || targetKDstr == null) return "";
				decimal targetKDPer = decimal.Parse(targetKDstr) / 100;

				float totalKDPer = CalTotalKDper(totaldata.TotalKill, totaldata.TotalDeath);
				//目標KD%よりトータルKD%の方が大きい場合はすぐ返す
				if (float.Parse(targetKDstr) < totalKDPer) return resultstr;

				string strdateFormat = DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
				string strdate;
				string[] strcmp;
				int comptmp = 0;

				decimal K = 0;
				decimal D = 0;

				foreach (Data reclist in DataList)
				{
					strdate = reclist.Date;
					strcmp = strdate.Split(new char[] { ' ' });

					//データの方をyyyyMMdd形式にし、カルチャ非依存で比較
					strcmp[0] = strcmp[0].Replace("/", "");
					comptmp = strdateFormat.CompareTo(strcmp[0]);

					//今日の分だけKDを加算
					if (comptmp == 0)
					{
						K += int.Parse(reclist.Kill);
						D += int.Parse(reclist.Death);
					}
				}

				//KDの合計が0の場合は返す
				if ((K + D) <= 0) return "";
				//今日のKD%が目標KD%より低い場合は返す
				decimal todayKDPer = (decimal)K / ((decimal)K + (decimal)D);
				if (todayKDPer < targetKDPer) return "";

				decimal dTotalKill = decimal.Parse(totaldata.TotalKill);
				decimal dTotalDeath = decimal.Parse(totaldata.TotalDeath);

				//ペースを計算
				/*
				 * (Kill * x + totalkill) / (Kill * x + totalkill) + (Death * x + totaldeath) = targetKD
				 * を展開
				 */

				if ((K - targetKDPer * K - targetKDPer * D) <= 0) return "";
				double pace = (double)((dTotalKill * targetKDPer + dTotalDeath * targetKDPer - dTotalKill) / (K - targetKDPer * K - targetKDPer * D));

				//四捨五入
				pace = ToHalfAdjust(pace, 1);

				if (pace >= 365)
				{
					resultstr = "(今日のペースであと: 1年以上)";
				}
				else
				{
					resultstr = "(今日のペースであと: " + pace + "日)";
				}

				return resultstr;

			}
			catch (System.DivideByZeroException)
			{
				return "";
			}
		}

		/// <summary>
		/// 今日のペースであと何日かを計算(Exp)
		/// </summary>
		/// <param name="NeceExp">必要Exp</param>
		/// <returns>今日のペースの結果文字列</returns>
		private string CalPaceExp(int NeceExp)
		{
			if (NeceExp <= 0) return "";

			string strdateFormat = DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
			string strdate;
			string[] strcmp;
			int comptmp = -1;
			bool blre = false;

			//今日のデータがなかったらreturn
			foreach (Data item in DataList)
			{
				strdate = item.Date;
				strcmp = strdate.Split(new char[] { ' ' });

				//データの方をyyyyMMdd形式にし、カルチャ非依存で比較
				strcmp[0] = strcmp[0].Replace("/", "");
				comptmp = strdateFormat.CompareTo(strcmp[0]);

				if (comptmp == 0)
				{
					blre = true;
					break;
				}
			}
			if (blre == false) return "";

			string resultstr = "";
			int TodayUpExp = CalUpExp();

			//ペース計算
			int cnt;
			for (cnt = 1; cnt <= 365; cnt++)
			{
				NeceExp -= TodayUpExp;
				if (NeceExp <= 0) break;
			}
			if (cnt < 365)
			{
				resultstr = "(今日のペースであと: " + cnt + "日)";
			}
			else
			{
				resultstr = "(今日のペースであと: 1年以上)";
			}

			return resultstr;
		}

		/// <summary>
		/// 今日のExp増加分計算
		/// </summary>
		/// <returns>今日の増加分Exp</returns>
		private int CalUpExp()
		{
			if (totaldata.NewExp == "-1" || totaldata.NewExp == null || totaldata.NewExp == ""
			 || totaldata.PreviousExp == "-1" || totaldata.PreviousExp == null || totaldata.PreviousExp == "")
			{
				return -1;
			}

			return int.Parse(totaldata.NewExp) - int.Parse(totaldata.PreviousExp);

			//int todayminexp = -1;
			//int todaymaxexp = 0;
			//int itemexp = 0;
			//int itempreexp = 0;

			//string strdateFormat = DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
			//string strdate;
			//string[] strcmp;
			//int comptmp = 0;

			//foreach (Data item in DataList)
			//{
			//    if (item.Exp == null) continue;
			//    itemexp = int.Parse(item.Exp);

			//    if (item.PreExp != "")
			//    {
			//        if (item.PreExp == null) continue;
			//        itempreexp = int.Parse(item.PreExp);
			//    }
			//    else
			//    {
			//        itempreexp = -1;
			//    }

			//    strdate = item.Date;
			//    strcmp = strdate.Split(new char[] { ' ' });

			//    //データの方をyyyyMMdd形式にし、カルチャ非依存で比較
			//    strcmp[0] = strcmp[0].Replace("/", "");
			//    comptmp = strdateFormat.CompareTo(strcmp[0]);

			//    //今日の分だけ
			//    if (comptmp == 0)
			//    {
			//        //最小Exp
			//        if (todayminexp == -1 || ((itempreexp != -1) && (itempreexp < todayminexp)))
			//        {
			//            todayminexp = itempreexp;
			//        }
			//        //最大Exp
			//        if (itemexp > todaymaxexp)
			//        {
			//            todaymaxexp = itemexp;
			//        }
			//    }
			//}

			//if (todayminexp == -1) return 0;

			////最小Expと最大Expの差分を計算し今日の増加分とする
			//return todaymaxexp - todayminexp;
		}

		/// <summary>
		/// 今日のMoney増加分計算
		/// </summary>
		/// <returns>今日の増加分Money 不正:"err"</returns>
		private string CalUpMoney()
		{
			if (totaldata.NewMoney == "-1" || totaldata.NewMoney == null || totaldata.NewMoney == ""
			 || totaldata.PreviousMoney == "-1" || totaldata.PreviousMoney == null || totaldata.PreviousMoney == "")
			{
				return "err";
			}

			return (int.Parse(totaldata.NewMoney) - int.Parse(totaldata.PreviousMoney)).ToString();

			//DateTime mindate = DateTime.Now;
			//int startmoney = -1;
			//int itempremoney = 0;

			//string strdateFormat = DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
			//string strdate;
			//string[] strcmp;
			//int comptmp = 0;

			//foreach (Data item in DataList)
			//{
			//    if (item.PreMoney != "")
			//    {
			//        try
			//        {
			//            if (item.PreMoney != null)
			//            {
			//                itempremoney = int.Parse(item.PreMoney);
			//            }
			//        }
			//        catch
			//        {
			//            return "err";
			//        }
			//    }
			//    else
			//    {
			//        itempremoney = -1;
			//    }

			//    strdate = item.Date;
			//    strcmp = strdate.Split(new char[] { ' ' });

			//    //データの方をyyyyMMdd形式にし、カルチャ非依存で比較
			//    strcmp[0] = strcmp[0].Replace("/", "");
			//    comptmp = strdateFormat.CompareTo(strcmp[0]);

			//    //今日の分だけ
			//    if (comptmp == 0)
			//    {
			//        DateTime date2 = DateTime.Parse(item.Date);
			//        //最小indexNo
			//        if (date2 < mindate)
			//        {
			//            mindate = date2;
			//            startmoney = itempremoney;
			//        }
			//    }
			//}

			//if (startmoney == -1) return "err";

			////現在moneyとstartmoneyの差分を計算し今日の増加分とする
			//return (int.Parse(totaldata.NewMoney) - startmoney).ToString();
		}

		/// <summary>
		/// ListViewのアイテム選択がカレンダーなのか任意選択なのか
		/// </summary>
		private enum SelectedListItem
		{
			CalendarSelect,
			UserSelect,
			AllSerect
		}
		private SelectedListItem _SLI = SelectedListItem.CalendarSelect;
		/// <summary>
		/// カレンダーセレクトイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
		{
			//カレンダーセレクト
			_SLI = SelectedListItem.CalendarSelect;

			//pictureBoxGraphを描画しなおす
			pictureBoxGraph.Invalidate();

			//ListView表示
			ListViewItemAdd();
		}

		/// <summary>
		/// 全てのデータ選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolStripMenuItemAllSelect_Click(object sender, EventArgs e)
		{
			//オールセレクトにする
			_SLI = SelectedListItem.AllSerect;

			listViewMain.Items.Clear();

			//区間内KD用
			int SectionK = 0;
			int SectionD = 0;

			//区間内WL用
			int SectionW = 0;
			int SectionL = 0;
			int SectionDr = 0;

			//listViewMainに描画抑制
			listViewMain.BeginUpdate();

			foreach (Data reclist in DataList)
			{
				//リストビューにアイテム追加
				listViewMain.Items.Add(CreateListViewItem(reclist));

				//区間内のKDを加算
				SectionK += int.Parse(reclist.Kill);
				SectionD += int.Parse(reclist.Death);

				//区間内のWLを加算
				if (reclist.WinLose == "勝ち") SectionW++;
				if (reclist.WinLose == "負け") SectionL++;
				if (reclist.WinLose == "引き分け") SectionDr++;
			}

			//textBoxSectionKD表示
			PrintSectionKD(SectionK, SectionD, SectionW, SectionL, SectionDr);

			//listViewMainに描画
			listViewMain.EndUpdate();

			//pictureBoxGraphを描画しなおす
			pictureBoxGraph.Invalidate();
		}

		/// <summary>
		/// 数字か . のみ入力できるようにする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void textboxNum_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b' && e.KeyChar != '.')
			{
				e.Handled = true;
			}
		}
		/// <summary>
		/// EnterキーでもOKを押した事にする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBoxOption_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter) OptionOK();
		}

		/// <summary>
		/// グラフとリストビューの境界線
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void splitContainer3_SplitterMoved(object sender, SplitterEventArgs e)
		{
			pictureBoxGraph.Invalidate();
		}

		/// <summary>
		/// オプションOKボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonOptionOK_Click(object sender, EventArgs e)
		{
			OptionOK();
		}
		private void tabPageOption_Leave(object sender, EventArgs e)
		{
			OptionOK();
		}
		private void OptionOK()
		{
			Settings.Instance.ErrorLogPath = textBoxerrorlogPath.Text;
			Settings.Instance.GraphDaycountCheck = checkBoxGraphDaycount.Checked;
			Settings.Instance.GetRankCheck = checkBoxGetRank.Checked;
			Settings.Instance.LoginMinCheck = checkBoxLoginMin.Checked;
			Settings.Instance.TitleIDDraw = checkBoxTitleIDDraw.Checked;

			Settings.Instance.LoginLeftFormCheck = (int)numericUpDownLoginLeftFormCheck.Value;
			Settings.Instance.PreExe = (int)numericUpDownPreExe.Value;
			Settings.Instance.getLinkGameExecute = (int)numericUpDowngetLinkGameExecute.Value;
			Settings.Instance.AutoClose = (int)numericUpDownAutoClose.Value;

			Settings.Instance.rbSALogin1x = radioButtonSALoginver1x.Checked;
			Settings.Instance.rbSALogin2x = radioButtonSALoginver2x.Checked;

			Settings.Instance.GraphXLineClear = checkBoxXlineclear.Checked;

			Settings.Instance.GraphDayUnit = (int)nUDDayCount.Value;

			if (rbHighlightKillTop.Checked) Settings.Instance.HighlightKill = 1;
			if (rbHighlightKillBottom.Checked) Settings.Instance.HighlightKill = -1;
			if (rbHighlightKillNone.Checked) Settings.Instance.HighlightKill = 0;
			if (rbHighlightDeathTop.Checked) Settings.Instance.HighlightDeath = 1;
			if (rbHighlightDeathBottom.Checked) Settings.Instance.HighlightDeath = -1;
			if (rbHighlightDeathNone.Checked) Settings.Instance.HighlightDeath = 0;
			if (rbHighlightKDTop.Checked) Settings.Instance.HighlightKD = 1;
			if (rbHighlightKDBottom.Checked) Settings.Instance.HighlightKD = -1;
			if (rbHighlightKDNone.Checked) Settings.Instance.HighlightKD = 0;

			Settings.SaveToBinaryFile();

			PrintTextboxMainInfo();

			GetState();

			TitleDraw();

			ListViewItemAdd();
		}

		/// <summary>
		/// 縦線表示チェックボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkBoxXlineclear_CheckedChanged(object sender, EventArgs e)
		{
			pictureBoxGraph.Invalidate();
			pictureBoxCategoryGraph.Invalidate();
		}

		/// <summary>
		/// オプションタブがアクティブ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPage2_Enter(object sender, EventArgs e)
		{
			textBoxerrorlogPath.Text = Settings.Instance.ErrorLogPath;
			checkBoxGraphDaycount.Checked = Settings.Instance.GraphDaycountCheck;
			checkBoxGetRank.Checked = Settings.Instance.GetRankCheck;
			checkBoxLoginMin.Checked = Settings.Instance.LoginMinCheck;
			numericUpDownLoginLeftFormCheck.Value = Settings.Instance.LoginLeftFormCheck;
			numericUpDownPreExe.Value = Settings.Instance.PreExe;
			numericUpDowngetLinkGameExecute.Value = Settings.Instance.getLinkGameExecute;
			numericUpDownAutoClose.Value = Settings.Instance.AutoClose;
			checkBoxTitleIDDraw.Checked = Settings.Instance.TitleIDDraw;
			checkBoxXlineclear.Checked = Settings.Instance.GraphXLineClear;

			if (Settings.Instance.GraphDayUnit == 0) Settings.Instance.GraphDayUnit = 1;
			nUDDayCount.Value = (decimal)Settings.Instance.GraphDayUnit;

			if (Settings.Instance.rbSALogin1x == false && Settings.Instance.rbSALogin2x == false)
			{
				radioButtonSALoginver1x.Checked = true;
			}
			else
			{
				radioButtonSALoginver1x.Checked = Settings.Instance.rbSALogin1x;
				radioButtonSALoginver2x.Checked = Settings.Instance.rbSALogin2x;
			}

			//ハイライト設定
			//Kill
			if (Settings.Instance.HighlightKill > 0)
			{
				rbHighlightKillTop.Checked = true;
			}
			else if (Settings.Instance.HighlightKill < 0)
			{
				rbHighlightKillBottom.Checked = true;
			}
			else
			{
				rbHighlightKillNone.Checked = true;
			}
			//Death
			if (Settings.Instance.HighlightDeath > 0)
			{
				rbHighlightDeathTop.Checked = true;
			}
			else if (Settings.Instance.HighlightDeath < 0)
			{
				rbHighlightDeathBottom.Checked = true;
			}
			else
			{
				rbHighlightDeathNone.Checked = true;
			}
			//K/D
			if (Settings.Instance.HighlightKD > 0)
			{
				rbHighlightKDTop.Checked = true;
			}
			else if (Settings.Instance.HighlightKD < 0)
			{
				rbHighlightKDBottom.Checked = true;
			}
			else
			{
				rbHighlightKDNone.Checked = true;
			}

			//SALoginデフォルト値(4,9,12,13)
			if (numericUpDownLoginLeftFormCheck.Value == 0
				|| numericUpDownPreExe.Value == 0
				|| numericUpDowngetLinkGameExecute.Value == 0
				|| numericUpDownAutoClose.Value == 0)
			{
				numericUpDownLoginLeftFormCheck.Value = 4;
				numericUpDownPreExe.Value = 9;
				numericUpDowngetLinkGameExecute.Value = 12;
				numericUpDownAutoClose.Value = 13;
			}
		}

		//private bool windowsizereset = false;
		private int dis1;
		private int dis2;
		private int dis3;
		/// <summary>
		/// デフォルトサイズに戻す
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SetDefaultSize()
		{
			//tabpageをアクティブにしてからSplitterDistanceを設定しないとちゃんと戻らない?
			dis1 = 71;
			dis2 = 149;
			dis3 = 180;

			//windowsizereset = true;
			this.WindowState = FormWindowState.Normal;
			this.Size = new Size(697, 700);
			tabControl1.SelectedIndex = 0;

			splitContainer1.SplitterDistance = dis1;
			splitContainer2.SplitterDistance = dis2;
			splitContainer3.SplitterDistance = dis3;
		}
		private void buttonWindowsSizeReset_Click(object sender, EventArgs e)
		{
			SetDefaultSize();
		}
		private void tabPageDataList_Enter(object sender, EventArgs e)
		{
		}
		private void FormSAAR_Resize(object sender, EventArgs e)
		{
		}
		private void FormSAAR_SizeChanged(object sender, EventArgs e)
		{
		}
		/// <summary>
		/// ユーザサイズに戻す
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SetUserSize()
		{
			if (Settings.Instance.UserSizeWinX != 0 && Settings.Instance.UserSizeWinY != 0)
			{
				if (Settings.Instance.UserWinMax)
				{
					this.WindowState = FormWindowState.Maximized;
				}
				else
				{
					this.WindowState = FormWindowState.Normal;
				}
				this.Size = new Size(Settings.Instance.UserSizeWinX, Settings.Instance.UserSizeWinY);
			}

			if (Settings.Instance.UserSizedis1 != 0)
			{
				dis1 = Settings.Instance.UserSizedis1;
			}
			if (Settings.Instance.UserSizedis2 != 0)
			{
				dis2 = Settings.Instance.UserSizedis2;
			}
			if (Settings.Instance.UserSizedis3 != 0)
			{
				dis3 = Settings.Instance.UserSizedis3;
			}

			//windowsizereset = true;

			tabControl1.SelectedIndex = 0;

			splitContainer1.SplitterDistance = dis1;
			splitContainer2.SplitterDistance = dis2;
			splitContainer3.SplitterDistance = dis3;
			//windowsizereset = false;
		}
		private void buttonUserSizeReturn_Click(object sender, EventArgs e)
		{
			SetUserSize();
		}
		/// <summary>
		/// ユーザサイズ保存
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonUserSizeSave_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("現在のサイズを保存しますか？", "ユーザサイズ保存", MessageBoxButtons.YesNo)
				== DialogResult.Yes)
			{
				if (this.WindowState == FormWindowState.Normal)
				{
					Settings.Instance.UserWinMax = false;
				}
				else
				{
					Settings.Instance.UserWinMax = true;
				}
				Settings.Instance.UserSizeWinX = this.Size.Width;
				Settings.Instance.UserSizeWinY = this.Size.Height;
				Settings.Instance.UserSizedis1 = splitContainer1.SplitterDistance;
				Settings.Instance.UserSizedis2 = splitContainer2.SplitterDistance;
				Settings.Instance.UserSizedis3 = splitContainer3.SplitterDistance;
			}
		}

		/// <summary>
		/// 色の設定
		/// </summary>
		private void buttonOptionMGColor_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			if (cd.ShowDialog() == DialogResult.OK)
			{
				Settings.Instance.MGColor_R = cd.Color.R;
				Settings.Instance.MGColor_G = cd.Color.G;
				Settings.Instance.MGColor_B = cd.Color.B;
			}
			Settings.Instance.ColorSet = true;
		}
		private void buttonbuttonOptionMGColor2_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			if (cd.ShowDialog() == DialogResult.OK)
			{
				Settings.Instance.MGColor2_R = cd.Color.R;
				Settings.Instance.MGColor2_G = cd.Color.G;
				Settings.Instance.MGColor2_B = cd.Color.B;
			}
			Settings.Instance.ColorSet = true;
		}
		private void buttonOptionRGColor_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			if (cd.ShowDialog() == DialogResult.OK)
			{
				Settings.Instance.RGColor_R = cd.Color.R;
				Settings.Instance.RGColor_G = cd.Color.G;
				Settings.Instance.RGColor_B = cd.Color.B;
			}
			Settings.Instance.ColorSet = true;
		}
		private void buttonOptionCGColor_Click(object sender, EventArgs e)
		{
			ColorDialog cd = new ColorDialog();
			if (cd.ShowDialog() == DialogResult.OK)
			{
				Settings.Instance.CGColor_R = cd.Color.R;
				Settings.Instance.CGColor_G = cd.Color.G;
				Settings.Instance.CGColor_B = cd.Color.B;
			}
			Settings.Instance.ColorSet = true;
		}
		private void buttonOptionColorReset_Click(object sender, EventArgs e)
		{
			ColorReset();
			Settings.Instance.ColorSet = true;
		}
		private void ColorReset()
		{
			Settings.Instance.MGColor_R = 255;
			Settings.Instance.MGColor_G = 0;
			Settings.Instance.MGColor_B = 0;
			Settings.Instance.MGColor2_R = 0;
			Settings.Instance.MGColor2_G = 0;
			Settings.Instance.MGColor2_B = 255;
			Settings.Instance.RGColor_R = 139;
			Settings.Instance.RGColor_G = 19;
			Settings.Instance.RGColor_B = 69;
			Settings.Instance.CGColor_R = 128;
			Settings.Instance.CGColor_G = 0;
			Settings.Instance.CGColor_B = 128;
		}

		/// <summary>
		/// SALoginバージョン選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioButtonSALoginver2x_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonSALoginver2x.Checked)
			{
				groupBoxLoginOption.Enabled = false;
			}
		}
		private void radioButtonSALoginver1x_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonSALoginver1x.Checked)
			{
				groupBoxLoginOption.Enabled = true;
			}
		}

		/// <summary>
		/// ListViewアイテム選択イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			int SectionK = 0;
			int SectionD = 0;

			int SectionW = 0;
			int SectionL = 0;
			int SectionDr = 0;

			if (listViewMain.SelectedItems.Count == 0)
			{
				//アイテム選択が0個の時はカレンダーセレクトに
				_SLI = SelectedListItem.CalendarSelect;

				//区間内のKDを計算
				for (int i = 0; i < listViewMain.Items.Count; i++)
				{
					SectionK += int.Parse(listViewMain.Items[i].SubItems[columnHeaderKill.Index].Text);
					SectionD += int.Parse(listViewMain.Items[i].SubItems[columnHeaderDeath.Index].Text);

					if (listViewMain.Items[i].SubItems[columnHeaderWinLose.Index].Text == "勝ち") SectionW++;
					if (listViewMain.Items[i].SubItems[columnHeaderWinLose.Index].Text == "負け") SectionL++;
					if (listViewMain.Items[i].SubItems[columnHeaderWinLose.Index].Text == "引き分け") SectionDr++;
				}
			}
			else
			{
				//任意セレクト
				_SLI = SelectedListItem.UserSelect;

				//区間内のKDを計算
				ListView.SelectedListViewItemCollection _ListViewItems = listViewMain.SelectedItems;
				foreach (ListViewItem item in _ListViewItems)
				{
					SectionK += int.Parse(item.SubItems[columnHeaderKill.Index].Text);
					SectionD += int.Parse(item.SubItems[columnHeaderDeath.Index].Text);

					if (item.SubItems[columnHeaderWinLose.Index].Text == "勝ち") SectionW++;
					if (item.SubItems[columnHeaderWinLose.Index].Text == "負け") SectionL++;
					if (item.SubItems[columnHeaderWinLose.Index].Text == "引き分け") SectionDr++;
				}
			}

			//pictureBoxGraphを描画しなおす
			pictureBoxGraph.Invalidate();

			//区間内のKDを表示
			PrintSectionKD(SectionK, SectionD, SectionW, SectionL, SectionDr);
		}

		/// <summary>
		/// pictureboxペイント(グラフ描画)イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureBoxGraph_Paint(object sender, PaintEventArgs e)
		{
			if (_SLI == SelectedListItem.AllSerect && Settings.Instance.GraphDaycountCheck == true)
			{
				lbGraphDayUnitInfo.Text = Settings.Instance.GraphDayUnit.ToString() + "日毎";
			}
			else
			{
				lbGraphDayUnitInfo.Text = "";
			}

			//Y目盛りを描画
			for (int i = 0; i < 10; i++)
			{
				//50ラインだけ濃く
				if (i == 5)
				{

					e.Graphics.DrawLine(new Pen(Color.FromArgb(135, 120, 100)), 0, (pictureBoxGraph.Height / 10) * i - 1, pictureBoxGraph.Width, (pictureBoxGraph.Height / 10) * i - 1);
				}
				else
				{
					e.Graphics.DrawLine(Pens.DarkGray, 0, (pictureBoxGraph.Height / 10) * i - 1, pictureBoxGraph.Width, (pictureBoxGraph.Height / 10) * i - 1);
				}
			}

			//ListViewに１行以下しか表示されていない場合はreturn
			if (listViewMain.Items.Count <= 1)
			{
				return;
			}

			//セレクトによってグラフにするデータを決める
			int count = 0;
			if (_SLI == SelectedListItem.CalendarSelect)
			{
				count = listViewMain.Items.Count;
			}
			if (_SLI == SelectedListItem.UserSelect)
			{
				if (listViewMain.SelectedItems.Count < 2)
				{
					return;
				}
				count = listViewMain.SelectedItems.Count;
			}

			int DayUnit = Settings.Instance.GraphDayUnit;
			if (_SLI == SelectedListItem.AllSerect)
			{
				if (Settings.Instance.GraphDaycountCheck)
				{
					List<string> buf = new List<string>();
					foreach (Data item in DataList)
					{
						string[] strcmp = item.Date.Split(new char[] { ' ' });

						if (!(buf.Exists(s => s == strcmp[0])))
						{
							count++;
							buf.Add(strcmp[0]);
						}
					}

					//指定日毎
					int ctmp = count / DayUnit;
					if (count % DayUnit == 0)
					{
						count = ctmp;
					}
					else
					{
						count = ctmp + 1;
					}
					if (count <= 1) return;
				}
				else
				{
					count = listViewMain.Items.Count;
				}
			}

			//行数分Xpointを決める
			int[] Xpoint = new int[count];
			for (int i = 0; i < count; i++)
			{
				if (i == 0)
				{
					//一番左はX座標は常に0
					Xpoint[i] = 0;
				}
				else if (i == (count - 1))
				{
					//一番右はX座標は常にpictureBoxGraphの最後
					Xpoint[i] = pictureBoxGraph.Width;
				}
				else
				{
					Xpoint[i] = (int)(((float)pictureBoxGraph.Width / ((float)count - 1.0F)) * (float)i);
				}
			}

			if (Settings.Instance.GraphXLineClear == false)
			{
				//Xpointに縦線を引く
				for (int i = 1; i < (count - 1); i++)
				{
					e.Graphics.DrawLine(Pens.DarkGray, Xpoint[i], 0, Xpoint[i], pictureBoxGraph.Height);
				}
			}

			//KD率を格納する
			float[] KDrate = new float[count];
			//日付バッファ
			List<string> bufstrdate = new List<string>();
			if ((_SLI == SelectedListItem.AllSerect) && (Settings.Instance.GraphDaycountCheck == true))
			{
				foreach (Data item in DataList)
				{
					string[] strcmp = item.Date.Split(new char[] { ' ' });
					if (!bufstrdate.Exists(s => s == strcmp[0])) bufstrdate.Add(strcmp[0]);
				}
			}

			if (_SLI == SelectedListItem.CalendarSelect)
			{
				for (int i = 0; i < count; i++)
				{
					KDrate[i] = float.Parse(listViewMain.Items[i].SubItems[columnHeaderRate.Index].Text) / 100;
				}
			}
			if (_SLI == SelectedListItem.UserSelect)
			{
				for (int i = 0; i < count; i++)
				{
					KDrate[i] = float.Parse(listViewMain.SelectedItems[i].SubItems[columnHeaderRate.Index].Text) / 100;
				}
			}
			if (_SLI == SelectedListItem.AllSerect)
			{
				//指定日毎表示
				if (Settings.Instance.GraphDaycountCheck)
				{
					int ite = 0;
					for (int i = 0; i < count; i++)
					{
						float Ktmp = 0;
						float Dtmp = 0;

						for (int j = 0; j < DayUnit; j++)
						{
							foreach (Data item in DataList)
							{
								string[] strcmp = item.Date.Split(new char[] { ' ' });

								if (bufstrdate[ite] == strcmp[0])
								{
									Ktmp += float.Parse(item.Kill);
									Dtmp += float.Parse(item.Death);
								}
							}
							ite++;
							if (ite >= bufstrdate.Count) break;
						}

						if ((Ktmp + Dtmp) != 0)
						{
							KDrate[i] = Ktmp / (Ktmp + Dtmp);
						}
						else
						{
							KDrate[i] = 0;
						}
					}
				}
				else
				{
					for (int i = 0; i < count; i++)
					{
						KDrate[i] = float.Parse(listViewMain.Items[i].SubItems[columnHeaderRate.Index].Text) / 100;
					}
				}
			}


			//それぞれのYpointを決める
			int[] Ypoint = new int[count];

			for (int i = 0; i < count; i++)
			{
				Ypoint[i] = (int)(((float)pictureBoxGraph.Height) - ((float)pictureBoxGraph.Height * KDrate[i]));
			}

			//グラフ本線を描画する
			Point[] p = new Point[count];
			for (int i = 0; i < count; i++)
			{
				p[i] = new Point(Xpoint[i], Ypoint[i]);
			}

			Pen cr;
			if ((_SLI == SelectedListItem.AllSerect) && (Settings.Instance.GraphDaycountCheck == true))
			{
				cr = new Pen(Color.FromArgb(Settings.Instance.MGColor2_R, Settings.Instance.MGColor2_G, Settings.Instance.MGColor2_B));
			}
			else
			{
				cr = new Pen(Color.FromArgb(Settings.Instance.MGColor_R, Settings.Instance.MGColor_G, Settings.Instance.MGColor_B));
			}

			e.Graphics.DrawLines(cr, p);

			cr.Dispose();
		}

		private bool order_asc = true;
		/// <summary>
		/// ListViewソート
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			//昇順降順を逆にする
			order_asc = !order_asc;

			//昇順の場合
			if (order_asc)
			{
				if ((((ListView)sender).Columns[e.Column] == columnHeaderKill) || (((ListView)sender).Columns[e.Column] == columnHeaderDeath)
					|| (((ListView)sender).Columns[e.Column] == columnHeaderExp) || (((ListView)sender).Columns[e.Column] == columnHeaderMoney))
				{
					//ListViewItemSorterIntを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerIntAsc(e.Column);
				}
				else if (((ListView)sender).Columns[e.Column] == columnHeaderRate)
				{
					//ListViewItemSorterFloatを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerFloatAsc(e.Column);
				}
				else if (((ListView)sender).Columns[e.Column] == columnHeaderDate)
				{
					//ListViewItemComparerDateAscを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerDateAsc(e.Column);
				}
				else
				{
					//ListViewItemSorterStrを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerStrAsc(e.Column);
				}
			}
			//降順の場合
			else
			{
				if ((((ListView)sender).Columns[e.Column] == columnHeaderKill) || (((ListView)sender).Columns[e.Column] == columnHeaderDeath)
					|| (((ListView)sender).Columns[e.Column] == columnHeaderExp) || (((ListView)sender).Columns[e.Column] == columnHeaderMoney))
				{
					//ListViewItemSorterIntを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerIntDesc(e.Column);
				}
				else if (((ListView)sender).Columns[e.Column] == columnHeaderRate)
				{
					//ListViewItemSorterFloatを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerFloatDesc(e.Column);
				}
				else if (((ListView)sender).Columns[e.Column] == columnHeaderDate)
				{
					//ListViewItemComparerDateDescを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerDateDesc(e.Column);
				}
				else
				{
					//ListViewItemSorterStrを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerStrDesc(e.Column);
				}
			}

			//グラフ再描画
			pictureBoxGraph.Invalidate();
		}

		private void listViewCategory_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			//昇順降順を逆にする
			order_asc = !order_asc;

			//昇順の場合
			if (order_asc)
			{
				if ((((ListView)sender).Columns[e.Column] == columnHeaderCategoryKill) || (((ListView)sender).Columns[e.Column] == columnHeaderCategoryDeath))
				{
					//ListViewItemSorterIntを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerIntAsc(e.Column);
				}
				else if (((ListView)sender).Columns[e.Column] == columnHeaderCategoryRate)
				{
					//ListViewItemSorterFloatを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerFloatAsc(e.Column);
				}
				else if (((ListView)sender).Columns[e.Column] == columnHeaderCategoryDate)
				{
					//ListViewItemComparerDateAscを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerDateAsc(e.Column);
				}
				else
				{
					//ListViewItemSorterStrを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerStrAsc(e.Column);
				}
			}
			//降順の場合
			else
			{
				if ((((ListView)sender).Columns[e.Column] == columnHeaderCategoryKill) || (((ListView)sender).Columns[e.Column] == columnHeaderCategoryDeath))
				{
					//ListViewItemSorterIntを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerIntDesc(e.Column);
				}
				else if (((ListView)sender).Columns[e.Column] == columnHeaderCategoryRate)
				{
					//ListViewItemSorterFloatを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerFloatDesc(e.Column);
				}
				else if (((ListView)sender).Columns[e.Column] == columnHeaderCategoryDate)
				{
					//ListViewItemComparerDateDescを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerDateDesc(e.Column);
				}
				else
				{
					//ListViewItemSorterStrを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerStrDesc(e.Column);
				}
			}

			//グラフ再描画
			pictureBoxCategoryGraph.Invalidate();
		}

		#region ログイン
		/// <summary>
		/// ログインボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLogin_Click(object sender, EventArgs e)
		{
			if (comboBoxActiveUser.SelectedIndex == -1)
			{
				return;
			}

			string strLoginSite = "";
			string strLoginID = "";
			string strLoginPW = "";

			foreach (AccountData item in ada.DataList)
			{
				if (comboBoxActiveUser.Text == item.UserName)
				{
					strLoginSite = item.LoginSite;
					strLoginID = item.ID;
					strLoginPW = item.Pass;
				}
			}

			//SALogin ver1.x
			if ((Settings.Instance.rbSALogin1x == false && Settings.Instance.rbSALogin2x == false)
				|| radioButtonSALoginver1x.Checked)
			{
				string strLoginLeftFormCheck;
				strLoginLeftFormCheck = Settings.Instance.LoginLeftFormCheck.ToString();
				string strPreExe;
				strPreExe = Settings.Instance.PreExe.ToString();
				string strgetLinkGameExecute;
				strgetLinkGameExecute = Settings.Instance.getLinkGameExecute.ToString();
				string strAutoClose;
				strAutoClose = Settings.Instance.AutoClose.ToString();

				string command = strLoginID + "," + strLoginPW + "," + strLoginLeftFormCheck + "," + strPreExe + "," + strgetLinkGameExecute + "," + strAutoClose;

				if (File.Exists(GetAppPath() + @"\SALogin\SALogin.exe") == false)
				{
					MessageBox.Show("SALogin.exeがありません", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					Process.Start(GetAppPath() + @"\SALogin\SALogin.exe", command);
					if (checkBoxLoginMin.Checked) this.WindowState = FormWindowState.Minimized;
				}
			}
			//SALogin ver2.x
			else
			{
				string command = strLoginSite + "," + strLoginID + "," + strLoginPW + ",";

				if (File.Exists(GetAppPath() + @"\SALogin\SALogin2.exe") == false)
				{
					MessageBox.Show("SALogin2.exeがありません", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					Process.Start(GetAppPath() + @"\SALogin\SALogin2.exe", command);
					if (checkBoxLoginMin.Checked) this.WindowState = FormWindowState.Minimized;
				}
			}
		}
		#endregion

		/// <summary>
		/// 実行ファイルのあるディレクトリパス取得
		/// </summary>
		/// <returns></returns>
		public string GetAppPath()
		{
			//return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			return System.Windows.Forms.Application.StartupPath;
		}

		/// <summary>
		/// error.logパス参照ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonLogPathRef_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.InitialDirectory = @"C:\Program Files\RedBanana\SuddenAttack";
			ofd.Filter = "logファイル(*.log;*.txt)|*.log;*.txt";
			ofd.RestoreDirectory = true;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				textBoxerrorlogPath.Text = ofd.FileName;
			}
		}


		#region listViewMain編集

		/// <summary>
		/// メモの編集コンテキストメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolStripMenuItemListViewMemo_Click(object sender, EventArgs e)
		{
			FormListViewMemoEdit f = new FormListViewMemoEdit();
			f.ShowDialog();

			if (f.OK)
			{
				//選択された項目のMemo欄に記入(複数選択可)
				ListView.SelectedListViewItemCollection _ListViewItems = listViewMain.SelectedItems;
				foreach (ListViewItem var in _ListViewItems)
				{

					var.SubItems[columnHeaderMemo.Index].Text = f.textBoxMemoEdit.Text;
					for (int i = 0; i < DataList.Count; i++)
					{
						if (((Data)DataList[i]).Date == var.SubItems[columnHeaderDate.Index].Text)
						{
							if (((Data)DataList[i]).IndexNo == var.Tag.ToString())
							{
								((Data)DataList[i]).Memo = var.SubItems[columnHeaderMemo.Index].Text;
							}
						}
					}

				}
			}

			f.Close();
			f.Dispose();

			//XMLに記録
			SaveDataXML();

			//集計カテゴリページのコンボボックス
			LoadCategoryCombobox();
		}

		#endregion

		#region listViewMain項目削除

		private void ToolStripMenuItemListViewRemove_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("この項目を削除しますか？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{

				//選択された項目の削除(複数選択可)
				ListView.SelectedListViewItemCollection _ListViewItems = listViewMain.SelectedItems;
				foreach (ListViewItem var in _ListViewItems)
				{

					for (int i = 0; i < DataList.Count; i++)
					{
						if (((Data)DataList[i]).Date == var.SubItems[columnHeaderDate.Index].Text)
						{
							if (((Data)DataList[i]).IndexNo == var.Tag.ToString())
							{
								DataList.Remove(DataList[i]);
							}
						}
					}

				}

				//XMLに記録
				SaveDataXML();

				//listview更新
				ListViewItemAdd();

				//pictureBoxGraphを描画しなおす
				pictureBoxGraph.Invalidate();

				//TextboxMainInfo表示
				PrintTextboxMainInfo();
			}
		}

		#endregion

		#region カテゴリ別抽出

		/// <summary>
		/// カテゴリ抽出のtextBox,pictureBox,listViewを初期化する
		/// </summary>
		private void CategoryIni()
		{
			textBoxCategoryResult.Text = "K/D:--% (K:-- D:--) W/L:--% (W:-- L:-- D:--)";
			listViewCategory.Items.Clear();
			pictureBoxCategoryGraph.Invalidate();
		}

		/// <summary>
		/// 集計カテゴリページアクティブ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabPageTotal2_Enter(object sender, EventArgs e)
		{
			//CheckChanged();
		}

		/// <summary>
		/// コンボボックスに使用アイテム追加
		/// </summary>
		private void LoadCategoryCombobox()
		{
			List<string> CategoryTemplist = new List<string>();

			//コンボボックスに期間(開始)追加
			CategoryTemplist.Clear();
			comboBoxCategoryPeriod1.Items.Clear();
			foreach (Data item in DataList)
			{
				string[] str = item.Date.Split(new char[] { ' ' });
				if (!CategoryTemplist.Contains(str[0])) CategoryTemplist.Add(str[0]);
			}
			foreach (var item in CategoryTemplist)
			{
				comboBoxCategoryPeriod1.Items.Add(item.ToString());
			}
			if (comboBoxCategoryPeriod1.Items.Count >= 1)
			{
				comboBoxCategoryPeriod1.SelectedIndex = 0;
			}

			//コンボボックスに期間(終了)追加
			CategoryTemplist.Clear();
			comboBoxCategoryPeriod2.Items.Clear();
			foreach (Data item in DataList)
			{
				string[] str = item.Date.Split(new char[] { ' ' });
				if (!CategoryTemplist.Contains(str[0])) CategoryTemplist.Add(str[0]);
			}
			foreach (var item in CategoryTemplist)
			{
				comboBoxCategoryPeriod2.Items.Add(item.ToString());
			}
			if (comboBoxCategoryPeriod2.Items.Count >= 1)
			{
				comboBoxCategoryPeriod2.SelectedIndex = 0;
			}

			//コンボボックスに使用マップ追加
			CategoryTemplist.Clear();
			comboBoxCategoryMap.Items.Clear();
			foreach (Data item in DataList)
			{
				if (!CategoryTemplist.Contains(item.Map)) CategoryTemplist.Add(item.Map);
			}
			foreach (var item in CategoryTemplist)
			{
				if (item != null) comboBoxCategoryMap.Items.Add(item.ToString());
			}

			//コンボボックスに使用チーム追加
			CategoryTemplist.Clear();
			comboBoxCategoryTeam.Items.Clear();
			foreach (Data item in DataList)
			{
				if (!CategoryTemplist.Contains(item.Team)) CategoryTemplist.Add(item.Team);
			}
			foreach (var item in CategoryTemplist)
			{
				if (item != null) comboBoxCategoryTeam.Items.Add(item.ToString());
			}

			//コンボボックスに使用メイン武器追加
			CategoryTemplist.Clear();
			comboBoxCategoryMainArms.Items.Clear();
			foreach (Data item in DataList)
			{
				if (!CategoryTemplist.Contains(item.Mainarms)) CategoryTemplist.Add(item.Mainarms);
			}
			foreach (var item in CategoryTemplist)
			{
				if (item != null) comboBoxCategoryMainArms.Items.Add(item.ToString());
			}

			//コンボボックスに使用サブ武器追加
			CategoryTemplist.Clear();
			comboBoxCategorySubArms.Items.Clear();
			foreach (Data item in DataList)
			{
				if (!CategoryTemplist.Contains(item.Subarms)) CategoryTemplist.Add(item.Subarms);
			}
			foreach (var item in CategoryTemplist)
			{
				if (item != null) comboBoxCategorySubArms.Items.Add(item.ToString());
			}

			//コンボボックスに使用ナイフ追加
			CategoryTemplist.Clear();
			comboBoxCategoryKnife.Items.Clear();
			foreach (Data item in DataList)
			{
				if (!CategoryTemplist.Contains(item.Knife)) CategoryTemplist.Add(item.Knife);
			}
			foreach (var item in CategoryTemplist)
			{
				if (item != null) comboBoxCategoryKnife.Items.Add(item.ToString());
			}

			//コンボボックスに使用その他追加
			CategoryTemplist.Clear();
			comboBoxCategoryOther.Items.Clear();
			foreach (Data item in DataList)
			{
				if (!CategoryTemplist.Contains(item.Other)) CategoryTemplist.Add(item.Other);
			}
			foreach (var item in CategoryTemplist)
			{
				if (item != null) comboBoxCategoryOther.Items.Add(item.ToString());
			}

			//コンボボックスに使用メモ追加
			CategoryTemplist.Clear();
			comboBoxCategoryMemo.Items.Clear();
			foreach (Data item in DataList)
			{
				if (!CategoryTemplist.Contains(item.Memo)) CategoryTemplist.Add(item.Memo);
			}
			foreach (var item in CategoryTemplist)
			{
				if (item != null && item != "") comboBoxCategoryMemo.Items.Add(item.ToString());
			}

			//コンボボックスに使用タイプ追加
			CategoryTemplist.Clear();
			comboBoxCategoryGameType.Items.Clear();
			foreach (Data item in DataList)
			{
				if (!CategoryTemplist.Contains(item.GameType)) CategoryTemplist.Add(item.GameType);
			}
			foreach (var item in CategoryTemplist)
			{
				if (item != null) comboBoxCategoryGameType.Items.Add(item.ToString());
			}
		}

		/// <summary>
		/// チェックボックスチェック
		/// </summary>
		#region チェックボックスチェック
		private void CheckChanged()
		{
			CategoryIni();

			if (checkBoxCategoryPeriod.Checked)
			{
				comboBoxCategoryPeriod1.Enabled = true;
				comboBoxCategoryPeriod2.Enabled = true;
			}
			else
			{
				comboBoxCategoryPeriod1.Enabled = false;
				comboBoxCategoryPeriod2.Enabled = false;
			}

			if (checkBoxCategoryMap.Checked)
			{
				comboBoxCategoryMap.Enabled = true;
			}
			else
			{
				comboBoxCategoryMap.Enabled = false;
			}

			if (checkBoxCategoryTeam.Checked)
			{
				comboBoxCategoryTeam.Enabled = true;
			}
			else
			{
				comboBoxCategoryTeam.Enabled = false;
			}

			if (checkBoxCategoryMainArms.Checked)
			{
				comboBoxCategoryMainArms.Enabled = true;
			}
			else
			{
				comboBoxCategoryMainArms.Enabled = false;
			}

			if (checkBoxCategorySubArms.Checked)
			{
				comboBoxCategorySubArms.Enabled = true;
			}
			else
			{
				comboBoxCategorySubArms.Enabled = false;
			}

			if (checkBoxCategoryKnife.Checked)
			{
				comboBoxCategoryKnife.Enabled = true;
			}
			else
			{
				comboBoxCategoryKnife.Enabled = false;
			}

			if (checkBoxCategoryOther.Checked)
			{
				comboBoxCategoryOther.Enabled = true;
			}
			else
			{
				comboBoxCategoryOther.Enabled = false;
			}

			if (checkBoxCategoryMemo.Checked)
			{
				comboBoxCategoryMemo.Enabled = true;
			}
			else
			{
				comboBoxCategoryMemo.Enabled = false;
			}

			if (checkBoxCategoryGameType.Checked)
			{
				comboBoxCategoryGameType.Enabled = true;
			}
			else
			{
				comboBoxCategoryGameType.Enabled = false;
			}
		}
		private void checkBoxCategoryPeriod_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		private void checkBoxCategoryMap_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		private void checkBoxCategoryTeam_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		private void checkBoxCategoryMainArms_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		private void checkBoxCategorySubArms_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		private void checkBoxCategoryKnife_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		private void checkBoxCategoryOther_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		private void checkBoxCategoryMemo_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		private void checkBoxCategoryGameType_CheckedChanged(object sender, EventArgs e)
		{
			CheckChanged();
		}
		#endregion

		#region コンボボックスチェンジ
		private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
		{
			CategoryIni();
		}
		#endregion

		/// <summary>
		/// 抽出するボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonCategoryTotal_Click(object sender, EventArgs e)
		{
			CategoryIni();

			int CategoryTotalKill = 0;
			int CategoryTotalDeath = 0;
			int CategoryTotalWin = 0;
			int CategoryTotalLose = 0;
			int CategoryTotalDraw = 0;

			List<string> PeriodList = new List<string>();
			if (checkBoxCategoryPeriod.Checked
				&& comboBoxCategoryPeriod1.SelectedIndex != -1
				&& comboBoxCategoryPeriod2.SelectedIndex != -1)
			{
				DateTime d1 = DateTime.Parse(comboBoxCategoryPeriod1.Text);
				DateTime d2 = DateTime.Parse(comboBoxCategoryPeriod2.Text);
				for (DateTime i = d1; i <= d2; i = i.AddDays(1))
				{
					PeriodList.Add(i.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo));
				}
			}

			//ここからキーワードから絞っていく
			List<Data> FindDataList = new List<Data>();
			foreach (Data item in DataList)
			{
				FindDataList.Add(item);
			}

			List<Data> FindDataPeriod = new List<Data>();
			if (checkBoxCategoryPeriod.Checked
				&& comboBoxCategoryPeriod1.SelectedIndex != -1
				&& comboBoxCategoryPeriod2.SelectedIndex != -1)
			{
				foreach (var item in PeriodList)
				{
					FindDataPeriod.AddRange(FindDataList.FindAll(xi => xi.Date.Substring(0, 10).Replace("/", "") == item));
				}
			}
			else
			{
				FindDataPeriod = FindDataList;
			}

			List<Data> FindDataMap = new List<Data>();
			if (checkBoxCategoryMap.Checked)
			{
				FindDataMap = FindDataPeriod.FindAll(item => item.Map == comboBoxCategoryMap.Text);
			}
			else
			{
				FindDataMap = FindDataPeriod;
			}

			List<Data> FindDataTeam = new List<Data>();
			if (checkBoxCategoryTeam.Checked)
			{
				FindDataTeam = FindDataMap.FindAll(item => item.Team == comboBoxCategoryTeam.Text);
			}
			else
			{
				FindDataTeam = FindDataMap;
			}

			List<Data> FindDataMainArms = new List<Data>();
			if (checkBoxCategoryMainArms.Checked)
			{
				FindDataMainArms = FindDataTeam.FindAll(item => item.Mainarms == comboBoxCategoryMainArms.Text);
			}
			else
			{
				FindDataMainArms = FindDataTeam;
			}

			List<Data> FindDataSubArms = new List<Data>();
			if (checkBoxCategorySubArms.Checked)
			{
				FindDataSubArms = FindDataMainArms.FindAll(item => item.Subarms == comboBoxCategorySubArms.Text);
			}
			else
			{
				FindDataSubArms = FindDataMainArms;
			}

			List<Data> FindDataKnife = new List<Data>();
			if (checkBoxCategoryKnife.Checked)
			{
				FindDataKnife = FindDataSubArms.FindAll(item => item.Knife == comboBoxCategoryKnife.Text);
			}
			else
			{
				FindDataKnife = FindDataSubArms;
			}

			List<Data> FindDataOther = new List<Data>();
			if (checkBoxCategoryOther.Checked)
			{
				FindDataOther = FindDataKnife.FindAll(item => item.Other == comboBoxCategoryOther.Text);
			}
			else
			{
				FindDataOther = FindDataKnife;
			}

			List<Data> FindDataMemo = new List<Data>();
			if (checkBoxCategoryMemo.Checked)
			{
				FindDataMemo = FindDataOther.FindAll(item => item.Memo == comboBoxCategoryMemo.Text);
			}
			else
			{
				FindDataMemo = FindDataOther;
			}

			List<Data> FindDataGameType = new List<Data>();
			if (checkBoxCategoryGameType.Checked)
			{
				FindDataGameType = FindDataMemo.FindAll(item => item.GameType == comboBoxCategoryGameType.Text);
			}
			else
			{
				FindDataGameType = FindDataMemo;
			}

			foreach (var item in FindDataGameType)
			{
				CategoryTotalKill += int.Parse(item.Kill);
				CategoryTotalDeath += int.Parse(item.Death);
				if (item.WinLose == "勝ち")
				{
					CategoryTotalWin++;
				}
				else if (item.WinLose == "負け")
				{
					CategoryTotalLose++;
				}
				else if (item.WinLose == "引き分け")
				{
					CategoryTotalDraw++;
				}
				else
				{
					//CategoryTotalLose++;
				}
			}

			string str1 = "";
			string str2 = "";
			if (!(checkBoxCategoryPeriod.Checked == false && checkBoxCategoryMap.Checked == false && checkBoxCategoryTeam.Checked == false && checkBoxCategoryMainArms.Checked == false && checkBoxCategorySubArms.Checked == false && checkBoxCategoryKnife.Checked == false && checkBoxCategoryOther.Checked == false && checkBoxCategoryMemo.Checked == false && checkBoxCategoryGameType.Checked == false))
			{
				str1 = "K/D: " + CalTotalKDper(CategoryTotalKill.ToString(), CategoryTotalDeath.ToString()) + "%(K: " + CategoryTotalKill + " D: " + CategoryTotalDeath + ")";
				str2 = "W/L: " + CalTotalWLper(CategoryTotalWin.ToString(), CategoryTotalLose.ToString(), CategoryTotalDraw.ToString()) + "%(W: " + CategoryTotalWin + " L: " + CategoryTotalLose + " D: " + CategoryTotalDraw + ")";

				ListViewCategoryItemAdd(FindDataGameType.ToArray());
			}
			//チェックボックスに一つもチェックがなかった場合は全抽出
			else
			{
				CategoryTotalKill = 0;
				CategoryTotalDeath = 0;
				CategoryTotalWin = 0;
				CategoryTotalLose = 0;
				CategoryTotalDraw = 0;

				foreach (Data item in DataList)
				{
					CategoryTotalKill += int.Parse(item.Kill);
					CategoryTotalDeath += int.Parse(item.Death);
					if (item.WinLose == "勝ち")
					{
						CategoryTotalWin++;
					}
					else if (item.WinLose == "負け")
					{
						CategoryTotalLose++;
					}
					else if (item.WinLose == "引き分け")
					{
						CategoryTotalDraw++;
					}
					else
					{
						//CategoryTotalLose++;
					}
				}

				str1 = "K/D: " + CalTotalKDper(CategoryTotalKill.ToString(), CategoryTotalDeath.ToString()) + "%(K: " + CategoryTotalKill + " D: " + CategoryTotalDeath + ")";
				str2 = "W/L: " + CalTotalWLper(CategoryTotalWin.ToString(), CategoryTotalLose.ToString(), CategoryTotalDraw.ToString()) + "%(W: " + CategoryTotalWin + " L: " + CategoryTotalLose + " D: " + CategoryTotalDraw + ")";

				ListViewCategoryItemAdd(null);
			}

			textBoxCategoryResult.Text = "";
			textBoxCategoryResult.Text += str1 + " " + str2;

			pictureBoxCategoryGraph.Invalidate();
		}

		/// <summary>
		/// 抽出グラフ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureBoxCategory_Paint(object sender, PaintEventArgs e)
		{
			//Y目盛りを描画
			for (int i = 0; i < 10; i++)
			{
				//50ラインだけ濃く
				if (i == 5)
				{
					e.Graphics.DrawLine(new Pen(Color.FromArgb(135, 120, 100)), 0, (pictureBoxCategoryGraph.Height / 10) * i - 1, pictureBoxCategoryGraph.Width, (pictureBoxCategoryGraph.Height / 10) * i - 1);
				}
				else
				{
					e.Graphics.DrawLine(Pens.DarkGray, 0, (pictureBoxCategoryGraph.Height / 10) * i - 1, pictureBoxCategoryGraph.Width, (pictureBoxCategoryGraph.Height / 10) * i - 1);
				}
			}

			//ListViewに１行以下しか表示されていない場合はキープのみ描画してreturn
			if (listViewCategory.Items.Count <= 1)
			{
				if (keywordkeep) DrawKeepGraph(e);
				return;
			}

			int count = listViewCategory.Items.Count;

			//行数分Xpointを決める
			int[] Xpoint = new int[count];
			for (int i = 0; i < count; i++)
			{
				if (i == 0)
				{
					//一番左はX座標は常に0
					Xpoint[i] = 0;
				}
				else if (i == (count - 1))
				{
					//一番右はX座標は常にpictureBoxCategoryGraphの最後
					Xpoint[i] = pictureBoxCategoryGraph.Width;
				}
				else
				{
					Xpoint[i] = (int)(((float)pictureBoxCategoryGraph.Width / ((float)count - 1.0F)) * (float)i);
				}
			}

			if (Settings.Instance.GraphXLineClear == false)
			{
				//Xpointに縦線を引く
				for (int i = 1; i < (count - 1); i++)
				{
					e.Graphics.DrawLine(Pens.DarkGray, Xpoint[i], 0, Xpoint[i], pictureBoxCategoryGraph.Height);
				}
			}

			//KD率を格納する
			float[] KDrate = new float[count];

			for (int i = 0; i < count; i++)
			{
				KDrate[i] = float.Parse(listViewCategory.Items[i].SubItems[columnHeaderCategoryRate.Index].Text) / 100;
			}

			//それぞれのYpointを決める
			int[] Ypoint = new int[count];

			for (int i = 0; i < count; i++)
			{
				Ypoint[i] = (int)(((float)pictureBoxCategoryGraph.Height) - ((float)pictureBoxCategoryGraph.Height * KDrate[i]));
			}

			//グラフ本線を描画する
			Point[] p = new Point[count];
			for (int i = 0; i < count; i++)
			{
				p[i] = new Point(Xpoint[i], Ypoint[i]);
			}

			Pen cr = new Pen(Color.FromArgb(Settings.Instance.CGColor_R, Settings.Instance.CGColor_G, Settings.Instance.CGColor_B));

			e.Graphics.DrawLines(cr, p);

			cr.Dispose();

			//キープグラフ描画
			if (keywordkeep)
			{
				DrawKeepGraph(e);
			}
		}

		/// <summary>
		/// キーワード抽出グラフキープ
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private List<int> XpointKeep;
		private List<int> YpointKeep;
		private bool keywordkeep = false;
		private void buttonKeywordKeep_Click(object sender, EventArgs e)
		{
			XpointKeep = new List<int>();
			YpointKeep = new List<int>();

			//ListViewに１行以下しか表示されていない場合はreturn
			if (listViewCategory.Items.Count <= 1)
			{
				return;
			}

			//キープフラグ
			keywordkeep = true;

			int count = listViewCategory.Items.Count;

			//行数分Xpointを決める
			for (int i = 0; i < count; i++)
			{
				if (i == 0)
				{
					//一番左はX座標は常に0
					XpointKeep.Insert(0, 0);
				}
				else if (i == (count - 1))
				{
					//一番右はX座標は常にpictureBoxCategoryGraphの最後
					XpointKeep.Add(pictureBoxCategoryGraph.Width);
				}
				else
				{
					XpointKeep.Add((int)(((float)pictureBoxCategoryGraph.Width / ((float)count - 1.0F)) * (float)i));
				}
			}

			//KD率を格納する
			float[] KDrate = new float[count];

			for (int i = 0; i < count; i++)
			{
				KDrate[i] = float.Parse(listViewCategory.Items[i].SubItems[columnHeaderCategoryRate.Index].Text) / 100;
			}

			//それぞれのYpointを決める
			for (int i = 0; i < count; i++)
			{
				YpointKeep.Add((int)(((float)pictureBoxCategoryGraph.Height) - ((float)pictureBoxCategoryGraph.Height * KDrate[i])));
			}

			pictureBoxCategoryGraph.Invalidate();
		}
		/// <summary>
		/// キーワード抽出グラフキープ解除
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonKeywordKeepRelease_Click(object sender, EventArgs e)
		{
			keywordkeep = false;

			XpointKeep.Clear();
			YpointKeep.Clear();

			pictureBoxCategoryGraph.Invalidate();
		}

		/// <summary>
		/// キープグラフ描画
		/// </summary>
		private void DrawKeepGraph(PaintEventArgs e)
		{
			if (keywordkeep)
			{
				Point[] pk = new Point[XpointKeep.Count];
				for (int i = 0; i < XpointKeep.Count; i++)
				{
					pk[i] = new Point(XpointKeep[i], YpointKeep[i]);
				}
				Pen crk = new Pen(Color.FromArgb((Settings.Instance.CGColor_R + 100) % 255, (Settings.Instance.CGColor_G + 100) % 255, (Settings.Instance.CGColor_B + 100) % 255));
				if (XpointKeep.Count > 0) e.Graphics.DrawLines(crk, pk);
				crk.Dispose();
			}
		}

		/// <summary>
		/// 抽出リストビューにアイテム追加
		/// </summary>
		/// <param name="list">Dataリスト</param>
		private void ListViewCategoryItemAdd(Data[] list)
		{
			listViewCategory.BeginUpdate();

			listViewCategory.Items.Clear();

			List<Data> AddList = new List<Data>();
			if (list == null)
			{
				foreach (Data item in DataList)
				{
					AddList.Add(item);
				}
			}
			else
			{
				foreach (var item in list)
				{
					AddList.Add(item);
				}
			}

			foreach (var item in AddList)
			{
				//KD%計算(Kill:0 Death:0の場合はKD%:0)
				float kdper = 0;
				if ((float.Parse(item.Kill) + float.Parse(item.Death)) != 0)
				{
					kdper = float.Parse(item.Kill) / (float.Parse(item.Kill) + float.Parse(item.Death)) * 100;
				}

				ListViewItem itemx = new ListViewItem();

				itemx.Text = item.Date;
				itemx.SubItems.Add(item.Kill);
				itemx.SubItems.Add(item.Death);
				itemx.SubItems.Add(kdper.ToString());
				itemx.SubItems.Add(item.Map);
				itemx.SubItems.Add(item.Team);
				itemx.SubItems.Add(item.WinLose);
				itemx.SubItems.Add(item.Mainarms);
				itemx.SubItems.Add(item.Subarms);
				itemx.SubItems.Add(item.Knife);
				itemx.SubItems.Add(item.Other);
				itemx.SubItems.Add(item.Memo);
				itemx.Tag = (item.IndexNo);

				listViewCategory.Items.Add(itemx);
			}

			listViewCategory.EndUpdate();
		}
		#endregion

		#region ランキング

		/// <summary>
		/// ランキングを記録するボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonRankingRec_Click(object sender, EventArgs e)
		{
			Rd.Clear();

			if (comboBoxActiveUser.Items.Count == 0)
			{
				MessageBox.Show("ユーザ登録をしてください");
				return;
			}
			if (comboBoxActiveUser.SelectedIndex == -1)
			{
				MessageBox.Show("アクティブユーザを選択してください");
				return;
			}

			//取得が終わるか中止されるまで押せないようにする
			buttonRankingRec.Enabled = false;

			//フォーカス
			buttonRankingStop.Focus();

			GetRanking();
		}

		/// <summary>
		/// ランキング中止ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonRankingStop_Click(object sender, EventArgs e)
		{
			WbRanking.Stop();
			_WBC = WbRankingComp.End;

			buttonRankingRec.Enabled = true;

			toolStripProgressBarNet.Value = 0;
		}

		private WebBrowser WbRanking;
		private enum WbRankingComp
		{
			Date,
			Total,
			Kill,
			HS,
			Clan,
			End,
		}
		private WbRankingComp _WBC = WbRankingComp.End;

		private string RankName;
		private string ClanName;
		private ArrayList RankList = new ArrayList();
		private RankData Rd = new RankData();

		/// <summary>
		/// RankData.xmlから読み込む
		/// </summary>
		/// <returns></returns>
		private bool LoadRankDataXml()
		{
			string id = "";
			foreach (AccountData item in ada.DataList)
			{
				if (comboBoxActiveUser.Text == item.UserName)
				{
					id = item.ID;
				}
			}

			string datapath = GetAppPath() + "\\data\\" + id + "\\RankData.xml";

			RankList.Clear();

			if (File.Exists(datapath) == true)
			{
				Type[] et = new Type[] { typeof(RankData) };

				System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList), et);

				try
				{
					using (System.IO.FileStream fs = new System.IO.FileStream(datapath, System.IO.FileMode.Open))
					{
						RankList = (ArrayList)serializer.Deserialize(fs);
					}
				}
				catch (System.Exception e)
				{
					MessageBox.Show(e.ToString(), "RankData.xml読み込み失敗");
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// ブラウザ初期化
		/// </summary>
		private void WbRankIni()
		{
			WbRanking = new WebBrowser();
			WbRanking.Visible = false;
			WbRanking.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WbRanking_DocumentCompleted);
			this.Controls.Add(WbRanking);
		}

		/// <summary>
		/// ランキング取得
		/// </summary>
		private void GetRanking()
		{
			string cn = "";
			foreach (AccountData item in ada.DataList)
			{
				if (item.UserName == comboBoxActiveUser.Text)
				{
					cn = item.ClanName;
				}
			}

			//エンコード(エスケープはしない)
			RankName = HttpUtility.UrlEncode(comboBoxActiveUser.Text);//System.Web.dll
			ClanName = HttpUtility.UrlEncode(cn);//System.Web.dll

			_WBC = WbRankingComp.Date;

			//スクリプトエラーを表示しないようにする
			WbRanking.ScriptErrorsSuppressed = true;

			//まずDate
			//	WbRanking.Navigate("http://suddenattack.redbanana.jp/Ranking/Personal/__inc_Ranking_Main_Ajax.asp");
			//WbRanking.Navigate("http://suddenattack.gameyarou.jp/Ranking/Personal/__inc_Ranking_Main_Ajax.asp");
			WbRanking.Navigate("http://sa.nexon.co.jp/ranking/person.aspx");

			toolStripProgressBarNet.PerformStep();
		}

		/// <summary>
		/// ランキングブラウザ読み込み完了
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void WbRanking_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			switch (_WBC)
			{
				case WbRankingComp.Date:
					Regex regdate = new Regex(".*最新ランキングアップデート : <span class=\"n_data_01\">(?<date>.*?)</span>.*");

					for (Match m = regdate.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.Date = m.Groups["date"].Value.Replace(".", "/");
					}

					//同じ日付がないか検証
					foreach (RankData item in RankList)
					{
						int cmp = Rd.Date.CompareTo(item.Date);
						if (cmp == 0)
						{
							toolStripProgressBarNet.Value = 0;

							//ボタン許可
							buttonRankingRec.Enabled = true;

							MessageBox.Show("ランキングの更新はありません");

							_WBC = WbRankingComp.End;
							return;
						}
					}

					_WBC = WbRankingComp.Total;

					//2番目にTotal
					//	WbRanking.Navigate("http://suddenattack.redbanana.jp/Ranking/Personal/__inc_Ranking_Search_List_Ajax.asp?search_word=" + RankName);
					WbRanking.Navigate("http://suddenattack.gameyarou.jp/Ranking/Personal/__inc_Ranking_Search_List_Ajax.asp?search_word=" + RankName);

					break;
				case WbRankingComp.Total:
					Regex regrank = new Regex(".*>(?<rank>.*?)位.*");

					for (Match m = regrank.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.TotalRank = m.Groups["rank"].Value;
					}

					_WBC = WbRankingComp.Kill;

					//3番目にKill
					//	WbRanking.Navigate("http://suddenattack.redbanana.jp/Ranking/Personal/__inc_Ranking_Search_List_Ajax.asp?search_word=" + RankName + "&rank_cd=K");
					WbRanking.Navigate("http://suddenattack.gameyarou.jp/Ranking/Personal/__inc_Ranking_Search_List_Ajax.asp?search_word=" + RankName + "&rank_cd=K");

					toolStripProgressBarNet.PerformStep();
					break;
				case WbRankingComp.Kill:
					Regex regkillrank = new Regex(".*>(?<killrank>.*?)位.*");

					for (Match m = regkillrank.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.KillRank = m.Groups["killrank"].Value;
					}

					Regex regkill = new Regex(".*right\" class=\"n_data_02\">(?<kill>.*?)<.*");

					for (Match m = regkill.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.Kill = m.Groups["kill"].Value.Replace(",", "");
					}

					Regex regdeath = new Regex(".*left\" class=\"n_data_02\">(?<death>.*?)<.*");

					for (Match m = regdeath.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.Death = m.Groups["death"].Value.Replace(",", "");
					}

					_WBC = WbRankingComp.HS;

					//4番目にHS
					//	WbRanking.Navigate("http://suddenattack.redbanana.jp/Ranking/Personal/__inc_Ranking_Search_List_Ajax.asp?search_word=" + RankName + "&rank_cd=H");
					WbRanking.Navigate("http://suddenattack.gameyarou.jp/Ranking/Personal/__inc_Ranking_Search_List_Ajax.asp?search_word=" + RankName + "&rank_cd=H");

					toolStripProgressBarNet.PerformStep();

					break;
				case WbRankingComp.HS:
					Regex reghsrank = new Regex(".*>(?<hsrank>.*?)位.*");

					for (Match m = reghsrank.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.HSRank = m.Groups["hsrank"].Value;
					}

					Regex reghs = new Regex(".*right\" class=\"n_data_02\">(?<hs>.*?)<.*");

					for (Match m = reghs.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.HS = m.Groups["hs"].Value.Replace(",", "");
					}

					//5番目にクラン
					string cn = "";
					foreach (AccountData item in ada.DataList)
					{
						if (item.UserName == comboBoxActiveUser.Text)
						{
							cn = item.ClanName;
						}
					}
					if (cn != null && cn != "")
					{
						Rd.ClanName = "";
						Rd.ClanRank = "0";
						Rd.ClanWin = "0";
						Rd.ClanLose = "0";
						Rd.ClanDraw = "0";

						_WBC = WbRankingComp.Clan;

						//	WbRanking.Navigate("http://suddenattack.redbanana.jp/Ranking/Clan/__inc_Ranking_Search_List_Ajax.asp?search_word=" + ClanName);
						WbRanking.Navigate("http://suddenattack.gameyarou.jp/Ranking/Clan/__inc_Ranking_Search_List_Ajax.asp?search_word=" + ClanName);

						toolStripProgressBarNet.PerformStep();
					}
					else
					{
						Rd.ClanName = "";
						Rd.ClanRank = "0";
						Rd.ClanWin = "0";
						Rd.ClanLose = "0";
						Rd.ClanDraw = "0";

						toolStripProgressBarNet.PerformStep();

						WbRankingAllCompleted();
					}

					break;
				case WbRankingComp.Clan:
					Regex regclanname = new Regex(".*style=\"cursor:pointer;\">(?<clanname>.*?)</span>.*");
					Regex regclanrank = new Regex(".*n_data_01\">(?<clanrank>.*?)位.*");
					Regex regwin = new Regex(".*戦(?<win>.*?)勝.*");
					Regex reglose = new Regex(".*勝(?<lose>.*?)敗.*");
					Regex regdraw = new Regex(".*敗(?<draw>.*?)分.*");

					for (Match m = regclanname.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.ClanName = m.Groups["clanname"].Value;
					}

					for (Match m = regclanrank.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.ClanRank = m.Groups["clanrank"].Value;
					}

					for (Match m = regwin.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.ClanWin = m.Groups["win"].Value.Replace(",", "");
					}

					for (Match m = reglose.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.ClanLose = m.Groups["lose"].Value.Replace(",", "");
					}

					for (Match m = regdraw.Match(WbRanking.DocumentText); m.Success; m = m.NextMatch())
					{
						Rd.ClanDraw = m.Groups["draw"].Value.Replace(",", "");
					}

					toolStripProgressBarNet.PerformStep();

					WbRankingAllCompleted();

					break;
				default:

					break;
			}
		}

		/// <summary>
		/// ブラウザの読み込み全て完了
		/// </summary>
		private void WbRankingAllCompleted()
		{
			_WBC = WbRankingComp.End;

			RankList.Add(Rd);

			string id = "";
			foreach (AccountData item in ada.DataList)
			{
				if (item.UserName == comboBoxActiveUser.Text)
				{
					id = item.ID;
				}
			}
			string datapath = GetAppPath() + "\\data\\" + id + "\\RankData.xml";

			//RankData.xmlに保存
			Type[] et = new Type[] { typeof(RankData) };

			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList), et);
			try
			{
				using (System.IO.FileStream fs = new System.IO.FileStream(datapath, System.IO.FileMode.Create))
				{
					serializer.Serialize(fs, RankList);
				}
			}
			catch (System.Exception e)
			{
				MessageBox.Show(e.ToString(), "RankData.xml保存失敗");
				return;
			}

			PrintTextboxMainInfo();

			toolStripProgressBarNet.Value = 0;

			//ListViewRanking表示
			RankingListViewItemAdd();

			//pictureBoxGraphを描画しなおす
			pictureBoxGraphRanking.Invalidate();

			//ボタン許可
			buttonRankingRec.Enabled = true;

			System.Media.SystemSounds.Asterisk.Play();
			//MessageBox.Show("ランキングを記録しました");
		}

		/// <summary>
		/// Rankingリストビューにアイテムを追加
		/// </summary>
		private void RankingListViewItemAdd()
		{
			listViewRanking.Items.Clear();

			//listViewRankingに描画抑制
			listViewRanking.BeginUpdate();

			foreach (RankData item in RankList)
			{
				//HS%計算(kill:0の場合はHS%:0)
				float hsrate = 0.0f;
				try
				{
					if (item.Kill != null && (float.Parse(item.Kill) != 0))
					{
						hsrate = float.Parse(item.HS) / float.Parse(item.Kill) * 100;
					}
				}
				catch (ArgumentNullException)
				{
					hsrate = -1;

					item.Kill = "0";
					item.HS = "0";
					item.Kill = "0";
				}
				catch (FormatException)
				{
					hsrate = -2;

					item.Kill = "0";
					item.HS = "0";
					item.Kill = "0";
				}
				catch (Exception)
				{
					hsrate = -3;

					item.Kill = "0";
					item.HS = "0";
					item.Kill = "0";
				}

				//クラン勝率計算
				float cwrate = 0.0f;
				try
				{
					if ((item.ClanWin != null && item.ClanLose != null && item.ClanDraw != null)
						&& (float.Parse(item.ClanWin) + float.Parse(item.ClanLose) + float.Parse(item.ClanDraw) != 0))
					{
						cwrate = float.Parse(item.ClanWin) / (float.Parse(item.ClanWin) + float.Parse(item.ClanLose) + float.Parse(item.ClanDraw)) * 100;
						cwrate = (float)ToHalfAdjust(cwrate, 2);
					}
					else
					{
						cwrate = 0.0f;
					}
				}
				catch (ArgumentNullException)
				{
					cwrate = -1;

					item.ClanWin = "0";
					item.ClanLose = "0";
					item.ClanDraw = "0";
				}
				catch (FormatException)
				{
					hsrate = -2;

					item.ClanWin = "0";
					item.ClanLose = "0";
					item.ClanDraw = "0";
				}
				catch (Exception)
				{
					hsrate = -3;

					item.ClanWin = "0";
					item.ClanLose = "0";
					item.ClanDraw = "0";
				}

				//表示文字列
				string totalstr = item.TotalRank + "位";
				string killstr = item.KillRank + "位";
				string hsstr = item.HSRank + "位";
				string clanstr = "";
				string clanrecstr = "";
				if (item.ClanName == "" || item.ClanName == null)
				{
					clanstr = "---位";
					clanrecstr = "---";
				}
				else
				{
					clanstr = item.ClanRank + "位";
					clanrecstr = item.ClanName + "  Win:" + item.ClanWin + " Lose:" + item.ClanLose + " Draw:" + item.ClanDraw + " (" + cwrate.ToString() + "%)";
				}

				//Rankingリストビューにアイテム追加
				ListViewItem itemx = new ListViewItem();

				itemx.Text = item.Date;
				itemx.SubItems.Add(totalstr);
				itemx.SubItems.Add(killstr);
				itemx.SubItems.Add(hsstr);
				itemx.SubItems.Add(item.HS);
				itemx.SubItems.Add(hsrate.ToString());
				itemx.SubItems.Add(clanstr);
				itemx.SubItems.Add(clanrecstr);

				listViewRanking.Items.Add(itemx);
			}

			//listViewRankingに描画
			listViewRanking.EndUpdate();
		}

		/// <summary>
		/// ランキンググラフ描画イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pictureBoxGraphRanking_Paint(object sender, PaintEventArgs e)
		{
			List<RankData> grRnklst = new List<RankData>();
			foreach (RankData item in RankList)
			{
				grRnklst.Add(item);
			}

			//降順の場合
			if (!blRankorder_asc)
			{
				grRnklst.Reverse();
			}

			//Y目盛りを描画
			for (int i = 0; i < 10; i++)
			{
				e.Graphics.DrawLine(Pens.DarkGray, 0, (pictureBoxGraphRanking.Height / 10) * i - 1, pictureBoxGraphRanking.Width, (pictureBoxGraphRanking.Height / 10) * i - 1);
			}

			//ListViewRankingに１行以下しか表示されていない場合はreturn
			if (listViewRanking.Items.Count <= 1)
			{
				return;
			}

			int count = 0;
			//グラフにするデータを決める
			if (!radioButtonRankClan.Checked)
			{
				count = listViewRanking.Items.Count;
			}
			//クラングラフのみ
			else
			{
				foreach (RankData item in grRnklst)
				{
					if (item.ClanRank != "0" && item.ClanRank != null)
					{
						count++;
					}
				}
			}

			//行数分Xpointを決める
			int[] Xpoint = new int[count];

			for (int i = 0; i < count; i++)
			{
				if (i == 0)
				{
					//一番左はX座標は常に0
					Xpoint[i] = 0;
				}
				else if (i == (count - 1))
				{
					//一番右はX座標は常にpictureBoxGraphRankingの最後
					Xpoint[i] = pictureBoxGraphRanking.Width;
				}
				else
				{
					Xpoint[i] = (int)(((float)pictureBoxGraphRanking.Width / ((float)count - 1.0F)) * (float)i);
				}
			}

			if (Settings.Instance.GraphXLineClear == false)
			{
				//Xpointに縦線を引く
				for (int i = 1; i < (count - 1); i++)
				{
					e.Graphics.DrawLine(Pens.DarkGray, Xpoint[i], 0, Xpoint[i], pictureBoxGraphRanking.Height);
				}
			}

			//min,max
			int min = -1;
			int max = -1;

			//リストのポイント
			List<int> listpoint = new List<int>();

			//総合ランキング
			if (radioButtonRankTotal.Checked)
			{
				List<int> list = new List<int>();

				if (grRnklst.Count >= 2)
				{
					foreach (RankData item in grRnklst)
					{
						try
						{
							list.Add(int.Parse(item.TotalRank));
						}
						catch (ArgumentNullException)
						{
							list.Add(-1);
						}
					}

					list.Sort((x, y) => x - y);

					max = list[0];

					min = list[list.Count - 1];

					//1dotあたりの値
					double Ipd = ((double)min - (double)max) / (double)pictureBoxGraph.Height;

					foreach (RankData item in grRnklst)
					{
						int ai;

						try
						{
							ai = (int)((double.Parse(item.TotalRank) - ((double)max)) / Ipd);
						}
						catch (ArgumentNullException)
						{
							ai = -1;
						}

						listpoint.Add(ai);
					}
				}
			}
			//killランキング
			if (radioButtonRankKill.Checked)
			{
				List<int> list = new List<int>();

				if (grRnklst.Count >= 2)
				{
					foreach (RankData item in grRnklst)
					{
						try
						{
							list.Add(int.Parse(item.KillRank));
						}
						catch (ArgumentNullException)
						{
							list.Add(-1);
						}
					}

					list.Sort((x, y) => x - y);

					max = list[0];

					min = list[list.Count - 1];

					//1dotあたりの値
					double Ipd = ((double)min - (double)max) / (double)pictureBoxGraph.Height;

					foreach (RankData item in grRnklst)
					{
						int ai;

						try
						{
							ai = (int)((double.Parse(item.KillRank) - ((double)max)) / Ipd);

						}
						catch (ArgumentNullException)
						{
							ai = -1;
						}

						listpoint.Add(ai);
					}
				}
			}
			//HSランキング
			if (radioButtonRankHSRank.Checked)
			{
				List<int> list = new List<int>();

				if (grRnklst.Count >= 2)
				{
					foreach (RankData item in grRnklst)
					{
						try
						{
							list.Add(int.Parse(item.HSRank));
						}
						catch (ArgumentNullException)
						{
							list.Add(-1);
						}
					}

					list.Sort((x, y) => x - y);

					max = list[0];

					min = list[list.Count - 1];

					//1dotあたりの値
					double Ipd = ((double)min - (double)max) / (double)pictureBoxGraph.Height;

					foreach (RankData item in grRnklst)
					{
						int ai;

						try
						{
							ai = (int)((double.Parse(item.HSRank) - ((double)max)) / Ipd);
						}
						catch (ArgumentNullException)
						{
							ai = -1;
						}

						listpoint.Add(ai);
					}
				}
			}
			//HS数
			if (radioButtonRankHS.Checked)
			{
				List<int> list = new List<int>();

				if (grRnklst.Count >= 2)
				{
					foreach (RankData item in grRnklst)
					{
						try
						{
							list.Add(int.Parse(item.HS));
						}
						catch (ArgumentNullException)
						{
							list.Add(-1);
						}
					}

					list.Sort((x, y) => x - y);

					min = list[0];

					max = list[list.Count - 1];

					//1dotあたりの値
					double Ipd = ((double)max - (double)min) / (double)pictureBoxGraph.Height;

					foreach (RankData item in grRnklst)
					{
						int ai;

						try
						{
							ai = pictureBoxGraph.Height - (int)((double.Parse(item.HS) - ((double)min)) / Ipd);
						}
						catch (ArgumentNullException)
						{
							ai = -1;
						}

						listpoint.Add(ai);
					}
				}
			}
			//HS%
			if (radioButtonRankHSRate.Checked)
			{
				List<double> list = new List<double>();
				List<double> tmplist = new List<double>();

				if (grRnklst.Count >= 2)
				{
					foreach (RankData item in grRnklst)
					{
						//HS%計算(kill:0の場合はHS%:0)
						double hsrate = 0.0f;
						try
						{
							if (double.Parse(item.Kill) != 0)
							{
								hsrate = double.Parse(item.HS) / double.Parse(item.Kill);
							}
						}
						catch (ArgumentNullException)
						{
							hsrate = -1;
						}

						list.Add(hsrate);
						tmplist.Add(hsrate);
					}

					list.Sort();

					min = (int)(list[0] * 100);

					max = (int)(list[list.Count - 1] * 100);

					double tmpmin = list[0];
					double tmpmax = list[list.Count - 1];
					//1dotあたりの値
					double Ipd = (tmpmax - tmpmin) / (double)pictureBoxGraph.Height;

					foreach (var item in tmplist)
					{
						int tmp = (int)((item - tmpmin) / Ipd);
						int ai = pictureBoxGraph.Height - tmp;
						listpoint.Add(ai);
					}
				}
			}
			//クランランキング
			if (radioButtonRankClan.Checked)
			{
				List<int> list = new List<int>();
				List<int> listcpy = new List<int>();

				bool tmpbl = false;
				foreach (RankData item in grRnklst)
				{
					if (item.ClanRank != "0" && item.ClanRank != null)
					{
						tmpbl = true;
					}
				}

				if (tmpbl)
				{
					foreach (RankData item in grRnklst)
					{
						try
						{
							if (item.ClanRank != "0" && item.ClanRank != null)
							{
								list.Add(int.Parse(item.ClanRank));
							}
						}
						catch (ArgumentNullException)
						{
							list.Add(-1);
						}
					}

					foreach (var item in list)
					{
						listcpy.Add(item);
					}

					list.Sort((x, y) => x - y);

					max = list[0];

					min = list[list.Count - 1];
				}

				//1dotあたりの値
				double Ipd = ((double)min - (double)max) / (double)pictureBoxGraph.Height;

				foreach (int item in listcpy)
				{
					int ai;

					try
					{
						ai = (int)(((double)item - ((double)max)) / Ipd);
					}
					catch (ArgumentNullException)
					{
						ai = -1;
					}

					listpoint.Add(ai);
				}
			}
			//ラジオボタンが押されていない場合
			if ((radioButtonRankTotal.Checked == false) && (radioButtonRankKill.Checked == false) && (radioButtonRankHSRank.Checked == false)
				&& (radioButtonRankHS.Checked == false) && (radioButtonRankHSRate.Checked == false) && (radioButtonRankClan.Checked == false))
			{
				return;
			}

			//min,maxラベル
			if (min != -1) labelRankGraphMin.Text = labelRankGraphMin2.Text = min.ToString();
			if (max != -1) labelRankGraphMax.Text = labelRankGraphMax2.Text = max.ToString();

			//マイナスチェック
			bool mcheck = false;
			foreach (var item in listpoint)
			{
				if (item > 0) mcheck = true;
			}
			if (mcheck == false) return;

			//グラフ本線を描画する
			Point[] p = new Point[count];
			for (int i = 0; i < count; i++)
			{
				p[i] = new Point(Xpoint[i], listpoint[i]);
			}

			Pen cr = new Pen(Color.FromArgb(Settings.Instance.RGColor_R, Settings.Instance.RGColor_G, Settings.Instance.RGColor_B));

			e.Graphics.DrawLines(cr, p);

			cr.Dispose();
		}

		private bool blRankorder_asc = true;
		/// <summary>
		/// ランキング日付カラムクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listViewRanking_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			//Dateの場合昇順降順を逆にする
			if ((((ListView)sender).Columns[e.Column] == columnHeaderRankDate))
			{
				blRankorder_asc = !blRankorder_asc;
			}

			//昇順の場合
			if (blRankorder_asc)
			{
				if ((((ListView)sender).Columns[e.Column] == columnHeaderRankDate))
				{
					//ListViewItemComparerDateAscを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerDateAsc(e.Column);
				}
			}
			//降順の場合
			else
			{
				if ((((ListView)sender).Columns[e.Column] == columnHeaderRankDate))
				{
					//ListViewItemComparerDateDescを指定する
					((ListView)sender).ListViewItemSorter = new ListViewItemComparerDateDesc(e.Column);
				}
			}

			//グラフ再描画
			pictureBoxGraphRanking.Invalidate();
		}

		/// <summary>
		/// ラジオボタンチェンジイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void radioButtonRank_CheckedChanged(object sender, EventArgs e)
		{
			//pictureBoxGraphRankingを描画しなおす
			pictureBoxGraphRanking.Invalidate();
		}

		/// <summary>
		/// ランキングデータ削除
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolStripMenuItemRankListViewRemove_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("この項目を削除しますか？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{

				//選択された項目の削除(複数選択可)
				ListView.SelectedListViewItemCollection _ListViewItems = listViewRanking.SelectedItems;
				foreach (ListViewItem var in _ListViewItems)
				{
					for (int i = 0; i < RankList.Count; i++)
					{
						if (((RankData)RankList[i]).Date == var.SubItems[columnHeaderRankDate.Index].Text)
						{
							RankList.Remove(RankList[i]);
						}
					}

				}

				//RankData.xmlに保存
				string id = "";
				foreach (AccountData item in ada.DataList)
				{
					if (item.UserName == comboBoxActiveUser.Text)
					{
						id = item.ID;
					}
				}
				string datapath = GetAppPath() + "\\data\\" + id + "\\RankData.xml";

				Type[] et = new Type[] { typeof(RankData) };

				System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList), et);
				try
				{
					using (System.IO.FileStream fs = new System.IO.FileStream(datapath, System.IO.FileMode.Create))
					{
						serializer.Serialize(fs, RankList);
					}
				}
				catch (System.Exception er)
				{
					MessageBox.Show(er.ToString(), "RankData.xml保存失敗");
					return;
				}

				//RankingListView更新
				RankingListViewItemAdd();

				//pictureBoxGraphを描画しなおす
				pictureBoxGraphRanking.Invalidate();
			}
		}

		#endregion

		#region アカウント管理タブ

		private AccountDataArray ada = new AccountDataArray();

		/// <summary>
		/// アカウント情報読み込み
		/// </summary>
		private void LoadAccount()
		{
			string path = GetAppPath() + "\\user\\user.xml";
			if (File.Exists(path) == false)
			{
				MessageBox.Show("アカウント管理タブでアカウント情報を設定して下さい");
				return;
			}

			//XmlSerializerオブジェクトの作成
			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AccountDataArray));

			//ファイルを開く
			System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open);

			//XMLファイルから読み込み、逆シリアル化する
			ada = (AccountDataArray)serializer.Deserialize(fs);

			//閉じる
			fs.Close();
		}

		/// <summary>
		/// アカウント情報保存
		/// </summary>
		private void SaveAccount()
		{
			string path = GetAppPath() + "\\user\\user.xml";
			if (File.Exists(path) == false)
			{
				Directory.CreateDirectory(GetAppPath() + "\\user");
			}

			//XmlSerializerオブジェクトを作成
			//書き込むオブジェクトの型を指定する
			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AccountDataArray));

			//ファイルを開く
			System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);

			//シリアル化し、XMLファイルに保存する
			serializer.Serialize(fs, ada);
			//閉じる
			fs.Close();
		}

		/// <summary>
		/// アクティブユーザコンボボックスを更新
		/// </summary>
		public void RefreshActiveUserBox()
		{
			comboBoxActiveUser.Items.Clear();

			if (ada.DataList.Count == 0)
			{
				comboBoxActiveUser.SelectedIndex = -1;
				return;
			}

			foreach (AccountData item in ada.DataList)
			{
				comboBoxActiveUser.Items.Add(item.UserName);
			}

			try
			{
				comboBoxActiveUser.SelectedIndex = Settings.Instance.ActiveUser;
			}
			catch
			{
				comboBoxActiveUser.SelectedIndex = -1;
			}
		}

		/// <summary>
		/// IDを自動で付ける
		/// </summary>
		/// <returns>ID</returns>
		private string IDsearch()
		{
			int i = 0;
			string idstr = "User" + i.ToString();
			bool bl;
			while (true)
			{
				bl = true;
				foreach (AccountData item in ada.DataList)
				{
					if (item.ID == idstr)
					{
						bl = false;
						break;
					}
				}

				if (bl)
				{
					break;
				}
				else
				{
					i++;
					idstr = "User" + i.ToString();
				}
			}

			return idstr;
		}

		/// <summary>
		/// アカウント新規作成ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAccountNew_Click(object sender, EventArgs e)
		{
			//アカウント登録ウィンドウ表示
			FormAccount f = new FormAccount();
			f.ShowDialog();

			if (f.OK)
			{
				//IDが未記入の時にかぶらないように自動で付ける
				string IDbuf;
				if (f.ID == "" || f.ID == null)
				{
					IDbuf = IDsearch();
				}
				else
				{
					IDbuf = f.ID;
				}

				//アカウント登録処理
				AccountData ad = new AccountData();
				//ad.IndexNo = indexcnt.ToString();
				ad.UserName = f.UserName;
				ad.TargetKD = f.TargetKD;
				ad.TargetHS = f.TargetHS;
				ad.ID = IDbuf;
				ad.Pass = f.Pass;
				ad.ClanName = f.ClanName;
				ad.LoginSite = f.LoginSite;

				bool saveok = true;
				if (ad.UserName == null || ad.UserName == "")
				{
					MessageBox.Show("ゲーム内ユーザ名が未記入です");
					saveok = false;
				}

				//username/ID重複チェック
				foreach (AccountData item in ada.DataList)
				{
					if (item.UserName == ad.UserName)
					{
						MessageBox.Show("ゲーム内ユーザ名が重複しています");
						saveok = false;
						break;
					}

					if (item.ID == ad.ID)
					{
						MessageBox.Show("IDが重複しています");
						saveok = false;
						break;
					}
				}

				if (saveok)
				{
					ada.DataList.Add(ad);
				}
			}
			else
			{
				buttonAccountADJ.Enabled = false;
				buttonAccountDel.Enabled = false;
				buttonAccountNew.Focus();
			}

			f.Close();
			f.Dispose();

			//アクティブユーザコンボボックスアイテム更新
			RefreshActiveUserBox();

			//Title
			TitleDraw();

			//リストビュー更新
			RefreshlistViewAccount();

			//メニューストリップのアクティブユーザメニュー更新
			MenuRefresh();
		}

		/// <summary>
		/// アカウント修正ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAccountADJ_Click(object sender, EventArgs e)
		{
			//アクティブユーザ待避
			string oldactive = comboBoxActiveUser.Text;

			string un = ""; //= listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountUser.Index].Text;
			string tkd = ""; //= listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountTargetKD.Index].Text;
			string ths = ""; //= listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountTargetHS.Index].Text;
			string id = ""; //= listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountID.Index].Text;
			string ps = ""; //= listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountPass.Index].Text;
			string cn = ""; //= listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountClan.Index].Text;
			int ls = 0;

			foreach (AccountData item in ada.DataList)
			{
				if (item.ID == listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountID.Index].Text)
				{
					un = item.UserName;
					tkd = item.TargetKD;
					ths = item.TargetHS;
					id = item.ID;
					ps = item.Pass;
					cn = item.ClanName;
					if (item.LoginSite == "" || item.LoginSite == null)
					{
						ls = 0;
					}
					else
					{
						if (item.LoginSite == "gameyarou")
						{
							ls = 0;
						}
						else if (item.LoginSite == "gamania")
						{
							ls = 1;
						}
						else if (item.LoginSite == "daletto")
						{
							ls = 2;
						}
					}

					break;
				}
			}

			//アカウント登録ウィンドウ表示
			FormAccount f = new FormAccount(un, tkd, ths, id, ps, cn, ls);
			f.ShowDialog();

			if (f.OK)
			{
				//IDが未記入の時にかぶらないように自動で付ける
				string IDbuf;
				if (f.ID == "" || f.ID == null)
				{
					IDbuf = IDsearch();
				}
				else
				{
					IDbuf = f.ID;
				}

				//アカウント登録処理
				AccountData ad = new AccountData();
				ad.UserName = f.UserName;
				ad.TargetKD = f.TargetKD;
				ad.TargetHS = f.TargetHS;
				ad.ID = IDbuf;
				ad.Pass = f.Pass;
				ad.ClanName = f.ClanName;
				ad.LoginSite = f.LoginSite;

				bool saveok = true;
				if (f.UserName == null || f.UserName == "")
				{
					MessageBox.Show("ゲーム内ユーザ名が未記入です");
					saveok = false;
				}

				//username/ID重複チェック
				foreach (AccountData item in ada.DataList)
				{
					if (ad.UserName != un)
					{
						if (ad.UserName == item.UserName)
						{
							MessageBox.Show("ゲーム内ユーザ名が重複しています");
							saveok = false;
							break;
						}
					}

					if (ad.ID != id)
					{
						if (ad.ID == item.ID)
						{
							MessageBox.Show("IDが重複しています");
							saveok = false;
							break;
						}
					}
				}

				if (saveok)
				{

					foreach (AccountData item in ada.DataList)
					{
						if (listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountID.Index].Text == item.ID)
						{
							ada.DataList.Remove(item);

							ada.DataList.Add(ad);

							break;
						}
					}
				}
			}
			else
			{
				buttonAccountADJ.Enabled = false;
				buttonAccountDel.Enabled = false;
				buttonAccountNew.Focus();
			}

			f.Close();
			f.Dispose();

			//アクティブユーザコンボボックスアイテム更新
			RefreshActiveUserBox();

			//アクティブユーザを元通りにする
			comboBoxActiveUser.SelectedIndex = comboBoxActiveUser.FindStringExact(oldactive);

			//Title
			TitleDraw();

			//リストビュー更新
			RefreshlistViewAccount();

			buttonAccountADJ.Enabled = false;
			buttonAccountDel.Enabled = false;
			buttonAccountNew.Focus();

			//メニューストリップのアクティブユーザメニュー更新
			MenuRefresh();
		}

		/// <summary>
		/// アカウント削除ボタン
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAccountDel_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("このアカウントを削除しますか？", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				//アカウントデータを削除
				foreach (AccountData item in ada.DataList)
				{
					if (listViewAccount.SelectedItems[0].SubItems[columnHeaderAccountID.Index].Text == item.ID)
					{
						ada.DataList.Remove(item);

						break;
					}
				}
			}

			//アクティブユーザコンボボックスアイテム更新
			RefreshActiveUserBox();

			TitleDraw();

			//リストビュー更新
			RefreshlistViewAccount();

			buttonAccountADJ.Enabled = false;
			buttonAccountDel.Enabled = false;
			buttonAccountNew.Focus();

			//メニューストリップのアクティブユーザメニュー更新
			MenuRefresh();
		}

		/// <summary>
		/// バージョンメニュー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuVersion_Click(object sender, EventArgs e)
		{
			VersionInfo vi = new VersionInfo(Title);
			vi.ShowDialog();
		}

		/// <summary>
		/// アクティブユーザコンボボックスchanged
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBoxActiveUser_SelectedIndexChanged(object sender, EventArgs e)
		{
			TitleDraw();

			//data.xmlを読み込む
			LoadDataXml();

			//TotalData.xmlを読み込む
			LoadTotalData();

			//RankData.xmlを読み込む
			LoadRankDataXml();

			//TextboxMainInfo表示
			PrintTextboxMainInfo();

			//ListView表示
			ListViewItemAdd();

			//ListViewRanking表示
			RankingListViewItemAdd();

			//ログインサイトボタン更新
			foreach (AccountData item in ada.DataList)
			{
				if (comboBoxActiveUser.Text == item.UserName)
				{
					switch (item.LoginSite)
					{
						case "gameyarou":
							buttonGoLoginSite.Text = "ログインサイト表示\r\n[ゲームヤロウ]";
							break;
						case "gamania":
							buttonGoLoginSite.Text = "ログインサイト表示\r\n[ガマニア]";
							break;
						case "daletto":
							buttonGoLoginSite.Text = "ログインサイト表示\r\n[ダレット]";
							break;
						case "NEXON":
							buttonGoLoginSite.Text = "ログインサイト表示\r\n[NEXON]";
							break;
						default:
							break;
					}
				}
			}

			//メニューストリップのアクティブユーザメニュー更新
			MenuRefresh();

			//グラフ再描画
			pictureBoxGraph.Invalidate();
			pictureBoxGraphRanking.Invalidate();
			pictureBoxCategoryGraph.Invalidate();

			//コンボボックスに使用アイテム追加
			LoadCategoryCombobox();
		}

		/// <summary>
		/// アクティブユーザメニュー更新
		/// </summary>
		private void MenuRefresh()
		{
			int cnt = menuActiveUser.DropDownItems.Count;
			for (int i = (cnt - 1); i >= 0; i--)
			{
				menuActiveUser.DropDownItems.RemoveAt(i);
			}
			for (int i = 0; i < comboBoxActiveUser.Items.Count; i++)
			{
				ToolStripMenuItem _ItemNew = new ToolStripMenuItem();
				_ItemNew.Text = comboBoxActiveUser.Items[i].ToString();
				_ItemNew.Checked = false;
				if (comboBoxActiveUser.SelectedIndex == i)
				{
					_ItemNew.CheckState = CheckState.Checked;
					_ItemNew.Checked = true;
				}
				_ItemNew.Click += new EventHandler(_ItemNew_Click);
				menuActiveUser.DropDownItems.Add(_ItemNew);
			}
		}

		/// <summary>
		/// アクティブユーザメニュー選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void _ItemNew_Click(object sender, EventArgs e)
		{
			foreach (ToolStripMenuItem item in menuActiveUser.DropDownItems)
			{
				item.Checked = false;
			}
			((ToolStripMenuItem)sender).Checked = true;

			for (int i = 0; i < comboBoxActiveUser.Items.Count; i++)
			{
				if (((ToolStripMenuItem)sender).Text == comboBoxActiveUser.Items[i].ToString())
				{
					comboBoxActiveUser.SelectedIndex = i;
					break;
				}
			}
		}

		/// <summary>
		/// リストビュー更新
		/// </summary>
		private void RefreshlistViewAccount()
		{
			listViewAccount.Items.Clear();

			//描画抑制
			listViewAccount.BeginUpdate();

			foreach (AccountData item in ada.DataList)
			{
				//Pass欄
				string passstr = "";
				for (int i = 0; i < item.Pass.Length; i++)
				{
					passstr += "*";
				}

				ListViewItem itemx = new ListViewItem();

				itemx.Text = item.UserName;
				itemx.SubItems.Add(item.TargetKD);
				itemx.SubItems.Add(item.TargetHS);
				itemx.SubItems.Add(item.ID);
				itemx.SubItems.Add(passstr);
				itemx.SubItems.Add(item.ClanName);
				if (item.LoginSite == "gameyarou")
				{
					itemx.SubItems.Add("ゲームヤロウ");
				}
				else if (item.LoginSite == "gamania")
				{
					itemx.SubItems.Add("ガマニア");
				}
				else if (item.LoginSite == "daletto")
				{
					itemx.SubItems.Add("ダレット");
				}
				else if (item.LoginSite == "NEXON")
				{
					itemx.SubItems.Add("NEXON");
				}


				listViewAccount.Items.Add(itemx);
			}

			//描画
			listViewAccount.EndUpdate();
		}

		private void tabPageAccount_Enter(object sender, EventArgs e)
		{
			RefreshlistViewAccount();
		}

		/// <summary>
		/// リストビュー選択
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listViewAccount_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (listViewAccount.SelectedItems.Count >= 1)
			{
				buttonAccountADJ.Enabled = true;
				buttonAccountDel.Enabled = true;

				if (e.ItemIndex != 0)
				{
					buttonAccountUP.Enabled = true;
				}
				else
				{
					buttonAccountUP.Enabled = false;
				}

				if (e.ItemIndex != (ada.DataList.Count - 1))
				{
					buttonAccountDown.Enabled = true;
				}
				else
				{
					buttonAccountDown.Enabled = false;
				}
			}
			else
			{
				buttonAccountADJ.Enabled = false;
				buttonAccountDel.Enabled = false;
				buttonAccountUP.Enabled = false;
				buttonAccountDown.Enabled = false;
			}
		}

		/// <summary>
		/// アカウント管理リストビューの順序を上げる
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAccountUP_Click(object sender, EventArgs e)
		{
			int focusindex = listViewAccount.SelectedItems[0].Index;
			if (listViewAccount.SelectedItems.Count == 1 && focusindex != 0)
			{
				//アクティブユーザ待避
				string oldactive = comboBoxActiveUser.Text;

				//上の要素と交換
				int element = listViewAccount.SelectedItems[0].Index;
				AccountData acbuf = new AccountData();
				acbuf = (AccountData)ada.DataList[element];
				ada.DataList[element] = ada.DataList[element - 1];
				ada.DataList[element - 1] = acbuf;

				//アクティブユーザコンボボックスアイテム更新
				RefreshActiveUserBox();

				//アクティブユーザを元通りにする
				comboBoxActiveUser.SelectedIndex = comboBoxActiveUser.FindStringExact(oldactive);

				//Title
				TitleDraw();

				//リストビュー更新
				RefreshlistViewAccount();

				listViewAccount.Focus();
				listViewAccount.Items[focusindex - 1].Selected = true;

				//メニューストリップのアクティブユーザメニュー更新
				MenuRefresh();
			}

		}
		/// <summary>
		/// アカウント管理リストビューの順序を下げる
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAccountDown_Click(object sender, EventArgs e)
		{
			int focusindex = listViewAccount.SelectedItems[0].Index;
			if (listViewAccount.SelectedItems.Count == 1 && focusindex != (ada.DataList.Count - 1))
			{
				//アクティブユーザ待避
				string oldactive = comboBoxActiveUser.Text;

				//下の要素と交換
				int element = listViewAccount.SelectedItems[0].Index;
				AccountData acbuf = new AccountData();
				acbuf = (AccountData)ada.DataList[element];
				ada.DataList[element] = ada.DataList[element + 1];
				ada.DataList[element + 1] = acbuf;

				//アクティブユーザコンボボックスアイテム更新
				RefreshActiveUserBox();

				//アクティブユーザを元通りにする
				comboBoxActiveUser.SelectedIndex = comboBoxActiveUser.FindStringExact(oldactive);

				//Title
				TitleDraw();

				//リストビュー更新
				RefreshlistViewAccount();

				listViewAccount.Focus();
				listViewAccount.Items[focusindex + 1].Selected = true;

				//メニューストリップのアクティブユーザメニュー更新
				MenuRefresh();
			}
		}
		#endregion

		#region WEBブラウザ
		//private const string gameyarouURL = "http://suddenattack.gameyarou.jp/index.asp";
		private const string gameyarouURL = "http://suddenattack.gameyarou.jp/";//"http://www.gameyarou.jp/?d_url=https%3A%2F%2Fsecure%2Egameyarou%2Ejp%2FMember%2FLogin%2F%5F%5FCommon%5FLogin%5FForm%2Easp%3Fr%5Furl%3Dhttp%3A%2F%2Fsuddenattack%2Egameyarou%2Ejp";
		private const string gamaniaURL = "https://service.gamania.co.jp/suddenattack/login/index.asp";
		private const string dalettoURL = "http://www.daletto.com/pc/sa/login.html";
		//private const string NEXONURL = "http://sa.nexon.co.jp/top.aspx";
		private const string NEXONURL = "http://www.suddenattack.jp/";

		/// <summary>
		/// ログインサイト表示
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonGoLoginSite_Click(object sender, EventArgs e)
		{
			string loginsite = "";
			foreach (AccountData item in ada.DataList)
			{
				if (comboBoxActiveUser.Text == item.UserName)
				{
					loginsite = item.LoginSite;
				}
			}

			switch (loginsite)
			{
				case "gameyarou":
					webBrowserLoginSite.Navigate(new Uri(gameyarouURL));
					break;
				case "gamania":
					webBrowserLoginSite.Navigate(new Uri(gamaniaURL));
					break;
				case "daletto":
					webBrowserLoginSite.Navigate(new Uri(dalettoURL));
					break;
				case "NEXON":
					webBrowserLoginSite.Navigate(new Uri(NEXONURL));
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// アカウント自動入力
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void buttonAccountInput_Click(object sender, EventArgs e)
		{
			if (webBrowserLoginSite.Url == null) return;

			string loginsite = "";
			string ID = "";
			string PW = "";
			foreach (AccountData item in ada.DataList)
			{
				if (comboBoxActiveUser.Text == item.UserName)
				{
					loginsite = item.LoginSite;
					ID = item.ID;
					PW = item.Pass;
				}
			}

			switch (loginsite)
			{
				case "gameyarou":
					if (webBrowserLoginSite.Url.ToString() == gameyarouURL)
					{
						try
						{
							HtmlElement id = webBrowserLoginSite.Document.All.GetElementsByName("user_id")[0];
							HtmlElement pass = webBrowserLoginSite.Document.All.GetElementsByName("user_pwd")[0];

							id.SetAttribute("value", ID);
							pass.SetAttribute("value", PW);
						}
						catch
						{
						}
					}
					break;
				case "gamania":
					if (webBrowserLoginSite.Url.ToString() == gamaniaURL)
					{
						try
						{
							HtmlElement id = webBrowserLoginSite.Document.All.GetElementsByName("txtAct")[0];
							HtmlElement pass = webBrowserLoginSite.Document.All.GetElementsByName("txtPwd")[0];

							id.SetAttribute("Value", ID);
							pass.SetAttribute("Value", PW);
						}
						catch
						{
						}
					}
					break;
				case "daletto":
					try
					{
						HtmlElement id = webBrowserLoginSite.Document.All.GetElementsByName("id")[0];
						HtmlElement pass = webBrowserLoginSite.Document.All.GetElementsByName("pw")[0];

						id.SetAttribute("Value", ID);
						pass.SetAttribute("Value", PW);
					}
					catch
					{
					}
					break;
				case "NEXON":
					try
					{
						HtmlElement id = webBrowserLoginSite.Document.All.GetElementsByName("login_id")[0];
						HtmlElement pass = webBrowserLoginSite.Document.All.GetElementsByName("login_password")[0];

						id.SetAttribute("Value", ID);
						pass.SetAttribute("Value", PW);
					}
					catch
					{
					}
					break;
				default:
					break;
			}
		}

		private void buttonBack_Click(object sender, EventArgs e)
		{
			webBrowserLoginSite.GoBack();
		}

		private void buttonForward_Click(object sender, EventArgs e)
		{
			webBrowserLoginSite.GoForward();
		}

		private void buttonRefresh_Click(object sender, EventArgs e)
		{
			webBrowserLoginSite.Refresh();
		}

		private void buttonStop_Click(object sender, EventArgs e)
		{
			webBrowserLoginSite.Stop();
		}

		private void buttonClose_Click(object sender, EventArgs e)
		{
			webBrowserLoginSite.Url = null;
			textBoxURL.Text = "";
		}

		private void webBrowserLoginSite_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			if (webBrowserLoginSite.Url != null)
			{
				textBoxURL.Text = webBrowserLoginSite.Url.ToString();
			}
		}

		private void buttonGo_Click(object sender, EventArgs e)
		{
			if (textBoxURL.Text != null && textBoxURL.Text != "")
			{
				try
				{
					webBrowserLoginSite.Url = new Uri(textBoxURL.Text);
				}
				catch
				{
				}
			}
		}
		#endregion
	}
}
