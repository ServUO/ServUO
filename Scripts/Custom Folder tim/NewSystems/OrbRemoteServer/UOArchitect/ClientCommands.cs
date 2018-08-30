using System;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Multis;
using Server.Engines.UOArchitect;
using Server.Commands;

namespace System.Scripts.Commands
{
	public class ClientCommands
	{
		public static void Initialize()
		{
            CommandSystem.Register("NudgeSelfDown", AccessLevel.GameMaster, new CommandEventHandler(NudgeSelfDown_OnCommand));
            CommandSystem.Register("NudgeSelfUp", AccessLevel.GameMaster, new CommandEventHandler(NudgeSelfUp_OnCommand));
		}

		[Usage( "NudgeSelfDown <amount>")]
		[Description( "Decreases your z by the specified amount." )]
		private static void NudgeSelfDown_OnCommand( CommandEventArgs e )
		{
			if(e.Arguments.Length > 0)
			{
				int zoffset = e.GetInt32(0);
				e.Mobile.Location = new Point3D(e.Mobile.Location, e.Mobile.Location.Z - zoffset);
			}
		}

		[Usage( "NudgeSelfUp <amount>")]
		[Description( "Increases your z by the specified amount." )]
		private static void NudgeSelfUp_OnCommand( CommandEventArgs e )
		{
			if(e.Arguments.Length > 0)
			{
				int zoffset = e.GetInt32(0);
				e.Mobile.Location = new Point3D(e.Mobile.Location, e.Mobile.Location.Z + zoffset);
			}
		}

	}

	public class MultiDeleteCommand
	{
		private static Mobile _mobile = null;

		public static void Initialize()
		{
            CommandSystem.Register("MRemove", AccessLevel.GameMaster, new CommandEventHandler(MultiRemove_OnCommand));
		}

		[Usage( "MRemove")]
		[Description( "Allows you to delete items until you press ESC." )]
		private static void MultiRemove_OnCommand( CommandEventArgs e )
		{
			UOAR_ObjectTarget target = new UOAR_ObjectTarget();
			target.OnTargetObject += new UOAR_ObjectTarget.TargetObjectEvent(OnTargetObject);
				
			_mobile = e.Mobile;

			e.Mobile.SendMessage("Target items to delete them. Press ESC to stop.");
			// send the target to the char
			e.Mobile.Target = target;

		}

		private static void OnTargetObject(object obj)
		{
			if( (obj is Item)  &&  !((obj is BaseMulti) || (obj is HouseSign)) )
			{
				(obj as Item).Delete();
			}
			else
			{
				_mobile.SendMessage("You can't delete this object.");
			}

			UOAR_ObjectTarget target = new UOAR_ObjectTarget();
			target.OnTargetObject += new UOAR_ObjectTarget.TargetObjectEvent(OnTargetObject);
			
			// send the target to the char
			_mobile.Target = target;
		}
	}
}
