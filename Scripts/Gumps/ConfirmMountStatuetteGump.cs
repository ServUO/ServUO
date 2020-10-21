using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Gumps
{
    public class ConfirmMountStatuetteGump : BaseConfirmGump
    {
        private readonly Item m_Item;

        public ConfirmMountStatuetteGump(Item item)
        {
            m_Item = item;
        }

        public override int LabelNumber => 1075084;  // This statuette will be destroyed when its trapped creature is summoned. The creature will be bonded to you but will disappear if released. <br><br>Do you wish to proceed?

        public override void Confirm(Mobile from)
        {
            if (m_Item == null || m_Item.Deleted)
                return;

            BaseCreature m = null;

            if (m_Item is ICreatureStatuette)
            {
                m = Activator.CreateInstance(((ICreatureStatuette)m_Item).CreatureType) as BaseCreature;
            }

            if (m != null)
            {
                if ((from.Followers + m.ControlSlots) > from.FollowersMax)
                {
                    m.Delete();
                    from.SendLocalizedMessage(1114321); // You have too many followers to control that pet.
                }
                else
                {
                    m.SetControlMaster(from);

                    m.IsBonded = true;
                    m.MoveToWorld(from.Location, from.Map);
                    m_Item.Delete();

                    PetTrainingHelper.GetAbilityProfile(m, true).OnTame();

                    Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                    {
                        m.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502799, from.NetState); // It seems to accept you as master.
                        from.SendLocalizedMessage(1049666); // Your pet has bonded with you!
                    });
                }
            }
        }
    }
}

namespace Server.Mobiles
{
    public interface ICreatureStatuette
    {
        Type CreatureType { get; }
    }
}
