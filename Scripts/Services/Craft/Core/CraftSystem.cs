using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Craft
{
    public enum CraftECA
    {
        ChanceMinusSixty,
        FiftyPercentChanceMinusTenPercent,
        ChanceMinusSixtyToFourtyFive
    }

    public abstract class CraftSystem
    {
        private readonly int m_MinCraftEffect;
        private readonly int m_MaxCraftEffect;
        private readonly double m_Delay;
        private bool m_Resmelt;
        private bool m_Repair;
        private bool m_MarkOption;
        private bool m_CanEnhance;
        #region SA
        private bool m_QuestOption;
		private bool m_CanAlter;
        #endregion

        private readonly CraftItemCol m_CraftItems;
        private readonly CraftGroupCol m_CraftGroups;
        private readonly CraftSubResCol m_CraftSubRes;
        private readonly CraftSubResCol m_CraftSubRes2;

        public int MinCraftEffect
        {
            get
            {
                return this.m_MinCraftEffect;
            }
        }
        public int MaxCraftEffect
        {
            get
            {
                return this.m_MaxCraftEffect;
            }
        }
        public double Delay
        {
            get
            {
                return this.m_Delay;
            }
        }

        public CraftItemCol CraftItems
        {
            get
            {
                return this.m_CraftItems;
            }
        }
        public CraftGroupCol CraftGroups
        {
            get
            {
                return this.m_CraftGroups;
            }
        }
        public CraftSubResCol CraftSubRes
        {
            get
            {
                return this.m_CraftSubRes;
            }
        }
        public CraftSubResCol CraftSubRes2
        {
            get
            {
                return this.m_CraftSubRes2;
            }
        }
		
        public abstract SkillName MainSkill { get; }

        public virtual int GumpTitleNumber
        {
            get
            {
                return 0;
            }
        }
        public virtual string GumpTitleString
        {
            get
            {
                return "";
            }
        }

        public virtual CraftECA ECA
        {
            get
            {
                return CraftECA.ChanceMinusSixty;
            }
        }

        private readonly Dictionary<Mobile, CraftContext> m_ContextTable = new Dictionary<Mobile, CraftContext>();

        public abstract double GetChanceAtMin(CraftItem item);

        public virtual bool RetainsColorFrom(CraftItem item, Type type)
        {
            return false;
        }

        public CraftContext GetContext(Mobile m)
        {
            if (m == null)
                return null;

            if (m.Deleted)
            {
                this.m_ContextTable.Remove(m);
                return null;
            }

            CraftContext c = null;
            this.m_ContextTable.TryGetValue(m, out c);

            if (c == null)
                this.m_ContextTable[m] = c = new CraftContext();

            return c;
        }

        public void OnMade(Mobile m, CraftItem item)
        {
            CraftContext c = this.GetContext(m);

            if (c != null)
                c.OnMade(item);
        }

        public bool Resmelt
        {
            get
            {
                return this.m_Resmelt;
            }
            set
            {
                this.m_Resmelt = value;
            }
        }

        public bool Repair
        {
            get
            {
                return this.m_Repair;
            }
            set
            {
                this.m_Repair = value;
            }
        }

        public bool MarkOption
        {
            get
            {
                return this.m_MarkOption;
            }
            set
            {
                this.m_MarkOption = value;
            }
        }

        public bool CanEnhance
        {
            get
            {
                return this.m_CanEnhance;
            }
            set
            {
                this.m_CanEnhance = value;
            }
        }
		
        #region SA
        public bool QuestOption
        {
            get
            {
                return this.m_QuestOption;
            }
            set
            {
                this.m_QuestOption = value;
            }
        }
		
		public bool CanAlter
        {
            get
            {
                return this.m_CanAlter;
            }
            set
            {
                this.m_CanAlter = value;
            }
        }
        #endregion

        public CraftSystem(int minCraftEffect, int maxCraftEffect, double delay)
        {
            this.m_MinCraftEffect = minCraftEffect;
            this.m_MaxCraftEffect = maxCraftEffect;
            this.m_Delay = delay;

            this.m_CraftItems = new CraftItemCol();
            this.m_CraftGroups = new CraftGroupCol();
            this.m_CraftSubRes = new CraftSubResCol();
            this.m_CraftSubRes2 = new CraftSubResCol();

            this.InitCraftList();
        }

        public virtual bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            return true;
        }

        public void CreateItem(Mobile from, Type type, Type typeRes, BaseTool tool, CraftItem realCraftItem)
        { 
            // Verify if the type is in the list of the craftable item
            CraftItem craftItem = this.m_CraftItems.SearchFor(type);
            if (craftItem != null)
            {
                // The item is in the list, try to create it
                // Test code: items like sextant parts can be crafted either directly from ingots, or from different parts
                realCraftItem.Craft(from, this, typeRes, tool);
                //craftItem.Craft( from, this, typeRes, tool );
            }
        }

        public int AddCraft(Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount)
        {
            return this.AddCraft(typeItem, group, name, this.MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, "");
        }

        public int AddCraft(Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message)
        {
            return this.AddCraft(typeItem, group, name, this.MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, message);
        }

        public int AddCraft(Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount)
        {
            return this.AddCraft(typeItem, group, name, skillToMake, minSkill, maxSkill, typeRes, nameRes, amount, "");
        }

        public int AddCraft(Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message)
        {
            CraftItem craftItem = new CraftItem(typeItem, group, name);
            craftItem.AddRes(typeRes, nameRes, amount, message);
            craftItem.AddSkill(skillToMake, minSkill, maxSkill);

            this.DoGroup(group, craftItem);
            return this.m_CraftItems.Add(craftItem);
        }

        private void DoGroup(TextDefinition groupName, CraftItem craftItem)
        {
            int index = this.m_CraftGroups.SearchFor(groupName);

            if (index == -1)
            {
                CraftGroup craftGroup = new CraftGroup(groupName);
                craftGroup.AddCraftItem(craftItem);
                this.m_CraftGroups.Add(craftGroup);
            }
            else
            {
                this.m_CraftGroups.GetAt(index).AddCraftItem(craftItem);
            }
        }

        public void SetItemHue(int index, int hue)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.ItemHue = hue;
        }

        public void SetManaReq(int index, int mana)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.Mana = mana;
        }

        public void SetStamReq(int index, int stam)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.Stam = stam;
        }

        public void SetHitsReq(int index, int hits)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.Hits = hits;
        }
		
        public void SetUseAllRes(int index, bool useAll)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.UseAllRes = useAll;
        }

        public void SetNeedHeat(int index, bool needHeat)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.NeedHeat = needHeat;
        }

        public void SetNeedOven(int index, bool needOven)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.NeedOven = needOven;
        }

        public void SetBeverageType(int index, BeverageType requiredBeverage)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.RequiredBeverage = requiredBeverage;
        }

        public void SetNeedMill(int index, bool needMill)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.NeedMill = needMill;
        }

        public void SetNeededExpansion(int index, Expansion expansion)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.RequiredExpansion = expansion;
        }

        public void AddRes(int index, Type type, TextDefinition name, int amount)
        {
            this.AddRes(index, type, name, amount, "");
        }

        public void AddRes(int index, Type type, TextDefinition name, int amount, TextDefinition message)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.AddRes(type, name, amount, message);
        }

        public void AddSkill(int index, SkillName skillToMake, double minSkill, double maxSkill)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.AddSkill(skillToMake, minSkill, maxSkill);
        }

        public void SetUseSubRes2(int index, bool val)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.UseSubRes2 = val;
        }

        public void AddRecipe(int index, int id)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.AddRecipe(id, this);
        }

        public void ForceNonExceptional(int index)
        {
            CraftItem craftItem = this.m_CraftItems.GetAt(index);
            craftItem.ForceNonExceptional = true;
        }

        public void SetSubRes(Type type, string name)
        {
            this.m_CraftSubRes.ResType = type;
            this.m_CraftSubRes.NameString = name;
            this.m_CraftSubRes.Init = true;
        }

        public void SetSubRes(Type type, int name)
        {
            this.m_CraftSubRes.ResType = type;
            this.m_CraftSubRes.NameNumber = name;
            this.m_CraftSubRes.Init = true;
        }

        public void AddSubRes(Type type, int name, double reqSkill, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, message);
            this.m_CraftSubRes.Add(craftSubRes);
        }

        public void AddSubRes(Type type, int name, double reqSkill, int genericName, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, genericName, message);
            this.m_CraftSubRes.Add(craftSubRes);
        }

        public void AddSubRes(Type type, string name, double reqSkill, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, message);
            this.m_CraftSubRes.Add(craftSubRes);
        }

        public void SetSubRes2(Type type, string name)
        {
            this.m_CraftSubRes2.ResType = type;
            this.m_CraftSubRes2.NameString = name;
            this.m_CraftSubRes2.Init = true;
        }

        public void SetSubRes2(Type type, int name)
        {
            this.m_CraftSubRes2.ResType = type;
            this.m_CraftSubRes2.NameNumber = name;
            this.m_CraftSubRes2.Init = true;
        }

        public void AddSubRes2(Type type, int name, double reqSkill, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, message);
            this.m_CraftSubRes2.Add(craftSubRes);
        }

        public void AddSubRes2(Type type, int name, double reqSkill, int genericName, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, genericName, message);
            this.m_CraftSubRes2.Add(craftSubRes);
        }

        public void AddSubRes2(Type type, string name, double reqSkill, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, message);
            this.m_CraftSubRes2.Add(craftSubRes);
        }

        public abstract void InitCraftList();

        public abstract void PlayCraftEffect(Mobile from);

        public abstract int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item);

        public abstract int CanCraft(Mobile from, BaseTool tool, Type itemType);
    }
}