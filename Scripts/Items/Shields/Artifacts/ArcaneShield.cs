using System;

namespace Server.Items
{
    public class ArcaneShield : WoodenKiteShield
    {
        [Constructable]
        public ArcaneShield()
        {
            this.ItemID = 0x1B78;
            this.Hue = 0x556;
            this.Attributes.NightSight = 1;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.DefendChance = 15;
            this.Attributes.CastSpeed = 1;
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

            if (this.Attributes.NightSight == 0)
                this.Attributes.NightSight = 1;
        }
    }
}