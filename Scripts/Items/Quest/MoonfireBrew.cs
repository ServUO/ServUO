using System;

namespace Server.Engines.Quests.Hag
{
    public class MoonfireBrew : Item
    {
        [Constructable]
        public MoonfireBrew()
            : base(0xF04)
        {
            this.Weight = 1.0;
        }

        public MoonfireBrew(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1055065;
            }
        }// a bottle of magical moonfire brew
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