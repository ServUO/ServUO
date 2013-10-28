using System;

namespace Server.Items
{
    public class GargishEarrings : BaseEarrings
    {
        [Constructable]
        public GargishEarrings()
            : base(0x4213)
        {
            //Weight = 0.1;
        }

        public GargishEarrings(Serial serial)
            : base(serial)
        {
        }

        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
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
    }
}