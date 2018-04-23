using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Targeting;

namespace Server.Items.Crops
{
	public class EmptyLobsterTrap : BaseLobsterTrap
	{
        public override bool CanSetTrap { get { return true; } }

		[Constructable]
		public EmptyLobsterTrap() : this( 1 ) { }

		[Constructable]
        public EmptyLobsterTrap(int amount)
            : base(0x44CF)
		{
            Stackable = true;
			Weight = .1;
			Movable = true;
            Name = "empty lobster trap";
		}

		public override void OnDoubleClick( Mobile from )
		{
            if (from.Mounted && !TrapHelper.CanWorkMounted)
            {
                from.SendMessage( "You cannot deploy a Lobster Trap why mounted." );
                return;
            }

            from.BeginTarget(-1, true, TargetFlags.None, new TargetCallback(OnTarget));
		}

        public void OnTarget(Mobile from, object obj)
        {

            IPoint3D p3D = obj as IPoint3D;

            if (p3D == null)
                return;

            Point3D m_pnt = from.Location;
            Map m_map = from.Map;
            int x = p3D.X, y = p3D.Y;
            Point3D p = new Point3D(x, y, m_map.GetAverageZ(x, y));

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010);
                return;
            }
            else if (!from.InRange(p3D, 4))
            {
                from.SendMessage("The trap is too cumbersome to deploy that far away.");
                return;
            }

            ArrayList trapshere = TrapHelper.CheckTrap(p, m_map, 0);
            if (trapshere.Count > 0)
            {
                from.SendMessage("This location is too close to another trap. ");
                return;
            }

            ArrayList trapsnear = TrapHelper.CheckTrap(p, m_map, 1);
            if ((trapsnear.Count > 0))
            {
                from.SendMessage("You can't place any more");
                return;
            }

            if (this.BumpZ)
                ++m_pnt.Z;

            if (!from.Mounted)
                from.Animate(32, 5, 1, true, false, 0);

            from.SendMessage("You deploy the Lobster trap.");

            this.Consume();

            Item item = new Buoy(from);
            item.MoveToWorld(p, m_map);
        }

        public EmptyLobsterTrap(Serial serial)
            : base(serial)
        {
        }

		public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 );
        }

		public override void Deserialize( GenericReader reader )
        { 
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
	}

	public class Buoy : BaseLobsterTrap
	{
		private static Mobile f_owner;
		public Timer thisTimer;

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner{ get{ return f_owner; } set{ f_owner = value; } }

		[Constructable]
		public Buoy( Mobile Owner ) : base( 0x44CC )
		{
			Movable = false;
			Name = "Buoy";
			f_owner = Owner;
			init( this );
		}
        public static void init(Buoy buoy)
		{
            buoy.thisTimer = new TrapHelper.CatchTimer(buoy, typeof(Bobbingbouy), buoy.Owner);
            buoy.thisTimer.Start();
		}

		public override void OnDoubleClick( Mobile from )
		{
            if (from.Mounted && !TrapHelper.CanWorkMounted)
            {
                from.SendMessage( "You cannot pull the trap from the water why mounted!" );
                return;
            }
			else from.SendMessage( "The trap is not ready yet!" );
		}

        public Buoy(Serial serial)
            : base(serial)
        {
        }

		public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer ); writer.Write( (int) 0 );
            writer.Write( f_owner );
        }

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			f_owner = reader.ReadMobile();
			init( this );
		}
	}

    public class Bobbingbouy : BaseLobsterTrap
    {
        private Mobile f_owner;
        private DateTime m_lastvisit;
        private Timer m_Timer;

        [Constructable]
		public Bobbingbouy() : this(null) 
        {
            this.m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(5));
            this.m_Timer.Start();
        }

		[Constructable]
        public Bobbingbouy(Mobile Owner)
            : base(0x44CB)
		{
			Movable = false;
			Name = "buoy";
			f_owner = Owner;
			m_lastvisit = DateTime.UtcNow;

            this.m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(5));
            this.m_Timer.Start();
		}

        public override void OnDoubleClick(Mobile from)
        {
            if (f_owner == null || f_owner.Deleted)
                f_owner = from;

            if (from != f_owner)
            {
                from.SendMessage("You realize that the trap isn't yours so you leave it alone.");//1116391
                return;
            }

            FullTrap trap = new FullTrap();
            from.AddToBackpack(trap);
            this.Delete();
        }

        public Bobbingbouy(Serial serial) : base(serial) { }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Timer != null)
                this.m_Timer.Stop();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( m_lastvisit );
			writer.Write( f_owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_lastvisit = reader.ReadDateTime();
			f_owner = reader.ReadMobile();
            this.m_Timer = new InternalTimer(this, TimeSpan.Zero);
            this.m_Timer.Start();
		}

        private void ChangeDirection()
        {
            if (this.ItemID == 0x44CB)
                this.ItemID += 1;
            else if (this.ItemID == 0x44CC)
                this.ItemID += 1;
            else if (this.ItemID == 0x44CD)
                this.ItemID += 1;
            else if (this.ItemID == 0x44CE)
                this.ItemID -= 3;
        }

        private class InternalTimer : Timer
        {
            private readonly Bobbingbouy bouy;
            public InternalTimer(Bobbingbouy b, TimeSpan delay) : base(delay, TimeSpan.FromMinutes(1))
            {
                this.bouy = b;
                this.Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                if (this.bouy != null && !this.bouy.Deleted)
                    this.bouy.ChangeDirection();
                bouy.Delete();
            }
        }
    }

	public class FullTrap : BaseLobsterTrap
	{
        private string fullname;
		private DateTime lastpicked;
		private Mobile f_owner;
		private DateTime m_lastvisit;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastSowerVisit{ get{ return m_lastvisit; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner{ get{ return f_owner; } set{ f_owner = value; } }

        public string FullName { get { return fullname; } set { fullname = value; } }
		public DateTime LastPick{ get{ return lastpicked; } set{ lastpicked = value; } }

		[Constructable]
		public FullTrap() : this(null)
        { }

        [Constructable]
        public FullTrap(Mobile Owner)
            : base(0x44CF)
        {
            Movable = true;
            Stackable = true;
            Name = "Full Lobster Trap";
            f_owner = Owner;
            m_lastvisit = DateTime.UtcNow;
        }

        public override void OnDoubleClick(Mobile from)
        {
            switch (Utility.Random(3))
            {
                case 0: from.AddToBackpack(new EmptyLobsterTrap());
                    from.SendMessage("There is nothing left in the trap to remove.");
                    this.Delete();break;
                case 1: from.AddToBackpack(new EmptyLobsterTrap());
                    from.AddToBackpack(new Fish(Utility.Random(1, 4)));
                    from.SendMessage("You remove Lobsters from the trap and put it in your pack.");
                        this.Delete(); break;
                case 2: from.AddToBackpack(new Fish());
                    from.SendMessage("You remove Lobsters from the trap and put it in your pack.");
                    this.Delete(); break;
            }
        }

        public FullTrap(Serial serial) : base(serial) { }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( m_lastvisit );
			writer.Write( f_owner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_lastvisit = reader.ReadDateTime();
			f_owner = reader.ReadMobile();
            //init(this, true);
		}
	}
}