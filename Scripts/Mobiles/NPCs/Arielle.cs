using System;

namespace Server.Engines.Quests
{
    public class Arielle : MondainQuester
    {
        [Constructable]
        public Arielle()
            : base("Arielle")
        {
            BaseSoundID = 0x46F;

            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Arielle(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(TheJoysOfLifeQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Body = 128;
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