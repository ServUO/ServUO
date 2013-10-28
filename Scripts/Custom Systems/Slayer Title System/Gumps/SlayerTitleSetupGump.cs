using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;

namespace CustomsFramework.Systems.SlayerTitleSystem
{
    public class SlayerTitleSetupGump : Gump
    {
        private const Int32 HUE_Enabled = 0;
        private const Int32 HUE_Disabled = 0x22;

        private Boolean _CoreEnabled;
        private Int32 _TitleIndex;

        private List<TitleDefinition> _TitleDefinitions = new List<TitleDefinition>();

        public SlayerTitleSetupGump() : base(150, 100)
        {
            if (SlayerTitleCore.Core == null)
                return;

            _CoreEnabled = SlayerTitleCore.Core.Enabled;
            _TitleIndex = 0;

            foreach (TitleDefinition def in SlayerTitleCore.Core.TitleDefinitions)
                _TitleDefinitions.Add(new TitleDefinition(def.DefinitionName, def.Enabled, def.CreatureRegistry, def.TitleRegistry));

            Setup();
        }

        public SlayerTitleSetupGump(Boolean coreEnabled, List<TitleDefinition> titleDefinitions, Int32 titleIndex) : base(150, 100)
        {
            _CoreEnabled = coreEnabled;
            _TitleIndex = titleIndex;
            _TitleDefinitions = titleDefinitions;

            Setup();
        }

        private void Setup()
        {
            Add(new CustomCoreGumpling(365, 335, 0x80E, "Slayer Title System", _CoreEnabled, CoreEnableChanged, SlayerTitleCore.Core.Version, SaveButtonPressed));
            Add(new GumpLabel(94, 60, 0x3E, "Registered Title Definitions"));

            List<TitleDefinition> titles = new List<TitleDefinition>();

            Boolean lastEntryBlank = false;

            for (int i = _TitleIndex; i < _TitleDefinitions.Count && i < _TitleIndex + 10; i++)
            {
                titles.Add(_TitleDefinitions[i]);

                if (_TitleDefinitions[i].DefinitionName == "")
                    lastEntryBlank = true;
            }

            bool anotherPage = false;

            if (titles.Count == 10)
                anotherPage = true;
            else if (!lastEntryBlank)
                titles.Add(new TitleDefinition("", false, new List<Type>(), new List<TitleEntry>()));

            if (_TitleIndex > 0)
                Add(new GreyLeftArrow(10, 63, PreviousPagePressed));

            if (anotherPage)
                Add(new GreyRightArrow(277, 63, NextPagePressed));

            Int32 counter = _TitleIndex;

            for (int i = 0; i < titles.Count; i++)
            {
                TitleDefinition def = titles[i];

                LabelAddRemoveGumpling g = new LabelAddRemoveGumpling(counter++, 10, 80 + (i * 22), 285, def.DefinitionName, (def.Enabled ? HUE_Enabled : HUE_Disabled));

                g.OnEdit += EditDefinitionPressed;
                g.OnRemove += RemoveDefinitionPressed;

                Add(g);
            }
        }

        public override void OnAddressChange()
        {
            if (Address != null)
            {
                Address.CloseGump(typeof(SlayerTitleSetupGump));
                Address.CloseGump(typeof(TitleDefinitionGump));
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
                Address.SendGump(new SlayerTitleSetupGump(_CoreEnabled, _TitleDefinitions, _TitleIndex - 10));
        }

        private void NextPagePressed(IGumpComponent sender, object param)
        {
            if (Address != null)
                Address.SendGump(new SlayerTitleSetupGump(_CoreEnabled, _TitleDefinitions, _TitleIndex + 10));
        }

        private void SaveButtonPressed(IGumpComponent sender, object param)
        {
            SlayerTitleCore.Core.Enabled = _CoreEnabled;

            SlayerTitleCore.Core.TitleDefinitions.Clear();

            foreach (TitleDefinition def in _TitleDefinitions)
                SlayerTitleCore.Core.TitleDefinitions.Add(def);

            SlayerTitleCore.Core.CrossReferenceDefinitions();

            if (Address != null)
                Address.SendMessage("Slayer Title System is {0}!  System contains {1} title definitions.", (SlayerTitleCore.Core.Enabled ? "enabled" : "disabled"), SlayerTitleCore.Core.TitleDefinitions.Count);
        }

        private void EditDefinitionPressed(IGumpComponent sender, object param)
        {
            if (sender.Container is LabelAddRemoveGumpling)
            {
                Int32 index = ((LabelAddRemoveGumpling)sender.Container).Index;

                if (Address != null)
                    Address.SendGump(new TitleDefinitionGump(_CoreEnabled, _TitleDefinitions, index, null, false, null, null, null, false));
            }
        }

        private void RemoveDefinitionPressed(IGumpComponent sender, object param)
        {
            if (sender.Container is LabelAddRemoveGumpling)
            {
                Int32 index = ((LabelAddRemoveGumpling)sender.Container).Index;

                if (index < _TitleDefinitions.Count)
                    _TitleDefinitions.RemoveAt(index);

                if (Address != null)
                    Address.SendGump(new SlayerTitleSetupGump(_CoreEnabled, _TitleDefinitions, _TitleIndex));
            }
        }
    }
}
