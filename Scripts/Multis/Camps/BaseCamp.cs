using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
    public abstract class BaseCamp : BaseMulti
    {
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), OnTick);
        }

        public static List<BaseCamp> _Camps = new List<BaseCamp>();

        private List<Item> m_Items;
        private List<Mobile> m_Mobiles;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TimeOfDecay { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseCreature Prisoner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseContainer Treasure1 { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseContainer Treasure2 { get; set; }

        public override bool HandlesOnMovement
        {
            get { return true; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual TimeSpan DecayDelay { get { return TimeSpan.FromMinutes(30.0); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Decaying { get { return TimeOfDecay != DateTime.MinValue; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceDecay
        {
            get { return false; }
            set { SetDecayTime(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RestrictDecay { get; set; }

        public BaseCamp(int multiID)
            : base(multiID)
        {
            m_Items = new List<Item>();
            m_Mobiles = new List<Mobile>();

            Visible = false;

            CheckAddComponents();
            _Camps.Add(this);
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

        public void CheckAddComponents()
        {
            if (Deleted)
                return;

            AddComponents();
        }

        public virtual void AddComponents()
        {
        }

        public virtual void CheckDecay()
        {
            if (RestrictDecay)
                return;

            if (!Decaying)
            {
                if (((Treasure1 == null || Treasure1.Items.Count == 0) && (Treasure2 == null || Treasure2.Items.Count == 0)) ||
                    (Prisoner != null && (Prisoner.Deleted || !Prisoner.CantWalk)))
                {
                    SetDecayTime();
                }
            }
            else if(TimeOfDecay < DateTime.UtcNow)
            {
                Delete();
            }
        }

        public virtual void SetDecayTime()
        {
            if (Deleted || RestrictDecay)
                return;

            TimeOfDecay = DateTime.UtcNow + DecayDelay;
        }

        public virtual void AddItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            if (Map == null)
                return;

            m_Items.Add(item);

            int zavg = this.Map.GetAverageZ(X + xOffset, Y + yOffset);

            if (!Map.CanFit(X + xOffset, Y + yOffset, zavg, item.ItemData.Height))
            {
                for (int z = 1; z <= 39; z++)
                {
                    if (Map.CanFit(X + xOffset, Y + yOffset, zavg + z, item.ItemData.Height))
                    {
                        zavg += z;
                        break;
                    }
                }
            }

            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, zavg + zOffset), Map);
        }

        public virtual void AddMobile(Mobile m, int xOffset, int yOffset, int zOffset)
        {
            if (Map == null)
                return;

            if(!m_Mobiles.Contains(m))
                m_Mobiles.Add(m);

            int zavg = Map.GetAverageZ(X + xOffset, Y + yOffset);

            if (!Map.CanSpawnMobile(X + xOffset, Y + yOffset, zavg))
            {
                for (int z = 1; z <= 39; z++)
                {
                    if (Map.CanSpawnMobile(X + xOffset, Y + yOffset, zavg + z))
                    {
                        zavg += z;
                        break;
                    }
                }
            }

            m.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, zavg + zOffset), Map);
            SetCreature(m as BaseCreature);
        }

        private void SetCreature(BaseCreature bc)
        {
            if (bc != null)
            {
                //int zavg = Map.GetAverageZ(bc.X, bc.Y);
                IPoint3D p = bc.Location; //new Point3D(bc.X, bc.Y, zavg);

                Server.Spells.SpellHelper.GetSurfaceTop(ref p);

                Point3D loc = new Point3D(p);
                bc.RangeHome = bc.IsPrisoner ? 0 : 6;
                bc.Home = loc;

                if (bc.Location != loc)
                    bc.Location = loc;

                if (bc is BaseVendor || bc is Banker)
                    bc.Direction = Direction.South;
            }
        }

        public virtual void OnEnter(Mobile m)
        {
        }

        public virtual void OnExit(Mobile m)
        {
        }

        public override void OnLocationChange(Point3D old)
        {
            foreach (var item in m_Items)
            {
                item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
            }

            foreach (var m in m_Mobiles)
            {
                m.Location = new Point3D(X + (m.X - old.X), Y + (m.Y - old.Y), Z + (m.Z - old.Z));
                SetCreature(m as BaseCreature);
            }
        }

        public override void OnMapChange()
        {
            foreach (var item in m_Items)
            {
                item.Map = Map;
            }

            foreach (var m in m_Mobiles)
            {
                m.Map = Map;
            }
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

                if (!bc.IsPrisoner)
                    m_Mobiles[i].Delete();
                else if (m_Mobiles[i].CantWalk)
                    m_Mobiles[i].Delete();
            }

            m_Items.Clear();
            m_Mobiles.Clear();
            _Camps.Remove(this);
        }

		protected virtual void AddCampChests()
		{
			Treasure1 = new TreasureLevel1();
            ((TreasureLevel1)Treasure1).Locked = false;
            AddItem(Treasure1, 2, 2, 0);

            Treasure2 = new TreasureLevel3();
            AddItem(Treasure2, -2, -2, 0);
		}

		public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            writer.Write(Prisoner);
            writer.Write(Treasure1);
            writer.Write(Treasure2);

            writer.Write(m_Items, true);
            writer.Write(m_Mobiles, true);
            writer.WriteDeltaTime(TimeOfDecay);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                    {
                        Prisoner = reader.ReadMobile() as BaseCreature;
                        Treasure1 = reader.ReadItem() as BaseContainer;
                        Treasure2 = reader.ReadItem() as BaseContainer;

                        goto case 0;
                    }
                case 1:
                case 0:
                    {
                        m_Items = reader.ReadStrongItemList();
                        m_Mobiles = reader.ReadStrongMobileList();
                        TimeOfDecay = reader.ReadDeltaTime();

                        break;
                    }
            }

            if (version == 0 && ItemID == 0x10EE)
            {
                ItemID = 0x1F6D;
            }

            if (version == 1)
                Delete();

            if (Prisoner != null)
                Prisoner.IsPrisoner = true;

            _Camps.Add(this);
        }

        public static void OnTick()
        {
            List<BaseCamp> list = new List<BaseCamp>(_Camps);

            list.ForEach(c =>
                {
                    if (!c.Deleted && c.Map != null && c.Map != Map.Internal && !c.RestrictDecay)
                        c.CheckDecay();
                });

            ColUtility.Free(list);
        }
    }
}
