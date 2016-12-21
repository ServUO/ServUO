using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Factions;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public enum ClothingQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public interface IArcaneEquip
    {
        bool IsArcane { get; }
        int CurArcaneCharges { get; set; }
        int MaxArcaneCharges { get; set; }
    }

    public abstract class BaseClothing : Item, IDyable, IScissorable, IFactionItem, ICraftable, IWearableDurability, ISetItem, IVvVItem, IOwnerRestricted
    {
        #region Factions
        private FactionItem m_FactionState;

        public FactionItem FactionItemState
        {
            get
            {
                return this.m_FactionState;
            }
            set
            {
                this.m_FactionState = value;

                if (this.m_FactionState == null)
                    this.Hue = 0;

                this.LootType = (this.m_FactionState == null ? LootType.Regular : LootType.Blessed);
            }
        }
        #endregion

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

        public virtual bool CanFortify { get { return !IsImbued && NegativeAttributes.Antique < 3; } }
        public virtual bool CanRepair { get { return m_NegativeAttributes.NoRepair == 0; } }
		public virtual bool CanAlter { get { return true; } }

        private int m_MaxHitPoints;
        private int m_HitPoints;
        private Mobile m_Crafter;
        private ClothingQuality m_Quality;
        private bool m_PlayerConstructed;
        protected CraftResource m_Resource;
        private int m_StrReq = -1;

        private bool m_Altered;

        private AosAttributes m_AosAttributes;
        private AosArmorAttributes m_AosClothingAttributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private AosElementAttributes m_AosResistances;
        private SAAbsorptionAttributes m_SAAbsorptionAttributes;
        private NegativeAttributes m_NegativeAttributes;

        #region Stygian Abyss
        private int m_TimesImbued;
        private bool m_IsImbued;
        private int m_GorgonLenseCharges;
        private LenseType m_GorgonLenseType;

        private int m_PhysImbuing;
        private int m_FireImbuing;
        private int m_ColdImbuing;
        private int m_PoisonImbuing;
        private int m_EnergyImbuing;
        #endregion

        #region Runic Reforging
        private bool m_BlockRepair;
        private ItemPower m_ItemPower;
        private ReforgedPrefix m_ReforgedPrefix;
        private ReforgedSuffix m_ReforgedSuffix;
        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get
            {
                return this.m_MaxHitPoints;
            }
            set
            {
                this.m_MaxHitPoints = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get 
            {
                return this.m_HitPoints;
            }
            set 
            {
                if (value != this.m_HitPoints && this.MaxHitPoints > 0)
                {
                    this.m_HitPoints = value;

                    if (this.m_HitPoints < 0)
                        this.Delete();
                    else if (this.m_HitPoints > this.MaxHitPoints)
                        this.m_HitPoints = this.MaxHitPoints;

                    this.InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StrRequirement
        {
            get
            {
                return (int)((double)(m_StrReq == -1 ? (Core.AOS ? AosStrReq : OldStrReq) : m_StrReq) * (m_NegativeAttributes.Massive > 0 ? 1.5 : 1));
            }
            set
            {
                this.m_StrReq = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ClothingQuality Quality
        {
            get
            {
                return this.m_Quality;
            }
            set
            {
                this.m_Quality = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get
            {
                return this.m_PlayerConstructed;
            }
            set
            {
                this.m_PlayerConstructed = value;
            }
        }

        #region Stygian Abyss
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
                if (this.TimesImbued >= 1 && !m_IsImbued)
                    m_IsImbued = true;

                return m_IsImbued;
            }
            set
            {
                if (this.TimesImbued >= 1)
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

        public int PhysImbuing
        {
            get { return m_PhysImbuing; }
            set { m_PhysImbuing = value; }
        }

        public int FireImbuing
        {
            get { return m_FireImbuing; }
            set { m_FireImbuing = value; }
        }

        public int ColdImbuing
        {
            get { return m_ColdImbuing; }
            set { m_ColdImbuing = value; }
        }

        public int PoisonImbuing
        {
            get { return m_PoisonImbuing; }
            set { m_PoisonImbuing = value; }
        }

        public int EnergyImbuing
        {
            get { return m_EnergyImbuing; }
            set { m_EnergyImbuing = value; }
        }
        #endregion

        #region Runic Reforging
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
        public bool BlockRepair
        {
            get { return m_BlockRepair; }
            set { m_BlockRepair = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemPower ItemPower
        {
            get { return m_ItemPower; }
            set { m_ItemPower = value; InvalidateProperties(); }
        }
        #endregion

        #region Personal Bless Deed
        private Mobile m_BlessedBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BlessedBy
        {
            get
            {
                return this.m_BlessedBy;
            }
            set
            {
                this.m_BlessedBy = value;
                this.InvalidateProperties();
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (this.BlessedFor == from && this.BlessedBy == from && this.RootParent == from)
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
                this.m_From = from;
                this.m_Item = item; // BaseArmor, BaseWeapon or BaseClothing
            }

            public override void OnClick()
            {
                this.m_Item.BlessedFor = null;
                this.m_Item.BlessedBy = null;

                Container pack = this.m_From.Backpack;

                if (pack != null)
                {
                    pack.DropItem(new PersonalBlessDeed(this.m_From));
                    this.m_From.SendLocalizedMessage(1062200); // A personal bless deed has been placed in your backpack.
                }
            }
        }
        #endregion

        public virtual CraftResource DefaultResource
        {
            get
            {
                return CraftResource.None;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return this.m_Resource;
            }
            set
            {
                this.m_Resource = value;
                this.Hue = CraftResources.GetHue(this.m_Resource);
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes
        {
            get
            {
                return this.m_AosAttributes;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosArmorAttributes ClothingAttributes
        {
            get
            {
                return this.m_AosClothingAttributes;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SkillBonuses
        {
            get
            {
                return this.m_AosSkillBonuses;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosElementAttributes Resistances
        {
            get
            {
                return this.m_AosResistances;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SAAbsorptionAttributes SAAbsorptionAttributes
        {
            get
            {
                return this.m_SAAbsorptionAttributes;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NegativeAttributes NegativeAttributes
        {
            get 
            { 
                return m_NegativeAttributes;
            }
            set 
            {
            }
        }

        public virtual int BasePhysicalResistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseFireResistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseColdResistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int BasePoisonResistance
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseEnergyResistance
        {
            get
            {
                return 0;
            }
        }
        
        #region Mondain's Legacy Sets
        public override int PhysicalResistance
        {
            get
            {
                return this.BasePhysicalResistance + this.m_AosResistances.Physical;
            }
        }
        public override int FireResistance
        {
            get
            {
                return this.BaseFireResistance + this.m_AosResistances.Fire;
            }
        }
        public override int ColdResistance
        {
            get
            {
                return this.BaseColdResistance + this.m_AosResistances.Cold;
            }
        }
        public override int PoisonResistance
        {
            get
            {
                return this.BasePoisonResistance + this.m_AosResistances.Poison;
            }
        }
        public override int EnergyResistance
        {
            get
            {
                return this.BaseEnergyResistance + this.m_AosResistances.Energy;
            }
        }
        #endregion

        public virtual int ArtifactRarity
        {
            get
            {
                return 0;
            }
        }

        public override bool DisplayWeight
        {
            get
            {
                if (IsVvVItem)
                    return true;

                return base.DisplayWeight;
            }
        }

        public virtual int BaseStrBonus
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseDexBonus
        {
            get
            {
                return 0;
            }
        }
        public virtual int BaseIntBonus
        {
            get
            {
                return 0;
            }
        }

        public override double DefaultWeight
        {
            get
            {
                if (NegativeAttributes == null || NegativeAttributes.Unwieldly == 0)
                    return base.DefaultWeight;

                return base.DefaultWeight * 3;
            }
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            if (!Ethics.Ethic.CheckTrade(from, to, newOwner, this))
                return false;

            return base.AllowSecureTrade(from, to, newOwner, accepted);
        }

        public virtual Race RequiredRace
        {
            get
            {
                return null;
            }
        }

        #region Stygian Abyss
        public virtual bool CanBeWornByGargoyles
        {
            get
            {
                return false;
            }
        }
        #endregion

        public override bool CanEquip(Mobile from)
        {
            if (!Ethics.Ethic.CheckEquip(from, this))
                return false;

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

                #region Stygian Abyss
                if (from.Race == Race.Gargoyle && !this.CanBeWornByGargoyles)
                {
                    from.SendLocalizedMessage(1111708); // Gargoyles can't wear this.
                    return false;
                }
                #endregion
                else if (this.RequiredRace != null && from.Race != this.RequiredRace)
                {
                    if (this.RequiredRace == Race.Elf)
                        from.SendLocalizedMessage(1072203); // Only Elves may use this.
                    else if (this.RequiredRace == Race.Gargoyle)
                        from.SendLocalizedMessage(1111707); // Only gargoyles can wear this.
                    else
                        from.SendMessage("Only {0} may use this.", this.RequiredRace.PluralName);

                    return false;
                }
                else if (!this.AllowMaleWearer && !from.Female)
                {
                    if (this.AllowFemaleWearer)
                        from.SendLocalizedMessage(1010388); // Only females can wear this.
                    else
                        from.SendMessage("You may not wear this.");

                    return false;
                }
                else if (!this.AllowFemaleWearer && from.Female)
                {
                    if (this.AllowMaleWearer)
                        from.SendLocalizedMessage(1063343); // Only males can wear this.
                    else
                        from.SendMessage("You may not wear this.");

                    return false;
                }
                #region Personal Bless Deed
                else if (this.BlessedBy != null && this.BlessedBy != from)
                {
                    from.SendLocalizedMessage(1075277); // That item is blessed by another player.

                    return false;
                }
                #endregion
                else
                {
                    int strBonus = this.ComputeStatBonus(StatType.Str);
                    int strReq = this.ComputeStatReq(StatType.Str);

                    if (from.Str < strReq || (from.Str + strBonus) < 1)
                    {
                        from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                        return false;
                    }
                }
            }

            return base.CanEquip(from);
        }

        public virtual int AosStrReq
        {
            get
            {
                return 10;
            }
        }
        public virtual int OldStrReq
        {
            get
            {
                return 0;
            }
        }

        public virtual int InitMinHits
        {
            get
            {
                return 0;
            }
        }
        public virtual int InitMaxHits
        {
            get
            {
                return 0;
            }
        }

        public virtual bool AllowMaleWearer
        {
            get
            {
                return true;
            }
        }
        public virtual bool AllowFemaleWearer
        {
            get
            {
                return true;
            }
        }
        public virtual bool CanBeBlessed
        {
            get
            {
                return true;
            }
        }

        public int ComputeStatReq(StatType type)
        {
            return AOS.Scale(this.StrRequirement, 100 - this.GetLowerStatReq());
        }

        public int ComputeStatBonus(StatType type)
        {
            if (type == StatType.Str)
                return this.BaseStrBonus + this.Attributes.BonusStr;
            else if (type == StatType.Dex)
                return this.BaseDexBonus + this.Attributes.BonusDex;
            else
                return this.BaseIntBonus + this.Attributes.BonusInt;
        }

        public virtual void AddStatBonuses(Mobile parent)
        {
            if (parent == null)
                return;

            int strBonus = this.ComputeStatBonus(StatType.Str);
            int dexBonus = this.ComputeStatBonus(StatType.Dex);
            int intBonus = this.ComputeStatBonus(StatType.Int);

            if (strBonus == 0 && dexBonus == 0 && intBonus == 0)
                return;

            string modName = this.Serial.ToString();

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

                    #region Stygian Abyss
                    if (m.Race == Race.Gargoyle && !clothing.CanBeWornByGargoyles)
                    {
                        m.SendLocalizedMessage(1111708); // Gargoyles can't wear this.
                        m.AddToBackpack(clothing);
                    }
                    #endregion

                    if (clothing.RequiredRace != null && m.Race != clothing.RequiredRace)
                    {
                        if (clothing.RequiredRace == Race.Elf)
                            m.SendLocalizedMessage(1072203); // Only Elves may use this.
                        #region Stygian Abyss
                        else if (clothing.RequiredRace == Race.Gargoyle)
                            m.SendLocalizedMessage(1111707); // Only gargoyles can wear this.
                        #endregion
                        else
                            m.SendMessage("Only {0} may use this.", clothing.RequiredRace.PluralName);

                        m.AddToBackpack(clothing);
                    }
                    else if (!clothing.AllowMaleWearer && !m.Female && m.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (clothing.AllowFemaleWearer)
                            m.SendLocalizedMessage(1010388); // Only females can wear this.
                        else
                            m.SendMessage("You may not wear this.");

                        m.AddToBackpack(clothing);
                    }
                    else if (!clothing.AllowFemaleWearer && m.Female && m.AccessLevel < AccessLevel.GameMaster)
                    {
                        if (clothing.AllowMaleWearer)
                            m.SendLocalizedMessage(1063343); // Only males can wear this.
                        else
                            m.SendMessage("You may not wear this.");

                        m.AddToBackpack(clothing);
                    }
                }
            }
        }

        public int GetLowerStatReq()
        {
            if (!Core.AOS)
                return 0;

            return this.m_AosClothingAttributes.LowerStatReq;
        }

        public override void OnAdded(object parent)
        {
            Mobile mob = parent as Mobile;

            if (mob != null)
            {
                if (Core.AOS)
                    this.m_AosSkillBonuses.AddTo(mob);

                #region Mondain's Legacy Sets
                if (this.IsSetItem)
                {
                    this.m_SetEquipped = SetHelper.FullSetEquipped(mob, this.SetID, this.Pieces);

                    if (this.m_SetEquipped)
                    {
                        this.m_LastEquipped = true;
                        SetHelper.AddSetBonus(mob, this.SetID);
                    }
                }
                #endregion

                this.AddStatBonuses(mob);
                mob.CheckStatTimers();
            }

            base.OnAdded(parent);
        }

        public override void OnRemoved(object parent)
        {
            Mobile mob = parent as Mobile;

            if (mob != null)
            {
                if (Core.AOS)
                    this.m_AosSkillBonuses.Remove();

                #region Mondain's Legacy Sets
                if (this.IsSetItem && this.m_SetEquipped)
                    SetHelper.RemoveSetBonus(mob, this.SetID, this);
                #endregion

                string modName = this.Serial.ToString();

                mob.RemoveStatMod(modName + "Str");
                mob.RemoveStatMod(modName + "Dex");
                mob.RemoveStatMod(modName + "Int");

                mob.CheckStatTimers();
            }

            base.OnRemoved(parent);
        }

        public virtual int OnHit(BaseWeapon weapon, int damageTaken)
        {
            if (m_MaxHitPoints == 0)
                return damageTaken;

            int Absorbed = Utility.RandomMinMax(1, 4);

            damageTaken -= Absorbed;

            if (damageTaken < 0) 
                damageTaken = 0;

            if (25 > Utility.Random(100)) // 25% chance to lower durability
            {
                if (Core.AOS && this.m_AosClothingAttributes.SelfRepair + (this.IsSetItem && this.m_SetEquipped ? this.m_SetSelfRepair : 0) > Utility.Random(10))
                {
                    this.HitPoints += 2;
                }
                else
                {
                    int wear;

                    if (weapon.Type == WeaponType.Bashing)
                        wear = Absorbed / 2;
                    else
                        wear = Utility.Random(2);

                    if (NegativeAttributes.Antique > 0)
                        wear *= 2;

                    if (wear > 0 && this.m_MaxHitPoints > 0)
                    {
                        if (this.m_HitPoints >= wear)
                        {
                            this.HitPoints -= wear;
                            wear = 0;
                        }
                        else
                        {
                            wear -= this.HitPoints;
                            this.HitPoints = 0;
                        }

                        if (wear > 0)
                        {
                            if (this.m_MaxHitPoints > wear)
                            {
                                this.MaxHitPoints -= wear;

                                if (this.Parent is Mobile)
                                    ((Mobile)this.Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                            }
                            else
                            {
                                this.Delete();
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
            this.Layer = layer;
            this.Hue = hue;

            this.m_Resource = this.DefaultResource;
            this.m_Quality = ClothingQuality.Regular;

            this.m_HitPoints = this.m_MaxHitPoints = Utility.RandomMinMax(this.InitMinHits, this.InitMaxHits);

            this.m_AosAttributes = new AosAttributes(this);
            this.m_AosClothingAttributes = new AosArmorAttributes(this);
            this.m_AosSkillBonuses = new AosSkillBonuses(this);
            this.m_AosResistances = new AosElementAttributes(this);
            this.m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);
            m_NegativeAttributes = new NegativeAttributes(this);

            #region Mondain's Legacy Sets
            this.m_SetAttributes = new AosAttributes(this);
            this.m_SetSkillBonuses = new AosSkillBonuses(this);
            #endregion
        }

        public override void OnAfterDuped(Item newItem)
        {
            BaseClothing clothing = newItem as BaseClothing;

            if (clothing == null)
                return;

            clothing.m_AosAttributes = new AosAttributes(newItem, this.m_AosAttributes);
            clothing.m_AosResistances = new AosElementAttributes(newItem, this.m_AosResistances);
            clothing.m_AosSkillBonuses = new AosSkillBonuses(newItem, this.m_AosSkillBonuses);
            clothing.m_AosClothingAttributes = new AosArmorAttributes(newItem, this.m_AosClothingAttributes);
            clothing.m_SAAbsorptionAttributes = new SAAbsorptionAttributes(newItem, this.m_SAAbsorptionAttributes);
            clothing.m_NegativeAttributes = new NegativeAttributes(newItem, m_NegativeAttributes);

            #region Mondain's Legacy
            clothing.m_SetAttributes = new AosAttributes(newItem, this.m_SetAttributes);
            clothing.m_SetSkillBonuses = new AosSkillBonuses(newItem, this.m_SetSkillBonuses);
            #endregion
        }

        public BaseClothing(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            if (base.AllowEquipedCast(from))
                return true;

            return (this.m_AosAttributes.SpellChanneling != 0);
        }

        public void UnscaleDurability()
        {
            int scale = 100 + this.m_AosClothingAttributes.DurabilityBonus;

            this.m_HitPoints = ((this.m_HitPoints * 100) + (scale - 1)) / scale;
            this.m_MaxHitPoints = ((this.m_MaxHitPoints * 100) + (scale - 1)) / scale;

            this.InvalidateProperties();
        }

        public void ScaleDurability()
        {
            int scale = 100 + this.m_AosClothingAttributes.DurabilityBonus;

            this.m_HitPoints = ((this.m_HitPoints * scale) + 99) / 100;
            this.m_MaxHitPoints = ((this.m_MaxHitPoints * scale) + 99) / 100;

            this.InvalidateProperties();
        }

        public override bool CheckPropertyConfliction(Mobile m)
        {
            if (base.CheckPropertyConfliction(m))
                return true;

            if (this.Layer == Layer.Pants)
                return (m.FindItemOnLayer(Layer.InnerLegs) != null);

            if (this.Layer == Layer.Shirt)
                return (m.FindItemOnLayer(Layer.InnerTorso) != null);

            return false;
        }

        private string GetNameString()
        {
            string name = this.Name;

            if (name == null)
                name = String.Format("#{0}", this.LabelNumber);

            return name;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            int oreType;

            switch ( this.m_Resource )
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
                        list.Add(1151757, String.Format("#{0}\t{1}", prefix, GetNameString())); // ~1_PREFIX~ ~2_ITEM~
                    else
                        list.Add(1151756, String.Format("#{0}\t{1}\t#{2}", prefix, GetNameString(), RunicReforging.GetSuffixName(m_ReforgedSuffix))); // ~1_PREFIX~ ~2_ITEM~ of ~3_SUFFIX~
                }
                else if (m_ReforgedSuffix != ReforgedSuffix.None)
                {
                    if (m_ReforgedSuffix == ReforgedSuffix.Blackthorn)
                        list.Add(1154548, String.Format("{0}", GetNameString())); // ~1_TYPE~ bearing the crest of Blackthorn
                    else if (m_ReforgedSuffix == ReforgedSuffix.Minax)
                        list.Add(1154507, String.Format("{0}", GetNameString())); // ~1_ITEM~ bearing the crest of Minax
                    else
                        list.Add(1151758, String.Format("{0}\t#{1}", GetNameString(), RunicReforging.GetSuffixName(m_ReforgedSuffix))); // ~1_ITEM~ of ~2_SUFFIX~
                }
            }
            else if (oreType != 0)
                list.Add(1053099, "#{0}\t{1}", oreType, this.GetNameString()); // ~1_oretype~ ~2_armortype~
            else if (this.Name == null)
                list.Add(this.LabelNumber);
            else
                list.Add(this.Name);
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            if (IsVvVItem)
                list.Add(1154937); // VvV Item
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (OwnerName != null)
            {
                list.Add(1153213, OwnerName);
            }

            #region Stygian Abyss
            if (IsImbued == true)
                list.Add(1080418); // (Imbued)

            if (m_GorgonLenseCharges > 0)
                list.Add(1112590, m_GorgonLenseCharges.ToString()); //Gorgon Lens Charges: ~1_val~
            #endregion
			
            if (this.m_Crafter != null)
				list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Altered)
                list.Add(1111880); // Altered

            #region Factions
            if (this.m_FactionState != null)
                list.Add(1041350); // faction item
            #endregion

            #region Mondain's Legacy Sets
            if (this.IsSetItem)
            {
                if (this.MixedSet)
                    list.Add(1073491, this.Pieces.ToString()); // Part of a Weapon/Armor Set (~1_val~ pieces)
                else
                    list.Add(1072376, this.Pieces.ToString()); // Part of an Armor Set (~1_val~ pieces)

                if (SetID == SetItem.Bestial)
                    list.Add(1151541, BestialSetHelper.GetTotalBerserk(this).ToString()); // Berserk ~1_VAL~

                if (this.BardMasteryBonus)
                    list.Add(1151553); // Activate: Bard Mastery Bonus x2<br>(Effect: 1 min. Cooldown: 30 min.)

                if (this.m_SetEquipped)
                {
                    if (this.MixedSet)
                        list.Add(1073492); // Full Weapon/Armor Set Present
                    else
                        list.Add(1072377); // Full Armor Set Present

                    this.GetSetProperties(list);
                }
            }
            #endregion

            if (this.m_Quality == ClothingQuality.Exceptional)
                list.Add(1060636); // exceptional

            if (this.RequiredRace == Race.Elf)
                list.Add(1075086); // Elves Only
            #region Stygian Abyss
            else if (this.RequiredRace == Race.Gargoyle)
                list.Add(1111709); // Gargoyles Only
            #endregion

            if (m_NegativeAttributes != null)
                m_NegativeAttributes.GetProperties(list, this);

            if (this.m_AosSkillBonuses != null)
                this.m_AosSkillBonuses.GetProperties(list);

            int prop;

            if ((prop = this.ArtifactRarity) > 0)
                list.Add(1061078, prop.ToString()); // artifact rarity ~1_val~

            if ((prop = this.m_AosAttributes.WeaponDamage) != 0)
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%

            if ((prop = this.m_AosAttributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = this.m_AosAttributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = this.m_AosAttributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = this.m_AosAttributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = this.m_AosAttributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = this.m_AosAttributes.AttackChance) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = this.m_AosAttributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = this.m_AosAttributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = this.m_AosAttributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = this.m_AosAttributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%

            if ((prop = this.m_AosClothingAttributes.LowerStatReq) != 0)
                list.Add(1060435, prop.ToString()); // lower requirements ~1_val~%

            if ((prop = this.m_AosAttributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = this.m_AosClothingAttributes.MageArmor) != 0)
                list.Add(1060437); // mage armor

            if ((prop = this.m_AosAttributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = this.m_AosAttributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = this.m_AosAttributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = this.m_AosAttributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = this.m_AosAttributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = this.m_AosAttributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = this.m_AosClothingAttributes.SelfRepair) != 0)
                list.Add(1060450, prop.ToString()); // self repair ~1_val~

            if ((prop = this.m_AosAttributes.SpellChanneling) != 0)
                list.Add(1060482); // spell channeling

            if ((prop = this.m_AosAttributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = this.m_AosAttributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = this.m_AosAttributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = this.m_AosAttributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            if (Core.ML && (prop = this.m_AosAttributes.IncreasedKarmaLoss) != 0)
                list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%

            #region SA
            if ((prop = this.m_SAAbsorptionAttributes.CastingFocus) != 0)
                list.Add(1113696, prop.ToString()); // Casting Focus ~1_val~%

            if ((prop = this.m_SAAbsorptionAttributes.EaterFire) != 0)
                list.Add(1113593, prop.ToString()); // Fire Eater ~1_Val~%

            if ((prop = this.m_SAAbsorptionAttributes.EaterCold) != 0)
                list.Add(1113594, prop.ToString()); // Cold Eater ~1_Val~%

            if ((prop = this.m_SAAbsorptionAttributes.EaterPoison) != 0)
                list.Add(1113595, prop.ToString()); // Poison Eater ~1_Val~%

            if ((prop = this.m_SAAbsorptionAttributes.EaterEnergy) != 0)
                list.Add(1113596, prop.ToString()); // Energy Eater ~1_Val~%

            if ((prop = this.m_SAAbsorptionAttributes.EaterKinetic) != 0)
                list.Add(1113597, prop.ToString()); // Kinetic Eater ~1_Val~%

            if ((prop = this.m_SAAbsorptionAttributes.EaterDamage) != 0)
                list.Add(1113598, prop.ToString()); // Damage Eater ~1_Val~%

            if ((prop = this.m_SAAbsorptionAttributes.ResonanceFire) != 0)
                list.Add(1113691, prop.ToString()); // Fire Resonance ~1_val~%

            if ((prop = this.m_SAAbsorptionAttributes.ResonanceCold) != 0)
                list.Add(1113692, prop.ToString()); // Cold Resonance ~1_val~%

            if ((prop = this.m_SAAbsorptionAttributes.ResonancePoison) != 0)
                list.Add(1113693, prop.ToString()); // Poison Resonance ~1_val~%

            if ((prop = this.m_SAAbsorptionAttributes.ResonanceEnergy) != 0)
                list.Add(1113694, prop.ToString()); // Energy Resonance ~1_val~%

            if ((prop = this.m_SAAbsorptionAttributes.ResonanceKinetic) != 0)
                list.Add(1113695, prop.ToString()); // Kinetic Resonance ~1_val~%
            #endregion

            base.AddResistanceProperties(list);

            if ((prop = this.m_AosClothingAttributes.DurabilityBonus) > 0)
                list.Add(1060410, prop.ToString()); // durability ~1_val~%

            if ((prop = this.ComputeStatReq(StatType.Str)) > 0)
                list.Add(1061170, prop.ToString()); // strength requirement ~1_val~

            if (this.m_HitPoints >= 0 && this.m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", this.m_HitPoints, this.m_MaxHitPoints); // durability ~1_val~ / ~2_val~

            #region Mondain's Legacy Sets
            if (this.IsSetItem && !this.m_SetEquipped)
            {
                list.Add(1072378); // <br>Only when full set is present:				
                this.GetSetProperties(list);
            }
            #endregion

            if (m_ItemPower != ItemPower.None)
            {
                if (m_ItemPower <= ItemPower.LegendaryArtifact)
                    list.Add(1151488 + ((int)m_ItemPower - 1));
                else
                    list.Add(1152281 + ((int)m_ItemPower - 9));
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

            this.AddEquipInfoAttributes(from, attrs);

            int number;

            if (this.Name == null)
            {
                number = this.LabelNumber;
            }
            else
            {
                this.LabelTo(from, this.Name);
                number = 1041000;
            }

            if (attrs.Count == 0 && this.Crafter == null && this.Name != null)
                return;

            EquipmentInfo eqInfo = new EquipmentInfo(number, this.m_Crafter, false, attrs.ToArray());

            from.Send(new DisplayEquipmentInfo(this, eqInfo));
        }

        public virtual void AddEquipInfoAttributes(Mobile from, List<EquipInfoAttribute> attrs)
        {
            if (this.DisplayLootType)
            {
                if (this.LootType == LootType.Blessed)
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                else if (this.LootType == LootType.Cursed)
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            }

            #region Factions
            if (this.m_FactionState != null)
                attrs.Add(new EquipInfoAttribute(1041350)); // faction item
            #endregion

            if (this.m_Quality == ClothingQuality.Exceptional)
                attrs.Add(new EquipInfoAttribute(1018305 - (int)this.m_Quality));
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
            NegativeAttributes  = 0x00000800,
            #region Imbuing
            //TimesImbued = 0x12000000,
            #endregion
            Altered = 0x00001000
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
            SetSelfRepair = 0x00000800,
        }
        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(9); // version

            writer.Write(_VvVItem);
            writer.Write(_Owner);
            writer.Write(_OwnerName);

            //Version 8
            writer.Write((bool)this.m_IsImbued);

            // Version 7
            m_SAAbsorptionAttributes.Serialize(writer);

            #region Runic Reforging
            writer.Write((int)m_ReforgedPrefix);
            writer.Write((int)m_ReforgedSuffix);
            writer.Write((int)m_ItemPower);
            writer.Write(m_BlockRepair);
            #endregion

            #region Stygian Abyss
            writer.Write(m_GorgonLenseCharges);
            writer.Write((int)m_GorgonLenseType);

            writer.Write(m_PhysImbuing);
            writer.Write(m_FireImbuing);
            writer.Write(m_ColdImbuing);
            writer.Write(m_PoisonImbuing);
            writer.Write(m_EnergyImbuing);

            // Version 6
            writer.Write((int)this.m_TimesImbued);

            #endregion

            writer.Write(this.m_BlessedBy);

            #region Mondain's Legacy Sets
            SetFlag sflags = SetFlag.None;

            SetSaveFlag(ref sflags, SetFlag.Attributes, !this.m_SetAttributes.IsEmpty);
            SetSaveFlag(ref sflags, SetFlag.SkillBonuses, !this.m_SetSkillBonuses.IsEmpty);
            SetSaveFlag(ref sflags, SetFlag.PhysicalBonus, this.m_SetPhysicalBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.FireBonus, this.m_SetFireBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.ColdBonus, this.m_SetColdBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.PoisonBonus, this.m_SetPoisonBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.EnergyBonus, this.m_SetEnergyBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.SetHue, this.m_SetHue != 0);
            SetSaveFlag(ref sflags, SetFlag.LastEquipped, this.m_LastEquipped);
            SetSaveFlag(ref sflags, SetFlag.SetEquipped, this.m_SetEquipped);
            SetSaveFlag(ref sflags, SetFlag.SetSelfRepair, this.m_SetSelfRepair != 0);

            writer.WriteEncodedInt((int)sflags);

            if (GetSaveFlag(sflags, SetFlag.Attributes))
                this.m_SetAttributes.Serialize(writer);

            if (GetSaveFlag(sflags, SetFlag.SkillBonuses))
                this.m_SetSkillBonuses.Serialize(writer);

            if (GetSaveFlag(sflags, SetFlag.PhysicalBonus))
                writer.WriteEncodedInt((int)this.m_SetPhysicalBonus);

            if (GetSaveFlag(sflags, SetFlag.FireBonus))
                writer.WriteEncodedInt((int)this.m_SetFireBonus);

            if (GetSaveFlag(sflags, SetFlag.ColdBonus))
                writer.WriteEncodedInt((int)this.m_SetColdBonus);

            if (GetSaveFlag(sflags, SetFlag.PoisonBonus))
                writer.WriteEncodedInt((int)this.m_SetPoisonBonus);

            if (GetSaveFlag(sflags, SetFlag.EnergyBonus))
                writer.WriteEncodedInt((int)this.m_SetEnergyBonus);

            if (GetSaveFlag(sflags, SetFlag.SetHue))
                writer.WriteEncodedInt((int)this.m_SetHue);

            if (GetSaveFlag(sflags, SetFlag.LastEquipped))
                writer.Write((bool)this.m_LastEquipped);

            if (GetSaveFlag(sflags, SetFlag.SetEquipped))
                writer.Write((bool)this.m_SetEquipped);

            if (GetSaveFlag(sflags, SetFlag.SetSelfRepair))
                writer.WriteEncodedInt((int)this.m_SetSelfRepair);
            #endregion

            // Version 5
            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.NegativeAttributes, !m_NegativeAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Resource, this.m_Resource != this.DefaultResource);
            SetSaveFlag(ref flags, SaveFlag.Attributes, !this.m_AosAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.ClothingAttributes, !this.m_AosClothingAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SkillBonuses, !this.m_AosSkillBonuses.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Resistances, !this.m_AosResistances.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.MaxHitPoints, this.m_MaxHitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.HitPoints, this.m_HitPoints != 0);
            SetSaveFlag(ref flags, SaveFlag.PlayerConstructed, this.m_PlayerConstructed != false);
            SetSaveFlag(ref flags, SaveFlag.Crafter, this.m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Quality, this.m_Quality != ClothingQuality.Regular);
            SetSaveFlag(ref flags, SaveFlag.StrReq, this.m_StrReq != -1);
            #region Imbuing
            //SetSaveFlag(ref flags, SaveFlag.TimesImbued, this.m_TimesImbued != 0);
            #endregion
            SetSaveFlag(ref flags, SaveFlag.Altered, m_Altered);

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.NegativeAttributes))
                m_NegativeAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Resource))
                writer.WriteEncodedInt((int)this.m_Resource);

            if (GetSaveFlag(flags, SaveFlag.Attributes))
                this.m_AosAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.ClothingAttributes))
                this.m_AosClothingAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                this.m_AosSkillBonuses.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.Resistances))
                this.m_AosResistances.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.MaxHitPoints))
                writer.WriteEncodedInt((int)this.m_MaxHitPoints);

            if (GetSaveFlag(flags, SaveFlag.HitPoints))
                writer.WriteEncodedInt((int)this.m_HitPoints);

            if (GetSaveFlag(flags, SaveFlag.Crafter))
                writer.Write((Mobile)this.m_Crafter);

            if (GetSaveFlag(flags, SaveFlag.Quality))
                writer.WriteEncodedInt((int)this.m_Quality);

            if (GetSaveFlag(flags, SaveFlag.StrReq))
                writer.WriteEncodedInt((int)this.m_StrReq);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 9:
                    {
                        _VvVItem = reader.ReadBool();
                        _Owner = reader.ReadMobile();
                        _OwnerName = reader.ReadString();
                        goto case 8;
                    }
                case 8:
                        {
                            this.m_IsImbued = reader.ReadBool();
                            goto case 7;
                        }
                case 7:
                    {
                        m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this, reader);

                        #region Runic Reforging
                        m_ReforgedPrefix = (ReforgedPrefix)reader.ReadInt();
                        m_ReforgedSuffix = (ReforgedSuffix)reader.ReadInt();
                        m_ItemPower = (ItemPower)reader.ReadInt();
                        m_BlockRepair = reader.ReadBool();
                        #endregion

                        #region Stygian Abyss
                        m_GorgonLenseCharges = reader.ReadInt();
                        m_GorgonLenseType = (LenseType)reader.ReadInt();

                        m_PhysImbuing = reader.ReadInt();
                        m_FireImbuing = reader.ReadInt();
                        m_ColdImbuing = reader.ReadInt();
                        m_PoisonImbuing = reader.ReadInt();
                        m_EnergyImbuing = reader.ReadInt();
                        goto case 6;
                    }
                case 6:
                    {
                        if(version == 6)
                            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);

                        this.m_TimesImbued = reader.ReadInt();
                       
                        #endregion

                        this.m_BlessedBy = reader.ReadMobile();

                        #region Mondain's Legacy Sets
                        SetFlag sflags = (SetFlag)reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.Attributes))
                            this.m_SetAttributes = new AosAttributes(this, reader);
                        else
                            this.m_SetAttributes = new AosAttributes(this);

                        if (GetSaveFlag(sflags, SetFlag.ArmorAttributes))
                            this.m_SetSelfRepair = (new AosArmorAttributes(this, reader)).SelfRepair;

                        if (GetSaveFlag(sflags, SetFlag.SkillBonuses))
                            this.m_SetSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            this.m_SetSkillBonuses = new AosSkillBonuses(this);

                        if (GetSaveFlag(sflags, SetFlag.PhysicalBonus))
                            this.m_SetPhysicalBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.FireBonus))
                            this.m_SetFireBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.ColdBonus))
                            this.m_SetColdBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.PoisonBonus))
                            this.m_SetPoisonBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.EnergyBonus))
                            this.m_SetEnergyBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.SetHue))
                            this.m_SetHue = reader.ReadEncodedInt();

                        if (GetSaveFlag(sflags, SetFlag.LastEquipped))
                            this.m_LastEquipped = reader.ReadBool();

                        if (GetSaveFlag(sflags, SetFlag.SetEquipped))
                            this.m_SetEquipped = reader.ReadBool();

                        if (GetSaveFlag(sflags, SetFlag.SetSelfRepair))
                            this.m_SetSelfRepair = reader.ReadEncodedInt();
                        #endregion

                        goto case 5;
                    }
                case 5:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.NegativeAttributes))
                            m_NegativeAttributes = new NegativeAttributes(this, reader);
                        else
                            m_NegativeAttributes = new NegativeAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.Resource))
                            this.m_Resource = (CraftResource)reader.ReadEncodedInt();
                        else
                            this.m_Resource = this.DefaultResource;

                        if (GetSaveFlag(flags, SaveFlag.Attributes))
                            this.m_AosAttributes = new AosAttributes(this, reader);
                        else
                            this.m_AosAttributes = new AosAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.ClothingAttributes))
                            this.m_AosClothingAttributes = new AosArmorAttributes(this, reader);
                        else
                            this.m_AosClothingAttributes = new AosArmorAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                            this.m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            this.m_AosSkillBonuses = new AosSkillBonuses(this);

                        if (GetSaveFlag(flags, SaveFlag.Resistances))
                            this.m_AosResistances = new AosElementAttributes(this, reader);
                        else
                            this.m_AosResistances = new AosElementAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.MaxHitPoints))
                            this.m_MaxHitPoints = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.HitPoints))
                            this.m_HitPoints = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Crafter))
                            this.m_Crafter = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.Quality))
                            this.m_Quality = (ClothingQuality)reader.ReadEncodedInt();
                        else
                            this.m_Quality = ClothingQuality.Regular;

                        if (GetSaveFlag(flags, SaveFlag.StrReq))
                            this.m_StrReq = reader.ReadEncodedInt();
                        else
                            this.m_StrReq = -1;

                        if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                            this.m_PlayerConstructed = true;

                        if (GetSaveFlag(flags, SaveFlag.Altered))
                            m_Altered = true;

                        break;
                    }
                case 4:
                    {
                        this.m_Resource = (CraftResource)reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                    {
                        this.m_AosAttributes = new AosAttributes(this, reader);
                        this.m_AosClothingAttributes = new AosArmorAttributes(this, reader);
                        this.m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        this.m_AosResistances = new AosElementAttributes(this, reader);

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_PlayerConstructed = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Crafter = reader.ReadMobile();
                        this.m_Quality = (ClothingQuality)reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        this.m_Crafter = null;
                        this.m_Quality = ClothingQuality.Regular;
                        break;
                    }
            }

            #region Mondain's Legacy Sets
            if (this.m_SetAttributes == null)
                this.m_SetAttributes = new AosAttributes(this);

            if (this.m_SetSkillBonuses == null)
                this.m_SetSkillBonuses = new AosSkillBonuses(this);
            #endregion

            if (version < 2)
                this.m_PlayerConstructed = true; // we don't know, so, assume it's crafted

            if (version < 3)
            {
                this.m_AosAttributes = new AosAttributes(this);
                this.m_AosClothingAttributes = new AosArmorAttributes(this);
                this.m_AosSkillBonuses = new AosSkillBonuses(this);
                this.m_AosResistances = new AosElementAttributes(this);
            }

            if (version < 4)
                this.m_Resource = this.DefaultResource;

            if (this.m_MaxHitPoints == 0 && this.m_HitPoints == 0)
                this.m_HitPoints = this.m_MaxHitPoints = Utility.RandomMinMax(this.InitMinHits, this.InitMaxHits);

            Mobile parent = this.Parent as Mobile;

            if (parent != null)
            {
                if (Core.AOS)
                    this.m_AosSkillBonuses.AddTo(parent);

                this.AddStatBonuses(parent);
                parent.CheckStatTimers();
            }
        }

        #endregion

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;
            else if (this.RootParent is Mobile && from != this.RootParent)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public virtual bool Scissor(Mobile from, Scissors scissors)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502437); // Items you wish to cut must be in your backpack.
                return false;
            }

            if (Ethics.Ethic.IsImbued(this))
            {
                from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
                return false;
            }

            CraftSystem system = DefTailoring.CraftSystem;

            CraftItem item = system.CraftItems.SearchFor(this.GetType());

            if (item != null && item.Resources.Count == 1 && item.Resources.GetAt(0).Amount >= 2)
            {
                try
                {
                    Type resourceType = null;

                    CraftResourceInfo info = CraftResources.GetInfo(this.m_Resource);

                    if (info != null && info.ResourceTypes.Length > 0)
                        resourceType = info.ResourceTypes[0];

                    if (resourceType == null)
                        resourceType = item.Resources.GetAt(0).ItemType;

                    Item res = (Item)Activator.CreateInstance(resourceType);

                    this.ScissorHelper(from, res, this.m_PlayerConstructed ? (item.Resources.GetAt(0).Amount / 2) : 1);

                    res.LootType = LootType.Regular;

                    return true;
                }
                catch
                {
                }
            }

            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        public void DistributeBonuses(int amount)
        {
            for (int i = 0; i < amount; ++i)
            {
                switch ( Utility.Random(5) )
                {
                    case 0:
                        ++this.m_AosResistances.Physical;
                        break;
                    case 1:
                        ++this.m_AosResistances.Fire;
                        break;
                    case 2:
                        ++this.m_AosResistances.Cold;
                        break;
                    case 3:
                        ++this.m_AosResistances.Poison;
                        break;
                    case 4:
                        ++this.m_AosResistances.Energy;
                        break;
                }
            }

            #region Stygian Abyss
            m_PhysImbuing = m_AosResistances.Physical;
            m_FireImbuing = m_AosResistances.Fire;
            m_ColdImbuing = m_AosResistances.Cold;
            m_PoisonImbuing = m_AosResistances.Poison;
            m_EnergyImbuing = m_AosResistances.Energy;
            #endregion

            this.InvalidateProperties();
        }

        #region ICraftable Members

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (ClothingQuality)quality;

            if (makersMark)
                this.Crafter = from;

            #region Mondain's Legacy
            if (!craftItem.ForceNonExceptional)
            {
                if (this.DefaultResource != CraftResource.None)
                {
                    Type resourceType = typeRes;

                    if (resourceType == null)
                        resourceType = craftItem.Resources.GetAt(0).ItemType;

                    this.Resource = CraftResources.GetFromType(resourceType);
                }
                else
                {
                    this.Hue = resHue;
                }
            }
            #endregion

            this.PlayerConstructed = true;

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
                this.Hue = 0;

            return quality;
        }

        #endregion

        #region Mondain's Legacy Sets
        public override bool OnDragLift(Mobile from)
        {
            if (this.Parent is Mobile && from == this.Parent)
            {
                if (this.IsSetItem && this.m_SetEquipped)
                    SetHelper.RemoveSetBonus(from, this.SetID, this);
            }

            return base.OnDragLift(from);
        }

        public virtual SetItem SetID
        {
            get
            {
                return SetItem.None;
            }
        }
        public virtual int Pieces
        {
            get
            {
                return 0;
            }
        }

        public virtual bool BardMasteryBonus
        {
            get
            {
                return (this.SetID == SetItem.Virtuoso);
            }
        }

        public virtual bool MixedSet
        {
            get
            {
                return false;
            }
        }

        public bool IsSetItem
        {
            get
            {
                return this.SetID == SetItem.None ? false : true;
            }
        }

        private int m_SetHue;
        private bool m_SetEquipped;
        private bool m_LastEquipped;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetHue
        {
            get
            {
                return this.m_SetHue;
            }
            set
            {
                this.m_SetHue = value;
                this.InvalidateProperties();
            }
        }

        public bool SetEquipped
        {
            get
            {
                return this.m_SetEquipped;
            }
            set
            {
                this.m_SetEquipped = value;
            }
        }

        public bool LastEquipped
        {
            get
            {
                return this.m_LastEquipped;
            }
            set
            {
                this.m_LastEquipped = value;
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
                return this.m_SetAttributes;
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
                return this.m_SetSkillBonuses;
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
                return this.m_SetSelfRepair;
            }
            set
            {
                this.m_SetSelfRepair = value;
                this.InvalidateProperties();
            }
        }

        private int m_SetPhysicalBonus, m_SetFireBonus, m_SetColdBonus, m_SetPoisonBonus, m_SetEnergyBonus;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetPhysicalBonus
        {
            get
            {
                return this.m_SetPhysicalBonus;
            }
            set
            {
                this.m_SetPhysicalBonus = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetFireBonus
        {
            get
            {
                return this.m_SetFireBonus;
            }
            set
            {
                this.m_SetFireBonus = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetColdBonus
        {
            get
            {
                return this.m_SetColdBonus;
            }
            set
            {
                this.m_SetColdBonus = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetPoisonBonus
        {
            get
            {
                return this.m_SetPoisonBonus;
            }
            set
            {
                this.m_SetPoisonBonus = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetEnergyBonus
        {
            get
            {
                return this.m_SetEnergyBonus;
            }
            set
            {
                this.m_SetEnergyBonus = value;
                this.InvalidateProperties();
            }
        }

        public virtual void GetSetProperties(ObjectPropertyList list)
        {
            int prop;

            if (!this.m_SetEquipped)
            {
                if (this.SetID == SetItem.Virtuoso)
                    list.Add(1151571); // Mastery Bonus Cooldown: 15 min.

                if (this.m_SetPhysicalBonus != 0)
                    list.Add(1072382, this.m_SetPhysicalBonus.ToString()); // physical resist +~1_val~%

                if (this.m_SetFireBonus != 0)
                    list.Add(1072383, this.m_SetFireBonus.ToString()); // fire resist +~1_val~%

                if (this.m_SetColdBonus != 0)
                    list.Add(1072384, this.m_SetColdBonus.ToString()); // cold resist +~1_val~%

                if (this.m_SetPoisonBonus != 0)
                    list.Add(1072385, this.m_SetPoisonBonus.ToString()); // poison resist +~1_val~%

                if (this.m_SetEnergyBonus != 0)
                    list.Add(1072386, this.m_SetEnergyBonus.ToString()); // energy resist +~1_val~%			
            }

            if ((prop = this.m_SetSelfRepair) != 0)
                list.Add(1060450, prop.ToString()); // self repair ~1_val~		

            SetHelper.GetSetProperties(list, this);
        }

        public int SetResistBonus(ResistanceType resist)
        {
            switch (resist)
            {
                case ResistanceType.Physical: return m_SetEquipped ? LastEquipped ? (PhysicalResistance * Pieces) + m_SetPhysicalBonus : 0 : PhysicalResistance;
                case ResistanceType.Fire: return m_SetEquipped ? LastEquipped ? (FireResistance * Pieces) + m_SetFireBonus : 0 : FireResistance;
                case ResistanceType.Cold: return m_SetEquipped ? LastEquipped ? (ColdResistance * Pieces) + m_SetColdBonus : 0 : ColdResistance;
                case ResistanceType.Poison: return m_SetEquipped ? LastEquipped ? (PoisonResistance * Pieces) + m_SetPoisonBonus : 0 : PoisonResistance;
                case ResistanceType.Energy: return m_SetEquipped ? LastEquipped ? (EnergyResistance * Pieces) + m_SetEnergyBonus : 0 : EnergyResistance;
            }

            return 0;
        }
        #endregion

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