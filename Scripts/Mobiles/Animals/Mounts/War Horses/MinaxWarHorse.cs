using System;

namespace Server.Mobiles
{
    public class MinaxWarHorse : BaseWarHorse
    {
        [Constructable]
        public MinaxWarHorse()
            : base(0x78, 0x3EAF, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
        }

        public MinaxWarHorse(Serial serial)
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