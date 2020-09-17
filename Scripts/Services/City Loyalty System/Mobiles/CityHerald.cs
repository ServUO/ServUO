using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Engines.CityLoyalty
{
    public class CityHerald : BaseCreature
    {
        public City City { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltySystem CitySystem { get { return CityLoyaltySystem.GetCityInstance(City); } set { } }

        public override bool IsInvulnerable => true;

        private string _Announcement;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Announcement
        {
            get
            {
                return _Announcement;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    AnnouncementExpires = DateTime.MinValue;

                    if (Timer != null)
                    {
                        Timer.Stop();
                        Timer = null;
                    }
                }

                if (value != _Announcement)
                {
                    AnnouncementExpires = DateTime.UtcNow + TimeSpan.FromHours(CityLoyaltySystem.AnnouncementPeriod);

                    if (Timer != null)
                        Timer.Stop();

                    Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), DoAnnouncement);
                    Timer.Start();
                }

                _Announcement = value;

                if (_Announcement != null)
                    DoAnnouncement();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime AnnouncementExpires { get; set; }

        public Timer Timer { get; set; }

        [Constructable]
        public CityHerald(City city) : base(AIType.AI_Vendor, FightMode.None, 10, 1, .4, .2)
        {
            City = city;
            SpeechHue = 0x3B2;
            Female = Utility.RandomDouble() > 0.75;
            Blessed = true;

            Name = Female ? NameList.RandomName("female") : NameList.RandomName("male");
            Title = "the city herald";

            Body = Female ? 0x191 : 0x190;
            HairItemID = Race.RandomHair(Female);
            FacialHairItemID = Race.RandomFacialHair(Female);
            HairHue = Race.RandomHairHue();
            Hue = Race.RandomSkinHue();

            SetStr(150);
            SetInt(50);
            SetDex(150);

            EquipItem(new Doublet(1157));
            EquipItem(new FancyShirt(1908));
            EquipItem(new FeatheredHat(1157));
            EquipItem(new LongPants(1908));

            Boots boots = new Boots
            {
                Hue = 2012
            };
            EquipItem(boots);

            EquipItem(new GoldRing());

            Frozen = true;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new DonateGoldEntry(from, this));
        }

        private class DonateGoldEntry : ContextMenuEntry
        {
            public CityHerald Herald { get; private set; }
            public Mobile Player { get; private set; }

            public DonateGoldEntry(Mobile player, CityHerald herald) : base(1156237, 3) // Donate Gold
            {
                Player = player;
                Herald = herald;

                Enabled = player.InRange(herald.Location, 5);
            }

            public override void OnClick()
            {
                if (Player.Prompt != null)
                {
                    Player.SendLocalizedMessage(1079166); // You already have a text entry request pending.
                }
                else
                {
                    Player.SendLocalizedMessage(1155865); // Enter amount to deposit:
                    Player.BeginPrompt(
                        (from, text) =>
                        {
                            int amount = Utility.ToInt32(text);

                            if (amount > 0)
                            {
                                if (Banker.Withdraw(from, amount, true))
                                {
                                    CityLoyaltySystem.GetCityInstance(Herald.City).AddToTreasury(from, amount, true);

                                    Herald.SayTo(from, 1152926); // The City thanks you for your generosity!
                                }
                                else
                                    from.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                            }
                            else
                                from.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                        },
                        (from, text) =>
                        {
                            from.SendLocalizedMessage(1155867); // The amount entered is invalid. Verify that there are sufficient funds to complete this transaction.
                        });
                }
            }
        }

        public void DoAnnouncement()
        {
            if (!string.IsNullOrEmpty(_Announcement))
            {
                PublicOverheadMessage(MessageType.Regular, 0x3B2, 502976); // Hear ye! Hear ye!
                Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                    {
                        PublicOverheadMessage(MessageType.Regular, 0x3B2, false, _Announcement);
                    });
            }

            if (DateTime.UtcNow > AnnouncementExpires)
            {
                AnnouncementExpires = DateTime.MinValue;

                if (Timer != null)
                {
                    Timer.Stop();
                    Timer = null;
                }
            }
        }

        public CityHerald(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
			
            writer.Write((int)City);
            writer.Write(_Announcement);
            writer.Write(AnnouncementExpires);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
			Frozen = true;
			
            City = (City)reader.ReadInt();
            _Announcement = reader.ReadString();
            AnnouncementExpires = reader.ReadDateTime();

            if (CitySystem != null)
                CitySystem.Herald = this;

            if (DateTime.UtcNow < AnnouncementExpires)
            {
                Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), DoAnnouncement);
                Timer.Start();
            }

        }
    }
}