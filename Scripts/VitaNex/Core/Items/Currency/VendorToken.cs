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
#endregion

namespace VitaNex.Items
{
	public interface IVendorToken
	{
		string Name { get; set; }
		int Amount { get; set; }
		bool Stackable { get; set; }

		void Deserialize(GenericReader reader);
		void Serialize(GenericWriter writer);
	}

	public abstract class VendorToken : Item, IVendorToken
	{
		public override bool DisplayWeight => false;

		public VendorToken()
			: this(1)
		{ }

		public VendorToken(int amount)
			: base(0xEED)
		{
			Name = "Vendor Token";
			Hue = 85;
			Weight = 0;
			Stackable = true;
			Amount = Math.Max(1, Math.Min(60000, amount));
			LootType = LootType.Blessed;
		}

		public VendorToken(Serial serial)
			: base(serial)
		{ }

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

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}