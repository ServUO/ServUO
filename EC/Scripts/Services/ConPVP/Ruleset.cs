using System;
using System.Collections;

namespace Server.Engines.ConPVP
{
    public class Ruleset
    {
        private readonly RulesetLayout m_Layout;
        private readonly ArrayList m_Flavors = new ArrayList();
        private BitArray m_Options;
        private string m_Title;
        private Ruleset m_Base;
        private bool m_Changed;
        public Ruleset(RulesetLayout layout)
        {
            this.m_Layout = layout;
            this.m_Options = new BitArray(layout.TotalLength);
        }

        public RulesetLayout Layout
        {
            get
            {
                return this.m_Layout;
            }
        }
        public BitArray Options
        {
            get
            {
                return this.m_Options;
            }
        }
        public string Title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
            }
        }
        public Ruleset Base
        {
            get
            {
                return this.m_Base;
            }
        }
        public ArrayList Flavors
        {
            get
            {
                return this.m_Flavors;
            }
        }
        public bool Changed
        {
            get
            {
                return this.m_Changed;
            }
            set
            {
                this.m_Changed = value;
            }
        }
        public void ApplyDefault(Ruleset newDefault)
        {
            this.m_Base = newDefault;
            this.m_Changed = false;

            this.m_Options = new BitArray(newDefault.m_Options);

            this.ApplyFlavorsTo(this);
        }

        public void ApplyFlavorsTo(Ruleset ruleset)
        {
            for (int i = 0; i < this.m_Flavors.Count; ++i)
            {
                Ruleset flavor = (Ruleset)this.m_Flavors[i];

                this.m_Options.Or(flavor.m_Options);
            }
        }

        public void AddFlavor(Ruleset flavor)
        {
            if (this.m_Flavors.Contains(flavor))
                return;

            this.m_Flavors.Add(flavor);
            this.m_Options.Or(flavor.m_Options);
        }

        public void RemoveFlavor(Ruleset flavor)
        {
            if (!this.m_Flavors.Contains(flavor))
                return;

            this.m_Flavors.Remove(flavor);
            this.m_Options.And(flavor.m_Options.Not());
            flavor.m_Options.Not();
        }

        public void SetOptionRange(string title, bool value)
        {
            RulesetLayout layout = this.m_Layout.FindByTitle(title);

            if (layout == null)
                return;

            for (int i = 0; i < layout.TotalLength; ++i)
                this.m_Options[i + layout.Offset] = value;

            this.m_Changed = true;
        }

        public bool GetOption(string title, string option)
        {
            int index = 0;
            RulesetLayout layout = this.m_Layout.FindByOption(title, option, ref index);

            if (layout == null)
                return true;

            return this.m_Options[layout.Offset + index];
        }

        public void SetOption(string title, string option, bool value)
        {
            int index = 0;
            RulesetLayout layout = this.m_Layout.FindByOption(title, option, ref index);

            if (layout == null)
                return;

            this.m_Options[layout.Offset + index] = value;

            this.m_Changed = true;
        }
    }
}