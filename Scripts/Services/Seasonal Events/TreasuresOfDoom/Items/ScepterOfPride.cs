using System;
using Server;

namespace Server.Items
{
    public class ScepterOfPride : Scepter
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1155623; } } // Sceptre of Pride

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public ScepterOfPride()
        {
            WeaponAttributes.HitLeechStam = 70;
            WeaponAttributes.HitLeechMana = 70;
            WeaponAttributes.HitLeechHits = 70;
            Attributes.WeaponSpeed = 30;

            Slayer = SlayerName.Exorcism;
            Slayer2 = SlayerName.Silver;
        }

        public ScepterOfPride(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version
        }
    }
}