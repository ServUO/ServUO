using System;
using System.IO;
using Server.Gumps;
using Server.Network;

namespace Server.ACC
{
	public abstract class ACCSystem
	{
		public abstract string Name();
		public abstract void Save( GenericWriter idx, GenericWriter tdb, GenericWriter writer );
		public abstract void Load( BinaryReader idx, BinaryReader tdb, BinaryFileReader reader );
		public abstract void Gump( Mobile from, Gump gump, ACCGumpParams subParams );
		public abstract void Help( Mobile from, Gump gump );
		public abstract void OnResponse( NetState state, RelayInfo info, ACCGumpParams subParams );
		public abstract void Enable();
		public abstract void Disable();

		public bool Enabled{ get{ return ACC.SysEnabled( this.ToString() ); } }

		public void StartSave( string path )
		{
			path += Name()+"/";

			if( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );

			try
			{
				GenericWriter idx = new BinaryFileWriter( path+Name()+".idx", false );
				GenericWriter tdb = new BinaryFileWriter( path+Name()+".tdb", false );
				GenericWriter bin = new BinaryFileWriter( path+Name()+".bin", true );

				Console.Write( " - Saving {0}...", Name() );
				Save( idx, tdb, bin );

				idx.Close();
				tdb.Close();
				bin.Close();

				Console.WriteLine( "Done." );
			}
			catch( Exception err )
			{
				Console.WriteLine( "Failed. Exception: "+err );
			}
		}

		public void StartLoad( string path )
		{
			path += Name()+"/";

			string idxPath = path+Name()+".idx";
			string tdbPath = path+Name()+".tdb";
			string binPath = path+Name()+".bin";

			if( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );

			if( File.Exists( idxPath ) && File.Exists( tdbPath ) && File.Exists( binPath ) )
			{
				using( FileStream idx = new FileStream( idxPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
				{
					BinaryReader idxReader = new BinaryReader( idx );

					using( FileStream tdb = new FileStream( tdbPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
					{
						BinaryReader tdbReader = new BinaryReader( tdb );

						using( FileStream bin = new FileStream( binPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
						{
							BinaryFileReader binReader = new BinaryFileReader( new BinaryReader( bin ) );

							Console.Write( " - Loading {0}", Name() );
							Load( idxReader, tdbReader, binReader );

							idxReader.Close();
							tdbReader.Close();
							binReader.Close();

							bin.Close();
							tdb.Close();
							idx.Close();

							Console.WriteLine( "   - Done." );
						}
					}
				}
			}
		}
	}
}