using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class PlagueBeastInnard : Item, IScissorable, ICarvable
    {
        public PlagueBeastInnard(int itemID, int hue)
            : base(itemID)
        {
            Name = "plague beast innards";
            Hue = hue;
            Movable = false;
            Weight = 1.0;
        }

        public PlagueBeastInnard(Serial serial)
            : base(serial)
        {
        }

        public PlagueBeastLord Owner => RootParent as PlagueBeastLord;
        public virtual bool Scissor(Mobile from, Scissors scissors)
        {
            return false;
        }

        public virtual bool Carve(Mobile from, Item with)
        {
            return false;
        }

        public virtual bool OnBandage(Mobile from)
        {
            return false;
        }

        public override bool IsAccessibleTo(Mobile check)
        {
            if ((int)check.AccessLevel >= (int)AccessLevel.GameMaster)
                return true;

            PlagueBeastLord owner = Owner;

            if (owner == null)
                return false;

            if (!owner.InRange(check, 2))
                owner.PrivateOverheadMessage(MessageType.Label, 0x3B2, 500446, check.NetState); // That is too far away.
            else if (owner.OpenedBy != null && owner.OpenedBy != check) // TODO check
                owner.PrivateOverheadMessage(MessageType.Label, 0x3B2, 500365, check.NetState); // That is being used by someone else
            else if (owner.Frozen)
                return true;

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            PlagueBeastLord owner = Owner;

            if (owner == null || !owner.Alive)
                Delete();
        }
    }

    public class PlagueBeastComponent : PlagueBeastInnard
    {
        private PlagueBeastOrgan m_Organ;
        public PlagueBeastComponent(int itemID, int hue)
            : this(itemID, hue, false)
        {
        }

        public PlagueBeastComponent(int itemID, int hue, bool movable)
            : base(itemID, hue)
        {
            Movable = movable;
        }

        public PlagueBeastComponent(Serial serial)
            : base(serial)
        {
        }

        public PlagueBeastOrgan Organ
        {
            get
            {
                return m_Organ;
            }
            set
            {
                m_Organ = value;
            }
        }
        public bool IsBrain => ItemID == 0x1CF0;
        public bool IsGland => ItemID == 0x1CEF;
        public bool IsReceptacle => ItemID == 0x9DF;
        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (target is PlagueBeastBackpack)
                return base.DropToItem(from, target, p);

            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            return false;
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            return false;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (m_Organ != null && m_Organ.OnDropped(from, dropped, this))
            {
                if (dropped is PlagueBeastComponent)
                    m_Organ.Components.Add((PlagueBeastComponent)dropped);
            }

            return true;
        }

        public override bool OnDragLift(Mobile from)
        {
            if (IsAccessibleTo(from))
            {
                if (m_Organ != null && m_Organ.OnLifted(from, this))
                {
                    from.SendLocalizedMessage(IsGland ? 1071895 : 1071914, null, 0x3B2); // * You rip the organ out of the plague beast's flesh *

                    if (m_Organ.Components.Contains(this))
                        m_Organ.Components.Remove(this);

                    m_Organ = null;
                    from.PlaySound(0x1CA);
                }

                return true;
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteItem(m_Organ);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_Organ = reader.ReadItem<PlagueBeastOrgan>();
        }
    }
}