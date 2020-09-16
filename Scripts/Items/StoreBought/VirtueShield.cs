namespace Server.Items
{
    public class VirtueShield : BaseShield, Engines.Craft.IRepairable
    {
        public Engines.Craft.CraftSystem RepairSystem => Engines.Craft.DefBlacksmithy.CraftSystem;
        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 8;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override bool IsArtifact => true;

        [Constructable]
        public VirtueShield()
            : base(0x7818)
        {
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 10;

            LootType = LootType.Blessed;
        }

        public override bool OnEquip(Mobile from)
        {
            bool yes = base.OnEquip(from);

            if (yes)
            {
                Effects.PlaySound(from.Location, from.Map, 0x1F7);
                from.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
            }

            return yes;
        }

        public VirtueShield(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                HitPoints = MaxHitPoints = 255;
            }
        }
    }
}
