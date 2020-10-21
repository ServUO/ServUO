using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Server.Gumps
{
    public abstract class BaseGump : Gump, IDisposable
    {
        public static int CenterLoc = 1114513;     // <center>~1_val~</center>
        public static int AlignRightLoc = 1114514; // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>

        private Gump _Parent;

        public PlayerMobile User { get; }
        public bool Open { get; set; }

        public virtual bool CloseOnMapChange => false;

        public Gump Parent
        {
            get { return _Parent; }
            set
            {
                _Parent = value;

                if (_Parent != null)
                {
                    if (_Parent is BaseGump)
                    {
                        var bGump = (BaseGump)_Parent;

                        if (!bGump.Children.Contains(this))
                        {
                            bGump.Children.Add(this);
                        }
                        else
                        {
                            bGump.Children.Remove(this);
                        }
                    }
                }
            }
        }

        public List<BaseGump> Children { get; set; }

        public BaseGump(PlayerMobile user, int x = 50, int y = 50, BaseGump parent = null)
            : base(x, y)
        {
            if (user == null)
                return;

            Children = new List<BaseGump>();

            User = user;
            Parent = parent;
        }

        ~BaseGump()
        {
            Dispose();
        }

        public static BaseGump SendGump(BaseGump gump)
        {
            if (gump == null)
                return null;

            BaseGump g = gump.User.FindGump(gump.GetType()) as BaseGump;

            if (g == gump)
                gump.Refresh();
            else
                gump.SendGump();

            return gump;
        }

        public virtual void SendGump()
        {
            AddGumpLayout();
            Open = true;
            User.SendGump(this, false);
        }

        public void Dispose()
        {
            ColUtility.ForEach(Children.AsEnumerable(), child => Children.Remove(child));
            Children = null;

            Children = null;
            Parent = null;

            foreach (KeyValuePair<string, Spoof> kvp in _TextTooltips)
            {
                kvp.Value.Free();
            }

            foreach (KeyValuePair<Dictionary<int, string>, Spoof> kvp in _ClilocTooltips)
            {
                kvp.Value.Free();
            }

            _ClilocTooltips.Clear();
            _TextTooltips.Clear();

            OnDispose();
        }

        public virtual void OnDispose()
        {
        }

        public abstract void AddGumpLayout();

        public virtual void Refresh(bool recompile = true, bool close = true)
        {
            OnBeforeRefresh();

            if (User == null || User.NetState == null)
                return;

            if (close)
            {
                User.NetState.Send(new CloseGump(TypeID, 0));
                User.NetState.RemoveGump(this);
            }
            else
            {
                User.NetState.RemoveGump(this);
            }

            if (recompile)
            {
                Entries.Clear();
                AddGumpLayout();
            }

            /*Children.ForEach(child => 
                {
                    if(child.Open)
                        child.Refresh(recompile, close);
                });*/

            User.SendGump(this);
            OnAfterRefresh();
        }

        public void RefreshParent(bool resend = false)
        {
            if (Parent is BaseGump)
                ((BaseGump)Parent).Refresh();

            if (resend)
                Refresh();
        }

        public virtual void OnBeforeRefresh()
        {
        }

        public virtual void OnAfterRefresh()
        {
        }

        public virtual void OnClosed()
        {
            Children.ForEach(child => child.Close());
            Children.Clear();

            Open = false;

            if (Parent != null)
            {
                if (Parent is BaseGump)
                    ((BaseGump)Parent).OnChildClosed(this);

                Parent = null;
            }
        }

        public virtual void OnChildClosed(BaseGump gump)
        {
        }

        public sealed override void OnResponse(NetState state, RelayInfo info)
        {
            OnResponse(info);

            if (info.ButtonID == 0)
                OnClosed();
        }

        public virtual void OnResponse(RelayInfo info)
        {
        }

        public virtual void OnServerClosed(NetState state)
        {
            OnClosed();
        }

        public virtual void Close()
        {
            if (User == null || User.NetState == null)
                return;

            OnServerClose(User.NetState);

            User.Send(new CloseGump(TypeID, 0));
            User.NetState.RemoveGump(this);
        }

        public static T GetGump<T>(PlayerMobile pm, Func<T, bool> predicate) where T : Gump
        {
            return EnumerateGumps<T>(pm).FirstOrDefault(x => predicate == null || predicate(x));
        }

        public static IEnumerable<T> EnumerateGumps<T>(PlayerMobile pm, Func<T, bool> predicate = null) where T : Gump
        {
            NetState ns = pm.NetState;

            if (ns == null)
                yield break;

            foreach (BaseGump gump in ns.Gumps.OfType<BaseGump>().Where(g => g.GetType() == typeof(T) &&
                (predicate == null || predicate(g as T))))
            {
                yield return gump as T;
            }
        }

        public static List<T> GetGumps<T>(PlayerMobile pm) where T : Gump
        {
            NetState ns = pm.NetState;
            List<T> list = new List<T>();

            if (ns == null)
                return list;

            foreach (BaseGump gump in ns.Gumps.OfType<BaseGump>().Where(g => g.GetType() == typeof(T)))
            {
                list.Add(gump as T);
            }

            return list;
        }

        public static List<BaseGump> GetGumps(PlayerMobile pm, bool checkOpen = false)
        {
            NetState ns = pm.NetState;
            List<BaseGump> list = new List<BaseGump>();

            if (ns == null)
                return list;

            foreach (BaseGump gump in ns.Gumps.OfType<BaseGump>().Where(g => (!checkOpen || g.Open)))
            {
                list.Add(gump);
            }

            return list;
        }

        public static void CheckCloseGumps(PlayerMobile pm, bool checkOpen = false)
        {
            NetState ns = pm.NetState;

            if (ns != null)
            {
                List<BaseGump> gumps = GetGumps(pm, checkOpen);

                foreach (BaseGump gump in gumps.Where(g => g.CloseOnMapChange))
                {
                    pm.CloseGump(gump.GetType());
                }

                ColUtility.Free(gumps);
            }
        }

        public new void AddItemProperty(Item item)
        {
            item.SendPropertiesTo(User);

            base.AddItemProperty(item);
        }

        public void AddMobileProperty(Mobile mob)
        {
            mob.SendPropertiesTo(User);

            AddItemProperty(mob.Serial.Value);
        }

        public void AddProperties(Spoof spoof)
        {
            User.Send(spoof.PropertyList);

            AddItemProperty(spoof.Serial.Value);
        }

        #region Formatting
        public static int C16232(int c16)
        {
            c16 &= 0x7FFF;

            int r = (((c16 >> 10) & 0x1F) << 3);
            int g = (((c16 >> 05) & 0x1F) << 3);
            int b = (((c16 >> 00) & 0x1F) << 3);

            return (r << 16) | (g << 8) | (b << 0);
        }

        public static int C16216(int c16)
        {
            return c16 & 0x7FFF;
        }

        public static int C32216(int c32)
        {
            c32 &= 0xFFFFFF;

            int r = (((c32 >> 16) & 0xFF) >> 3);
            int g = (((c32 >> 08) & 0xFF) >> 3);
            int b = (((c32 >> 00) & 0xFF) >> 3);

            return (r << 10) | (g << 5) | (b << 0);
        }

        protected string Color(string color, string str)
        {
            return string.Format("<basefont color={0}>{1}", color, str);
        }

        protected string Color(int color, string str)
        {
            return string.Format("<basefont color=#{0:X6}>{1}", color, str);
        }

        protected string ColorAndCenter(string color, string str)
        {
            return string.Format("<center><basefont color={0}>{1}</center>", color, str);
        }

        protected string ColorAndSize(string color, int size, string str)
        {
            return string.Format("<basefont color={0} size={1}>{2}", color, size.ToString(), str);
        }

        protected string ColorAndCenterAndSize(string color, int size, string str)
        {
            return string.Format("<basefont color={0} size={1}><center>{2}</center>", color, size.ToString(), str);
        }

        protected string ColorAndCenter(int color, string str)
        {
            return string.Format("<basefont color=#{0:X6}><center>{1}</center>", color, str);
        }

        protected string Center(string str)
        {
            return string.Format("<CENTER>{0}</CENTER>", str);
        }

        protected string ColorAndAlignRight(int color, string str)
        {
            return string.Format("<DIV ALIGN=RIGHT><basefont color=#{0:X6}>{1}</DIV>", color, str);
        }

        protected string ColorAndAlignRight(string color, string str)
        {
            return string.Format("<DIV ALIGN=RIGHT><basefont color={0}>{1}</DIV>", color, str);
        }

        protected string AlignRight(string str)
        {
            return string.Format("<DIV ALIGN=RIGHT>{0}</DIV>", str);
        }

        public void AddHtmlTextDefinition(int x, int y, int length, int height, TextDefinition text, bool background, bool scrollbar)
        {
            if (text.Number > 0)
            {
                AddHtmlLocalized(x, y, length, height, text.Number, false, false);
            }
            else if (!string.IsNullOrEmpty(text.String))
            {
                AddHtml(x, y, length, height, text.String, background, scrollbar);
            }
        }

        public void AddHtmlTextDefinition(int x, int y, int length, int height, TextDefinition text, int hue, bool background, bool scrollbar)
        {
            if (text.Number > 0)
            {
                AddHtmlLocalized(x, y, length, height, text.Number, hue, false, false);
            }
            else if (!string.IsNullOrEmpty(text.String))
            {
                AddHtml(x, y, length, height, Color(hue, text.String), background, scrollbar);
            }
        }

        public void AddHtmlLocalizedCentered(int x, int y, int length, int height, int localization, bool background, bool scrollbar)
        {
            AddHtmlLocalized(x, y, length, height, 1113302, string.Format("#{0}", localization), 0, background, scrollbar);
        }

        public void AddHtmlLocalizedCentered(int x, int y, int length, int height, int localization, int hue, bool background, bool scrollbar)
        {
            AddHtmlLocalized(x, y, length, height, 1113302, string.Format("#{0}", localization), hue, background, scrollbar);
        }

        public void AddHtmlLocalizedAlignRight(int x, int y, int length, int height, int localization, bool background, bool scrollbar)
        {
            AddHtmlLocalized(x, y, length, height, 1114514, string.Format("#{0}", localization), 0, background, scrollbar);
        }

        public void AddHtmlLocalizedAlignRight(int x, int y, int length, int height, int localization, int hue, bool background, bool scrollbar)
        {
            AddHtmlLocalized(x, y, length, height, 1114514, string.Format("#{0}", localization), hue, background, scrollbar);
        }
        #endregion

        #region Tooltips
        private readonly Dictionary<string, Spoof> _TextTooltips = new Dictionary<string, Spoof>();
        private readonly Dictionary<Dictionary<int, string>, Spoof> _ClilocTooltips = new Dictionary<Dictionary<int, string>, Spoof>();

        public void AddTooltipTextDefinition(TextDefinition text)
        {
            if (text.Number > 0)
            {
                AddTooltip(text.Number);
            }
            else if (!string.IsNullOrEmpty(text.String))
            {
                AddTooltip(text.String);
            }
        }

        public void AddTooltip(string text, System.Drawing.Color color)
        {
            AddTooltip(string.Empty, text, System.Drawing.Color.Empty, color);
        }

        public void AddTooltip(string title, string text)
        {
            AddTooltip(title, text, System.Drawing.Color.Empty, System.Drawing.Color.Empty);
        }

        public void AddTooltip(int cliloc, string format, params string[] args)
        {
            base.AddTooltip(cliloc, string.Format(format, args));
        }

        public void AddTooltip(int[] clilocs)
        {
            AddTooltip(clilocs, new string[clilocs.Length]);
        }

        public void AddTooltip(string[] args)
        {
            int[] clilocs = new int[Math.Min(Spoof.EmptyClilocs.Length, args.Length)];

            for (int i = 0; i < args.Length; i++)
            {
                if (i >= Spoof.EmptyClilocs.Length)
                    break;

                clilocs[i] = Spoof.EmptyClilocs[i];
            }

            AddTooltip(clilocs, args);
        }

        public void AddTooltip(int[] clilocs, string[] args)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            int emptyIndex = 0;

            for (int i = 0; i < clilocs.Length; i++)
            {
                string str = string.Empty;

                if (i < args.Length)
                {
                    str = args[i] ?? string.Empty;
                }

                int cliloc = clilocs[i];

                if (cliloc <= 0)
                {
                    if (emptyIndex <= Spoof.EmptyClilocs.Length)
                    {
                        cliloc = Spoof.EmptyClilocs[emptyIndex];
                        emptyIndex++;
                    }
                }

                if (cliloc > 0)
                {
                    dictionary[cliloc] = str;
                }
            }

            Spoof spoof;

            if (!_ClilocTooltips.TryGetValue(dictionary, out spoof) || spoof == null || spoof.Deleted)
            {
                spoof = Spoof.Acquire();
            }

            spoof.ClilocTable = dictionary;

            _ClilocTooltips[dictionary] = spoof;
            AddProperties(spoof);
        }

        public void AddTooltip(string title, string text, System.Drawing.Color titleColor, System.Drawing.Color textColor)
        {
            title = title ?? string.Empty;
            text = text ?? string.Empty;

            if (titleColor.IsEmpty || titleColor == System.Drawing.Color.Transparent)
            {
                titleColor = System.Drawing.Color.White;
            }

            if (textColor.IsEmpty || textColor == System.Drawing.Color.Transparent)
            {
                textColor = System.Drawing.Color.White;
            }

            Spoof spoof;

            if (!_TextTooltips.TryGetValue(text, out spoof) || spoof == null || spoof.Deleted)
            {
                spoof = Spoof.Acquire();
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                spoof.Text = string.Concat(string.Format("<basefont color=#{0:X}>{1}", titleColor.ToArgb(), title),
                            '\n',
                            string.Format("<basefont color=#{0:X}>{1}", textColor.ToArgb(), text));
            }
            else
            {
                spoof.Text = string.Format("<basefont color=#{0:X}>{1}", textColor.ToArgb(), text); //  text.WrapUOHtmlColor(textColor, false);
            }

            _TextTooltips[text] = spoof;
            AddProperties(spoof);
        }

        public sealed class Spoof : Entity
        {
            private static readonly char[] _Split = { '\n' };
            private static int _UID = -1;

            private static int NewUID
            {
                get
                {
                    if (_UID == int.MinValue)
                    {
                        _UID = -1;
                    }

                    return --_UID;
                }
            }

            public static int[] EmptyClilocs =
            {
                1042971, 1070722, // ~1_NOTHING~
			    1114057, 1114778, 1114779, // ~1_val~
			    1150541, // ~1_TOKEN~
			    1153153, // ~1_year~
            };

            private static readonly List<Spoof> _SpoofPool = new List<Spoof>();

            public static Spoof Acquire()
            {
                if (_SpoofPool.Count == 0)
                {
                    return new Spoof();
                }
                else
                {
                    Spoof spoof = _SpoofPool[0];
                    _SpoofPool.Remove(spoof);

                    return spoof;
                }
            }

            public void Free()
            {
                Packet.Release(ref _PropertyList);

                _Text = string.Empty;
                _ClilocTable = null;

                _SpoofPool.Add(this);
            }

            private ObjectPropertyList _PropertyList;

            public ObjectPropertyList PropertyList
            {
                get
                {
                    if (_PropertyList == null)
                    {
                        _PropertyList = new ObjectPropertyList(this);

                        if (!string.IsNullOrEmpty(Text))
                        {
                            string text = StripHtmlBreaks(Text, true);

                            if (text.IndexOf('\n') >= 0)
                            {
                                string[] lines = text.Split(_Split);

                                foreach (string str in lines)
                                {
                                    _PropertyList.Add(str);
                                }
                            }
                            else
                            {
                                _PropertyList.Add(text);
                            }
                        }
                        else if (_ClilocTable != null)
                        {
                            foreach (KeyValuePair<int, string> kvp in _ClilocTable)
                            {
                                int cliloc = kvp.Key;
                                string args = kvp.Value;

                                if (cliloc <= 0 && !string.IsNullOrEmpty(args))
                                {
                                    _PropertyList.Add(args);
                                }
                                else if (string.IsNullOrEmpty(args))
                                {
                                    _PropertyList.Add(cliloc);
                                }
                                else
                                {
                                    _PropertyList.Add(cliloc, args);
                                }
                            }
                        }

                        _PropertyList.Terminate();
                        _PropertyList.SetStatic();
                    }

                    return _PropertyList;
                }
            }

            private string _Text = string.Empty;
            public string Text
            {
                get { return _Text ?? string.Empty; }
                set
                {
                    if (_Text != value)
                    {
                        _Text = value;

                        Packet.Release(ref _PropertyList);
                    }
                }
            }

            private Dictionary<int, string> _ClilocTable;
            public Dictionary<int, string> ClilocTable
            {
                get { return _ClilocTable; }
                set
                {
                    if (_ClilocTable != value)
                    {
                        _ClilocTable = value;

                        Packet.Release(ref _PropertyList);
                    }
                }
            }

            public Spoof()
                : base(NewUID, Point3D.Zero, Map.Internal)
            { }
        }

        public static string StripHtmlBreaks(string str, bool preserve)
        {
            return Regex.Replace(str, @"<br[^>]?>", preserve ? "\n" : " ", RegexOptions.IgnoreCase);
        }
        #endregion
    }
}
