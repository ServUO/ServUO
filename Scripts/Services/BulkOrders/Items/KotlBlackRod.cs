using System;

namespace Server.Items
{
    public class KotlBlackRod : QuarterStaff
    {
        public override int LabelNumber { get { return 1156990; } } // kotl black rod
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public KotlBlackRod()
        {
            Hue = 1150;

            WeaponAttributes.MageWeapon = 30;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 2;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 10;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public KotlBlackRod(Serial serial)
            : base(serial)
        {
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

    public class GargishKotlBlackRod : GargishGnarledStaff
    {
        public override int LabelNumber { get { return 1156994; } } // gargish kotl black rod
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishKotlBlackRod()
        {
            Hue = 1150;

            WeaponAttributes.MageWeapon = 30;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 2;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 10;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public GargishKotlBlackRod(Serial serial)
            : base(serial)
        {
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
