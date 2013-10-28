using System;

namespace Server.Items
{
    public class GargishNecklace : BaseNecklace
    {
        [Constructable]
        public GargishNecklace()
            : base(0x4210)
        {
            //Weight = 0.1;
        }

        public GargishNecklace(Serial serial)
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