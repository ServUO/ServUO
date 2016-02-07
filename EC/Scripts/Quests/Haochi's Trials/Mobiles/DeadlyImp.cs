using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class DeadlyImp : BaseCreature
    {
        [Constructable]
        public DeadlyImp()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a deadly imp";
            this.Body = 74;
            this.BaseSoundID = 422;
            this.Hue = 0x66A;

            this.SetStr(91, 115);
            this.SetDex(61, 80);
            this.SetInt(86, 105);

            this.SetHits(1000);

            this.SetDamage(50, 80);

            this.SetDamageType(ResistanceType.Fire, 100);

            this.SetResistance(ResistanceType.Physical, 95, 98);
            this.SetResistance(ResistanceType.Fire, 95, 98);
            this.SetResistance(ResistanceType.Cold, 95, 98);
            this.SetResistance(ResistanceType.Poison, 95, 98);
            this.SetResistance(ResistanceType.Energy, 95, 98);

            this.SetSkill(SkillName.Magery, 120.0);
            this.SetSkill(SkillName.Tactics, 120.0);
            this.SetSkill(SkillName.Wrestling, 120.0);

            this.Fame = 2500;
            this.Karma = -2500;

            this.CantWalk = true;
        }

        public DeadlyImp(Serial serial)
            : base(serial)
        {
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
                        qs.AddObjective(new SecondTrialReturnObjective(false));
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