using System;
using Server.Mobiles;

namespace Server.Multis
{
    public class ElfBrigandCamp : HumanBrigandCamp
    { 
        [Constructable]
        public ElfBrigandCamp()
            : base()
        {
        }

        public ElfBrigandCamp(Serial serial)
            : base(serial)
        {
        }

        public override Mobile Camper
        {
            get
            {
                return new ElfBrigand();
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