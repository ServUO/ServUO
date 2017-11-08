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
            m_Items = new List<Item>();
            m_Mobiles = new List<Mobile>();
            m_DecayDelay = TimeSpan.FromMinutes(30.0);
            RefreshDecay(true);

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
                return m_DecayDelay;
            }
            set
            {
                m_DecayDelay = value;
                RefreshDecay(true);
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
            if (Deleted)
                return;
			
            AddComponents();
        }

        public virtual void AddComponents()
        {
        }

        public virtual void RefreshDecay(bool setDecayTime)
        {
            if (Deleted)
                return;

            if (m_DecayTimer != null)
                m_DecayTimer.Stop();

            if (setDecayTime)
                m_DecayTime = DateTime.UtcNow + DecayDelay;

            m_DecayTimer = Timer.DelayCall(DecayDelay, new TimerCallback(Delete));
        }

        public virtual void AddItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            m_Items.Add(item);

            int zavg = Map.GetAverageZ(X + xOffset, Y + yOffset);
            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, zavg + zOffset), Map);
        }

        public virtual void AddMobile(Mobile m, int wanderRange, int xOffset, int yOffset, int zOffset)
        {
            m_Mobiles.Add(m);

            int zavg = Map.GetAverageZ(X + xOffset, Y + yOffset);
            Point3D loc = new Point3D(X + xOffset, Y + yOffset, zavg + zOffset);
            BaseCreature bc = m as BaseCreature;

            if (bc != null)
            {
                bc.RangeHome = wanderRange; 
                bc.Home = loc; 
            }

            if (m is BaseVendor || m is Banker)
                m.Direction = Direction.South;

            m.MoveToWorld(loc, Map);
        }

        public virtual void OnEnter(Mobile m)
        {
            RefreshDecay(true);
        }

        public virtual void OnExit(Mobile m)
        {
            RefreshDecay(true);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            bool inOldRange = Utility.InRange(oldLocation, Location, EventRange);
            bool inNewRange = Utility.InRange(m.Location, Location, EventRange);

            if (inNewRange && !inOldRange)
                OnEnter(m);
            else if (inOldRange && !inNewRange)
                OnExit(m);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int i = 0; i < m_Items.Count; ++i)
                m_Items[i].Delete();

            for (int i = 0; i < m_Mobiles.Count; ++i)
            {
                BaseCreature bc = (BaseCreature)m_Mobiles[i];

                if (bc.IsPrisoner == false)
                    m_Mobiles[i].Delete();
                else if (m_Mobiles[i].CantWalk == true)
                    m_Mobiles[i].Delete();
            }

            m_Items.Clear();
            m_Mobiles.Clear();
        }

		protected virtual void AddCampChests()
		{
			TreasureLevel1 crate = new TreasureLevel1();
			crate.Locked = false;
			AddItem(crate, 2, 2, 0);

			TreasureLevel3 chest = new TreasureLevel3();
			AddItem(chest, -2, -2, 0);
		}

		public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(m_Items, true);
            writer.Write(m_Mobiles, true);
            writer.WriteDeltaTime(m_DecayTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        m_Items = reader.ReadStrongItemList();
                        m_Mobiles = reader.ReadStrongMobileList();
                        m_DecayTime = reader.ReadDeltaTime();

                        RefreshDecay(false);

                        break;
                    }
            }

            if (version == 0 && ItemID == 0x10EE)
            {
                ItemID = 0x1F6D;
            }
        }
    }
}
