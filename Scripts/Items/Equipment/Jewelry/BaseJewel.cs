using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Misc;

using System;
using System.Collections.Generic;

namespace Server.Items
{
    public enum GemType
    {
        None,
        StarSapphire,
        Emerald,
        Sapphire,
        Ruby,
        Citrine,
        Amethyst,
        Tourmaline,
        Amber,
        Diamond
    }

    public abstract class BaseJewel : Item, ICraftable, ISetItem, IWearableDurability, IResource, IVvVItem, IOwnerRestricted, ITalismanProtection, IArtifact, ICombatEquipment, IQuality
    {
        private int m_MaxHitPoints;
        private int m_HitPoints;

        private AosAttributes m_AosAttributes;
        private AosElementAttributes m_AosResistances;
        private AosSkillBonuses m_AosSkillBonuses;
        private SAAbsorptionAttributes m_SAAbsorptionAttributes;
        private NegativeAttributes m_NegativeAttributes;
        private CraftResource m_Resource;
        private GemType m_GemType;

        #region Stygian Abyss
        private int m_TimesImbued;
        private bool m_IsImbued;
        private int m_GorgonLenseCharges;
        private LenseType m_GorgonLenseType;
        #endregion

        #region Runic Reforging
        private ItemPower m_ItemPower;
        private ReforgedPrefix m_ReforgedPrefix;
        private ReforgedSuffix m_ReforgedSuffix;
        #endregion

        private TalismanAttribute m_TalismanProtection;

        private bool _VvVItem;
        private Mobile _Owner;
        private string _OwnerName;

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanAttribute Protection
        {
            get { return m_TalismanProtection; }
            set { m_TalismanProtection = value; InvalidateProperties(); }
        }

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

