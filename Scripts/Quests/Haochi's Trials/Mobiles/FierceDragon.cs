using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class FierceDragon : BaseCreature
    {
        [Constructable]
        public FierceDragon()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a fierce dragon";
            this.Body = 103;
            this.BaseSoundID = 362;

            this.SetStr(6000, 6020);
            this.SetDex(0);
            this.SetInt(850, 870);

            this.SetDamage(50, 80);

            this.SetDamageType(ResistanceType.Fire, 100);

            this.SetResistance(ResistanceType.Physical, 95, 98);
            this.SetResistance(ResistanceType.Fire, 95, 98);
            this.SetResistance(ResistanceType.Cold, 95, 98);
            this.SetResistance(ResistanceType.Poison, 95, 98);
            this.SetResistance(ResistanceType.Energy, 95, 98);

            this.SetSkill(SkillName.Tactics, 120.0);
            this.SetSkill(SkillName.Wrestling, 120.0);
            this.SetSkill(SkillName.Magery, 120.0);

            this.Fame = 15000;
            this.Karma = 15000;

            this.CantWalk = true;
        }

        public FierceDragon(Serial serial)
            : base(serial)
        {
        }

        public override int GetIdleSound()
        {
            return 0x2C4;
        }

        public override int GetAttackSound()
        {
            return 0x2C0;
        }

        public override int GetDeathSound()
        {
            return 0x2C1;
        }

        public override int GetAngerSound()
        {
            return 0x2C4;
        }

        public override int GetHurtSound()
        {
            return 0x2C3;
        }

        public override void AggressiveAction(Mobile aggressor, bool criminal)
        {
            base.AggressiveAction(aggressor, criminal);

            PlayerMobile player = aggressor as PlayerMobile;
            if (player != null)
            {
                QuestSystem qs = player.Quest;
                if (qs is HaochisTrialsQuest)
                {
                    QuestObjective obj = qs.FindObjective(typeof(SecondTrialAttackObjective));
                    if (obj != null && !obj.Completed)
                    {
                        obj.Complete();
                        qs.AddObjective(new SecondTrialReturnObjective(true));
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}