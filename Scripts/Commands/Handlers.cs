using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server.Commands.Generic;
using Server.Gumps;
using Server.Items;
using Server.Menus.ItemLists;
using Server.Menus.Questions;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using Server.Targets;

namespace Server.Commands
{
    public class CommandHandlers
    {
        public static void Initialize()
        {
            CommandSystem.Prefix = "[";

            Register("Go", AccessLevel.Counselor, new CommandEventHandler(Go_OnCommand));

            Register("DropHolding", AccessLevel.Counselor, new CommandEventHandler(DropHolding_OnCommand));

            Register("GetFollowers", AccessLevel.GameMaster, new CommandEventHandler(GetFollowers_OnCommand));

            Register("ClearFacet", AccessLevel.Administrator, new CommandEventHandler(ClearFacet_OnCommand));

            Register("Where", AccessLevel.Counselor, new CommandEventHandler(Where_OnCommand));

            Register("AutoPageNotify", AccessLevel.Counselor, new CommandEventHandler(APN_OnCommand));
            Register("APN", AccessLevel.Counselor, new CommandEventHandler(APN_OnCommand));

            Register("Animate", AccessLevel.GameMaster, new CommandEventHandler(Animate_OnCommand));

            Register("Cast", AccessLevel.Counselor, new CommandEventHandler(Cast_OnCommand));

            Register("Stuck", AccessLevel.Counselor, new CommandEventHandler(Stuck_OnCommand));

            Register("Help", AccessLevel.Player, new CommandEventHandler(Help_OnCommand));

            Register("Save", AccessLevel.Administrator, new CommandEventHandler(Save_OnCommand));
            Register("BackgroundSave", AccessLevel.Administrator, new CommandEventHandler(BackgroundSave_OnCommand));
            Register("BGSave", AccessLevel.Administrator, new CommandEventHandler(BackgroundSave_OnCommand));
            Register("SaveBG", AccessLevel.Administrator, new CommandEventHandler(BackgroundSave_OnCommand));

            Register("Move", AccessLevel.GameMaster, new CommandEventHandler(Move_OnCommand));
            Register("Client", AccessLevel.Counselor, new CommandEventHandler(Client_OnCommand));

            Register("SMsg", AccessLevel.Counselor, new CommandEventHandler(StaffMessage_OnCommand));
            Register("SM", AccessLevel.Counselor, new CommandEventHandler(StaffMessage_OnCommand));
            Register("S", AccessLevel.Counselor, new CommandEventHandler(StaffMessage_OnCommand));

            Register("BCast", AccessLevel.GameMaster, new CommandEventHandler(BroadcastMessage_OnCommand));
            Register("BC", AccessLevel.GameMaster, new CommandEventHandler(BroadcastMessage_OnCommand));
            Register("B", AccessLevel.GameMaster, new CommandEventHandler(BroadcastMessage_OnCommand));

            Register("Bank", AccessLevel.GameMaster, new CommandEventHandler(Bank_OnCommand));

            Register("Echo", AccessLevel.Counselor, new CommandEventHandler(Echo_OnCommand));

            Register("Sound", AccessLevel.GameMaster, new CommandEventHandler(Sound_OnCommand));

            Register("ViewEquip", AccessLevel.GameMaster, new CommandEventHandler(ViewEquip_OnCommand));

            Register("Light", AccessLevel.Counselor, new CommandEventHandler(Light_OnCommand));
            Register("Stats", AccessLevel.Counselor, new CommandEventHandler(Stats_OnCommand));

            Register("ReplaceBankers", AccessLevel.Administrator, new CommandEventHandler(ReplaceBankers_OnCommand));

            Register("SpeedBoost", AccessLevel.Counselor, new CommandEventHandler(SpeedBoost_OnCommand));
        }

        public static void Register(string command, AccessLevel access, CommandEventHandler handler)
        {
            CommandSystem.Register(command, access, handler);
        }

        [Usage("Where")]
        [Description("Tells the commanding player his coordinates, region, and facet.")]
        public static void Where_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Map map = from.Map;

            from.SendMessage("You are at {0} {1} {2} in {3}.", from.X, from.Y, from.Z, map);

