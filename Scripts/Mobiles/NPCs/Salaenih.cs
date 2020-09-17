using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class ShakingThingsUpQuest : BaseQuest
    {
        public ShakingThingsUpQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(RedSolenWarrior), "red solen warriors", 10));
            AddObjective(new SlayObjective(typeof(BlackSolenWarrior), "black solen warriors", 10));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        public override bool AllObjectives => false;
        /* Shaking Things Up */
        public override object Title => 1073083;
        /* A Solen hive is a fascinating piece of ecology. It's put together like a finely crafted clock. Who knows 
        what happens if you remove something? So let's find out. Exterminate a few of the warriors and I'll make it 
        worth your while. */
        public override object Description => 1073573;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* I don't think you've gotten their attention yet -- you need to kill at least 10 Solen Warriors. */
        public override object Uncomplete => 1073593;
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

    public class ArachnophobiaQuest : BaseQuest
    {
        public ArachnophobiaQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(GiantBlackWidow), "giant black widows", 12));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Arachnophobia */
        public override object Title => 1073079;
        /* I've seen them hiding in their webs among the woods. Glassy eyes, spindly legs, poisonous fangs. Monsters, 
        I say! Deadly horrors, these black widows. Someone must exterminate the abominations! If only I could find a 
        worthy hero for such a task, then I could give them this considerable reward. */
        public override object Description => 1073569;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* You've got a good start, but to stop the black-eyed fiends, you need to kill a dozen. */
        public override object Uncomplete => 1073589;
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

    public class MiniSwampThingQuest : BaseQuest
    {
        public MiniSwampThingQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Bogling), "boglings", 20));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Mini Swamp Thing */
        public override object Title => 1073072;
        /* Some say killing a boggling brings good luck. I don't place much stock in old wives' tales, but I can say a few 
        dead bogglings would certainly be lucky for me! Help me out and I can reward you for your efforts.  */
        public override object Description => 1073562;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* Go back and kill all 20 bogglings! */
        public override object Uncomplete => 1073582;
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

    public class Salaenih : MondainQuester
    {
        [Constructable]
        public Salaenih()
            : base("Salaenih", "the expeditionist")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Salaenih(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(WarriorCasteQuest),
                    typeof(ShakingThingsUpQuest),
                    typeof(ArachnophobiaQuest),
                    typeof(SquishyQuest),
                    typeof(BigJobQuest),
                    typeof(VoraciousPlantsQuest),
                    typeof(SpecimensQuest),
                    typeof(ColdHeartedQuest),
                    typeof(MiniSwampThingQuest),
                    typeof(AnimatedMonstrosityQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Elf;

            Hue = 0x851D;
            HairItemID = 0x2FCD;
            HairHue = 0x324;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new WarCleaver());

            Item item;

            item = new WoodlandLegs
            {
                Hue = 0x1BB
            };
            AddItem(item);

            item = new WoodlandArms
            {
                Hue = 0x1BB
            };
            AddItem(item);

            item = new WoodlandChest
            {
                Hue = 0x1BB
            };
            AddItem(item);

            item = new WoodlandBelt
            {
                Hue = 0x597
            };
            AddItem(item);

            item = new VultureHelm
            {
                Hue = 0x1BB
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
}