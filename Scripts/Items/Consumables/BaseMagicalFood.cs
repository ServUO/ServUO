using System;
using System.Collections;

namespace Server.Items
{
    public enum MagicalFood
    {
        None = 0x0,
        GrapesOfWrath = 0x1,
        EnchantedApple = 0x2,
    }

    public class BaseMagicalFood : Food
    {
        private static Hashtable m_Table;
        private static Hashtable m_Cooldown;
        [Constructable]
        public BaseMagicalFood(int itemID)
            : base(itemID)
        {
            Weight = 1.0;
            FillFactor = 0;
            Stackable = false;
        }

        public BaseMagicalFood(Serial serial)
            : base(serial)
        {
        }

        public virtual MagicalFood FoodID => MagicalFood.None;
        public virtual TimeSpan Cooldown => TimeSpan.Zero;
        public virtual TimeSpan Duration => TimeSpan.Zero;
        public virtual int EatMessage => 0;
        public static bool IsUnderInfluence(Mobile mob, MagicalFood id)
        {
            if (m_Table != null && m_Table[mob] != null && ((int)m_Table[mob] & (int)id) > 0)
                return true;

            return false;
        }

        public static bool CoolingDown(Mobile mob, MagicalFood id)
        {
            if (m_Cooldown != null && m_Cooldown[mob] != null && ((int)m_Cooldown[mob] & (int)id) > 0)
                return true;

            return false;
        }

        public static void StartInfluence(Mobile mob, MagicalFood id, TimeSpan duration, TimeSpan cooldown)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            if (m_Table[mob] == null)
                m_Table[mob] = 0;

            m_Table[mob] = (int)m_Table[mob] | (int)id;

            Timer.DelayCall(duration, new TimerStateCallback(EndInfluence), new object[] { mob, id, cooldown });
        }

        public static void EndInfluence(object obj)
        {
            if (obj is object[] && (((object[])obj).Length == 3))
            {
                object[] args = (object[])obj;

                if (args[0] is Mobile && args[1] is MagicalFood && args[2] is TimeSpan)
                    EndInfluence((Mobile)args[0], (MagicalFood)args[1], (TimeSpan)args[2]);
            }
        }

        public static void EndInfluence(Mobile mob, MagicalFood id, TimeSpan cooldown)
        {
            m_Table[mob] = (int)m_Table[mob] & ~((int)id);

            if (cooldown != TimeSpan.Zero)
            {
                if (m_Cooldown == null)
                    m_Cooldown = new Hashtable();

                if (m_Cooldown[mob] == null)
                    m_Cooldown[mob] = 0;

                m_Cooldown[mob] = (int)m_Cooldown[mob] | (int)id;

                Timer.DelayCall(cooldown, new TimerStateCallback(EndCooldown), new object[] { mob, id });
            }
        }

        public static void EndCooldown(object obj)
        {
            if (obj is object[] && (((object[])obj).Length == 2))
            {
                object[] args = (object[])obj;

                if (args[0] is Mobile && args[1] is MagicalFood)
                    EndCooldown((Mobile)args[0], (MagicalFood)args[1]);
            }
        }

        public static void EndCooldown(Mobile mob, MagicalFood id)
        {
            m_Cooldown[mob] = (int)m_Cooldown[mob] & ~((int)id);
        }

        public override bool Eat(Mobile from)
        {
            if (!IsUnderInfluence(from, FoodID))
            {
                if (!CoolingDown(from, FoodID))
                {
                    from.SendLocalizedMessage(EatMessage);

                    StartInfluence(from, FoodID, Duration, Cooldown);
                    Consume();

                    return true;
                }
                else
                    from.SendLocalizedMessage(1070772); // You must wait a few seconds before you can use that item.
            }

            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}