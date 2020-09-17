using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class ChillInTheAirQuest : BaseQuest
    {
        public ChillInTheAirQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(IceElemental), "ice elementals", 15));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* A Chill in the Air */
        public override object Title => 1073663;
        /* Feel that chill in the air? It means an icy death for the unwary, for deadly Ice Elementals are about. 
        Who knows what magic summoned them, what's important now is getting rid of them. I don't have much, but 
        I'll give all I can if you'd only stop the cold-hearted monsters. */
        public override object Description => 1073702;
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse => 1073733;
        /* The chill won't lift until you eradicate a few Ice Elemenals. */
        public override object Uncomplete => 1073743;
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

    public class IndustriousAsAnAntLionQuest : BaseQuest
    {
        public IndustriousAsAnAntLionQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(AntLion), "ant lions", 12));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* Industrious as an Ant Lion */
        public override object Title => 1073665;
        /* Ants are industrious and Lions are noble so who'd think an Ant Lion would be such a problem? The Ant Lion's 
        have been causing mindless destruction in these parts. I suppose it's just how ants are. But I need you to help 
        eliminate the infestation. Would you be willing to help for a bit of reward? */
        public override object Description => 1073704;
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse => 1073733;
        /* Please, rid us of the Ant Lion infestation. */
        public override object Uncomplete => 1073745;
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

    public class ThePerilsOfFarmingQuest : BaseQuest
    {
        public ThePerilsOfFarmingQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(WhippingVine), "whipping vines", 15));

            AddReward(new BaseReward(typeof(LargeTreasureBag), 1072706));
        }

        /* The Perils of Farming */
        public override object Title => 1073664;
        /* I should be trimming back the vegetation here, but something nasty has taken root. Viscious vines I can't go near. 
        If there's any hope of getting things under control, some one's going to need to destroy a few of those Whipping Vines. 
        Someone strong and fast and tough. */
        public override object Description => 1073703;
        /* Perhaps you'll change your mind and return at some point. */
        public override object Refuse => 1073733;
        /* How are farmers supposed to work with these Whipping Vines around? */
        public override object Uncomplete => 1073744;
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

    public class CommonBrigandsQuest : BaseQuest
    {
        public CommonBrigandsQuest()
            : base()
        {
            AddObjective(new SlayObjective("common brigands", 20, typeof(Brigand)));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Common Brigands */
        public override object Title => 1073082;
        /* Thank goodness, a hero like you has arrived! Brigands have descended upon this area like locusts, stealing and 
        looting where ever they go. We need someone to put these vile curs where they belong -- in their graves. Are you 
        up to the task?  */
        public override object Description => 1073572;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* The Brigands still plague us. Have you killed 20 of their number? */
        public override object Uncomplete => 1073592;
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

    public class ArchEnemiesQuest : BaseQuest
    {
        public ArchEnemiesQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(RatmanArcher), "ratman archers", 10));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Arch Enemies */
        public override object Title => 1073085;
        /* Vermin! They get into everything! I told the boy to leave out some poisoned cheese -- and they shot him. 
        What else can I do? Unless…these ratmen are skilled with a bow, but I'd lay a wager you're better, eh? Could 
        you skin a few of the wretches for me? */
        public override object Description => 1073575;
        /* I hope you'll reconsider. Until then, farwell. */
        public override object Refuse => 1073580;
        /* I don't see 10 tails from Ratman Archers on your belt -- and until I do, no reward for you. */
        public override object Uncomplete => 1073595;
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

    public class VerminQuest : BaseQuest
    {
        public VerminQuest()
            : base()
        {
            AddObjective(new SlayObjective(typeof(Ratman), "ratmen", 12));

            AddReward(new BaseReward(typeof(TrinketBag), 1072341));
        }

        /* Vermin */
        public override object Title => 1072995;
        /* You've got to help me out! Those ratmen have been causing absolute havok around here.  Kill them off before 
        they destroy my land.  I'll pay you if you kill off twelve of those dirty rats. */
        public override object Description => 1073029;
        /* Well, okay. But if you decide you are up for it after all, c'mon back and see me. */
        public override object Refuse => 1072270;
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete => 1072271;
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

    public class Tiana : MondainQuester
    {
        [Constructable]
        public Tiana()
            : base("Tiana", "the guard")
        {
        }

        public Tiana(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(MaraudersQuest),
                    typeof(ChillInTheAirQuest),
                    typeof(IndustriousAsAnAntLionQuest),
                    typeof(ThePerilsOfFarmingQuest),
                    typeof(UnholyConstructQuest),
                    typeof(CommonBrigandsQuest),
                    typeof(ArchEnemiesQuest),
                    typeof(TrollingForTrollsQuest),
                    typeof(DeadManWalkingQuest),
                    typeof(ForkedTonguesQuest),
                    typeof(VerminQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            CantWalk = true;
            Race = Race.Elf;

            Hue = 0x824E;
            HairItemID = 0x2FCC;
            HairHue = 0x385;
        }

        public override void InitOutfit()
        {
            AddItem(new ElvenBoots());
            AddItem(new HidePants());
            AddItem(new HideFemaleChest());
            AddItem(new HidePauldrons());
            AddItem(new WoodlandBelt(0x657));

            Item item;

            item = new RavenHelm
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
