using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class VilePoisonQuest : BaseQuest
    {
        public VilePoisonQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(TaintedTreeSample), "tainted tree sample", 1, typeof(Ioseph), "Ioseph (Jhelom)"));

            AddReward(new BaseReward(1074962)); // A step closer to entering Blighted Grove.
        }

        public override QuestChain ChainID => QuestChain.BlightedGrove;
        public override Type NextQuest => typeof(RockAndHardPlaceQuest);
        /* Vile Poison */
        public override object Title => 1074950;
        /* Heya!  I'm sure glad to see you.  Listen I'm in a bit of a bind here.  I'm supposed to be gathering 
        poisoned water at the base of that corrupted tree there, but I can't get in under the roots to get a 
        good sample.  The branches and brush are so tainted that they can't be cut, burned or even magically 
        passed.  It's put my work at a real standstill.  If you help me out, I'll help you get in there too.  
        Whadda ya say? */
        public override object Description => 1074956;
        /* Okay.  If you change your mind, I'll probably still be stuck here trying to get in. */
        public override object Refuse => 1074964;
        /* My friend, Iosep, is a weaponsmith in Jhelom.  If anyone can help us, he can! */
        public override object Uncomplete => 1074968;
        /* Greetings.  What have you there?  Ah, a sample from a poisonous tree, you say?  My friend Jamal 
        sent you?  Well, let me see that then, and we'll get to work. */
        public override object Complete => 1074991;
        public override bool CanOffer()
        {
            return MondainsLegacy.BlightedGrove;
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

    public class RockAndHardPlaceQuest : BaseQuest
    {
        public RockAndHardPlaceQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Granite), "rocks", 4, 0x1779));
            AddObjective(new ObtainObjective(typeof(BlueDiamond), "blue diamonds", 2, 0x3198));

            AddReward(new BaseReward(1074962)); // A step closer to entering Blighted Grove.
        }

        public override QuestChain ChainID => QuestChain.BlightedGrove;
        public override Type NextQuest => typeof(SympatheticMagicQuest);
        /* A Rock and a Hard Place */
        public override object Title => 1074951;
        /* Hmm, I've never even heard of something that can damage diamond like that.  I guess we'll have to go with plan 
        B.  Let's try something similar.  Sometimes there's a natural immunity to be found when you use a substance that's 
        like the one you're trying to cut.  A sort of "sympathetic" thing.  Y'know? */
        public override object Description => 1074958;
        /* Sure, no problem.  I thought you were interested in figuring this out. */
        public override object Refuse => 1074965;
        /* If you're a miner, you should have no trouble getting that stuff.  If not, you can probably buy some 
        samples from a miner? */
        public override object Uncomplete => 1074969;
        /* Have you got the granite and diamonds?  Great, let me see them and we'll see what effect this venom has upon them. */
        public override object Complete => 1074992;
        public override bool CanOffer()
        {
            return MondainsLegacy.BlightedGrove;
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

    public class SympatheticMagicQuest : BaseQuest
    {
        public SympatheticMagicQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(BarkFragment), "bark", 10, 0x318F));

            AddReward(new BaseReward(1074962)); // A step closer to entering Blighted Grove.
        }

        public override QuestChain ChainID => QuestChain.BlightedGrove;
        public override Type NextQuest => typeof(AlreadyDeadQuest);
        /* Sympathetic Magic */
        public override object Title => 1074952;
        /* This is some nasty stuff, that's for certain.  I don't even want to think about what sort of blight 
        caused this venomous reaction from that old tree.  Let's get to work … we'll need to try something really 
        hard but still workable as our base material.  Nothing's harder than stone and diamond.  Let's try them first. */
        public override object Description => 1074957;
        /* Sure, no problem.  I thought you were interested in figuring this out. */
        public override object Refuse => 1074965;
        /* I think a lumberjack can help supply bark. */
        public override object Uncomplete => 1074970;
        /* You're back with the bark already?  Terrific!  I bet this will do the trick. */
        public override object Complete => 1074993;
        public override bool CanOffer()
        {
            return MondainsLegacy.BlightedGrove;
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

    public class AlreadyDeadQuest : BaseQuest
    {
        public AlreadyDeadQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Bone), "workable samples", 10));

            AddReward(new BaseReward(1074962)); // A step closer to entering Blighted Grove.
        }

        public override QuestChain ChainID => QuestChain.BlightedGrove;
        public override Type NextQuest => typeof(EurekaQuest);
        /* Already Dead */
        public override object Title => 1074953;
        /* Amazing!  The bark was reduced to ash in seconds.  Whatever this taint is, it plays havok with living things.  
        And of course, it took the edge off both diamonds and granite even faster.  What we need is something workable but 
        dead; something that can hold an edge without melting.  See what you can come up with, please. */
        public override object Description => 1074959;
        /* Sure, no problem.  I thought you were interested in figuring this out. */
        public override object Refuse => 1074965;
        /* I'm thinking we need something fairly brittle or it won't hold an edge.  And, it can't be alive, of course. */
        public override object Uncomplete => 1074971;
        /* Great thought!  Bone might just do the trick. */
        public override object Complete => 1074994;
        public override bool CanOffer()
        {
            return MondainsLegacy.BlightedGrove;
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

    public class EurekaQuest : BaseQuest
    {
        public EurekaQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(SealedNotesForJamal), "sealed notes for jamal", 1, typeof(Jamal), "Jamal (near Blighted Grove)"));

            AddReward(new BaseReward(1074962)); // A step closer to entering Blighted Grove.
        }

        public override QuestChain ChainID => QuestChain.BlightedGrove;
        public override Type NextQuest => typeof(SubContractingQuest);
        /* Eureka! */
        public override object Title => 1074954;
        /* We're in business!  I've put together the instructions for chopping sort of sword, in the style 
        of one of those new-fangled elven machetes.  Take those back to Jamal for me, if you would. */
        public override object Description => 1074960;
        /* Well, okay.  I guess I thought you'd want to see this through. */
        public override object Refuse => 1074966;
        /* I'm sure Jamal is eager to get this information.  He's probably still hanging around near that 
        big old blighted tree. */
        public override object Uncomplete => 1074972;
        /* Heya!  You're back.  Was Iosep able to help?  Let me see what he's sent. */
        public override object Complete => 1074995;
        public override bool CanOffer()
        {
            return MondainsLegacy.BlightedGrove;
        }

        public override void OnCompleted()
        {
            if (Owner.Skills.Blacksmith.Value >= 45.0)
            {
                Owner.AcquireRecipe((int)Craft.SmithRecipes.BoneMachete);
                Owner.SendLocalizedMessage(1075006); // You have learned how to smith a bone handled machete!
            }
            else
                Owner.SendLocalizedMessage(1075005); // You observe carefully but you can't grasp the complexities of smithing a bone handled machete.
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

    public class SubContractingQuest : BaseQuest
    {
        public SubContractingQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(SamplesOfCorruptedWater), "samples of corrupted water", 3));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        public override QuestChain ChainID => QuestChain.BlightedGrove;
        /* Sub Contracting */
        public override object Title => 1074955;
        /* Wonderful!  Now we can both get in there!  Let me show you these instructions for making this machete.  
        If you're not skilled in smithing, I'm not sure how much sense it will make though.  Listen, if you're 
        heading in there anyway … maybe you'd do me one more favor?  I'm ah ... buried in work out here ... so if 
        you'd go in and get me a few water samples, I'd be obliged. */
        public override object Description => 1074961;
        /* Oh.  Right, I guess you're really ... ah ... busy too. */
        public override object Refuse => 1074967;
        /* Once you're inside, look for places where the water has twisted and warped the natural creatures. */
        public override object Uncomplete => 1074973;
        /* I hear sloshing ... that must mean you've got my water samples.  Whew, I'm so glad you braved the 
        dangers in there ... I mean, I would have but I'm so busy out here.  Here's your reward! */
        public override object Complete => 1074996;
        public override bool CanOffer()
        {
            return MondainsLegacy.BlightedGrove;
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