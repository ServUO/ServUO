#region References
using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Network;
using Server.Services.Virtues;
using Server.Spells;
using Server.Spells.Bushido;
using Server.Spells.Chivalry;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Sixth;
using Server.Spells.SkillMasteries;
using Server.Spells.Spellweaving;
using Server.Misc;

using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Server.Items
{
    public interface ISlayer
    {
        SlayerName Slayer { get; set; }
        SlayerName Slayer2 { get; set; }
    }

    public abstract class BaseWeapon : Item, IWeapon, IUsesRemaining, ICraftable, ISlayer, IDurability, ISetItem, IVvVItem, IOwnerRestricted, IResource, IArtifact, ICombatEquipment, IEngravable, IQuality
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

        #region IUsesRemaining members
        private int m_UsesRemaining;
        private bool m_ShowUsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining { get { return m_UsesRemaining; } set { m_UsesRemaining = value; InvalidateProperties(); } }

        public bool ShowUsesRemaining { get { return m_ShowUsesRemaining; } set { m_ShowUsesRemaining = value; InvalidateProperties(); } }

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

        /* Weapon internals work differently now (Mar 13 2003)
        *
        * The attributes defined below default to -1.
        * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
        * If not, the attribute value itself is used. Here's the list:
        *  - MinDamage
        *  - MaxDamage
        *  - Speed
        *  - HitSound
        *  - MissSound
        *  - StrRequirement, DexRequirement, IntRequirement
        *  - WeaponType
        *  - WeaponAnimation
        *  - MaxRange
        */

        #region Var declarations
        // Instance values. These values are unique to each weapon.
        private ItemQuality m_Quality;
        private Mobile m_Crafter;
        private Poison m_Poison;
        private int m_PoisonCharges;
        private bool m_Identified;
        private int m_Hits;
        private int m_MaxHits;
        private SlayerName m_Slayer;
        private SlayerName m_Slayer2;

        private TalismanSlayerName m_Slayer3;

        private SkillMod m_MageMod, m_MysticMod;
        private CraftResource m_Resource;
        private bool m_PlayerConstructed;

        private bool m_Altered;

        private AosAttributes m_AosAttributes;
        private AosWeaponAttributes m_AosWeaponAttributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private AosElementAttributes m_AosElementDamages;
        private SAAbsorptionAttributes m_SAAbsorptionAttributes;
        private NegativeAttributes m_NegativeAttributes;
        private ExtendedWeaponAttributes m_ExtendedWeaponAttributes;

        // Overridable values. These values are provided to override the defaults which get defined in the individual weapon scripts.
        private int m_StrReq, m_DexReq, m_IntReq;
        private int m_MinDamage, m_MaxDamage;
        private int m_HitSound, m_MissSound;
        private float m_Speed;
        private int m_MaxRange;
        private SkillName m_Skill;
        private WeaponType m_Type;
        private WeaponAnimation m_Animation;

        private int m_TimesImbued;
        private bool m_IsImbued;
        private bool m_DImodded;

        private ItemPower m_ItemPower;
        private ReforgedPrefix m_ReforgedPrefix;
        private ReforgedSuffix m_ReforgedSuffix;
        #endregion

        #region Virtual Properties
        public virtual WeaponAbility PrimaryAbility => null;
        public virtual WeaponAbility SecondaryAbility => null;

        public virtual int DefMaxRange => 1;
        public virtual int DefHitSound => 0;
        public virtual int DefMissSound => 0;
        public virtual SkillName DefSkill => SkillName.Swords;
        public virtual WeaponType DefType => WeaponType.Slashing;
        public virtual WeaponAnimation DefAnimation => WeaponAnimation.Slash1H;

        public virtual int StrengthReq => 0;
        public virtual int DexterityReq => 0;
        public virtual int IntelligenceReq => 0;
        public virtual int MinDamage => 0;
        public virtual int MaxDamage => 0;
        public virtual float Speed => 0.0f;
        public virtual int AosMaxRange => DefMaxRange;
        public virtual int AosHitSound => DefHitSound;
        public virtual int AosMissSound => DefMissSound;
        public virtual SkillName AosSkill => DefSkill;
        public virtual WeaponType AosType => DefType;
        public virtual WeaponAnimation AosAnimation => DefAnimation;

        public virtual int InitMinHits => 0;
        public virtual int InitMaxHits => 0;

        public virtual bool CanFortify => !IsImbued && NegativeAttributes.Antique < 4;
        public virtual bool CanRepair => m_NegativeAttributes.NoRepair == 0;
        public virtual bool CanAlter => true;

        public override int PhysicalResistance => m_AosWeaponAttributes.ResistPhysicalBonus;
        public override int FireResistance => m_AosWeaponAttributes.ResistFireBonus;
        public override int ColdResistance => m_AosWeaponAttributes.ResistColdBonus;
        public override int PoisonResistance => m_AosWeaponAttributes.ResistPoisonBonus;
        public override int EnergyResistance => m_AosWeaponAttributes.ResistEnergyBonus;

        public override double DefaultWeight
        {
            get
            {
                if (NegativeAttributes == null || NegativeAttributes.Unwieldly == 0)
                    return base.DefaultWeight;

                return 50;
            }
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

        private class UnBlessEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly BaseWeapon m_Weapon; // BaseArmor, BaseWeapon or BaseClothing

            public UnBlessEntry(Mobile from, BaseWeapon weapon)
                : base(6208, -1)
            {
                m_From = from;
                m_Weapon = weapon;
            }

            public override void OnClick()
            {
                m_Weapon.BlessedFor = null;
                m_Weapon.BlessedBy = null;

                Container pack = m_From.Backpack;

                if (pack != null)
                {
                    pack.DropItem(new PersonalBlessDeed(m_From));
                    m_From.SendLocalizedMessage(1062200); // A personal bless deed has been placed in your backpack.
                }
            }
        }
        #endregion

        #endregion

        #region Getters & Setters
        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes { get { return m_AosAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosWeaponAttributes WeaponAttributes { get { return m_AosWeaponAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SkillBonuses { get { return m_AosSkillBonuses; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosElementAttributes AosElementDamages { get { return m_AosElementDamages; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SAAbsorptionAttributes AbsorptionAttributes { get { return m_SAAbsorptionAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public NegativeAttributes NegativeAttributes { get { return m_NegativeAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ExtendedWeaponAttributes ExtendedWeaponAttributes { get { return m_ExtendedWeaponAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ConsecratedWeaponContext ConsecratedContext { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Identified
        {
            get { return m_Identified; }
            set
            {
                m_Identified = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_Hits; }
            set
            {
                if (m_Hits == value)
                {
                    return;
                }

                if (value > m_MaxHits)
                {
                    value = m_MaxHits;
                }

                m_Hits = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHits; }
            set
            {
                m_MaxHits = value;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int PoisonCharges
        {
            get { return m_PoisonCharges; }
            set
            {
                m_PoisonCharges = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison Poison
        {
            get { return m_Poison; }
            set
            {
                m_Poison = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get { return m_Quality; }
            set
            {
                UnscaleDurability();
                UnscaleUses();
                m_Quality = value;
                ScaleDurability();
                ScaleUses();
                InvalidateProperties();
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
        public SlayerName Slayer
        {
            get { return m_Slayer; }
            set
            {
                m_Slayer = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerName Slayer2
        {
            get { return m_Slayer2; }
            set
            {
                m_Slayer2 = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanSlayerName Slayer3
        {
            get { return m_Slayer3; }
            set
            {
                m_Slayer3 = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                UnscaleDurability();
                m_Resource = value;
                Hue = CraftResources.GetHue(m_Resource);
                InvalidateProperties();
                ScaleDurability();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed { get { return m_PlayerConstructed; } set { m_PlayerConstructed = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRange
        {
            get { return (m_MaxRange == -1 ? AosMaxRange : m_MaxRange); }
            set
            {
                m_MaxRange = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponAnimation Animation { get { return (m_Animation == (WeaponAnimation)(-1) ? AosAnimation : m_Animation); } set { m_Animation = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public WeaponType Type { get { return (m_Type == (WeaponType)(-1) ? AosType : m_Type); } set { m_Type = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName Skill
        {
            get { return (m_Skill == (SkillName)(-1) ? AosSkill : m_Skill); }
            set
            {
                m_Skill = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitSound { get { return (m_HitSound == -1 ? AosHitSound : m_HitSound); } set { m_HitSound = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MissSound { get { return (m_MissSound == -1 ? AosMissSound : m_MissSound); } set { m_MissSound = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinimumDamage
        {
            get { return (m_MinDamage == -1 ? MinDamage : m_MinDamage); }
            set
            {
                m_MinDamage = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaximumDamage
        {
            get { return (m_MaxDamage == -1 ? MaxDamage : m_MaxDamage); }
            set
            {
                m_MaxDamage = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public float WeaponSpeed
        {
            get
            {
                if (m_Speed != -1)
                {
                    return m_Speed;
                }

                return Speed;
            }
            set
            {
                m_Speed = value;
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

                return m_StrReq == -1 ? StrengthReq : m_StrReq;
            }
            set
            {
                m_StrReq = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DexRequirement { get { return (m_DexReq == -1 ? DexterityReq : m_DexReq); } set { m_DexReq = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int IntRequirement { get { return (m_IntReq == -1 ? IntelligenceReq : m_IntReq); } set { m_IntReq = value; } }

        public Mobile FocusWeilder { get; set; }
        public Mobile EnchantedWeilder { get; set; }

        public int LastParryChance { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimesImbued
        {
            get { return m_TimesImbued; }
            set { m_TimesImbued = value; }
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
        public bool DImodded
        {
            get { return m_DImodded; }
            set { m_DImodded = value; }
        }

        public int[] BaseResists => new int[] { 0, 0, 0, 0, 0 };

        public virtual void OnAfterImbued(Mobile m, int mod, int value)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SearingWeapon
        {
            get { return HasSocket<SearingWeapon>(); }
            set
            {
                if (!value)
                {
                    RemoveSocket<SearingWeapon>();
                }
                else if (!SearingWeapon)
                {
                    AttachSocket(new SearingWeapon(this));
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemPower ItemPower
        {
            get { return m_ItemPower; }
            set { m_ItemPower = value; InvalidateProperties(); }
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
        #endregion

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (SearingWeapon && Parent == from)
            {
                list.Add(new SearingWeapon.ToggleExtinguishEntry(from, this));
            }

            if (BlessedFor == from && BlessedBy == from && RootParent == from)
            {
                list.Add(new UnBlessEntry(from, this));
            }
        }

        public override void OnAfterDuped(Item newItem)
        {
            base.OnAfterDuped(newItem);

            BaseWeapon weap = newItem as BaseWeapon;

            if (weap == null)
            {
                return;
            }

            weap.m_AosAttributes = new AosAttributes(newItem, m_AosAttributes);
            weap.m_AosElementDamages = new AosElementAttributes(newItem, m_AosElementDamages);
            weap.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
            weap.m_AosWeaponAttributes = new AosWeaponAttributes(newItem, m_AosWeaponAttributes);
            weap.m_NegativeAttributes = new NegativeAttributes(newItem, m_NegativeAttributes);
            weap.m_ExtendedWeaponAttributes = new ExtendedWeaponAttributes(newItem, m_ExtendedWeaponAttributes);
            weap.m_SetAttributes = new AosAttributes(newItem, m_SetAttributes);
            weap.m_SetSkillBonuses = new AosSkillBonuses(newItem, m_SetSkillBonuses);
            weap.m_SAAbsorptionAttributes = new SAAbsorptionAttributes(newItem, m_SAAbsorptionAttributes);
        }

        public virtual void UnscaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = ((m_Hits * 100) + (scale - 1)) / scale;
            m_MaxHits = ((m_MaxHits * 100) + (scale - 1)) / scale;

            InvalidateProperties();
        }

        public virtual void ScaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = ((m_Hits * scale) + 99) / 100;
            m_MaxHits = ((m_MaxHits * scale) + 99) / 100;

            if (m_MaxHits > 255)
                m_MaxHits = 255;

            if (m_Hits > 255)
                m_Hits = 255;

            InvalidateProperties();
        }

        public int GetDurabilityBonus()
        {
            int bonus = 0;

            if (m_Quality == ItemQuality.Exceptional)
            {
                bonus += 20;
            }

            bonus += m_AosWeaponAttributes.DurabilityBonus;

            if (m_Resource == CraftResource.Heartwood)
            {
                return bonus;
            }

            CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);
            CraftAttributeInfo attrInfo = null;

            if (resInfo != null)
            {
                attrInfo = resInfo.AttributeInfo;
            }

            if (attrInfo != null)
            {
                bonus += attrInfo.WeaponDurability;
            }

            return bonus;
        }

        public int GetLowerStatReq()
        {
            int v = m_AosWeaponAttributes.LowerStatReq;

            if (m_Resource == CraftResource.Heartwood)
            {
                return v;
            }

            CraftResourceInfo info = CraftResources.GetInfo(m_Resource);

            if (info != null)
            {
                CraftAttributeInfo attrInfo = info.AttributeInfo;

                if (attrInfo != null)
                {
                    v += attrInfo.WeaponLowerRequirements;
                }
            }

            if (v > 100)
            {
                v = 100;
            }

            return v;
        }

        public static void BlockEquip(Mobile m, TimeSpan duration)
        {
            if (m.BeginAction(typeof(BaseWeapon)))
            {
                new ResetEquipTimer(m, duration).Start();
            }
        }

        private class ResetEquipTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public ResetEquipTimer(Mobile m, TimeSpan duration)
                : base(duration)
            {
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                m_Mobile.EndAction(typeof(BaseWeapon));
                m_Mobile.SendLocalizedMessage(1060168); // Your confusion has passed, you may now arm a weapon!
            }
        }

        public override bool CheckConflictingLayer(Mobile m, Item item, Layer layer)
        {
            if (base.CheckConflictingLayer(m, item, layer))
            {
                return true;
            }

            if (Layer == Layer.TwoHanded && layer == Layer.OneHanded)
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500214); // You already have something in both hands.
                return true;
            }
            else if (Layer == Layer.OneHanded && layer == Layer.TwoHanded && !(item is BaseShield) && !(item is BaseEquipableLight))
            {
                m.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500215); // // You can only wield one weapon at a time.
                return true;
            }

            return false;
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
            }

            if (!RaceDefinitions.ValidateEquipment(from, this))
            {
                return false;
            }
            else if (from.Dex < DexRequirement)
            {
                from.SendLocalizedMessage(1071936); // You cannot equip that.
                return false;
            }
            else if (from.Str < AOS.Scale(StrRequirement, 100 - GetLowerStatReq()))
            {
                from.SendLocalizedMessage(500213); // You are not strong enough to equip that.
                return false;
            }
            else if (from.Int < IntRequirement)
            {
                from.SendLocalizedMessage(1071936); // You cannot equip that.
                return false;
            }
            else if (!from.CanBeginAction(typeof(BaseWeapon)))
            {
                from.SendLocalizedMessage(3000201); // You must wait to perform another action.
                return false;
            }
            else if (BlessedBy != null && BlessedBy != from)
            {
                from.SendLocalizedMessage(1075277); // That item is blessed by another player.

                return false;
            }
            else
            {
                return base.CanEquip(from);
            }
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
                {
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));
                }

                if (dexBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));
                }

                if (intBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
                }
            }

            from.NextCombatTime = Core.TickCount + (int)GetDelay(from).TotalMilliseconds;

            if (m_AosWeaponAttributes.MageWeapon != 0 && m_AosWeaponAttributes.MageWeapon != 30)
            {
                if (m_MageMod != null)
                {
                    m_MageMod.Remove();
                }

                m_MageMod = new DefaultSkillMod(SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeapon);
                from.AddSkillMod(m_MageMod);
            }

            if ((m_ExtendedWeaponAttributes.MysticWeapon != 0 && m_ExtendedWeaponAttributes.MysticWeapon != 30) || Enhancement.GetValue(from, ExtendedWeaponAttribute.MysticWeapon) > 0)
            {
                AddMysticMod(from);
            }

            InDoubleStrike = false;

            return true;
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                m_AosSkillBonuses.AddTo(from);

                if (IsSetItem)
                {
                    m_SetEquipped = SetHelper.FullSetEquipped(from, SetID, Pieces);

                    if (m_SetEquipped)
                    {
                        m_LastEquipped = true;
                        SetHelper.AddSetBonus(from, SetID);
                    }
                }

                if (HasSocket<Caddellite>())
                {
                    Caddellite.UpdateBuff(from);
                }

                if (ExtendedWeaponAttributes.Focus > 0)
                {
                    Focus.UpdateBuff(from);
                }

                from.CheckStatTimers();
                from.Delta(MobileDelta.WeaponDamage);
            }
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile m = (Mobile)parent;
                BaseWeapon weapon = m.Weapon as BaseWeapon;

                string modName = Serial.ToString();

                m.RemoveStatMod(modName + "Str");
                m.RemoveStatMod(modName + "Dex");
                m.RemoveStatMod(modName + "Int");

                if (weapon != null)
                {
                    m.NextCombatTime = Core.TickCount + (int)weapon.GetDelay(m).TotalMilliseconds;
                }

                if (m_MageMod != null)
                {
                    m_MageMod.Remove();
                    m_MageMod = null;
                }

                m_AosSkillBonuses.Remove();

                ImmolatingWeaponSpell.StopImmolating(this, (Mobile)parent);
                Spells.Mysticism.EnchantSpell.OnWeaponRemoved(this, m);

                if (FocusWeilder != null)
                    FocusWeilder = null;

                SkillMasterySpell.OnWeaponRemoved(m, this);
                ForceOfNature.Remove(m);

                if (IsSetItem && m_SetEquipped)
                {
                    SetHelper.RemoveSetBonus(m, SetID, this);
                }

                if (HasSocket<Caddellite>())
                {
                    Caddellite.UpdateBuff(m);
                }

                if (SearingWeapon)
                {
                    Server.Items.SearingWeapon.OnWeaponRemoved(this);
                }

                if (ExtendedWeaponAttributes.Focus > 0)
                {
                    Focus.UpdateBuff(m);
                }

                m.CheckStatTimers();

                m.Delta(MobileDelta.WeaponDamage);
            }

            LastParryChance = 0;
        }

        public void AddMysticMod(Mobile from)
        {
            if (m_MysticMod != null)
                m_MysticMod.Remove();

            int value = m_ExtendedWeaponAttributes.MysticWeapon;

            if (Enhancement.GetValue(from, ExtendedWeaponAttribute.MysticWeapon) > value)
                value = Enhancement.GetValue(from, ExtendedWeaponAttribute.MysticWeapon);

            m_MysticMod = new DefaultSkillMod(SkillName.Mysticism, true, -30 + value);
            from.AddSkillMod(m_MysticMod);
        }

        public void RemoveMysticMod()
        {
            if (m_MysticMod != null)
            {
                m_MysticMod.Remove();
                m_MysticMod = null;
            }
        }

        public virtual SkillName GetUsedSkill(Mobile m, bool checkSkillAttrs)
        {
            SkillName sk;

            if (checkSkillAttrs && m_AosWeaponAttributes.UseBestSkill != 0)
            {
                double swrd = m.Skills[SkillName.Swords].Value;
                double fenc = m.Skills[SkillName.Fencing].Value;
                double mcng = m.Skills[SkillName.Macing].Value;
                double val;

                sk = SkillName.Swords;
                val = swrd;

                if (fenc > val)
                {
                    sk = SkillName.Fencing;
                    val = fenc;
                }
                if (mcng > val)
                {
                    sk = SkillName.Macing;
                }
            }
            else if (m_AosWeaponAttributes.MageWeapon != 0)
            {
                if (m.Skills[SkillName.Magery].Value > m.Skills[Skill].Value)
                {
                    sk = SkillName.Magery;
                }
                else
                {
                    sk = Skill;
                }
            }
            else if (m_ExtendedWeaponAttributes.MysticWeapon != 0 || Enhancement.GetValue(m, ExtendedWeaponAttribute.MysticWeapon) > 0)
            {
                if (m.Skills[SkillName.Mysticism].Value > m.Skills[Skill].Value)
                {
                    sk = SkillName.Mysticism;
                }
                else
                {
                    sk = Skill;
                }
            }
            else
            {
                sk = Skill;

                if (sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman &&
                    m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value)
                {
                    sk = SkillName.Wrestling;
                }
            }

            return sk;
        }

        public virtual double GetAttackSkillValue(Mobile attacker, Mobile defender)
        {
            return attacker.Skills[GetUsedSkill(attacker, true)].Value;
        }

        public virtual double GetDefendSkillValue(Mobile attacker, Mobile defender)
        {
            return defender.Skills[GetUsedSkill(defender, true)].Value;
        }

        public static bool CheckAnimal(Mobile m, Type type)
        {
            return AnimalForm.UnderTransformation(m, type);
        }

        public virtual bool CheckHit(Mobile attacker, IDamageable damageable)
        {
            Mobile defender = damageable as Mobile;

            if (defender == null)
            {
                if (damageable is IDamageableItem)
                    return ((IDamageableItem)damageable).CheckHit(attacker);

                return true;
            }

            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

            Skill atkSkill = attacker.Skills[atkWeapon.Skill];
            Skill defSkill = defender.Skills[defWeapon.Skill];

            double atkValue = atkWeapon.GetAttackSkillValue(attacker, defender);
            double defValue = defWeapon.GetDefendSkillValue(attacker, defender);

            double ourValue, theirValue;

            int bonus = 0;

            if (atkValue <= -20.0)
                atkValue = -19.9;

            if (defValue <= -20.0)
                defValue = -19.9;

            bonus += AosAttributes.GetValue(attacker, AosAttribute.AttackChance);

            if (attacker is BaseCreature bc && !bc.Controlled && defender is BaseCreature bc2 && bc2.Controlled)
            {
                bonus = Math.Max(bonus, 45);
            }

            //SA Gargoyle cap is 50, else 45
            bonus = Math.Min(attacker.Race == Race.Gargoyle ? 50 : 45, bonus);

            ourValue = (atkValue + 20.0) * (100 + bonus);

            bonus = AosAttributes.GetValue(defender, AosAttribute.DefendChance);

            ForceArrow.ForceArrowInfo info = ForceArrow.GetInfo(attacker, defender);

            if (info != null && info.Defender == defender)
                bonus -= info.DefenseChanceMalus;

            int max = 45 + BaseArmor.GetRefinedDefenseChance(defender) + WhiteTigerFormSpell.GetDefenseCap(defender);

            // Defense Chance Increase = 45%
            if (bonus > max)
                bonus = max;

            theirValue = (defValue + 20.0) * (100 + bonus);

            bonus = 0;

            double chance = ourValue / (theirValue * 2.0);

            chance *= 1.0 + ((double)bonus / 100);

            if (atkWeapon is BaseThrown)
            {
                //Distance malas
                if (attacker.InRange(defender, 1))  //Close Quarters
                {
                    chance -= (.12 - Math.Min(12, (attacker.Skills[SkillName.Throwing].Value + attacker.RawDex) / 20) / 10);
                }
                else if (attacker.GetDistanceToSqrt(defender) < ((BaseThrown)atkWeapon).MinThrowRange)  //too close
                {
                    chance -= .12;
                }

                //shield penalty
                BaseShield shield = attacker.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

                if (shield != null)
                {
                    double malus = Math.Min(90, 1200 / Math.Max(1.0, attacker.Skills[SkillName.Parry].Value));

                    chance = chance - (chance * (malus / 100));
                }
            }

            if (defWeapon is BaseThrown)
            {
                BaseShield shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

                if (shield != null)
                {
                    double malus = Math.Min(90, 1200 / Math.Max(1.0, defender.Skills[SkillName.Parry].Value));

                    chance = chance + (chance * (malus / 100));
                }
            }

            if (chance < 0.02)
            {
                chance = 0.02;
            }

            if (m_AosWeaponAttributes.MageWeapon > 0 && attacker.Skills[SkillName.Magery].Value > atkSkill.Value)
                return attacker.CheckSkill(SkillName.Magery, chance);

            return attacker.CheckSkill(atkSkill.SkillName, chance);
        }

        public virtual TimeSpan GetDelay(Mobile m)
        {
            double speed = Speed;

            if (speed == 0)
            {
                return TimeSpan.FromHours(1.0);
            }

            double delayInSeconds;

            int bonus = AosAttributes.GetValue(m, AosAttribute.WeaponSpeed);

            if (bonus > 60)
            {
                bonus = 60;
            }

            double ticks;

            int stamTicks = m.Stam / 30;

            ticks = speed * 4;
            ticks = Math.Floor((ticks - stamTicks) * (100.0 / (100 + bonus)));

            // Swing speed currently capped at one swing every 1.25 seconds (5 ticks).
            if (ticks < 5)
            {
                ticks = 5;
            }

            delayInSeconds = ticks * 0.25;

            return TimeSpan.FromSeconds(delayInSeconds);
        }

        public virtual void OnBeforeSwing(Mobile attacker, IDamageable damageable)
        {
            Mobile defender = damageable as Mobile;

            WeaponAbility a = WeaponAbility.GetCurrentAbility(attacker);

            if (a != null && (!a.OnBeforeSwing(attacker, defender)))
            {
                WeaponAbility.ClearCurrentAbility(attacker);
            }

            SpecialMove move = SpecialMove.GetCurrentMove(attacker);

            if (move != null && !move.OnBeforeSwing(attacker, defender))
            {
                SpecialMove.ClearCurrentMove(attacker);
            }
        }

        public virtual TimeSpan OnSwing(Mobile attacker, IDamageable damageable)
        {
            return OnSwing(attacker, damageable, 1.0);
        }

        public virtual TimeSpan OnSwing(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            bool canSwing = !attacker.Paralyzed && !attacker.Frozen;

            if (canSwing)
            {
                Spell sp = attacker.Spell as Spell;

                canSwing = (sp == null || !sp.IsCasting || !sp.BlocksMovement);
            }

            if (canSwing && attacker.HarmfulCheck(damageable))
            {
                attacker.DisruptiveAction();

                if (attacker.NetState != null)
                {
                    attacker.Send(new Swing(0, attacker, damageable));
                }

                if (attacker is BaseCreature bc)
                {
                    WeaponAbility ab = bc.TryGetWeaponAbility();

                    if (ab != null)
                    {
                        if (bc.WeaponAbilityChance > Utility.RandomDouble())
                        {
                            WeaponAbility.SetCurrentAbility(bc, ab);
                        }
                        else
                        {
                            WeaponAbility.ClearCurrentAbility(bc);
                        }
                    }
                }

                if (CheckHit(attacker, damageable))
                {
                    OnHit(attacker, damageable, damageBonus);
                }
                else
                {
                    OnMiss(attacker, damageable);
                }
            }

            return GetDelay(attacker);
        }

        #region Sounds
        public virtual int GetHitAttackSound(Mobile attacker, Mobile defender)
        {
            int sound = attacker.GetAttackSound();

            if (sound == -1)
            {
                sound = HitSound;
            }

            return sound;
        }

        public virtual int GetHitDefendSound(Mobile attacker, Mobile defender)
        {
            return defender.GetHurtSound();
        }

        public virtual int GetMissAttackSound(Mobile attacker, Mobile defender)
        {
            if (attacker.GetAttackSound() == -1)
            {
                return MissSound;
            }
            else
            {
                return -1;
            }
        }

        public virtual int GetMissDefendSound(Mobile attacker, Mobile defender)
        {
            return -1;
        }
        #endregion

        public static bool CheckParry(Mobile defender)
        {
            if (defender == null)
            {
                return false;
            }

            BaseShield shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

            double parry = defender.Skills[SkillName.Parry].Value;
            double bushidoNonRacial = defender.Skills[SkillName.Bushido].NonRacialValue;
            double bushido = defender.Skills[SkillName.Bushido].Value;

            if (shield != null || !defender.Player)
            {
                double chance = (parry - bushidoNonRacial) / 400.0;
                // As per OSI, no negitive effect from the Racial stuffs, ie, 120 parry and '0' bushido with humans

                if (chance < 0) // chance shouldn't go below 0
                {
                    chance = defender.Player ? 0 : .1;
                }

                // Skill Masteries
                chance += HeightenedSensesSpell.GetParryBonus(defender);

                // Parry/Bushido over 100 grants a 5% bonus.
                if (parry >= 100.0 || bushido >= 100.0)
                {
                    chance += 0.05;
                }

                // Evasion grants a variable bonus post ML. 50% prior.
                if (Evasion.IsEvading(defender))
                {
                    chance *= Evasion.GetParryScalar(defender);
                }

                // Low dexterity lowers the chance.
                if (defender.Player && defender.Dex < 80)
                {
                    chance = chance * (20 + defender.Dex) / 100;
                }

                bool success = defender.CheckSkill(SkillName.Parry, chance);

                if (shield != null && success)
                {
                    shield.LastParryChance = (int)(chance * 100);
                    shield.InvalidateProperties();
                }

                return success;
            }
            else if (!(defender.Weapon is Fists) && !(defender.Weapon is BaseRanged))
            {
                BaseWeapon weapon = defender.Weapon as BaseWeapon;

                if (weapon.Attributes.BalancedWeapon > 0)
                {
                    return false;
                }

                double divisor = (weapon.Layer == Layer.OneHanded && defender.Player) ? 48000.0 : 41140.0;

                double chance = (parry * bushido) / divisor;

                double aosChance = parry / 800.0;

                // Parry or Bushido over 100 grant a 5% bonus.
                if (parry >= 100.0)
                {
                    chance += 0.05;
                    aosChance += 0.05;
                }
                else if (bushido >= 100.0)
                {
                    chance += 0.05;
                }

                // Evasion grants a variable bonus post ML. 50% prior.
                if (Evasion.IsEvading(defender))
                {
                    chance *= Evasion.GetParryScalar(defender);
                }

                // Low dexterity lowers the chance.
                if (defender.Dex < 80)
                {
                    chance = chance * (20 + defender.Dex) / 100;
                }

                bool success;

                if (chance > aosChance)
                {
                    success = defender.CheckSkill(SkillName.Parry, chance);
                }
                else
                {
                    success = (aosChance > Utility.RandomDouble());
                    // Only skillcheck if wielding a shield & there's no effect from Bushido
                }

                if (success)
                {
                    weapon.LastParryChance = (int)(chance * 100);
                    weapon.InvalidateProperties();
                }

                return success;
            }

            return false;
        }

        public virtual int AbsorbDamageAOS(Mobile attacker, Mobile defender, int damage)
        {
            int originalDamage = damage;

            bool blocked = false;

            if (defender.Player || defender.Body.IsHuman || (defender is BaseCreature &&
                                                            ((BaseCreature)defender).Controlled &&
                                                            defender.Skills[SkillName.Wrestling].Base >= 100))
            {
                blocked = CheckParry(defender);
                BaseWeapon weapon = defender.Weapon as BaseWeapon;

                if (blocked)
                {
                    defender.FixedEffect(0x37B9, 10, 16);
                    damage = 0;

                    defender.Animate(AnimationType.Parry, 0);

                    // Successful block removes the Honorable Execution penalty.
                    HonorableExecution.RemovePenalty(defender);

                    if (CounterAttack.IsCountering(defender))
                    {
                        if (weapon != null)
                        {
                            IDamageable combatant = defender.Combatant;

                            defender.FixedParticles(0x3779, 1, 15, 0x158B, 0x0, 0x3, EffectLayer.Waist);
                            weapon.OnSwing(defender, attacker);

                            if (combatant != null && defender.Combatant != combatant && combatant.Alive)
                                defender.Combatant = combatant;
                        }

                        CounterAttack.StopCountering(defender);
                    }

                    if (Confidence.IsConfident(defender))
                    {
                        defender.SendLocalizedMessage(1063117);
                        // Your confidence reassures you as you successfully block your opponent's blow.

                        double bushido = defender.Skills.Bushido.Value;

                        defender.Hits += Utility.RandomMinMax(1, (int)(bushido / 12)) + MasteryInfo.AnticipateHitBonus(defender) / 10;
                        defender.Stam += Utility.RandomMinMax(1, (int)(bushido / 5)) + MasteryInfo.AnticipateHitBonus(defender) / 10;
                    }

                    BaseShield shield = defender.FindItemOnLayer(Layer.TwoHanded) as BaseShield;

                    if (shield != null)
                    {
                        shield.OnHit(this, damage);

                        #region Stygian Abyss
                        if (shield.ArmorAttributes.ReactiveParalyze > 0 && 30 > Utility.Random(100))
                        {
                            int secs = (int)Math.Max(1, Math.Max(8, defender.Skills[SkillName.EvalInt].Value / 10) - attacker.Skills[SkillName.MagicResist].Value / 10);

                            attacker.Paralyze(TimeSpan.FromSeconds(secs));
                            attacker.PlaySound(0x204);
                            attacker.FixedEffect(0x376A, 6, 1);
                        }
                        #endregion
                    }

                    #region Stygian Abyss
                    else if (weapon != null && weapon.Layer == Layer.TwoHanded && weapon.WeaponAttributes.ReactiveParalyze > 0 && 30 > Utility.Random(100))
                    {
                        int secs = (int)Math.Max(1, Math.Max(8, defender.Skills[SkillName.EvalInt].Value / 10) - attacker.Skills[SkillName.MagicResist].Value / 10);

                        attacker.Paralyze(TimeSpan.FromSeconds(secs));
                        attacker.PlaySound(0x204);
                        attacker.FixedEffect(0x376A, 6, 1);
                    }
                    #endregion

                    //Skill Masteries
                    SkillMasterySpell.OnParried(attacker, defender);
                }
            }

            if (!blocked)
            {
                IWearableDurability toHit = GetRandomValidItem(defender) as IWearableDurability;

                if (toHit != null)
                {
                    toHit.OnHit(this, damage); // call OnHit to lose durability

                    if (toHit is Item && !((Item)toHit).Deleted && (attacker is VeriteElemental || attacker is ValoriteElemental))
                        VeriteElemental.OnHit(defender, (Item)toHit, damage);
                }
            }

            return damage;
        }

        private Item GetRandomValidItem(Mobile m)
        {
            Item[] items = m.Items.Where(item => _DamageLayers.Contains(item.Layer) && item is IWearableDurability).ToArray();

            if (items.Length == 0)
                return null;

            return items[Utility.Random(items.Length)];
        }

        private readonly List<Layer> _DamageLayers = new List<Layer>()
        {
            Layer.FirstValid,
            Layer.OneHanded,
            Layer.TwoHanded,
            Layer.Shoes,
            Layer.Pants,
            Layer.Shirt,
            Layer.Helm,
            Layer.Arms,
            Layer.Gloves,
            Layer.Ring,
            Layer.Talisman,
            Layer.Neck,
            Layer.Waist,
            Layer.InnerTorso,
            Layer.Bracelet,
            Layer.MiddleTorso,
            Layer.Earrings,
            Layer.Cloak,
            Layer.OuterTorso,
            Layer.OuterLegs,
            Layer.InnerLegs,
        };

        public virtual int AbsorbDamage(Mobile attacker, Mobile defender, int damage)
        {
            return AbsorbDamageAOS(attacker, defender, damage);
        }

        public virtual int GetPackInstinctBonus(Mobile attacker, Mobile defender)
        {
            if (attacker.Player || defender.Player)
            {
                return 0;
            }

            BaseCreature bc = attacker as BaseCreature;

            if (bc == null || bc.PackInstinct == PackInstinct.None || (!bc.Controlled && !bc.Summoned))
            {
                return 0;
            }

            Mobile master = bc.ControlMaster;

            if (master == null)
            {
                master = bc.SummonMaster;
            }

            if (master == null)
            {
                return 0;
            }

            int inPack = 1;

            IPooledEnumerable eable = defender.GetMobilesInRange(1);

            foreach (Mobile m in eable)
            {
                if (m != attacker && m is BaseCreature)
                {
                    BaseCreature tc = (BaseCreature)m;

                    if ((tc.PackInstinct & bc.PackInstinct) == 0 || (!tc.Controlled && !tc.Summoned))
                    {
                        continue;
                    }

                    Mobile theirMaster = tc.ControlMaster;

                    if (theirMaster == null)
                    {
                        theirMaster = tc.SummonMaster;
                    }

                    if (master == theirMaster && tc.Combatant == defender)
                    {
                        ++inPack;
                    }
                }
            }

            eable.Free();

            if (inPack >= 5)
            {
                return 100;
            }
            else if (inPack >= 4)
            {
                return 75;
            }
            else if (inPack >= 3)
            {
                return 50;
            }
            else if (inPack >= 2)
            {
                return 25;
            }

            return 0;
        }

        private bool m_InDoubleStrike;
        private bool m_ProcessingMultipleHits;

        public bool InDoubleStrike
        {
            get { return m_InDoubleStrike; }
            set
            {
                m_InDoubleStrike = value;

                if (m_InDoubleStrike)
                    ProcessingMultipleHits = true;
                else
                    ProcessingMultipleHits = false;
            }
        }

        public bool ProcessingMultipleHits
        {
            get { return m_ProcessingMultipleHits; }
            set
            {
                m_ProcessingMultipleHits = value;

                if (!m_ProcessingMultipleHits)
                    BlockHitEffects = false;
            }
        }

        public bool EndDualWield { get; set; }
        public bool BlockHitEffects { get; set; }
        public DateTime NextSelfRepair { get; set; }

        public void OnHit(Mobile attacker, IDamageable damageable)
        {
            OnHit(attacker, damageable, 1.0);
        }

        public virtual void OnHit(Mobile attacker, IDamageable damageable, double damageBonus)
        {
            if (EndDualWield)
            {
                ProcessingMultipleHits = false;
                EndDualWield = false;
            }

            Mobile defender = damageable as Mobile;
            Clone clone = null;

            if (defender != null)
            {
                clone = MirrorImage.GetDeflect(attacker, defender);
            }

            if (clone != null)
            {
                defender = clone;
            }

            PlaySwingAnimation(attacker);

            if (defender != null)
                PlayHurtAnimation(defender);

            attacker.PlaySound(GetHitAttackSound(attacker, defender));

            if (defender != null)
                defender.PlaySound(GetHitDefendSound(attacker, defender));

            int damage = ComputeDamage(attacker, defender);

            WeaponAbility a = WeaponAbility.GetCurrentAbility(attacker);
            SpecialMove move = SpecialMove.GetCurrentMove(attacker);

            bool ranged = this is BaseRanged;
            int phys, fire, cold, pois, nrgy, chaos, direct;

            if (a is MovingShot || SkillMasterySpell.HasSpell<ShieldBashSpell>(attacker))
            {
                phys = 100;
                fire = cold = pois = nrgy = chaos = direct = 0;
            }
            else
            {
                GetDamageTypes(attacker, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

                if (!OnslaughtSpell.HasOnslaught(attacker, defender) &&
                    ConsecratedContext != null &&
                    ConsecratedContext.Owner == attacker &&
                    ConsecratedContext.ConsecrateProcChance >= Utility.Random(100))
                {
                    phys = damageable.PhysicalResistance;
                    fire = damageable.FireResistance;
                    cold = damageable.ColdResistance;
                    pois = damageable.PoisonResistance;
                    nrgy = damageable.EnergyResistance;

                    int low = phys, type = 0;

                    if (fire < low) { low = fire; type = 1; }
                    if (cold < low) { low = cold; type = 2; }
                    if (pois < low) { low = pois; type = 3; }
                    if (nrgy < low) { low = nrgy; type = 4; }

                    phys = fire = cold = pois = nrgy = chaos = direct = 0;

                    if (type == 0) phys = 100;
                    else if (type == 1) fire = 100;
                    else if (type == 2) cold = 100;
                    else if (type == 3) pois = 100;
                    else if (type == 4) nrgy = 100;
                }
                else if (ranged)
                {
                    IRangeDamage rangeDamage = attacker.FindItemOnLayer(Layer.Cloak) as IRangeDamage;

                    if (rangeDamage != null)
                    {
                        rangeDamage.AlterRangedDamage(ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct);
                    }
                }
            }

            bool splintering = false;

            if (m_AosWeaponAttributes.SplinteringWeapon > 0 && m_AosWeaponAttributes.SplinteringWeapon > Utility.Random(100))
            {
                if (SplinteringWeaponContext.CheckHit(attacker, defender, a, this))
                    splintering = true;
            }

            double chance = NegativeAttributes.Antique > 0 ? 5 : 0;
            bool acidicTarget = MaxRange <= 1 && m_AosAttributes.SpellChanneling == 0 && !(this is Fists) && (defender is Slime || defender is ToxicElemental || defender is CorrosiveSlime);

            if (acidicTarget || (defender != null && splintering) || Utility.Random(40) <= chance)
            {
                if (MaxRange <= 1 && acidicTarget)
                {
                    attacker.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500263); // *Acid blood scars your weapon!*
                }

                int selfRepair = m_AosWeaponAttributes.SelfRepair + (IsSetItem && m_SetEquipped ? m_SetSelfRepair : 0);

                if (selfRepair > 0 && NextSelfRepair < DateTime.UtcNow)
                {
                    HitPoints += selfRepair;

                    NextSelfRepair = DateTime.UtcNow + TimeSpan.FromSeconds(60);
                }
                else
                {
                    if (m_MaxHits > 0)
                    {
                        if (m_Hits >= 1)
                        {
                            if (splintering)
                            {
                                HitPoints = Math.Max(0, HitPoints - 10);
                            }
                            else
                            {
                                HitPoints--;
                            }
                        }
                        else if (m_MaxHits > 0)
                        {
                            MaxHitPoints--;

                            if (Parent is Mobile)
                                ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.

                            if (m_MaxHits <= 0)
                                Delete();
                        }
                    }
                }
            }

            WeaponAbility weavabil;
            bool bladeweaving = Bladeweave.BladeWeaving(attacker, out weavabil);
            bool ignoreArmor = (a is ArmorIgnore || (move != null && move.IgnoreArmor(attacker)) || (bladeweaving && weavabil is ArmorIgnore));

            // object is not a mobile, so we end here
            if (defender == null)
            {
                AOS.Damage(damageable, attacker, damage, ignoreArmor, phys, fire, cold, pois, nrgy, chaos, direct, false, ranged ? Server.DamageType.Ranged : Server.DamageType.Melee);

                // TODO: WeaponAbility/SpecialMove OnHit(...) convert target to IDamageable
                // Figure out which specials work on items. For now AI only.
                if (ignoreArmor)
                {
                    Effects.PlaySound(damageable.Location, damageable.Map, 0x56);
                    Effects.SendTargetParticles(damageable, 0x3728, 200, 25, 0, 0, 9942, EffectLayer.Waist, 0);
                }

                WeaponAbility.ClearCurrentAbility(attacker);
                SpecialMove.ClearCurrentMove(attacker);

                if (WeaponAttributes.HitLeechHits > 0)
                {
                    attacker.SendLocalizedMessage(1152566); // You fail to leech life from your target!
                }

                return;
            }

            #region Damage Multipliers
            /*
            * The following damage bonuses multiply damage by a factor.
            * Capped at x3 (300%).
            */
            int percentageBonus = 0;

            if (a != null)
            {
                percentageBonus += (int)(a.DamageScalar * 100) - 100;
            }

            if (move != null)
            {
                percentageBonus += (int)(move.GetDamageScalar(attacker, defender) * 100) - 100;
            }

            percentageBonus += (int)(ForceOfNature.GetDamageScalar(attacker, defender) * 100) - 100;

            if (ConsecratedContext != null && ConsecratedContext.Owner == attacker)
            {
                percentageBonus += ConsecratedContext.ConsecrateDamageBonus;
            }

            percentageBonus += (int)(damageBonus * 100) - 100;

            CheckSlayerResult cs1 = CheckSlayers(attacker, defender, Slayer);
            CheckSlayerResult cs2 = CheckSlayers(attacker, defender, Slayer2);
            CheckSlayerResult suit = CheckSlayers(attacker, defender, SetHelper.GetSetSlayer(attacker));
            CheckSlayerResult tal = CheckTalismanSlayer(attacker, defender);

            if (cs1 == CheckSlayerResult.None && cs2 == CheckSlayerResult.None)
            {
                cs1 = CheckSlayers(attacker, defender, SlayerSocket.GetSlayer(this));
            }

            if (cs1 != CheckSlayerResult.None)
            {
                if (cs1 == CheckSlayerResult.SuperSlayer)
                    percentageBonus += 100;
                else if (cs1 == CheckSlayerResult.Slayer)
                    percentageBonus += 200;
            }

            if (cs2 != CheckSlayerResult.None)
            {
                if (cs2 == CheckSlayerResult.SuperSlayer)
                    percentageBonus += 100;
                else if (cs2 == CheckSlayerResult.Slayer)
                    percentageBonus += 200;
            }

            if (suit != CheckSlayerResult.None)
            {
                percentageBonus += 100;
            }

            if (tal != CheckSlayerResult.None)
            {
                percentageBonus += 100;
            }

            if (CheckSlayerOpposition(attacker, defender) != CheckSlayerResult.None)
            {
                percentageBonus += 100;
                defender.FixedEffect(0x37B9, 10, 5);
            }
            else if (cs1 != CheckSlayerResult.None || cs2 != CheckSlayerResult.None || suit != CheckSlayerResult.None || tal != CheckSlayerResult.None)
            {
                defender.FixedEffect(0x37B9, 10, 5);
            }

            #region Enemy of One
            EnemyOfOneContext enemyOfOneContext = EnemyOfOneSpell.GetContext(defender);

            if (enemyOfOneContext != null && !enemyOfOneContext.IsWaitingForEnemy && !enemyOfOneContext.IsEnemy(attacker))
            {
                percentageBonus += 100;
            }
            else
            {
                enemyOfOneContext = EnemyOfOneSpell.GetContext(attacker);

                if (enemyOfOneContext != null)
                {
                    enemyOfOneContext.OnHit(defender);

                    if (enemyOfOneContext.IsEnemy(defender))
                    {
                        defender.FixedEffect(0x37B9, 10, 5, 1160, 0);
                        percentageBonus += enemyOfOneContext.DamageScalar;
                    }
                }
            }
            #endregion

            int packInstinctBonus = GetPackInstinctBonus(attacker, defender);

            if (packInstinctBonus != 0)
            {
                percentageBonus += packInstinctBonus;
            }

            if (m_InDoubleStrike)
            {
                percentageBonus -= 10;
            }

            TransformContext context = TransformationSpellHelper.GetContext(defender);

            if ((m_Slayer == SlayerName.Silver || m_Slayer2 == SlayerName.Silver || SetHelper.GetSetSlayer(attacker) == SlayerName.Silver)
                && ((context != null && context.Spell is NecromancerSpell && context.Type != typeof(HorrificBeastSpell))
                || (defender is BaseCreature && (defender.Body == 747 || defender.Body == 748 || defender.Body == 749 || defender.Hue == 0x847E))))
            {
                // Every necromancer transformation other than horrific beast takes an additional 25% damage
                percentageBonus += 25;
            }

            if (attacker is PlayerMobile && !(defender is PlayerMobile))
            {
                PlayerMobile pmAttacker = (PlayerMobile)attacker;

                if (pmAttacker.HonorActive && pmAttacker.InRange(defender, 1))
                {
                    percentageBonus += 25;
                }

                if (pmAttacker.SentHonorContext != null && pmAttacker.SentHonorContext.Target == defender)
                {
                    percentageBonus += pmAttacker.SentHonorContext.PerfectionDamageBonus;
                }
            }

            percentageBonus -= Block.GetMeleeReduction(defender);

            percentageBonus += BattleLust.GetBonus(attacker, defender);

            if (this is BaseThrown)
            {
                double dist = attacker.GetDistanceToSqrt(defender);
                int max = ((BaseThrown)this).MaxThrowRange;

                if (dist > max)
                    percentageBonus -= 47;
            }

            if (RunedSashOfWarding.IsUnderEffects(defender, WardingEffect.WeaponDamage))
                percentageBonus -= 10;

            BaseTalisman talisman = attacker.Talisman as BaseTalisman;

            if (talisman != null && talisman.Killer != null)
            {
                percentageBonus += talisman.Killer.DamageBonus(defender);
            }

            if (this is ButchersWarCleaver)
            {
                if (defender is Bull || defender is Cow || defender is Gaman)
                {
                    percentageBonus += 100;
                }
            }

            if (m_ExtendedWeaponAttributes.AssassinHoned > 0 && GetOppositeDir(attacker.Direction) == defender.Direction)
            {
                if (!ranged || 0.5 > Utility.RandomDouble())
                {
                    percentageBonus += (int)(146.0 / Speed);
                }
            }

            if (m_ExtendedWeaponAttributes.Focus > 0)
            {
                percentageBonus += Focus.GetBonus(attacker, defender);
                Focus.OnHit(attacker, defender);
            }

            percentageBonus = Math.Min(percentageBonus, 300);

            // bonus is seprate from weapon damage, ie not capped
            percentageBonus += Spells.Mysticism.StoneFormSpell.GetMaxResistBonus(attacker);

            damage = AOS.Scale(damage, 100 + percentageBonus);
            #endregion

            damage = AbsorbDamage(attacker, defender, damage);

            if (damage == 0) // parried
            {
                if ((a != null && a.Validate(attacker)) || (move != null && move.Validate(attacker)))
                {
                    if (a != null && a.CheckMana(attacker, true))
                    {
                        WeaponAbility.ClearCurrentAbility(attacker);
                        SpecialMove.ClearCurrentMove(attacker);

                        attacker.SendLocalizedMessage(1061140); // Your attack was parried!
                    }
                }

                return;
            }

            // Skill Masteries
            if (WhiteTigerFormSpell.CheckEvasion(defender))
            {
                defender.Emote("*evades*"); // Is this right?
                return;
            }

            #region Mondain's Legacy
            if (ImmolatingWeaponSpell.IsImmolating(attacker, this))
            {
                int d = ImmolatingWeaponSpell.GetImmolatingDamage(attacker);
                d = AOS.Damage(defender, attacker, d, 0, 100, 0, 0, 0);

                ImmolatingWeaponSpell.DoDelayEffect(attacker, defender);

                if (d > 0)
                {
                    defender.Damage(d);
                }
            }
            #endregion

            #region SA
            if (defender != null && Server.Items.SearingWeapon.CanSear(this) && attacker.Mana > 0)
            {
                int d = SearingWeaponContext.Damage;

                if ((ranged && 10 > Utility.Random(100)) || 20 > Utility.Random(100))
                {
                    AOS.Damage(defender, attacker, d, 0, 100, 0, 0, 0);
                    AOS.Damage(attacker, null, 4, false, 0, 0, 0, 0, 0, 0, 100, false, false, false);

                    defender.FixedParticles(0x36F4, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);

                    SearingWeaponContext.CheckHit(attacker, defender);
                    attacker.Mana--;
                }
            }
            #endregion

            #region BoneBreaker/Swarm/Sparks
            bool sparks = false;
            if (a == null && move == null)
            {
                if (m_ExtendedWeaponAttributes.BoneBreaker > 0 && !AnimalForm.UnderTransformation(attacker))
                    BoneBreakerContext.CheckHit(attacker, defender);

                if (m_ExtendedWeaponAttributes.HitSwarm > 0 && Utility.Random(100) < m_ExtendedWeaponAttributes.HitSwarm)
                    SwarmContext.CheckHit(attacker, defender);

                if (m_ExtendedWeaponAttributes.HitSparks > 0 && Utility.Random(100) < m_ExtendedWeaponAttributes.HitSparks)
                {
                    SparksContext.CheckHit(attacker, defender);
                    sparks = true;
                }
            }
            #endregion

            Timer.DelayCall(d => AddBlood(d, damage), defender);

            int damageGiven = damage;

            if (a != null && !a.OnBeforeDamage(attacker, defender))
            {
                WeaponAbility.ClearCurrentAbility(attacker);
                a = null;
            }

            if (move != null && !move.OnBeforeDamage(attacker, defender))
            {
                SpecialMove.ClearCurrentMove(attacker);
                move = null;
            }

            if (Feint.Registry.ContainsKey(defender) && Feint.Registry[defender].Enemy == attacker)
                damage -= (int)(damage * ((double)Feint.Registry[defender].DamageReduction / 100));

            // Skill Masteries
            if (this is Fists)
                damage += (int)(damage * (MasteryInfo.GetKnockoutModifier(attacker, defender is PlayerMobile) / 100.0));

            SkillMasterySpell.OnHit(attacker, defender, ref damage);

            // Bane
            if (m_ExtendedWeaponAttributes.Bane > 0 && defender.Hits < defender.HitsMax / 2)
            {
                double inc = Math.Min(350, defender.HitsMax * .3);
                inc -= defender.Hits / (double)defender.HitsMax * inc;

                Effects.SendTargetEffect(defender, 0x37BE, 1, 4, 0x30, 3);

                damage += (int)inc;
            }

            damage += WhirlwindAttack.DamageBonus(attacker, defender);

            damageGiven = AOS.Damage(
                defender,
                attacker,
                damage,
                ignoreArmor,
                phys,
                fire,
                cold,
                pois,
                nrgy,
                chaos,
                direct,
                false,
                ranged ? Server.DamageType.Ranged : Server.DamageType.Melee);

            DualWield.DoHit(attacker, defender, damage);

            if (sparks)
            {
                int mana = attacker.Mana + damageGiven;
                if (!defender.Player) mana *= 2;
                attacker.Mana = Math.Min(attacker.ManaMax, attacker.Mana + mana);
            }

            double propertyBonus = (move == null) ? 1.0 : move.GetPropertyBonus(attacker);

            int lifeLeech = 0;
            int stamLeech = 0;
            int manaLeech = 0;

            if ((int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitLeechStam) * propertyBonus) > Utility.Random(100))
            {
                stamLeech += 100; // HitLeechStam% chance to leech 100% of damage as stamina
            }

            lifeLeech = (int)(WeaponAttributes.HitLeechHits * propertyBonus);
            manaLeech = (int)(WeaponAttributes.HitLeechMana * propertyBonus);

            int toHealCursedWeaponSpell = 0;

            if (CurseWeaponSpell.IsCursed(attacker, this))
            {
                toHealCursedWeaponSpell += AOS.Scale(damageGiven, 50); // Additional 50% life leech for cursed weapons (necro spell)
            }

            context = TransformationSpellHelper.GetContext(attacker);

            if (stamLeech != 0)
            {
                attacker.Stam += AOS.Scale(damageGiven, stamLeech);
            }

            if (lifeLeech != 0)
            {
                int toHeal = Utility.RandomMinMax(0, (int)(AOS.Scale(damageGiven, lifeLeech) * 0.3));

                if (defender is BaseCreature && ((BaseCreature)defender).TaintedLifeAura)
                {
                    AOS.Damage(attacker, defender, toHeal, false, 0, 0, 0, 0, 0, 0, 100, false, false, false);
                    attacker.SendLocalizedMessage(1116778); //The tainted life force energy damages you as your body tries to absorb it.
                }
                else
                {
                    attacker.Hits += toHeal;
                }

                Effects.SendPacket(defender.Location, defender.Map, new ParticleEffect(EffectType.FixedFrom, defender.Serial, Serial.Zero, 0x377A, defender.Location, defender.Location, 1, 15, false, false, 1926, 0, 0, 9502, 1, defender.Serial, 16, 0));
                Effects.SendPacket(defender.Location, defender.Map, new ParticleEffect(EffectType.FixedFrom, defender.Serial, Serial.Zero, 0x3728, defender.Location, defender.Location, 1, 12, false, false, 1963, 0, 0, 9042, 1, defender.Serial, 16, 0));
            }

            if (toHealCursedWeaponSpell != 0 && !(defender is BaseCreature && ((BaseCreature)defender).TaintedLifeAura))
            {
                attacker.Hits += toHealCursedWeaponSpell;
            }

            if (manaLeech != 0)
            {
                attacker.Mana += Utility.RandomMinMax(0, (int)(AOS.Scale(damageGiven, manaLeech) * 0.4));
            }

            if (lifeLeech != 0 || stamLeech != 0 || manaLeech != 0 || toHealCursedWeaponSpell != 0)
            {
                attacker.PlaySound(0x44D);
            }

            if (attacker is VampireBatFamiliar || attacker is VampireBat)
            {
                BaseCreature bc = (BaseCreature)attacker;
                Mobile caster = bc.ControlMaster;

                if (caster == null)
                {
                    caster = bc.SummonMaster;
                }

                if (caster != null && caster.Map == bc.Map && caster.InRange(bc, 2))
                {
                    caster.Hits += damage;
                }
                else
                {
                    bc.Hits += damage;
                }
            }

            if (!BlockHitEffects)
            {
                int physChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitPhysicalArea) * propertyBonus);
                int fireChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitFireArea) * propertyBonus);
                int coldChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitColdArea) * propertyBonus);
                int poisChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitPoisonArea) * propertyBonus);
                int nrgyChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitEnergyArea) * propertyBonus);

                if (physChance != 0 && physChance > Utility.Random(100))
                {
                    DoAreaAttack(attacker, defender, damageGiven, 0x10E, 50, 100, 0, 0, 0, 0);
                }

                if (fireChance != 0 && fireChance > Utility.Random(100))
                {
                    DoAreaAttack(attacker, defender, damageGiven, 0x11D, 1160, 0, 100, 0, 0, 0);
                }

                if (coldChance != 0 && coldChance > Utility.Random(100))
                {
                    DoAreaAttack(attacker, defender, damageGiven, 0x0FC, 2100, 0, 0, 100, 0, 0);
                }

                if (poisChance != 0 && poisChance > Utility.Random(100))
                {
                    DoAreaAttack(attacker, defender, damageGiven, 0x205, 1166, 0, 0, 0, 100, 0);
                }

                if (nrgyChance != 0 && nrgyChance > Utility.Random(100))
                {
                    DoAreaAttack(attacker, defender, damageGiven, 0x1F1, 120, 0, 0, 0, 0, 100);
                }

                int maChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitMagicArrow) * propertyBonus);
                int harmChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitHarm) * propertyBonus);
                int fireballChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitFireball) * propertyBonus);
                int lightningChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitLightning) * propertyBonus);
                int dispelChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitDispel) * propertyBonus);
                int explosChance = (int)(ExtendedWeaponAttributes.GetValue(attacker, ExtendedWeaponAttribute.HitExplosion) * propertyBonus);

                #region Mondains Legacy
                int velocityChance = this is BaseRanged ? ((BaseRanged)this).Velocity : 0;
                #endregion

                #region Stygian Abyss
                int curseChance = (int)(m_AosWeaponAttributes.HitCurse * propertyBonus);
                int fatigueChance = (int)(m_AosWeaponAttributes.HitFatigue * propertyBonus);
                int manadrainChance = (int)(m_AosWeaponAttributes.HitManaDrain * propertyBonus);
                #endregion

                if (maChance != 0 && maChance > Utility.Random(100))
                {
                    DoMagicArrow(attacker, defender);
                }

                if (harmChance != 0 && harmChance > Utility.Random(100))
                {
                    DoHarm(attacker, defender);
                }

                if (fireballChance != 0 && fireballChance > Utility.Random(100))
                {
                    DoFireball(attacker, defender);
                }

                if (lightningChance != 0 && lightningChance > Utility.Random(100))
                {
                    DoLightning(attacker, defender);
                }

                if (dispelChance != 0 && dispelChance > Utility.Random(100))
                {
                    DoDispel(attacker, defender);
                }

                if (explosChance != 0 && explosChance > Utility.Random(100))
                {
                    DoExplosion(attacker, defender);
                }

                #region Mondains Legacy
                if (velocityChance != 0 && velocityChance > Utility.Random(100))
                {
                    DoHitVelocity(attacker, damageable);
                }
                #endregion

                #region Stygian Abyss
                if (curseChance != 0 && curseChance > Utility.Random(100))
                {
                    DoCurse(attacker, defender);
                }

                if (fatigueChance != 0 && fatigueChance > Utility.Random(100))
                {
                    DoFatigue(attacker, defender, damageGiven);
                }

                if (manadrainChance != 0 && manadrainChance > Utility.Random(100))
                {
                    DoManaDrain(attacker, defender, damageGiven);
                }
                #endregion

                int laChance = (int)(AosWeaponAttributes.GetValue(attacker, AosWeaponAttribute.HitLowerAttack) * propertyBonus);

                if (laChance != 0 && laChance > Utility.Random(100))
                {
                    DoLowerAttack(attacker, defender);
                }

                int hldWep = m_AosWeaponAttributes.HitLowerDefend;
                int hldGlasses = 0;

                Item helm = attacker.FindItemOnLayer(Layer.Helm);

                if (helm != null)
                {
                    AosWeaponAttributes attrs = RunicReforging.GetAosWeaponAttributes(helm);

                    if (attrs != null)
                        hldGlasses = attrs.HitLowerDefend;
                }

                if ((hldWep > 0 && hldWep > Utility.Random(100)) || (hldGlasses > 0 && hldGlasses > Utility.Random(100)))
                {
                    DoLowerDefense(attacker, defender);
                }
            }

            if (attacker is BaseCreature)
            {
                ((BaseCreature)attacker).OnGaveMeleeAttack(defender);
            }

            if (defender is BaseCreature)
            {
                ((BaseCreature)defender).OnGotMeleeAttack(attacker);
            }

            if (a != null)
            {
                a.OnHit(attacker, defender, damage);
            }

            ForceOfNature.OnHit(attacker, defender);

            if (move != null)
            {
                move.OnHit(attacker, defender, damage);
            }

            if (defender is IHonorTarget && ((IHonorTarget)defender).ReceivedHonorContext != null)
            {
                ((IHonorTarget)defender).ReceivedHonorContext.OnTargetHit(attacker);
            }

            if (!ranged)
            {
                if (AnimalForm.UnderTransformation(attacker, typeof(GiantSerpent)))
                {
                    defender.ApplyPoison(attacker, Poison.Lesser);
                }

                if (AnimalForm.UnderTransformation(defender, typeof(BullFrog)))
                {
                    attacker.ApplyPoison(defender, Poison.Regular);
                }
            }

            BaseFamiliar.OnHit(attacker, damageable);
            WhiteTigerFormSpell.OnHit(attacker, defender);
        }

        public Direction GetOppositeDir(Direction d)
        {
            Direction direction = Direction.Down;

            if (d == Direction.West)
                direction = Direction.East;

            if (d == Direction.East)
                direction = Direction.West;

            if (d == Direction.North)
                direction = Direction.South;

            if (d == Direction.South)
                direction = Direction.North;

            if (d == Direction.Right)
                direction = Direction.Left;

            if (d == Direction.Left)
                direction = Direction.Right;

            if (d == Direction.Up)
                direction = Direction.Down;

            if (d == Direction.Down)
                direction = Direction.Up;

            return direction;
        }

        public virtual double GetAosSpellDamage(Mobile attacker, Mobile defender, int bonus, int dice, int sides)
        {
            int damage = Utility.Dice(dice, sides, bonus) * 100;
            int damageBonus = 0;

            int inscribeSkill = attacker.Skills[SkillName.Inscribe].Fixed;
            int inscribeBonus = (inscribeSkill + (1000 * (inscribeSkill / 1000))) / 200;

            damageBonus += inscribeBonus;
            damageBonus += attacker.Int / 10;
            damageBonus += SpellHelper.GetSpellDamageBonus(attacker, defender, SkillName.Magery, attacker is PlayerMobile && defender is PlayerMobile);
            damage = AOS.Scale(damage, 100 + damageBonus);

            if (defender != null && Feint.Registry.ContainsKey(defender) && Feint.Registry[defender].Enemy == attacker)
                damage -= (int)(damage * ((double)Feint.Registry[defender].DamageReduction / 100));

            // All hit spells use 80 eval
            int evalScale = 30 + ((9 * 800) / 100);

            damage = AOS.Scale(damage, evalScale);

            return damage / 100;
        }

        #region Do<AoSEffect>
        public virtual void DoMagicArrow(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
            {
                return;
            }

            attacker.DoHarmful(defender);

            double damage = GetAosSpellDamage(attacker, defender, 10, 1, 4);

            attacker.MovingParticles(defender, 0x36E4, 5, 0, false, true, 3006, 4006, 0);
            attacker.PlaySound(0x1E5);

            SpellHelper.Damage(TimeSpan.FromSeconds(1.0), defender, attacker, damage, 0, 100, 0, 0, 0);

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }

        public virtual void DoHarm(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
            {
                return;
            }

            attacker.DoHarmful(defender);

            double damage = GetAosSpellDamage(attacker, defender, 17, 1, 5);

            if (!defender.InRange(attacker, 2))
            {
                damage *= 0.25; // 1/4 damage at > 2 tile range
            }
            else if (!defender.InRange(attacker, 1))
            {
                damage *= 0.50; // 1/2 damage at 2 tile range
            }

            defender.FixedParticles(0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist);
            defender.PlaySound(0x0FC);

            SpellHelper.Damage(TimeSpan.Zero, defender, attacker, damage, 0, 0, 100, 0, 0);

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }

        public virtual void DoFireball(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
            {
                return;
            }

            attacker.DoHarmful(defender);

            double damage = GetAosSpellDamage(attacker, defender, 19, 1, 5);

            attacker.MovingParticles(defender, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
            attacker.PlaySound(0x15E);

            SpellHelper.Damage(TimeSpan.FromSeconds(1.0), defender, attacker, damage, 0, 100, 0, 0, 0);

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }

        public virtual void DoLightning(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
            {
                return;
            }

            attacker.DoHarmful(defender);

            double damage = GetAosSpellDamage(attacker, defender, 23, 1, 4);

            defender.BoltEffect(0);

            SpellHelper.Damage(TimeSpan.Zero, defender, attacker, damage, 0, 0, 0, 0, 100);

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }

        public virtual void DoDispel(Mobile attacker, Mobile defender)
        {
            bool dispellable = false;

            if (defender is BaseCreature)
            {
                dispellable = ((BaseCreature)defender).Summoned && !((BaseCreature)defender).IsAnimatedDead;
            }

            if (!dispellable)
            {
                return;
            }

            if (!attacker.CanBeHarmful(defender, false))
            {
                return;
            }

            attacker.DoHarmful(defender);

            MagerySpell sp = new DispelSpell(attacker, null);

            if (sp.CheckResisted(defender))
            {
                defender.FixedEffect(0x3779, 10, 20);
            }
            else
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(defender.Location, defender.Map, EffectItem.DefaultDuration), 0x3728, 8, 20, 5042);
                Effects.PlaySound(defender, defender.Map, 0x201);

                defender.Delete();
            }
        }

        public virtual void DoExplosion(Mobile attacker, Mobile defender)
        {
            if (!attacker.CanBeHarmful(defender, false))
            {
                return;
            }

            attacker.DoHarmful(defender);

            double damage = GetAosSpellDamage(attacker, defender, 40, 1, 5);

            defender.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
            defender.PlaySound(0x307);

            SpellHelper.Damage(TimeSpan.FromSeconds(1.0), defender, attacker, damage, 0, 100, 0, 0, 0);

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }

        public virtual void DoHitVelocity(Mobile attacker, IDamageable damageable)
        {
            int bonus = (int)attacker.GetDistanceToSqrt(damageable);

            if (bonus > 0)
            {
                AOS.Damage(damageable, attacker, bonus * 3, 100, 0, 0, 0, 0);

                if (attacker.Player)
                {
                    attacker.SendLocalizedMessage(1072794); // Your arrow hits its mark with velocity!
                }

                if (damageable is Mobile && ((Mobile)damageable).Player)
                {
                    ((Mobile)damageable).SendLocalizedMessage(1072795); // You have been hit by an arrow with velocity!
                }
            }

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }

        #region Stygian Abyss
        public virtual void DoCurse(Mobile attacker, Mobile defender)
        {
            attacker.SendLocalizedMessage(1113717); // You have hit your target with a curse effect.
            defender.SendLocalizedMessage(1113718); // You have been hit with a curse effect.

            defender.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
            defender.PlaySound(0x1EA);
            TimeSpan duration = TimeSpan.FromSeconds(30);

            defender.AddStatMod(
                new StatMod(StatType.Str, string.Format("[Magic] {0} Curse", StatType.Str), -10, duration));
            defender.AddStatMod(
                new StatMod(StatType.Dex, string.Format("[Magic] {0} Curse", StatType.Dex), -10, duration));
            defender.AddStatMod(
                new StatMod(StatType.Int, string.Format("[Magic] {0} Curse", StatType.Int), -10, duration));

            int percentage = -10; //(int)(SpellHelper.GetOffsetScalar(Caster, m, true) * 100);
            string args = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", percentage, percentage, percentage, 10, 10, 10, 10);

            Spells.Fourth.CurseSpell.AddEffect(defender, duration, 10, 10, 10);
            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.Curse, 1075835, 1075836, duration, defender, args));

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }

        public virtual void DoFatigue(Mobile attacker, Mobile defender, int damagegiven)
        {
            // Message?
            // Effects?
            defender.Stam -= (damagegiven * (100 - m_AosWeaponAttributes.HitFatigue)) / 100;

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }

        public virtual void DoManaDrain(Mobile attacker, Mobile defender, int damagegiven)
        {
            // Message?
            defender.FixedParticles(0x3789, 10, 25, 5032, EffectLayer.Head);
            defender.PlaySound(0x1F8);
            defender.Mana -= (damagegiven * (100 - m_AosWeaponAttributes.HitManaDrain)) / 100;

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }
        #endregion

        public virtual void DoLowerAttack(Mobile from, Mobile defender)
        {
            if (HitLower.ApplyAttack(defender))
            {
                defender.PlaySound(0x28E);
                Effects.SendTargetEffect(defender, 0x37BE, 1, 4, 0xA, 3);
            }
        }

        public virtual void DoLowerDefense(Mobile from, Mobile defender)
        {
            if (HitLower.ApplyDefense(defender))
            {
                defender.PlaySound(0x28E);
                Effects.SendTargetEffect(defender, 0x37BE, 1, 4, 0x23, 3);
            }
        }

        public virtual void DoAreaAttack(Mobile from, Mobile defender, int damageGiven, int sound, int hue, int phys, int fire, int cold, int pois, int nrgy)
        {
            Map map = from.Map;

            if (map == null)
            {
                return;
            }

            IEnumerable<IDamageable> list = SpellHelper.AcquireIndirectTargets(from, from, from.Map, 5);

            int count = 0;

            foreach (IDamageable m in list)
            {
                ++count;

                from.DoHarmful(m, true);
                m.FixedEffect(0x3779, 1, 15, hue, 0);
                AOS.Damage(m, from, damageGiven / 2, phys, fire, cold, pois, nrgy, Server.DamageType.SpellAOE);
            }

            if (count > 0)
            {
                Effects.PlaySound(from.Location, map, sound);
            }

            if (ProcessingMultipleHits)
                BlockHitEffects = true;
        }
        #endregion

        public virtual CheckSlayerResult CheckSlayers(Mobile attacker, Mobile defender, SlayerName slayer)
        {
            if (slayer == SlayerName.None)
                return CheckSlayerResult.None;

            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            SlayerEntry atkSlayer = SlayerGroup.GetEntryByName(slayer);

            if (atkSlayer != null && atkSlayer.Slays(defender) && _SuperSlayers.Contains(atkSlayer.Name))
            {
                return CheckSlayerResult.SuperSlayer;
            }

            if (atkSlayer != null && atkSlayer.Slays(defender))
            {
                return CheckSlayerResult.Slayer;
            }

            return CheckSlayerResult.None;
        }

        public CheckSlayerResult CheckSlayerOpposition(Mobile attacker, Mobile defender)
        {
            ISlayer defISlayer = Spellbook.FindEquippedSpellbook(defender);

            if (defISlayer == null)
            {
                defISlayer = defender.Weapon as ISlayer;
            }

            if (defISlayer != null)
            {
                SlayerEntry defSlayer = SlayerGroup.GetEntryByName(defISlayer.Slayer);
                SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName(defISlayer.Slayer2);
                SlayerEntry defSetSlayer = SlayerGroup.GetEntryByName(SetHelper.GetSetSlayer(defender));

                if (defISlayer is Item && defSlayer == null && defSlayer2 == null)
                {
                    defSlayer = SlayerGroup.GetEntryByName(SlayerSocket.GetSlayer((Item)defISlayer));
                }

                if (defSlayer != null && defSlayer.Group.OppositionSuperSlays(attacker) ||
                    defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays(attacker) ||
                    defSetSlayer != null && defSetSlayer.Group.OppositionSuperSlays(attacker))
                {
                    return CheckSlayerResult.Opposition;
                }
            }

            return CheckSlayerResult.None;
        }

        public CheckSlayerResult CheckTalismanSlayer(Mobile attacker, Mobile defender)
        {
            BaseTalisman talisman = attacker.Talisman as BaseTalisman;

            if (talisman != null && TalismanSlayer.Slays(talisman.Slayer, defender))
            {
                return CheckSlayerResult.Slayer;
            }
            else if (Slayer3 != TalismanSlayerName.None && TalismanSlayer.Slays(Slayer3, defender))
            {
                return CheckSlayerResult.Slayer;
            }

            return CheckSlayerResult.None;
        }

        private readonly List<SlayerName> _SuperSlayers = new List<SlayerName>()
        {
            SlayerName.Repond, SlayerName.Silver, SlayerName.Fey,
            SlayerName.ElementalBan, SlayerName.Exorcism, SlayerName.ArachnidDoom,
            SlayerName.ReptilianDeath, SlayerName.Dinosaur, SlayerName.Myrmidex,
            SlayerName.Eodon
        };

        #region Blood
        public void AddBlood(Mobile defender, int damage)
        {
            if (damage <= 5 || defender == null || defender.Map == null || !defender.HasBlood || !CanDrawBlood(defender))
            {
                return;
            }

            Map m = defender.Map;
            Rectangle2D b = new Rectangle2D(defender.X - 2, defender.Y - 2, 5, 5);

            int count = Utility.RandomMinMax(2, 3);

            for (int i = 0; i < count; i++)
            {
                var p = m.GetRandomSpawnPoint(b);
                p.Z = defender.Z;

                SpellHelper.AdjustField(ref p, m, 16, false);

                AddBlood(defender, p, m);
            }
        }

        protected virtual void AddBlood(Mobile defender, Point3D target, Map map)
        {
            Blood blood = CreateBlood(defender);

            int id = blood.ItemID;

            blood.ItemID = 1; // No Draw

            blood.OnBeforeSpawn(target, map);
            blood.MoveToWorld(target, map);
            blood.OnAfterSpawn();

            Effects.SendMovingEffect(defender, blood, id, 7, 10, true, false, blood.Hue, 0);

            Timer.DelayCall(TimeSpan.FromMilliseconds(750), b => b.ItemID = id, blood);
        }

        protected virtual bool CanDrawBlood(Mobile defender)
        {
            return defender.HasBlood;
        }

        protected virtual Blood CreateBlood(Mobile defender)
        {
            return new Blood
            {
                Hue = defender.BloodHue
            };
        }
        #endregion

        #region Elemental Damage
        public static int[] GetElementDamages(Mobile m)
        {
            int[] o = new[] { 100, 0, 0, 0, 0, 0, 0 };

            BaseWeapon w = m.Weapon as BaseWeapon ?? Fists;

            if (w != null)
            {
                w.GetDamageTypes(m, out o[0], out o[1], out o[2], out o[3], out o[4], out o[5], out o[6]);
            }

            return o;
        }

        public virtual void GetDamageTypes(
            Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            if (wielder is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)wielder;

                phys = bc.PhysicalDamage;
                fire = bc.FireDamage;
                cold = bc.ColdDamage;
                pois = bc.PoisonDamage;
                nrgy = bc.EnergyDamage;
                chaos = bc.ChaosDamage;
                direct = bc.DirectDamage;
            }
            else
            {
                fire = m_AosElementDamages.Fire;
                cold = m_AosElementDamages.Cold;
                pois = m_AosElementDamages.Poison;
                nrgy = m_AosElementDamages.Energy;
                chaos = m_AosElementDamages.Chaos;
                direct = m_AosElementDamages.Direct;

                phys = 100 - fire - cold - pois - nrgy - chaos - direct;

                CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);

                if (resInfo != null)
                {
                    CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

                    if (attrInfo != null)
                    {
                        int left = phys;

                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponColdDamage, ref cold, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponEnergyDamage, ref nrgy, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponFireDamage, ref fire, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponPoisonDamage, ref pois, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponChaosDamage, ref chaos, left);
                        left = ApplyCraftAttributeElementDamage(attrInfo.WeaponDirectDamage, ref direct, left);

                        phys = left;
                    }
                }
            }
        }

        private int ApplyCraftAttributeElementDamage(int attrDamage, ref int element, int totalRemaining)
        {
            if (totalRemaining <= 0)
            {
                return 0;
            }

            if (attrDamage <= 0)
            {
                return totalRemaining;
            }

            int appliedDamage = attrDamage;

            if ((appliedDamage + element) > 100)
            {
                appliedDamage = 100 - element;
            }

            if (appliedDamage > totalRemaining)
            {
                appliedDamage = totalRemaining;
            }

            element += appliedDamage;

            return totalRemaining - appliedDamage;
        }
        #endregion

        public virtual void OnMiss(Mobile attacker, IDamageable damageable)
        {
            Mobile defender = damageable as Mobile;

            PlaySwingAnimation(attacker);
            attacker.PlaySound(GetMissAttackSound(attacker, defender));

            if (defender != null)
                defender.PlaySound(GetMissDefendSound(attacker, defender));

            WeaponAbility ability = WeaponAbility.GetCurrentAbility(attacker);

            if (ability != null)
            {
                ability.OnMiss(attacker, defender);
            }

            SpecialMove move = SpecialMove.GetCurrentMove(attacker);

            if (move != null)
            {
                move.OnMiss(attacker, defender);
            }

            if (defender is IHonorTarget && ((IHonorTarget)defender).ReceivedHonorContext != null)
            {
                ((IHonorTarget)defender).ReceivedHonorContext.OnTargetMissed(attacker);
            }

            SkillMasterySpell.OnMiss(attacker, defender);
        }

        public virtual void GetBaseDamageRange(Mobile attacker, out int min, out int max)
        {
            if (attacker is BaseCreature)
            {
                BaseCreature c = (BaseCreature)attacker;

                if (c.DamageMin >= 0)
                {
                    min = c.DamageMin;
                    max = c.DamageMax;
                    return;
                }

                if (this is Fists && !attacker.Body.IsHuman)
                {
                    min = attacker.Str / 28;
                    max = attacker.Str / 28;
                    return;
                }
            }

            if (this is Fists && TransformationSpellHelper.UnderTransformation(attacker, typeof(HorrificBeastSpell)))
            {
                min = 5;
                max = 15;
            }
            else
            {
                min = MinDamage;
                max = MaxDamage;
            }
        }

        public virtual double GetBaseDamage(Mobile attacker)
        {
            int min, max;

            GetBaseDamageRange(attacker, out min, out max);

            return Utility.RandomMinMax(min, max);
        }

        public virtual double GetBonus(double value, double scalar, double threshold, double offset)
        {
            double bonus = value * scalar;

            if (value >= threshold)
            {
                bonus += offset;
            }

            return bonus / 100;
        }

        public virtual void GetStatusDamage(Mobile from, out int min, out int max)
        {
            int baseMin, baseMax;

            GetBaseDamageRange(from, out baseMin, out baseMax);

            min = Math.Max((int)ScaleDamageAOS(from, baseMin, false), 1);
            max = Math.Max((int)ScaleDamageAOS(from, baseMax, false), 1);
        }

        public virtual double ScaleDamageAOS(Mobile attacker, double damage, bool checkSkills)
        {
            if (checkSkills)
            {
                attacker.CheckSkill(SkillName.Tactics, 0.0, attacker.Skills[SkillName.Tactics].Cap);
                // Passively check tactics for gain
                attacker.CheckSkill(SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap);
                // Passively check Anatomy for gain

                if (Type == WeaponType.Axe)
                {
                    attacker.CheckSkill(SkillName.Lumberjacking, 0.0, 100.0); // Passively check Lumberjacking for gain
                }
            }

            #region Physical bonuses
            /*
            * These are the bonuses given by the physical characteristics of the mobile.
            * No caps apply.
            */
            double strengthBonus = GetBonus(attacker.Str, 0.300, 100.0, 5.00);
            double anatomyBonus = GetBonus(attacker.Skills[SkillName.Anatomy].Value, 0.500, 100.0, 5.00);
            double tacticsBonus = GetBonus(attacker.Skills[SkillName.Tactics].Value, 0.625, 100.0, 6.25);
            double lumberBonus = GetBonus(attacker.Skills[SkillName.Lumberjacking].Value, 0.200, 100.0, 10.00);

            if (Type != WeaponType.Axe)
            {
                lumberBonus = 0.0;
            }
            #endregion

            #region Modifiers
            /*
            * The following are damage modifiers whose effect shows on the status bar.
            * Capped at 100% total.
            */
            int damageBonus = AosAttributes.GetValue(attacker, AosAttribute.WeaponDamage);

            if (damageBonus > 100)
            {
                damageBonus = 100;
            }
            #endregion

            double totalBonus = strengthBonus + anatomyBonus + tacticsBonus + lumberBonus +
                                (damageBonus / 100.0);

            return damage + (int)(damage * totalBonus);
        }

        public virtual int ComputeDamageAOS(Mobile attacker, Mobile defender)
        {
            return (int)ScaleDamageAOS(attacker, GetBaseDamage(attacker), true);
        }

        public virtual int ScaleDamageByDurability(int damage)
        {
            int scale = 100;

            if (m_MaxHits > 0 && m_Hits < m_MaxHits)
            {
                scale = 50 + ((50 * m_Hits) / m_MaxHits);
            }

            return AOS.Scale(damage, scale);
        }

        public virtual int ComputeDamage(Mobile attacker, Mobile defender)
        {
            return ComputeDamageAOS(attacker, defender);
        }

        public virtual void PlayHurtAnimation(Mobile from)
        {
            if (from.Mounted)
            {
                return;
            }

            from.Animate(AnimationType.Impact, 0);
        }

        public virtual void PlaySwingAnimation(Mobile from)
        {
            from.Animate(AnimationType.Attack, GetNewAnimationAction(from));
        }

        public int GetNewAnimationAction(Mobile from)
        {
            switch (Animation)
            {
                default:
                case WeaponAnimation.Wrestle: return 0;
                case WeaponAnimation.Bash1H: return 3;
                case WeaponAnimation.Pierce1H: return 5;
                case WeaponAnimation.Slash1H: return 4;
                case WeaponAnimation.Bash2H: return 6;
                case WeaponAnimation.Pierce2H: return 8;
                case WeaponAnimation.Slash2H: return 7;
                case WeaponAnimation.ShootBow: return 1;
                case WeaponAnimation.ShootXBow: return 2;
                case WeaponAnimation.Throwing: return 9;
            }
        }

        #region Serialization/Deserialization
        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
            {
                flags |= toSet;
            }
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(20); // version

            // Version 20 - Removes all era checks and old code
            // Version 19 - Removes m_SearingWeapon as its handled as a socket now
            // Version 18 - removed VvV Item (handled in VvV System) and BlockRepair (Handled as negative attribute)

            writer.Write(m_UsesRemaining);
            writer.Write(m_ShowUsesRemaining);

            writer.Write(_Owner);
            writer.Write(_OwnerName);

            // Version 15 converts old leech to new leech

            //Version 14
            writer.Write(m_IsImbued);

            //version 13, converted SaveFlags to long, added negative attributes

            //version 12
            #region Runic Reforging
            writer.Write((int)m_ReforgedPrefix);
            writer.Write((int)m_ReforgedSuffix);
            writer.Write((int)m_ItemPower);
            #endregion

            writer.Write(m_DImodded);

            // Version 11
            writer.Write(m_TimesImbued);
            // Version 10
            writer.Write(m_BlessedBy); // Bless Deed

            #region Veteran Rewards
            writer.Write(m_EngravedText);
            #endregion

            #region Mondain's Legacy
            writer.Write((int)m_Slayer3);
            #endregion

            #region Mondain's Legacy Sets
            SetFlag sflags = SetFlag.None;

            SetSaveFlag(ref sflags, SetFlag.Attributes, !m_SetAttributes.IsEmpty);
            SetSaveFlag(ref sflags, SetFlag.SkillBonuses, !m_SetSkillBonuses.IsEmpty);
            SetSaveFlag(ref sflags, SetFlag.Hue, m_SetHue != 0);
            SetSaveFlag(ref sflags, SetFlag.LastEquipped, m_LastEquipped);
            SetSaveFlag(ref sflags, SetFlag.SetEquipped, m_SetEquipped);
            SetSaveFlag(ref sflags, SetFlag.SetSelfRepair, m_SetSelfRepair != 0);
            SetSaveFlag(ref sflags, SetFlag.PhysicalBonus, m_SetPhysicalBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.FireBonus, m_SetFireBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.ColdBonus, m_SetColdBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.PoisonBonus, m_SetPoisonBonus != 0);
            SetSaveFlag(ref sflags, SetFlag.EnergyBonus, m_SetEnergyBonus != 0);

            writer.WriteEncodedInt((int)sflags);

            if (GetSaveFlag(sflags, SetFlag.PhysicalBonus))
            {
                writer.WriteEncodedInt(m_SetPhysicalBonus);
            }

            if (GetSaveFlag(sflags, SetFlag.FireBonus))
            {
                writer.WriteEncodedInt(m_SetFireBonus);
            }

            if (GetSaveFlag(sflags, SetFlag.ColdBonus))
            {
                writer.WriteEncodedInt(m_SetColdBonus);
            }

            if (GetSaveFlag(sflags, SetFlag.PoisonBonus))
            {
                writer.WriteEncodedInt(m_SetPoisonBonus);
            }

            if (GetSaveFlag(sflags, SetFlag.EnergyBonus))
            {
                writer.WriteEncodedInt(m_SetEnergyBonus);
            }

            if (GetSaveFlag(sflags, SetFlag.Attributes))
            {
                m_SetAttributes.Serialize(writer);
            }

            if (GetSaveFlag(sflags, SetFlag.SkillBonuses))
            {
                m_SetSkillBonuses.Serialize(writer);
            }

            if (GetSaveFlag(sflags, SetFlag.Hue))
            {
                writer.Write(m_SetHue);
            }

            if (GetSaveFlag(sflags, SetFlag.LastEquipped))
            {
                writer.Write(m_LastEquipped);
            }

            if (GetSaveFlag(sflags, SetFlag.SetEquipped))
            {
                writer.Write(m_SetEquipped);
            }

            if (GetSaveFlag(sflags, SetFlag.SetSelfRepair))
            {
                writer.WriteEncodedInt(m_SetSelfRepair);
            }
            #endregion

            // Version 9
            SaveFlag flags = SaveFlag.None;

            //SetSaveFlag(ref flags, SaveFlag.DamageLevel, m_DamageLevel != WeaponDamageLevel.Regular);
            //SetSaveFlag(ref flags, SaveFlag.AccuracyLevel, m_AccuracyLevel != WeaponAccuracyLevel.Regular);
            //SetSaveFlag(ref flags, SaveFlag.DurabilityLevel, m_DurabilityLevel != WeaponDurabilityLevel.Regular);
            SetSaveFlag(ref flags, SaveFlag.Quality, m_Quality != ItemQuality.Normal);
            SetSaveFlag(ref flags, SaveFlag.Hits, m_Hits != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxHits, m_MaxHits != 0);
            SetSaveFlag(ref flags, SaveFlag.Slayer, m_Slayer != SlayerName.None);
            SetSaveFlag(ref flags, SaveFlag.Poison, m_Poison != null);
            SetSaveFlag(ref flags, SaveFlag.PoisonCharges, m_PoisonCharges != 0);
            SetSaveFlag(ref flags, SaveFlag.Crafter, m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Identified, m_Identified);
            SetSaveFlag(ref flags, SaveFlag.StrReq, m_StrReq != -1);
            SetSaveFlag(ref flags, SaveFlag.DexReq, m_DexReq != -1);
            SetSaveFlag(ref flags, SaveFlag.IntReq, m_IntReq != -1);
            SetSaveFlag(ref flags, SaveFlag.MinDamage, m_MinDamage != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxDamage, m_MaxDamage != -1);
            SetSaveFlag(ref flags, SaveFlag.HitSound, m_HitSound != -1);
            SetSaveFlag(ref flags, SaveFlag.MissSound, m_MissSound != -1);
            SetSaveFlag(ref flags, SaveFlag.Speed, m_Speed != -1);
            SetSaveFlag(ref flags, SaveFlag.MaxRange, m_MaxRange != -1);
            SetSaveFlag(ref flags, SaveFlag.Skill, m_Skill != (SkillName)(-1));
            SetSaveFlag(ref flags, SaveFlag.Type, m_Type != (WeaponType)(-1));
            SetSaveFlag(ref flags, SaveFlag.Animation, m_Animation != (WeaponAnimation)(-1));
            SetSaveFlag(ref flags, SaveFlag.Resource, m_Resource != CraftResource.Iron);
            SetSaveFlag(ref flags, SaveFlag.xAttributes, !m_AosAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.xWeaponAttributes, !m_AosWeaponAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.PlayerConstructed, m_PlayerConstructed);
            SetSaveFlag(ref flags, SaveFlag.SkillBonuses, !m_AosSkillBonuses.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Slayer2, m_Slayer2 != SlayerName.None);
            SetSaveFlag(ref flags, SaveFlag.ElementalDamages, !m_AosElementDamages.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.EngravedText, !string.IsNullOrEmpty(m_EngravedText));
            SetSaveFlag(ref flags, SaveFlag.xAbsorptionAttributes, !m_SAAbsorptionAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.xNegativeAttributes, !m_NegativeAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.Altered, m_Altered);
            SetSaveFlag(ref flags, SaveFlag.xExtendedWeaponAttributes, !m_ExtendedWeaponAttributes.IsEmpty);

            writer.Write((long)flags);

            if (GetSaveFlag(flags, SaveFlag.Quality))
            {
                writer.Write((int)m_Quality);
            }

            if (GetSaveFlag(flags, SaveFlag.Hits))
            {
                writer.Write(m_Hits);
            }

            if (GetSaveFlag(flags, SaveFlag.MaxHits))
            {
                writer.Write(m_MaxHits);
            }

            if (GetSaveFlag(flags, SaveFlag.Slayer))
            {
                writer.Write((int)m_Slayer);
            }

            if (GetSaveFlag(flags, SaveFlag.Poison))
            {
                Poison.Serialize(m_Poison, writer);
            }

            if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
            {
                writer.Write(m_PoisonCharges);
            }

            if (GetSaveFlag(flags, SaveFlag.Crafter))
            {
                writer.Write(m_Crafter);
            }

            if (GetSaveFlag(flags, SaveFlag.StrReq))
            {
                writer.Write(m_StrReq);
            }

            if (GetSaveFlag(flags, SaveFlag.DexReq))
            {
                writer.Write(m_DexReq);
            }

            if (GetSaveFlag(flags, SaveFlag.IntReq))
            {
                writer.Write(m_IntReq);
            }

            if (GetSaveFlag(flags, SaveFlag.MinDamage))
            {
                writer.Write(m_MinDamage);
            }

            if (GetSaveFlag(flags, SaveFlag.MaxDamage))
            {
                writer.Write(m_MaxDamage);
            }

            if (GetSaveFlag(flags, SaveFlag.HitSound))
            {
                writer.Write(m_HitSound);
            }

            if (GetSaveFlag(flags, SaveFlag.MissSound))
            {
                writer.Write(m_MissSound);
            }

            if (GetSaveFlag(flags, SaveFlag.Speed))
            {
                writer.Write(m_Speed);
            }

            if (GetSaveFlag(flags, SaveFlag.MaxRange))
            {
                writer.Write(m_MaxRange);
            }

            if (GetSaveFlag(flags, SaveFlag.Skill))
            {
                writer.Write((int)m_Skill);
            }

            if (GetSaveFlag(flags, SaveFlag.Type))
            {
                writer.Write((int)m_Type);
            }

            if (GetSaveFlag(flags, SaveFlag.Animation))
            {
                writer.Write((int)m_Animation);
            }

            if (GetSaveFlag(flags, SaveFlag.Resource))
            {
                writer.Write((int)m_Resource);
            }

            if (GetSaveFlag(flags, SaveFlag.xAttributes))
            {
                m_AosAttributes.Serialize(writer);
            }

            if (GetSaveFlag(flags, SaveFlag.xWeaponAttributes))
            {
                m_AosWeaponAttributes.Serialize(writer);
            }

            if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
            {
                m_AosSkillBonuses.Serialize(writer);
            }

            if (GetSaveFlag(flags, SaveFlag.Slayer2))
            {
                writer.Write((int)m_Slayer2);
            }

            if (GetSaveFlag(flags, SaveFlag.ElementalDamages))
            {
                m_AosElementDamages.Serialize(writer);
            }

            if (GetSaveFlag(flags, SaveFlag.EngravedText))
            {
                writer.Write(m_EngravedText);
            }

            #region SA
            if (GetSaveFlag(flags, SaveFlag.xAbsorptionAttributes))
            {
                m_SAAbsorptionAttributes.Serialize(writer);
            }

            if (GetSaveFlag(flags, SaveFlag.xNegativeAttributes))
            {
                m_NegativeAttributes.Serialize(writer);
            }
            #endregion

            if (GetSaveFlag(flags, SaveFlag.xExtendedWeaponAttributes))
            {
                m_ExtendedWeaponAttributes.Serialize(writer);
            }
        }

        [Flags]
        private enum SaveFlag : long
        {
            None = 0x00000000,
            Empty1 = 0x00000001,
            Empty2 = 0x00000002,
            Empty3 = 0x00000004,
            Quality = 0x00000008,
            Hits = 0x00000010,
            MaxHits = 0x00000020,
            Slayer = 0x00000040,
            Poison = 0x00000080,
            PoisonCharges = 0x00000100,
            Crafter = 0x00000200,
            Identified = 0x00000400,
            StrReq = 0x00000800,
            DexReq = 0x00001000,
            IntReq = 0x00002000,
            MinDamage = 0x00004000,
            MaxDamage = 0x00008000,
            HitSound = 0x00010000,
            MissSound = 0x00020000,
            Speed = 0x00040000,
            MaxRange = 0x00080000,
            Skill = 0x00100000,
            Type = 0x00200000,
            Animation = 0x00400000,
            Resource = 0x00800000,
            xAttributes = 0x01000000,
            xWeaponAttributes = 0x02000000,
            PlayerConstructed = 0x04000000,
            SkillBonuses = 0x08000000,
            Slayer2 = 0x10000000,
            ElementalDamages = 0x20000000,
            EngravedText = 0x40000000,
            xAbsorptionAttributes = 0x80000000,
            xNegativeAttributes = 0x100000000,
            Altered = 0x200000000,
            xExtendedWeaponAttributes = 0x400000000
        }

        #region Mondain's Legacy Sets
        private static void SetSaveFlag(ref SetFlag flags, SetFlag toSet, bool setIf)
        {
            if (setIf)
            {
                flags |= toSet;
            }
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
            WeaponAttributes = 0x00000002,
            SkillBonuses = 0x00000004,
            Hue = 0x00000008,
            LastEquipped = 0x00000010,
            SetEquipped = 0x00000020,
            SetSelfRepair = 0x00000040,
            PhysicalBonus = 0x00000080,
            FireBonus = 0x00000100,
            ColdBonus = 0x00000200,
            PoisonBonus = 0x00000400,
            EnergyBonus = 0x00000800,
        }
        #endregion

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 20: // Removed Eras
                case 19: // Removed SearingWeapon
                case 18:
                case 17:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        m_ShowUsesRemaining = reader.ReadBool();
                        goto case 16;
                    }
                case 16:
                    {
                        if (version == 17)
                            reader.ReadBool();

                        _Owner = reader.ReadMobile();
                        _OwnerName = reader.ReadString();
                        goto case 15;
                    }
                case 15:
                case 14:
                    {
                        m_IsImbued = reader.ReadBool();
                        goto case 13;
                    }
                case 13:
                case 12:
                    {
                        #region Runic Reforging
                        m_ReforgedPrefix = (ReforgedPrefix)reader.ReadInt();
                        m_ReforgedSuffix = (ReforgedSuffix)reader.ReadInt();
                        m_ItemPower = (ItemPower)reader.ReadInt();

                        if (version < 18 && reader.ReadBool())
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                            {
                                m_NegativeAttributes.NoRepair = 1;
                            });
                        }
                        #endregion

                        #region Stygian Abyss
                        m_DImodded = reader.ReadBool();

                        if (version == 18)
                        {
                            if (reader.ReadBool())
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                                {
                                    AttachSocket(new SearingWeapon(this));
                                });
                            }
                        }
                        goto case 11;
                    }
                case 11:
                    {
                        m_TimesImbued = reader.ReadInt();

                        #endregion

                        goto case 10;
                    }
                case 10:
                    {
                        m_BlessedBy = reader.ReadMobile();
                        m_EngravedText = reader.ReadString();
                        m_Slayer3 = (TalismanSlayerName)reader.ReadInt();

                        SetFlag flags = (SetFlag)reader.ReadEncodedInt();
                        if (GetSaveFlag(flags, SetFlag.PhysicalBonus))
                        {
                            m_SetPhysicalBonus = reader.ReadEncodedInt();
                        }

                        if (GetSaveFlag(flags, SetFlag.FireBonus))
                        {
                            m_SetFireBonus = reader.ReadEncodedInt();
                        }

                        if (GetSaveFlag(flags, SetFlag.ColdBonus))
                        {
                            m_SetColdBonus = reader.ReadEncodedInt();
                        }

                        if (GetSaveFlag(flags, SetFlag.PoisonBonus))
                        {
                            m_SetPoisonBonus = reader.ReadEncodedInt();
                        }

                        if (GetSaveFlag(flags, SetFlag.EnergyBonus))
                        {
                            m_SetEnergyBonus = reader.ReadEncodedInt();
                        }

                        if (GetSaveFlag(flags, SetFlag.Attributes))
                        {
                            m_SetAttributes = new AosAttributes(this, reader);
                        }
                        else
                        {
                            m_SetAttributes = new AosAttributes(this);
                        }

                        if (GetSaveFlag(flags, SetFlag.WeaponAttributes))
                        {
                            m_SetSelfRepair = (new AosWeaponAttributes(this, reader)).SelfRepair;
                        }

                        if (GetSaveFlag(flags, SetFlag.SkillBonuses))
                        {
                            m_SetSkillBonuses = new AosSkillBonuses(this, reader);
                        }
                        else
                        {
                            m_SetSkillBonuses = new AosSkillBonuses(this);
                        }

                        if (GetSaveFlag(flags, SetFlag.Hue))
                        {
                            m_SetHue = reader.ReadInt();
                        }

                        if (GetSaveFlag(flags, SetFlag.LastEquipped))
                        {
                            m_LastEquipped = reader.ReadBool();
                        }

                        if (GetSaveFlag(flags, SetFlag.SetEquipped))
                        {
                            m_SetEquipped = reader.ReadBool();
                        }

                        if (GetSaveFlag(flags, SetFlag.SetSelfRepair))
                        {
                            m_SetSelfRepair = reader.ReadEncodedInt();
                        }

                        goto case 5;
                    }
                case 9:
                case 8:
                case 7:
                case 6:
                case 5:
                    {
                        SaveFlag flags;

                        if (version < 13)
                            flags = (SaveFlag)reader.ReadInt();
                        else
                            flags = (SaveFlag)reader.ReadLong();

                        if (version < 20 && GetSaveFlag(flags, SaveFlag.Empty1))
                        {
                            reader.ReadInt();
                        }

                        if (version < 20 && GetSaveFlag(flags, SaveFlag.Empty2))
                        {
                            reader.ReadInt();
                        }

                        if (version < 20 && GetSaveFlag(flags, SaveFlag.Empty3))
                        {
                            reader.ReadInt();
                        }

                        if (GetSaveFlag(flags, SaveFlag.Quality))
                        {
                            m_Quality = (ItemQuality)reader.ReadInt();
                        }
                        else
                        {
                            m_Quality = ItemQuality.Normal;
                        }

                        if (GetSaveFlag(flags, SaveFlag.Hits))
                        {
                            m_Hits = reader.ReadInt();
                        }

                        if (GetSaveFlag(flags, SaveFlag.MaxHits))
                        {
                            m_MaxHits = reader.ReadInt();
                        }

                        if (GetSaveFlag(flags, SaveFlag.Slayer))
                        {
                            m_Slayer = (SlayerName)reader.ReadInt();
                        }

                        if (GetSaveFlag(flags, SaveFlag.Poison))
                        {
                            m_Poison = Poison.Deserialize(reader);
                        }

                        if (GetSaveFlag(flags, SaveFlag.PoisonCharges))
                        {
                            m_PoisonCharges = reader.ReadInt();
                        }

                        if (GetSaveFlag(flags, SaveFlag.Crafter))
                        {
                            m_Crafter = reader.ReadMobile();
                        }

                        if (GetSaveFlag(flags, SaveFlag.Identified))
                        {
                            m_Identified = (version >= 6 || reader.ReadBool());
                        }

                        if (GetSaveFlag(flags, SaveFlag.StrReq))
                        {
                            m_StrReq = reader.ReadInt();
                        }
                        else
                        {
                            m_StrReq = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.DexReq))
                        {
                            m_DexReq = reader.ReadInt();
                        }
                        else
                        {
                            m_DexReq = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.IntReq))
                        {
                            m_IntReq = reader.ReadInt();
                        }
                        else
                        {
                            m_IntReq = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.MinDamage))
                        {
                            m_MinDamage = reader.ReadInt();
                        }
                        else
                        {
                            m_MinDamage = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.MaxDamage))
                        {
                            m_MaxDamage = reader.ReadInt();
                        }
                        else
                        {
                            m_MaxDamage = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.HitSound))
                        {
                            m_HitSound = reader.ReadInt();
                        }
                        else
                        {
                            m_HitSound = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.MissSound))
                        {
                            m_MissSound = reader.ReadInt();
                        }
                        else
                        {
                            m_MissSound = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.Speed))
                        {
                            if (version < 9)
                            {
                                m_Speed = reader.ReadInt();
                            }
                            else
                            {
                                m_Speed = reader.ReadFloat();
                            }
                        }
                        else
                        {
                            m_Speed = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.MaxRange))
                        {
                            m_MaxRange = reader.ReadInt();
                        }
                        else
                        {
                            m_MaxRange = -1;
                        }

                        if (GetSaveFlag(flags, SaveFlag.Skill))
                        {
                            m_Skill = (SkillName)reader.ReadInt();
                        }
                        else
                        {
                            m_Skill = (SkillName)(-1);
                        }

                        if (GetSaveFlag(flags, SaveFlag.Type))
                        {
                            m_Type = (WeaponType)reader.ReadInt();
                        }
                        else
                        {
                            m_Type = (WeaponType)(-1);
                        }

                        if (GetSaveFlag(flags, SaveFlag.Animation))
                        {
                            m_Animation = (WeaponAnimation)reader.ReadInt();
                        }
                        else
                        {
                            m_Animation = (WeaponAnimation)(-1);
                        }

                        if (GetSaveFlag(flags, SaveFlag.Resource))
                        {
                            m_Resource = (CraftResource)reader.ReadInt();
                        }
                        else
                        {
                            m_Resource = CraftResource.Iron;
                        }

                        if (GetSaveFlag(flags, SaveFlag.xAttributes))
                        {
                            m_AosAttributes = new AosAttributes(this, reader);
                        }
                        else
                        {
                            m_AosAttributes = new AosAttributes(this);
                        }

                        if (GetSaveFlag(flags, SaveFlag.xWeaponAttributes))
                        {
                            m_AosWeaponAttributes = new AosWeaponAttributes(this, reader);
                        }
                        else
                        {
                            m_AosWeaponAttributes = new AosWeaponAttributes(this);
                        }

                        if (version < 7 && m_AosWeaponAttributes.MageWeapon != 0)
                        {
                            m_AosWeaponAttributes.MageWeapon = 30 - m_AosWeaponAttributes.MageWeapon;
                        }

                        if (m_AosWeaponAttributes.MageWeapon != 0 && m_AosWeaponAttributes.MageWeapon != 30 && Parent is Mobile)
                        {
                            m_MageMod = new DefaultSkillMod(SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeapon);
                            ((Mobile)Parent).AddSkillMod(m_MageMod);
                        }

                        if (GetSaveFlag(flags, SaveFlag.PlayerConstructed))
                        {
                            m_PlayerConstructed = true;
                        }

                        if (GetSaveFlag(flags, SaveFlag.SkillBonuses))
                        {
                            m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        }
                        else
                        {
                            m_AosSkillBonuses = new AosSkillBonuses(this);
                        }

                        if (GetSaveFlag(flags, SaveFlag.Slayer2))
                        {
                            m_Slayer2 = (SlayerName)reader.ReadInt();
                        }

                        if (GetSaveFlag(flags, SaveFlag.ElementalDamages))
                        {
                            m_AosElementDamages = new AosElementAttributes(this, reader);
                        }
                        else
                        {
                            m_AosElementDamages = new AosElementAttributes(this);
                        }

                        if (GetSaveFlag(flags, SaveFlag.EngravedText))
                        {
                            m_EngravedText = reader.ReadString();
                        }

                        #region Stygian Abyss
                        if (version > 9 && GetSaveFlag(flags, SaveFlag.xAbsorptionAttributes))
                        {
                            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this, reader);
                        }
                        else
                        {
                            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);
                        }

                        if (version >= 13 && GetSaveFlag(flags, SaveFlag.xNegativeAttributes))
                        {
                            m_NegativeAttributes = new NegativeAttributes(this, reader);
                        }
                        else
                        {
                            m_NegativeAttributes = new NegativeAttributes(this);
                        }
                        #endregion

                        if (GetSaveFlag(flags, SaveFlag.Altered))
                        {
                            m_Altered = true;
                        }

                        if (GetSaveFlag(flags, SaveFlag.xExtendedWeaponAttributes))
                        {
                            m_ExtendedWeaponAttributes = new ExtendedWeaponAttributes(this, reader);
                        }
                        else
                        {
                            m_ExtendedWeaponAttributes = new ExtendedWeaponAttributes(this);
                        }

                        if (m_ExtendedWeaponAttributes.MysticWeapon != 0 && m_ExtendedWeaponAttributes.MysticWeapon != 30 && Parent is Mobile)
                        {
                            m_MysticMod = new DefaultSkillMod(SkillName.Mysticism, true, -30 + m_ExtendedWeaponAttributes.MysticWeapon);
                            ((Mobile)Parent).AddSkillMod(m_MysticMod);
                        }

                        break;
                    }
            }

            #region Mondain's Legacy Sets
            if (m_SetAttributes == null)
            {
                m_SetAttributes = new AosAttributes(this);
            }

            if (m_SetSkillBonuses == null)
            {
                m_SetSkillBonuses = new AosSkillBonuses(this);
            }
            #endregion

            if (Parent is Mobile)
            {
                m_AosSkillBonuses.AddTo((Mobile)Parent);
            }

            int strBonus = m_AosAttributes.BonusStr;
            int dexBonus = m_AosAttributes.BonusDex;
            int intBonus = m_AosAttributes.BonusInt;

            if (Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0))
            {
                Mobile m = (Mobile)Parent;

                string modName = Serial.ToString();

                if (strBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));
                }

                if (dexBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));
                }

                if (intBonus != 0)
                {
                    m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
                }
            }

            if (Parent is Mobile)
            {
                ((Mobile)Parent).CheckStatTimers();
            }

            if (m_Hits <= 0 && m_MaxHits <= 0)
            {
                m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);
            }

            if (m_Slayer == SlayerName.DaemonDismissal || m_Slayer == SlayerName.BalronDamnation)
                m_Slayer = SlayerName.Exorcism;

            if (m_Slayer2 == SlayerName.DaemonDismissal || m_Slayer2 == SlayerName.BalronDamnation)
                m_Slayer2 = SlayerName.Exorcism;
        }
        #endregion

        public BaseWeapon(int itemID)
            : base(itemID)
        {
            Layer = (Layer)ItemData.Quality;

            m_Quality = ItemQuality.Normal;
            m_StrReq = -1;
            m_DexReq = -1;
            m_IntReq = -1;
            m_MinDamage = -1;
            m_MaxDamage = -1;
            m_HitSound = -1;
            m_MissSound = -1;
            m_Speed = -1;
            m_MaxRange = -1;
            m_Skill = (SkillName)(-1);
            m_Type = (WeaponType)(-1);
            m_Animation = (WeaponAnimation)(-1);

            m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);

            m_Resource = CraftResource.Iron;

            m_AosAttributes = new AosAttributes(this);
            m_AosWeaponAttributes = new AosWeaponAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);
            m_AosElementDamages = new AosElementAttributes(this);
            m_NegativeAttributes = new NegativeAttributes(this);
            m_ExtendedWeaponAttributes = new ExtendedWeaponAttributes(this);

            #region Stygian Abyss
            m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);
            #endregion

            #region Mondain's Legacy Sets
            m_SetAttributes = new AosAttributes(this);
            m_SetSkillBonuses = new AosSkillBonuses(this);
            #endregion

            m_AosSkillBonuses = new AosSkillBonuses(this);

            if (this is ITool)
            {
                m_UsesRemaining = Utility.RandomMinMax(25, 75);
            }
            else
            {
                m_UsesRemaining = 150;
            }
        }

        public BaseWeapon(Serial serial)
            : base(serial)
        { }

        private string GetNameString()
        {
            string name = Name;

            if (name == null)
            {
                name = string.Format("#{0}", LabelNumber);
            }

            return name;
        }

        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                base.Hue = value;
                InvalidateProperties();
            }
        }

        public int GetElementalDamageHue()
        {
            int phys, fire, cold, pois, nrgy, chaos, direct;
            GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);
            //Order is Cold, Energy, Fire, Poison, Physical left

            int currentMax = 50;
            int hue = 0;

            if (pois >= currentMax)
            {
                hue = 1267 + (pois - 50) / 10;
                currentMax = pois;
            }

            if (fire >= currentMax)
            {
                hue = 1255 + (fire - 50) / 10;
                currentMax = fire;
            }

            if (nrgy >= currentMax)
            {
                hue = 1273 + (nrgy - 50) / 10;
                currentMax = nrgy;
            }

            if (cold >= currentMax)
            {
                hue = 1261 + (cold - 50) / 10;
                currentMax = cold;
            }

            return hue;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_ExtendedWeaponAttributes.AssassinHoned > 0)
            {
                list.Add(1152207); // Assassin's Edge
                return;
            }

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

                #region Mondain's Legacy
                case CraftResource.OakWood:
                    oreType = 1072533;
                    break; // oak
                case CraftResource.AshWood:
                    oreType = 1072534;
                    break; // ash
                case CraftResource.YewWood:
                    oreType = 1072535;
                    break; // yew
                case CraftResource.Heartwood:
                    oreType = 1072536;
                    break; // heartwood
                case CraftResource.Bloodwood:
                    oreType = 1072538;
                    break; // bloodwood
                case CraftResource.Frostwood:
                    oreType = 1072539;
                    break; // frostwood
                #endregion

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
            {
                list.Add(1053099, "#{0}\t{1}", oreType, GetNameString()); // ~1_oretype~ ~2_armortype~
            }
            #region High Seas
            else if (SearingWeapon)
            {
                list.Add(1151318, string.Format("#{0}", LabelNumber));
            }
            #endregion
            else if (Name == null)
            {
                list.Add(LabelNumber);
            }
            else
            {
                list.Add(Name);
            }

            /*
            * Want to move this to the engraving tool, let the non-harmful
            * formatting show, and remove CLILOCs embedded: more like OSI
            * did with the books that had markup, etc.
            *
            * This will have a negative effect on a few event things imgame
            * as is.
            *
            * If we cant find a more OSI-ish way to clean it up, we can
            * easily put this back, and use it in the deserialize
            * method and engraving tool, to make it perm cleaned up.
            */

            if (!string.IsNullOrEmpty(m_EngravedText))
            {
                list.Add(1062613, Utility.FixHtml(m_EngravedText));
            }
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            if (base.AllowEquipedCast(from))
            {
                return true;
            }

            return m_AosAttributes.SpellChanneling > 0 || Enhancement.GetValue(from, AosAttribute.SpellChanneling) > 0;
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

        public virtual int GetLuckBonus()
        {
            #region Mondain's Legacy
            if (m_Resource == CraftResource.Heartwood)
            {
                return 0;
            }
            #endregion

            CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);

            if (resInfo == null)
            {
                return 0;
            }

            CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

            if (attrInfo == null)
            {
                return 0;
            }

            return attrInfo.WeaponLuck;
        }

        public override void AddCraftedProperties(ObjectPropertyList list)
        {
            if (OwnerName != null)
            {
                list.Add(1153213, OwnerName);
            }

            if (m_Crafter != null)
            {
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
            }

            if (m_Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }

            if (IsImbued)
            {
                list.Add(1080418); // (Imbued)
            }

            if (m_Altered)
            {
                list.Add(1111880); // Altered
            }
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            if (IsVvVItem)
                list.Add(1154937); // VvV Item
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            if (ShowUsesRemaining)
            {
                list.Add(1060584, UsesRemaining.ToString()); // uses remaining: ~1_val~
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            #region Mondain's Legacy Sets
            if (IsSetItem)
            {
                list.Add(1073491, Pieces.ToString()); // Part of a Weapon/Armor Set (~1_val~ pieces)

                if (SetID == SetItem.Bestial)
                    list.Add(1151541, BestialSetHelper.GetTotalBerserk(this).ToString()); // Berserk ~1_VAL~

                if (BardMasteryBonus)
                    list.Add(1151553); // Activate: Bard Mastery Bonus x2<br>(Effect: 1 min. Cooldown: 30 min.)

                if (m_SetEquipped)
                {
                    list.Add(1073492); // Full Weapon/Armor Set Present
                    GetSetProperties(list);
                }
            }
            #endregion

            if (m_ExtendedWeaponAttributes.Focus > 0)
            {
                list.Add(1150018); // Focus
            }

            if (m_NegativeAttributes.Brittle == 0 && m_AosAttributes.Brittle != 0)
            {
                list.Add(1116209); // Brittle
            }

            if (m_NegativeAttributes != null)
                m_NegativeAttributes.GetProperties(list, this);

            if (m_AosSkillBonuses != null)
            {
                m_AosSkillBonuses.GetProperties(list);
            }

            if (RaceDefinitions.GetRequiredRace(this) == Race.Elf)
            {
                list.Add(1075086); // Elves Only
            }
            else if (RaceDefinitions.GetRequiredRace(this) == Race.Gargoyle)
            {
                list.Add(1111709); // Gargoyles Only
            }

            if (ArtifactRarity > 0)
            {
                list.Add(1061078, ArtifactRarity.ToString()); // artifact rarity ~1_val~
            }

            if (m_Poison != null && m_PoisonCharges > 0 && CanShowPoisonCharges())
            {
                #region Mondain's Legacy mod
                list.Add(m_Poison.LabelNumber, m_PoisonCharges.ToString());
                #endregion
            }

            if (m_Slayer != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer);
                if (entry != null)
                {
                    list.Add(entry.Title);
                }
            }

            if (m_Slayer2 != SlayerName.None)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(m_Slayer2);
                if (entry != null)
                {
                    list.Add(entry.Title);
                }
            }

            #region Mondain's Legacy
            if (m_Slayer3 != TalismanSlayerName.None)
            {
                if (m_Slayer3 == TalismanSlayerName.Wolf)
                {
                    list.Add(1075462);
                }
                else if (m_Slayer3 == TalismanSlayerName.Goblin)
                {
                    list.Add(1095010);
                }
                else if (m_Slayer3 == TalismanSlayerName.Undead)
                {
                    list.Add(1060479);
                }
                else
                {
                    list.Add(1072503 + (int)m_Slayer3);
                }
            }
            #endregion

            if (HasSocket<Caddellite>())
            {
                list.Add(1158662); // Caddellite Infused
            }

            double focusBonus = 1;
            int enchantBonus = 0;
            bool fcMalus = false;
            int damBonus = 0;
            SpecialMove move = null;
            AosWeaponAttribute bonus = AosWeaponAttribute.HitColdArea;

            #region Focus Attack
            if (FocusWeilder != null)
            {
                move = SpecialMove.GetCurrentMove(FocusWeilder);

                if (move is FocusAttack)
                {
                    focusBonus = move.GetPropertyBonus(FocusWeilder);
                    damBonus = (int)(move.GetDamageScalar(FocusWeilder, null) * 100) - 100;
                }
            }
            #endregion

            #region Stygian Abyss
            if (EnchantedWeilder != null)
            {
                if (Spells.Mysticism.EnchantSpell.IsUnderSpellEffects(EnchantedWeilder, this))
                {
                    bonus = Spells.Mysticism.EnchantSpell.BonusAttribute(EnchantedWeilder);
                    enchantBonus = Spells.Mysticism.EnchantSpell.BonusValue(EnchantedWeilder);
                    fcMalus = Spells.Mysticism.EnchantSpell.CastingMalus(EnchantedWeilder, this);
                }
            }
            #endregion

            int prop;
            double fprop;

            if ((prop = m_AosWeaponAttributes.DurabilityBonus) != 0)
            {
                list.Add(1151780, prop.ToString()); // durability +~1_VAL~%
            }

            if (m_ExtendedWeaponAttributes.Bane > 0)
            {
                list.Add(1154671); // Bane
            }

            if (m_ExtendedWeaponAttributes.BoneBreaker > 0)
            {
                list.Add(1157318); // Bone Breaker
            }

            if ((prop = m_ExtendedWeaponAttributes.HitSwarm) != 0)
            {
                list.Add(1157325, prop.ToString()); // Swarm ~1_val~%
            }

            if ((prop = m_ExtendedWeaponAttributes.HitSparks) != 0)
            {
                list.Add(1157326, prop.ToString()); // Sparks ~1_val~%
            }

            if ((prop = m_ExtendedWeaponAttributes.AssassinHoned) != 0)
            {
                list.Add(1152206); // Assassin Honed
            }

            if ((prop = m_AosWeaponAttributes.SplinteringWeapon) != 0)
            {
                list.Add(1112857, prop.ToString()); //splintering weapon ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitDispel * focusBonus) != 0)
            {
                list.Add(1060417, ((int)fprop).ToString()); // hit dispel ~1_val~%
            }
            else if (bonus == AosWeaponAttribute.HitDispel && enchantBonus != 0)
            {
                list.Add(1060417, ((int)(enchantBonus * focusBonus)).ToString()); // hit dispel ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitFireball * focusBonus) != 0)
            {
                list.Add(1060420, ((int)fprop).ToString()); // hit fireball ~1_val~%
            }
            else if (bonus == AosWeaponAttribute.HitFireball && enchantBonus != 0)
            {
                list.Add(1060420, ((int)(enchantBonus * focusBonus)).ToString()); // hit fireball ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitLightning * focusBonus) != 0)
            {
                list.Add(1060423, ((int)fprop).ToString()); // hit lightning ~1_val~%
            }
            else if (bonus == AosWeaponAttribute.HitLightning && enchantBonus != 0)
            {
                list.Add(1060423, ((int)(enchantBonus * focusBonus)).ToString()); // hit lightning ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitHarm * focusBonus) != 0)
            {
                list.Add(1060421, ((int)fprop).ToString()); // hit harm ~1_val~%
            }
            else if (bonus == AosWeaponAttribute.HitHarm && enchantBonus != 0)
            {
                list.Add(1060421, ((int)(enchantBonus * focusBonus)).ToString()); // hit harm ~1_val~%
            }

            if ((fprop = m_ExtendedWeaponAttributes.HitExplosion * focusBonus) != 0)
            {
                list.Add(1158922, ((int)fprop).ToString()); // hit explosion ~1_val~%
            }

            if (SearingWeapon)
            {
                list.Add(1151183); // Searing Weapon
            }

            if ((fprop = m_AosWeaponAttributes.HitMagicArrow * focusBonus) != 0)
            {
                list.Add(1060426, ((int)fprop).ToString()); // hit magic arrow ~1_val~%
            }
            else if (bonus == AosWeaponAttribute.HitMagicArrow && enchantBonus != 0)
            {
                list.Add(1060426, ((int)(enchantBonus * focusBonus)).ToString()); // hit magic arrow ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitPhysicalArea * focusBonus) != 0)
            {
                list.Add(1060428, ((int)fprop).ToString()); // hit physical area ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitFireArea * focusBonus) != 0)
            {
                list.Add(1060419, ((int)fprop).ToString()); // hit fire area ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitColdArea * focusBonus) != 0)
            {
                list.Add(1060416, ((int)fprop).ToString()); // hit cold area ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitPoisonArea * focusBonus) != 0)
            {
                list.Add(1060429, ((int)fprop).ToString()); // hit poison area ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitEnergyArea * focusBonus) != 0)
            {
                list.Add(1060418, ((int)fprop).ToString()); // hit energy area ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitLeechStam * focusBonus) != 0)
            {
                list.Add(1060430, Math.Min(100, (int)fprop).ToString()); // hit stamina leech ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitLeechMana * focusBonus) != 0)
            {
                list.Add(1060427, Math.Min(100, (int)fprop).ToString()); // hit mana leech ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitLeechHits * focusBonus) != 0)
            {
                list.Add(1060422, Math.Min(100, (int)fprop).ToString()); // hit life leech ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitFatigue * focusBonus) != 0)
            {
                list.Add(1113700, ((int)fprop).ToString()); // Hit Fatigue ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitManaDrain * focusBonus) != 0)
            {
                list.Add(1113699, ((int)fprop).ToString()); // Hit Mana Drain ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitCurse * focusBonus) != 0)
            {
                list.Add(1113712, ((int)fprop).ToString()); // Hit Curse ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitLowerAttack * focusBonus) != 0)
            {
                list.Add(1060424, ((int)fprop).ToString()); // hit lower attack ~1_val~%
            }

            if ((fprop = m_AosWeaponAttributes.HitLowerDefend * focusBonus) != 0)
            {
                list.Add(1060425, ((int)fprop).ToString()); // hit lower defense ~1_val~%
            }

            if ((prop = m_AosWeaponAttributes.BloodDrinker) != 0)
            {
                list.Add(1113591, prop.ToString()); // Blood Drinker
            }

            if ((prop = m_AosWeaponAttributes.BattleLust) != 0)
            {
                list.Add(1113710, prop.ToString()); // Battle Lust
            }

            if (ImmolatingWeaponSpell.IsImmolating(RootParent as Mobile, this))
            {
                list.Add(1111917); // Immolated
            }

            if (this is BaseRanged && (prop = ((BaseRanged)this).Velocity) != 0)
            {
                list.Add(1072793, prop.ToString()); // Velocity ~1_val~%
            }

            if ((prop = m_AosAttributes.LowerAmmoCost) != 0)
            {
                list.Add(1075208, prop.ToString()); // Lower Ammo Cost ~1_Percentage~%
            }

            if ((prop = m_ExtendedWeaponAttributes.MysticWeapon) != 0)
            {
                list.Add(1155881, (30 - prop).ToString());   // mystic weapon -~1_val~ skill
            }
            else if ((prop = Parent is Mobile ? Enhancement.GetValue((Mobile)Parent, ExtendedWeaponAttribute.MysticWeapon) : 0) != 0)
            {
                list.Add(1155881, (30 - prop).ToString());   // mystic weapon -~1_val~ skill
            }

            if ((prop = m_AosWeaponAttributes.SelfRepair) != 0)
            {
                list.Add(1060450, prop.ToString()); // self repair ~1_val~
            }

            if ((prop = m_AosAttributes.NightSight) != 0)
            {
                list.Add(1060441); // night sight
            }

            if ((prop = fcMalus ? 1 : m_AosAttributes.SpellChanneling) != 0)
            {
                list.Add(1060482); // spell channeling
            }

            if ((prop = m_AosWeaponAttributes.MageWeapon) != 0)
            {
                list.Add(1060438, (30 - prop).ToString()); // mage weapon -~1_val~ skill
            }

            if (m_AosAttributes.BalancedWeapon > 0 && Layer == Layer.TwoHanded)
            {
                list.Add(1072792); // Balanced
            }

            if ((prop = (GetLuckBonus() + m_AosAttributes.Luck)) != 0)
            {
                list.Add(1060436, prop.ToString()); // luck ~1_val~
            }

            if ((prop = m_AosAttributes.EnhancePotions) != 0)
            {
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%
            }

            if ((prop = m_AosWeaponAttributes.ReactiveParalyze) != 0)
            {
                list.Add(1112364); // reactive paralyze
            }

            if ((prop = m_AosAttributes.BonusStr) != 0)
            {
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~
            }

            if ((prop = m_AosAttributes.BonusInt) != 0)
            {
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~
            }

            if ((prop = m_AosAttributes.BonusDex) != 0)
            {
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~
            }

            if ((prop = m_AosAttributes.BonusHits) != 0)
            {
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~
            }

            if ((prop = m_AosAttributes.BonusStam) != 0)
            {
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~
            }

            if ((prop = m_AosAttributes.BonusMana) != 0)
            {
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~
            }

            if ((prop = m_AosAttributes.RegenHits) != 0)
            {
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~
            }

            if ((prop = m_AosAttributes.RegenStam) != 0)
            {
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~
            }

            if ((prop = m_AosAttributes.RegenMana) != 0)
            {
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~
            }

            if ((prop = m_AosAttributes.ReflectPhysical) != 0)
            {
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%
            }

            if ((prop = m_AosAttributes.SpellDamage) != 0)
            {
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%
            }

            if ((prop = m_AosAttributes.CastRecovery) != 0)
            {
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~
            }

            if ((prop = fcMalus ? m_AosAttributes.CastSpeed - 1 : m_AosAttributes.CastSpeed) != 0)
            {
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~
            }

            if ((prop = (m_AosAttributes.AttackChance)) != 0)
            {
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%
            }

            if ((prop = m_AosAttributes.DefendChance) != 0)
            {
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%
            }

            if ((prop = m_AosAttributes.LowerManaCost) != 0)
            {
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%
            }

            if ((prop = m_AosAttributes.LowerRegCost) != 0)
            {
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%
            }

            if ((prop = m_AosAttributes.WeaponSpeed) != 0)
            {
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%
            }

            if ((prop = (m_AosAttributes.WeaponDamage + damBonus)) != 0)
            {
                list.Add(1060401, prop.ToString()); // damage increase ~1_val~%
            }

            if ((prop = m_AosAttributes.IncreasedKarmaLoss) != 0)
            {
                list.Add(1075210, prop.ToString()); // Increased Karma Loss ~1val~%
            }

            #region Stygian Abyss
            if ((prop = m_SAAbsorptionAttributes.CastingFocus) != 0)
            {
                list.Add(1113696, prop.ToString()); // Casting Focus ~1_val~%
            }

            if ((prop = m_SAAbsorptionAttributes.EaterFire) != 0)
            {
                list.Add(1113593, prop.ToString()); // Fire Eater ~1_Val~%
            }

            if ((prop = m_SAAbsorptionAttributes.EaterCold) != 0)
            {
                list.Add(1113594, prop.ToString()); // Cold Eater ~1_Val~%
            }

            if ((prop = m_SAAbsorptionAttributes.EaterPoison) != 0)
            {
                list.Add(1113595, prop.ToString()); // Poison Eater ~1_Val~%
            }

            if ((prop = m_SAAbsorptionAttributes.EaterEnergy) != 0)
            {
                list.Add(1113596, prop.ToString()); // Energy Eater ~1_Val~%
            }

            if ((prop = m_SAAbsorptionAttributes.EaterKinetic) != 0)
            {
                list.Add(1113597, prop.ToString()); // Kinetic Eater ~1_Val~%
            }

            if ((prop = m_SAAbsorptionAttributes.EaterDamage) != 0)
            {
                list.Add(1113598, prop.ToString()); // Damage Eater ~1_Val~%
            }

            if ((prop = m_SAAbsorptionAttributes.ResonanceFire) != 0)
            {
                list.Add(1113691, prop.ToString()); // Fire Resonance ~1_val~%
            }

            if ((prop = m_SAAbsorptionAttributes.ResonanceCold) != 0)
            {
                list.Add(1113692, prop.ToString()); // Cold Resonance ~1_val~%
            }

            if ((prop = m_SAAbsorptionAttributes.ResonancePoison) != 0)
            {
                list.Add(1113693, prop.ToString()); // Poison Resonance ~1_val~%
            }

            if ((prop = m_SAAbsorptionAttributes.ResonanceEnergy) != 0)
            {
                list.Add(1113694, prop.ToString()); // Energy Resonance ~1_val~%
            }

            if ((prop = m_SAAbsorptionAttributes.ResonanceKinetic) != 0)
            {
                list.Add(1113695, prop.ToString()); // Kinetic Resonance ~1_val~%
            }
            #endregion

            base.AddResistanceProperties(list);

            if ((prop = GetLowerStatReq()) != 0)
            {
                list.Add(1060435, prop.ToString()); // lower requirements ~1_val~%
            }

            if ((prop = m_AosWeaponAttributes.UseBestSkill) != 0)
            {
                list.Add(1060400); // use best weapon skill
            }

            int phys, fire, cold, pois, nrgy, chaos, direct;

            GetDamageTypes(null, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct);

            #region Mondain's Legacy
            if (chaos != 0)
            {
                list.Add(1072846, chaos.ToString()); // chaos damage ~1_val~%
            }

            if (direct != 0)
            {
                list.Add(1079978, direct.ToString()); // Direct Damage: ~1_PERCENT~%
            }
            #endregion

            if (phys != 0)
            {
                list.Add(1060403, phys.ToString()); // physical damage ~1_val~%
            }

            if (fire != 0)
            {
                list.Add(1060405, fire.ToString()); // fire damage ~1_val~%
            }

            if (cold != 0)
            {
                list.Add(1060404, cold.ToString()); // cold damage ~1_val~%
            }

            if (pois != 0)
            {
                list.Add(1060406, pois.ToString()); // poison damage ~1_val~%
            }

            if (nrgy != 0)
            {
                list.Add(1060407, nrgy.ToString()); // energy damage ~1_val
            }

            if (chaos != 0)
            {
                list.Add(1072846, chaos.ToString()); // chaos damage ~1_val~%
            }

            if (direct != 0)
            {
                list.Add(1079978, direct.ToString()); // Direct Damage: ~1_PERCENT~%
            }

            list.Add(1061168, "{0}\t{1}", MinDamage.ToString(), MaxDamage.ToString()); // weapon damage ~1_val~ - ~2_val~

            list.Add(1061167, string.Format("{0}s", Speed)); // weapon speed ~1_val~

            if (MaxRange > 1)
            {
                list.Add(1061169, MaxRange.ToString()); // range ~1_val~
            }

            int strReq = AOS.Scale(StrRequirement, 100 - GetLowerStatReq());

            if (strReq > 0)
            {
                list.Add(1061170, strReq.ToString()); // strength requirement ~1_val~
            }

            if (Layer == Layer.TwoHanded)
            {
                list.Add(1061171); // two-handed weapon
            }
            else
            {
                list.Add(1061824); // one-handed weapon
            }

            if (m_AosWeaponAttributes.UseBestSkill == 0)
            {
                switch (Skill)
                {
                    case SkillName.Swords:
                        list.Add(1061172);
                        break; // skill required: swordsmanship
                    case SkillName.Macing:
                        list.Add(1061173);
                        break; // skill required: mace fighting
                    case SkillName.Fencing:
                        list.Add(1061174);
                        break; // skill required: fencing
                    case SkillName.Archery:
                        list.Add(1061175);
                        break; // skill required: archery
                    case SkillName.Throwing:
                        list.Add(1112075); // skill required: throwing
                        break;
                }
            }

            if (m_Hits >= 0 && m_MaxHits > 0)
            {
                list.Add(1060639, "{0}\t{1}", m_Hits, m_MaxHits); // durability ~1_val~ / ~2_val~
            }

            if (IsSetItem && !m_SetEquipped)
            {
                list.Add(1072378); // <br>Only when full set is present:
                GetSetProperties(list);
            }

            if (LastParryChance > 0)
            {
                list.Add(1158861, LastParryChance.ToString()); // Last Parry Chance: ~1_val~%
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

        public bool CanShowPoisonCharges()
        {
            if (PrimaryAbility == WeaponAbility.InfectiousStrike || SecondaryAbility == WeaponAbility.InfectiousStrike)
                return true;

            return RootParent is Mobile && SkillMasterySpell.HasSpell((Mobile)RootParent, typeof(InjectedStrikeSpell));
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool drop = base.DropToWorld(from, p);

            EnchantedHotItemSocket.CheckDrop(from, this);

            return drop;
        }

        public static BaseWeapon Fists { get; set; }

        #region ICraftable Members
        public int OnCraft(
            int quality,
            bool makersMark,
            Mobile from,
            CraftSystem craftSystem,
            Type typeRes,
            ITool tool,
            CraftItem craftItem,
            int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
            {
                Crafter = from;
            }

            PlayerConstructed = true;

            if (typeRes == null)
            {
                typeRes = craftItem.Resources.GetAt(0).ItemType;
            }

            if (!craftItem.ForceNonExceptional)
            {
                Resource = CraftResources.GetFromType(typeRes);
            }

            CraftContext context = craftSystem.GetContext(from);

            if (Quality == ItemQuality.Exceptional)
            {
                Attributes.WeaponDamage += 35;
            }

            if (!craftItem.ForceNonExceptional)
            {
                if (tool is BaseRunicTool)
                {
                    ((BaseRunicTool)tool).ApplyAttributesTo(this);
                }
            }

            if (Quality == ItemQuality.Exceptional)
            {
                double div = Siege.SiegeShard ? 12.5 : 20;

                Attributes.WeaponDamage += (int)(from.Skills.ArmsLore.Value / div);
                from.CheckSkill(SkillName.ArmsLore, 0, 100);
            }

            if (craftItem != null && !craftItem.ForceNonExceptional)
            {
                CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);

                if (resInfo == null)
                {
                    return quality;
                }

                CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

                if (attrInfo == null)
                {
                    return quality;
                }

                DistributeMaterialBonus(attrInfo);
            }
            #endregion

            return quality;
        }

        public virtual void DistributeMaterialBonus(CraftAttributeInfo attrInfo)
        {
            if (m_Resource != CraftResource.Heartwood)
            {
                m_AosAttributes.WeaponDamage += attrInfo.WeaponDamage;
                m_AosAttributes.WeaponSpeed += attrInfo.WeaponSwingSpeed;
                m_AosAttributes.AttackChance += attrInfo.WeaponHitChance;
                m_AosAttributes.RegenHits += attrInfo.WeaponRegenHits;
                m_AosWeaponAttributes.HitLeechHits += attrInfo.WeaponHitLifeLeech;
            }
            else
            {
                switch (Utility.Random(6))
                {
                    case 0: m_AosAttributes.WeaponDamage += attrInfo.WeaponDamage; break;
                    case 1: m_AosAttributes.WeaponSpeed += attrInfo.WeaponSwingSpeed; break;
                    case 2: m_AosAttributes.AttackChance += attrInfo.WeaponHitChance; break;
                    case 3: m_AosAttributes.Luck += attrInfo.WeaponLuck; break;
                    case 4: m_AosWeaponAttributes.LowerStatReq += attrInfo.WeaponLowerRequirements; break;
                    case 5: m_AosWeaponAttributes.HitLeechHits += attrInfo.WeaponHitLifeLeech; break;
                }
            }
        }

        #region Mondain's Legacy Sets
        public override bool OnDragLift(Mobile from)
        {
            if (Parent is Mobile && from == Parent)
            {
                if (IsSetItem && m_SetEquipped)
                {
                    SetHelper.RemoveSetBonus(from, SetID, this);
                }
            }

            return base.OnDragLift(from);
        }

        public virtual SetItem SetID => SetItem.None;
        public virtual int Pieces => 0;

        public virtual bool BardMasteryBonus => (SetID == SetItem.Virtuoso);

        public bool IsSetItem => SetID != SetItem.None;

        private int m_SetHue;
        private bool m_SetEquipped;
        private bool m_LastEquipped;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetHue
        {
            get { return m_SetHue; }
            set
            {
                m_SetHue = value;
                InvalidateProperties();
            }
        }

        public bool SetEquipped { get { return m_SetEquipped; } set { m_SetEquipped = value; } }

        public bool LastEquipped { get { return m_LastEquipped; } set { m_LastEquipped = value; } }

        private AosAttributes m_SetAttributes;
        private AosSkillBonuses m_SetSkillBonuses;
        private int m_SetSelfRepair;
        private int m_SetPhysicalBonus, m_SetFireBonus, m_SetColdBonus, m_SetPoisonBonus, m_SetEnergyBonus;

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes SetAttributes { get { return m_SetAttributes; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosSkillBonuses SetSkillBonuses { get { return m_SetSkillBonuses; } set { } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SetSelfRepair
        {
            get { return m_SetSelfRepair; }
            set
            {
                m_SetSelfRepair = value;
                InvalidateProperties();
            }
        }

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

            if ((prop = m_SetSelfRepair) != 0 && WeaponAttributes.SelfRepair == 0)
            {
                list.Add(1060450, prop.ToString()); // self repair ~1_val~
            }

            SetHelper.GetSetProperties(list, this);
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

    public enum CheckSlayerResult
    {
        None,
        Slayer,
        SuperSlayer,
        Opposition
    }
}
