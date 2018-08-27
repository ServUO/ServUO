using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Neville : BaseEscort
    {
        public override Type[] Quests { get { return new Type[] { typeof(EscortToDugan) }; } }

        [Constructable]
        public Neville()
            : base()
        {
            Name = "Neville Brightwhistle";
        }

        public Neville(Serial serial)
            : base(serial)
        {
        }

        public override void Advertise()
        {
            Say(1095004); // Please help me, where am I?
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new Shoes(0x70A));
            SetWearable(new LongPants(0x1BB));
            SetWearable(new FancyShirt(0x588));
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
