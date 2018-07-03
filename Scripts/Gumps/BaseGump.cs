using System;
using Server;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
    public abstract class BaseGump : Gump, IDisposable
    {
        public static int CenterLoc = 1154645;     // <center>~1_val~</center>
        public static int AlignRightLoc = 1114514; // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>

        private Gump _Parent;

        public PlayerMobile User { get; private set; }
        public bool Open { get; set; }

        public virtual bool CloseOnMapChange { get { return false; } }

        public Gump Parent 
        {
            get { return _Parent; }
            set
            {
                _Parent = value;

                if (_Parent != null)
                {
                    if(_Parent is BaseGump && !((BaseGump)_Parent).Children.Contains(this))
                        ((BaseGump)_Parent).Children.Add(this);
                }
                else if (_Parent is BaseGump && ((BaseGump)_Parent).Children.Contains(this))
                {
                    ((BaseGump)_Parent).Children.Remove(this);
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
            if(gump == null)
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
                if(Parent is BaseGump)
                    ((BaseGump)Parent).OnChildClosed(this);

                Parent = null;
            }
        }

        public virtual void OnChildClosed(BaseGump gump)
        {
        }

        public override sealed void OnResponse(NetState state, RelayInfo info)
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
            User.CloseGump(this.GetType());
        }
        
        public static T GetGump<T>(PlayerMobile pm, Func<T, bool> predicate) where T : Gump
        {
            return EnumerateGumps<T>(pm).FirstOrDefault(x => predicate == null || predicate(x));
        }

        public static IEnumerable<T> EnumerateGumps<T>(PlayerMobile pm, Func<T, bool> predicate = null) where T : Gump
        {
            var ns = pm.NetState;

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
            var ns = pm.NetState;
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
            var ns = pm.NetState;
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
            var ns = pm.NetState;

            if (ns != null)
            {
                var gumps = GetGumps(pm, checkOpen);

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

            base.AddItemProperty(mob.Serial.Value);
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
            return String.Format("<basefont color={0}>{1}", color, str);
        }

        protected string ColorAndCenter(string color, string str)
        {
            return String.Format("<center><basefont color={0}>{1}</center>", color, str);
        }

        protected string ColorAndSize(string color, int size, string str)
        {
            return String.Format("<basefont color={0} size={1}>{2}", color, size.ToString(), str);
        }

        protected string ColorAndCenterAndSize(string color, int size, string str)
        {
            return String.Format("<basefont color={0} size={1}><center>{2}</center>", color, size.ToString(), str);
        }

        protected string Color(int color, string str)
        {
            return String.Format("<basefont color=#{0:X6}>{1}", color, str);
        }

        protected string ColorAndCenter(int color, string str)
        {
            return String.Format("<basefont color=#{0:X6}><center>{1}</center>", color, str);
        }

        protected string Center(string str)
        {
            return String.Format("<CENTER>{0}</CENTER>", str);
        }

        public void AddHtmlLocalizedCentered(int x, int y, int length, int height, int localization, bool background, bool scrollbar)
        {
            AddHtmlLocalized(x, y, length, height, 1113302, String.Format("#{0}", localization), 0, background, scrollbar);
        }

        public void AddHtmlLocalizedCentered(int x, int y, int length, int height, int localization, int hue, bool background, bool scrollbar)
        {
            AddHtmlLocalized(x, y, length, height, 1113302, String.Format("#{0}", localization), hue, background, scrollbar);
        }

        public void AddHtmlLocalizedAlignRight(int x, int y, int length, int height, int localization, bool background, bool scrollbar)
        {
            AddHtmlLocalized(x, y, length, height, 1114514, String.Format("#{0}", localization), 0, background, scrollbar);
        }

        public void AddHtmlLocalizedAlignRight(int x, int y, int length, int height, int localization, int hue, bool background, bool scrollbar)
        {
            AddHtmlLocalized(x, y, length, height, 1114514, String.Format("#{0}", localization), hue, background, scrollbar);
        }
        #endregion
    }
}