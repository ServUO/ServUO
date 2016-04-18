using System;

namespace Server.Engines.Quests
{ 
    public class Strongroot : MondainQuester
    { 
        [Constructable]
        public Strongroot()
            : base("Strongroot")
        { 
        }

        public Strongroot(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(CaretakerOfTheLandQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Body = 301;
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