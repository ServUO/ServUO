using Server.Mobiles;

namespace Server.Items
{
    public class UnderworldTele : Teleporter
    {
        [Constructable]
        public UnderworldTele()
        {
        }

        public UnderworldTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is PlayerMobile player)
            {
                if (player.AbyssEntry)
                {
                    return base.OnMoveOver(player);
                }

                player.SendLocalizedMessage(1112226); // Thou must be on a Sacred Quest to pass through.	
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
