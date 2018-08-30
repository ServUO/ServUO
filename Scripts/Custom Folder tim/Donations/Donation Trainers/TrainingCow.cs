using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "a training cow corpse" )]
	public class TrainingCow : BaseCreature
	{
        #region ItemDeleteTimer [01-04]

        private static readonly TimeSpan m_Delay = TimeSpan.FromDays(3);
        private InternalTimer m_ItemDeleteTimer;

        [CommandProperty(AccessLevel.GameMaster, true)]
        public TimeSpan Remaining
        {
            get { if (m_ItemDeleteTimer != null) return m_ItemDeleteTimer.GetDelay().Subtract(DateTime.Now); else return TimeSpan.Zero; }
        }

        #endregion Edited By: A.A.R

       
    [Constructable]
		public TrainingCow() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a training cow";
			Body = Utility.RandomList( 0xD8, 0xE7 );
			Hue = 1153;
			BaseSoundID = 0x78;
			
		  SetStr( 1000, 1000 );
			SetDex( 1000 );
			SetInt( 1000 );

			SetHits( 300000 , 300000 );

			SetDamage( 1, 1 );

			SetDamageType( ResistanceType.Physical, 1 );

			SetResistance( ResistanceType.Physical, 1 );
			SetResistance( ResistanceType.Fire, 1 );
			SetResistance( ResistanceType.Cold, 1 );
			SetResistance( ResistanceType.Poison, 1 );
			SetResistance( ResistanceType.Energy, 1 );

			SetSkill( SkillName.Tactics, 1.0, 1.0 );
			SetSkill( SkillName.Wrestling, 1.0, 1.0 );
			SetSkill( SkillName.Anatomy, 1.0, 1.0 );

			Fame = 2500;
			Karma = -2500;
      CantWalk = true;
			VirtualArmor = 200;
      if (m_ItemDeleteTimer == null)
        {
        m_ItemDeleteTimer = new TrainingCow.InternalTimer(this, DateTime.Now + m_Delay);
        m_ItemDeleteTimer.Start();
        Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerCallback(InvalidateProperties));
        }
		}

		public override int Meat{ get{ return 12; } }
		public override int Hides{ get{ return 8; } } 

        #region ItemDeleteTimer [02-04]

        public override void GetProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            if (m_ItemDeleteTimer != null)
            {
                string lisths;
                TimeSpan timetouse = m_ItemDeleteTimer.GetDelay().Subtract(DateTime.Now);

                if (timetouse.Days > 0)
                {
                    int day = timetouse.Days;
                    lisths = String.Format("{0} days", day.ToString());
                }
                else if (timetouse.Minutes > 0)
                {
                    int min = timetouse.Minutes;
                    lisths = String.Format("{0} minutes", min.ToString());
                }
                else if (timetouse.Seconds > 0)
                {
                    int sec = timetouse.Seconds;
                    lisths = String.Format("{0} seconds", sec.ToString());
                }
                else
                {
                    lisths = ("<BASEFONT COLOR=#C0C0C0> | <BASEFONT COLOR=#C0C0C0>");
                }

                list.Add("<BASEFONT COLOR=#C0C0C0>Time Remaining: {0}<BASEFONT COLOR=#C0C0C0>", lisths);
                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerCallback(InvalidateProperties));
            }
        }

        protected class InternalTimer : Timer
        {
            private Mobile m_Item;
            public InternalTimer(Mobile item, DateTime end): base(end.Subtract(DateTime.Now))
            {
                m_Item = item;
                Priority = TimerPriority.FiveSeconds;
            }

            public DateTime GetDelay()
            {
                return this.Next;
            }
            
            protected override void OnTick()
            {
                if (m_Item != null && !m_Item.Deleted)
                {
                    m_Item.Delete();
                }

                Stop();
            }            

        }

        #endregion Edited By: A.A.R    

		public TrainingCow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
            #region ItemDeleteTimer [03-04]

            writer.Write((bool)(m_ItemDeleteTimer != null));
            if (m_ItemDeleteTimer != null)
            {
                writer.WriteDeltaTime(m_ItemDeleteTimer.GetDelay());
            }

            #endregion Edited By: A.A.R
			
		}
    public override bool OnBeforeDeath()
		{
    World.Broadcast(0x32, true, "Die Kuh ist gestorben!");
    return base.OnBeforeDeath();
    }
    
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
            #region ItemDeleteTimer [04-04]

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        if (reader.ReadBool())
                        {
                            DateTime end = reader.ReadDeltaTime();
                            //Insert Item Name Below:
                            m_ItemDeleteTimer = new TrainingCow.InternalTimer(this, end);
                            m_ItemDeleteTimer.Start();
                        }
                        break;
                    }
            }

            #endregion Edited By: A.A.R      
      
      
			
		}
		public override void OnGotMeleeAttack( Mobile attacker )
		{
		 SetHits( 300000, 300000 );
		}
    
  }
}
