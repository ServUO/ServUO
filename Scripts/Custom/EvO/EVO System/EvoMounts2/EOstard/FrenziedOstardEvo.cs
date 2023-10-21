using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	[CorpseName( "a frenzied ostard corpse" )]
	public class EvoFrenziedOstard : BaseEvoMount, IEvoCreature
	{
		public override BaseEvoSpec GetEvoSpec()
		{
			return FrenziedOstardEvoSpec.Instance;
		}

		public override BaseEvoEgg GetEvoEgg()
		{
			return new FrenziedOstardEvoEgg();
		}

		public override bool AddPointsOnDamage { get { return true; } }
		public override bool AddPointsOnMelee { get { return true; } }
		public override Type GetEvoDustType() { return typeof( FrenziedOstardEvoDust ); }

		//public override bool HasBreath{ get{ return false; } }

		public EvoFrenziedOstard( string name ) : base( name, 218, 0x3EA4 )
		{
		}

        public EvoFrenziedOstard(Serial serial) : base(serial)
		{
		}

		public override WeaponAbility GetWeaponAbility()	
		{
			return WeaponAbility.Dismount;
		} 

		public override bool SubdueBeforeTame{ get{ return false; } } // Must be beaten into submission
		
		public override int GetAngerSound()
		{
			return 0x271;
		}

		public override int GetIdleSound()
		{
			return 0x272;
		}

		public override int GetAttackSound()
		{
			return 0x273;
		}

		public override int GetHurtSound()
		{
			return 0x274;
		}

		public override int GetDeathSound()
		{
			return 0x275;
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write( (int)0 );			
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}