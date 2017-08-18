using System;
using System.Collections.Generic;
using Server.Mobiles;
using System.Text;
using Server;

namespace Server.Items
{
    public enum ThieveConsumableEffect
    {
        None,
        BalmOfStrengthEffect,
        BalmOfWisdomEffect,
        BalmOfSwiftnessEffect,
        BalmOfProtectionEffect,
        StoneSkinLotionEffect,
        LifeShieldLotionEffect,
    }

    public class ThieveConsumableInfo
    {
        public ThieveConsumableEffect Effect;
        public Timer EffectTimer;

        public ThieveConsumableInfo(BaseThieveConsumable.InternalTimer t, ThieveConsumableEffect e)
        {
            Effect = e;
            EffectTimer = t;
        }
    }

    public abstract class BaseThieveConsumable : Item
    {
        public BaseThieveConsumable(int itemId)
            : base(itemId)
        {
        }

        public class InternalTimer : Timer
        {
            public PlayerMobile pm;
            public ThieveConsumableEffect effect;

            protected override void OnTick()
            {
                BaseThieveConsumable.RemoveEffect(pm,effect);
            }

            public InternalTimer(PlayerMobile p, ThieveConsumableEffect e, TimeSpan delay)
                : base(delay)
            {
                pm = p;
                effect = e; 
            }
        }

        public   TimeSpan m_EffectDuration;
        protected  ThieveConsumableEffect m_EffectType;

        protected virtual void OnUse(PlayerMobile by)
        {
        }

        protected virtual void ApplyEffect(PlayerMobile pm)
        {

            if (m_EffectDuration == TimeSpan.Zero)
            {
                m_EffectDuration = TimeSpan.FromMinutes(30); 
            }

            InternalTimer t = new InternalTimer(pm,m_EffectType,m_EffectDuration);
            t.Start(); 

            ThieveConsumableInfo info = new ThieveConsumableInfo(t, this.m_EffectType);
            
            if (EffectTable.ContainsKey(pm))
            {
                RemoveEffect(pm, EffectTable[pm].Effect); 
            }
                
            EffectTable.Add(pm, info);
            this.Consume(); 
        }

        protected static void RemoveEffect(PlayerMobile pm, ThieveConsumableEffect effectType)
        {
            if (EffectTable.ContainsKey(pm))
            {

                EffectTable[pm].EffectTimer.Stop();
                EffectTable.Remove(pm);

                pm.SendLocalizedMessage(1095134);//The effects of the balm or lotion have worn off.

                if (effectType == ThieveConsumableEffect.BalmOfStrengthEffect || effectType == ThieveConsumableEffect.BalmOfSwiftnessEffect || effectType == ThieveConsumableEffect.BalmOfWisdomEffect)
                {
                    pm.RemoveStatMod("Balm");
                }
                else if (effectType == ThieveConsumableEffect.StoneSkinLotionEffect)
                {

                    List<ResistanceMod> list = pm.ResistanceMods;

                    for (int i = 0; i < list.Count; i++)
                    {
                        ResistanceMod curr = list[i];
                        if ((curr.Type == ResistanceType.Cold && curr.Offset == -5) || (curr.Type == ResistanceType.Fire && curr.Offset == -5) || (curr.Type == ResistanceType.Physical && curr.Offset == 30))
                        {
                            list.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        private static Dictionary<PlayerMobile, ThieveConsumableInfo> EffectTable = new Dictionary<PlayerMobile, ThieveConsumableInfo>();

        public static bool CanUse(PlayerMobile pm, BaseThieveConsumable consum)
        {
            if (CheckThieveConsumable(pm) != ThieveConsumableEffect.None)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsUnderThieveConsumableEffect(PlayerMobile pm, ThieveConsumableEffect eff)
        {
            if (EffectTable.ContainsKey(pm))
            {
                if (EffectTable[pm].Effect == eff)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static ThieveConsumableEffect CheckThieveConsumable(PlayerMobile pm)
        {
            if (EffectTable.ContainsKey(pm))
            {
                return EffectTable[pm].Effect;
            }
            else
            {
                return ThieveConsumableEffect.None;
            }
        }

        public BaseThieveConsumable(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)m_EffectType);
            writer.Write(m_EffectDuration); 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_EffectType = (ThieveConsumableEffect)reader.ReadInt();
            m_EffectDuration = reader.ReadTimeSpan(); 
        }
    }
}
