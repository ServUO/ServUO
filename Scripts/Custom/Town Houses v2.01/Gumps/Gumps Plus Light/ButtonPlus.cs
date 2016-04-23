using System;
using Server;
using Server.Gumps;

namespace Knives.TownHouses
{
	public class ButtonPlus : GumpButton
	{
		private string c_Name;
		private object c_Callback;
		private object c_Param;

		public string Name{ get{ return c_Name; } }

		public ButtonPlus( int x, int y, int normalID, int pressedID, int buttonID, string name, GumpCallback back ) : base( x, y, normalID, pressedID, buttonID, GumpButtonType.Reply, 0 )
		{
			c_Name = name;
			c_Callback = back;
			c_Param = "";
		}

		public ButtonPlus( int x, int y, int normalID, int pressedID, int buttonID, string name, GumpStateCallback back, object param ) : base( x, y, normalID, pressedID, buttonID, GumpButtonType.Reply, 0 )
		{
			c_Name = name;
			c_Callback = back;
			c_Param = param;
		}

		public void Invoke()
		{
			if ( c_Callback is GumpCallback )
				((GumpCallback)c_Callback)();
			else if ( c_Callback is GumpStateCallback )
				((GumpStateCallback)c_Callback)( c_Param );
		}
	}
}