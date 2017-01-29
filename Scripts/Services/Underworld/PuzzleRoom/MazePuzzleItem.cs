using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Gumps;

namespace Server.Items 
{
	public class MazePuzzleItem : BaseDecayingItem
	{
		private MagicKey m_Key;
		private List<int> m_Path;
		private List<int> m_Progress;
		
		public List<int> Path { get { return m_Path; } set { m_Path = value; } }
		public List<int> Progress { get { return m_Progress; } set { m_Progress = value; } }
	
		[CommandProperty(AccessLevel.GameMaster)]
		public MagicKey Key { get { return m_Key; } set { m_Key = value; } }

        public override int LabelNumber { get { return 1113379; } } // Puzzle Board
		public override int Lifespan { get { return 600; } }
	
		[Constructable]
		public MazePuzzleItem(MagicKey key) : base(0x2AAA) 
		{
			Hue = 914;
			m_Key = key;
		}
		
		public override void OnDoubleClick(Mobile from)
		{
			if(!IsChildOf(from.Backpack))
				from.SendLocalizedMessage(500325); // I am too far away to do that.
			else if(IsInPuzzleRoom(from))
			{
                from.CloseGump(typeof(PuzzleChest.PuzzleGump));
                from.CloseGump(typeof(PuzzleChest.StatusGump));
				from.CloseGump(typeof(MazePuzzleGump));
				from.SendGump(new MazePuzzleGump(from, this, m_Path, m_Progress));
			}
		}

        private static Rectangle2D m_Bounds = new Rectangle2D(1089, 1162, 16, 12);

        public static bool IsInPuzzleRoom(Mobile from)
        {
            return from.Map == Map.TerMur && m_Bounds.Contains(new Point2D(from.X, from.Y));
        }

        public override void OnDelete()
        {
            Mobile m = RootParent as Mobile;

            if (m != null)
            {
                if (m.HasGump(typeof(MazePuzzleGump)))
                    m.CloseGump(typeof(MazePuzzleGump));
            }
        }
		
		private Timer m_DamageTimer;
		
		public void DoDamage(Mobile m)
		{
			if(m_DamageTimer != null && m_DamageTimer.Running)
				m_DamageTimer.Stop();
				
			m_DamageTimer = new InternalTimer(this, m);
			m_DamageTimer.Start();
		}
		
		public void ApplyShock(Mobile m, int tick)
		{
            if (m == null || !m.Alive || this.Deleted)
            {
                if (m_DamageTimer != null)
                    m_DamageTimer.Stop();
            }
            else
            {
                int damage = (int)(75 / Math.Max(1, tick - 1)) + Utility.RandomMinMax(1, 9);

                AOS.Damage(m, damage, 0, 0, 0, 0, 100);

                m.BoltEffect(0);

                m.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.CenterFeet);
                m.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);
                m.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Head);
                m.PlaySound(0x1DC);

                m.LocalOverheadMessage(Server.Network.MessageType.Regular, 0x21, 1114443); // * Your body convulses from electric shock *
                m.NonlocalOverheadMessage(Server.Network.MessageType.Regular, 0x21, 1114443, m.Name); //  * ~1_NAME~ spasms from electric shock *
            }
		}
		
		private class InternalTimer : Timer
		{
			private MazePuzzleItem m_Item;
			private Mobile m_From;
			private DateTime m_NextDamage;
			private int m_Tick;
			
			public InternalTimer(MazePuzzleItem item, Mobile from) : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
			{
				m_Item = item;
				m_From = from;
				m_NextDamage = DateTime.UtcNow;
				m_Tick = 0;
				
				if(item != null)
					item.ApplyShock(from, 0);
			}
			
			protected override void OnTick()
			{
				m_Tick++;
				
				if(m_From == null || m_Item == null || !m_From.Alive || m_Item.Deleted)
				{
					this.Stop();
					return;
				}
				
				if(DateTime.UtcNow > m_NextDamage)
				{
					m_Item.ApplyShock(m_From, m_Tick);
					
					int delay;
					
					if(m_Tick < 3)
						delay = 2;
					else if (m_Tick < 5)
						delay = 4;
					else
						delay = 6;
					
					if(m_Tick >= 10)
						this.Stop();
					else
                        m_NextDamage = DateTime.UtcNow + TimeSpan.FromSeconds(delay);
				}
			}
		}
		
		public void OnPuzzleCompleted(Mobile m)
		{
			if(m != null)
			{
				Container pack = m.Backpack;
				
				if(pack != null)
				{
					Item copperKey = pack.FindItemByType(typeof(CopperPuzzleKey));
					Item goldKey = pack.FindItemByType(typeof(GoldPuzzleKey));
					
					if(copperKey == null)
						pack.DropItem(new CopperPuzzleKey());
					else if (goldKey == null)
						pack.DropItem(new GoldPuzzleKey());
					else
						return;
					
					m.SendLocalizedMessage(1113382); // You've solved the puzzle!! An item has been placed in your bag.
				}
				
				//TODO: Message/Effects?
			}

            Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerCallback(Delete));
		}
		
		public MazePuzzleItem( Serial serial ) : base(serial) 
		{
		}
		
		public override void Serialize(GenericWriter writer) 
		{
			base.Serialize(writer);
			writer.Write((int)0); // ver
			writer.Write(m_Key);
		}
		
		public override void Deserialize(GenericReader reader) 
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			m_Key = reader.ReadItem() as MagicKey;
		}
	}
}