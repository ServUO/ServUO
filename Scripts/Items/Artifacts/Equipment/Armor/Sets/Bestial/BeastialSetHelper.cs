using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public class BerserkImpl : Berserk
    {
        [CallPriority(10)]
        public static void Configure()
        {   
            Register(new BerserkImpl("SetBerserk", 0));
        }

        private bool m_Active;
        private bool m_IsTempBody;
        private int m_TempBodyHue;
        private readonly string m_Name;
        private readonly int m_Level;
        private List<Item> m_EquipBestial;

        public override bool Active { get { return m_Active; } set { m_Active = value; } }
        public override bool IsTempBody { get { return m_IsTempBody; } set { m_IsTempBody = value; } }
        public override int TempBodyColor { get { return m_TempBodyHue; } set { m_TempBodyHue = value; } }
        public override string Name { get { return m_Name; } }
        public override int Level { get { return m_Level; } }
        public override List<Item> EquipBestial { get { return m_EquipBestial; } set { m_EquipBestial = value; } }
        public override bool FullBestialEquip { get { return m_EquipBestial.Count() == 4; } }

        public BerserkImpl(string name, int level)
        {
            m_Name = name;
            m_Level = level;
        }

        public override Timer ConstructTimer(Mobile m)
        {
            return new BerserkTimer(m, this);
        }

        public override void OnRemoveEffect(Timer t)
        {
            ((BerserkTimer)t).RemoveEffect();
        }

        public static bool CheckBestialArmor(Mobile m)
        {
            return m.Items.Where(i => i != null && i is ISetItem && ((ISetItem)i).SetID == SetItem.Bestial && i.Parent is Mobile && ((Mobile)i.Parent).FindItemOnLayer(i.Layer) == i) != null;
        }

        public static void CheckEquipBestial(Mobile m)
        {
            if (m.Berserk.EquipBestial != null)
                m.Berserk.EquipBestial.Clear();

            m.Berserk.EquipBestial = m.Items.Where(i => i != null && i is ISetItem && ((ISetItem)i).SetID == SetItem.Bestial && i.Parent is Mobile && ((Mobile)i.Parent).FindItemOnLayer(i.Layer) == i).ToList();
        }

        public static int AddBestialHueParent(Mobile m)
        {
            int color = m.Berserk.EquipBestial.FirstOrDefault().Hue;

            CheckEquipBestial(m);           
            
            if (m.Berserk.FullBestialEquip)
            {
                if (m.HueMod != -1)
                {
                    if (!m.Berserk.IsTempBody)
                    {
                        m.Berserk.TempBodyColor = m.HueMod;
                        m.Berserk.IsTempBody = true;
                    }
                }

                m.HueMod = color;                
            }

            return color;
        }

        public static void DropBestialHueParent(Mobile m)
        {
            if (m.Berserk.IsTempBody)
            {
                m.HueMod = m.Berserk.TempBodyColor;
                m.Berserk.IsTempBody = false;
            }
            else
            {
                m.HueMod = -1;
            }
        }

        public class BerserkTimer : Timer
        {
            private readonly BerserkImpl m_Berserk;
            private readonly Mobile m_Mobile;
            private int m_Count = 0;
            private bool msg;
            private const int MaxCount = 9;
            
            public BerserkTimer(Mobile m, BerserkImpl p)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Mobile = m;
                m_Berserk = p;

                CheckEquipBestial(m_Mobile);

                if (!m_Berserk.Active)
                {
                    m_Mobile.SendLocalizedMessage(1151532); //You enter a berserk rage!
                    m_Berserk.Active = true;
                    
                    m_Berserk.EquipBestial.ForEach(k => k.Hue = 1255);

                    if (m_Mobile.HueMod != -1)
                    {
                        m_Berserk.TempBodyColor = m_Mobile.HueMod;
                        m_Berserk.IsTempBody = true;
                    }

                    if (m_Berserk.FullBestialEquip)
                        m_Mobile.HueMod = 1255; 
                }
                else
                {
                    msg = false;

                    foreach (var item in m_Berserk.EquipBestial.Where(i => i.Hue <= 1260))
                    {
                        item.Hue++;

                        if (!msg)
                        {
                            m_Mobile.SendLocalizedMessage(1151533, "", item.Hue); //Your rage grows!

                            if (m_Berserk.FullBestialEquip)
                                m_Mobile.HueMod++;

                            msg = true;
                        }
                    }
                }
            }

            protected override void OnTick()
            {
                if (!m_Mobile.Alive)
                    RemoveEffect();

                m_Count++;

                CheckEquipBestial(m_Mobile);

                if (m_Count >= MaxCount)
                {
                    RemoveEffect();
                }
                else
                {
                    if (m_Count % 3 == 0)
                    {
                        msg = false;

                        foreach (var item in m_Berserk.EquipBestial.Where(i => i.Hue > 1255))
                        {
                            item.Hue--;

                            if (!msg)
                            {
                                m_Mobile.SendLocalizedMessage(1151534, "", item.Hue); //Your rage recedes.

                                if (m_Berserk.FullBestialEquip)
                                    m_Mobile.HueMod--;

                                msg = true;
                            }
                        }
                    }
                }
            }

            public void RemoveEffect()
            {
                Stop();

                CheckEquipBestial(m_Mobile);
                
                m_Berserk.EquipBestial.ForEach(k => k.Hue = 2010);

                m_Berserk.Active = false;
                m_Mobile.SendLocalizedMessage(1151535); //Your berserk rage has subsided.               

                if (m_Berserk.IsTempBody)
                {
                    m_Mobile.HueMod = m_Berserk.TempBodyColor;
                    m_Berserk.IsTempBody = false;
                }
                else
                {
                    m_Mobile.HueMod = -1;
                }

                m_Mobile.Berserk = null;               
            }            
        }

        public class GargoyleBerserkTimer : Timer
        {
            private Mobile m_Mobile;

            public GargoyleBerserkTimer(Mobile m)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Mobile = m;

                m_Mobile.PlaySound(0x20F);
                m_Mobile.PlaySound(m_Mobile.Body.IsFemale ? 0x338 : 0x44A);
                m_Mobile.FixedParticles(0x376A, 1, 31, 9961, 1160, 0, EffectLayer.Waist);
                m_Mobile.FixedParticles(0x37C4, 1, 31, 9502, 43, 2, EffectLayer.Waist);

                BuffInfo.AddBuff(m_Mobile, new BuffInfo(BuffIcon.Berserk, 1080449, 1115021, "15\t3", false));

                m_Mobile.GargoyleBerserk = true;
            }

            protected override void OnTick()
            {
                float percentage = (float)m_Mobile.Hits / m_Mobile.HitsMax;

                if (percentage >= 0.8 || !m_Mobile.Alive)
                    RemoveEffect();
            }

            public void RemoveEffect()
            {
                m_Mobile.PlaySound(0xF8);
                BuffInfo.RemoveBuff(m_Mobile, BuffIcon.Berserk);

                m_Mobile.GargoyleBerserk = false;

                Stop();
            }
        }
    }
}