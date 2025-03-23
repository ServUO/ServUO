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

using ReqFlags = VitaNex.Modules.WebStats.WebStatsRequestFlags;
#endregion

namespace VitaNex.Modules.WebStats
{
	public class WebStatsOptions : CoreModuleOptions
	{
		[CommandProperty(WebStats.Access)]
		public TimeSpan UpdateInterval { get; set; }

		[CommandProperty(WebStats.Access, true)]
		public ReqFlags RequestFlags { get; private set; }

		[CommandProperty(WebStats.Access)]
		public bool DisplayServer
		{
			get => GetRequestFlag(ReqFlags.Server);
			set => SetRequestFlag(ReqFlags.Server, value);
		}

		[CommandProperty(WebStats.Access)]
		public bool DisplayStats
		{
			get => GetRequestFlag(ReqFlags.Stats);
			set => SetRequestFlag(ReqFlags.Stats, value);
		}

		private bool GetRequestFlag(ReqFlags flag)
		{
			return RequestFlags.HasFlag(flag);
		}

		private void SetRequestFlag(ReqFlags flag, bool value)
		{
			switch (flag)
			{
				case ReqFlags.None:
					RequestFlags = value ? flag : ReqFlags.All;
					return;
				case ReqFlags.All:
					RequestFlags = value ? flag : ReqFlags.None;
					return;
			}

			if (value)
			{
				RequestFlags |= flag;
			}
			else
			{
				RequestFlags &= ~flag;
			}
		}

		public WebStatsOptions()
			: base(typeof(WebStats))
		{
			RequestFlags = ReqFlags.Server | ReqFlags.Stats;
		}

		public WebStatsOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			base.Clear();

			UpdateInterval = TimeSpan.Zero;
			RequestFlags = ReqFlags.None;
		}

		public override void Reset()
		{
			base.Reset();

			UpdateInterval = TimeSpan.FromSeconds(30.0);
			RequestFlags = ReqFlags.Server | ReqFlags.Stats;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				case 0:
				{
					writer.Write(UpdateInterval);
					writer.WriteFlag(RequestFlags);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
				case 0:
				{
					if (version < 1)
					{
						reader.ReadShort();
						reader.ReadInt();
					}

					UpdateInterval = reader.ReadTimeSpan();
					RequestFlags = reader.ReadFlag<ReqFlags>();
				}
				break;
			}
		}
	}
}