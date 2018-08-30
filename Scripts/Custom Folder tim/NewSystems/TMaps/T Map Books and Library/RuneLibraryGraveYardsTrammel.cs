#region AuthorHeader
//  
//	Runebook Library Changed by Partystuffcloseouts/Soultaker
//  GraveYards Ver. 1.0 
//  
//  Based on Runebook T-Hunting Files Original Ideas and code by // A_Li_N // Senior Member
//  
#endregion AuthorHeader
using System;
using System.IO;
using System.Collections;
using Server.Commands;
using Server;
using Server.Items;

namespace Server.Items
{
	public class RuneLibraryGraveYardsTrammel : Item
	{
		private static string pathlist = "Data/RuneLibraryGraveYards.txt";
		private static string entry = "# Name X Y Z";
		private static string[] mapNums;
		private static string[] mapNames;
		private static string[] xs;
		private static string[] ys;
		private static string[] zs;
		private static int size = 0;

		private static ArrayList library;

      public static void Initialize()
      {
         CommandSystem.Register( "RuneLibraryGraveYardsTrammel", AccessLevel.Administrator, new CommandEventHandler( RuneLibraryGraveYardsTrammel_OnCommand ) );
      }

		public static void RuneLibraryGraveYardsTrammel_OnCommand( CommandEventArgs args )
		{
			Mobile m = args.Mobile;
         RuneLibraryGraveYardsTrammel rl = new RuneLibraryGraveYardsTrammel(m);
		}

		private static void readLine()
		{
			if( File.Exists( pathlist ) )
			{
				size = 0;
				string nums = "";
				string name = "";
				string x = "";
				string y = "";
				string z = "";

				StreamReader f = new StreamReader( pathlist );
				while( (entry = f.ReadLine()) != null )
				{
					string[] parts = null;
					parts = entry.Split();

					nums += parts[0]+" ";
					name += parts[1]+" ";
					x += parts[2]+" ";
					y += parts[3]+" ";
					z += parts[4]+" ";
					size++;
				}
				f.Close();

				mapNums = nums.Split();
				mapNames = name.Split();
				xs = x.Split();
				ys = y.Split();
				zs = z.Split();
			}
		}

		[Constructable]
		public RuneLibraryGraveYardsTrammel (Mobile from)
		{
			library = new ArrayList();

			readLine();
			Runebook rb = new Runebook(0);
			int nameStart = 1;
			int nameEnd = 1;
			for( int i=0; i<size; i++ )
			{
				if( rb.Entries.Count == 16 )
				{
					rb.Name = "GraveYards Trammel " + nameStart + " - " + (nameEnd-1);
					library.Add(rb);
					rb = new Runebook(0);
					nameStart = nameEnd;
				}
				int x = int.Parse(xs[i]);
				int y = int.Parse(ys[i]);
				int z = int.Parse(zs[i]);
				Point3D targ = new Point3D(x, y, z);
				RecallRune rr = new RecallRune();
				rr.Target = targ;
				rr.TargetMap = Map.Trammel;
				rr.Description = mapNums[i] + " " + mapNames[i];
				rr.House = null;
				rr.Marked = true;		
				rb.OnDragDrop(from, rr );
				nameEnd++;
			}
			rb.Name = "GraveYards Trammel " + nameStart + " - " + (nameEnd-1);
			library.Add(rb);

			int height = 6;
			int offx;
			int offy;
			int offz;
			for(int p=0; p<library.Count; p++)
			{
				Runebook librarybook = (Runebook)library[p];
				librarybook.Movable = false;
				librarybook.MaxCharges = 12;
				librarybook.CurCharges = 12;
				if(p < 4)
				{
					offx = from.Location.X-1;
					offy = from.Location.Y-1;
					offz = from.Location.Z+height;
				}
				else if(p >= 4 && p < 5)
				{
					offx = from.Location.X;
					offy = from.Location.Y-1;
					offz = from.Location.Z+height+2;
					height += 2;
				}
				else if(p >= 5 && p < 9)
				{
					offx = from.Location.X;
					offy = from.Location.Y-1;
					offz = from.Location.Z+height;
				}
				else
				{
					offx = from.Location.X+1;
					offy = from.Location.Y-1;
					offz = from.Location.Z+height;
				}
				Point3D loc = new Point3D(offx, offy, offz);
				librarybook.MoveToWorld(loc, from.Map);
				if( height == 0 )
					height = 8;
				height -= 2;
			}
		}

		public RuneLibraryGraveYardsTrammel( Serial serial ) : base( serial )
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
