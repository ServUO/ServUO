using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;

namespace Knives.TownHouses
{
    public delegate void GumpStateCallback(object obj);
    public delegate void GumpCallback();

	public abstract class GumpPlusLight : Gump
	{
        public static void RefreshGump(Mobile m, Type type)
        {
            if (m.NetState == null)
                return;

            foreach (Gump g in m.NetState.Gumps)
                if (g is GumpPlusLight && g.GetType() == type)
                {
                    m.CloseGump(type);
                    ((GumpPlusLight)g).NewGump();
                    return;
                }
        }

        private Mobile c_Owner;
		private Hashtable c_Buttons, c_Fields;

		public Mobile Owner{ get{ return c_Owner; } }

		public GumpPlusLight( Mobile m, int x, int y ) : base( x, y )
		{
			c_Owner = m;

			c_Buttons = new Hashtable();
			c_Fields = new Hashtable();

            Timer.DelayCall(TimeSpan.FromSeconds(0), new TimerCallback(NewGump));
		}

		public void Clear()
		{
			Entries.Clear();
			c_Buttons.Clear();
			c_Fields.Clear();
		}

		public virtual void NewGump()
		{
			NewGump( true );
		}

		public void NewGump( bool clear )
		{
			if ( clear )
				Clear();

			BuildGump();

			c_Owner.SendGump( this );
		}

		public void SameGump()
		{
			c_Owner.SendGump( this );
		}

        protected abstract void BuildGump();

		private int UniqueButton()
		{
			int random = 0;

			do
			{
				random = Utility.Random( 20000 );

			}while( c_Buttons[random] != null );

			return random;
		}

        private int UniqueTextId()
        {
            int random = 0;

            do
            {
                random = Utility.Random(20000);

            } while (c_Buttons[random] != null);

            return random;
        }

        public void AddBackgroundZero(int x, int y, int width, int height, int back)
        {
            AddBackgroundZero(x, y, width, height, back, true);
        }

        public void AddBackgroundZero(int x, int y, int width, int height, int back, bool over)
        {
            BackgroundPlus plus = new BackgroundPlus(x, y, width, height, back, over);

            Entries.Insert(0, plus);
        }

        public new void AddBackground(int x, int y, int width, int height, int back)
        {
            AddBackground(x, y, width, height, back, true);
        }

        public void AddBackground(int x, int y, int width, int height, int back, bool over)
        {
            BackgroundPlus plus = new BackgroundPlus(x, y, width, height, back, over);

            Add(plus);
        }

        public void AddButton(int x, int y, int id, GumpCallback callback)
        {
            AddButton(x, y, id, id, "None", callback);
        }

        public void AddButton(int x, int y, int id, GumpStateCallback callback, object arg)
        {
            AddButton(x, y, id, id, "None", callback, arg);
        }

        public void AddButton(int x, int y, int id, string name, GumpCallback callback)
        {
            AddButton(x, y, id, id, name, callback);
        }

        public void AddButton(int x, int y, int id, string name, GumpStateCallback callback, object arg)
        {
            AddButton(x, y, id, id, name, callback, arg);
        }

        public void AddButton(int x, int y, int up, int down, GumpCallback callback)
        {
            AddButton(x, y, up, down, "None", callback);
        }

        public void AddButton(int x, int y, int up, int down, string name, GumpCallback callback)
		{
			int id = UniqueButton();

			ButtonPlus button = new ButtonPlus( x, y, up, down, id, name, callback );

			Add( button );

			c_Buttons[id] = button;
		}

		public void AddButton( int x, int y, int up, int down, GumpStateCallback callback, object arg )
		{
			AddButton( x, y, up, down, "None", callback, arg );
		}

		public void AddButton( int x, int y, int up, int down, string name, GumpStateCallback callback, object arg )
		{
			int id = UniqueButton();

			ButtonPlus button = new ButtonPlus( x, y, up, down, id, name, callback, arg );

			Add( button );

			c_Buttons[id] = button;
		}

        public void AddHtml(int x, int y, int width, string text)
        {
            AddHtml(x, y, width, 21, HTML.White + text, false, false, true);
        }

        public void AddHtml(int x, int y, int width, string text, bool over)
        {
            AddHtml(x, y, width, 21, HTML.White + text, false, false, over);
        }

        public new void AddHtml(int x, int y, int width, int height, string text, bool back, bool scroll)
        {
            AddHtml(x, y, width, height, HTML.White + text, back, scroll, true);
        }

        public void AddHtml(int x, int y, int width, int height, string text, bool back, bool scroll, bool over)
        {
            HtmlPlus html = new HtmlPlus(x, y, width, height, HTML.White + text, back, scroll, over);

            Add(html);
        }

        public void AddTextField(int x, int y, int width, int height, int color, int back, string name, string text)
		{
            int id = UniqueTextId();

            AddImageTiled(x, y, width, height, back);
            base.AddTextEntry(x, y, width, height, color, id, text);

            c_Fields[id] = name;
            c_Fields[name] = text;
		}

		public string GetTextField( string name )
		{
			if ( c_Fields[name] == null )
				return "";

			return c_Fields[name].ToString();
		}

        public int GetTextFieldInt(string name)
        {
            return Utility.ToInt32(GetTextField(name));
        }

		protected virtual void OnClose()
		{
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			string name = "";

            try
            {
                if (info.ButtonID == -5)
                {
                    NewGump();
                    return;
                }

                foreach (TextRelay t in info.TextEntries)
                    c_Fields[c_Fields[t.EntryID].ToString()] = t.Text;

                if (info.ButtonID == 0)
                    OnClose();

                if (c_Buttons[info.ButtonID] == null || !(c_Buttons[info.ButtonID] is ButtonPlus))
                    return;

                name = ((ButtonPlus)c_Buttons[info.ButtonID]).Name;

                ((ButtonPlus)c_Buttons[info.ButtonID]).Invoke();

            }
            catch (Exception e)
            {
                Errors.Report("An error occured during a gump response.  More information can be found on the console.");
                if (name != "")
                    Console.WriteLine("{0} gump name triggered an error.", name);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}