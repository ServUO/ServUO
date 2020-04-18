using Server.Spells;
using System;

namespace Server.Mobiles
{
    [CorpseName("a meer corpse")]
    public class MeerWarrior : BaseCreature
    {
        [Constructable]
        public MeerWarrior()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a meer warrior";
            Body = 771;

            SetStr(86, 100);
            SetDex(186, 200);
            SetInt(86, 100);

            SetHits(52, 60);

            SetDamage(12, 19);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 5, 15);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 91.0, 100.0);
            SetSkill(SkillName.Tactics, 91.0, 100.0);
            SetSkill(SkillName.Wrestling, 91.0, 100.0);

            Fame = 2000;
            Karma = 5000;
        }

        public MeerWarrior(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses => true;
        public override bool InitialInnocent => true;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (from != null && !willKill && amount > 3 && !InRange(from, 7))
            {
                MovingEffect(from, 0xF51, 10, 0, false, false);
                SpellHelper.Damage(TimeSpan.FromSeconds(1.0), from, this, Utility.RandomMinMax(30, 40), 100, 0, 0, 0, 0);
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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
