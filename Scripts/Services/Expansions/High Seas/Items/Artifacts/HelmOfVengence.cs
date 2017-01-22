using Server;
using System;

namespace Server.Items
{
    public class HelmOfVengence : NorseHelm
    {
        public override int LabelNumber { get { return 1116621; } }

        public override int BasePhysicalResistance { get { return 11; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 14; } }
        public override int BasePoisonResistance { get { return 7; } }
        public override int BaseEnergyResistance { get { return 8; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public HelmOfVengence()
        {
            Hue = 2012;
            Attributes.RegenMana = 3;
            Attributes.ReflectPhysical = 30;
            Attributes.AttackChance = 7;
            Attributes.WeaponDamage = 10;
            Attributes.LowerManaCost = 8;
        }

        public HelmOfVengence(Serial serial)
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