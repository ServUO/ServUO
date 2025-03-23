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
using System.Collections;
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Modules.Games
{
	public sealed class ArcadeProfile : PropertyObject, IEnumerable<KeyValuePair<string, GameStatistics>>
	{
		private Dictionary<string, GameStatistics> _Statistics = new Dictionary<string, GameStatistics>();

		public GameStatistics this[string key]
		{
			get => _Statistics.GetValue(key) ?? (_Statistics[key] = new GameStatistics());
			set => _Statistics[key] = value;
		}

		public Dictionary<string, GameStatistics>.KeyCollection Categories => _Statistics.Keys;

		public Mobile Owner { get; private set; }

		public int Credits { get; set; }

		public ArcadeProfile(Mobile owner)
		{
			Owner = owner;
		}

		public ArcadeProfile(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			foreach (var v in _Statistics.Values)
			{
				v.Clear();
			}

			_Statistics.Clear();
		}

		public override void Reset()
		{
			foreach (var v in _Statistics.Values)
			{
				v.Reset();
			}
		}

		public void Log(IGame g, string context, double value, bool offset)
		{
			if (offset)
			{
				if (value > 0)
				{
					LogIncrease(g, context, value);
				}
				else if (value < 0)
				{
					LogDecrease(g, context, value);
				}
			}
			else
			{
				this[g.Name][context] = value;
			}
		}

		public void LogIncrease(IGame g, string context, double value)
		{
			value = Math.Abs(value);

			this[g.Name][context] += value;
		}

		public void LogDecrease(IGame g, string context, double value)
		{
			value = Math.Abs(value);

			this[g.Name][context] -= value;
		}

		public IEnumerator<KeyValuePair<string, GameStatistics>> GetEnumerator()
		{
			return _Statistics.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			writer.Write(Owner);

			switch (version)
			{
				case 0:
				{
					writer.Write(Credits);

					writer.WriteBlockDictionary(
						_Statistics,
						(w, k, v) =>
						{
							w.Write(k);
							v.Serialize(w);
						});
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			Owner = reader.ReadMobile();

			switch (version)
			{
				case 0:
				{
					Credits = reader.ReadInt();

					_Statistics = reader.ReadBlockDictionary(
						r =>
						{
							var k = r.ReadString();
							var v = new GameStatistics(r);

							return new KeyValuePair<string, GameStatistics>(k, v);
						},
						_Statistics);
				}
				break;
			}
		}
	}
}