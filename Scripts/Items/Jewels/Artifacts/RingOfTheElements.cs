using System;

namespace Server.Items
{
    public class RingOfTheElements : GoldRing
    {
        [Constructable]
        public RingOfTheElements()
        {
            this.Hue = 0x4E9;
            this.Attributes.Luck = 100;
            this.Resistances.Fire = 16;
            this.Resistances.Cold = 16;
            this.Resistances.Poison = 16;
            this.Resistances.Energy = 16;
        }

        public RingOfTheElements(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061104;
            }
        }// Ring of the Elements
        public override int ArtifactRarity
        {
            get
            {
                return 11;
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