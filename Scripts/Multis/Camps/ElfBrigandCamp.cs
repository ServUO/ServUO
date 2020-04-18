using Server.Mobiles;

namespace Server.Multis
{
    public class ElfBrigandCamp : BrigandCamp
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

        public override Mobile Brigands => new ElfBrigand();
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}