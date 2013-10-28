using System;

namespace Server.Items
{
    public class TormentedChains : Item
    {
        [Constructable]
        public TormentedChains()
            : base(Utility.Random(6663, 2))
        {
            this.Name = "chains of the tormented";
            this.Weight = 1.0;						
        }

        public TormentedChains(Serial serial)
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