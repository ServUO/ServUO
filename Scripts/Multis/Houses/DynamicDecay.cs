using System;

namespace Server.Multis
{
    public static class DynamicDecay
    {
		private static readonly DecayStageInfo m_DefaultLikeNew = new DecayStageInfo(TimeSpan.FromHours(1), TimeSpan.FromHours(1));
		private static readonly DecayStageInfo m_DefaultSlightly = new DecayStageInfo(TimeSpan.FromHours(1), TimeSpan.FromHours(2));
		private static readonly DecayStageInfo m_DefaultSomewhat = new DecayStageInfo(TimeSpan.FromHours(1), TimeSpan.FromHours(2));
		private static readonly DecayStageInfo m_DefaultFairly = new DecayStageInfo(TimeSpan.FromHours(1), TimeSpan.FromHours(2));
		private static readonly DecayStageInfo m_DefaultGreatly = new DecayStageInfo(TimeSpan.FromHours(1), TimeSpan.FromHours(2));
		private static readonly DecayStageInfo m_DefaultIDOC = new DecayStageInfo(TimeSpan.FromHours(12), TimeSpan.FromHours(24));

		[ConfigProperty("Housing.DynamicDecay")]
		public static bool Enabled { get => Config.Get("Housing.DynamicDecay", Core.ML); set => Config.Set("Housing.DynamicDecay", value); }

		[ConfigProperty("Housing.DynamicRangeLikeNew")]
		public static DecayStageInfo LevelLikeNew { get => Config.Get("Housing.DynamicRangeLikeNew", m_DefaultLikeNew); set => Config.Set("Housing.DynamicRangeLikeNew", value); }

		[ConfigProperty("Housing.DynamicRangeSlightly")]
		public static DecayStageInfo LevelSlightly { get => Config.Get("Housing.DynamicRangeSlightly", m_DefaultSlightly); set => Config.Set("Housing.DynamicRangeSlightly", value); }

		[ConfigProperty("Housing.DynamicRangeSomewhat")]
		public static DecayStageInfo LevelSomewhat { get => Config.Get("Housing.DynamicRangeSomewhat", m_DefaultSomewhat); set => Config.Set("Housing.DynamicRangeSomewhat", value); }

		[ConfigProperty("Housing.DynamicRangeFairly")]
		public static DecayStageInfo LevelFairly { get => Config.Get("Housing.DynamicRangeFairly", m_DefaultFairly); set => Config.Set("Housing.DynamicRangeFairly", value); }

		[ConfigProperty("Housing.DynamicRangeGreatly")]
		public static DecayStageInfo LevelGreatly { get => Config.Get("Housing.DynamicRangeGreatly", m_DefaultGreatly); set => Config.Set("Housing.DynamicRangeGreatly", value); }

		[ConfigProperty("Housing.DynamicRangeIDOC")]
		public static DecayStageInfo LevelIDOC { get => Config.Get("Housing.DynamicRangeIDOC", m_DefaultIDOC); set => Config.Set("Housing.DynamicRangeIDOC", value); }

        public static void Register(DecayLevel level, TimeSpan min, TimeSpan max)
        {
			switch (level)
			{
				case DecayLevel.LikeNew: LevelLikeNew = new DecayStageInfo(min, max); break;
				case DecayLevel.Slightly: LevelSlightly = new DecayStageInfo(min, max); break;
				case DecayLevel.Somewhat: LevelSomewhat = new DecayStageInfo(min, max); break;
				case DecayLevel.Fairly: LevelFairly = new DecayStageInfo(min, max); break;
				case DecayLevel.Greatly: LevelGreatly = new DecayStageInfo(min, max); break;
				case DecayLevel.IDOC: LevelIDOC = new DecayStageInfo(min, max); break;
			}
		}

        public static bool Decays(DecayLevel level)
		{
			return level >= DecayLevel.LikeNew && level <= DecayLevel.IDOC;
		}

        public static TimeSpan GetRandomDuration(DecayLevel level)
        {
			DecayStageInfo info;

			switch (level)
			{
				case DecayLevel.LikeNew: info = LevelLikeNew; break;
				case DecayLevel.Slightly: info = LevelSlightly; break;
				case DecayLevel.Somewhat: info = LevelSomewhat; break;
				case DecayLevel.Fairly: info = LevelFairly; break;
				case DecayLevel.Greatly: info = LevelGreatly; break;
				case DecayLevel.IDOC: info = LevelIDOC; break;
				default: return TimeSpan.Zero;
			}

			return Utility.RandomMinMax(info.MinDuration, info.MaxDuration);
        }
	}

	[Parsable, PropertyObject]
	public struct DecayStageInfo
	{
		public static readonly DecayStageInfo Empty = new DecayStageInfo(TimeSpan.Zero, TimeSpan.Zero);

		public static bool TryParse(string input, out DecayStageInfo value)
		{
			try
			{
				value = Parse(input);
				return true;
			}
			catch
			{
				value = Empty;
				return false;
			}
		}

		public static DecayStageInfo Parse(string input)
		{
			var i = input.IndexOf('(');

			if (i >= 0)
			{
				input = input.Substring(i + 1);

				i = input.IndexOf(')');

				if (i >= 0)
				{
					input = input.Substring(0, i);
				}
			}

			var args = input.Split('~');

			var min = TimeSpan.Parse(args[0]);
			var max = TimeSpan.Parse(args[1]);

			return new DecayStageInfo(min, max);
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public TimeSpan MinDuration { get; set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
		public TimeSpan MaxDuration { get; set; }

		public DecayStageInfo(TimeSpan min, TimeSpan max)
		{
			MinDuration = min;
			MaxDuration = max;
		}

		public override string ToString()
		{
			return $"{MinDuration}~{MaxDuration}";
		}
	}
}
