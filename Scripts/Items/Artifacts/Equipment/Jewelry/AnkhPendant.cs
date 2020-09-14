using Server.Regions;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum VirtueType
    {
        None = 0,
        Honesty = 1,
        Compassion = 2,
        Valor = 3,
        Justice = 4,
        Sacrafice = 5,
        Honor = 6,
        Spirituality = 7,
        Humility = 8
    }

    public class AnkhPendant : BaseNecklace
    {
        public static void Initialize()
        {
            EventSink.Speech += EventSink_Speech;
        }

        public override int LabelNumber => 1079525;  // Ankh Pendant

        [Constructable]
        public AnkhPendant()
            : base(0x3BB5)
        {
            Hue = Utility.RandomBool() ? 2213 : 0;
        }

        public static void EventSink_Speech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            Item ankh = from.FindItemOnLayer(Layer.Neck);

            if (!(ankh is AnkhPendant))
                return;

            string str = e.Speech.ToLower();
            VirtueType t = VirtueType.None;

            if (str == "ahm")
                t = VirtueType.Honesty;
            else if (str == "mu")
                t = VirtueType.Compassion;
            else if (str == "ra")
                t = VirtueType.Valor;
            else if (str == "beh")
                t = VirtueType.Justice;
            else if (str == "cah")
                t = VirtueType.Sacrafice;
            else if (str == "summ")
                t = VirtueType.Honor;
            else if (str == "om")
                t = VirtueType.Spirituality;
            else if (str == "lum")
                t = VirtueType.Humility;

            if (t != VirtueType.None && CheckShrine(t, from))
                ApplyBonus(t, from);
        }

        public static int GetHitsRegenModifier(Mobile from)
        {
            if (!m_Table.ContainsKey(from))
                return 0;

            if (CheckExpired(from))
                return 0;

            AnkhPendantBonusContext context = m_Table[from];

            if (context == null)
                return 0;

            switch (context.VType)
            {
                case VirtueType.Honesty: break;
                case VirtueType.Compassion:
                    return 2;
                case VirtueType.Valor: break;
                case VirtueType.Justice:
                    return context.DoBump ? 2 : 1;
                case VirtueType.Sacrafice:
                    return context.DoBump ? 2 : 1;
                case VirtueType.Honor: break;
                case VirtueType.Spirituality:
                    return context.DoBump ? 2 : 1;
                case VirtueType.Humility:
                    if (context.Random == 0)
                        return 3;
                    break;
            }
            return 0;
        }

        public static int GetStamRegenModifier(Mobile from)
        {
            if (!m_Table.ContainsKey(from))
                return 0;

            if (CheckExpired(from))
                return 0;

            AnkhPendantBonusContext context = m_Table[from];

            if (context == null)
                return 0;

            switch (context.VType)
            {
                case VirtueType.Honesty: break;
                case VirtueType.Compassion: break;
                case VirtueType.Valor:
                    return 2;
                case VirtueType.Justice:
                    break;
                case VirtueType.Sacrafice:
                    return context.DoBump ? 2 : 1;
                case VirtueType.Honor:
                    return context.DoBump ? 2 : 1;
                case VirtueType.Spirituality:
                    return context.DoBump2 ? 2 : 1;
                case VirtueType.Humility:
                    if (context.Random == 1)
                        return 3;
                    break;
            }
            return 0;
        }

        public static int GetManaRegenModifier(Mobile from)
        {
            if (!m_Table.ContainsKey(from))
                return 0;

            if (CheckExpired(from))
                return 0;

            AnkhPendantBonusContext context = m_Table[from];

            if (context == null)
                return 0;

            switch (context.VType)
            {
                case VirtueType.Honesty:
                    return 2;
                case VirtueType.Compassion: break;
                case VirtueType.Valor:
                    break;
                case VirtueType.Justice:
                    return context.DoBump ? 2 : 1;
                case VirtueType.Sacrafice:
                    break;
                case VirtueType.Honor:
                    return context.DoBump ? 2 : 1;
                case VirtueType.Spirituality:
                    return context.DoBump3 ? 2 : 1;
                case VirtueType.Humility:
                    if (context.Random == 2)
                        return 3;
                    break;
            }
            return 0;
        }

        public static bool CheckExpired(Mobile from)
        {
            if (m_Table.ContainsKey(from) && m_Table[from].Expired)
            {
                AddToCooldown(from);
                return true;
            }
            return false;
        }

        public static bool CheckShrine(VirtueType t, Mobile from)
        {
            Region r = from.Region;
            Map map = from.Map;

            if (r is DungeonRegion || r is TownRegion || (map != Map.Trammel && map != Map.Felucca))
                return false;

            bool atShrine = false;

            for (int i = 0; i < m_ShrineLocs.Length; i++)
            {
                if (m_ShrineLocs[i].Contains(new Point2D(from.X, from.Y)) && (int)t == i + 1)
                {
                    atShrine = true;
                    break;
                }
            }

            if (atShrine)
            {
                if (IsUnderEffects(from))
                {
                    from.SendLocalizedMessage(1079544, string.Format("#{0}", GetCliloc(m_Table[from].VType)));
                    return false;
                }

                if (IsWaitingCooldown(from))
                {
                    TimeSpan ts = DateTime.UtcNow - m_Cooldown[from];

                    if (ts.TotalHours >= 1)
                        from.SendLocalizedMessage(1079550, ((int)ts.TotalHours).ToString()); //You can improve your fortunes again in about ~1_TIME~ hours.
                    else
                        from.SendLocalizedMessage(1079547); //Your fortunes are about to improve.

                    return false;
                }
            }

            return atShrine;
        }

        public static Rectangle2D[] ShrineLocs => m_ShrineLocs;
        private static readonly Rectangle2D[] m_ShrineLocs = new Rectangle2D[]
        {
            new Rectangle2D(4208, 563, 2, 2), //Honesty
			new Rectangle2D(1857, 874, 2, 2), //Compassion
			new Rectangle2D(2491, 3930, 2, 2), //Valor
			new Rectangle2D(1300, 633, 2, 2), //Justice
			new Rectangle2D(3354, 289, 2, 2), //Sacrafice
			new Rectangle2D(1726, 3527, 2, 2), //Honor
			new Rectangle2D(1605, 2489, 2, 2), //Spirituality
			new Rectangle2D(4273, 3696, 2, 2), //Humility
		};

        private static void ApplyBonus(VirtueType t, Mobile from)
        {
            m_Table[from] = new AnkhPendantBonusContext(from, t);

            from.Delta(MobileDelta.WeaponDamage);

            from.SendLocalizedMessage(1079546, string.Format("#{0}", GetCliloc(t)));
        }

        private static int GetCliloc(VirtueType t)
        {
            switch (t)
            {
                default:
                case VirtueType.Honesty: return 1079539;
                case VirtueType.Compassion: return 1079535;
                case VirtueType.Valor: return 1079543;
                case VirtueType.Justice: return 1079536;
                case VirtueType.Sacrafice: return 1079538;
                case VirtueType.Honor: return 1079540;
                case VirtueType.Spirituality: return 1079542;
                case VirtueType.Humility: return 1079541;
            }
        }

        public static bool IsUnderEffects(Mobile from)
        {
            return m_Table.ContainsKey(from);
        }

        public static bool IsWaitingCooldown(Mobile from)
        {
            if (m_Cooldown.ContainsKey(from) && m_Cooldown[from] < DateTime.UtcNow)
                m_Cooldown.Remove(from);

            return m_Cooldown.ContainsKey(from);
        }

        public static void AddToCooldown(Mobile from)
        {
            if (m_Table.ContainsKey(from))
                m_Table.Remove(from);

            from.Delta(MobileDelta.WeaponDamage);

            if (from.NetState != null)
                from.SendLocalizedMessage(1079553); //The effects of meditating at the shrine have worn off.	

            m_Cooldown[from] = DateTime.UtcNow + TimeSpan.FromHours(24);
        }

        private static readonly Dictionary<Mobile, AnkhPendantBonusContext> m_Table = new Dictionary<Mobile, AnkhPendantBonusContext>();
        private static readonly Dictionary<Mobile, DateTime> m_Cooldown = new Dictionary<Mobile, DateTime>();

        private class AnkhPendantBonusContext
        {
            private readonly Mobile m_Mobile;
            private readonly VirtueType m_Type;
            private readonly int m_Random;
            private readonly bool m_DoBump;
            private readonly bool m_DoBump2;
            private readonly bool m_DoBump3;
            private readonly DateTime m_Expires;

            public VirtueType VType => m_Type;
            public bool DoBump => m_DoBump;
            public bool DoBump2 => m_DoBump2;
            public bool DoBump3 => m_DoBump3;
            public int Random => m_Random;

            public bool Expired => DateTime.UtcNow > m_Expires;

            public AnkhPendantBonusContext(Mobile from, VirtueType type)
            {
                m_Mobile = from;
                m_Type = type;
                m_Random = -1;
                m_Expires = DateTime.UtcNow + TimeSpan.FromMinutes(60);

                switch (type)
                {
                    case VirtueType.Honesty:
                    case VirtueType.Compassion:
                    case VirtueType.Valor: break;
                    case VirtueType.Humility:
                        m_Random = Utility.Random(3);
                        break;
                    case VirtueType.Justice:
                    case VirtueType.Sacrafice:
                    case VirtueType.Honor:
                        m_DoBump = Utility.RandomBool();
                        break;
                    case VirtueType.Spirituality:
                        m_DoBump = 0.25 > Utility.RandomDouble();
                        m_DoBump2 = 0.25 > Utility.RandomDouble();
                        m_DoBump3 = 0.25 > Utility.RandomDouble();
                        break;

                }
            }
        }

        public AnkhPendant(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                reader.ReadBool();
        }
    }
}
