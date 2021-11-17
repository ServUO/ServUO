using System;

namespace Server
{
    public static class ShardSettings
	{
		[CallPriority(Int32.MinValue)]
		public static void Configure()
		{
			Core.OnExpansionChanged += Invalidate;
			
			Invalidate();
		}

		public static void Invalidate()
		{
			if (Core.AOS)
			{
				Mobile.AOSStatusHandler = AOS.GetStatus;
			}
			else
			{
				Mobile.AOSStatusHandler = null;
			}
		}
    }
}
