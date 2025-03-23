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
#endregion

namespace VitaNex.IO
{
	public enum DataStoreStatus
	{
		Idle,
		Disposed,
		Initializing,
		Importing,
		Exporting,
		Copying
	}

	public enum DataStoreResult
	{
		Null,
		Busy,
		Error,
		OK
	}

	public interface IDataStore
	{
		DirectoryInfo Root { get; set; }
		string Name { get; set; }
		DataStoreStatus Status { get; }
		List<Exception> Errors { get; }
		bool HasErrors { get; }

		DataStoreResult Import();
		DataStoreResult Export();
	}

	public static class DateStoreIndex
	{
		public static int Value { get; set; }
	}

	public sealed class DataStoreComparer<TKey> : IEqualityComparer<TKey>
	{
		public static DataStoreComparer<TKey> Default { get; private set; }

		static DataStoreComparer()
		{
			Default = new DataStoreComparer<TKey>();
		}

		private IEqualityComparer<TKey> _Impl;

		public IEqualityComparer<TKey> Impl
		{
			get => _Impl ?? EqualityComparer<TKey>.Default;
			set => _Impl = value ?? EqualityComparer<TKey>.Default;
		}

		public DataStoreComparer()
			: this(null)
		{ }

		public DataStoreComparer(IEqualityComparer<TKey> impl)
		{
			Impl = impl;
		}

		public bool Equals(TKey x, TKey y)
		{
			return Impl.Equals(x, y);
		}

		public int GetHashCode(TKey obj)
		{
			return Impl.GetHashCode(obj);
		}
	}

	public abstract class DataStore<TKey, TVal> : Dictionary<TKey, TVal>, IDataStore, IDisposable
	{
		public readonly object SyncRoot = new object();

		public new TVal this[TKey key]
		{
			get
			{
				lock (SyncRoot)
				{
					return base[key];
				}
			}
			set
			{
				lock (SyncRoot)
				{
					base[key] = value;
				}
			}
		}

		public new DataStoreComparer<TKey> Comparer => (DataStoreComparer<TKey>)base.Comparer;

		public virtual DirectoryInfo Root { get; set; }
		public virtual string Name { get; set; }

		public DataStoreStatus Status { get; protected set; }
		public List<Exception> Errors { get; protected set; }

		public bool HasErrors => Errors.Count > 0;

		public bool IsDisposed => Status == DataStoreStatus.Disposed;

		public DataStore(string root, string name = null)
			: this(IOUtility.EnsureDirectory(root), name)
		{ }

		public DataStore(DirectoryInfo root, string name = null)
			: base(DataStoreComparer<TKey>.Default)
		{
			++DateStoreIndex.Value;

			Status = DataStoreStatus.Initializing;
			Errors = new List<Exception>();

			if (String.IsNullOrWhiteSpace(name))
			{
				name = "DataStore" + DateStoreIndex.Value;
			}

			Name = name;

			try
			{
				Root = root.EnsureDirectory(false);
			}
			catch
			{
				Root = IOUtility.EnsureDirectory(VitaNexCore.SavesDirectory + "/DataStores/" + Name);
			}

			Status = DataStoreStatus.Idle;
		}

		~DataStore()
		{
			Dispose();
		}

		public virtual DataStoreResult Import()
		{
			try
			{
				if (Status != DataStoreStatus.Idle)
				{
					return DataStoreResult.Busy;
				}

				lock (SyncRoot)
				{
					Errors.Free(true);
				}

				Status = DataStoreStatus.Importing;

				try
				{
					lock (SyncRoot)
					{
						OnImport();
					}
				}
				catch (Exception e1)
				{
					lock (SyncRoot)
					{
						Errors.Add(e1);
					}

					Status = DataStoreStatus.Idle;
					return DataStoreResult.Error;
				}

				Status = DataStoreStatus.Idle;
				return DataStoreResult.OK;
			}
			catch (Exception e2)
			{
				lock (SyncRoot)
				{
					Errors.Add(e2);
				}

				Status = DataStoreStatus.Idle;
				return DataStoreResult.Error;
			}
		}

		public virtual DataStoreResult Export()
		{
			try
			{
				if (Status != DataStoreStatus.Idle)
				{
					return DataStoreResult.Busy;
				}

				lock (SyncRoot)
				{
					Errors.Free(true);
				}

				Status = DataStoreStatus.Exporting;

				try
				{
					lock (SyncRoot)
					{
						OnExport();
					}
				}
				catch (Exception e1)
				{
					lock (SyncRoot)
					{
						Errors.Add(e1);
					}

					Status = DataStoreStatus.Idle;
					return DataStoreResult.Error;
				}

				Status = DataStoreStatus.Idle;
				return DataStoreResult.OK;
			}
			catch (Exception e2)
			{
				lock (SyncRoot)
				{
					Errors.Add(e2);
				}

				Status = DataStoreStatus.Idle;
				return DataStoreResult.Error;
			}
		}

