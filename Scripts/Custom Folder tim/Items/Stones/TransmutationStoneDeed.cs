using System;
using Server.Network;
using Server.Prompts;
using Server.Guilds;
using Server.Multis;
using Server.Regions;
using Server.Targeting;
using Server.Targets;
using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class TransmutationStoneDeed : Item, IRewardItem
   {
      // public override int LabelNumber{ get{ return 1041055; } } // a guild deed
       private bool m_IsRewardItem;
       [CommandProperty(AccessLevel.GameMaster)]
       public bool IsRewardItem
       {
           get { return m_IsRewardItem; }
           set { m_IsRewardItem = value; InvalidateProperties(); }
       }
      [Constructable]
      public TransmutationStoneDeed() : base( 0x14F0 )
      {
         Name = "a transmutation stone deed";
         Weight = 1.0;
      }

      public TransmutationStoneDeed( Serial serial ) : base( serial )
      {
      }

      public override void Serialize( GenericWriter writer )
      {
         base.Serialize( writer );

         writer.Write( (int) 0 ); // version
         writer.Write((bool)m_IsRewardItem);
      }

      public override void Deserialize( GenericReader reader )
      {
         base.Deserialize( reader );

         int version = reader.ReadInt();
         m_IsRewardItem = reader.ReadBool();
         if ( Weight == 0.0 )
            Weight = 1.0;
      }

      public override void OnDoubleClick( Mobile from )
		{
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                from.SendMessage("This does not belong to you!!");
                return;
            }
			if ( IsChildOf( from.Backpack ) )
				from.Target = new InternalTarget( this );
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		private class InternalTarget : Target
		{
			private TransmutationStoneDeed m_Deed;

			public InternalTarget( TransmutationStoneDeed deed ) : base( -1, true, TargetFlags.None )
			{
				m_Deed = deed;

				CheckLOS = false;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				IPoint3D p = targeted as IPoint3D;
				Map map = from.Map;

				if ( p == null || map == null || m_Deed.Deleted )
					return;

				TransmutationStone stone = new TransmutationStone();
				BaseHouse house = BaseHouse.FindHouseAt( from.Location, from.Map, 20 );

				if ( m_Deed.IsChildOf( from.Backpack ) )
				{
					Server.Spells.SpellHelper.GetSurfaceTop( ref p );

					if ( house != null && house.IsInside( from ) && house.IsOwner( from ) )
					{
						stone.MoveToWorld( new Point3D( p ), map );
                                                //stone.Owner = from;
						m_Deed.Delete();
					}
					else if ( house != null && house.IsInside( from ) && !house.IsOwner( from ) )
					{
						from.SendLocalizedMessage( 500274 ); // You can only place this in a house that you own!
						stone.Delete();
					}
					else if ( house == null )
					{
						from.SendLocalizedMessage( 500269 ); // You cannot build that there.
						stone.Delete();
					}
				}
				else
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
			}
   }
   }
}
