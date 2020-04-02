using System;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute(0x1765, 0x1767)]
    public class AbyssalCloth : Item, ICommodity, IScissorable
    {
        [Constructable]
        public AbyssalCloth()
            : this(1)
        {
        }

        [Constructable]
        public AbyssalCloth(int amount)
            : base(0x1767)
        {
            Stackable = true;
            Amount = amount;			
			Hue = 2075;
        }

        public AbyssalCloth(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113350;
            }
        }// abyssal cloth
		
        TextDefinition ICommodity.Description
        {
            get
            {
                return LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
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
		
		public bool Scissor(Mobile from, Scissors scissors)
        {
            if (Deleted || !from.CanSee(this))
                return false;

            base.ScissorHelper(from, new Bandage(), 1);

            return true;
        }
    }
}
