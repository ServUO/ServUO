using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class MagicalBotanicalArchwaySeed : Item
    {
        public override int LabelNumber => 1157337; // Magical Botanical Archway Seed

        [Constructable]
        public MagicalBotanicalArchwaySeed()
            : base(0x0DCF)
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(FlowerArchesGump));
                from.SendGump(new FlowerArchesGump(this));
            }
        }

        public MagicalBotanicalArchwaySeed(Serial serial)
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

    public class FlowerArchesGump : Gump
    {
        readonly Item _Item;

        public FlowerArchesGump(Item item)
            : base(0, 0)
        {
            _Item = item;

            AddPage(0);

            AddBackground(50, 89, 250, 195, 2600);

            AddHtmlLocalized(103, 114, 200, 20, 1157335, false, false); // Please make a selection:

            AddButton(92, 155, 1209, 1210, 1, GumpButtonType.Reply, 0);
            AddItem(75, 178, 0x9EFD);

            AddButton(153, 155, 1209, 1210, 2, GumpButtonType.Reply, 0);
            AddItem(139, 178, 0x9EFE);

            AddButton(217, 155, 1209, 1210, 3, GumpButtonType.Reply, 0);
            AddItem(205, 182, 0x9EFF);
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
                        from.AddToBackpack(new FloweredTrellisDeed(TrellisType.Leafy));
                        break;
                    }
                case 2:
                    {
                        from.AddToBackpack(new FloweredTrellisDeed(TrellisType.Rose));
                        break;
                    }
                case 3:
                    {
                        from.AddToBackpack(new FloweredTrellisDeed(TrellisType.Bigflow));
                        break;
                    }
            }

            _Item.Delete();
        }
    }
}
