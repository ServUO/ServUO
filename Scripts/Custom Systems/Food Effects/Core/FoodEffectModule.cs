using System;
using Server;
using Server.Gumps;

namespace CustomsFramework.Systems.FoodEffects
{
    public class FoodEffectModule : BaseModule
    {
        private FoodEffect m_FoodEffect;
        private EffectTimer m_Timer;
        private BuffInfo m_BuffInfo;

        public FoodEffectModule(Mobile from) : base()
        {
            this.LinkMobile(from);

            BaseCore.OnEnabledChanged += BaseCore_OnEnabledChanged;
        }

        public FoodEffectModule(CustomSerial serial) : base(serial)
        {
            BaseCore.OnEnabledChanged += BaseCore_OnEnabledChanged;
        }

        public override String Name
        {
            get
            {
                if (this.LinkedMobile != null)
                    return String.Format("Food Effect Module - {0}", this.LinkedMobile.Name);
                else
                    return "Unlinked Food Effect Module";
            }
        }

        public override String Description
        {
            get
            {
                if (this.LinkedMobile != null)
                    return String.Format("Food Effect Module that is linked to {0}", this.LinkedMobile.Name);
                else
                    return "Unlinked Food Effect Module";
            }
        }

        public override String Version { get { return FoodEffectsCore.SystemVersion; } }
        public override AccessLevel EditLevel { get { return AccessLevel.Developer; } }
        public override Gump SettingsGump { get { return base.SettingsGump; } }

        public override void Prep()
        {
            base.Prep();
        }

        private void BaseCore_OnEnabledChanged(BaseCoreEventArgs e)
        {
            if (FoodEffectsCore.Core == e.Core && !e.Core.Enabled)
                EffectExpired(true);
        }

        public void ApplyEffect(FoodEffect effect)
        {
            ApplyEffect(effect, false);
        }

        public void ApplyEffect(FoodEffect effect, Boolean silent)
        {
            if (FoodEffectsCore.Core == null || !FoodEffectsCore.Core.Enabled || !Core.AOS)
            {
                EffectExpired(true);
                return;
            }

            if (LinkedMobile != null)
            {
                if (m_Timer != null)
                {
                    m_Timer.Stop();
                    m_Timer = null;
                }

                LinkedMobile.RemoveStatMod("Food-StrBonus");
                LinkedMobile.RemoveStatMod("Food-DexBonus");
                LinkedMobile.RemoveStatMod("Food-IntBonus");

                if (m_BuffInfo != null)
                    BuffInfo.RemoveBuff(LinkedMobile, m_BuffInfo);

                if (m_FoodEffect != null)
                    FoodEffectsCore.InvokeOnEffectCanceled(LinkedMobile, m_FoodEffect);

                m_FoodEffect = new FoodEffect(effect.RegenHits, effect.RegenStam, effect.RegenMana, effect.StrBonus, effect.DexBonus, effect.IntBonus, effect.Duration);

                if (m_FoodEffect.StrBonus != 0)
                    LinkedMobile.AddStatMod(new StatMod(StatType.Str, "Food-StrBonus", m_FoodEffect.StrBonus, m_FoodEffect.EffectTimeSpan));

                if (m_FoodEffect.DexBonus != 0)
                    LinkedMobile.AddStatMod(new StatMod(StatType.Dex, "Food-DexBonus", m_FoodEffect.DexBonus, m_FoodEffect.EffectTimeSpan));

                if (m_FoodEffect.IntBonus != 0)
                    LinkedMobile.AddStatMod(new StatMod(StatType.Int, "Food-IntBonus", m_FoodEffect.IntBonus, m_FoodEffect.EffectTimeSpan));

                if (!silent)
                {
                    LinkedMobile.FixedEffect(0x375A, 10, 15);
                    LinkedMobile.PlaySound(0x1EE);

                    LinkedMobile.SendMessage(12, "The food you ate is now affecting your performance...");
                }

                m_Timer = new EffectTimer(this);

                FoodEffectsCore.InvokeOnEffectActivated(LinkedMobile, m_FoodEffect);

                m_BuffInfo = new BuffInfo(BuffIcon.ActiveMeditation, 1074240, 1114057, m_FoodEffect.EffectTimeSpan, LinkedMobile, m_FoodEffect.GetBuffInfoText());
                BuffInfo.AddBuff(LinkedMobile, m_BuffInfo);
            }
        }

        public void EffectExpired()
        {
            EffectExpired(false);
        }

        public void EffectExpired(Boolean silent)
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_FoodEffect != null)
            {
                if (LinkedMobile != null)
                {
                    LinkedMobile.RemoveStatMod("Food-StrBonus");
                    LinkedMobile.RemoveStatMod("Food-DexBonus");
                    LinkedMobile.RemoveStatMod("Food-IntBonus");

                    if (m_BuffInfo != null)
                        BuffInfo.RemoveBuff(LinkedMobile, m_BuffInfo);

                    if (!silent)
                    {
                        LinkedMobile.PlaySound(0x1E6);

                        LinkedMobile.SendMessage(12, "Your food effect has worn off");
                    }

                    if (m_FoodEffect != null)
                        FoodEffectsCore.InvokeOnEffectExpired(LinkedMobile, m_FoodEffect);
                }

                m_FoodEffect = null;
            }
        }

        public Int32 GetRegenModifier(FoodEffectRegenType regenType)
        {
            if (m_FoodEffect == null)
                return 0;

            switch(regenType)
            {
                case FoodEffectRegenType.Hits:
                    return m_FoodEffect.RegenHits;
                case FoodEffectRegenType.Stam:
                    return m_FoodEffect.RegenStam;
                default:
                    return m_FoodEffect.RegenMana;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);

            // Version 0

            if (m_FoodEffect == null)
                writer.Write((Boolean)false);
            else
            {
                writer.Write((Boolean)true);
                writer.Write((Int32)m_FoodEffect.RegenHits);
                writer.Write((Int32)m_FoodEffect.RegenStam);
                writer.Write((Int32)m_FoodEffect.RegenMana);
                writer.Write((Int32)m_FoodEffect.StrBonus);
                writer.Write((Int32)m_FoodEffect.DexBonus);
                writer.Write((Int32)m_FoodEffect.IntBonus);
                writer.Write((Int32)((m_FoodEffect.Added.AddMinutes(m_FoodEffect.Duration)).Subtract(DateTime.UtcNow)).TotalSeconds);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            Int32 version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    // Version 0

                    if (reader.ReadBool())
                        m_FoodEffect = new FoodEffect(reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), (reader.ReadInt() / 60) + 1);

                    if (m_FoodEffect != null)
                        ApplyEffect(m_FoodEffect, true);

                    break;
            }
        }

        private class EffectTimer : Timer
        {
            private readonly FoodEffectModule m_Module;

            public EffectTimer(FoodEffectModule module) : base(module.m_FoodEffect.EffectTimeSpan)
            {
                this.Priority = TimerPriority.FiftyMS;
                this.m_Module = module;
                this.Start();
            }

            protected override void OnTick()
            {
                m_Module.EffectExpired();
            }
        }
    }
}
