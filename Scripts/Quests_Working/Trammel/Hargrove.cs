using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class ChopChopOnTheDoubleQuest : BaseQuest
    { 
        public ChopChopOnTheDoubleQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Log), "log", 60, 0x1BDD));
						
            this.AddReward(new BaseReward(typeof(LumberjacksSatchel), 1074282)); // Craftsman's Satchel
        }

        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(3);
            }
        }
        /* Chop Chop, On The Double! */
        public override object Title
        {
            get
            {
                return 1075537;
            }
        }
        /* That's right, move it! I need sixty logs on the double, and they need to be freshly cut! If you can get them to 
        me fast I'll have your payment in your hands before you have the scent of pine out from beneath your nostrils. Just 
        get a sharp axe and hack away at some of the trees in the land and your lumberjacking skill will rise in no time. */
        public override object Description
        {
            get
            {
                return 1075538;
            }
        }
        /* Or perhaps you'd rather not. */
        public override object Refuse
        {
            get
            {
                return 1072981;
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
        /* Ahhh! The smell of fresh cut lumber. And look at you, all strong and proud, as if you had done an honest days work! */
        public override object Complete
        {
            get
            {
                return 1075539;
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

    public class Hargrove : MondainQuester
    {
        [Constructable]
        public Hargrove()
            : base("Hargrove", "the lumberjack")
        { 
        }

        public Hargrove(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(ChopChopOnTheDoubleQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83FF;
            this.HairItemID = 0x203C;
            this.HairHue = 0x0;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());			
            this.AddItem(new BattleAxe());
            this.AddItem(new Boots(0x901));
            this.AddItem(new StuddedLegs());
            this.AddItem(new Shirt(0x288));
            this.AddItem(new Bandana(0x20));
			
            Item item;
			
            item = new PlateGloves();
            item.Hue = 0x21E;
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

    public class LumberjacksSatchel : Backpack
    {
        [Constructable]
        public LumberjacksSatchel()
            : base()
        {
            this.Hue = BaseReward.SatchelHue();
			
            this.AddItem(new Gold(15));
            this.AddItem(new Hatchet());
        }

        public LumberjacksSatchel(Serial serial)
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