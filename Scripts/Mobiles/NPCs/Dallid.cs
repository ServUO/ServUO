using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class TappingTheKegQuest : BaseQuest
    {
        public TappingTheKegQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(BarrelTap), "barrel taps", 10, 0x1004));

            AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* Tapping the Keg */
        public override object Title => 1074037;
        /* I have acquired a barrel of human brewed beer. I am loathe to drink it, but how else to prove how 
        inferior it is? I suppose I shall need a barrel tap to drink. Go, bring me a barrel tap quickly, so 
        I might get this over with. */
        public override object Description => 1074131;
        /* Fine then, I'm shall find another to run my errands then. */
        public override object Refuse => 1074063;
        /* Hurry up! I don't have all day to wait for you to bring what I desire! */
        public override object Uncomplete => 1074064;
        /* These human made goods are laughable! It offends so -- I must show you what elven skill is capable of! */
        public override object Complete => 1074065;
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

    public class BreezesSongQuest : BaseQuest
    {
        public BreezesSongQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(FancyWindChimes), "fancy wind chimes", 10, 0x2833));

            AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* Breeze's Song */
        public override object Title => 1074052;
        /* I understand humans cruely enslave the very wind to their selfish whims! Fancy wind chimes, what a monstrous 
        idea! You must bring me proof of this terrible depredation - hurry, bring me wind chimes! */
        public override object Description => 1074146;
        /* Fine then, I'm shall find another to run my errands then. */
        public override object Refuse => 1074063;
        /* Hurry up! I don't have all day to wait for you to bring what I desire! */
        public override object Uncomplete => 1074064;
        /* These human made goods are laughable! It offends so -- I must show you what elven skill is capable of! */
        public override object Complete => 1074065;
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

    public class WaitingToBeFilledQuest : BaseQuest
    {
        public WaitingToBeFilledQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Bottle), "empty bottles", 20, 0xF0E));

            AddReward(new BaseReward(typeof(TinkersCraftsmanSatchel), 1074282));
        }

        /* Waiting to be Filled */
        public override object Title => 1074036;
        /* The only good thing I can say about human made bottles is that they are empty and may yet still be filled 
        with elven wine. Go now, fetch a number of empty bottles so that I might save them from the fate of carrying 
        human-made wine. */
        public override object Description => 1074130;
        /* Fine then, I'm shall find another to run my errands then. */
        public override object Refuse => 1074063;
        /* Hurry up! I don't have all day to wait for you to bring what I desire! */
        public override object Uncomplete => 1074064;
        /* These human made goods are laughable! It offends so -- I must show you what elven skill is capable of! */
        public override object Complete => 1074065;
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

    public class Dallid : MondainQuester
    {
        [Constructable]
        public Dallid()
            : base("Dallid", "the cook")
        {
        }

        public Dallid(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(TappingTheKegQuest),
                    typeof(BreezesSongQuest),
                    typeof(WaitingToBeFilledQuest),
                    typeof(MougGuurMustDieQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Elf;

            Hue = 0x8376;
            HairItemID = 0x2FCD;
            HairHue = 0x100;
        }

        public override void InitOutfit()
        {
            AddItem(new Boots(0x901));
            AddItem(new ShortPants(0x733));
            AddItem(new Shirt(0x70E));
            AddItem(new FullApron(0x1BE));
            AddItem(new Cleaver());
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