using System;

namespace Server.Items
{
    public class ArcaneShield : WoodenKiteShield
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArcaneShield()
        {
            Hue = 0x556;
            Attributes.NightSight = 1;
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 15;
            Attributes.CastSpeed = 1;
        }

        public ArcaneShield(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1061101;
            }
        }// Arcane Shield 
        public override int ArtifactRarity
        {
            get
            {
                return 11;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
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