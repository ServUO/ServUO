// Trees.cs 
//
// Based on script by Alari (alarihyena@gmail.com)
//  
//  Version: 1.0a
//  Modified by Thorlack (at) moonpoint.com on 9/23/2005
//  Modifications:
//  1. Inserted missing "(" in line for Ohii tree
//  2. Added specific instances of trees where just random trees were present
//   
//  Edited by Amythest (at) moonpoint.com on 9/25/2005
//  1. Added more trees that were not included in the script created by Alari.
//  2. Edited the Random tree batches to include the red leaves and ones missing.
//  3. Added Single trees that were put in batches of randoms.
//  

using System;
using Server;
using Server.Items;


namespace Server.Items
{


// =============================================================================


	public class CedarTreeRand : BaseAddon
	{
		[Constructable]
		public CedarTreeRand()
		{
			if ( Utility.RandomBool() )
			{
				// trunk
				AddComponent( new AddonComponent ( 3286 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3287 ), 0, 0, 0 );
			}
			else
			{
				// trunk
				AddComponent( new AddonComponent ( 3288 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3289 ), 0, 0, 0 );
			}
		}

		public CedarTreeRand( Serial serial ) : base( serial )
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


// =============================================================================


	public class CedarTree1 : BaseAddon
	{
		[Constructable]
		public CedarTree1()
		{
			// trunk
			AddComponent( new AddonComponent ( 3286 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3287 ), 0, 0, 0 );
		}
		
		public CedarTree1( Serial serial ) : base( serial )
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


// =============================================================================


	public class CedarTree2 : BaseAddon
	{
		[Constructable]
		public CedarTree2()
		{
			// trunk
			AddComponent( new AddonComponent ( 3288 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3289 ), 0, 0, 0 );
		}

