using System;

namespace Server.Items
{
    public class MorphItem : Item
    {
        private int m_InactiveItemID;
        private int m_ActiveItemID;
        private int m_InRange;
        private int m_OutRange;
        [Constructable]
        public MorphItem(int inactiveItemID, int activeItemID, int range)
            : this(inactiveItemID, activeItemID, range, range)
        {
        }

        [Constructable]
        public MorphItem(int inactiveItemID, int activeItemID, int inRange, int outRange)
            : base(inactiveItemID)
        {
            this.Movable = false;

            this.InactiveItemID = inactiveItemID;
            this.ActiveItemID = activeItemID;
            this.InRange = inRange;
            this.OutRange = outRange;
        }

        public MorphItem(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int InactiveItemID
        {
            get
            {
                return this.m_InactiveItemID;
            }
            set
            {
                this.m_InactiveItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ActiveItemID
        {
            get
            {
                return this.m_ActiveItemID;
            }
            set
            {
                this.m_ActiveItemID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int InRange
        {
            get
            {
                return this.m_InRange;
            }
            set
            {
                if (value > 18)
                    value = 18;
                this.m_InRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int OutRange
        {
            get
            {
                return this.m_OutRange;
            }
            set
            {
                if (value > 18)
                    value = 18;
                this.m_OutRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentRange
        {
            get
            {
                return this.ItemID == this.InactiveItemID ? this.InRange : this.OutRange;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Utility.InRange(m.Location, this.Location, this.CurrentRange) || Utility.InRange(oldLocation, this.Location, this.CurrentRange))
                this.Refresh();
        }

        public override void OnMapChange()
        {
            if (!this.Deleted)
                this.Refresh();
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            if (!this.Deleted)
                this.Refresh();
        }

        public void Refresh()
        {
            bool found = false;

            foreach (Mobile mob in this.GetMobilesInRange(this.CurrentRange))
            {
                if (mob.Hidden && mob.IsStaff())
                    continue;

                found = true;
                break;
            }

            if (found)
                this.ItemID = this.ActiveItemID;
            else
                this.ItemID = this.InactiveItemID;

            this.Visible = (this.ItemID != 0x1);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_OutRange);

            writer.Write((int)this.m_InactiveItemID);
            writer.Write((int)this.m_ActiveItemID);
            writer.Write((int)this.m_InRange);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_OutRange = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        this.m_InactiveItemID = reader.ReadInt();
                        this.m_ActiveItemID = reader.ReadInt();
                        this.m_InRange = reader.ReadInt();

                        if (version < 1)
                            this.m_OutRange = this.m_InRange;

                        break;
                    }
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Refresh));
        }
    }
}