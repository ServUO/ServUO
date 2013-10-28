using System;
using System.Collections.Generic;
using Server.Targeting;

namespace Server.Spells.Spellweaving
{
    public class WildfireSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Wildfire", "Haelyn",
            -1,
            false);
        public WildfireSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(2.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 66.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 50;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Point3D p)
        {
            if (!this.Caster.CanSee(p))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckSequence())
            {
                int level = GetFocusLevel(this.Caster);
                double skill = this.Caster.Skills[this.CastSkill].Value;

                int tiles = 2 + level;
                int damage = 15 + level;
                int duration = (int)Math.Max(1, skill / 24) + level; 
				
                for (int x = p.X - tiles; x <= p.X + tiles; x += tiles)
                {
                    for (int y = p.Y - tiles; y <= p.Y + tiles; y += tiles)
                    { 
                        if (p.X == x && p.Y == y)
                            continue;
							
                        Point3D p3d = new Point3D(x, y, this.Caster.Map.GetAverageZ(x, y));
					
                        if (this.Caster.Map.CanFit(p3d, 12, true, false))
                            new FireItem(duration).MoveToWorld(p3d, this.Caster.Map);
                    }
                }
				
                Effects.PlaySound(p, this.Caster.Map, 0x5CF);
				
                new InternalTimer(this.Caster, p, damage, tiles, duration).Start();
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly WildfireSpell m_Owner;
            public InternalTarget(WildfireSpell owner)
                : base(12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile m, object o)
            {
                if (o is IPoint3D)
                {
                    this.m_Owner.Target(new Point3D((IPoint3D)o));
                }
            }

            protected override void OnTargetFinish(Mobile m)
            {
                this.m_Owner.FinishSequence();
            }
        }

        public class InternalTimer : Timer
        { 
            private readonly Mobile m_Owner;
            private readonly Point3D m_Location;
            private readonly int m_Damage;
            private readonly int m_Range;
            private int m_LifeSpan;
            public InternalTimer(Mobile owner, Point3D location, int damage, int range, int duration)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), duration)
            {
                this.m_Owner = owner;
                this.m_Location = location;
                this.m_Damage = damage;
                this.m_Range = range;
                this.m_LifeSpan = duration;
            }

            protected override void OnTick()
            { 
                if (this.m_Owner == null)
                    return;
					
                this.m_LifeSpan -= 1;
					
                foreach (Mobile m in this.GetTargets())
                {
                    this.m_Owner.DoHarmful(m);
					
                    // magic ressit?
					
                    if (this.m_Owner.Map.CanFit(m.Location, 12, true, false))
                        new FireItem(this.m_LifeSpan).MoveToWorld(m.Location, m.Map);
						
                    Effects.PlaySound(m.Location, m.Map, 0x5CF);
					
                    AOS.Damage(m, this.m_Owner, this.m_Damage, 0, 100, 0, 0, 0);	
                }
            }

            private List<Mobile> GetTargets()
            {
                List<Mobile> m_Targets = new List<Mobile>();
			
                foreach (Mobile m in this.m_Owner.Map.GetMobilesInRange(this.m_Location, this.m_Range))
                {
                    if (m != this.m_Owner && SpellHelper.ValidIndirectTarget(this.m_Owner, m) && this.m_Owner.CanBeHarmful(m, false))
                        m_Targets.Add(m);
                }
				
                return m_Targets;					
            }
        }

        public class FireItem : Item
        { 
            public FireItem(int duration)
                : base(Utility.RandomBool() ? 0x398C : 0x3996)
            {
                this.Movable = false;
                Timer.DelayCall(TimeSpan.FromSeconds(duration), new TimerCallback(Delete));
            }

            public FireItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();
            }
        }
    }
}