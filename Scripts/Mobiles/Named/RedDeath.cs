using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Red Death corpse")]
    public class RedDeath : SkeletalMount
    {
        [Constructable]
        public RedDeath()
            : base("Red Death")
        {
            this.Hue = 0x21;
            this.BaseSoundID = 0x1C3;

            this.AI = AIType.AI_Melee;
            this.FightMode = FightMode.Closest;

            this.SetStr(319, 324);
            this.SetDex(241, 244);
            this.SetInt(242, 255);

            this.SetHits(1540, 1605);

            this.SetDamage(25, 29);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Fire, 75);
            this.SetDamageType(ResistanceType.Cold, 0);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 90);
            this.SetResistance(ResistanceType.Cold, 0);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 0);

            this.SetSkill(SkillName.Wrestling, 121.4, 143.7);
            this.SetSkill(SkillName.Tactics, 120.9, 142.2);
            this.SetSkill(SkillName.MagicResist, 120.1, 142.3);
            this.SetSkill(SkillName.Anatomy, 120.2, 144.0);

            this.Fame = 28000;
            this.Karma = -28000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public RedDeath(Serial serial)
            : base(serial)
        {
        }
		public override bool CanBeParagon { get { return false; } }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }
        public override int BreathChaosDamage
        {
            get
            {
                return 100;
            }
        }
        public override int BreathFireDamage
        {
            get
            {
                return 0;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 3);
            this.AddLoot(LootPack.MedScrolls, 2);
            this.AddLoot(LootPack.HighScrolls, 2);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.WhirlwindAttack;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new ResolvesBridle());
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