using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections;

namespace Server
{
	public class DailyRaresSystem
	{
		// Enables Daily Rare System
		public static readonly bool DailyRaresEnabled = true;

		// Enables Custom Rares
		public static readonly bool EnableCustomRares = true;

		// Enables Maps "If your server only uses a set number of maps, Disable the ones you dont need."
		public static readonly bool EnableFelucca = true;
		public static readonly bool EnableTrammel = true;
		public static readonly bool EnableMalas = true;
		public static readonly bool EnableIlshenar = true;
		public static readonly bool EnableTokuno = true;

		public static void StartRareSpawn( bool isSpawn )
		{
			// Delete all old rare not taken.
			ArrayList toDelete = new ArrayList();

			foreach ( Item item in World.Items.Values )
			{
				if ( item is BaseDailyRare )
				{
					BaseDailyRare bdr = (BaseDailyRare)item;
					if ( bdr.HasBeenMoved == false )
						toDelete.Add( bdr );
				}
					
				if ( item is BaseDailyRareCont )
				{
					BaseDailyRareCont bdrc = (BaseDailyRareCont)item;
					if ( bdrc.HasBeenMoved == false )
						toDelete.Add( bdrc );
				}

				// Future Support For Water Barrels / Tubs / Buckets
				/*if ( item.ItemID == 3703 || item.ItemID == 3715 || item.ItemID == 5344 )
				{	
					//Respawn old used barrels for water barrels
					if ( item.Visible == false )
					{
						item.Visible = true;
						item.Z += 20;
					}
				}*/
			}

			foreach ( Item delete in toDelete )
			{
				delete.Delete();
			}

			if ( isSpawn )
				SpawnRares();
		}

