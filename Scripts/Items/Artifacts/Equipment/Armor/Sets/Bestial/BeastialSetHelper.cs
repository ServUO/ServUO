using System;
using Server.Mobiles;
using System.Collections.Generic;
using System.Linq;

namespace Server.Berserk
{
    public class BeastialSetHelper
    {
        public static bool CheckBestialArmor(PlayerMobile pm)
        {
            return pm.Items.Where(i => i != null && i is ISetItem && ((ISetItem)i).SetID == SetItem.Bestial && i.Parent is Mobile && ((Mobile)i.Parent).FindItemOnLayer(i.Layer) == i) != null;
        }

        public static void CheckEquipBestial(PlayerMobile pm)
        {
            if (pm.EquipBestial != null)
                pm.EquipBestial.Clear();

            List<Item> equipment = pm.Items.Where(i => i != null && i is ISetItem && ((ISetItem)i).SetID == SetItem.Bestial && i.Parent is Mobile && ((Mobile)i.Parent).FindItemOnLayer(i.Layer) == i).ToList();

            pm.EquipBestial = equipment;
            pm.m_EquipBestialAmount = equipment.Count();
        }

        public static int AddBestialHueParent(PlayerMobile pm)
        {
            int color = pm.Items.FirstOrDefault(i => i != null && i is ISetItem && ((ISetItem)i).SetID == SetItem.Bestial && i.Parent is Mobile && ((Mobile)i.Parent).FindItemOnLayer(i.Layer) == i).Hue;

            CheckEquipBestial(pm);

            if (pm.m_EquipBestialAmount == 4)
            {
                pm.TempBodyColor = pm.Hue;
                pm.Hue = color;
                pm.BestialBodyHue = pm.TempBodyColor;
                pm.IsBodyHue = true;
            }

            return color;
        }

        public static void DropBestialHueParent(PlayerMobile pm)
        {
            if (pm.IsBodyHue)
            {
                pm.Hue = pm.BestialBodyHue;
                pm.IsBodyHue = false;
            }
        }        
    }

    public class BestialBerserkTimer : Timer
    {
        private PlayerMobile m_Owner;
        private int m_Count = 0;
        private bool msg;
        private const int MaxCount = 10;

        public BestialBerserkTimer(PlayerMobile owner)
            : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
        {
            m_Owner = owner;

            BeastialSetHelper.CheckEquipBestial(m_Owner);

            if (!m_Owner.BestialBerserk)
            {
                m_Owner.SendLocalizedMessage(1151532); //You enter a berserk rage!
                m_Owner.BestialBerserk = true;

                foreach (var item in m_Owner.EquipBestial)
                {
                    item.Hue = 1255;
                }

                if (m_Owner.m_EquipBestialAmount == 4)
                {
                    m_Owner.TempBodyColor = m_Owner.Hue;
                    m_Owner.Hue = 1255;
                    m_Owner.BestialBodyHue = m_Owner.TempBodyColor;
                    m_Owner.IsBodyHue = true;
                }
            }
            else
            {
                msg = false;

                foreach (var item in m_Owner.EquipBestial.Where(i => i.Hue < 1260))
                {
                    item.Hue++;

                    if (!msg)
                    {
                        m_Owner.SendLocalizedMessage(1151533, "", item.Hue); //Your rage grows!

                        if (m_Owner.m_EquipBestialAmount == 4)
                            m_Owner.Hue++;

                        msg = true;
                    }
                }
            }
        }

        protected override void OnTick()
        {
            if (!m_Owner.Alive)
                RemoveEffect();

            m_Count++;

            BeastialSetHelper.CheckEquipBestial(m_Owner);

            if (m_Count >= MaxCount)
            {
                RemoveEffect();
            }
            else
            {
                if (m_Count % 3 == 0)
                {
                    msg = false;

                    foreach (var item in m_Owner.EquipBestial.Where(i => i.Hue > 1255))
                    {
                        item.Hue--;

                        if (!msg)
                        {
                            m_Owner.SendLocalizedMessage(1151534, "", item.Hue); //Your rage recedes.

                            if (m_Owner.m_EquipBestialAmount == 4)
                                m_Owner.Hue--;

                            msg = true;
                        }
                    }
                }
            }
        }

        public void RemoveEffect()
        {
            Stop();

            BeastialSetHelper.CheckEquipBestial(m_Owner);

            foreach (var item in m_Owner.EquipBestial)
            {
                item.Hue = 2010;
            }

            m_Owner.BestialBerserk = false;
            m_Owner.SendLocalizedMessage(1151535); //Your berserk rage has subsided.               

            if (m_Owner.IsBodyHue)
            {
                m_Owner.Hue = m_Owner.BestialBodyHue;
                m_Owner.IsBodyHue = false;
            }
        }
    }

    public class BerserkTimer : Timer
    {
        private PlayerMobile m_Owner;

        public BerserkTimer(PlayerMobile owner)
            : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
        {
            m_Owner = owner;

            m_Owner.PlaySound(0x20F);
            m_Owner.PlaySound(m_Owner.Body.IsFemale ? 0x338 : 0x44A);
            m_Owner.FixedParticles(0x376A, 1, 31, 9961, 1160, 0, EffectLayer.Waist);
            m_Owner.FixedParticles(0x37C4, 1, 31, 9502, 43, 2, EffectLayer.Waist);

            BuffInfo.AddBuff(m_Owner, new BuffInfo(BuffIcon.Berserk, 1080449, 1115021, "15\t3", false));

            m_Owner.Berserk = true;
        }

        protected override void OnTick()
        {
            float percentage = (float)m_Owner.Hits / m_Owner.HitsMax;

            if (percentage >= 0.8)
                RemoveEffect();
        }

        public void RemoveEffect()
        {
            m_Owner.PlaySound(0xF8);
            BuffInfo.RemoveBuff(m_Owner, BuffIcon.Berserk);

            m_Owner.Berserk = false;

            Stop();
        }
    }
}
