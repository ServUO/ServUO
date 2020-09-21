using System.Collections;

namespace Server.Spells.First
{
    public class ReactiveArmorSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Reactive Armor", "Flam Sanct",
            236,
            9011,
            Reagent.Garlic,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh);
        private static readonly Hashtable m_Table = new Hashtable();
        public ReactiveArmorSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.First;
        public static void EndArmor(Mobile m)
        {
            if (m_Table.Contains(m))
            {
                ResistanceMod[] mods = (ResistanceMod[])m_Table[m];

                if (mods != null)
                {
                    for (int i = 0; i < mods.Length; ++i)
                        m.RemoveResistanceMod(mods[i]);
                }

                m_Table.Remove(m);
                BuffInfo.RemoveBuff(m, BuffIcon.ReactiveArmor);
            }
        }

        public override void OnCast()
        {
            /* The reactive armor spell increases the caster's physical resistance, while lowering the caster's elemental resistances.
            * 15 + (Inscription/20) Physcial bonus
            * -5 Elemental
            * The reactive armor spell has an indefinite duration, becoming active when cast, and deactivated when re-cast. 
            * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out, even after dying—until you “turn them off” by casting them again. 
            * (+20 physical -5 elemental at 100 Inscription)
            */
            if (CheckSequence())
            {
                Mobile targ = Caster;

                ResistanceMod[] mods = (ResistanceMod[])m_Table[targ];

                if (mods == null)
                {
                    targ.PlaySound(0x1E9);
                    targ.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);

                    mods = new ResistanceMod[5]
                    {
                        new ResistanceMod(ResistanceType.Physical, 15 + (int)(targ.Skills[SkillName.Inscribe].Value / 20)),
                        new ResistanceMod(ResistanceType.Fire, -5),
                        new ResistanceMod(ResistanceType.Cold, -5),
                        new ResistanceMod(ResistanceType.Poison, -5),
                        new ResistanceMod(ResistanceType.Energy, -5)
                    };

                    m_Table[targ] = mods;

                    for (int i = 0; i < mods.Length; ++i)
                        targ.AddResistanceMod(mods[i]);

                    int physresist = 15 + (int)(targ.Skills[SkillName.Inscribe].Value / 20);
                    string args = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", physresist, 5, 5, 5, 5);

                    BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ReactiveArmor, 1075812, 1075813, args.ToString()));
                }
                else
                {
                    targ.PlaySound(0x1ED);
                    targ.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);

                    m_Table.Remove(targ);

                    for (int i = 0; i < mods.Length; ++i)
                        targ.RemoveResistanceMod(mods[i]);

                    BuffInfo.RemoveBuff(Caster, BuffIcon.ReactiveArmor);
                }
            }

            FinishSequence();

        }

        public static bool HasArmor(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }
    }
}