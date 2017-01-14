using Server;
using System;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;

namespace Server.Gumps
{
	public class CompassDirectionGump : Gump
	{
		private Mobile m_From;
		
		public CompassDirectionGump(Mobile from) : base(120, 50)
		{
			m_From = from;
            List<Point3D> pointList = Server.Regions.MazeOfDeathRegion.Path;
			
			Point3D cur = m_From.Location;
			Point3D northLoc = new Point3D(cur.X, cur.Y - 1, cur.Z);
			Point3D eastLoc = new Point3D(cur.X + 1, cur.Y, cur.Z);
			Point3D southLoc = new Point3D(cur.X, cur.Y + 1, cur.Z);
			Point3D westLoc = new Point3D(cur.X - 1, cur.Y, cur.Z);
			
			//this.Closable = false;
			
			//Empty radar
			AddImage(0, 0, 9007);
			
			//Arrows
			if(pointList.Contains(northLoc))
				AddImage(100, 50, 4501);
			
			if(pointList.Contains(eastLoc))
				AddImage(100, 100, 4503);

			if(pointList.Contains(southLoc))
				AddImage(50, 100, 4505);
				
			if(pointList.Contains(westLoc))
				AddImage(50, 50, 4507);
		}
	}
}