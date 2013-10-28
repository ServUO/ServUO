using System;
using System.Collections;
using System.Collections.Generic;
using Server.Accounting;
using Server.Engines.Help;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
using Server.Spells;

namespace Server.Commands.Generic
{
    public class TargetCommands
    {
        private static readonly List<BaseCommand> m_AllCommands = new List<BaseCommand>();
        public static List<BaseCommand> AllCommands
        {
            get
            {
                return m_AllCommands;
            }
        }
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
            Register(new Factions.FactionKickCommand(Factions.FactionKickType.Kick));
            Register(new Factions.FactionKickCommand(Factions.FactionKickType.Ban));
            Register(new Factions.FactionKickCommand(Factions.FactionKickType.Unban));
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
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.Simple | CommandSupport.Complex | CommandSupport.Self;
            this.Commands = new string[] { "Condition" };
            this.ObjectTypes = ObjectTypes.All;
            this.Usage = "Condition <condition>";
            this.Description = "Checks that the given condition matches a targeted object.";
            this.ListOptimized = true;
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
                        this.AddResponse("True - that object matches the condition.");
                    else
                        this.AddResponse("False - that object does not match the condition.");
                }
            }
            catch (Exception ex)
            {
                e.Mobile.SendMessage(ex.Message);
            }
        }
    }

    public class BringToPackCommand : BaseCommand
    {
        public BringToPackCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.AllItems;
            this.Commands = new string[] { "BringToPack" };
            this.ObjectTypes = ObjectTypes.Items;
            this.Usage = "BringToPack";
            this.Description = "Brings a targeted item to your backpack.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Item item = obj as Item;

            if (item != null)
            {
                if (e.Mobile.PlaceInBackpack(item))
                    this.AddResponse("The item has been placed in your backpack.");
                else
                    this.AddResponse("Your backpack could not hold the item.");
            }
        }
    }

    public class RefreshHouseCommand : BaseCommand
    {
        public RefreshHouseCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.Simple;
            this.Commands = new string[] { "RefreshHouse" };
            this.ObjectTypes = ObjectTypes.Items;
            this.Usage = "RefreshHouse";
            this.Description = "Refreshes a targeted house sign.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj is HouseSign)
            {
                BaseHouse house = ((HouseSign)obj).Owner;

                if (house == null)
                {
                    this.LogFailure("That sign has no house attached.");
                }
                else
                {
                    house.RefreshDecay();
                    this.AddResponse("The house has been refreshed.");
                }
            }
            else
            {
                this.LogFailure("That is not a house sign.");
            }
        }
    }

    public class CountCommand : BaseCommand
    {
        public CountCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.Complex;
            this.Commands = new string[] { "Count" };
            this.ObjectTypes = ObjectTypes.All;
            this.Usage = "Count";
            this.Description = "Counts the number of objects that a command modifier would use. Generally used with condition arguments.";
            this.ListOptimized = true;
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count == 1)
                this.AddResponse("There is one matching object.");
            else
                this.AddResponse(String.Format("There are {0} matching objects.", list.Count));
        }
    }

    public class OpenBrowserCommand : BaseCommand
    {
        public OpenBrowserCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.AllMobiles;
            this.Commands = new string[] { "OpenBrowser", "OB" };
            this.ObjectTypes = ObjectTypes.Mobiles;
            this.Usage = "OpenBrowser <url>";
            this.Description = "Opens the web browser of a targeted player to a specified url.";
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
                        this.LogFailure("That player is not online.");
                    }
                    else
                    {
                        string url = e.GetString(0);

                        CommandLogging.WriteLine(from, "{0} {1} requesting to open web browser of {2} to {3}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(mob), url);

                        if (echo)
                            this.AddResponse("Awaiting user confirmation...");
                        else
                            this.AddResponse("Open web browser request sent.");

                        mob.SendGump(new WarningGump(1060637, 30720, String.Format("A game master is requesting to open your web browser to the following URL:<br>{0}", url), 0xFFC000, 320, 240, new WarningGumpCallback(OpenBrowser_Callback), new object[] { from, url, echo }));
                    }
                }
                else
                {
                    this.LogFailure("That is not a player.");
                }
            }
            else
            {
                this.LogFailure("Format: OpenBrowser <url>");
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            this.Execute(e, obj, true);
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            for (int i = 0; i < list.Count; ++i)
                this.Execute(e, list[i], false);
        }
    }

    public class IncreaseCommand : BaseCommand
    {
        public IncreaseCommand()
        {
            this.AccessLevel = AccessLevel.Counselor;
            this.Supports = CommandSupport.All;
            this.Commands = new string[] { "Increase", "Inc" };
            this.ObjectTypes = ObjectTypes.Both;
            this.Usage = "Increase {<propertyName> <offset> ...}";
            this.Description = "Increases the value of a specified property by the specified offset.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj is BaseMulti)
            {
                this.LogFailure("This command does not work on multis.");
            }
            else if (e.Length >= 2)
            {
                string result = Properties.IncreaseValue(e.Mobile, obj, e.Arguments);

                if (result == "The property has been increased." || result == "The properties have been increased." || result == "The property has been decreased." || result == "The properties have been decreased." || result == "The properties have been changed.")
                    this.AddResponse(result);
                else
                    this.LogFailure(result);
            }
            else
            {
                this.LogFailure("Format: Increase {<propertyName> <offset> ...}");
            }
        }
    }

    public class PrivSoundCommand : BaseCommand
    {
        public PrivSoundCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.AllMobiles;
            this.Commands = new string[] { "PrivSound" };
            this.ObjectTypes = ObjectTypes.Mobiles;
            this.Usage = "PrivSound <index>";
            this.Description = "Plays a sound to a given target.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile from = e.Mobile;

            if (e.Length == 1)
            {
                int index = e.GetInt32(0);
                Mobile mob = (Mobile)obj;

                CommandLogging.WriteLine(from, "{0} {1} playing sound {2} for {3}", from.AccessLevel, CommandLogging.Format(from), index, CommandLogging.Format(mob));
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
            this.m_InGump = inGump;

            this.AccessLevel = AccessLevel.Counselor;
            this.Supports = CommandSupport.AllMobiles;
            this.ObjectTypes = ObjectTypes.Mobiles;

            if (inGump)
            {
                this.Commands = new string[] { "Message", "Msg" };
                this.Usage = "Message \"text\"";
                this.Description = "Sends a message to a targeted player.";
            }
            else
            {
                this.Commands = new string[] { "Tell" };
                this.Usage = "Tell \"text\"";
                this.Description = "Sends a system message to a targeted player.";
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile mob = (Mobile)obj;
            Mobile from = e.Mobile;

            CommandLogging.WriteLine(from, "{0} {1} {2} {3} \"{4}\"", from.AccessLevel, CommandLogging.Format(from), this.m_InGump ? "messaging" : "telling", CommandLogging.Format(mob), e.ArgString);

            if (this.m_InGump)
                mob.SendGump(new MessageSentGump(mob, from.Name, e.ArgString));
            else
                mob.SendMessage(e.ArgString);
        }
    }

    public class AddToPackCommand : BaseCommand
    {
        public AddToPackCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.All;
            this.Commands = new string[] { "AddToPack", "AddToCont" };
            this.ObjectTypes = ObjectTypes.Both;
            this.ListOptimized = true;
            this.Usage = "AddToPack <name> [params] [set {<propertyName> <value> ...}]";
            this.Description = "Adds an item by name to the backpack of a targeted player or npc, or a targeted container. Optional constructor parameters. Optional set property list.";
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
                    this.LogFailure("That is not a container.");
            }

            Add.Invoke(e.Mobile, e.Mobile.Location, e.Mobile.Location, e.Arguments, packs);
        }
    }

    public class AddCommand : BaseCommand
    {
        public AddCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.Simple | CommandSupport.Self;
            this.Commands = new string[] { "Add" };
            this.ObjectTypes = ObjectTypes.All;
            this.Usage = "Add [<name> [params] [set {<propertyName> <value> ...}]]";
            this.Description = "Adds an item or npc by name to a targeted location. Optional constructor parameters. Optional set property list. If no arguments are specified, this brings up a categorized add menu.";
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
            this.AccessLevel = AccessLevel.Counselor;
            this.Supports = CommandSupport.Simple;
            this.Commands = new string[] { "Teleport", "Tele" };
            this.ObjectTypes = ObjectTypes.All;
            this.Usage = "Teleport";
            this.Description = "Teleports your character to a targeted location.";
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
                Effects.SendLocationParticles(EffectItem.Create(fromLoc, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                Effects.SendLocationParticles(EffectItem.Create(toLoc, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                from.PlaySound(0x1FE);
            }
        }
    }

    public class DismountCommand : BaseCommand
    {
        public DismountCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.AllMobiles;
            this.Commands = new string[] { "Dismount" };
            this.ObjectTypes = ObjectTypes.Mobiles;
            this.Usage = "Dismount";
            this.Description = "Forcefully dismounts a given target.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile from = e.Mobile;
            Mobile mob = (Mobile)obj;

            CommandLogging.WriteLine(from, "{0} {1} dismounting {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(mob));

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
                this.AddResponse("They have been dismounted.");
            else
                this.LogFailure("They were not mounted.");
        }
    }

    public class RestockCommand : BaseCommand
    {
        public RestockCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.AllNPCs;
            this.Commands = new string[] { "Restock" };
            this.ObjectTypes = ObjectTypes.Mobiles;
            this.Usage = "Restock";
            this.Description = "Manually restocks a targeted vendor, refreshing the quantity of every item the vendor sells to the maximum. This also invokes the maximum quantity adjustment algorithms.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj is BaseVendor)
            {
                CommandLogging.WriteLine(e.Mobile, "{0} {1} restocking {2}", e.Mobile.AccessLevel, CommandLogging.Format(e.Mobile), CommandLogging.Format(obj));

                ((BaseVendor)obj).Restock();
                this.AddResponse("The vendor has been restocked.");
            }
            else
            {
                this.AddResponse("That is not a vendor.");
            }
        }
    }

    public class GetTypeCommand : BaseCommand
    {
        public GetTypeCommand()
        {
            this.AccessLevel = AccessLevel.Counselor;
            this.Supports = CommandSupport.All;
            this.Commands = new string[] { "GetType" };
            this.ObjectTypes = ObjectTypes.All;
            this.Usage = "GetType";
            this.Description = "Gets the type name of a targeted object.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj == null)
            {
                this.AddResponse("The object is null.");
            }
            else
            {
                Type type = obj.GetType();

                if (type.DeclaringType == null)
                    this.AddResponse(String.Format("The type of that object is {0}.", type.Name));
                else
                    this.AddResponse(String.Format("The type of that object is {0}.", type.FullName));
            }
        }
    }

    public class GetCommand : BaseCommand
    {
        public GetCommand()
        {
            this.AccessLevel = AccessLevel.Counselor;
            this.Supports = CommandSupport.All;
            this.Commands = new string[] { "Get" };
            this.ObjectTypes = ObjectTypes.All;
            this.Usage = "Get <propertyName>";
            this.Description = "Gets one or more property values by name of a targeted object.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Length >= 1)
            {
                for (int i = 0; i < e.Length; ++i)
                {
                    string result = Properties.GetValue(e.Mobile, obj, e.GetString(i));

                    if (result == "Property not found." || result == "Property is write only." || result.StartsWith("Getting this property"))
                        this.LogFailure(result);
                    else
                        this.AddResponse(result);
                }
            }
            else
            {
                this.LogFailure("Format: Get <propertyName>");
            }
        }
    }

    public class AliasedSetCommand : BaseCommand
    {
        private readonly string m_Name;
        private readonly string m_Value;
        public AliasedSetCommand(AccessLevel level, string command, string name, string value, ObjectTypes objects)
        {
            this.m_Name = name;
            this.m_Value = value;

            this.AccessLevel = level;

            if (objects == ObjectTypes.Items)
                this.Supports = CommandSupport.AllItems;
            else if (objects == ObjectTypes.Mobiles)
                this.Supports = CommandSupport.AllMobiles;
            else
                this.Supports = CommandSupport.All;

            this.Commands = new string[] { command };
            this.ObjectTypes = objects;
            this.Usage = command;
            this.Description = String.Format("Sets the {0} property to {1}.", name, value);
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            string result = Properties.SetValue(e.Mobile, obj, this.m_Name, this.m_Value);

            if (result == "Property has been set.")
                this.AddResponse(result);
            else
                this.LogFailure(result);
        }
    }

    public class SetCommand : BaseCommand
    {
        public SetCommand()
        {
            this.AccessLevel = AccessLevel.Counselor;
            this.Supports = CommandSupport.All;
            this.Commands = new string[] { "Set" };
            this.ObjectTypes = ObjectTypes.Both;
            this.Usage = "Set <propertyName> <value> [...]";
            this.Description = "Sets one or more property values by name of a targeted object.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (e.Length >= 2)
            {
                for (int i = 0; (i + 1) < e.Length; i += 2)
                {
                    string result = Properties.SetValue(e.Mobile, obj, e.GetString(i), e.GetString(i + 1));

                    if (result == "Property has been set.")
                        this.AddResponse(result);
                    else
                        this.LogFailure(result);
                }
            }
            else
            {
                this.LogFailure("Format: Set <propertyName> <value>");
            }
        }
    }

    public class DeleteCommand : BaseCommand
    {
        public DeleteCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
            this.Commands = new string[] { "Delete", "Remove" };
            this.ObjectTypes = ObjectTypes.Both;
            this.Usage = "Delete";
            this.Description = "Deletes a targeted item or mobile. Does not delete players.";
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 1)
            {
                e.Mobile.SendGump(new WarningGump(1060637, 30720, String.Format("You are about to delete {0} objects. This cannot be undone without a full server revert.<br><br>Continue?", list.Count), 0xFFC000, 420, 280, new WarningGumpCallback(OnConfirmCallback), new object[] { e, list }));
                this.AddResponse("Awaiting confirmation...");
            }
            else
            {
                base.ExecuteList(e, list);
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            if (obj is Item)
            {
                CommandLogging.WriteLine(e.Mobile, "{0} {1} deleting {2}", e.Mobile.AccessLevel, CommandLogging.Format(e.Mobile), CommandLogging.Format(obj));
                ((Item)obj).Delete();
                this.AddResponse("The item has been deleted.");
            }
            else if (obj is Mobile && !((Mobile)obj).Player)
            {
                CommandLogging.WriteLine(e.Mobile, "{0} {1} deleting {2}", e.Mobile.AccessLevel, CommandLogging.Format(e.Mobile), CommandLogging.Format(obj));
                ((Mobile)obj).Delete();
                this.AddResponse("The mobile has been deleted.");
            }
            else
            {
                this.LogFailure("That cannot be deleted.");
            }
        }

        private void OnConfirmCallback(Mobile from, bool okay, object state)
        {
            object[] states = (object[])state;
            CommandEventArgs e = (CommandEventArgs)states[0];
            ArrayList list = (ArrayList)states[1];

            bool flushToLog = false;

            if (okay)
            {
                this.AddResponse("Delete command confirmed.");

                if (list.Count > 20)
                {
                    CommandLogging.Enabled = false;
                    NetState.Pause();
                }

                base.ExecuteList(e, list);

                if (list.Count > 20)
                {
                    NetState.Resume();
                    flushToLog = true;
                    CommandLogging.Enabled = true;
                }
            }
            else
            {
                this.AddResponse("Delete command aborted.");
            }

            this.Flush(from, flushToLog);
        }
    }

    public class KillCommand : BaseCommand
    {
        private readonly bool m_Value;
        public KillCommand(bool value)
        {
            this.m_Value = value;

            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.AllMobiles;
            this.Commands = value ? new string[] { "Kill" } : new string[] { "Resurrect", "Res" };
            this.ObjectTypes = ObjectTypes.Mobiles;

            if (value)
            {
                this.Usage = "Kill";
                this.Description = "Kills a targeted player or npc.";
            }
            else
            {
                this.Usage = "Resurrect";
                this.Description = "Resurrects a targeted ghost.";
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile mob = (Mobile)obj;
            Mobile from = e.Mobile;

            if (this.m_Value)
            {
                if (!mob.Alive)
                {
                    this.LogFailure("They are already dead.");
                }
                else if (!mob.CanBeDamaged())
                {
                    this.LogFailure("They cannot be harmed.");
                }
                else
                {
                    CommandLogging.WriteLine(from, "{0} {1} killing {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(mob));
                    mob.Kill();

                    this.AddResponse("They have been killed.");
                }
            }
            else
            {
                if (mob.IsDeadBondedPet)
                {
                    BaseCreature bc = mob as BaseCreature;

                    if (bc != null)
                    {
                        CommandLogging.WriteLine(from, "{0} {1} resurrecting {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(mob));

                        bc.PlaySound(0x214);
                        bc.FixedEffect(0x376A, 10, 16);

                        bc.ResurrectPet();

                        this.AddResponse("It has been resurrected.");
                    }
                }
                else if (!mob.Alive)
                {
                    CommandLogging.WriteLine(from, "{0} {1} resurrecting {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(mob));

                    mob.PlaySound(0x214);
                    mob.FixedEffect(0x376A, 10, 16);

                    mob.Resurrect();

                    this.AddResponse("They have been resurrected.");
                }
                else
                {
                    this.LogFailure("They are not dead.");
                }
            }
        }
    }

    public class HideCommand : BaseCommand
    {
        private readonly bool m_Value;
        public HideCommand(bool value)
        {
            this.m_Value = value;

            this.AccessLevel = AccessLevel.Counselor;
            this.Supports = CommandSupport.AllMobiles;
            this.Commands = new string[] { value ? "Hide" : "Unhide" };
            this.ObjectTypes = ObjectTypes.Mobiles;

            if (value)
            {
                this.Usage = "Hide";
                this.Description = "Makes a targeted mobile disappear in a puff of smoke.";
            }
            else
            {
                this.Usage = "Unhide";
                this.Description = "Makes a targeted mobile appear in a puff of smoke.";
            }
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile m = (Mobile)obj;

            CommandLogging.WriteLine(e.Mobile, "{0} {1} {2} {3}", e.Mobile.AccessLevel, CommandLogging.Format(e.Mobile), this.m_Value ? "hiding" : "unhiding", CommandLogging.Format(m));

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
            m.Hidden = this.m_Value;

            if (this.m_Value)
                this.AddResponse("They have been hidden.");
            else
                this.AddResponse("They have been revealed.");
        }
    }

    public class FirewallCommand : BaseCommand
    {
        public FirewallCommand()
        {
            this.AccessLevel = AccessLevel.Administrator;
            this.Supports = CommandSupport.AllMobiles;
            this.Commands = new string[] { "Firewall" };
            this.ObjectTypes = ObjectTypes.Mobiles;
            this.Usage = "Firewall";
            this.Description = "Adds a targeted player to the firewall (list of blocked IP addresses). This command does not ban or kick.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Mobile from = e.Mobile;
            Mobile targ = (Mobile)obj;
            NetState state = targ.NetState;

            if (state != null)
            {
                CommandLogging.WriteLine(from, "{0} {1} firewalling {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targ));

                try
                {
                    Firewall.Add(state.Address);
                    this.AddResponse("They have been firewalled.");
                }
                catch (Exception ex)
                {
                    this.LogFailure(ex.Message);
                }
            }
            else
            {
                this.LogFailure("They are not online.");
            }
        }
    }

    public class KickCommand : BaseCommand
    {
        private readonly bool m_Ban;
        public KickCommand(bool ban)
        {
            this.m_Ban = ban;

            this.AccessLevel = (ban ? AccessLevel.Administrator : AccessLevel.GameMaster);
            this.Supports = CommandSupport.AllMobiles;
            this.Commands = new string[] { ban ? "Ban" : "Kick" };
            this.ObjectTypes = ObjectTypes.Mobiles;

            if (ban)
            {
                this.Usage = "Ban";
                this.Description = "Bans the account of a targeted player.";
            }
            else
            {
                this.Usage = "Kick";
                this.Description = "Disconnects a targeted player.";
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
                        CommandLogging.WriteLine(from, "{0} {1} {2} {3}", from.AccessLevel, CommandLogging.Format(from), this.m_Ban ? "banning" : "kicking", CommandLogging.Format(targ));

                        targ.Say("I've been {0}!", this.m_Ban ? "banned" : "kicked");

                        this.AddResponse(String.Format("They have been {0}.", this.m_Ban ? "banned" : "kicked"));

                        targState.Dispose();

                        if (this.m_Ban)
                        {
                            targAccount.Banned = true;
                            targAccount.SetUnspecifiedBan(from);
                            from.SendGump(new BanDurationGump(targAccount));
                        }
                    }
                }
                else if (targState == null)
                {
                    this.LogFailure("They are not online.");
                }
            }
            else
            {
                this.LogFailure("You do not have the required access level to do this.");
            }
        }
    }

    public class TraceLockdownCommand : BaseCommand
    {
        public TraceLockdownCommand()
        {
            this.AccessLevel = AccessLevel.Administrator;
            this.Supports = CommandSupport.Simple;
            this.Commands = new string[] { "TraceLockdown" };
            this.ObjectTypes = ObjectTypes.Items;
            this.Usage = "TraceLockdown";
            this.Description = "Finds the BaseHouse for which a targeted item is locked down or secured.";
        }

        public override void Execute(CommandEventArgs e, object obj)
        {
            Item item = obj as Item;

            if (item == null)
                return;

            if (!item.IsLockedDown && !item.IsSecure)
            {
                this.LogFailure("That is not locked down.");
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

            this.LogFailure("No house was found.");
        }
    }
}