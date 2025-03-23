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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Server;
using Server.Network;

using VitaNex.Collections;
#endregion

namespace VitaNex.Network
{
	public delegate void OPLQueryValidator(Mobile viewer, IEntity target, ref bool allow);

	/// <summary>
	///     Provides methods for extending Item and Mobile ObjectPropertyLists by invoking event subscriptions - No more
	///     GetProperties overrides!
	/// </summary>
	public sealed class ExtendedOPL : IList<string>, IDisposable
	{
		private static readonly object _OPLLock = new object();

		private static readonly string[] _EmptyBuffer = new string[0];

		/// <summary>
		///     Breaks EmptyClilocs every Nth entry. EG: When 5 entries have been added, use the next clilocID
		/// </summary>
		public static int ClilocBreak = 5;

		/// <summary>
		///     Breaks EmptyClilocs when the current string value length exceeds this threshold, regardless of the current
		///     ClilocBreak.
		/// </summary>
		public static int ClilocThreshold = 160;

		/// <summary>
		///     Empty clilocID list extended with ~1_VAL~ *support should be the same as ~1_NOTHING~
		///     The default settings for an ExtendedOPL instance allows for up to 65 custom cliloc entries.
		///     Capacity is equal to the number of available empty clilocs multiplied by the cliloc break value.
		///     Default: 65 == 13 * 5
		///     Clilocs with multiple argument support will be parsed accordingly.
		///     It is recommended to use clilocs that contain no characters other than the argument placeholders and whitespace.
		/// </summary>
		public static int[] EmptyClilocs =
		{
			//1042971, 1070722, // ~1_NOTHING~ (Reserved by ObjectPropertyList)
			1114057, 1114778, 1114779, // ~1_val~
			1150541, // ~1_TOKEN~
			1153153, // ~1_year~

			1041522, // ~1~~2~~3~
			1060847, // ~1_val~ ~2_val~
			1116560, // ~1_val~ ~2_val~
			1116690, // ~1_val~ ~2_val~ ~3_val~
			1116691, // ~1_val~ ~2_val~ ~3_val~
			1116692, // ~1_val~ ~2_val~ ~3_val~
			1116693, // ~1_val~ ~2_val~ ~3_val~
			1116694 // ~1_val~ ~2_val~ ~3_val~
		};

		public static bool Initialized { get; private set; }

		/// <summary>
		///     Gets a value representing the parent OPL PacketHandler that was overridden, if any
		/// </summary>
		public static PacketHandler ReqOplParent { get; private set; }

		/// <summary>
		///     Gets a value representing the parent BatchOPL PacketHandler that was overridden, if any
		/// </summary>
		public static PacketHandler ReqBatchOplParent { get; private set; }

#if !ServUOX
		/// <summary>
		///     Gets a value representing the parent BatchOPL PacketHandler that was overridden, if any
		/// </summary>
		public static PacketHandler ReqBatchOplParent6017 { get; private set; }
#endif

		/// <summary>
		///     Gets a value represting the handler to use when decoding OPL packet 0xD6
		/// </summary>
		public static OutgoingPacketOverrideHandler OutParent0xD6 { get; private set; }

		/// <summary>
		///     Event called when an Item based OPL is requested
		/// </summary>
		public static event Action<Item, Mobile, ExtendedOPL> OnItemOPLRequest;

		/// <summary>
		///     Event called when a Mobile based OPL is requested
		/// </summary>
		public static event Action<Mobile, Mobile, ExtendedOPL> OnMobileOPLRequest;

		/// <summary>
		///     Event called when an IEntity based OPL is requested that doesn't match an Item or Mobile
		/// </summary>
		public static event Action<IEntity, Mobile, ExtendedOPL> OnEntityOPLRequest;

		/// <summary>
		///     Event called when an IEntity based OPL is requested
		/// </summary>
		public static event OPLQueryValidator OnValidateQuery;

