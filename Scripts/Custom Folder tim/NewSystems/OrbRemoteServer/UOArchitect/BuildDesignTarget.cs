using System;
using Server.Targeting;
using Server.Items;
using Server.Mobiles;
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

			ArrayList itemSerials = new ArrayList(m_Request.Items.Count); // Used to store the serials of the created items

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

						#region MobileSaver
						if ( item.ItemID == 0x1 )
						{
							Spawner spawner = MobileSaver.LoadMobile( item );

							if ( spawner != null )
								itemSerials.Add( spawner.Serial.Value );

							item.Delete();
							continue;
						}
						#endregion

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

			m_Request.SendResponse(new BuildResponse(serials)); // send the serial list back to the client
		}

		private Item CreateItem(int ItemID) // Create the appropriate item class - Defaults to a Static Item
		{
			Item item = null;

			switch(ItemID)
			{
				#region MobileSaver
				case 0x0001: // Now used for Mobile Importing.
				{
					item = new Static( 0x1 );
					break;
				}
				#endregion
				case 0x1F19: // Add any unwanted items here.
				case 0x0FB7: // TODO: Boat parts: planks, tillerman, etc.
					break;



				case 0x0FB1:  //forge
					item = new SmallForgeAddon();
					break;
				case 0x0FAF:  //anvil east
					item = new AnvilEastAddon();
					break;
				case 0x0FB0:  //anvil south
					item = new AnvilSouthAddon();
					break;

				case 0x2DD8: //Elven Forge
					item = new ElvenForgeAddon();
					break;

				case 0x1922: //FlourMill East
					item = new FlourMillEastAddon();
					break;
					case 0x1920: case 0x1924: break; //Don't add those items since the addon has them.
				case 0x192E: //FlourMill South
					item = new FlourMillSouthAddon();
					break;
					case 0x192C: case 0x1930: break; //Don't add those items since the addon has them.

				case 0x1060: //Loom East
					item = new LoomEastAddon();
					break;
					case 0x105F: break; //Don't add those items since the addon has them.
				case 0x1061: //Loom South
					item = new LoomSouthAddon();
					break;
					case 0x1062: break; //Don't add those items since the addon has them.

				case 0x1019: //Spinningwheel East
					item = new SpinningwheelEastAddon();
					break;
				case 0x1015: //Spinningwheel South
					item = new SpinningwheelSouthAddon();
					break;


				// Housing Metal Doors
				case 0x679:
					item = new NorthWestDoor();
					break;
				case 0x67B:
					item = new NorthEastDoor();
					break;
				case 0x675:
					item = new SouthWestDoor();
					break;
				case 0x677:
					item = new SouthEastDoor();
					break;
				case 0x683:
					item = new WestNorthDoor();
					break;
				case 0x681:
					item = new WestSouthDoor();
					break;
				case 0x67F:
					item = new EastNorthDoor();
					break;
				case 0x67D:
					item = new EastSouthDoor();
					break;

				/*
				case 0x0675: // Metal Doors 2 NOTE: Some doors seem to open the wrong way, but there's no way to determine correct CCW/CW from the POL file.
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
				*/

				case 0x0685: // Barred Metal Doors
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

				case 0x0695: // Rattan Doors
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

				case 0x06A5: // Dark Wood Doors
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

				case 0x06B5: // Medium Wood Doors
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

				/*
				case 0x06C5: // Metal Doors
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
				*/

				case 0x06D5: // Light Wood Doors
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

				case 0x06E5: // Strong Wood Doors
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

				case 0x2A05: //South facing West half Paper door (SE)
					item = new SWPaperSEDoor();
					break;
				case 0x2A07: //South facing East half Paper door (SE)
					item = new SEPaperSEDoor();
					break;
				case 0x2A09: //East facing South half Paper door (SE)
					item = new ESPaperSEDoor();
					break;
				case 0x2A0B: //East facing North half Paper door (SE)
					item = new ENPaperSEDoor();
					break;

				case 0x2A0D: //South facing West half Cloth door (SE)
					item = new SWClothSEDoor();
					break;
				case 0x2A0F: //South facing East half Cloth door (SE)
					item = new SEClothSEDoor();
					break;
				case 0x2A11: //East facing South half Cloth door (SE)
					item = new ESClothSEDoor();
					break;
				case 0x2A13: //East facing North half Cloth door (SE)
					item = new ENClothSEDoor();
					break;

				case 0x2A16: //South facing West half Wooden door (SE)
					item = new SWWoodenSEDoor();
					break;
				case 0x2A17: //South facing East half Wooden door (SE)
					item = new SEWoodenSEDoor();
					break;
				case 0x2A19: //East facing South half Wooden door (SE)
					item = new ESWoodenSEDoor();
					break;
				case 0x2A1B: //East facing North half Wooden door (SE)
					item = new ENWoodenSEDoor();
					break;



				case 0xE77: //Barrel
					item = new Barrel();
					item.Movable = false;
					break;
				case 0xE7F: //Keg
					item = new Keg();
					item.Movable = false;
					break;
				case 0xE7A: //PicnicBasket
					item = new PicnicBasket();
					item.Movable = false;
					break;
				case 0x990: //Basket
					item = new Basket();
					item.Movable = false;
					break;
				case 0x9AA: //WoodenBox (0xE7D)
					item = new WoodenBox();
					item.Movable = false;
					break;
				case 0xE7D: //WoodenBox (0xE7D)
					item = new WoodenBox();
					item.Movable = false;
					item.ItemID = 0xE7D;
					break;
				case 0x9A9: //SmallCrate (0xE7E)
					item = new SmallCrate();
					item.Movable = false;
					break;
				case 0xE7E: //SmallCrate (0xE7E)
					item = new SmallCrate();
					item.Movable = false;
					item.ItemID = 0xE7E;
					break;
				case 0xE3F: //MediumCrate (0xE3E)
					item = new MediumCrate();
					item.Movable = false;
					break;
				case 0xE3E: //MediumCrate (0xE3E)
					item = new MediumCrate();
					item.Movable = false;
					item.ItemID = 0xE3E;
					break;
				case 0xE3D: //LargeCrate (0xE3C)
					item = new LargeCrate();
					item.Movable = false;
					break;
				case 0xE3C: //LargeCrate (0xE3C)
					item = new LargeCrate();
					item.Movable = false;
					item.ItemID = 0xE3C;
					break;
				case 0x9A8: //MetalBox (0xE80)
					item = new MetalBox();
					item.Movable = false;
					break;
				case 0xE80: //MetalBox (0xE80)
					item = new MetalBox();
					item.Movable = false;
					item.ItemID = 0xE80;
					break;
				case 0x9AB: //MetalChest (0xE7C)
					item = new MetalChest();
					item.Movable = false;
					break;
				case 0xE7C: //MetalChest (0xE7C)
					item = new MetalChest();
					item.Movable = false;
					item.ItemID = 0xE7C;
					break;
				case 0xE41: //MetalGoldenChest (0xE40)
					item = new MetalGoldenChest();
					item.Movable = false;
					break;
				case 0xE40: //MetalGoldenChest (0xE40)
					item = new MetalGoldenChest();
					item.Movable = false;
					item.ItemID = 0xE40;
					break;
				case 0xe43: //WoodenChest (0xe42)
					item = new WoodenChest();
					item.Movable = false;
					break;
				case 0xe42: //WoodenChest (0xe42)
					item = new WoodenChest();
					item.Movable = false;
					item.ItemID = 0xe42;
					break;
				case 0x280B: //PlainWoodenChest (0x280C)
					item = new PlainWoodenChest();
					item.Movable = false;
					break;
				case 0x280C: //PlainWoodenChest (0x280C)
					item = new PlainWoodenChest();
					item.Movable = false;
					item.ItemID = 0x280C;
					break;
				case 0x280D: //OrnateWoodenChest (0x280E)
					item = new OrnateWoodenChest();
					item.Movable = false;
					break;
				case 0x280E: //OrnateWoodenChest (0x280E)
					item = new OrnateWoodenChest();
					item.Movable = false;
					item.ItemID = 0x280E;
					break;
				case 0x280F: //GildedWoodenChest (0x2810)
					item = new GildedWoodenChest();
					item.Movable = false;
					break;
				case 0x2810: //GildedWoodenChest (0x2810)
					item = new GildedWoodenChest();
					item.Movable = false;
					item.ItemID = 0x2810;
					break;
				case 0x2811: //WoodenFootLocker (0x2812)
					item = new WoodenFootLocker();
					item.Movable = false;
					break;
				case 0x2812: //WoodenFootLocker (0x2812)
					item = new WoodenFootLocker();
					item.Movable = false;
					item.ItemID = 0x2812;
					break;
				case 0x2813: //FinishedWoodenChest (0x2814)
					item = new FinishedWoodenChest();
					item.Movable = false;
					break;
				case 0x2814: //FinishedWoodenChest (0x2814)
					item = new FinishedWoodenChest();
					item.Movable = false;
					item.ItemID = 0x2814;
					break;

				case 0x2815: //TallCabinet (0x2816)
					item = new TallCabinet();
					item.Movable = false;
					break;
				case 0x2816: //TallCabinet (0x2816)
					item = new TallCabinet();
					item.Movable = false;
					item.ItemID = 0x2816;
					break;
				case 0x2817: //ShortCabinet (0x2818)
					item = new ShortCabinet();
					item.Movable = false;
					break;
				case 0x2818: //ShortCabinet (0x2818)
					item = new ShortCabinet();
					item.Movable = false;
					item.ItemID = 0x2818;
					break;
				case 0x2857: //RedArmoire (0x2858)
					item = new RedArmoire();
					item.Movable = false;
					break;
				case 0x2858: //RedArmoire (0x2858)
					item = new RedArmoire();
					item.Movable = false;
					item.ItemID = 0x2858;
					break;
				case 0x285D: //CherryArmoire (0x285E)
					item = new CherryArmoire();
					item.Movable = false;
					break;
				case 0x285E: //CherryArmoire (0x285E)
					item = new CherryArmoire();
					item.Movable = false;
					item.ItemID = 0x285E;
					break;
				case 0x285B: //MapleArmoire (0x285C)
					item = new MapleArmoire();
					item.Movable = false;
					break;
				case 0x285C: //MapleArmoire (0x285C)
					item = new MapleArmoire();
					item.Movable = false;
					item.ItemID = 0x285C;
					break;
				case 0x2859: //ElegantArmoire (0x285A)
					item = new ElegantArmoire();
					item.Movable = false;
					break;
				case 0x285A: //ElegantArmoire (0x285A)
					item = new ElegantArmoire();
					item.Movable = false;
					item.ItemID = 0x285A;
					break;
				case 0xA97: //FullBookcase (0xa97, 0xa99, 0xa98, 0xa9a, 0xa9b, 0xa9c)
					item = new FullBookcase();
					item.Movable = false;
					break;
				case 0xA99: //FullBookcase (0xa97, 0xa99, 0xa98, 0xa9a, 0xa9b, 0xa9c)
					item = new FullBookcase();
					item.Movable = false;
					item.ItemID = 0xa99;
					break;
				case 0xA98: //FullBookcase (0xa97, 0xa99, 0xa98, 0xa9a, 0xa9b, 0xa9c)
					item = new FullBookcase();
					item.Movable = false;
					item.ItemID = 0xa98;
					break;
				case 0xA9a: //FullBookcase (0xa97, 0xa99, 0xa98, 0xa9a, 0xa9b, 0xa9c)
					item = new FullBookcase();
					item.Movable = false;
					item.ItemID = 0xa9a;
					break;
				case 0xA9b: //FullBookcase (0xa97, 0xa99, 0xa98, 0xa9a, 0xa9b, 0xa9c)
					item = new FullBookcase();
					item.Movable = false;
					item.ItemID = 0xa9b;
					break;
				case 0xA9c: //FullBookcase (0xa97, 0xa99, 0xa98, 0xa9a, 0xa9b, 0xa9c)
					item = new FullBookcase();
					item.Movable = false;
					item.ItemID = 0xa9c;
					break;
				case 0xA9D: //EmptyBookcase (0xa9e)
					item = new EmptyBookcase();
					item.Movable = false;
					break;
				case 0xa9e: //EmptyBookcase (0xa9e)
					item = new EmptyBookcase ();
					item.Movable = false;
					item.ItemID = 0xa9e;
					break;
				case 0xA2C: //Drawer (0xa34)
					item = new Drawer();
					item.Movable = false;
					break;
				case 0xa34: //Drawer (0xa34)
					item = new Drawer();
					item.Movable = false;
					item.ItemID = 0xa34;
					break;
				case 0xA30: //FancyDrawer (0xa38)
					item = new FancyDrawer();
					item.Movable = false;
					break;
				case 0xa38: //FancyDrawer (0xa38)
					item = new FancyDrawer();
					item.Movable = false;
					item.ItemID = 0xa38;
					break;
				case 0xA4F: //Armoire (0xa53)
					item = new Armoire();
					item.Movable = false;
					break;
				case 0xa53: //Armoire (0xa53)
					item = new Armoire();
					item.Movable = false;
					item.ItemID = 0xa53;
					break;
				case 0xA4D: //FancyArmoire (0xa51)
					item = new FancyArmoire();
					item.Movable = false;
					break;
				case 0xa51: //FancyArmoire (0xa51)
					item = new FancyArmoire();
					item.Movable = false;
					item.ItemID = 0xa51;
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
/*
Added.

Misc
-ElvenForge
-FlourMill(East/South)
-Loom(East/South)Addon

Doors
-Paper sliding doors
-Cloth sliding doors
-Wooden sliding doors

Containers
-Barrel
-Keg
-PicnicBasket
-Basket
-WoodenBox
-SmallCrate
-MediumCrate
-LargeCrate
-MetalBox
-MetalChest
-MetalGoldenChest
-WoodenChest
-PlainWoodenChest
-OrnateWoodenChest
-OrnateWoodenChest
-GildedWoodenChest
-GildedWoodenChest
-WoodenFootLocker
-WoodenFootLocker
-FinishedWoodenChest
-FinishedWoodenChest
-TallCabinet
-ShortCabinet
-RedArmoire
-CherryArmoire
-MapleArmoire
-ElegantArmoire
-FullBookcase 
-EmptyBookcase
-Drawer
-FancyDrawer
-Armoire
-FancyArmoire

*/