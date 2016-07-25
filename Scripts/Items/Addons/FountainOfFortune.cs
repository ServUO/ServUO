using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Gumps;
using Server.Spells.Necromancy;
using Server.Spells.Fourth;
using Server.Spells.Chivalry;
using Server.Network;

namespace Server.Items
{
    public class FountainOfFortune : BaseAddon
    {
        public override int LabelNumber { get { return 1113618; } } // Font of Fortune   

        private Dictionary<Mobile, DateTime> m_RewardCooldown;
        public Dictionary<Mobile, DateTime> RewardCooldown { get { return m_RewardCooldown; } }
        private static List<FountainOfFortune> m_Fountains = new List<FountainOfFortune>();

        private static Timer m_Timer;

        [Constructable]
        public FountainOfFortune()
        {
            int itemID = 0x1731;

            AddComponent(new AddonComponent(itemID++), -2, +1, 0);
            AddComponent(new AddonComponent(itemID++), -1, +1, 0);
            AddComponent(new AddonComponent(itemID++), +0, +1, 0);
            AddComponent(new AddonComponent(itemID++), +1, +1, 0);

            AddComponent(new AddonComponent(itemID++), +1, +0, 0);
            AddComponent(new AddonComponent(itemID++), +1, -1, 0);
            AddComponent(new AddonComponent(itemID++), +1, -2, 0);

            AddComponent(new AddonComponent(itemID++), +0, -2, 0);
            AddComponent(new AddonComponent(itemID++), +0, -1, 0);
            AddComponent(new AddonComponent(itemID++), +0, +0, 0);

            AddComponent(new AddonComponent(itemID++), -1, +0, 0);
            AddComponent(new AddonComponent(itemID++), -2, +0, 0);

            AddComponent(new AddonComponent(itemID++), -2, -1, 0);
            AddComponent(new AddonComponent(itemID++), -1, -1, 0);

            AddComponent(new AddonComponent(itemID++), -1, -2, 0);
            AddComponent(new AddonComponent(++itemID), -2, -2, 0);

            m_RewardCooldown = new Dictionary<Mobile, DateTime>();

            Movable = false;

            AddFountain(this);
        }

        public override void Delete()
        {
            base.Delete();

            RemoveFountain(this);

            if (m_RewardCooldown != null)
                m_RewardCooldown.Clear();
        }

        public static void AddFountain(FountainOfFortune fountain)
        {
            if (!m_Fountains.Contains(fountain))
            {
                m_Fountains.Add(fountain);
                StartTimer();
            }
        }

