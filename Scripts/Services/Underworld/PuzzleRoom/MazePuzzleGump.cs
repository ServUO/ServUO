using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

namespace Server.Gumps
{
	public class MazePuzzleGump : Gump
	{
		private MazePuzzleItem m_Item;
		private List<int> m_Path;
		private List<int> m_Progress;

        public MazePuzzleGump(Mobile from, MazePuzzleItem item, List<int> path, List<int> progress)
            : this(from, item, path, progress, false)
        {
        }

		public MazePuzzleGump(Mobile from, MazePuzzleItem item, List<int> path, List<int> progress, bool showNext) : base(25, 25)
		{
			m_Item = item;
			m_Path = path;
			m_Progress = progress;
			
			AddBackground(50, 50, 550, 440, 2600);
			AddBackground(100, 75, 450, 90, 2600);
            AddBackground(90, 175, 270, 270, 2600);
            AddBackground(100, 185, 250, 250, 5120);
            AddBackground(370, 175, 178, 200, 5120);

            AddImage(145, 95, 10451);
            AddImage(435, 95, 10451);
            AddImage(0, 50, 10400);
            AddImage(0, 200, 10401);
            AddImage(0, 360, 10402);

            AddImage(565, 50, 10410);
            AddImage(565, 200, 10411);
            AddImage(565, 360, 10412);

            AddImage(370, 175, 10452);
            AddImage(370, 360, 10452);

            AddImageTiled(125, 207, 195, 3, 5031);
            AddImageTiled(125, 247, 195, 3, 5031);
            AddImageTiled(125, 287, 195, 3, 5031);
            AddImageTiled(125, 327, 195, 3, 5031);
            AddImageTiled(125, 367, 195, 3, 5031);
            AddImageTiled(125, 407, 195, 3, 5031);

            AddImageTiled(123, 205, 3, 195, 5031);
            AddImageTiled(163, 205, 3, 195, 5031);
            AddImageTiled(203, 205, 3, 195, 5031);
            AddImageTiled(243, 205, 3, 195, 5031);
            AddImageTiled(283, 205, 3, 195, 5031);
            AddImageTiled(323, 205, 3, 195, 5031);

            AddImage(420, 250, 1417);
            AddImage(440, 270, 2642);

            AddHtmlLocalized(220, 90, 210, 16, 1153747, false, false); // <center>GENERATOR CONTROL PANEL</center>
            AddHtmlLocalized(220, 115, 210, 16, 1153748, false, false); // <center>Use the Directional Controls to</center>
            AddHtmlLocalized(220, 131, 210, 16, 1153749, false, false); // <center>Close the Grid Circuit</center>

			if(m_Path == null)
			{
				m_Path = GetRandomPath();
				m_Item.Path = m_Path;
			}
				
			if(m_Progress == null || m_Progress.Count == 0)
			{
				m_Progress = new List<int>();
				m_Progress.Add(0);
			}
				
			int x = 110;
			int y = 195;
			int xOffset = 0;
			int yOffset = 0;
			
			for(int i = 0; i < 36; i++)
			{
				int itemID = m_Progress.Contains(i) ? 2152 : 9720;
				
				if(i < 6)
				{
					xOffset =  x + (40 * i);
					yOffset = y;
				}
				
				else if (i < 12)
				{
					xOffset =  x + (40 * (i - 6));
					yOffset = y + 40;
				}
				else if (i < 18)
				{
					xOffset = x + (40 * (i - 12));
					yOffset = y + 80;
				}
				else if (i < 24)
				{
					xOffset =  x + (40 * (i - 18));
					yOffset = y + 120;
				}
				else if (i < 30)
				{
					xOffset =  x + (40 * (i - 24));
					yOffset = y + 160;
				}
				else if (i < 36)
				{
					xOffset =  x + (40 * (i - 30));
					yOffset = y + 200;
				}
				
				AddImage(xOffset, yOffset, itemID);
				
				if(i == m_Progress[m_Progress.Count - 1])
					AddImage(xOffset + 8, yOffset + 8, 5032); 

                if(showNext && m_Progress.Count <= m_Path.Count && i == m_Path[m_Progress.Count])
                    AddImage(xOffset + 8, yOffset + 8, 2361); 

			}
			
			if(from.Skills[SkillName.Lockpicking].Base >= 100)
			{
                AddHtmlLocalized(410, 415, 150, 32, 1153750, false, false); // Attempt to Decipher the Circuit Path
				AddButton(370, 415, 4005, 4005, 5, GumpButtonType.Reply, 0);
			}
			
			AddButton(453, 245, 10700, 10701, 1, GumpButtonType.Reply, 0); // up
            AddButton(478, 281, 10710, 10711, 2, GumpButtonType.Reply, 0); // right
            AddButton(453, 305, 10720, 10721, 3, GumpButtonType.Reply, 0); // down 
            AddButton(413, 281, 10730, 10731, 4, GumpButtonType.Reply, 0); // left
			
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			
			if(info.ButtonID > 0 && info.ButtonID < 5)
			{
				int id = info.ButtonID;
				int current = m_Progress[m_Progress.Count - 1];
				int next = 35;
				int pick;
				
				if(m_Progress.Count >= 0 && m_Progress.Count < m_Path.Count)
					next = m_Path[m_Progress.Count];
				
				switch(id)
				{
					default:
					case 1: pick = current - 6; break;
					case 2: pick = current + 1; break;
					case 3: pick = current + 6; break;
					case 4: pick = current - 1; break;
				}
				
				if(m_Progress.Contains(pick) || pick < 0 || pick > 35)      //Off board or already chosen spot
				{
                    from.PlaySound(0x5B6);
					from.SendGump(new MazePuzzleGump(from, m_Item, m_Path, m_Progress));
				}
				//else if(m_Progress.Count == m_Path.Count && next == 35) //End!
                else if ((current == 34 || current == 28 || current == 29) && pick == 35)
				{
                    from.PlaySound(0x3D);
					m_Item.OnPuzzleCompleted(from);
				}
				else if(pick == next)                      		            //Found next 
				{
					m_Progress.Add(pick);
					m_Item.Progress = m_Progress;

                    from.PlaySound(0x1F5);
					from.SendGump(new MazePuzzleGump(from, m_Item, m_Path, m_Progress));
				}
				else
				{
					m_Item.DoDamage(from);
                    m_Progress.Clear();
                    m_Progress = null;
					from.SendGump(new MazePuzzleGump(from, m_Item, m_Path, m_Progress));
				}
			}
			
			else if(info.ButtonID == 5)
			{
                from.SendGump(new MazePuzzleGump(from, m_Item, m_Path, m_Progress, true));
			}
		}
		
