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
using System.Drawing;

using Server;
using Server.Commands;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.EquipmentSets
{
	public sealed class EquipmentSetsOptions : CoreModuleOptions
	{
		private string _AdminCommand = String.Empty;

		[CommandProperty(EquipmentSets.Access)]
		public string AdminCommand
		{
			get => _AdminCommand;
			set => CommandUtility.Replace(_AdminCommand ?? value, EquipmentSets.Access, HandleAdminCommand, (_AdminCommand = value));
		}

		[CommandProperty(EquipmentSets.Access)]
		public KnownColor SetNameColor { get; set; }

		[CommandProperty(EquipmentSets.Access)]
		public KnownColor PartNameColor { get; set; }

		[CommandProperty(EquipmentSets.Access)]
		public KnownColor ModNameColor { get; set; }

		[CommandProperty(EquipmentSets.Access)]
		public KnownColor InactiveColor { get; set; }

		public Color SetNameColorRaw => Color.FromKnownColor(SetNameColor);
		public Color PartNameColorRaw => Color.FromKnownColor(PartNameColor);
		public Color ModNameColorRaw => Color.FromKnownColor(ModNameColor);
		public Color InactiveColorRaw => Color.FromKnownColor(InactiveColor);

		public EquipmentSetsOptions()
			: base(typeof(EquipmentSets))
		{
			SetNameColor = KnownColor.Gold;
			PartNameColor = KnownColor.LawnGreen;
			ModNameColor = KnownColor.SkyBlue;
			InactiveColor = KnownColor.Transparent;

			AdminCommand = "EquipSets";
		}

		public EquipmentSetsOptions(GenericReader reader)
			: base(reader)
		{ }

		public void HandleAdminCommand(CommandEventArgs e)
		{
			if (e.Mobile != null && !e.Mobile.Deleted && e.Mobile is PlayerMobile)
			{
				SuperGump.Send(new EquipmentSetsAdminUI((PlayerMobile)e.Mobile));
			}
		}

		public override void Clear()
		{
			SetNameColor = KnownColor.White;
			PartNameColor = KnownColor.White;
			ModNameColor = KnownColor.White;
			InactiveColor = KnownColor.Transparent;

			AdminCommand = "EquipSets";
		}

		public override void Reset()
		{
			SetNameColor = KnownColor.LightGoldenrodYellow;
			PartNameColor = KnownColor.LawnGreen;
			ModNameColor = KnownColor.SkyBlue;
			InactiveColor = KnownColor.Transparent;

			AdminCommand = "EquipSets";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteFlag(SetNameColor);
					writer.WriteFlag(PartNameColor);
					writer.WriteFlag(ModNameColor);
					writer.WriteFlag(InactiveColor);

					writer.Write(AdminCommand);
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
				case 0:
				{
					SetNameColor = reader.ReadFlag<KnownColor>();
					PartNameColor = reader.ReadFlag<KnownColor>();
					ModNameColor = reader.ReadFlag<KnownColor>();
					InactiveColor = reader.ReadFlag<KnownColor>();

					AdminCommand = reader.ReadString();
				}
				break;
			}
		}
	}
}