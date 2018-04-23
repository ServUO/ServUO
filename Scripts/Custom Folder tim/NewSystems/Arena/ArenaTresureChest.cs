using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xe41, 0xe40 )]
	public class ArenaTreasureChest : BaseTreasureChest
	{
		[Constructable]
		public ArenaTreasureChest() : base( 0xE41 )
		{
			Name = "Arena Champion";
			Hue = 1161;

			DropItem( new ChampionStatue() );
			
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );
			DropItem( new Gold( Utility.RandomMinMax( 1000, 2000 ) ) );

					{
							int chance = Utility.Random( 5 );

							switch ( chance )
							{
								case 0:
									{
										Map map = this.Map;
										BaseWeapon weapon = Loot.RandomWeapon();
										BaseRunicTool.ApplyAttributesTo( weapon, 4, 10, 50 );
										DropItem( weapon );
										break;
									}
								case 1:
									{
										Map map = this.Map;
										BaseArmor armor = Loot.RandomArmor();
										BaseRunicTool.ApplyAttributesTo( armor, 4, 10, 50 );
										DropItem( armor );
										break;
									}
								case 2:
									{
										BaseJewel jewel = Loot.RandomJewelry();
										BaseRunicTool.ApplyAttributesTo( jewel, 4, 10, 50 );
										DropItem( jewel );
										break;
									}
								case 3:
									{
										Map map = this.Map;
										BaseHat hat = Loot.RandomHat();
										BaseRunicTool.ApplyAttributesTo( hat, 4, 10, 50 );
										DropItem( hat );
										break;
									}
								case 4:
									{
										BaseWand wand = Loot.RandomWand();
										DropItem( wand );
										break;
									}
					}

			
			switch ( Utility.Random( 32 ) )
				{
					default:			
					case 0: DropItem ( new VoiceOfTheFallenKing() ); break;
					case 1: DropItem ( new TunicOfFire() ); break;
					case 2: DropItem ( new SpiritOfTheTotem() ); break;
					case 3: DropItem ( new ShadowDancerLeggings() ); break;
					case 4: DropItem ( new OrnateCrownOfTheHarrower() ); break;
					case 5: DropItem ( new LeggingsOfBane() ); break;
					case 6: DropItem ( new JackalsCollar() ); break;
					case 7: DropItem ( new InquisitorsResolution() ); break;
					case 8: DropItem ( new HuntersHeaddress() ); break;
					case 9: DropItem ( new HelmOfInsight() ); break;
					case 10: DropItem ( new HatOfTheMagi() ); break;
					case 11: DropItem ( new GauntletsOfNobility() ); break;
					case 12: DropItem ( new DivineCountenance() ); break;
					case 13: DropItem ( new ArmorOfFortune() ); break;
					case 14: DropItem ( new ArcaneShield() ); break;
					case 15: DropItem ( new Aegis() ); break;
					case 16: DropItem ( new ZyronicClaw() ); break;
					case 17: DropItem ( new TitansHammer() ); break;
					case 18: DropItem ( new TheTaskmaster() ); break;
					case 19: DropItem ( new TheDragonSlayer() ); break;
					case 20: DropItem ( new StaffOfTheMagi() ); break;
					case 21: DropItem ( new SerpentsFang() ); break;
					case 22: DropItem ( new LegacyOfTheDreadLord() ); break;
					case 23: DropItem ( new BreathOfTheDead() ); break;
					case 24: DropItem ( new BoneCrusher() ); break;
					case 25: DropItem ( new BladeOfTheRighteous() ); break;
					case 26: DropItem ( new BladeOfInsanity() ); break;
					case 27: DropItem ( new AxeOfTheHeavens() ); break;
					case 28: DropItem ( new RingOfTheVile() ); break;
					case 29: DropItem ( new RingOfTheElements() ); break;
					case 30: DropItem ( new OrnamentOfTheMagician() ); break;
					case 31: DropItem ( new BraceletOfHealth() ); break;
				
				}

				switch ( Utility.Random( 32 ) )
				{
					default:			
					case 0: DropItem ( new VoiceOfTheFallenKing() ); break;
					case 1: DropItem ( new TunicOfFire() ); break;
					case 2: DropItem ( new SpiritOfTheTotem() ); break;
					case 3: DropItem ( new ShadowDancerLeggings() ); break;
					case 4: DropItem ( new OrnateCrownOfTheHarrower() ); break;
					case 5: DropItem ( new LeggingsOfBane() ); break;
					case 6: DropItem ( new JackalsCollar() ); break;
					case 7: DropItem ( new InquisitorsResolution() ); break;
					case 8: DropItem ( new HuntersHeaddress() ); break;
					case 9: DropItem ( new HelmOfInsight() ); break;
					case 10: DropItem ( new HatOfTheMagi() ); break;
					case 11: DropItem ( new GauntletsOfNobility() ); break;
					case 12: DropItem ( new DivineCountenance() ); break;
					case 13: DropItem ( new ArmorOfFortune() ); break;
					case 14: DropItem ( new ArcaneShield() ); break;
					case 15: DropItem ( new Aegis() ); break;
					case 16: DropItem ( new ZyronicClaw() ); break;
					case 17: DropItem ( new TitansHammer() ); break;
					case 18: DropItem ( new TheTaskmaster() ); break;
					case 19: DropItem ( new TheDragonSlayer() ); break;
					case 20: DropItem ( new StaffOfTheMagi() ); break;
					case 21: DropItem ( new SerpentsFang() ); break;
					case 22: DropItem ( new LegacyOfTheDreadLord() ); break;
					case 23: DropItem ( new BreathOfTheDead() ); break;
					case 24: DropItem ( new BoneCrusher() ); break;
					case 25: DropItem ( new BladeOfTheRighteous() ); break;
					case 26: DropItem ( new BladeOfInsanity() ); break;
					case 27: DropItem ( new AxeOfTheHeavens() ); break;
					case 28: DropItem ( new RingOfTheVile() ); break;
					case 29: DropItem ( new RingOfTheElements() ); break;
					case 30: DropItem ( new OrnamentOfTheMagician() ); break;
					case 31: DropItem ( new BraceletOfHealth() ); break;
				
				}

				switch ( Utility.Random( 32 ) )
				{
					default:			
					case 0: DropItem ( new VoiceOfTheFallenKing() ); break;
					case 1: DropItem ( new TunicOfFire() ); break;
					case 2: DropItem ( new SpiritOfTheTotem() ); break;
					case 3: DropItem ( new ShadowDancerLeggings() ); break;
					case 4: DropItem ( new OrnateCrownOfTheHarrower() ); break;
					case 5: DropItem ( new LeggingsOfBane() ); break;
					case 6: DropItem ( new JackalsCollar() ); break;
					case 7: DropItem ( new InquisitorsResolution() ); break;
					case 8: DropItem ( new HuntersHeaddress() ); break;
					case 9: DropItem ( new HelmOfInsight() ); break;
					case 10: DropItem ( new HatOfTheMagi() ); break;
					case 11: DropItem ( new GauntletsOfNobility() ); break;
					case 12: DropItem ( new DivineCountenance() ); break;
					case 13: DropItem ( new ArmorOfFortune() ); break;
					case 14: DropItem ( new ArcaneShield() ); break;
					case 15: DropItem ( new Aegis() ); break;
					case 16: DropItem ( new ZyronicClaw() ); break;
					case 17: DropItem ( new TitansHammer() ); break;
					case 18: DropItem ( new TheTaskmaster() ); break;
					case 19: DropItem ( new TheDragonSlayer() ); break;
					case 20: DropItem ( new StaffOfTheMagi() ); break;
					case 21: DropItem ( new SerpentsFang() ); break;
					case 22: DropItem ( new LegacyOfTheDreadLord() ); break;
					case 23: DropItem ( new BreathOfTheDead() ); break;
					case 24: DropItem ( new BoneCrusher() ); break;
					case 25: DropItem ( new BladeOfTheRighteous() ); break;
					case 26: DropItem ( new BladeOfInsanity() ); break;
					case 27: DropItem ( new AxeOfTheHeavens() ); break;
					case 28: DropItem ( new RingOfTheVile() ); break;
					case 29: DropItem ( new RingOfTheElements() ); break;
					case 30: DropItem ( new OrnamentOfTheMagician() ); break;
					case 31: DropItem ( new BraceletOfHealth() ); break;
				
				}
			}
		}	

		public ArenaTreasureChest( Serial serial ) : base( serial )
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
}