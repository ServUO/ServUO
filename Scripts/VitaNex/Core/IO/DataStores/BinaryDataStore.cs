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
using System.Threading;

using Server;
#endregion

namespace VitaNex.IO
{
	public class BinaryDataStore<T1, T2> : DataStore<T1, T2>
	{
		private readonly ManualResetEvent _Sync = new ManualResetEvent(true);

		public FileInfo Document { get; set; }

		public Func<GenericWriter, bool> OnSerialize { get; set; }
		public Func<GenericReader, bool> OnDeserialize { get; set; }

		public bool Async { get; set; }

		public BinaryDataStore(string root, string name)
			: this(IOUtility.EnsureDirectory(root), name)
		{ }

		public BinaryDataStore(DirectoryInfo root, string name)
			: base(root, name)
		{
			Document = IOUtility.EnsureFile(root.FullName + "/" + name + ((name.IndexOf('.') != -1) ? String.Empty : ".bin"));
		}

		public override DataStoreResult Export()
		{
			_Sync.WaitOne();

			var res = base.Export();

			if (res != DataStoreResult.OK)
			{
				_Sync.Set();
			}
			else if (!_Sync.WaitOne(0))
			{
				Status = DataStoreStatus.Exporting;
			}

			return res;
		}

		protected override void OnExport()
		{
			_Sync.Reset();

			if (Async)
			{
				Document.SerializeAsync(OnExport);
			}
			else
			{
				Document.Serialize(OnExport);
			}
		}

		protected virtual void OnExport(GenericWriter writer)
		{
			try
			{
				var handled = false;

				if (OnSerialize != null)
				{
					handled = OnSerialize(writer);
				}

				if (!handled)
				{
					Serialize(writer);
				}
			}
			finally
			{
				if (Status == DataStoreStatus.Exporting)
				{
					Status = DataStoreStatus.Idle;
				}

				_Sync.Set();
			}
		}

		protected override void OnImport()
		{
			if (Document.Exists && Document.Length > 0)
			{
				Document.Deserialize(OnImport);
			}
		}

		protected virtual void OnImport(GenericReader reader)
		{
			var handled = false;

			if (OnDeserialize != null)
			{
				handled = OnDeserialize(reader);
			}

			if (!handled)
			{
				Deserialize(reader);
			}
		}

		protected virtual void Serialize(GenericWriter writer)
		{ }

		protected virtual void Deserialize(GenericReader reader)
		{ }
	}
}
