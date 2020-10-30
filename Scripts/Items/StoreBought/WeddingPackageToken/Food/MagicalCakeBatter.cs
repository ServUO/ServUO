using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class MagicalCakeBatter : Item
    {
        public override int LabelNumber => 1157336; // Magical Cake Batter

        [Constructable]
        public MagicalCakeBatter()
            : base(0x103F)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(CakeGump));
                from.SendGump(new CakeGump(this));
            }
        }

        public MagicalCakeBatter(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class CakeGump : Gump
    {
        readonly Item _Item;

        public CakeGump(Item item)
            : base(0, 0)
        {
            _Item = item;

            AddPage(0);

            AddBackground(50, 89, 250, 195, 2600);

            AddHtmlLocalized(103, 114, 200, 20, 1157335, false, false); // Please make a selection:

            AddButton(92, 155, 1209, 1210, 1, GumpButtonType.Reply, 0);
            AddItem(75, 178, 0x9EB0);

            AddButton(133, 155, 1209, 1210, 2, GumpButtonType.Reply, 0);
            AddItem(119, 178, 0x9EB1);

            AddButton(177, 155, 1209, 1210, 3, GumpButtonType.Reply, 0);
            AddItem(165, 182, 0x9EB2);

            AddButton(217, 155, 1209, 1210, 4, GumpButtonType.Reply, 0);
            AddItem(205, 182, 0x9EB3);

            AddButton(267, 155, 1209, 1210, 5, GumpButtonType.Reply, 0);
            AddItem(220, 133, 0x9EB4);

            AddButton(333, 155, 1209, 1210, 6, GumpButtonType.Reply, 0);
            AddItem(272, 133, 0x9EB5);

            AddButton(388, 155, 1209, 1210, 7, GumpButtonType.Reply, 0);
            AddItem(374, 178, 0x9EB6);

            AddButton(426, 155, 1209, 1210, 8, GumpButtonType.Reply, 0);
            AddItem(413, 175, 0x9ED4);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (_Item == null || _Item.Deleted)
                return;            

            switch (info.ButtonID)
            {
                case 0:
                    {
                        return;
                    }
                case 1:
                    {
                        from.AddToBackpack(new WeddingCake(0x9EB0));
                        break;
                    }
                case 2:
                    {
                        from.AddToBackpack(new WeddingCake(0x9EB1));
                        break;
                    }
                case 3:
                    {
                        from.AddToBackpack(new WeddingCake(0x9EB2));
                        break;
                    }
                case 4:
                    {
                        from.AddToBackpack(new WeddingCake(0x9EB3));
                        break;
                    }
                case 5:
                    {
                        from.AddToBackpack(new WeddingCake(0x9EB4));
                        break;
                    }
                case 6:
                    {
                        from.AddToBackpack(new WeddingCake(0x9EB5));
                        break;
                    }
                case 7:
                    {
                        from.AddToBackpack(new WeddingCake(0x9EB6));
                        break;
                    }
                case 8:
                    {
                        from.AddToBackpack(new WeddingCake(0x9ED4));
                        break;
                    }
            }

            _Item.Delete();
        }
    }
}
