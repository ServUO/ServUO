using System;

namespace Server.Items
{ 
    public class SecretWall : Item
    { 
        private Point3D m_PointDest;
        private Map m_MapDest;
        private bool m_Locked;
        private bool m_Active;
        [Constructable]
        public SecretWall(int itemID)
            : base(itemID)
        {
            this.m_Active = true;
            this.m_Locked = true;
        }

        public SecretWall(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get
            {
                return this.m_PointDest;
            }
            set
            {
                this.m_PointDest = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest
        {
            get
            {
                return this.m_MapDest;
            }
            set
            {
                this.m_MapDest = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked
        {
            get
            {
                return this.m_Locked;
            }
            set
            {
                this.m_Locked = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return this.m_Active;
            }
            set
            {
                this.m_Active = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 2))
            {
                if (!this.m_Locked && this.m_Active)
                {
                    from.MoveToWorld(this.m_PointDest, this.m_MapDest);
                    from.SendLocalizedMessage(1072790); // The wall becomes transparent, and you push your way through it.
                }
                else
                    from.Say(502684); // This door appears to be locked.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((Point3D)this.m_PointDest);
            writer.Write((Map)this.m_MapDest);
            writer.Write((bool)this.m_Locked);	
            writer.Write((bool)this.m_Active);						
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            this.m_PointDest = reader.ReadPoint3D();
            this.m_MapDest = reader.ReadMap();
            this.m_Locked = reader.ReadBool();
            this.m_Active = reader.ReadBool();
        }
    }

    public class SecretSwitch : Item
    {
        private SecretWall m_Wall;
        private bool m_TurnedOn;
        [Constructable]
        public SecretSwitch()
            : this(0x108F, null)
        { 
        }

        [Constructable]
        public SecretSwitch(int itemID, SecretWall wall)
            : base(itemID)
        { 
            this.m_Wall = wall;
        }

        public SecretSwitch(Serial serial)
            : base(serial)
        { 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecretWall Wall
        {
            get
            {
                return this.m_Wall;
            }
            set
            {
                this.m_Wall = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get
            {
                return this.m_TurnedOn;
            }
            set
            {
                this.m_TurnedOn = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 2) && this.m_Wall != null)
            {
                if (this.m_TurnedOn)
                    this.ItemID -= 1;
                else
                {
                    this.ItemID += 1;
					
                    Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(Lock));
                }
					
                this.m_TurnedOn = !this.m_TurnedOn;
                this.m_Wall.Locked = !this.m_Wall.Locked;
				
                if (Utility.RandomBool())
                {
                    Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0);
                    Effects.PlaySound(from.Location, from.Map, 0x229);
					
                    AOS.Damage(from, Utility.Random(4, 5), 0, 0, 0, 100, 0);
                }
				
                from.SendLocalizedMessage(1072739); // You hear a click behind the wall.
                from.PlaySound(0x3E5);
            }
        }

        public virtual void Lock()
        {
            if (this.m_Wall != null)
            {
                if (this.m_TurnedOn)
                    this.ItemID -= 1;
			
                this.m_TurnedOn = false;
                this.m_Wall.Locked = true;				
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((Item)this.m_Wall);
            writer.Write((bool)this.m_TurnedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            this.m_Wall = reader.ReadItem() as SecretWall;
            this.m_TurnedOn = reader.ReadBool();
        }
    }
}