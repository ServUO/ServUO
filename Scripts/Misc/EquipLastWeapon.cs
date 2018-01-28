using System;
using Server;
using Server.Mobiles;
using Server.Items;

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
                Item toEquip = from.LastWeapon;
                Item toDisarm = from.FindItemOnLayer(Layer.OneHanded);

                if (toDisarm == null || !toDisarm.Movable)
                    toDisarm = from.FindItemOnLayer(Layer.TwoHanded);

                if (toDisarm != null)
                {
                    from.Backpack.DropItem(toDisarm);
                }

                if (toEquip != null && toDisarm.Movable && toDisarm is BaseWeapon && toEquip.IsChildOf(from.Backpack))
                {
                    from.EquipItem(toEquip);
                }
            }
        }
    }
}