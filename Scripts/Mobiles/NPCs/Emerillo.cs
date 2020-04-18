using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class CulinaryCrisisQuest : BaseQuest
    {
        public CulinaryCrisisQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(Dates), "bunch of dates", 20, 0x1727));
            AddObjective(new ObtainObjective(typeof(CheeseWheel), "wheels of cheese", 5, 0x97E));

            AddReward(new BaseReward(typeof(TreasureBag), 1072583));
        }

        /* Culinary Crisis */
        public override object Title => 1074755;
        /* You have NO idea how impossible this is.  Simply intolerable!  How can one expect an artiste' like me to 
        create masterpieces of culinary delight without the best, fresh ingredients?  Ever since this whositwhatsit 
        started this uproar, my thrice-daily produce deliveries have ended.  I can't survive another hour without 
        produce! */
        public override object Description => 1074756;
        /* You have no artistry in your soul. */
        public override object Refuse => 1074757;
        /* I must have fresh produce and cheese at once! */
        public override object Uncomplete => 1074758;
        /* Those dates look bruised!  Oh no, and you fetched a soft cheese.  *deep pained sigh*  Well, even I can only 
        do so much with inferior ingredients.  BAM! */
        public override object Complete => 1074759;
        public override bool CanOffer()
        {
            return MondainsLegacy.Bedlam;
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

    public class Emerillo : MondainQuester
    {
        [Constructable]
        public Emerillo()
            : base("Emerillo", "the cook")
        {
        }

        public Emerillo(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(CulinaryCrisisQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x83F4;
            HairItemID = 0x203C;
            HairHue = 0x454;
            FacialHairItemID = 0x204C;
            FacialHairHue = 0x454;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Sandals(0x75D));
            AddItem(new LongPants(0x529));
            AddItem(new Shirt(0x38B));
            AddItem(new HalfApron(0x8FD));
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