using Server.Mobiles;

namespace Server.Items
{
    public class ParoxysmusSwampDragonStatuette : BaseImprisonedMobile
    {
        [Constructable]
        public ParoxysmusSwampDragonStatuette()
            : base(0x2619)
        {
            Weight = 1.0;
            Hue = 0x851;
        }

        public ParoxysmusSwampDragonStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072084;// Paroxysmus' Swamp Dragon		
        public override BaseCreature Summon => new ParoxysmusSwampDragon();
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