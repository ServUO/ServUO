using System;
using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Mobiles;

namespace CustomsFramework.Systems.SlayerTitleSystem
{
    public class TitleDefinitionGump : Gump
    {
        private const Int32 HUE_ValidEntry = 0;
        private const Int32 HUE_InvalidEntry = 0x22;

        private Boolean _CoreEnabled;
        private Int32 _TitleIndex;

        private List<TitleDefinition> _TitleDefinitions = new List<TitleDefinition>();

        private List<String> _CreatureNames = new List<string>();
        private List<String> _Titles = new List<string>();
        private List<String> _Counts = new List<string>();

        public TitleDefinitionGump(Boolean coreEnabled, List<TitleDefinition> titleDefinitions, Int32 titleIndex, String definitionName, Boolean definitionEnabled, List<String> creatureNames, List<String> titles, List<String> counts, Boolean validate)
            : base(50, 100)
        {
            _CoreEnabled = coreEnabled;
            _TitleDefinitions = titleDefinitions;
            _TitleIndex = titleIndex;
            _CreatureNames = creatureNames;
            _Titles = titles;
            _Counts = counts;

            TitleDefinition definition;

            if (_TitleIndex < _TitleDefinitions.Count)
                definition = _TitleDefinitions[_TitleIndex];
            else
                definition = new TitleDefinition("", false, new List<Type>(), new List<TitleEntry>());

            if (_CreatureNames == null)
            {
                _CreatureNames = new List<string>();
                _Titles = new List<string>();
                _Counts = new List<string>();

                foreach (Type t in definition.CreatureRegistry)
                    _CreatureNames.Add(t.Name);

                foreach (TitleEntry entry in definition.TitleRegistry)
                {
                    _Titles.Add(entry.Title);
                    _Counts.Add(entry.CountNeeded.ToString());
                }
            }

            Add(new StoneyBackground(730, 470));
            Add(new GumpLabel(10, 5, 0x4F1, "Title Definition Name"));
            Add(new EntryField(150, 4, 570, "Definition Name", 0x0, (definitionName == null ? definition.DefinitionName : definitionName)));

            if (validate && (definitionName == null ? definition.DefinitionName : definitionName) == "")
                Add(new GumpLabel(100, 30, HUE_InvalidEntry, "Title Definition Name must be entered"));

            Add(new GumpLabel(10, 30, 0x54F, "Enabled"));
            Add(new GreyCheckbox(63, 25, "Definition Enabled", (definitionName == null ? definition.Enabled : definitionEnabled)));
            Add(new GumpImageTiled(2, 55, 722, 4, 0x13ED));
            Add(new GumpLabel(300, 60, 0x3E, "Creature Registry"));

            Type typeCheck = typeof(BaseCreature);
            Boolean creaturesValid = true;

            for (int col = 0; col < 2; col++)
            {
                for (int row = 0; row < 10; row++)
                {
                    Int32 idx = row + (col * 10);
                    String name = "";

                    if (idx < _CreatureNames.Count)
                        name = _CreatureNames[idx];

                    Type type = ScriptCompiler.FindTypeByName(name);

                    Boolean valid = type != null || name == "";

                    if (typeCheck != null && type != null)
                        valid &= typeCheck.IsAssignableFrom(type);

                    creaturesValid &= valid;

                    Add(new EntryField(10 + (col * 360), 80 + (row * 22), 350, String.Format("Creature_{0}", idx), (valid ? HUE_ValidEntry : HUE_InvalidEntry), name));
                }
            }

            if (!creaturesValid)
                Add(new GumpLabel(10, 60, HUE_InvalidEntry, "Invalid BaseCreature Type Specified"));

            Boolean titlesValid = true;

            Add(new GumpLabel(314, 305, 0x3E, "Title Registry"));

            for (int col = 0; col < 2; col++)
            {
                for (int row = 0; row < 5; row++)
                {
                    Int32 idx = row + (col * 5);
                    String title = "";
                    String count = "";

                    if (idx < _Titles.Count)
                        title = _Titles[idx];

                    if (idx < _Counts.Count)
                        count = _Counts[idx];

                    if (title != "" && count == "")
                        count = "0";

                    Int32 test;
                    Boolean countValid = Int32.TryParse(count, out test);

                    if (title != "" && count == "0")
                        countValid = false;

                    if (test < 0)
                        countValid = false;

                    if (title == "" && count == "")
                        countValid = true;

                    titlesValid &= countValid;

                    Add(new EntryField(10 + (col * 360), 325 + (row * 22), 295, String.Format("Title_{0}", idx), HUE_ValidEntry, title));
                    Add(new EntryField(310 + (col * 360), 325 + (row * 22), 50, String.Format("Count_{0}", idx), (countValid ? HUE_ValidEntry : HUE_InvalidEntry), count));
                }
            }

            if (!titlesValid)
                Add(new GumpLabel(10, 305, HUE_InvalidEntry, "Invalid Count Value Specified"));

            Add(new ApplyCancelGumpling(560, 440, ApplyButtonPressed, CancelButtonPressed));
        }

