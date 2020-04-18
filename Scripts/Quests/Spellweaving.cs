using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class PatienceQuest : BaseQuest
    {
        public PatienceQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(MiniatureMushroom), "miniature mushrooms", 20, 0xD16, 3600));

            AddReward(new BaseReward(1074872)); // The opportunity to learn the ways of the Arcanist.
        }

        public override QuestChain ChainID => QuestChain.Spellweaving;
        public override Type NextQuest => typeof(NeedsOfManyHeartwoodQuest);
        /* Patience */
        public override object Title => 1072753;
        /* Learning to weave spells and control the forces of nature requires sacrifice, 
        discipline, focus, and an unwavering dedication to Sosaria herself.  We do not 
        teach the unworthy.  They do not comprehend the lessons nor the dedication 
        required.  If you would walk the path of the Arcanist, then you must do as I 
        require without hesitation or question.  Your first task is to gather miniature 
        mushrooms ... 20 of them from the branches of our mighty home.  I give you one 
        hour to complete the task. */
        public override object Description => 1072762;
        /* * nods* Not everyone has the temperment to undertake the way of the Arcanist. */
        public override object Refuse => 1072767;
        /* The mushrooms I seek can be found growing here in The Heartwood. Seek them out 
        and gather them.  You are running out of time. */
        public override object Uncomplete => 1072774;
        /* Have you gathered the mushrooms? */
        public override object Complete => 1074166;
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
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

    public class NeedsOfManyHeartwoodQuest : BaseQuest
    {
        public NeedsOfManyHeartwoodQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Cotton), "bale of cotton", 10, 0xDF9));

            AddReward(new BaseReward(1074872)); // The opportunity to learn the ways of the Arcanist.
        }

        public override QuestChain ChainID => QuestChain.Spellweaving;
        public override Type NextQuest => typeof(NeedsOfManyPartHeartwoodQuest);
        /* Needs of the Many - The Heartwood */
        public override object Title => 1072797;
        /* The way of the Arcanist involves cooperation with others and a strong 
        committment to the community of your people.  We have run low on the 
        cotton we use to pack wounds and our people have need.  Bring 10 
        bales of cotton to me. */
        public override object Description => 1072763;
        /* You endanger your progress along the path with your unwillingness. */
        public override object Refuse => 1072768;
        /* I care not where you acquire the cotton, merely that you provide it. */
        public override object Uncomplete => 1072775;
        /* Well, where are the cotton bales? */
        public override object Complete => 1074110;
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
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

    public class NeedsOfManyPartHeartwoodQuest : BaseQuest
    {
        public NeedsOfManyPartHeartwoodQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Board), "boards", 250, 0x1BD7));

            AddReward(new BaseReward(1074872)); // The opportunity to learn the ways of the Arcanist.
        }

        public override QuestChain ChainID => QuestChain.Spellweaving;
        public override Type NextQuest => typeof(MakingContributionHeartwoodQuest);
        /* Needs of the Many - The Heartwood */
        public override object Title => 1072797;
        /* We must look to the defense of our people! Bring boards for new arrows. */
        public override object Description => 1072764;
        /* The people have need of these items.  You are proving yourself inadequate 
        to the demands of a member of this community. */
        public override object Refuse => 1072769;
        /* The requirements are simple -- 250 boards. */
        public override object Uncomplete => 1072776;
        /* Well, where are the boards? */
        public override object Complete => 1074152;
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
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

    public class MakingContributionHeartwoodQuest : BaseQuest
    {
        public MakingContributionHeartwoodQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(SackFlour), "sack of flour", 1, 0x1039));
            AddObjective(new ObtainObjective(typeof(JarHoney), "jar of honey", 10, 0x9EC));
            AddObjective(new ObtainObjective(typeof(FishSteak), "fish steak", 20, 0x97B));

            AddReward(new BaseReward(1074872)); // The opportunity to learn the ways of the Arcanist.
        }

        public override QuestChain ChainID => QuestChain.Spellweaving;
        public override Type NextQuest => typeof(UnnaturalCreationsQuest);
        /* Making a Contribution - The Heartwood */
        public override object Title => 1072798;
        /* With health and defense assured, we need look to the need of the community 
        for food and drink.  We will feast on fish steaks, sweets, and wine.  You 
        will supply the ingredients, the cooks will prepare the meal.  As a Arcanist 
        relies upon others to build focus and lend their power to her workings, the 
        community needs the effort of all to survive. */
        public override object Description => 1072765;
        /* Do not falter now.  You have begun to show promise. */
        public override object Refuse => 1072770;
        /* Where are the items you've been tasked to supply for the feast? */
        public override object Uncomplete => 1072777;
        /* Ah good, you're back.  We're eager for the feast. */
        public override object Complete => 1074158;
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
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

    public class UnnaturalCreationsQuest : BaseQuest
    {
        public UnnaturalCreationsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(ExodusOverseer), "exodus overseers", 5));
            AddObjective(new SlayObjective(typeof(ExodusMinion), "exodus minions", 2));

            AddReward(new BaseReward(typeof(ArcaneCircleScroll), 1071026)); // Arcane Circle			
            AddReward(new BaseReward(typeof(GiftOfRenewalScroll), 1071027)); // Gift of Renewal
            AddReward(new BaseReward(typeof(SpellweavingBook), 1031600)); // Spellweaving Spellbook
        }

        public override QuestChain ChainID => QuestChain.Spellweaving;
        /* Unnatural Creations */
        public override object Title => 1072758;
        /* You have proven your desire to contribute to the community and serve the 
        people.  Now you must demonstrate your willingness to defend Sosaria from 
        the greatest blight that plagues her.  Unnatural creatures, brought to a 
        sort of perverted life, despoil our fair world.  Destroy them -- 5 Exodus 
        Overseers and 2 Exodus Minions. */
        public override object Description => 1072780;
        /* You must serve Sosaria with all your heart and strength.  
        Your unwillingness does not reflect favorably upon you. */
        public override object Refuse => 1072771;
        /* Every moment you procrastinate, these unnatural creatures damage Sosaria. */
        public override object Uncomplete => 1072779;
        /* Well done!  Well done, indeed.  You are worthy to become an arcanist! */
        public override object Complete => 1074167;
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
        }

        public override void GiveRewards()
        {
            Owner.Spellweaving = true;

            base.GiveRewards();
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