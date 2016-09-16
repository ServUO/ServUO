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
            this.InitStats(100, 100, 25);
            this.Body = 334;
            this.Female = false;
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