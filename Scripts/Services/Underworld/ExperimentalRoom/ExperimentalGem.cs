using Server;
using System;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
	public enum Room
	{
        RoomZero    = 0,
		RoomOne     = 1,
		RoomTwo     = 2, 
		RoomThree   = 3,
        RoomFour    = 4
	}
		
	public class ExperimentalGem : BaseDecayingItem
	{
        private static readonly int Neutral     = 0x356;  
        private static readonly int Red         = 0x26;
        private static readonly int White       = 0x481;
        private static readonly int Blue        = 0x4;
        private static readonly int Pink        = 0x4B2;
        private static readonly int Orange      = 0x30;
        private static readonly int LightGreen  = 0x3D;
        private static readonly int DarkGreen   = 0x557;
        private static readonly int Brown       = 0x747;

        private static readonly TimeSpan HueToHueDelay = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan HueToLocDelay = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan RoomToRoomDelay = TimeSpan.FromSeconds(20);

        private static readonly Rectangle2D m_Entrance = new Rectangle2D(980, 1117, 17, 3);

		private bool m_Active;
		private bool m_IsExtremeHue;
		private bool m_Complete;
		private Room m_CurrentRoom;
		private double m_Completed;
		private double m_ToComplete;
		private int m_LastIndex;
        private int m_CurrentHue;
        private int m_Span;
		private Timer m_Timer;
		private DateTime m_Expire;
        private Mobile m_Owner;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Active { get { return m_Active; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsExtremeHue { get { return m_IsExtremeHue; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public bool Complete { get { return m_Complete; } set { m_Complete = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
        public Room CurrentRoom { get { return m_CurrentRoom; } set { m_CurrentRoom = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public double Completed { get { return m_Completed; } set { m_Completed = value; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public double ToComplete { get { return m_ToComplete; } set { m_ToComplete = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentHue { get { return m_CurrentHue; } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int LastIndex { get { return m_LastIndex; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return m_Owner; } set { m_Owner = value; } }

        public override int Lifespan { get { return m_Span; } }

        public override int LabelNumber
        { 
            get
            {
                if (m_Active)
                    return 1113409; // An Experimental Gem [Activated]
                return 1113380; //An Experimental Gem
            } 
        } 
		
		[Constructable]
		public ExperimentalGem() : base(6463)
		{
			m_LastIndex = -1;
			m_IsExtremeHue = false;
            m_CurrentHue = Neutral;
			m_CurrentRoom = Room.RoomZero;
			m_Active = false;
			m_Expire = DateTime.MaxValue;
            m_Span = 0;
			m_Completed = 0;
			m_ToComplete = 0;

            Hue = Neutral;
		}
		
		public override void OnDoubleClick(Mobile from)
		{
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
            else if (ExperimentalRoomController.IsInCooldown(from))
                from.SendLocalizedMessage(1113413); // You have recently participated in this challenge. You must wait 24 hours to try again.
            else if (!m_Active && m_Entrance.Contains(from.Location) && from.Map == Map.TerMur)
            {
                if (from.HasGump(typeof(ExperimentalGem.InternalGump)))
                    from.CloseGump(typeof(ExperimentalGem.InternalGump));

                from.SendGump(new InternalGump(this));
            }
            else if (m_Active)
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1113408); // The gem is already active. You must clear the rooms before it is destroyed!
		}

        public override void Decay()
        {
        }

        public void Activate(Mobile from)
        {
            m_Active = true;

            StartTimer();

            m_CurrentRoom = Room.RoomOne;
            m_ToComplete = Utility.RandomMinMax(5, 8);

            Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerCallback(BeginRoom_Callback));

            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1113405); // Your gem is now active. You may enter the Experimental Room.

            InvalidateProperties();
        }

        public void BeginRoom_Callback()
        {
            m_Timer = new InternalTimer(this);
            m_LastIndex = -1;
            SelectNewHue();
        }

        public override void StartTimer()
        {
            if (m_Span == 0)
            {
                TimeLeft = 1800;
                m_Span = 1800;
                base.StartTimer();
                InvalidateProperties();
            }
        }
		
		private void SelectNewHue()
		{
			int index = -1;
			int[] list = GetRoomHues();
			
			do
			{
				index = Utility.Random(list.Length);
			}
			while(index == m_LastIndex);

            m_Expire = DateTime.UtcNow + HueToLocDelay;
			m_Holding = false;
			m_LastIndex = index;

            if (IsExtreme(list[index]))
                m_IsExtremeHue = true;
			
			Hue = list[index];
            m_CurrentHue = Hue;
		}
		
		private bool m_Holding;

        public void OnTick()
        {
            if (m_Holding || m_Expire > DateTime.UtcNow)
                return;

            Mobile m = (Mobile)RootParent;
            int floorHue = GetFloorHue(m);
            int nextHue = GetRevertedHue(m, m_CurrentHue, floorHue);

            if (m != null && nextHue >= 0)                                   //Standing in the right spot
            {
                if (m_IsExtremeHue && nextHue != Neutral)					 // from extreme back to regular
                {
                    m_Completed += 0.5;

                    Hue = nextHue;
                    m_CurrentHue = Hue;
                    m_IsExtremeHue = false;
                    m_LastIndex = GetIndexFor(nextHue);
                    m_Expire = DateTime.UtcNow + HueToLocDelay;
                    m.PlaySound(0x51);

                    m_Completed += 0.5;
                }
                else										                // Neutralized, new color
                {
                    m_Completed++;

                    m_IsExtremeHue = false;
                    m_Holding = true;
                    Hue = Neutral;
                    m_CurrentHue = Neutral;

                    if (m_Completed < m_ToComplete)
                    {
                        Timer.DelayCall(HueToHueDelay, new TimerCallback(SelectNewHue));
                        m.PlaySound(0x51);
                    }
                }

                if (m_Completed >= m_ToComplete)		                    // next room or complete!
                {
                    if (m_CurrentRoom == Room.RoomThree)		            // puzzle completed
                    {
                        m_Holding = true;
                        Hue = Neutral;
                        m_CurrentHue = Neutral;
                        CompletePuzzle(m);

                        m.PlaySound(0x1FF);
                        m.LocalOverheadMessage(MessageType.Regular, 0x21, 1113403); // Congratulations!! The last room has been unlocked!! Hurry through to claim your reward!
                    }
                    else									                // on to the next room!
                    {
                        m_CurrentRoom++;

                        m_Holding = true;
                        Hue = Neutral;
                        m_CurrentHue = Neutral;

                        m_Completed = 0;

                        switch (m_CurrentRoom)
                        {
                            default:
                            case Room.RoomOne:
                                m_ToComplete = Utility.RandomMinMax(5, 8);
                                break;
                            case Room.RoomTwo:
                                m_ToComplete = Utility.RandomMinMax(10, 15);
                                break;
                            case Room.RoomThree:
                                m_ToComplete = Utility.RandomMinMax(15, 25);
                                break;
                        }

                        m_LastIndex = -1;
                        Timer.DelayCall(RoomToRoomDelay, new TimerCallback(SelectNewHue));

                        m.PlaySound(0x1FF);
                        m.LocalOverheadMessage(MessageType.Regular, 0x21, 1113402); // The next room has been unlocked! Hurry through the door before your gem's state changes again!
                    }
                }
            }
            else if (m_IsExtremeHue) 							//Already extreme, failed
            {
                if (m != null && m.AccessLevel == AccessLevel.Player)
                    OnPuzzleFailed(m);
                else
                {
                    if (m != null)
                        m.SendMessage("As a GM, you get another chance!");

                    m_Expire = DateTime.UtcNow + HueToLocDelay;
                }

                if (m != null)
                    m.LocalOverheadMessage(MessageType.Regular, 0x21, 1113400); // You fail to neutralize the gem in time and are expelled from the room!!
            }
            else if (Hue != -1)									                //set to extreme hue
            {
                int hue = GetExtreme(m_CurrentHue);

                Hue = hue;
                m_CurrentHue = hue;
                m_IsExtremeHue = true;
                m_LastIndex = GetIndexFor(hue);

                if (m != null)
                    m.LocalOverheadMessage(MessageType.Regular, 0x21, 1113401); // The state of your gem worsens!!

                m_Expire = DateTime.UtcNow + HueToLocDelay;
            }
        }
        

        private string GetFloorString(int hue)
        {
            switch (hue)
            {
                case 0x481:                         //White
                    return "white";
                case 0x4B2:                         //Pink
                    return "pink";
                case 0x30:                          //Orange
                    return "orange";
                case 0x3D:                          //LightGreen
                    return "light green";
                case 0x26:                          //Red
                    return "red";
                case 0x4:                           //Blue
                    return "blue";
                case 0x557:                         //Dark Green
                    return "dark green";
                case 0x747:                         //Brown
                    return "brown";
            }

            return "NOT STANDING ON A COLOR";
        }

		private void CompletePuzzle(Mobile m)
		{
			if(m_Timer != null)
			{
				m_Timer.Stop();
				m_Timer = null;
			}
			
			m_Complete = true;
            Hue = Neutral;
            m_CurrentHue = Hue;

            m_CurrentRoom = Room.RoomFour;
		}
		
		private void OnPuzzleFailed(Mobile m)
		{
            if (m != null)
            {
                int x = Utility.RandomMinMax(m_Entrance.X, m_Entrance.X + m_Entrance.Width);
                int y = Utility.RandomMinMax(m_Entrance.Y, m_Entrance.Y + m_Entrance.Height);
                int z = m.Map.GetAverageZ(x, y);

                Point3D from = m.Location;
                Point3D p = new Point3D(x, y, z);

                m.PlaySound(0x1FE);
                Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                Effects.SendLocationParticles(EffectItem.Create(p, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                BaseCreature.TeleportPets(m, p, Map.TerMur);
                m.MoveToWorld(p, Map.TerMur);
            }
			
			Reset();
		}
		
		private void Reset()
		{
			if(m_Timer != null)
			{
				m_Timer.Stop();
				m_Timer = null;
			}
			
			m_Complete = false;
			m_Completed = 0;
			m_ToComplete = 0;
			m_IsExtremeHue = false;
            m_Active = false;
			m_CurrentRoom = Room.RoomOne;
			m_LastIndex = -1;
			m_Expire = DateTime.MaxValue;
            m_CurrentHue = Neutral;
            Hue = Neutral;
            InvalidateProperties();
		}
		
		public override void Delete()
		{
			if(m_Timer != null)
				m_Timer.Stop();

            if(m_Owner != null)
                ExperimentalRoomController.AddToTable(m_Owner);
	
			base.Delete();
		}
		
		private class InternalTimer : Timer
		{
			private ExperimentalGem m_Gem;
			
			public InternalTimer(ExperimentalGem gem) : base(TimeSpan.FromSeconds(.5), TimeSpan.FromSeconds(.5))
			{
				m_Gem = gem;
				Start();
			}
	
			protected override void OnTick()
			{
				if(m_Gem != null)
					m_Gem.OnTick();
				else
					Stop();
			}
		}
		
		private int[] GetRoomHues()
		{
			switch(m_CurrentRoom)
			{
				default:
				case Room.RoomOne:
					return m_RoomHues[0];
				case Room.RoomTwo:
					return m_RoomHues[1];
				case Room.RoomThree:
					return m_RoomHues[2];
			}
		}

        private Rectangle2D GetRoomRec()
        {
			switch(m_CurrentRoom)
			{
				default:
				case Room.RoomOne:
					return m_RoomRecs[0];
				case Room.RoomTwo:
                    return m_RoomRecs[1];
				case Room.RoomThree:
                    return m_RoomRecs[2];
			}
        }

        public int GetIndexFor(int hue)
        {
            int[] hues = GetRoomHues();

            for (int i = 0; i < hues.Length; i++)
            {
                if (hue == hues[i])
                    return i;
            }

            return White; //Oops, something happened, this should never happened.
        }

        public static int GetExtreme(int hue)
        {
            switch (hue)
            {
                case 0x4B2: return Red;     // Pink to Red
                case 0x481: return Blue;    // White to Blue
                case 0x30: return Brown;    // Orange to Brown
                case 0x3D: return DarkGreen;// LightGreen to DarkGreen
            }

            return -1;
        }

        public static int GetRegular(int hue)
        {
            switch (hue)
            {
                case 0x26: return Pink;         // Red to Pink
                case 0x4: return White;         // Blue to White
                case 0x747: return Orange;      // Brown to Orange
                case 0x557: return LightGreen;  // DarkGreen to LightGreen
            }

            return -1;
        }

        /*  Extreme     Normal      SlowOpposite        FastOpposite
         * 
         *  Red         Pink        White               Blue
         *  Blue        White       Pink                Red
         *  Brown       Orange      LightGreen          DarkGreen
         *  DarkGreen   LightGreen  Orange              Brown
         */

        /// <summary>
        /// Checks locations the player is standing, in relation to the gem hue.
        /// </summary>
        /// <param name="oldHue">current gem hue</param>
        /// <param name="floorHue">where they are standing</param>
        /// <returns>-1 if they are in the wrong spot, the new gem hue if in the correct spot</returns>
        public int GetRevertedHue(Mobile from, int oldHue, int floorHue)
        {
            if (from == null)
                return -1;

            if (!GetRoomRec().Contains(from.Location))
                return -1;

            switch (oldHue)
            {
                case 0x481:                         //White
                    if (floorHue == Pink || floorHue == Red)
                        return Neutral;
                    break;
                case 0x4B2:                         //Pink
                    if (floorHue == White || floorHue == Blue)
                        return Neutral;
                    break;
                case 0x30:                          //Orange
                    if (floorHue == LightGreen || floorHue == DarkGreen)
                        return Neutral;
                    break;
                case 0x3D:                          //LightGreen
                    if (floorHue == Orange || floorHue == Brown)
                        return Neutral;
                    break;
                case 0x26:                          //Red
                    if (floorHue == White)
                        return Pink;
                    if (floorHue == Blue)
                        return Neutral;
                    break;
                case 0x4:                           //Blue
                    if (floorHue == Pink)
                        return White;
                    if (floorHue == Red)
                        return Neutral;
                    break;
                case 0x557:                         //Dark Green
                    if (floorHue == Orange)
                        return Orange;
                    if (floorHue == Brown)
                        return Neutral;
                    break;
                case 0x747:                         //Brown
                  if (floorHue == LightGreen)
                        return Orange;
                    if (floorHue == DarkGreen)
                        return Neutral;
                    break;
            }

            return -1;
        }

        public static int GetFloorHue(Mobile from)
        {
            if (from == null || from.Map != Map.TerMur)
                return 0;

            for (int i = 0; i < m_FloorRecs.Length; i++)
            {
                if (m_FloorRecs[i].Contains(from.Location))
                    return m_FloorHues[i];
            }

            return 0;
        }

        public bool IsExtreme(int hue)
        {
            foreach (int i in ExtremeHues)
            {
                if (i == hue)
                    return true;
            }

            return false;
        }

        public int[] RegularHues = new int[]
        {
            White,
            Pink,
            LightGreen,
            Orange,
        };

        private int[] ExtremeHues = new int[]
        {
            Red,
            Blue, 
            Brown,
            DarkGreen
        };

		private static int[][] m_RoomHues = new int[][]
		{
            //Room One
            new int[] { White, Pink, Red, Blue },

            //Room Two
            new int[] { Pink, Blue, Red, Orange, LightGreen, White },

            //Room Three
            new int[] { Blue, Pink, DarkGreen, Orange, Brown, LightGreen, Red, White },
		};

        private static Rectangle2D[] m_FloorRecs = new Rectangle2D[]
        {
            //Room One
            new Rectangle2D(977, 1104, 5, 5),   // White, opposite of pink
            new Rectangle2D(987, 1104, 5, 5),   // Pink, opposite of white
            new Rectangle2D(977, 1109, 5, 5),   // Blue, opposite of red
            new Rectangle2D(987, 1109, 5, 5),   // Red, opposite of Blue

            //Room Two
            new Rectangle2D(977, 1092, 6, 3),   // White, opposite of pink
            new Rectangle2D(986, 1092, 6, 3),   //Red, opposite of Blue
            new Rectangle2D(977, 1095, 6, 3),   //Blue, opposite of red
            new Rectangle2D(986, 1095, 6, 3),   //LightGreen, oppostie of Orange
            new Rectangle2D(977, 1098, 6, 3),   //Orange, opposite of LightGreen
            new Rectangle2D(986, 1098, 6, 3),   //Pink, opposite of white

            //Room Three
            new Rectangle2D(977, 1074, 3, 5),   //Red, opposite of Blue
            new Rectangle2D(980, 1074, 3, 5),   //White, oppostie of Pink
            new Rectangle2D(986, 1074, 3, 5),   //Brown, oppostie of DarkGreen
            new Rectangle2D(989, 1074, 3, 5),   //LightGreen, opposite of Orange
            new Rectangle2D(977, 1079, 3, 5),   //DarkGreen, opposite of Brown
            new Rectangle2D(980, 1079, 3, 5),   //Orange, opposite of LightGreen
            new Rectangle2D(986, 1079, 3, 5),   //Blue, opposite of red
            new Rectangle2D(989, 1079, 3, 5),   //Pink, opposite of White
        };

        private static int[] m_FloorHues = new int[]
        {
            //Room One
            White,
            Pink,
            Red,
            Blue,

            //Room Two
            White,
            Red,
            Blue,
            LightGreen,
            Orange,
            Pink,

            //Room Three
            Red, 
            White,
            Brown,
            LightGreen,
            DarkGreen,
            Orange,
            Blue,
            Pink
        };

        private static Rectangle2D[] m_RoomRecs = new Rectangle2D[]
        {
            new Rectangle2D(977, 1104, 15, 10), //RoomOne
            new Rectangle2D(977, 1092, 15, 9), //RoomTwo
            new Rectangle2D(977, 1074, 15, 10), //RoomThree
        };

        public ExperimentalGem(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
            writer.Write(m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            switch (v)
            {
                case 1:
                    m_Owner = reader.ReadMobile();
                    goto case 0;
                case 0:
                    break;
            }

            Reset();
        }

        private class InternalGump : Gump
        {
            private ExperimentalGem m_Gem;

            public InternalGump(ExperimentalGem gem) : base(50, 50)
            {
                m_Gem = gem;

                AddPage(0);
                AddBackground(0, 0, 297, 115, 9200);

                AddImageTiled(5, 10, 285, 25, 2624);
                AddHtmlLocalized(10, 15, 275, 25, 1113407, 0x7FFF, false, false); // Experimental Room Access

                AddImageTiled(5, 40, 285, 40, 2624);
                AddHtmlLocalized(10, 40, 275, 40, 1113391, 0x7FFF, false, false); // Click CANCEL to read the instruction book or OK to start the timer now.

                AddButton(5, 85, 4017, 4018, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 87, 80, 25, 1011012, 0x7FFF, false, false);   //CANCEL

                AddButton(215, 85, 4023, 4024, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(250, 87, 80, 25, 1006044, 0x7FFF, false, false);  //OK
            }

            public override void OnResponse(Server.Network.NetState state, RelayInfo info)
            {
                if (info.ButtonID == 1)
                    m_Gem.Activate(state.Mobile);
            }
        }
	}
}