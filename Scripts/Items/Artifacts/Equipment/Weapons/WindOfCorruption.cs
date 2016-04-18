using System;
using Server;

namespace Server.Items
{
    public class WindOfCorruption : Cyclone
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1150358; } } // Wind of Corruption

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public WindOfCorruption()
        {
            WeaponAttributes.HitLeechStam = 40;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.HitLowerDefend = 40;
            AosElementDamages.Chaos = 100;

            Slayer = SlayerName.Fey;
            Hue = 1171;
        }

        public WindOfCorruption(Serial serial)
            : base(serial)
        {
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

    public class WindOfCorruptionHuman : Bow
    {
        public override int LabelNumber { get { return 1150358; } } // Wind of Corruption

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public WindOfCorruptionHuman()
        {
            WeaponAttributes.HitLeechStam = 40;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.HitLowerDefend = 40;
            AosElementDamages.Chaos = 100;

            Slayer = SlayerName.Fey;
            Hue = 1171;
        }

        public WindOfCorruptionHuman(Serial serial)
            : base(serial)
        {
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