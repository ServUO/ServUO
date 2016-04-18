using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseImprisonedMobile : Item
    {
        [Constructable]
        public BaseImprisonedMobile(int itemID)
            : base(itemID)
        {
        }

        public BaseImprisonedMobile(Serial serial)
            : base(serial)
        {
        }

        public abstract BaseCreature Summon { get; }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
                from.SendGump(new ConfirmBreakCrystalGump(this));
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
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

        public virtual void Release(Mobile from, BaseCreature summon)
        { 
        }
    }
}