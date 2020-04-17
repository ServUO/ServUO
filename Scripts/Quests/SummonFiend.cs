using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class FiendishFriendsQuest : BaseQuest
    {
        public FiendishFriendsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Imp), "imps", 50));

            AddReward(new BaseReward(1074873)); // The opportunity to prove yourself worthy of learning to Summon Fiends. (Sufficient spellweaving skill is required to cast the spell)
        }

        public override QuestChain ChainID => QuestChain.SummonFiend;
        public override Type NextQuest => typeof(CrackingTheWhipQuest);
        /* Fiendish Friends */
        public override object Title => 1074283;
        /* It is true that a skilled arcanist can summon and dominate an imp to serve at their pleasure.  To do such 
        at thing though, you must master the miserable little fiends utterly by demonstrating your superiority.  Rough 
        them up some - kill a few.  That will do the trick. */
        public override object Description => 1074285;
        /* You're probably right.  They're not worth the effort. */
        public override object Refuse => 1074287;
        /* Surely you're not having difficulties swatting down those annoying pests? */
        public override object Uncomplete => 1074289;
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

    public class CrackingTheWhipQuest : BaseQuest
    {
        public CrackingTheWhipQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(StoutWhip), "stout whip", 1, 0x166F));

            AddReward(new BaseReward(1075028)); // A step closer to learning to summon and control fiends.
        }

        public override QuestChain ChainID => QuestChain.SummonFiend;
        public override Type NextQuest => typeof(IronWillQuest);
        /* Cracking the Whip */
        public override object Title => 1074295;
        /* Now that you've shown those mini pests your might, you should collect suitable implements to 
        use to train your summoned pet.  I suggest a stout whip. */
        public override object Description => 1074300;
        /* Heh. Changed your mind, eh? */
        public override object Refuse => 1074313;
        /* Well, hurry up.  If you don't get a whip how do you expect to control the little devil? */
        public override object Uncomplete => 1074317;
        /* That's a well-made whip.  No imp will ignore the sting of that lash. */
        public override object Complete => 1074321;
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

    public class IronWillQuest : BaseQuest
    {
        public IronWillQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(ArcaneDaemon), "arcane daemon", 1));

            AddReward(new BaseReward(typeof(SummonFiendScroll), 1071033));
        }

        public override QuestChain ChainID => QuestChain.SummonFiend;
        public override Type NextQuest => typeof(TheBrainsOfTheOperationQuest);
        /* Iron Will */
        public override object Title => 1074296;
        /* Now you just need to make the little buggers fear you -- if you can slay an arcane daemon, 
        you'll earn their subservience. */
        public override object Description => 1074302;
        /* If you're not up for it, so be it. */
        public override object Refuse => 1074314;
        /* You need to vanquish an arcane daemon before the imps will fear you properly. */
        public override object Uncomplete => 1074318;
        public override bool CanOffer()
        {
            return MondainsLegacy.Spellweaving;
        }

        public override void GiveRewards()
        {
            /* You've demonstrated your strength, got a means of control, and taught the imps to fear 
            you.  You're ready now to summon them. */
            Owner.SendLocalizedMessage(1074322, null, 0x2A);

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