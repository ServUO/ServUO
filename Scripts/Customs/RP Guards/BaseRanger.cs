using System; 
using System.Collections; 
using Server.Misc; 
using Server.Items; 
using Server.Mobiles;


namespace Server.Mobiles 
{ 
	public class BaseRanger : BaseCreature 
	{ 

		public BaseRanger(AIType ai, FightMode fm, int PR, int FR, double AS, double PS) : base( ai, fm, PR, FR, AS, PS )
		{
			SpeechHue = Utility.RandomDyedHue(); 
			Hue = Utility.RandomSkinHue();
			Container pack = new Backpack();

				pack.DropItem( new Bandage(10) );

				pack.Movable = false;

				AddItem( pack );
		}

		public override bool IsEnemy( Mobile m )
		{
            if (m is BaseRanger || m is BaseVendor || m is PlayerVendor || m is TownCrier )

				return false;
			if (m != null && (m.Criminal == true))
				return true;
			if (m is PlayerMobile && (m.Kills > 4))
				return true;
			if (m is PlayerMobile && (m.Criminal == false))
				return false;
			if (m is BaseCreature)
			{
				BaseCreature c = (BaseCreature)m;

				if( c.Controlled || c.FightMode == FightMode.Aggressor || c.FightMode == FightMode.None )

					return false;
			}	

			return true;
		}
		//public virtual void OnAggressiveAction( Mobile aggressor )
		//{
			//Mobile currentCombat = BaseCreature.Combatant;

			//if( currentCombat != null && !aggressor.Hidden && currentCombat != aggressor && BaseCreature.GetDistanceToSqrt( currentCombat ) > BaseCreature.GetDistanceToSqrt( aggressor ) )
				//BaseCreature.Combatant = aggressor;
		//}
		private void HealSelf()
        {
            if (BandageContext.GetContext(this) == null)
            {
                BandageContext.BeginHeal(this, this);
            }

            return;
        }

        public override void OnThink()
        {
            if (Utility.RandomDouble() < 0.6 && Hits < (HitsMax - 15) && !Hidden)
                HealSelf();
        }
		public BaseRanger( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
		} 
	} 
}   