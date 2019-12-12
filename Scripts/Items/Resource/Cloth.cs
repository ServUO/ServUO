using System;
using Server.Network;

namespace Server.Items
{
    [FlipableAttribute(0x1766, 0x1768)]
    public class Cloth : Item, IScissorable, IDyable, ICommodity
    {
        [Constructable]
        public Cloth()
            : this(1)
        {
        }

        [Constructable]
        public Cloth(int amount)
            : base(0x1766)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Cloth(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }
        TextDefinition ICommodity.Description
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
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

        public override void OnSingleClick(Mobile from)
        {
            int number = (this.Amount == 1) ? 1049124 : 1049123;

            from.Send(new MessageLocalized(this.Serial, this.ItemID, MessageType.Regular, 0x3B2, 3, number, "", this.Amount.ToString()));
        }

        public bool Scissor(Mobile from, Scissors scissors)
        {
            if (this.Deleted || !from.CanSee(this))
                return false;

            base.ScissorHelper(from, new Bandage(), 1);

            return true;
        }
    }

    public class CutUpCloth : Item
    {
        public override int LabelNumber { get { return 1044458; } } // cut-up cloth

        [Constructable]
        public CutUpCloth()
            : base(0x1767)
        {
        }

        public CutUpCloth(Serial serial)
            : base(serial)
        {
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

        public void CutUp(Mobile from, Item[] items)
        {
            //Container backpack = from.Backpack;

            for (int i = 0; i < items.Length; i++)
            {
                BoltOfCloth boc = items[i] as BoltOfCloth;

                if (boc != null)
                    boc.Scissor(from, null);
            }
        }
    }

    public class CombineCloth : Item
    {
        public override int LabelNumber { get { return 1044459; } } // combine cloth

        [Constructable]
        public CombineCloth()
            : base(0x1767)
        {
        }

        public CombineCloth(Serial serial)
            : base(serial)
        {
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

        public static bool CheckHue(int hue, int[] hues, out int count)
        {
            int result = 0;
            bool success = true;

            for (int i = 0; i < hues.Length; i++)
            {
                if (hues[i] == hue)
                {
                    result = i;
                    success = false;
                }
            }

            count = result;

            return success;
        }

        public void Combine(Mobile from, Item[] items)
        {
            Container backpack = from.Backpack;

            int[] hues = new int[backpack.Items.Count];
            int[] amounts = new int[backpack.Items.Count];

            for (int i = 0; i < items.Length; i++)
            {
                Cloth c = items[i] as Cloth;

                if (c != null)
                {
                    int count;

                    if (CheckHue(c.Hue, hues, out count))
                    {
                        hues[i] = c.Hue;
                        amounts[i] = c.Amount;
                    }
                    else
                    {
                        amounts[count] += c.Amount;
                    }

                    c.Delete();
                }
            }

            for (int i = 0; i < hues.Length; i++)
            {
                Cloth cloth = new Cloth();
                cloth.Hue = hues[i];
                cloth.Amount = amounts[i];

                if (cloth.Amount > 0)
                    backpack.DropItem(cloth);
                else
                    cloth.Delete();
            }
        }
    }
}