            if (map != null)
            {
                Region reg = from.Region;

                if (!reg.IsDefault)
                {
                    StringBuilder builder = new StringBuilder();

                    builder.Append(reg.ToString());
                    reg = reg.Parent;

                    while (reg != null)
                    {
                        builder.Append(" <- " + reg.ToString());
                        reg = reg.Parent;
                    }

                    from.SendMessage("Your region is {0}.", builder.ToString());
                }
            }
        }

        [Usage("DropHolding")]
        [Description("Drops the item, if any, that a targeted player is holding. The item is placed into their backpack, or if that's full, at their feet.")]
        public static void DropHolding_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(DropHolding_OnTarget));
            e.Mobile.SendMessage("Target the player to drop what they are holding.");
        }

        public static void DropHolding_OnTarget(Mobile from, object obj)
        {
            if (obj is Mobile && ((Mobile)obj).Player)
            {
                Mobile targ = (Mobile)obj;
                Item held = targ.Holding;

                if (held == null)
                {
                    from.SendMessage("They are not holding anything.");
                }
                else
                {
                    if (from.AccessLevel == AccessLevel.Counselor)
                    {
                        Engines.Help.PageEntry pe = Engines.Help.PageQueue.GetEntry(targ);

                        if (pe == null || pe.Handler != from)
                        {
                            if (pe == null)
                                from.SendMessage("You may only use this command on someone who has paged you.");
                            else
                                from.SendMessage("You may only use this command if you are handling their help page.");

                            return;
                        }
                    }

                    if (targ.AddToBackpack(held))
                        from.SendMessage("The item they were holding has been placed into their backpack.");
                    else
                        from.SendMessage("The item they were holding has been placed at their feet.");

                    held.ClearBounce();

                    targ.Holding = null;
                }
            }
            else
            {
                from.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(DropHolding_OnTarget));
                from.SendMessage("That is not a player. Try again.");
            }
        }

        public static void DeleteList_Callback(Mobile from, bool okay, object state)
        {
            if (okay)
            {
                List<IEntity> list = (List<IEntity>)state;

                CommandLogging.WriteLine(from, "{0} {1} deleting {2} object{3}", from.AccessLevel, CommandLogging.Format(from), list.Count, list.Count == 1 ? "" : "s");

                NetState.Pause();

                for (int i = 0; i < list.Count; ++i)
                    list[i].Delete();

                NetState.Resume();

                from.SendMessage("You have deleted {0} object{1}.", list.Count, list.Count == 1 ? "" : "s");
            }
            else
            {
                from.SendMessage("You have chosen not to delete those objects.");
            }
        }

        [Usage("ClearFacet")]
        [Description("Deletes all items and mobiles in your facet. Players and their inventory will not be deleted.")]
        public static void ClearFacet_OnCommand(CommandEventArgs e)
        {
            Map map = e.Mobile.Map;

            if (map == null || map == Map.Internal)
            {
                e.Mobile.SendMessage("You may not run that command here.");
                return;
            }

            List<IEntity> list = new List<IEntity>();

            foreach (Item item in World.Items.Values)
                if (item.Map == map && item.Parent == null)
                    list.Add(item);

            foreach (Mobile m in World.Mobiles.Values)
                if (m.Map == map && !m.Player)
                    list.Add(m);

            if (list.Count > 0)
            {
                CommandLogging.WriteLine(e.Mobile, "{0} {1} starting facet clear of {2} ({3} object{4})", e.Mobile.AccessLevel, CommandLogging.Format(e.Mobile), map, list.Count, list.Count == 1 ? "" : "s");

                e.Mobile.SendGump(
                    new WarningGump(1060635, 30720,
                        String.Format("You are about to delete {0} object{1} from this facet.  Do you really wish to continue?",
                            list.Count, list.Count == 1 ? "" : "s"),
                        0xFFC000, 360, 260, new WarningGumpCallback(DeleteList_Callback), list));
            }
            else
            {
                e.Mobile.SendMessage("There were no objects found to delete.");
            }
        }

        [Usage("GetFollowers")]
        [Description("Teleports all pets of a targeted player to your location.")]
        public static void GetFollowers_OnCommand(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(GetFollowers_OnTarget));
            e.Mobile.SendMessage("Target a player to get their pets.");
        }

        public static void GetFollowers_OnTarget(Mobile from, object obj)
        {
            if (obj is PlayerMobile)
            {
                PlayerMobile master = (PlayerMobile)obj;
                List<Mobile> pets = master.AllFollowers;

                if (pets.Count > 0)
                {
                    CommandLogging.WriteLine(from, "{0} {1} getting all followers of {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(master));

                    from.SendMessage("That player has {0} pet{1}.", pets.Count, pets.Count != 1 ? "s" : "");

                    for (int i = 0; i < pets.Count; ++i)
                    {
                        Mobile pet = (Mobile)pets[i];

                        if (pet is IMount)
                            ((IMount)pet).Rider = null; // make sure it's dismounted

                        pet.MoveToWorld(from.Location, from.Map);
                    }
                }
                else
                {
                    from.SendMessage("There were no pets found for that player.");
                }
            }
            else if (obj is Mobile && ((Mobile)obj).Player)
            {
                Mobile master = (Mobile)obj;
                ArrayList pets = new ArrayList();

                foreach (Mobile m in World.Mobiles.Values)
                {
                    if (m is BaseCreature)
                    {
                        BaseCreature bc = (BaseCreature)m;

                        if ((bc.Controlled && bc.ControlMaster == master) || (bc.Summoned && bc.SummonMaster == master))
                            pets.Add(bc);
                    }
                }

                if (pets.Count > 0)
                {
                    CommandLogging.WriteLine(from, "{0} {1} getting all followers of {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(master));

                    from.SendMessage("That player has {0} pet{1}.", pets.Count, pets.Count != 1 ? "s" : "");

                    for (int i = 0; i < pets.Count; ++i)
                    {
                        Mobile pet = (Mobile)pets[i];

                        if (pet is IMount)
                            ((IMount)pet).Rider = null; // make sure it's dismounted

                        pet.MoveToWorld(from.Location, from.Map);
                    }
                }
                else
                {
                    from.SendMessage("There were no pets found for that player.");
                }
            }
            else
            {
                from.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(GetFollowers_OnTarget));
                from.SendMessage("That is not a player. Try again.");
            }
        }

        public static void ReplaceBankers_OnCommand(CommandEventArgs e)
        {
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in World.Mobiles.Values)
                if ((m is Banker) && !(m is BaseCreature))
                    list.Add(m);

            foreach (Mobile m in list)
            {
                Map map = m.Map;

                if (map != null)
                {
                    bool hasBankerSpawner = false;

                    foreach (Item item in m.GetItemsInRange(0))
                    {
                        if (item is Spawner)
                        {
                            Spawner spawner = (Spawner)item;

                            for (int i = 0; !hasBankerSpawner && i < spawner.SpawnNames.Count; ++i)
                                hasBankerSpawner = Insensitive.Equals((string)spawner.SpawnNames[i], "banker");

                            if (hasBankerSpawner)
                                break;
                        }
                    }

                    if (!hasBankerSpawner)
                    {
                        Spawner spawner = new Spawner(1, 1, 5, 0, 4, "banker");

                        spawner.MoveToWorld(m.Location, map);
                    }
                }
            }
        }

        [Usage("ViewEquip")]
        [Description("Lists equipment of a targeted mobile. From the list you can move, delete, or open props.")]
        public static void ViewEquip_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new ViewEqTarget();
        }

        [Usage("Sound <index> [toAll=true]")]
        [Description("Plays a sound to players within 12 tiles of you. The (toAll) argument specifies to everyone, or just those who can see you.")]
        public static void Sound_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 1)
                PlaySound(e.Mobile, e.GetInt32(0), true);
            else if (e.Length == 2)
                PlaySound(e.Mobile, e.GetInt32(0), e.GetBoolean(1));
            else
                e.Mobile.SendMessage("Format: Sound <index> [toAll]");
        }

        [Usage("Echo <text>")]
        [Description("Relays (text) as a system message.")]
        public static void Echo_OnCommand(CommandEventArgs e)
        {
            string toEcho = e.ArgString.Trim();

            if (toEcho.Length > 0)
                e.Mobile.SendMessage(toEcho);
            else
                e.Mobile.SendMessage("Format: Echo \"<text>\"");
        }

        [Usage("Bank")]
        [Description("Opens the bank box of a given target.")]
        public static void Bank_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new BankTarget();
        }

        [Usage("Help")]
        [Description("Lists all available commands.")]
        public static void Help_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            List<CommandEntry> list = new List<CommandEntry>();

            foreach (CommandEntry entry in CommandSystem.Entries.Values)
                if (m.AccessLevel >= entry.AccessLevel)
                    list.Add(entry);

            list.Sort();

            StringBuilder sb = new StringBuilder();

            if (list.Count > 0)
                sb.Append(list[0].Command);

            for (int i = 1; i < list.Count; ++i)
            {
                string v = list[i].Command;

                if ((sb.Length + 1 + v.Length) >= 256)
                {
                    m.SendAsciiMessage(0x482, sb.ToString());
                    sb = new StringBuilder();
                    sb.Append(v);
                }
                else
                {
                    sb.Append(' ');
                    sb.Append(v);
                }
            }

            if (sb.Length > 0)
                m.SendAsciiMessage(0x482, sb.ToString());
        }

        [Usage("SMsg <text>")]
        [Aliases("S", "SM")]
        [Description("Broadcasts a message to all online staff.")]
        public static void StaffMessage_OnCommand(CommandEventArgs e)
        {
            BroadcastMessage(AccessLevel.Counselor, e.Mobile.SpeechHue, String.Format("[{0}] {1}", e.Mobile.Name, e.ArgString));
        }

        [Usage("BCast <text>")]
        [Aliases("B", "BC")]
        [Description("Broadcasts a message to everyone online.")]
        public static void BroadcastMessage_OnCommand(CommandEventArgs e)
        {
            BroadcastMessage(AccessLevel.Player, 0x482, String.Format("Staff message from {0}:", e.Mobile.Name));
            BroadcastMessage(AccessLevel.Player, 0x482, e.ArgString);
        }

        public static void BroadcastMessage(AccessLevel ac, int hue, string message) 
        { 
            foreach (NetState state in NetState.Instances)
            {
                Mobile m = state.Mobile;

                if (m != null && m.AccessLevel >= ac)
                    m.SendMessage(hue, message);
            }
        }

        [Usage("AutoPageNotify")]
        [Aliases("APN")]
        [Description("Toggles your auto-page-notify status.")]
        public static void APN_OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            m.AutoPageNotify = !m.AutoPageNotify;

            m.SendMessage("Your auto-page-notify has been turned {0}.", m.AutoPageNotify ? "on" : "off");
        }

        [Usage("Animate <action> <frameCount> <repeatCount> <forward> <repeat> <delay>")]
        [Description("Makes your character do a specified animation.")]
        public static void Animate_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 6)
            {
                e.Mobile.Animate(e.GetInt32(0), e.GetInt32(1), e.GetInt32(2), e.GetBoolean(3), e.GetBoolean(4), e.GetInt32(5));
            }
            else
            {
                e.Mobile.SendMessage("Format: Animate <action> <frameCount> <repeatCount> <forward> <repeat> <delay>");
            }
        }

        [Usage("Cast <name>")]
        [Description("Casts a spell by name.")]
        public static void Cast_OnCommand(CommandEventArgs e)
        {
            if (e.Length == 1)
            {
                if (!Multis.DesignContext.Check(e.Mobile))
                    return; // They are customizing

                Spell spell = SpellRegistry.NewSpell(e.GetString(0), e.Mobile, null);

                if (spell != null)
                    spell.Cast();
                else
                    e.Mobile.SendMessage("That spell was not found.");
            }
            else
            {
                e.Mobile.SendMessage("Format: Cast <name>");
            }
        }

        [Usage("Stuck")]
        [Description("Opens a menu of towns, used for teleporting stuck mobiles.")]
        public static void Stuck_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new StuckMenuTarget();
        }

        [Usage("Light <level>")]
        [Description("Set your local lightlevel.")]
        public static void Light_OnCommand(CommandEventArgs e)
        {
            e.Mobile.LightLevel = e.GetInt32(0);
        }

        [Usage("Stats")]
        [Description("View some stats about the server.")]
        public static void Stats_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage("Open Connections: {0}", Network.NetState.Instances.Count);
            e.Mobile.SendMessage("Mobiles: {0}", World.Mobiles.Count);
            e.Mobile.SendMessage("Items: {0}", World.Items.Count);
        }

        [Usage("SpeedBoost [true|false]")]
        [Description("Enables a speed boost for the invoker.  Disable with paramaters.")]
        private static void SpeedBoost_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length <= 1)
            {
                if (e.Length == 1 && !e.GetBoolean(0))
                {
                    from.Send(SpeedControl.Disable);
                    from.SendMessage("Speed boost has been disabled.");
                }
                else
                {
                    from.Send(SpeedControl.MountSpeed);
                    from.SendMessage("Speed boost has been enabled.");
                }
            }
            else
            {
                from.SendMessage("Format: SpeedBoost [true|false]");
            }
        }

        private static void PlaySound(Mobile m, int index, bool toAll)
        {
            Map map = m.Map;

            if (map == null)
                return;

            CommandLogging.WriteLine(m, "{0} {1} playing sound {2} (toAll={3})", m.AccessLevel, CommandLogging.Format(m), index, toAll);

            Packet p = new PlaySound(index, m.Location);

            p.Acquire();

            foreach (NetState state in m.GetClientsInRange(12))
            {
                if (toAll || state.Mobile.CanSee(m))
                    state.Send(p);
            }

            p.Release();
        }

        [Usage("Client")]
        [Description("Opens the client gump menu for a given player.")]
        private static void Client_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new ClientTarget();
        }

        [Usage("Move")]
        [Description("Repositions a targeted item or mobile.")]
        private static void Move_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new PickMoveTarget();
        }

        [Usage("Save")]
        [Description("Saves the world.")]
        private static void Save_OnCommand(CommandEventArgs e)
        {
            Misc.AutoSave.Save();
        }

        [Usage("BackgroundSave")]
        [Aliases("BGSave", "SaveBG")]
        [Description("Saves the world, writing to the disk in the background")]
        private static void BackgroundSave_OnCommand(CommandEventArgs e)
        {
            Misc.AutoSave.Save(true);
        }

        private static bool FixMap(ref Map map, ref Point3D loc, Item item)
        {
            if (map == null || map == Map.Internal)
            {
                Mobile m = item.RootParent as Mobile;

                return (m != null && FixMap(ref map, ref loc, m));
            }

            return true;
        }

        private static bool FixMap(ref Map map, ref Point3D loc, Mobile m)
        {
            if (map == null || map == Map.Internal)
            {
                map = m.LogoutMap;
                loc = m.LogoutLocation;
            }

            return (map != null && map != Map.Internal);
        }

        [Usage("Go [name | serial | (x y [z]) | (deg min (N | S) deg min (E | W))]")]
        [Description("With no arguments, this command brings up the go menu. With one argument, (name), you are moved to that regions \"go location.\" Or, if a numerical value is specified for one argument, (serial), you are moved to that object. Two or three arguments, (x y [z]), will move your character to that location. When six arguments are specified, (deg min (N | S) deg min (E | W)), your character will go to an approximate of those sextant coordinates.")]
        private static void Go_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length == 0)
            {
                GoGump.DisplayTo(from);
                return;
            }

            if (e.Length == 1)
            {
                try
                {
                    int ser = e.GetInt32(0);

                    IEntity ent = World.FindEntity(ser);

                    if (ent is Item)
                    {
                        Item item = (Item)ent;

                        Map map = item.Map;
                        Point3D loc = item.GetWorldLocation();

                        Mobile owner = item.RootParent as Mobile;

                        if (owner != null && (owner.Map != null && owner.Map != Map.Internal) && !BaseCommand.IsAccessible(from, owner) /* !from.CanSee( owner )*/)
                        {
                            from.SendMessage("You can not go to what you can not see.");
                            return;
                        }
                        else if (owner != null && (owner.Map == null || owner.Map == Map.Internal) && owner.Hidden && owner.AccessLevel >= from.AccessLevel)
                        {
                            from.SendMessage("You can not go to what you can not see.");
                            return;
                        }
                        else if (!FixMap(ref map, ref loc, item))
                        {
                            from.SendMessage("That is an internal item and you cannot go to it.");
                            return;
                        }

                        from.MoveToWorld(loc, map);

                        return;
                    }
                    else if (ent is Mobile)
                    {
                        Mobile m = (Mobile)ent;

                        Map map = m.Map;
                        Point3D loc = m.Location;

                        Mobile owner = m;

                        if (owner != null && (owner.Map != null && owner.Map != Map.Internal) && !BaseCommand.IsAccessible(from, owner) /* !from.CanSee( owner )*/)
                        {
                            from.SendMessage("You can not go to what you can not see.");
                            return;
                        }
                        else if (owner != null && (owner.Map == null || owner.Map == Map.Internal) && owner.Hidden && owner.AccessLevel >= from.AccessLevel)
                        {
                            from.SendMessage("You can not go to what you can not see.");
                            return;
                        }
                        else if (!FixMap(ref map, ref loc, m))
                        {
                            from.SendMessage("That is an internal mobile and you cannot go to it.");
                            return;
                        }

                        from.MoveToWorld(loc, map);

                        return;
                    }
                    else
                    {
                        string name = e.GetString(0);
                        Map map;

                        for (int i = 0; i < Map.AllMaps.Count; ++i)
                        {
                            map = Map.AllMaps[i];

                            if (map.MapIndex == 0x7F || map.MapIndex == 0xFF)
                                continue;

                            if (Insensitive.Equals(name, map.Name))
                            {
                                from.Map = map;
                                return;
                            }
                        }

                        Dictionary<string, Region> list = from.Map.Regions;

                        foreach (KeyValuePair<string, Region> kvp in list)
                        {
                            Region r = kvp.Value;

                            if (Insensitive.Equals(r.Name, name))
                            {
                                from.Location = new Point3D(r.GoLocation);
                                return;
                            }
                        }

                        for (int i = 0; i < Map.AllMaps.Count; ++i)
                        {
                            Map m = Map.AllMaps[i];

                            if (m.MapIndex == 0x7F || m.MapIndex == 0xFF || from.Map == m)
                                continue;

                            foreach (Region r in m.Regions.Values)
                            {
                                if (Insensitive.Equals(r.Name, name))
                                {
                                    from.MoveToWorld(r.GoLocation, m);
                                    return;
                                }
                            }
                        }

                        if (ser != 0)
                            from.SendMessage("No object with that serial was found.");
                        else
                            from.SendMessage("No region with that name was found.");

                        return;
                    }
                }
                catch
                {
                }

                from.SendMessage("Region name not found");
            }
            else if (e.Length == 2 || e.Length == 3)
            {
                Map map = from.Map;

                if (map != null)
                {
                    try
                    {
                        /*
                        * This to avoid being teleported to (0,0) if trying to teleport
                        * to a region with spaces in its name.
                        */
                        int x = int.Parse(e.GetString(0));
                        int y = int.Parse(e.GetString(1));
                        int z = (e.Length == 3) ? int.Parse(e.GetString(2)) : map.GetAverageZ(x, y);

                        from.Location = new Point3D(x, y, z);
                    }
                    catch
                    {
                        from.SendMessage("Region name not found.");
                    }
                }
            }
            else if (e.Length == 6)
            {
                Map map = from.Map;

                if (map != null)
                {
                    Point3D p = Sextant.ReverseLookup(map, e.GetInt32(3), e.GetInt32(0), e.GetInt32(4), e.GetInt32(1), Insensitive.Equals(e.GetString(5), "E"), Insensitive.Equals(e.GetString(2), "S"));

                    if (p != Point3D.Zero)
                        from.Location = p;
                    else
                        from.SendMessage("Sextant reverse lookup failed.");
                }
            }
            else
            {
                from.SendMessage("Format: Go [name | serial | (x y [z]) | (deg min (N | S) deg min (E | W)]");
            }
        }

        private class ViewEqTarget : Target
        {
            public ViewEqTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!BaseCommand.IsAccessible(from, targeted))
                {
                    from.SendMessage("That is not accessible.");
                    return;
                }

                if (targeted is Mobile)
                    from.SendMenu(new EquipMenu(from, (Mobile)targeted, GetEquip((Mobile)targeted)));
            }

            private static ItemListEntry[] GetEquip(Mobile m)
            {
                ItemListEntry[] entries = new ItemListEntry[m.Items.Count];

                for (int i = 0; i < m.Items.Count; ++i)
                {
                    Item item = m.Items[i];

                    entries[i] = new ItemListEntry(String.Format("{0}: {1}", item.Layer, item.GetType().Name), item.ItemID, item.Hue);
                }

                return entries;
            }

            private class EquipMenu : ItemListMenu
            {
                private readonly Mobile m_Mobile;
                public EquipMenu(Mobile from, Mobile m, ItemListEntry[] entries)
                    : base("Equipment", entries)
                {
                    this.m_Mobile = m;

                    CommandLogging.WriteLine(from, "{0} {1} viewing equipment of {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(m));
                }

                public override void OnResponse(NetState state, int index)
                {
                    if (index >= 0 && index < this.m_Mobile.Items.Count)
                    {
                        Item item = this.m_Mobile.Items[index];

                        state.Mobile.SendMenu(new EquipDetailsMenu(this.m_Mobile, item));
                    }
                }

                private class EquipDetailsMenu : QuestionMenu
                {
                    private readonly Mobile m_Mobile;
                    private readonly Item m_Item;
                    public EquipDetailsMenu(Mobile m, Item item)
                        : base(String.Format("{0}: {1}", item.Layer, item.GetType().Name), new string[] { "Move", "Delete", "Props" })
                    {
                        this.m_Mobile = m;
                        this.m_Item = item;
                    }

                    public override void OnCancel(NetState state)
                    {
                        state.Mobile.SendMenu(new EquipMenu(state.Mobile, this.m_Mobile, ViewEqTarget.GetEquip(this.m_Mobile)));
                    }

                    public override void OnResponse(NetState state, int index)
                    {
                        if (index == 0)
                        {
                            CommandLogging.WriteLine(state.Mobile, "{0} {1} moving equipment item {2} of {3}", state.Mobile.AccessLevel, CommandLogging.Format(state.Mobile), CommandLogging.Format(this.m_Item), CommandLogging.Format(this.m_Mobile));
                            state.Mobile.Target = new MoveTarget(this.m_Item);
                        }
                        else if (index == 1)
                        {
                            CommandLogging.WriteLine(state.Mobile, "{0} {1} deleting equipment item {2} of {3}", state.Mobile.AccessLevel, CommandLogging.Format(state.Mobile), CommandLogging.Format(this.m_Item), CommandLogging.Format(this.m_Mobile));
                            this.m_Item.Delete();
                        }
                        else if (index == 2)
                        {
                            CommandLogging.WriteLine(state.Mobile, "{0} {1} opening properties for equipment item {2} of {3}", state.Mobile.AccessLevel, CommandLogging.Format(state.Mobile), CommandLogging.Format(this.m_Item), CommandLogging.Format(this.m_Mobile));
                            state.Mobile.SendGump(new PropertiesGump(state.Mobile, this.m_Item));
                        }
                    }
                }
            }
        }

        private class BankTarget : Target
        {
            public BankTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    Mobile m = (Mobile)targeted;

                    BankBox box = (m.Player ? m.BankBox : m.FindBankNoCreate());

                    if (box != null)
                    {
                        CommandLogging.WriteLine(from, "{0} {1} opening bank box of {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targeted));

                        if (from == targeted)
                            box.Open();
                        else
                            box.DisplayTo(from);
                    }
                    else
                    {
                        from.SendMessage("They have no bank box.");
                    }
                }
            }
        }

        private class DismountTarget : Target
        {
            public DismountTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    CommandLogging.WriteLine(from, "{0} {1} dismounting {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targeted));

                    Mobile targ = (Mobile)targeted;

                    for (int i = 0; i < targ.Items.Count; ++i)
                    {
                        Item item = targ.Items[i];

                        if (item is IMountItem)
                        {
                            IMount mount = ((IMountItem)item).Mount;

                            if (mount != null)
                                mount.Rider = null;

                            if (targ.Items.IndexOf(item) == -1)
                                --i;
                        }
                    }

                    for (int i = 0; i < targ.Items.Count; ++i)
                    {
                        Item item = targ.Items[i];

                        if (item.Layer == Layer.Mount)
                        {
                            item.Delete();
                            --i;
                        }
                    }
                }
            }
        }

        private class ClientTarget : Target
        {
            public ClientTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;

                    if (targ.NetState != null)
                    {
                        CommandLogging.WriteLine(from, "{0} {1} opening client menu of {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targeted));
                        from.SendGump(new ClientGump(from, targ.NetState));
                    }
                }
            }
        }

        private class StuckMenuTarget : Target
        {
            public StuckMenuTarget()
                : base(-1, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    if (((Mobile)targeted).AccessLevel >= from.AccessLevel && targeted != from)
                        from.SendMessage("You can't do that to someone with higher Accesslevel than you!");
                    else
                        from.SendGump(new StuckMenu(from, (Mobile)targeted, false));
                }
            }
        }
    }
}