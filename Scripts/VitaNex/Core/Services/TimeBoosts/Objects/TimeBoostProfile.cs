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
using System.Linq;

using Server;
using Server.Accounting;
#endregion

namespace VitaNex.TimeBoosts
{
	public sealed class TimeBoostProfile : PropertyObject, IEnumerable<KeyValuePair<ITimeBoost, int>>
	{
		public IAccount Owner { get; private set; }

		private Dictionary<ITimeBoost, int> _Boosts = new Dictionary<ITimeBoost, int>();

		public int this[ITimeBoost key] { get => _Boosts.GetValue(key); set => _Boosts.Update(key, value); }

		public TimeSpan TotalTime => TimeSpan.FromTicks(_Boosts.Sum(kv => kv.Key.Value.Ticks * kv.Value));

		public TimeBoostProfile(IAccount owner)
		{
			Owner = owner;
		}

		public TimeBoostProfile(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			_Boosts.Clear();
		}

		public override void Reset()
		{
			_Boosts.Clear();
		}

		#region Credit
		public bool CanCredit(ITimeBoost b, int amount)
		{
			return b != null && amount >= 0 && (long)this[b] + amount <= Int32.MaxValue;
		}

		public bool Credit(ITimeBoost b, int amount)
		{
			if (CanCredit(b, amount))
			{
				this[b] += amount;
				return true;
			}

			return false;
		}

		public bool CreditHours(int value)
		{
			return Credit(value, 0);
		}

		public bool CreditMinutes(int value)
		{
			return Credit(0, value);
		}

		public bool Credit(int hours, int minutes)
		{
			return Credit(hours, minutes, false) && Credit(hours, minutes, true);
		}

		private bool Credit(int hours, int minutes, bool update)
		{
			return Credit(hours, minutes, update, out _, out _);
		}

		private bool Credit(int hours, int minutes, bool update, out int totalHours, out int totalMinutes)
		{
			totalHours = totalMinutes = 0;

			return (hours == 0 || Credit(hours, 0, update, out totalHours)) && (minutes == 0 || Credit(minutes, 1, update, out totalMinutes));
		}

		private bool Credit(int time, byte index, bool update, out int totalTime)
		{
			totalTime = 0;

			if (!TimeBoosts.Times.InBounds(index))
			{
				return false;
			}

			ITimeBoost k;
			int v, t = time;

			var i = TimeBoosts.Times[index].Length;

			while (--i >= 0)
			{
				k = TimeBoosts.Times[index][i];
				v = k.RawValue;

				if (time >= v && this[k] < Int32.MaxValue)
				{
					time -= v;
					totalTime += v;

					if (update)
					{
						++this[k];
					}
				}
			}

			return totalTime >= t;
		}
		#endregion Credit

		#region Consume
		public bool CanConsume(ITimeBoost b, int amount)
		{
			return b != null && amount >= 0 && this[b] >= amount;
		}

		public bool Consume(ITimeBoost b, int amount)
		{
			if (CanConsume(b, amount))
			{
				this[b] -= amount;
				return true;
			}

			return false;
		}

		public bool ConsumeHours(int value)
		{
			return Consume(value, 0);
		}

		public bool ConsumeMinutes(int value)
		{
			return Consume(0, value);
		}

		public bool Consume(int hours, int minutes)
		{
			return Consume(hours, minutes, false) && Consume(hours, minutes, true);
		}

		private bool Consume(int hours, int minutes, bool update)
		{
			return Consume(hours, minutes, update, out _, out _);
		}

		private bool Consume(int hours, int minutes, bool update, out int totalHours, out int totalMinutes)
		{
			totalHours = totalMinutes = 0;

			return (hours == 0 || Consume(hours, 0, update, out totalHours)) && (minutes == 0 || Consume(minutes, 1, update, out totalMinutes));
		}

		private bool Consume(int time, byte index, bool update, out int totalTime)
		{
			totalTime = 0;

			if (!TimeBoosts.Times.InBounds(index))
			{
				return false;
			}

			ITimeBoost k;
			int v, t = time;

			var i = TimeBoosts.Times[index].Length;

			while (--i >= 0)
			{
				k = TimeBoosts.Times[index][i];
				v = k.RawValue;

				if (time >= v && this[k] > 0)
				{
					time -= v;
					totalTime += v;

					if (update)
					{
						--this[k];
					}
				}
			}

			return totalTime >= t;
		}
		#endregion Consume

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<KeyValuePair<ITimeBoost, int>> GetEnumerator()
		{
			return _Boosts.GetEnumerator();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Owner);

			writer.WriteDictionary(_Boosts, (w, k, v) =>
			{
				w.Write(k);
				w.Write(v);
			});
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Owner = reader.ReadAccount();

			_Boosts = reader.ReadDictionary(r =>
			{
				var k = r.ReadTimeBoost();
				var v = r.ReadInt();

				return new KeyValuePair<ITimeBoost, int>(k, v);
			},
			_Boosts);

			_Boosts.RemoveValueRange(v => v <= 0);
		}
	}
}
