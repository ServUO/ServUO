using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

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
        #region Static Properties
        public static List<CraftSystem> Systems { get; set; }

        private static readonly Type[] _GlobalNoConsume =
        {
            typeof(CapturedEssence), typeof(EyeOfTheTravesty), typeof(DiseasedBark),  typeof(LardOfParoxysmus), typeof(GrizzledBones), typeof(DreadHornMane),

            typeof(Blight), typeof(Corruption), typeof(Muculent), typeof(Scourge), typeof(Putrefaction), typeof(Taint),

            // Tailoring
            typeof(MidnightBracers), typeof(CrimsonCincture), typeof(GargishCrimsonCincture), typeof(LeurociansMempoOfFortune), typeof(TheScholarsHalo),

            // Blacksmithy
            typeof(LeggingsOfBane), typeof(GauntletsOfNobility),

            // Carpentry
            typeof(StaffOfTheMagi), typeof(BlackrockMoonstone),

            // Tinkering
            typeof(RingOfTheElements), typeof(HatOfTheMagi), typeof(AutomatonActuator),

            // Inscription
            typeof(AntiqueDocumentsKit)
        };
        #endregion

        #region Static Methods
        public static CraftSystem GetSystem(Type type, bool subClass = false)
        {
            CraftSystem sys = null;

            for (int i = 0; i < Systems.Count; i++)
            {
                var system = Systems[i];

                if (system.CraftItems == null)
                {
                    continue;
                }

                CraftItem crItem = system.CraftItems.SearchFor(type);

                if (crItem == null && subClass)
                {
                    crItem = system.CraftItems.SearchForSubclass(type);
                }

                if (crItem != null)
                {
                    sys = system;
                    break;
                }
            }

            return sys;
        }
        #endregion

        private readonly Dictionary<Mobile, CraftContext> m_ContextTable = new Dictionary<Mobile, CraftContext>();

        #region Properties

        public double Delay { get; }

        public int MinCraftEffect { get; }

        public int MaxCraftEffect { get; }

        public CraftItemCol CraftItems { get; }

        public CraftGroupCol CraftGroups { get; }

        public CraftSubResCol CraftSubRes { get; }

        public CraftSubResCol CraftSubRes2 { get; }

        public bool CanEnhance { get; set; }

        public bool CanAlter { get; set; }

        public bool Resmelt { get; set; }

        public bool Repair { get; set; }

        public bool MarkOption { get; set; }

        public bool QuestOption { get; set; }

        #endregion

        #region Constructor
        public CraftSystem(int minCraftEffect, int maxCraftEffect, double delay)
        {
            MinCraftEffect = minCraftEffect;
            MaxCraftEffect = maxCraftEffect;
            Delay = delay;

            CraftItems = new CraftItemCol();
            CraftGroups = new CraftGroupCol();
            CraftSubRes = new CraftSubResCol();
            CraftSubRes2 = new CraftSubResCol();

            InitCraftList();
            AddSystem(this);
        }
        #endregion

        #region Abstract
        public abstract SkillName MainSkill { get; }

        public abstract double GetChanceAtMin(CraftItem item);

        public abstract void InitCraftList();

        public abstract void PlayCraftEffect(Mobile from);

        public abstract int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item);

        public abstract int CanCraft(Mobile from, ITool tool, Type itemType);
        #endregion

        #region Virtual
        public virtual int GumpTitleNumber => 0;

        public virtual string GumpTitleString => "";

        public virtual CraftECA ECA => CraftECA.ChanceMinusSixty;

        public virtual bool RetainsColorFrom(CraftItem item, Type type)
        {
            return false;
        }

        public virtual bool RetainsColorFromException(CraftItem item, Type type)
        {
            return false;
        }

        public virtual bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            return !_GlobalNoConsume.Any(t => t == resourceType);
        }

        public virtual bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem, ref MasterCraftsmanTalisman talisman)
        {
            if (!ConsumeOnFailure(from, resourceType, craftItem))
                return false;

            Item item = from.FindItemOnLayer(Layer.Talisman);

            if (item is MasterCraftsmanTalisman)
            {
                MasterCraftsmanTalisman mct = (MasterCraftsmanTalisman)item;

                if (mct.Charges > 0)
                {
                    talisman = mct;
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Methods
        public void AddContext(Mobile m, CraftContext c)
        {
            if (c == null || m == null || c.System != this)
                return;

            m_ContextTable[m] = c;
        }

        public CraftContext GetContext(Mobile m)
        {
            if (m == null)
                return null;

            if (m.Deleted)
            {
                m_ContextTable.Remove(m);
                return null;
            }

            CraftContext c = null;
            m_ContextTable.TryGetValue(m, out c);

            if (c == null)
                m_ContextTable[m] = c = new CraftContext(m, this);

            return c;
        }

        public void OnMade(Mobile m, CraftItem item)
        {
            CraftContext c = GetContext(m);

            if (c != null)
                c.OnMade(item);
        }

        public void OnRepair(Mobile m, ITool tool, Item deed, Item addon, IEntity e)
        {
            Item source;

            if (tool is Item)
            {
                source = (Item)tool;
            }
            else
            {
                source = deed ?? addon;
            }

            EventSink.InvokeRepairItem(new RepairItemEventArgs(m, source, e));
        }

        private void AddSystem(CraftSystem system)
        {
            if (Systems == null)
                Systems = new List<CraftSystem>();

            Systems.Add(system);
        }

        public void CreateItem(Mobile from, Type type, Type typeRes, ITool tool, CraftItem realCraftItem)
        {
            // Verify if the type is in the list of the craftable item
            CraftItem craftItem = CraftItems.SearchFor(type);
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
            return AddCraft(typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, "");
        }

        public int AddCraft(Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message)
        {
            return AddCraft(typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, message);
        }

        public int AddCraft(Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount)
        {
            return AddCraft(typeItem, group, name, skillToMake, minSkill, maxSkill, typeRes, nameRes, amount, "");
        }

        public int AddCraft(Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message)
        {
            CraftItem craftItem = new CraftItem(typeItem, group, name);
            craftItem.AddRes(typeRes, nameRes, amount, message);
            craftItem.AddSkill(skillToMake, minSkill, maxSkill);

            DoGroup(group, craftItem);
            return CraftItems.Add(craftItem);
        }

        private void DoGroup(TextDefinition groupName, CraftItem craftItem)
        {
            int index = CraftGroups.SearchFor(groupName);

            if (index == -1)
            {
                CraftGroup craftGroup = new CraftGroup(groupName);
                craftGroup.AddCraftItem(craftItem);
                CraftGroups.Add(craftGroup);
            }
            else
            {
                CraftGroups.GetAt(index).AddCraftItem(craftItem);
            }
        }

        public void SetItemHue(int index, int hue)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.ItemHue = hue;
        }

        public void SetManaReq(int index, int mana)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.Mana = mana;
        }

        public void SetStamReq(int index, int stam)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.Stam = stam;
        }

        public void SetHitsReq(int index, int hits)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.Hits = hits;
        }

        public void SetUseAllRes(int index, bool useAll)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.UseAllRes = useAll;
        }

        public void SetForceTypeRes(int index, bool value)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.ForceTypeRes = value;
        }

        public void SetNeedHeat(int index, bool needHeat)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.NeedHeat = needHeat;
        }

        public void SetNeedOven(int index, bool needOven)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.NeedOven = needOven;
        }

        public void SetNeedMaker(int index, bool needMaker)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.NeedMaker = needMaker;
        }

        public void SetNeedWater(int index, bool needWater)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.NeedWater = needWater;
        }

        public void SetBeverageType(int index, BeverageType requiredBeverage)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.RequiredBeverage = requiredBeverage;
        }

        public void SetNeedMill(int index, bool needMill)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.NeedMill = needMill;
        }

        public void SetRequiresBasketWeaving(int index)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.RequiresBasketWeaving = true;
        }

        public void SetRequireResTarget(int index)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.RequiresResTarget = true;
        }

        public void SetRequiresMechanicalLife(int index)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.RequiresMechanicalLife = true;
        }

        public void SetData(int index, object data)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.Data = data;
        }

        public void SetDisplayID(int index, int id)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.DisplayID = id;
        }

        /// <summary>
        /// Add a callback Action to allow mutating the crafted item. Handy when you have a single Item Type but you want to create variations of it.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="action"></param>
        public void SetMutateAction(int index, Action<Mobile, Item, ITool> action)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.MutateAction = action;
        }

        public void SetForceSuccess(int index, int success)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.ForceSuccessChance = success;
        }

        public void AddRes(int index, Type type, TextDefinition name, int amount)
        {
            AddRes(index, type, name, amount, "");
        }

        public void AddRes(int index, Type type, TextDefinition name, int amount, TextDefinition message)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.AddRes(type, name, amount, message);
        }

        public void AddResCallback(int index, Func<Mobile, ConsumeType, int> func)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.ConsumeResCallback = func;
        }

        public void AddSkill(int index, SkillName skillToMake, double minSkill, double maxSkill)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.AddSkill(skillToMake, minSkill, maxSkill);
        }

        public void SetUseSubRes2(int index, bool val)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.UseSubRes2 = val;
        }

        public void AddRecipe(int index, int id)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.AddRecipe(id, this);
        }

        public void ForceNonExceptional(int index)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.ForceNonExceptional = true;
        }

        public void ForceExceptional(int index)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.ForceExceptional = true;
        }

        public void SetMinSkillOffset(int index, double skillOffset)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.MinSkillOffset = skillOffset;
        }

        public void AddCraftAction(int index, Action<Mobile, CraftItem, ITool> action)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.TryCraft = action;
        }

        public void AddCreateItem(int index, Func<Mobile, CraftItem, ITool, Item> func)
        {
            CraftItem craftItem = CraftItems.GetAt(index);
            craftItem.CreateItem = func;
        }

        public void SetSubRes(Type type, string name)
        {
            CraftSubRes.ResType = type;
            CraftSubRes.NameString = name;
            CraftSubRes.Init = true;
        }

        public void SetSubRes(Type type, int name)
        {
            CraftSubRes.ResType = type;
            CraftSubRes.NameNumber = name;
            CraftSubRes.Init = true;
        }

        public void AddSubRes(Type type, int name, double reqSkill, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, message);
            CraftSubRes.Add(craftSubRes);
        }

        public void AddSubRes(Type type, int name, double reqSkill, int genericName, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, genericName, message);
            CraftSubRes.Add(craftSubRes);
        }

        public void AddSubRes(Type type, string name, double reqSkill, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, message);
            CraftSubRes.Add(craftSubRes);
        }

        public void SetSubRes2(Type type, string name)
        {
            CraftSubRes2.ResType = type;
            CraftSubRes2.NameString = name;
            CraftSubRes2.Init = true;
        }

        public void SetSubRes2(Type type, int name)
        {
            CraftSubRes2.ResType = type;
            CraftSubRes2.NameNumber = name;
            CraftSubRes2.Init = true;
        }

        public void AddSubRes2(Type type, int name, double reqSkill, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, message);
            CraftSubRes2.Add(craftSubRes);
        }

        public void AddSubRes2(Type type, int name, double reqSkill, int genericName, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, genericName, message);
            CraftSubRes2.Add(craftSubRes);
        }

        public void AddSubRes2(Type type, string name, double reqSkill, object message)
        {
            CraftSubRes craftSubRes = new CraftSubRes(type, name, reqSkill, message);
            CraftSubRes2.Add(craftSubRes);
        } 
        #endregion

    }
}
