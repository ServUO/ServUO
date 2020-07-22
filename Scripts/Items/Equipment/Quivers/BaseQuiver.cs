using Server.ContextMenus;
using Server.Engines.Craft;
using Server.Misc;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public interface IRangeDamage
    {
        void AlterRangedDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct);
    }

    [Alterable(typeof(DefTailoring), typeof(GargishLeatherWingArmor), true)]
    public class BaseQuiver : Container, ICraftable, ISetItem, IVvVItem, IOwnerRestricted, IRangeDamage, IArtifact, ICanBeElfOrHuman
    {
        private bool _VvVItem;
        private Mobile _Owner;
        private string _OwnerName;
        private bool _ElvesOnly;

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

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ElfOnly
        {
            get { return _ElvesOnly; }
            set { _ElvesOnly = value; }
        }

        public override int DefaultGumpID => 0x108;

        public override int DefaultMaxItems => 1;

        public override int DefaultMaxWeight => 50;

        public override double DefaultWeight => 2.0;

        public override bool DisplayWeight
        {
            get
            {
                if (IsVvVItem)
                    return true;

                return base.DisplayWeight;
            }
        }

        public virtual int ArtifactRarity => 0;

        private AosAttributes m_Attributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private AosElementAttributes m_Resistances;
        private int m_Capacity;
        private int m_WeightReduction;
        private int m_DamageIncrease;

        public virtual bool CanAlter => true;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsArrowAmmo { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes
        {
            get
            {
                return m_Attributes;
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
        public AosElementAttributes Resistances
        {
            get
            {
                return m_Resistances;
            }
            set
            {
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Capacity
        {
            get
            {
                return m_Capacity;
            }
            set
            {
                m_Capacity = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int LowerAmmoCost
        {
            get
            {
                return m_Attributes.LowerAmmoCost;
            }
            set
            {
                m_Attributes.LowerAmmoCost = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int WeightReduction
        {
            get
            {
                return m_WeightReduction;
            }
            set
            {
                m_WeightReduction = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageIncrease
        {
            get
            {
                return m_DamageIncrease;
            }
            set
            {
                m_DamageIncrease = value;
                InvalidateProperties();
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
            }
        }

        public Item Ammo => Items.Count > 0 ? Items[0] : null;

        public BaseQuiver()
            : this(0x2FB7)
        {
        }

        public BaseQuiver(int itemID)
            : base(itemID)
        {
            Weight = 2.0;
            Capacity = 500;
            Layer = Layer.Cloak;

            m_Attributes = new AosAttributes(this);
            m_AosSkillBonuses = new AosSkillBonuses(this);
            m_Resistances = new AosElementAttributes(this);
            m_SetAttributes = new AosAttributes(this);
            m_SetSkillBonuses = new AosSkillBonuses(this);
            DamageIncrease = 10;
            IsArrowAmmo = true;
        }

        public BaseQuiver(Serial serial)
            : base(serial)
        {
        }

        public override bool DisplaysContent => false;

        public override void OnAfterDuped(Item newItem)
        {
            BaseQuiver quiver = newItem as BaseQuiver;

            if (quiver != null)
            {
                quiver.m_Attributes = new AosAttributes(newItem, m_Attributes);
                quiver.m_AosSkillBonuses = new AosSkillBonuses(newItem, m_AosSkillBonuses);
                quiver.m_Resistances = new AosElementAttributes(newItem, m_Resistances);
                quiver.m_SetAttributes = new AosAttributes(newItem, m_SetAttributes);
                quiver.m_SetSkillBonuses = new AosSkillBonuses(newItem, m_SetSkillBonuses);
            }

            GargishLeatherWingArmor wing = newItem as GargishLeatherWingArmor;

            if (wing != null)
            {
                int phys, fire, cold, pois, nrgy, chaos, direct;
                phys = fire = cold = pois = nrgy = chaos = direct = 0;

                AlterRangedDamage(ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct);

                wing.AosElementDamages.Physical = phys;
                wing.AosElementDamages.Fire = fire;
                wing.AosElementDamages.Cold = cold;
                wing.AosElementDamages.Poison = pois;
                wing.AosElementDamages.Energy = nrgy;
                wing.AosElementDamages.Chaos = chaos;
                wing.AosElementDamages.Direct = direct;
            }

            base.OnAfterDuped(newItem);
        }

        public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            InvalidateProperties();

            base.UpdateTotal(sender, type, delta);
        }

        public override int GetTotal(TotalType type)
        {
            int total = base.GetTotal(type);

            if (type == TotalType.Weight)
                total -= total * m_WeightReduction / 100;

            return total;
        }

        private static readonly Type[] m_Ammo = new Type[]
        {
            typeof(Arrow), typeof(Bolt)
        };

        public bool CheckType(Item item)
        {
            Type type = item.GetType();
            Item ammo = Ammo;

            if (ammo != null)
            {
                if (ammo.GetType() == type)
                    return true;
            }
            else
            {
                for (int i = 0; i < m_Ammo.Length; i++)
                {
                    if (type == m_Ammo[i])
                        return true;
                }
            }

            return false;
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (!Movable)
                return false;

            if (!CheckType(item))
            {
                if (message)
                    m.SendLocalizedMessage(1074836); // The container can not hold that type of object.

                return false;
            }

            Item ammo = Ammo;

            if (ammo != null && ammo.Amount > 0)
            {
                if (IsArrowAmmo && item is Bolt)
                    return false;

                if (!IsArrowAmmo && item is Arrow)
                    return false;
            }

            if (!checkItems || Items.Count < DefaultMaxItems)
            {
                int currentAmount = 0;

                Items.ForEach(i => currentAmount += i.Amount);

                if (item.Amount + currentAmount <= m_Capacity)
                    return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
            }

            return false;
        }

        public override bool CheckStack(Mobile from, Item item)
        {
            if (!CheckType(item))
            {
                return false;
            }

            Item ammo = Ammo;

            if (ammo != null)
            {
                int currentAmount = Items.Sum(i => i.Amount);

                if (item.Amount + currentAmount <= m_Capacity)
                    return base.CheckStack(from, item);
            }

            return false;
        }

        public override void AddItem(Item dropped)
        {
            base.AddItem(dropped);

            InvalidateWeight();
            IsArrowAmmo = dropped is Arrow;
        }

        public override void RemoveItem(Item dropped)
        {
            base.RemoveItem(dropped);

            InvalidateWeight();
        }

        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                Mobile mob = (Mobile)parent;

                m_Attributes.AddStatBonuses(mob);
                m_AosSkillBonuses.AddTo(mob);

                BaseRanged ranged = mob.Weapon as BaseRanged;

                if (ranged != null)
                    ranged.InvalidateProperties();

                if (IsSetItem)
                {
                    m_SetEquipped = SetHelper.FullSetEquipped(mob, SetID, Pieces);

                    if (m_SetEquipped)
                    {
                        m_LastEquipped = true;
                        SetHelper.AddSetBonus(mob, SetID);
                    }
                }
            }
        }

        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile mob = (Mobile)parent;

                m_Attributes.RemoveStatBonuses(mob);
                m_AosSkillBonuses.Remove();

                if (IsSetItem && m_SetEquipped)
                    SetHelper.RemoveSetBonus(mob, SetID, this);
            }
        }

        public override bool OnDragLift(Mobile from)
        {
            if (Parent is Mobile && from == Parent)
            {
                if (IsSetItem && m_SetEquipped)
                    SetHelper.RemoveSetBonus(from, SetID, this);
            }

            return base.OnDragLift(from);
        }

        public override bool CanEquip(Mobile m)
        {
            if (!RaceDefinitions.ValidateEquipment(m, this))
            { 
                return false;
            }

            if (m.IsPlayer())
            {
                if (_Owner != null && m != _Owner)
                {
                    m.SendLocalizedMessage(501023); // You must be the owner to use this item.
                    return false;
                }

                if (this is IAccountRestricted && ((IAccountRestricted)this).Account != null)
                {
                    Accounting.Account acct = m.Account as Accounting.Account;

                    if (acct == null || acct.Username != ((IAccountRestricted)this).Account)
                    {
                        m.SendLocalizedMessage(1071296); // This item is Account Bound and your character is not bound to it. You cannot use this item.
                        return false;
                    }
                }

                if (IsVvVItem && !Engines.VvV.ViceVsVirtueSystem.IsVvV(m))
                {
                    m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                    return false;
                }
            }

            return true;
        }

        public virtual int BasePhysicalResistance => 0;
        public virtual int BaseFireResistance => 0;
        public virtual int BaseColdResistance => 0;
        public virtual int BasePoisonResistance => 0;
        public virtual int BaseEnergyResistance => 0;

        public override int PhysicalResistance => BasePhysicalResistance + m_Resistances.Physical;
        public override int FireResistance => BaseFireResistance + m_Resistances.Fire;
        public override int ColdResistance => BaseColdResistance + m_Resistances.Cold;
        public override int PoisonResistance => BasePoisonResistance + m_Resistances.Poison;
        public override int EnergyResistance => BaseEnergyResistance + m_Resistances.Energy;

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);

            if (IsVvVItem)
                list.Add(1154937); // VvV Item
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
                list.Add(1063341); // exceptional
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            m_AosSkillBonuses.GetProperties(list);

            Item ammo = Ammo;

            if (ammo != null)
            {
                if (ammo is Arrow)
                    list.Add(1075265, "{0}\t{1}", ammo.Amount, Capacity); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ arrows
                else if (ammo is Bolt)
                    list.Add(1075266, "{0}\t{1}", ammo.Amount, Capacity); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ bolts
            }
            else
                list.Add(1075265, "{0}\t{1}", 0, Capacity); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ arrows


            if (ArtifactRarity > 0)
            {
                list.Add(1061078, ArtifactRarity.ToString()); // artifact rarity ~1_val~
            }

            int prop;

            if ((prop = m_DamageIncrease) != 0)
                list.Add(1074762, prop.ToString()); // Damage modifier: ~1_PERCENT~%

            int phys, fire, cold, pois, nrgy, chaos, direct;
            phys = fire = cold = pois = nrgy = chaos = direct = 0;

            AlterRangedDamage(ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct);

            if (phys != 0)
                list.Add(1060403, phys.ToString()); // physical damage ~1_val~%

            if (fire != 0)
                list.Add(1060405, fire.ToString()); // fire damage ~1_val~%

            if (cold != 0)
                list.Add(1060404, cold.ToString()); // cold damage ~1_val~%

            if (pois != 0)
                list.Add(1060406, pois.ToString()); // poison damage ~1_val~%

            if (nrgy != 0)
                list.Add(1060407, nrgy.ToString()); // energy damage ~1_val

            if (chaos != 0)
                list.Add(1072846, chaos.ToString()); // chaos damage ~1_val~%

            if (direct != 0)
                list.Add(1079978, direct.ToString()); // Direct Damage: ~1_PERCENT~%

            if ((prop = m_Attributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = m_Attributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = m_Attributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = m_Attributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = m_Attributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = m_Attributes.AttackChance) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = m_Attributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = m_Attributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = m_Attributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = m_Attributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%	

            if ((prop = m_Attributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = m_Attributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = m_Attributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = m_Attributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = m_Attributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = m_Attributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = m_Attributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = m_Attributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = m_Attributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = m_Attributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = m_Attributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            if ((prop = m_Attributes.LowerAmmoCost) > 0)
                list.Add(1075208, prop.ToString()); // Lower Ammo Cost ~1_Percentage~%

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
                    SetHelper.GetSetProperties(list, this);
                }
            }

            base.AddResistanceProperties(list);

            double weight = 0;

            if (ammo != null)
                weight = ammo.Weight * ammo.Amount;

            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", Items.Count, DefaultMaxItems, (int)weight, DefaultMaxWeight); // Contents: ~1_COUNT~/~2_MAXCOUNT items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones

            if ((prop = m_WeightReduction) != 0)
                list.Add(1072210, prop.ToString()); // Weight reduction: ~1_PERCENTAGE~%	

            if (IsSetItem && !m_SetEquipped)
            {
                list.Add(1072378); // <br>Only when full set is present:				
                SetHelper.GetSetProperties(list, this);
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
            Attributes = 0x00000001,
            DamageModifier = 0x00000002,
            LowerAmmoCost = 0x00000004,
            WeightReduction = 0x00000008,
            Crafter = 0x00000010,
            Quality = 0x00000020,
            Capacity = 0x00000040,
            DamageIncrease = 0x00000080,
            SetAttributes = 0x00000100,
            SetHue = 0x00000200,
            LastEquipped = 0x00000400,
            SetEquipped = 0x00000800,
            SetSkillAttributes = 0x00002000,
            SetPhysical = 0x00004000,
            SetFire = 0x00008000,
            SetCold = 0x00010000,
            SetPoison = 0x00020000,
            SetEnergy = 0x00040000,
            ElvesOnly = 0x00080000,
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            if (from.Items.Contains(this) || (from.Backpack != null && IsChildOf(from.Backpack)))
                list.Add(new RefillQuiverEntry(this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(4); // version

            writer.Write(_VvVItem);
            writer.Write(_Owner);
            writer.Write(_OwnerName);

            // Version 3 takes out LowerAmmoCost

            SaveFlag flags = SaveFlag.None;

            // Version 2
            writer.Write(IsArrowAmmo);

            // Version 1
            m_AosSkillBonuses.Serialize(writer);
            m_Resistances.Serialize(writer);

            SetSaveFlag(ref flags, SaveFlag.Attributes, !m_Attributes.IsEmpty);
            //SetSaveFlag(ref flags, SaveFlag.LowerAmmoCost, m_LowerAmmoCost != 0);
            SetSaveFlag(ref flags, SaveFlag.WeightReduction, m_WeightReduction != 0);
            SetSaveFlag(ref flags, SaveFlag.DamageIncrease, m_DamageIncrease != 0);
            SetSaveFlag(ref flags, SaveFlag.Crafter, m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Quality, true);
            SetSaveFlag(ref flags, SaveFlag.Capacity, m_Capacity > 0);

            #region Mondain's Legacy Sets
            SetSaveFlag(ref flags, SaveFlag.SetAttributes, !m_SetAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SetSkillAttributes, !m_SetSkillBonuses.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SetHue, m_SetHue != 0);
            SetSaveFlag(ref flags, SaveFlag.LastEquipped, m_LastEquipped);
            SetSaveFlag(ref flags, SaveFlag.SetEquipped, m_SetEquipped);
            SetSaveFlag(ref flags, SaveFlag.SetPhysical, m_SetPhysicalBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.SetFire, m_SetFireBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.SetCold, m_SetColdBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.SetPoison, m_SetPoisonBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.SetEnergy, m_SetEnergyBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.ElvesOnly, _ElvesOnly);
            #endregion

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.Attributes))
                m_Attributes.Serialize(writer);

            //if (GetSaveFlag(flags, SaveFlag.LowerAmmoCost))
            //    writer.Write((int)m_LowerAmmoCost);

            if (GetSaveFlag(flags, SaveFlag.WeightReduction))
                writer.Write(m_WeightReduction);

            if (GetSaveFlag(flags, SaveFlag.DamageIncrease))
                writer.Write(m_DamageIncrease);

            if (GetSaveFlag(flags, SaveFlag.Crafter))
                writer.Write(m_Crafter);

            if (GetSaveFlag(flags, SaveFlag.Quality))
                writer.Write((int)m_Quality);

            if (GetSaveFlag(flags, SaveFlag.Capacity))
                writer.Write(m_Capacity);

            #region Mondain's Legacy Sets
            if (GetSaveFlag(flags, SaveFlag.SetPhysical))
                writer.WriteEncodedInt(m_SetPhysicalBonus);

            if (GetSaveFlag(flags, SaveFlag.SetFire))
                writer.WriteEncodedInt(m_SetFireBonus);

            if (GetSaveFlag(flags, SaveFlag.SetCold))
                writer.WriteEncodedInt(m_SetColdBonus);

            if (GetSaveFlag(flags, SaveFlag.SetPoison))
                writer.WriteEncodedInt(m_SetPoisonBonus);

            if (GetSaveFlag(flags, SaveFlag.SetEnergy))
                writer.WriteEncodedInt(m_SetEnergyBonus);

            if (GetSaveFlag(flags, SaveFlag.SetAttributes))
                m_SetAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SetSkillAttributes))
                m_SetSkillBonuses.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SetHue))
                writer.Write(m_SetHue);

            if (GetSaveFlag(flags, SaveFlag.LastEquipped))
                writer.Write(m_LastEquipped);

            if (GetSaveFlag(flags, SaveFlag.SetEquipped))
                writer.Write(m_SetEquipped);

            if (GetSaveFlag(flags, SaveFlag.ElvesOnly))
                writer.Write(_ElvesOnly);
            #endregion
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    {
                        _VvVItem = reader.ReadBool();
                        _Owner = reader.ReadMobile();
                        _OwnerName = reader.ReadString();
                        goto case 3;
                    }
                case 3:
                case 2:
                    IsArrowAmmo = reader.ReadBool();
                    goto case 1;
                case 1:
                    {
                        if (version == 1)
                        {
                            IsArrowAmmo = (Ammo == null || Ammo is Arrow);
                        }
                        m_AosSkillBonuses = new AosSkillBonuses(this, reader);
                        m_Resistances = new AosElementAttributes(this, reader);
                        goto case 0;
                    }
                case 0:
                    {
                        if (version == 0)
                        {
                            m_AosSkillBonuses = new AosSkillBonuses(this);
                            m_Resistances = new AosElementAttributes(this);
                        }

                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Attributes))
                            m_Attributes = new AosAttributes(this, reader);
                        else
                            m_Attributes = new AosAttributes(this);

                        if (version < 3 && GetSaveFlag(flags, SaveFlag.LowerAmmoCost))
                            m_Attributes.LowerAmmoCost = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.WeightReduction))
                            m_WeightReduction = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.DamageIncrease))
                            m_DamageIncrease = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Crafter))
                            m_Crafter = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.Quality))
                            m_Quality = (ItemQuality)reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Capacity))
                            m_Capacity = reader.ReadInt();

                        #region Mondain's Legacy Sets
                        if (GetSaveFlag(flags, SaveFlag.SetPhysical))
                            m_SetPhysicalBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.SetFire))
                            m_SetFireBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.SetCold))
                            m_SetColdBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.SetPoison))
                            m_SetPoisonBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.SetEnergy))
                            m_SetEnergyBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.SetAttributes))
                            m_SetAttributes = new AosAttributes(this, reader);
                        else
                            m_SetAttributes = new AosAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.SetSkillAttributes))
                            m_SetSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            m_SetSkillBonuses = new AosSkillBonuses(this);

                        if (GetSaveFlag(flags, SaveFlag.SetHue))
                            m_SetHue = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.LastEquipped))
                            m_LastEquipped = reader.ReadBool();

                        if (GetSaveFlag(flags, SaveFlag.SetEquipped))
                            m_SetEquipped = reader.ReadBool();

                        if (GetSaveFlag(flags, SaveFlag.ElvesOnly))
                            _ElvesOnly = reader.ReadBool();
                        #endregion

                        break;
                    }
            }

            int strBonus = ComputeStatBonus(StatType.Str);
            int dexBonus = ComputeStatBonus(StatType.Dex);
            int intBonus = ComputeStatBonus(StatType.Int);

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
            {
                m_AosSkillBonuses.AddTo((Mobile)Parent);
                ((Mobile)Parent).CheckStatTimers();
            }
        }

        public int ComputeStatBonus(StatType type)
        {
            switch (type)
            {
                case StatType.Str: return Attributes.BonusStr;
                case StatType.Dex: return Attributes.BonusDex;
                case StatType.Int: return Attributes.BonusInt;
            }

            return 0;
        }

        public virtual void AlterBowDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct)
        {
        }

        public virtual void AlterRangedDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct)
        {
            AlterBowDamage(ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct);
        }

        public void InvalidateWeight()
        {
            if (RootParent is Mobile)
            {
                Mobile m = (Mobile)RootParent;

                m.UpdateTotals();
            }
        }

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            Quality = (ItemQuality)quality;

            if (makersMark)
                Crafter = from;

            return quality;
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

        public class RefillQuiverEntry : ContextMenuEntry
        {
            private readonly BaseQuiver m_quiver;

            public RefillQuiverEntry(BaseQuiver bq)
            : base(6230)
            {
                m_quiver = bq;

                Enabled = m_quiver.Ammo == null || m_quiver.Ammo.Amount < m_quiver.Capacity;
            }

            bool Refill<T>(Mobile m, Container c) where T : Item
            {
                List<T> list = c.FindItemsByType<T>(true).ToList();

                if (list.Count > 0)
                {
                    int amt = 0;
                    list = list.OrderByDescending(e => e.Amount).ToList();

                    int famount = m_quiver.Ammo == null ? 0 : m_quiver.Ammo.Amount;
                    if (m_quiver.Ammo != null)
                        m_quiver.Ammo.Delete();

                    while ((famount < m_quiver.Capacity) && (list.Count > 0))
                    {
                        T data = list[list.Count - 1];
                        int remaining = m_quiver.Capacity - famount;
                        if (data.Amount > remaining)
                        {
                            famount += remaining;
                            amt += remaining;
                            data.Amount -= remaining;
                        }
                        else
                        {
                            famount += data.Amount;
                            amt += data.Amount;
                            data.Delete();
                            list.RemoveAt(list.Count - 1);
                        }
                    }

                    if ((amt > 0) && (m != null))
                    {
                        T obj = (T)Activator.CreateInstance(typeof(T));
                        obj.Amount = famount;
                        m_quiver.DropItem(obj);
                        m.SendLocalizedMessage(1072664, amt.ToString());
                        return true;
                    }
                }
                return false;
            }

            public override void OnClick()
            {
                if ((m_quiver == null) || m_quiver.Deleted || (m_quiver.Ammo != null && m_quiver.Ammo.Amount >= m_quiver.Capacity))
                    return;

                object owner = m_quiver.Parent;
                while (owner != null)
                {
                    if (owner is Mobile)
                        break;
                    if (owner is Item)
                    {
                        owner = ((Item)owner).Parent;
                        continue;
                    }
                    owner = null;
                }

                if (owner == null)
                    return;

                if (!(owner is Mobile))
                    return;

                Mobile m = (Mobile)owner;


                if (m.Backpack == null)
                    return;

                if (!(m.Items.Contains(m_quiver) || m_quiver.IsChildOf(m.Backpack)))
                    return;

                // Try to fill from the bank box
                if ((m.BankBox != null) && (m.BankBox.Opened))
                {
                    if (m_quiver.IsArrowAmmo ? Refill<Arrow>(m, m.BankBox) : Refill<Bolt>(m, m.BankBox))
                        return;
                }

                // Otherwise look for secure containers within two tiles
                IPooledEnumerable<Item> items = m.Map.GetItemsInRange(m.Location, 1);
                foreach (Item i in items)
                {
                    if (!(i is Container))
                        continue;

                    Container c = (Container)i;

                    if (!c.IsSecure || !c.IsAccessibleTo(m))
                        continue;

                    if (m_quiver.IsArrowAmmo ? Refill<Arrow>(m, c) : Refill<Bolt>(m, c))
                        return;
                }

                m.SendLocalizedMessage(1072673); //There are no source containers nearby.
            }
        }
    }
}
