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
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex
{
	public class PlayerNamesOptions : CoreServiceOptions
	{
		private bool _IgnoreCase;

		[CommandProperty(PlayerNames.Access)]
		public virtual bool IgnoreCase
		{
			get => _IgnoreCase;
			set
			{
				if (_IgnoreCase && !value)
				{
					PlayerNames.Registry.Comparer.Impl = EqualityComparer<string>.Default;
				}
				else if (!_IgnoreCase && value)
				{
					PlayerNames.Registry.Comparer.Impl = StringComparer.OrdinalIgnoreCase;
				}
				else
				{
					return;
				}

				_IgnoreCase = value;

				if (PlayerNames.Registry.Count > 0)
				{
					PlayerNames.Clear();
					PlayerNames.Index();
				}
			}
		}

		[CommandProperty(PlayerNames.Access)]
		public virtual bool IndexOnStart { get; set; }

		[CommandProperty(PlayerNames.Access)]
		public virtual bool NameSharing { get; set; }

		public PlayerNamesOptions()
			: base(typeof(PlayerNames))
		{
			IndexOnStart = false;
			NameSharing = true;
			IgnoreCase = false;
		}

		public PlayerNamesOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			base.Clear();

			IndexOnStart = false;
			NameSharing = true;
			IgnoreCase = false;
		}

		public override void Reset()
		{
			base.Reset();

			IndexOnStart = false;
			NameSharing = true;
			IgnoreCase = false;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(2);

			switch (version)
			{
				case 2:
					writer.Write(IgnoreCase);
					goto case 1;
				case 1:
					writer.Write(NameSharing);
					goto case 0;
				case 0:
					writer.Write(IndexOnStart);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 2:
					IgnoreCase = reader.ReadBool();
					goto case 1;
				case 1:
					NameSharing = reader.ReadBool();
					goto case 0;
				case 0:
					IndexOnStart = reader.ReadBool();
					break;
			}
		}
	}
}