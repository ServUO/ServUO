// Landscaping.cs 
//
// Created By Amythest
//  
//  Version: 1.0
//  Last Modified by Amythest (at) moonpoint.com on 9/28/2005
//  

using System;
using Server;
using Server.Items;


namespace Server.Items
{


// =============================================================================

	public class FlowersMiniRand : BaseAddon
	{
		[Constructable]
		public FlowersMiniRand()
		{
			switch ( Utility.Random( 12 ) )
			{
				case 0:
				// flowers
				AddComponent( new AddonComponent ( 3127 ), 0, 0, 0 );
				break;

				case 1:
				// flowers
				AddComponent( new AddonComponent ( 3128 ), 0, 0, 0 );
				break;

				case 2:
				// flowers
				AddComponent( new AddonComponent ( 3141 ), 0, 0, 0 );
				break;

				case 3:
				// flowers
				AddComponent( new AddonComponent ( 3142 ), 0, 0, 0 );
				break;

				case 4:
				// flowers
				AddComponent( new AddonComponent ( 3143 ), 0, 0, 0 );
				break;

				case 5:
				// flowers
				AddComponent( new AddonComponent ( 3144 ), 0, 0, 0 );
				break;

				case 6:
				// flowers
				AddComponent( new AddonComponent ( 3145 ), 0, 0, 0 );
				break;

				case 7:
				// flowers
				AddComponent( new AddonComponent ( 3146 ), 0, 0, 0 );
				break;

				case 8:
				// flowers
				AddComponent( new AddonComponent ( 3147 ), 0, 0, 0 );
				break;

				case 9:
				// flowers
				AddComponent( new AddonComponent ( 3148 ), 0, 0, 0 );
				break;

				case 10:
				// flowers
				AddComponent( new AddonComponent ( 3149 ), 0, 0, 0 );
				break;

				case 11:
				// flowers
				AddComponent( new AddonComponent ( 3150 ), 0, 0, 0 );
				break;
			}

		}

		public FlowersMiniRand( Serial serial ) : base( serial )
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

	public class FlowersMiniPurpleRand : BaseAddon
	{
		[Constructable]
		public FlowersMiniPurpleRand()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0:
				// flowers
				AddComponent( new AddonComponent ( 3142 ), 0, 0, 0 );
				break;

				case 1:
				// flowers
				AddComponent( new AddonComponent ( 3145 ), 0, 0, 0 );
				break;

				case 2:
				// flowers
				AddComponent( new AddonComponent ( 3146 ), 0, 0, 0 );
				break;
			}

		}

		public FlowersMiniPurpleRand( Serial serial ) : base( serial )
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

	public class FlowersMiniPurpleYellowRand : BaseAddon
	{
		[Constructable]
		public FlowersMiniPurpleYellowRand()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0:
				// flowers
				AddComponent( new AddonComponent ( 3128 ), 0, 0, 0 );
				break;

				case 1:
				// flowers
				AddComponent( new AddonComponent ( 3149 ), 0, 0, 0 );
				break;

