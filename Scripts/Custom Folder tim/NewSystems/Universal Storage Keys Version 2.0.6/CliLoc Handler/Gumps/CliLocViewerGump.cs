using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using Server;
using Server.Items;
using Server.Misc;
using Server.Commands;
using Server.Network;
using Solaris.CliLocHandler;

namespace Server.Gumps
{
    //CliLocViewer gump - displays a list of active CliLoc entries. Can be limited by a Filter string
    public class CliLocViewerGump : Gump
    {
        protected int _Width = 750;
        protected int _Height = 400;

        protected int _EntriesPerPage = 14;

        protected int _X = 10;
        protected int _Y = 10;

        protected int _Start;
        protected int _End;

        protected Hashtable _FilteredCliLocs;

        //used for the seek field
        protected int _TopNumber;

        protected Mobile _Owner;
        protected string _FilterText;
        protected int _Page;
        protected int _MaxPages;

        public Mobile Owner { get { return _Owner; } }
        public string FilterText { get { return _FilterText; } }
        public Hashtable FilteredCliLocs { get { return _FilteredCliLocs; } }
        public int Page { get { return _Page; } }

        public CliLocViewerGump(CliLocViewerGump oldgump) : this(oldgump.Owner,oldgump.FilterText,oldgump.Page)
        {
        }

        public CliLocViewerGump(Mobile owner) : this(owner,null)
        {
        }

        public CliLocViewerGump(Mobile owner,string filtertext) : this(owner,filtertext,0)
        {
        }

        public CliLocViewerGump(Mobile owner,string filtertext,int page) : base(10,10)
        {
            _Owner = owner;
            _FilterText = filtertext;
            _Page = page;

            _FilteredCliLocs = GetFilteredCliLocs(filtertext);

            _Owner.CloseGump(typeof(CliLocViewerGump));

            DetermineLayout();

            DrawPage();

            AddExportButton();

            DrawEntries();

            DrawControls();
        }

        protected void DetermineLayout()
        {
            _MaxPages = _FilteredCliLocs.Count / _EntriesPerPage + 1;

            _Page = Math.Max(0,Math.Min(_Page,_MaxPages - 1));

            _Start = _Page * _EntriesPerPage;

            _End = Math.Min((_Page + 1) * _EntriesPerPage,_FilteredCliLocs.Count);
        }

        protected void DrawPage()
        {
            AddBackground(0,0,_Width,_Height,9270);
            AddImageTiled(11,10,_Width - 23,_Height - 20,2624);
            AddAlphaRegion(11,10,_Width - 22,_Height - 20);

            AddLabel(_X,_Y,88,"Client Locallization - Data viewer");
        }

        protected void DrawEntries()
        {
            AddLabel(_X,_Y += 30,1152,"Number");
            AddLabel(_X + 150,_Y,1152,"Value");

            _Y += 10;

            IDictionaryEnumerator enumerator = _FilteredCliLocs.GetEnumerator();

            //seek ahead until you reach the start
            for (int i = 0; i < _Start; i++)
            {
                enumerator.MoveNext();
            }

            for (int i = _Start; i < _End; i++)
            {
                if (enumerator.MoveNext())
                {
                    if (i == _Start)
                    {
                        _TopNumber = (int)enumerator.Key;
                    }
                    AddLabel(_X,_Y += 20,1152,enumerator.Key.ToString());
                    AddLabel(_X + 150,_Y,1152,enumerator.Value.ToString());
                }
            }
        }

        protected void DrawControls()
        {
            AddPageButtons();

            _Y = _Height - 50;

            //TODO: fix this if it is considered useful
            //AddLabel( _X, _Y, 88, "Seek to Number:" );
            //AddTextField( _X + 80, _Y, 100, 20, 1, _TopNumber.ToString() );
            //AddButton( _X + 200, _Y, 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0 );

            AddLabel(_X + 300,_Y,88,"Filter:");
            AddTextField(_X + 350,_Y,100,20,2,_FilterText);
            AddButton(_X + 470,_Y,0x15E1,0x15E5,2,GumpButtonType.Reply,0);
        }

