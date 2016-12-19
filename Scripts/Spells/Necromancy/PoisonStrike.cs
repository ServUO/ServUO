using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Necromancy
{
    public class PoisonStrikeSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Poison Strike", "In Vas Nox",
            203,
            9031,
            Reagent.NoxCrystal);
        public PoisonStrikeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds((Core.ML ? 1.75 : 1.5));
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 50.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 17;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return false;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IDamageable m)
        {
            if (this.CheckHSequence(m))
            {
                Mobile mob = m as Mobile;
                SpellHelper.Turn(this.Caster, m);

                /* Creates a blast of poisonous energy centered on the target.
                * The main target is inflicted with a large amount of Poison damage, and all valid targets in a radius of 2 tiles around the main target are inflicted with a lesser effect.
                * One tile from main target receives 50% damage, two tiles from target receives 33% damage.
                */

                Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0);
                Effects.PlaySound(m.Location, m.Map, 0x229);

                double damage = Utility.RandomMinMax((Core.ML ? 32 : 36), 40) * ((300 + (this.GetDamageSkill(this.Caster) * 9)) / 1000);
				
                double sdiBonus = (double)AosAttributes.GetValue(this.Caster, AosAttribute.SpellDamage) / 100;
				
                double pvmDamage = damage * (1 + sdiBonus);
				
                if (Core.ML && sdiBonus > 0.15)
                    sdiBonus = 0.15;
                double pvpDamage = damage * (1 + sdiBonus);

                Map map = m.Map;

                if (map != null)
                {
                    List<IDamageable> targets = new List<IDamageable>();
			
                    if (this.Caster.CanBeHarmful(m, false))
                        targets.Add(m);

                    IPooledEnumerable eable = m.Map.GetObjectsInRange(m.Location, 2);

                    foreach (object o in eable)
                    {
                        IDamageable id = o as IDamageable;

                        if (!(this.Caster is BaseCreature && id is BaseCreature))
                        {
                            if ((id is Mobile && (Mobile)id == Caster) || id == m)
                                continue;

                            if ((!(id is Mobile) || SpellHelper.ValidIndirectTarget(this.Caster, (Mobile)id)) && this.Caster.CanBeHarmful(id, false))
                                targets.Add(id);
                        }
                    }

                    eable.Free();

                    for (int i = 0; i < targets.Count; ++i)
                    {
                        IDamageable id = targets[i];

                        int num;

                        if (Utility.InRange(id.Location, m.Location, 0))
                            num = 1;
                        else if (Utility.InRange(id.Location, m.Location, 1))
                            num = 2;
                        else
                            num = 3;

                        this.Caster.DoHarmful(id);
                        SpellHelper.Damage(this, id, ((id is PlayerMobile && this.Caster.Player) ? pvpDamage : pvmDamage) / num, 0, 0, 0, 100, 0);
                    }

                    targets.Clear();
                    targets.TrimExcess();
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly PoisonStrikeSpell m_Owner;
            public InternalTarget(PoisonStrikeSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                    this.m_Owner.Target((IDamageable)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}