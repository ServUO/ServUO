using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class DisciplineQuest : BaseQuest
    {
        public DisciplineQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Rat), "rats", 50, "Sanctuary"));

            AddReward(new BaseReward(1074872)); // The opportunity to learn the ways of the Arcanist.
        }

        public override QuestChain ChainID => QuestChain.SpellweavingS;
        public override Type NextQuest => typeof(NeedsOfTheManySanctuaryQuest);
        /* Discipline */
        public override object Title => 1072752;
        /* Learning to weave spells and control the forces of nature requires sacrifice, discipline, focus, and an unwavering dedication 
        to Sosaria herself.  We do not teach the unworthy.  They do not comprehend the lessons nor the dedication required.  If you would 
        walk the path of the Arcanist, then you must do as I require without hesitation or question.  Your first task is to rid our home 
        of rats ... 50 of them in the next hour. */
        public override object Description => 1072761;
        /* *nods* Not everyone has the temperment to undertake the way of the Arcanist. */
        public override object Refuse => 1072767;
        /* You waste my time.  The task is simple. Kill 50 rats in an hour. */
        public override object Uncomplete => 1072773;
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

    public class NeedsOfTheManySanctuaryQuest : BaseQuest
    {
        public NeedsOfTheManySanctuaryQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Cotton), "bale of cotton", 10, 0xDF9));

            AddReward(new BaseReward(1074872)); // The opportunity to learn the ways of the Arcanist.
        }

        public override QuestChain ChainID => QuestChain.SpellweavingS;
        public override Type NextQuest => typeof(MakingContributionSanctuaryQuest);
        /* Needs of the Many - Sanctuary */
        public override object Title => 1072754;
        /* The way of the Arcanist involves cooperation with others and a strong committment to the community of your 
        people.  We have run low on the cotton we use to pack wounds and our people have need.  Bring 10 bales of 
        cotton to me. */
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

    public class MakingContributionSanctuaryQuest : BaseQuest
    {
        public MakingContributionSanctuaryQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Board), "boards", 250, 0x1BD7));

            AddReward(new BaseReward(1074872)); // The opportunity to learn the ways of the Arcanist.
        }

        public override QuestChain ChainID => QuestChain.SpellweavingS;
        public override Type NextQuest => typeof(SuppliesForSanctuaryQuest);
        /* Making a Contribution - Sanctuary */
        public override object Title => 1072755;
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

    public class SuppliesForSanctuaryQuest : BaseQuest
    {
        public SuppliesForSanctuaryQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(SackFlour), "sack of flour", 1, 0x1039));
            AddObjective(new ObtainObjective(typeof(JarHoney), "jar of honey", 10, 0x9EC));
            AddObjective(new ObtainObjective(typeof(FishSteak), "fish steak", 20, 0x97B));

            AddReward(new BaseReward(1074872)); // The opportunity to learn the ways of the Arcanist.
        }

        public override QuestChain ChainID => QuestChain.SpellweavingS;
        public override Type NextQuest => typeof(TheHumanBlightQuest);
        /* Supplies for Sanctuary */
        public override object Title => 1072756;
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

    public class TheHumanBlightQuest : BaseQuest
    {
        public TheHumanBlightQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(SeveredHumanEars), "severed human ears", 30, 0x312F));

            AddReward(new BaseReward(typeof(ArcaneCircleScroll), 1071026)); // Arcane Circle			
            AddReward(new BaseReward(typeof(GiftOfRenewalScroll), 1071027)); // Gift of Renewal
            AddReward(new BaseReward(typeof(SpellweavingBook), 1031600)); // Spellweaving Spellbook
        }

        public override QuestChain ChainID => QuestChain.SpellweavingS;
        /* The Human Blight */
        public override object Title => 1072757;
        /* You have proven your desire to contribute to the community and serve the people.  Now you must 
        demonstrate your willingness to defend Sosaria from the greatest blight that plagues her.  The 
        human vermin that have spread as a disease, despoiling the land are the greatest blight we face.  
        Kill humans and return to me the proof of your actions. Bring me 30 human ears. */
        public override object Description => 1072766;
        /* You must serve Sosaria with all your heart and strength.  Your unwillingness does not reflect 
        favorably upon you. */
        public override object Refuse => 1072771;
        /* Why do you delay?  The human blight must be averted. */
        public override object Uncomplete => 1072778;
        /* I will take the ears you have collected now.  Hand them here. */
        public override object Complete => 1074160;
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