        public static void RemoveFountain(FountainOfFortune fountain)
        {
            if (m_Fountains.Contains(fountain))
                m_Fountains.Remove(fountain);

            if (m_Fountains.Count == 0 && m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public static void StartTimer()
        {
            if (m_Timer != null && m_Timer.Running)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(DefragTables));
            m_Timer.Start();
        }

        public FountainOfFortune(Serial serial) : base(serial)
        {
        }

        public bool OnTarget(Mobile from, Item coin)
        {
            DefragTables();

            if (IsCoolingDown(from))
            {
                from.SendLocalizedMessage(1113368); // You already made a wish today. Try again tomorrow!
                return false;
            }

            if (.20 >= Utility.RandomDouble())
            {
                Item item = null;
                switch (Utility.Random(4))
                {
                    case 0: item = new SolesOfProvidence(); break;
                    case 1: item = new GemologistsSatchel(); break;
                    case 2: item = new RelicFragment(5); break;
                    case 3: item = new EnchantEssence(5); break;
                }

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
                    item.MoveToWorld(from.Location, from.Map);

                from.SendLocalizedMessage(1074360, String.Format("#{0}", item.LabelNumber.ToString())); // You receive a reward: ~1_REWARD~
            }
            else
            {
                FountainOfFortune.Bless(from);
            }

            from.PlaySound(0x22);

            m_RewardCooldown[from] = DateTime.UtcNow + TimeSpan.FromHours(24);

            if (coin.Amount <= 1)
                coin.Delete();
            else
                coin.Amount--;

            return false;
        }

        public static void DefragTables()
        {
            foreach (FountainOfFortune fountain in m_Fountains)
            {
                List<Mobile> list = new List<Mobile>(fountain.RewardCooldown.Keys);

                foreach (Mobile m in list)
                {
                    if (fountain.RewardCooldown.ContainsKey(m) && fountain.RewardCooldown[m] < DateTime.UtcNow)
                        fountain.RewardCooldown.Remove(m);
                }

                list.Clear();
            }
        }

        public bool IsCoolingDown(Mobile from)
        {
            foreach (FountainOfFortune fountain in m_Fountains)
            {
                if (fountain.RewardCooldown != null && fountain.RewardCooldown.ContainsKey(from))
                    return true;
            }

            return false;
        }

        #region Blessings
        private static readonly TimeSpan BlessingDuration = TimeSpan.FromHours(1.0);
        private static readonly int StatBoost = 10;

        public enum BlessingType
        {
            Strength,
            Dexterity,
            Intelligence,
            Luck,
            Protection
        }

        private static Dictionary<Mobile, BlessingType> m_Blessings = new Dictionary<Mobile, BlessingType>();

        public static bool HasAnyBlessing(Mobile m)
        {
            return m_Blessings.ContainsKey(m);
        }

        public static bool HasBlessing(Mobile m, BlessingType b)
        {
            if (m_Blessings.ContainsKey(m))
            {
                BlessingType blessing = m_Blessings[m];

                if (blessing == b)
                    return true;
            }

            return false;
        }

        private delegate bool CheckForBalm(Mobile m);

        private class BlessingDefinition
        {
            private BlessingType m_Type;
            private int m_Cliloc;

            public BlessingDefinition(BlessingType type, int cliloc)
            {
                m_Type = type;
                m_Cliloc = cliloc;
            }

            public virtual void Apply(Mobile m)
            {
                m_Blessings.Add(m, m_Type);

                m.FixedParticles(0x376A, 1, 32, 5005, EffectLayer.Waist);
                m.SendLocalizedMessage(m_Cliloc);

                Timer.DelayCall(BlessingDuration, new TimerStateCallback<Mobile>(Remove), m);
            }

            public virtual void Remove(Mobile m)
            {
                m.SendLocalizedMessage(1113370); // The Font of Fortune's blessing has faded.
                m_Blessings.Remove(m);
            }
        }

        private class StatBoostDefinition : BlessingDefinition
        {
            private StatType m_Stat;
            private CheckForBalm m_HasBalm;

            public StatBoostDefinition(BlessingType type, int cliloc, StatType stat, CheckForBalm hasBalm)
                : base(type, cliloc)
            {
                m_Stat = stat;
                m_HasBalm = hasBalm;
            }

            public override void Apply(Mobile m)
            {
                if (m_HasBalm(m))
                {
                    BalmOrLotion.IncreaseDuration(m);
                    m.SendLocalizedMessage(1113372); // The duration of your balm has been increased by an hour!
                }
                else
                {
                    base.Apply(m);

                    m.AddStatMod(new StatMod(m_Stat, "[FontOfFortune] Stat Offset", StatBoost, BlessingDuration));
                }
            }

            public override void Remove(Mobile m)
            {
                base.Remove(m);

                m.RemoveStatMod("[FontOfFortune] Stat Offset");
            }
        }

        private static BlessingDefinition[] m_BlessingDefs = new BlessingDefinition[]
            {
                new StatBoostDefinition( BlessingType.Strength,     1113373, StatType.Str, BalmOfStrength.UnderEffect ),
                new StatBoostDefinition( BlessingType.Dexterity,    1113374, StatType.Dex, BalmOfSwiftness.UnderEffect ),
                new StatBoostDefinition( BlessingType.Intelligence, 1113371, StatType.Int, BalmOfWisdom.UnderEffect ),

                new BlessingDefinition( BlessingType.Luck,          1079551 ),
                new BlessingDefinition( BlessingType.Protection,    1113375 )
            };

        public static void Bless(Mobile m)
        {
            BlessingDefinition def = m_BlessingDefs[Utility.Random(m_BlessingDefs.Length)];
            def.Apply(m);
        }
        #endregion        

        #region Healing & Resurrecting
        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.InRange(this.Location, 5) && !m_Table.ContainsKey(m))
            {
                if (!m.Alive)
                {
                    m.CloseGump(typeof(ResurrectGump));
                    m.SendGump(new FoFResurrectGump(m));
                }
                else if (m.Poisoned || IsCursed(m))
                {
                    RemoveCurseSpell.DoGraphicalEffect(m);
                    RemoveCurseSpell.DoRemoveCurses(m);

                    if (m.Poisoned && m.CurePoison(m))
                        m.SendLocalizedMessage(1010059); // You have been cured of all poisons.

                    AddCooldown(m);
                }
                else if (m.Hits < (m.HitsMax / 2.0))
                {
                    m.Hits = m.HitsMax;

                    m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                    m.PlaySound(0x202);

                    m.SendLocalizedMessage(1113365); // Your wounds have been mended.

                    AddCooldown(m);
                }
            }
        }

        public static void AddCooldown(Mobile m)
        {
            m_Table[m] = Timer.DelayCall(TimeSpan.FromMinutes(10.0), new TimerCallback(
                    delegate
                    {
                        m_Table.Remove(m);
                    }));
        }

        private bool IsCursed(Mobile m)
        {
            for (int i = 0; i < m_Effects.Length; i++)
            {
                UnderEffect effect = m_Effects[i];

                if (effect(m))
                    return true;
            }

            return false;
        }

        private delegate bool UnderEffect(Mobile m);

        private static UnderEffect[] m_Effects = new UnderEffect[]
            {
                CorpseSkinSpell.UnderEffect,
                StrangleSpell.UnderEffect,
                EvilOmenSpell.UnderEffect,
                BloodOathSpell.UnderEffect,
                CurseSpell.UnderEffect,
                MortalStrike.IsWounded
            };

        private static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_RewardCooldown = new Dictionary<Mobile, DateTime>();

            AddFountain(this);
        }
    }

    public class FoFResurrectGump : ResurrectGump
    {
        public FoFResurrectGump(Mobile owner)
            : base(owner, ResurrectMessage.Generic)
        {
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            from.CloseGump(typeof(ResurrectGump));

            if (info.ButtonID == 2)
            {
                from.PlaySound(0x214);
                from.Resurrect();

                from.Hits = (int)(from.HitsMax * 0.8);

                FountainOfFortune.AddCooldown(from);
            }
        }
    }
}