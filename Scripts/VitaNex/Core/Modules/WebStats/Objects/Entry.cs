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

using Server;
#endregion

namespace VitaNex.Modules.WebStats
{
	public sealed class WebStatsEntry
	{
		public SimpleType Data { get; set; }

		public bool Persist { get; set; }

		public object Value { get => Data.Value; set => Data = new SimpleType(value); }

		public WebStatsEntry(SimpleType value, bool persist)
		{
			Data = value;
			Persist = persist;
		}

		public WebStatsEntry(GenericReader reader)
		{
			Deserialize(reader);
		}

		public bool TryCast<T>(out T value)
		{
			return Data.TryCast(out value);
		}

		public T Cast<T>()
			where T : struct
		{
			return Data.Cast<T>();
		}

		public void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Persist);

					if (Persist)
					{
						Data.Serialize(writer);
					}
				}
				break;
			}
		}

		public void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					Persist = reader.ReadBool();

					if (Persist)
					{
						Data = new SimpleType(reader);
					}
				}
				break;
			}
		}
	}
}