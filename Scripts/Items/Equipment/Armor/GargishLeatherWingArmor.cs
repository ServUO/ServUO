using System;

namespace Server.Items
{
    [FlipableAttribute(0x457E, 0x457F)]
    public class GargishLeatherWingArmor : BaseArmor
    {
        [Constructable]
        public GargishLeatherWingArmor()
            : base(0x457E)
        {
            this.Weight = 2.0;
            this.Layer = Layer.Cloak;

        }

        public GargishLeatherWingArmor(Serial serial)
            : base(serial)
        {
        }

        public override int PhysicalResistance { get { return 0; } }
        public override int FireResistance { get { return 0; } }
        public override int ColdResistance { get { return 0; } }
        public override int PoisonResistance { get { return 0; } }
        public override int EnergyResistance { get { return 0; } }

        public override int AosStrReq
        {
            get
            {
                return 10;
            }
        }
        public override int OldStrReq
        {
            get
            {
                return 10;
            }
        }
        public override int ArmorBase
        {
            get
            {
                return 13;
            }
        }
        public override ArmorMaterialType MaterialType
        {
            get
            {
                return ArmorMaterialType.Leather;
            }
        }
        public override CraftResource DefaultResource
        {
            get
            {
                return CraftResource.RegularLeather;
            }
        }
        public override ArmorMeditationAllowance DefMedAllowance
        {
            get
            {
                return ArmorMeditationAllowance.All;
            }
        }
        public override Race RequiredRace
        {
            get
            {
                return Race.Gargoyle;
            }
        }
        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
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
