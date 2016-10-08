using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using Server.ContextMenus;
using System.Globalization;
using Server.Gumps;
using System.Linq;

namespace Server.Engines.CityLoyalty
{
	public class GuardCaptain : BaseCreature
	{
        [CommandProperty(AccessLevel.GameMaster)]
		public City City { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BoxOfRopes Box { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextShout { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltySystem CitySystem { get { return CityLoyaltySystem.GetCityInstance(City); } set { } }

        public override bool IsInvulnerable { get { return true; } }

        private Dictionary<PlayerMobile, DateTime> _BannerCooldown;

		[Constructable]
		public GuardCaptain(City city) : base(AIType.AI_Vendor, FightMode.None, 10, 1, 0.2, 0.4)
		{
			City = city;
            Female = Utility.RandomDouble() > 0.75;
            Blessed = true;

            Name = Female ? NameList.RandomName("female") : NameList.RandomName("male");
            Title = "the guard captain";

            Body = Female ? 0x191 : 0x190;
            HairItemID = Race.RandomHair(Female);
            FacialHairItemID = Race.RandomFacialHair(Female);
            HairHue = Race.RandomHairHue();
            Hue = Race.RandomSkinHue();

            SetStr(150);
            SetInt(50);
            SetDex(150);

            SetWearable(new ShortPants(1508));

            if (Female)
                SetWearable(new FemaleStuddedChest());
            else
                SetWearable(new PlateChest());

            SetWearable(new BodySash(1326));
            SetWearable(new Halberd());

            CantWalk = true;
		}

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (CitySystem != null && m is PlayerMobile && InRange(m.Location, 2))
            {
                Raider raider = ((PlayerMobile)m).AllFollowers.FirstOrDefault(mob => mob is Raider && mob.InRange(this.Location, 2)) as Raider;

                if (raider != null)
                {
                    CitySystem.AwardLove(m, 1000);

                    SayTo(m, 1152250, m.Name); // Thank you, ~1_name~, for your assistance during these difficult times.
                    raider.Delete();
                }
            }

            base.OnMovement(m, oldLocation);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> entries)
        {
            base.GetContextMenuEntries(from, entries);

            if(from is PlayerMobile)
                entries.Add(new InternalEntry(from as PlayerMobile, this));
        }

        public class InternalEntry : ContextMenuEntry
        {
            public PlayerMobile Player { get; set; }
            public GuardCaptain Guard { get; set; }

            public InternalEntry(PlayerMobile from, GuardCaptain guard)
                : base(1152366, 3) // City Banner
            {
                Player = from;
                Guard = guard;

                Enabled = CityLoyaltySystem.HasCitizenship(from, Guard.City);
            }

            public override void OnClick()
            {
                CityLoyaltySystem thisSystem = CityLoyaltySystem.GetCityInstance(Guard.City);
                CityLoyaltySystem theirSystem = CityLoyaltySystem.GetCitizenship(Player);

                Guard.CheckBannerCooldown();

                if (theirSystem != null && thisSystem != null && CityLoyaltySystem.HasCitizenship(Player, Guard.City))
                {
                    if(Guard.IsInBannerCooldown(Player))
                        Guard.SayTo(Player, 1152364, String.Format("#{0}", CityLoyaltySystem.BannerLocalization(thisSystem.City))); // I have quite a backlog of orders and I cannot satisfy your request for a ~1_ITEM~ right now.
                    if (theirSystem.GetLoyaltyRating(Player) < LoyaltyRating.Adored)
                        Guard.SayTo(Player, 1152363, String.Format("#{0}", CityLoyaltySystem.GetCityLocalization(thisSystem.City))); // I apologize, but you are not well-enough renowned in the city of ~1_CITY~ to make this purchase.
                    else
                    {
                        string args = String.Format("#{0}\t{1}", CityLoyaltySystem.BannerLocalization(thisSystem.City), CityLoyaltySystem.BannerCost.ToString("N0", CultureInfo.GetCultureInfo("en-US")));
				        Player.SendGump(new ConfirmCallbackGump(Player, 1049004, 1152365, Player, args, Guard.OnConfirm)); 
                    }
                }
            }
        }
		
		private void OnConfirm(Mobile m, object o)
		{
            PlayerMobile pm = m as PlayerMobile;

			if(pm != null)
			{
                if (Banker.Withdraw(pm, CityLoyaltySystem.BannerCost))
				{
					CityBannerDeed deed = new CityBannerDeed(City);
                    CitySystem.AddToTreasury(m, CityLoyaltySystem.BannerCost);

					if(pm.Backpack == null || !pm.Backpack.TryDropItem(pm, deed, false))
					{
						pm.BankBox.DropItem(deed);
						pm.SendMessage("The deed has been placed in your bank box.");
					}
					else
						pm.SendMessage("The deed has been placed in your backpack.");

                    AddToCooldown(pm);
				}
				else
					SayTo(pm, 1152302); // I am afraid your bank box does not contain the funds needed to complete this transaction.
			}
		}

        public void AddToCooldown(PlayerMobile pm)
        {
            if (_BannerCooldown == null)
                _BannerCooldown = new Dictionary<PlayerMobile, DateTime>();

            _BannerCooldown[pm] = DateTime.UtcNow + TimeSpan.FromHours(CityLoyaltySystem.BannerCooldownDuration);
        }

        public bool IsInBannerCooldown(PlayerMobile m)
        {
            if (_BannerCooldown != null && m is PlayerMobile)
            {
                if (_BannerCooldown.ContainsKey((PlayerMobile)m))
                    return _BannerCooldown[m] > DateTime.UtcNow;
            }

            return false;
        }

        public void CheckBannerCooldown()
        {
            if (_BannerCooldown == null || _BannerCooldown.Count == 0)
                return;

            List<PlayerMobile> list = new List<PlayerMobile>(_BannerCooldown.Keys);

            foreach (PlayerMobile pm in list)
            {
                if (_BannerCooldown[pm] < DateTime.UtcNow)
                    _BannerCooldown.Remove(pm);
            }

            list.Clear();
            list.TrimExcess();
        }

        public override void OnThink()
        {
            base.OnThink();

            if (NextShout < DateTime.UtcNow)
            {
                Say(Utility.Random(1152252, 6));
                NextShout = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(45, 65));
            }
        }

		public GuardCaptain(Serial serial) : base(serial)
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
			int version = reader.ReadInt();
			
			City = (City)reader.ReadInt();

            if (CitySystem != null)
                CitySystem.Captain = this;
		}
	}
}