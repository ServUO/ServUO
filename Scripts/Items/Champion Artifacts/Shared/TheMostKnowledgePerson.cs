using System;

namespace Server.Items
{
    public class TheMostKnowledgePerson : BaseOuterTorso
    {
        [Constructable]
        public TheMostKnowledgePerson()
            : base(0x2684)
        {
            this.Hue = 0x117;
            this.StrRequirement = 0;

            this.Attributes.BonusHits = 3 + Utility.RandomMinMax(0, 2);
        }

        public TheMostKnowledgePerson(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094893;
            }
        }// The Most Knowledge Person [Replica]
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
            }
        }
        public override bool CanBeBlessed
        {
            get
            {
                return false;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}