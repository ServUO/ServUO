using System;
using Server.Mobiles;

namespace Server.Items
{
    public class ParoxysmusSwampDragonStatuette : BaseImprisonedMobile
    {
        [Constructable]
        public ParoxysmusSwampDragonStatuette()
            : base(0x2619)
        {
            this.Weight = 1.0;
            this.Hue = 0x851;
        }

        public ParoxysmusSwampDragonStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072084;
            }
        }// Paroxysmus' Swamp Dragon		
        public override BaseCreature Summon
        {
            get
            {
                return new ParoxysmusSwampDragon();
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