using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Jaacar : MondainQuester
    {
        [Constructable]
        public Jaacar()
            : base("Jaacar")
        {
        }

        public Jaacar(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(BadCompany)
                };
            }
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            Body = 723;
            Female = false;

            Frozen = true;
            Direction = Direction.Down;
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
                Body = 723;
            }

            Frozen = true;
            Direction = Direction.Down;
        }
    }
}