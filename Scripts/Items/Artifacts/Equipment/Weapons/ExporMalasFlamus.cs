using System;

namespace Server.Items
{
    public class ExporMalasFlamus : BladedStaff
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ExporMalasFlamus()
        {
            Name = "Expor Malas Flamus";
            Hue = 1161;

            SearingWeapon = true;

            WeaponAttributes.HitLowerAttack = 50;
            WeaponAttributes.HitLeechHits = 100;
            WeaponAttributes.HitManaDrain = 100;
            WeaponAttributes.HitLeechStam = 50;
            WeaponAttributes.HitFireArea = 70;

            Attributes.WeaponDamage = 30;

            AosElementDamages.Physical = 0;
            AosElementDamages.Fire = 100;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public ExporMalasFlamus(Serial serial)
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
            reader.ReadInt();
        }
    }

    public class GargishExporMalasFlamus : GargishKatana
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GargishExporMalasFlamus()
        {
            Name = "Gargish Expor Malas Flamus";
            Hue = 1161;

            SearingWeapon = true;

            WeaponAttributes.HitLowerAttack = 50;
            WeaponAttributes.HitLeechHits = 100;
            WeaponAttributes.HitManaDrain = 100;
            WeaponAttributes.HitLeechStam = 50;
            WeaponAttributes.HitFireArea = 70;

            Attributes.WeaponDamage = 30;

            AosElementDamages.Physical = 0;
            AosElementDamages.Fire = 100;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public GargishExporMalasFlamus(Serial serial)
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
            reader.ReadInt();
        }
    }
}
