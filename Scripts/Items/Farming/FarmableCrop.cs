using System;
using Server.Network;

namespace Server.Items
{
    public abstract class FarmableCrop : Item
    {
        private bool m_Picked;
        public FarmableCrop(int itemID)
            : base(itemID)
        {
            this.Movable = false;
        }

        public FarmableCrop(Serial serial)
            : base(serial)
        {
        }

        public abstract Item GetCropObject();

        public abstract int GetPickedID();

        public override void OnDoubleClick(Mobile from)
        {
            Map map = this.Map;
            Point3D loc = this.Location;

            if (this.Parent != null || this.Movable || this.IsLockedDown || this.IsSecure || map == null || map == Map.Internal)
                return;

            if (!from.InRange(loc, 2) || !from.InLOS(this))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (!this.m_Picked)
                this.OnPicked(from, loc, map);
        }

        public virtual void OnPicked(Mobile from, Point3D loc, Map map)
        {
            this.ItemID = this.GetPickedID();

            Item spawn = this.GetCropObject();

            if (spawn != null)
                spawn.MoveToWorld(loc, map);

            this.m_Picked = true;

            this.Unlink();

            Timer.DelayCall(TimeSpan.FromMinutes(5.0), new TimerCallback(Delete));
        }

        public void Unlink()
        {
            ISpawner se = this.Spawner;

            if (se != null)
            {
                this.Spawner.Remove(this);
                this.Spawner = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_Picked);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    this.m_Picked = reader.ReadBool();
                    break;
            }
            if (this.m_Picked)
            {
                this.Unlink();
                this.Delete();
            }
        }
    }
}