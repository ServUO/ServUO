using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a grubber corpse")]
    public class Grubber : BaseCreature
    {
        [Constructable]
        public Grubber()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.06, 0.1)
        {
            Name = "a grubber";
            Body = 270;

            SetStr(15);
            SetDex(2000);
            SetInt(1000);

            SetHits(200);
            SetStam(500);
            SetMana(0);

            SetDamage(1);

            SetDamageType(ResistanceType.Physical, 100);

            SetSkill(SkillName.MagicResist, 200.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);

            Fame = 1000;
            Karma = 0;

            VirtualArmor = 4;
        }

        public override IDamageable Combatant
        {
            get { return base.Combatant; }
            set
            {
                base.Combatant = value;

                if (0.10 > Utility.RandomDouble())
                    StopFlee();
                else if (!CheckFlee())
                    BeginFlee(TimeSpan.FromSeconds(10));
            }
        }

        public Grubber(Serial serial)
            : base(serial)
        {
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 1; } }

        public override int GetAttackSound()
        {
            return 0xC9;
        }

        public override int GetHurtSound()
        {
            return 0xCA;
        }

        public override int GetDeathSound()
        {
            return 0xCB;
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