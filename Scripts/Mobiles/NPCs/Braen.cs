using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class UnholyConstructQuest : BaseQuest
    {
        public UnholyConstructQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Golem), "golems", 10));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706)); // A large bag of treasure.
        }

        /* Unholy Construct */
        public override object Title => 1073666;
        /* They're unholy, I say. Golems, a walking mockery of all life, born of 
        blackest magic. They're not truly alive, so destroying them isn't a crime, 
        it's a service. A service I will gladly pay for. */
        public override object Description => 1073705;
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse => 1073733;
        /* The unholy brutes, the Golems, must be smited! */
        public override object Uncomplete => 1073746;
        /* Reduced those Golems to component parts? Good, then -- you deserve this reward! */
        public override object Complete => 1073787;
    }

    public class Braen : MondainQuester
    {
        [Constructable]
        public Braen()
            : base("Braen", "the thaumaturgist")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Braen(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { typeof(UnholyConstructQuest) };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Elf;

            Hue = 0x876C;
            HairItemID = 0x2FBF;
            HairHue = 0x2C2;
        }

        public override void InitOutfit()
        {
            AddItem(new Sandals(0x722));
            AddItem(RandomWand.CreateWand());
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