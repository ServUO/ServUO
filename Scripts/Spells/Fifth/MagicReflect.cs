using System;
using System.Collections.Generic;
using System.Linq;

using Server.Spells.Mysticism;
using Server.Spells.Spellweaving;
using Server.Spells.Necromancy;
using Server.Spells.SkillMasteries;

namespace Server.Spells.Fifth
{
    public class MagicReflectSpell : MagerySpell
    {
        public override SpellCircle Circle => SpellCircle.Fifth;

        private static readonly SpellInfo m_Info = new SpellInfo(
            "Magic Reflection", "In Jux Sanct",
            242,
            9012,
            Reagent.Garlic,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk);

        private static readonly List<MagicReflectContext> m_Table = new List<MagicReflectContext>();

        public MagicReflectSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            /* The magic reflection spell decreases the caster's physical resistance, while increasing the caster's elemental resistances.
            * Physical decrease = 25 - (Inscription/20).
            * Elemental resistance = +10 (-20 physical, +10 elemental at GM Inscription)
            * The magic reflection spell has an indefinite duration, becoming active when cast, and deactivated when re-cast.
            * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out, even after dying—until you “turn them off” by casting them again. 
            */
            if (CheckSequence())
            {
                Mobile targ = Caster;

                var context = GetContext(targ);

                targ.PlaySound(0x1E9);
                targ.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);

                if (context == null)
                {
                    int physiMod = 20 - (int)(targ.Skills[SkillName.Inscribe].Value / 20);
                    int otherMod = 10;

                    var mods = new ResistanceMod[5]
                    {
                        new ResistanceMod(ResistanceType.Physical,  -physiMod),
                        new ResistanceMod(ResistanceType.Fire,      otherMod),
                        new ResistanceMod(ResistanceType.Cold,      otherMod),
                        new ResistanceMod(ResistanceType.Poison,    otherMod),
                        new ResistanceMod(ResistanceType.Energy,    otherMod)
                    };

                    context = new MagicReflectContext(Caster, mods);
                    m_Table.Add(context);

                    for (int i = 0; i < mods.Length; ++i)
                        targ.AddResistanceMod(mods[i]);

                    string buffFormat = string.Format("-{0}\t+{1}\t+{1}\t+{1}\t+{1}\t{2}\t{3}", physiMod, otherMod, "-5", context.ReflectPool);
                    BuffInfo.AddBuff(targ, new BuffInfo(BuffIcon.MagicReflection, 1015197, 1149979, buffFormat, true));
                }
                else if (CooldownTimer.InCooldown(Caster))
                {
                    Caster.SendLocalizedMessage(1150073, CooldownTimer.GetRemaining(Caster)); // You must wait ~1_seconds~ seconds to tap into your magic reflection pool. 
                }
                else if (context.NextReplenish > DateTime.UtcNow || !context.TryReplenish())
                {
                    Expire(context);
                }
            }

