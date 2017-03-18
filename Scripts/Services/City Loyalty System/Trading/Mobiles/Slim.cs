using System;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Engines.Points;
using Server.Targeting;
using Server.Gumps;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;

namespace Server.Engines.CityLoyalty
{
	public class SlimTheFence : BaseCreature
	{
        public override bool IsInvulnerable { get { return true; } }

		[Constructable]
		public SlimTheFence() : base(AIType.AI_Vendor, FightMode.None, 10, 1, .4, .2)
		{
            Body = 0x190;
            SpeechHue = 0x3B2;
            Blessed = true;

            Name = "Slim";
            Title = "the Fence";

            HairItemID = 8264;
            FacialHairItemID = 8269;

            HairHue = 1932;
            FacialHairHue = 1932;
            Hue = Race.RandomSkinHue();

            SetStr(150);
            SetInt(50);
            SetDex(150);

            EquipItem(new JinBaori(1932));
            EquipItem(new FancyShirt(1932));
            EquipItem(new LongPants(1));

            var boots = new Boots();
            boots.Hue = 1;
            EquipItem(boots);

            CantWalk = true;
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			
			list.Add(new TurnInEntry(from, this));
		}
		
		private class TurnInEntry : ContextMenuEntry
		{
			public SlimTheFence Slim { get; private set; }
			public Mobile Player { get; private set; }
			
			public TurnInEntry(Mobile player, SlimTheFence slim) : base(1151729, 3) // Turn in a trade order
			{
				Player = player;
				Slim = slim;

                Enabled = CityTradeSystem.HasTrade(Player);
			}
			
			public override void OnClick()
			{
                if (CityTradeSystem.HasTrade(Player))
                {
                    Player.Target = new InternalTarget(Slim);
                    Player.SendLocalizedMessage(1151730); // Target the trade order you wish to turn in.
                }
			}
			
			private class InternalTarget : Target
			{
				public SlimTheFence Slim { get; private set; }
				
				public InternalTarget(SlimTheFence slim) : base(-1, false, TargetFlags.None)
				{
					Slim = slim;
				}
				
				protected override void OnTarget(Mobile from, object targeted)
				{
					TradeOrderCrate order = targeted as TradeOrderCrate;
					
					if(order != null)
					{
                        if (CityLoyaltySystem.CityTrading.TryTurnInToSlim(from, order, Slim))
                        {
                            from.AddToBackpack(Slim.GiveAward());
                            from.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.

                            Titles.AwardKarma(from, -100, true);
                        }
					}
					else
					{
						from.SendLocalizedMessage(1151731); // That is not a valid trade order. Please try again.
						from.Target = new InternalTarget(Slim);
					}
				}
			}
		}

        private Item GiveAward()
        {
            if (0.05 > Utility.RandomDouble())
            {
                switch (Utility.Random(4))
                {
                    case 0: return new ExpiredVoucher();
                    case 1: return new ForgedArtwork();
                    case 2: return new LittleBlackBook();
                    case 3: return new SlimsShadowVeil();
                }
            }

            switch (Utility.Random(3))
            {
                default:
                case 0: 
                    return new Skeletonkey();
                case 1:
                    Item item = Loot.RandomArmorOrShieldOrWeaponOrJewelry(false, false, true);

                    int min, max;
                    TreasureMapChest.GetRandomItemStat(out min, out max, 1.0);

                    RunicReforging.GenerateRandomItem(item, 0, min, max, this.Map);
                    return item;
                case 2:
                    return ScrollofTranscendence.CreateRandom(1, 10);
            }
        }

        public SlimTheFence(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
}