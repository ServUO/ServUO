/**************************** SearchImage.cs *******************************
 *
 *					(C) 2008, Lokai
 *			
 * Description: Command that displays a gump that lets you
 *	  search all 16384 images in the game. Images are 
 *	  displayed 10 to a page. You can set the search text
 *	  at the command or in the Gump using the Text Box
 *	  provided.
 *	  
 * Updated November 5, 2009: Now lets you create Static items
 *	  from the list available by targeting a location.
 *	  
 * Modified by Lord Greywolf: Added Tiling of images, and case-
 *    insensitive searching.
 *    
 * Updated November 17, 2009: Added useful lists, browsing of
 *    images by name or first letter, and images hiding. GetImage
 *    command and gump incorporated into SearchImage.cs. Added
 *    new Alias command: FindImage. Added BrowseImage and BrowseImages
 *    commands to search alphabetical list of image names.
 *    Also added Item Details gump.
 *   
/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server.Commands;
using Server.Targeting;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Custom
{
	public class SearchImageCommand
    {
        private static bool ImagesLoaded;
        public static List<string> ImageNames;

		public static void Initialize()
        {
            ImageNames = new List<string>();
            ImagesLoaded = LoadImages();
			//These are the commands the GM can use to show the new Gump.
            if (ImagesLoaded)
            {
                CommandSystem.Register("SearchImage", AccessLevel.GameMaster, new CommandEventHandler(SearchImage_OnCommand));
                CommandSystem.Register("GetImage", AccessLevel.GameMaster, new CommandEventHandler(SearchImage_OnCommand));
                CommandSystem.Register("FindImage", AccessLevel.GameMaster, new CommandEventHandler(SearchImage_OnCommand));
                CommandSystem.Register("BrowseImage", AccessLevel.GameMaster, new CommandEventHandler(BrowseImages_OnCommand));
                CommandSystem.Register("BrowseImages", AccessLevel.GameMaster, new CommandEventHandler(BrowseImages_OnCommand));
            }
            else
                Console.WriteLine("The Image commands were not registered due to an error loading the images.");
        }

        [Usage("BrowseImages")]
        [Aliases("BrowseImages")]
        [Description("Shows the BrowseImageNamesGump.")]
        public static void BrowseImages_OnCommand(CommandEventArgs e)
        {
            //Initialize index to 0
            int index = 0;

            //Initialize search string to ""
            string search = "";

            //If the GM gives the command with no search text, then send the regular Gump.
            if (e.Arguments.Length == 0) e.Mobile.SendGump(new BrowseImageNamesGump(0));
            else
            {
                //Try to find the index using the string passed to the Gump.
                try
                {
                    search = e.Arguments[0];
                    for (int x = 0; x < ImageNames.Count; x++)
                    {
                        if (ImageNames[x] == search)
                        {
                            index = x;
                            break;
                        }
                    }
                    e.Mobile.SendGump(new BrowseImageNamesGump(index));
                }
                catch { e.Mobile.SendGump(new BrowseImageNamesGump(0)); }
            }
        }

        [Usage("SearchImage {[search text] -or- [start index]}")]
        [Aliases("GetImage", "FindImage")]
		[Description("Shows the SearchImageGump, displaying matching entries.")]
		public static void SearchImage_OnCommand(CommandEventArgs e)
        {
            //Initialize image to 0
            int image = 0;

			//Initialize search string to ""
			string search = "";

			//If the GM gives the command with no search text, then send the regular Gump.
			if (e.Arguments.Length == 0) e.Mobile.SendGump(new SearchImageGump());
			else
            {
                //Try to convert the first argument to a number and set the image to that number.
                try { image = Convert.ToInt32(e.Arguments[0]); search = "ALL"; }
                catch
                {
                    //Try to set the first argument to a search string passed to the Gump.
                    try { search = e.Arguments[0]; }
                    catch { }
                }
                e.Mobile.SendGump(new SearchImageGump(image, search));
			}
        }

        private static bool LoadImages()
        {
            bool success = false;
            string name;
            try
            {
                //TileData.ItemTable.Length used, instead of 16384, in case a different TileData.mul is used.
                for (int x = 0; x < TileData.ItemTable.Length; x++) 
                {
                    name = TileData.ItemTable[x].Name;
                    if (name == "MissingName") name = "Missing Name";
                    name = name.Replace("%s%", "s");
                    name = name.Replace("%es%", "es");
                    if (!ImageNames.Contains(name)) ImageNames.Add(name);
                }
                ImageNames.Sort();
                Console.WriteLine("Image lists successfully loaded.");
                success = true;
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

            return success;
        }
	}

    public class BrowseImageNamesGump : Gump
    {
        private int m_Index = 0;
        private string[] letters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m",
                    "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        public BrowseImageNamesGump(int index)
            : base(140, 70)
        {
            m_Index = index;
            AddBackground(0, 0, 240, 700, 0x13BE);
			int y = 20;
            int num = 32; //This is the maximum number of image names we will display
            string name = ""; //Initialize the name of the image.

            List<string> list = new List<string>();
            list = SearchImageCommand.ImageNames;
            if (list != null && list.Count > 0)
            {
                if (list.Count - index < 32) num = list.Count - index;

                //Loop through the 32 names.
                for (int q = index; q < index + num; q++)
                {
                    name = SearchImageCommand.ImageNames[q];
                    AddButton(10, y, 2118, 2118, 100 + q, GumpButtonType.Reply, 0);
                    AddLabel(30, y - 2, 777, name);
                    y += 20;
                }
                y = 40;
                for (int letter = 0; letter < 26;letter++ )
                {
                    AddButton(208, y + 30, 2118, 2118, 20 + letter, GumpButtonType.Reply, 0);
                    AddLabel(223, y + 30, 777, letters[letter].ToUpper());
                    y += 20;
                }
                if (index > 1) AddButton(120, 5, 0x1519, 0x1519, 3, GumpButtonType.Reply, 0); // Previous Page
                if (index + 32 < list.Count) AddButton(120, 680, 0x151A, 0x151A, 4, GumpButtonType.Reply, 0); // Next Page
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile m = state.Mobile;
			int x = info.ButtonID;
            if (x >= 100) m.SendGump(new SearchImageGump(0, SearchImageCommand.ImageNames[x - 100])); //Previous Page
            else if (x >= 20)
            {
                int z = x - 20;
                for (int n = 0; n < SearchImageCommand.ImageNames.Count; n++)
                    if (SearchImageCommand.ImageNames[n].StartsWith(letters[z]))
                    {
                        m_Index = n;
                        break;
                    }
                m.SendGump(new BrowseImageNamesGump(m_Index));
            }
            else if (x == 3) m.SendGump(new BrowseImageNamesGump(m_Index - 32)); //Previous Page
            else if (x == 4) m.SendGump(new BrowseImageNamesGump(m_Index + 32)); //Next Page
        }
    }

    public class ImageDetailsGump : Gump
    {
        private int m_Index;
        private string m_Search;

        public ImageDetailsGump(int index, string search)
            : base(140, 70)
        {
            m_Index = index;
            m_Search = search;
            AddBackground(0, 0, 500, 300, 0x13BE);
            AddPage(0);

            ItemData id = TileData.ItemTable[index];

            AddLabel(200, 20, 777, id.Name);

            AddPage(1);
            AddLabel(30, 70, 0, string.Format("Bridge: {0}", id.Bridge.ToString()));
            AddLabel(30, 90, 0, string.Format("CalcHeight: {0}", id.CalcHeight.ToString()));
            AddLabel(30, 110, 0, string.Format("Height: {0}", id.Height.ToString()));
            AddLabel(30, 130, 0, string.Format("Impassable: {0}", id.Impassable.ToString()));
            AddLabel(30, 150, 0, string.Format("Quality: {0}", id.Quality.ToString()));
            AddLabel(30, 170, 0, string.Format("Quantity: {0}", id.Quantity.ToString()));
            AddLabel(30, 190, 0, string.Format("Surface: {0}", id.Surface.ToString()));
            AddLabel(30, 210, 0, string.Format("Value: {0}", id.Value.ToString()));
            AddLabel(30, 230, 0, string.Format("Weight: {0}", id.Weight.ToString()));
            AddLabel(30, 250, 0, string.Format("Flag Value: {0}", ((int)id.Flags).ToString()));
            AddLabel(30, 270, 0, string.Format("Flags: {0}", id.Flags.ToString()));
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile m = state.Mobile;
            m.SendGump(new SearchImageGump(m_Search));
        }
    }

	public class SearchImageGump : Gump
	{
		private string m_Search = "";
		private int m_Index = 0;
        private bool m_ShowImage;

		//If the base constructor is called, set the search text to "", and start at index 0.
		public SearchImageGump() : this(0, "") { }

		//If no index is given, set the index to '0'.
		public SearchImageGump(string search) : this(0, search) { }

        public SearchImageGump(int index, string search)
            : this(index, search, true)
        {
        }

        public SearchImageGump(int index, string search, bool showimage)
            : base(0, 54)
        {
            //Set the external private variables so we can use them later during the OnResponse method.
            m_Index = index;
            m_Search = search;
            m_ShowImage = showimage;

            //Initialize internal variables used in the Gump
            int x = 20; //This is the X-coordinate of where the first image will be located.
            int num = 10; //This is the maximum number of images we will display
            string name = ""; //Initialize the name of the image.

            AddPage(0);
            AddBackground(0, 0, 760, 145, 0x13BE);
            if (m_ShowImage)
            {
                AddBackground(0, 146, 760, 364, 0x13BE);
                AddAlphaRegion(6, 152, 748, 352);
                AddImageTiledButton(737, 148, 22400, 22400, 50, GumpButtonType.Reply, 0, 0, 0, 80, 20, 1078519);
            }
            else
            {
                AddBackground(729, 146, 31, 25, 0x13BE);
                AddImageTiledButton(737, 148, 22402, 22402, 50, GumpButtonType.Reply, 0, 0, 0, 80, 20, 1078518);
            }
            AddBackground(0, 511, 180, 39, 0x13BE);
            AddBackground(181, 511, 289, 39, 0x13BE);
            AddBackground(471, 511, 289, 39, 0x13BE);
            AddBackground(0, 551, 620, 39, 0x13BE);
            AddBackground(621, 551, 139, 39, 0x13BE);

            List<int> list = new List<int>();

            if (m_Search != "")
            {
                list.AddRange(GetList(m_Search));
                if (list != null && list.Count > 0)
                {
                    if (list.Count - index < 10) num = list.Count - index;

                    //Loop through the 10 images displayed.
                    for (int q = index; q < index + num; q++)
                    {
                        try
                        {
                            //Show the item.
                            if (m_ShowImage) AddItem(x, 155, list[q]);
                            AddImageTiledButton(x + 5, 90, 4030, 4031, list[q] + 100000, GumpButtonType.Reply, 0, 1352, 0, 140, 20, 500927);
                            AddImageTiledButton(x + 35, 90, 4009, 4010, list[q] + 200000, GumpButtonType.Reply, 0, 1352, 0, 140, 20, 1027024);
                            AddLabel(x + 5, 115, 20, "Details");
                            AddImageTiledButton(x + 49, 118, 1210, 1210, list[q] + 300000, GumpButtonType.Reply, 0, 1352, 0, 140, 20, 1049074);

                            //Derive the name from the ItemTable in TileData, and reformat, if necessary.
                            name = TileData.ItemTable[list[q]].Name;
                            if (name == "MissingName") name = "Missing Name";
                            name = name.Replace("%s%", "s");
                            name = name.Replace("%es%", "es");

                            //Display the name of the item.
                            AddHtml(x + 7, 32, 70, 60, name, false, false);
                        }
                        catch
                        {
                            //If displaying the name or item fails, display a canned message.
                            AddHtml(x, 92, 60, 120, string.Format("Unable to show Image ID {0}.", list[q].ToString()), false, false);
                        }
                        //Show the number of the item above the name.
                        AddLabel(x + 7, 12, 80, list[q].ToString());
                        x += 70; //Increment the X-coordinate by 70 to make room for the next image.
                    }

                    //Add icons to move forward and backward through pages.
                    if (index > 1) AddButton(7, 13, 0x1519, 0x1519, 3, GumpButtonType.Reply, 0); // Previous Page
                    if (index + 10 < list.Count) AddButton(707, 13, 0x151A, 0x151A, 4, GumpButtonType.Reply, 0); // Next Page
                }
                else AddHtml(20, 92, 260, 40, "No results found for that search.", false, false);
            }
            else AddHtml(20, 92, 260, 40, "Please enter a search string in the box.", false, false);

            //Display the number of images found in the last search.
            if (list == null) AddLabel(20, 520, 380, "0 images found.");
            else AddLabel(20, 520, 380, string.Format("{0} images found.", list.Count.ToString()));

            //Text boxes and buttons for starting a new search.
            AddLabel(200, 520, 380, "Search by name:");
            AddTextEntry(330, 520, 50, 20, 32, 1, search);
            AddButton(415, 520, 4015, 4016, 2, GumpButtonType.Reply, 0);

            AddLabel(490, 520, 380, "Search by number:");
            AddTextEntry(620, 520, 50, 20, 32, 2, "1");
            AddButton(705, 520, 4015, 4016, 5, GumpButtonType.Reply, 0);

            AddLabel(665, 560, 455, "< BROWSE >");
            AddButton(635, 560, 4030, 4030, 1, GumpButtonType.Reply, 0);

            AddPage(1); //Useful Lists
            AddLabel(20, 560, 380, "Useful Lists:");
            AddLabel(150, 560, 395, "Construction");
            AddButton(135, 563, 2118, 2118, 0, GumpButtonType.Page, 2);
            AddLabel(270, 560, 395, "Furniture");
            AddButton(255, 563, 2118, 2118, 0, GumpButtonType.Page, 3);
            AddLabel(370, 560, 395, "Nature");
            AddButton(355, 563, 2118, 2118, 0, GumpButtonType.Page, 4);
            AddLabel(460, 560, 395, "Equipment");
            AddButton(445, 563, 2118, 2118, 0, GumpButtonType.Page, 5);
            AddLabel(570, 560, 395, "Deco");
            AddButton(555, 563, 2118, 2118, 0, GumpButtonType.Page, 6);

            AddPage(2); //Construction
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 1);
            AddLabel(36, 562, 777, "Walls");
            AddButton(19, 564, 2118, 2118, 0, GumpButtonType.Page, 10);
            AddLabel(136, 562, 777, "Doors");
            AddButton(119, 564, 2118, 2118, 0, GumpButtonType.Page, 20);
            AddLabel(236, 562, 777, "Floors");
            AddButton(219, 564, 2118, 2118, 0, GumpButtonType.Page, 30);
            AddLabel(336, 562, 777, "Roofs");
            AddButton(319, 564, 2118, 2118, 0, GumpButtonType.Page, 40);
            AddLabel(436, 562, 777, "Stairs");
            AddButton(419, 564, 2118, 2118, 51, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Misc");
            AddButton(519, 564, 2118, 2118, 0, GumpButtonType.Page, 60);

            AddPage(10); //Walls
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 2);
            AddLabel(36, 562, 777, "Wood");
            AddButton(19, 564, 2118, 2118, 11, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Stone");
            AddButton(119, 564, 2118, 2118, 12, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "Marble");
            AddButton(219, 564, 2118, 2118, 13, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Plaster");
            AddButton(319, 564, 2118, 2118, 14, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Elven");
            AddButton(419, 564, 2118, 2118, 15, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Other");
            AddButton(519, 564, 2118, 2118, 16, GumpButtonType.Reply, 0);

            AddPage(20); //Doors
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 2);
            AddLabel(36, 562, 777, "Secret");
            AddButton(19, 564, 2118, 2118, 21, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Gates");
            AddButton(119, 564, 2118, 2118, 22, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "Normal");
            AddButton(219, 564, 2118, 2118, 23, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Elven");
            AddButton(319, 564, 2118, 2118, 24, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Crystal");
            AddButton(419, 564, 2118, 2118, 25, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Samurai");
            AddButton(519, 564, 2118, 2118, 26, GumpButtonType.Reply, 0);

            AddPage(30); //Floors
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 2);
            AddLabel(36, 562, 777, "Paved");
            AddButton(19, 564, 2118, 2118, 31, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Tile");
            AddButton(119, 564, 2118, 2118, 32, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "Wood");
            AddButton(219, 564, 2118, 2118, 33, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Rustic");
            AddButton(319, 564, 2118, 2118, 34, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Tatami");
            AddButton(419, 564, 2118, 2118, 35, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Other");
            AddButton(519, 564, 2118, 2118, 36, GumpButtonType.Reply, 0);

            AddPage(40); //Roofs
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 2);
            AddLabel(36, 562, 777, "Rustic");
            AddButton(19, 564, 2118, 2118, 41, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Ceramic");
            AddButton(119, 564, 2118, 2118, 42, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "Hard");
            AddButton(219, 564, 2118, 2118, 43, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Wooden");
            AddButton(319, 564, 2118, 2118, 44, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Elven");
            AddButton(419, 564, 2118, 2118, 45, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "NewAge");
            AddButton(519, 564, 2118, 2118, 46, GumpButtonType.Reply, 0);

            AddPage(60); //Misc
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 2);
            AddLabel(36, 562, 777, "Arches");
            AddButton(19, 564, 2118, 2118, 61, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Various");
            AddButton(119, 564, 2118, 2118, 62, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "RoundWalls");
            AddButton(219, 564, 2118, 2118, 63, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Battlements");
            AddButton(319, 564, 2118, 2118, 64, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Roofs");
            AddButton(419, 564, 2118, 2118, 65, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Fences");
            AddButton(519, 564, 2118, 2118, 66, GumpButtonType.Reply, 0);

            AddPage(3); //Furniture
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 1);
            AddLabel(36, 562, 777, "Chairs");
            AddButton(19, 564, 2118, 2118, 71, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Storage");
            AddButton(119, 564, 2118, 2118, 72, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "Tables");
            AddButton(219, 564, 2118, 2118, 73, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Beds");
            AddButton(319, 564, 2118, 2118, 74, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Shelves");
            AddButton(419, 564, 2118, 2118, 75, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Dressers");
            AddButton(519, 564, 2118, 2118, 76, GumpButtonType.Reply, 0);

            AddPage(4); //Nature
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 1);
            AddLabel(36, 562, 777, "Trees");
            AddButton(19, 564, 2118, 2118, 81, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Plants");
            AddButton(119, 564, 2118, 2118, 82, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "Rocks");
            AddButton(219, 564, 2118, 2118, 83, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Animals");
            AddButton(319, 564, 2118, 2118, 84, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Foliage");
            AddButton(419, 564, 2118, 2118, 85, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Water");
            AddButton(519, 564, 2118, 2118, 86, GumpButtonType.Reply, 0);

            AddPage(5); //Equipment
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 1);
            AddLabel(36, 562, 777, "LightSource");
            AddButton(19, 564, 2118, 2118, 91, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Weapons");
            AddButton(119, 564, 2118, 2118, 92, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "Armor");
            AddButton(219, 564, 2118, 2118, 93, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Tools");
            AddButton(319, 564, 2118, 2118, 94, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Bottles");
            AddButton(419, 564, 2118, 2118, 95, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Containers");
            AddButton(519, 564, 2118, 2118, 96, GumpButtonType.Reply, 0);

            AddPage(6); //Deco
            AddButton(3, 564, 2436, 2436, 0, GumpButtonType.Page, 1);
            AddLabel(36, 562, 777, "Banners");
            AddButton(19, 564, 2118, 2118, 101, GumpButtonType.Reply, 0);
            AddLabel(136, 562, 777, "Signs");
            AddButton(119, 564, 2118, 2118, 102, GumpButtonType.Reply, 0);
            AddLabel(236, 562, 777, "Stones");
            AddButton(219, 564, 2118, 2118, 103, GumpButtonType.Reply, 0);
            AddLabel(336, 562, 777, "Consumables");
            AddButton(319, 564, 2118, 2118, 104, GumpButtonType.Reply, 0);
            AddLabel(436, 562, 777, "Debris");
            AddButton(419, 564, 2118, 2118, 105, GumpButtonType.Reply, 0);
            AddLabel(536, 562, 777, "Symbols");
            AddButton(519, 564, 2118, 2118, 106, GumpButtonType.Reply, 0);
        }

        private int[] m_SecretDoors = new int[] {
            808,810,804,806,812,814,816,818,824,826,820,822,828,830,832,834,
            840,842,836,838,844,846,848,850,856,858,852,854,860,862,864,866 };

        private int[] m_Gates = new int[] {
            2088,2090,2084,2086,2092,2094,2096,2098,2109,2111,2105,2107,2113,2115,2117,2119,
            2128,2130,2124,2126,2132,2134,2136,2138,2154,2156,2150,2152,2158,2160,2162,2164,
            8177,8179,8173,8175,8181,8183,8185,8187 };

        private int[] m_NormalDoors = new int[] {
            1657,1659,1653,1655,1661,1663,1665,1667,1689,1691,1685,1687,1693,1695,1697,1699,
            1705,1707,1701,1703,1709,1711,1713,1715,1721,1723,1717,1719,1725,1727,1729,1731,
            1753,1755,1749,1751,1757,1759,1761,1763,1769,1771,1765,1767,1773,1775,1777,1779 };

        private int[] m_ElvenDoors = new int[] {
            12718,12716,11592,11590,11621,12704,11619,12706,11625,12708,11623,12710,
            11629,12714,11627,12712,12260,12700,12258,12702 };

        private int[] m_CrystalDoors = new int[] {
            13951,13953,13947,13949,13955,13957,13959,13961,13967,13969,13963,13965,13971,13973,13975,13977 };

        private int[] m_SEDoors = new int[] {
            10767,10765,10771,10769,9251,10759,10757,10763,10761,9247,10775,10773,10779,10777 };

        private int[] m_PavedFloors = new int[] {
            1305,1306,1307,1308,1309,1310,1311,1312,1313,1314,1315,1316,1181,1182,1183,1184,
            1317,1318,1319,1320,1321,1322,1323,1324,1327,1328,1329,1330,1331,1332,1333,1334,
            1280,1281,1282,1283,1407,1408,1409,1410,1276,1277,1278,1279,1411,1412,1413,
            1250,1250,1250,1250,1250,1250,1250,1335,1336,1337,1338 };

        private int[] m_TileFloors = new int[] {
            1264,1265,1262,1263,1259,1260,1261,1258,1266,1275,1272,1273,1274,1270,1271,1268,1269,1267,
            1293,1294,1297,1298,1299,1300,1179,1180,1295,1296,1173,1174,1175,1176,
            1395,1396,1403,1404,1595,1596,1373,1374,1397,1398,1401,1402,1597,1598,1375,1376 };

        private int[] m_WoodFloors = new int[] {
            1204,1200,1203,1199,1201,1198,1202,1197,1189,1190,1193,1195,1196,
            1216,1212,1215,1211,1213,1210,1214,1209,1191,1192,1205,1206,1207,1208,
            1233,1228,1235,1230,1234,1229,1232,1231,1185,1186,1226,1227,1222,1223,1224,1225,
            1247,1241,1249,1244,1248,1240,1246,1245,1187,1188,1242,1243,1236,1237,1238,1239,
            9275,9276,9277,9278,9279,9280,9281,9282,9283,9284,9285,9286,
            9287,9288,9289,9290,9291,9292,9293,9294,9295,9296,9297,9298,
            10592,10593,10594,10595,10596,10598,10599,10600,10601,10602,10603,10604,
            10605,10606,10607,10608,10609,10610,10611,10612,10613,10614,10615,10597,
            10018,10019,10020,10021,10022,10023,10024,10025,10026,10027,10028,10029,
            10030,10031,10032,10033,10034,10035,10036,10037,10038,10039,10040,10041,
            10042,10043,10044,10045,10046,10047,10048,10049,10050,10051,10052,10053,
            10054,10055,10056,10057,10058,10059,10060,10061,10062,10063,10064,10065,
            10616,10617,10618,10619,10620,10621,10622,10623,10624,10625,10626,10627,
            10628,10629,10630,10631,10632,10633,10634,10635,10636,10637,10638,10639 };

        private int[] m_RusticFloors = new int[] {
            1288,1284,1286,1287,1285,1289,1290,1220,1218,1217,1219,1221,1291,1292,
            1040,1041,1039,1042,1035,1036,1038,1037,1339,1340,1341,1342,1043,1045,1044,1046,
            6013,6014,6015,6016,6017,6077,6078,6079,6080,
            12788,12789,12790,12791,12792,12795,1301,1302,1303,1304,12793,12794,
            13465,13471,13477,13483,13522,4850,4862,4868,4874,4880,4886,4892,
            4896,4899,4902,4905,4908,4911,4914,4917,4920,4923,4926,4929,4932,4935,4938,4941,
            12906,12907,12908,12909,12910,12911,12809,12810,
            12888,12889,12890,12891,12892,12893,12894,12895,12896,12897,12898,12899,12900,12901,
            9299,9300,9301,9302,9305,9306,9307,9303,9304,9308,9309,
            9310,9311,9312,9313,9314,9315,9320,9321,9322,9316,9317,9318,9319,9323 };

        private int[] m_TatamiFloors = new int[] {
            10688,10689,10690,10691,10692,10693,10694,10695,10696,10697,10698,10699,10704,10705,10710,10711,
            10812,10813,10814,10818,10815,10816,10817,10820,10785,10786,10787,10788,10789,10790,10791,10792,
            9269,9270,9271,9272,9253,9254,9255,9256,9257,9258,9259,9260,9261,9262,9263,9264,9265,9266,9267,9268 };

        private int[] m_OtherFloors = new int[] {
            11189,11190,11191,11192,11215,11500,11501,11502,11576,11577,11578,11579,11723,11724,11725,11726,
            13746,13747,13748,13749,13750,13751,13752,13753,13754,13755,13756,13757,
            13850,13851,13852,13853,13854,13855,13856,13857,13858 };

        private int[] m_WoodWalls = new int[] {
            10,7,12,6,13,8,11,9,14,15,18,18,18,16,17,17,17,19,22,22,22,20,21,21,21,23,
            171,168,173,166,172,167,170,169,186,9472,9479,9478,9473,185,
            178,175,181,174,180,176,179,177,188,187,948,948,948,947,949,949,949,950,
            191,191,191,189,190,190,190,192,9367,9367,9367,9365,9366,9366,9366,9368,
            9363,9363,9363,9361,9362,9362,9362,9364,9359,9359,9359,9357,9358,9358,9358,9360,
            11585,11585,11585,11546,11584,11584,11584,11549,
            11587,11587,11589,11589,11588,11588,11586,11586,
            11583,11583,11583,11545,11582,11582,11582,11548,
            11581,11581,11581,11544,11580,11580,11580,11547 };

        private int[] m_StoneWalls = new int[] {
            30,28,33,26,32,27,31,29,10670,34,35,10675,10668,10669,10671,10672,10677,10676,10674,10673,
            37,37,37,36,38,38,38,39,464,464,464,463,465,465,465,466,467,468,
            10660,10661,10662,10663,10667,10666,10665,10664,489,489,489,488,490,490,490,491,
            200,200,200,199,201,201,201,204,202,203,222,222,222,220,221,221,221,223,
            345,345,345,344,346,346,346,347,348,349,352,352,352,350,351,351,351,353,355,354,
            357,357,357,356,358,358,358,359,362,362,362,360,361,361,361,363,
            517,515,512,511,513,516,518,514,519,9519,9527,9526,9520,522,
            598,598,598,597,599,599,599,601,592,592,592,591,593,593,593,600,
            595,595,595,594,596,596,596,602,589,589,589,588,590,590,590,603,
            968,968,968,967,969,969,969,970,990,991,972,972,972,971,973,973,973,974,
            983,980,993,979,994,981,984,982,983,980,993,992,994,981,984,982,
            960,960,960,958,961,961,961,962,976,976,976,975,977,977,977,978,
            10581,10581,10581,10584,10578,10578,10578,10587,
            10580,10580,10580,10583,10577,10577,10577,10586,
            10579,10579,10579,10582,10576,10576,10576,10585 };

        private int[] m_MarbleWalls = new int[] {
            249,249,249,248,250,250,250,251,252,9484,9491,9490,9485,253,
            255,255,255,254,256,256,256,257,258,9496,9503,9502,9497,259,
            261,261,261,260,262,262,262,263,264,9508,9515,9514,9509,265,
            267,267,267,266,268,268,268,269,271,271,271,270,272,272,272,273,
            1091,1091,1091,1090,1092,1092,1092,1093,280,280,280,279,281,281,281,282,
            670,670,670,669,671,671,671,672,686,685,664,664,664,663,665,665,665,666,667,668,
            658,658,658,657,659,659,659,660,661,9532,9941,9940,9533,662,674,674,674,673,675,675,675,676,
            698,698,698,697,699,699,699,700,1105,1105,1105,1104,1106,1106,1106,1107,
            694,694,694,693,695,695,695,696 };

        private int[] m_PlasterWalls = new int[] {
            312,310,307,309,308,311,313,298,314,315,312,310,307,306,308,311,313,298,
            302,310,296,295,297,311,303,298,304,310,300,299,301,311,305,298,
            336,336,336,332,334,334,334,298,342,343,338,338,338,335,339,339,339,298,340,341,
            910,910,910,909,911,911,911,898,912,915,907,914,908,916,913,898,
            912,915,907,906,908,916,913,898,902,915,896,895,897,916,903,898,
            904,915,900,899,901,916,905,898,9351,9351,9351,9349,9350,9350,9350,9354,
            10745,10743,10740,10742,10741,10744,10746,10731,10747,10748,
            10745,10743,10740,10739,10741,10744,10746,10731,10747,10748,
            10735,10729,10729,10728,10730,10736,10736,10731,
            10737,10733,10733,10732,10734,10738,10738,10731,
            10726,10722,10721,10723,10727,10724,9373,9378,9373,10800,9374,9377,9374,10806,
            9373,9384,9373,10803,9374,9383,9374,10809,9371,9380,9371,10801,9372,9379,9372,10807,
            9371,9386,9371,10804,9372,9385,9372,10810,9382,9382,9376,10802,9375,9381,9381,10808};

        private int[] m_ElvenWalls = new int[] {
            11132,11717,11131,11131,11131,11720,11133,11136,11134,11135,11720,
            11130,11716,11729,11719,11728,11715,11727,11718,
            11198,11183,11197,11186,11200,11202,11201,11199,11199,
            11196,11182,11195,11195,11195,11185,11194,11181,11193,11193,11193,11184,
            11508,11211,11507,11212,11510,11512,11511,11509,
            11506,11209,11505,11210,11504,11207,11503,11208 };

        private int[] m_OtherWalls = new int[] {
            55,52,57,51,58,53,56,54,10681,59,60,10686,10678,10679,10680,10682,10687,10685,10684,10683,
            62,62,62,61,63,63,63,64,66,66,66,65,67,67,67,68,88,88,88,89,87,87,87,90,92,94,93,91,
            10656,10657,10659,10658,95,95,95,97,96,96,96,98,99,99,99,101,100,100,100,102,
            105,105,105,107,106,106,106,108,444,444,440,438,439,445,445,441,452,453,
            446,450,448,449,451,447,441,454,455,427,422,423,421,426,425,428,424,429,430,432,431,
            8539,8539,8539,8540,8538,8538,8538,419,149,146,151,144,150,145,148,147,152,
            9460,9467,9466,9461,153,159,156,161,154,160,155,158,157,552,552,552,550,551,551,551,553,
            1072,1058,546,547,1057,545,545,553,1059,1059,1059,1061,1060,1060,1060,
            9348,9348,9348,9346,9347,9347,9347,9353,9345,9345,9345,9343,9344,9344,9344,9352,
            10650,10650,10650,10653,10647,10647,10647,10644,10649,10649,10649,10652,10646,10646,10646,10643,
            10648,10648,10648,10651,10645,10645,10645,10642,10015,10015,10015,10005,10012,10012,10012,10076,
            10014,10014,10014,10004,10011,10011,10011,10075,10013,10013,10013,10003,10010,10010,10010,10074,
            10082,10082,10082,10069,10079,10079,10079,10076,10081,10081,10081,10068,10078,10078,10078,10075,
            10080,10080,10080,10067,10077,10077,10077,10074,10560,10560,10560,10554,10557,10557,10557,10563,
            10559,10559,10559,10553,10556,10556,10556,10562,10558,10558,10558,10552,10555,10555,10555,10561,
            10575,10575,10575,10566,10569,10569,10569,10572,10574,10574,10574,10565,10568,10568,10568,10571,
            10573,10573,10573,10564,10567,10567,10567,10570,
            13795,13796,13796,13785,13794,13794,13793,13788,13798,13798,13797,13797,
            13792,13792,13792,13784,13791,13791,13791,13787,13790,13790,13790,13783,13789,13789,13789,13786,
            13883,13897,13895,13846,13896,13898,13882,13849,13885,13885,13884,13884,
            13881,13881,13881,13845,13880,13880,13880,13848,13879,13879,13879,13844,13878,13878,13878,13847,13843 };

        private int[] m_RusticRoofs = new int[] {
            11344,11331,11343,11332,11330,11342,11335,11334,11336,11333,11338,11341,11339,11340,11337,
            11350,11345,11348,11347,11346,11349,11353,11352,11354,11351,11356,11359,11357,11358,11355,
            11374,11369,11372,11371,11370,11373,11362,11361,11363,11360,11364,11368,11365,11367,11366,
            9964,9947,9963,9948,9946,9962,9951,9950,9952,9949,9954,9955,9956,9953,9957,9958,9959,9960,9961,
            9994,9977,9993,9978,9976,9992,9981,9980,9982,9979,9984,9985,9986,9983,9987,9988,9989,9990,9991 };

        private int[] m_CeramicRoofs = new int[] {
            9159,9160,9155,9156,9153,9154,9157,9158,9163,9164,9165,9166,9167,9168,9161,9162,
            9151,9150,9152,9171,9169,9170,9184,9185,9180,9181,9178,9179,9182,9183,9188,9189,9190,9191,9192,
            9193,9186,9187,9174,9172,9176,9177,9173,9175,9206,9207,9202,9203,9200,9201,9204,9205,9210,9211,
            9212,9213,9214,9215,9208,9209,9196,9194,9198,9199,9195,9197,10540,10541,10536,10537,10534,10535,
            10538,10539,10544,10545,10546,10547,10548,10549,10542,10543,10530,10528,10532,10533,10529,10531,
            10492,10493,10488,10489,10486,10487,10490,10491,10496,10497,10498,10499,10500,10501,10494,10495,
            10482,10480,10484,10485,10481,10483,10514,10515,10510,10511,10508,10509,10512,10513,10518,10519,
            10520,10521,10522,10523,10516,10517,10504,10502,10506,10507,10503,10505 };

        private int[] m_HardRoofs = new int[] {
            11314,11301,11313,11302,11300,11312,11305,11304,11306,11303,11308,11311,11309,11310,11307,
            11389,11376,11388,11377,11375,11387,11380,11379,11381,11378,11383,11386,11384,11385,11382,
            10436,10419,10435,10420,10418,10434,10423,10422,10424,10421,10426,10437,10427,
            10428,10425,10430,10431,10432,10433,10795,10796,10475,10476,10477,10478,10798 };

        private int[] m_WoodenRoofs = new int[] {
            10458,10441,10457,10442,10440,10456,10445,10444,10446,10443,10448,10451,10449,10450,
            10447,10452,10453,10454,10455,10473,10472,10459,10468,10469,10470,10474,10471,
            11329,11316,11328,11317,11315,11327,11320,11319,11321,11318,11323,11326,11324,11325,11322 };

        private int[] m_ElvenRoofs = new int[] {
            11155,11138,11154,11139,11137,11153,11142,11141,11143,11140,11145,11148,11146,11147,11144,11149,
            11150,11151,11152,12064,12065,12066,12067,12063,11174,11157,11173,11158,11156,11172,11161,
            11160,11159,11162,11164,11167,11165,11166,11163,11168,11169,11170,11171,12068,12069,12070,12071,
            11175,11176,12072 };

        private int[] m_NewAgeRoofs = new int[] {
            13762,13759,13760,13761,13762,13763,13764,13765,13776,13777,13766,13767,13768,13769,
            13770,13771,13772,13773,13774,13775,13859,13860,13861,13862,13863,13864,13865,13875,
            13876,13866,13867,13868,13869,13870,13871,13872,13873,13874,13877 };

        private int[] m_MiscArches = new int[] {
            44,41,40,42,43,29,470,471,469,473,472,466,476,477,475,479,478,466,209,207,205,206,208,204,
            218,216,212,215,217,204,71,72,69,70,73,54,83,80,78,82,79,81,84,111,112,109,110,113,90,
            1082,1081,1080,1083,1084,251,1087,1086,1085,1088,1089,257,276,275,274,277,278,263,
            1096,1095,1094,1097,1098,672,1101,1100,1099,1102,1103,666,690,689,688,691,692,660,
            368,370,365,364,366,369,367,353,10718,10719,10716,10725,10717,10720,
            11722,11713,11714,12059,11711,11721,11712,11188,11179,11180,12060,11177,11187,11178,
            11214,11205,11206,12061,11203,11213,11204,11575,11542,11543,12062,11540,11574,11541 };

        private int[] m_VariousMisc = new int[] {
            480,481,482,483,484,485,474,486,487,225,213,225,224,214,226,227,226,211,228,219,229,210,
            631,636,633,632,634,635,641,642,284,283,285,287,286,288,7978,289,290,292,291,
            680,679,681,683,682,684,701,702,704,703,395,394,396,397,398,399,403,402,400,401,405,404,406,
            951,952,953,954,955,956,957,959,963,964,965,966,13550,13556,13559,13562,13568,13574,
            13582,13586,13592,13598,13604,6425,6424,6419,6427,6426,6417,6429,6428,6416,6431,6430,6418,
            6173,6174,6175,6176,6177,6178,6179,6180,6181,6182,6183,6184,13802,13803,13804,13814,13815,
            13838,13839,13840,13841,13842 };

        private int[] m_RoundedWalls = new int[] {
            16134,9541,9537,9538,9550,9551,9555,9541,9539,9543,9554,9553,9555,
            9555,9548,9549,9550,9544,9541,9555,9546,9543,9535,9536,9541,
            538,536,534,535,528,530,529,531,538,533,529,532,528,527,534,537 };

        private int[] m_Battlements = new int[] {
            118,116,115,114,117,119,373,374,375,376,386,387,388,389,420,380,378,379,377,381,371,382,383,384,
            385,390,391,392,393,711,715,714,713,716,717,712,718,724,719,721,720,722,723,725,726,727,728,729 };

        private int[] m_MiscRoofs = new int[] {
            24,25,49,50,85,86,120,121,456,457,162,163,164,165,193,194,
            230,231,293,294,330,331,433,434,435,436,409,410,494,495,407,408,
            492,493,523,524,554,555,677,678,10722,10721,10723,10724,10726,10727 };

        private int[] m_MiscFences = new int[] {
            2083,2082,2081,2123,2122,2121,2141,2140,2142,2143,2147,2146,2148,2149,
            2226,2227,2228,2229,2243,2244,2245,2246,2230,2231,2232,2233,2234,2235,
            2236,2237,2238,2239,2240,2241,2242,2285,2283,2284,2286,2299,2300,
            2289,2287,2288,2290,2294,2291,2292,2293,2297,2295,2296,2298 };

        private int[] m_Stairs = new int[] {
            1848,1849,1852,1851,1850,1856,1854,1862,1861,1955,1956,1959,1958,1957,1963,1961,
            1928,1929,1932,1931,1930,1936,1934,1939,1938,1825,1826,1829,1828,1827,1833,1831,1836,1835,
            1822,1823,1865,1847,1846,1869,1867,1801,1802,1805,1804,1803,1809,1807,1812,1811,
            1006,1007,1010,1009,1008,1014,1012,1017,1016,1900,1901,1904,1903,1902,1908,1906,1911,1910,
            1872,1873,1876,1875,1874,1880,1878,1883,1882,1978,1979,1980,1991 };

		//This method simply loops through all images, and compares the Name in the ItemTable with the
		// search string provided, and if found, adds it to a List which is then returned.
        // If one of the predefined lists is requested, it will return the list.
		private IEnumerable<int> GetList(string search)
		{
			List<int> list = new List<int>();

            if (search == "ALL") { for (int x = 0; x < TileData.ItemTable.Length; x++) list.Add(x); }

            else if (search == "WoodWalls")
            {
                for (int x = 0; x < m_WoodWalls.Length; x++)
                    if (!list.Contains(m_WoodWalls[x])) list.Add(m_WoodWalls[x]);
            }
            else if (search == "StoneWalls")
            {
                for (int x = 0; x < m_StoneWalls.Length; x++)
                    if (!list.Contains(m_StoneWalls[x])) list.Add(m_StoneWalls[x]);
            }
            else if (search == "MarbleWalls")
            {
                for (int x = 0; x < m_MarbleWalls.Length; x++)
                    if (!list.Contains(m_MarbleWalls[x])) list.Add(m_MarbleWalls[x]);
            }
            else if (search == "PlasterWalls")
            {
                for (int x = 0; x < m_PlasterWalls.Length; x++)
                    if (!list.Contains(m_PlasterWalls[x])) list.Add(m_PlasterWalls[x]);
            }
            else if (search == "ElvenWalls")
            {
                for (int x = 0; x < m_ElvenWalls.Length; x++)
                    if (!list.Contains(m_ElvenWalls[x])) list.Add(m_ElvenWalls[x]);
            }
            else if (search == "OtherWalls")
            {
                for (int x = 0; x < m_OtherWalls.Length; x++)
                    if (!list.Contains(m_OtherWalls[x])) list.Add(m_OtherWalls[x]);
            }

            else if (search == "SecretDoors")
            {
                for (int x = 0; x < m_SecretDoors.Length; x++)
                    if (!list.Contains(m_SecretDoors[x])) list.Add(m_SecretDoors[x]);
            }
            else if (search == "Gates")
            {
                for (int x = 0; x < m_Gates.Length; x++)
                    if (!list.Contains(m_Gates[x])) list.Add(m_Gates[x]);
            }
            else if (search == "NormalDoors")
            {
                for (int x = 0; x < m_NormalDoors.Length; x++)
                    if (!list.Contains(m_NormalDoors[x])) list.Add(m_NormalDoors[x]);
            }
            else if (search == "ElvenDoors")
            {
                for (int x = 0; x < m_ElvenDoors.Length; x++)
                    if (!list.Contains(m_ElvenDoors[x])) list.Add(m_ElvenDoors[x]);
            }
            else if (search == "CrystalDoors")
            {
                for (int x = 0; x < m_CrystalDoors.Length; x++)
                    if (!list.Contains(m_CrystalDoors[x])) list.Add(m_CrystalDoors[x]);
            }
            else if (search == "SamuraiDoors")
            {
                for (int x = 0; x < m_SEDoors.Length; x++)
                    if (!list.Contains(m_SEDoors[x])) list.Add(m_SEDoors[x]);
            }

            else if (search == "PavedFloors")
            {
                for (int x = 0; x < m_PavedFloors.Length; x++)
                    if (!list.Contains(m_PavedFloors[x])) list.Add(m_PavedFloors[x]);
            }
            else if (search == "TileFloors")
            {
                for (int x = 0; x < m_TileFloors.Length; x++)
                    if (!list.Contains(m_TileFloors[x])) list.Add(m_TileFloors[x]);
            }
            else if (search == "WoodFloors")
            {
                for (int x = 0; x < m_WoodFloors.Length; x++)
                    if (!list.Contains(m_WoodFloors[x])) list.Add(m_WoodFloors[x]);
            }
            else if (search == "RusticFloors")
            {
                for (int x = 0; x < m_RusticFloors.Length; x++)
                    if (!list.Contains(m_RusticFloors[x])) list.Add(m_RusticFloors[x]);
            }
            else if (search == "TatamiFloors")
            {
                for (int x = 0; x < m_TatamiFloors.Length; x++)
                    if (!list.Contains(m_TatamiFloors[x])) list.Add(m_TatamiFloors[x]);
            }
            else if (search == "OtherFloors")
            {
                for (int x = 0; x < m_OtherFloors.Length; x++)
                    if (!list.Contains(m_OtherFloors[x])) list.Add(m_OtherFloors[x]);
            }
            else if (search == "RusticRoofs")
            {
                for (int x = 0; x < m_RusticRoofs.Length; x++)
                    if (!list.Contains(m_RusticRoofs[x])) list.Add(m_RusticRoofs[x]);
            }
            else if (search == "CeramicRoofs")
            {
                for (int x = 0; x < m_CeramicRoofs.Length; x++)
                    if (!list.Contains(m_CeramicRoofs[x])) list.Add(m_CeramicRoofs[x]);
            }
            else if (search == "HardRoofs")
            {
                for (int x = 0; x < m_HardRoofs.Length; x++)
                    if (!list.Contains(m_HardRoofs[x])) list.Add(m_HardRoofs[x]);
            }
            else if (search == "WoodenRoofs")
            {
                for (int x = 0; x < m_WoodenRoofs.Length; x++)
                    if (!list.Contains(m_WoodenRoofs[x])) list.Add(m_WoodenRoofs[x]);
            }
            else if (search == "ElvenRoofs")
            {
                for (int x = 0; x < m_ElvenRoofs.Length; x++)
                    if (!list.Contains(m_ElvenRoofs[x])) list.Add(m_ElvenRoofs[x]);
            }
            else if (search == "NewAgeRoofs")
            {
                for (int x = 0; x < m_NewAgeRoofs.Length; x++)
                    if (!list.Contains(m_NewAgeRoofs[x])) list.Add(m_NewAgeRoofs[x]);
            }
            else if (search == "MiscArches")
            {
                for (int x = 0; x < m_MiscArches.Length; x++)
                    if (!list.Contains(m_MiscArches[x])) list.Add(m_MiscArches[x]);
            }
            else if (search == "VariousMisc")
            {
                for (int x = 0; x < m_VariousMisc.Length; x++)
                    if (!list.Contains(m_VariousMisc[x])) list.Add(m_VariousMisc[x]);
            }
            else if (search == "RoundedWalls")
            {
                for (int x = 0; x < m_RoundedWalls.Length; x++)
                    if (!list.Contains(m_RoundedWalls[x])) list.Add(m_RoundedWalls[x]);
            }
            else if (search == "Battlements")
            {
                for (int x = 0; x < m_Battlements.Length; x++)
                    if (!list.Contains(m_Battlements[x])) list.Add(m_Battlements[x]);
            }
            else if (search == "MiscRoofs")
            {
                for (int x = 0; x < m_MiscRoofs.Length; x++)
                    if (!list.Contains(m_MiscRoofs[x])) list.Add(m_MiscRoofs[x]);
            }
            else if (search == "MiscFences")
            {
                for (int x = 0; x < m_MiscFences.Length; x++)
                    if (!list.Contains(m_MiscFences[x])) list.Add(m_MiscFences[x]);
            }
            else if (search == "Stairs")
            {
                for (int x = 0; x < m_Stairs.Length; x++)
                    if (!list.Contains(m_Stairs[x])) list.Add(m_Stairs[x]);
            }
                //These 6 lists are pretty interesting...
            else if (search == "Foliage") {
                for (int x = 0; x < TileData.ItemTable.Length; x++)
                    if ((TileData.ItemTable[x].Flags & TileFlag.Foliage) != 0) list.Add(x);
            }
            else if (search == "LightSource"){
                for (int x = 0; x < TileData.ItemTable.Length; x++)
                    if ((TileData.ItemTable[x].Flags & TileFlag.LightSource) != 0) list.Add(x);
            }
            else if (search == "Containers"){
                for (int x = 0; x < TileData.ItemTable.Length; x++)
                    if ((TileData.ItemTable[x].Flags & TileFlag.Container) != 0) list.Add(x);
            }
            else if (search == "Weapons"){
                for (int x = 0; x < TileData.ItemTable.Length; x++)
                    if ((TileData.ItemTable[x].Flags & TileFlag.Weapon) != 0) list.Add(x);
            }
            else if (search == "Armor"){
                for (int x = 0; x < TileData.ItemTable.Length; x++)
                    if ((TileData.ItemTable[x].Flags & TileFlag.Armor) != 0) list.Add(x);
            }
            else if (search == "Consumables"){
                for (int x = 0; x < TileData.ItemTable.Length; x++)
                    if ((int)TileData.ItemTable[x].Flags == 18432) list.Add(x);
            }

                    //TODO: These need to be refined/replaced with better lists, but they are a start...
            else if (search == "Chairs") { list.AddRange(GetList("chair")); list.AddRange(GetList("stool")); }
            else if (search == "Storage") { list.AddRange(GetList("crate")); list.AddRange(GetList("box")); }
            else if (search == "Tables") { list.AddRange(GetList("table")); }
            else if (search == "Beds") { list.AddRange(GetList("bed")); }
            else if (search == "Shelves") { list.AddRange(GetList("shelf")); }
            else if (search == "Dressers") { list.AddRange(GetList("dresser")); }
            else if (search == "Trees") { list.AddRange(GetList("tree")); }
            else if (search == "Plants") { list.AddRange(GetList("plant")); }
            else if (search == "Rocks") { list.AddRange(GetList("rock")); }
            else if (search == "Animals") { list.AddRange(GetList("cat")); list.AddRange(GetList("dog")); list.AddRange(GetList("bird")); list.AddRange(GetList("sheep")); list.AddRange(GetList("pig")); list.AddRange(GetList("cow")); list.AddRange(GetList("deer")); list.AddRange(GetList("bear")); }
            else if (search == "Monsters") { list.AddRange(GetList("rat")); list.AddRange(GetList("lizard")); list.AddRange(GetList("orc")); }
            else if (search == "Water") { list.AddRange(GetList("water")); }
            else if (search == "Utensils") { list.AddRange(GetList("knife")); list.AddRange(GetList("fork")); list.AddRange(GetList("spoon")); }
            else if (search == "Tools") { list.AddRange(GetList("shovel")); list.AddRange(GetList("pick")); list.AddRange(GetList("tool")); list.AddRange(GetList("hammer")); list.AddRange(GetList("froe")); list.AddRange(GetList("saw")); }
            else if (search == "Bottles") { list.AddRange(GetList("bottle")); }
            else if (search == "Bags") { list.AddRange(GetList("bag")); list.AddRange(GetList("backpack")); }
            else if (search == "Banners") { list.AddRange(GetList("banner")); list.AddRange(GetList("flag")); }
            else if (search == "Signs") { list.AddRange(GetList("sign")); }
            else if (search == "Stones") { list.AddRange(GetList("stone")); }
            else if (search == "Debris") { list.AddRange(GetList("debris")); }
            else if (search == "Symbols") { list.AddRange(GetList("symbol")); }
                    //TODO: These need to be refined/replaced with better lists, but they are a start...

            else
            {
                for (int x = 0; x < TileData.ItemTable.Length; x++)
                {
                    try { if (Insensitive.Contains(TileData.ItemTable[x].Name, search)) list.Add(x); }
                    catch { }
                }
            }
			return list;
		}

		private bool FindItem(Mobile m, int itemID)
		{
			IPooledEnumerable eable = m.Map.GetItemsInRange(new Point3D(m.X, m.Y, m.Z), 0);
			foreach (Item item in eable)
			{
				if (item.Z == m.Z && item.ItemID == itemID) return true;
			}
			return false;
		}

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile m = state.Mobile;
            int x = info.ButtonID;
            TextRelay tr1 = info.GetTextEntry(1);
            TextRelay tr2 = info.GetTextEntry(2);

            if (x >= 300000)
            {
                int itemID = x - 300000;
                m.SendGump(new ImageDetailsGump(itemID, m_Search));
            }
            else if (x >= 200000)
            {
                int itemID = x - 200000;
                string[] subArgs = new string[2];
                subArgs[0] = "static";
                subArgs[1] = Convert.ToString(itemID);
                BoundingBoxPicker.Begin(m, new BoundingBoxCallback(TileBox_Callback), new TileState(subArgs));
            }
            else if (x >= 100000)
            {
                int itemID = x - 100000;
                m.Target = new CreateItemTarget(itemID, m_Search, m_Index, m_ShowImage);
            }
            else if (x == 50) m.SendGump(new SearchImageGump(m_Index, m_Search, !m_ShowImage)); //Show or Hide images
            else if (x == 1)
            {
                m.CloseGump(typeof(SearchImageGump));
                m.SendGump(new BrowseImageNamesGump(0));
            }
            else if (x == 2 && tr1 != null)
            {
                //Try to read the text typed in the search box.
                string temp = "";
                try { temp = tr1.Text; }
                catch { }

                //If no text found, send an error, and re-display the gump.
                if (temp.Length < 1)
                {
                    m.SendMessage("Please enter the search string.");
                    m.SendGump(new SearchImageGump(m_Search));
                }
                else m.SendGump(new SearchImageGump(temp));
            }
            else if (x == 5 && tr2 != null)
            {
                //Try to interpret the number typed in the browse box.
                int temp = 0;
                try { temp = Convert.ToInt32(tr2.Text, 10); }
                catch { }

                //If out of range, send an error, and re-display the gump.
                if (temp > TileData.ItemTable.Length || temp < 1)
                {
                    m.SendMessage("Please enter a decimal number between 1 and TileData.ItemTable.Length.");
                    m.SendGump(new SearchImageGump(m_Index, m_Search, m_ShowImage));
                }
                else m.SendGump(new SearchImageGump(temp, "ALL", m_ShowImage));
            }
            else if (x == 3) m.SendGump(new SearchImageGump(m_Index - 10, m_Search, m_ShowImage)); //Previous Page
            else if (x == 4) m.SendGump(new SearchImageGump(m_Index + 10, m_Search, m_ShowImage)); //Next Page
            else if (x == 11) m.SendGump(new SearchImageGump(0, "WoodWalls"));
            else if (x == 12) m.SendGump(new SearchImageGump(0, "StoneWalls"));
            else if (x == 13) m.SendGump(new SearchImageGump(0, "MarbleWalls"));
            else if (x == 14) m.SendGump(new SearchImageGump(0, "PlasterWalls"));
            else if (x == 15) m.SendGump(new SearchImageGump(0, "ElvenWalls"));
            else if (x == 16) m.SendGump(new SearchImageGump(0, "OtherWalls"));
            else if (x == 21) m.SendGump(new SearchImageGump(0, "SecretDoors"));
            else if (x == 22) m.SendGump(new SearchImageGump(0, "Gates"));
            else if (x == 23) m.SendGump(new SearchImageGump(0, "NormalDoors"));
            else if (x == 24) m.SendGump(new SearchImageGump(0, "ElvenDoors"));
            else if (x == 25) m.SendGump(new SearchImageGump(0, "CrystalDoors"));
            else if (x == 26) m.SendGump(new SearchImageGump(0, "SamuraiDoors"));
            else if (x == 31) m.SendGump(new SearchImageGump(0, "PavedFloors"));
            else if (x == 32) m.SendGump(new SearchImageGump(0, "TileFloors"));
            else if (x == 33) m.SendGump(new SearchImageGump(0, "WoodFloors"));
            else if (x == 34) m.SendGump(new SearchImageGump(0, "RusticFloors"));
            else if (x == 35) m.SendGump(new SearchImageGump(0, "TatamiFloors"));
            else if (x == 36) m.SendGump(new SearchImageGump(0, "OtherFloors"));
            else if (x == 41) m.SendGump(new SearchImageGump(0, "RusticRoofs"));
            else if (x == 42) m.SendGump(new SearchImageGump(0, "CeramicRoofs"));
            else if (x == 43) m.SendGump(new SearchImageGump(0, "HardRoofs"));
            else if (x == 44) m.SendGump(new SearchImageGump(0, "WoodenRoofs"));
            else if (x == 45) m.SendGump(new SearchImageGump(0, "ElvenRoofs"));
            else if (x == 46) m.SendGump(new SearchImageGump(0, "NewAgeRoofs"));
            else if (x == 61) m.SendGump(new SearchImageGump(0, "MiscArches"));
            else if (x == 62) m.SendGump(new SearchImageGump(0, "VariousMisc"));
            else if (x == 63) m.SendGump(new SearchImageGump(0, "RoundedWalls"));
            else if (x == 64) m.SendGump(new SearchImageGump(0, "Battlements"));
            else if (x == 65) m.SendGump(new SearchImageGump(0, "MiscRoofs"));
            else if (x == 66) m.SendGump(new SearchImageGump(0, "MiscFences"));
            else if (x == 51) m.SendGump(new SearchImageGump(0, "Stairs"));
            else if (x == 71) m.SendGump(new SearchImageGump(0, "Chairs"));
            else if (x == 72) m.SendGump(new SearchImageGump(0, "Storage"));
            else if (x == 73) m.SendGump(new SearchImageGump(0, "Tables"));
            else if (x == 74) m.SendGump(new SearchImageGump(0, "Beds"));
            else if (x == 75) m.SendGump(new SearchImageGump(0, "Shelves"));
            else if (x == 76) m.SendGump(new SearchImageGump(0, "Dressers"));
            else if (x == 81) m.SendGump(new SearchImageGump(0, "Trees"));
            else if (x == 82) m.SendGump(new SearchImageGump(0, "Plants"));
            else if (x == 83) m.SendGump(new SearchImageGump(0, "Rocks"));
            else if (x == 84) m.SendGump(new SearchImageGump(0, "Animals"));
            else if (x == 85) m.SendGump(new SearchImageGump(0, "Monsters"));
            else if (x == 86) m.SendGump(new SearchImageGump(0, "Water"));
            else if (x == 91) m.SendGump(new SearchImageGump(0, "LightSource"));
            else if (x == 92) m.SendGump(new SearchImageGump(0, "Weapons"));
            else if (x == 93) m.SendGump(new SearchImageGump(0, "Armor"));
            else if (x == 94) m.SendGump(new SearchImageGump(0, "Tools"));
            else if (x == 95) m.SendGump(new SearchImageGump(0, "Bottles"));
            else if (x == 96) m.SendGump(new SearchImageGump(0, "Containers"));
            else if (x == 101) m.SendGump(new SearchImageGump(0, "Banners"));
            else if (x == 102) m.SendGump(new SearchImageGump(0, "Signs"));
            else if (x == 103) m.SendGump(new SearchImageGump(0, "Stones"));
            else if (x == 104) m.SendGump(new SearchImageGump(0, "Consumables"));
            else if (x == 105) m.SendGump(new SearchImageGump(0, "Debris"));
            else if (x == 106) m.SendGump(new SearchImageGump(0, "Symbols"));
            else m.CloseGump(typeof(SearchImageGump));
        }

		private void TileBox_Callback( Mobile from, Map map, Point3D start, Point3D end, object state )
		{
			TileState ts = (TileState)state;
			if ( ts.m_UseFixedZ ) start.Z = end.Z = ts.m_FixedZ;
			Server.Commands.Add.Invoke( from, start, end, ts.m_Args );
            from.SendGump(new SearchImageGump(m_Index, m_Search, m_ShowImage));
		}

		private class TileState
		{
			public bool m_UseFixedZ;
			public int m_FixedZ;
			public string[] m_Args;

			public TileState( string[] args ) : this( false, 0, args ) { }

			public TileState( int fixedZ, string[] args ) : this( true, fixedZ, args ) { }

			public TileState( bool useFixedZ, int fixedZ, string[] args )
			{
				m_UseFixedZ = useFixedZ;
				m_FixedZ = fixedZ;
				m_Args = args;
			}
		}

		private class CreateItemTarget : Target
		{
			private int m_ItemID;
			private string m_Search;
            private int mIndex;
            private bool mShowImage;

			public CreateItemTarget(int itemID, string search, int index, bool showImage) : base(18, true, TargetFlags.None)
			{
				m_ItemID = itemID;
				m_Search = search;
                mIndex = index;
                mShowImage = showImage;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				IPoint3D target = targeted as IPoint3D;
				if (target == null) return;
				Map map = from.Map;
				if (map == null) return;
				Point3D location = new Point3D(target);
				if (target is StaticTarget) location.Z -= TileData.ItemTable[((StaticTarget)target).ItemID & 0x3FFF].CalcHeight;
				Item newItem = new Static(m_ItemID);
				newItem.MoveToWorld(location, from.Map);
				from.Target = new CreateItemTarget(m_ItemID, m_Search, mIndex, mShowImage);
			}

			protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
			{
                from.SendGump(new SearchImageGump(mIndex, m_Search, mShowImage));
				base.OnTargetCancel(from, cancelType);
			}
		}
	}
}
