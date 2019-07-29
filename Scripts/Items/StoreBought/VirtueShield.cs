using System;
using Server.Gumps;

namespace Server.Items
{
    public class VirtueShield : BaseShield, Server.Engines.Craft.IRepairable
    {
		public Server.Engines.Craft.CraftSystem RepairSystem { get { return Server.Engines.Craft.DefBlacksmithy.CraftSystem; } }	
        public override int BasePhysicalResistance { get { return 8; } }
        public override int BaseFireResistance { get { return 8; } }
        public override int BaseColdResistance { get { return 8; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 8; } }

        public override bool CanBeWornByGargoyles { get { return true; } }
        public override int LabelNumber { get { return 1109616; } } // Virtue Shield

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }
        public override bool IsArtifact { get { return true; } }

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

            writer.Write((int)1);
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
