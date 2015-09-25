using System;
using Server;
using Server.Prompts;

namespace Server.Multis
{
	public class NewRenameBoatPrompt : Prompt
	{
		private BaseShip _boat;

		public NewRenameBoatPrompt(BaseShip boat)
		{
			_boat = boat;
		}

		public override void OnResponse(Mobile from, string text)
		{
			_boat.EndRename(from, text);
		}
	}
}