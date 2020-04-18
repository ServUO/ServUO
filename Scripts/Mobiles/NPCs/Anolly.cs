using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class StopHarpingOnMeQuest : BaseQuest
    {
        public StopHarpingOnMeQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(LapHarp), "lap harp", 20, 0xEB2));

            AddReward(new BaseReward(typeof(CarpentersCraftsmanSatchel), 1074282));
        }

        /* Stop Harping on Me */
        public override object Title => 1073881;
        /* Humans artistry can be a remarkable thing. For instance, I have heard of a wonderful 
        instrument which creates the most melodious of music. A lap harp. I would be ever so 
        grateful if I could examine one in person. */
        public override object Description => 1074071;
        /* I will patiently await your reconsideration. */
        public override object Refuse => 1073921;
        /* I will be in your debt if you bring me lap harp. */
        public override object Uncomplete => 1073927;
        /* My thanks for your service. Now, I will show you something of elven carpentry. */
        public override object Complete => 1073969;
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

    public class TheFarEyeQuest : BaseQuest
    {
        public TheFarEyeQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Spyglass), "spyglasses", 20, 0x14F5));

            AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* The Far Eye */
        public override object Title => 1073908;
        /* The wonders of human invention! Turning sand and metal into a far-seeing eye! This is 
        something I must experience for myself. Bring me some of these spyglasses friend human. */
        public override object Description => 1074098;
        /* I will patiently await your reconsideration. */
        public override object Refuse => 1073921;
        /* I will be in your debt if you bring me spyglasses. */
        public override object Uncomplete => 1073954;
        /* Enjoy my thanks for your service. */
        public override object Complete => 1073978;
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

    public class Anolly : MondainQuester
    {
        [Constructable]
        public Anolly()
            : base("Anolly", "the bark weaver")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Anolly(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(StopHarpingOnMeQuest),
                    typeof(TheFarEyeQuest),
                    typeof(NothingFancyQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x8835;
            HairItemID = 0x2FC0;
            HairHue = 0x325;
        }

        public override void InitOutfit()
        {
            AddItem(new Sandals(0x901));
            AddItem(new FullApron(0x1BB));
            AddItem(new ShortPants(0x3B2));
            AddItem(new SmithHammer());
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