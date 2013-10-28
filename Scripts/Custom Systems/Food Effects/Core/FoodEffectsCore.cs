using System;
using System.Collections.Generic;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Misc;

namespace CustomsFramework.Systems.FoodEffects
{
    public delegate Boolean FoodAllowanceHandler(Food food);

    public class FoodEffectsCore : BaseCore
    {
        #region Event Handlers
        public delegate void FoodEffectEventHandler(Mobile from, FoodEffect effect);

        public static event FoodEffectEventHandler OnEffectActivated;
        public static event FoodEffectEventHandler OnEffectCanceled;
        public static event FoodEffectEventHandler OnEffectExpired;
        public static event BaseCoreEventHandler OnFoodEffectSystemUpdate;

        public static void InvokeOnEffectActivated(Mobile from, FoodEffect effect)
        {
            if (OnEffectActivated != null)
                OnEffectActivated(from, effect);
        }

        public static void InvokeOnEffectCanceled(Mobile from, FoodEffect effect)
        {
            if (OnEffectCanceled != null)
                OnEffectCanceled(from, effect);
        }

        public static void InvokeOnEffectExpired(Mobile from, FoodEffect effect)
        {
            if (OnEffectExpired != null)
                OnEffectExpired(from, effect);
        }

        public static void InvokeOnFoodEffectSystemUpdate(FoodEffectsCore core)
        {
            if (OnFoodEffectSystemUpdate != null)
                OnFoodEffectSystemUpdate(new BaseCoreEventArgs(core));
        }
        #endregion

        private Dictionary<Type, FoodEffect> m_FoodEffects = new Dictionary<Type, FoodEffect>();
        public Dictionary<Type, FoodEffect> FoodEffects { get { return m_FoodEffects; } }

        public const String SystemVersion = "1.6";

        private static FoodEffectsCore m_Core;
        public static FoodEffectsCore Core { get { return m_Core; } }

        private FoodAllowanceHandler m_ShouldFoodBeAllowed;
        public FoodAllowanceHandler ShouldFoodBeAllowed { get { return m_ShouldFoodBeAllowed; } set { m_ShouldFoodBeAllowed = value; } }

        public FoodEffectsCore() : base()
        {
            this.Enabled = false;
        }

        public FoodEffectsCore(CustomSerial serial) : base(serial)
        {
        }

        public override String Name { get { return "Food Effect Core"; } }
        public override String Description { get { return "Core that contains everything for the Food Effect System."; } }
        public override String Version { get { return SystemVersion; } }
        public override AccessLevel EditLevel { get { return AccessLevel.Developer; } }
        public override Gump SettingsGump { get { return null; } }

        public static void Initialize()
        {
            FoodEffectsCore core = World.GetCore(typeof(FoodEffectsCore)) as FoodEffectsCore;

            if (core == null)
            {
                core = new FoodEffectsCore();
                core.Prep();

                core.FoodEffects.Add(typeof(BreadLoaf), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(Cookies), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(Cake), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(Muffins), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(Quiche), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(MeatPie), new FoodEffect(10, 10, 0, 5, 5, 0, 30));
                core.FoodEffects.Add(typeof(SausagePizza), new FoodEffect(10, 10, 0, 5, 5, 0, 30));
                core.FoodEffects.Add(typeof(CheesePizza), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(FruitPie), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(PeachCobbler), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(ApplePie), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(PumpkinPie), new FoodEffect(5, 5, 5, 0, 0, 0, 30));
                core.FoodEffects.Add(typeof(WasabiClumps), new FoodEffect(10, 10, 10, 0, 0, 0, 15));
                core.FoodEffects.Add(typeof(GreenTea), new FoodEffect(10, 10, 10, 0, 0, 0, 30));

                core.FoodEffects.Add(typeof(DarkChocolate), new FoodEffect(-5, -5, -5, 0, 0, 5, 20));
                core.FoodEffects.Add(typeof(MilkChocolate), new FoodEffect(-5, -5, -5, 0, 0, 10, 10));
                core.FoodEffects.Add(typeof(WhiteChocolate), new FoodEffect(-5, -5, -5, 0, 0, 20, 5));

                core.FoodEffects.Add(typeof(CookedBird), new FoodEffect(5, 5, 0, 5, 5, 0, 30));
                core.FoodEffects.Add(typeof(ChickenLeg), new FoodEffect(5, 5, 0, 5, 5, 0, 30));
                core.FoodEffects.Add(typeof(FriedEggs), new FoodEffect(5, 5, 0, 5, 5, 0, 30));
                core.FoodEffects.Add(typeof(LambLeg), new FoodEffect(5, 5, 0, 5, 5, 0, 30));
                core.FoodEffects.Add(typeof(Ribs), new FoodEffect(5, 5, 0, 5, 5, 0, 30));

                core.FoodEffects.Add(typeof(FishSteak), new FoodEffect(5, -2, 5, 5, 0, 5, 30));
                core.FoodEffects.Add(typeof(MisoSoup), new FoodEffect(10, -2, 10, 10, 0, 10, 20)); // 60
                core.FoodEffects.Add(typeof(WhiteMisoSoup), new FoodEffect(10, -2, 10, 10, 0, 10, 20)); // 60
                core.FoodEffects.Add(typeof(RedMisoSoup), new FoodEffect(10, -2, 10, 10, 0, 10, 20)); // 60
                core.FoodEffects.Add(typeof(AwaseMisoSoup), new FoodEffect(10, -2, 10, 10, 0, 10, 20)); // 60
                core.FoodEffects.Add(typeof(SushiRolls), new FoodEffect(10, 0, 10, 20, 0, 20, 10)); //90
                core.FoodEffects.Add(typeof(SushiPlatter), new FoodEffect(10, 0, 10, 20, 0, 20, 10)); //90
            }

            m_Core = core;

            CommandSystem.Register("FES", AccessLevel.Developer, new CommandEventHandler(FES_OnCommand));
        }

