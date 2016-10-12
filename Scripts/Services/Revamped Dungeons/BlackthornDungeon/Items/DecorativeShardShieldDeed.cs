#region References
using System;
using Server.Gumps;
using Server.Network;
#endregion

namespace Server.Items
{
    public class DecorativeShardShieldDeed : Item
    {
        public override int LabelNumber { get { return 1153729; } } // Deed for a Decorative Shard Shield

        [Constructable]
        public DecorativeShardShieldDeed()
            : base(0x14F0)
        {
            this.Weight = 1.0;
            this.LootType = LootType.Blessed;
        }

        public DecorativeShardShieldDeed(Serial serial)
            : base(serial)
        {
        }
        public override bool DisplayLootType
        {
            get
            {
                return Core.ML;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target
        {
            if (!this.IsChildOf(from.Backpack)) // Make sure its in their pack
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else
            {
                from.CloseGump(typeof(DecorativeShardShieldGump));
                from.SendGump(new DecorativeShardShieldGump(this));
            }
        }
    }

    public class DecorativeShardShieldGump : Gump
    {
        Mobile m_Mobile;
        Item m_Item;

        public DecorativeShardShieldGump(Item item) : base(0, 0)
        {
            m_Item = item;

            AddPage(0);

            AddBackground(50, 89, 508, 195, 2600);

            AddLabel(103, 114, 0, @"Choose from the following:");

            AddButton(92, 155, 1209, 1210, 1, GumpButtonType.Reply, 0);
            AddItem(75, 178, 25472);

            AddButton(133, 155, 1209, 1210, 2, GumpButtonType.Reply, 0);
            AddItem(119, 178, 25473);

            AddButton(177, 155, 1209, 1210, 3, GumpButtonType.Reply, 0);
            AddItem(165, 182, 25474);

            AddButton(217, 155, 1209, 1210, 4, GumpButtonType.Reply, 0);
            AddItem(205, 182, 25475);

            AddButton(267, 155, 1209, 1210, 5, GumpButtonType.Reply, 0);
            AddItem(220, 133, 25476);

            AddButton(333, 155, 1209, 1210, 6, GumpButtonType.Reply, 0);
            AddItem(272, 133, 25477);

            AddButton(388, 155, 1209, 1210, 7, GumpButtonType.Reply, 0);
            AddItem(374, 178, 25478);

            AddButton(426, 155, 1209, 1210, 8, GumpButtonType.Reply, 0);
            AddItem(413, 175, 25479);

            AddButton(480, 155, 1209, 1210, 9, GumpButtonType.Reply, 0);
            AddItem(463, 176, 25480);

            //

            AddButton(92, 255, 1209, 1210, 10, GumpButtonType.Reply, 0);
            AddItem(75, 178, 25481);

            AddButton(133, 255, 1209, 1210, 11, GumpButtonType.Reply, 0);
            AddItem(119, 178, 25482);

            AddButton(177, 255, 1209, 1210, 12, GumpButtonType.Reply, 0);
            AddItem(165, 182, 25483);

            AddButton(217, 255, 1209, 1210, 13, GumpButtonType.Reply, 0);
            AddItem(205, 182, 25484);

            AddButton(267, 255, 1209, 1210, 14, GumpButtonType.Reply, 0);
            AddItem(220, 133, 25485);

            AddButton(333, 255, 1209, 1210, 15, GumpButtonType.Reply, 0);
            AddItem(272, 133, 25486);

            AddButton(388, 255, 1209, 1210, 16, GumpButtonType.Reply, 0);
            AddItem(374, 178, 25487);

            AddButton(426, 255, 1209, 1210, 17, GumpButtonType.Reply, 0);
            AddItem(413, 175, 25488);

            AddButton(480, 255, 1209, 1210, 18, GumpButtonType.Reply, 0);
            AddItem(463, 176, 25489);

            //

            AddButton(92, 355, 1209, 1210, 19, GumpButtonType.Reply, 0);
            AddItem(75, 178, 25490);

            AddButton(133, 355, 1209, 1210, 20, GumpButtonType.Reply, 0);
            AddItem(119, 178, 25491);

            AddButton(177, 355, 1209, 1210, 21, GumpButtonType.Reply, 0);
            AddItem(165, 182, 25492);

            AddButton(217, 355, 1209, 1210, 22, GumpButtonType.Reply, 0);
            AddItem(205, 182, 25493);

            AddButton(267, 355, 1209, 1210, 23, GumpButtonType.Reply, 0);
            AddItem(220, 133, 25494);

            AddButton(333, 355, 1209, 1210, 24, GumpButtonType.Reply, 0);
            AddItem(272, 133, 25495);

            AddButton(388, 355, 1209, 1210, 25, GumpButtonType.Reply, 0);
            AddItem(374, 178, 25496);

            AddButton(426, 355, 1209, 1210, 26, GumpButtonType.Reply, 0);
            AddItem(413, 175, 25497);

            AddButton(480, 355, 1209, 1210, 27, GumpButtonType.Reply, 0);
            AddItem(463, 176, 25498);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        from.PlaceInBackpack(new AOLLegendsShield());
                        m_Item.Delete();
                        break;
                    }
                case 2:
                    {
                        from.PlaceInBackpack(new ArirangShield());
                        m_Item.Delete();
                        break;
                    }
                case 3:
                    {
                        from.PlaceInBackpack(new AsukaShield());
                        m_Item.Delete();
                        break;
                    }
                case 4:
                    {
                        from.PlaceInBackpack(new AlanticShield());
                        m_Item.Delete();
                        break;
                    }
                case 5:
                    {
                        from.PlaceInBackpack(new BajaShield());
                        m_Item.Delete();
                        break;
                    }
                case 6:
                    {
                        from.PlaceInBackpack(new BalhaeShield());
                        m_Item.Delete();
                        break;
                    }
                case 7:
                    {
                        from.PlaceInBackpack(new CatskillsShield());
                        m_Item.Delete();
                        break;
                    }
                case 8:
                    {
                        from.PlaceInBackpack(new ChesapeakeShield());
                        m_Item.Delete();
                        break;
                    }
                case 9:
                    {
                        from.PlaceInBackpack(new DrachenfelsShield());
                        m_Item.Delete();
                        break;
                    }
                case 10:
                    {
                        from.PlaceInBackpack(new EuropaShield());
                        m_Item.Delete();
                        break;
                    }
                case 11:
                    {
                        from.PlaceInBackpack(new FormosaShield());
                        m_Item.Delete();
                        break;
                    }
                case 12:
                    {
                        from.PlaceInBackpack(new GreatLakesShield());
                        m_Item.Delete();
                        break;
                    }
                case 13:
                    {
                        from.PlaceInBackpack(new HokutoShield());
                        m_Item.Delete();
                        break;
                    }
                case 14:
                    {
                        from.PlaceInBackpack(new IzumoShield());
                        m_Item.Delete();
                        break;
                    }
                case 15:
                    {
                        from.PlaceInBackpack(new LakeAustinShield());
                        m_Item.Delete();
                        break;
                    }
                case 16:
                    {
                        from.PlaceInBackpack(new LakeSuperiorShield());
                        m_Item.Delete();
                        break;
                    }
                case 17:
                    {
                        from.PlaceInBackpack(new MizuhoShield());
                        m_Item.Delete();
                        break;
                    }
                case 18:
                    {
                        from.PlaceInBackpack(new MugenShield());
                        m_Item.Delete();
                        break;
                    }
                case 19:
                    {
                        from.PlaceInBackpack(new NapaValleyShield());
                        m_Item.Delete();
                        break;
                    }
                case 20:
                    {
                        from.PlaceInBackpack(new OceaniaShield());
                        m_Item.Delete();
                        break;
                    }
                case 21:
                    {
                        from.PlaceInBackpack(new OrginShield());
                        m_Item.Delete();
                        break;
                    }
                case 22:
                    {
                        from.PlaceInBackpack(new PacificShield());
                        m_Item.Delete();
                        break;
                    }
                case 23:
                    {
                        from.PlaceInBackpack(new SakuraShield());
                        m_Item.Delete();
                        break;
                    }
                case 24:
                    {
                        from.PlaceInBackpack(new SiegePerilousShield());
                        m_Item.Delete();
                        break;
                    }
                case 25:
                    {
                        from.PlaceInBackpack(new SonomaShield());
                        m_Item.Delete();
                        break;
                    }
                case 26:
                    {
                        from.PlaceInBackpack(new WakokuShield());
                        m_Item.Delete();
                        break;
                    }
                case 27:
                    {
                        from.PlaceInBackpack(new YamatoShield());
                        m_Item.Delete();
                        break;
                    }
            }
        }
    }
}