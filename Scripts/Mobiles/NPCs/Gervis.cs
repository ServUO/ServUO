using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class BatteredBucklersQuest : BaseQuest
    {
        public BatteredBucklersQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Buckler), "buckler", 10, 0x1B73));

            AddReward(new BaseReward(typeof(SmithsSatchel), 1074282)); // Craftsman's Satchel
        }

        /* Battered Bucklers */
        public override object Title => 1075511;
        /* Hey there! Yeah... you! Ya' any good with a hammer? Tell ya what, if yer thinking about tryin' some metal work, 
        and have a bit of skill, I can show ya how to bend it into shape. Just get some of those ingots there, and grab a 
        hammer and use it over here at this forge. I need a few more bucklers hammered out to fill this here order with...  
        hmmm about ten more. that'll give some taste of how to work the metal. */
        public override object Description => 1075512;
        /* Not enough muscle on yer bones to use it? hmph, probably afraid of the sparks markin' up yer loverly skin... to 
        good for some honest labor... ha!... off with ya! */
        public override object Refuse => 1075514;
        /* Come On! Whats that... a bucket? We need ten bucklers... not spitoons. */
        public override object Uncomplete => 1075515;
        /* Thanks for the help. Here's something for ya to remember me by. */
        public override object Complete => 1075516;
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

    public class Gervis : MondainQuester
    {
        [Constructable]
        public Gervis()
            : base("Gervis", "the blacksmith trainer")
        {
            SetSkill(SkillName.Blacksmith, 65.0, 88.0);
        }

        public Gervis(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(BatteredBucklersQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x83F5;
            HairItemID = 0x203B;
            HairHue = 0x5EC;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new SmithHammer());
            AddItem(new Boots(0x3B2));
            AddItem(new ShortPants(0x1BB));
            AddItem(new Shirt(0x71F));
            AddItem(new FullApron(0x3B2));

            Item item;

            item = new LeatherGloves
            {
                Hue = 0x3B2
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

    public class SmithsSatchel : Backpack
    {
        [Constructable]
        public SmithsSatchel()
            : base()
        {
            Hue = BaseReward.SatchelHue();

            DropItem(new IronIngot(10));
            DropItem(new SmithHammer());
        }

        public SmithsSatchel(Serial serial)
            : base(serial)
        {
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