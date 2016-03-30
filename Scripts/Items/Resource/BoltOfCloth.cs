using System;
using Server.Network;

namespace Server.Items
{
    [FlipableAttribute(0xF95, 0xF96, 0xF97, 0xF98, 0xF99, 0xF9A, 0xF9B, 0xF9C)]
    public class BoltOfCloth : Item, IScissorable, IDyable, ICommodity
    {
        [Constructable]
        public BoltOfCloth()
            : this(1)
        {
        }

        [Constructable]
        public BoltOfCloth(int amount)
            : base(0xF95)
        {
            this.Stackable = true;
            this.Weight = 5.0;
            this.Amount = amount;
        }

        public BoltOfCloth(Serial serial)
            : base(serial)
        {
        }

        int ICommodity.DescriptionNumber
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

        public bool Scissor(Mobile from, Scissors scissors)
        {
            if (this.Deleted || !from.CanSee(this))
                return false;

            base.ScissorHelper(from, new Cloth(), 50);

            return true;
        }

        public override void OnSingleClick(Mobile from)
        {
            int number = (this.Amount == 1) ? 1049122 : 1049121;

            from.Send(new MessageLocalized(this.Serial, this.ItemID, MessageType.Label, 0x3B2, 3, number, "", (this.Amount * 50).ToString()));
        }
    }
}