		public static void Init()
		{
			if (Initialized)
			{
				return;
			}

			OutParent0xD6 = OutgoingPacketOverrides.GetHandler(0xD6);
			OutgoingPacketOverrides.Register(0xD6, OnEncode0xD6);

			ReqOplParent = PacketHandlers.GetExtendedHandler(0x10);
			PacketHandlers.RegisterExtended(ReqOplParent.PacketID, ReqOplParent.Ingame, OnQueryProperties);

			ReqBatchOplParent = PacketHandlers.GetHandler(0xD6);
			PacketHandlers.Register(ReqBatchOplParent.PacketID, ReqBatchOplParent.Length, ReqBatchOplParent.Ingame, OnBatchQueryProperties);

#if !ServUOX
			ReqBatchOplParent6017 = PacketHandlers.Get6017Handler(0xD6);
			PacketHandlers.Register6017(ReqBatchOplParent6017.PacketID, ReqBatchOplParent6017.Length, ReqBatchOplParent6017.Ingame, OnBatchQueryProperties);
#endif

			Initialized = true;
		}

		public static ObjectPropertyList ResolveOPL(IEntity e)
		{
			return ResolveOPL(e, false);
		}

		public static ObjectPropertyList ResolveOPL(IEntity e, Mobile v)
		{
			return ResolveOPL(e, v, false);
		}

		public static ObjectPropertyList ResolveOPL(IEntity e, bool headerOnly)
		{
			return ResolveOPL(e, null, headerOnly);
		}

		public static ObjectPropertyList ResolveOPL(IEntity e, Mobile v, bool headerOnly)
		{
			if (e?.Deleted != false)
			{
				return null;
			}

			ObjectPropertyList opl = null;

			if (e is Item item)
			{
				if (item.BeginAction(_OPLLock))
				{
					opl = new ObjectPropertyList(item);

					if (headerOnly)
					{
						item.AddNameProperty(opl);
					}
					else
					{
						item.GetProperties(opl);
						item.AppendChildProperties(opl);
					}

					item.EndAction(_OPLLock);
				}
			}
			else if (e is Mobile mob)
			{
				if (mob.BeginAction(_OPLLock))
				{
					opl = new ObjectPropertyList(mob);

					if (headerOnly)
					{
						mob.AddNameProperties(opl);
					}
					else
					{
						mob.GetProperties(opl);
					}

					mob.EndAction(_OPLLock);
				}
			}
			else
			{
				opl = new ObjectPropertyList(e);

				var result = e.InvokeMethod("AddNameProperty", opl);

				if (result == null || result is Exception)
				{
					result = e.InvokeMethod("AddNameProperties", opl);
				}

				if (result == null || result is Exception)
				{
					opl.Add(e.Name ?? string.Empty);
				}
			}

			if (opl != null)
			{
				if (!headerOnly)
				{
					using (var eopl = new ExtendedOPL(opl))
					{
						InvokeOPLRequest(e, v, eopl);
					}
				}

				opl.Terminate();
				opl.SetStatic();
			}

			return opl;
		}

		private static void OnEncode0xD6(NetState state, PacketReader reader, ref byte[] buffer, ref int length)
		{
			if (state?.Mobile?.Deleted != false || reader == null || buffer == null || length < 0)
			{
				return;
			}

			var pos = reader.Seek(0, SeekOrigin.Current);

			reader.Seek(5, SeekOrigin.Begin);

#if ServUOX
			var serial = reader.ReadSerial();
#else
			var serial = reader.ReadInt32();
#endif

			reader.Seek(pos, SeekOrigin.Begin);

			var ent = World.FindEntity(serial);

			if (ent == null)
			{
				return;
			}

			ObjectPropertyList opl = null;

			try
			{
				opl = ent.GetOPL(state.Mobile);

				if (opl != null)
				{
					buffer = opl.Compile(state.CompressionEnabled, out length);
				}
			}
			finally
			{
				if (opl != null)
				{
					opl.Release();
				}
			}
		}

		private static void OnBatchQueryProperties(NetState state, PacketReader pvSrc)
		{
			if (state?.Mobile?.Deleted != false || pvSrc == null)
			{
				return;
			}

			var length = pvSrc.Size - 3;

			if (length < 0 || length % 4 != 0)
			{
				return;
			}

			var count = length / 4;

			Serial s;

			for (var i = 0; i < count; ++i)
			{
#if ServUOX
				s = pvSrc.ReadSerial();
#else
				s = pvSrc.ReadInt32();
#endif

				if (s.IsValid)
				{
					HandleQueryProperties(state.Mobile, World.FindEntity(s));
				}
			}
		}

