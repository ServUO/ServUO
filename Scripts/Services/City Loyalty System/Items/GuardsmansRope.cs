using System;
using Server;
using Server.Mobiles;
using Server.Engines.CityLoyalty;
using Server.Targeting;

namespace Server.Items
{
    public class GuardsmansRope : BaseDecayingItem
	{
        public override int LabelNumber { get { return 1152261; } } // guardsman's rope
        public override int Lifespan { get { return 3600; } }

        public GuardsmansRope() : base(0x14F8)
        {
            Hue = 1944;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1152244); // What would you like to arrest?
                from.BeginTarget(10, false, TargetFlags.None, (m, targeted) =>
                    {
                        if (targeted is Raider)
                        {
                            Raider raider = targeted as Raider;

                            if (raider.InRange(m.Location, 1))
                            {
                                if (raider.TryArrest(from))
                                    Delete();
                            }
                            else
                                m.SendLocalizedMessage(1152242); // You cannot reach that.
                        }
                        else
                            m.SendLocalizedMessage(1152243); // You cannot arrest that.
                    });
            }
            else
                from.SendLocalizedMessage(1116249); // That must be in your backpack for you to use it.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);


        }

        public GuardsmansRope(Serial serial)
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
			int version = reader.ReadInt();
		}
	}
}