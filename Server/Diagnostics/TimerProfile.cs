#region References
using System.Collections.Generic;
using System.IO;
#endregion

namespace Server.Diagnostics
{
	public class TimerProfile : BaseProfile
	{
		private static readonly Dictionary<string, TimerProfile> _profiles = new Dictionary<string, TimerProfile>();

		public static IEnumerable<TimerProfile> Profiles => _profiles.Values;

		public static TimerProfile Acquire(string name)
		{
			if (!Core.Profiling)
			{
				return null;
			}


			if (!_profiles.TryGetValue(name, out TimerProfile prof))
			{
				_profiles.Add(name, prof = new TimerProfile(name));
			}

			return prof;
		}

		public long Created { get; set; }

		public long Started { get; set; }

		public long Stopped { get; set; }

		public TimerProfile(string name)
			: base(name)
		{ }

		public override void WriteTo(TextWriter op)
		{
			base.WriteTo(op);

			op.Write("\t{0,12:N0} {1,12:N0} {2,-12:N0}", Created, Started, Stopped);
		}
	}
}
