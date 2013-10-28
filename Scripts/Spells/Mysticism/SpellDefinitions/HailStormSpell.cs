using System;
using System.Collections.Generic;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public class HailStormSpell : MysticSpell
    {
        public static List<Point3D> HailStormArea = new List<Point3D>();
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Hail Storm", "Kal Des Ylem",
            230,
            9022,
            Reagent.BlackPearl,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.DragonBlood);
        public HailStormSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // A torrent of rain brings down a storm of hailstones upon the caster's enemies in the surrounding area. 
        public override int RequiredMana
        {
            get
            {
                return 40;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 70;
            }
        }
        public static bool IsHailStorming(Point3D point)
        {
            bool hailing = false;

            for (int i = 0; i < HailStormArea.Count; i++)
            {
                if (HailStormArea[i].X >= (point.X - 5) && HailStormArea[i].X <= (point.X + 5))
                    if (HailStormArea[i].Y >= (point.Y - 5) && HailStormArea[i].Y <= (point.Y + 5))
                    {
                        hailing = true;
                        break;
                    }
            }

            return hailing;
        }

        public static void RemoveHailPoint(Point3D point)
        {
            for (int i = 0; i < HailStormArea.Count; i++)
                if (point.X == HailStormArea[i].X && point.Y == HailStormArea[i].Y)
                    HailStormArea.Remove(HailStormArea[i]);
        }

        public override void OnCast()
        {
            this.Caster.Target = new MysticSpellTarget(this, true, TargetFlags.None);
        }

        public override void OnTarget(Object o)
        {
            IPoint3D p = o as IPoint3D;

            if (p == null)
                return;
            else if (this.CheckSequence())
            {
                Point3D point = new Point3D(p);

                // Can you stack multiple hail sotrms on OSI?
                if (!IsHailStorming(point))
                {
                    HailStormArea.Add(point);
                    new HailStormTimer(this, this.Caster, point).Start();
                    this.Caster.PlaySound(0x649);
                }
                else
                    this.Caster.SendMessage("It is already hailing there.");
            }

            this.FinishSequence();
        }

        public class HailStormTimer : Timer
        {
            public Mobile Caster;
            private readonly MysticSpell m_Spell;
            private readonly Point3D m_StormPoint;
            private readonly Map m_StormMap;
            private readonly int m_Damage;
            private readonly int m_MaxCount;
            private Point3D m_LastTarget = new Point3D();
            private int m_Count;
            public HailStormTimer(MysticSpell spell, Mobile caster, Point3D point)
                : base(TimeSpan.FromMilliseconds(100.0), TimeSpan.FromMilliseconds(100.0))
            {
                this.m_Spell = spell;
                this.Caster = caster;
                this.m_StormPoint = point;
                this.m_StormMap = caster.Map;
                this.m_Count = 0;
                this.m_MaxCount = 75;
                this.m_Damage = (int)((caster.Skills[SkillName.Mysticism].Value + (caster.Skills[SkillName.Imbuing].Value / 2)) / 4);
            }

            protected override void OnTick()
            {
                this.m_Count++;

                //if ( Caster != null )
                //Ability.Aura( m_LastTarget, m_StormMap, Caster, m_Damage, m_Damage, ResistanceType.Cold, 0, null, "" );

                if (this.m_Count > this.m_MaxCount || this.Caster == null)
                {
                    HailStormSpell.RemoveHailPoint(this.m_StormPoint);
                    this.Stop();
                    return;
                }

                if ((this.m_Count % 10) == 0)
                //Ability.Aura( m_StormPoint, m_StormMap, Caster, 15, 15, ResistanceType.Cold, 5, null, "" );

                    this.m_LastTarget.X = this.m_StormPoint.X + Utility.RandomMinMax(-5, 5);
                this.m_LastTarget.Y = this.m_StormPoint.Y + Utility.RandomMinMax(-5, 5);

                Effects.SendMovingParticles(
                    new Entity(Serial.Zero, new Point3D(this.m_LastTarget.X - 10, this.m_LastTarget.Y - 10, this.m_LastTarget.Z + 30), this.m_StormMap),
                    new Entity(Serial.Zero, this.m_LastTarget, this.m_StormMap),
                    0x36D4 /*0x1EA7*/, 15, 0, false, false, 1365, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
            }
        }
    }
}
/*



*/