		public static void SpawnRares()
		{
			if ( DailyRaresEnabled == true )
			{
				// Spawn Rares (Felucca)
				if ( EnableFelucca == true )
				{
					DailyRocks fdrs = new DailyRocks();
					fdrs.MoveToWorld( new Point3D( 2683, 2056, 17 ), Map.Felucca );

					DailyRock fdr = new DailyRock();
					fdr.MoveToWorld( new Point3D( 5511, 3116, -4 ), Map.Felucca );

					DailyFruitBasket fdfb = new DailyFruitBasket();
					fdfb.MoveToWorld( new Point3D( 286, 986, 6 ), Map.Felucca );

					DailyClosedBarrel fdcb = new DailyClosedBarrel();
					fdcb.MoveToWorld( new Point3D( 5191, 587, 0 ), Map.Felucca );

					DailyCandle fdc1 = new DailyCandle();
					fdc1.Burning = true;
					fdc1.ItemID = 0xB1A;
					fdc1.MoveToWorld( new Point3D( 5576, 1829, 6 ), Map.Felucca );

					DailyCandle fdc2 = new DailyCandle();
					fdc2.Burning = true;
					fdc2.ItemID = 0xB1A;
					fdc2.MoveToWorld( new Point3D( 5582, 1829, 6 ), Map.Felucca );

					DailyHay fdh = new DailyHay();
					fdh.MoveToWorld( new Point3D( 5999, 3771, 21 ), Map.Felucca );

					DailyFullJars fdfj = new DailyFullJars();
					fdfj.MoveToWorld( new Point3D( 3660, 2504, 6 ), Map.Felucca );

					DailyFruitBasket fdb1 = new DailyFruitBasket();
					fdb1.CantBeLifted = true;
					fdb1.MoveToWorld( new Point3D( 3787, 1121, 26 ), Map.Felucca );

					DailyFruitBasket fdb2 = new DailyFruitBasket();
					fdb2.CantBeLifted = true;
					fdb2.MoveToWorld( new Point3D( 3667, 2255, 31 ), Map.Felucca );

					DailyFruitBasket fdb3 = new DailyFruitBasket();
					fdb3.CantBeLifted = true;
					fdb3.MoveToWorld( new Point3D( 1132, 2220, 66 ), Map.Felucca );

					DailyFruitBasket fdb4 = new DailyFruitBasket();
					fdb4.CantBeLifted = true;
					fdb4.MoveToWorld( new Point3D( 1135, 2220, 46 ), Map.Felucca );

					DailyFruitBasket fdb5 = new DailyFruitBasket();
					fdb5.CantBeLifted = true;
					fdb5.MoveToWorld( new Point3D( 2636, 2081, 16 ), Map.Felucca );
				}

				// Spawn Rares (Trammel)
				if ( EnableTrammel == true )
				{
					DailyRocks tdrs = new DailyRocks();
					tdrs.MoveToWorld( new Point3D( 2683, 2056, 17 ), Map.Trammel );

					DailyRock tdr = new DailyRock();
					tdr.MoveToWorld( new Point3D( 5511, 3116, -4 ), Map.Trammel );

					DailyFruitBasket tdfb = new DailyFruitBasket();
					tdfb.MoveToWorld( new Point3D( 286, 986, 6 ), Map.Trammel );

					DailyClosedBarrel tdcb = new DailyClosedBarrel();
					tdcb.MoveToWorld( new Point3D( 5191, 587, 0 ), Map.Trammel );

					DailyCandle tdc1 = new DailyCandle();
					tdc1.Burning = true;
					tdc1.ItemID = 0xB1A;
					tdc1.MoveToWorld( new Point3D( 5576, 1829, 6 ), Map.Trammel );

					DailyCandle tdc2 = new DailyCandle();
					tdc2.Burning = true;
					tdc2.ItemID = 0xB1A;
					tdc2.MoveToWorld( new Point3D( 5582, 1829, 6 ), Map.Trammel );

					DailyFruitBasket tdb1 = new DailyFruitBasket();
					tdb1.CantBeLifted = true;
					tdb1.MoveToWorld( new Point3D( 3787, 1121, 26 ), Map.Trammel );

					DailyFruitBasket tdb2 = new DailyFruitBasket();
					tdb2.CantBeLifted = true;
					tdb2.MoveToWorld( new Point3D( 3683, 2204, 31 ), Map.Trammel );

					DailyFruitBasket tdb3 = new DailyFruitBasket();
					tdb3.CantBeLifted = true;
					tdb3.MoveToWorld( new Point3D( 1132, 2220, 66 ), Map.Trammel );

					DailyFruitBasket tdb4 = new DailyFruitBasket();
					tdb4.CantBeLifted = true;
					tdb4.MoveToWorld( new Point3D( 1135, 2220, 46 ), Map.Trammel );

					DailyFruitBasket tdb5 = new DailyFruitBasket();
					tdb5.CantBeLifted = true;
					tdb5.MoveToWorld( new Point3D( 2636, 2081, 16 ), Map.Trammel );
				}

				// Spawn Rares (Ilshenar)
				if ( EnableIlshenar == true )
				{
					DailyBrokenChair idbc = new DailyBrokenChair();
					idbc.MoveToWorld( new Point3D( 148, 945, -29 ), Map.Ilshenar );
				}

				// Spawn Rares (Malas)
				if ( EnableMalas == true )
				{
					DailyMeatPie mdmp = new DailyMeatPie();
					mdmp.MoveToWorld( new Point3D( 2113, 1311, -44 ), Map.Malas );
				}

				// Spawn Rares (Tokuno)
				if ( EnableMalas == true )
				{
					// None
				}

				if ( EnableCustomRares == true )
				{
					// Spawn Custom Rares (Felucca)
					if ( EnableFelucca == true )
					{
						DailyLogs fdl = new DailyLogs();
						fdl.MoveToWorld( new Point3D( 626, 1152, 0 ), Map.Felucca );

						DailyArrows fda = new DailyArrows();
						fda.MoveToWorld( new Point3D( 3048, 3371, 21 ), Map.Felucca );

						DailyDung fdd = new DailyDung();
						fdd.MoveToWorld( new Point3D( 2524, 386, 15 ), Map.Felucca );

						DailyRedDresser fdrd = new DailyRedDresser();
						fdrd.MoveToWorld( new Point3D( 4648, 1222, 0 ), Map.Felucca );

						DailyDresser fdd2 = new DailyDresser();
						fdd2.MoveToWorld( new Point3D( 2257, 1216, 0 ), Map.Felucca );

						DailyEmptyJars fdej = new DailyEmptyJars();
						fdej.MoveToWorld( new Point3D( 5732, 93, 0 ), Map.Felucca );

						DailyOrfluer fdo = new DailyOrfluer();
						fdo.MoveToWorld( new Point3D( 1239, 2568, 0 ), Map.Felucca );

						DailyScareCrow fdsc = new DailyScareCrow();
						fdsc.MoveToWorld( new Point3D( 833, 2351, 0 ), Map.Felucca );

						DailyBookcase fdbc = new DailyBookcase();
						fdbc.MoveToWorld( new Point3D( 399, 1216, 0 ), Map.Felucca );
						
						BlanketOfDarkness fbod = new BlanketOfDarkness();
						fbod.MoveToWorld( new Point3D( 5787, 577, 10 ), Map.Felucca );
						
						BlanketOfDarkness fbods = new BlanketOfDarkness();
						fbods.MoveToWorld( new Point3D( 5865, 559, 15 ), Map.Felucca );
						
						//Fel Only Items
						DailyCopperCoins fdcc = new DailyCopperCoins();
						fdcc.MoveToWorld( new Point3D( 5496, 623, 26 ), Map.Felucca );
						
						DailyStump fds = new DailyStump();
						fds.MoveToWorld( new Point3D( 535, 992, 0 ), Map.Felucca );
						
						DailyEggWeb fdew = new DailyEggWeb();
						fdew.MoveToWorld( new Point3D( 5478, 1982, 0 ), Map.Felucca );
						
						DailyHooch fdh = new DailyHooch();
						fdh.MoveToWorld( new Point3D( 5322, 783, 0 ), Map.Felucca );
						
						DailyDragonTrophy fddt = new DailyDragonTrophy();
						fddt.MoveToWorld( new Point3D( 5645, 1402, 35 ), Map.Felucca );
						
						DailyScribeScrollG fdssg = new DailyScribeScrollG();
						fdssg.MoveToWorld( new Point3D( 6110, 81, 6 ), Map.Felucca );
						
						DailyPegBoard fdpb = new DailyPegBoard();
						fdpb.MoveToWorld( new Point3D( 5830, 347, 15 ), Map.Felucca );
						
						DailyPegBoard fdpb2 = new DailyPegBoard();
						fdpb2.MoveToWorld( new Point3D( 5833, 347, 15 ), Map.Felucca );
						
						DailyWhiteFuton fdwf = new DailyWhiteFuton();
						fdwf.MoveToWorld( new Point3D( 6381, 94, -20 ), Map.Felucca );
						
						DailyCatStatue fdcs = new DailyCatStatue();
						fdcs.MoveToWorld( new Point3D( 5525, 59, 5 ), Map.Felucca );
						
						DailyCocoonWeb fdcw = new DailyCocoonWeb();
						fdcw.MoveToWorld( new Point3D( 5777, 1945, 0 ), Map.Felucca );
						
						DailyFirePit fdfp = new DailyFirePit();
						fdfp.MoveToWorld( new Point3D( 5708, 522, 0 ), Map.Felucca );
						
						DailySolenEgg fdse = new DailySolenEgg();
						fdse.MoveToWorld( new Point3D( 5792, 1996, 0 ), Map.Felucca );
						
						DailySolenCocoon fdsco = new DailySolenCocoon();
						fdsco.MoveToWorld( new Point3D( 5664, 1871, 7 ), Map.Felucca );
						
						DailyBust fdbu = new DailyBust();
						fdbu.MoveToWorld( new Point3D( 4380, 921, 19 ), Map.Felucca );
						
						DailyGreenTable fdgt = new DailyGreenTable();
						fdgt.MoveToWorld( new Point3D( 1627, 1583, -20 ), Map.Felucca );

                        DailyChessmen fdcm = new DailyChessmen();
                        fdcm.MoveToWorld(new Point3D(1627, 1583, -20), Map.Felucca);

                        DailyPinkFuton fdpf = new DailyPinkFuton();
                        fdpf.MoveToWorld(new Point3D(3672, 2648, 5), Map.Felucca);

                        DailyLobsterTrophy fdlt = new DailyLobsterTrophy();
                        fdlt.MoveToWorld(new Point3D(1859, 2832, 10), Map.Felucca);

					}

					// Spawn Custom Rares (Trammel)
					if ( EnableTrammel == true )
					{
						DailyLogs tdl = new DailyLogs();
						tdl.MoveToWorld( new Point3D( 626, 1152, 0 ), Map.Trammel );

						DailyArrows tda = new DailyArrows();
						tda.MoveToWorld( new Point3D( 3048, 3371, 21 ), Map.Trammel );

						DailyDung tdd = new DailyDung();
						tdd.MoveToWorld( new Point3D( 2524, 386, 15 ), Map.Trammel );

						DailyRedDresser tdrd = new DailyRedDresser();
						tdrd.MoveToWorld( new Point3D( 4648, 1222, 0 ), Map.Trammel );

						DailyDresser tdd2 = new DailyDresser();
						tdd2.MoveToWorld( new Point3D( 2257, 1216, 0 ), Map.Trammel );

						DailyEmptyJars tdej = new DailyEmptyJars();
						tdej.MoveToWorld( new Point3D( 5732, 93, 0 ), Map.Trammel );

						DailyBlanket tdb = new DailyBlanket();
						tdb.MoveToWorld( new Point3D( 3677, 2609, 2 ), Map.Trammel );

						DailyBookcase tdbc = new DailyBookcase();
						tdbc.MoveToWorld( new Point3D( 399, 1216, 0 ), Map.Trammel );
						
						BlanketOfDarkness fbod = new BlanketOfDarkness();
						fbod.MoveToWorld( new Point3D( 5787, 577, 10 ), Map.Trammel );
						
						BlanketOfDarkness fbods = new BlanketOfDarkness();
						fbods.MoveToWorld( new Point3D( 5865, 559, 15 ), Map.Trammel );
						
						//Tram Only items
						DailyFancyLampPost tflp = new DailyFancyLampPost();
						tflp.Burning = true;
						tflp.ItemID = 0xB20;
						tflp.MoveToWorld( new Point3D( 3623, 2480, 0 ), Map.Trammel );
						
						DailyChickenCoop tdcc = new DailyChickenCoop();
						tdcc.MoveToWorld( new Point3D( 304, 945, 2 ), Map.Trammel );
						
						DailyBowStringer tdbs = new DailyBowStringer();
						tdbs.MoveToWorld( new Point3D( 1503, 363, 0 ), Map.Trammel );
						
						DailyEmptyKit tdek = new DailyEmptyKit();
						tdek.MoveToWorld( new Point3D( 2321, 649, 0 ), Map.Trammel );
						
						DailyGeode tdg = new DailyGeode();
						tdg.MoveToWorld( new Point3D( 2503, 427, 20 ), Map.Trammel );

						
					}

					// Spawn Custom Rares (Ilshenar)
					if ( EnableIlshenar == true )
					{
						DailyRuinedBookcase idrbc = new DailyRuinedBookcase();
						idrbc.MoveToWorld( new Point3D( 1218, 1152, -25 ), Map.Ilshenar );

						DailyFlax idf = new DailyFlax();
						idf.MoveToWorld( new Point3D( 588, 1176, -97 ), Map.Ilshenar );

						DailyCurtian idc = new DailyCurtian();
						idc.MoveToWorld( new Point3D( 1508, 537, 10 ), Map.Ilshenar );

						DailyCurtian idc2 = new DailyCurtian();
						idc2.MoveToWorld( new Point3D( 1508, 534, 10 ), Map.Ilshenar );
						
						DailyVBTrophy idvb = new DailyVBTrophy();
						idvb.MoveToWorld( new Point3D( 164, 755, -28 ), Map.Ilshenar );
					
						DailyWHTrophy idwh = new DailyWHTrophy();
						idwh.MoveToWorld( new Point3D( 664, 661, 5 ), Map.Ilshenar );
						
						DailyLantern idl = new DailyLantern();
						idl.MoveToWorld( new Point3D( 891, 1571, -28 ), Map.Ilshenar );
					}

					// Spawn Custom Rares (Malas)
					if ( EnableMalas == true )
					{
						DailyCrystal mdc = new DailyCrystal();
						mdc.MoveToWorld( new Point3D( 915, 722, -86 ), Map.Malas );

						DailyMushroom mdm = new DailyMushroom();
						mdm.MoveToWorld( new Point3D( 2014, 1179, -84 ), Map.Malas );

						DailySpittoon mds = new DailySpittoon();
						mds.MoveToWorld( new Point3D( 1052, 1438, -71 ), Map.Malas );
						
						DailyKnifeDart mdkd = new DailyKnifeDart();
						mdkd.MoveToWorld( new Point3D( 999, 509, -30 ), Map.Malas );
						
						//AcademicBooksW mabw = new AcademicBooksW();
						//mabw.MoveToWorld( new Point3D( 169, 1609, 2 ), Map.Malas );
						
						//AcademicBooksW mabs = new AcademicBooksW();
						//mabs.MoveToWorld( new Point3D( 166, 1643, 0 ), Map.Malas );
						
						//AcademicBooksE mabe = new AcademicBooksE();
						//mabe.MoveToWorld( new Point3D( 80, 1650, 20 ), Map.Malas );
					}

					// Spawn Custom Rares (Tokuno)
					if ( EnableTokuno == true )
					{
						DailySwords tds = new DailySwords();
						tds.MoveToWorld( new Point3D( 308, 421, 32 ), Map.Tokuno );

						DailyMenu tdm = new DailyMenu();
						tdm.MoveToWorld( new Point3D( 239, 1072, 20 ), Map.Tokuno );

						DailySushi tds2 = new DailySushi();
						tds2.MoveToWorld( new Point3D( 674, 1296, 30 ), Map.Tokuno );
						
						DailyAxeDart tdad = new DailyAxeDart();
						tdad.MoveToWorld( new Point3D( 538, 942, 49 ), Map.Tokuno );
						
						DailyBucketOfWater tdbw = new DailyBucketOfWater();
						tdbw.MoveToWorld( new Point3D( 260, 1133, 26 ), Map.Tokuno );
						
						DailyMapKW tdmkw = new DailyMapKW();
						tdmkw.MoveToWorld( new Point3D( 1230, 765, 43 ), Map.Tokuno );
					}
				}
			}
		}

