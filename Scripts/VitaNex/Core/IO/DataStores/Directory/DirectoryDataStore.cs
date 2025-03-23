#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace VitaNex.IO
{
	public abstract class DirectoryDataStore<TKey, TVal> : DataStore<TKey, TVal>
	{
		public virtual string FileExtension { get; set; }

		public DirectoryDataStore(DirectoryInfo root)
			: base(root)
		{
			FileExtension = "vnc";
		}

		public DirectoryDataStore(DirectoryInfo root, string name)
			: base(root, name)
		{
			FileExtension = "vnc";
		}

		public DirectoryDataStore(DirectoryInfo root, string name, string fileExt)
			: base(root, name)
		{
			FileExtension = String.IsNullOrWhiteSpace(fileExt) ? "vnc" : fileExt;
		}

		protected override void OnExport()
		{
			Root.EmptyDirectory(false);

			foreach (var kv in this)
			{
				OnExport(kv);
			}
		}

		protected virtual void OnExport(KeyValuePair<TKey, TVal> kv)
		{
			var key = kv.Key;
			var value = kv.Value;

			var fileName = IOUtility.GetSafeFileName(String.Format("{0} ({1}).{2}", key, value, FileExtension), '%');

			try
			{
				OnExport(IOUtility.EnsureFile(Root.FullName + "/" + fileName, true), key, value);
			}
			catch (Exception e)
			{
				lock (SyncRoot)
				{
					Errors.Add(e);
				}
			}
		}

		protected override void OnImport()
		{
			foreach (var file in Root.GetFiles().Where(file => file.Name.EndsWith("." + FileExtension)))
			{
				OnImport(file);
			}
		}

		protected virtual void OnImport(FileInfo file)
		{
			try
			{
				OnImport(file, out var key, out var val);

				if (key != null && val != null)
				{
					this[key] = val;
				}
			}
			catch (Exception e)
			{
				lock (SyncRoot)
				{
					Errors.Add(e);
				}
			}
		}

		protected abstract void OnExport(FileInfo file, TKey key, TVal val);
		protected abstract void OnImport(FileInfo file, out TKey key, out TVal val);
	}
}
