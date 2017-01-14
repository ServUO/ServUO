using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class TheSongOfTheWindQuest : BaseQuest
    { 
        public TheSongOfTheWindQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(FancyWindChimes), "fancy wind chimes", 10, 0x2833));
			
            this.AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* The Song of the Wind */
        public override object Title
        {
            get
            {
                return 1073910;
            }
        }
        /* To give voice to the passing wind, this is an idea worthy of an elf! Friend, bring me some of the amazing fancy 
        wind chimes so that I may listen to the song of the passing breeze. Do this, and I will share with you treasured 
        elven secrets. */
        public override object Description
        {
            get
            {
                return 1074100;
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
        /* I will be in your debt if you bring me fancy wind chimes. */
        public override object Uncomplete
        {
            get
            {
                return 1073956;
            }
        }
        /* Such a delightful sound, I think I shall never tire of it. */
        public override object Complete
        {
            get
            {
                return 1073980;
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

    public class BeerGogglesQuest : BaseQuest
    { 
        public BeerGogglesQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(BarrelTap), "barrel tap", 25, 0x1004));
			
            this.AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* Beer Goggles */
        public override object Title
        {
            get
            {
                return 1073895;
            }
        }
        /* Oh, the deviltry! Why would humans lock their precious liquors inside a wooden coffin? I understand I need a "keg tap" 
        to access the golden brew within such a wooden abomination. Perhaps, if you could bring me such a tap, we could share a 
        drink and I could teach you. */
        public override object Description
        {
            get
            {
                return 1074085;
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
        /* I will be in your debt if you bring me barrel taps. */
        public override object Uncomplete
        {
            get
            {
                return 1073941;
            }
        }
        /* My thanks for your service.  Here is something for you to enjoy. */
        public override object Complete
        {
            get
            {
                return 1073971;
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

    public class MessageInBottleQuest : BaseQuest
    { 
        public MessageInBottleQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Bottle), "empty bottles", 50, 0xF0E));
			
            this.AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* Message in a Bottle */
        public override object Title
        {
            get
            {
                return 1073894;
            }
        }
        /* We elves are interested in trading our wines with humans but we understand human usually trade such brew in strange transparent 
        bottles. If you could provide some of these empty glass bottles, I might engage in a bit of elven winemaking. */
        public override object Description
        {
            get
            {
                return 1074084;
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
        /* I will be in your debt if you bring me empty bottles. */
        public override object Uncomplete
        {
            get
            {
                return 1073940;
            }
        }
        /* My thanks for your service.  Here is something for you to enjoy. */
        public override object Complete
        {
            get
            {
                return 1073971;
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

    public class Tholef : MondainQuester
    { 
        [Constructable]
        public Tholef()
            : base("Tholef", "the grape tender")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Tholef(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(TheSongOfTheWindQuest),
                    typeof(BeerGogglesQuest),
                    typeof(MessageInBottleQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Elf;
			
            this.Hue = 0x876C;
            this.HairItemID = 0x2FC2;
            this.HairHue = 0x15A;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Sandals(0x901));
            this.AddItem(new ShortPants(0x28C));
            this.AddItem(new Shirt(0x28C));
            this.AddItem(new FullApron(0x72B));
			
            Item item;
			
            item = new LeafArms();
            item.Hue = 0x28C;					
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