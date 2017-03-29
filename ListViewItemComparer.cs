using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace SAAR
{
	/// <summary>
	/// ListViewのstr項目の昇順並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparerStrAsc : IComparer
	{
		private int _column;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="col">並び替える列番号</param>
		public ListViewItemComparerStrAsc(int col)
		{
			_column = col;
		}

		//xがyより小さいときはマイナスの数、大きいときはプラスの数、
		//同じときは0を返す
		public int Compare(object x, object y)
		{
			//ListViewItemの取得
			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			//xとyを文字列として比較する
			return string.Compare(itemx.SubItems[_column].Text, itemy.SubItems[_column].Text);
		}
	}

	/// <summary>
	/// ListViewのstr項目の降順並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparerStrDesc : IComparer
	{
		private int _column;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="col">並び替える列番号</param>
		public ListViewItemComparerStrDesc(int col)
		{
			_column = col;
		}

		//xがyより小さいときはマイナスの数、大きいときはプラスの数、
		//同じときは0を返す
		public int Compare(object x, object y)
		{
			//ListViewItemの取得
			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			//xとyを文字列として比較する
			return -(string.Compare(itemx.SubItems[_column].Text, itemy.SubItems[_column].Text));
		}
	}

	/// <summary>
	/// ListViewのint項目の昇順並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparerIntAsc : IComparer
	{
		private int _column;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="col">並び替える列番号</param>
		public ListViewItemComparerIntAsc(int col)
		{
			_column = col;
		}

		//xがyより小さいときはマイナスの数、大きいときはプラスの数、
		//同じときは0を返す
		public int Compare(object x, object y)
		{
			//ListViewItemの取得
			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			if (itemx.SubItems[_column].Text == "" || itemx.SubItems[_column].Text == null) return -1;
			if (itemy.SubItems[_column].Text == "" || itemy.SubItems[_column].Text == null) return -1;

			//xとyを比較する
			if ((int.Parse(itemx.SubItems[_column].Text)) > (int.Parse(itemy.SubItems[_column].Text)))
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}
	}

	/// <summary>
	/// ListViewのint項目の降順並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparerIntDesc : IComparer
	{
		private int _column;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="col">並び替える列番号</param>
		public ListViewItemComparerIntDesc(int col)
		{
			_column = col;
		}

		//xがyより小さいときはマイナスの数、大きいときはプラスの数、
		//同じときは0を返す
		public int Compare(object x, object y)
		{
			//ListViewItemの取得
			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			if (itemx.SubItems[_column].Text == "" || itemx.SubItems[_column].Text == null) return -1;
			if (itemy.SubItems[_column].Text == "" || itemy.SubItems[_column].Text == null) return -1;

			//xとyを比較する
			if ((int.Parse(itemx.SubItems[_column].Text)) > (int.Parse(itemy.SubItems[_column].Text)))
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
	}

	/// <summary>
	/// ListViewのfloat項目の昇順並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparerFloatAsc : IComparer
	{
		private int _column;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="col">並び替える列番号</param>
		public ListViewItemComparerFloatAsc(int col)
		{
			_column = col;
		}

		//xがyより小さいときはマイナスの数、大きいときはプラスの数、
		//同じときは0を返す
		public int Compare(object x, object y)
		{
			//ListViewItemの取得
			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			//xとyを比較する
			if ((float.Parse(itemx.SubItems[_column].Text)) > (float.Parse(itemy.SubItems[_column].Text)))
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}
	}

	/// <summary>
	/// ListViewのfloat項目の降順並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparerFloatDesc : IComparer
	{
		private int _column;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="col">並び替える列番号</param>
		public ListViewItemComparerFloatDesc(int col)
		{
			_column = col;
		}

		//xがyより小さいときはマイナスの数、大きいときはプラスの数、
		//同じときは0を返す
		public int Compare(object x, object y)
		{
			//ListViewItemの取得
			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			//xとyを比較する
			if ((float.Parse(itemx.SubItems[_column].Text)) > (float.Parse(itemy.SubItems[_column].Text)))
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
	}

	/// <summary>
	/// ListViewの日付項目の昇順並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparerDateAsc : IComparer
	{
		private int _column;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="col">並び替える列番号</param>
		public ListViewItemComparerDateAsc(int col)
		{
			_column = col;
		}

		//xがyより小さいときはマイナスの数、大きいときはプラスの数、
		//同じときは0を返す
		public int Compare(object x, object y)
		{
			//ListViewItemの取得
			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			string strx = itemx.SubItems[_column].Text;
			string stry = itemy.SubItems[_column].Text;

			//hhが一桁の時には先頭に0を付与
			if (strx.Length == 18)
			{
				strx = strx.Insert(11, "0");
			}
			if (stry.Length == 18)
			{
				stry = stry.Insert(11, "0");
			}

			//xとyを文字列として比較する
			return string.Compare(strx, stry);

		}
	}

	/// <summary>
	/// ListViewの日付項目の降順並び替えに使用するクラス
	/// </summary>
	public class ListViewItemComparerDateDesc : IComparer
	{
		private int _column;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="col">並び替える列番号</param>
		public ListViewItemComparerDateDesc(int col)
		{
			_column = col;
		}

		//xがyより小さいときはマイナスの数、大きいときはプラスの数、
		//同じときは0を返す
		public int Compare(object x, object y)
		{
			//ListViewItemの取得
			ListViewItem itemx = (ListViewItem)x;
			ListViewItem itemy = (ListViewItem)y;

			string strx = itemx.SubItems[_column].Text;
			string stry = itemy.SubItems[_column].Text;

			//hhが一桁の時には先頭に0を付与
			if (strx.Length == 18)
			{
				strx = strx.Insert(11, "0");
			}
			if (stry.Length == 18)
			{
				stry = stry.Insert(11, "0");
			}

			//xとyを文字列として比較する
			return -(string.Compare(strx, stry));

		}
	}
}
