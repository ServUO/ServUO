using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Mobiles
{
    public class HarborMaster : BaseCreature
    {
        public static Dictionary<Mobile, Timer> _Table = new Dictionary<Mobile, Timer>();

        [Constructable]
        public HarborMaster()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            InitStats(31, 41, 51);

            SetSkill(SkillName.Mining, 36, 68);

            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();
            Blessed = true;

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                Title = "the Harbor Mistress";
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                Title = "the Harbor Master";
            }

            AddItem(new Shirt(Utility.RandomDyedHue()));
            AddItem(new Boots());
            AddItem(new LongPants(Utility.RandomNeutralHue()));
            AddItem(new QuarterStaff());

            Utility.AssignRandomHair(this);
        }

        public HarborMaster(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => false;
        public override bool ClickTitle => false;

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.AddCustomContextEntries(from, list);

            if (from.Alive)
            {
                list.Add(new ShipRecallRuneEntry(from, this));

                if (BaseBoat.HasBoat(from) && !_Table.ContainsKey(from))
                {
                    list.Add(new AbandonShipEntry(from, this));
                }
            }
        }

        private class ShipRecallRuneEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly Mobile m_Vendor;

            public ShipRecallRuneEntry(Mobile from, Mobile vendor)
                : base(1149570, 6)
            {
                m_From = from;
                m_Vendor = vendor;
            }

            private static readonly Type[] m_ShipTypes = new Type[]
            {
                typeof(TokunoGalleon),  typeof(GargishGalleon),
                typeof(OrcishGalleon),  typeof(BritannianShip)
            };

            private static bool IsSpecialShip(BaseBoat b)
            {
                return m_ShipTypes.Any(t => t == b.GetType());
            }

            public override void OnClick()
            {
                if (m_Vendor == null || m_Vendor.Deleted)
                    return;

                BaseBoat boat = BaseBoat.GetBoat(m_From);

                if (boat != null)
                {
                    if (Banker.Withdraw(m_From, 100, true))
                    {
                        if (IsSpecialShip(boat))
                        {
                            RecallRune newRune = new RecallRune();
                            newRune.SetGalleon((BaseGalleon)boat);
                            m_From.AddToBackpack(newRune);
                            m_Vendor.Say(1149580); // A recall rune to your ship has been placed in your backpack.
                        }
                        else
                        {
                            KeyType[] Types = Enum.GetValues(typeof(KeyType)).Cast<KeyType>().ToArray();
                            Key packKey = new Key(Types[Utility.Random(Types.Length)], boat.PPlank.KeyValue, boat)
                            {
                                MaxRange = 10,
                                Name = "a ship key"
                            };

                            m_From.AddToBackpack(packKey);
                        }
                    }
                    else
                    {
                        m_Vendor.Say(500192); // Begging thy pardon, but thou canst not afford that.
                    }
                }
                else
                {
                    m_Vendor.Say(1116767); // The ship could not be located.
                }
            }
        }

        private class AbandonShipEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly Mobile m_Vendor;

            public AbandonShipEntry(Mobile from, Mobile vendor)
                : base(1150110, 6)
            {
                m_From = from;
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                if (m_Vendor == null || m_Vendor.Deleted)
                    return;

                m_From.SendGump(new AbandonGump());
            }
        }

        private class AbandonGump : Gump
        {
            public AbandonGump()
                : base(250, 200)
            {
                AddPage(0);

                AddBackground(0, 0, 240, 142, 0x13BE);
                AddImageTiled(6, 6, 228, 100, 0xA40);
                AddImageTiled(6, 116, 228, 20, 0xA40);
                AddAlphaRegion(6, 6, 228, 142);
                AddHtmlLocalized(8, 8, 228, 100, 1150112, 0x7FFF, false, false); // WARNING: Your ship and all items aboard it or in its cargo hold will be deleted if you continue. Make certain you wish to abandon your ship before proceeding. Are you sure you wish to abandon your ship?
                AddButton(114, 116, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(148, 118, 450, 20, 1073996, 0x7FFF, false, false); // ACCEPT
                AddButton(6, 116, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 118, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                switch (info.ButtonID)
                {
                    case 0:
                        {
                            from.SendLocalizedMessage(1042021); // Cancelled.
                            break;
                        }
                    case 1:
                        {
                            BaseBoat boat = BaseBoat.GetBoat(from);

                            if (boat != null && !_Table.ContainsKey(from))
                            {
                                _Table[from] = new AbandonTimer(from, boat);
                                from.SendLocalizedMessage(1150111); // Your ship has been abandoned. It will decay within five minutes.
                            }

                            break;
                        }
                }
            }
        }

        private class AbandonTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly BaseBoat m_Boat;
            private readonly DateTime m_End;

            public AbandonTimer(Mobile m, BaseBoat boat)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_Boat = boat;
                m_End = DateTime.UtcNow + TimeSpan.FromMinutes(5.0);

                Start();
            }

            protected override void OnTick()
            {
                if (m_Boat == null || m_Mobile == null || m_Boat.Map == Map.Internal)
                {
                    if (m_Mobile != null && _Table.ContainsKey(m_Mobile))
                    {
                        _Table.Remove(m_Mobile);
                    }

                    Stop();
                }
                else if (DateTime.UtcNow > m_End)
                {
                    if (m_Mobile != null && _Table.ContainsKey(m_Mobile))
                    {
                        _Table.Remove(m_Mobile);
                    }

                    m_Boat.Delete();
                    Stop();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
