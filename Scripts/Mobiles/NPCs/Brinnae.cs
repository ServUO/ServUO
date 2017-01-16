using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class EvilEyeQuest : BaseQuest
    { 
        public EvilEyeQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Gazer), "gazers", 12));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Evil Eye */
        public override object Title
        {
            get
            {
                return 1073084;
            }
        }
        /* Kind traveler, hear my plea. You know of the evil orbs? The wrathful eyes? Some call 
        them gazers? They must be a nest nearby, for they are tormenting us poor folk. We need 
        to drive back their numbers. But we are not strong enough to face such horrors ourselves, 
        we need a true hero. */
        public override object Description
        {
            get
            {
                return 1073574;
            }
        }
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse
        {
            get
            {
                return 1073580;
            }
        }
        /* Have you annihilated a dozen Gazers yet, kind traveler? */
        public override object Uncomplete
        {
            get
            {
                return 1073594;
            }
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

    public class ImpishDelightsQuest : BaseQuest
    { 
        public ImpishDelightsQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Imp), "imps", 12));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Impish Delights */
        public override object Title
        {
            get
            {
                return 1073077;
            }
        }
        /* Imps! Do you hear me? Imps! They're everywhere! They're in everything! Oh, don't be fooled 
        by their size - they vicious little devils! Half-sized evil incarnate, they are! Somebody 
        needs to send them back to where they came from, if you know what I mean. */
        public override object Description
        {
            get
            {
                return 1073567;
            }
        }
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse
        {
            get
            {
                return 1073580;
            }
        }
        /* Don't let the little devils scare you! You  kill 12 imps - then we'll talk reward. */
        public override object Uncomplete
        {
            get
            {
                return 1073587;
            }
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

    public class StirringTheNestQuest : BaseQuest
    { 
        public StirringTheNestQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(RedSolenQueen), "red solen queens", 3));
            this.AddObjective(new SlayObjective(typeof(BlackSolenQueen), "black solen queens", 3));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override bool AllObjectives
        {
            get
            {
                return false;
            }
        }
        /* Stirring the Nest */
        public override object Title
        {
            get
            {
                return 1073087;
            }
        }
        /* Were you the sort of child that enjoyed knocking over anthills? Well, perhaps you'd like 
        to try something a little bigger? There's a Solen nest nearby and I bet if you killed a queen 
        or two, it would be quite the sight to behold.  I'd even pay to see that - what do you say? */
        public override object Description
        {
            get
            {
                return 1073577;
            }
        }
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse
        {
            get
            {
                return 1073580;
            }
        }
        /* Dead Solen Queens isn't too much to ask, is it? */
        public override object Uncomplete
        {
            get
            {
                return 1073597;
            }
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

    public class UndeadMagesQuest : BaseQuest
    { 
        public UndeadMagesQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(BoneMagi), "bone mages", 10));
            this.AddObjective(new SlayObjective(typeof(SkeletalMage), "skeletal mages", 10));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override bool AllObjectives
        {
            get
            {
                return false;
            }
        }
        /* Undead Mages */
        public override object Title
        {
            get
            {
                return 1073080;
            }
        }
        /* Why must the dead plague the living? With their foul necromancy and dark sorceries, the undead 
        menace the countryside. I fear what will happen if no one is strong enough to face these nightmare 
        sorcerers and thin their numbers. */
        public override object Description
        {
            get
            {
                return 1073570;
            }
        }
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse
        {
            get
            {
                return 1073580;
            }
        }
        /* Surely, a brave soul like yourself can kill 10 Bone Magi and Skeletal Mages? */
        public override object Uncomplete
        {
            get
            {
                return 1073590;
            }
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

    public class TheAfterlifeQuest : BaseQuest
    { 
        public TheAfterlifeQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Mummy), "mummies", 15));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* The Afterlife */
        public override object Title
        {
            get
            {
                return 1073073;
            }
        }
        /* Nobody told me about the Mummy's Curse. How was I supposed to know you shouldn't disturb the tombs? 
        Oh, sure, now all I hear about is the curse of the vengeful dead. I'll tell you what - make a few of 
        these mummies go away and we'll keep this between you and me. */
        public override object Description
        {
            get
            {
                return 1073563;
            }
        }
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse
        {
            get
            {
                return 1073580;
            }
        }
        /* Uh, I don't think you're quite done killing Mummies yet. */
        public override object Uncomplete
        {
            get
            {
                return 1073583;
            }
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

    public class FriendlyNeighborhoodSpiderkillerQuest : BaseQuest
    { 
        public FriendlyNeighborhoodSpiderkillerQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(DreadSpider), "dread spiders", 8));
			
            this.AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Friendly Neighborhood Spider-killer */
        public override object Title
        {
            get
            {
                return 1073662;
            }
        }
        /* They aren't called Dread Spiders because they're fluffy and cuddly now, are they? No, there's nothing 
        appealing about those wretches so I sure wouldn't lose any sleep if you were to exterminate a few. I'd 
        even part with a generous amount of gold, I would. */
        public override object Description
        {
            get
            {
                return 1073701;
            }
        }
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse
        {
            get
            {
                return 1073733;
            }
        }
        /* Dread Spiders? I say keep exterminating the arachnid vermin. */
        public override object Uncomplete
        {
            get
            {
                return 1073742;
            }
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

    public class GargoylesWrathQuest : BaseQuest
    { 
        public GargoylesWrathQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(GargoyleEnforcer), "gargoyle enforcers", 6));
			
            this.AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Gargoyle's Wrath */
        public override object Title
        {
            get
            {
                return 1073658;
            }
        }
        /* It is regretable that the Gargoyles insist upon warring with us. Their Enforcers attack men on sight, despite 
        all efforts at reason. To help maintain order in this region, I have been authorized to encourage bounty hunters 
        to reduce their numbers. Eradicate their number and I will reward you handsomely. */
        public override object Description
        {
            get
            {
                return 1073697;
            }
        }
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse
        {
            get
            {
                return 1073733;
            }
        }
        /* I won't be able to pay you until you've gotten enough Gargoyle Enforcers. */
        public override object Uncomplete
        {
            get
            {
                return 1073738;
            }
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

    public class ThreeWishesQuest : BaseQuest
    { 
        public ThreeWishesQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Efreet), "efreets", 8));
			
            this.AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Three Wishes */
        public override object Title
        {
            get
            {
                return 1073660;
            }
        }
        /* If I had but one wish, it would be to rid myself of these dread Efreet! Fire and ash, they are cunning and 
        deadly! You look a brave soul - would you be interested in earning a rich reward for slaughtering a few of the 
        smoky devils? */
        public override object Description
        {
            get
            {
                return 1073699;
            }
        }
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse
        {
            get
            {
                return 1073733;
            }
        }
        /* Those smoky devils, the Efreets, are still about. */
        public override object Uncomplete
        {
            get
            {
                return 1073740;
            }
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

    public class ForkedTongueQuest : BaseQuest
    { 
        public ForkedTongueQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(OphidianKnight), "ophidian knight-errants", 10));
            this.AddObjective(new SlayObjective(typeof(OphidianMage), "ophidian mages", 10));
			
            this.AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        public override bool AllObjectives
        {
            get
            {
                return false;
            }
        }
        /* Forked Tongue */
        public override object Title
        {
            get
            {
                return 1073655;
            }
        }
        /* I must implore you, brave traveler, to do battle with the vile reptiles which haunt these parts. Those hideous 
        abominations, the Ophidians, are a blight across the land. If you were able to put down a host of the scaly 
        warriors, the Knights or the Avengers, I would forever be in your debt. */
        public override object Description
        {
            get
            {
                return 1073694;
            }
        }
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse
        {
            get
            {
                return 1073733;
            }
        }
        /* Have you killed the Ophidian Knights or Avengers? */
        public override object Uncomplete
        {
            get
            {
                return 1073735;
            }
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

    public class MongbatMenaceQuest : BaseQuest
    { 
        public MongbatMenaceQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Mongbat), "mongbats", 10));
            this.AddObjective(new SlayObjective(typeof(GreaterMongbat), "greater mongbats", 4));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Mongbat Menace! */
        public override object Title
        {
            get
            {
                return 1073003;
            }
        }
        /* I imagine you don't know about the mongbats.  Well, you may think you do, but I know more than just about anyone.  
        You see they come in two varieties ... the stronger and the weaker.  Either way, they're a menace.  Exterminate ten 
        of the weaker ones and four of the stronger and I'll pay you an honest wage. */
        public override object Description
        {
            get
            {
                return 1073033;
            }
        }
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse
        {
            get
            {
                return 1072270;
            }
        }
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete
        {
            get
            {
                return 1072271;
            }
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

    public class Brinnae : MondainQuester
    { 
        [Constructable]
        public Brinnae()
            : base("Brinnae", "the wise")
        { 
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Brinnae(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(EvilEyeQuest),
                    typeof(ImpishDelightsQuest),
                    typeof(StirringTheNestQuest),
                    typeof(UndeadMagesQuest),
                    typeof(TheAfterlifeQuest),
                    typeof(FriendlyNeighborhoodSpiderkillerQuest),
                    typeof(GargoylesWrathQuest),
                    typeof(ThreeWishesQuest),
                    typeof(ForkedTongueQuest),
                    typeof(CircleOfLifeQuest),
                    typeof(MongbatMenaceQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x8382;
            this.HairItemID = 0x2FD0;
            this.HairHue = 0x852;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x1BB));
            this.AddItem(new LeafArms());
            this.AddItem(new FemaleLeafChest());
            this.AddItem(new HidePants());
            this.AddItem(new ElvenCompositeLongbow());
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