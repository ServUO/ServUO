using System;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a meer corpse")]
    public class MeerWarrior : BaseCreature
    {
        [Constructable]
        public MeerWarrior()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a meer warrior";
            this.Body = 771;

            this.SetStr(86, 100);
            this.SetDex(186, 200);
            this.SetInt(86, 100);

            this.SetHits(52, 60);

            this.SetDamage(12, 19);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 5, 15);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 25, 35);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.MagicResist, 91.0, 100.0);
            this.SetSkill(SkillName.Tactics, 91.0, 100.0);
            this.SetSkill(SkillName.Wrestling, 91.0, 100.0);

            this.VirtualArmor = 22;

            this.Fame = 2000;
            this.Karma = 5000;
        }

        public MeerWarrior(Serial serial)
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
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && !willKill && amount > 3 && !this.InRange(from, 7))
            {
                this.MovingEffect(from, 0xF51, 10, 0, false, false);
                SpellHelper.Damage(TimeSpan.FromSeconds(1.0), from, this, Utility.RandomMinMax(30, 40) - (Core.AOS ? 0 : 10), 100, 0, 0, 0, 0);
            }

            base.OnDamage(amount, from, willKill);
        }

        public override int GetHurtSound()
        {
            return 0x156;
        }

        public override int GetDeathSound()
        {
            return 0x15C;
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