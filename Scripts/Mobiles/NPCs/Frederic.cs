using System;

namespace Server.Engines.Quests
{ 
    public class Frederic : MondainQuester
    {
        [Constructable]
        public Frederic()
            : base("The Ghost of Frederic Smithson")
        { 
        }

        public Frederic(Serial serial)
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
			
            this.Body = 0x1A;
            this.Hue = 0x455;
            this.CantWalk = true;
            this.Frozen = true;
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