        private Mobile m_BlessedBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BlessedBy
        {
            get
            {
                return m_BlessedBy;
            }
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
            private readonly BaseJewel m_Item;

            public UnBlessEntry(Mobile from, BaseJewel item)
                : base(6208, -1)
            {
                m_From = from;
                m_Item = item;
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

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get
            {
                return m_MaxHitPoints;
            }
            set
            {
                m_MaxHitPoints = value;

                if (m_MaxHitPoints > 255)
                    m_MaxHitPoints = 255;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get
            {
                return m_HitPoints;
            }
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

        [CommandProperty(AccessLevel.Player)]
        public AosAttributes Attributes
        {
            get
            {
                return m_AosAttributes;
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
                return m_AosResistances;
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
                return m_AosSkillBonuses;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SAAbsorptionAttributes AbsorptionAttributes
        {
            get
            {
                return m_SAAbsorptionAttributes;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.Player)]
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
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public GemType GemType
        {
            get
            {
                return m_GemType;
            }
            set
            {
                GemType old = m_GemType;
                m_GemType = value;
                OnGemTypeChange(old);
                InvalidateProperties();
            }
        }

        #region SA
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

        public virtual int[] BaseResists => new int[] { 0, 0, 0, 0, 0 };

        public virtual void OnAfterImbued(Mobile m, int mod, int value)
        {
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
        public ItemPower ItemPower
        {
            get { return m_ItemPower; }
            set { m_ItemPower = value; InvalidateProperties(); }
        }
        #endregion

        public override int PhysicalResistance => m_AosResistances.Physical;
        public override int FireResistance => m_AosResistances.Fire;
        public override int ColdResistance => m_AosResistances.Cold;
        public override int PoisonResistance => m_AosResistances.Poison;
        public override int EnergyResistance => m_AosResistances.Energy;
        public virtual int BaseGemTypeNumber => 0;

        public virtual int InitMinHits => 0;
        public virtual int InitMaxHits => 0;

        public override int LabelNumber
        {
            get
            {
                if (m_GemType == GemType.None)
                    return base.LabelNumber;

                return BaseGemTypeNumber + (int)m_GemType - 1;
            }
        }

        public override double DefaultWeight
        {
            get
            {
                if (NegativeAttributes == null || NegativeAttributes.Unwieldly == 0)
                    return base.DefaultWeight;

                return 50;
            }
        }

        public override void OnAfterDuped(Item newItem)
        {
            BaseJewel jewel = newItem as BaseJewel;

            if (jewel == null)
                return;

            jewel.m_AosAttributes = new AosAttributes(newItem, m_AosAttributes);
            jewel.m_AosResistances = new AosElementAttributes(newItem, m_AosResistances);
            jewel.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
            jewel.m_NegativeAttributes = new NegativeAttributes(newItem, m_NegativeAttributes);
            jewel.m_TalismanProtection = new TalismanAttribute(m_TalismanProtection);

            #region Mondain's Legacy
            jewel.m_SetAttributes = new AosAttributes(newItem, m_SetAttributes);
            jewel.m_SetSkillBonuses = new AosSkillBonuses(newItem, m_SetSkillBonuses);
            #endregion

            jewel.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);

            base.OnAfterDuped(newItem);
        }

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

        private Mobile m_Crafter;
        private ItemQuality m_Quality;
        private bool m_PlayerConstructed;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return m_Crafter;
            }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get
            {
                return m_Quality;
            }
            set
            {
                m_Quality = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get
            {
                return m_PlayerConstructed;
            }
            set
            {
                m_PlayerConstructed = value;
                InvalidateProperties();
            }
        }

        public BaseJewel(int itemID, Layer layer)
            : base(itemID)
        {
            m_AosAttributes = new AosAttributes(this);
            m_AosResistances = new AosElementAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);
            m_Resource = CraftResource.Iron;
            m_GemType = GemType.None;

            Layer = layer;

            m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            m_SetAttributes = new AosAttributes(this);
            m_SetSkillBonuses = new AosSkillBonuses(this);
            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);
            m_NegativeAttributes = new NegativeAttributes(this);
            m_TalismanProtection = new TalismanAttribute();
        }

        #region Stygian Abyss
        public override bool CanEquip(Mobile from)
        {
            if (BlessedBy != null && BlessedBy != from)
            {
                from.SendLocalizedMessage(1075277); // That item is blessed by another player.
                return false;
            }

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
            }

            if (from.AccessLevel < AccessLevel.GameMaster && !RaceDefinitions.ValidateEquipment(from, this))
            {
                return false;
            }

            return base.CanEquip(from);
        }

        public virtual int OnHit(BaseWeapon weap, int damageTaken)
        {
            if (m_TimesImbued == 0 && m_MaxHitPoints == 0)
                return damageTaken;

            //Sanity check incase some one has a bad state Jewel.
            if (m_TimesImbued >= 1 && m_MaxHitPoints == 0)
                return damageTaken;

            double chance = NegativeAttributes.Antique > 0 ? 80 : 25;

            if (chance >= Utility.Random(100)) // 25% chance to lower durability
            {
                int wear = 1;

                if (wear > 0)
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
                                ((Mobile)Parent).LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.
                        }
                        else
                        {
                            Delete();
                        }
                    }
                }
            }

