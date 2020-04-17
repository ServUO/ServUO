using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class MaraudersQuest : BaseQuest
    {
        public MaraudersQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Ogre), "ogres", 10, "Sanctuary"));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override QuestChain ChainID => QuestChain.Marauders;
        public override Type NextQuest => typeof(TheBrainsOfTheOperationQuest);
        /* Marauders */
        public override object Title => 1072374;
        /* What a miserable place we live in.  Look around you at the changes we've wrought. The trees 
        are sprouting leaves once more and the grass is reclaiming the blood-soaked soil.  Who would 
        have imagined we'd find ourselves here?  Our "neighbors" are anything but friendly and those 
        ogres are the worst of the lot. Maybe you'd be interested in helping our community by disposing 
        of some of our least amiable neighbors? */
        public override object Description => 1072686;
        /* I quite understand your reluctance.  If you reconsider, I'll be here. */
        public override object Refuse => 1072687;
        /* You can't miss those ogres, they're huge and just outside the gates here. */
        public override object Uncomplete => 1072688;
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

    public class TheBrainsOfTheOperationQuest : BaseQuest
    {
        public TheBrainsOfTheOperationQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(OgreLord), "ogre lords", 10, "Sanctuary"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        public override QuestChain ChainID => QuestChain.Marauders;
        public override Type NextQuest => typeof(TheBrawnQuest);
        /* The Brains of the Operation */
        public override object Title => 1072692;
        /* *sigh*  We have so much to do to clean this area up.  Even the fine work you did on those ogres didn't 
        have much of an impact on the community.  It's the ogre lords that direct the actions of the other ogres, 
        let's strike at the leaders and perhaps that will thwart the miserable curs. */
        public override object Description => 1072707;
        /* Reluctance doesn't become a hero like you.  But, as you wish. */
        public override object Refuse => 1072708;
        /* Ogre Lords are pretty easy to recognize.  They're the ones ordering the other ogres about in a lordly 
        manner.  Striking down their leadership will throw the ogres into confusion and dismay! */
        public override object Uncomplete => 1072709;
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

    public class TheBrawnQuest : BaseQuest
    {
        public TheBrawnQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Cyclops), "cyclops", 6, "Sanctuary"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        public override QuestChain ChainID => QuestChain.Marauders;
        public override Type NextQuest => typeof(TheBiggerTheyAreQuest);
        /* The Brawn */
        public override object Title => 1072693;
        /* Inconceiveable!  We've learned that the ogre leadership has recruited some heavy-duty guards to their 
        cause.  I've never personally fought a cyclopian warrior, but I'm sure you could easily best a few and 
        report back how much trouble they'll cause to our growing community? */
        public override object Description => 1072710;
        /* Oh, I see.  *sigh*  Perhaps I overestimated your abilities. */
        public override object Refuse => 1072711;
        /* Make sure you fully assess all of the cyclopian tactical abilities! */
        public override object Uncomplete => 1072712;
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

    public class TheBiggerTheyAreQuest : BaseQuest
    {
        public TheBiggerTheyAreQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Titan), "titans", 3, "Sanctuary"));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        public override QuestChain ChainID => QuestChain.Marauders;
        /* The Bigger They Are ... */
        public override object Title => 1072694;
        /* The ogre insurgency has taken a turn for the worse! I've just been advised that the titans have concluded 
        their discussions with the ogres and they've allied. We have virtually no information about titans.  Engage 
        them and appraise their mettle. */
        public override object Description => 1072713;
        /* Certainly.  You've done enough to merit a breather.  When you're ready for more, report back to me. */
        public override object Refuse => 1072714;
        /* Those titans don't skulk very well.  You should be able to track them easily ... their footsteps are 
        easily the largest around. */
        public override object Uncomplete => 1072715;
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