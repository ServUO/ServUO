using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class LethalDartsQuest : BaseQuest
    {
        public LethalDartsQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Bolt), "crossbow bolts", 10, 0x1BFB));

            AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        public override TimeSpan RestartDelay => TimeSpan.FromMinutes(2);

        /* Lethal Darts */
        public override object Title => 1073876;
        /* We elves are no strangers to archery but I would be interested in learning whether there 
        is anything to learn from the human approach. I would gladly trade you something I have if 
        you could teach me of the deadly crossbow bolt. */
        public override object Description => 1074066;
        /* I will patiently await your reconsideration. */
        public override object Refuse => 1073921;
        /* I will be in your debt if you bring me crossbow bolts. */
        public override object Uncomplete => 1073922;
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete => 1073968;
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

    public class SimpleBowQuest : BaseQuest
    {
        public SimpleBowQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Bow), "bows", 10, 0x13B2));

            AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        /* A Simple Bow */
        public override object Title => 1073877;
        /* I wish to try a bow crafted in the human style. Is it possible for you to bring me 
        such a weapon? I would be happy to return this favor. */
        public override object Description => 1074067;
        /* I will patiently await your reconsideration. */
        public override object Refuse => 1073921;
        /* I will be in your debt if you bring me bows. */
        public override object Uncomplete => 1073923;
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete => 1073968;
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

    public class IngeniousArcheryPartOneQuest : BaseQuest
    {
        public IngeniousArcheryPartOneQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Crossbow), "crossbows", 10, 0xF50));

            AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        /* Ingenious Archery, Part I */
        public override object Title => 1073878;
        /* I have heard of a curious type of bow, you call it a "crossbow". It sounds fascinating and I would 
        very much like to examine one closely. Would you be able to obtain such an instrument for me? */
        public override object Description => 1074068;
        /* I will patiently await your reconsideration. */
        public override object Refuse => 1073921;
        /* I will be in your debt if you bring me crossbows. */
        public override object Uncomplete => 1073924;
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete => 1073968;
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

    public class IngeniousArcheryPartTwoQuest : BaseQuest
    {
        public IngeniousArcheryPartTwoQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(HeavyCrossbow), "heavy crossbows", 8, 0x13FD));

            AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        /* Ingenious Archery, Part II */
        public override object Title => 1073879;
        /* These human "crossbows" are complex and clever. The "heavy crossbow" is a remarkable 
        instrument of war. I am interested in seeing one up close, if you could arrange for one 
        to make its way to my hands. */
        public override object Description => 1074069;
        /* I will patiently await your reconsideration. */
        public override object Refuse => 1073921;
        /* I will be in your debt if you bring me heavy crossbows. */
        public override object Uncomplete => 1073925;
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete => 1073968;
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

    public class IngeniousArcheryPartThreeQuest : BaseQuest
    {
        public IngeniousArcheryPartThreeQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(RepeatingCrossbow), "repeating crossbows", 10, 0x26C3));

            AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        /* Ingenious Archery, Part III */
        public override object Title => 1073880;
        /* My friend, I am in search of a device, a instrument of remarkable human ingenuity. It is a 
        repeating crossbow. If you were to obtain such a device, I would gladly reveal to you some of 
        the secrets of elven craftsmanship. */
        public override object Description => 1074070;
        /* I will patiently await your reconsideration. */
        public override object Refuse => 1073921;
        /* I will be in your debt if you bring me repeating crossbows. */
        public override object Uncomplete => 1073926;
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete => 1073968;
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

    public class Cillitha : MondainQuester
    {
        [Constructable]
        public Cillitha()
            : base("Cillitha", "the bowcrafter")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Cillitha(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(LethalDartsQuest),
                    typeof(SimpleBowQuest),
                    typeof(IngeniousArcheryPartOneQuest),
                    typeof(IngeniousArcheryPartTwoQuest),
                    typeof(IngeniousArcheryPartThreeQuest),
                    typeof(StopHarpingOnMeQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x83E6;
            HairItemID = 0x2FC2;
            HairHue = 0x8E;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots(0x901));
            AddItem(new ElvenShirt(0x714));
            AddItem(new LeafLegs());
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