/*
.--.      .--.   ____    ,---.  ,---.   .-''-.     .-'''-.  
|  |_     |  | .'  __ `. |   /  |   | .'_ _   \   / _     \ 
| _( )_   |  |/   '  \  \|  |   |  .'/ ( ` )   ' (`' )/`--' 
|(_ o _)  |  ||___|  /  ||  | _ |  |. (_ o _)  |(_ o _).    
| (_,_) \ |  |   _.-`   ||  _( )_  ||  (_,_)___| (_,_). '.  
|  |/    \|  |.'   _    |\ (_ o._) /'  \   .---..---.  \  : 
|  '  /\  `  ||  _( )_  | \ (_,_) /  \  `-'    /\    `-'  | 
|    /  \    |\ (_ o _) /  \     /    \       /  \       /  
`---'    `---` '.(_,_).'    `---`      `'-..-'    `-...-'   
 
 * Written for Oblivion Shard
 * www.oblivion2.com
 * Author:		GM Waves
 * Date:		May 23, 2013
 * Version:		1.0
 * For:			Runuo v2.1
 */

using System;
using Server.Network;

namespace Server.Items
{
	class SwiftBoots : BaseArmor
	{
		public SwiftBoots( Serial serial )
			: base(serial)
		{

		}

		#region Private Variables

		private bool _enabled;
		private DateTime _lastUsed;
		private Timer _timer, _bodyCheckTimer;
		//TODO Add charges

		#endregion

		#region Configuration

		// Players can use it only once a day by default
		// Be sure to edit warning message: "You need to wait for a day until the magic recharges."
		private static readonly TimeSpan UsageInterval = new TimeSpan(1, 0, 0, 0);
		// Effect lasts for 1 minute by default
		private static readonly TimeSpan EffectDuration = new TimeSpan(0, 0, 1, 0);
		// Hue of the boots
		private const int BootsHue = 2763;

		#endregion


		#region Constructors

		[Constructable]
		public SwiftBoots()
			: base(5899)
		{
			_lastUsed = DateTime.Now.Subtract(UsageInterval);
			Name = "Boots of Swiftness";
			Hue = BootsHue;

		}

		#endregion


		#region Properties

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime LastUsed
		{
			get { return _lastUsed; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool CanBeUsed
		{
			get { return _lastUsed.Add(UsageInterval) <= DateTime.Now; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Enabled
		{
			get { return _enabled; }
		}

		#endregion


		#region Overrides

		public override ArmorMaterialType MaterialType
		{
			get { return ArmorMaterialType.Cloth; }
		}

		public override bool OnEquip( Mobile from )
		{
			// Boost speed
			// only if allowed
			if ( CanBeUsed )
			{
				BoostSpeed(from);
			}
			else
			{
				from.SendMessage(38, "You need to wait for a day until the magic recharges.");
			}

			return base.OnEquip(from);
		}

		public override void OnRemoved( object parent )
		{
			// normal
			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;
				if ( _timer != null && _timer.Running )
					from.SendMessage(38, "You slow down as you take off your magical boots.");
				NormalSpeed(from);
			}

			base.OnRemoved(parent);
		}

		#endregion


		#region Timers

		private class DurationTimer : Timer
		{
			private readonly Mobile _from;
			private readonly SwiftBoots _boots;

			public DurationTimer( Mobile from, SwiftBoots swiftBoots )
				: base(EffectDuration)
			{
				Priority = TimerPriority.EveryTick;
				_from = from;
				_boots = swiftBoots;
			}

			protected override void OnTick()
			{

				_boots.NormalSpeed(_from);
				// two minutes ends
				_from.SendMessage(38, "Your feet cannot withstand anymore swiftness...");
			}
		}

		private class BodyModCheck : Timer
		{
			private readonly Mobile _from;
			private readonly SwiftBoots _boots;

			public BodyModCheck( Mobile from, SwiftBoots swiftBoots )
				: base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
			{
				Priority = TimerPriority.OneSecond;
				_from = from;
				_boots = swiftBoots;
			}

			protected override void OnTick()
			{
				if ( _from.IsBodyMod )
				{
					//polymorphed
					_boots.NormalSpeed(_from);

					_from.SendMessage(38, "You lose your swiftness as you change your form.");


				}
				if ( _from.Map == Map.Internal )
				{
					// test this to see if it stops the timer on boot after player logs
					//Console.WriteLine("Player logged out.");
					_boots.NormalSpeed(_from);
				}

			}
		}

		#endregion


		#region Methods

		private void BoostSpeed( Mobile from )
		{
			//can only use in human form
			if ( from.IsBodyMod )
			{

				from.SendMessage(38, "You cannot use this in your current form.");
				return;
			}
			//can only use if damaged below 25% ? Should I implement this

			from.SendMessage(6, "You feel faster than the wind.");

			//set timers and last used time
			_lastUsed = DateTime.Now;
			_timer = new DurationTimer(from, this);
			_bodyCheckTimer = new BodyModCheck(from, this);

			//boost speed
			from.Send(SpeedControl.MountSpeed);

			//start timers and set our private variable
			_timer.Start();
			_bodyCheckTimer.Start();
			_enabled = true;
		}

		private void NormalSpeed( Mobile from )
		{
			//disable speedboost
			from.Send(SpeedControl.Disable);
			//stop timers if running
			if ( _timer != null && _timer.Running )
				_timer.Stop();
			if ( _bodyCheckTimer != null && _bodyCheckTimer.Running )
				_bodyCheckTimer.Stop();

			_enabled = false;
		}

		#endregion


		#region Serailization

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write(0);

			writer.Write(_lastUsed);
			writer.Write(_enabled);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			switch ( version )
			{
				case 0:
					_lastUsed = reader.ReadDateTime();
					_enabled = reader.ReadBool();
					break;
			}
		}

		#endregion

	}
}
