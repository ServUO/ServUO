using System;

namespace Server.Items
{
    [FlipableAttribute(0x4960, 0x4966)]     
    public class PosedGoblinTopiary : Item
    {
         public override int LabelNumber{ get{ return 1070878; } } // a decorative topiary
       
        [Constructable]
        public PosedGoblinTopiary() : base(0x4960)
        {
            this.Weight = 1.0;
            this.Name = ("a posed goblin topiary");
        }

        public PosedGoblinTopiary(Serial serial) : base(serial)
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