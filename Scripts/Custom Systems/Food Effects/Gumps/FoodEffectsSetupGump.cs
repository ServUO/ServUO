using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;

namespace CustomsFramework.Systems.FoodEffects
{
    public class FoodEffectsSetupGump : Gump
    {
        private Boolean _CoreEnabled;
        private Int32 _EffectIndex;

        private List<Type> _FoodTypes = new List<Type>();
        private List<FoodEffect> _FoodEffects = new List<FoodEffect>();

        public FoodEffectsSetupGump()
            : base(150, 100)
        {
            if (FoodEffectsCore.Core == null)
                return;

            _CoreEnabled = FoodEffectsCore.Core.Enabled;
            _EffectIndex = 0;

            foreach (KeyValuePair<Type, FoodEffect> pair in FoodEffectsCore.Core.FoodEffects)
                _FoodTypes.Add(pair.Key);

            _FoodTypes.Sort(new TypeSorter());

            for (Int32 i = 0; i < _FoodTypes.Count; i++)
                _FoodEffects.Add(FoodEffectsCore.Core.FoodEffects[_FoodTypes[i]]);

            Setup();
        }

        public FoodEffectsSetupGump(Boolean coreEnabled, List<Type> foodTypes, List<FoodEffect> foodEffects, Int32 effectIndex)
            : base(150, 100)
        {
            _CoreEnabled = coreEnabled;
            _EffectIndex = effectIndex;
            _FoodTypes = foodTypes;
            _FoodEffects = foodEffects;

            Setup();
        }

        private void Setup()
        {
            Add(new CustomCoreGumpling(260, 336, 0x80E, "Food Effect System", _CoreEnabled, CoreEnableChanged, FoodEffectsCore.Core.Version, SaveButtonPressed));
            Add(new GumpLabel(64, 60, 0x3E, "Food Buffs"));

            List<Type> foodTypes = new List<Type>();

            Boolean lastEntryBlank = false;

            for (Int32 i = _EffectIndex; i < _FoodTypes.Count && i < _EffectIndex + 10; i++)
            {
                foodTypes.Add(_FoodTypes[i]);

                if (_FoodTypes[i] == null)
                    lastEntryBlank = true;
            }

            Boolean anotherPage = false;

            if (foodTypes.Count == 10)
                anotherPage = true;
            else if (!lastEntryBlank)
                foodTypes.Add(null);

            if (_EffectIndex > 0)
                Add(new GreyLeftArrow(10, 63, PreviousPagePressed));

            if (anotherPage)
                Add(new GreyRightArrow(172, 63, NextPagePressed));

            Int32 counter = _EffectIndex;

            for (Int32 i = 0; i < foodTypes.Count; i++)
            {
                LabelAddRemoveGumpling g = new LabelAddRemoveGumpling(counter++, 10, 80 + (i * 22), 180, (foodTypes[i] != null ? foodTypes[i].Name : ""));

                g.OnEdit += EditEffectPressed;
                g.OnRemove += RemoveEffectPressed;

                Add(g);
            }
        }

        public override void OnAddressChange()
        {
            if (Address != null)
            {
                Address.CloseGump(typeof(FoodEffectsSetupGump));
                Address.CloseGump(typeof(FoodEffectGump));
            }
        }

        private void CoreEnableChanged(IGumpComponent sender, object param)
        {
            if (param is Boolean)
                _CoreEnabled = (Boolean)param;
        }

        private void PreviousPagePressed(IGumpComponent sender, object param)
        {
            if (Address != null)
                Address.SendGump(new FoodEffectsSetupGump(_CoreEnabled, _FoodTypes, _FoodEffects, _EffectIndex - 10));
        }

        private void NextPagePressed(IGumpComponent sender, object param)
        {
            if (Address != null)
                Address.SendGump(new FoodEffectsSetupGump(_CoreEnabled, _FoodTypes, _FoodEffects, _EffectIndex + 10));
        }

        private void SaveButtonPressed(IGumpComponent sender, object param)
        {
            FoodEffectsCore.Core.Enabled = _CoreEnabled;

            FoodEffectsCore.Core.FoodEffects.Clear();

            for (Int32 i = 0; i < _FoodTypes.Count; i++)
                if (_FoodTypes[i] != null)
                    FoodEffectsCore.Core.FoodEffects[_FoodTypes[i]] = _FoodEffects[i];

            if (Address != null)
                Address.SendMessage("Food effect System is {0}!  System contains {1} foods defined.", (FoodEffectsCore.Core.Enabled ? "enabled" : "disabled"), FoodEffectsCore.Core.FoodEffects.Keys.Count);

            FoodEffectsCore.InvokeOnFoodEffectSystemUpdate(FoodEffectsCore.Core);
        }

        private void EditEffectPressed(IGumpComponent sender, object param)
        {
            if (sender.Container is LabelAddRemoveGumpling)
            {
                Int32 index = ((LabelAddRemoveGumpling)sender.Container).Index;

                if (Address != null)
                    Address.SendGump(new FoodEffectGump(_CoreEnabled, _FoodTypes, _FoodEffects, index, null, false));
            }
        }

        private void RemoveEffectPressed(IGumpComponent sender, object param)
        {
            if (sender.Container is LabelAddRemoveGumpling)
            {
                Int32 index = ((LabelAddRemoveGumpling)sender.Container).Index;

                if (index < _FoodTypes.Count)
                {
                    _FoodTypes.RemoveAt(index);
                    _FoodEffects.RemoveAt(index);
                }

                if (Address != null)
                    Address.SendGump(new FoodEffectsSetupGump(_CoreEnabled, _FoodTypes, _FoodEffects, _EffectIndex));
            }
        }

        private class TypeSorter : IComparer<Type>
        {
            public Int32 Compare(Type x, Type y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
    }
}