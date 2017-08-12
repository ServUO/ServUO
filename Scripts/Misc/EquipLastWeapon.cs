using System;
using Server;
using Server.Mobiles;

namespace Server.Network
{
    public class EquipLastWeaponPacket
    {
        public static void Initialize()
        {
            PacketHandlers.RegisterEncoded(0x1E, true, new OnEncodedPacketReceive(EquipLastWeaponRequest));
        }

        public static void EquipLastWeaponRequest(NetState state, IEntity e, EncodedReader reader)
        {
            PlayerMobile from = state.Mobile as PlayerMobile;

            if (from != null)
            {
                if (from.LastWeapon != null && from.LastWeapon.IsChildOf(from.Backpack))
                {
                    Item toEquip = from.LastWeapon;
                    Item toDisarm = from.FindItemOnLayer(Layer.OneHanded);

                    if (toDisarm == null || !toDisarm.Movable)
                        toDisarm = from.FindItemOnLayer(Layer.TwoHanded);

                    if (toDisarm != null && toDisarm.Movable)
                        from.Backpack.DropItem(toDisarm);

                    from.EquipItem(toEquip);
                }
            }
        }
    }
}