        public override void OnAddressChange()
        {
            if (Address != null)
            {
                Address.CloseGump(typeof(SlayerTitleSetupGump));
                Address.CloseGump(typeof(TitleDefinitionGump));
            }
        }

        public void ApplyButtonPressed(IGumpComponent sender, object param)
        {
            String titleName = GetTextEntry("Definition Name");
            Boolean enabled = GetCheck("Definition Enabled");

            if (DataIsValid)
            {
                List<Type> creatureRegistry = new List<Type>();
                List<TitleEntry> titleRegistry = new List<TitleEntry>();

                for (int i = 0; i < 20; i++)
                {
                    string name = GetTextEntry(String.Format("Creature_{0}", i));

                    Type type = ScriptCompiler.FindTypeByName(name);

                    if (type != null)
                        creatureRegistry.Add(type);
                }

                for (int i = 0; i < 10; i++)
                {
                    string title = GetTextEntry(String.Format("Title_{0}", i));
                    string count = GetTextEntry(String.Format("Count_{0}", i));

                    Int32 cnt = 0;
                    Int32.TryParse(count, out cnt);

                    if (title != "" && cnt > 0)
                        titleRegistry.Add(new TitleEntry(title, cnt));
                }

                if (_TitleIndex < _TitleDefinitions.Count)
                    _TitleDefinitions[_TitleIndex] = new TitleDefinition(titleName, enabled, creatureRegistry, titleRegistry);
                else
                    _TitleDefinitions.Add(new TitleDefinition(titleName, enabled, creatureRegistry, titleRegistry));

                if (Address != null)
                    Address.SendGump(new SlayerTitleSetupGump(_CoreEnabled, _TitleDefinitions, (_TitleIndex / 10) * 10));
            }
            else
            {
                if (Address != null)
                    Address.SendGump(new TitleDefinitionGump(_CoreEnabled, _TitleDefinitions, _TitleIndex, titleName, enabled, _CreatureNames, _Titles, _Counts, true));
            }
        }

        public void CancelButtonPressed(IGumpComponent sender, object param)
        {
            if (Address != null)
                Address.SendGump(new SlayerTitleSetupGump(_CoreEnabled, _TitleDefinitions, (_TitleIndex / 10) * 10));
        }

        private Boolean DataIsValid
        {
            get
            {
                bool valid = true;
                Type typeCheck = typeof(BaseCreature);

                _CreatureNames.Clear();
                _Titles.Clear();
                _Counts.Clear();

                if (GetTextEntry("Definition Name") == "")
                    valid = false;

                for (int i = 0; i < 20; i++)
                {
                    string name = GetTextEntry(String.Format("Creature_{0}", i));

                    Type type = ScriptCompiler.FindTypeByName(name);

                    if (name != "" && type == null)
                        valid = false;

                    if (typeCheck != null && type != null)
                        valid &= typeCheck.IsAssignableFrom(type);

                    if (name != "")
                        _CreatureNames.Add(name);
                }

                for (int i = 0; i < 10; i++)
                {
                    string title = GetTextEntry(String.Format("Title_{0}", i));
                    string count = GetTextEntry(String.Format("Count_{0}", i));

                    if (title == "")
                        count = "";
                    else if (count == "")
                        count = "0";

                    if (title != "" && count != "")
                    {
                        Int32 test;

                        if (!Int32.TryParse(count, out test))
                            valid = false;

                        if (title != "" && (count == "0" || test < 0))
                            valid = false;

                        _Titles.Add(title);
                        _Counts.Add(count);
                    }
                }

                return valid;
            }
        }
    }
}