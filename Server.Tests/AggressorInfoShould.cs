using System;
using Xunit;

namespace Server.Tests
{
	public class AggressorInfoShould : IClassFixture<LoadWorldFixture>
	{
		private readonly Mobile _Attacker;
		private readonly Mobile _Defender;

		public AggressorInfoShould()
		{
			_Attacker = new Mobile();
			_Defender = new Mobile();
		}

		[Fact]
		public void HaveInitialState()
		{
			const bool criminal = true;

			var sut = AggressorInfo.Create(_Attacker, _Defender, criminal);

			Assert.False(sut.Reported);
			Assert.Equal(criminal, sut.CriminalAggression);
			Assert.False(sut.Expired);
			Assert.Equal(criminal, sut.CanReportMurder);
			Assert.Equal(_Attacker, sut.Attacker);
			Assert.Equal(_Defender, sut.Defender);
			// ReSharper disable once RedundantTypeArgumentsOfMethod
			Assert.InRange<DateTime>(sut.LastCombatTime, DateTime.UtcNow.AddSeconds(-5.0), DateTime.UtcNow);
		}

		[Fact]
		public void CreateTheSameInstanceAfterCallingFree()
		{
			var sut = AggressorInfo.Create(_Attacker, _Defender, true);
			var sut2 = AggressorInfo.Create(_Attacker, _Defender, true);

			sut.Free();

			var sut3 = AggressorInfo.Create(_Attacker, _Defender, true);

			Assert.NotSame(sut, sut2);
			Assert.Same(sut, sut3);
		}
	}

	// ReSharper disable once ClassNeverInstantiated.Global
	public class LoadWorldFixture : IDisposable
	{
		public LoadWorldFixture()
		{
			Misc.MapDefinitions.Configure();
			World.Load();
		}

		public void Dispose() { }
	}
}