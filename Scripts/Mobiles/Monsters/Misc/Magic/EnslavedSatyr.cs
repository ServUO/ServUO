using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an enslaved satyr corpse")] 
    public class EnslavedSatyr : Satyr
    { 
        [Constructable]
        public EnslavedSatyr()
            : base()
        {
            this.Name = "an enslaved satyr";

            this.Fame = 10000;
            this.Karma = -10000;
        }

        public EnslavedSatyr(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            if (Utility.RandomDouble() < 0.1)				
                c.DropItem(new ParrotItem());	
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