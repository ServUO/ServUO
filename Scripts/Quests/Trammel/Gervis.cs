using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class BatteredBucklersQuest : BaseQuest
    { 
        public BatteredBucklersQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Buckler), "buckler", 10, 0x1B73));
						
            this.AddReward(new BaseReward(typeof(SmithsSatchel), 1074282)); // Craftsman's Satchel
        }

        /* Battered Bucklers */
        public override object Title
        {
            get
            {
                return 1075511;
            }
        }
        /* Hey there! Yeah... you! Ya' any good with a hammer? Tell ya what, if yer thinking about tryin' some metal work, 
        and have a bit of skill, I can show ya how to bend it into shape. Just get some of those ingots there, and grab a 
        hammer and use it over here at this forge. I need a few more bucklers hammered out to fill this here order with...  
        hmmm about ten more. that'll give some taste of how to work the metal. */
        public override object Description
        {
            get
            {
                return 1075512;
            }
        }
        /* Not enough muscle on yer bones to use it? hmph, probably afraid of the sparks markin' up yer loverly skin... to 
        good for some honest labor... ha!... off with ya! */
        public override object Refuse
        {
            get
            {
                return 1075514;
            }
        }
        /* Come On! Whats that... a bucket? We need ten bucklers... not spitoons. */
        public override object Uncomplete
        {
            get
            {
                return 1075515;
            }
        }
        /* Thanks for the help. Here's something for ya to remember me by. */
        public override object Complete
        {
            get
            {
                return 1075516;
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

    public class Gervis : MondainQuester
    {
        [Constructable]
        public Gervis()
            : base("Gervis", "the blacksmith trainer")
        { 
            this.SetSkill(SkillName.Blacksmith, 65.0, 88.0);
        }

        public Gervis(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(BatteredBucklersQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83F5;
            this.HairItemID = 0x203B;
            this.HairHue = 0x5EC;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());			
            this.AddItem(new SmithHammer());
            this.AddItem(new Boots(0x3B2));
            this.AddItem(new ShortPants(0x1BB));
            this.AddItem(new Shirt(0x71F));
            this.AddItem(new FullApron(0x3B2));
			
            Item item;
			
            item = new LeatherGloves();
            item.Hue = 0x3B2;
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

    public class SmithsSatchel : Backpack
    {
        [Constructable]
        public SmithsSatchel()
            : base()
        {
            this.Hue = BaseReward.SatchelHue();
			
            this.AddItem(new IronIngot(10));
            this.AddItem(new SmithHammer());
        }

        public SmithsSatchel(Serial serial)
            : base(serial)
        {
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