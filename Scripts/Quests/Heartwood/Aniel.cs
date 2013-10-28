using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class CircleOfLifeQuest : BaseQuest
    { 
        public CircleOfLifeQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(BogThing), "bog things", 8));
			
            this.AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Circle of Life */
        public override object Title
        {
            get
            {
                return 1073656;
            }
        }
        /* There's been a bumper crop of evil with the Bog Things in these parts, my friend. Though they are 
        foul creatures, they are also most fecund. Slay one and you make the land more fertile. Even better, 
        slay several and I will give you whatever coin I can spare. */
        public override object Description
        {
            get
            {
                return 1073695;
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
        /* Continue to seek and kill the Bog Things. */
        public override object Uncomplete
        {
            get
            {
                return 1073736;
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

    public class DustToDustQuest : BaseQuest
    { 
        public DustToDustQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(EarthElemental), "earth elementals", 12));
			
            this.AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Dust to Dust */
        public override object Title
        {
            get
            {
                return 1073074;
            }
        }
        /* You want to hear about trouble? I got trouble. How's angry piles of granite walking around for 
        trouble? Maybe they don't like the mining, maybe it's the farming. I don't know. All I know is 
        someone's got to turn them back to potting soil. And it'd be worth a pretty penny to the soul that 
        does it. */
        public override object Description
        {
            get
            {
                return 1073564;
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
        /* You got rocks in your head? I said to kill 12 earth elementals, okay? */
        public override object Uncomplete
        {
            get
            {
                return 1073584;
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

    public class ArchSupportQuest : BaseQuest
    { 
        public ArchSupportQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(FootStool), "foot stools", 10, 0xB5E));
			
            this.AddReward(new BaseReward(typeof(CarpentersCraftsmanSatchel), 1074282));
        }

        /* Arch Support */
        public override object Title
        {
            get
            {
                return 1073882;
            }
        }
        /* How clever humans are - to understand the need of feet to rest from time to time!  Imagine creating 
        a special stool just for weary toes.  I would like to examine and learn the secret of their making.  
        Would you bring me some foot stools to examine? */
        public override object Description
        {
            get
            {
                return 1074072;
            }
        }
        /* I will patiently await your reconsideration. */
        public override object Refuse
        {
            get
            {
                return 1073921;
            }
        }
        /* I will be in your debt if you bring me foot stools. */
        public override object Uncomplete
        {
            get
            {
                return 1073928;
            }
        }
        /* My thanks for your service. Now, I will show you something of elven carpentry. */
        public override object Complete
        {
            get
            {
                return 1073969;
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

    public class Aniel : MondainQuester
    { 
        [Constructable]
        public Aniel()
            : base("Aniel", "the aborist")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Aniel(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(GlassyFoeQuest),
                    typeof(CircleOfLifeQuest),
                    typeof(DustToDustQuest),
                    typeof(ArchSupportQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x8384;
            this.HairItemID = 0x2FC2;
            this.HairHue = 0x36;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new HalfApron(0x759));
            this.AddItem(new ElvenPants(0x901));
            this.AddItem(new LeafChest());
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