using System;

namespace Server.Items
{
    public class TheRedeemer : PaladinSword
    {
        public override int LabelNumber { get { return 1077442; } } // The Redeemer
        public override bool IsArtifact { get { return true; } }

        public override int ArtifactRarity { get { return 7; } }

        public override int InitMinHits { get { return 100; } }
        public override int InitMaxHits { get { return 100; } }

        [Constructable]
        public TheRedeemer()
        {
            Hue = 2304;
            Slayer = SlayerName.Silver;
            Slayer2 = SlayerName.Exorcism;
            Attributes.WeaponDamage = 55;
        }

        public TheRedeemer(Serial serial)
            : base(serial)
        {
        }

        public override bool CanFortify { get { return false; } }

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
