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
            Hue = 0x21;
            BaseSoundID = 0x1C3;

            AI = AIType.AI_Melee;
            FightMode = FightMode.Closest;

            SetStr(319, 324);
            SetDex(241, 244);
            SetInt(242, 255);

            SetHits(1540, 1605);

            SetDamage(25, 29);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 75);
            SetDamageType(ResistanceType.Cold, 0);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 90);
            SetResistance(ResistanceType.Cold, 0);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 0);

            SetSkill(SkillName.Wrestling, 121.4, 143.7);
            SetSkill(SkillName.Tactics, 120.9, 142.2);
            SetSkill(SkillName.MagicResist, 120.1, 142.3);
            SetSkill(SkillName.Anatomy, 120.2, 144.0);

            Fame = 28000;
            Karma = -28000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }

            SetWeaponAbility(WeaponAbility.WhirlwindAttack);
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
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.HighScrolls, 2);
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