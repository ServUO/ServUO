using System;

namespace Server.Engines.Quests
{ 
    public class Szandor : MondainQuester
    {
        [Constructable]
        public Szandor()
            : base("Szandor", "the late architect")
        { 
        }

        public Szandor(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return null;
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Body = 0x32;
            this.Hue = 0x83F2;
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