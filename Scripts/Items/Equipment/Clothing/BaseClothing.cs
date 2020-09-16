using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Network;
using Server.Misc;

using System;
using System.Collections.Generic;

namespace Server.Items
{
    public interface IArcaneEquip
    {
        bool IsArcane { get; }
        int CurArcaneCharges { get; set; }
        int MaxArcaneCharges { get; set; }
        int TempHue { get; set; }
    }

    public abstract class BaseClothing : Item, IDyable, IScissorable, ICraftable, IWearableDurability, IResource, ISetItem, IVvVItem, IOwnerRestricted, IArtifact, ICombatEquipment, IEngravable, IQuality
    {
        private string m_EngravedText;

        [CommandProperty(AccessLevel.GameMaster)]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set
            {
                m_EngravedText = value;
                InvalidateProperties();
            }
        }

        private bool _VvVItem;
        private Mobile _Owner;
        private string _OwnerName;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsVvVItem
        {
            get { return _VvVItem; }
            set { _VvVItem = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return _Owner; }
            set { _Owner = value; if (_Owner != null) _OwnerName = _Owner.Name; InvalidateProperties(); }
        }

        public virtual string OwnerName
        {
            get { return _OwnerName; }
            set { _OwnerName = value; InvalidateProperties(); }
        }

        public virtual bool CanFortify => !IsImbued && NegativeAttributes.Antique < 4;
        public virtual bool CanRepair => m_NegativeAttributes.NoRepair == 0;
        public virtual bool CanAlter => true;

        private int m_MaxHitPoints;
        private int m_HitPoints;
        private Mobile m_Crafter;
        private ItemQuality m_Quality;
        protected CraftResource m_Resource;
        private int m_StrReq = -1;

        private bool m_Altered;

        private AosAttributes m_AosAttributes;
        private AosArmorAttributes m_AosClothingAttributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private AosElementAttributes m_AosResistances;
        private SAAbsorptionAttributes m_SAAbsorptionAttributes;
        private NegativeAttributes m_NegativeAttributes;
        private AosWeaponAttributes m_AosWeaponAttributes;

        private int m_TimesImbued;
        private bool m_IsImbued;
        private int m_GorgonLenseCharges;
        private LenseType m_GorgonLenseType;

