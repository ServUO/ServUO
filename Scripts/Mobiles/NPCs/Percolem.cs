using System;

namespace Server.Engines.Quests
{
    public class Percolem : MondainQuester, ITierQuester
    {
        public TierQuestInfo TierInfo => TierQuestInfo.Percolem;

        [Constructable]
        public Percolem()
            : base("Percolem", "the Hunter")
        {
        }

        public Percolem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { };

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = 0x840C;
            HairItemID = 0x203C;
            HairHue = 0x3B3;
        }

        public override void InitOutfit()
        {
            CantWalk = true;

            AddItem(new Items.Boots());
            AddItem(new Items.Shirt(1436));
            AddItem(new Items.ShortPants(1436));
            AddItem(new Items.CompositeBow());

            Blessed = true;
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
