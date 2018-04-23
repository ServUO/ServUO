//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//			   This is a Sunny Production ©2005				    \\
//				     Based on RunUO©						    \\
//				    Version: Beta 1.1							\\
//	    Many thanks to my Csharp tutors Will and Eclipse	    \\
//////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
//
using System;
using Server.Network;
using System.Collections;

namespace Server.Gumps
{
    public class AdvGump : Gump
    {
        public AdvGump() : this(0, 0)
        {
        }

        public AdvGump(int x, int y) : base(x, y)
        {
        }

        public void AddTable(int x, int y, int[] collumns, ArrayList data, string[] color)
        {
            AddTable(x, y, collumns, data, color, 0, 0);
        }

        public void AddTable(int x, int y, int[] collumns, ArrayList data, string[] color, int style, int bordersize)
        {
            CreateTable(x, y, collumns, data, color, style, bordersize, true, true);
        }

        public void AddTable(int x, int y, int[] collumns, string[] color, ArrayList data, int style, int bordersize)
        {
            CreateTable(x, y, collumns, data, color, style, bordersize, false, false);
        }

        private void CreateTable(int x, int y, int[] collumns, ArrayList data, string[] color, int style, int bordersize, bool cutwords, bool lettermult)
        {
            bool option2 = false;
            bool option3 = false;
            int widthmult = 1;
            if (lettermult)
                widthmult = 8;
            int CollLength = collumns.GetLength(0);
            string[] StringArray = new string[CollLength];
            int TotWidth = bordersize;
            foreach (int colwidth in collumns)
                TotWidth += (colwidth * widthmult + bordersize);
            const int RowHeight = 18;
            int Rows = (int)(data.Count / CollLength);
            int Height = RowHeight * Rows;


            if (CollLength < 1 || CollLength != color.GetLength(0))
                return;

            else
            {
                switch (style)
                {
                    default:
                    case 0: break;

                    case 1://was 6 ==table with 2 lines
                        AddImageTiled(x + collumns[0] * widthmult + bordersize, y, bordersize, Height + bordersize, 0x0A40);
                        AddImageTiled(x, y + RowHeight, TotWidth, bordersize, 0x0A40);
                        break;


                    case 20://was 2 ==table with collums met gaasje
                        int X2 = x;
                        int Y2 = y;

                        int startrow = 0;
                        if(option2)
                            startrow = 1;

                        for (int j = startrow; j < Rows; j++)
                        {
                            for (int i = 0; i < CollLength; i++)
                            {
                                if (!option2 || option2 && i != 0)
                                {
                                    if (option3)
                                        AddImageTiled(X2, Y2 + j * RowHeight + bordersize, collumns[i] * widthmult, RowHeight, 0x0A40);

                                    AddAlphaRegion(X2, Y2 + j * RowHeight + bordersize, collumns[i] * widthmult, RowHeight);
                                }
                            X2 += (collumns[i] * widthmult + bordersize);
                            }
                            X2 = x;
                        }

                        if (option2)
                            goto case 1;
                        break;

                    case 21:// zwart gaasje
                        option3 = true;
                        goto case 20;

                    case 22://was 3 ==table with collums met gaasje and 2 lines
                        option2 = true;
                        goto case 20;

                    case 23:// ==table with collums met "zwart" gaasje and 2 lines
                        option2 = true;
                        goto case 21;

                    case 30://was 4 ==table met om de rij gaasje
                        for (int i = 1; i < Rows; i += 2)
                        {
                            if (option2)
                            {
                                if (option3)
                                    AddImageTiled(x + collumns[0] * widthmult + bordersize, y + i * RowHeight + bordersize, TotWidth - bordersize * 4 - collumns[0] * widthmult, RowHeight - bordersize, 0x0A40);

                                AddAlphaRegion(x + collumns[0] * widthmult + bordersize, y + i * RowHeight + bordersize, TotWidth - bordersize * 4 - collumns[0] * widthmult, RowHeight - bordersize);
                            }
                            else
                            {
                                if (option3)
                                    AddImageTiled(x + bordersize, y + i * RowHeight + bordersize, TotWidth - bordersize, RowHeight - bordersize, 0x0A40);

                                AddAlphaRegion(x + bordersize, y + i * RowHeight + bordersize, TotWidth - bordersize, RowHeight - bordersize);
                            }
                        }
                        if (option2)
                            goto case 1;
                        break;

                    case 31://==zwart gaasje
                        option3 = true;
                        goto case 30;

                    case 32://was 5 ==table met om de rij gaasje eerste tabel niet en twee lijntjes
                        option2 = true;
                        goto case 30;

                    case 33://==table met om de rij zwart gaasje eerste tabel niet en twee lijntjes
                        option2 = true;
                        goto case 31;

                    case 100://was 1 like propsgump
                        AddImageTiled(x, y, TotWidth, Height + 2, 9354);

                        int X1 = x;
                        int Y1 = y;

                        for (int i = 0; i <= Rows; i++)
                            AddImageTiled(X1, Y1 + i * RowHeight, TotWidth, bordersize, 0x0A40);

                        AddImageTiled(X1, Y1, bordersize, Height, 0x0A40);

                        for (int i = 0; i < CollLength; i++)
                        {
                            X1 += (collumns[i] * widthmult + bordersize);
                            AddImageTiled(X1, Y1, bordersize, Height, 0x0A40);
                        }
                        break;
                }

                int coll = 0;

                foreach (string str in data)
                {
                    string datastr = str;
                    if (cutwords)
                        if (datastr.Length > collumns[coll])
                            datastr = datastr.Substring(0, collumns[coll]);

                    StringArray[coll] = StringArray[coll] + datastr + "<br>";
                    coll++;
                    if (coll == CollLength)
                        coll = 0;
                }

                int collwidth = 0;

                for (int i = 0; i < CollLength; i++)
                {
                    StringArray[i] = Colorize(StringArray[i], color[i]);
                    int width = collumns[i] * widthmult;
                    AddHtml(x + collwidth + bordersize + 1, y + bordersize - 1, width, Height, StringArray[i], false, false);
                    collwidth += (width + bordersize);
                }
            }
        }

//From here on extra gump features.\\

        public void DeleteGump(NetState state)
        {
            if (state != null)
                state.Gumps.Remove((Gump)this);
        }

        public static void DeleteAllGumpsOfType(Mobile m, Type type)
        {
            DeleteAllGumpsOfType(m.NetState, type);
        }

        public static void DeleteAllGumpsOfType(NetState state, Type type)
        {
            if (state != null)
            {
                ArrayList delarr = new ArrayList();

                foreach (Gump gump in state.Gumps)
                    if (gump.GetType() == type)
                        delarr.Add(gump);

                foreach (Gump delgump in delarr)
                    if (delgump != null)
                        state.Gumps.Remove(delgump);
            }
        }

        public static Gump FindGump(Mobile m, Type type)
        {
            if (m != null && m.NetState != null)
                foreach (Gump gump in m.NetState.Gumps)
                    if (gump.GetType() == type)
                        return gump;

            return null;
        }

        public string Bolden(string str)
        {
            return String.Format("<b>{0}</b>", str);
        }

        public string Center(string str)
        {
            return String.Format("<center>{0}</center>", str);
        }

        public string Colorize(string str, string color)
        {
            if (color == "")
                return str;

            return String.Format("<basefont color=#{0}>{1}</basefont>", color, str);
        }
    }
}