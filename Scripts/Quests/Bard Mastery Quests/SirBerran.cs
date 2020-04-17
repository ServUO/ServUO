using Server.Items;
using Server.Mobiles;
using Server.Spells.SkillMasteries;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class WieldingTheSonicBladeQuest : BaseQuest
    {
        public WieldingTheSonicBladeQuest() : base()
        {
            AddObjective(new DiscordObjective());

            AddReward(new BaseReward(1115699)); // Recognition for mastery of song wielding.
            AddReward(new BaseReward(typeof(BookOfMasteries), 1028794));
        }

        public override object Title => 1115696;        //Wielding the Sonic Blade

        public override object Description => 1115697;  /*This quest is the single quest required for a player to unlock the discordance mastery 
                                                                        * abilities for bards. This quest can be completed multiple times to reinstate the discordance 
                                                                        * mastery. To prove yourself worthy, you must first be a master of discordance and musicianship. 
                                                                        * You must be willing to distort your notes to bring pain to even the most indifferent ears.*/

        public override object Refuse => 1115700;        //You must strive to spread discord.

        public override object Uncomplete => 1115700;    //You must strive to spread discord.

        public override object Complete => 1115701;      /* You have proven yourself worthy of wielding your music as a weapon. Rend the ears of your 
                                                                         * foes with your wails of discord. Let your song be feared as much as any sword.*/

        public override bool CanOffer()
        {
            if (Owner == null)
                return false;

            if (Owner.Skills[SkillName.Musicianship].Base < 90 || Owner.Skills[SkillName.Discordance].Base < 90)
            {
                Owner.SendLocalizedMessage(1115703); // Your skills in this focus area are less than the required master level. (90 minimum)
                return false;
            }

            foreach (BaseQuest q in Owner.Quests)
            {
                if (q is TheBeaconOfHarmonyQuest || q is IndoctrinationOfABattleRouserQuest)
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

            MasteryInfo.LearnMastery(Owner, SkillName.Discordance, 3);

            SkillMasterySpell.SetActiveMastery(Owner, SkillName.Discordance);
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

    public class DiscordObjective : SimpleObjective
    {
        private static readonly Type m_Type = typeof(Goat);

        private readonly List<string> m_Descr = new List<string>();
        public override List<string> Descriptions => m_Descr;

        public DiscordObjective()
            : base(5, -1)
        {
            m_Descr.Add("Discord five goats.");
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
                    Quest.Owner.SendLocalizedMessage(1115749, true, (MaxProgress - CurProgress).ToString()); // Creatures remaining to be discorded: 
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

    public class SirBerran : MondainQuester
    {
        [Constructable]
        public SirBerran()
        {
            Name = "Sir Berran";
            Title = "the Song Weilder";
        }

        public SirBerran(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(WieldingTheSonicBladeQuest),
                };

        public override void InitBody()
        {
            InitStats(125, 100, 25);

            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            Female = false;
            Body = 0x190;

            SetWearable(new ChainChest(), 0x2BF);
            SetWearable(new Shoes());
            SetWearable(new ShortPants());
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
