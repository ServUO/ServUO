using System;
using System.Globalization;
using Server;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Spells.Necromancy;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Items;
using System.Collections.Generic;

namespace Server.Spells.SkillMasteries
{
    public class RejuvinateSpell : SkillMasterySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Rejuvinate", "In Vas Ort Grav Mani",
                204,
				9061
            );

        public override double RequiredSkill { get { return 90; } }
        public override double UpKeep { get { return 0; } }
        public override int RequiredMana { get { return 10; } }

        public int RequiredTithing { get { return 100; } }

        public override SkillName CastSkill { get { return SkillName.Chivalry; } }
        public override SkillName DamageSkill { get { return SkillName.Chivalry; } }

        public RejuvinateSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void SendCastEffect()
        {
            Caster.FixedEffect(0x37C4, 87, (int)(GetCastDelay().TotalSeconds * 28), 4, 3);
        }

        public override bool CheckCast()
        {
            if (Caster is PlayerMobile && (Caster.Player && Caster.TithingPoints < RequiredTithing))
            {
                Caster.SendLocalizedMessage(1060173, RequiredTithing.ToString()); // You must have at least ~1_TITHE_REQUIREMENT~ Tithing Points to use this ability,
                return false;
            }

            BaseWeapon weapon = GetWeapon();

            if (weapon == null)
            {
                Caster.SendLocalizedMessage(1156006); // You must have a swordsmanship weapon equipped to use this ability.
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            Caster.Target = new MasteryTarget(this);
        }

        protected override void OnTarget(object o)
        {
            Mobile m = o as Mobile;

            if (m != null)
            {
                if (m.IsDeadBondedPet)
                {
                    Caster.SendLocalizedMessage(1046439); // That is not a valid target.
                }
                else if (m is BaseCreature && ((BaseCreature)m).IsAnimatedDead)
                {
                    Caster.SendLocalizedMessage(1046439); // That is not a valid target.
                }
                else if (m is Golem)
                {
                    Caster.SendLocalizedMessage(1046439); // That is not a valid target.
                }
                else if (m.Hits > m.HitsMax && m.Stam >= m.StamMax && m.Mana >= m.ManaMax)
                {
                    Caster.SendLocalizedMessage(1155788); // Your target is already at full health, mana and stamina!
                }
                else if (CheckBSequence(m))
                {
                    double rejuv = ((double)GetMasteryLevel() * 33.3) / 100;

                    if (rejuv > 1.0) rejuv = 1.0;

                    int hitsNeeds = m.HitsMax - m.Hits;
                    int stamNeeds = m.StamMax - m.Stam;
                    int manaNeeds = m.ManaMax - m.Mana;

                    int toRejuv = 0;

                    if (hitsNeeds > 0)
                    {
                        toRejuv = (int)Math.Ceiling(hitsNeeds * rejuv);

                        if (toRejuv > 0)
                            SpellHelper.Heal(toRejuv, m, Caster, false); 
                    }

                    if (stamNeeds > 0)
                    {
                        toRejuv = (int)Math.Ceiling(stamNeeds * rejuv);

                        if (toRejuv > 0)
                            m.Stam += toRejuv;
                    }

                    if (manaNeeds > 0)
                    {
                        toRejuv = (int)Math.Ceiling(manaNeeds * rejuv);

                        if (toRejuv > 0)
                            m.Mana += toRejuv;
                    }

                    if (Caster.Karma > Utility.Random(5000))
                    {
                        if (m.Poison != null)
                            m.CurePoison(Caster);

                        StatMod mod;

                        mod = m.GetStatMod("[Magic] Str Offset");
                        if (mod != null && mod.Offset < 0)
                            m.RemoveStatMod("[Magic] Str Offset");

                        mod = m.GetStatMod("[Magic] Dex Offset");
                        if (mod != null && mod.Offset < 0)
                            m.RemoveStatMod("[Magic] Dex Offset");

                        mod = m.GetStatMod("[Magic] Int Offset");
                        if (mod != null && mod.Offset < 0)
                            m.RemoveStatMod("[Magic] Int Offset");

                        m.Paralyzed = false;

                        EvilOmenSpell.TryEndEffect(m);
                        StrangleSpell.RemoveCurse(m);
                        CorpseSkinSpell.RemoveCurse(m);
                        CurseSpell.RemoveEffect(m);
                        MortalStrike.EndWound(m);
                        BloodOathSpell.RemoveCurse(m);
                        MindRotSpell.ClearMindRotScalar(m);

                        BuffInfo.RemoveBuff(m, BuffIcon.Clumsy);
                        BuffInfo.RemoveBuff(m, BuffIcon.FeebleMind);
                        BuffInfo.RemoveBuff(m, BuffIcon.Weaken);
                        BuffInfo.RemoveBuff(m, BuffIcon.Curse);
                        BuffInfo.RemoveBuff(m, BuffIcon.MassCurse);
                        BuffInfo.RemoveBuff(m, BuffIcon.MortalStrike);
                        BuffInfo.RemoveBuff(m, BuffIcon.Mindrot);
                    }

                    Caster.PlaySound(0x102);

                    m.SendLocalizedMessage(1155789); // You feel completely rejuvenated!

                    if (Caster != m)
                    {
                        m.PlaySound(0x102);
                        Caster.SendLocalizedMessage(1155790); // Your target has been rejuvenated!
                    }

                    int skill = ((int)Caster.Skills[CastSkill].Value + GetWeaponSkill() + GetMasteryLevel() * 40) / 3;
                    int duration;

                    if (skill >= 120)
                        duration = 60;
                    else if (skill >= 110)
                        duration = 120;
                    else
                        duration = 180;

                    TimeSpan d;

                    if(Caster.AccessLevel == AccessLevel.Player)
                        d = TimeSpan.FromMinutes(duration);
                    else
                        d =  TimeSpan.FromSeconds(10);

                    AddToCooldown(d);
                }
            }
            else
                Caster.SendLocalizedMessage(1046439); // That is not a valid target.
        }

        public override bool CheckSequence()
        {
            int requiredTithing = this.RequiredTithing;

            if (Caster is PlayerMobile && (Caster.Player && Caster.TithingPoints < requiredTithing))
            {
                Caster.SendLocalizedMessage(1060173, RequiredTithing.ToString()); // You must have at least ~1_TITHE_REQUIREMENT~ Tithing Points to use this ability,
                return false;
            }

            if (AosAttributes.GetValue(Caster, AosAttribute.LowerRegCost) > Utility.Random(100))
                requiredTithing = 0;

            if (requiredTithing > 0 && Caster is PlayerMobile)
                Caster.TithingPoints -= requiredTithing;

            return base.CheckSequence();
        }
    }
}