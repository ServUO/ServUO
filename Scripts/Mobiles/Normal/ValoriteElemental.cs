using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class ValoriteElemental : BaseCreature
    {
        [Constructable]
        public ValoriteElemental()
            : this(2)
        {
        }

        [Constructable]
        public ValoriteElemental(int oreAmount)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            // TODO: Gas attack
            this.Name = "a valorite elemental";
            this.Body = 112;
            this.BaseSoundID = 268;

            this.SetStr(226, 255);
            this.SetDex(126, 145);
            this.SetInt(71, 92);

            this.SetHits(136, 153);

            this.SetDamage(28);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Cold, 25);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 50.1, 95.0);
            this.SetSkill(SkillName.Tactics, 60.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 100.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 38;

            Item ore = new ValoriteOre(oreAmount);
            ore.ItemID = 0x19B9;
            this.PackItem(ore);
        }

        public ValoriteElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Gems, 4);
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)from;

                if (bc.Controlled || bc.BardTarget == this)
                    damage = 0; // Immune to pets and provoked creatures
            }
        }

        public override void CheckReflect(Mobile caster, ref bool reflect)
        {
            reflect = true; // Every spell is reflected back to the caster
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