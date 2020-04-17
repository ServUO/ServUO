using Server.Items;
using Server.Mobiles;
using Server.Spells.SkillMasteries;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class IndoctrinationOfABattleRouserQuest : BaseQuest
    {
        public IndoctrinationOfABattleRouserQuest() : base()
        {
            AddObjective(new ProvocationObjective());

            AddReward(new BaseReward(1115659)); // Recognition for mastery of battle rousing.
            AddReward(new BaseReward(typeof(BookOfMasteries), 1028794));
        }

        public override object Title => 1115657;        //Indoctrination of a Battle Rouser

        public override object Description => 1115656;  /*This quest is the single quest required for a player to unlock the provocation
                                                                        * mastery abilities for bards. This quest can be completed multiple times to reinstate
                                                                        * the provocation mastery. To prove yourself worthy, you must be able to incite even 
                                                                        * the most peaceful to frenzied battle lust.*/

        public override object Refuse => 1115660;        //To inspire you must persevere. 

        public override object Uncomplete => 1115660;    //To inspire you must persevere. 

        public override object Complete => 1115661;      /* You have proven yourself worthy of driving armies. Go forth, and have the blessing
                                                                         * and curse of the War Heralds always in your heart and mind. May peace always dwell 
                                                                         * before you, may destruction mark your wake, and may fury be your constant companion. 
                                                                         * Sow the seeds of battle and glory in the music of war.*/

        public override bool CanOffer()
        {
            if (Owner == null)
                return false;

            if (Owner.Skills[SkillName.Musicianship].Base < 90 || Owner.Skills[SkillName.Provocation].Base < 90)
            {
                Owner.SendLocalizedMessage(1115703); // Your skills in this focus area are less than the required master level. (90 minimum)
                return false;
            }

            foreach (BaseQuest q in Owner.Quests)
            {
                if (q is TheBeaconOfHarmonyQuest || q is WieldingTheSonicBladeQuest)
                {
                    Owner.SendLocalizedMessage(1115702); //You must quit your other mastery quests before engaging on a new one.
                    return false;
                }
            }

            return true;
        }

        public override void GiveRewards()
        {
            base.GiveRewards();

            MasteryInfo.LearnMastery(Owner, SkillName.Provocation, 3);

            SkillMasterySpell.SetActiveMastery(Owner, SkillName.Provocation);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ProvocationObjective : SimpleObjective
    {
        private readonly List<string> m_Descr = new List<string>();
        public override List<string> Descriptions => m_Descr;

        public ProvocationObjective()
            : base(5, -1)
        {
            m_Descr.Add("Incite rabbits into battle with 5 wandering healers.");
        }

        public override bool Update(object obj)
        {
            if (obj is Mobile && (((Mobile)obj).GetType() == typeof(WanderingHealer) || ((Mobile)obj).GetType() == typeof(EvilWanderingHealer)))
            {
                CurProgress++;

                if (Completed)
                    Quest.OnCompleted();
                else
                {
                    Quest.Owner.SendLocalizedMessage(1115748, true, (MaxProgress - CurProgress).ToString()); // Conflicts remaining to be incited: 
                    Quest.Owner.PlaySound(Quest.UpdateSound);
                }

                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SirHareus : MondainQuester
    {
        [Constructable]
        public SirHareus()
        {
            Name = "Sir Hareus";
            Title = "the Battle Rouser";
        }

        public SirHareus(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(IndoctrinationOfABattleRouserQuest),
                };

        public override void InitBody()
        {
            InitStats(125, 100, 25);

            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            Female = false;
            Body = 0x190;

            SetWearable(new ChainChest(), 0x30A);
            SetWearable(new Shoes(GetShoeHue()));
            SetWearable(new LongPants(GetRandomHue()));
            SetWearable(new Halberd());
            SetWearable(new BodySash(0x355));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