		public static void StartExtrasSpawn( bool isSpawn )
		{
			// Delete all old rare not taken.
			ArrayList toDelete = new ArrayList();

			foreach ( Item item in World.Items.Values )
			{
				if ( item is DailyRareSpawner )
				{
					DailyRareSpawner drs = (DailyRareSpawner)item;
						toDelete.Add( drs );
				}
					
				if ( item is DailyTownCrate )
				{
					DailyTownCrate dtc = (DailyTownCrate)item;
						toDelete.Add( dtc );
				}
			}

			foreach ( Item delete in toDelete )
			{
				delete.Delete();
			}

			if ( isSpawn )
				SpawnExtras();
		}

		public static void SpawnExtras()
		{
			// Spawn Custom Rares (Felucca)
			if ( EnableFelucca == true )
			{
				// Artist Array List
				ArrayList art = new ArrayList();
				art.Add( "Artist" );

				// Blacksmith Item Array List
				ArrayList bsmith = new ArrayList();
				bsmith.Add( "CopperWire" );
				bsmith.Add( "GoldWire" );
				bsmith.Add( "IronWire" );
				bsmith.Add( "SilverWire" );
				bsmith.Add( "RareIngot" );
				bsmith.Add( "IronIngot" );
				bsmith.Add( "HorseShoes" );
				bsmith.Add( "ForgedMetal" );
				bsmith.Add( "IronOre" );
				bsmith.Add( "SmithHammer" );
				bsmith.Add( "Tongs" );

				// Spawn Npcs
				Spawner fdrs1 = new DailyRareSpawner();
				fdrs1.MoveToWorld( new Point3D( 4523, 1066, 0 ), Map.Felucca );
				//drs1.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs2 = new DailyRareSpawner();
				fdrs2.MoveToWorld( new Point3D( 4530, 1059, 0 ), Map.Felucca );
				//fdrs2.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs3 = new DailyRareSpawner();
				fdrs3.MoveToWorld( new Point3D( 4523, 1059, 0 ), Map.Felucca );
				//fdrs3.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs4 = new DailyRareSpawner();
				fdrs4.MoveToWorld( new Point3D( 2907, 714, 0 ), Map.Felucca );
				//fdrs4.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs5 = new DailyRareSpawner();
				fdrs5.MoveToWorld( new Point3D( 2907, 708, 0 ), Map.Felucca );
				//fdrs5.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs6 = new DailyRareSpawner();
				fdrs6.MoveToWorld( new Point3D( 1447, 1664, 10 ), Map.Felucca );
				//fdrs6.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				VoransTownCrate fdtc1 = new DailyBarrel(); // Bucs Smith Barrels
				fdtc1.MoveToWorld( new Point3D( 2636, 2085, 10 ), Map.Felucca );
				//fdtc1.InitSpawn( 5, TimeSpan.FromMinutes( 60 ), TimeSpan.FromMinutes( 300 ), bsmith );

				VoransTownCrate fdtc2 = new DailyBarrel(); // Bucs Smith Barrels
				fdtc2.MoveToWorld( new Point3D( 2636, 2084, 10 ), Map.Felucca );
				//fdtc2.InitSpawn( 5, TimeSpan.FromMinutes( 60 ), TimeSpan.FromMinutes( 300 ), bsmith );
			}

			// Spawn Custom Rares (Trammel)
			if ( EnableTrammel == true )
			{
				ArrayList art = new ArrayList();
				art.Add( "Artist" );

				// Spawn Npcs
				Spawner fdrs1 = new DailyRareSpawner();
				fdrs1.MoveToWorld( new Point3D( 4523, 1066, 0 ), Map.Trammel );
				//fdrs1.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs2 = new DailyRareSpawner();
				fdrs2.MoveToWorld( new Point3D( 4530, 1059, 0 ), Map.Trammel );
				//fdrs2.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs3 = new DailyRareSpawner();
				fdrs3.MoveToWorld( new Point3D( 4523, 1059, 0 ), Map.Trammel );
				//fdrs3.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs4 = new DailyRareSpawner();
				fdrs4.MoveToWorld( new Point3D( 2907, 714, 0 ), Map.Trammel );
				//fdrs4.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs5 = new DailyRareSpawner();
				fdrs5.MoveToWorld( new Point3D( 2907, 708, 0 ), Map.Trammel );
				//fdrs5.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );

				Spawner fdrs6 = new DailyRareSpawner();
				fdrs6.MoveToWorld( new Point3D( 1447, 1664, 10 ), Map.Trammel );
				//fdrs6.InitSpawn( 1, TimeSpan.FromMinutes( 5 ), TimeSpan.FromMinutes( 10 ), art );
			}

			// Spawn Custom Rares (Ilshenar)
			if ( EnableIlshenar == true )
			{
				//No Spawns Yet
			}

			// Spawn Custom Rares (Malas)
			if ( EnableMalas == true )
			{
				//No Spawns Yet
			}

			// Spawn Custom Rares (Tokuno)
			if ( EnableTokuno == true )
			{
				//No Spawns Yet
			}
		}
	}
}