using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class SquishyQuest : BaseQuest
    { 
        public SquishyQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Slime), "slimes", 12));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Squishy */
        public override object Title
        {
            get
            {
                return 1072998;
            }
        }
        /* Have you ever seen what a slime can do to good gear?  Well, it's not pretty, let me tell 
        you!  If you take on my task to destroy twelve of them, bear that in mind.  They'll corrode 
        your equipment faster than anything. */
        public override object Description
        {
            get
            {
                return 1073031;
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

    public class BigJobQuest : BaseQuest
    { 
        public BigJobQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Ogre), "ogres", 5));
            this.AddObjective(new SlayObjective(typeof(Ettin), "ettins", 5));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* A Big Job */
        public override object Title
        {
            get
            {
                return 1072988;
            }
        }
        /* It's a big job but you look to be just the adventurer to do it! I'm so glad you came by ... 
        I'm paying well for the death of five ogres and five ettins.  Hop to it, if you're so inclined. */
        public override object Description
        {
            get
            {
                return 1073017;
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

    public class TrollingForTrollsQuest : BaseQuest
    { 
        public TrollingForTrollsQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Troll), "trolls", 10));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Trolling for Trolls */
        public override object Title
        {
            get
            {
                return 1072985;
            }
        }
        /* They may not be bright, but they're incredibly destructive. Kill off ten trolls and I'll 
        consider it a favor done for me. */
        public override object Description
        {
            get
            {
                return 1073014;
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

    public class OrcSlayingQuest : BaseQuest
    { 
        public OrcSlayingQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Orc), "orcs", 8));
            this.AddObjective(new SlayObjective(typeof(OrcCaptain), "orc captains", 4));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Orc Slaying */
        public override object Title
        {
            get
            {
                return 1072986;
            }
        }
        /* Those green-skinned freaks have run off with more of my livestock.  I want an orc scout 
        killed for each sheep I lost and an orc for each chicken.  So that's four orc scouts and 
        eight orcs I'll pay you to slay. */
        public override object Description
        {
            get
            {
                return 1073015;
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

    public class ColdHeartedQuest : BaseQuest
    { 
        public ColdHeartedQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(IceSerpent), "giant ice serpents", 6));
            this.AddObjective(new SlayObjective(typeof(FrostSpider), "frost spiders", 6));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Cold Hearted */
        public override object Title
        {
            get
            {
                return 1072991;
            }
        }
        /* It's a big job but you look to be just the adventurer to do it! I'm so glad you came by ... I'm paying 
        well for the death of six giant ice serpents and six frost spiders.  Hop to it, if you're so inclined. */
        public override object Description
        {
            get
            {
                return 1073027;
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

    public class ForkedTonguesQuest : BaseQuest
    { 
        public ForkedTonguesQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Lizardman), "lizardmen", 10));
			
            this.AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Forked Tongues */
        public override object Title
        {
            get
            {
                return 1072984;
            }
        }
        /* You can't trust them, you know.  Lizardmen I mean.  They have forked tongues ... and you know 
        what that means.  Exterminate ten of them and I'll reward you. */
        public override object Description
        {
            get
            {
                return 1073013;
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

    public class Cloorne : MondainQuester
    { 
        [Constructable]
        public Cloorne()
            : base("Cloorne", "the expeditionist")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Cloorne(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(SquishyQuest),
                    typeof(BigJobQuest),
                    typeof(TrollingForTrollsQuest),
                    typeof(OrcSlayingQuest),
                    typeof(ColdHeartedQuest),
                    typeof(CreepyCrawliesQuest),
                    typeof(ForkedTonguesQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8376;
            this.HairItemID = 0x2FBF;
            this.HairHue = 0x386;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x3B3));
            this.AddItem(new WingedHelm());
            this.AddItem(new RadiantScimitar());
			
            Item item;
			
            item = new WoodlandLegs();
            item.Hue = 0x732;					
            this.AddItem(item); 
			
            item = new HideChest();
            item.Hue = 0x727;					
            this.AddItem(item); 
			
            item = new LeafArms();
            item.Hue = 0x749;					
            this.AddItem(item); 
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