		private static void OnQueryProperties(NetState state, PacketReader pvSrc)
		{
			if (state?.Mobile?.Deleted != false || pvSrc == null)
			{
				return;
			}

#if ServUOX
			var serial = pvSrc.ReadSerial();
#else
			var serial = (Serial)pvSrc.ReadInt32();
#endif

			if (serial.IsValid)
			{
				HandleQueryProperties(state.Mobile, World.FindEntity(serial));
			}
		}

		private static void HandleQueryProperties(Mobile viewer, IEntity e)
		{
			if (viewer?.Deleted != false || e?.Deleted != false)
			{
				return;
			}

			if (!viewer.CanSee(e))
			{
				return;
			}

			if (OnValidateQuery != null)
			{
				var allow = true;

				OnValidateQuery(viewer, e, ref allow);

				if (!allow)
				{
					return;
				}
			}

#if ServUOX
			if (viewer.InUpdateRange(e))
			{
				SendPropertiesTo(viewer, e);
			}
#else
			if (e is Mobile m)
			{
				if (Utility.InUpdateRange(viewer, m))
				{
					SendPropertiesTo(viewer, m);
				}
			}
			else if (e is Item item)
			{
				if (Utility.InUpdateRange(viewer, item.GetWorldLocation()))
				{
					SendPropertiesTo(viewer, item);
				}
			}
#endif
		}

		/// <summary>
		///     Forces the compilation of a new Mobile or Item based ObjectPropertyList and sends it to the specified Mobile
		/// </summary>
		/// <param name="to">Mobile viewer, the Mobile viewing the OPL</param>
		/// <param name="e">Entity owner, the Entity which owns the OPL</param>
		public static void SendPropertiesTo(Mobile to, IEntity e)
		{
			if (e is Mobile m)
			{
				SendPropertiesTo(to, m);
			}
			else if (e is Item item)
			{
				SendPropertiesTo(to, item);
			}
		}

		/// <summary>
		///     Forces the compilation of a new Mobile based ObjectPropertyList and sends it to the specified Mobile
		/// </summary>
		/// <param name="to">Mobile viewer, the Mobile viewing the OPL</param>
		/// <param name="m">Mobile owner, the Mobile which owns the OPL</param>
		public static void SendPropertiesTo(Mobile to, Mobile m)
		{
			if (to == null || m == null)
			{
				return;
			}

			ObjectPropertyList opl = null;

			try
			{
				opl = m.GetOPL(to);

				if (opl != null)
				{
					to.Send(opl);
				}
			}
			finally
			{
				opl?.Release();
			}
		}

		/// <summary>
		///     Forces the compilation of a new Item based ObjectPropertyList and sends it to the specified Mobile
		/// </summary>
		/// <param name="to">Mobile viewer, the Mobile viewing the OPL</param>
		/// <param name="item"></param>
		public static void SendPropertiesTo(Mobile to, Item item)
		{
			if (to == null || item == null)
			{
				return;
			}

			ObjectPropertyList opl = null;

			try
			{
				opl = item.GetOPL(to);

				if (opl != null)
				{
					to.Send(opl);
				}
			}
			finally
			{
				if (opl != null)
				{
					opl.Release();
				}
			}
		}

		private static void InvokeOPLRequest(IEntity entity, Mobile viewer, ExtendedOPL eopl)
		{
			if (entity?.Deleted != false || eopl?.IsDisposed != false)
			{
				return;
			}

			if (entity is Mobile mob)
			{
				OnMobileOPLRequest?.Invoke(mob, viewer, eopl);
			}
			else if (entity is Item item)
			{
				OnItemOPLRequest?.Invoke(item, viewer, eopl);
			}

			OnEntityOPLRequest?.Invoke(entity, viewer, eopl);
		}

		public static void AddTo(ObjectPropertyList opl, params object[] args)
		{
			if (opl?.Entity?.Deleted != false || args.IsNullOrEmpty())
			{
				return;
			}

			using (var o = new ExtendedOPL(opl))
			{
				foreach (var a in args)
				{
					if (a is IEntity e)
					{
						o.Add(e);
					}
					else if (a != null)
					{
						o.Add(a.ToString());
					}
					else
					{
						o.Add(String.Empty);
					}
				}
			}
		}

		public static void AddTo(ObjectPropertyList opl, IEnumerable<object> args)
		{
			if (opl?.Entity?.Deleted != false || args == null)
			{
				return;
			}

			using (var o = new ExtendedOPL(opl))
			{
				foreach (var a in args)
				{
					if (a is IEntity e)
					{
						o.Add(e);
					}
					else if (a != null)
					{
						o.Add(a.ToString());
					}
					else
					{
						o.Add(String.Empty);
					}
				}
			}
		}

