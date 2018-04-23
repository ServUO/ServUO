using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class RandomSnowGlobeDeed : Item
	{

		[Constructable]
		public RandomSnowGlobeDeed() : this( null )
		{
		}

		[Constructable]
		public RandomSnowGlobeDeed ( string name ) : base ( 0x14F0 )
		{
			Name = "Random Snow Globe Deed";
			Hue = 1150;
		}

		public RandomSnowGlobeDeed ( Serial serial ) : base ( serial )
		{
		}

      		public override void OnDoubleClick( Mobile from ) 
      		{
			if ( !IsChildOf( from.Backpack ) )
			{
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                switch (Utility.Random(33))
                {
                    case 0: from.AddToBackpack(new BritainGlobe()); break;
                    case 1: from.AddToBackpack(new BucsGlobe()); break;
                    case 2: from.AddToBackpack(new CoveGlobe()); break;
                    case 3: from.AddToBackpack(new DeluciaGlobe()); break;
                    case 4: from.AddToBackpack(new EmpathGlobe()); break;
                    case 5: from.AddToBackpack(new JhelomGlobe()); break;
                    case 6: from.AddToBackpack(new LycaeumGlobe()); break;
                    case 7: from.AddToBackpack(new MaginciaGlobe()); break;
                    case 8: from.AddToBackpack(new MinocGlobe()); break;
                    case 9: from.AddToBackpack(new MoonglowGlobe()); break;
                    case 10: from.AddToBackpack(new NujelmGlobe()); break;
                    case 11: from.AddToBackpack(new OccloGlobe()); break;
                    case 12: from.AddToBackpack(new PapuaGlobe()); break;
                    case 13: from.AddToBackpack(new SerpentsGlobe()); break;
                    case 14: from.AddToBackpack(new SkaraGlobe()); break;
                    case 15: from.AddToBackpack(new TrinsicGlobe()); break;
                    case 16: from.AddToBackpack(new VesperGlobe()); break;
                    case 17: from.AddToBackpack(new WindGlobe()); break;
                    case 18: from.AddToBackpack(new YewGlobe()); break;
                    case 19: from.AddToBackpack(new BlackthorneGlobe()); break;
                    case 20: from.AddToBackpack(new ChaosGlobe()); break;
                    case 21: from.AddToBackpack(new CitadelGlobe()); break;
                    case 22: from.AddToBackpack(new CompassionGlobe()); break;
                    case 23: from.AddToBackpack(new EtheralGlobe()); break;
                    case 24: from.AddToBackpack(new ExodusGlobe()); break;
                    case 25: from.AddToBackpack(new LakeOfFireGlobe()); break;
                    case 26: from.AddToBackpack(new HonestyGlobe()); break;
                    case 27: from.AddToBackpack(new HonorGlobe()); break;
                    case 28: from.AddToBackpack(new HumilityGlobe()); break;
                    case 29: from.AddToBackpack(new JusticeGlobe()); break;
                    case 30: from.AddToBackpack(new KarnaughGlobe()); break;
                    case 31: from.AddToBackpack(new LakeshireGlobe()); break;
                    case 32: from.AddToBackpack(new MistasGlobe()); break;
                    case 33: from.AddToBackpack(new MontorGlobe()); break;
                    case 34: from.AddToBackpack(new SacrificeGlobe()); break;
                    case 35: from.AddToBackpack(new SpiritualityGlobe()); break;
                    case 36: from.AddToBackpack(new TavernGlobe()); break;
                    case 37: from.AddToBackpack(new ValorGlobe()); break;
                    case 38: from.AddToBackpack(new CastleBritanniaGlobe()); break;
                    case 39: from.AddToBackpack(new HeartwoodCityGlobe()); break;
                    case 40: from.AddToBackpack(new ZentoCityGlobe()); break;
                    case 41: from.AddToBackpack(new TheWasteGlobe()); break;
                    case 42: from.AddToBackpack(new BushidoDojoGlobe()); break;
                    case 43: from.AddToBackpack(new EchoFieldsGlobe()); break;
                    case 44: from.AddToBackpack(new CraneMarshGlobe()); break;
                    case 45: from.AddToBackpack(new YomotsuMinesGlobe()); break;
                    case 46: from.AddToBackpack(new KitsuneWoodsGlobe()); break;
                    case 47: from.AddToBackpack(new DefiancePointGlobe()); break;
                    case 48: from.AddToBackpack(new WinterSpurGlobe()); break;
                    case 49: from.AddToBackpack(new FanDancerDojoGlobe()); break;
                    case 50: from.AddToBackpack(new MountShoGlobe()); break;
                    case 51: from.AddToBackpack(new LotusLakeGlobe()); break;
                    case 52: from.AddToBackpack(new StormPointGlobe()); break;
                    case 53: from.AddToBackpack(new SleepingDragonValleyGlobe()); break;
                    case 54: from.AddToBackpack(new LunaGlobe()); break;
                    case 55: from.AddToBackpack(new UmbraGlobe()); break;
                    case 56: from.AddToBackpack(new DoomGlobe()); break;
                    case 57: from.AddToBackpack(new OrcFortGlobe()); break;
                    case 58: from.AddToBackpack(new ForgottenPyramidGlobe()); break;
                    case 59: from.AddToBackpack(new NorthernMountiansGlobe()); break;
                    case 60: from.AddToBackpack(new HansesHostelGlobe()); break;
                    case 61: from.AddToBackpack(new CorruptedForestGlobe()); break;
                    case 62: from.AddToBackpack(new GrimswindRuinsGlobe()); break;
                    case 63: from.AddToBackpack(new GreenAcresGlobe()); break;
                    case 64: from.AddToBackpack(new HedgeMazeGlobe()); break;
                    case 65: from.AddToBackpack(new MarbleIslandGlobe()); break;
                    case 66: from.AddToBackpack(new GreatWaterfallGlobe()); break;
                    case 67: from.AddToBackpack(new TheCryptGlobe()); break;
                    case 68: from.AddToBackpack(new IslandTempleGlobe()); break;
                    case 69: from.AddToBackpack(new IceIslandGlobe()); break;
                    case 70: from.AddToBackpack(new OphidianTempleGlobe()); break;
                    case 71: from.AddToBackpack(new HiddenValleyGlobe()); break;
                    case 72: from.AddToBackpack(new GargoyleCityGlobe()); break;
                    case 73: from.AddToBackpack(new LightHouseGlobe()); break;
                    case 74: from.AddToBackpack(new SwampLandsGlobe()); break;
                    case 75: from.AddToBackpack(new DestardGlobe()); break;
                    case 76: from.AddToBackpack(new DecietGlobe()); break;
                    case 77: from.AddToBackpack(new DespiseGlobe()); break;
                    case 78: from.AddToBackpack(new HythlothGlobe()); break;
                    case 79: from.AddToBackpack(new ShameGlobe()); break;
                    case 80: from.AddToBackpack(new WrongGlobe()); break;
                    case 81: from.AddToBackpack(new FireGlobe()); break;
                    case 82: from.AddToBackpack(new IceGlobe()); break;
                    case 83: from.AddToBackpack(new DaemonTempleGlobe()); break;
                    case 84: from.AddToBackpack(new TerathonKeepGlobe()); break;
                    case 85: from.AddToBackpack(new CovetusGlobe()); break;
                    case 86: from.AddToBackpack(new SavageCampGlobe()); break;
                    case 87: from.AddToBackpack(new WispGlobe()); break;
                    case 88: from.AddToBackpack(new IlshenarGlobe()); break;
                    case 89: from.AddToBackpack(new TokunoGlobe()); break;
                    case 90: from.AddToBackpack(new TrammelLandsGlobe()); break;
                    case 91: from.AddToBackpack(new FeluccaLandsGlobe()); break;
                    case 92: from.AddToBackpack(new MalasGlobe()); break;
                    case 93: from.AddToBackpack(new PvpLandsGlobe()); break;
                    case 94: from.AddToBackpack(new PvmLandsGlobe()); break;
                    case 95: from.AddToBackpack(new NorthPoleGlobe()); break;
                    case 96: from.AddToBackpack(new SantasWorkShopGlobe()); break;
                    case 97: from.AddToBackpack(new StarRoomGlobe()); break;
                    case 98: from.AddToBackpack(new MiningMountiansGlobe()); break;
                    case 99: from.AddToBackpack(new YourShardGlobe()); break;
                   
                }
                this.Delete();
			}

		}

		public override void Serialize ( GenericWriter writer)
		{
			base.Serialize ( writer );

			writer.Write ( (int) 0);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize ( reader );

			int version = reader.ReadInt();
		}
	}
}