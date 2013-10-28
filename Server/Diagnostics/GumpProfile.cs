#region Header
// **********
// ServUO - GumpProfile.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Diagnostics
{
	public class GumpProfile : BaseProfile
	{
		private static readonly Dictionary<Type, GumpProfile> _profiles = new Dictionary<Type, GumpProfile>();

		public static IEnumerable<GumpProfile> Profiles { get { return _profiles.Values; } }

		public static GumpProfile Acquire(Type type)
		{
			if (!Core.Profiling)
			{
				return null;
			}

			GumpProfile prof;

			if (!_profiles.TryGetValue(type, out prof))
			{
				_profiles.Add(type, prof = new GumpProfile(type));
			}

			return prof;
		}

		public GumpProfile(Type type)
			: base(type.FullName)
		{ }
	}
}