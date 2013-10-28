using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a speckled scorpion corpse")]
    public class SpeckledScorpion : Scorpion
    {
        [Constructable]
        public SpeckledScorpion()
            : base()
        {
            this.Name = "a speckled scorpion";
        }

        public SpeckledScorpion(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);	
			
            if (Utility.RandomDouble() < 0.4)
                c.DropItem(new SpeckledPoisonSac());
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
    }
}