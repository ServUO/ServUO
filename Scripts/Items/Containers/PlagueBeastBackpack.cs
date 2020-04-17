using System.Collections.Generic;

namespace Server.Items
{
    public class PlagueBeastBackpack : BaseContainer
    {
        private static readonly int[,,] m_Positions = new int[,,]
        {
            { { 275, 85 }, { 360, 111 }, { 375, 184 }, { 332, 228 }, { 141, 105 }, { 189, 75 } },
            { { 274, 34 }, { 327, 89 }, { 354, 168 }, { 304, 225 }, { 113, 86 }, { 189, 75 } },
            { { 276, 79 }, { 369, 117 }, { 372, 192 }, { 336, 230 }, { 141, 116 }, { 189, 75 } },
        };
        private static readonly int[] m_BrainHues = new int[]
        {
            0x2B, 0x42, 0x54, 0x60
        };
        public PlagueBeastBackpack()
            : base(0x261B)
        {
            Layer = Layer.Backpack;
        }

        public PlagueBeastBackpack(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultMaxWeight => 0;
        public override int DefaultMaxItems => 0;
        public override int DefaultGumpID => 0x2A63;
        public override int DefaultDropSound => 0x23F;
        public void Initialize()
        {
            AddInnard(0x1CF6, 0x0, 227, 128);
            AddInnard(0x1D10, 0x0, 251, 128);
            AddInnard(0x1FBE, 0x21, 240, 83);

            AddInnard(new PlagueBeastHeart(), 229, 104);

            AddInnard(0x1D06, 0x0, 283, 91);
            AddInnard(0x1FAF, 0x21, 315, 107);
            AddInnard(0x1FB9, 0x21, 289, 87);
            AddInnard(0x9E7, 0x21, 304, 96);
            AddInnard(0x1B1A, 0x66D, 335, 102);
            AddInnard(0x1D10, 0x0, 338, 146);
            AddInnard(0x1FB3, 0x21, 358, 167);
            AddInnard(0x1D0B, 0x0, 357, 155);
            AddInnard(0x9E7, 0x21, 339, 184);
            AddInnard(0x1B1A, 0x66D, 157, 172);
            AddInnard(0x1D11, 0x0, 147, 157);
            AddInnard(0x1FB9, 0x21, 121, 131);
            AddInnard(0x9E7, 0x21, 166, 176);
            AddInnard(0x1D0B, 0x0, 122, 138);
            AddInnard(0x1D0D, 0x0, 118, 150);
            AddInnard(0x1FB3, 0x21, 97, 123);
            AddInnard(0x1D08, 0x0, 115, 113);
            AddInnard(0x9E7, 0x21, 109, 109);
            AddInnard(0x9E7, 0x21, 91, 122);
            AddInnard(0x9E7, 0x21, 94, 160);
            AddInnard(0x1B19, 0x66D, 170, 121);
            AddInnard(0x1FAF, 0x21, 161, 111);
            AddInnard(0x1D0B, 0x0, 158, 112);
            AddInnard(0x9E7, 0x21, 159, 101);
            AddInnard(0x1D10, 0x0, 132, 177);
            AddInnard(0x1D0E, 0x0, 110, 178);
            AddInnard(0x1FB3, 0x21, 95, 194);
            AddInnard(0x1FAF, 0x21, 154, 203);
            AddInnard(0x1B1A, 0x66D, 110, 237);
            AddInnard(0x9E7, 0x21, 111, 171);
            AddInnard(0x9E7, 0x21, 90, 197);
            AddInnard(0x9E7, 0x21, 166, 205);
            AddInnard(0x9E7, 0x21, 96, 242);
            AddInnard(0x1D10, 0x0, 334, 196);
            AddInnard(0x1D0B, 0x0, 322, 270);

            List<PlagueBeastOrgan> organs = new List<PlagueBeastOrgan>();
            PlagueBeastOrgan organ;

            for (int i = 0; i < 6; i++)
            {
                int random = Utility.Random(3);

                if (i == 5)
                    random = 0;

                switch (random)
                {
                    default:
                    case 0:
                        organ = new PlagueBeastRockOrgan();
                        break;
                    case 1:
                        organ = new PlagueBeastMaidenOrgan();
                        break;
                    case 2:
                        organ = new PlagueBeastRubbleOrgan();
                        break;
                }

                organs.Add(organ);
                AddInnard(organ, m_Positions[random, i, 0], m_Positions[random, i, 1]);
            }

            organ = new PlagueBeastBackupOrgan();
            organs.Add(organ);
            AddInnard(organ, 129, 214);

            for (int i = 0; i < m_BrainHues.Length; i++)
            {
                int random = Utility.Random(organs.Count);
                organ = organs[random];
                organ.BrainHue = m_BrainHues[i];
                organs.RemoveAt(random);
            }

            organs.Clear();

            AddInnard(new PlagueBeastMainOrgan(), 240, 161);
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (dropped is PlagueBeastInnard || dropped is PlagueBeastGland)
                return base.TryDropItem(from, dropped, sendFullMessage);

            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (IsAccessibleTo(from) && (item is PlagueBeastInnard || item is PlagueBeastGland))
            {
                Rectangle2D ir = ItemBounds.Table[item.ItemID];
                int x, y;
                int cx = p.X + ir.X + ir.Width / 2;
                int cy = p.Y + ir.Y + ir.Height / 2;

                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    PlagueBeastComponent innard = Items[i] as PlagueBeastComponent;

                    if (innard != null)
                    {
                        Rectangle2D r = ItemBounds.Table[innard.ItemID];

                        x = innard.X + r.X;
                        y = innard.Y + r.Y;

                        if (cx >= x && cx <= x + r.Width && cy >= y && cy <= y + r.Height)
                        {
                            innard.OnDragDrop(from, item);
                            break;
                        }
                    }
                }

                return base.OnDragDropInto(from, item, p);
            }

            return false;
        }

        public void AddInnard(int itemID, int hue, int x, int y)
        {
            AddInnard(new PlagueBeastInnard(itemID, hue), x, y);
        }

        public void AddInnard(PlagueBeastInnard innard, int x, int y)
        {
            AddItem(innard);
            innard.Location = new Point3D(x, y, 0);
            innard.Map = Map;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}