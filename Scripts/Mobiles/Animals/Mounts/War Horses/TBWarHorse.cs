using System;

namespace Server.Mobiles
{
    public class TBWarHorse : BaseWarHorse
    {
        [Constructable]
        public TBWarHorse()
            : base(0x76, 0x3EB2, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
        }

        public TBWarHorse(Serial serial)
            : base(serial)
        {
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