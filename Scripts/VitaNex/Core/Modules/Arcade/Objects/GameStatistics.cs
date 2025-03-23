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
	public sealed class GameStatistics : PropertyObject, IEnumerable<KeyValuePair<string, double>>
	{
		private readonly Dictionary<string, double> _Statistics = new Dictionary<string, double>();

		public double this[string key] { get => _Statistics.GetValue(key); set => _Statistics[key] = value; }

		public Dictionary<string, double>.KeyCollection Entries => _Statistics.Keys;

		public GameStatistics()
		{ }

		public GameStatistics(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			_Statistics.Clear();
		}

		public override void Reset()
		{
			_Statistics.Keys.ForEach(k => _Statistics[k] = 0);
		}

		public bool Remove(string key)
		{
			return key != null && _Statistics.Remove(key);
		}

		public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
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

			writer.SetVersion(0);

			writer.WriteDictionary(
				_Statistics,
				(w, k, v) =>
				{
					w.Write(k);
					w.Write(v);
				});
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			reader.ReadDictionary(
				r =>
				{
					var k = r.ReadString();
					var v = r.ReadDouble();

					return new KeyValuePair<string, double>(k, v);
				},
				_Statistics);
		}
	}
}