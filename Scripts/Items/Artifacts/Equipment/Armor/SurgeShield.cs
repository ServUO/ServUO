using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum SurgeType
    {
        None,
        Hits,
        Mana,
        Stam
    }

    public class SurgeShield : BronzeShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1116232;

        private int m_Charges;
        private SurgeType m_Surge;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return m_Charges; } set { m_Charges = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SurgeType Surge
        {
            get { return m_Surge; }
            set
            {
                m_Surge = value;

                if (m_Surge == SurgeType.None)
                    Charges = 0;
                else
                    Charges = 50;
            }
        }

        [Constructable]
        public SurgeShield()
        {
            if (0.90 > Utility.RandomDouble())
                Hue = 2125;
            else
                Hue = 448;

            Attributes.Brittle = 1;

            switch (Utility.Random(5))
            {
                case 0: PhysicalBonus = 5; break;
                case 1: FireBonus = 5; break;
                case 2: ColdBonus = 5; break;
                case 3: PoisonBonus = 5; break;
                case 4: EnergyBonus = 5; break;
            }

            switch (Utility.Random(3))
            {
                case 0: Surge = SurgeType.Hits; break;
                case 1: Surge = SurgeType.Stam; break;
                case 2: Surge = SurgeType.Mana; break;
            }

            if (Utility.RandomBool())
                Attributes.AttackChance = 5;
            else
                Attributes.LowerManaCost = 4;

            if (Utility.RandomBool())
                Attributes.SpellChanneling = 1;
            else
                Attributes.CastSpeed = 1;
        }

        public static Dictionary<Mobile, SurgeType> Table => m_Table;
        private static readonly Dictionary<Mobile, SurgeType> m_Table = new Dictionary<Mobile, SurgeType>();

        public override void OnDoubleClick(Mobile from)
        {
            if (Surge == SurgeType.None || Charges <= 0)
                base.OnDoubleClick(from);
            else if (from.FindItemOnLayer(Layer.TwoHanded) != this)
                from.SendLocalizedMessage(1116250); //That must be equipped before you can use it.
            else if (!IsUnderEffects(from, m_Surge))
            {
                Charges--;

                m_Table[from] = m_Surge;
                Timer.DelayCall(TimeSpan.FromSeconds(20.0), new TimerStateCallback(RemoveEffects), from);

                from.PlaySound(0x0F5);
                from.PlaySound(0x1ED);
                from.FixedParticles(0x375A, 1, 30, 9966, 33, 2, EffectLayer.Waist);
                from.FixedParticles(0x37B9, 1, 30, 9502, 43, 3, EffectLayer.Waist);

                from.SendMessage("You feel a surge of energy through your body.");

                BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.Surge, 1151397, 1151398, m_Surge.ToString()));
            }
            else
            {
                from.SendLocalizedMessage(1116174); //You must wait for the energy to recharge before using the surge effect again.
            }
        }

        public static void RemoveEffects(object obj)
        {
            Mobile from = (Mobile)obj;

            if (m_Table.ContainsKey(from))
                m_Table.Remove(from);

            BuffInfo.RemoveBuff(from, BuffIcon.Surge);
            //TODO: Message?
        }

        public static bool IsUnderEffects(Mobile from, SurgeType type)
        {
            return m_Table.ContainsKey(from) && m_Table[from] == type;
        }

        public SurgeShield(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Charges);
            writer.Write((int)m_Surge);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Charges = reader.ReadInt();
            m_Surge = (SurgeType)reader.ReadInt();
        }
    }
}