            FinishSequence();
        }

        public static void Expire(MagicReflectContext context)
        {
            var caster = context.Caster;

            caster.PlaySound(0x1ED);
            caster.FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);

            if (context.NextReplenish > DateTime.UtcNow)
            {
                CooldownTimer.AddTimer(context.Caster, context.NextReplenish - DateTime.UtcNow);
            }

            EndReflect(context);
        }

        public static void EndReflect(Mobile m)
        {
            var context = GetContext(m);

            if (context != null)
            {
                EndReflect(context);
            }
        }

        public static void EndReflect(MagicReflectContext context)
        {
            m_Table.Remove(context);
            BuffInfo.RemoveBuff(context.Caster, BuffIcon.MagicReflection);

            var mods = context.Mods;

            if (mods != null)
            {
                for (int i = 0; i < mods.Length; ++i)
                {
                    context.Caster.RemoveResistanceMod(mods[i]);
                }
            }

            context.Caster.Delta(MobileDelta.WeaponDamage);
        }

        public static MagicReflectContext GetContext(Mobile m)
        {
            return m_Table.FirstOrDefault(c => c.Caster == m);
        }

        public static bool HasReflect(Mobile m)
        {
            return m_Table.Any(c => c.Caster == m);
        }

        public static bool CheckReflectDamage(Mobile m, Spell spell)
        {
            var context = GetContext(m);

            if (context != null)
            {
                var reduce = 0;

                if (spell is MagerySpell)
                {
                    reduce = (int)(((MagerySpell)spell).Circle + 1) * 10;
                }
                else if (spell is MysticSpell)
                {
                    reduce = (int)(((MysticSpell)spell).Circle + 1) * 10;
                }
                else if (spell is NecromancerSpell)
                {
                    reduce = (int)((NecromancerSpell)spell).RequiredSkill;
                }
                else if (spell is ArcanistSpell)
                {
                    reduce = (int)((ArcanistSpell)spell).RequiredSkill;
                }
                else if (spell is SkillMasterySpell)
                {
                    reduce = (int)((SkillMasterySpell)spell).RequiredSkill;
                }

                if (reduce > context.ReflectPool)
                {
                    m.SendLocalizedMessage(1149981); // Your magic is not great enough to reflect the incoming spell.
                    context.ReflectPool = 0;
                    CooldownTimer.AddTimer(m);
                }
                else
                {
                    m.SendLocalizedMessage(1149980); // You reflect the incoming spell.
                    context.ReflectPool -= reduce;

                    if (context.ReflectPool == 0)
                    {
                        m.SendLocalizedMessage(1150066); // Your magic reflection pool has been depleted.
                        CooldownTimer.AddTimer(m);
                    }

                    string buffFormat = string.Format("{0}\t+{1}\t+{1}\t+{1}\t+{1}\t{2}\t{3}", context.Mods[0].Offset, context.Mods[1].Offset, "-5", context.ReflectPool);
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.MagicReflection, 1015197, 1149979, buffFormat, true));

                    return true;
                }
            }

            return false;
        }

        public class MagicReflectContext
        {
            private int _Pool;

            public bool Active { get; set; }
            public Mobile Caster { get; set; }
            public ResistanceMod[] Mods { get; set; }
            public int ReflectPool
            {
                get { return _Pool; }
                set
                {
                    var old = _Pool;
                    _Pool = value;

                    if (old < _Pool)
                    {
                        NextReplenish = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                    }
                }
            }

            public DateTime NextReplenish { get; set; }

            public MagicReflectContext(Mobile caster, ResistanceMod[] mods)
            {
                Caster = caster;
                Mods = mods;

                if (CooldownTimer.InCooldown(caster))
                {
                    caster.SendLocalizedMessage(1150073, CooldownTimer.GetRemaining(caster)); // You must wait ~1_seconds~ seconds to tap into your magic reflection pool. 
                }
                else
                {
                    ReflectPool = CalculateReflectPool();
                }
            }

            public bool TryReplenish()
            {
                var pool = CalculateReflectPool();

                if (pool > ReflectPool)
                {
                    ReflectPool = pool;
                    Caster.SendLocalizedMessage(1150072); //Your magic reflection pool has been replenished.

                    string buffFormat = string.Format("{0}\t+{1}\t+{1}\t+{1}\t+{1}\t{2}\t{3}", Mods[0].Offset, Mods[1].Offset, "-5", ReflectPool);
                    BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.MagicReflection, 1015197, 1149979, buffFormat, true));

                    return true;
                }

                return false;
            }

            private int CalculateReflectPool()
            {
                var scribe = Caster.Skills[SkillName.Inscribe].Value;

                return Math.Min(100, (int)(((Caster.Skills[SkillName.Magery].Value / 20) + (scribe >= 50.0 ? scribe / 20 : 0)) * (1 + Math.Floor(Caster.Skills[SkillName.MagicResist].Value * .075))));
            }
        }

        private class CooldownTimer : Timer
        {
            public Dictionary<Mobile, DateTime> ExpireTable { get; set; } = new Dictionary<Mobile, DateTime>();

            public static CooldownTimer Instance { get; set; }

            public static void AddTimer(Mobile caster)
            {
                AddTimer(caster, TimeSpan.FromSeconds(30));
            }

            public static void AddTimer(Mobile caster, TimeSpan ts)
            {
                if (Instance == null)
                {
                    Instance = new CooldownTimer();
                    Instance.Start();
                }

                Instance.ExpireTable[caster] = DateTime.UtcNow + ts;
            }

            public static void RemoveTimer(Mobile caster)
            {
                if (Instance != null && Instance.ExpireTable.ContainsKey(caster))
                {
                    Instance.ExpireTable.Remove(caster);

                    if (Instance.ExpireTable.Count == 0)
                    {
                        Instance.Stop();
                        Instance = null;
                    }
                }
            }

            public static bool InCooldown(Mobile m)
            {
                return Instance != null && Instance.ExpireTable.ContainsKey(m) && Instance.ExpireTable[m] > DateTime.UtcNow;
            }

            public static string GetRemaining(Mobile m)
            {
                if (InCooldown(m))
                {
                    return ((int)(Instance.ExpireTable[m] - DateTime.UtcNow).TotalSeconds).ToString();
                }

                return "less than 1";
            }

            public CooldownTimer()
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
            }

            protected override void OnTick()
            {
                List<Mobile> list = ExpireTable.Keys.Where(m => ExpireTable[m] < DateTime.UtcNow).ToList();

                for (int i = 0; i < list.Count; i++)
                {
                    RemoveTimer(list[i]);
                }

                ColUtility.Free(list);
            }
        }
    }
}
