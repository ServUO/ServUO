using System;

namespace Server.Mobiles
{
    [CorpseName("a rock mite corpse")]
    public class RockMite : BaseCreature
    {
        [Constructable]
        public RockMite()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a rock mite";
            this.Body = 787;
            this.BaseSoundID = 1006;
            this.Hue = 2500;

            this.SetStr(733, 754);
            this.SetDex(126, 144);
            this.SetInt(75, 94);

            this.SetHits(803, 817);
            this.SetStam(126, 144);
            this.SetMana(75, 94);

            this.SetDamage(12, 19);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Cold, 50);

            this.SetResistance(ResistanceType.Physical, 50, 59);
            this.SetResistance(ResistanceType.Fire, 80, 90);
            this.SetResistance(ResistanceType.Cold, 52, 59);
            this.SetResistance(ResistanceType.Poison, 80, 85);
            this.SetResistance(ResistanceType.Energy, 80, 90);

            this.SetSkill(SkillName.MagicResist, 88.5, 119.6);
            this.SetSkill(SkillName.Tactics, 84.9, 112.9);
            this.SetSkill(SkillName.Wrestling, 82.7, 119.8);
			this.SetSkill(SkillName.Parry, 90.0, 100.0);
			this.SetSkill(SkillName.DetectHidden, 42.9);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 40;
        }

        public RockMite(Serial serial)
            : base(serial)
        {
        }

        public override int GetAngerSound()
        {
            return 0x5A;
        }

        public override int GetIdleSound()
        {
            return 0x5A;
        }

        public override int GetAttackSound()
        {
            return 0x164;
        }

        public override int GetHurtSound()
        {
            return 0x187;
        }

        public override int GetDeathSound()
        {
            return 0x1BA;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Gems, Utility.Random(1, 5));
        }

        public override bool IsEnemy(Mobile m)
        {
            return base.IsEnemy(m);
        }
		
		// Tail Swipe

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            SolenHelper.OnBlackDamage(from);

            base.OnDamage(amount, from, willKill);
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
