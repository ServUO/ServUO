//
//  X-RunUO - Ultima Online Server Emulator
//  Copyright (C) 2015 Pedro Pardal
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 3.0 of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this program.
//

using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Engines.InstancedPeerless
{
	public enum InstanceState
	{
		Available,
		Reserved,
		Fighting,
		Looting
	}

	public class PeerlessInstance
	{
		private const int BusyHue = 1;
		private const int EmptyHue = 60;

		private PeerlessPlatform m_Owner;
		private InstanceRegion m_Region;
		private List<Mobile> m_Fighters;
		private Mobile m_Boss;
		private Item m_Light;
		private InstanceState m_State;

		private Map m_Map;
		private Point3D m_EntranceLocation;
		private Point3D m_BossSpawnLocation;
		private Rectangle2D m_RegionBounds;

		private Timer m_SliceTimer;
		private Timer m_KickTimer;

		public Mobile Boss { get { return m_Boss; } }

		public InstanceState State
		{
			get { return m_State; }
			set
			{
				m_State = value;

				if ( m_State == InstanceState.Available )
					m_Light.Hue = 0;
				else
					m_Light.Hue = 0x21;
			}
		}

		public Map Map { get { return m_Map; } }
		public Point3D EntranceLocation { get { return m_EntranceLocation; } }
		public Rectangle2D RegionBounds { get { return m_RegionBounds; } }

		public PeerlessInstance( PeerlessPlatform platform, Map map, Item light, Point3D entranceLoc, Point3D bossSpawnLoc, Rectangle2D regionBounds )
		{
			m_Owner = platform;
			m_Map = map;
			m_Light = light;

			m_EntranceLocation = entranceLoc;
			m_BossSpawnLocation = bossSpawnLoc;
			m_RegionBounds = regionBounds;

			Initialize();
		}

		protected virtual void Initialize()
		{
			State = InstanceState.Available;

			m_Fighters = new List<Mobile>();
			m_Region = new InstanceRegion( this );
		}

		public void AddFighter( Mobile m )
		{
			m_Fighters.Add( m );
		}

		public void RemoveFighter( Mobile m )
		{
			m_Fighters.Remove( m );
		}

		public void Activate()
		{
			State = InstanceState.Reserved;

			Timer.DelayCall( TimeSpan.FromMinutes( 1.0 ), new TimerCallback( StartFight ) );
		}

		private void StartFight()
		{
			State = InstanceState.Fighting;

			m_Boss = Activator.CreateInstance( m_Owner.BossType ) as Mobile;
			m_Boss.OnBeforeSpawn( m_BossSpawnLocation, m_Owner.Map );
			m_Boss.MoveToWorld( m_BossSpawnLocation, m_Owner.Map );

			m_SliceTimer = new SliceTimer( this );
			m_SliceTimer.Start();

			m_KickTimer = Timer.DelayCall( TimeSpan.FromHours( 2.0 ), new TimerCallback( Kick ) );
		}

		public void OnSlice()
		{
			if ( m_Fighters.Count == 0 )
			{
				FreeInstance();
				return;
			}

			if ( m_State == InstanceState.Fighting && m_Boss.Deleted )
			{
				if ( m_KickTimer != null )
					m_KickTimer.Stop();

				m_KickTimer = Timer.DelayCall( TimeSpan.FromMinutes( 15.0 ), new TimerCallback( Kick ) );

				State = InstanceState.Looting;
			}
		}

		private void Kick()
		{
			List<Mobile> fighters = new List<Mobile>( m_Fighters );

			foreach ( Mobile m in fighters )
				Kick( m );

			FreeInstance();
		}

		public void Kick( Mobile m )
		{
            Map map = m_Owner.Map;

            if (map == null || map == Map.Internal)
            {
                // Error, lets move to luna bank just incase
                if (m.Murderer)
                {
                    BaseCreature.TeleportPets(m, new Point3D(1459, 1629, 0), Map.Felucca);
                    m.MoveToWorld(new Point3D(1459, 1629, 0), Map.Felucca);
                }
                else
                {
                    BaseCreature.TeleportPets(m, new Point3D(989, 520, -50), Map.Malas);
                    m.MoveToWorld(new Point3D(989, 520, -50), Map.Malas);
                }
            }
            else
            {
                BaseCreature.TeleportPets(m, m_Owner.ExitLocation, map);
                m.MoveToWorld(m_Owner.ExitLocation, map);
            }

			RemoveFighter( m );
		}

		private void FreeInstance()
		{
			if ( m_Boss != null )
			{
				m_Boss.Delete();
				m_Boss = null;
			}

			if ( m_SliceTimer != null )
				m_SliceTimer.Stop();

			if ( m_KickTimer != null )
				m_KickTimer.Stop();

			State = InstanceState.Available;

			m_Owner.OnFreeInstance( this );
		}

		public void OnDelete()
		{
			if ( m_SliceTimer != null )
				m_SliceTimer.Stop();

			if ( m_KickTimer != null )
				m_KickTimer.Stop();

			if ( m_Boss != null )
				m_Boss.Delete();

			foreach ( Mobile m in m_Fighters )
				Kick( m );

			m_Region.Unregister();
		}

		#region Serialization
		public PeerlessInstance( GenericReader reader )
		{
			m_Owner = reader.ReadItem<PeerlessPlatform>();
			m_Light = reader.ReadItem();
			m_Map = reader.ReadMap();
			m_EntranceLocation = reader.ReadPoint3D();
			m_BossSpawnLocation = reader.ReadPoint3D();
			m_RegionBounds = reader.ReadRect2D();

			Mobile boss = reader.ReadMobile();

			if ( boss != null )
				boss.Delete();

			Initialize();
		}

		public void Serialize( GenericWriter writer )
		{
			writer.Write( m_Owner );
			writer.Write( m_Light );
			writer.Write( m_Map );
			writer.Write( m_EntranceLocation );
			writer.Write( m_BossSpawnLocation );
			writer.Write( m_RegionBounds );
			writer.Write( m_Boss );
		}
		#endregion
	}

	public class SliceTimer : Timer
	{
		private PeerlessInstance m_Instance;

		public SliceTimer( PeerlessInstance instance )
			: base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
		{
			m_Instance = instance;
		}

		protected override void OnTick()
		{
			m_Instance.OnSlice();
		}
	}
}