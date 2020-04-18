using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class TheyreBreedingLikeRabbitsQuest : BaseQuest
    {
        public TheyreBreedingLikeRabbitsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Rabbit), "rabbits", 10));

            AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* They're Breeding Like Rabbits */
        public override object Title => 1072244;
        /* Aaaahhhh!  They're everywhere!  Aaaaahhh!  Ahem.  Actually, friend, how do you feel about rabbits?  
        Well, we're being overrun by them.  We're finding fuzzy bunnies everywhere. Aaaaahhh! */
        public override object Description => 1072259;
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

    public class TheyllEatAnythingQuest : BaseQuest
    {
        public TheyllEatAnythingQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Pig), "pigs", 10));

            AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* They'll Eat Anything */
        public override object Title => 1072248;
        /* Pork is the fruit of the land!  You can barbeque it, boil it, bake it, sautee it.  There's pork kebabs, 
        pork creole, pork gumbo, pan fried, deep fried, stir fried.  There's apple pork, peppered pork, pork soup, 
        pork salad, pork and potatoes, pork burger, pork sandwich, pork stew, pork chops, pork loins, shredded pork.  
        So, lets get some piggies butchered! */
        public override object Description => 1072262;
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

    public class NoGoodFishStealingQuest : BaseQuest
    {
        public NoGoodFishStealingQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Walrus), "walruses", 10));

            AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* No Good, Fish Stealing ... */
        public override object Title => 1072251;
        /* Mighty creatures they are, aye.  Fierce and strong, can't blame 'em for wanting to feed themselves an' all.  
        Blame or no, they're eating all the fish up, so they got to go.  Lend a hand? */
        public override object Description => 1072265;
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

    public class HeroInTheMakingQuest : BaseQuest
    {
        public HeroInTheMakingQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Mongbat), "mongbats", 10));

            AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* A Hero in the Making */
        public override object Title => 1072246;
        /* Are you new around here?  Well, nevermind that.  You look ready for adventure, I can see the gleam of glory in 
        your eyes!  Nothing is more valiant, more noble, more praiseworthy than mongbat slaying.*/
        public override object Description => 1072257;
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

    public class BullfightingSortOfQuest : BaseQuest
    {
        public BullfightingSortOfQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Bull), "bulls", 10));

            AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* Bullfighting ... Sort Of */
        public override object Title => 1072247;
        /* You there! Yes, you.  Listen, I've got a little problem on my hands, but a brave, bold hero like yourself should find 
        it a snap to solve.  Bottom line -- we need some of the bulls in the area culled.  You're welcome to any meat or hides, 
        and of course, I'll give you a nice reward. */
        public override object Description => 1072254;
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

    public class FineFeastQuest : BaseQuest
    {
        public FineFeastQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Sheep), "sheep", 10));

            AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* A Fine Feast. */
        public override object Title => 1072243;
        /* Mmm, I do love mutton!  It's slaughtering time again and my usual hirelings haven't turned up.  I've arranged for a butcher 
        to come by and cut everything up but the basic sheep killing part I haven't gotten worked out yet.  Are you up for the task? */
        public override object Description => 1072261;
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

    public class ForcedMigrationQuest : BaseQuest
    {
        public ForcedMigrationQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Bird), "birds", 10));

            AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* Forced Migration */
        public override object Title => 1072250;
        /* Chirp chirp ... tweet chirp.  Tra la la.  Bloody birds and their blasted noise.  I've tried everything but 
        they just won't stop that infernal clamor.  Return me to blessed silence and I'll make it worth your while. */
        public override object Description => 1072264;
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

    public class FilthyPestsQuest : BaseQuest
    {
        public FilthyPestsQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Sewerrat), "sewer rats", 10));

            AddReward(new BaseReward(typeof(SmallTrinketBag), 1072268));
        }

        /* Filthy Pests! */
        public override object Title => 1072242;
        /* They're everywhere I tell you!  They crawl in the walls, they scurry in the bushes.  Disgusting critters.  
        Say ... I don't suppose you're up for some sewer rat killing?  Sewer rats now, not any other kind of squeaker 
        will do. */
        public override object Description => 1072253;
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

    public class Saril : MondainQuester
    {
        [Constructable]
        public Saril()
            : base("Saril", "the guard")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Saril(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(TheyreBreedingLikeRabbitsQuest),
                    typeof(ThinningTheHerdQuest),
                    typeof(TheyllEatAnythingQuest),
                    typeof(NoGoodFishStealingQuest),
                    typeof(HeroInTheMakingQuest),
                    typeof(WildBoarCullQuest),
                    typeof(ForcedMigrationQuest),
                    typeof(BullfightingSortOfQuest),
                    typeof(FineFeastQuest),
                    typeof(OverpopulationQuest),
                    typeof(DeadManWalkingQuest),
                    typeof(ForkedTonguesQuest),
                    typeof(TrollingForTrollsQuest),
                    typeof(FilthyPestsQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x8361;
            HairItemID = 0x2FC1;
            HairHue = 0x127;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x3B2));
            AddItem(new WingedHelm());
            AddItem(new RadiantScimitar());
            AddItem(new WoodlandLegs());
            AddItem(new WoodlandArms());
            AddItem(new WoodlandChest());
            AddItem(new WoodlandBelt());
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