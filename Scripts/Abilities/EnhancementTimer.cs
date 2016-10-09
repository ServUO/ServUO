// Created by Peoharen
using System;
using System.Collections;

namespace Server
{
    public class EnhancementTimer : Timer
    {
        private readonly ArrayList AL = new ArrayList();
        private readonly Mobile m_Mobile;
        private readonly string m_Title;
        private int m_Duration;

        public EnhancementTimer(Mobile m, int duration, string title, params object[] args)
            : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
        {
            if (args.Length < 1 || (args.Length % 2) != 0)
                throw new Exception("EnhancementTimer: args.length must be an even number greater than 0");

            Enhancement.AddMobile(m);
            this.m_Mobile = m;
            this.m_Title = title;
            this.m_Duration = duration;

            AosAttribute att;
            AosWeaponAttribute weapon;
            AosArmorAttribute armor;
            SAAbsorptionAttribute absorb;
            int number = 0;

            for (int i = 0; i < args.Length - 1; i += 2)
            {
                if (!(args[i + 1] is int))
                    throw new Exception("EnhancementTimer: The second value must be an integer");

                number = (int)args[i + 1];

                if (args[i] is AosAttribute)
                {
                    att = (AosAttribute)args[i];
                    Enhancement.SetValue(m, att, (Enhancement.GetValue(m, att) + number), this.m_Title);
                    this.AL.Add(att);
                    this.AL.Add(number);
                }
                else if (args[i] is AosWeaponAttribute)
                {
                    weapon = (AosWeaponAttribute)args[i];
                    Enhancement.SetValue(m, weapon, (Enhancement.GetValue(m, weapon) + number), this.m_Title);
                    this.AL.Add(weapon);
                    this.AL.Add(number);
                }
                else if (args[i] is AosArmorAttribute)
                {
                    armor = (AosArmorAttribute)args[i];
                    Enhancement.SetValue(m, armor, (Enhancement.GetValue(m, armor) + number), this.m_Title);
                    this.AL.Add(armor);
                    this.AL.Add(number);
                }
                else if (args[i] is SAAbsorptionAttribute)
                {
                    absorb = (SAAbsorptionAttribute)args[i];
                    Enhancement.SetValue(m, absorb, (Enhancement.GetValue(m, absorb) + number), this.m_Title);
                    this.AL.Add(absorb);
                    this.AL.Add(number);
                }
            }
        }

        public void End()
        {
            if (Enhancement.EnhancementList.ContainsKey(this.m_Mobile))
            {
                AosAttribute att;
                AosWeaponAttribute weapon;
                AosArmorAttribute armor;
                SAAbsorptionAttribute absorb;
                int number = 0;

                for (int i = 0; i < this.AL.Count - 1; i += 2)
                {
                    number = (int)this.AL[i + 1];

                    if (this.AL[i] is AosAttribute)
                    {
                        att = (AosAttribute)this.AL[i];
                        Enhancement.SetValue(this.m_Mobile, att, (Enhancement.GetValue(this.m_Mobile, att) - number), this.m_Title);
                    }
                    else if (this.AL[i] is AosWeaponAttribute)
                    {
                        weapon = (AosWeaponAttribute)this.AL[i];
                        Enhancement.SetValue(this.m_Mobile, weapon, (Enhancement.GetValue(this.m_Mobile, weapon) - number), this.m_Title);
                    }
                    else if (this.AL[i] is AosArmorAttribute)
                    {
                        armor = (AosArmorAttribute)this.AL[i];
                        Enhancement.SetValue(this.m_Mobile, armor, (Enhancement.GetValue(this.m_Mobile, armor) - number), this.m_Title);
                    }
                    else if (this.AL[i] is SAAbsorptionAttribute)
                    {
                        absorb = (SAAbsorptionAttribute)this.AL[i];
                        Enhancement.SetValue(this.m_Mobile, absorb, (Enhancement.GetValue(this.m_Mobile, absorb) - number), this.m_Title);
                    }
                }
            }

            this.Stop();
        }

        protected override void OnTick()
        {
            this.m_Duration--;

            if (this.m_Mobile == null)
                this.Stop();

            if (this.m_Duration < 0)
            {
                this.End();
            }
        }
    }
}