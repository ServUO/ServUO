using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class MougGuurMustDieQuest : BaseQuest
    {
        public MougGuurMustDieQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(MougGuur), "moug-guur", 1, "Sanctuary"));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override QuestChain ChainID => QuestChain.MiniBoss;
        public override Type NextQuest => typeof(LeaderOfThePackQuest);
        /* Moug-Guur Must Die */
        public override object Title => 1072368;
        /* You there!  Yes, you.  Kill Moug-Guur, the leader of the orcs in this depressing place, and I'll make 
        it worth your while. */
        public override object Description => 1072561;
        /* Fine. It's no skin off my teeth. */
        public override object Refuse => 1072571;
        /* Small words.  Kill Moug-Guur.  Go.  Now! */
        public override object Uncomplete => 1072572;
        /* You're better than I thought you'd be.  Not particularly bad, but not entirely inept. */
        public override object Complete => 1072573;
        public override bool CanOffer()
        {
            return MondainsLegacy.Sanctuary;
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

    public class LeaderOfThePackQuest : BaseQuest
    {
        public LeaderOfThePackQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Chiikkaha), "chiikkaha", 1, "Sanctuary"));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override QuestChain ChainID => QuestChain.MiniBoss;
        public override Type NextQuest => typeof(SayonaraSzavetraQuest);
        /* Leader of the Pack */
        public override object Title => 1072560;
        /* Well now that Moug-Guur is no more -- and I can't say I'm weeping for his demise -- it's time for the 
        ratmen to experience a similar loss of leadership.  Slay Chiikkaha.  In return, I'll satisfy your greed 
        temporarily. */
        public override object Description => 1072574;
        /* Alright, if you'd rather not, then run along and do whatever worthless things you do when I'm not 
        giving you direction. */
        public override object Refuse => 1072575;
        /* How difficult is this?  The rats live in the tunnels.  Go into the tunnels and find the biggest, meanest 
        rat and execute him.  Loitering around here won't get the task done. */
        public override object Uncomplete => 1072576;
        /* It's about time!  Could you have taken longer?	 */
        public override object Complete => 1072577;
        public override bool CanOffer()
        {
            return MondainsLegacy.Sanctuary;
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

    public class SayonaraSzavetraQuest : BaseQuest
    {
        public SayonaraSzavetraQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Szavetra), "szavetra", 1, "Sanctuary"));

            AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        public override QuestChain ChainID => QuestChain.MiniBoss;
        /* Sayonara, Szavetra */
        public override object Title => 1072375;
        /* Hmm, maybe you aren't entirely worthless.  I suspect a demoness of Szavetra's calibre will tear you 
        apart ...  We might as well find out.  Kill the succubus, yada yada, and you'll be richly rewarded. */
        public override object Description => 1072578;
        /* Hah!  I knew you couldn't handle it. */
        public override object Refuse => 1072579;
        /* Hahahaha!  I can see the fear in your eyes.  Pathetic.  Szavetra is waiting for you. */
        public override object Uncomplete => 1072581;
        /* Amazing!  Simply astonishing ... you survived.  Well, I supposed I should indulge your avarice 
        with a reward.*/
        public override object Complete => 1072582;
        public override bool CanOffer()
        {
            return MondainsLegacy.Sanctuary;
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