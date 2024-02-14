#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public abstract class BaseEvoDeed : Item
	{
		public abstract IEvoCreature GetEvoCreature();

		public BaseEvoDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "an evolution creature contract";
			LootType = LootType.Blessed;
			Hue = 1160;
		}

		public BaseEvoDeed( Serial serial ) : base ( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
				Use( from );
		}

		public void Use( Mobile from )
		{
			IEvoCreature evo = GetEvoCreature();

			if ( null != evo )
			{
				BaseEvoSpec spec = BaseEvoEgg.GetEvoSpec( evo );
				BaseCreature creature = evo as BaseCreature;

				if ( null != spec && null != spec.Stages && !creature.Deleted )
				{
					if ( spec.Tamable && spec.MinTamingToHatch > from.Skills[SkillName.AnimalTaming].Value )
					{
						from.SendMessage( "A minimum animal taming skill of {0} is required to use this contract.", spec.Stages[0].MinTameSkill );
						creature.Delete();
					}
					else if ( from.FollowersMax - from.Followers < spec.Stages[0].ControlSlots )
					{
						from.SendMessage( "You have too many followers to use this contract." );
						creature.Delete();
					}
					else
					{
						creature.Controlled = true;
						creature.ControlMaster = from;
						creature.IsBonded = true;
						creature.MoveToWorld( from.Location, from.Map );
						creature.ControlOrder = OrderType.Follow;
						Delete();
						from.SendMessage( "You are now master of a mercenary apprentice." );
					}
				}
			}
		}

		// Reflection is used since the class type could be either
		// BaseEvo or BaseMountEvo.

		public static BaseEvoSpec GetEvoSpec( IEvoCreature evo )
		{
			return Xanthos.Utilities.Misc.InvokeParameterlessMethod( evo, "GetEvoSpec" ) as BaseEvoSpec;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}