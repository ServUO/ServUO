using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items.Crops
{
	public class UpRootGump : Gump
	{
		private Mobile m_Owner;
		private BaseCrop m_crop;

		public UpRootGump( Mobile owner, BaseCrop crop ) : base( 60, 60 )
		{
			owner.CloseGump( typeof( UpRootGump ) );

			m_Owner = owner;
			m_crop = crop;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			AddImage( 0, 0, 0x816 );
			AddButton( 34, 74, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0 ); // OK
			AddButton( 88, 74, 0x995, 0x996, 2, GumpButtonType.Reply, 0 ); // Cancel

			string msg = "Do you wish to destroy this crop?";
			AddHtml( 30, 25, 120, 40, msg, false, false );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			if ( info.ButtonID == 1 )
			{
				if ( m_crop != null && !m_crop.Deleted )
				{
					if ( m_crop is WheatCrop )
						((WheatCrop)m_crop).UpRoot( m_Owner );
					else if ( m_crop is CottonCrop )
						((CottonCrop)m_crop).UpRoot( m_Owner );
					else if ( m_crop is CarrotCrop )
						((CarrotCrop)m_crop).UpRoot( m_Owner );
					else if ( m_crop is FlaxCrop )
						((FlaxCrop)m_crop).UpRoot( m_Owner );
					else if ( m_crop is LettuceCrop )
						((LettuceCrop)m_crop).UpRoot( m_Owner );
					else if ( m_crop is OnionCrop )
						((OnionCrop)m_crop).UpRoot( m_Owner );
					else if ( m_crop is CabbageCrop )
						((CabbageCrop)m_crop).UpRoot( m_Owner );
                    else if (m_crop is CornCrop)
                        ((CornCrop)m_crop).UpRoot(m_Owner);
                    else if (m_crop is GarlicCrop)
                        ((GarlicCrop)m_crop).UpRoot(m_Owner);
                    else if (m_crop is MandrakeCrop)
                        ((MandrakeCrop)m_crop).UpRoot(m_Owner);
                    else if (m_crop is NightShadeCrop)
                        ((NightShadeCrop)m_crop).UpRoot(m_Owner);
                    else if (m_crop is BloodMossCrop)
                        ((BloodMossCrop)m_crop).UpRoot(m_Owner);
                    else if (m_crop is GinsengCrop)
                        ((GinsengCrop)m_crop).UpRoot(m_Owner);
				}
			}
		}
	}
}