		public static void AddTo(ObjectPropertyList opl, string line, params object[] args)
		{
			if (opl?.Entity?.Deleted != false || line == null)
			{
				return;
			}

			using (var o = new ExtendedOPL(opl))
			{
				if (!args.IsNullOrEmpty())
				{
					o.Add(line, args);
				}
				else
				{
					o.Add(line);
				}
			}
		}

		public static void AddTo(ObjectPropertyList opl, string[] lines)
		{
			if (opl?.Entity?.Deleted != false || lines.IsNullOrEmpty())
			{
				return;
			}

			using (var o = new ExtendedOPL(opl))
			{
				o.AddRange(lines.Where(s => s != null));
			}
		}

		private List<string> _Buffer = ListPool<string>.AcquireObject();

		bool ICollection<string>.IsReadOnly => _Buffer == null;

		public int Count => _Buffer?.Count ?? 0;

		public string this[int index]
		{
			get => _Buffer?[index];
			set
			{
				if (_Buffer != null)
				{
					_Buffer[index] = value;
				}
			}
		}

		/// <summary>
		///     Gets or sets the underlying ObjectPropertyList
		/// </summary>
		public ObjectPropertyList Opl { get; set; }

		/// <summary>
		///		Gets or sets the number of lines each cliloc can hold
		/// </summary>
		public int LineBreak { get; set; } = ClilocBreak;

		public bool IsDisposed { get; private set; }

		/// <summary>
		///     Create with pre-defined OPL
		/// </summary>
		/// <param name="opl">ObjectPropertyList object to wrap and extend</param>
		public ExtendedOPL(ObjectPropertyList opl)
		{
			Opl = opl;
		}

		/// <summary>
		///     Create with pre-defined OPL
		/// </summary>
		/// <param name="opl">ObjectPropertyList object to wrap and extend</param>
		/// <param name="capacity">Capacity of the extension</param>
		public ExtendedOPL(ObjectPropertyList opl, int capacity)
			: this(opl)
		{
			_Buffer.Capacity = capacity;
		}

		/// <summary>
		///     Create with pre-defined OPL
		/// </summary>
		/// <param name="opl">ObjectPropertyList object to wrap and extend</param>
		/// <param name="list">Pre-defined list to append to the specified OPL</param>
		public ExtendedOPL(ObjectPropertyList opl, IEnumerable<string> list)
			: this(opl)
		{
			AddRange(list);
		}

		~ExtendedOPL()
		{
			Dispose(true);
		}

		void IDisposable.Dispose()
		{
			Dispose(false);
		}

		private void Dispose(bool gc)
		{
			if (IsDisposed)
			{
				return;
			}

			try
			{
				if (!gc)
				{
					GC.SuppressFinalize(this);
				}
			}
			catch
			{ }

			try
			{
				if (Opl != null)
				{
					Flush();

					Opl = null;
				}
			}
			catch
			{ }

			ObjectPool.Free(ref _Buffer);

			IsDisposed = true;
		}

		public bool NextEmpty(out ClilocInfo info)
		{
			info = null;

			if (IsDisposed || Opl == null)
			{
				return false;
			}

			for (int i = 0, index; i < EmptyClilocs.Length; i++)
			{
				index = EmptyClilocs[i];

				if (!Opl.Contains(index))
				{
					info = ClilocLNG.NULL.Lookup(index);

					if (info != null)
					{
						return true;
					}
				}
			}

			return false;
		}

		public void Flush()
		{
			if (IsDisposed)
			{
				return;
			}

			Apply();

			_Buffer?.Free(true);
		}

