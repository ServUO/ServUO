using System;
using OrbServerSDK;
using Server.Engines.OrbRemoteServer;
using UOArchitectInterface;
using Server;
using Server.Mobiles;
using Server.Multis;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Engines.UOArchitect
{
	public class SelectItemsRequest : BaseOrbToolRequest  
	{
		private ArrayList _itemSerials = new ArrayList();
		private SelectItemsRequestArgs _args = null;

		public static void Initialize()
		{
			OrbServer.Register("UOAR_SelectItems", typeof(SelectItemsRequest), AccessLevel.GameMaster, true);
		}

		public override void OnRequest(OrbClientInfo clientInfo, OrbRequestArgs reqArgs)
		{
			FindOnlineMobile(clientInfo);

			if(reqArgs == null || !(reqArgs is SelectItemsRequestArgs) || !this.IsOnline)
				SendResponse(null);

			_args = (SelectItemsRequestArgs)reqArgs;

			if(_args.SelectType == SelectTypes.Area)
			{
				BoundingBoxPickerEx picker = new BoundingBoxPickerEx();
				picker.OnCancelled += new BoundingBoxExCancelled(OnTargetCancelled);
				picker.Begin( this.Mobile, new BoundingBoxCallback( BoundingBox_Callback ), null );
			}
			else
			{	
				UOAR_ObjectTarget target = new UOAR_ObjectTarget();
				target.OnCancelled += new UOAR_ObjectTarget.TargetCancelEvent(OnTargetCancelled);
				target.OnTargetObject += new UOAR_ObjectTarget.TargetObjectEvent(OnTargetObject);
				
				this.Mobile.SendMessage("Target the first item you want to select.");
				// send the target to the char
				this.Mobile.Target = target;
			}	
		}

		private void BoundingBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
		{
			Utility.FixPoints( ref start, ref end );
			Rectangle2D rect = new Rectangle2D(start, end);

			#region MobileSaver
			from.SendMessage( "Extracting Mobiles" );

			foreach ( Mobile m in map.GetMobilesInBounds( rect ) )
			{
				if ( m != null && m is BaseCreature )
				{
					int saveflag = MobileSaver.GetSaveFlag( m );

					if ( saveflag > 0 )
					{
						DesignItem designItem = new DesignItem();
						designItem.ItemID = (short)0x1;
						designItem.X = m.X;
						designItem.Y = m.Y;
						designItem.Z = m.Z + saveflag;
						designItem.Hue = (short)m.Hue;
					}
				}
			}
			#endregion

			for ( int x = 0; x <= rect.Width; ++x )
			{
				for ( int y = 0; y <= rect.Height; ++y )
				{
					int tileX = rect.Start.X + x;
					int tileY = rect.Start.Y + y;

					Sector sector = map.GetSector( tileX, tileY );

					for ( int i = 0; i < sector.Items.Count; ++i )
					{
						Item item = (Item)sector.Items[i];

						if(_args.UseMinZ && item.Z < _args.MinZ)
							continue;
						else if(_args.UseMaxZ && item.Z > _args.MaxZ)
							continue;

						if ( item.Visible && item.X == tileX && item.Y == tileY && !((item is BaseMulti) || (item is HouseSign)) )
						{
							_itemSerials.Add(item.Serial.Value);
						}
					}
				}
			}

			if(_itemSerials.Count > 0)
				SendResponse(new SelectItemsResponse((int[])_itemSerials.ToArray(typeof(int))));
			else
				SendResponse(null);
		}

		private void OnTargetCancelled()
		{
			if(_itemSerials.Count > 0)
			{
				SendResponse(new SelectItemsResponse((int[])_itemSerials.ToArray(typeof(int))));
			}
			else
			{
				SendResponse(null);
			}
		}

		private void OnTargetObject(object obj)
		{
			if( (obj is Item)  &&  !((obj is BaseMulti) || (obj is HouseSign)) )
			{
				int serial = (obj as Item).Serial.Value;

				if(_itemSerials.IndexOf(serial) == -1)
				{
					// add the item's serial # to the ArrayList
					_itemSerials.Add( ((Item)obj).Serial.Value );
				}
			}
			else
			{
				this.Mobile.SendMessage("That object is not valid for this selection.");
			}

			if(_args.Multiple)
			{
				UOAR_ObjectTarget target = new UOAR_ObjectTarget();
				target.OnCancelled += new UOAR_ObjectTarget.TargetCancelEvent(OnTargetCancelled);
				target.OnTargetObject += new UOAR_ObjectTarget.TargetObjectEvent(OnTargetObject);
			
				this.Mobile.SendMessage("Select another item to add it to your selection or press ESC to finish.");
				// send the target to the char
				this.Mobile.Target = target;
			}
			else
			{
				OnTargetCancelled();
			}
		}
	}
}
