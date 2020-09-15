using Server.ContextMenus;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class UndertakersStaff : GnarledStaff
    {
        private static readonly Dictionary<Mobile, CorpseRetrieveTimer> _Timers = new Dictionary<Mobile, CorpseRetrieveTimer>();

        private int _Charges;
        private bool _SummonAll;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return _Charges; } set { _Charges = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SummonAll { get { return _SummonAll; } set { _SummonAll = value; InvalidateProperties(); } }

        public override int LabelNumber => 1071498;  // Undertaker's Staff
        public override bool IsArtifact => true;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public UndertakersStaff()
        {
            if (!Siege.SiegeShard)
                LootType = LootType.Blessed;

            Charges = 100;
            SummonAll = true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1071518, string.Format("#{0}", _SummonAll ? "1071508" : "1071507"));
            list.Add(1060584, _Charges.ToString());
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> entries)
        {
            base.GetContextMenuEntries(from, entries);

            if (IsChildOf(from.Backpack))
            {
                SimpleContextMenuEntry entry1 = new SimpleContextMenuEntry(from, 1071507, m =>
                    {
                        SummonAll = false;
                        InvalidateProperties();
                    }); // Summon Most Recent Corpse Only

                SimpleContextMenuEntry entry2 = new SimpleContextMenuEntry(from, 1071508, m =>
                    {
                        _SummonAll = true;
                        InvalidateProperties();
                    }); // Summon All Corpses

                if (_SummonAll)
                    entry2.Flags |= CMEFlags.Highlighted;
                else
                    entry1.Flags |= CMEFlags.Highlighted;

                entry1.Enabled = !IsSummoning();
                entry2.Enabled = !IsSummoning();

                entries.Add(entry1);
                entries.Add(entry2);
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (!_Timers.ContainsKey(m) && (IsChildOf(m.Backpack) || m.FindItemOnLayer(Layer) == this))
            {
                TryGetCorpse(m);
            }
        }

        private void TryGetCorpse(Mobile m)
        {
            if (CanGetCorpse(m))
            {
                m.PlaySound(0xF5);

                if (_SummonAll)
                {
                    List<Corpse> corpses = GetCorpses(m);

                    if (corpses != null)
                    {
                        m.SendLocalizedMessage(1071527, corpses.Count.ToString()); // The staff reaches out to ~1_COUNT~ of your corpses and tries to draw them to you...

                        _Timers[m] = new CorpseRetrieveTimer(m, corpses, this);
                    }
                    else
                    {
                        m.SendLocalizedMessage(1071511); // The staff glows slightly, then fades. Its magic is unable to locate a corpse of yours to recover.
                    }
                }
                else
                {
                    Corpse corpse = GetCorpse(m);

                    if (corpse != null)
                    {
                        m.SendLocalizedMessage(1071528); // The staff reaches out to your corpse and tries to draw it to you...

                        _Timers[m] = new CorpseRetrieveTimer(m, new List<Corpse> { corpse }, this);
                    }
                    else
                    {
                        m.SendLocalizedMessage(1071511); // The staff glows slightly, then fades. Its magic is unable to locate a corpse of yours to recover.
                    }
                }
            }
        }

        private bool CanGetCorpse(Mobile m, bool firstCheck = true)
        {
            if (m.Criminal)
            {
                m.SendLocalizedMessage(1071510); // You are a criminal and cannot use this item...
                return false;
            }
            else if (Spells.SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1071514); // You cannot use this item during the heat of battle.
                return false;
            }

            return true;
        }

        public void TryEndSummon(Mobile m, List<Corpse> corpses)
        {
            if (_Timers.ContainsKey(m))
                _Timers.Remove(m);

            if (corpses == null || corpses.Count == 0)
            {
                m.SendLocalizedMessage(1071511); // The staff glows slightly, then fades. Its magic is unable to locate a corpse of yours to recover.
                return;
            }

            bool tooFar = false;
            bool notEnoughTime = false;
            bool tooManySummons = false;
            bool success = true;

            if (_SummonAll)
            {
                List<Corpse> copy = new List<Corpse>(corpses);

                foreach (Corpse c in copy)
                {
                    bool remove = false;

                    if (c.Map != m.Map)
                    {
                        remove = true;
                        tooFar = true;
                    }

                    if (c.Killer is PlayerMobile && c.Killer != m && c.TimeOfDeath + TimeSpan.FromSeconds(180) > DateTime.UtcNow)
                    {
                        remove = true;
                        notEnoughTime = true;
                    }

                    if (Corpse.PlayerCorpses != null && Corpse.PlayerCorpses.ContainsKey(c) && Corpse.PlayerCorpses[c] >= 3)
                    {
                        remove = true;
                        tooManySummons = true;
                    }

                    if (remove)
                        corpses.Remove(c);
                }

                if (corpses.Count == 0)
                    success = false;
            }
            else
            {
                Corpse c = corpses[0];

                if (c.Map != m.Map)
                    tooFar = true;

                if (c.Killer is PlayerMobile && c.Killer != m && c.TimeOfDeath + TimeSpan.FromSeconds(180) > DateTime.UtcNow)
                    notEnoughTime = true;

                if (Corpse.PlayerCorpses != null && Corpse.PlayerCorpses.ContainsKey(c) && Corpse.PlayerCorpses[c] >= 3)
                    tooManySummons = true;

                if (tooFar || notEnoughTime || tooManySummons)
                {
                    if (tooFar)
                        m.SendLocalizedMessage(1071512); // ...but the corpse is too far away!
                    else if (notEnoughTime)
                        m.SendLocalizedMessage(1071515); // ...but not enough time has passed since you were slain in battle!
                    else
                        m.SendLocalizedMessage(1071517); // ...but the corpse has already been summoned too many times!

                    success = false;
                }
            }

            if (success)
            {
                m.PlaySound(0xFA);

                foreach (Corpse c in corpses)
                {
                    c.MoveToWorld(m.Location, m.Map);

                    if (Corpse.PlayerCorpses != null && Corpse.PlayerCorpses.ContainsKey(c))
                        Corpse.PlayerCorpses[c]++;
                }

                if (_SummonAll)
                {
                    m.SendLocalizedMessage(1071530, corpses.Count.ToString()); // ...and succeeds in summoning ~1_COUNT~ of them!

                    if (tooFar)
                        m.SendLocalizedMessage(1071513); // ...but one of them is too far away!
                    else if (notEnoughTime)
                        m.SendLocalizedMessage(1071516); // ...but one of them deflects the magic because of the stain of war!
                    else if (tooManySummons)
                        m.SendLocalizedMessage(1071519); // ...but one of them has already been summoned too many times!
                }
                else
                    m.SendLocalizedMessage(1071529); // ...and succeeds in the summoning of it!

                if (Charges <= 0)
                {
                    m.SendLocalizedMessage(1071509); // The staff has been reduced to pieces!
                    Delete();
                }
                else
                {
                    Charges--;
                }
            }
        }

        private int GetCorpseCount(Mobile m)
        {
            if (Corpse.PlayerCorpses == null)
                return 0;

            int count = 0;

            foreach (KeyValuePair<Corpse, int> kvp in Corpse.PlayerCorpses)
            {
                if (kvp.Key.Owner == m && kvp.Value < 3)
                    count++;
            }

            return count;
        }

        private List<Corpse> GetCorpses(Mobile m)
        {
            if (Corpse.PlayerCorpses == null)
                return null;

            List<Corpse> list = null;

            foreach (KeyValuePair<Corpse, int> kvp in Corpse.PlayerCorpses)
            {
                if (kvp.Key.Owner == m && kvp.Value < 3)
                {
                    if (list == null)
                        list = new List<Corpse>();

                    if (!list.Contains(kvp.Key))
                        list.Add(kvp.Key);
                }

                if (list != null && list.Count >= 15)
                    break;
            }

            return list;
        }

        private Corpse GetCorpse(Mobile m)
        {
            Corpse corpse = m.Corpse as Corpse;

            if (corpse == null || Corpse.PlayerCorpses == null || !Corpse.PlayerCorpses.ContainsKey(corpse))
                return null;

            return corpse;
        }

        public static bool TryRemoveTimer(Mobile m)
        {
            if (_Timers.ContainsKey(m))
            {
                _Timers[m].Stop();
                _Timers.Remove(m);

                m.FixedEffect(0x3735, 6, 30);
                m.PlaySound(0x5C);

                m.SendLocalizedMessage(1071525); // You have been disrupted while attempting to pull your corpse!
                return true;
            }

            return false;
        }

        public bool IsSummoning()
        {
            foreach (CorpseRetrieveTimer timer in _Timers.Values)
            {
                if (timer.Staff == this)
                    return true;
            }

            return false;
        }

        public class CorpseRetrieveTimer : Timer
        {
            public Mobile From { get; set; }
            public List<Corpse> Corpses { get; set; }
            public UndertakersStaff Staff { get; set; }

            public CorpseRetrieveTimer(Mobile from, List<Corpse> corpses, UndertakersStaff staff)
                : base(TimeSpan.FromSeconds(3))
            {
                From = from;
                Corpses = corpses;
                Staff = staff;

                Start();
            }

            protected override void OnTick()
            {
                Staff.TryEndSummon(From, Corpses);
            }
        }

        public UndertakersStaff(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(_Charges);
            writer.Write(_SummonAll);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Charges = reader.ReadInt();
            _SummonAll = reader.ReadBool();
        }
    }
}
