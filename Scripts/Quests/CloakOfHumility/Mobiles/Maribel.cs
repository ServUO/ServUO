using System;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Maribel : HumilityQuestMobile
    {
        public override int Greeting { get { return 1075754; } }

        public override bool IsActiveVendor { get { return true; } }

        [Constructable]
        public Maribel()
            : base("Maribel", "the Waitress")
        {
        }

        public override void InitSBInfo()
        {
            SBInfos.Add(new SBWaiter());
        }

        public Maribel(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = true;
            this.Race = Race.Human;
            this.Body = 0x191;

            this.Hue = 0x83EA;
            this.HairItemID = 0x2049;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Server.Items.Backpack());
            this.AddItem(new Server.Items.Sandals());
            this.AddItem(new Server.Items.FancyDress(2205));
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