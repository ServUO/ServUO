using Server.Network;
using System;

namespace Server.Items
{
    public abstract class FarmableCrop : Item
    {
        private bool m_Picked;
        public FarmableCrop(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        public FarmableCrop(Serial serial)
            : base(serial)
        {
        }

        public abstract Item GetCropObject();

        public abstract int GetPickedID();

        public override void OnDoubleClick(Mobile from)
        {
            Map map = Map;
            Point3D loc = Location;

            if (Parent != null || Movable || IsLockedDown || IsSecure || map == null || map == Map.Internal)
                return;

            if (!from.InRange(loc, 2) || !from.InLOS(this))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (!m_Picked)
                OnPicked(from, loc, map);
        }

        public virtual void OnPicked(Mobile from, Point3D loc, Map map)
        {
            ItemID = GetPickedID();

            Item spawn = GetCropObject();

            if (spawn != null)
                spawn.MoveToWorld(loc, map);

            m_Picked = true;

            Unlink();

            Timer.DelayCall(TimeSpan.FromMinutes(5.0), Delete);
        }

        public void Unlink()
        {
            ISpawner se = Spawner;

            if (se != null)
            {
                Spawner.Remove(this);
                Spawner = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_Picked);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 0:
                    m_Picked = reader.ReadBool();
                    break;
            }
            if (m_Picked)
            {
                Unlink();
                Delete();
            }
        }
    }
}
