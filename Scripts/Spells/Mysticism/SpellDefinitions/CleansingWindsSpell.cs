using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public class CleansingWindsSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Cleansing Winds", "In Vas Mani Hur",
            230,
            9022,
            Reagent.Garlic,
            Reagent.Ginseng,
            Reagent.MandrakeRoot,
            Reagent.DragonBlood);
        public CleansingWindsSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // Soothing winds attempt to neutralize poisons, lift curses, and heal a valid Target. 
        public override int RequiredMana
        {
            get
            {
                return 20;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 58;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new MysticSpellTarget(this, TargetFlags.Beneficial);
        }

        public override void OnTarget(Object o)
        {
            IPoint3D p = o as IPoint3D;

            if (p == null)
                return;
            else if (this.CheckSequence())
            {
                List<Mobile> targets = new List<Mobile>();
                StatMod mod;

                foreach (Mobile mob in this.Caster.Map.GetMobilesInRange(new Point3D(p), 3))
                {
                    if (mob == null)
                        continue;

                    if (this.Caster is PlayerMobile)
                        if (this.Caster.CanBeBeneficial(mob, false))
                            targets.Add(mob);
                }

                Mobile m;
                int toheal = (int)(this.Caster.Skills[SkillName.Mysticism].Value * 0.1);
                this.Caster.PlaySound(0x64D);

                for (int i = 0; i < targets.Count; i++)
                {
                    m = targets[i];

                    if (!m.Alive)
                        continue;

                    m.Heal(toheal + Utility.RandomMinMax(1, 5));

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
                    m.Asleep = false; // SA Mysticism Edit
                    m.CurePoison(this.Caster);
                    EvilOmenSpell.TryEndEffect(m);
                    StrangleSpell.RemoveCurse(m);
                    CorpseSkinSpell.RemoveCurse(m);
                    CurseSpell.RemoveEffect(m);
                    MortalStrike.EndWound(m);

                    if (Core.ML)
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
            }
        }
    }
}
/*




*/