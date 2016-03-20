using System;
using Server.Engines.Craft;
using System.Linq;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
    public class BaseQuiver : Container, ICraftable, ISetItem
    {
        public override int DefaultGumpID
        {
            get
            {
                return 0x108;
            }
        }
        public override int DefaultMaxItems
        {
            get
            {
                return 1;
            }
        }
        public override int DefaultMaxWeight
        {
            get
            {
                return 50;
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 2.0;
            }
        }

        private AosAttributes m_Attributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private AosElementAttributes m_Resistances;
        private int m_Capacity;
        //private int m_LowerAmmoCost;
        private int m_WeightReduction;
        private int m_DamageIncrease;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsArrowAmmo { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public AosAttributes Attributes
        {
            get
            {
                return this.m_Attributes;
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
                return this.m_Resistances;
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
                return this.m_Capacity;
            }
            set
            {
                this.m_Capacity = value;
                this.InvalidateProperties();
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
                return this.m_WeightReduction;
            }
            set
            {
                this.m_WeightReduction = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageIncrease
        {
            get
            {
                return this.m_DamageIncrease;
            }
            set
            {
                this.m_DamageIncrease = value;
                this.InvalidateProperties();
            }
        }

        private Mobile m_Crafter;
        private ClothingQuality m_Quality;
        private bool m_PlayerConstructed;

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
		
        public Item Ammo
        {
            get
            {
                return this.Items.Count > 0 ? this.Items[0] : null;
            }
        }

        public BaseQuiver()
            : this(0x2FB7)
        {
        }

        public BaseQuiver(int itemID)
            : base(itemID)
        {
            this.Weight = 2.0;
            this.Capacity = 500;
            this.Layer = Layer.Cloak;

            this.m_Attributes = new AosAttributes(this);
            this.m_AosSkillBonuses = new AosSkillBonuses(this);
            this.m_Resistances = new AosElementAttributes(this);
            this.m_SetAttributes = new AosAttributes(this);
            this.m_SetSkillBonuses = new AosSkillBonuses(this);
            this.DamageIncrease = 10;
            IsArrowAmmo = true;
        }

        public BaseQuiver(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDuped(Item newItem)
        {
            BaseQuiver quiver = newItem as BaseQuiver;

            if (quiver == null)
                return;

            quiver.m_Attributes = new AosAttributes(newItem, this.m_Attributes);
            quiver.m_AosSkillBonuses = new AosSkillBonuses(newItem, this.m_AosSkillBonuses);
            quiver.m_Resistances = new AosElementAttributes(newItem, this.m_Resistances);
        }

        public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            this.InvalidateProperties();

            base.UpdateTotal(sender, type, delta);
        }

        public override int GetTotal(TotalType type)
        {
            int total = base.GetTotal(type);

            if (type == TotalType.Weight)
                total -= total * this.m_WeightReduction / 100;

            return total;
        }

        private static readonly Type[] m_Ammo = new Type[]
        {
            typeof(Arrow), typeof(Bolt)
        };

        public bool CheckType(Item item)
        {
            Type type = item.GetType();
            Item ammo = this.Ammo;

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
            if (!this.CheckType(item))
            {
                if (message)
                    m.SendLocalizedMessage(1074836); // The container can not hold that type of object.

                return false;
            }

            Item ammo = Ammo;
            if(ammo != null && ammo.Amount > 0)
            {
                if (IsArrowAmmo && item is Bolt)
                    return false;
                if (!IsArrowAmmo && item is Arrow)
                    return false;
            }

            if (this.Items.Count < this.DefaultMaxItems)
            {
                if (item.Amount <= this.m_Capacity)
                    return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);

                return false;
            }
            else if (checkItems)
                return false;

            if (ammo == null || ammo.Deleted)
                return false;

            return false;
        }

        public override void AddItem(Item dropped)
        {
            base.AddItem(dropped);

            this.InvalidateWeight();
            IsArrowAmmo = dropped is Arrow;
        }
		
        public override void RemoveItem(Item dropped)
        {
            base.RemoveItem(dropped);

            this.InvalidateWeight();
        }
		
        public override void OnAdded(object parent)
        {
            if (parent is Mobile)
            {
                Mobile mob = (Mobile)parent;

                this.m_Attributes.AddStatBonuses(mob);
                this.m_AosSkillBonuses.AddTo(mob);

                BaseRanged ranged = mob.Weapon as BaseRanged;

                if (ranged != null)
                    ranged.InvalidateProperties();

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
            }
        }
		
        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
                Mobile mob = (Mobile)parent;

                this.m_Attributes.RemoveStatBonuses(mob);
                this.m_AosSkillBonuses.Remove();

                #region Mondain's Legacy Sets
                if (this.IsSetItem && this.m_SetEquipped)
                    SetHelper.RemoveSetBonus(mob, this.SetID, this);
                #endregion
            }
        }

        public override bool OnDragLift(Mobile from)
        {
            #region Mondain's Legacy Sets
            if (this.Parent is Mobile && from == this.Parent)
            {
                if (this.IsSetItem && this.m_SetEquipped)
                    SetHelper.RemoveSetBonus(from, this.SetID, this);
            }
            #endregion

            return base.OnDragLift(from);
        }

        public override bool CanEquip(Mobile m)
        {
            if (m.NetState != null && !m.NetState.SupportsExpansion(Expansion.ML))
            {
                m.SendLocalizedMessage(1072791); // You must upgrade to Mondain's Legacy in order to use that item.				
                return false;
            }

            return true;
        }

        public virtual int BasePhysicalResistance { get { return 0; } }
        public virtual int BaseFireResistance { get { return 0; } }
        public virtual int BaseColdResistance { get { return 0; } }
        public virtual int BasePoisonResistance { get { return 0; } }
        public virtual int BaseEnergyResistance { get { return 0; } }

        public override int PhysicalResistance { get { return this.BasePhysicalResistance + this.m_Resistances.Physical; } }
        public override int FireResistance { get { return this.BaseFireResistance + this.m_Resistances.Fire; } }
        public override int ColdResistance { get { return this.BaseColdResistance + this.m_Resistances.Cold; } }
        public override int PoisonResistance { get { return this.BasePoisonResistance + this.m_Resistances.Poison; } }
        public override int EnergyResistance { get { return this.BaseEnergyResistance + this.m_Resistances.Energy; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
				
            if (this.m_Crafter != null)
				list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~

            if (this.m_Quality == ClothingQuality.Exceptional)
                list.Add(1063341); // exceptional

            this.m_AosSkillBonuses.GetProperties(list);

            Item ammo = this.Ammo;

            if (ammo != null)
            {
                if (ammo is Arrow)
                    list.Add(1075265, "{0}\t{1}", ammo.Amount, this.Capacity); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ arrows
                else if (ammo is Bolt)
                    list.Add(1075266, "{0}\t{1}", ammo.Amount, this.Capacity); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ bolts
            }
            else
                list.Add(1075265, "{0}\t{1}", 0, this.Capacity); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ arrows

            int prop;

            if ((prop = this.m_DamageIncrease) != 0)
                list.Add(1074762, prop.ToString()); // Damage modifier: ~1_PERCENT~%
			
            int phys, fire, cold, pois, nrgy, chaos, direct;
            phys = fire = cold = pois = nrgy = chaos = direct = 0;

            this.AlterBowDamage(ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct);

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

            if ((prop = this.m_Attributes.DefendChance) != 0)
                list.Add(1060408, prop.ToString()); // defense chance increase ~1_val~%

            if ((prop = this.m_Attributes.BonusDex) != 0)
                list.Add(1060409, prop.ToString()); // dexterity bonus ~1_val~

            if ((prop = this.m_Attributes.EnhancePotions) != 0)
                list.Add(1060411, prop.ToString()); // enhance potions ~1_val~%

            if ((prop = this.m_Attributes.CastRecovery) != 0)
                list.Add(1060412, prop.ToString()); // faster cast recovery ~1_val~

            if ((prop = this.m_Attributes.CastSpeed) != 0)
                list.Add(1060413, prop.ToString()); // faster casting ~1_val~

            if ((prop = this.m_Attributes.AttackChance) != 0)
                list.Add(1060415, prop.ToString()); // hit chance increase ~1_val~%

            if ((prop = this.m_Attributes.BonusHits) != 0)
                list.Add(1060431, prop.ToString()); // hit point increase ~1_val~

            if ((prop = this.m_Attributes.BonusInt) != 0)
                list.Add(1060432, prop.ToString()); // intelligence bonus ~1_val~

            if ((prop = this.m_Attributes.LowerManaCost) != 0)
                list.Add(1060433, prop.ToString()); // lower mana cost ~1_val~%

            if ((prop = this.m_Attributes.LowerRegCost) != 0)
                list.Add(1060434, prop.ToString()); // lower reagent cost ~1_val~%	

            if ((prop = this.m_Attributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

            if ((prop = this.m_Attributes.BonusMana) != 0)
                list.Add(1060439, prop.ToString()); // mana increase ~1_val~

            if ((prop = this.m_Attributes.RegenMana) != 0)
                list.Add(1060440, prop.ToString()); // mana regeneration ~1_val~

            if ((prop = this.m_Attributes.NightSight) != 0)
                list.Add(1060441); // night sight

            if ((prop = this.m_Attributes.ReflectPhysical) != 0)
                list.Add(1060442, prop.ToString()); // reflect physical damage ~1_val~%

            if ((prop = this.m_Attributes.RegenStam) != 0)
                list.Add(1060443, prop.ToString()); // stamina regeneration ~1_val~

            if ((prop = this.m_Attributes.RegenHits) != 0)
                list.Add(1060444, prop.ToString()); // hit point regeneration ~1_val~

            if ((prop = this.m_Attributes.SpellDamage) != 0)
                list.Add(1060483, prop.ToString()); // spell damage increase ~1_val~%

            if ((prop = this.m_Attributes.BonusStam) != 0)
                list.Add(1060484, prop.ToString()); // stamina increase ~1_val~

            if ((prop = this.m_Attributes.BonusStr) != 0)
                list.Add(1060485, prop.ToString()); // strength bonus ~1_val~

            if ((prop = this.m_Attributes.WeaponSpeed) != 0)
                list.Add(1060486, prop.ToString()); // swing speed increase ~1_val~%

            if ((prop = this.m_Attributes.LowerAmmoCost) > 0)
                list.Add(1075208, prop.ToString()); // Lower Ammo Cost ~1_Percentage~%

            #region Mondain's Legacy Sets
            if (this.IsSetItem)
            {
                list.Add(1073491, this.Pieces.ToString()); // Part of a Weapon/Armor Set (~1_val~ pieces)

                if (this.m_SetEquipped)
                {
                    list.Add(1073492); // Full Weapon/Armor Set Present					
                    SetHelper.GetSetProperties(list, this);
                }
            }
            #endregion

            base.AddResistanceProperties(list);

            double weight = 0;

            if (ammo != null)
                weight = ammo.Weight * ammo.Amount;

            list.Add(1072241, "{0}\t{1}\t{2}\t{3}", this.Items.Count, this.DefaultMaxItems, (int)weight, this.DefaultMaxWeight); // Contents: ~1_COUNT~/~2_MAXCOUNT items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones

            if ((prop = this.m_WeightReduction) != 0)
                list.Add(1072210, prop.ToString()); // Weight reduction: ~1_PERCENTAGE~%	

            #region Mondain's Legacy Sets
            if (this.IsSetItem && !this.m_SetEquipped)
            {
                list.Add(1072378); // <br>Only when full set is present:				
                SetHelper.GetSetProperties(list, this);
            }
            #endregion
        }
		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			if (m_Crafter != null)
			{
				LabelTo(from, 1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
			}
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

            #region Mondain's Legacy Sets
            SetAttributes = 0x00000100,
            SetHue = 0x00000200,
            LastEquipped = 0x00000400,
            SetEquipped = 0x00000800,
            SetSkillAttributes = 0x00002000,
            #endregion

            DamageIncrease = 0x00000080
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            if(from.Items.Contains(this) || (from.Backpack != null && IsChildOf(from.Backpack)))
                list.Add(new RefillQuiverEntry(this));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(3); // version

            // Version 3 takes out LowerAmmoCost

            SaveFlag flags = SaveFlag.None;

            // Version 2
            writer.Write(IsArrowAmmo);

            // Version 1
            m_AosSkillBonuses.Serialize(writer);
            m_Resistances.Serialize(writer);

            SetSaveFlag(ref flags, SaveFlag.Attributes, !this.m_Attributes.IsEmpty);
            //SetSaveFlag(ref flags, SaveFlag.LowerAmmoCost, this.m_LowerAmmoCost != 0);
            SetSaveFlag(ref flags, SaveFlag.WeightReduction, this.m_WeightReduction != 0);
            SetSaveFlag(ref flags, SaveFlag.DamageIncrease, this.m_DamageIncrease != 0);
            SetSaveFlag(ref flags, SaveFlag.Crafter, this.m_Crafter != null);
            SetSaveFlag(ref flags, SaveFlag.Quality, true);
            SetSaveFlag(ref flags, SaveFlag.Capacity, this.m_Capacity > 0);

            #region Mondain's Legacy Sets
            SetSaveFlag(ref flags, SaveFlag.SetAttributes, !this.m_SetAttributes.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SetSkillAttributes, !this.m_SetSkillBonuses.IsEmpty);
            SetSaveFlag(ref flags, SaveFlag.SetHue, this.m_SetHue != 0);
            SetSaveFlag(ref flags, SaveFlag.LastEquipped, this.m_LastEquipped);
            SetSaveFlag(ref flags, SaveFlag.SetEquipped, this.m_SetEquipped);
            #endregion

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.Attributes))
                this.m_Attributes.Serialize(writer);

            //if (GetSaveFlag(flags, SaveFlag.LowerAmmoCost))
            //    writer.Write((int)this.m_LowerAmmoCost);

            if (GetSaveFlag(flags, SaveFlag.WeightReduction))
                writer.Write((int)this.m_WeightReduction);

            if (GetSaveFlag(flags, SaveFlag.DamageIncrease))
                writer.Write((int)this.m_DamageIncrease);

            if (GetSaveFlag(flags, SaveFlag.Crafter))
                writer.Write((Mobile)this.m_Crafter);

            if (GetSaveFlag(flags, SaveFlag.Quality))
                writer.Write((int)this.m_Quality);

            if (GetSaveFlag(flags, SaveFlag.Capacity))
                writer.Write((int)this.m_Capacity);

            #region Mondain's Legacy Sets
            if (GetSaveFlag(flags, SaveFlag.SetAttributes))
                this.m_SetAttributes.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SetSkillAttributes))
                this.m_SetSkillBonuses.Serialize(writer);

            if (GetSaveFlag(flags, SaveFlag.SetHue))
                writer.Write((int)this.m_SetHue);

            if (GetSaveFlag(flags, SaveFlag.LastEquipped))
                writer.Write((bool)this.m_LastEquipped);

            if (GetSaveFlag(flags, SaveFlag.SetEquipped))
                writer.Write((bool)this.m_SetEquipped);
            #endregion
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                case 2:
                    IsArrowAmmo = reader.ReadBool();
                    goto case 1;
                case 1:
                    {
                        if(version == 1)
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
                            this.m_Attributes = new AosAttributes(this, reader);
                        else
                            this.m_Attributes = new AosAttributes(this);

                        if (version < 3 && GetSaveFlag(flags, SaveFlag.LowerAmmoCost))
                            m_Attributes.LowerAmmoCost = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.WeightReduction))
                            this.m_WeightReduction = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.DamageIncrease))
                            this.m_DamageIncrease = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Crafter))
                            this.m_Crafter = reader.ReadMobile();

                        if (GetSaveFlag(flags, SaveFlag.Quality))
                            this.m_Quality = (ClothingQuality)reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.Capacity))
                            this.m_Capacity = reader.ReadInt();

                        #region Mondain's Legacy Sets
                        if (GetSaveFlag(flags, SaveFlag.SetAttributes))
                            this.m_SetAttributes = new AosAttributes(this, reader);
                        else
                            this.m_SetAttributes = new AosAttributes(this);

                        if (GetSaveFlag(flags, SaveFlag.SetSkillAttributes))
                            this.m_SetSkillBonuses = new AosSkillBonuses(this, reader);
                        else
                            this.m_SetSkillBonuses = new AosSkillBonuses(this);

                        if (GetSaveFlag(flags, SaveFlag.SetHue))
                            this.m_SetHue = reader.ReadInt();

                        if (GetSaveFlag(flags, SaveFlag.LastEquipped))
                            this.m_LastEquipped = reader.ReadBool();

                        if (GetSaveFlag(flags, SaveFlag.SetEquipped))
                            this.m_SetEquipped = reader.ReadBool();
                        #endregion

                        break;
                    }
            }
        }

        public virtual void AlterBowDamage(ref int phys, ref int fire, ref int cold, ref int pois, ref int nrgy, ref int chaos, ref int direct)
        {
        }

        public void InvalidateWeight()
        {
            if (this.RootParent is Mobile)
            {
                Mobile m = (Mobile)this.RootParent;

                m.UpdateTotals();
            }
        }
		
        #region ICraftable
        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            this.Quality = (ClothingQuality)quality;

            if (makersMark)
                this.Crafter = from;

            return quality;
        }

        #endregion

        #region Mondain's Legacy Sets
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

        public bool IsSetItem
        {
            get
            {
                return this.SetID != SetItem.None;
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
        #endregion

        public class RefillQuiverEntry : ContextMenuEntry
        {
            private readonly BaseQuiver m_quiver;

            public RefillQuiverEntry(BaseQuiver bq)
            : base(6230)
            {
                m_quiver = bq;
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
                        m_quiver.AddItem(obj);
                        m.SendLocalizedMessage(1072664, amt.ToString());
                        return true;
                    }
                }
                return false;
            }

            public override void OnClick()
            {
                if ((m_quiver == null) || m_quiver.Deleted)
                    return;

                object owner = m_quiver.Parent;
                while(owner != null)
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
                var items = m.Map.GetItemsInRange(m.Location, 1);
                foreach(Item i in items)
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