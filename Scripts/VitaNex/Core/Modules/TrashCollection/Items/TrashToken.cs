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
using Server.Mobiles;

using VitaNex.Items;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public sealed class TrashToken : Item, IVendorToken, ITrashTokenProperties
	{
		public override bool DisplayWeight => false;

		[Constructable]
		public TrashToken()
			: this(1)
		{ }

		[Constructable]
		public TrashToken(int amount)
			: base(0xEED)
		{
			Name = "Trash Token";
			Hue = 85;
			Weight = 0;

			LootType = LootType.Blessed;

			Stackable = true;
			Amount = Math.Max(1, Math.Min(60000, amount));
		}

		public TrashToken(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{ }
				break;
				case 0:
				{ }
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
				{ }
				break;
				case 0:
				{
					ItemID = 0xEED;
				}
				break;
			}
		}

		public override DeathMoveResult OnInventoryDeath(Mobile parent)
		{
			if (parent is BaseCreature)
			{
				return DeathMoveResult.MoveToCorpse;
			}

			return base.OnInventoryDeath(parent);
		}

		public override int GetDropSound()
		{
			if (Amount <= 1)
			{
				return 0x2E4;
			}

			if (Amount <= 5)
			{
				return 0x2E5;
			}

			return 0x2E6;
		}
	}
}