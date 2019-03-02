/* Copyright (C) 2013 Ian Karlinsey
 * 
 * UltimeLive is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * UltimaLive is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with UltimaLive.  If not, see <http://www.gnu.org/licenses/>. 
 */

using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UltimaLive
{
    public class IncStaticAltCommand : BaseCommand
    {
        public IncStaticAltCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "IncStaticAlt" };
            ObjectTypes = ObjectTypes.Statics;
            Usage = "IncStaticAlt";
            Description = "Increases the Z value of a static.";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            bool retVal = true;
            if (e.Length != 1)
            {
                e.Mobile.SendMessage("You must specify an amount to change the z coord.");
                retVal = false;
            }
            return retVal;
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("Command IncStaticAlt confirmed");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("Command IncStaticAlt cancelled");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You're going to change height to {0} statics. This action can't be reverted once confirmed, without a server reload before a save, or by deleting the correct .live file after a save!<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Awaiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int change = e.GetInt32(0);

            StaticTarget st = obj as StaticTarget;
            if (st!=null)
            {
                
                new IncStaticAltitude(e.Mobile.Map.MapID, st, change).DoOperation();
            }
        }
    }

    public class SetStaticAltCommand : BaseCommand
    {
        public SetStaticAltCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            //Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
            Supports = CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "SetStaticAlt" };
            ObjectTypes = ObjectTypes.Statics;
            Usage = "SetStaticAlt";
            Description = "Set the Z value of a static.";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            bool retVal = true;
            if (e.Length != 1)
            {
                e.Mobile.SendMessage("You must specify the z coord.");
                retVal = false;
            }
            return retVal;
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("Command SetStaticAlt confirmed");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("Command SetStaticAlt cancelled");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You're going to change height to {0} statics. This action can't be reverted once confirmed, without a server reload before a save, or by deleting the correct .live file after a save!<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Awaiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int change = e.GetInt32(0);

            StaticTarget st = obj as StaticTarget;
            if (st != null)
            {
                new SetStaticAltitude(e.Mobile.Map.MapID, st, change).DoOperation();
            }
        }
    }

    public class SetStaticIDCommand : BaseCommand
    {
        public SetStaticIDCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            //Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
            Supports = CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "SetStaticID" };
            ObjectTypes = ObjectTypes.Statics;
            Usage = "SetStaticID";
            Description = "Set the ID value of a static.";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            bool retVal = true;
            if (e.Length != 1)
            {
                e.Mobile.SendMessage("You must specify the Item ID");
                retVal = false;
            }
            return retVal;
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("Command SetStaticID confirmed");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("Command SetStaticID cancelled");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You're going to change ID to {0} statics. This action can't be reverted once confirmed, without a server reload before a save, or by deleting the correct .live file after a save!<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Awaiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int newID;
            if (e.Arguments[0][0] == '^')
            {
                newID = Utility.IntRandomCoded(e.Arguments[0]);
            }
            else
            {
                newID = Utility.ToInt32(e.Arguments[0]);
            }

            if (newID > 1 && newID <= TileData.MaxItemValue)
            {
                StaticTarget st = obj as StaticTarget;
                if (st != null)
                {
                    new SetStaticID(e.Mobile.Map.MapID, st, newID).DoOperation();
                }
            }
        }
    }

    public class SetStaticHueCommand : BaseCommand
    {
        public SetStaticHueCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            //Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
            Supports = CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "SetStaticHue" };
            ObjectTypes = ObjectTypes.Statics;
            Usage = "SetStaticHue";
            Description = "Set the hue value of a static.";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            bool retVal = true;
            if (e.Length != 1)
            {
                e.Mobile.SendMessage("You must specify the hue");
                retVal = false;
            }
            return retVal;
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("Command SetStaticHue confirmed");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("Command SetStaticHue cancelled");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You're going to change HUE to {0} statics. This action can't be reverted once confirmed, without a server reload before a save, or by deleting the correct .live file after a save!<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Awaiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int newHue;
            if (e.Arguments[0][0] == '^')
            {
                newHue = Utility.IntRandomCoded(e.Arguments[0]);
            }
            else
            {
                newHue = Utility.ToInt32(e.Arguments[0]);
            }

            StaticTarget st = obj as StaticTarget;
            if (st != null && newHue >= 0 && newHue <= 0xBB8)
            {
                new SetStaticHue(e.Mobile.Map.MapID, st, newHue).DoOperation();
            }
        }
    }

    public class DelStaticCommand : BaseCommand
    {
        public DelStaticCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            //Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
            Supports = CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "DelStatic" };
            ObjectTypes = ObjectTypes.Statics;
            Usage = "DelStatic";
            Description = "Deletes static.";
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("Command DelStatic confirmed");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("Command DelStatic cancelled");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You're going to delete {0} statics. This action can't be reverted once confirmed, without a server reload before a save, or by deleting the correct .live file after a save!<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Awaiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            StaticTarget st = obj as StaticTarget;
            if (st != null)
            {
                new DeleteStatic(e.Mobile.Map.MapID, st).DoOperation();
            }
        }
    }

    public class AddStaticCommand : BaseCommand
    {
        public AddStaticCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            //Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
            Supports = CommandSupport.Self | CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "addStatic" };
            ObjectTypes = ObjectTypes.Statics;
            Usage = "addStatic itemId Hue [altitude]";
            Description = "Add a static.";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            bool retVal = true;
            if (e.Length < 2 || e.Length > 3)
            {
                e.Mobile.SendMessage("You must specify the Item ID, a Hue, and optionally a Z value");
                retVal = false;
            }
            return retVal;
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("Command AddStatic confirmed");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("Command AddStatic cancelled");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You're going to add {0} statics. This action can't be reverted once confirmed, without a server reload before a save, or by deleting the correct .live file after a save!<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Waiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int newID;
            if (e.Arguments[0][0] == '^')
            {
                newID = Utility.IntRandomCoded(e.Arguments[0]);
            }
            else
            {
                newID = Utility.ToInt32(e.Arguments[0]);
            }

            int newHue;
            if (e.Arguments[1][0] == '^')
            {
                newHue = Utility.IntRandomCoded(e.Arguments[1]);
            }
            else
            {
                newHue = Utility.ToInt32(e.Arguments[1]);
            }

            if (newID > 1 && newID <= TileData.MaxItemValue && newHue >= 0 && newHue <= 0xBB8)
            {
                if (obj is IPoint3D)
                {
                    IPoint3D location = (IPoint3D)obj;
                    int newZ = location.Z;
                    if (e.Length == 3)
                    {
                        newZ = e.GetInt32(2);
                    }
                    Console.WriteLine("adding static in Location {0} {1} {2} Map {3} - static id {4} hue {5}", location.X, location.Y, newZ, e.Mobile.Map, newID, newHue);
                    new AddStatic(e.Mobile.Map.MapID, newID, newZ, location.X, location.Y, newHue).DoOperation();
                }
            }
        }
    }

    public class MoveStaticCommand : BaseCommand
    {
        public MoveStaticCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Single;

            Commands = new string[] { "MoveStatic" };
            ObjectTypes = ObjectTypes.Statics;
            Usage = "MoveStatic";
            Description = "Move a static.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int newID = e.GetInt32(0);

            StaticTarget st = obj as StaticTarget;
            if (st != null)
            {
                if (e.Mobile.Map == st.Map)
                {
                    e.Mobile.Target = new MoveStaticDestinationTarget(st, e.Mobile.Map.MapID);
                }
                else
                {
                    e.Mobile.SendMessage("You can't change location to statics if the map is not the same!");
                }
            }
        }

        private class MoveStaticDestinationTarget : Target
        {
            protected StaticTarget m_StaticTarget;
            protected int m_MapID;
            public MoveStaticDestinationTarget(StaticTarget statTarget, int mapID)
              : base(-1, true, TargetFlags.None)
            {
                m_StaticTarget = statTarget;
                m_MapID = mapID;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (from.Map == null || from.Map == Map.Internal)
                {
                    return;
                }

                if (m_MapID != from.Map.MapID)
                {
                    from.SendMessage("The targets must be in the same map.");
                    return;
                }

                if (o is IPoint3D)
                {
                    IPoint3D location = (IPoint3D)o;
                    MoveStatic ms = new MoveStatic(from.Map.MapID, m_StaticTarget, location.X, location.Y);
                    ms.DoOperation();
                }
            }
        }
    }

    /*public class IncrementStaticPriority : BaseCommand
    {
      public IncrementStaticPriority()
      {
        AccessLevel = AccessLevel.Seer;
        Supports = CommandSupport.Single;

        Commands = new string[] { "IncStaticPriority" };
        ObjectTypes = ObjectTypes.All;
        Usage = "IncStaticPriority";
        Description = "Increment a static priority.";
      }

      public override void Execute(CommandEventArgs e, object obj)
      {
        if (obj is StaticTarget)
        {
          StaticPriority sp = new StaticPriority(e.Mobile.Map.MapID, (StaticTarget)obj, true);
          sp.DoOperation();
        }
      }
    }

    public class LowerStaticPriority : BaseCommand
    {
      public LowerStaticPriority()
      {
        AccessLevel = AccessLevel.Seer;
        Supports = CommandSupport.Single;

        Commands = new string[] { "LowStaticPriority" };
        ObjectTypes = ObjectTypes.All;
        Usage = "LowStaticPriority";
        Description = "Lowers a static priority.";
      }

      public override void Execute(CommandEventArgs e, object obj)
      {
        if (obj is StaticTarget)
        {
          StaticPriority sp = new StaticPriority(e.Mobile.Map.MapID, (StaticTarget)obj, false);
          sp.DoOperation();
        }
      }
    }*/

    public class IncLandAltCommand : BaseCommand
    {
        public IncLandAltCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "IncLandAlt" };
            ObjectTypes = ObjectTypes.Lands;
            Usage = "IncLandAlt";
            Description = "Increase / decrease the Z altitude of a land tile";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            bool retVal = true;
            if (e.Length != 1)
            {
                e.Mobile.SendMessage("You must specify an amount to change the z coord.");
                retVal = false;
            }
            return retVal;
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("Command IncLandAlt confirmed");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("Command IncLandAlt cancelled");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You are going to change height to {0} LAND. This action can't be reverted, without a server reload before a save, or by deleting the correct .live file after a save!<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Waiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int change = e.GetInt32(0);

            if (obj is IPoint3D)
            {
                IPoint3D location = (IPoint3D)obj;
                new IncLandAltitude(location.X, location.Y, e.Mobile.Map.MapID, change).DoOperation();
            }
        }
    }

    public class SetLandAltCommand : BaseCommand
    {
        public SetLandAltCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "SetLandAlt" };
            ObjectTypes = ObjectTypes.Lands;
            Usage = "SetLandAlt";
            Description = "Set the Z altitude of a land tile";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            bool retVal = true;
            if (e.Length != 1)
            {
                e.Mobile.SendMessage("You must specify the z coord.");
                retVal = false;
            }
            return retVal;
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("Command SetLandAlt confirmed");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("Command SetLandAlt cancelled");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You are going to set height to {0} LAND. This action can't be reverted, without a server reload before a save, or by deleting the correct .live file after a save!<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Waiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int change = e.GetInt32(0);

            if (obj is IPoint3D)
            {
                IPoint3D location = (IPoint3D)obj;
                new SetLandAltitude(location.X, location.Y, e.Mobile.Map.MapID, change).DoOperation();
            }
        }
    }

    public class SetLandIDCommand : BaseCommand
    {
        public SetLandIDCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            //Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
            Supports = CommandSupport.Simple | CommandSupport.Area;

            Commands = new string[] { "SetLandID" };
            ObjectTypes = ObjectTypes.Lands;
            Usage = "SetLandID";
            Description = "Set the ID value of a land tile";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            bool retVal = true;
            if (e.Length != 1)
            {
                e.Mobile.SendMessage("You must specify the Item ID");
                retVal = false;
            }
            return retVal;
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            if (okay)
            {
                AddResponse("SetLandID confirmed.");
                base.ExecuteList(e, list);
            }
            else
            {
                AddResponse("SetLandID cancelled.");
            }
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, string.Format("You are going to set ID {0} to LAND. You can't revert, once confirmed, without a server reload or by deleting the relative .live save after the first save.<br><br>Confirm the command?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                AddResponse("Waiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Mobile == null || e.Mobile.Map == null || e.Mobile.Map == Map.Internal)
            {
                return;
            }

            int newID;
            if (e.Arguments[0][0] == '^')
            {
                newID = Utility.IntRandomCoded(e.Arguments[0]);
            }
            else
            {
                newID = Utility.ToInt32(e.Arguments[0]);
            }


            if (newID > 0 && newID <= TileData.MaxLandValue)
            {
                if (obj is IPoint3D)
                {
                    IPoint3D location = (IPoint3D)obj;
                    new SetLandID(location.X, location.Y, e.Mobile.Map.MapID, newID).DoOperation();
                }
            }
        }
    }


    public class Live
    {
        public static void Initialize()
        {
            Register("LiveFreeze", AccessLevel.Seer, new CommandEventHandler(LiveFreeze_OnCommand));
            Register("GetBlockNumber", AccessLevel.Developer, new CommandEventHandler(getBlockNumber_OnCommand));
            Register("QueryClientHash", AccessLevel.Developer, new CommandEventHandler(queryClientHash_OnCommand));
            Register("updateblock", AccessLevel.Developer, new CommandEventHandler(updateBlock_OnCommand));
            Register("CircularIndent", AccessLevel.Developer, new CommandEventHandler(circularIndent_OnCommand));
            TargetCommands.Register(new IncStaticAltCommand());
            TargetCommands.Register(new SetStaticHueCommand());
            TargetCommands.Register(new SetStaticAltCommand());
            TargetCommands.Register(new SetStaticIDCommand());
            TargetCommands.Register(new DelStaticCommand());
            TargetCommands.Register(new AddStaticCommand());
            TargetCommands.Register(new MoveStaticCommand());
            /*TargetCommands.Register(new LowerStaticPriority());
            TargetCommands.Register(new IncrementStaticPriority());*/
            TargetCommands.Register(new IncLandAltCommand());
            TargetCommands.Register(new SetLandAltCommand());
            TargetCommands.Register(new SetLandIDCommand());
        }

        public static void Register(string command, AccessLevel access, CommandEventHandler handler)
        {
            CommandSystem.Register(command, access, handler);
        }

        #region Leveling Target
        public class RadialTarget : Target
        {
            private int m_Height;
            private int m_TType;
            private int m_Radius;

            public int Height
            {
                get { return m_Height; }
                set { m_Height = value; }
            }

            public int TType
            {
                get { return m_TType; }
                set { m_TType = value; }
            }

            public int Radius
            {
                get { return m_Radius; }
                set { m_Radius = value; }
            }

            public RadialTarget(int TType, int Radius, int Height) : base(-1, true, TargetFlags.None)
            {
                m_TType = TType;
                m_Radius = Radius;
                m_Height = Height;
            }

            public override Packet GetPacketFor(NetState ns)
            {
                List<TargetObject> objs = new List<TargetObject>();
                List<Point2D> circle = UltimaLiveUtility.rasterCircle(new Point2D(0, 0), m_Radius);

                foreach (Point2D p in circle)
                {
                    TargetObject t = new TargetObject
                    {
                        ItemID = 0xA12,
                        Hue = 35,
                        xOffset = p.X,
                        yOffset = p.Y,
                        zOffset = 0
                    };
                    objs.Add(t);
                }
                return new TargetObjectList(objs, ns.Mobile, true);
            }
        }
        #endregion

        #region Custom Land Commands
        [Usage("CircularIndent")]
        [Description("Makes a circular indent in the terrain.")]
        private static void circularIndent_OnCommand(CommandEventArgs e)
        {
            if (e.Length != 2)
            {
                e.Mobile.SendMessage("You must specify a radius and a depth.");
                return;
            }
            int radius = e.GetInt32(0);
            int depth = e.GetInt32(1);

            if (radius > 0)
            {
                e.Mobile.Target = new CircularIndentTarget(radius, depth);
            }
        }
        #endregion

        #region Custom Land Targets
        private class CircularIndentTarget : BaseLandRadialTarget
        {
            private int m_Radius;
            private int m_Depth;
            public CircularIndentTarget(int radius, int depth)
              : base(1, radius, 0)
            {
                m_Radius = radius;
                m_Depth = depth;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (base.SetupTarget(from, o))
                {
                    List<Point2D> circle = UltimaLiveUtility.rasterFilledCircle(new Point2D(m_Location.X, m_Location.Y), m_Radius);

                    MapOperationSeries moveSeries = new MapOperationSeries(null, from.Map.MapID);

                    bool first = true;
                    foreach (Point2D p in circle)
                    {
                        if (first)
                        {
                            moveSeries = new MapOperationSeries(new IncLandAltitude(p.X, p.Y, from.Map.MapID, m_Depth), from.Map.MapID);
                            first = false;
                        }
                        else
                        {
                            moveSeries.Add(new IncLandAltitude(p.X, p.Y, from.Map.MapID, m_Depth));
                        }
                    }

                    moveSeries.DoOperation();
                }
            }

        }
        #endregion

        #region Static Commands
        [Usage("LiveFreeze")]
        [Description("Makes a targeted area of dynamic items static.")]
        public static void LiveFreeze_OnCommand(CommandEventArgs e)
        {
            BoundingBoxPicker.Begin(e.Mobile, new BoundingBoxCallback(LiveFreezeBox_Callback), null);
        }

        private class StateInfo
        {
            public Map m_Map;
            public Point3D m_Start, m_End;

            public StateInfo(Map map, Point3D start, Point3D end)
            {
                m_Map = map;
                m_Start = start;
                m_End = end;
            }
        }

        private class DeltaState
        {
            public int m_X, m_Y;
            public List<Item> m_List;
            public DeltaState(Point2D p)
            {
                m_X = p.X;
                m_Y = p.Y;
                m_List = new List<Item>();
            }
        }

        private const string LiveFreezeWarning = "Those items will be frozen into the map. Do you wish to proceed?";
        private static void LiveFreezeBox_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            SendWarning(from, "You are about to freeze a section of items.", LiveFreezeWarning, map, start, end, new WarningGumpCallback(LiveFreezeWarning_Callback));
        }

        private static void LiveFreezeWarning_Callback(Mobile from, bool okay, object state)
        {
            if (!okay)
            {
                return;
            }

            StateInfo si = (StateInfo)state;

            LiveFreeze(from, si.m_Map, si.m_Start, si.m_End);
        }

        private static bool DoNotFreeze(Item item, bool checkvisible)
        {
            return (item.Map == null || item.Map == Map.Internal || (checkvisible && !item.Visible) || item.Name != null || XmlAttach.FindAttachment(item) != null || item.Spawner != null);
        }

        public static void LiveFreeze(Mobile from, Map targetMap, Point3D start3d, Point3D end3d)
        {
            Dictionary<Point2D, List<Item>> ItemsByBlockLocation = new Dictionary<Point2D, List<Item>>();
            if (targetMap != null && targetMap != Map.Internal && start3d.X >= 0 && end3d.X <= targetMap.Width && start3d.Y >= 0 && start3d.Y <= targetMap.Height)
            {
                Point2D start = targetMap.Bound(new Point2D(start3d));
                Point2D end = targetMap.Bound(new Point2D(end3d));
                IPooledEnumerable<Item> eable = targetMap.GetItemsInBounds(new Rectangle2D(start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1));

                Console.WriteLine(string.Format("Invoking live freeze from {0},{1} to {2},{3}", start.X, start.Y, end.X, end.Y));

                foreach (Item item in eable)
                {
                    if (item is Static || item is BaseFloor || item is BaseWall)
                    {
                        //all invisible objects or with non-null name or that contains an attachment or that are in a spawner can't be freezed
                        if (DoNotFreeze(item, true))
                        {
                            continue;
                        }

                        Point2D p = new Point2D(item.X >> 3, item.Y >> 3);
                        if (!(ItemsByBlockLocation.ContainsKey(p)))
                        {
                            ItemsByBlockLocation.Add(p, new List<Item>());
                        }
                        ItemsByBlockLocation[p].Add(item);
                    }
                }

                eable.Free();
            }
            else
            {
                from.SendMessage("That was not a proper area. Please retarget and reissue the command.");
                return;
            }

            TileMatrix matrix = targetMap.Tiles;
            foreach (KeyValuePair<Point2D, List<Item>> kvp in ItemsByBlockLocation)
            {
                StaticTile[][][] blockOfTiles = matrix.GetStaticBlock(kvp.Key.X, kvp.Key.Y);
                Dictionary<Point2D, List<StaticTile>> newBlockStatics = new Dictionary<Point2D, List<StaticTile>>();

                foreach (Item item in kvp.Value)
                {
                    int xOffset = item.X - (kvp.Key.X * 8);
                    int yOffset = item.Y - (kvp.Key.Y * 8);
                    if (xOffset < 0 || xOffset >= 8 || yOffset < 0 || yOffset >= 8)
                    {
                        continue;
                    }

                    StaticTile newTile = new StaticTile((ushort)item.ItemID, (byte)xOffset, (byte)yOffset, (sbyte)item.Z, (short)item.Hue);
                    Point2D refPoint = new Point2D(xOffset, yOffset);

                    if (!(newBlockStatics.ContainsKey(refPoint)))
                    {
                        newBlockStatics.Add(refPoint, new List<StaticTile>());
                    }

                    newBlockStatics[refPoint].Add(newTile);
                    item.Delete();
                }

                for (int i = 0; i < blockOfTiles.Length; i++)
                {
                    for (int j = 0; j < blockOfTiles[i].Length; j++)
                    {
                        for (int k = 0; k < blockOfTiles[i][j].Length; k++)
                        {
                            Point2D refPoint = new Point2D(i, j);
                            if (!(newBlockStatics.ContainsKey(refPoint)))
                            {
                                newBlockStatics.Add(refPoint, new List<StaticTile>());
                            }

                            newBlockStatics[refPoint].Add(blockOfTiles[i][j][k]);
                        }
                    }
                }

                StaticTile[][][] newblockOfTiles = new StaticTile[8][][];

                for (int i = 0; i < 8; i++)
                {
                    newblockOfTiles[i] = new StaticTile[8][];
                    for (int j = 0; j < 8; j++)
                    {
                        Point2D p = new Point2D(i, j);
                        int length = 0;
                        if (newBlockStatics.ContainsKey(p))
                        {
                            length = newBlockStatics[p].Count;
                        }
                        newblockOfTiles[i][j] = new StaticTile[length];
                        for (int k = 0; k < length; k++)
                        {
                            if (newBlockStatics.ContainsKey(p))
                            {
                                newblockOfTiles[i][j][k] = newBlockStatics[p][k];
                            }
                        }
                    }
                }

                matrix.SetStaticBlock(kvp.Key.X, kvp.Key.Y, newblockOfTiles);
                int blockNum = ((kvp.Key.X * matrix.BlockHeight) + kvp.Key.Y);

                List<Mobile> candidates = new List<Mobile>();
                int bX = kvp.Key.X * 8;
                int bY = kvp.Key.Y * 8;

                MapChangeTracker.MarkStaticsBlockForSave(targetMap.MapID, kvp.Key);
                List<int> associated;
                if (MapRegistry.MapAssociations.TryGetValue(targetMap.MapID, out associated) && associated != null)
                {
                    foreach (int mapnum in associated)
                    {
                        Map map = Map.Maps[mapnum];
                        if (map != targetMap)
                        {
                            map.ChangeMatrix(targetMap.Tiles, true);
                        }

                        IPooledEnumerable<Mobile> eable = map.GetMobilesInRange(new Point3D(bX, bY, 0));

                        foreach (Mobile m in eable)
                        {
                            if (m.Player)
                            {
                                candidates.Add(m);
                            }
                        }
                        eable.Free();
                        CRC.InvalidateBlockCRC(mapnum, blockNum);
                    }
                }
                foreach (Mobile m in candidates)
                {
                    m.Send(new UpdateStaticsPacket(new Point2D(kvp.Key.X, kvp.Key.Y), m));
                }
            }
        }

        public static void SendWarning(Mobile m, string header, string baseWarning, Map map, Point3D start, Point3D end, WarningGumpCallback callback)
        {
            m.SendGump(new WarningGump(1060635, 30720, string.Format(baseWarning, string.Format(header, map)), 0xFFC000, 420, 400, callback, new StateInfo(map, start, end)));
        }

        #endregion

        #region Land Targets

        private class BaseLandRadialTarget : RadialTarget
        {
            protected IPoint3D m_Location;
            public BaseLandRadialTarget(int TType, int Radius, int Height)
              : base(TType, Radius, Height)
            {
            }

            protected override void OnTarget(Mobile from, object o)
            {
            }

            protected bool SetupTarget(Mobile from, object o)
            {
                if (!BaseCommand.IsAccessible(from, o))
                {
                    from.SendMessage("That is not accessible.");
                    return false;
                }

                if (!(o is IPoint3D))
                {
                    return false;
                }

                m_Location = (IPoint3D)o;

                return true;
            }
        }

        private class BaseLandTarget : Target
        {
            protected IPoint3D m_Location;
            public BaseLandTarget()
              : base(-1, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object o)
            {
            }

            protected bool SetupTarget(Mobile from, object o)
            {
                if (!BaseCommand.IsAccessible(from, o))
                {
                    from.SendMessage("That is not accessible.");
                    return false;
                }

                if (!(o is IPoint3D))
                {
                    return false;
                }

                m_Location = (IPoint3D)o;

                return true;
            }
        }
        #endregion

        #region Miscellaneous Commands
        [Usage("GetBlockNumber")]
        [Description("Returns the current block number")]
        private static void getBlockNumber_OnCommand(CommandEventArgs e)
        {
            int x = e.Mobile.Location.X;
            int y = e.Mobile.Location.Y;
            Map map = e.Mobile.Map;
            TileMatrix tm = map.Tiles;

            int blocknum = (((x >> 3) * tm.BlockHeight) + (y >> 3));

            e.Mobile.SendMessage(string.Format("Your block number is {0}", blocknum));
        }

        [Usage("updateblock")]
        [Description("Sends Update statics & terrain Packet to the client.")]
        public static void updateBlock_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.Send(new UpdateTerrainPacket(new Point2D(from.X >> 3, from.Y >> 3), from));
            from.Send(new UpdateStaticsPacket(new Point2D(from.X >> 3, from.Y >> 3), from));
            Console.WriteLine("Sending update statics packet");
        }

        [Usage("queryclienthash")]
        [Description("sends the client a request to hash its surrounding blocks")]
        public static void queryClientHash_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (from.Map != null)
            {
                from.Send(new QueryClientHash(from));
            }
        }
        #endregion
    }
}
