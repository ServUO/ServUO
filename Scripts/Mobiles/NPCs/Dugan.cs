using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Dugan : MondainQuester
    {
        [Constructable]
        public Dugan()
            : base("Elder Dugan", "the Prospector")
        {
        }

        public Dugan(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(Missing)
                };
            }
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;
            Body = 0x190;

            Hue = 0x83EA;
            HairItemID = 0x203C;
            CantWalk = true;

            Direction = Direction.Left;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Shoes(1819));
            AddItem(new LeatherArms());
            AddItem(new LeatherChest());
            AddItem(new LeatherLegs());
            AddItem(new LeatherGloves());
            AddItem(new GnarledStaff());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                Female = true;
                CantWalk = true;

                Direction = Direction.Left;

                var item = FindItemOnLayer(Layer.Shoes);
                if (item != null)
                    item.Hue = 1819;
            }
        }
    }
}