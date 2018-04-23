using System;
using OrbServerSDK;
using UOArchitectInterface;
using Server.Engines.OrbRemoteServer;

namespace Server.Scripts.UOArchitect
{
	public class MoveItemsCommand : OrbCommand 
	{
		public static void Initialize()
		{
			OrbServer.Register("UOAR_MoveItems", typeof(MoveItemsCommand), AccessLevel.GameMaster, true);
		}

		public override void OnCommand(OrbClientInfo client, OrbCommandArgs cmdArgs)
		{
			if(cmdArgs == null || !(cmdArgs is MoveItemsArgs) )
				return;

			MoveItemsArgs args = (MoveItemsArgs)cmdArgs;

			if(args.Count > 0)
			{
				int xoffset = args.Xoffset;
				int yoffset = args.Yoffset;
				int zoffset = args.Zoffset;

				int[] serials = args.ItemSerials;

				for(int i=0; i < serials.Length; ++i)
				{
					Item item = World.FindItem(serials[i]);

					if(item == null)
						continue;

					int newX = item.X + xoffset;
					int newY = item.Y + yoffset;
					int newZ = item.Z + zoffset;

					item.Location = new Point3D(newX, newY, newZ);
				}
			}
		}
	}
}
