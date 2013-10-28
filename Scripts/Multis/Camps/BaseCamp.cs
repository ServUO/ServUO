using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
    public abstract class BaseCamp : BaseMulti
    {
        private List<Item> m_Items;
        private List<Mobile> m_Mobiles;
        private DateTime m_DecayTime;
        private Timer m_DecayTimer;
        private TimeSpan m_DecayDelay;
        public BaseCamp(int multiID)
            : base(multiID)
        {
            this.m_Items = new List<Item>();
            this.m_Mobiles = new List<Mobile>();
            this.m_DecayDelay = TimeSpan.FromMinutes(30.0);
            this.RefreshDecay(true);

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(CheckAddComponents));
        }

        public BaseCamp(Serial serial)
            : base(serial)
        {
        }

        public virtual int EventRange
        {
            get
            {
                return 10;
            }
        }
        public virtual TimeSpan DecayDelay
        {
            get
            {
                return this.m_DecayDelay;
            }
            set
            {
                this.m_DecayDelay = value;
                this.RefreshDecay(true);
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public void CheckAddComponents()
        {
            if (this.Deleted)
                return;
			
            this.AddComponents();
        }

        public virtual void AddComponents()
        {
        }

        public virtual void RefreshDecay(bool setDecayTime)
        {
            if (this.Deleted)
                return;

            if (this.m_DecayTimer != null)
                this.m_DecayTimer.Stop();

            if (setDecayTime)
                this.m_DecayTime = DateTime.UtcNow + this.DecayDelay;

            this.m_DecayTimer = Timer.DelayCall(this.DecayDelay, new TimerCallback(Delete));
        }

        public virtual void AddItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            this.m_Items.Add(item);

            int zavg = this.Map.GetAverageZ(this.X + xOffset, this.Y + yOffset);
            item.MoveToWorld(new Point3D(this.X + xOffset, this.Y + yOffset, zavg + zOffset), this.Map);
        }

        public virtual void AddMobile(Mobile m, int wanderRange, int xOffset, int yOffset, int zOffset)
        {
            this.m_Mobiles.Add(m);

            int zavg = this.Map.GetAverageZ(this.X + xOffset, this.Y + yOffset);
            Point3D loc = new Point3D(this.X + xOffset, this.Y + yOffset, zavg + zOffset);
            BaseCreature bc = m as BaseCreature;

            if (bc != null)
            {
                bc.RangeHome = wanderRange; 
                bc.Home = loc; 
            }

            if (m is BaseVendor || m is Banker)
                m.Direction = Direction.South;

            m.MoveToWorld(loc, this.Map);
        }

        public virtual void OnEnter(Mobile m)
        {
            this.RefreshDecay(true);
        }

        public virtual void OnExit(Mobile m)
        {
            this.RefreshDecay(true);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            bool inOldRange = Utility.InRange(oldLocation, this.Location, this.EventRange);
            bool inNewRange = Utility.InRange(m.Location, this.Location, this.EventRange);

            if (inNewRange && !inOldRange)
                this.OnEnter(m);
            else if (inOldRange && !inNewRange)
                this.OnExit(m);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int i = 0; i < this.m_Items.Count; ++i)
                this.m_Items[i].Delete();

            for (int i = 0; i < this.m_Mobiles.Count; ++i)
            {
                BaseCreature bc = (BaseCreature)this.m_Mobiles[i];

                if (bc.IsPrisoner == false)
                    this.m_Mobiles[i].Delete();
                else if (this.m_Mobiles[i].CantWalk == true)
                    this.m_Mobiles[i].Delete();
            }

            this.m_Items.Clear();
            this.m_Mobiles.Clear();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Items, true);
            writer.Write(this.m_Mobiles, true);
            writer.WriteDeltaTime(this.m_DecayTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Items = reader.ReadStrongItemList();
                        this.m_Mobiles = reader.ReadStrongMobileList();
                        this.m_DecayTime = reader.ReadDeltaTime();

                        this.RefreshDecay(false);

                        break;
                    }
            }
        }
    }

    public class LockableBarrel : LockableContainer
    {
        [Constructable]
        public LockableBarrel()
            : base(0xE77)
        {
            this.Weight = 1.0;
        }

        public LockableBarrel(Serial serial)
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

            if (this.Weight == 8.0)
                this.Weight = 1.0;
        }
    }
}