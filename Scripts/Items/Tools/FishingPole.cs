using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Engines.Harvest;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public interface IBaitable
    {
        Type BaitType { get; }
        bool EnhancedBait { get; }
    }

    public class FishingPole : Item, IUsesRemaining, IResource, IQuality, IBaitable
    {
        private Type m_BaitType;
        private bool m_EnhancedBait;
        private HookType m_HookType;
        private int m_HookUses;
        private int m_BaitUses;
        private int m_OriginalHue;

        private Mobile m_Crafter;
        private ItemQuality m_Quality;

        private AosAttributes m_AosAttributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private CraftResource m_Resource;
        private bool m_PlayerConstructed;

        private int m_UsesRemaining;
        private bool m_ShowUsesRemaining;

        private int m_LowerStatReq;

        [CommandProperty(AccessLevel.GameMaster)]
        public Type BaitType
        {
            get { return m_BaitType; }
            set
            {
                m_BaitType = value;

                if (m_BaitType == null)
                    m_EnhancedBait = false;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool EnhancedBait
        {
            get { return m_EnhancedBait; }
            set { m_EnhancedBait = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public HookType HookType
        {
            get { return m_HookType; }
            set
            {
                m_HookType = value;

                if (m_HookType == HookType.None)
                    Hue = m_OriginalHue;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HookUses
        {
            get
            {
                return m_HookUses;
            }
            set
            {
                m_HookUses = value;

                if (m_HookUses <= 0)
                {
                    m_HookUses = 0;
                    HookType = HookType.None;
                }

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int BaitUses
        {
            get
            {
                return m_BaitUses;
            }
            set
            {
                m_BaitUses = value;

                if (m_BaitUses <= 0)
                {
                    m_BaitUses = 0;
                    BaitType = null;
                }

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes
        {
            get { return m_AosAttributes; }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SkillBonuses
        {
            get { return m_AosSkillBonuses; }
            set { }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set { m_Resource = value; Hue = CraftResources.GetHue(m_Resource); InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get { return m_PlayerConstructed; }
            set { m_PlayerConstructed = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int OriginalHue
        {
            get { return m_OriginalHue; }
            set { m_OriginalHue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get { return m_Quality; }
            set
            {
                UnscaleUses();
                m_Quality = value;
                ScaleUses();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining
        {
            get { return m_ShowUsesRemaining; }
            set { m_ShowUsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LowerStatReq
        {
            get { return m_LowerStatReq; }
            set { m_LowerStatReq = value; InvalidateProperties(); }
        }

        [Constructable]
        public FishingPole()
            : base(0x0DC0)
        {
            Layer = Layer.OneHanded;
            Weight = 8.0;
            Resource = CraftResource.RegularWood;

            m_BaitType = null;
            m_HookType = HookType.None;

            m_AosAttributes = new AosAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);

            UsesRemaining = 150;
        }

        public void ScaleUses()
        {
            m_UsesRemaining = (m_UsesRemaining * GetUsesScalar()) / 100;
            InvalidateProperties();
        }

        public void UnscaleUses()
        {
            m_UsesRemaining = (m_UsesRemaining * 100) / GetUsesScalar();
        }

        public int GetUsesScalar()
        {
            if (m_Quality == ItemQuality.Exceptional)
                return 200;

            return 100;
        }

        public int GetStrRequirement()
        {
            return AOS.Scale(10, 100 - m_LowerStatReq);
        }

        public void OnFishedHarvest(Mobile from, bool caughtAnything)
        {
            if (m_HookType != HookType.None)
            {
                HookUses--;

                if (m_HookType == HookType.None)
                    from.SendLocalizedMessage(1149854); //As the magic of the hook fades, it transforms to a normal fishhook.  The fishing pole returns to normal.
            }

            if (caughtAnything && m_BaitType != null)
                BaitUses--;
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            if (base.AllowEquipedCast(from))
                return true;

            return (m_AosAttributes.SpellChanneling != 0);
        }

        public virtual int GetLuckBonus()
        {
            if (m_Resource == CraftResource.Heartwood)
                return 0;

            CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);

            if (resInfo == null)
                return 0;

            CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

            if (attrInfo == null)
                return 0;

            return attrInfo.WeaponLuck;
        }

        public override void OnDoubleClick(Mobile from)
        {
            Fishing.System.BeginHarvesting(from, this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            BaseHarvestTool.AddContextMenuEntries(from, this, list, Fishing.System);
        }

        public override bool CanEquip(Mobile from)
        {
            if (from.Str < GetStrRequirement())
            {
                from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                return false;
            }

            return base.CanEquip(from);
        }

        public override bool OnEquip(Mobile from)
        {
            int strBonus = m_AosAttributes.BonusStr;
            int dexBonus = m_AosAttributes.BonusDex;
            int intBonus = m_AosAttributes.BonusInt;

            if ((strBonus != 0 || dexBonus != 0 || intBonus != 0))
            {
                Mobile m = from;

                string modName = Serial.ToString();

                if (strBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                if (dexBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                if (intBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
            }

            return true;
        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.AddTo(from);

                from.CheckStatTimers();
            }
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile m = (Mobile)parent;

                string modName = Serial.ToString();

                m.RemoveStatMod(modName + "Str");
                m.RemoveStatMod(modName + "Dex");
                m.RemoveStatMod(modName + "Int");

                m_AosSkillBonuses.Remove();

                m.CheckStatTimers();
            }
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.Name); // crafted by ~1_NAME~

            if (m_Quality == ItemQuality.Exceptional)
                list.Add(1060636); // exceptional
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            if (Siege.SiegeShard && m_ShowUsesRemaining)
            {
                list.Add(1060584, UsesRemaining.ToString()); // uses remaining: ~1_val~
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_AosAttributes.Brittle != 0)
                list.Add(1116209); // Brittle

            if (m_AosSkillBonuses != null)
                m_AosSkillBonuses.GetProperties(list);

            base.AddResistanceProperties(list);

            int prop = 0;

            if ((prop = (m_AosAttributes.WeaponDamage)) != 0)
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%

            if ((prop = m_AosAttributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = m_AosAttributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = m_AosAttributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = m_AosAttributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = (m_AosAttributes.AttackChance)) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = m_AosAttributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = m_AosAttributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = m_AosAttributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = m_AosAttributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = m_AosAttributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%

            if ((prop = m_LowerStatReq) != 0)
                list.Add(1060435, m_LowerStatReq.ToString()); // lower requirements ~1_val~%

            if ((prop = m_AosAttributes.SpellChanneling) != 0)
                list.Add(1060482); // spell channeling	

            if (!CraftResources.IsStandard(m_Resource))
                list.Add(CraftResources.GetName(m_Resource));

            if ((prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = m_AosAttributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = m_AosAttributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = m_AosAttributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = m_AosAttributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = m_AosAttributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = m_AosAttributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = m_AosAttributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = m_AosAttributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = m_AosAttributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            //if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
            //	list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%

            int hookCliloc = BaseFishingHook.GetHookType(m_HookType);

            if (m_HookType > HookType.None && hookCliloc > 0)
            {
                list.Add(1150885, string.Format("#{0}", hookCliloc)); //special hook: ~1_token~
                list.Add(1150889, string.Format("#{0}", BaseFishingHook.GetCondition(m_HookUses))); //Hook condition: ~1_val~
            }

            if (m_BaitType != null)
            {
                object label = FishInfo.GetFishLabel(m_BaitType);
                if (label is int)
                    list.Add(1116468, string.Format("#{0}", (int)label)); //baited to attract: ~1_val~
                else if (label is string)
                    list.Add(1116468, (string)label);

                list.Add(1116466, m_BaitUses.ToString()); // amount: ~1_val~
            }

            list.Add(1061170, GetStrRequirement().ToString()); // strength requirement ~1_val~
        }

        public FishingPole(Serial serial)
            : base(serial)
        {
        }

        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(4); // version

            writer.Write(m_PlayerConstructed);
            writer.Write(m_LowerStatReq);

            writer.Write(m_UsesRemaining);
            writer.Write(m_ShowUsesRemaining);

            writer.Write(m_OriginalHue);

            writer.Write(FishInfo.GetIndexFromType(m_BaitType));
            writer.Write((int)m_HookType);
            writer.Write(m_HookUses);
            writer.Write(m_BaitUses);
            writer.Write(m_EnhancedBait);

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.xAttributes, !m_AosAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SkillBonuses, !m_AosSkillBonuses.IsEmpty);

            writer.Write((int)flags);

            if (GetSaveFlag(flags, SaveFlag.xAttributes))
                m_AosAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                m_AosSkillBonuses.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    m_PlayerConstructed = reader.ReadBool();
                    m_LowerStatReq = reader.ReadInt();
                    goto case 3;
                case 3:
                    m_UsesRemaining = reader.ReadInt();
                    m_ShowUsesRemaining = reader.ReadBool();
                    goto case 2;
                case 2:
                    m_OriginalHue = reader.ReadInt();
                    int idx = reader.ReadInt();
                    m_BaitType = FishInfo.GetTypeFromIndex(idx);
                    m_HookType = (HookType)reader.ReadInt();
                    m_HookUses = reader.ReadInt();
                    m_BaitUses = reader.ReadInt();
                    m_EnhancedBait = reader.ReadBool();

                    SaveFlag flags = (SaveFlag)reader.ReadInt();

                    if (GetSaveFlag(flags, SaveFlag.xAttributes))
                        m_AosAttributes = new AosAttributes(this, reader);
                    else
                        m_AosAttributes = new AosAttributes(this);

                    if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                        m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                    else
                        m_AosSkillBonuses = new AosSkillBonuses(this);
                    break;
                case 1:
                    m_AosAttributes = new AosAttributes(this);
                    m_AosSkillBonuses = new AosSkillBonuses(this);
                    break;
            }

            if (Parent is Mobile)
                m_AosSkillBonuses.AddTo((Mobile)Parent);

            int strBonus = m_AosAttributes.BonusStr;
            int dexBonus = m_AosAttributes.BonusDex;
            int intBonus = m_AosAttributes.BonusInt;

            if (Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0))
            {
                Mobile m = (Mobile)Parent;

                string modName = Serial.ToString();

                if (strBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                if (dexBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                if (intBonus != 0)
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
            }

            if (Parent is Mobile)
                ((Mobile)Parent).CheckStatTimers();

            if (m_BaitType != null && m_BaitUses <= 0)
                BaitType = null;

            if (m_HookType != HookType.None && m_HookUses <= 0)
                HookType = HookType.None;

            if (version < 3 && m_Crafter != null)
            {
                m_PlayerConstructed = true;

                if (m_Resource == CraftResource.None)
                    Resource = CraftResource.RegularWood;
                else
                {
                    DistributeMaterialBonus();
                }
            }
        }

        private enum SaveFlag
        {
            None = 0x00000000,
            xAttributes = 0x00000001,
            SkillBonuses = 0x00000002
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            PlayerConstructed = true;

            if (makersMark) // Add to CraftItem.cs mark table
                Crafter = from;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);
            DistributeMaterialBonus();

            return quality;
        }

        public void DistributeMaterialBonus()
        {
            CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);

            if (resInfo == null)
                return;

            CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

            if (attrInfo != null)
                DistributeMaterialBonus(attrInfo);
        }

        public void DistributeMaterialBonus(CraftAttributeInfo attrInfo)
        {
            if (m_Resource != CraftResource.Heartwood)
            {
                Attributes.SpellChanneling = attrInfo.OtherSpellChanneling;
                Attributes.Luck = attrInfo.OtherLuck;
                Attributes.RegenHits = attrInfo.OtherRegenHits;
                LowerStatReq = attrInfo.OtherLowerRequirements;
            }
            else
            {
                switch (Utility.Random(5))
                {
                    case 0: Attributes.Luck += 40; break;
                    case 1: Attributes.Luck += 10; break;
                    case 2: Attributes.RegenHits += attrInfo.OtherRegenHits; break;
                    case 3: Attributes.SpellChanneling = attrInfo.OtherSpellChanneling; break;
                    case 4: LowerStatReq = attrInfo.OtherLowerRequirements; break;
                }
            }
        }
    }
}
