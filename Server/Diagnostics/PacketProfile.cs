#region Header
// **********
// ServUO - PacketProfile.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
#endregion

namespace Server.Diagnostics
{
	public abstract class BasePacketProfile : BaseProfile
	{
		private long _totalLength;

		public long TotalLength { get { return _totalLength; } }

		public double AverageLength { get { return (double)_totalLength / Math.Max(1, Count); } }

		protected BasePacketProfile(string name)
			: base(name)
		{ }

		public void Finish(int length)
		{
			Finish();

			_totalLength += length;
		}

		public override void WriteTo(TextWriter op)
		{
			base.WriteTo(op);

			op.Write("\t{0,12:F2} {1,-12:N0}", AverageLength, TotalLength);
		}
	}

	public class PacketSendProfile : BasePacketProfile
	{
		private static readonly Dictionary<Type, PacketSendProfile> _profiles = new Dictionary<Type, PacketSendProfile>();

		public static IEnumerable<PacketSendProfile> Profiles { get { return _profiles.Values; } }

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static PacketSendProfile Acquire(Type type)
		{
			PacketSendProfile prof;

			if (!_profiles.TryGetValue(type, out prof))
			{
				_profiles.Add(type, prof = new PacketSendProfile(type));
			}

			return prof;
		}

		private long _created;

		public void Increment()
		{
			Interlocked.Increment(ref _created);
		}

		public PacketSendProfile(Type type)
			: base(type.FullName)
		{ }

		public override void WriteTo(TextWriter op)
		{
			base.WriteTo(op);

			op.Write("\t{0,12:N0}", _created);
		}
	}

	public class PacketReceiveProfile : BasePacketProfile
	{
		private static readonly Dictionary<int, PacketReceiveProfile> _profiles = new Dictionary<int, PacketReceiveProfile>();

		public static IEnumerable<PacketReceiveProfile> Profiles { get { return _profiles.Values; } }

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static PacketReceiveProfile Acquire(int packetId)
		{
			PacketReceiveProfile prof;

			if (!_profiles.TryGetValue(packetId, out prof))
			{
				_profiles.Add(packetId, prof = new PacketReceiveProfile(packetId));
			}

			return prof;
		}

		public PacketReceiveProfile(int packetId)
			: base(String.Format("0x{0:X2}", packetId))
		{ }
	}
}