		/// <summary>
		///     Applies all changes to the underlying ObjectPropertyList
		/// </summary>
		public void Apply()
		{
			if (IsDisposed)
			{
				return;
			}

			if (_Buffer.IsNullOrEmpty())
			{
				return;
			}

			var opl = Opl;

			if (opl?.Entity?.Deleted != false)
			{
				Clear();
				return;
			}

			ObjectPool.Acquire(out StringBuilder final);

			int take, limit = LineBreak, threshold = ClilocThreshold;

			while (_Buffer.Count > 0)
			{
				if (!NextEmpty(out var info))
				{
					break;
				}

				if (!info.HasArgs || opl.Contains(info.Index))
				{
					continue;
				}

				try
				{
					take = 0;

					while (take < _Buffer.Count)
					{
						if (take > 0)
						{
							final.Append('\n');
						}

						if (!String.IsNullOrWhiteSpace(_Buffer[take]))
						{
							final.Append(_Buffer[take]);
						}
						else
						{
							final.Append(' ');
						}

						if (++take >= limit || final.Length >= threshold)
						{
							break;
						}
					}

					if (take == 0)
					{
						break;
					}

					_Buffer.RemoveRange(0, take);

					if (final.Length > 0)
					{
						opl.Add(info.Index, info.ToString(final));
					}
				}
				finally
				{
					final.Clear();
				}
			}

			ObjectPool.Free(ref final);

			Clear();
		}

		public void Add(string format, params object[] args)
		{
			if (_Buffer != null)
			{
				_Buffer.Add(String.Format(format ?? String.Empty, args));
			}
		}

		public void Add(int number, params object[] args)
		{
			if (_Buffer == null)
			{
				return;
			}

			var info = ClilocLNG.NULL.Lookup(number);

			if (info != null)
			{
				_Buffer.Add(info.ToString(args));
			}
		}

		public void Add(IEntity o)
		{
			if (o != null && Opl != null && o != Opl.Entity)
			{
				AddRange(o.GetOPLStrings());
			}
		}

		public void Add(IEntity o, Mobile viewer)
		{
			if (o != null && Opl != null && o != Opl.Entity)
			{
				AddRange(o.GetOPLStrings(viewer));
			}
		}

		public void Add(IEntity o, ClilocLNG lng)
		{
			if (o != null && Opl != null && o != Opl.Entity)
			{
				AddRange(o.GetOPLStrings(lng));
			}
		}

		public void Add(ObjectPropertyList opl)
		{
			if (opl != null && opl != Opl && opl.Entity != Opl.Entity)
			{
				AddRange(opl.DecodePropertyList());
			}
		}

		public void Add(ObjectPropertyList opl, Mobile viewer)
		{
			if (opl != null && opl != Opl && opl.Entity != Opl.Entity)
			{
				AddRange(opl.DecodePropertyList(viewer));
			}
		}

		public void Add(ObjectPropertyList opl, ClilocLNG lng)
		{
			if (opl != null && opl != Opl && opl.Entity != Opl.Entity)
			{
				AddRange(opl.DecodePropertyList(lng));
			}
		}

		public void AddRange(IEnumerable<string> lines)
		{
			_Buffer?.AddRange(lines.Select(line => line ?? String.Empty));
		}

		public void Add(string line)
		{
			_Buffer?.Add(line ?? String.Empty);
		}

		public bool Remove(string line)
		{
			return _Buffer?.Remove(line ?? String.Empty) == true;
		}

		public int RemoveAll(Predicate<string> match)
		{
			return _Buffer?.RemoveAll(match) ?? 0;
		}

		public void RemoveRange(int index, int count)
		{
			_Buffer?.RemoveRange(index, count);
		}

		public void RemoveAt(int index)
		{
			_Buffer?.RemoveAt(index);
		}

		public void Insert(int index, string line)
		{
			_Buffer?.Insert(index, line ?? String.Empty);
		}

		public void Insert(int index, string format, params object[] args)
		{
			_Buffer?.Insert(index, String.Format(format ?? String.Empty, args));
		}

		public void Insert(int index, int number, params object[] args)
		{
			_Buffer?.Insert(index, Clilocs.GetString(ClilocLNG.NULL, number, args));
		}

		public int IndexOf(string line)
		{
			return _Buffer?.IndexOf(line ?? String.Empty) ?? -1;
		}

		public bool Contains(string line)
		{
			return _Buffer?.Contains(line ?? String.Empty) == true;
		}

		public void Clear()
		{
			_Buffer?.Clear();
		}

		public void CopyTo(string[] array, int arrayIndex)
		{
			_Buffer?.CopyTo(array, arrayIndex);
		}

		public IEnumerator<string> GetEnumerator()
		{
			if (_Buffer != null)
			{
				return _Buffer.GetEnumerator();
			}

			return _EmptyBuffer.GetEnumerator<string>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
