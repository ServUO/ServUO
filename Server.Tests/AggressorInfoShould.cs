using System;
using Xunit;

namespace Server.Tests
{
	public class AggressorInfoShould
	{
		[Fact]
		public void HaveInitialState()
		{
			Server.Misc.MapDefinitions.Configure();
			Server.World.Load();
			
			var attacker = new Mobile();
			var defender = new Mobile();

			var criminal = true;
			
			var sut = AggressorInfo.Create(attacker, defender, criminal);
			
			Assert.False(sut.Reported);
			Assert.Equal(criminal, sut.CriminalAggression);
			Assert.False(sut.Expired);
			Assert.Equal(criminal, sut.CanReportMurder);
			Assert.Equal(attacker, sut.Attacker);
			Assert.Equal(defender, sut.Defender); 
			Assert.InRange<DateTime>(sut.LastCombatTime, DateTime.UtcNow.AddSeconds(-5.0), DateTime.UtcNow);
		}

        [Fact]
        public void CreateTheSameInstanceAfterCallingFree()
		{
			Server.Misc.MapDefinitions.Configure();
			Server.World.Load();
			
			var attacker = new Mobile();
			var defender = new Mobile();
			
			var sut = AggressorInfo.Create(attacker, defender, true);
			
			var sut2 = AggressorInfo.Create(attacker, defender, true);
			
			sut.Free();
			
			var sut3 = AggressorInfo.Create(attacker, defender, true);
			
			Assert.NotSame(sut, sut2);
			Assert.Same(sut, sut3);
		}
	}
}