        protected void AddPageButtons()
        {
            //page buttons
            _Y = _Height - 30;

            if (_Page > 0)
            {
                AddButton(20,_Y,0x15E3,0x15E7,4,GumpButtonType.Reply,0);
            }
            else
            {
                AddImage(20,_Y,0x25EA);
            }
            AddLabel(40,_Y,88,"Previous Page");

            if (_Page < _MaxPages - 1)
            {
                AddButton(_Width - 40,_Y,0x15E1,0x15E5,5,GumpButtonType.Reply,0);
            }
            else
            {
                AddImage(_Width - 40,_Y,0x25E6);
            }
            AddLabel(_Width - 120,_Y,88,"Next Page");

            AddLabel(_Width / 2 - 10,_Y,88,String.Format("({0}/{1})",_Page + 1,_MaxPages));
        }

        public void AddExportButton()
        {
            AddLabel(_Width - 120,10,88,"Export");
            AddButton(_Width - 40,10,0x15E1,0x15E5,19,GumpButtonType.Reply,0);
        }

        public void AddTextField(int x,int y,int width,int height,int index,string text)
        {
            AddImageTiled(x - 2,y - 2,width + 4,height + 4,0xA2C);
            AddAlphaRegion(x - 2,y - 2,width + 4,height + 4);
            AddTextEntry(x + 2,y + 2,width - 4,height - 4,1153,index,text);
        }

        public string GetTextField(RelayInfo info,int index)
        {
            TextRelay relay = info.GetTextEntry(index);
            return (relay == null ? null : relay.Text.Trim());
        }

        public override void OnResponse(NetState sender,RelayInfo info)
        {
            //store flags
            int buttonid = info.ButtonID;

            //right click
            if (buttonid == 0)
            {
                return;
            }

            if (buttonid == 4 && _Page > 0)
            {
                _Page -= 1;
            }

            else if (buttonid == 5 && _Page < _MaxPages - 1)
            {
                _Page += 1;
            }

            else if (buttonid == 19)
            {
                if (_FilteredCliLocs.Count < 1)
                {
                    _Owner.SendMessage("Unable to export less than 1 entry.");
                }
                else
                {
                    try
                    {
                        StreamWriter writer = new StreamWriter("output.txt");

                        IDictionaryEnumerator enumerator = CliLoc.CliLocs.GetEnumerator();

                        //seek ahead until you reach the entry

                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Value.ToString().ToLower().IndexOf(FilterText.ToLower()) > -1)
                            {
                                writer.WriteLine("{0}\t{1}",enumerator.Key.ToString(),enumerator.Value.ToString());
                            }
                        }
                        writer.Close();
                    }
                    catch (Exception e) { Console.WriteLine("Error in Exporting CliLocs: {0}",e.Message); }
                }
            }
            else if (buttonid == 1)     //seek to number controls.  Buggy I think
            {
                try
                {
                    int seeknumber = int.Parse(GetTextField(info,1));

                    if (_FilteredCliLocs.ContainsKey(seeknumber))
                    {
                        IDictionaryEnumerator enumerator = _FilteredCliLocs.GetEnumerator();

                        //seek ahead until you reach the entry
                        int index = 0;
                        while ((int)enumerator.Key != seeknumber)
                        {
                            index++;
                            enumerator.MoveNext();
                        }
                        _Page = index / _EntriesPerPage;
                    }
                }
                catch
                {
                }
            }
            else if (buttonid == 2)
            {
                _FilterText = GetTextField(info,2);
            }

            _Owner.SendGump(new CliLocViewerGump(this));
        }

        protected static Hashtable GetFilteredCliLocs(string filterstring)
        {
            if (filterstring == null)
            {
                return CliLoc.CliLocs;
            }

            Hashtable filtered = new Hashtable();

            IDictionaryEnumerator enumerator = CliLoc.CliLocs.GetEnumerator();

            //seek ahead until you reach the entry

            while (enumerator.MoveNext())
            {
                if (enumerator.Value.ToString().ToLower().IndexOf(filterstring.ToLower()) > -1)
                {
                    filtered.Add((int)enumerator.Key,enumerator.Value.ToString());
                }
            }

            return filtered;
        }
    }
}