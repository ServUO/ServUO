using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class ChopChopOnTheDoubleQuest : BaseQuest
    {
        public ChopChopOnTheDoubleQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Log), "log", 60, 0x1BDD));

            AddReward(new BaseReward(typeof(LumberjacksSatchel), 1074282)); // Craftsman's Satchel
        }

        public override TimeSpan RestartDelay => TimeSpan.FromMinutes(3);
        /* Chop Chop, On The Double! */
        public override object Title => 1075537;
        /* That's right, move it! I need sixty logs on the double, and they need to be freshly cut! If you can get them to 
        me fast I'll have your payment in your hands before you have the scent of pine out from beneath your nostrils. Just 
        get a sharp axe and hack away at some of the trees in the land and your lumberjacking skill will rise in no time. */
        public override object Description => 1075538;
        /* Or perhaps you'd rather not. */
        public override object Refuse => 1072981;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
        /* Ahhh! The smell of fresh cut lumber. And look at you, all strong and proud, as if you had done an honest days work! */
        public override object Complete => 1075539;
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

        public override Type[] Quests => new Type[]
                {
                    typeof(ChopChopOnTheDoubleQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x83FF;
            HairItemID = 0x203C;
            HairHue = 0x0;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new BattleAxe());
            AddItem(new Boots(0x901));
            AddItem(new StuddedLegs());
            AddItem(new Shirt(0x288));
            AddItem(new Bandana(0x20));

            Item item;

            item = new PlateGloves
            {
                Hue = 0x21E
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

    public class LumberjacksSatchel : Backpack
    {
        [Constructable]
        public LumberjacksSatchel()
            : base()
        {
            Hue = BaseReward.SatchelHue();

            AddItem(new Gold(15));
            AddItem(new Hatchet());
        }

        public LumberjacksSatchel(Serial serial)
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