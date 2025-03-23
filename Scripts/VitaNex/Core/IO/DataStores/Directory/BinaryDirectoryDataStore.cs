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
using System.IO;

using Server;
#endregion

namespace VitaNex.IO
{
	public class BinaryDirectoryDataStore<TKey, TVal> : DirectoryDataStore<TKey, TVal>
	{
		public Func<GenericWriter, TKey, TVal, bool> OnSerialize { get; set; }
		public Func<GenericReader, Tuple<TKey, TVal>> OnDeserialize { get; set; }

		public bool Async { get; set; }

		public BinaryDirectoryDataStore(string root, string name, string fileExt)
			: this(IOUtility.EnsureDirectory(root + "/" + name), name, fileExt)
		{ }

		public BinaryDirectoryDataStore(DirectoryInfo root, string name, string fileExt)
			: base(root, name)
		{
			FileExtension = String.IsNullOrWhiteSpace(fileExt) ? "vnc" : fileExt;
		}

		protected override void OnExport(FileInfo file, TKey key, TVal val)
		{
			if (Async)
			{
				file.SerializeAsync(writer => OnExport(writer, key, val));
			}
			else
			{
				file.Serialize(writer => OnExport(writer, key, val));
			}
		}

		protected virtual void OnExport(GenericWriter writer, TKey key, TVal val)
		{
			var handled = false;

			if (OnSerialize != null)
			{
				handled = OnSerialize(writer, key, val);
			}

			if (!handled)
			{
				Serialize(writer, key, val);
			}
		}

		protected override void OnImport(FileInfo file, out TKey key, out TVal val)
		{
			var keyBox = default(TKey);
			var valBox = default(TVal);

			if (file == null || !file.Exists || file.Length == 0)
			{
				key = keyBox;
				val = valBox;
				return;
			}

			file.Deserialize(reader =>
			{
				var handled = false;

				if (OnDeserialize != null)
				{
					var entry = OnDeserialize(reader);

					if (entry != null)
					{
						keyBox = entry.Item1;
						valBox = entry.Item2;
						handled = true;
					}
				}

				if (!handled)
				{
					Deserialize(reader, out keyBox, out valBox);
				}
			});

			key = keyBox;
			val = valBox;
		}

		protected virtual void Serialize(GenericWriter writer, TKey key, TVal val)
		{ }

		protected virtual void Deserialize(GenericReader reader, out TKey key, out TVal val)
		{
			key = default(TKey);
			val = default(TVal);
		}
	}
}
