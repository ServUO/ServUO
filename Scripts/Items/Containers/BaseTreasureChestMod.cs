// Treasure Chest Pack - Version 0.99H
// By Nerun

using Server;
using Server.Items;
using Server.Network;
using System;

namespace Server.Items
{
	public abstract class BaseTreasureChestMod : LockableContainer
	{
		private ChestTimer m_DeleteTimer;
		//public override bool Decays { get{ return true; } }
		//public override TimeSpan DecayTime{ get{ return TimeSpan.FromMinutes( Utility.Random( 10, 15 ) ); } }
		public override int DefaultGumpID{ get{ return 0x42; } }
		public override int DefaultDropSound{ get{ return 0x42; } }
		public override Rectangle2D Bounds{ get{ return new Rectangle2D( 20, 105, 150, 180 ); } }
		public override bool IsDecoContainer{get{ return false; }}
		
		public BaseTreasureChestMod( int itemID ) : base ( itemID )
		{
			Locked = true;
			Movable = false;

			Key key = (Key)FindItemByType( typeof(Key) );

			if( key != null )
				key.Delete();

            if(Core.SA)
                RefinementComponent.Roll(this, 1, 0.08);
		}

		public BaseTreasureChestMod( Serial serial ) : base( serial )
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

			if( !Locked )
				StartDeleteTimer();
		}

		public override void OnTelekinesis( Mobile from )
		{
			if ( CheckLocked( from ) )
			{
				Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5022 );
				Effects.PlaySound( Location, Map, 0x1F5 );
				return;
			}

			base.OnTelekinesis( from );
			Name = "a treasure chest";
			StartDeleteTimer();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( CheckLocked( from ) )
				return;

			base.OnDoubleClick( from );
			Name = "a treasure chest";
			StartDeleteTimer();
		}

        protected void AddLoot(Item item)
        {
            if (item == null)
                return;

            if (Core.SA && RandomItemGenerator.Enabled)
            {
                int min, max;
                TreasureMapChest.GetRandomItemStat(out min, out max);

                RunicReforging.GenerateRandomItem(item, 0, min, max);
            }

            DropItem(item);
        }

		private void StartDeleteTimer()
		{
			if( m_DeleteTimer == null )
				m_DeleteTimer = new ChestTimer( this );
			else
				m_DeleteTimer.Delay = TimeSpan.FromSeconds( Utility.Random( 1, 2 ));
				
			m_DeleteTimer.Start();
		}

		private class ChestTimer : Timer
		{
			private BaseTreasureChestMod m_Chest;
			
			public ChestTimer( BaseTreasureChestMod chest ) : base ( TimeSpan.FromMinutes( Utility.Random( 2, 5 ) ) )
			{
				m_Chest = chest;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Chest.Delete();
			}
		}
	}
}