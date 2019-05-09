using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an interred grizzle corpse")]
    public class InterredGrizzle : BaseCreature
    {
        [Constructable]
        public InterredGrizzle()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an interred grizzle";
            this.Body = 259;

            this.SetStr(451, 500);
            this.SetDex(201, 250);
            this.SetInt(801, 850);

            this.SetHits(1500);
            this.SetStam(150);

            this.SetDamage(16, 19);

            this.SetDamageType(ResistanceType.Physical, 30);
            this.SetDamageType(ResistanceType.Fire, 70);

            this.SetResistance(ResistanceType.Physical, 35, 55);
            this.SetResistance(ResistanceType.Fire, 20, 65);
            this.SetResistance(ResistanceType.Cold, 55, 80);
            this.SetResistance(ResistanceType.Poison, 20, 35);
            this.SetResistance(ResistanceType.Energy, 60, 80);

            this.SetSkill(SkillName.Meditation, 77.7, 84.0);
            this.SetSkill(SkillName.EvalInt, 72.2, 79.6);
            this.SetSkill(SkillName.Magery, 83.7, 89.6);
            this.SetSkill(SkillName.Poisoning, 0);
            this.SetSkill(SkillName.Anatomy, 0);
            this.SetSkill(SkillName.MagicResist, 80.2, 87.3);
            this.SetSkill(SkillName.Tactics, 104.5, 105.1);
            this.SetSkill(SkillName.Wrestling, 105.1, 109.4);

            this.Fame = 3700;  // Guessed
            this.Karma = -3700;  // Guessed

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }
		public override bool CanBeParagon { get { return false; } }
        /*
        public override bool OnBeforeDeath()
        {
        SpillAcid( 1, 4, 10, 6, 10 );

        return base.OnBeforeDeath();
        }
        */
        public InterredGrizzle(Serial serial)
            : base(serial)
        {
        }
		
		public override int TreasureMapLevel { get { return 4; } }

        public override void GenerateLoot() // -- Need to verify
        {
            this.AddLoot(LootPack.FilthyRich);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Utility.RandomDouble() < 0.04)
                SpillAcid(null, Utility.RandomMinMax(1, 3));

            base.OnDamage(amount, from, willKill);
        }

        public override Item NewHarmfulItem()
        {
            return new InfernalOoze(this, false, Utility.RandomMinMax(6, 10));
        }

        public override int GetAngerSound()
        {
            return 0x581;
        }

        public override int GetIdleSound()
        {
            return 0x582;
        }

        public override int GetAttackSound()
        {
            return 0x580;
        }

        public override int GetHurtSound()
        {
            return 0x583;
        }

        public override int GetDeathSound()
        {
            return 0x584;
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
