using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Misc;
using System.Collections.Generic;
using Server.Items;
using System.Linq;

namespace Server.Spells.SkillMasteries
{
    public class NetherBlastSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Nether Blast", "In Vas Por Grav",
                204,
				9061,
                Reagent.DragonBlood,
                Reagent.DaemonBone
            );

        public override double RequiredSkill { get { return 90; } }
        public override double UpKeep { get { return 0; } }
        public override int RequiredMana { get { return 40; } }
        public override bool PartyEffects { get { return false; } }

        public override SkillName CastSkill { get { return SkillName.Mysticism; } }
        public override SkillName DamageSkill
        {
            get
            {
                if (Caster.Skills[SkillName.Focus].Value > Caster.Skills[SkillName.Imbuing].Value)
                    return SkillName.Focus;

                return SkillName.Imbuing;
            }
        }

        public NetherBlastSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            Caster.FixedEffect(0x37C4, 87, (int)(GetCastDelay().TotalSeconds * 28), 0x66C, 3);
        }

        public override bool CheckCast()
        {
            if (HasSpell(Caster, this.GetType()))
            {
                Caster.SendMessage("You cannot use this ability until the last one has expired.");
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            Caster.Target = new MasteryTarget(this, 10, true, Server.Targeting.TargetFlags.None);
        }

        protected override void OnTarget(object o)
        {
            if (o is IPoint3D)
            {
                IPoint3D p = o as IPoint3D;

                if (SpellHelper.CheckTown(Caster, Caster) && CheckSequence())
                {
                    double skill = ((Caster.Skills[CastSkill].Value * 2) + Caster.Skills[DamageSkill].Value) / 3;
                    TimeSpan duration = TimeSpan.FromSeconds(1.0 + (skill / 40.0));

                    Direction d = Utility.GetDirection(Caster, p);
                    Point3D loc = Caster.Location;

                    for (int i = 0; i < 5; ++i)
                    {
                        Server.Timer.DelayCall(TimeSpan.FromMilliseconds(i*250), () =>
                            {
                                int x = loc.X;
                                int y = loc.Y;

                                Movement.Movement.Offset(d, ref x, ref y);

                                loc = new Point3D(x, y, Caster.Map.GetAverageZ(x, y));

                                bool canFit = SpellHelper.AdjustField(ref loc, Caster.Map, 12, false);

                                if (canFit)
                                {
                                    Item item = new InternalItem(Caster, 0x37CC, loc, Caster.Map, duration);
                                    item.ProcessDelta();
                                    Effects.SendLocationParticles(EffectItem.Create(loc, Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5048);
                                }
                            });
                    }

                    Caster.PlaySound(0x211);
                }
            }
        }

        [DispellableField]
        public class InternalItem : Item
        {
            public override bool BlocksFit { get { return true; } }

            public Mobile Caster { get; set; }
            public Timer Timer { get; set; }
            public DateTime Expires { get; set; }

            public InternalItem(Mobile caster, int itemID, Point3D loc, Map map, TimeSpan duration)
                : base(itemID)
            {
                Visible = false;
                Movable = false;
                Light = LightType.Circle300;

                Expires = DateTime.UtcNow + duration;
                MoveToWorld(loc, map);

                if (caster.InLOS(this))
                    Visible = true;
                else
                    Delete();

                if (Deleted)
                    return;

                Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnTick);
                Timer.Start();

                Caster = caster;
            }

            private void OnTick()
            {
                if (DateTime.UtcNow > Expires)
                    Delete();

                if (this.Deleted)
                    return;

                IPooledEnumerable eable = GetMobilesInRange(0);

                foreach(Mobile m in eable)
                {
                    if ((m.Z + 16) > this.Z && (this.Z + 12) > m.Z && m != Caster && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false))
                        OnMoveOver(m);
                }

                eable.Free();
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (Timer != null)
                {
                    Timer.Stop();
                    Timer = null;
                }
            }

            public InternalItem(Serial serial)
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

                Delete();
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (Visible && Caster != null && (!Core.AOS || m != Caster) && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false))
                {
                    if (SpellHelper.CanRevealCaster(m))
                        Caster.RevealingAction();

                    SkillName damageSkill = Caster.Skills[SkillName.Focus].Value > Caster.Skills[SkillName.Imbuing].Value ? SkillName.Focus : SkillName.Imbuing;

                    double skill = ((Caster.Skills[SkillName.Mysticism].Value) + Caster.Skills[damageSkill].Value * 2) / 3;
                    skill /= m.Player ? 3.5 : 2;

                    int damage = (int)skill + Utility.RandomMinMax(-3, 3);

                    AOS.Damage(m, Caster, damage, 0, 0, 0, 0, 0, 100, 0, DamageType.SpellAOE);
                }

                return true;
            }
        }
    }
}