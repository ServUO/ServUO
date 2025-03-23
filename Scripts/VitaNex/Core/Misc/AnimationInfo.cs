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
#endregion

namespace Server
{
	[PropertyObject]
	public struct AnimationInfo : IEquatable<AnimationInfo>
	{
		public static readonly AnimationInfo Empty = new AnimationInfo(0, 0, 0, true, false, 0);

		[CommandProperty(AccessLevel.Counselor, true)]
		public int AnimID { get; private set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public int Frames { get; private set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public int Count { get; private set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public bool Forward { get; private set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public bool Repeat { get; private set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public int Delay { get; private set; }

		public AnimationInfo(int animID, int frames)
			: this(animID, frames, true)
		{ }

		public AnimationInfo(int animID, int frames, bool forward)
			: this(animID, frames, 1, forward, false, 0)
		{ }

		public AnimationInfo(int animID, int frames, int count, bool forward, bool repeat)
			: this(animID, frames, count, forward, repeat, 0)
		{ }

		public AnimationInfo(int animID, int frames, int count, bool forward, bool repeat, int delay)
			: this()
		{
			AnimID = animID;
			Frames = frames;
			Count = count;
			Forward = forward;
			Repeat = repeat;
			Delay = delay;
		}

		public AnimationInfo(GenericReader reader)
			: this()
		{
			Deserialize(reader);
		}

		public bool Animate(Mobile m)
		{
			if (m != null && !m.Deleted && this != Empty && AnimID > 0 && Frames > 0)
			{
				m.Animate(AnimID, Frames, Count, Forward, Repeat, Delay);
				return true;
			}

			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = AnimID;
				hash = (hash * 397) ^ Frames;
				hash = (hash * 397) ^ Count;
				hash = (hash * 397) ^ (Forward ? 1 : 0);
				hash = (hash * 397) ^ (Repeat ? 1 : 0);
				hash = (hash * 397) ^ Delay;
				return hash;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is AnimationInfo && Equals((AnimationInfo)obj);
		}

		public bool Equals(AnimationInfo other)
		{
			return AnimID.Equals(other.AnimID) && Frames.Equals(other.Frames) && Count.Equals(other.Count) &&
				   Forward.Equals(other.Forward) && Repeat.Equals(other.Repeat) && Delay.Equals(other.Delay);
		}

		public void Serialize(GenericWriter writer)
		{
			writer.SetVersion(0);

			writer.Write(AnimID);
			writer.Write(Frames);
			writer.Write(Count);
			writer.Write(Forward);
			writer.Write(Repeat);
			writer.Write(Delay);
		}

		public void Deserialize(GenericReader reader)
		{
			reader.GetVersion();

			AnimID = reader.ReadInt();
			Frames = reader.ReadInt();
			Count = reader.ReadInt();
			Forward = reader.ReadBool();
			Repeat = reader.ReadBool();
			Delay = reader.ReadInt();
		}

		#region Operators
		public static bool operator ==(AnimationInfo l, AnimationInfo r)
		{
			return l.Equals(r);
		}

		public static bool operator !=(AnimationInfo l, AnimationInfo r)
		{
			return !l.Equals(r);
		}

		public static bool operator >(AnimationInfo l, AnimationInfo r)
		{
			return l.AnimID > r.AnimID;
		}

		public static bool operator <(AnimationInfo l, AnimationInfo r)
		{
			return l.AnimID < r.AnimID;
		}

		public static bool operator >=(AnimationInfo l, AnimationInfo r)
		{
			return l.AnimID >= r.AnimID;
		}

		public static bool operator <=(AnimationInfo l, AnimationInfo r)
		{
			return l.AnimID <= r.AnimID;
		}

		public static bool operator ==(AnimationInfo l, int r)
		{
			return l.AnimID.Equals(r);
		}

		public static bool operator !=(AnimationInfo l, int r)
		{
			return !l.AnimID.Equals(r);
		}

		public static bool operator >(AnimationInfo l, int r)
		{
			return l.AnimID > r;
		}

		public static bool operator <(AnimationInfo l, int r)
		{
			return l.AnimID < r;
		}

		public static bool operator >=(AnimationInfo l, int r)
		{
			return l.AnimID >= r;
		}

		public static bool operator <=(AnimationInfo l, int r)
		{
			return l.AnimID <= r;
		}

		public static bool operator ==(int l, AnimationInfo r)
		{
			return l.Equals(r.AnimID);
		}

		public static bool operator !=(int l, AnimationInfo r)
		{
			return !l.Equals(r.AnimID);
		}

		public static bool operator >(int l, AnimationInfo r)
		{
			return l > r.AnimID;
		}

		public static bool operator <(int l, AnimationInfo r)
		{
			return l < r.AnimID;
		}

		public static bool operator >=(int l, AnimationInfo r)
		{
			return l >= r.AnimID;
		}

		public static bool operator <=(int l, AnimationInfo r)
		{
			return l <= r.AnimID;
		}
		#endregion Operators
	}
}