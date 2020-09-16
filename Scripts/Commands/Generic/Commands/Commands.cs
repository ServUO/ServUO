#region References
using Server.Accounting;
using Server.Engines.Help;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Server.Commands.Generic
{
    public class TargetCommands
    {
        private static readonly List<BaseCommand> m_AllCommands = new List<BaseCommand>();

        public static List<BaseCommand> AllCommands => m_AllCommands;

        public static void Initialize()
        {
            Register(new KillCommand(true));
            Register(new KillCommand(false));
            Register(new HideCommand(true));
            Register(new HideCommand(false));
            Register(new KickCommand(true));
            Register(new KickCommand(false));
            Register(new FirewallCommand());
            Register(new TeleCommand());
            Register(new SetCommand());
            Register(new AliasedSetCommand(AccessLevel.GameMaster, "Immortal", "blessed", "true", ObjectTypes.Mobiles));
            Register(new AliasedSetCommand(AccessLevel.GameMaster, "Invul", "blessed", "true", ObjectTypes.Mobiles));
            Register(new AliasedSetCommand(AccessLevel.GameMaster, "Mortal", "blessed", "false", ObjectTypes.Mobiles));
            Register(new AliasedSetCommand(AccessLevel.GameMaster, "NoInvul", "blessed", "false", ObjectTypes.Mobiles));
            Register(new AliasedSetCommand(AccessLevel.GameMaster, "Squelch", "squelched", "true", ObjectTypes.Mobiles));
            Register(new AliasedSetCommand(AccessLevel.GameMaster, "Unsquelch", "squelched", "false", ObjectTypes.Mobiles));

            Register(new AliasedSetCommand(AccessLevel.GameMaster, "ShaveHair", "HairItemID", "0", ObjectTypes.Mobiles));
            Register(new AliasedSetCommand(AccessLevel.GameMaster, "ShaveBeard", "FacialHairItemID", "0", ObjectTypes.Mobiles));

            Register(new GetCommand());
            Register(new GetTypeCommand());
            Register(new DeleteCommand());
            Register(new RestockCommand());
            Register(new DismountCommand());
            Register(new AddCommand());
            Register(new AddToPackCommand());
            Register(new TellCommand(true));
            Register(new TellCommand(false));
            Register(new PrivSoundCommand());
            Register(new IncreaseCommand());
            Register(new OpenBrowserCommand());
            Register(new CountCommand());
            Register(new InterfaceCommand());
            Register(new RefreshHouseCommand());
            Register(new ConditionCommand());
            Register(new BringToPackCommand());
            Register(new TraceLockdownCommand());
        }

        public static void Register(BaseCommand command)
        {
            m_AllCommands.Add(command);

            List<BaseCommandImplementor> impls = BaseCommandImplementor.Implementors;

            for (int i = 0; i < impls.Count; ++i)
            {
                BaseCommandImplementor impl = impls[i];

                if ((command.Supports & impl.SupportRequirement) != 0)
                    impl.Register(command);
            }
        }
    }

    public class ConditionCommand : BaseCommand
    {
        public ConditionCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Simple | CommandSupport.Complex | CommandSupport.Self;
            Commands = new[] { "Condition" };
            ObjectTypes = ObjectTypes.All;
            Usage = "Condition <condition>";
            Description = "Checks that the given condition matches a targeted object.";
            ListOptimized = true;
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            try
            {
                string[] args = e.Arguments;
                ObjectConditional condition = ObjectConditional.Parse(e.Mobile, ref args);

                for (int i = 0; i < list.Count; ++i)
                {
                    if (condition.CheckCondition(list[i]))
                        AddResponse("True - that object matches the condition.");
                    else
                        AddResponse("False - that object does not match the condition.");
                }
            }
            catch (Exception ex)
            {
                e.Mobile.SendMessage(ex.Message);
                Diagnostics.ExceptionLogging.LogException(ex);
            }
        }
    }

    public class BringToPackCommand : BaseCommand
    {
        public BringToPackCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllItems;
            Commands = new[] { "BringToPack" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "BringToPack";
            Description = "Brings a targeted item to your backpack.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Item item = obj as Item;

            if (item != null)
            {
                if (e.Mobile.PlaceInBackpack(item))
                    AddResponse("The item has been placed in your backpack.");
                else
                    AddResponse("Your backpack could not hold the item.");
            }
        }
    }

    public class RefreshHouseCommand : BaseCommand
    {
        public RefreshHouseCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Simple;
            Commands = new[] { "RefreshHouse" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "RefreshHouse";
            Description = "Refreshes a targeted house sign.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj is HouseSign)
            {
                BaseHouse house = ((HouseSign)obj).Owner;

                if (house == null)
                {
                    LogFailure("That sign has no house attached.");
                }
                else
                {
                    house.RefreshDecay();
                    AddResponse("The house has been refreshed.");
                }
            }
            else
            {
                LogFailure("That is not a house sign.");
            }
        }
    }

    public class CountCommand : BaseCommand
    {
        public CountCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Complex;
            Commands = new[] { "Count" };
            ObjectTypes = ObjectTypes.All;
            Usage = "Count";
            Description =
                "Counts the number of objects that a command modifier would use. Generally used with condition arguments.";
            ListOptimized = true;
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count == 1)
                AddResponse("There is one matching object.");
            else
                AddResponse(string.Format("There are {0} matching objects.", list.Count));
        }
    }

    public class OpenBrowserCommand : BaseCommand
    {
        public OpenBrowserCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllMobiles;
            Commands = new[] { "OpenBrowser", "OB" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "OpenBrowser <url>";
            Description = "Opens the web browser of a targeted player to a specified url.";
        }

        public static void OpenBrowser_Callback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            Mobile gm = (Mobile)states[0];
            string url = (string)states[1];
            bool echo = (bool)states[2];

            if (okay)
            {
                if (echo)
                    gm.SendMessage("{0} : has opened their web browser to : {1}", from.Name, url);

                from.LaunchBrowser(url);
            }
            else
            {
                if (echo)
                    gm.SendMessage("{0} : has chosen not to open their web browser to : {1}", from.Name, url);

                from.SendMessage("You have chosen not to open your web browser.");
            }
        }

        public void Execute(CommandEventArgs e, object obj, bool echo)
        {
            if (e.Length == 1)
            {
                Mobile mob = (Mobile)obj;
                Mobile from = e.Mobile;

                if (mob.Player)
                {
                    NetState ns = mob.NetState;

                    if (ns == null)
                    {
                        LogFailure("That player is not online.");
                    }
                    else
                    {
                        string url = e.GetString(0);

                        CommandLogging.WriteLine(
                            from,
                            "{0} {1} requesting to open web browser of {2} to {3}",
                            from.AccessLevel,
                            CommandLogging.Format(from),
                            CommandLogging.Format(mob),
                            url);

                        if (echo)
                            AddResponse("Awaiting user confirmation...");
                        else
                            AddResponse("Open web browser request sent.");

                        mob.SendGump(
                            new WarningGump(
                                1060637,
                                30720,
                                string.Format("A game master is requesting to open your web browser to the following URL:<br>{0}", url),
                                0xFFC000,
                                320,
                                240,
                                OpenBrowser_Callback,
                                new object[] { from, url, echo }));
                    }
                }
                else
                {
                    LogFailure("That is not a player.");
                }
            }
            else
            {
                LogFailure("Format: OpenBrowser <url>");
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Execute(e, obj, true);
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            for (int i = 0; i < list.Count; ++i)
                Execute(e, list[i], false);
        }
    }

    public class IncreaseCommand : BaseCommand
    {
        public IncreaseCommand()
        {
            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.All;
            Commands = new[] { "Increase", "Inc" };
            ObjectTypes = ObjectTypes.Both;
            Usage = "Increase {<propertyName> <offset> ...}";
            Description = "Increases the value of a specified property by the specified offset.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj is BaseMulti)
            {
                LogFailure("This command does not work on multis.");
            }
            else if (e.Length >= 2)
            {
                string result = Properties.IncreaseValue(e.Mobile, obj, e.Arguments);

                if (result == "The property has been increased." || result == "The properties have been increased." ||
                    result == "The property has been decreased." || result == "The properties have been decreased." ||
                    result == "The properties have been changed.")
                    AddResponse(result);
                else
                    LogFailure(result);
            }
            else
            {
                LogFailure("Format: Increase {<propertyName> <offset> ...}");
            }
        }
    }

    public class PrivSoundCommand : BaseCommand
    {
        public PrivSoundCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllMobiles;
            Commands = new[] { "PrivSound" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "PrivSound <index>";
            Description = "Plays a sound to a given target.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile from = e.Mobile;

            if (e.Length == 1)
            {
                int index = e.GetInt32(0);
                Mobile mob = (Mobile)obj;

                CommandLogging.WriteLine(
                    from,
                    "{0} {1} playing sound {2} for {3}",
                    from.AccessLevel,
                    CommandLogging.Format(from),
                    index,
                    CommandLogging.Format(mob));
                mob.Send(new PlaySound(index, mob.Location));
            }
            else
            {
                from.SendMessage("Format: PrivSound <index>");
            }
        }
    }

    public class TellCommand : BaseCommand
    {
        private readonly bool m_InGump;

        public TellCommand(bool inGump)
        {
            m_InGump = inGump;

            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.AllMobiles;
            ObjectTypes = ObjectTypes.Mobiles;

            if (inGump)
            {
                Commands = new[] { "Message", "Msg" };
                Usage = "Message \"text\"";
                Description = "Sends a message to a targeted player.";
            }
            else
            {
                Commands = new[] { "Tell" };
                Usage = "Tell \"text\"";
                Description = "Sends a system message to a targeted player.";
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile mob = (Mobile)obj;
            Mobile from = e.Mobile;

            CommandLogging.WriteLine(
                from,
                "{0} {1} {2} {3} \"{4}\"",
                from.AccessLevel,
                CommandLogging.Format(from),
                m_InGump ? "messaging" : "telling",
                CommandLogging.Format(mob),
                e.ArgString);

            if (m_InGump)
                mob.SendGump(new MessageSentGump(mob, from.Name, e.ArgString));
            else
                mob.SendMessage(e.ArgString);
        }
    }

    public class AddToPackCommand : BaseCommand
    {
        public AddToPackCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.All;
            Commands = new[] { "AddToPack", "AddToCont" };
            ObjectTypes = ObjectTypes.Both;
            ListOptimized = true;
            Usage = "AddToPack <name> [params] [set {<propertyName> <value> ...}]";
            Description =
                "Adds an item by name to the backpack of a targeted player or npc, or a targeted container. Optional constructor parameters. Optional set property list.";
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (e.Arguments.Length == 0)
                return;

            List<Container> packs = new List<Container>(list.Count);

            for (int i = 0; i < list.Count; ++i)
            {
                object obj = list[i];
                Container cont = null;

                if (obj is Mobile)
                    cont = ((Mobile)obj).Backpack;
                else if (obj is Container)
                    cont = (Container)obj;

                if (cont != null)
                    packs.Add(cont);
                else
                    LogFailure("That is not a container.");
            }

            Add.Invoke(e.Mobile, e.Mobile.Location, e.Mobile.Location, e.Arguments, packs);
        }
    }

    public class AddCommand : BaseCommand
    {
        public AddCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Simple | CommandSupport.Self;
            Commands = new[] { "Add" };
            ObjectTypes = ObjectTypes.All;
            Usage = "Add [<name> [params] [set {<propertyName> <value> ...}]]";
            Description =
                "Adds an item or npc by name to a targeted location. Optional constructor parameters. Optional set property list. If no arguments are specified, this brings up a categorized add menu.";
        }

        public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
        {
            if (e.Length >= 1)
            {
                Type t = ScriptCompiler.FindTypeByName(e.GetString(0));

                if (t == null)
                {
                    e.Mobile.SendMessage("No type with that name was found.");

                    string match = e.GetString(0).Trim();

                    if (match.Length < 3)
                    {
                        e.Mobile.SendMessage("Invalid search string.");
                        e.Mobile.SendGump(new AddGump(e.Mobile, match, 0, Type.EmptyTypes, false));
                    }
                    else
                    {
                        e.Mobile.SendGump(new AddGump(e.Mobile, match, 0, AddGump.Match(match).ToArray(), true));
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                e.Mobile.SendGump(new CategorizedAddGump(e.Mobile));
            }

            return false;
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            IPoint3D p = obj as IPoint3D;

            if (p == null)
                return;

            if (p is Item)
                p = ((Item)p).GetWorldTop();
            else if (p is Mobile)
                p = ((Mobile)p).Location;

            Add.Invoke(e.Mobile, new Point3D(p), new Point3D(p), e.Arguments);
        }
    }

    public class TeleCommand : BaseCommand
    {
        public TeleCommand()
        {
            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.Simple;
            Commands = new[] { "Teleport", "Tele" };
            ObjectTypes = ObjectTypes.All;
            Usage = "Teleport";
            Description = "Teleports your character to a targeted location.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            IPoint3D p = obj as IPoint3D;

            if (p == null)
                return;

            Mobile from = e.Mobile;

            SpellHelper.GetSurfaceTop(ref p);

            //CommandLogging.WriteLine( from, "{0} {1} teleporting to {2}", from.AccessLevel, CommandLogging.Format( from ), new Point3D( p ) );

            Point3D fromLoc = from.Location;
            Point3D toLoc = new Point3D(p);

            from.Location = toLoc;
            from.ProcessDelta();

            if (!from.Hidden)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(fromLoc, from.Map, EffectItem.DefaultDuration),
                    0x3728,
                    10,
                    10,
                    2023);
                Effects.SendLocationParticles(EffectItem.Create(toLoc, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                from.PlaySound(0x1FE);
            }
        }
    }

    public class DismountCommand : BaseCommand
    {
        public DismountCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllMobiles;
            Commands = new[] { "Dismount" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "Dismount";
            Description = "Forcefully dismounts a given target.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile from = e.Mobile;
            Mobile mob = (Mobile)obj;

            CommandLogging.WriteLine(
                from,
                "{0} {1} dismounting {2}",
                from.AccessLevel,
                CommandLogging.Format(from),
                CommandLogging.Format(mob));

            bool takenAction = false;

            for (int i = 0; i < mob.Items.Count; ++i)
            {
                Item item = mob.Items[i];

                if (item is IMountItem)
                {
                    IMount mount = ((IMountItem)item).Mount;

                    if (mount != null)
                    {
                        mount.Rider = null;
                        takenAction = true;
                    }

                    if (mob.Items.IndexOf(item) == -1)
                        --i;
                }
            }

            for (int i = 0; i < mob.Items.Count; ++i)
            {
                Item item = mob.Items[i];

                if (item.Layer == Layer.Mount)
                {
                    takenAction = true;
                    item.Delete();
                    --i;
                }
            }

            if (takenAction)
                AddResponse("They have been dismounted.");
            else
                LogFailure("They were not mounted.");
        }
    }

    public class RestockCommand : BaseCommand
    {
        public RestockCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllNPCs;
            Commands = new[] { "Restock" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "Restock";
            Description =
                "Manually restocks a targeted vendor, refreshing the quantity of every item the vendor sells to the maximum. This also invokes the maximum quantity adjustment algorithms.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj is BaseVendor)
            {
                CommandLogging.WriteLine(
                    e.Mobile,
                    "{0} {1} restocking {2}",
                    e.Mobile.AccessLevel,
                    CommandLogging.Format(e.Mobile),
                    CommandLogging.Format(obj));

                ((BaseVendor)obj).Restock();
                AddResponse("The vendor has been restocked.");
            }
            else
            {
                AddResponse("That is not a vendor.");
            }
        }
    }

    public class GetTypeCommand : BaseCommand
    {
        public GetTypeCommand()
        {
            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.All;
            Commands = new[] { "GetType" };
            ObjectTypes = ObjectTypes.All;
            Usage = "GetType";
            Description = "Gets the type name of a targeted object.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj == null)
            {
                AddResponse("The object is null.");
            }
            else
            {
                Type type = obj.GetType();

                if (type.DeclaringType == null)
                    AddResponse(string.Format("The type of that object is {0}.", type.Name));
                else
                    AddResponse(string.Format("The type of that object is {0}.", type.FullName));
            }
        }
    }

    public class GetCommand : BaseCommand
    {
        public GetCommand()
        {
            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.All;
            Commands = new[] { "Get" };
            ObjectTypes = ObjectTypes.All;
            Usage = "Get <propertyName>";
            Description = "Gets one or more property values by name of a targeted object.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Length >= 1)
            {
                for (int i = 0; i < e.Length; ++i)
                {
                    string result = Properties.GetValue(e.Mobile, obj, e.GetString(i));

                    if (result == "Property not found." || result == "Property is write only." ||
                        result.StartsWith("Getting this property"))
                        LogFailure(result);
                    else
                        AddResponse(result);
                }
            }
            else
            {
                LogFailure("Format: Get <propertyName>");
            }
        }
    }

    public class AliasedSetCommand : BaseCommand
    {
        private readonly string m_Name;
        private readonly string m_Value;

        public AliasedSetCommand(AccessLevel level, string command, string name, string value, ObjectTypes objects)
        {
            m_Name = name;
            m_Value = value;

            AccessLevel = level;

            if (objects == ObjectTypes.Items)
                Supports = CommandSupport.AllItems;
            else if (objects == ObjectTypes.Mobiles)
                Supports = CommandSupport.AllMobiles;
            else
                Supports = CommandSupport.All;

            Commands = new[] { command };
            ObjectTypes = objects;
            Usage = command;
            Description = string.Format("Sets the {0} property to {1}.", name, value);
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            string result = Properties.SetValue(e.Mobile, obj, m_Name, m_Value);

            if (result == "Property has been set.")
                AddResponse(result);
            else
                LogFailure(result);
        }
    }

    public class SetCommand : BaseCommand
    {
        public SetCommand()
        {
            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.All;
            Commands = new[] { "Set" };
            ObjectTypes = ObjectTypes.Both;
            Usage = "Set <propertyName> <value> [...]";
            Description = "Sets one or more property values by name of a targeted object.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Length >= 2)
            {
                for (int i = 0; (i + 1) < e.Length; i += 2)
                {
                    string result = Properties.SetValue(e.Mobile, obj, e.GetString(i), e.GetString(i + 1));

                    if (result == "Property has been set.")
                        AddResponse(result);
                    else
                        LogFailure(result);
                }
            }
            else
            {
                LogFailure("Format: Set <propertyName> <value>");
            }
        }
    }

    public class DeleteCommand : BaseCommand
    {
        private readonly List<object> _DeleteConfirm = new List<object>();

        public DeleteCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
            Commands = new[] { "Delete", "Remove" };
            ObjectTypes = ObjectTypes.Both;
            Usage = "Delete";
            Description = "Deletes a targeted item or mobile. Does not delete players.";
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            bool flushToLog = false;

            if (okay)
            {
                AddResponse("Delete command confirmed.");

                if (list.Count > 20)
                {
                    CommandLogging.Enabled = false;
                    NetState.Pause();
                }

                foreach (object o in list)
                {
                    _DeleteConfirm.Add(o);
                }

                base.ExecuteList(e, list);

                foreach (object o in list)
                {
                    _DeleteConfirm.Remove(o);
                }

                if (list.Count > 20)
                {
                    NetState.Resume();
                    flushToLog = true;
                    CommandLogging.Enabled = true;
                }
            }
            else
            {
                AddResponse("Delete command aborted.");
            }

            Flush(from, flushToLog);
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                string message = string.Format(
                    "You are about to delete {0} objects. " + "This cannot be undone without a full server revert.<br><br>Continue?",
                    list.Count);

                e.Mobile.SendGump(
                    new WarningGump(1060637, 30720, message, 0xFFC000, 420, 280, OnConfirmCallback, new object[] { e, list }));

                AddResponse("Awaiting confirmation...");
                return;
            }

            if (list.Count > 0)
            {
                object obj = list[0];

                if (obj != null && !_DeleteConfirm.Contains(obj))
                {
                    string message;

                    if (DeleteConfirmAttribute.Find(obj, out message))
                    {
                        if (string.IsNullOrWhiteSpace(message))
                        {
                            message = string.Format("Confirm deletion of {0}", obj);
                        }

                        e.Mobile.SendGump(
                            new WarningGump(1060637, 30720, message, 0xFFC000, 420, 280, OnConfirmCallback, new object[] { e, list }));

                        AddResponse("Awaiting confirmation...");
                        return;
                    }
                }
            }

            base.ExecuteList(e, list);
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (!ListOptimized && !_DeleteConfirm.Contains(obj))
            {
                string message;

                if (DeleteConfirmAttribute.Find(obj, out message))
                {
                    if (string.IsNullOrWhiteSpace(message))
                    {
                        message = string.Format("Confirm deletion of {0}", obj);
                    }

                    ArrayList list = new ArrayList
                    {
                        obj
                    };

                    e.Mobile.SendGump(
                        new WarningGump(1060637, 30720, message, 0xFFC000, 420, 280, OnConfirmCallback, new object[] { e, list }));

                    AddResponse("Awaiting confirmation...");
                    return;
                }
            }

            if (obj is Item)
            {
                CommandLogging.WriteLine(
                    e.Mobile,
                    "{0} {1} deleting {2}",
                    e.Mobile.AccessLevel,
                    CommandLogging.Format(e.Mobile),
                    CommandLogging.Format(obj));
                ((Item)obj).Delete();
                AddResponse("The item has been deleted.");
            }
            else if (obj is Mobile && !((Mobile)obj).Player)
            {
                CommandLogging.WriteLine(
                    e.Mobile,
                    "{0} {1} deleting {2}",
                    e.Mobile.AccessLevel,
                    CommandLogging.Format(e.Mobile),
                    CommandLogging.Format(obj));
                ((Mobile)obj).Delete();
                AddResponse("The mobile has been deleted.");
            }
            else
            {
                LogFailure("That cannot be deleted.");
            }

            _DeleteConfirm.Remove(obj);
        }
    }

    public class KillCommand : BaseCommand
    {
        private readonly bool m_Value;

        public KillCommand(bool value)
        {
            m_Value = value;

            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.AllMobiles;
            Commands = value ? new[] { "Kill" } : new[] { "Resurrect", "Res" };
            ObjectTypes = ObjectTypes.Mobiles;

            if (value)
            {
                Usage = "Kill";
                Description = "Kills a targeted player or npc.";
            }
            else
            {
                Usage = "Resurrect";
                Description = "Resurrects a targeted ghost.";
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile mob = (Mobile)obj;
            Mobile from = e.Mobile;

            if (m_Value)
            {
                if (!mob.Alive)
                {
                    LogFailure("They are already dead.");
                }
                else if (!mob.CanBeDamaged())
                {
                    LogFailure("They cannot be harmed.");
                }
                else
                {
                    CommandLogging.WriteLine(
                        from,
                        "{0} {1} killing {2}",
                        from.AccessLevel,
                        CommandLogging.Format(from),
                        CommandLogging.Format(mob));
                    mob.Kill();

                    AddResponse("They have been killed.");
                }
            }
            else
            {
                if (mob.IsDeadBondedPet)
                {
                    BaseCreature bc = mob as BaseCreature;

                    if (bc != null)
                    {
                        CommandLogging.WriteLine(
                            from,
                            "{0} {1} resurrecting {2}",
                            from.AccessLevel,
                            CommandLogging.Format(from),
                            CommandLogging.Format(mob));

                        bc.PlaySound(0x214);
                        bc.FixedEffect(0x376A, 10, 16);

                        bc.ResurrectPet();

                        AddResponse("It has been resurrected.");
                    }
                }
                else if (!mob.Alive)
                {
                    CommandLogging.WriteLine(
                        from,
                        "{0} {1} resurrecting {2}",
                        from.AccessLevel,
                        CommandLogging.Format(from),
                        CommandLogging.Format(mob));

                    mob.PlaySound(0x214);
                    mob.FixedEffect(0x376A, 10, 16);

                    mob.Resurrect();

                    AddResponse("They have been resurrected.");
                }
                else
                {
                    LogFailure("They are not dead.");
                }
            }
        }
    }

    public class HideCommand : BaseCommand
    {
        private readonly bool m_Value;

        public HideCommand(bool value)
        {
            m_Value = value;

            AccessLevel = AccessLevel.Counselor;
            Supports = CommandSupport.AllMobiles;
            Commands = new[] { value ? "Hide" : "Unhide" };
            ObjectTypes = ObjectTypes.Mobiles;

            if (value)
            {
                Usage = "Hide";
                Description = "Makes a targeted mobile disappear in a puff of smoke.";
            }
            else
            {
                Usage = "Unhide";
                Description = "Makes a targeted mobile appear in a puff of smoke.";
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile m = (Mobile)obj;

            CommandLogging.WriteLine(
                e.Mobile,
                "{0} {1} {2} {3}",
                e.Mobile.AccessLevel,
                CommandLogging.Format(e.Mobile),
                m_Value ? "hiding" : "unhiding",
                CommandLogging.Format(m));

            Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z + 4), m.Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z), m.Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y, m.Z - 4), m.Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z + 4), m.Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z), m.Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(m.X, m.Y + 1, m.Z - 4), m.Map, 0x3728, 13);

            Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 11), m.Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 7), m.Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z + 3), m.Map, 0x3728, 13);
            Effects.SendLocationEffect(new Point3D(m.X + 1, m.Y + 1, m.Z - 1), m.Map, 0x3728, 13);

            m.PlaySound(0x228);
            m.Hidden = m_Value;

            if (m_Value)
                AddResponse("They have been hidden.");
            else
                AddResponse("They have been revealed.");
        }
    }

    public class FirewallCommand : BaseCommand
    {
        public FirewallCommand()
        {
            AccessLevel = AccessLevel.Administrator;
            Supports = CommandSupport.AllMobiles;
            Commands = new[] { "Firewall" };
            ObjectTypes = ObjectTypes.Mobiles;
            Usage = "Firewall";
            Description =
                "Adds a targeted player to the firewall (list of blocked IP addresses). This command does not ban or kick.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile from = e.Mobile;
            Mobile targ = (Mobile)obj;
            NetState state = targ.NetState;

            if (state != null)
            {
                CommandLogging.WriteLine(
                    from,
                    "{0} {1} firewalling {2}",
                    from.AccessLevel,
                    CommandLogging.Format(from),
                    CommandLogging.Format(targ));

                try
                {
                    Firewall.Add(state.Address);
                    AddResponse("They have been firewalled.");
                }
                catch (Exception ex)
                {
                    LogFailure(ex.Message);
                }
            }
            else
            {
                LogFailure("They are not online.");
            }
        }
    }

    public class KickCommand : BaseCommand
    {
        private readonly bool m_Ban;

        public KickCommand(bool ban)
        {
            m_Ban = ban;

            AccessLevel = (ban ? AccessLevel.Administrator : AccessLevel.GameMaster);
            Supports = CommandSupport.AllMobiles;
            Commands = new[] { ban ? "Ban" : "Kick" };
            ObjectTypes = ObjectTypes.Mobiles;

            if (ban)
            {
                Usage = "Ban";
                Description = "Bans the account of a targeted player.";
            }
            else
            {
                Usage = "Kick";
                Description = "Disconnects a targeted player.";
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile from = e.Mobile;
            Mobile targ = (Mobile)obj;

            if (from.AccessLevel > targ.AccessLevel)
            {
                NetState fromState = from.NetState, targState = targ.NetState;

                if (fromState != null && targState != null)
                {
                    Account fromAccount = fromState.Account as Account;
                    Account targAccount = targState.Account as Account;

                    if (fromAccount != null && targAccount != null)
                    {
                        CommandLogging.WriteLine(
                            from,
                            "{0} {1} {2} {3}",
                            from.AccessLevel,
                            CommandLogging.Format(from),
                            m_Ban ? "banning" : "kicking",
                            CommandLogging.Format(targ));

                        targ.Say("I've been {0}!", m_Ban ? "banned" : "kicked");

                        AddResponse(string.Format("They have been {0}.", m_Ban ? "banned" : "kicked"));

                        targState.Dispose();

                        if (m_Ban)
                        {
                            targAccount.Banned = true;
                            targAccount.SetUnspecifiedBan(from);
                            from.SendGump(new BanDurationGump(targAccount));
                        }
                    }
                }
                else if (targState == null)
                {
                    LogFailure("They are not online.");
                }
            }
            else
            {
                LogFailure("You do not have the required access level to do this.");
            }
        }
    }

    public class TraceLockdownCommand : BaseCommand
    {
        public TraceLockdownCommand()
        {
            AccessLevel = AccessLevel.Administrator;
            Supports = CommandSupport.Simple;
            Commands = new[] { "TraceLockdown" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "TraceLockdown";
            Description = "Finds the BaseHouse for which a targeted item is locked down or secured.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Item item = obj as Item;

            if (item == null)
                return;

            if (!item.IsLockedDown && !item.IsSecure)
            {
                LogFailure("That is not locked down.");
                return;
            }

            foreach (BaseHouse house in BaseHouse.AllHouses)
            {
                if (house.IsSecure(item) || house.IsLockedDown(item))
                {
                    e.Mobile.SendGump(new PropertiesGump(e.Mobile, house));
                    return;
                }
            }

            LogFailure("No house was found.");
        }
    }
}
