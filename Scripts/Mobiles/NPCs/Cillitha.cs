using System;
using Server.Items;

namespace Server.Engines.Quests
{ 
    public class LethalDartsQuest : BaseQuest
    { 
        public LethalDartsQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Bolt), "crossbow bolts", 10, 0x1BFB));
			
            this.AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(2);
            }
        }

        /* Lethal Darts */
        public override object Title
        {
            get
            {
                return 1073876;
            }
        }
        /* We elves are no strangers to archery but I would be interested in learning whether there 
        is anything to learn from the human approach. I would gladly trade you something I have if 
        you could teach me of the deadly crossbow bolt. */
        public override object Description
        {
            get
            {
                return 1074066;
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
        /* I will be in your debt if you bring me crossbow bolts. */
        public override object Uncomplete
        {
            get
            {
                return 1073922;
            }
        }
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete
        {
            get
            {
                return 1073968;
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

    public class SimpleBowQuest : BaseQuest
    { 
        public SimpleBowQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Bow), "bows", 10, 0x13B2));
			
            this.AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        /* A Simple Bow */
        public override object Title
        {
            get
            {
                return 1073877;
            }
        }
        /* I wish to try a bow crafted in the human style. Is it possible for you to bring me 
        such a weapon? I would be happy to return this favor. */
        public override object Description
        {
            get
            {
                return 1074067;
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
        /* I will be in your debt if you bring me bows. */
        public override object Uncomplete
        {
            get
            {
                return 1073923;
            }
        }
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete
        {
            get
            {
                return 1073968;
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

    public class IngeniousArcheryPartOneQuest : BaseQuest
    { 
        public IngeniousArcheryPartOneQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Crossbow), "crossbows", 10, 0xF50));
			
            this.AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        /* Ingenious Archery, Part I */
        public override object Title
        {
            get
            {
                return 1073878;
            }
        }
        /* I have heard of a curious type of bow, you call it a "crossbow". It sounds fascinating and I would 
        very much like to examine one closely. Would you be able to obtain such an instrument for me? */
        public override object Description
        {
            get
            {
                return 1074068;
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
        /* I will be in your debt if you bring me crossbows. */
        public override object Uncomplete
        {
            get
            {
                return 1073924;
            }
        }
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete
        {
            get
            {
                return 1073968;
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

    public class IngeniousArcheryPartTwoQuest : BaseQuest
    { 
        public IngeniousArcheryPartTwoQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(HeavyCrossbow), "heavy crossbows", 8, 0x13FD));
			
            this.AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        /* Ingenious Archery, Part II */
        public override object Title
        {
            get
            {
                return 1073879;
            }
        }
        /* These human "crossbows" are complex and clever. The "heavy crossbow" is a remarkable 
        instrument of war. I am interested in seeing one up close, if you could arrange for one 
        to make its way to my hands. */
        public override object Description
        {
            get
            {
                return 1074069;
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
        /* I will be in your debt if you bring me heavy crossbows. */
        public override object Uncomplete
        {
            get
            {
                return 1073925;
            }
        }
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete
        {
            get
            {
                return 1073968;
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

    public class IngeniousArcheryPartThreeQuest : BaseQuest
    { 
        public IngeniousArcheryPartThreeQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(RepeatingCrossbow), "repeating crossbows", 10, 0x26C3));
			
            this.AddReward(new BaseReward(typeof(FletcherCraftsmanSatchel), 1074282));
        }

        /* Ingenious Archery, Part III */
        public override object Title
        {
            get
            {
                return 1073880;
            }
        }
        /* My friend, I am in search of a device, a instrument of remarkable human ingenuity. It is a 
        repeating crossbow. If you were to obtain such a device, I would gladly reveal to you some of 
        the secrets of elven craftsmanship. */
        public override object Description
        {
            get
            {
                return 1074070;
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
        /* I will be in your debt if you bring me repeating crossbows. */
        public override object Uncomplete
        {
            get
            {
                return 1073926;
            }
        }
        /* My thanks for your service. Now, I shall teach you of elven archery. */
        public override object Complete
        {
            get
            {
                return 1073968;
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

    public class Cillitha : MondainQuester
    { 
        [Constructable]
        public Cillitha()
            : base("Cillitha", "the bowcrafter")
        { 
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Cillitha(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(LethalDartsQuest),
                    typeof(SimpleBowQuest),
                    typeof(IngeniousArcheryPartOneQuest),
                    typeof(IngeniousArcheryPartTwoQuest),
                    typeof(IngeniousArcheryPartThreeQuest),
                    typeof(StopHarpingOnMeQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.Race = Race.Elf;
			
            this.Hue = 0x83E6;
            this.HairItemID = 0x2FC2;
            this.HairHue = 0x8E;
        }

        public override void InitOutfit()
        {
            this.AddItem(new ElvenBoots(0x901));
            this.AddItem(new ElvenShirt(0x714));
            this.AddItem(new LeafLegs());
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