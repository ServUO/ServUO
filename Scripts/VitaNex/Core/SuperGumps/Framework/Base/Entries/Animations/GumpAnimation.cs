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

using Server.Gumps;
using Server.Network;

using VitaNex.Crypto;
#endregion

namespace VitaNex.SuperGumps
{
	public sealed class GumpAnimationBreak : SuperGumpEntry
	{
		private const string _Format = "{ animbreak }";

		private static readonly byte[] _Layout = Gump.StringToBuffer("animbreak");

		public override bool IgnoreModalOffset => true;

		public override string Compile()
		{
			return _Format;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(_Layout);
		}
	}

	public class GumpAnimation : SuperGumpEntry, IEquatable<GumpAnimation>
	{
		private static readonly Action<GumpAnimation> _EmptyHandler = anim => { };
		private static readonly GumpEntry[] _EmptyEntries = new GumpEntry[0];

		private static readonly byte[] _BeginLayout = Server.Gumps.Gump.StringToBuffer("{ ");
		private static readonly byte[] _EndLayout = Server.Gumps.Gump.StringToBuffer(" }");

		private const string _Format = "{{ anim {0} }}";

		private static readonly byte[] _Layout = Server.Gumps.Gump.StringToBuffer("anim");

		public override bool IgnoreModalOffset => true;

		private bool _Animated;

		public SuperGump Gump { get; private set; }
		public string UID { get; private set; }
		public string Name { get; private set; }
		public GumpAnimationState State { get; private set; }

		public Stack<GumpEntry> Entries { get; protected set; }
		public Action<GumpAnimation> Handler { get; protected set; }

		public object[] Args { get; set; }

		public GumpAnimation(
			SuperGump gump,
			string name,
			int take,
			long delay,
			long duration,
			bool repeat,
			bool wait,
			object[] args,
			Action<GumpAnimation> handler)
		{
			Gump = gump;
			Name = name ?? String.Empty;

			UID = String.Format("{{{0} {1} {2}}}", Name, Gump.Serial, Gump.User.Serial.Value);

			take = Math.Max(-1, Math.Min(Gump.Entries.Count, take));

			Entries = new Stack<GumpEntry>(take == -1 ? Gump.Entries.Count : take);

			if (take == -1 || take > 0)
			{
				var count = Gump.Entries.Count;

				while (--count >= 0)
				{
					if (count >= Gump.Entries.Count)
					{
						continue;
					}

					var e = Gump.Entries[count];

					if (e == null || e == this || e is GumpAnimation)
					{
						continue;
					}

					if (e is GumpAnimationBreak)
					{
						break;
					}

					Entries.Push(e);

					if (take != -1 && --take <= 0)
					{
						break;
					}
				}
			}

			Handler = handler ?? _EmptyHandler;

			UID = CryptoGenerator.GenString(CryptoHashType.MD5, UID);
			State = GumpAnimationState.Acquire(UID, delay, duration, repeat, wait);

			Args = args ?? new object[0];
		}

		public T GetArg<T>(int index, T def = default(T))
		{
			try
			{
				return (T)Args[index];
			}
			catch
			{
				return def;
			}
		}

		public void Reset()
		{
			_Animated = false;

			if (State != null)
			{
				State.Reset();
			}
		}

		public void Animate()
		{
			if (!_Animated && State != null && State.Animating)
			{
				_Animated = true;

				OnAnimate();

				if (Gump != null)
				{
					Gump.OnAnimate(this);
				}

				State.Animated();
			}
		}

		protected virtual void OnAnimate()
		{
			if (Handler != null)
			{
				Handler(this);
			}
		}

		public override sealed string Compile()
		{
			return String.Format(_Format, GetHashCode());
		}

		public override sealed void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(_Layout);
			disp.AppendLayout(GetHashCode());
		}

		public override int GetHashCode()
		{
			return UID.GetContentsHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is GumpAnimation && Equals((GumpAnimation)obj);
		}

		public virtual bool Equals(GumpAnimation anim)
		{
			return !ReferenceEquals(anim, null) && String.Equals(UID, anim.UID);
		}

		public override void Dispose()
		{
			State = null;
			Handler = null;

			Entries.Free(true);
			Entries = null;

			if (Gump == null || Gump.IsDisposed)
			{
				GumpAnimationState.Free(UID);
			}

			Gump = null;

			base.Dispose();
		}
	}
}