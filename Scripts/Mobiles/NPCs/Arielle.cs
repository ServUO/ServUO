using System;

namespace Server.Engines.Quests
{ 
    public class Arielle : MondainQuester
    { 
        [Constructable]
        public Arielle()
            : base("Arielle")
        { 
            this.BaseSoundID = 0x46F;
			
            this.SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Arielle(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(TheJoysOfLifeQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;			
            this.Body = 128;
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