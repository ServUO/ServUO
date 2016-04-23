using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis;
using Server.Targeting;

namespace Knives.TownHouses
{
	public class TownHouse : VersionHouse
	{
		private static ArrayList s_TownHouses = new ArrayList();
		public static ArrayList AllTownHouses{ get{ return s_TownHouses; } }

		private TownHouseSign c_Sign;
		private Item c_Hanger;
        private ArrayList c_Sectors = new ArrayList();

        public TownHouseSign ForSaleSign { get { return c_Sign; } }

		public Item Hanger
		{
			get
			{
				if ( c_Hanger == null )
				{
					c_Hanger = new Item( 0xB98 );
					c_Hanger.Movable = false;
					c_Hanger.Location = Sign.Location;
					c_Hanger.Map = Sign.Map;
				}

				return c_Hanger;
			}
			set{ c_Hanger = value; }
		}

		public TownHouse( Mobile m, TownHouseSign sign, int locks, int secures ) : base( 0x1DD6 | 0x4000, m, locks, secures )
		{
			c_Sign = sign;

            SetSign( 0, 0, 0 );

			s_TownHouses.Add( this );
		}

		public void InitSectorDefinition()
		{
            if (c_Sign == null || c_Sign.Blocks.Count == 0)
                return;

			int minX = ((Rectangle2D)c_Sign.Blocks[0]).Start.X;
			int minY = ((Rectangle2D)c_Sign.Blocks[0]).Start.Y;
			int maxX = ((Rectangle2D)c_Sign.Blocks[0]).End.X;
			int maxY = ((Rectangle2D)c_Sign.Blocks[0]).End.Y;

			foreach( Rectangle2D rect in c_Sign.Blocks )
			{
				if ( rect.Start.X < minX )
					minX = rect.Start.X;
				if ( rect.Start.Y < minY )
					minY = rect.Start.Y;
				if ( rect.End.X > maxX )
					maxX = rect.End.X;
				if ( rect.End.Y > maxY )
					maxY = rect.End.Y;
			}

            foreach (Sector sector in c_Sectors)
                sector.OnMultiLeave(this);

            c_Sectors.Clear();
            for (int x = minX; x < maxX; ++x)
                for (int y = minY; y < maxY; ++y)
                    if(!c_Sectors.Contains(Map.GetSector(new Point2D(x, y))))
                        c_Sectors.Add(Map.GetSector(new Point2D(x, y)));

            foreach (Sector sector in c_Sectors)
                sector.OnMultiEnter(this);

            Components.Resize(maxX - minX, maxY - minY);
            Components.Add(0x520, Components.Width - 1, Components.Height - 1, -5);
        }

        public override Rectangle2D[] Area
        {
            get
            {
                if (c_Sign == null)
                    return new Rectangle2D[100];

                Rectangle2D[] rects = new Rectangle2D[c_Sign.Blocks.Count];

                for (int i = 0; i < c_Sign.Blocks.Count && i < rects.Length; ++i)
                    rects[i] = (Rectangle2D)c_Sign.Blocks[i];

                return rects;
            }
        }

		public override bool IsInside( Point3D p, int height )
		{
            if (c_Sign == null)
                return false;

			if ( Map == null || Region == null )
			{
				Delete();
				return false;
			}

			Sector sector = null;

            try
            {
                if (c_Sign is RentalContract && Region.Contains(p))
                    return true;

                sector = Map.GetSector(p);

                foreach (BaseMulti m in sector.Multis)
                {
                    if (m != this
                    && m is TownHouse
                    && ((TownHouse)m).ForSaleSign is RentalContract
                    && ((TownHouse)m).IsInside(p, height))
                        return false;
                }

                return Region.Contains(p);
            }
            catch(Exception e)
            {
                Errors.Report("Error occured in IsInside().  More information on the console.");
                Console.WriteLine("Info:{0}, {1}, {2}", Map, sector, Region, sector != null ? "" + sector.Multis : "**");
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

		public override int GetNewVendorSystemMaxVendors()
		{
			return 50;
		}

		public override int GetAosMaxSecures()
		{
			return MaxSecures;
		}

		public override int GetAosMaxLockdowns()
		{
			return MaxLockDowns;
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			if ( c_Hanger != null )
				c_Hanger.Map = Map;
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			base.OnLocationChange( oldLocation );

			if ( c_Hanger != null )
				c_Hanger.Location = Sign.Location;
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( e.Mobile != Owner || !IsInside( e.Mobile ) )
				return;

            if (e.Speech.ToLower() == "check house rent")
                c_Sign.CheckRentTimer();

            Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(AfterSpeech), e.Mobile);
        }

        private void AfterSpeech(object o)
        {
            if (!(o is Mobile))
                return;

            if (((Mobile)o).Target is HouseBanTarget && ForSaleSign != null && ForSaleSign.NoBanning)
            {
                ((Mobile)o).Target.Cancel((Mobile)o, TargetCancelType.Canceled);
                ((Mobile)o).SendMessage(0x161, "You cannot ban people from this house.");
            }
        }

		public override void OnDelete()
		{
            if (c_Hanger != null)
                c_Hanger.Delete();

            foreach (Item item in Sign.GetItemsInRange(0))
                if (item != Sign)
                    item.Visible = true;

            c_Sign.ClearHouse();
            Doors.Clear();

            s_TownHouses.Remove(this);

            base.OnDelete();
		}

        public TownHouse(Serial serial)
            : base(serial)
		{
            s_TownHouses.Add(this);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 3 );

			// Version 2

			writer.Write( c_Hanger );

			// Version 1

			writer.Write( c_Sign );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version >= 2 )
				c_Hanger = reader.ReadItem();

			c_Sign = (TownHouseSign)reader.ReadItem();

            if (version <= 2)
            {
                int count = reader.ReadInt();
                for (int i = 0; i < count; ++i)
                    reader.ReadRect2D();
            }

			if( Price == 0 )
				Price = 1;

            ItemID = 0x1DD6 | 0x4000;
        }
	}
}