				case 2:
				// flowers
				AddComponent( new AddonComponent ( 3150 ), 0, 0, 0 );
				break;
			}

		}

		public FlowersMiniPurpleYellowRand( Serial serial ) : base( serial )
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

	public class FlowersMiniYellowRand : BaseAddon
	{
		[Constructable]
		public FlowersMiniYellowRand()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0:
				// flowers
				AddComponent( new AddonComponent ( 3127 ), 0, 0, 0 );
				break;

				case 1:
				// flowers
				AddComponent( new AddonComponent ( 3141 ), 0, 0, 0 );
				break;

				case 2:
				// flowers
				AddComponent( new AddonComponent ( 3143 ), 0, 0, 0 );
				break;

				case 3:
				// flowers
				AddComponent( new AddonComponent ( 3144 ), 0, 0, 0 );
				break;

				case 4:
				// flowers
				AddComponent( new AddonComponent ( 3147 ), 0, 0, 0 );
				break;

				case 5:
				// flowers
				AddComponent( new AddonComponent ( 3148 ), 0, 0, 0 );
				break;
			}

		}

		public FlowersMiniYellowRand( Serial serial ) : base( serial )
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

	public class FlowerRand : BaseAddon
	{
		[Constructable]
		public FlowerRand()
		{
			switch ( Utility.Random( 16 ) )
			{
				case 0:
				// flowers
				AddComponent( new AddonComponent ( 3203 ), 0, 0, 0 );
				break;

				case 1:
				// flowers
				AddComponent( new AddonComponent ( 3204 ), 0, 0, 0 );
				break;

				case 2:
				// flowers
				AddComponent( new AddonComponent ( 3205 ), 0, 0, 0 );
				break;

				case 3:
				// flowers
				AddComponent( new AddonComponent ( 3206 ), 0, 0, 0 );
				break;

				case 4:
				// flowers
				AddComponent( new AddonComponent ( 3207 ), 0, 0, 0 );
				break;

				case 5:
				// flowers
				AddComponent( new AddonComponent ( 3208 ), 0, 0, 0 );
				break;
				case 6:
				// flowers
				AddComponent( new AddonComponent ( 3209 ), 0, 0, 0 );
				break;

				case 7:
				// flowers
				AddComponent( new AddonComponent ( 3210 ), 0, 0, 0 );
				break;

				case 8:
				// flowers
				AddComponent( new AddonComponent ( 3211 ), 0, 0, 0 );
				break;

				case 9:
				// flowers
				AddComponent( new AddonComponent ( 3212 ), 0, 0, 0 );
				break;

				case 10:
				// flowers
				AddComponent( new AddonComponent ( 3213 ), 0, 0, 0 );
				break;

				case 11:
				// flowers
				AddComponent( new AddonComponent ( 3214 ), 0, 0, 0 );
				break;

				case 12:
				// flowers
				AddComponent( new AddonComponent ( 3262 ), 0, 0, 0 );
				break;

				case 13:
				// flowers
				AddComponent( new AddonComponent ( 3263 ), 0, 0, 0 );
				break;

				case 14:
				// flowers
				AddComponent( new AddonComponent ( 3264 ), 0, 0, 0 );
				break;

				case 15:
				// flowers
				AddComponent( new AddonComponent ( 3265 ), 0, 0, 0 );
				break;
			}

		}

		public FlowerRand( Serial serial ) : base( serial )
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

	public class ForestBrambleRand : BaseAddon
	{
		[Constructable]
		public ForestBrambleRand()
		{
			switch ( Utility.Random( 7 ) )
			{
				case 0:
				// bramble
				AddComponent( new AddonComponent ( 12320 ), 0, 0, 0 );
				break;

				case 1:
				// bramble
				AddComponent( new AddonComponent ( 12321 ), 0, 0, 0 );
				break;

				case 2:
				// bramble
				AddComponent( new AddonComponent ( 12322 ), 0, 0, 0 );
				break;

				case 3:
				// bramble
				AddComponent( new AddonComponent ( 12323 ), 0, 0, 0 );
				break;

				case 4:
				// bramble
				AddComponent( new AddonComponent ( 12324 ), 0, 0, 0 );
				break;

				case 5:
				// bramble
				AddComponent( new AddonComponent ( 3391 ), 0, 0, 0 );
				break;

				case 6:
				// bramble
				AddComponent( new AddonComponent ( 3392 ), 0, 0, 0 );
				break;
			}

		}

		public ForestBrambleRand( Serial serial ) : base( serial )
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

	public class ForestGrassPlantRand : BaseAddon
	{
		[Constructable]
		public ForestGrassPlantRand()
		{
			switch ( Utility.Random( 10 ) )
			{
				case 0:
				// plant
				AddComponent( new AddonComponent ( 3244 ), 0, 0, 0 );
				break;

				case 1:
				// plant
				AddComponent( new AddonComponent ( 3245 ), 0, 0, 0 );
				break;

				case 2:
				// plant
				AddComponent( new AddonComponent ( 3247 ), 0, 0, 0 );
				break;

				case 3:
				// plant
				AddComponent( new AddonComponent ( 3248 ), 0, 0, 0 );
				break;

				case 4:
				// plant
				AddComponent( new AddonComponent ( 3249 ), 0, 0, 0 );
				break;

				case 5:
				// plant
				AddComponent( new AddonComponent ( 3250 ), 0, 0, 0 );
				break;

				case 6:
				// plant
				AddComponent( new AddonComponent ( 3251 ), 0, 0, 0 );
				break;

				case 7:
				// plant
				AddComponent( new AddonComponent ( 3252 ), 0, 0, 0 );
				break;

				case 8:
				// plant
				AddComponent( new AddonComponent ( 3253 ), 0, 0, 0 );
				break;

				case 9:
				// plant
				AddComponent( new AddonComponent ( 3254 ), 0, 0, 0 );
				break;
			}

		}

		public ForestGrassPlantRand( Serial serial ) : base( serial )
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

	public class ForestPlantRand : BaseAddon
	{
		[Constructable]
		public ForestPlantRand()
		{
			switch ( Utility.Random( 11 ) )
			{
				case 0:
				// plant
				AddComponent( new AddonComponent ( 3219 ), 0, 0, 0 );
				break;

				case 1:
				// plant
				AddComponent( new AddonComponent ( 3220 ), 0, 0, 0 );
				break;

				case 2:
				// plant
				AddComponent( new AddonComponent ( 3223 ), 0, 0, 0 );
				break;

				case 3:
				// plant
				AddComponent( new AddonComponent ( 3259 ), 0, 0, 0 );
				break;

				case 4:
				// plant
				AddComponent( new AddonComponent ( 3267 ), 0, 0, 0 );
				break;

				case 5:
				// plant
				AddComponent( new AddonComponent ( 3268 ), 0, 0, 0 );
				break;

				case 6:
				// plant
				AddComponent( new AddonComponent ( 3269 ), 0, 0, 0 );
				break;

				case 7:
				// plant
				AddComponent( new AddonComponent ( 3270 ), 0, 0, 0 );
				break;

				case 8:
				// plant
				AddComponent( new AddonComponent ( 3271 ), 0, 0, 0 );
				break;

				case 9:
				// plant
				AddComponent( new AddonComponent ( 3272 ), 0, 0, 0 );
				break;

				case 10:
				// plant
				AddComponent( new AddonComponent ( 3273 ), 0, 0, 0 );
				break;
			}

		}

		public ForestPlantRand( Serial serial ) : base( serial )
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

	public class FernPlantRand : BaseAddon
	{
		[Constructable]
		public FernPlantRand()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0:
				// plant
				AddComponent( new AddonComponent ( 3231 ), 0, 0, 0 );
				break;

				case 1:
				// plant
				AddComponent( new AddonComponent ( 3232 ), 0, 0, 0 );
				break;

				case 2:
				// plant
				AddComponent( new AddonComponent ( 3233 ), 0, 0, 0 );
				break;

				case 3:
				// plant
				AddComponent( new AddonComponent ( 3234 ), 0, 0, 0 );
				break;

				case 4:
				// plant
				AddComponent( new AddonComponent ( 3235 ), 0, 0, 0 );
				break;

				case 5:
				// plant
				AddComponent( new AddonComponent ( 3236 ), 0, 0, 0 );
				break;
			}

		}

		public FernPlantRand( Serial serial ) : base( serial )
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

	public class MushroomPlantRand : BaseAddon
	{
		[Constructable]
		public MushroomPlantRand()
		{
			switch ( Utility.Random( 14 ) )
			{
				case 0:
				// plant
				AddComponent( new AddonComponent ( 3340 ), 0, 0, 0 );
				break;

				case 1:
				// plant
				AddComponent( new AddonComponent ( 3341 ), 0, 0, 0 );
				break;

				case 2:
				// plant
				AddComponent( new AddonComponent ( 3342 ), 0, 0, 0 );
				break;

				case 3:
				// plant
				AddComponent( new AddonComponent ( 3343 ), 0, 0, 0 );
				break;

				case 4:
				// plant
				AddComponent( new AddonComponent ( 3344 ), 0, 0, 0 );
				break;

				case 5:
				// plant
				AddComponent( new AddonComponent ( 3345 ), 0, 0, 0 );
				break;

				case 6:
				// plant
				AddComponent( new AddonComponent ( 3346 ), 0, 0, 0 );
				break;

				case 7:
				// plant
				AddComponent( new AddonComponent ( 3347 ), 0, 0, 0 );
				break;

				case 8:
				// plant
				AddComponent( new AddonComponent ( 3348 ), 0, 0, 0 );
				break;

				case 9:
				// plant
				AddComponent( new AddonComponent ( 3349 ), 0, 0, 0 );
				break;

				case 10:
				// plant
				AddComponent( new AddonComponent ( 3350 ), 0, 0, 0 );
				break;

				case 11:
				// plant
				AddComponent( new AddonComponent ( 3351 ), 0, 0, 0 );
				break;

				case 12:
				// plant
				AddComponent( new AddonComponent ( 3352 ), 0, 0, 0 );
				break;

				case 13:
				// plant
				AddComponent( new AddonComponent ( 3353 ), 0, 0, 0 );
				break;
			}

		}

		public MushroomPlantRand( Serial serial ) : base( serial )
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

	public class DesertCactusPlantRand : BaseAddon
	{
		[Constructable]
		public DesertCactusPlantRand()
		{
			switch ( Utility.Random( 12 ) )
			{
				case 0:
				// plant
				AddComponent( new AddonComponent ( 3365 ), 0, 0, 0 );
				break;

				case 1:
				// plant
				AddComponent( new AddonComponent ( 3366 ), 0, 0, 0 );
				break;

				case 2:
				// plant
				AddComponent( new AddonComponent ( 3367 ), 0, 0, 0 );
				break;

				case 3:
				// plant
				AddComponent( new AddonComponent ( 3368 ), 0, 0, 0 );
				// flowers
				AddComponent( new AddonComponent ( 3369 ), 0, 0, 0 );
				break;

				case 4:
				// plant
				AddComponent( new AddonComponent ( 3370 ), 0, 0, 0 );
				break;

				case 5:
				// plant
				AddComponent( new AddonComponent ( 3370 ), 0, 0, 0 );
				// flowers
				AddComponent( new AddonComponent ( 3371 ), 0, 0, 0 );
				break;

				case 6:
				// plant
				AddComponent( new AddonComponent ( 3372 ), 0, 0, 0 );
				break;

				case 7:
				// plant
				AddComponent( new AddonComponent ( 3372 ), 0, 0, 0 );
				// flowers
				AddComponent( new AddonComponent ( 3373 ), 0, 0, 0 );
				break;

				case 8:
				// plant
				AddComponent( new AddonComponent ( 3374 ), 0, 0, 0 );
				break;

				case 9:
				// plant
				AddComponent( new AddonComponent ( 3374 ), 0, 0, 0 );
				// flowers
				AddComponent( new AddonComponent ( 3375 ), 0, 0, 0 );
				break;

				case 10:
				// plant
				AddComponent( new AddonComponent ( 3381 ), 0, 0, 0 );
				break;

				case 11:
				// plant
				AddComponent( new AddonComponent ( 3381 ), 0, 0, 0 );
				// flowers
				AddComponent( new AddonComponent ( 3382 ), 0, 0, 0 );
				break;
			}

		}

		public DesertCactusPlantRand( Serial serial ) : base( serial )
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

	public class DesertPlantRand : BaseAddon
	{
		[Constructable]
		public DesertPlantRand()
		{
			switch ( Utility.Random( 7 ) )
			{
				case 0:
				// plant
				AddComponent( new AddonComponent ( 3391 ), 0, 0, 0 );
				break;

				case 1:
				// plant
				AddComponent( new AddonComponent ( 3392 ), 0, 0, 0 );
				break;

				case 2:
				// plant
				AddComponent( new AddonComponent ( 3270 ), 0, 0, 0 );
				break;

				case 3:
				// plant
				AddComponent( new AddonComponent ( 3376 ), 0, 0, 0 );
				break;

				case 4:
				// plant
				AddComponent( new AddonComponent ( 3377 ), 0, 0, 0 );
				break;

				case 5:
				// plant
				AddComponent( new AddonComponent ( 3378 ), 0, 0, 0 );
				break;

				case 6:
				// plant
				AddComponent( new AddonComponent ( 3379 ), 0, 0, 0 );
				break;
			}

		}

		public DesertPlantRand( Serial serial ) : base( serial )
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

	public class SwampPlantRand : BaseAddon
	{
		[Constructable]
		public SwampPlantRand()
		{
			switch ( Utility.Random( 19 ) )
			{
				case 0:
				// plant
				AddComponent( new AddonComponent ( 3219 ), 0, 0, 0 );
				break;

				case 1:
				// plant
				AddComponent( new AddonComponent ( 3220 ), 0, 0, 0 );
				break;

				case 2:
				// plant
				AddComponent( new AddonComponent ( 3223 ), 0, 0, 0 );
				break;

				case 3:
				// plant
				AddComponent( new AddonComponent ( 3224 ), 0, 0, 0 );
				break;

				case 4:
				// plant
				AddComponent( new AddonComponent ( 3237 ), 0, 0, 0 );
				break;

				case 5:
				// plant
				AddComponent( new AddonComponent ( 3228 ), 0, 0, 0 );
				break;

				case 6:
				// plant
				AddComponent( new AddonComponent ( 3229 ), 0, 0, 0 );
				break;

				case 7:
				// plant
				AddComponent( new AddonComponent ( 3241 ), 0, 0, 0 );
				break;

				case 8:
				// plant
				AddComponent( new AddonComponent ( 3255 ), 0, 0, 0 );
				break;

				case 9:
				// plant
				AddComponent( new AddonComponent ( 3256 ), 0, 0, 0 );
				break;

				case 10:
				// plant
				AddComponent( new AddonComponent ( 3257 ), 0, 0, 0 );
				break;

				case 11:
				// plant
				AddComponent( new AddonComponent ( 3258 ), 0, 0, 0 );
				break;

				case 12:
				// plant
				AddComponent( new AddonComponent ( 3259 ), 0, 0, 0 );
				break;

				case 13:
				// plant
				AddComponent( new AddonComponent ( 3260 ), 0, 0, 0 );
				break;

				case 14:
				// plant
				AddComponent( new AddonComponent ( 3261 ), 0, 0, 0 );
				break;

				case 15:
				// plant
				AddComponent( new AddonComponent ( 3267 ), 0, 0, 0 );
				break;

				case 16:
				// plant
				AddComponent( new AddonComponent ( 3268 ), 0, 0, 0 );
				break;

				case 17:
				// plant
				AddComponent( new AddonComponent ( 3269 ), 0, 0, 0 );
				break;

				case 18:
				// plant
				AddComponent( new AddonComponent ( 3271 ), 0, 0, 0 );
				break;
			}

		}

		public SwampPlantRand( Serial serial ) : base( serial )
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

	public class WaterPlantRand : BaseAddon
	{
		[Constructable]
		public WaterPlantRand()
		{
			switch ( Utility.Random( 14 ) )
			{
				case 0:
				// plant
				AddComponent( new AddonComponent ( 3332 ), 0, 0, 0 );
				break;

				case 1:
				// plant
				AddComponent( new AddonComponent ( 3333 ), 0, 0, 0 );
				break;

				case 2:
				// plant
				AddComponent( new AddonComponent ( 3334 ), 0, 0, 0 );
				break;

				case 3:
				// plant
				AddComponent( new AddonComponent ( 3335 ), 0, 0, 0 );
				break;

				case 4:
				// plant
				AddComponent( new AddonComponent ( 3336 ), 0, 0, 0 );
				break;

				case 5:
				// plant
				AddComponent( new AddonComponent ( 3337 ), 0, 0, 0 );
				break;

				case 6:
				// plant
				AddComponent( new AddonComponent ( 3338 ), 0, 0, 0 );
				break;

				case 7:
				// plant
				AddComponent( new AddonComponent ( 3339 ), 0, 0, 0 );
				break;

				case 8:
				// plant
				AddComponent( new AddonComponent ( 3516 ), 0, 0, 0 );
				break;

				case 9:
				// plant
				AddComponent( new AddonComponent ( 3517 ), 0, 0, 0 );
				break;

				case 10:
				// plant
				AddComponent( new AddonComponent ( 3518 ), 0, 0, 0 );
				break;

				case 11:
				// plant
				AddComponent( new AddonComponent ( 3521 ), 0, 0, 0 );
				break;

				case 12:
				// plant
				AddComponent( new AddonComponent ( 3522 ), 0, 0, 0 );
				break;

				case 13:
				// plant
				AddComponent( new AddonComponent ( 3523 ), 0, 0, 0 );
				break;
			}

		}

		public WaterPlantRand( Serial serial ) : base( serial )
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

	public class Rock1Rand : BaseAddon
	{
		[Constructable]
		public Rock1Rand()
		{
			switch ( Utility.Random( 11 ) )
			{
				case 0:
				// rock
				AddComponent( new AddonComponent ( 4963 ), 0, 0, 0 );
				break;

				case 1:
				// rock
				AddComponent( new AddonComponent ( 4964 ), 0, 0, 0 );
				break;

				case 2:
				// rock
				AddComponent( new AddonComponent ( 4965 ), 0, 0, 0 );
				break;

				case 3:
				// rock
				AddComponent( new AddonComponent ( 4966 ), 0, 0, 0 );
				break;

				case 4:
				// rock
				AddComponent( new AddonComponent ( 4967 ), 0, 0, 0 );
				break;

				case 5:
				// rock
				AddComponent( new AddonComponent ( 4968 ), 0, 0, 0 );
				break;

				case 6:
				// rock
				AddComponent( new AddonComponent ( 4969 ), 0, 0, 0 );
				break;

				case 7:
				// rock
				AddComponent( new AddonComponent ( 4970 ), 0, 0, 0 );
				break;

				case 8:
				// rock
				AddComponent( new AddonComponent ( 4971 ), 0, 0, 0 );
				break;

				case 9:
				// rock
				AddComponent( new AddonComponent ( 4972 ), 0, 0, 0 );
				break;

				case 10:
				// rock
				AddComponent( new AddonComponent ( 4973 ), 0, 0, 0 );
				break;
			}

		}

		public Rock1Rand( Serial serial ) : base( serial )
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

	public class Rock2Rand : BaseAddon
	{
		[Constructable]
		public Rock2Rand()
		{
			switch ( Utility.Random( 11 ) )
			{
				case 0:
				// rock
				AddComponent( new AddonComponent ( 6001 ), 0, 0, 0 );
				break;

				case 1:
				// rock
				AddComponent( new AddonComponent ( 6002 ), 0, 0, 0 );
				break;

				case 2:
				// rock
				AddComponent( new AddonComponent ( 6003 ), 0, 0, 0 );
				break;

				case 3:
				// rock
				AddComponent( new AddonComponent ( 6004 ), 0, 0, 0 );
				break;

				case 4:
				// rock
				AddComponent( new AddonComponent ( 6005 ), 0, 0, 0 );
				break;

				case 5:
				// rock
				AddComponent( new AddonComponent ( 6006 ), 0, 0, 0 );
				break;

				case 6:
				// rock
				AddComponent( new AddonComponent ( 6007 ), 0, 0, 0 );
				break;

				case 7:
				// rock
				AddComponent( new AddonComponent ( 6008 ), 0, 0, 0 );
				break;

				case 8:
				// rock
				AddComponent( new AddonComponent ( 6009 ), 0, 0, 0 );
				break;

				case 9:
				// rock
				AddComponent( new AddonComponent ( 6010 ), 0, 0, 0 );
				break;

				case 10:
				// rock
				AddComponent( new AddonComponent ( 6011 ), 0, 0, 0 );
				break;
			}

		}

		public Rock2Rand( Serial serial ) : base( serial )
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
}