        // Called after all cores are loaded
        public override void Prep()
        {
            EventSink.OnConsume += EventSink_OnConsume;
            EventSink.PlayerDeath += EventSink_PlayerDeath;

            RegenRates.HitsBonusHandlers.Add(new RegenBonusHandler(GetHitsRegenModifier));
            RegenRates.StamBonusHandlers.Add(new RegenBonusHandler(GetStamRegenModifier));
            RegenRates.ManaBonusHandlers.Add(new RegenBonusHandler(GetManaRegenModifier));
        }

        [Usage("FES")]
        [Description("Displays the Food Effects System configuration gump.")]
        public static void FES_OnCommand(CommandEventArgs e)
        {
            if (m_Core == null)
                return;

            e.Mobile.SendGump(new FoodEffectsSetupGump());
        }

        private void EventSink_OnConsume(OnConsumeEventArgs e)
        {
            if (!this.Enabled || !Server.Core.AOS)
                return;

            FoodEffectModule module = e.Consumer.GetModule(typeof(FoodEffectModule)) as FoodEffectModule;

            if (module == null)
                module = new FoodEffectModule(e.Consumer);

            if (e.Consumed is Food)
            {
                Boolean effectAllowed = false;

                if (((Food)e.Consumed).PlayerConstructed)
                    effectAllowed = true;

                if (m_ShouldFoodBeAllowed != null && m_ShouldFoodBeAllowed((Food)e.Consumed))
                    effectAllowed = true;

                if (!m_FoodEffects.ContainsKey(e.Consumed.GetType()))
                    effectAllowed = false;

                if (effectAllowed)
                    module.ApplyEffect(m_FoodEffects[e.Consumed.GetType()]);
            }
        }

        private void EventSink_PlayerDeath(PlayerDeathEventArgs e)
        {
            if (!this.Enabled || !Server.Core.AOS)
                return;

            FoodEffectModule module = e.Mobile.GetModule(typeof(FoodEffectModule)) as FoodEffectModule;

            if (module != null)
                module.EffectExpired(true);
        }

        public Int32 GetHitsRegenModifier(Mobile m)
        {
            if (!Enabled || !Server.Core.AOS)
                return 0;

            FoodEffectModule module = m.GetModule(typeof(FoodEffectModule)) as FoodEffectModule;

            if (module == null)
                return 0;

            return module.GetRegenModifier(FoodEffectRegenType.Hits);
        }

        public Int32 GetStamRegenModifier(Mobile m)
        {
            if (!Enabled || !Server.Core.AOS)
                return 0;

            FoodEffectModule module = m.GetModule(typeof(FoodEffectModule)) as FoodEffectModule;

            if (module == null)
                return 0;

            return module.GetRegenModifier(FoodEffectRegenType.Hits);
        }

        public Int32 GetManaRegenModifier(Mobile m)
        {
            if (!Enabled || !Server.Core.AOS)
                return 0;

            FoodEffectModule module = m.GetModule(typeof(FoodEffectModule)) as FoodEffectModule;

            if (module == null)
                return 0;

            return module.GetRegenModifier(FoodEffectRegenType.Hits);
        }

        public static FoodEffect GetEffects(Food item)
        {
            if (!m_Core.Enabled || !Server.Core.AOS)
                return null;

            Boolean effectAllowed = false;

            if (item.PlayerConstructed)
                effectAllowed = true;

            if (m_Core.ShouldFoodBeAllowed != null && m_Core.ShouldFoodBeAllowed(item))
                effectAllowed = true;

            if (!m_Core.FoodEffects.ContainsKey(item.GetType()))
                effectAllowed = false;

            if (effectAllowed)
                return m_Core.FoodEffects[item.GetType()];
            else
                return null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 0);

            writer.Write((Int32)m_FoodEffects.Keys.Count);

            foreach (KeyValuePair<Type, FoodEffect> pair in m_FoodEffects)
            {
                writer.Write((String)pair.Key.Name);
                writer.Write((Int32)pair.Value.RegenHits);
                writer.Write((Int32)pair.Value.RegenStam);
                writer.Write((Int32)pair.Value.RegenMana);
                writer.Write((Int32)pair.Value.StrBonus);
                writer.Write((Int32)pair.Value.DexBonus);
                writer.Write((Int32)pair.Value.IntBonus);
                writer.Write((Int32)pair.Value.Duration);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            Int32 version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    Int32 count = reader.ReadInt();

                    for (Int32 i = 0; i < count; i++)
                    {
                        Type t = ScriptCompiler.FindTypeByName(reader.ReadString());
                        Int32 regenHits = reader.ReadInt();
                        Int32 regenStam = reader.ReadInt();
                        Int32 regenMana = reader.ReadInt();
                        Int32 strBonus = reader.ReadInt();
                        Int32 dexBonus = reader.ReadInt();
                        Int32 intBonus = reader.ReadInt();
                        Int32 duration = reader.ReadInt();

                        if (t != null)
                            m_FoodEffects.Add(t, new FoodEffect(regenHits, regenStam, regenMana, strBonus, dexBonus, intBonus, duration));
                    }

                    break;
            }
        }
    }
}
