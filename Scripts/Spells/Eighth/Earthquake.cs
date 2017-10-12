using System;
using System.Collections.Generic;

namespace Server.Spells.Eighth
{
    public class EarthquakeSpell : MagerySpell
    {
        public override DamageType SpellDamageType { get { return DamageType.SpellAOE; } }

        private static readonly SpellInfo m_Info = new SpellInfo(
            "Earthquake", "In Vas Por",
            233,
            9012,
            false,
            Reagent.Bloodmoss,
            Reagent.Ginseng,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);

        public EarthquakeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Eighth;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override void OnCast()
        {
            if (SpellHelper.CheckTown(Caster, Caster) && CheckSequence())
            {
                List<IDamageable> targets = new List<IDamageable>();

                Map map = Caster.Map;

                if (map != null)
                {
                    IPooledEnumerable eable = Caster.GetObjectsInRange(1 + (int)(Caster.Skills[SkillName.Magery].Value / 15.0));

                    foreach (object o in eable)
                    {
                        IDamageable id = o as IDamageable;

                        if (id == null || id is Mobile && (Mobile)id == Caster)
                            continue;

                        if ((!(id is Mobile) || SpellHelper.ValidIndirectTarget(Caster, id as Mobile)) && Caster.CanBeHarmful(id, false))
                        {
                            if (Core.AOS && !Caster.InLOS(id))
                                continue;

                            targets.Add(id);
                        }
                    }

                    eable.Free();
                }

                Caster.PlaySound(0x220);

                for (int i = 0; i < targets.Count; ++i)
                {
                    IDamageable id = targets[i];
                    Mobile m = id as Mobile;

                    int damage;

                    if (Core.AOS)
                    {
                        damage = id.Hits / 2;

                        if (m == null || !m.Player)
                            damage = Math.Max(Math.Min(damage, 100), 15);
                        damage += Utility.RandomMinMax(0, 15);
                    }
                    else
                    {
                        damage = (id.Hits * 6) / 10;

                        if ((m == null || !m.Player) && damage < 10)
                            damage = 10;
                        else if (damage > 75)
                            damage = 75;
                    }

                    Caster.DoHarmful(id);
                    SpellHelper.Damage(this, id, damage, 100, 0, 0, 0, 0);
                }

                targets.Clear();
                targets.TrimExcess();
            }

            FinishSequence();
        }
    }
}