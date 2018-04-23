// Scripted by Jumpnjahosofat //

using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{

	public class RandomArtifactDeed : Item
	{

		[Constructable]
		public RandomArtifactDeed() : this( null )
		{
		}

		[Constructable]
		public RandomArtifactDeed ( string name ) : base ( 0x14F0 )
		{
			Name = "Random Artifact Deed";
			Hue = 1172;
		}

		public RandomArtifactDeed ( Serial serial ) : base ( serial )
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
                    case 0: from.AddToBackpack(new ArmorOfFortune()); break;
                    case 1: from.AddToBackpack(new GauntletsOfNobility()); break;
                    case 2: from.AddToBackpack(new HelmOfInsight()); break;
                    case 3: from.AddToBackpack(new HolyKnightsBreastplate()); break;
                    case 4: from.AddToBackpack(new InquisitorsResolution()); break;
                    case 5: from.AddToBackpack(new JackalsCollar()); break;
                    case 6: from.AddToBackpack(new LeggingsOfBane()); break;
                    case 7: from.AddToBackpack(new MidnightBracers()); break;
                    case 8: from.AddToBackpack(new OrnateCrownOfTheHarrower()); break;
                    case 9: from.AddToBackpack(new ShadowDancerLeggings()); break;
                    case 10: from.AddToBackpack(new TunicOfFire()); break;
                    case 11: from.AddToBackpack(new VoiceOfTheFallenKing()); break;
                    case 12: from.AddToBackpack(new BraceletOfHealth()); break;
                    case 13: from.AddToBackpack(new OrnamentOfTheMagician()); break;
                    case 14: from.AddToBackpack(new RingOfTheElements()); break;
                    case 15: from.AddToBackpack(new RingOfTheVile()); break;
                    case 16: from.AddToBackpack(new Aegis()); break;
                    case 17: from.AddToBackpack(new ArcaneShield()); break;
                    case 18: from.AddToBackpack(new AxeOfTheHeavens()); break;
                    case 19: from.AddToBackpack(new BladeOfInsanity()); break;
                    case 20: from.AddToBackpack(new BladeOfTheRighteous()); break;
                    case 21: from.AddToBackpack(new BoneCrusher()); break;
                    case 22: from.AddToBackpack(new BreathOfTheDead()); break;
                    case 23: from.AddToBackpack(new Frostbringer()); break;
                    case 24: from.AddToBackpack(new LegacyOfTheDreadLord()); break;
                    case 25: from.AddToBackpack(new SerpentsFang()); break;
                    case 26: from.AddToBackpack(new StaffOfTheMagi()); break;
                    case 27: from.AddToBackpack(new TheBeserkersMaul()); break;
                    case 28: from.AddToBackpack(new TheDragonSlayer()); break;
                    case 29: from.AddToBackpack(new TheDryadBow()); break;
                    case 30: from.AddToBackpack(new TheTaskmaster()); break;
                    case 31: from.AddToBackpack(new TitansHammer()); break;
                    case 32: from.AddToBackpack(new ZyronicClaw()); break;
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