            return damageTaken;
        }

        public virtual void UnscaleDurability()
        {
        }

        public virtual void ScaleDurability()
        {
        }

        public virtual bool CanFortify => IsImbued == false && NegativeAttributes.Antique < 4;
        public virtual bool CanRepair => m_NegativeAttributes.NoRepair == 0;
        #endregion

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.AddTo(from);

                int strBonus = m_AosAttributes.BonusStr;
                int dexBonus = m_AosAttributes.BonusDex;
                int intBonus = m_AosAttributes.BonusInt;

                if (strBonus != 0 || dexBonus != 0 || intBonus != 0)
                {
                    string modName = Serial.ToString();

                    if (strBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                    if (dexBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                    if (intBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
                }

                from.CheckStatTimers();

                #region Mondain's Legacy Sets
                if (IsSetItem)
                {
                    m_SetEquipped = SetHelper.FullSetEquipped(from, SetID, Pieces);

                    if (m_SetEquipped)
                    {
                        m_LastEquipped = true;
                        SetHelper.AddSetBonus(from, SetID);
                    }
                }
                #endregion
            }
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.Remove();

                string modName = Serial.ToString();

                from.RemoveStatMod(modName + "Str");
                from.RemoveStatMod(modName + "Dex");
                from.RemoveStatMod(modName + "Int");

                from.CheckStatTimers();

                #region Mondain's Legacy Sets
                if (IsSetItem && m_SetEquipped)
                    SetHelper.RemoveSetBonus(from, SetID, this);
                #endregion
            }
        }

        public virtual void SetProtection(Type type, TextDefinition name, int amount)
        {
            m_TalismanProtection = new TalismanAttribute(type, name, amount);
        }

        public BaseJewel(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
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
            else
            {
                base.AddNameProperty(list);
            }
        }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
                name = string.Format("#{0}", LabelNumber);

            return name;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (OwnerName != null)
                list.Add(1153213, OwnerName);

            if (m_Crafter != null)
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (m_Quality == ItemQuality.Exceptional)
                list.Add(1063341); // exceptional

            if (IsImbued)
                list.Add(1080418); // (Imbued)
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            if (IsVvVItem)
                list.Add(1154937); // VvV Item
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (m_GorgonLenseCharges > 0)
                list.Add(1112590, m_GorgonLenseCharges.ToString()); //Gorgon Lens Charges: ~1_val~

            #region Mondain's Legacy Sets
            if (IsSetItem)
            {
                list.Add(1080240, Pieces.ToString()); // Part of a Jewelry Set (~1_val~ pieces)

                if (SetID == SetItem.Bestial)
                    list.Add(1151541, BestialSetHelper.GetTotalBerserk(this).ToString()); // Berserk ~1_VAL~

                if (BardMasteryBonus)
                    list.Add(1151553); // Activate: Bard Mastery Bonus x2<br>(Effect: 1 min. Cooldown: 30 min.)

                if (m_SetEquipped)
                {
                    list.Add(1080241); // Full Jewelry Set Present					
                    SetHelper.GetSetProperties(list, this);
                }
            }
            #endregion

            m_NegativeAttributes.GetProperties(list, this);
            m_AosSkillBonuses.GetProperties(list);

            int prop;

            if (RaceDefinitions.GetRequiredRace(this) == Race.Elf)
            {
                list.Add(1075086); // Elves Only
            }
            else if (RaceDefinitions.GetRequiredRace(this) == Race.Gargoyle)
            {
                list.Add(1111709); // Gargoyles Only
            }

            if ((prop = ArtifactRarity) > 0)
                list.Add(1061078, prop.ToString()); // artifact rarity ~1_val~

            if (m_TalismanProtection != null && !m_TalismanProtection.IsEmpty && m_TalismanProtection.Amount > 0)
                list.Add(1072387, "{0}\t{1}", m_TalismanProtection.Name != null ? m_TalismanProtection.Name.ToString() : "Unknown", m_TalismanProtection.Amount); // ~1_NAME~ Protection: +~2_val~%

            #region SA
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
            #endregion

            if ((prop = m_AosAttributes.SpellChanneling) != 0)
                list.Add(1060482); // spell channeling

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

            if ((prop = m_AosAttributes.IncreasedKarmaLoss) != 0)
                list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%

            base.AddResistanceProperties(list);

            if (m_HitPoints >= 0 && m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints); // durability ~1_val~ / ~2_val~

            if (IsSetItem && !m_SetEquipped)
            {
                list.Add(1072378); // <br>Only when full set is present:				
                SetHelper.GetSetProperties(list, this);
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

        public virtual void OnGemTypeChange(GemType old)
        {
        }

        public int GemLocalization()
        {
            switch (m_GemType)
            {
                default:
                case GemType.None: return 0;
                case GemType.StarSapphire: return 1023867;
                case GemType.Emerald: return 1023887;
                case GemType.Sapphire: return 1023887;
                case GemType.Ruby: return 1023868;
                case GemType.Citrine: return 1023875;
                case GemType.Amethyst: return 1023863;
                case GemType.Tourmaline: return 1023872;
                case GemType.Amber: return 1062607;
                case GemType.Diamond: return 1062608;
            }
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool drop = base.DropToWorld(from, p);

            EnchantedHotItemSocket.CheckDrop(from, this);

            return drop;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(12); // version

            // Version 12 - removed VvV Item (handled in VvV System) and BlockRepair (Handled as negative attribute)

            writer.Write(m_SetPhysicalBonus);
            writer.Write(m_SetFireBonus);
            writer.Write(m_SetColdBonus);
            writer.Write(m_SetPoisonBonus);
            writer.Write(m_SetEnergyBonus);

            writer.Write(m_PlayerConstructed);

            m_TalismanProtection.Serialize(writer);

            writer.Write(_Owner);
            writer.Write(_OwnerName);

            //Version 7
            writer.Write(m_IsImbued);

            // Version 6
            m_NegativeAttributes.Serialize(writer);

            // Version 5
            #region Region Reforging
            writer.Write((int)m_ReforgedPrefix);
            writer.Write((int)m_ReforgedSuffix);
            writer.Write((int)m_ItemPower);
            #endregion

            #region Stygian Abyss
            writer.Write(m_GorgonLenseCharges);
            writer.Write((int)m_GorgonLenseType);

            // Version 4
            writer.WriteEncodedInt(m_TimesImbued);

            m_SAAbsorptionAttributes.Serialize(writer);
            #endregion

            writer.Write(m_BlessedBy);
            writer.Write(m_LastEquipped);
            writer.Write(m_SetEquipped);
            writer.WriteEncodedInt(m_SetHue);

            m_SetAttributes.Serialize(writer);
            m_SetSkillBonuses.Serialize(writer);

            writer.Write(m_Crafter);
            writer.Write((int)m_Quality);

            // Version 3
            writer.WriteEncodedInt(m_MaxHitPoints);
            writer.WriteEncodedInt(m_HitPoints);

            writer.WriteEncodedInt((int)m_Resource);
            writer.WriteEncodedInt((int)m_GemType);

            m_AosAttributes.Serialize(writer);
            m_AosResistances.Serialize(writer);
            m_AosSkillBonuses.Serialize(writer);
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
                        m_SetPhysicalBonus = reader.ReadInt();
                        m_SetFireBonus = reader.ReadInt();
                        m_SetColdBonus = reader.ReadInt();
                        m_SetPoisonBonus = reader.ReadInt();
                        m_SetEnergyBonus = reader.ReadInt();
                        goto case 10;
                    }
                case 10:
                    {
                        m_PlayerConstructed = reader.ReadBool();
                        goto case 9;
                    }
                case 9:
                    {
                        m_TalismanProtection = new TalismanAttribute(reader);
                        goto case 8;
                    }
                case 8:
                    {
                        if (version == 11)
                            reader.ReadBool();

                        _Owner = reader.ReadMobile();
                        _OwnerName = reader.ReadString();
                        goto case 7;
                    }
                case 7:
                    {
                        m_IsImbued = reader.ReadBool();
                        goto case 6;
                    }
                case 6:
                    {
                        m_NegativeAttributes = new NegativeAttributes(this, reader);
                        goto case 5;
                    }
                case 5:
                    {
                        #region Runic Reforging
                        m_ReforgedPrefix = (ReforgedPrefix)reader.ReadInt();
                        m_ReforgedSuffix = (ReforgedSuffix)reader.ReadInt();
                        m_ItemPower = (ItemPower)reader.ReadInt();

                        if (version < 12 && reader.ReadBool())
                            m_NegativeAttributes.NoRepair = 1;
                        #endregion

                        #region Stygian Abyss
                        m_GorgonLenseCharges = reader.ReadInt();
                        m_GorgonLenseType = (LenseType)reader.ReadInt();
                        #endregion

                        goto case 4;
                    }
                case 4:
                    {
                        #region Stygian Abyss
                        m_TimesImbued = reader.ReadEncodedInt();

                        m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this, reader);
                        #endregion

                        m_BlessedBy = reader.ReadMobile();
                        m_LastEquipped = reader.ReadBool();
                        m_SetEquipped = reader.ReadBool();
                        m_SetHue = reader.ReadEncodedInt();

                        m_SetAttributes = new AosAttributes(this, reader);
                        m_SetSkillBonuses = new AosSkillBonuses(this, reader);

                        m_Crafter = reader.ReadMobile();
                        m_Quality = (ItemQuality)reader.ReadInt();
                        goto case 3;
                    }
                case 3:
                    {
                        m_MaxHitPoints = reader.ReadEncodedInt();
                        m_HitPoints = reader.ReadEncodedInt();

                        goto case 2;
                    }
                case 2:
                    {
                        m_Resource = (CraftResource)reader.ReadEncodedInt();
                        m_GemType = (GemType)reader.ReadEncodedInt();

                        goto case 1;
                    }
                case 1:
                    {
                        m_AosAttributes = new AosAttributes(this, reader);
                        m_AosResistances = new AosElementAttributes(this, reader);
                        m_AosSkillBonuses = new AosSkillBonuses(this, reader);

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

                        break;
                    }
                case 0:
                    {
                        m_AosAttributes = new AosAttributes(this);
                        m_AosResistances = new AosElementAttributes(this);
                        m_AosSkillBonuses = new AosSkillBonuses(this);

                        break;
                    }
            }

            if (m_NegativeAttributes == null)
                m_NegativeAttributes = new NegativeAttributes(this);

            if (m_TalismanProtection == null)
                m_TalismanProtection = new TalismanAttribute();

            #region Mondain's Legacy Sets
            if (m_SetAttributes == null)
                m_SetAttributes = new AosAttributes(this);

            if (m_SetSkillBonuses == null)
                m_SetSkillBonuses = new AosSkillBonuses(this);
            #endregion

            if (version < 2)
            {
                m_Resource = CraftResource.Iron;
                m_GemType = GemType.None;
            }
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            PlayerConstructed = true;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            if (!craftItem.ForceNonExceptional)
                Resource = CraftResources.GetFromType(resourceType);

            if (1 < craftItem.Resources.Count)
            {
                resourceType = craftItem.Resources.GetAt(1).ItemType;

                if (resourceType == typeof(StarSapphire))
                    GemType = GemType.StarSapphire;
                else if (resourceType == typeof(Emerald))
                    GemType = GemType.Emerald;
                else if (resourceType == typeof(Sapphire))
                    GemType = GemType.Sapphire;
                else if (resourceType == typeof(Ruby))
                    GemType = GemType.Ruby;
                else if (resourceType == typeof(Citrine))
                    GemType = GemType.Citrine;
                else if (resourceType == typeof(Amethyst))
                    GemType = GemType.Amethyst;
                else if (resourceType == typeof(Tourmaline))
                    GemType = GemType.Tourmaline;
                else if (resourceType == typeof(Amber))
                    GemType = GemType.Amber;
                else if (resourceType == typeof(Diamond))
                    GemType = GemType.Diamond;
            }

            #region Mondain's Legacy
            m_Quality = (ItemQuality)quality;

            if (makersMark)
                m_Crafter = from;
            #endregion

            return 1;
        }

        #endregion

        #region Mondain's Legacy Sets
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

        private AosAttributes m_SetAttributes;
        private AosSkillBonuses m_SetSkillBonuses;

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

        public int SetResistBonus(ResistanceType resist)
        {
            switch (resist)
            {
                case ResistanceType.Physical: return PhysicalResistance;
                case ResistanceType.Fire: return FireResistance;
                case ResistanceType.Cold: return ColdResistance;
                case ResistanceType.Poison: return PoisonResistance;
                case ResistanceType.Energy: return EnergyResistance;
            }

            return 0;
        }
        #endregion
    }
}
