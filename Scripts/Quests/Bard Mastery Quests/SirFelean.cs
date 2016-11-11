using System;
using Server.Items;
using Server.Mobiles;
using Server.Spells.SkillMasteries;
using System.Collections.Generic;

namespace Server.Engines.Quests
{ 
    public class TheBeaconOfHarmonyQuest : BaseQuest
    {
        public TheBeaconOfHarmonyQuest() : base()
        { 
            AddObjective(new PeacemakingObjective());

            AddReward(new BaseReward(1115679)); // Recognition for mastery of spirit soothing.
            AddReward(new BaseReward(typeof(BookOfMasteries), 1028794));
        }

        public override object Title { get { return 1115677; } }       //The Beacon of Harmony

        public override object Description { get { return 1115676; } } /* This quest is the single quest required for a player to unlock the peacemaking 
                                                                        * mastery abilities for bards. This quest can be completed multiple times to reinstate
                                                                        * the peacemaking mastery. To prove yourself worthy, you must first be a master of 
                                                                        * peacemaking and musicianship. You must be able to calm even the most vicious beast
                                                                        * into tranquility.*/

        public override object Refuse { get { return 1115680; } }       //To deliver peace you must persevere. 

        public override object Uncomplete { get { return 1115680; } }   //To deliver peace you must persevere. 

        public override object Complete { get { return 1115681; } }     /* You have proven yourself a beacon of peace and a bringer of harmony. Only 
                                                                         * a warrior may choose the peaceful solution, all others are condemned to it. 
                                                                         * May your message of peace flow into the world and shelter you from harm.*/

        public override bool CanOffer()
        {
            if (Owner == null)
                return false;

            if (Owner.Skills[SkillName.Musicianship].Base < 90 || Owner.Skills[SkillName.Peacemaking].Base < 90)
            {
                Owner.SendLocalizedMessage(1115703); // Your skills in this focus area are less than the required master level. (90 minimum)
                return false;
            }

            foreach (BaseQuest q in Owner.Quests)
            {
                if (q is IndoctrinationOfABattleRouserQuest || q is WieldingTheSonicBladeQuest)
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

            MasteryInfo.LearnMastery(Owner, SkillName.Peacemaking, Volume.Two);
            MasteryInfo.LearnMastery(Owner, SkillName.Peacemaking, Volume.Three);

            SkillMasterySpell.SetActiveMastery(Owner, SkillName.Peacemaking);
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

    public class PeacemakingObjective : SimpleObjective
    {
        private static readonly Type m_Type = typeof(Mongbat);

        private List<string> m_Descr = new List<string>();
        public override List<string> Descriptions { get { return m_Descr; } }

        public PeacemakingObjective()
            : base(5, -1)
        {
            m_Descr.Add("Calm five mongbats.");
        }

        public override bool Update(object obj)
        {
            if (obj is Mobile && ((Mobile)obj).GetType() == m_Type)
            {
                CurProgress++;

                if (Completed)
                    Quest.OnCompleted();
                else
                {
                    Quest.Owner.SendLocalizedMessage(1115747, true, (MaxProgress - CurProgress).ToString()); // Creatures remaining to be calmed:   ~1_val~.
                    Quest.Owner.PlaySound(Quest.UpdateSound);
                }

                return true;
            }

            return false;
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

    public class SirFelean : MondainQuester
    {
        [Constructable]
        public SirFelean()
        {
            Name = "Sir Felean";
            Title = "the Spirit Soother";
        }

        public SirFelean(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(TheBeaconOfHarmonyQuest),
                };
            }
        }

        public override void InitBody()
        {
            InitStats(125, 100, 25);

            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if (IsInvulnerable && !Core.AOS)
                NameHue = 0x35;

            Female = false;
            Body = 0x190;

            SetWearable(new ChainChest(), 0x35);
            SetWearable(new Shoes(GetShoeHue()));
            SetWearable(new LongPants());
            SetWearable(new Halberd());
            SetWearable(new BodySash(0x498));
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