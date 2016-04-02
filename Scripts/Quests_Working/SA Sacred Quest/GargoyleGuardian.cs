using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gargoyle corpse")]
    public class GargoyleGuardian : BaseCreature
    {
        [Constructable]
        public GargoyleGuardian()
            : base(AIType.AI_Mage, FightMode.None, 10, 1, 0.2, 0.4)
        {
            this.Name = "Abyss Guardian";
            this.Body = 0x2F3;
            this.BaseSoundID = 0x174;

            this.SetStr(760, 850);
            this.SetDex(102, 150);
            this.SetInt(152, 200);

            this.SetHits(482, 485);

            this.SetDamage(1000, 1150);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 15, 25);

            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 120.4, 160.0);
            this.SetSkill(SkillName.Anatomy, 50.5, 100.0);
            this.SetSkill(SkillName.Swords, 90.1, 100.0);
            this.SetSkill(SkillName.Macing, 90.1, 100.0);
            this.SetSkill(SkillName.Fencing, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.2);
            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Meditation, 90.1, 100.0);

            this.Fame = 10000;
            this.Karma = -10000;
            this.Frozen = true;
            this.CantWalk = true;
            this.Hue = 1153;

            this.VirtualArmor = 50;

            if (0.2 > Utility.RandomDouble())
                this.PackItem(new GargoylesPickaxe());
        }

        public GargoyleGuardian(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls);
            this.AddLoot(LootPack.Gems, 2);
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            if (from != null && from.Alive && 0.4 > Utility.RandomDouble())
            {
                this.ThrowHatchet(from);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (attacker != null && attacker.Alive && attacker.Weapon is BaseRanged && 0.4 > Utility.RandomDouble())
            {
                this.ThrowHatchet(attacker);
            }
        }

        public void ThrowHatchet(Mobile to)
        {
            int damage = 50;
            this.MovingEffect(to, 0xF43, 10, 0, false, false);
            this.DoHarmful(to);
            AOS.Damage(to, this, damage, 100, 0, 0, 0, 0);
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