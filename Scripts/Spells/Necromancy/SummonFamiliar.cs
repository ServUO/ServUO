using System;
using System.Collections;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

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

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(2.0);
            }
        }

        public override double RequiredSkill
        {
            get
            {
                return 30.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 17;
            }
        }

        public SummonFamiliarSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        private static readonly Hashtable m_Table = new Hashtable();

        public static Hashtable Table
        {
            get
            {
                return m_Table;
            }
        }

        public override bool CheckCast()
        {
            BaseCreature check = (BaseCreature)m_Table[this.Caster];

            if (check != null && !check.Deleted)
            {
                this.Caster.SendLocalizedMessage(1061605); // You already have a familiar.
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                this.Caster.CloseGump(typeof(SummonFamiliarGump));
                this.Caster.SendGump(new SummonFamiliarGump(this.Caster, m_Entries, this));
            }

            this.FinishSequence();
        }

        private static readonly SummonFamiliarEntry[] m_Entries = new SummonFamiliarEntry[]
        {
            new SummonFamiliarEntry(typeof(HordeMinionFamiliar), 1060146, 30.0, 30.0), // Horde Minion
            new SummonFamiliarEntry(typeof(ShadowWispFamiliar), 1060142, 50.0, 50.0), // Shadow Wisp
            new SummonFamiliarEntry(typeof(DarkWolfFamiliar), 1060143, 60.0, 60.0), // Dark Wolf
            new SummonFamiliarEntry(typeof(DeathAdder), 1060145, 80.0, 80.0), // Death Adder
            new SummonFamiliarEntry(typeof(VampireBatFamiliar), 1060144, 100.0, 100.0)// Vampire Bat
        };

        public static SummonFamiliarEntry[] Entries
        {
            get
            {
                return m_Entries;
            }
        }
    }

    public class SummonFamiliarEntry
    {
        private readonly Type m_Type;
        private readonly object m_Name;
        private readonly double m_ReqNecromancy;
        private readonly double m_ReqSpiritSpeak;

        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        public object Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public double ReqNecromancy
        {
            get
            {
                return this.m_ReqNecromancy;
            }
        }
        public double ReqSpiritSpeak
        {
            get
            {
                return this.m_ReqSpiritSpeak;
            }
        }

        public SummonFamiliarEntry(Type type, object name, double reqNecromancy, double reqSpiritSpeak)
        {
            this.m_Type = type;
            this.m_Name = name;
            this.m_ReqNecromancy = reqNecromancy;
            this.m_ReqSpiritSpeak = reqSpiritSpeak;
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
            this.m_From = from;
            this.m_Entries = entries;
            this.m_Spell = spell;

            this.AddPage(0);

            this.AddBackground(10, 10, 250, 178, 9270);
            this.AddAlphaRegion(20, 20, 230, 158);

            this.AddImage(220, 20, 10464);
            this.AddImage(220, 72, 10464);
            this.AddImage(220, 124, 10464);

            this.AddItem(188, 16, 6883);
            this.AddItem(198, 168, 6881);
            this.AddItem(8, 15, 6882);
            this.AddItem(2, 168, 6880);

            this.AddHtmlLocalized(30, 26, 200, 20, 1060147, EnabledColor16, false, false); // Chose thy familiar...

            double necro = from.Skills[SkillName.Necromancy].Value;
            double spirit = from.Skills[SkillName.SpiritSpeak].Value;

            for (int i = 0; i < entries.Length; ++i)
            {
                object name = entries[i].Name;

                bool enabled = (necro >= entries[i].ReqNecromancy && spirit >= entries[i].ReqSpiritSpeak);

                this.AddButton(27, 53 + (i * 21), 9702, 9703, i + 1, GumpButtonType.Reply, 0);

                if (name is int)
                    this.AddHtmlLocalized(50, 51 + (i * 21), 150, 20, (int)name, enabled ? EnabledColor16 : DisabledColor16, false, false);
                else if (name is string)
                    this.AddHtml(50, 51 + (i * 21), 150, 20, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", enabled ? EnabledColor32 : DisabledColor32, name), false, false);
            }
        }

        private static readonly Hashtable m_Table = new Hashtable();

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int index = info.ButtonID - 1;

            if (index >= 0 && index < this.m_Entries.Length)
            {
                SummonFamiliarEntry entry = this.m_Entries[index];

                double necro = this.m_From.Skills[SkillName.Necromancy].Value;
                double spirit = this.m_From.Skills[SkillName.SpiritSpeak].Value;

                BaseCreature check = (BaseCreature)SummonFamiliarSpell.Table[this.m_From];

                if (check != null && !check.Deleted)
                {
                    this.m_From.SendLocalizedMessage(1061605); // You already have a familiar.
                }
                else if (necro < entry.ReqNecromancy || spirit < entry.ReqSpiritSpeak)
                {
                    // That familiar requires ~1_NECROMANCY~ Necromancy and ~2_SPIRIT~ Spirit Speak.
                    this.m_From.SendLocalizedMessage(1061606, String.Format("{0:F1}\t{1:F1}", entry.ReqNecromancy, entry.ReqSpiritSpeak));

                    this.m_From.CloseGump(typeof(SummonFamiliarGump));
                    this.m_From.SendGump(new SummonFamiliarGump(this.m_From, SummonFamiliarSpell.Entries, this.m_Spell));
                }
                else if (entry.Type == null)
                {
                    this.m_From.SendMessage("That familiar has not yet been defined.");

                    this.m_From.CloseGump(typeof(SummonFamiliarGump));
                    this.m_From.SendGump(new SummonFamiliarGump(this.m_From, SummonFamiliarSpell.Entries, this.m_Spell));
                }
                else
                {
                    try
                    {
                        BaseCreature bc = (BaseCreature)Activator.CreateInstance(entry.Type);

                        bc.Skills.MagicResist = this.m_From.Skills.MagicResist;

                        if (BaseCreature.Summon(bc, this.m_From, this.m_From.Location, -1, TimeSpan.FromDays(1.0)))
                        {
                            this.m_From.FixedParticles(0x3728, 1, 10, 9910, EffectLayer.Head);
                            bc.PlaySound(bc.GetIdleSound());
                            SummonFamiliarSpell.Table[this.m_From] = bc;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                this.m_From.SendLocalizedMessage(1061825); // You decide not to summon a familiar.
            }
        }
    }
}
