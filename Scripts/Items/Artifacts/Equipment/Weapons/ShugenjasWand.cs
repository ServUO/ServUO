using System;

namespace Server.Items
{
    public class ShugenjasWand : MagicWand
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ShugenjasWand()
        {
            Name = "Shugenja's Wand";
            Hue = 1266;

            Attributes.SpellChanneling = 1;
            Attributes.CastRecovery = 2;
            Attributes.RegenMana = 10;

            WeaponAttributes.MageWeapon = 30;
            WeaponAttributes.HitLightning = 50;
            WeaponAttributes.HitLowerDefend = 50;

            AosElementDamages.Physical = 100;

            MaxHitPoints = 255;
            HitPoints = 255;
        }

        public ShugenjasWand(Serial serial)
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
