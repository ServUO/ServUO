#region Header
// **********
// ServUO - TargetProfile.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
#endregion

namespace Server.Diagnostics
{
	public class TargetProfile : BaseProfile
	{
		private static readonly Dictionary<Type, TargetProfile> _profiles = new Dictionary<Type, TargetProfile>();

		public static IEnumerable<TargetProfile> Profiles { get { return _profiles.Values; } }

		public static TargetProfile Acquire(Type type)
		{
			if (!Core.Profiling)
			{
				return null;
			}

			TargetProfile prof;

			if (!_profiles.TryGetValue(type, out prof))
			{
				_profiles.Add(type, prof = new TargetProfile(type));
			}

			return prof;
		}

		public TargetProfile(Type type)
			: base(type.FullName)
		{ }
	}
}