using System;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;

namespace Server.Spells.SkillMasteries
{
    public class FlamingShotSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Flameing Shot", "",
                -1,
                9002
            );

        public override int RequiredMana { get { return 30; } }

        public override DamageType SpellDamageType { get { return DamageType.SpellAOE; } }
        public override SkillName CastSkill { get { return SkillName.Archery; } }
        public override SkillName DamageSkill { get { return SkillName.Tactics; } }
        public override bool DelayedDamage { get { return true; } }

        public FlamingShotSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!CheckWeapon())
            {
                Caster.SendLocalizedMessage(1156000); // You must have an Archery weapon to use this ability!
                return false;
            }

            return base.CheckCast();
        }

        public override void SendCastEffect()
        {
            //Caster.FixedParticles(0x3709, 10, 30, 5052, 0x498, 0, EffectLayer.LeftFoot);
            Caster.PrivateOverheadMessage(MessageType.Regular, 1150, 1155999, Caster.NetState); // You ready a volley of flaming arrows!
            Effects.SendTargetParticles(Caster, 0x3709, 10, 30, 2724, 0, 9907, EffectLayer.LeftFoot, 0);
            Caster.PlaySound(0x5CF);
        }

        public override void OnCast()
        {
            Caster.Target = new MasteryTarget(this, allowGround: true);
        }

        protected override void OnTarget(object o)
        {
            BaseWeapon weapon = GetWeapon();

            if (weapon is BaseRanged && !(weapon is BaseThrown))
            {
                IPoint3D p = o as IPoint3D;

                if (p != null && SpellHelper.CheckTown(p, Caster) && CheckSequence())
                {
                    IPooledEnumerable eable = Caster.Map.GetMobilesInRange(new Point3D(p), 5);
                    List<Mobile> targets = new List<Mobile>();

                    foreach (Mobile mob in eable)
                    {
                        if (Caster != mob && SpellHelper.ValidIndirectTarget(Caster, mob) && Caster.CanBeHarmful(mob, false))
                        {
                            if (!Caster.InLOS(mob))
                                continue;

                            targets.Add(mob);
                        }
                    }
                    eable.Free();

                    double damage;

                    foreach (Mobile mob in targets)
                    {
                        Caster.MovingEffect(mob, ((BaseRanged)weapon).EffectID, 18, 1, false, false);

                        if (weapon.CheckHit(Caster, mob))
                        {
                            damage = GetNewAosDamage(40, 1, 5, mob);

                            if (targets.Count > 2)
                                damage = damage / targets.Count;

                            damage *= GetDamageScalar(mob);
                            Caster.DoHarmful(mob);
                            SpellHelper.Damage(this, mob, damage, 0, 100, 0, 0, 0);

                            Server.Timer.DelayCall(TimeSpan.FromMilliseconds(800), obj =>
                                {
                                    Mobile mobile = obj as Mobile;

                                    if(mobile != null)
                                        mobile.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                                }, mob );

                            mob.PlaySound(0x1DD);
                        }
                    }

                    weapon.PlaySwingAnimation(Caster);
                    Caster.PlaySound(0x101);
                }
            }
        }
    }
}