        private ItemPower m_ItemPower;
        private ReforgedPrefix m_ReforgedPrefix;
        private ReforgedSuffix m_ReforgedSuffix;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHitPoints; }
            set
            {
                m_MaxHitPoints = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_HitPoints; }
            set
            {
                if (value != m_HitPoints && MaxHitPoints > 0)
                {
                    m_HitPoints = value;

                    if (m_HitPoints < 0)
                        Delete();
                    else if (m_HitPoints > MaxHitPoints)
                        m_HitPoints = MaxHitPoints;

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrRequirement
        {
            get
            {
                if (m_NegativeAttributes.Massive > 0)
                {
                    return 125;
                }

                return m_StrReq == -1 ? StrReq : m_StrReq;
            }
            set
            {
                m_StrReq = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get { return m_Quality; }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimesImbued
        {
            get { return m_TimesImbued; }
            set { m_TimesImbued = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsImbued
        {
            get
            {
                if (TimesImbued >= 1 && !m_IsImbued)
                    m_IsImbued = true;

                return m_IsImbued;
            }
            set
            {
                if (TimesImbued >= 1)
                    m_IsImbued = true;
                else
                    m_IsImbued = value; InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int GorgonLenseCharges
        {
            get { return m_GorgonLenseCharges; }
            set { m_GorgonLenseCharges = value; if (value == 0) m_GorgonLenseType = LenseType.None; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public LenseType GorgonLenseType
        {
            get { return m_GorgonLenseType; }
            set { m_GorgonLenseType = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PhysNonImbuing { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int FireNonImbuing { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ColdNonImbuing { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonNonImbuing { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EnergyNonImbuing { get; set; }

        public virtual int[] BaseResists
        {
            get
            {
                int[] list = new int[5];

                list[0] = BasePhysicalResistance;
                list[1] = BaseFireResistance;
                list[2] = BaseColdResistance;
                list[3] = BasePoisonResistance;
                list[4] = BaseEnergyResistance;

                return list;
            }
        }

        public virtual void OnAfterImbued(Mobile m, int mod, int value)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ReforgedPrefix ReforgedPrefix
        {
            get { return m_ReforgedPrefix; }
            set { m_ReforgedPrefix = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ReforgedSuffix ReforgedSuffix
        {
            get { return m_ReforgedSuffix; }
            set { m_ReforgedSuffix = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemPower ItemPower
        {
            get { return m_ItemPower; }
            set { m_ItemPower = value; InvalidateProperties(); }
        }

        #region Personal Bless Deed
        private Mobile m_BlessedBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BlessedBy
        {
            get { return m_BlessedBy; }
            set
            {
                m_BlessedBy = value;
                InvalidateProperties();
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (BlessedFor == from && BlessedBy == from && RootParent == from)
            {
                list.Add(new UnBlessEntry(from, this));
            }
        }

        private class UnBlessEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly BaseClothing m_Item;

            public UnBlessEntry(Mobile from, BaseClothing item)
                : base(6208, -1)
            {
                m_From = from;
                m_Item = item; // BaseArmor, BaseWeapon or BaseClothing
            }

            public override void OnClick()
            {
                m_Item.BlessedFor = null;
                m_Item.BlessedBy = null;

                Container pack = m_From.Backpack;

                if (pack != null)
                {
                    pack.DropItem(new PersonalBlessDeed(m_From));
                    m_From.SendLocalizedMessage(1062200); // A personal bless deed has been placed in your backpack.
                }
            }
        }
        #endregion

        public virtual CraftResource DefaultResource => CraftResource.None;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return m_Resource;
            }
            set
            {
                m_Resource = value;
                Hue = CraftResources.GetHue(m_Resource);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes { get { return m_AosAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosArmorAttributes ClothingAttributes { get { return m_AosClothingAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SkillBonuses { get { return m_AosSkillBonuses; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosElementAttributes Resistances { get { return m_AosResistances; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SAAbsorptionAttributes SAAbsorptionAttributes { get { return m_SAAbsorptionAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NegativeAttributes NegativeAttributes { get { return m_NegativeAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosWeaponAttributes WeaponAttributes { get { return m_AosWeaponAttributes; } set { } }

        public virtual int BasePhysicalResistance => 0;
        public virtual int BaseFireResistance => 0;
        public virtual int BaseColdResistance => 0;
        public virtual int BasePoisonResistance => 0;
        public virtual int BaseEnergyResistance => 0;

        public override int PhysicalResistance => BasePhysicalResistance + m_AosResistances.Physical;
        public override int FireResistance => BaseFireResistance + m_AosResistances.Fire;
        public override int ColdResistance => BaseColdResistance + m_AosResistances.Cold;
        public override int PoisonResistance => BasePoisonResistance + m_AosResistances.Poison;
        public override int EnergyResistance => BaseEnergyResistance + m_AosResistances.Energy;

        public virtual int ArtifactRarity => 0;

        public override bool DisplayWeight
        {
            get
            {
                if (IsVvVItem)
                    return true;

                return base.DisplayWeight;
            }
        }

        public virtual int BaseStrBonus => 0;
        public virtual int BaseDexBonus => 0;
        public virtual int BaseIntBonus => 0;

        public override double DefaultWeight
        {
            get
            {
                if (NegativeAttributes == null || NegativeAttributes.Unwieldly == 0)
                    return base.DefaultWeight;

                return 50;
            }
        }

        public override bool CanEquip(Mobile from)
        {
            if (from.IsPlayer())
            {
                if (_Owner != null && _Owner != from)
                {
                    from.SendLocalizedMessage(501023); // You must be the owner to use this item.
                    return false;
                }

                if (this is IAccountRestricted && ((IAccountRestricted)this).Account != null)
                {
                    Accounting.Account acct = from.Account as Accounting.Account;

                    if (acct == null || acct.Username != ((IAccountRestricted)this).Account)
                    {
                        from.SendLocalizedMessage(1071296); // This item is Account Bound and your character is not bound to it. You cannot use this item.
                        return false;
                    }
                }

                if (IsVvVItem && !Engines.VvV.ViceVsVirtueSystem.IsVvV(from))
                {
                    from.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                    return false;
                }

                if (!RaceDefinitions.ValidateEquipment(from, this))
                {
                    return false;
                }
                else if (!AllowMaleWearer && !from.Female)
                {
                    if (AllowFemaleWearer)
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1010388); // Only females can wear this.
                    else
                        from.SendLocalizedMessage(1071936); // You cannot equip that.

                    return false;
                }
                else if (!AllowFemaleWearer && from.Female)
                {
                    if (AllowMaleWearer)
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1063343); // Only males can wear this.
                    else
                        from.SendLocalizedMessage(1071936); // You cannot equip that.

                    return false;
                }
                #region Personal Bless Deed
                else if (BlessedBy != null && BlessedBy != from)
                {
                    from.SendLocalizedMessage(1075277); // That item is blessed by another player.

                    return false;
                }
                #endregion
                else
                {
                    int strBonus = ComputeStatBonus(StatType.Str);
                    int strReq = ComputeStatReq(StatType.Str);

                    if (from.Str < strReq || (from.Str + strBonus) < 1)
                    {
                        from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                        return false;
                    }
                }
            }

            return base.CanEquip(from);
        }

        public virtual int StrReq => 10;

        public virtual int InitMinHits => 0;
        public virtual int InitMaxHits => 0;

        public virtual bool AllowMaleWearer => true;
        public virtual bool AllowFemaleWearer => true;

        public virtual bool CanBeBlessed => true;

        public int ComputeStatReq(StatType type)
        {
            return AOS.Scale(StrRequirement, 100 - GetLowerStatReq());
        }

        public int ComputeStatBonus(StatType type)
        {
            if (type == StatType.Str)
                return BaseStrBonus + Attributes.BonusStr;
            else if (type == StatType.Dex)
                return BaseDexBonus + Attributes.BonusDex;
            else
                return BaseIntBonus + Attributes.BonusInt;
        }

        public virtual void AddStatBonuses(Mobile parent)
        {
            if (parent == null)
                return;

            int strBonus = ComputeStatBonus(StatType.Str);
            int dexBonus = ComputeStatBonus(StatType.Dex);
            int intBonus = ComputeStatBonus(StatType.Int);

            if (strBonus == 0 && dexBonus == 0 && intBonus == 0)
                return;

            string modName = Serial.ToString();

            if (strBonus != 0)
                parent.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

            if (dexBonus != 0)
                parent.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

            if (intBonus != 0)
                parent.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
        }

        public static void ValidateMobile(Mobile m)
        {
            for (int i = m.Items.Count - 1; i >= 0; --i)
            {
                if (i >= m.Items.Count)
                    continue;

                Item item = m.Items[i];

                if (item is BaseClothing)
                {
                    BaseClothing clothing = (BaseClothing)item;

                    if (!RaceDefinitions.ValidateEquipment(m, clothing))
                    {
                        m.AddToBackpack(clothing);
                    }
                    else if (!clothing.AllowMaleWearer && !m.Female && m.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (clothing.AllowFemaleWearer)
                            m.SendLocalizedMessage(1010388); // Only females can wear this.
                        else
                            m.SendLocalizedMessage(1071936); // You cannot equip that.

                        m.AddToBackpack(clothing);
                    }
                    else if (!clothing.AllowFemaleWearer && m.Female && m.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (clothing.AllowMaleWearer)
                            m.SendLocalizedMessage(1063343); // Only males can wear this.
                        else
                            m.SendLocalizedMessage(1071936); // You cannot equip that.

                        m.AddToBackpack(clothing);
                    }
                }
            }
        }

        public int GetLowerStatReq()
        {
            return m_AosClothingAttributes.LowerStatReq;
        }

        public override void OnAdded(object parent)
        {
            Mobile mob = parent as Mobile;

            if (mob != null)
            {
                m_AosSkillBonuses.AddTo(mob);

                if (IsSetItem)
                {
                    m_SetEquipped = SetHelper.FullSetEquipped(mob, SetID, Pieces);

                    if (m_SetEquipped)
                    {
                        m_LastEquipped = true;
                        SetHelper.AddSetBonus(mob, SetID);
                    }
                }

                AddStatBonuses(mob);
                mob.CheckStatTimers();
            }

            base.OnAdded(parent);
        }

        public override void OnRemoved(object parent)
        {
            Mobile mob = parent as Mobile;

            if (mob != null)
            {
                m_AosSkillBonuses.Remove();

                if (IsSetItem && m_SetEquipped)
                    SetHelper.RemoveSetBonus(mob, SetID, this);

                string modName = Serial.ToString();

                mob.RemoveStatMod(modName + "Str");
                mob.RemoveStatMod(modName + "Dex");
                mob.RemoveStatMod(modName + "Int");

                mob.CheckStatTimers();
            }

            base.OnRemoved(parent);
        }

        public DateTime NextSelfRepair { get; set; }

        public virtual int OnHit(BaseWeapon weapon, int damageTaken)
        {
            if (m_MaxHitPoints == 0)
                return damageTaken;

            int Absorbed = Utility.RandomMinMax(1, 4);

            damageTaken -= Absorbed;

            if (damageTaken < 0)
                damageTaken = 0;

            double chance = NegativeAttributes.Antique > 0 ? 80 : 25;

            if (chance >= Utility.Random(100)) // 25% chance to lower durability
            {
                int selfRepair = m_AosClothingAttributes.SelfRepair + (IsSetItem && m_SetEquipped ? m_SetSelfRepair : 0);

                if (selfRepair > 0 && NextSelfRepair < DateTime.UtcNow)
                {
                    HitPoints += selfRepair;

                    NextSelfRepair = DateTime.UtcNow + TimeSpan.FromSeconds(60);
                }
                else
                {
                    int wear = 1;

                    if (weapon.Type == WeaponType.Bashing)
                        wear = Absorbed / 2;

                    if (wear > 0 && m_MaxHitPoints > 0)
                    {
                        if (m_HitPoints >= wear)
                        {
                            HitPoints -= wear;
                            wear = 0;
                        }
                        else
                        {
                            wear -= HitPoints;
                            HitPoints = 0;
                        }

                        if (wear > 0)
                        {
                            if (m_MaxHitPoints > wear)
                            {
                                MaxHitPoints -= wear;

                                if (Parent is Mobile)
                                    ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                            }
                            else
                            {
                                Delete();
                            }
                        }
                    }
                }
            }

            return damageTaken;
        }

        public BaseClothing(int itemID, Layer layer)
            : this(itemID, layer, 0)
        {
        }

        public BaseClothing(int itemID, Layer layer, int hue)
            : base(itemID)
        {
            Layer = layer;
            Hue = hue;

            m_Resource = DefaultResource;
            m_Quality = ItemQuality.Normal;

            m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            m_AosAttributes = new AosAttributes(this);
            m_AosClothingAttributes = new AosArmorAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);
            m_AosResistances = new AosElementAttributes(this);
            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);
            m_NegativeAttributes = new NegativeAttributes(this);
            m_AosWeaponAttributes = new AosWeaponAttributes(this);

            m_SetAttributes = new AosAttributes(this);
            m_SetSkillBonuses = new AosSkillBonuses(this);
        }

        public override void OnAfterDuped(Item newItem)
        {
            BaseClothing clothing = newItem as BaseClothing;

            if (clothing == null)
                return;

            clothing.m_AosAttributes = new AosAttributes(newItem, m_AosAttributes);
            clothing.m_AosResistances = new AosElementAttributes(newItem, m_AosResistances);
            clothing.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
            clothing.m_AosClothingAttributes = new AosArmorAttributes(newItem, m_AosClothingAttributes);
            clothing.m_SAAbsorptionAttributes = new SAAbsorptionAttributes(newItem, m_SAAbsorptionAttributes);
            clothing.m_NegativeAttributes = new NegativeAttributes(newItem, m_NegativeAttributes);
            clothing.m_AosWeaponAttributes = new AosWeaponAttributes(newItem, m_AosWeaponAttributes);

            clothing.m_SetAttributes = new AosAttributes(newItem, m_SetAttributes);
            clothing.m_SetSkillBonuses = new AosSkillBonuses(newItem, m_SetSkillBonuses);

            base.OnAfterDuped(newItem);
        }

        public BaseClothing(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            if (base.AllowEquipedCast(from))
                return true;

            return (m_AosAttributes.SpellChanneling != 0);
        }

        public void UnscaleDurability()
        {
            int scale = 100 + m_AosClothingAttributes.DurabilityBonus;

            m_HitPoints = ((m_HitPoints * 100) + (scale - 1)) / scale;
            m_MaxHitPoints = ((m_MaxHitPoints * 100) + (scale - 1)) / scale;

            InvalidateProperties();
        }

        public void ScaleDurability()
        {
            int scale = 100 + m_AosClothingAttributes.DurabilityBonus;

            m_HitPoints = ((m_HitPoints * scale) + 99) / 100;
            m_MaxHitPoints = ((m_MaxHitPoints * scale) + 99) / 100;

            if (m_MaxHitPoints > 255)
                m_MaxHitPoints = 255;

            if (m_HitPoints > 255)
                m_HitPoints = 255;

            InvalidateProperties();
        }

        public override bool CheckPropertyConfliction(Mobile m)
        {
            if (base.CheckPropertyConfliction(m))
                return true;

            if (Layer == Layer.Pants)
                return (m.FindItemOnLayer(Layer.InnerLegs) != null);

            if (Layer == Layer.Shirt)
                return (m.FindItemOnLayer(Layer.InnerTorso) != null);

            return false;
        }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
                name = string.Format("#{0}", LabelNumber);

            return name;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            int oreType;

            switch (m_Resource)
            {
                case CraftResource.DullCopper:
                    oreType = 1053108;
                    break; // dull copper
                case CraftResource.ShadowIron:
                    oreType = 1053107;
                    break; // shadow iron
                case CraftResource.Copper:
                    oreType = 1053106;
                    break; // copper
                case CraftResource.Bronze:
                    oreType = 1053105;
                    break; // bronze
                case CraftResource.Gold:
                    oreType = 1053104;
                    break; // golden
                case CraftResource.Agapite:
                    oreType = 1053103;
                    break; // agapite
                case CraftResource.Verite:
                    oreType = 1053102;
                    break; // verite
                case CraftResource.Valorite:
                    oreType = 1053101;
                    break; // valorite
                case CraftResource.SpinedLeather:
                    oreType = 1061118;
                    break; // spined
                case CraftResource.HornedLeather:
                    oreType = 1061117;
                    break; // horned
                case CraftResource.BarbedLeather:
                    oreType = 1061116;
                    break; // barbed
                case CraftResource.RedScales:
                    oreType = 1060814;
                    break; // red
                case CraftResource.YellowScales:
                    oreType = 1060818;
                    break; // yellow
                case CraftResource.BlackScales:
                    oreType = 1060820;
                    break; // black
                case CraftResource.GreenScales:
                    oreType = 1060819;
                    break; // green
                case CraftResource.WhiteScales:
                    oreType = 1060821;
                    break; // white
                case CraftResource.BlueScales:
                    oreType = 1060815;
                    break; // blue
                default:
                    oreType = 0;
                    break;
            }

            if (m_ReforgedPrefix != ReforgedPrefix.None || m_ReforgedSuffix != ReforgedSuffix.None)
            {
                if (m_ReforgedPrefix != ReforgedPrefix.None)
                {
                    int prefix = RunicReforging.GetPrefixName(m_ReforgedPrefix);

                    if (m_ReforgedSuffix == ReforgedSuffix.None)
                        list.Add(1151757, string.Format("#{0}\t{1}", prefix, GetNameString())); // ~1_PREFIX~ ~2_ITEM~
                    else
                        list.Add(1151756, string.Format("#{0}\t{1}\t#{2}", prefix, GetNameString(), RunicReforging.GetSuffixName(m_ReforgedSuffix))); // ~1_PREFIX~ ~2_ITEM~ of ~3_SUFFIX~
                }
                else if (m_ReforgedSuffix != ReforgedSuffix.None)
                {
                    RunicReforging.AddSuffixName(list, m_ReforgedSuffix, GetNameString());
                }
            }
            else if (oreType != 0)
                list.Add(1053099, "#{0}\t{1}", oreType, GetNameString()); // ~1_oretype~ ~2_armortype~
            else if (Name == null)
                list.Add(LabelNumber);
            else
                list.Add(Name);
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (OwnerName != null)
                list.Add(1153213, OwnerName);

            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Quality == ItemQuality.Exceptional)
                list.Add(1018303); // Exceptional

            if (IsImbued == true)
                list.Add(1080418); // (Imbued)

            if (m_Altered)
                list.Add(1111880); // Altered
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            if (IsVvVItem)
                list.Add(1154937); // VvV Item

            if (!string.IsNullOrEmpty(m_EngravedText))
            {
                list.Add(1158847, Utility.FixHtml(m_EngravedText)); // Embroidered: ~1_MESSAGE~	
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (m_GorgonLenseCharges > 0)
                list.Add(1112590, m_GorgonLenseCharges.ToString()); //Gorgon Lens Charges: ~1_val~

            if (IsSetItem)
            {
                if (MixedSet)
                    list.Add(1073491, Pieces.ToString()); // Part of a Weapon/Armor Set (~1_val~ pieces)
                else
                    list.Add(1072376, Pieces.ToString()); // Part of an Armor Set (~1_val~ pieces)

                if (SetID == SetItem.Bestial)
                    list.Add(1151541, BestialSetHelper.GetTotalBerserk(this).ToString()); // Berserk ~1_VAL~

                if (BardMasteryBonus)
                    list.Add(1151553); // Activate: Bard Mastery Bonus x2<br>(Effect: 1 min. Cooldown: 30 min.)

                if (m_SetEquipped)
                {
                    if (MixedSet)
                        list.Add(1073492); // Full Weapon/Armor Set Present
                    else
                        list.Add(1072377); // Full Armor Set Present

                    GetSetProperties(list);
                }
            }

            if (RaceDefinitions.GetRequiredRace(this) == Race.Elf)
            {
                list.Add(1075086); // Elves Only
            }
            else if (RaceDefinitions.GetRequiredRace(this) == Race.Gargoyle)
            {
                list.Add(1111709); // Gargoyles Only
            }

            if (m_NegativeAttributes != null)
                m_NegativeAttributes.GetProperties(list, this);

            if (m_AosSkillBonuses != null)
                m_AosSkillBonuses.GetProperties(list);

            int prop;

            if ((prop = ArtifactRarity) > 0)
                list.Add(1061078, prop.ToString()); // artifact rarity ~1_val~

            if ((prop = m_SAAbsorptionAttributes.EaterFire) != 0)
                list.Add(1113593, prop.ToString()); // Fire Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterCold) != 0)
                list.Add(1113594, prop.ToString()); // Cold Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterPoison) != 0)
                list.Add(1113595, prop.ToString()); // Poison Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterEnergy) != 0)
                list.Add(1113596, prop.ToString()); // Energy Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterKinetic) != 0)
                list.Add(1113597, prop.ToString()); // Kinetic Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.EaterDamage) != 0)
                list.Add(1113598, prop.ToString()); // Damage Eater ~1_Val~%

            if ((prop = m_SAAbsorptionAttributes.ResonanceFire) != 0)
                list.Add(1113691, prop.ToString()); // Fire Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.ResonanceCold) != 0)
                list.Add(1113692, prop.ToString()); // Cold Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.ResonancePoison) != 0)
                list.Add(1113693, prop.ToString()); // Poison Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.ResonanceEnergy) != 0)
                list.Add(1113694, prop.ToString()); // Energy Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.ResonanceKinetic) != 0)
                list.Add(1113695, prop.ToString()); // Kinetic Resonance ~1_val~%

            if ((prop = m_SAAbsorptionAttributes.CastingFocus) != 0)
                list.Add(1113696, prop.ToString()); // Casting Focus ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitColdArea) != 0)
                list.Add(1060416, prop.ToString()); // hit cold area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitDispel) != 0)
                list.Add(1060417, prop.ToString()); // hit dispel ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitEnergyArea) != 0)
                list.Add(1060418, prop.ToString()); // hit energy area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitFireArea) != 0)
                list.Add(1060419, prop.ToString()); // hit fire area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitFireball) != 0)
                list.Add(1060420, prop.ToString()); // hit fireball ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitHarm) != 0)
                list.Add(1060421, prop.ToString()); // hit harm ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLeechHits) != 0)
                list.Add(1060422, prop.ToString()); // hit life leech ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLightning) != 0)
                list.Add(1060423, prop.ToString()); // hit lightning ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLowerAttack) != 0)
                list.Add(1060424, prop.ToString()); // hit lower attack ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLowerDefend) != 0)
                list.Add(1060425, prop.ToString()); // hit lower defense ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitMagicArrow) != 0)
                list.Add(1060426, prop.ToString()); // hit magic arrow ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLeechMana) != 0)
                list.Add(1060427, prop.ToString()); // hit mana leech ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitPhysicalArea) != 0)
                list.Add(1060428, prop.ToString()); // hit physical area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitPoisonArea) != 0)
                list.Add(1060429, prop.ToString()); // hit poison area ~1_val~%

            if ((prop = m_AosWeaponAttributes.HitLeechStam) != 0)
                list.Add(1060430, prop.ToString()); // hit stamina leech ~1_val~%

            if ((prop = m_AosAttributes.SpellChanneling) != 0)
                list.Add(1060482); // spell channeling

            if ((prop = m_AosClothingAttributes.SelfRepair) != 0)
                list.Add(1060450, prop.ToString()); // self repair ~1_val~

            if ((prop = m_AosAttributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = m_AosAttributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = m_AosAttributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = m_AosAttributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = m_AosAttributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = m_AosAttributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = m_AosAttributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = m_AosAttributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = m_AosAttributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = m_AosAttributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = m_AosAttributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = m_AosAttributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = m_AosAttributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = m_AosAttributes.AttackChance) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = m_AosAttributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            if ((prop = m_AosAttributes.WeaponDamage) != 0)
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%

            if ((prop = m_AosAttributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = m_AosAttributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~		

            if ((prop = m_AosAttributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = m_AosAttributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = m_AosAttributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = m_AosAttributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%

            if ((prop = m_AosAttributes.LowerAmmoCost) != 0)
                list.Add(1075208, prop.ToString()); // Lower Ammo Cost ~1_Percentage~%

            if ((prop = m_AosAttributes.IncreasedKarmaLoss) != 0)
                list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%

            base.AddResistanceProperties(list);

            if ((prop = m_AosClothingAttributes.MageArmor) != 0)
                list.Add(1060437); // mage armor

            if ((prop = m_AosClothingAttributes.LowerStatReq) != 0)
                list.Add(1060435, prop.ToString()); // lower requirements ~1_val~%

            if ((prop = ComputeStatReq(StatType.Str)) > 0)
                list.Add(1061170, prop.ToString()); // strength requirement ~1_val~

            if ((prop = m_AosClothingAttributes.DurabilityBonus) > 0)
                list.Add(1151780, prop.ToString()); // durability +~1_VAL~%

            if (m_HitPoints >= 0 && m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints); // durability ~1_val~ / ~2_val~

            if (IsSetItem && !m_SetEquipped)
            {
                list.Add(1072378); // <br>Only when full set is present:				
                GetSetProperties(list);
            }
        }

        public override void AddItemPowerProperties(ObjectPropertyList list)
        {
            if (m_ItemPower != ItemPower.None)
            {
                if (m_ItemPower <= ItemPower.LegendaryArtifact)
                    list.Add(1151488 + ((int)m_ItemPower - 1));
                else
                    list.Add(1152281 + ((int)m_ItemPower - 9));
            }
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool drop = base.DropToWorld(from, p);

            EnchantedHotItemSocket.CheckDrop(from, this);

            return drop;
        }

        public virtual void AddEquipInfoAttributes(Mobile from, List<EquipInfoAttribute> attrs)
        {
            if (DisplayLootType)
            {
                if (LootType == LootType.Blessed)
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                else if (LootType == LootType.Cursed)
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            }

            if (m_Quality == ItemQuality.Exceptional)
                attrs.Add(new EquipInfoAttribute(1018305 - (int)m_Quality));
        }

        #region Serialization
        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Resource = 0x00000001,
            Attributes = 0x00000002,
            ClothingAttributes = 0x00000004,
            SkillBonuses = 0x00000008,
            Resistances = 0x00000010,
            MaxHitPoints = 0x00000020,
            HitPoints = 0x00000040,
            PlayerConstructed = 0x00000080,
            Crafter = 0x00000100,
            Quality = 0x00000200,
            StrReq = 0x00000400,
            NegativeAttributes = 0x00000800,
            Altered = 0x00001000,
            xWeaponAttributes = 0x00002000
        }

        #region Mondain's Legacy Sets
        private static void SetSaveFlag(ref SetFlag flags, SetFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SetFlag flags, SetFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        [Flags]
        private enum SetFlag
        {
            None = 0x00000000,
            Attributes = 0x00000001,
            ArmorAttributes = 0x00000002,
            SkillBonuses = 0x00000004,
            PhysicalBonus = 0x00000008,
            FireBonus = 0x00000010,
            ColdBonus = 0x00000020,
            PoisonBonus = 0x00000040,
            EnergyBonus = 0x00000080,
            SetHue = 0x00000100,
            LastEquipped = 0x00000200,
            SetEquipped = 0x00000400,
            SetSelfRepair = 0x00000800
        }
        #endregion

        public void xWeaponAttributesDeserializeHelper(GenericReader reader, BaseClothing item)
        {
            SaveFlag flags = (SaveFlag)reader.ReadInt();

            if (flags != SaveFlag.None)
                flags = SaveFlag.xWeaponAttributes;

            if (GetSaveFlag(flags, SaveFlag.xWeaponAttributes))
                m_AosWeaponAttributes = new AosWeaponAttributes(item, reader);
            else
                m_AosWeaponAttributes = new AosWeaponAttributes(item);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(12); // version

            // Embroidery Tool version 11
            writer.Write(m_EngravedText);

            // Version 10 - removed VvV Item (handled in VvV System) and BlockRepair (Handled as negative attribute)

            writer.Write(_Owner);
            writer.Write(_OwnerName);

            //Version 8
            writer.Write(m_IsImbued);

            // Version 7
            m_SAAbsorptionAttributes.Serialize(writer);

            #region Runic Reforging
            writer.Write((int)m_ReforgedPrefix);
            writer.Write((int)m_ReforgedSuffix);
            writer.Write((int)m_ItemPower);
            #endregion

            #region Stygian Abyss
            writer.Write(m_GorgonLenseCharges);
            writer.Write((int)m_GorgonLenseType);

            writer.Write(PhysNonImbuing);
            writer.Write(FireNonImbuing);
            writer.Write(ColdNonImbuing);
            writer.Write(PoisonNonImbuing);
            writer.Write(EnergyNonImbuing);

            // Version 6
            writer.Write(m_TimesImbued);

            #endregion

            writer.Write(m_BlessedBy);

            #region Mondain's Legacy Sets
            SetFlag sflags = SetFlag.None;

            SetSaveFlag(ref sflags, SetFlag.Attributes, !m_SetAttributes.IsEmpty);
            SetSaveFlag(ref sflags, SetFlag.SkillBonuses, !m_SetSkillBonuses.IsEmpty);
            SetSaveFlag(ref sflags, SetFlag.PhysicalBonus, m_SetPhysicalBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.FireBonus, m_SetFireBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.ColdBonus, m_SetColdBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.PoisonBonus, m_SetPoisonBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.EnergyBonus, m_SetEnergyBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.SetHue, m_SetHue != 0);
            SetSaveFlag(ref sflags, SetFlag.LastEquipped, m_LastEquipped);
            SetSaveFlag(ref sflags, SetFlag.SetEquipped, m_SetEquipped);
            SetSaveFlag(ref sflags, SetFlag.SetSelfRepair, m_SetSelfRepair != 0);

            writer.WriteEncodedInt((int)sflags);

            if (GetSaveFlag(sflags, SetFlag.Attributes))
                m_SetAttributes.Serialize(writer);

            if (GetSaveFlag(sflags, SetFlag.SkillBonuses))
                m_SetSkillBonuses.Serialize(writer);

            if (GetSaveFlag(sflags, SetFlag.PhysicalBonus))
                writer.WriteEncodedInt(m_SetPhysicalBonus);

            if (GetSaveFlag(sflags, SetFlag.FireBonus))
                writer.WriteEncodedInt(m_SetFireBonus);

            if (GetSaveFlag(sflags, SetFlag.ColdBonus))
                writer.WriteEncodedInt(m_SetColdBonus);

            if (GetSaveFlag(sflags, SetFlag.PoisonBonus))
                writer.WriteEncodedInt(m_SetPoisonBonus);

            if (GetSaveFlag(sflags, SetFlag.EnergyBonus))
                writer.WriteEncodedInt(m_SetEnergyBonus);

            if (GetSaveFlag(sflags, SetFlag.SetHue))
                writer.WriteEncodedInt(m_SetHue);

            if (GetSaveFlag(sflags, SetFlag.LastEquipped))
                writer.Write(m_LastEquipped);

            if (GetSaveFlag(sflags, SetFlag.SetEquipped))
                writer.Write(m_SetEquipped);

            if (GetSaveFlag(sflags, SetFlag.SetSelfRepair))
                writer.WriteEncodedInt(m_SetSelfRepair);
            #endregion

            // Version 5
            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.xWeaponAttributes, !m_AosWeaponAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.NegativeAttributes, !m_NegativeAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Resource, m_Resource != DefaultResource);
            SetSaveFlag(ref flags, SaveFlag.Attributes, !m_AosAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.ClothingAttributes, !m_AosClothingAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SkillBonuses, !m_AosSkillBonuses.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Resistances, !m_AosResistances.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.MaxHitPoints, m_MaxHitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.HitPoints, m_HitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.PlayerConstructed, PlayerConstructed != false);
            SetSaveFlag(ref flags, SaveFlag.Crafter, m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Quality, m_Quality != ItemQuality.Normal);
            SetSaveFlag(ref flags, SaveFlag.StrReq, m_StrReq != -1);
            #region Imbuing
            //SetSaveFlag(ref flags, SaveFlag.TimesImbued, m_TimesImbued != 0);
            #endregion
            SetSaveFlag(ref flags, SaveFlag.Altered, m_Altered);

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.xWeaponAttributes))
                m_AosWeaponAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.NegativeAttributes))
                m_NegativeAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Resource))
                writer.WriteEncodedInt((int)m_Resource);

            if (GetSaveFlag(flags, SaveFlag.Attributes))
                m_AosAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.ClothingAttributes))
                m_AosClothingAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                m_AosSkillBonuses.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Resistances))
                m_AosResistances.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.MaxHitPoints))
                writer.WriteEncodedInt(m_MaxHitPoints);

            if (GetSaveFlag(flags, SaveFlag.HitPoints))
                writer.WriteEncodedInt(m_HitPoints);

            if (GetSaveFlag(flags, SaveFlag.Crafter))
                writer.Write(m_Crafter);

            if (GetSaveFlag(flags, SaveFlag.Quality))
                writer.WriteEncodedInt((int)m_Quality);

            if (GetSaveFlag(flags, SaveFlag.StrReq))
                writer.WriteEncodedInt(m_StrReq);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 12:
                case 11:
                    {
                        m_EngravedText = reader.ReadString();
                        goto case 9;
                    }
                case 10:
                case 9:
                    {
                        if (version == 9)
                            reader.ReadBool();

                        _Owner = reader.ReadMobile();
                        _OwnerName = reader.ReadString();
                        goto case 8;
                    }
                case 8:
                    {
                        m_IsImbued = reader.ReadBool();
                        goto case 7;
                    }
                case 7:
                    {
                        m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this, reader);

                        #region Runic Reforging
                        m_ReforgedPrefix = (ReforgedPrefix)reader.ReadInt();
                        m_ReforgedSuffix = (ReforgedSuffix)reader.ReadInt();
                        m_ItemPower = (ItemPower)reader.ReadInt();

                        if (version == 9 && reader.ReadBool())
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                                {
                                    m_NegativeAttributes.NoRepair = 1;
                                });
                        }
                        #endregion

                        #region Stygian Abyss
                        m_GorgonLenseCharges = reader.ReadInt();
                        m_GorgonLenseType = (LenseType)reader.ReadInt();

                        PhysNonImbuing = reader.ReadInt();
                        FireNonImbuing = reader.ReadInt();
                        ColdNonImbuing = reader.ReadInt();
                        PoisonNonImbuing = reader.ReadInt();
                        EnergyNonImbuing = reader.ReadInt();
                        goto case 6;
                    }
                case 6:
                    {
                        if (version == 6)
                            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);

                        m_TimesImbued = reader.ReadInt();

                        #endregion

                        m_BlessedBy = reader.ReadMobile();

                        #region Mondain's Legacy Sets
                        SetFlag sflags = (SetFlag)reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.Attributes))
                            m_SetAttributes = new AosAttributes(this, reader);
                        else
                            m_SetAttributes = new AosAttributes(this);

                        if (GetSaveFlag(sflags, SetFlag.ArmorAttributes))
                            m_SetSelfRepair = (new AosArmorAttributes(this, reader)).SelfRepair;

                        if (GetSaveFlag(sflags, SetFlag.SkillBonuses))
                            m_SetSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            m_SetSkillBonuses = new AosSkillBonuses(this);

                        if (GetSaveFlag(sflags, SetFlag.PhysicalBonus))
                            m_SetPhysicalBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.FireBonus))
                            m_SetFireBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.ColdBonus))
                            m_SetColdBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.PoisonBonus))
                            m_SetPoisonBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.EnergyBonus))
                            m_SetEnergyBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.SetHue))
                            m_SetHue = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.LastEquipped))
                            m_LastEquipped = reader.ReadBool();

                        if (GetSaveFlag(sflags, SetFlag.SetEquipped))
                            m_SetEquipped = reader.ReadBool();

                        if (GetSaveFlag(sflags, SetFlag.SetSelfRepair))
                            m_SetSelfRepair = reader.ReadEncodedInt();
                        #endregion

                        goto case 5;
                    }
                case 5:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if (version > 11)
                        {
                            if (GetSaveFlag(flags, SaveFlag.xWeaponAttributes))
                                m_AosWeaponAttributes = new AosWeaponAttributes(this, reader);
                            else
                                m_AosWeaponAttributes = new AosWeaponAttributes(this);
                        }

                        if (GetSaveFlag(flags, SaveFlag.NegativeAttributes))
                            m_NegativeAttributes = new NegativeAttributes(this, reader);
                        else
                            m_NegativeAttributes = new NegativeAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.Resource))
                            m_Resource = (CraftResource)reader.ReadEncodedInt();
                        else
                            m_Resource = DefaultResource;

                        if (GetSaveFlag(flags, SaveFlag.Attributes))
                            m_AosAttributes = new AosAttributes(this, reader);
                        else
                            m_AosAttributes = new AosAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.ClothingAttributes))
                            m_AosClothingAttributes = new AosArmorAttributes(this, reader);
                        else
                            m_AosClothingAttributes = new AosArmorAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                            m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            m_AosSkillBonuses = new AosSkillBonuses(this);

                        if (GetSaveFlag(flags, SaveFlag.Resistances))
                            m_AosResistances = new AosElementAttributes(this, reader);
                        else
                            m_AosResistances = new AosElementAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.MaxHitPoints))
                            m_MaxHitPoints = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.HitPoints))
                            m_HitPoints = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Crafter))
                            m_Crafter = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.Quality))
                            m_Quality = (ItemQuality)reader.ReadEncodedInt();
                        else
                            m_Quality = ItemQuality.Normal;

                        if (GetSaveFlag(flags, SaveFlag.StrReq))
                            m_StrReq = reader.ReadEncodedInt();
                        else
                            m_StrReq = -1;

                        if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                            PlayerConstructed = true;

                        if (GetSaveFlag(flags, SaveFlag.Altered))
                            m_Altered = true;

                        break;
                    }
                case 4:
                    {
                        m_Resource = (CraftResource)reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                    {
                        m_AosAttributes = new AosAttributes(this, reader);
                        m_AosClothingAttributes = new AosArmorAttributes(this, reader);
                        m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        m_AosResistances = new AosElementAttributes(this, reader);

                        goto case 2;
                    }
                case 2:
                    {
                        PlayerConstructed = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Crafter = reader.ReadMobile();
                        m_Quality = (ItemQuality)reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        m_Crafter = null;
                        m_Quality = ItemQuality.Normal;
                        break;
                    }
            }

            if (m_SetAttributes == null)
                m_SetAttributes = new AosAttributes(this);

            if (m_SetSkillBonuses == null)
                m_SetSkillBonuses = new AosSkillBonuses(this);

            if (m_AosWeaponAttributes == null)
                m_AosWeaponAttributes = new AosWeaponAttributes(this);

            if (m_MaxHitPoints == 0 && m_HitPoints == 0)
                m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            Mobile parent = Parent as Mobile;

            if (parent != null)
            {
                m_AosSkillBonuses.AddTo(parent);

                AddStatBonuses(parent);
                parent.CheckStatTimers();
            }
        }

        #endregion

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;
            else if (RootParent is Mobile && from != RootParent)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public virtual bool Scissor(Mobile from, Scissors scissors)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
                return false;
            }

            CraftSystem system = DefTailoring.CraftSystem;

            CraftItem item = system.CraftItems.SearchFor(GetType());

            if (item != null && item.Resources.Count == 1 && item.Resources.GetAt(0).Amount >= 2)
            {
                try
                {
                    Type resourceType = null;

                    CraftResourceInfo info = CraftResources.GetInfo(m_Resource);

                    if (info != null && info.ResourceTypes.Length > 0)
                        resourceType = info.ResourceTypes[0];

                    if (resourceType == null)
                        resourceType = item.Resources.GetAt(0).ItemType;

                    Item res = (Item)Activator.CreateInstance(resourceType);

                    ScissorHelper(from, res, PlayerConstructed ? (item.Resources.GetAt(0).Amount / 2) : 1);

                    res.LootType = LootType.Regular;

                    return true;
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }
            }

            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        public void DistributeBonuses(Mobile from, int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                switch (Utility.Random(5))
                {
                    case 0: ++m_AosResistances.Physical; break;
                    case 1: ++m_AosResistances.Fire; break;
                    case 2: ++m_AosResistances.Cold; break;
                    case 3: ++m_AosResistances.Poison; break;
                    case 4: ++m_AosResistances.Energy; break;
                }
            }

            // Arms Lore Bonus - Verified on EA
            if (from != null)
            {
                double div = Siege.SiegeShard ? 12.5 : 20;
                int bonus = (int)Math.Min(4, (from.Skills.ArmsLore.Value / div));

                for (int i = 0; i < bonus; i++)
                {
                    switch (Utility.Random(5))
                    {
                        case 0: Resistances.Physical++; break;
                        case 1: Resistances.Fire++; break;
                        case 2: Resistances.Cold++; break;
                        case 3: Resistances.Poison++; break;
                        case 4: Resistances.Energy++; break;
                    }
                }

                from.CheckSkill(SkillName.ArmsLore, 0, 100);
            }

            PhysNonImbuing = PhysicalResistance;
            FireNonImbuing = FireResistance;
            ColdNonImbuing = ColdResistance;
            PoisonNonImbuing = PoisonResistance;
            EnergyNonImbuing = EnergyResistance;

            InvalidateProperties();
        }

        #region ICraftable Members

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            if (!craftItem.ForceNonExceptional)
            {
                if (DefaultResource != CraftResource.None)
                {
                    Type resourceType = typeRes;

                    if (resourceType == null)
                        resourceType = craftItem.Resources.GetAt(0).ItemType;

                    Resource = CraftResources.GetFromType(resourceType);
                }
                else
                {
                    Hue = resHue;
                }
            }

            PlayerConstructed = true;

            return quality;
        }

        #endregion

        public override bool OnDragLift(Mobile from)
        {
            if (Parent is Mobile && from == Parent)
            {
                if (IsSetItem && m_SetEquipped)
                    SetHelper.RemoveSetBonus(from, SetID, this);
            }

            return base.OnDragLift(from);
        }

        public virtual SetItem SetID => SetItem.None;

        public virtual int Pieces => 0;

        public virtual bool BardMasteryBonus => (SetID == SetItem.Virtuoso);

        public virtual bool MixedSet => false;

        public bool IsSetItem => SetID == SetItem.None ? false : true;

        private int m_SetHue;
        private bool m_SetEquipped;
        private bool m_LastEquipped;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetHue
        {
            get
            {
                return m_SetHue;
            }
            set
            {
                m_SetHue = value;
                InvalidateProperties();
            }
        }

        public bool SetEquipped
        {
            get
            {
                return m_SetEquipped;
            }
            set
            {
                m_SetEquipped = value;
            }
        }

        public bool LastEquipped
        {
            get
            {
                return m_LastEquipped;
            }
            set
            {
                m_LastEquipped = value;
            }
        }

        private AosAttributes m_SetAttributes;
        private AosSkillBonuses m_SetSkillBonuses;
        private int m_SetSelfRepair;

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes SetAttributes
        {
            get
            {
                return m_SetAttributes;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SetSkillBonuses
        {
            get
            {
                return m_SetSkillBonuses;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetSelfRepair
        {
            get
            {
                return m_SetSelfRepair;
            }
            set
            {
                m_SetSelfRepair = value;
                InvalidateProperties();
            }
        }

        private int m_SetPhysicalBonus, m_SetFireBonus, m_SetColdBonus, m_SetPoisonBonus, m_SetEnergyBonus;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetPhysicalBonus
        {
            get
            {
                return m_SetPhysicalBonus;
            }
            set
            {
                m_SetPhysicalBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetFireBonus
        {
            get
            {
                return m_SetFireBonus;
            }
            set
            {
                m_SetFireBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetColdBonus
        {
            get
            {
                return m_SetColdBonus;
            }
            set
            {
                m_SetColdBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetPoisonBonus
        {
            get
            {
                return m_SetPoisonBonus;
            }
            set
            {
                m_SetPoisonBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetEnergyBonus
        {
            get
            {
                return m_SetEnergyBonus;
            }
            set
            {
                m_SetEnergyBonus = value;
                InvalidateProperties();
            }
        }

        public virtual void GetSetProperties(ObjectPropertyList list)
        {
            int prop;

            if (!m_SetEquipped)
            {
                if (SetID == SetItem.Virtuoso)
                    list.Add(1151571); // Mastery Bonus Cooldown: 15 min.

                if (m_SetPhysicalBonus != 0)
                    list.Add(1072382, m_SetPhysicalBonus.ToString()); // physical resist +~1_val~%

                if (m_SetFireBonus != 0)
                    list.Add(1072383, m_SetFireBonus.ToString()); // fire resist +~1_val~%

                if (m_SetColdBonus != 0)
                    list.Add(1072384, m_SetColdBonus.ToString()); // cold resist +~1_val~%

                if (m_SetPoisonBonus != 0)
                    list.Add(1072385, m_SetPoisonBonus.ToString()); // poison resist +~1_val~%

                if (m_SetEnergyBonus != 0)
                    list.Add(1072386, m_SetEnergyBonus.ToString()); // energy resist +~1_val~%			
            }
            else if (m_SetEquipped && SetHelper.ResistsBonusPerPiece(this) && RootParentEntity is Mobile)
            {
                Mobile m = (Mobile)RootParentEntity;

                if (m_SetPhysicalBonus != 0)
                    list.Add(1080361, SetHelper.GetSetTotalResist(m, ResistanceType.Physical).ToString()); // physical resist ~1_val~% (total)

                if (m_SetFireBonus != 0)
                    list.Add(1080362, SetHelper.GetSetTotalResist(m, ResistanceType.Fire).ToString()); // fire resist ~1_val~% (total)

                if (m_SetColdBonus != 0)
                    list.Add(1080363, SetHelper.GetSetTotalResist(m, ResistanceType.Cold).ToString()); // cold resist ~1_val~% (total)

                if (m_SetPoisonBonus != 0)
                    list.Add(1080364, SetHelper.GetSetTotalResist(m, ResistanceType.Poison).ToString()); // poison resist ~1_val~% (total)

                if (m_SetEnergyBonus != 0)
                    list.Add(1080365, SetHelper.GetSetTotalResist(m, ResistanceType.Energy).ToString()); // energy resist ~1_val~% (total)
            }

            if ((prop = m_SetSelfRepair) != 0)
                list.Add(1060450, prop.ToString()); // self repair ~1_val~		

            SetHelper.GetSetProperties(list, this);
        }

        public int SetResistBonus(ResistanceType resist)
        {
            if (SetHelper.ResistsBonusPerPiece(this))
            {
                switch (resist)
                {
                    case ResistanceType.Physical: return m_SetEquipped ? PhysicalResistance + m_SetPhysicalBonus : PhysicalResistance;
                    case ResistanceType.Fire: return m_SetEquipped ? FireResistance + m_SetFireBonus : FireResistance;
                    case ResistanceType.Cold: return m_SetEquipped ? ColdResistance + m_SetColdBonus : ColdResistance;
                    case ResistanceType.Poison: return m_SetEquipped ? PoisonResistance + m_SetPoisonBonus : PoisonResistance;
                    case ResistanceType.Energy: return m_SetEquipped ? EnergyResistance + m_SetEnergyBonus : EnergyResistance;
                }
            }
            else
            {
                switch (resist)
                {
                    case ResistanceType.Physical: return m_SetEquipped ? LastEquipped ? (PhysicalResistance * Pieces) + m_SetPhysicalBonus : 0 : PhysicalResistance;
                    case ResistanceType.Fire: return m_SetEquipped ? LastEquipped ? (FireResistance * Pieces) + m_SetFireBonus : 0 : FireResistance;
                    case ResistanceType.Cold: return m_SetEquipped ? LastEquipped ? (ColdResistance * Pieces) + m_SetColdBonus : 0 : ColdResistance;
                    case ResistanceType.Poison: return m_SetEquipped ? LastEquipped ? (PoisonResistance * Pieces) + m_SetPoisonBonus : 0 : PoisonResistance;
                    case ResistanceType.Energy: return m_SetEquipped ? LastEquipped ? (EnergyResistance * Pieces) + m_SetEnergyBonus : 0 : EnergyResistance;
                }
            }

            return 0;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Altered
        {
            get { return m_Altered; }
            set
            {
                m_Altered = value;
                InvalidateProperties();
            }
        }
    }
}
