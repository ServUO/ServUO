using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections;

namespace Server.Spells.Necromancy
{
    public class SummonFamiliarSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Summon Familiar", "Kal Xen Bal",
            203,
            9031,
            Reagent.BatWing,
            Reagent.GraveDust,
            Reagent.DaemonBlood);

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.25);

        public override double RequiredSkill => 30.0;
        public override int RequiredMana => 17;

        public SummonFamiliarSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        private static readonly Hashtable m_Table = new Hashtable();

        public static Hashtable Table => m_Table;

        public override bool CheckCast()
        {
            BaseCreature check = (BaseCreature)m_Table[Caster];

            if (check != null && !check.Deleted)
            {
                Caster.SendLocalizedMessage(1061605); // You already have a familiar.
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if (CheckSequence())
            {
                Caster.CloseGump(typeof(SummonFamiliarGump));
                Caster.SendGump(new SummonFamiliarGump(Caster, m_Entries, this));
            }

            FinishSequence();
        }

        private static readonly SummonFamiliarEntry[] m_Entries = new SummonFamiliarEntry[]
        {
            new SummonFamiliarEntry(typeof(HordeMinionFamiliar), 1060146, 30.0, 30.0), // Horde Minion
            new SummonFamiliarEntry(typeof(ShadowWispFamiliar), 1060142, 50.0, 50.0), // Shadow Wisp
            new SummonFamiliarEntry(typeof(DarkWolfFamiliar), 1060143, 60.0, 60.0), // Dark Wolf
            new SummonFamiliarEntry(typeof(DeathAdder), 1060145, 80.0, 80.0), // Death Adder
            new SummonFamiliarEntry(typeof(VampireBatFamiliar), 1060144, 100.0, 100.0)// Vampire Bat
        };

        public static SummonFamiliarEntry[] Entries => m_Entries;
    }

    public class SummonFamiliarEntry
    {
        private readonly Type m_Type;
        private readonly object m_Name;
        private readonly double m_ReqNecromancy;
        private readonly double m_ReqSpiritSpeak;

        public Type Type => m_Type;
        public object Name => m_Name;
        public double ReqNecromancy => m_ReqNecromancy;
        public double ReqSpiritSpeak => m_ReqSpiritSpeak;

        public SummonFamiliarEntry(Type type, object name, double reqNecromancy, double reqSpiritSpeak)
        {
            m_Type = type;
            m_Name = name;
            m_ReqNecromancy = reqNecromancy;
            m_ReqSpiritSpeak = reqSpiritSpeak;
        }
    }

    public class SummonFamiliarGump : Gump
    {
        private readonly Mobile m_From;
        private readonly SummonFamiliarEntry[] m_Entries;

        private readonly SummonFamiliarSpell m_Spell;

        private const int EnabledColor16 = 0x0F20;
        private const int DisabledColor16 = 0x262A;

        private const int EnabledColor32 = 0x18CD00;
        private const int DisabledColor32 = 0x4A8B52;

        public SummonFamiliarGump(Mobile from, SummonFamiliarEntry[] entries, SummonFamiliarSpell spell)
            : base(200, 100)
        {
            m_From = from;
            m_Entries = entries;
            m_Spell = spell;

            AddPage(0);

            AddBackground(10, 10, 250, 178, 9270);
            AddAlphaRegion(20, 20, 230, 158);

            AddImage(220, 20, 10464);
            AddImage(220, 72, 10464);
            AddImage(220, 124, 10464);

            AddItem(188, 16, 6883);
            AddItem(198, 168, 6881);
            AddItem(8, 15, 6882);
            AddItem(2, 168, 6880);

            AddHtmlLocalized(30, 26, 200, 20, 1060147, EnabledColor16, false, false); // Chose thy familiar...

            double necro = from.Skills[SkillName.Necromancy].Value;
            double spirit = from.Skills[SkillName.SpiritSpeak].Value;

            for (int i = 0; i < entries.Length; ++i)
            {
                object name = entries[i].Name;

                bool enabled = (necro >= entries[i].ReqNecromancy && spirit >= entries[i].ReqSpiritSpeak);

                AddButton(27, 53 + (i * 21), 9702, 9703, i + 1, GumpButtonType.Reply, 0);

                if (name is int)
                    AddHtmlLocalized(50, 51 + (i * 21), 150, 20, (int)name, enabled ? EnabledColor16 : DisabledColor16, false, false);
                else if (name is string)
                    AddHtml(50, 51 + (i * 21), 150, 20, string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name), false, false);
            }
        }

        private static readonly Hashtable m_Table = new Hashtable();

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int index = info.ButtonID - 1;

            if (index >= 0 && index < m_Entries.Length)
            {
                SummonFamiliarEntry entry = m_Entries[index];

                double necro = m_From.Skills[SkillName.Necromancy].Value;
                double spirit = m_From.Skills[SkillName.SpiritSpeak].Value;

                BaseCreature check = (BaseCreature)SummonFamiliarSpell.Table[m_From];

                if (check != null && !check.Deleted)
                {
                    m_From.SendLocalizedMessage(1061605); // You already have a familiar.
                }
                else if (necro < entry.ReqNecromancy || spirit < entry.ReqSpiritSpeak)
                {
                    // That familiar requires ~1_NECROMANCY~ Necromancy and ~2_SPIRIT~ Spirit Speak.
                    m_From.SendLocalizedMessage(1061606, string.Format("{0:F1}\t{1:F1}", entry.ReqNecromancy, entry.ReqSpiritSpeak));

                    m_From.CloseGump(typeof(SummonFamiliarGump));
                    m_From.SendGump(new SummonFamiliarGump(m_From, SummonFamiliarSpell.Entries, m_Spell));
                }
                else if (entry.Type == null)
                {
                    m_From.SendMessage("That familiar has not yet been defined.");

                    m_From.CloseGump(typeof(SummonFamiliarGump));
                    m_From.SendGump(new SummonFamiliarGump(m_From, SummonFamiliarSpell.Entries, m_Spell));
                }
                else
                {
                    try
                    {
                        BaseCreature bc = (BaseCreature)Activator.CreateInstance(entry.Type);

                        bc.Skills.MagicResist = m_From.Skills.MagicResist;

                        if (BaseCreature.Summon(bc, m_From, m_From.Location, -1, TimeSpan.FromDays(1.0)))
                        {
                            m_From.FixedParticles(0x3728, 1, 10, 9910, EffectLayer.Head);
                            bc.PlaySound(bc.GetIdleSound());
                            SummonFamiliarSpell.Table[m_From] = bc;
                        }
                    }
                    catch (Exception e)
                    {
                        Diagnostics.ExceptionLogging.LogException(e);
                    }
                }
            }
            else
            {
                m_From.SendLocalizedMessage(1061825); // You decide not to summon a familiar.
            }
        }
    }
}
