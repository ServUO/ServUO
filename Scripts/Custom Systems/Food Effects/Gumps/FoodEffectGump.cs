using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Items;

namespace CustomsFramework.Systems.FoodEffects
{
    public class FoodEffectGump : Gump
    {
        private const Int32 HUE_ValidEntry = 0;
        private const Int32 HUE_InvalidEntry = 0x22;

        private Boolean _CoreEnabled;
        private Int32 _EffectIndex;

        private List<Type> _FoodTypes = new List<Type>();
        private List<FoodEffect> _FoodEffects = new List<FoodEffect>();

        private String[] _Values;

        public FoodEffectGump(Boolean coreEnabled, List<Type> foodTypes, List<FoodEffect> foodEffects, Int32 effectIndex, String[] values, Boolean validate)
            : base(150, 100)
        {
            _CoreEnabled = coreEnabled;
            _FoodTypes = foodTypes;
            _FoodEffects = foodEffects;
            _EffectIndex = effectIndex;
            _Values = values;

            if (_Values == null)
            {
                _Values = new String[] { "", "0", "0", "0", "0", "0", "0", "0" };

                if (_EffectIndex < _FoodTypes.Count)
                {
                    _Values[0] = _FoodTypes[_EffectIndex].Name;
                    _Values[1] = _FoodEffects[_EffectIndex].RegenHits.ToString();
                    _Values[2] = _FoodEffects[_EffectIndex].RegenStam.ToString();
                    _Values[3] = _FoodEffects[_EffectIndex].RegenMana.ToString();
                    _Values[4] = _FoodEffects[_EffectIndex].StrBonus.ToString();
                    _Values[5] = _FoodEffects[_EffectIndex].DexBonus.ToString();
                    _Values[6] = _FoodEffects[_EffectIndex].IntBonus.ToString();
                    _Values[7] = _FoodEffects[_EffectIndex].Duration.ToString();
                }
            }

            Add(new StoneyBackground(360, 220));
            Add(new FoodEffectGumpling(10, 6, 240, "Food Type", (validate || IsFoodType(_Values[0]) ? HUE_ValidEntry : HUE_InvalidEntry), _Values[0]));
            Add(new FoodEffectGumpling(10, 28, 50, "Hit Point Regen", (validate || IsValidNumber(_Values[1], true) ? HUE_ValidEntry : HUE_InvalidEntry), _Values[1]));
            Add(new FoodEffectGumpling(10, 50, 50, "Stamina Regen", (validate || IsValidNumber(_Values[2], true) ? HUE_ValidEntry : HUE_InvalidEntry), _Values[2]));
            Add(new FoodEffectGumpling(10, 72, 50, "Mana Regen", (validate || IsValidNumber(_Values[3], true) ? HUE_ValidEntry : HUE_InvalidEntry), _Values[3]));
            Add(new FoodEffectGumpling(10, 94, 50, "STR Bonus", (validate || IsValidNumber(_Values[4], true) ? HUE_ValidEntry : HUE_InvalidEntry), _Values[4]));
            Add(new FoodEffectGumpling(10, 116, 50, "DEX Bonus", (validate || IsValidNumber(_Values[5], true) ? HUE_ValidEntry : HUE_InvalidEntry), _Values[5]));
            Add(new FoodEffectGumpling(10, 138, 50, "INT Bonus", (validate || IsValidNumber(_Values[6], true) ? HUE_ValidEntry : HUE_InvalidEntry), _Values[6]));
            Add(new FoodEffectGumpling(10, 160, 50, "Duration", (validate || IsValidNumber(_Values[7], true) ? HUE_ValidEntry : HUE_InvalidEntry), _Values[7]));
            Add(new GumpLabel(170, 53, 0x0, "1 Regen = 0.1 / Sec"));
            Add(new GumpLabel(165, 163, 0x0, "Minutes"));

            if (validate)
            {
                if (!IsFoodType(_Values[0]))
                    Add(new GumpLabel(165, 31, HUE_InvalidEntry, "Food Type must be a valid Food"));

                Boolean valid = true;

                for (Int32 i = 1; i < 8; i++)
                    valid &= IsValidNumber(_Values[7], (i < 7));

                if (!valid)
                    Add(new GumpLabel(165, 119, HUE_InvalidEntry, "Invalid value(s) entered"));
            }

            Add(new ApplyCancelGumpling(190, 190, ApplyButtonPressed, CancelButtonPressed));
        }

        public override void OnAddressChange()
        {
            if (Address != null)
            {
                Address.CloseGump(typeof(FoodEffectsSetupGump));
                Address.CloseGump(typeof(FoodEffectGump));
            }
        }

        public void ApplyButtonPressed(IGumpComponent sender, object param)
        {
            String[] values = new String[]
            {
                GetTextEntry("Food Type"),
                GetTextEntry("Hit Point Regen"),
                GetTextEntry("Stamina Regen"),
                GetTextEntry("Mana Regen"),
                GetTextEntry("STR Bonus"),
                GetTextEntry("DEX Bonus"),
                GetTextEntry("INT Bonus"),
                GetTextEntry("Duration")
            };

            Boolean valid = true;

            valid &= IsFoodType(values[0]);

            for (Int32 i = 1; i < 8; i++)
                valid &= IsValidNumber(values[i], (i < 7));

            if (valid)
            {
                Int32[] parsedValues = new Int32[] { 0, 0, 0, 0, 0, 0, 0 };

                for (Int32 i = 1; i < 8; i++)
                    Int32.TryParse(values[i], out parsedValues[i - 1]);

                FoodEffect effect = new FoodEffect(parsedValues[0], parsedValues[1], parsedValues[2], parsedValues[3], parsedValues[4], parsedValues[5], parsedValues[6]);

                Type type = ScriptCompiler.FindTypeByName(values[0]);

                if (_EffectIndex < _FoodTypes.Count)
                {
                    _FoodTypes[_EffectIndex] = type;
                    _FoodEffects[_EffectIndex] = effect;
                }
                else
                {
                    _FoodTypes.Add(type);
                    _FoodEffects.Add(effect);
                }

                if (Address != null)
                    Address.SendGump(new FoodEffectsSetupGump(_CoreEnabled, _FoodTypes, _FoodEffects, (_EffectIndex / 10) * 10));
            }
            else if (Address != null)
                Address.SendGump(new FoodEffectGump(_CoreEnabled, _FoodTypes, _FoodEffects, _EffectIndex, values, true));
        }

        public void CancelButtonPressed(IGumpComponent sender, object param)
        {
            if (Address != null)
                Address.SendGump(new FoodEffectsSetupGump(_CoreEnabled, _FoodTypes, _FoodEffects, (_EffectIndex / 10) * 10));
        }

        private Boolean IsFoodType(String value)
        {
            Type typeCheck = typeof(Food);
            Type type = ScriptCompiler.FindTypeByName(value);

            Boolean valid = type != null || value == "";

            if (typeCheck != null && type != null)
                valid &= typeCheck.IsAssignableFrom(type);

            return valid;
        }

        private Boolean IsValidNumber(String value, Boolean allowNegatives)
        {
            Boolean valid = true;
            Int32 test;

            valid = Int32.TryParse(value, out test);

            if (!allowNegatives && test < 0)
                valid = false;

            return valid;
        }
    }
}