//using System;
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Windows.Forms;
//using Microsoft.Win32;
//using System.Collections.Generic;
//using System.Collections;


//[Serializable()]
//class LoginSetting
//{
//    public ArrayList ID { get; set; }
//    public ArrayList PW { get; set; }
//    public int LoginLeftFormCheck { get; set; }
//    public int getLinkGameExecute { get; set; }
//    public int AutoClose { get; set; }

//    //LoginSettingクラスのただ一つのインスタンス
//    [NonSerialized()]
//    private static LoginSetting _instance;
//    [System.Xml.Serialization.XmlIgnore]
//    public static LoginSetting Instance
//    {
//        get
//        {
//            if (_instance == null)
//                _instance = new LoginSetting();
//            return _instance;
//        }
//        set { _instance = value; }
//    }

//    /// <summary>
//    /// 設定をバイナリファイルから読み込み復元する
//    /// </summary>
//    public static void LoadFromBinaryFile()
//    {
//        string path = GetSettingPath();

//        if (File.Exists(path) == false)
//        {
//            return;
//        }

//        FileStream fs = new FileStream(path,
//            FileMode.Open,
//            FileAccess.Read);
//        BinaryFormatter bf = new BinaryFormatter();
//        //読み込んで逆シリアル化する
//        object obj = bf.Deserialize(fs);
//        fs.Close();

//        Instance = (LoginSetting)obj;
//    }

//    /// <summary>
//    /// 現在の設定をバイナリファイルに保存する
//    /// </summary>
//    public static void SaveToBinaryFile()
//    {
//        string path = GetSettingPath();

//        //SALoginフォルダが無かったら作成
//        if (Directory.Exists("SALogin") == false)
//        {
//            Directory.CreateDirectory("SALogin");
//        }

//        FileStream fs = new FileStream(path,
//            FileMode.Create,
//            FileAccess.Write);
//        BinaryFormatter bf = new BinaryFormatter();
//        //シリアル化して書き込む
//        bf.Serialize(fs, Instance);
//        fs.Close();
//    }

//    private static string GetSettingPath()
//    {
//        string dir = GetAppPath();
//        string path = dir + "\\" + "SALogin\\Login.config";
//        return path;
//    }

//}
