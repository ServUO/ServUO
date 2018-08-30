using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	public class ArenaDecor : BaseAddon
	{
		public override BaseAddonDeed Deed
		{
			get
			{
				return new ArenaDecorDeed();
			}
		}

		[ Constructable ]
		public ArenaDecor()
		{
						
			Item gt1 = new ArenaGateNS();
			gt1.MoveToWorld( new Point3D( 2367, 1151, -90 ), Map.Malas );
			
			Item gt2 = new ArenaGateNS();
			gt2.MoveToWorld( new Point3D( 2368, 1151, -90 ), Map.Malas );
			
			Item gt4 = new ArenaGateNS();
			gt4.MoveToWorld( new Point3D( 2367, 1103, -90 ), Map.Malas );

			Item gt5 = new ArenaGateNS();
			gt5.MoveToWorld( new Point3D( 2368, 1103, -90 ), Map.Malas );
			
			Item gt6 = new ArenaGateNS();
			gt6.MoveToWorld( new Point3D( 2349, 1130, -90 ), Map.Malas );

			Item gt7 = new ArenaGateNS();
			gt7.MoveToWorld( new Point3D( 2348, 1130, -90 ), Map.Malas );
			
			Item gt8 = new ArenaGateNS();
			gt8.MoveToWorld( new Point3D( 2347, 1130, -90 ), Map.Malas );

			Item gt9 = new ArenaGateNS();
			gt9.MoveToWorld( new Point3D( 2346, 1130, -90 ), Map.Malas );
			
			Item gt10 = new ArenaGateNS();
			gt10.MoveToWorld( new Point3D( 2345, 1130, -90 ), Map.Malas );

			Item gt11 = new ArenaGateNS();
			gt11.MoveToWorld( new Point3D( 2344, 1130, -90 ), Map.Malas );
			
			Item gt12 = new ArenaGateNS();
			gt12.MoveToWorld( new Point3D( 2349, 1124, -90 ), Map.Malas );

			Item gt13 = new ArenaGateNS();
			gt13.MoveToWorld( new Point3D( 2348, 1124, -90 ), Map.Malas );
			
			Item gt14 = new ArenaGateNS();
			gt14.MoveToWorld( new Point3D( 2347, 1124, -90 ), Map.Malas );

			Item gt15 = new ArenaGateNS();
			gt15.MoveToWorld( new Point3D( 2346, 1124, -90 ), Map.Malas );
			
			Item gt16 = new ArenaGateNS();
			gt16.MoveToWorld( new Point3D( 2345, 1124, -90 ), Map.Malas );

			Item gt17 = new ArenaGateNS();
			gt17.MoveToWorld( new Point3D( 2344, 1124, -90 ), Map.Malas );

			Item gt18 = new ArenaGateEW();
			gt18.MoveToWorld( new Point3D( 2343, 1125, -90 ), Map.Malas );

			Item gt19 = new ArenaGateEW();
			gt19.MoveToWorld( new Point3D( 2343, 1126, -90 ), Map.Malas );

			Item gt20 = new ArenaGateEW();
			gt20.MoveToWorld( new Point3D( 2343, 1127, -90 ), Map.Malas );

			Item gt21 = new ArenaGateEW();
			gt21.MoveToWorld( new Point3D( 2343, 1128, -90 ), Map.Malas );

			Item gt22 = new ArenaGateEW();
			gt22.MoveToWorld( new Point3D( 2343, 1129, -90 ), Map.Malas );

			Item gt23 = new ArenaGateEW();
			gt23.MoveToWorld( new Point3D( 2343, 1130, -90 ), Map.Malas );

			Item pc1 = new ArenaPortcullisNS();
			pc1.MoveToWorld( new Point3D( 2367, 1112, -90 ), Map.Malas );

			Item pc2 = new ArenaPortcullisNS();
			pc2.MoveToWorld( new Point3D( 2368, 1112, -90 ), Map.Malas );

			Item pc3 = new ArenaPortcullisNS();
			pc3.MoveToWorld( new Point3D( 2367, 1142, -90 ), Map.Malas );

			Item pc4 = new ArenaPortcullisNS();
			pc4.MoveToWorld( new Point3D( 2368, 1142, -90 ), Map.Malas );

			Item pc5 = new ArenaPortcullisEW();
			pc5.MoveToWorld( new Point3D( 2350, 1128, -90 ), Map.Malas );

			Item pc6 = new ArenaPortcullisEW();
			pc6.MoveToWorld( new Point3D( 2350, 1127, -90 ), Map.Malas );

			Item mg1 = new LifeGate();
			mg1.MoveToWorld( new Point3D( 2352, 1120, -90 ), Map.Malas );

			Item mg2 = new LifeGate();
			mg2.MoveToWorld( new Point3D( 2352, 1135, -90 ), Map.Malas );

			Item mg3 = new LifeGate();
			mg3.MoveToWorld( new Point3D( 2382, 1135, -90 ), Map.Malas );

			Item mg4 = new LifeGate();
			mg4.MoveToWorld( new Point3D( 2382, 1120, -90 ), Map.Malas );

			Item abs = new ArenaBankStone();
			abs.MoveToWorld( new Point3D( 2347, 1125, -90 ), Map.Malas );

			Item axm = new ArenaExitMoongate();
			axm.MoveToWorld( new Point3D( 2344, 1127, -90 ), Map.Malas );
			
			Mobile mage1 = new ArenaMob();
			mage1.Frozen = true;
			mage1.MoveToWorld( new Point3D( 2347, 1132, -69 ), Map.Malas );

			Mobile carpenter1 = new ArenaMob();
			carpenter1.Frozen = true;
			carpenter1.MoveToWorld( new Point3D( 2345, 1132, -69 ), Map.Malas );

			Mobile jeweler1 = new ArenaMob();
			jeweler1.Frozen = true;
			jeweler1.MoveToWorld( new Point3D( 2343, 1132, -69 ), Map.Malas );

			Mobile bard1 = new ArenaMob();
			bard1.Frozen = true;
			bard1.MoveToWorld( new Point3D( 2347, 1130, -69 ), Map.Malas );

			Mobile furtrader1 = new ArenaMob();
			furtrader1.Frozen = true;
			furtrader1.MoveToWorld( new Point3D( 2345, 1130, -69 ), Map.Malas );

			Mobile miner1 = new ArenaMob();
			miner1.Frozen = true;
			miner1.MoveToWorld( new Point3D( 2343, 1130, -69 ), Map.Malas );

			Mobile mapmaker1 = new ArenaMob();
			mapmaker1.Frozen = true;
			mapmaker1.MoveToWorld( new Point3D( 2347, 1125, -69 ), Map.Malas );

			Mobile rancher1 = new ArenaMob();
			rancher1.Frozen = true;
			rancher1.MoveToWorld( new Point3D( 2345, 1125, -69 ), Map.Malas );

			Mobile tailor1 = new ArenaMob();
			tailor1.Frozen = true;
			tailor1.MoveToWorld( new Point3D( 2343, 1125, -69 ), Map.Malas );

			Mobile tinker1 = new ArenaMob();
			tinker1.Frozen = true;
			tinker1.MoveToWorld( new Point3D( 2347, 1123, -69 ), Map.Malas );

			Mobile ranger1 = new ArenaMob();
			ranger1.Frozen = true;
			ranger1.MoveToWorld( new Point3D( 2345, 1123, -69 ), Map.Malas );

			Mobile scribe1 = new ArenaMob();
			scribe1.Frozen = true;
			scribe1.MoveToWorld( new Point3D( 2343, 1123, -69 ), Map.Malas );

			Mobile mage2 = new ArenaMob();
			mage2.Frozen = true;
			mage2.MoveToWorld( new Point3D( 2360, 1107, -68 ), Map.Malas );
			mage2.Direction = Direction.East;

			Mobile carpenter2 = new ArenaMob();
			carpenter2.Frozen = true;
			carpenter2.MoveToWorld( new Point3D( 2361, 1107, -68 ), Map.Malas );
			carpenter2.Direction = Direction.East;

			Mobile jeweler2 = new ArenaMob();
			jeweler2.Frozen = true;
			jeweler2.MoveToWorld( new Point3D( 2362, 1107, -68 ), Map.Malas );
			jeweler2.Direction = Direction.East;

			Mobile bard2 = new ArenaMob();
			bard2.Frozen = true;
			bard2.MoveToWorld( new Point3D( 2363, 1107, -68 ), Map.Malas );
			bard2.Direction = Direction.East;

			Mobile furtrader2 = new ArenaMob();
			furtrader2.Frozen = true;
			furtrader2.MoveToWorld( new Point3D( 2364, 1107, -68 ), Map.Malas );
			furtrader2.Direction = Direction.East;

			Mobile mage3 = new ArenaMob();
			mage3.Frozen = true;
			mage3.MoveToWorld( new Point3D( 2367, 1107, -68 ), Map.Malas );
			mage3.Direction = Direction.East;

			Mobile carpenter3 = new ArenaMob();
			carpenter3.Frozen = true;
			carpenter3.MoveToWorld( new Point3D( 2368, 1107, -68 ), Map.Malas );
			carpenter3.Direction = Direction.East;

			Mobile jeweler3 = new ArenaMob();
			jeweler3.Frozen = true;
			jeweler3.MoveToWorld( new Point3D( 2369, 1107, -68 ), Map.Malas );
			jeweler3.Direction = Direction.East;

			Mobile bard3 = new ArenaMob();
			bard3.Frozen = true;
			bard3.MoveToWorld( new Point3D( 2370, 1107, -68 ), Map.Malas );
			bard3.Direction = Direction.East;

			Mobile furtrader3 = new ArenaMob();
			furtrader3.Frozen = true;
			furtrader3.MoveToWorld( new Point3D( 2371, 1107, -68 ), Map.Malas );
			furtrader3.Direction = Direction.East;

			Mobile mage4 = new ArenaMob();
			mage4.Frozen = true;
			mage4.MoveToWorld( new Point3D( 2360, 1105, -68 ), Map.Malas );
			mage4.Direction = Direction.East;

			Mobile carpenter4 = new ArenaMob();
			carpenter4.Frozen = true;
			carpenter4.MoveToWorld( new Point3D( 2361, 1105, -68 ), Map.Malas );
			carpenter4.Direction = Direction.East;

			Mobile jeweler4 = new ArenaMob();
			jeweler4.Frozen = true;
			jeweler4.MoveToWorld( new Point3D( 2362, 1105, -68 ), Map.Malas );
			jeweler4.Direction = Direction.East;

			Mobile bard4 = new ArenaMob();
			bard4.Frozen = true;
			bard4.MoveToWorld( new Point3D( 2363, 1105, -68 ), Map.Malas );
			bard4.Direction = Direction.East;

			Mobile furtrader4 = new ArenaMob();
			furtrader4.Frozen = true;
			furtrader4.MoveToWorld( new Point3D( 2364, 1105, -68 ), Map.Malas );
			furtrader4.Direction = Direction.East;

			Mobile mage5 = new ArenaMob();
			mage5.Frozen = true;
			mage5.MoveToWorld( new Point3D( 2367, 1105, -68 ), Map.Malas );
			mage5.Direction = Direction.East;

			Mobile carpenter5 = new ArenaMob();
			carpenter5.Frozen = true;
			carpenter5.MoveToWorld( new Point3D( 2368, 1105, -68 ), Map.Malas );
			carpenter5.Direction = Direction.East;

			Mobile jeweler5 = new ArenaMob();
			jeweler5.Frozen = true;
			jeweler5.MoveToWorld( new Point3D( 2369, 1105, -68 ), Map.Malas );
			jeweler5.Direction = Direction.East;

			Mobile bard5 = new ArenaMob();
			bard5.Frozen = true;
			bard5.MoveToWorld( new Point3D( 2370, 1105, -68 ), Map.Malas );
			bard5.Direction = Direction.East;

			Mobile furtrader5 = new ArenaMob();
			furtrader5.Frozen = true;
			furtrader5.MoveToWorld( new Point3D( 2371, 1105, -68 ), Map.Malas );
			furtrader5.Direction = Direction.East;

			Mobile mage6 = new ArenaMob();
			mage6.Frozen = true;
			mage6.MoveToWorld( new Point3D( 2360, 1103, -68 ), Map.Malas );
			mage6.Direction = Direction.East;

			Mobile carpenter6 = new ArenaMob();
			carpenter6.Frozen = true;
			carpenter6.MoveToWorld( new Point3D( 2361, 1103, -68 ), Map.Malas );
			carpenter6.Direction = Direction.East;

			Mobile jeweler6 = new ArenaMob();
			jeweler6.Frozen = true;
			jeweler6.MoveToWorld( new Point3D( 2362, 1103, -68 ), Map.Malas );
			jeweler6.Direction = Direction.East;

			Mobile bard6 = new ArenaMob();
			bard6.Frozen = true;
			bard6.MoveToWorld( new Point3D( 2363, 1103, -68 ), Map.Malas );
			bard6.Direction = Direction.East;

			Mobile furtrader6 = new ArenaMob();
			furtrader6.Frozen = true;
			furtrader6.MoveToWorld( new Point3D( 2364, 1103, -68 ), Map.Malas );
			furtrader6.Direction = Direction.East;

			Mobile mage7 = new ArenaMob();
			mage7.Frozen = true;
			mage7.MoveToWorld( new Point3D( 2367, 1103, -68 ), Map.Malas );
			mage7.Direction = Direction.East;

			Mobile carpenter7 = new ArenaMob();
			carpenter7.Frozen = true;
			carpenter7.MoveToWorld( new Point3D( 2368, 1103, -68 ), Map.Malas );
			carpenter7.Direction = Direction.East;

			Mobile jeweler7 = new ArenaMob();
			jeweler7.Frozen = true;
			jeweler7.MoveToWorld( new Point3D( 2369, 1103, -68 ), Map.Malas );
			jeweler7.Direction = Direction.East;

			Mobile bard7 = new ArenaMob();
			bard7.Frozen = true;
			bard7.MoveToWorld( new Point3D( 2370, 1103, -68 ), Map.Malas );
			bard7.Direction = Direction.East;

			Mobile furtrader7 = new ArenaMob();
			furtrader7.Frozen = true;
			furtrader7.MoveToWorld( new Point3D( 2371, 1103, -68 ), Map.Malas );
			furtrader7.Direction = Direction.East;

			Mobile mage8 = new ArenaMob();
			mage8.Frozen = true;
			mage8.MoveToWorld( new Point3D( 2360, 1148, -68 ), Map.Malas );
			mage8.Direction = Direction.West;

			Mobile carpenter8 = new ArenaMob();
			carpenter8.Frozen = true;
			carpenter8.MoveToWorld( new Point3D( 2361, 1148, -68 ), Map.Malas );
			carpenter8.Direction = Direction.West;

			Mobile jeweler8 = new ArenaMob();
			jeweler8.Frozen = true;
			jeweler8.MoveToWorld( new Point3D( 2362, 1148, -68 ), Map.Malas );
			jeweler8.Direction = Direction.West;

			Mobile bard8 = new ArenaMob();
			bard8.Frozen = true;
			bard8.MoveToWorld( new Point3D( 2363, 1148, -68 ), Map.Malas );
			bard8.Direction = Direction.West;

			Mobile furtrader8 = new ArenaMob();
			furtrader8.Frozen = true;
			furtrader8.MoveToWorld( new Point3D( 2364, 1148, -68 ), Map.Malas );
			furtrader8.Direction = Direction.West;

			Mobile mage9 = new ArenaMob();
			mage9.Frozen = true;
			mage9.MoveToWorld( new Point3D( 2367, 1148, -68 ), Map.Malas );
			mage9.Direction = Direction.West;

			Mobile carpenter9 = new ArenaMob();
			carpenter9.Frozen = true;
			carpenter9.MoveToWorld( new Point3D( 2368, 1148, -68 ), Map.Malas );
			carpenter9.Direction = Direction.West;

			Mobile jeweler9 = new ArenaMob();
			jeweler9.Frozen = true;
			jeweler9.MoveToWorld( new Point3D( 2369, 1148, -68 ), Map.Malas );
			jeweler9.Direction = Direction.West;

			Mobile bard9 = new ArenaMob();
			bard9.Frozen = true;
			bard9.MoveToWorld( new Point3D( 2370, 1148, -68 ), Map.Malas );
			bard9.Direction = Direction.West;

			Mobile furtrader9 = new ArenaMob();
			furtrader9.Frozen = true;
			furtrader9.MoveToWorld( new Point3D( 2371, 1148, -68 ), Map.Malas );
			furtrader9.Direction = Direction.West;

			Mobile mage10 = new ArenaMob();
			mage10.Frozen = true;
			mage10.MoveToWorld( new Point3D( 2360, 1150, -68 ), Map.Malas );
			mage10.Direction = Direction.West;

			Mobile carpenter10 = new ArenaMob();
			carpenter10.Frozen = true;
			carpenter10.MoveToWorld( new Point3D( 2361, 1150, -68 ), Map.Malas );
			carpenter10.Direction = Direction.West;

			Mobile jeweler10 = new ArenaMob();
			jeweler10.Frozen = true;
			jeweler10.MoveToWorld( new Point3D( 2362, 1150, -68 ), Map.Malas );
			jeweler10.Direction = Direction.West;

			Mobile bard10 = new ArenaMob();
			bard10.Frozen = true;
			bard10.MoveToWorld( new Point3D( 2363, 1150, -68 ), Map.Malas );
			bard10.Direction = Direction.West;

			Mobile furtrader10 = new ArenaMob();
			furtrader10.Frozen = true;
			furtrader10.MoveToWorld( new Point3D( 2364, 1150, -68 ), Map.Malas );
			furtrader10.Direction = Direction.West;

			Mobile mage11 = new ArenaMob();
			mage11.Frozen = true;
			mage11.MoveToWorld( new Point3D( 2367, 1150, -68 ), Map.Malas );
			mage11.Direction = Direction.West;

			Mobile carpenter11 = new ArenaMob();
			carpenter11.Frozen = true;
			carpenter11.MoveToWorld( new Point3D( 2368, 1150, -68 ), Map.Malas );
			carpenter11.Direction = Direction.West;

			Mobile jeweler11 = new ArenaMob();
			jeweler11.Frozen = true;
			jeweler11.MoveToWorld( new Point3D( 2369, 1150, -68 ), Map.Malas );
			jeweler11.Direction = Direction.West;

			Mobile bard11 = new ArenaMob();
			bard11.Frozen = true;
			bard11.MoveToWorld( new Point3D( 2370, 1150, -68 ), Map.Malas );
			bard11.Direction = Direction.West;

			Mobile furtrader11 = new ArenaMob();
			furtrader11.Frozen = true;
			furtrader11.MoveToWorld( new Point3D( 2371, 1150, -68 ), Map.Malas );
			furtrader11.Direction = Direction.West;

			Mobile mage12 = new ArenaMob();
			mage12.Frozen = true;
			mage12.MoveToWorld( new Point3D( 2360, 1152, -68 ), Map.Malas );
			mage12.Direction = Direction.West;

			Mobile carpenter12 = new ArenaMob();
			carpenter12.Frozen = true;
			carpenter12.MoveToWorld( new Point3D( 2361, 1152, -68 ), Map.Malas );
			carpenter12.Direction = Direction.West;

			Mobile jeweler12 = new ArenaMob();
			jeweler12.Frozen = true;
			jeweler12.MoveToWorld( new Point3D( 2362, 1152, -68 ), Map.Malas );
			jeweler12.Direction = Direction.West;

			Mobile bard12 = new ArenaMob();
			bard12.Frozen = true;
			bard12.MoveToWorld( new Point3D( 2363, 1152, -68 ), Map.Malas );
			bard12.Direction = Direction.West;

			Mobile furtrader12 = new ArenaMob();
			furtrader12.Frozen = true;
			furtrader12.MoveToWorld( new Point3D( 2364, 1152, -68 ), Map.Malas );
			furtrader12.Direction = Direction.West;

			Mobile mage13 = new ArenaMob();
			mage13.Frozen = true;
			mage13.MoveToWorld( new Point3D( 2367, 1152, -68 ), Map.Malas );
			mage13.Direction = Direction.West;

			Mobile carpenter13 = new ArenaMob();
			carpenter13.Frozen = true;
			carpenter13.MoveToWorld( new Point3D( 2368, 1152, -68 ), Map.Malas );
			carpenter13.Direction = Direction.West;

			Mobile jeweler13 = new ArenaMob();
			jeweler13.Frozen = true;
			jeweler13.MoveToWorld( new Point3D( 2369, 1152, -68 ), Map.Malas );
			jeweler13.Direction = Direction.West;

			Mobile bard13 = new ArenaMob();
			bard13.Frozen = true;
			bard13.MoveToWorld( new Point3D( 2370, 1152, -68 ), Map.Malas );
			bard13.Direction = Direction.West;

			Mobile furtrader13 = new ArenaMob();
			furtrader13.Frozen = true;
			furtrader13.MoveToWorld( new Point3D( 2371, 1152, -68 ), Map.Malas );
			furtrader13.Direction = Direction.West;

			Mobile lb = new LordBritish();
			lb.Frozen = true;
			lb.MoveToWorld( new Point3D( 2347, 1127, -68 ), Map.Malas );

			Mobile arenaminotaur = new ArenaMinotaur();
			arenaminotaur.MoveToWorld( new Point3D( 2356, 1127, -90 ), Map.Malas );
			
		
		}

		public ArenaDecor( Serial serial ) : base( serial )
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

	public class ArenaDecorDeed : BaseAddonDeed
	{
		public override BaseAddon Addon
		{
			get
			{
				return new ArenaDecor();
			}
		}

		[Constructable]
		public ArenaDecorDeed()
		{
			Name = "Arena Decor";
		}

		public ArenaDecorDeed( Serial serial ) : base( serial )
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