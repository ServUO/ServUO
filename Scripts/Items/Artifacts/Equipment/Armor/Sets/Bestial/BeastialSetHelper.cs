using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class BestialSetHelper
    {
        public static readonly int BeserkHue = 1255;

        public static Dictionary<Mobile, BeserkTimer> _Table;

        public static void OnDamage(Mobile victim, Mobile attacker, ref int damage)
        {
            int equipped = TotalPieces(victim);

            if (/*victim != attacker && */equipped > 0 && victim.Hits - damage < (victim.HitsMax / 2))
            {
                if (_Table == null || !_Table.ContainsKey(victim))
                {
                    AddBeserk(victim);
                    return;
                }
                else if (!_Table[victim].Running)
                {
                    return;
                }

                int absorb = equipped * _Table[victim].Level + 2;

                damage = Math.Max(1, damage - absorb);

                _Table[victim].DamageTaken += damage;

                victim.SendLocalizedMessage(1151539, absorb.ToString()); // In your rage, you shrug off ~1_VALUE~ points of damage.
            }
        }

        public static int GetTotalBeserk(Item item)
        {
            if (item == null)
                return 0;

            Mobile m = item.RootParent as Mobile;

            if (m != null && _Table != null && _Table.ContainsKey(m))
                return _Table[m].Level;

            return 1;
        }

        public static void OnAdded(Mobile m, Item item)
        {
            if (_Table != null && _Table.ContainsKey(m) && _Table[m].Running && item is ISetItem && ((ISetItem)item).SetID == SetItem.Bestial)
            {
                item.Hue = BestialSetHelper.BeserkHue + _Table[m].Level;
            }
        }

        public static void OnRemoved(Mobile m, Item item)
        {
            if (TotalPieces(m) == 0)
            {
                if (_Table != null && _Table.ContainsKey(m))
                    _Table[m].EndBeserk();
            }

            if (item is ISetItem && ((ISetItem)item).SetID == SetItem.Bestial)
                item.Hue = 2010;
        }

        public static void DoHue(Mobile m, int hue)
        {
            foreach (Item i in m.Items.Where(item => item is ISetItem && ((ISetItem)item).SetID == SetItem.Bestial && item.Hue != hue))
            {
                i.Hue = hue;
            }

            m.HueMod = hue;
        }

        public static int TotalPieces(Mobile m)
        {
            return m.Items.Where(i => i is ISetItem && ((ISetItem)i).SetID == SetItem.Bestial).Count();
        }

        public static void AddBeserk(Mobile m)
        {
            if (_Table == null)
                _Table = new Dictionary<Mobile, BeserkTimer>();

            _Table[m] = new BeserkTimer(m);
        }

        public static void RemoveBeserk(Mobile m)
        {
            if (_Table != null && _Table.ContainsKey(m))
            {
                _Table.Remove(m);

                if (_Table.Count == 0)
                    _Table = null;
            }
        }

        public static bool IsBeserk(Mobile m)
        {
            return _Table != null && _Table.ContainsKey(m);
        }

        public class BeserkTimer : Timer
        {
            private int _DamageTaken;

            public Mobile Mobile { get; set; }
            public int DamageTaken
            {
                get { return _DamageTaken; }
                set
                {
                    int level = Level;
                    int old = _DamageTaken;

                    _DamageTaken = value;

                    if (old < _DamageTaken)
                        LastDamage = DateTime.UtcNow;

                    if (level < Level)
                    {
                        int hue = BestialSetHelper.BeserkHue + Level;

                        BestialSetHelper.DoHue(this.Mobile, hue);

                        if(level < 5)
                            Mobile.SendLocalizedMessage(1151533, "", hue); //Your rage grows!
                    }
                    else if (level > Level && level > 0)
                    {
                        int hue = BestialSetHelper.BeserkHue + Level;

                        BestialSetHelper.DoHue(this.Mobile, hue);

                        if (level > 1)
                            Mobile.SendLocalizedMessage(1151534, "", hue); //Your rage recedes.
                    }
                }
            }

            public int StartHue { get; set; }
            public DateTime LastDamage { get; set; }

            public int Level { get { return Math.Min(5, Math.Max(1, _DamageTaken / 50)); } }

            public BeserkTimer(Mobile m) : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                Mobile = m;
                StartHue = m.HueMod;

                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                    {
                        LastDamage = DateTime.UtcNow;
                        Start();
                    });
            }

            protected override void OnTick()
            {
                if (LastDamage + TimeSpan.FromSeconds(10) < DateTime.UtcNow || !Mobile.Alive)
                {
                    EndBeserk();
                }
                else if (LastDamage + TimeSpan.FromSeconds(3) < DateTime.UtcNow && Level > 1)
                {
                    DamageTaken -= 50;
                }
                else if (Mobile.HueMod == StartHue || Mobile.HueMod == -1)
                {
                    BestialSetHelper.DoHue(this.Mobile, BestialSetHelper.BeserkHue);

                    Mobile.SendLocalizedMessage(1151532); //You enter a berserk rage!
                }
            }

            public void EndBeserk()
            {
                BestialSetHelper.RemoveBeserk(this.Mobile);

                Mobile.HueMod = StartHue;
                Mobile.SendLocalizedMessage(1151535); //Your berserk rage has subsided. 

                foreach (Item item in Mobile.Items.Where(i => i is ISetItem && ((ISetItem)i).SetID == SetItem.Bestial))
                {
                    item.Hue = 2010;
                }

                Stop();
            }
        }
    }
}