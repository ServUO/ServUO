using System;

namespace Server.Items
{
    public class MinotaurHedge : Item
    {
        [Constructable]
        public MinotaurHedge()
            : base(Utility.Random(3215, 4))
        {
            this.Name = "minotaur hedge";
            this.Weight = 1.0;						
        }

        public MinotaurHedge(Serial serial)
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
    }
}