		public CedarTree2( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTreeRand : BaseAddon
	{
		[Constructable]
		public CypressTreeRand()
		{
			switch ( Utility.Random( 8 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3320 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3321 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3320 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3322 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3323 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3324 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3323 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3325 ), 0, 0, 0 );
				break;

				case 4:
				// trunk
				AddComponent( new AddonComponent ( 3326 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3327 ), 0, 0, 0 );
				break;

				case 5:
				// trunk
				AddComponent( new AddonComponent ( 3326 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3328 ), 0, 0, 0 );
				break;

				case 6:
				// trunk
				AddComponent( new AddonComponent ( 3329 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3330 ), 0, 0, 0 );
				break;

				case 7:
				// trunk
				AddComponent( new AddonComponent ( 3329 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3331 ), 0, 0, 0 );
				break;

			}

		}

		public CypressTreeRand( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTreeNoLeavesRand : BaseAddon
	{
		[Constructable]
		public CypressTreeNoLeavesRand()
		{
			switch ( Utility.Random( 8 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3320 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3320 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3323 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3323 ), 0, 0, 0 );
				break;

				case 4:
				// trunk
				AddComponent( new AddonComponent ( 3326 ), 0, 0, 0 );
				break;

				case 5:
				// trunk
				AddComponent( new AddonComponent ( 3326 ), 0, 0, 0 );
				break;

				case 6:
				// trunk
				AddComponent( new AddonComponent ( 3329 ), 0, 0, 0 );
				break;

				case 7:
				// trunk
				AddComponent( new AddonComponent ( 3329 ), 0, 0, 0 );
				break;

			}
		}

		public CypressTreeNoLeavesRand( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTree1Green : BaseAddon
	{
		[Constructable]
		public CypressTree1Green()
		{
			// trunk
			AddComponent( new AddonComponent ( 3320 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3321 ), 0, 0, 0 );
			
		}

		public CypressTree1Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTree1Red : BaseAddon
	{
		[Constructable]
		public CypressTree1Red()
		{
			// trunk
			AddComponent( new AddonComponent ( 3320 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3322 ), 0, 0, 0 );
			
		}

		public CypressTree1Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTree2Green : BaseAddon
	{
		[Constructable]
		public CypressTree2Green()
		{
			// trunk
			AddComponent( new AddonComponent ( 3323 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3324 ), 0, 0, 0 );
			
		}

		public CypressTree2Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTree2Red : BaseAddon
	{
		[Constructable]
		public CypressTree2Red()
		{
			// trunk
			AddComponent( new AddonComponent ( 3323 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3325 ), 0, 0, 0 );
			
		}

		public CypressTree2Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTree3Green : BaseAddon
	{
		[Constructable]
		public CypressTree3Green()
		{
			// trunk
			AddComponent( new AddonComponent ( 3326 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3327 ), 0, 0, 0 );
			
		}

		public CypressTree3Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTree3Red : BaseAddon
	{
		[Constructable]
		public CypressTree3Red()
		{
			// trunk
			AddComponent( new AddonComponent ( 3326 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3328 ), 0, 0, 0 );
			
		}

		public CypressTree3Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class CypressTree4Green : BaseAddon
	{
		[Constructable]
		public CypressTree4Green()
		{
			// trunk
			AddComponent( new AddonComponent ( 3329 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3330 ), 0, 0, 0 );
			
		}

		public CypressTree4Green( Serial serial ) : base( serial )
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

		
// =============================================================================
		
		
	public class CypressTree4Red : BaseAddon
	{
		[Constructable]
		public CypressTree4Red()
		{
			// trunk
			AddComponent( new AddonComponent ( 3329 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3331 ), 0, 0, 0 );
			
		}

		public CypressTree4Red( Serial serial ) : base( serial )
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


// =================================================================================


	public class OakTreeRand : BaseAddon
	{
		[Constructable]
		public OakTreeRand()
		{
			switch ( Utility.Random( 4 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3290 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3291 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3290 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3292 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3293 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3294 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3293 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3295 ), 0, 0, 0 );
				break;
			}

		}

		public OakTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class OakTreeMediumGreen : BaseAddon
	{
		[Constructable]
		public OakTreeMediumGreen()
		{
			// trunk
			AddComponent( new AddonComponent ( 3290 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3291 ), 0, 0, 0 );
		}
		
		public OakTreeMediumGreen( Serial serial ) : base( serial )
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


// =================================================================================


	public class OakTreeMediumRed : BaseAddon
	{
		[Constructable]
		public OakTreeMediumRed()
		{
			// trunk
			AddComponent( new AddonComponent ( 3290 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3292 ), 0, 0, 0 );
		}
		
		public OakTreeMediumRed( Serial serial ) : base( serial )
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


// =================================================================================


	public class OakTreeLargeGreen : BaseAddon
	{
		[Constructable]
		public OakTreeLargeGreen()
		{
			// trunk
			AddComponent( new AddonComponent ( 3293 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3294 ), 0, 0, 0 );
		}
		
		public OakTreeLargeGreen( Serial serial ) : base( serial )
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


// =================================================================================


	public class OakTreeLargeRed : BaseAddon
	{
		[Constructable]
		public OakTreeLargeRed()
		{
			// trunk
			AddComponent( new AddonComponent ( 3293 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3295 ), 0, 0, 0 );
		}
		
		public OakTreeLargeRed( Serial serial ) : base( serial )
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


// =================================================================================


	public class OhiiTree : BaseAddon
	{
		[Constructable]
		public OhiiTree()
		{
			// tree
			AddComponent( new AddonComponent ( 3230 ), 0, 0, 0 );
		}

		public OhiiTree( Serial serial ) : base( serial )
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


// =================================================================================


	public class SaplingTreeRand : BaseAddon
	{
		[Constructable]
		public SaplingTreeRand()
		{
			// tree
			AddComponent( new AddonComponent ( Utility.RandomList( 3305, 3306 ) ), 0, 0, 0 );
		}

		public SaplingTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class SaplingTree1 : BaseAddon
	{
		[Constructable]
		public SaplingTree1()
		{
			// tree
			AddComponent( new AddonComponent ( 3305 ) , 0, 0, 0 );
		}

		public SaplingTree1( Serial serial ) : base( serial )
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


// =================================================================================


	public class SaplingTree2 : BaseAddon
	{
		[Constructable]
		public SaplingTree2()
		{
			// tree
			AddComponent( new AddonComponent ( 3306 ) , 0, 0, 0 );
		}

		public SaplingTree2( Serial serial ) : base( serial )
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


// =================================================================================


	public class SmallPalmTreeRand : BaseAddon
	{
		[Constructable]
		public SmallPalmTreeRand()
		{
			// tree
			AddComponent( new AddonComponent ( Utility.RandomMinMax( 3225, 3229 ) ), 0, 0, 0 );
		}

		public SmallPalmTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class SmallPalmTree1 : BaseAddon
	{
		[Constructable]
		public SmallPalmTree1()
		{
			// tree
			AddComponent( new AddonComponent ( 3225 ), 0, 0, 0 );
		}

		public SmallPalmTree1( Serial serial ) : base( serial )
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


// =================================================================================


	public class SmallPalmTree2 : BaseAddon
	{
		[Constructable]
		public SmallPalmTree2()
		{
			// tree
			AddComponent( new AddonComponent ( 3226 ), 0, 0, 0 );
		}

		public SmallPalmTree2( Serial serial ) : base( serial )
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


// =================================================================================


	public class SmallPalmTree3 : BaseAddon
	{
		[Constructable]
		public SmallPalmTree3()
		{
			// tree
			AddComponent( new AddonComponent ( 3227 ), 0, 0, 0 );
		}

		public SmallPalmTree3( Serial serial ) : base( serial )
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


// =================================================================================


	public class SmallPalmTree4 : BaseAddon
	{
		[Constructable]
		public SmallPalmTree4()
		{
			// tree
			AddComponent( new AddonComponent ( 3228 ), 0, 0, 0 );
		}

		public SmallPalmTree4( Serial serial ) : base( serial )
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


// =================================================================================


	public class SmallPalmTree5 : BaseAddon
	{
		[Constructable]
		public SmallPalmTree5()
		{
			// tree
			AddComponent( new AddonComponent ( 3227 ), 0, 0, 0 );
		}

		public SmallPalmTree5( Serial serial ) : base( serial )
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


// =================================================================================


	public class SpiderTree : BaseAddon
	{
		[Constructable]
		public SpiderTree()
		{
			// tree
			AddComponent( new AddonComponent ( 3273 ), 0, 0, 0 );
		}

		public SpiderTree( Serial serial ) : base( serial )
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


// =================================================================================


	public class TreeLeavesRand : BaseAddon
	{
		[Constructable]
		public TreeLeavesRand()
		{
			// leaves
			switch ( Utility.Random( 4 ) )
			{
				case 0: 
				AddComponent( new AddonComponent ( 6943 ), 0, 0, 0 );
				break;

				case 1: 
				AddComponent( new AddonComponent ( 6944 ), 0, 0, 0 );
				break;

				case 2: 
				AddComponent( new AddonComponent ( 6945 ), 0, 0, 0 );
				break;

				case 3: 
				AddComponent( new AddonComponent ( 6946 ), 0, 0, 0 );
				break;
			}
		}

		public TreeLeavesRand( Serial serial ) : base( serial )
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

// =================================================================================


	public class TreeLeavesPile : BaseAddon
	{
		[Constructable]
		public TreeLeavesPile()
		{
			
			// leaf pile
				AddComponent( new AddonComponent ( 6947 ), 1, 1, 0 );
				AddComponent( new AddonComponent ( 6948 ), 1, 0, 0 );
				AddComponent( new AddonComponent ( 6949 ), 0, 0, 0 );
				AddComponent( new AddonComponent ( 6950 ), 0, 1, 0 );		
		}

		public TreeLeavesPile( Serial serial ) : base( serial )
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


// =================================================================================

// =================================================================================


	public class TreeStumpRand : BaseAddon
	{
		[Constructable]
		public TreeStumpRand()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0:
				// stump
				AddComponent( new AddonComponent ( 3670 ), 0, 0, 0 );
				break;

				case 1:
				// stump
				AddComponent( new AddonComponent ( 3671 ), 0, 0, 0 );
				break;

				case 2:
				// stump
				AddComponent( new AddonComponent ( 3672 ), 0, 0, 0 );
				break;

				case 3:
				// stump
				AddComponent( new AddonComponent ( 3673 ), 0, 0, 0 );
				break;

				case 4:
				// stump
				AddComponent( new AddonComponent ( 3500 ), 0, 0, 0 );
				break;

				case 5:
				// stump
				AddComponent( new AddonComponent ( 3501 ), 0, 0, 0 );
				break;
			}

		}

		public TreeStumpRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class TreeStump1AxeLeft : BaseAddon
	{
		[Constructable]
		public TreeStump1AxeLeft()
		{
			// tree
			AddComponent( new AddonComponent ( 3670 ), 0, 0, 0 );
		}

		public TreeStump1AxeLeft( Serial serial ) : base( serial )
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


// =================================================================================


	public class TreeStump1Left : BaseAddon
	{
		[Constructable]
		public TreeStump1Left()
		{
			// tree
			AddComponent( new AddonComponent ( 3671 ), 0, 0, 0 );
		}

		public TreeStump1Left( Serial serial ) : base( serial )
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


// =================================================================================


	public class TreeStump1AxeRight : BaseAddon
	{
		[Constructable]
		public TreeStump1AxeRight()
		{
			// tree
			AddComponent( new AddonComponent ( 3672 ), 0, 0, 0 );
		}

		public TreeStump1AxeRight( Serial serial ) : base( serial )
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


// =================================================================================


	public class TreeStump1Right : BaseAddon
	{
		[Constructable]
		public TreeStump1Right()
		{
			// tree
			AddComponent( new AddonComponent ( 3673 ), 0, 0, 0 );
		}

		public TreeStump1Right( Serial serial ) : base( serial )
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


// =================================================================================


	public class TreeStump2 : BaseAddon
	{
		[Constructable]
		public TreeStump2()
		{
			// tree
			AddComponent( new AddonComponent ( 3500 ), 0, 0, 0 );
		}

		public TreeStump2( Serial serial ) : base( serial )
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


// =================================================================================


	public class TreeStump3 : BaseAddon
	{
		[Constructable]
		public TreeStump3()
		{
			// tree
			AddComponent( new AddonComponent ( 3501 ), 0, 0, 0 );
		}

		public TreeStump3( Serial serial ) : base( serial )
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


// =================================================================================


	public class TuscanyPineTree : BaseAddon
	{
		[Constructable]
		public TuscanyPineTree()
		{
			// tree
			AddComponent( new AddonComponent ( 7038 ), 0, 0, 0 );
		}

		public TuscanyPineTree( Serial serial ) : base( serial )
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


// =================================================================================


	public class WalnutTreeRand : BaseAddon
	{
		[Constructable]
		public WalnutTreeRand()
		{
			switch ( Utility.Random( 4 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3296 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3297 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3296 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3298 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3299 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3300 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3299 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3301 ), 0, 0, 0 );
				break;
			}

		}

		public WalnutTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class WalnutTree1Green : BaseAddon
	{
		[Constructable]
		public WalnutTree1Green()
		{
			// trunk
			AddComponent( new AddonComponent ( 3296 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3297 ), 0, 0, 0 );
		}

		public WalnutTree1Green( Serial serial ) : base( serial )
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


// =================================================================================


	public class WalnutTree1Red : BaseAddon
	{
		[Constructable]
		public WalnutTree1Red()
		{
			// trunk
			AddComponent( new AddonComponent ( 3296 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3298 ), 0, 0, 0 );
		}

		public WalnutTree1Red( Serial serial ) : base( serial )
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


// =================================================================================


	public class WalnutTree2Green : BaseAddon
	{
		[Constructable]
		public WalnutTree2Green()
		{
			// trunk
			AddComponent( new AddonComponent ( 3299 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3300 ), 0, 0, 0 );
		}

		public WalnutTree2Green( Serial serial ) : base( serial )
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


// =================================================================================


	public class WalnutTree2Red : BaseAddon
	{
		[Constructable]
		public WalnutTree2Red()
		{
			// trunk
			AddComponent( new AddonComponent ( 3299 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3301 ), 0, 0, 0 );
		}

		public WalnutTree2Red( Serial serial ) : base( serial )
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


// =================================================================================


	public class WillowTreeRand : BaseAddon
	{
		[Constructable]
		public WillowTreeRand()
		{
			if ( Utility.RandomBool() )
			{
				// trunk
				AddComponent( new AddonComponent ( 3302 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3303 ), 0, 0, 0 );
			}
			else
			{
				// trunk
				AddComponent( new AddonComponent ( 3302 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3304 ), 0, 0, 0 );
			}
		}

		public WillowTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class WillowTreeGreen : BaseAddon
	{
		[Constructable]
		public WillowTreeGreen()
		{
			// trunk
			AddComponent( new AddonComponent ( 3302 ), 0, 0, 0 );

			// leaves
			AddComponent( new AddonComponent ( 3303 ), 0, 0, 0 );

		}

		public WillowTreeGreen( Serial serial ) : base( serial )
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


// =================================================================================


	public class WillowTreeRed : BaseAddon
	{
		[Constructable]
		public WillowTreeRed()
		{
			// trunk
			AddComponent( new AddonComponent ( 3302 ), 0, 0, 0 );

			// leaves
			AddComponent( new AddonComponent ( 3304 ), 0, 0, 0 );

		}

		public WillowTreeRed( Serial serial ) : base( serial )
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


// =================================================================================


	public class YuccTreeRand : BaseAddon
	{
		[Constructable]
		public YuccTreeRand()
		{
			// tree
			AddComponent( new AddonComponent ( Utility.RandomList( 3383, 3384 ) ), 0, 0, 0 );
		}

		public YuccTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class YuccaTree1 : BaseAddon
	{
		[Constructable]
		public YuccaTree1()
		{
			// tree
			AddComponent( new AddonComponent ( 3383 ), 0, 0, 0 );
		}

		public YuccaTree1( Serial serial ) : base( serial )
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


// =================================================================================


	public class YuccaTree2 : BaseAddon
	{
		[Constructable]
		public YuccaTree2()
		{
			// tree
			AddComponent( new AddonComponent ( 3384 ), 0, 0, 0 );
		}

		public YuccaTree2( Serial serial ) : base( serial )
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


// =================================================================================


/////////////////////////////////////////////////
//
// Automatically generated by the
// AddonGenerator script by Arya
//
/////////////////////////////////////////////////


	public class YewTree : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new YewTreeDeed();
			}
		}

		[ Constructable ]
		public YewTree()
		{
			AddonComponent ac = null;
			ac = new AddonComponent( 4807 );
			AddComponent( ac, 5, -4, 7 );
			ac = new AddonComponent( 4797 );
			AddComponent( ac, 4, -4, 0 );
			ac = new AddonComponent( 4806 );
			AddComponent( ac, 4, -3, 7 );
			ac = new AddonComponent( 4805 );
			AddComponent( ac, 3, -2, 7 );
			ac = new AddonComponent( 4804 );
			AddComponent( ac, 2, -1, 7 );
			ac = new AddonComponent( 4803 );
			AddComponent( ac, 1, -1, 7 );
			ac = new AddonComponent( 4802 );
			AddComponent( ac, 0, 0, 7 );
			ac = new AddonComponent( 4801 );
			AddComponent( ac, -1, 1, 7 );
			ac = new AddonComponent( 4800 );
			AddComponent( ac, -2, 2, 7 );
			ac = new AddonComponent( 4799 );
			AddComponent( ac, -3, 3, 7 );
			ac = new AddonComponent( 4798 );
			AddComponent( ac, -4, 4, 7 );
			ac = new AddonComponent( 4798 );
			AddComponent( ac, -1, 0, 0 );
			ac = new AddonComponent( 4796 );
			AddComponent( ac, 3, -3, 0 );
			ac = new AddonComponent( 4795 );
			AddComponent( ac, 2, -2, 0 );
			ac = new AddonComponent( 4794 );
			AddComponent( ac, 1, -1, 0 );
			ac = new AddonComponent( 4793 );
			AddComponent( ac, 0, 0, 0 );
			ac = new AddonComponent( 4792 );
			AddComponent( ac, -1, 1, 0 );
			ac = new AddonComponent( 4791 );
			AddComponent( ac, -2, 2, 0 );
			ac = new AddonComponent( 4789 );
			AddComponent( ac, -3, 3, 0 );

		}

		public YewTree( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class YewTreeDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new YewTree();
			}
		}

		[Constructable]
		public YewTreeDeed()
		{
			Name = "YewTree";
		}

		public YewTreeDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}


}


// =============================================================================


	public class GeneralTreeRand : BaseAddon
	{
		[Constructable]
		public GeneralTreeRand()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3277 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3278 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3277 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3279 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3280 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3281 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3280 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3282 ), 0, 0, 0 );
				break;

				case 4:
				// trunk
				AddComponent( new AddonComponent ( 3283 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3284 ), 0, 0, 0 );
				break;

				case 5:
				// trunk
				AddComponent( new AddonComponent ( 3283 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3285 ), 0, 0, 0 );
				break;
			}

		}

		public GeneralTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class GeneralCrookedTreeGreen : BaseAddon
	{
		[Constructable]
		public GeneralCrookedTreeGreen()
		{
			// trunk
			AddComponent( new AddonComponent ( 3277 ), 0, 0, 0 );

			// leaves
			AddComponent( new AddonComponent ( 3278 ), 0, 0, 0 );

		}

		public GeneralCrookedTreeGreen( Serial serial ) : base( serial )
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


// =================================================================================


	public class GeneralCrookedTreeRed : BaseAddon
	{
		[Constructable]
		public GeneralCrookedTreeRed()
		{
			// trunk
			AddComponent( new AddonComponent ( 3277 ), 0, 0, 0 );

			// leaves
			AddComponent( new AddonComponent ( 3279 ), 0, 0, 0 );

		}

		public GeneralCrookedTreeRed( Serial serial ) : base( serial )
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


// =================================================================================


	public class GeneralSmallTreeGreen : BaseAddon
	{
		[Constructable]
		public GeneralSmallTreeGreen()
		{
			// trunk
			AddComponent( new AddonComponent ( 3280 ), 0, 0, 0 );

			// leaves
			AddComponent( new AddonComponent ( 3281 ), 0, 0, 0 );

		}

		public GeneralSmallTreeGreen( Serial serial ) : base( serial )
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


// =================================================================================


	public class GeneralSmallTreeRed : BaseAddon
	{
		[Constructable]
		public GeneralSmallTreeRed()
		{
			// trunk
			AddComponent( new AddonComponent ( 3280 ), 0, 0, 0 );

			// leaves
			AddComponent( new AddonComponent ( 3282 ), 0, 0, 0 );

		}

		public GeneralSmallTreeRed( Serial serial ) : base( serial )
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


// =================================================================================


	public class GeneralMediumTreeGreen : BaseAddon
	{
		[Constructable]
		public GeneralMediumTreeGreen()
		{
			// trunk
			AddComponent( new AddonComponent ( 3283 ), 0, 0, 0 );

			// leaves
			AddComponent( new AddonComponent ( 3284 ), 0, 0, 0 );

		}

		public GeneralMediumTreeGreen( Serial serial ) : base( serial )
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


// =================================================================================


	public class GeneralMediumTreeRed : BaseAddon
	{
		[Constructable]
		public GeneralMediumTreeRed()
		{
			// trunk
			AddComponent( new AddonComponent ( 3283 ), 0, 0, 0 );

			// leaves
			AddComponent( new AddonComponent ( 3285 ), 0, 0, 0 );

		}

		public GeneralMediumTreeRed( Serial serial ) : base( serial )
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


// =================================================================================


	public class BananaTreeRand : BaseAddon
	{
		[Constructable]
		public BananaTreeRand()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0:
				// tree
				AddComponent( new AddonComponent ( 3243 ), 0, 0, 0 );
				break;

				case 1:
				// tree
				AddComponent( new AddonComponent ( 3240 ), 0, 0, 0 );
				break;

				case 2:
				// tree
				AddComponent( new AddonComponent ( 3242 ), 0, 0, 0 );
				break;
			}

		}

		public BananaTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class BananaTree1 : BaseAddon
	{
		[Constructable]
		public BananaTree1()
		{
			// tree
			AddComponent( new AddonComponent ( 3243 ), 0, 0, 0 );

		}

		public BananaTree1( Serial serial ) : base( serial )
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


// =================================================================================


	public class BananaTree2 : BaseAddon
	{
		[Constructable]
		public BananaTree2()
		{
			// tree
			AddComponent( new AddonComponent ( 3240 ), 0, 0, 0 );

		}

		public BananaTree2( Serial serial ) : base( serial )
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


// =================================================================================


	public class BananaTree3 : BaseAddon
	{
		[Constructable]
		public BananaTree3()
		{
			// tree
			AddComponent( new AddonComponent ( 3242 ), 0, 0, 0 );

		}

		public BananaTree3( Serial serial ) : base( serial )
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


// =================================================================================


	public class DeadTreeRand : BaseAddon
	{
		[Constructable]
		public DeadTreeRand()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0:
				// tree
				AddComponent( new AddonComponent ( 3274 ), 0, 0, 0 );
				break;

				case 1:
				// tree
				AddComponent( new AddonComponent ( 3275 ), 0, 0, 0 );
				break;

				case 2:
				// tree
				AddComponent( new AddonComponent ( 3276 ), 0, 0, 0 );
				break;
			}

		}

		public DeadTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class DeadTree1 : BaseAddon
	{
		[Constructable]
		public DeadTree1()
		{
			// tree
			AddComponent( new AddonComponent ( 3274 ), 0, 0, 0 );

		}

		public DeadTree1( Serial serial ) : base( serial )
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


// =================================================================================


	public class DeadTree2 : BaseAddon
	{
		[Constructable]
		public DeadTree2()
		{
			// tree
			AddComponent( new AddonComponent ( 3275 ), 0, 0, 0 );

		}

		public DeadTree2( Serial serial ) : base( serial )
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


// =================================================================================


	public class DeadTree3 : BaseAddon
	{
		[Constructable]
		public DeadTree3()
		{
			// tree
			AddComponent( new AddonComponent ( 3276 ), 0, 0, 0 );

		}

		public DeadTree3( Serial serial ) : base( serial )
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


// =================================================================================


	public class SpecialTreeNoLeavesRand : BaseAddon
	{
		[Constructable]
		public SpecialTreeNoLeavesRand()
		{
			switch ( Utility.Random( 4 ) )
			{
				case 0:
				// tree
				AddComponent( new AddonComponent ( 8778 ), 0, 0, 0 );
				break;

				case 1:
				// tree
				AddComponent( new AddonComponent ( 8779 ), 0, 0, 0 );
				break;

				case 2:
				// tree
				AddComponent( new AddonComponent ( 8780 ), 0, 0, 0 );
				break;
				
				case 3:
				// tree
				AddComponent( new AddonComponent ( 8781 ), 0, 0, 0 );
				break;
			}

		}

		public SpecialTreeNoLeavesRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class PlumTreeRand : BaseAddon
	{
		[Constructable]
		public PlumTreeRand()
		{
			switch ( Utility.Random( 10 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9967 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9968 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9969 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9970 ), 0, 0, 0 );
				break;

				case 4:
				// trunk
				AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9971 ), 0, 0, 0 );
				break;

				case 5:
				// trunk
				AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9967 ), 0, 0, 0 );
				break;
				
				case 6:
				// trunk
				AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9968 ), 0, 0, 0 );
				break;

				case 7:
				// trunk
				AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9969 ), 0, 0, 0 );
				break;

				case 8:
				// trunk
				AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9970 ), 0, 0, 0 );
				break;

				case 9:
				// trunk
				AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 9971 ), 0, 0, 0 );
				break;
			}

		}

		public PlumTreeRand( Serial serial ) : base( serial )
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


// =================================================================================


	public class PlumTree1a : BaseAddon
	{
		[Constructable]
		public PlumTree1a()
		{
			// tree
			AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9967 ), 0, 0, 0 );

		}

		public PlumTree1a( Serial serial ) : base( serial )
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


// =================================================================================


	public class PlumTree1b : BaseAddon
	{
		[Constructable]
		public PlumTree1b()
		{
			// tree
			AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9968 ), 0, 0, 0 );
		}

		public PlumTree1b( Serial serial ) : base( serial )
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


// =================================================================================


	public class PlumTree1c : BaseAddon
	{
		[Constructable]
		public PlumTree1c()
		{
			// tree
			AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9969 ), 0, 0, 0 );
		}

		public PlumTree1c( Serial serial ) : base( serial )
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


// =============================================================================


	public class PlumTree1d : BaseAddon
	{
		[Constructable]
		public PlumTree1d()
		{
			// tree
			AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9970 ), 0, 0, 0 );
		}

		public PlumTree1d( Serial serial ) : base( serial )
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


// =============================================================================


	public class PlumTree1e : BaseAddon
	{
		[Constructable]
		public PlumTree1e()
		{
			// tree
			AddComponent( new AddonComponent ( 9965 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9971 ), 0, 0, 0 );
		}

		public PlumTree1e( Serial serial ) : base( serial )
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


// =============================================================================


	public class PlumTree2a : BaseAddon
	{
		[Constructable]
		public PlumTree2a()
		{
			// tree
			AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9967 ), 0, 0, 0 );
		}

		public PlumTree2a( Serial serial ) : base( serial )
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


// =============================================================================


	public class PlumTree2b : BaseAddon
	{
		[Constructable]
		public PlumTree2b()
		{
			// tree
			AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9968 ), 0, 0, 0 );
		}

		public PlumTree2b( Serial serial ) : base( serial )
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


// =============================================================================


	public class PlumTree2c : BaseAddon
	{
		[Constructable]
		public PlumTree2c()
		{
			// tree
			AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9969 ), 0, 0, 0 );
		}

		public PlumTree2c( Serial serial ) : base( serial )
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


// =============================================================================


	public class PlumTree2d : BaseAddon
	{
		[Constructable]
		public PlumTree2d()
		{
			// tree
			AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9970 ), 0, 0, 0 );
		}

		public PlumTree2d( Serial serial ) : base( serial )
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


// =============================================================================


	public class PlumTree2e : BaseAddon
	{
		[Constructable]
		public PlumTree2e()
		{
			// tree
			AddComponent( new AddonComponent ( 9966 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 9971 ), 0, 0, 0 );
		}

		public PlumTree2e( Serial serial ) : base( serial )
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


// =============================================================================


	public class AppleTreeRand : BaseAddon
	{
		[Constructable]
		public AppleTreeRand()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3476 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3477 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3476 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3478 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3476 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3479 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3480 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3481 ), 0, 0, 0 );
				break;

				case 4:
				// trunk
				AddComponent( new AddonComponent ( 3480 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3482 ), 0, 0, 0 );
				break;

				case 5:
				// trunk
				AddComponent( new AddonComponent ( 3480 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3483 ), 0, 0, 0 );
				break;
			}

		}

		public AppleTreeRand( Serial serial ) : base( serial )
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


// =============================================================================


	public class AppleTree1Green : BaseAddon
	{
		[Constructable]
		public AppleTree1Green()
		{
			// tree
			AddComponent( new AddonComponent ( 3476 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3477 ), 0, 0, 0 );
		}

		public AppleTree1Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class AppleTree1Fruit : BaseAddon
	{
		[Constructable]
		public AppleTree1Fruit()
		{
			// tree
			AddComponent( new AddonComponent ( 3476 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3478 ), 0, 0, 0 );
		}

		public AppleTree1Fruit( Serial serial ) : base( serial )
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


// =========================================================================


	public class AppleTree1Red : BaseAddon
	{
		[Constructable]
		public AppleTree1Red()
		{
			// tree
			AddComponent( new AddonComponent ( 3476 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3479 ), 0, 0, 0 );
		}

		public AppleTree1Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class AppleTree2Green : BaseAddon
	{
		[Constructable]
		public AppleTree2Green()
		{
			// tree
			AddComponent( new AddonComponent ( 3480 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3481 ), 0, 0, 0 );
		}

		public AppleTree2Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class AppleTree2Fruit : BaseAddon
	{
		[Constructable]
		public AppleTree2Fruit()
		{
			// tree
			AddComponent( new AddonComponent ( 3480 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3482 ), 0, 0, 0 );
		}

		public AppleTree2Fruit( Serial serial ) : base( serial )
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


// =============================================================================


	public class AppleTree2Red : BaseAddon
	{
		[Constructable]
		public AppleTree2Red()
		{
			// tree
			AddComponent( new AddonComponent ( 3480 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3483 ), 0, 0, 0 );
		}

		public AppleTree2Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class PeachTreeRand : BaseAddon
	{
		[Constructable]
		public PeachTreeRand()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3484 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3485 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3484 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3486 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3484 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3487 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3488 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3489 ), 0, 0, 0 );
				break;

				case 4:
				// trunk
				AddComponent( new AddonComponent ( 3488 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3490 ), 0, 0, 0 );
				break;

				case 5:
				// trunk
				AddComponent( new AddonComponent ( 3488 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3491 ), 0, 0, 0 );
				break;
			}

		}

		public PeachTreeRand( Serial serial ) : base( serial )
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


// =============================================================================


	public class PeachTree1Green : BaseAddon
	{
		[Constructable]
		public PeachTree1Green()
		{
			// tree
			AddComponent( new AddonComponent ( 3484 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3485 ), 0, 0, 0 );
		}

		public PeachTree1Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class PeachTree1Fruit : BaseAddon
	{
		[Constructable]
		public PeachTree1Fruit()
		{
			// tree
			AddComponent( new AddonComponent ( 3484 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3486 ), 0, 0, 0 );
		}

		public PeachTree1Fruit( Serial serial ) : base( serial )
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


// =============================================================================


	public class PeachTree1Red : BaseAddon
	{
		[Constructable]
		public PeachTree1Red()
		{
			// tree
			AddComponent( new AddonComponent ( 3484 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3487 ), 0, 0, 0 );
		}

		public PeachTree1Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class PeachTree2Green : BaseAddon
	{
		[Constructable]
		public PeachTree2Green()
		{
			// tree
			AddComponent( new AddonComponent ( 3488 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3489 ), 0, 0, 0 );
		}

		public PeachTree2Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class PeachTree2Fruit : BaseAddon
	{
		[Constructable]
		public PeachTree2Fruit()
		{
			// tree
			AddComponent( new AddonComponent ( 3488 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3490 ), 0, 0, 0 );
		}

		public PeachTree2Fruit( Serial serial ) : base( serial )
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


// =============================================================================


	public class PeachTree2Red : BaseAddon
	{
		[Constructable]
		public PeachTree2Red()
		{
			// tree
			AddComponent( new AddonComponent ( 3488 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3491 ), 0, 0, 0 );
		}

		public PeachTree2Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class PearTreeRand : BaseAddon
	{
		[Constructable]
		public PearTreeRand()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3492 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3493 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3492 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3494 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3492 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3495 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3496 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3497 ), 0, 0, 0 );
				break;

				case 4:
				// trunk
				AddComponent( new AddonComponent ( 3496 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3498 ), 0, 0, 0 );
				break;

				case 5:
				// trunk
				AddComponent( new AddonComponent ( 3496 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3499 ), 0, 0, 0 );
				break;
			}

		}

		public PearTreeRand( Serial serial ) : base( serial )
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


// =============================================================================


	public class PearTree1Green : BaseAddon
	{
		[Constructable]
		public PearTree1Green()
		{
			// tree
			AddComponent( new AddonComponent ( 3492 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3493 ), 0, 0, 0 );
		}

		public PearTree1Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class PearTree1Fruit : BaseAddon
	{
		[Constructable]
		public PearTree1Fruit()
		{
			// tree
			AddComponent( new AddonComponent ( 3492 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3494 ), 0, 0, 0 );
		}

		public PearTree1Fruit( Serial serial ) : base( serial )
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


// =============================================================================


	public class PearTree1Red : BaseAddon
	{
		[Constructable]
		public PearTree1Red()
		{
			// tree
			AddComponent( new AddonComponent ( 3492 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3495 ), 0, 0, 0 );
		}

		public PearTree1Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class PearTree2Green : BaseAddon
	{
		[Constructable]
		public PearTree2Green()
		{
			// tree
			AddComponent( new AddonComponent ( 3496 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3497 ), 0, 0, 0 );
		}

		public PearTree2Green( Serial serial ) : base( serial )
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


// =============================================================================


	public class PearTree2Fruit : BaseAddon
	{
		[Constructable]
		public PearTree2Fruit()
		{
			// tree
			AddComponent( new AddonComponent ( 3496 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3498 ), 0, 0, 0 );
		}

		public PearTree2Fruit( Serial serial ) : base( serial )
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


// =============================================================================


	public class PearTree2Red : BaseAddon
	{
		[Constructable]
		public PearTree2Red()
		{
			// tree
			AddComponent( new AddonComponent ( 3496 ), 0, 0, 0 );
			// leaves
			AddComponent( new AddonComponent ( 3499 ), 0, 0, 0 );
		}

		public PearTree2Red( Serial serial ) : base( serial )
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


// =============================================================================


	public class ForestTreeRand : BaseAddon
	{
		[Constructable]
		public ForestTreeRand()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0:
				// trunk
				AddComponent( new AddonComponent ( 3277 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3278 ), 0, 0, 0 );
				break;

				case 1:
				// trunk
				AddComponent( new AddonComponent ( 3277 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3279 ), 0, 0, 0 );
				break;

				case 2:
				// trunk
				AddComponent( new AddonComponent ( 3280 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3281 ), 0, 0, 0 );
				break;

				case 3:
				// trunk
				AddComponent( new AddonComponent ( 3280 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3282 ), 0, 0, 0 );
				break;

				case 4:
				// trunk
				AddComponent( new AddonComponent ( 3283 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3284 ), 0, 0, 0 );
				break;

				case 5:
				// trunk
				AddComponent( new AddonComponent ( 3283 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3285 ), 0, 0, 0 );
				break;
				
				case 6:
				// trunk
				AddComponent( new AddonComponent ( 3290 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3291 ), 0, 0, 0 );
				break;
				
				case 7:
				// trunk
				AddComponent( new AddonComponent ( 3290 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3292 ), 0, 0, 0 );
				break;
				
				case 8:
				// trunk
				AddComponent( new AddonComponent ( 3293 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3294 ), 0, 0, 0 );
				break;
				
				case 9:
				// trunk
				AddComponent( new AddonComponent ( 3293 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3295 ), 0, 0, 0 );
				break;

				case 10:
				// trunk
				AddComponent( new AddonComponent ( 3296 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3297 ), 0, 0, 0 );
				break;

				case 11:
				// trunk
				AddComponent( new AddonComponent ( 3296 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3298 ), 0, 0, 0 );
				break;

				case 12:
				// trunk
				AddComponent( new AddonComponent ( 3299 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3300 ), 0, 0, 0 );
				break;

				case 13:
				// trunk
				AddComponent( new AddonComponent ( 3299 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3301 ), 0, 0, 0 );
				break;

				case 14:
				// trunk
				AddComponent( new AddonComponent ( 3302 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3303 ), 0, 0, 0 );
				break;

				case 15:
				// trunk
				AddComponent( new AddonComponent ( 3302 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3304 ), 0, 0, 0 );
				break;
				
				case 16:
				// trunk
				AddComponent( new AddonComponent ( 3320 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3321 ), 0, 0, 0 );
				break;
				
				case 17:
				// trunk
				AddComponent( new AddonComponent ( 3320 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3322 ), 0, 0, 0 );
				break;
				
				case 18:
				// trunk
				AddComponent( new AddonComponent ( 3323 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3324 ), 0, 0, 0 );
				break;
				
				case 19:
				// trunk
				AddComponent( new AddonComponent ( 3323 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3325 ), 0, 0, 0 );
				break;

				case 20:
				// trunk
				AddComponent( new AddonComponent ( 3326 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3327 ), 0, 0, 0 );
				break;

				case 21:
				// trunk
				AddComponent( new AddonComponent ( 3326 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3328 ), 0, 0, 0 );
				break;

				case 22:
				// trunk
				AddComponent( new AddonComponent ( 3329 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3330 ), 0, 0, 0 );
				break;

				case 23:
				// trunk
				AddComponent( new AddonComponent ( 3329 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3331 ), 0, 0, 0 );
				break;

				case 24:
				// trunk
				AddComponent( new AddonComponent ( 3476 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3477 ), 0, 0, 0 );
				break;

				case 25:
				// trunk
				AddComponent( new AddonComponent ( 3476 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3479 ), 0, 0, 0 );
				break;
				
				case 26:
				// trunk
				AddComponent( new AddonComponent ( 3480 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3481 ), 0, 0, 0 );
				break;
				
				case 27:
				// trunk
				AddComponent( new AddonComponent ( 3480 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3483 ), 0, 0, 0 );
				break;
				
				case 28:
				// trunk
				AddComponent( new AddonComponent ( 3484 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3485 ), 0, 0, 0 );
				break;
				
				case 29:
				// trunk
				AddComponent( new AddonComponent ( 3484 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3487 ), 0, 0, 0 );
				break;

				case 30:
				// trunk
				AddComponent( new AddonComponent ( 3488 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3489 ), 0, 0, 0 );
				break;

				case 31:
				// trunk
				AddComponent( new AddonComponent ( 3488 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3491 ), 0, 0, 0 );
				break;

				case 32:
				// trunk
				AddComponent( new AddonComponent ( 3492 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3493 ), 0, 0, 0 );
				break;

				case 33:
				// trunk
				AddComponent( new AddonComponent ( 3492 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3495 ), 0, 0, 0 );
				break;

				case 34:
				// trunk
				AddComponent( new AddonComponent ( 3496 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3497 ), 0, 0, 0 );
				break;

				case 35:
				// trunk
				AddComponent( new AddonComponent ( 3496 ), 0, 0, 0 );
				// leaves
				AddComponent( new AddonComponent ( 3499 ), 0, 0, 0 );
				break;
			}

		}

		public ForestTreeRand( Serial serial ) : base( serial )
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

// =============================================================================

