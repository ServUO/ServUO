using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Multis;

namespace Server.Network
{
    public sealed class BoatMovementRequest
    {
        public static void Initialize()
        {
            PacketHandlers.RegisterExtended(0x33, true, new OnPacketReceive(MultiMouseMovementRequest));
        }

        public static void MultiMouseMovementRequest(NetState state, PacketReader reader)
        {
            Serial playerSerial = reader.ReadInt32();
            Direction movement = (Direction)reader.ReadByte();
            reader.ReadByte(); // movement direction duplicated
            int speed = reader.ReadByte();

            Mobile mob = World.FindMobile(playerSerial);
            if (mob == null || mob.NetState == null || !mob.Mounted)
                return;

            IMount multi = mob.Mount;
            if (!(multi is BaseBoat))
                return;

            BaseBoat boat = (BaseBoat)multi;
            boat.OnMousePilotCommand(mob, movement, speed);
        }
    }
}