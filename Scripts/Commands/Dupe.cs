#region References
using System;
using System.Linq;

using Server.Items;
using Server.Targeting;
#endregion

namespace Server.Commands
{
    public class Dupe
    {
        public static void Initialize()
        {
            CommandSystem.Register("Dupe", AccessLevel.GameMaster, Dupe_OnCommand);
            CommandSystem.Register("DupeInBag", AccessLevel.GameMaster, DupeInBag_OnCommand);
        }

        [Usage("Dupe [amount]")]
        [Description("Dupes a targeted item.")]
        private static void Dupe_OnCommand(CommandEventArgs e)
        {
            var amount = 1;

            if (e.Length > 0)
            {
                amount = e.GetInt32(0);
            }

            e.Mobile.Target = new DupeTarget(false, Math.Max(1, amount));
            e.Mobile.SendMessage("What do you wish to dupe?");
        }

        [Usage("DupeInBag <count>")]
        [Description("Dupes an item at it's current location (count) number of times.")]
        private static void DupeInBag_OnCommand(CommandEventArgs e)
        {
            var amount = 1;

            if (e.Length > 0)
            {
                amount = e.GetInt32(0);
            }

            e.Mobile.Target = new DupeTarget(true, Math.Max(1, amount));
            e.Mobile.SendMessage("What do you wish to dupe?");
        }

        private class DupeTarget : Target
        {
            private readonly bool _InBag;
            private readonly int _Amount;

            public DupeTarget(bool inbag, int amount)
                : base(15, false, TargetFlags.None)
            {
                _InBag = inbag;
                _Amount = amount;
            }

            protected override void OnTarget(Mobile m, object targ)
            {
                var done = false;

                if (!(targ is Item))
                {
                    m.SendMessage("You can only dupe items.");
                    return;
                }

                CommandLogging.WriteLine(
                    m,
                    "{0} {1} duping {2} (inBag={3}; amount={4})",
                    m.AccessLevel,
                    CommandLogging.Format(m),
                    CommandLogging.Format(targ),
                    _InBag,
                    _Amount);

                var item = (Item)targ;

                Container pack;

                if (_InBag)
                {
                    if (item.Parent is Container)
                    {
                        pack = (Container)item.Parent;
                    }
                    else if (item.Parent is Mobile)
                    {
                        pack = ((Mobile)item.Parent).Backpack;
                    }
                    else
                    {
                        pack = null;
                    }
                }
                else
                {
                    pack = m.Backpack;
                }

                var t = item.GetType();

                var a = t.GetCustomAttributes(typeof(ConstructableAttribute), false);

                if (a.OfType<ConstructableAttribute>().Any(ca => ca.AccessLevel > m.AccessLevel))
                {
                    return;
                }

                try
                {
                    m.SendMessage("Duping {0}...", _Amount);

                    var noCtor = false;

                    for (var i = 0; i < _Amount; i++)
                    {
                        Item o;

                        try
                        {
                            o = Activator.CreateInstance(t, true) as Item;
                        }
                        catch
                        {
                            o = null;
                        }

                        if (o == null)
                        {
                            noCtor = true;
                            break;
                        }

                        CopyProperties(item, o);

                        o.Parent = null;

                        item.OnAfterDuped(o);

                        if (item is Container && o is Container)
                        {
                            m.SendMessage("Duping Container Children...");
                            DupeChildren(m, (Container)item, (Container)o);
                        }

                        if (pack != null)
                        {
                            pack.DropItem(o);
                        }
                        else
                        {
                            o.MoveToWorld(m.Location, m.Map);
                        }

                        o.UpdateTotals();
                        o.InvalidateProperties();
                        o.Delta(ItemDelta.Update);

                        CommandLogging.WriteLine(
                            m,
                            "{0} {1} duped {2} creating {3}",
                            m.AccessLevel,
                            CommandLogging.Format(m),
                            CommandLogging.Format(item),
                            CommandLogging.Format(o));
                    }

                    if (!noCtor)
                    {
                        m.SendMessage("Done");
                        done = true;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);

                    m.SendMessage("Error!");
                    return;
                }

                if (!done)
                {
                    m.SendMessage("Unable to dupe.  Item must have a 0 parameter constructor.");
                }
                else
                {
                    item.Delta(ItemDelta.Update);
                }
            }
        }

        public static Item DupeItem(Item item)
        {
            return DupeItem(null, item);
        }

        public static Item DupeItem(Mobile m, Item item)
        {
            try
            {
                var t = item.GetType();

                if (m != null)
                {
                    var a = t.GetCustomAttributes(typeof(ConstructableAttribute), false);

                    if (a.OfType<ConstructableAttribute>().Any(ca => ca.AccessLevel > m.AccessLevel))
                    {
                        return null;
                    }
                }

                Item o;

                try
                {
                    o = Activator.CreateInstance(t, true) as Item;
                }
                catch
                {
                    o = null;
                }

                if (o == null)
                {
                    return null;
                }

                CopyProperties(item, o);

                o.Parent = null;

                item.OnAfterDuped(o);

                if (item is Container && o is Container)
                {
                    DupeChildren(m, (Container)item, (Container)o);
                }

                if (m != null)
                {
                    o.MoveToWorld(m.Location, m.Map);

                    o.UpdateTotals();
                    o.InvalidateProperties();
                    o.Delta(ItemDelta.Update);

                    CommandLogging.WriteLine(m, "{0} {1} duped {2} creating {3}", m.AccessLevel, CommandLogging.Format(m),
                                             CommandLogging.Format(item), CommandLogging.Format(o));
                }

                item.Delta(ItemDelta.Update);

                return o;
            }
            catch
            {
                return null;
            }
        }

        public static void DupeChildren(Container src, Container dest)
        {
            DupeChildren(null, src, dest);
        }

        public static void DupeChildren(Mobile m, Container src, Container dest)
        {
            foreach (var item in src.Items)
            {
                try
                {
                    var t = item.GetType();

                    if (m != null)
                    {
                        var a = t.GetCustomAttributes(typeof(ConstructableAttribute), false);

                        if (a.OfType<ConstructableAttribute>().Any(ca => ca.AccessLevel > m.AccessLevel))
                        {
                            continue;
                        }
                    }

                    Item o;

                    try
                    {
                        o = Activator.CreateInstance(t, true) as Item;
                    }
                    catch
                    {
                        o = null;
                    }

                    if (o == null)
                    {
                        continue;
                    }

                    CopyProperties(item, o);

                    o.Parent = null;

                    item.OnAfterDuped(o);

                    if (item is Container && o is Container)
                    {
                        DupeChildren(m, (Container)item, (Container)o);
                    }

                    dest.DropItem(o);
                    o.Location = item.Location;

                    o.UpdateTotals();
                    o.InvalidateProperties();
                    o.Delta(ItemDelta.Update);

                    CommandLogging.WriteLine(
                        m,
                        "{0} {1} duped {2} creating {3}",
                        m.AccessLevel,
                        CommandLogging.Format(m),
                        CommandLogging.Format(item),
                        CommandLogging.Format(o));

                    item.Delta(ItemDelta.Update);
                }
                catch
                { }
            }
        }

        public static void CopyProperties(object src, object dest)
        {
            var props = src.GetType().GetProperties();

            foreach (var p in props)
            {
                try
                {
                    if (p.CanRead && p.CanWrite)
                    {
                        p.SetValue(dest, p.GetValue(src, null), null);
                    }
                }
                catch
                { }
            }
        }
    }
}