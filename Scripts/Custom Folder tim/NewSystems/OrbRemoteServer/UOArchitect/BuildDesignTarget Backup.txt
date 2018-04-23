using System;
using Server.Targeting;
using Server.Items;
using UOArchitectInterface;
using System.Collections;

namespace Server.Engines.UOArchitect
{
	public class BuildDesignTarget : Target
	{
		private BuildDesignRequest m_Request;

		public BuildDesignTarget(BuildDesignRequest request) : base( -1, true, TargetFlags.None )
		{
			m_Request = request;
		}

		protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
		{
			base.OnTargetCancel(from, cancelType);
			m_Request.SendResponse(null);
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			if(m_Request.Items.Count == 0)
				m_Request.SendResponse(null);

			// Used to store the serials of the created items
			ArrayList itemSerials = new ArrayList(m_Request.Items.Count);

			IPoint3D p = targeted as IPoint3D;

			if ( p != null )
			{
				if ( p is Item )
					p = ((Item)p).GetWorldLocation();

				for(int i=0; i < m_Request.Items.Count; ++i)
				{
					DesignItem designItem = m_Request.Items[i];

					try
					{
						Point3D location = new Point3D(p.X + designItem.X, p.Y + designItem.Y, p.Z + designItem.Z);
						
						Item item = CreateItem(designItem.ItemID); 
						item.MoveToWorld(location, from.Map);
						item.Hue = designItem.Hue;
						itemSerials.Add(item.Serial.Value);
					}
					catch(Exception e)
					{
						Console.WriteLine("Unable to import item: " + e.Message);
					}
				}

			}

			int[] serials = (int[])itemSerials.ToArray(typeof(int));

			// send the serial list back to the client
			m_Request.SendResponse(new BuildResponse(serials));
		}

		// Create the appropriate item class - Defaults to a Static Item
		private Item CreateItem(int ItemID)
		{
			Item item = null;

			switch(ItemID)
			{
					// Don't import nodraw, node crystals, odd items, etc. 
					// Add any unwanted items here. 
					// TODO: Boat parts: planks, tillerman, etc. 
				case 0x0001: 
				case 0x1F19: 
				case 0x0FB7: 
					break; 
					/* 
					* Put all usable items here, for example, forges, anvils, 
					* training dummies, lights, containers, messageboards, etc. 
					* We'll need to wait until some of them are scripted. 
					* 
					* The following are some examples, I intend to finish things 
					* up as quickly as I can. 
					*/ 
				case 0x0FB1:  //forge 
					item = new SmallForgeAddon();
					break; 
				case 0x0FAF:  //anvil east 
					item = new AnvilEastAddon(); 
					break; 
				case 0x0FB0:  //anvil south 
					item = new AnvilSouthAddon(); 
					break; 

					// TODO: Gates 
                   
					/* 
					* Now create all of the doors according to graphic. 
					* NOTE: Some doors seem to open the wrong way, but 
					*       there's no way to determine correct CCW/CW 
					*       from the POL file. 
					*/ 

					// Metal Doors 2 
				case 0x0675: 
					item = new MetalDoor2( DoorFacing.WestCW ); 
					break; 
				case 0x0677: 
					item = new MetalDoor2( DoorFacing.EastCCW ); 
					break; 
				case 0x067D: 
					item = new MetalDoor2( DoorFacing.SouthCW ); 
					break; 
				case 0x067F: 
					item = new MetalDoor2( DoorFacing.NorthCCW ); 
					break; 
    
					// Barred Metal Doors 
				case 0x0685: 
					item = new BarredMetalDoor( DoorFacing.WestCW ); 
					break; 
				case 0x0687: 
					item = new BarredMetalDoor( DoorFacing.EastCCW ); 
					break; 
				case 0x068D: 
					item = new BarredMetalDoor( DoorFacing.SouthCW ); 
					break; 
				case 0x068F: 
					item = new BarredMetalDoor( DoorFacing.NorthCCW ); 
					break; 
    
					// Rattan Doors 
				case 0x0695: 
					item = new RattanDoor( DoorFacing.WestCW ); 
					break; 
				case 0x0697: 
					item = new RattanDoor( DoorFacing.EastCCW ); 
					break; 
				case 0x069D: 
					item = new RattanDoor( DoorFacing.SouthCW ); 
					break; 
				case 0x069F: 
					item = new RattanDoor( DoorFacing.NorthCCW ); 
					break; 
                
					// Dark Wood Doors 
				case 0x06A5: 
					item = new DarkWoodDoor( DoorFacing.WestCW ); 
					break; 
				case 0x06A7: 
					item = new DarkWoodDoor( DoorFacing.EastCCW ); 
					break; 
				case 0x06AD: 
					item = new DarkWoodDoor( DoorFacing.SouthCW ); 
					break; 
				case 0x06AF: 
					item = new DarkWoodDoor( DoorFacing.NorthCCW ); 
					break; 
                   
					// Medium Wood Doors 
				case 0x06B5: 
					item = new MediumWoodDoor( DoorFacing.WestCW ); 
					break; 
				case 0x06B7: 
					item = new MediumWoodDoor( DoorFacing.EastCCW ); 
					break; 
				case 0x06BD: 
					item = new MediumWoodDoor( DoorFacing.SouthCW ); 
					break; 
				case 0x06BF: 
					item = new MediumWoodDoor( DoorFacing.NorthCCW ); 
					break; 
    
					// Metal Doors 
				case 0x06C5: 
					item = new MetalDoor( DoorFacing.WestCW ); 
					break; 
				case 0x06C7: 
					item = new MetalDoor( DoorFacing.EastCCW ); 
					break; 
				case 0x06CD: 
					item = new MetalDoor( DoorFacing.SouthCW ); 
					break; 
				case 0x06CF: 
					item = new MetalDoor( DoorFacing.NorthCCW ); 
					break; 
    
					// Light Wood Doors 
				case 0x06D5: 
					item = new LightWoodDoor( DoorFacing.WestCW ); 
					break; 
				case 0x06D7: 
					item = new LightWoodDoor( DoorFacing.EastCCW ); 
					break; 
				case 0x06DD: 
					item = new LightWoodDoor( DoorFacing.SouthCW ); 
					break; 
				case 0x06DF: 
					item = new LightWoodDoor( DoorFacing.NorthCCW ); 
					break; 
    
					// Strong Wood Doors 
				case 0x06E5: 
					item = new StrongWoodDoor( DoorFacing.WestCW ); 
					break; 
				case 0x06E7: 
					item = new StrongWoodDoor( DoorFacing.EastCCW ); 
					break; 
				case 0x06ED: 
					item = new StrongWoodDoor( DoorFacing.SouthCW ); 
					break; 
				case 0x06EF: 
					item = new StrongWoodDoor( DoorFacing.NorthCCW ); 
					break; 
    
				default: 
					item = new Static(ItemID);
					item.Movable = false; 
					break; 

			}

			return item;
		}
	}
}