		private int[] m_Possibles = new int[]
		{
			0,	1,	2,	3,	4,	5,
			6,	7,	8,	9,  10,	11,
			12, 13, 14, 15, 16, 17,
			18,	19,	20,	21,	22,	23,
			24,	25, 26, 27, 28, 29,
			30, 31, 32, 33, 34, 35
		};

        private int[][] m_Paths = new int[][]
        {
            new int[] { 0, 1, 2, 3, 4, 5, 11, 17, 23, 29, 35 },
            new int[] { 0, 6, 12, 18, 24, 30, 31, 32, 33, 34, 35 },
            new int[] { 0, 1, 2, 8, 14, 15, 16, 22, 28, 29, 35 },
            new int[] { 0, 1, 7, 13, 19, 20, 21, 27, 33, 34, 35 },
            new int[] { 0, 1, 7, 8, 14, 20, 26, 27, 33, 34, 35 },
            new int[] { 0, 1, 2, 3, 9, 10, 16, 15, 21, 27, 28, 34, 35 },
            new int[] { 0, 6, 12, 13, 19, 20, 26, 27, 28, 29, 35 },
            new int[] { 0, 6, 12, 18, 19, 25, 26, 20, 21, 22, 28, 34, 35 },
            new int[] { 0, 6, 7, 8, 14, 20, 21, 27, 28, 29, 35 },
            new int[] { 0, 6, 7, 13, 12, 18, 19, 20, 21, 27, 28, 34, 35 },
            new int[] { 0, 6, 12, 13, 19, 18, 24, 30, 31, 32, 33, 34, 35 },
            new int[] { 0, 1, 2, 8, 9, 15, 16, 10, 11, 17, 23, 29, 35 },
        };
		
		public List<int> GetRandomPath()
		{
            int r = Utility.Random(m_Paths.Length);
			return new List<int>(m_Paths[r]);
		}
	}
}