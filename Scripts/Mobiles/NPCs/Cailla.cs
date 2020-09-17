using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class KingOfBearsQuest : BaseQuest
    {
        public KingOfBearsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(GrizzlyBear), "grizzly bears", 10));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* King of Bears */
        public override object Title => 1072996;
        /* A pity really.  With the balance of nature awry, we have no choice but to accept the responsibility 
        of making it all right.  It's all a part of the circle of life, after all. So, yes, the grizzly bears 
        are running rampant. There are far too many in the region.  Will you shoulder your obligations as a 
        higher life form? */
        public override object Description => 1073030;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
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

    public class SpecimensQuest : BaseQuest
    {
        public SpecimensQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(RedSolenWorker), "red solen workers", 12));
            AddObjective(new SlayObjective(typeof(BlackSolenWorker), "black solen workers", 12));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        public override bool AllObjectives => false;
        /* Specimens */
        public override object Title => 1072999;
        /* I admire them, you know.  The solen have their place -- regimented, organized.  They're fascinating 
        to watch with their constant strife between red and black.  I can't help but want to stir things up from 
        time to time.  And that's where you come in.  Kill either twelve red or twelve black solen workers and 
        let's see what happens next! */
        public override object Description => 1073032;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
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

    public class DeadManWalkingQuest : BaseQuest
    {
        public DeadManWalkingQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Zombie), "zombies", 5));
            AddObjective(new SlayObjective(typeof(Skeleton), "skeletons", 5));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Dead Man Walking */
        public override object Title => 1072983;
        /* Why?  I ask you why?  They walk around after they're put in the ground.  It's just wrong in so many ways.  
        Put them to proper rest, I beg you.  I'll find some way to pay you for the kindness. Just kill five zombies 
        and five skeletons. */
        public override object Description => 1073009;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
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

    public class SpiritsQuest : BaseQuest
    {
        public SpiritsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Spectre), "spectres", 15));
            AddObjective(new SlayObjective(typeof(Shade), "shades", 15));
            AddObjective(new SlayObjective(typeof(Wraith), "wraiths", 15));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override bool AllObjectives => false;
        /* Spirits */
        public override object Title => 1073076;
        /* It is a piteous thing when the dead continue to walk the earth. Restless spirits are known to inhabit these 
        parts, taking the lives of unwary travelers. It is about time a hero put the dead back in their graves. I'm sure 
        such a hero would be justly rewarded. */
        public override object Description => 1073566;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* The restless spirts still walk -- you must kill 15 of them. */
        public override object Uncomplete => 1073586;
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

    public class RollTheBonesQuest : BaseQuest
    {
        public RollTheBonesQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(PatchworkSkeleton), "patchwork skeletons", 8));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Roll the Bones */
        public override object Title => 1073002;
        /* Why?  I ask you why?  They walk around after they're put in the ground.  It's just wrong in so many ways.  
        Put them to proper rest, I beg you.  I'll find some way to pay you for the kindness. Just kill eight patchwork 
        skeletons. */
        public override object Description => 1073011;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
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

    public class ItsGhastlyJobQuest : BaseQuest
    {
        public ItsGhastlyJobQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Ghoul), "ghouls", 12));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* It's a Ghastly Job */
        public override object Title => 1073008;
        /* Why?  I ask you why?  They walk around after they're put in the ground.  It's just wrong in so many ways.  
        Put them to proper rest, I beg you.  I'll find some way to pay you for the kindness. Just kill twelve ghouls. */
        public override object Description => 1073012;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
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

    public class TroglodytesQuest : BaseQuest
    {
        public TroglodytesQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Troglodyte), "troglodytes", 12));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Troglodytes! */
        public override object Title => 1074688;
        /* Oh nevermind, you don't look capable of my task afterall.  Haha! What was I thinking - you could never handle 
        killing troglodytes.  It'd be suicide.  What?  I don't know, I don't want to be responsible ... well okay if 
        you're really sure? */
        public override object Description => 1074689;
        /* Probably the wiser course of action. */
        public override object Refuse => 1074690;
        /* You still need to kill those troglodytes, remember? */
        public override object Uncomplete => 1074691;
        public override bool CanOffer()
        {
            return MondainsLegacy.PaintedCaves;
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

    public class UnholyKnightsQuest : BaseQuest
    {
        public UnholyKnightsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(BoneKnight), "bone knights", 16));
            AddObjective(new SlayObjective(typeof(SkeletalKnight), "skeletal knights", 16));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override bool AllObjectives => false;
        /* Unholy Knights */
        public override object Title => 1073075;
        /* Please, hear me kind traveler. You know when a knight falls, sometimes they are cursed to roam the earth as 
        undead mockeries of their former glory? That is too grim a fate for even any knight to suffer! Please, put them 
        out of their misery. I will offer you what payment I can if you will end the torment of these undead wretches. */
        public override object Description => 1073565;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* Your task is not done. Continue putting the Skeleton and Bone Knights to rest. */
        public override object Uncomplete => 1073585;
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

    public class FeatherInYerCapQuest : BaseQuest
    {
        public FeatherInYerCapQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(SalivasFeather), "saliva's feather", 1));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* A Feather in Yer Cap */
        public override object Title => 1074738;
        /* I've seen how you strut about, as if you were something special. I have some news for you, you don't impress 
        me at all. It's not enough to have a fancy hat you know.  That may impress people in the big city, but not here. 
        If you want a reputation you have to climb a mountain, slay some great beast, and then write about it. Trust me, 
        it's a long process.  The first step is doing a great feat. If I were you, I'd go pluck a feather from the harpy 
        Saliva, that would give you a good start. */
        public override object Description => 1074737;
        /* The path to greatness isn't for everyone obviously. */
        public override object Refuse => 1074736;
        /* If you're going to get anywhere in the adventuring game, you have to take some risks.  A harpy, well, it's 
        bad, but it's not a dragon. */
        public override object Uncomplete => 1074735;
        /* The hero returns from the glorious battle and - oh, such a small feather? */
        public override object Complete => 1074734;
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

    public class TaleOfTailQuest : BaseQuest
    {
        public TaleOfTailQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(AbscessTail), "abscess' tail", 1));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* A Tale of Tail */
        public override object Title => 1074726;
        /* I've heard of you, adventurer.  Your reputation is impressive, and now I'll put it to the test. This is not 
        something I ask lightly, for this task is fraught with danger, but it is vital.  Seek out the vile hydra Abscess, 
        slay it, and return to me with it's tail. */
        public override object Description => 1074727;
        /* Well, the beast will still be there when you are ready I suppose. */
        public override object Refuse => 1074728;
        /* Em, I thought I had explained already.  Abscess, the hydra, you know? Lots of heads but just the one tail.  
        I need the tail. I have my reasons. Go go go. */
        public override object Uncomplete => 1074729;
        /* Ah, the tail.  You did it!  You know the rumours about dried ground hydra tail powder are all true?  
        Thank you so much! */
        public override object Complete => 1074730;
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

    public class TrogAndHisDogQuest : BaseQuest
    {
        public TrogAndHisDogQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Lurg), "lurg", 1));
            AddObjective(new SlayObjective(typeof(Grobu), "grobu", 1));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* A Trog and His Dog */
        public override object Title => 1074681;
        /* I don't know if you can handle it, but I'll give you a go at it. Troglodyte chief - name of Lurg and his mangy 
        wolf pet need killing. Do the deed and I'll reward you. */
        public override object Description => 1074680;
        /* Perhaps I thought too highly of you. */
        public override object Refuse => 1074655;
        /* The trog chief and his mutt should be easy enough to find. Just kill them and report back.  Easy enough. */
        public override object Uncomplete => 1074682;
        /* Not half bad.  Here's your prize. */
        public override object Complete => 1074683;
        public override bool CanOffer()
        {
            return MondainsLegacy.PaintedCaves;
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

    public class Cailla : MondainQuester
    {
        [Constructable]
        public Cailla()
            : base("Cailla", "the guard")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Cailla(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(KingOfBearsQuest),
                    typeof(SpecimensQuest),
                    typeof(DeadManWalkingQuest),
                    typeof(SpiritsQuest),
                    typeof(RollTheBonesQuest),
                    typeof(ItsGhastlyJobQuest),
                    typeof(TroglodytesQuest),
                    typeof(UnholyKnightsQuest),
                    typeof(FriendlyNeighborhoodSpiderkillerQuest),
                    typeof(FeatherInYerCapQuest),
                    typeof(TaleOfTailQuest),
                    typeof(TrogAndHisDogQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x876B;
            HairItemID = 0x2FCE;
            HairHue = 0x2C8;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new MagicalShortbow());
            AddItem(new HidePants());
            AddItem(new HidePauldrons());
            AddItem(new HideGloves());
            AddItem(new HideFemaleChest());
            AddItem(new WoodlandBelt());

            Item item;

            item = new RavenHelm
            {
                Hue = 0x1BB
            };
            AddItem(item);
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