		public virtual DataStoreResult CopyTo(IDictionary<TKey, TVal> dbTarget)
		{
			return CopyTo(dbTarget, true);
		}

		public virtual DataStoreResult CopyTo(IDictionary<TKey, TVal> dbTarget, bool replace)
		{
			try
			{
				lock (SyncRoot)
				{
					Errors.Free(true);
				}

				if (Status != DataStoreStatus.Idle)
				{
					return DataStoreResult.Busy;
				}

				if (this == dbTarget)
				{
					return DataStoreResult.OK;
				}

				Status = DataStoreStatus.Copying;

				lock (SyncRoot)
				{
					foreach (var kvp in this)
					{
						dbTarget[kvp.Key] = kvp.Value;
					}
				}

				try
				{
					lock (SyncRoot)
					{
						OnCopiedTo(dbTarget);
					}
				}
				catch (Exception e1)
				{
					lock (SyncRoot)
					{
						Errors.Add(e1);
					}

					Status = DataStoreStatus.Idle;
					return DataStoreResult.Error;
				}

				Status = DataStoreStatus.Idle;
				return DataStoreResult.OK;
			}
			catch (Exception e2)
			{
				lock (SyncRoot)
				{
					Errors.Add(e2);
				}

				Status = DataStoreStatus.Idle;
				return DataStoreResult.Error;
			}
		}

		protected virtual void OnCopiedTo(IDictionary<TKey, TVal> dbTarget)
		{ }

		public virtual DataStoreResult CopyFrom(IDictionary<TKey, TVal> dbSource)
		{
			return CopyFrom(dbSource, true);
		}

		public virtual DataStoreResult CopyFrom(IDictionary<TKey, TVal> dbSource, bool replace)
		{
			try
			{
				lock (SyncRoot)
				{
					Errors.Free(true);
				}

				if (Status != DataStoreStatus.Idle)
				{
					return DataStoreResult.Busy;
				}

				if (this == dbSource)
				{
					return DataStoreResult.OK;
				}

				Status = DataStoreStatus.Copying;

				lock (SyncRoot)
				{
					foreach (var kvp in dbSource)
					{
						this[kvp.Key] = kvp.Value;
					}
				}

				try
				{
					lock (SyncRoot)
					{
						OnCopiedFrom(dbSource);
					}
				}
				catch (Exception e1)
				{
					lock (SyncRoot)
					{
						Errors.Add(e1);
					}

					Status = DataStoreStatus.Idle;
					return DataStoreResult.Error;
				}

				Status = DataStoreStatus.Idle;
				return DataStoreResult.OK;
			}
			catch (Exception e2)
			{
				lock (SyncRoot)
				{
					Errors.Add(e2);
				}

				Status = DataStoreStatus.Idle;
				return DataStoreResult.Error;
			}
		}

		protected virtual void OnCopiedFrom(IDictionary<TKey, TVal> dbSource)
		{ }

		protected virtual void OnImport()
		{ }

		protected virtual void OnExport()
		{ }

		public virtual new void Add(TKey key, TVal value)
		{
			lock (SyncRoot)
			{
				base.Add(key, value);
			}
		}

		public virtual new bool Remove(TKey key)
		{
			lock (SyncRoot)
			{
				return base.Remove(key);
			}
		}

		public virtual new bool ContainsKey(TKey key)
		{
			lock (SyncRoot)
			{
				return base.ContainsKey(key);
			}
		}

		public virtual new bool ContainsValue(TVal value)
		{
			lock (SyncRoot)
			{
				return base.ContainsValue(value);
			}
		}

		public virtual new void Clear()
		{
			lock (SyncRoot)
			{
				base.Clear();
			}
		}

		public virtual new bool TryGetValue(TKey key, out TVal value)
		{
			lock (SyncRoot)
			{
				return base.TryGetValue(key, out value);
			}
		}

		public void Dispose()
		{
			if (Status == DataStoreStatus.Disposed)
			{
				return;
			}

			Status = DataStoreStatus.Disposed;

			Clear();

			lock (SyncRoot)
			{
				Errors.Free(true);
			}

			Name = null;
			Root = null;
		}

		public override string ToString()
		{
			return Name ?? base.ToString();
		}
	}

	public class DataStore<T> : DataStore<int, T>
	{
		public DataStore(string root, string name = null)
			: base(root, name)
		{ }

		public DataStore(DirectoryInfo root, string name = null)
			: base(root, name)
		{ }
	}
}
