using System;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Engines.Points;
using Server.Targeting;
using Server.Gumps;
using Server.Network;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;

namespace Server.Engines.CityLoyalty
{
	public class TradeMinister : BaseCreature
	{
        [CommandProperty(AccessLevel.GameMaster)]
		public City City { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltySystem CitySystem { get { return CityLoyaltySystem.GetCityInstance(City); } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityItemDonation DonationCrate { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityPetDonation DonationPost { get; set; }

        public override bool IsInvulnerable { get { return true; } }

		[Constructable]
		public TradeMinister(City city) : base(AIType.AI_Vendor, FightMode.None, 10, 1, .4, .2)
		{
			City = city;
            SpeechHue = 0x3B2;
            Female = Utility.RandomDouble() > 0.75;
            Blessed = true;

            Name = Female ? NameList.RandomName("female") : NameList.RandomName("male");
            Title = "the minister of trade";

            Body = Female ? 0x191 : 0x190;
            HairItemID = Race.RandomHair(Female);
            FacialHairItemID = Race.RandomFacialHair(Female);
            HairHue = Race.RandomHairHue();
            Hue = Race.RandomSkinHue();

            SetStr(150);
            SetInt(50);
            SetDex(150);

            EquipItem(new Cloak(1157));
            EquipItem(new BodySash(1326));

            var chest = new ChainChest();
            chest.Hue = 1908;
            EquipItem(chest);

            var boots = new ThighBoots();
            boots.Hue = 1908;
            EquipItem(boots);

            EquipItem(new GoldRing());

			Ministers.Add(this);

            CantWalk = true;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if(from.InRange(this.Location, 4))
			{
				from.SendGump(new InternalGump());
			}
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			
			list.Add(new TradeOrderEntry(from, this));
			list.Add(new TurnInEntry(from, this));
		}
		
		private class TradeOrderEntry : ContextMenuEntry
		{
			public TradeMinister Minister { get; private set; }
			public Mobile Player { get; private set; }
			
			public TradeOrderEntry(Mobile player, TradeMinister minister) : base(1114453, 5) // Get Trade Order
			{
				Player = player;
				Minister = minister;
				
				Enabled = !CityTradeSystem.HasTrade(Player);
			}
			
			public override void OnClick()
			{
                if (!CityTradeSystem.HasTrade(Player))
                {
                    Player.SendGump(new InternalTradeOrderGump(Player as PlayerMobile, Minister));
                }
			}
		}
		
		private class TurnInEntry : ContextMenuEntry
		{
			public TradeMinister Minister { get; private set; }
			public Mobile Player { get; private set; }
			
			public TurnInEntry(Mobile player, TradeMinister minister) : base(1151729, 3) // Turn in a trade order
			{
				Player = player;
				Minister = minister;

                Enabled = CityTradeSystem.HasTrade(Player);
			}
			
			public override void OnClick()
			{
                if (CityTradeSystem.HasTrade(Player))
                {
                    Player.Target = new InternalTarget(Minister);
                    Player.SendLocalizedMessage(1151730); // Target the trade order you wish to turn in.
                }
			}
			
			private class InternalTarget : Target
			{
				public TradeMinister Minister { get; private set; }
				
				public InternalTarget(TradeMinister minister) : base(-1, false, TargetFlags.None)
				{
					Minister = minister;
				}
				
				protected override void OnTarget(Mobile from, object targeted)
				{
					TradeOrderCrate order = targeted as TradeOrderCrate;
					
					if(order != null)
					{
                        if (CityLoyaltySystem.CityTrading.TryTurnIn(from, order, Minister))
                        {
                            if (order.Entry != null && order.Entry.Distance > 0)
                            {
                                from.AddToBackpack(Minister.GiveReward(order.Entry));
                                from.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                            }

                            Titles.AwardKarma(from, 100, true);
                        }
					}
					else
					{
						from.SendLocalizedMessage(1151731); // That is not a valid trade order. Please try again.
						from.Target = new InternalTarget(Minister);
					}
				}
			}
		}

        private Item GiveReward(TradeEntry entry)
        {
            if (0.01 > Utility.RandomDouble())
            {
                switch (Utility.Random(2))
                {
                    default:
                    case 0: return CityBook.GetRandom();
                    case 1: return new RewardSign();
                }
            }
            else
            {
                switch (Utility.Random(2))
                {
                    default:
                    case 1: return RandomResource(entry);
                    case 2: return ScrollofTranscendence.CreateRandom(1, 10);
                }
            }
        }

        private Item RandomResource(TradeEntry entry)
        {
            int amount = 40;

            if (entry.Details.Count > 1)
                amount = 40 + Utility.RandomMinMax(10, entry.Details.Count * 20);

            switch (Utility.Random(4))
            {
                case 0:
                    switch (Utility.Random(9))
                    {
                        case 0: return new IronIngot(amount);
                        case 1: return new DullCopperIngot(amount);
                        case 2: return new ShadowIronIngot(amount);
                        case 3: return new CopperIngot(amount);
                        case 4: return new BronzeIngot(amount);
                        case 5: return new GoldIngot(amount);
                        case 6: return new AgapiteIngot(amount);
                        case 7: return new VeriteIngot(amount);
                        case 8: return new ValoriteIngot(amount);
                    }
                    break;
                case 1:
                    switch (Utility.Random(4))
                    {
                        case 0: return new Leather(amount);
                        case 1: return new SpinedLeather(amount);
                        case 2: return new HornedLeather(amount);
                        case 3: return new BarbedLeather(amount);
                    }
                    break;
                case 2:
                    switch (Utility.Random(7))
                    {
                        case 0: return new Board(amount);
                        case 1: return new OakBoard(amount);
                        case 2: return new AshBoard(amount);
                        case 3: return new YewBoard(amount);
                        case 4: return new BloodwoodBoard(amount);
                        case 5: return new HeartwoodBoard(amount);
                        case 6: return new FrostwoodBoard(amount);
                    }
                    break;
                case 3:
                    Item item = Loot.Construct(SkillHandlers.Imbuing.IngredTypes[Utility.Random(SkillHandlers.Imbuing.IngredTypes.Length)]);

                    amount /= 10;

                    if (item != null && item.Stackable)
                        item.Amount = amount;

                    return item;
            }
            return null;
        }
		
		private class InternalGump : Gump
		{
			public InternalGump() : base(40, 40)
			{
                AddBackground(0, 0, 500, 400, 9380);

                AddHtmlLocalized(30, 50, 400, 16, 1152962, 0x4800, false, false);	// City Trade Minister
                AddHtmlLocalized(30, 80, 440, 320, 1152963, 0x001F, false, false);
			}
		}
		
		public class InternalTradeOrderGump : Gump
		{
			public TradeMinister Minister { get; private set; }
            public PlayerMobile User { get; set; }

            public InternalTradeOrderGump(PlayerMobile user, TradeMinister minister)
                : base(40, 40)
			{
				Minister = minister;
                User = user;

                AddGumpLayout();
			}
			
			public void AddGumpLayout()
			{
				AddBackground(0, 0, 450, 550, 9300);

                AddHtmlLocalized(0, 30, 450, 20, 1154645, "#1114454", 0x4800, false, false); // An Offer For a trade Order
				AddHtmlLocalized(10, 70, 430, 250, 1114455, 0x001F, false, false);
				AddHtmlLocalized(10, 250, 430, 250, 1151721, 0x4800, false, false);

                AddButton(10, 518, 4005, 4007, 1, GumpButtonType.Reply, 0);
				AddHtmlLocalized(50, 518, 60, 16, 1049011, 0x001F, false, false); // I Accept!
				
				AddButton(410, 518, 4020, 4022, 0, GumpButtonType.Reply, 0);
				AddHtmlLocalized(360, 518, 60, 16, 1152889, 0x4800, false, false); // Cancel
			}

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info.ButtonID == 1)
                {
                    if (!Minister.InRange(User, 5))
                    {
                        User.SendLocalizedMessage(500295); // You are too far away to do that.
                    }
                    else
                    {
                        CityLoyaltySystem.CityTrading.TryOfferTrade(User, Minister);
                    }
                }
            }
		}
		
		public TradeMinister(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			writer.Write((int)City);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			City = (City)reader.ReadInt();

            if (CitySystem != null)
                CitySystem.Minister = this;

			Ministers.Add(this);
		}
		
		public static void Configure()
		{
			Ministers = new List<TradeMinister>();
		}
		
		public static List<TradeMinister> Ministers { get; private set; }
	}
}