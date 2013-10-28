using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.Craft;

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

    public abstract class BaseJewel : Item, ICraftable, ISetItem
    {
        private int m_MaxHitPoints;
        private int m_HitPoints;

        private AosAttributes m_AosAttributes;
        private AosElementAttributes m_AosResistances;
        private AosSkillBonuses m_AosSkillBonuses;
        private SAAbsorptionAttributes m_SAAbsorptionAttributes;
        private CraftResource m_Resource;
        private GemType m_GemType;
        private int m_TimesImbued;

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
            private readonly BaseJewel m_Item;

            public UnBlessEntry(Mobile from, BaseJewel item)
                : base(6208, -1)
            {
                this.m_From = from;
                this.m_Item = item;
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

        [CommandProperty(AccessLevel.Player)]
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
        public SAAbsorptionAttributes AbsorptionAttributes
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
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public GemType GemType
        {
            get
            {
                return this.m_GemType;
            }
            set
            {
                this.m_GemType = value;
                this.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TimesImbued
        {
            get
            {
                return this.m_TimesImbued;
            }
            set
            {
                this.m_TimesImbued = value;
                this.InvalidateProperties();
            }
        }

        public override int PhysicalResistance
        {
            get
            {
                return this.m_AosResistances.Physical;
            }
        }
        public override int FireResistance
        {
            get
            {
                return this.m_AosResistances.Fire;
            }
        }
        public override int ColdResistance
        {
            get
            {
                return this.m_AosResistances.Cold;
            }
        }
        public override int PoisonResistance
        {
            get
            {
                return this.m_AosResistances.Poison;
            }
        }
        public override int EnergyResistance
        {
            get
            {
                return this.m_AosResistances.Energy;
            }
        }
        public virtual int BaseGemTypeNumber
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

        public virtual Race RequiredRace
        {
            get
            {
                return null;
            }
        }
        public virtual bool CanBeWornByGargoyles
        {
            get
            {
                return false;
            }
        }

        public override int LabelNumber
        {
            get
            {
                if (this.m_GemType == GemType.None)
                    return base.LabelNumber;

                return this.BaseGemTypeNumber + (int)this.m_GemType - 1;
            }
        }

        public override void OnAfterDuped(Item newItem)
        {
            BaseJewel jewel = newItem as BaseJewel;

            if (jewel == null)
                return;

            jewel.m_AosAttributes = new AosAttributes(newItem, this.m_AosAttributes);
            jewel.m_AosResistances = new AosElementAttributes(newItem, this.m_AosResistances);
            jewel.m_AosSkillBonuses = new AosSkillBonuses(newItem, this.m_AosSkillBonuses);

            #region Mondain's Legacy
            jewel.m_SetAttributes = new AosAttributes(newItem, this.m_SetAttributes);
            jewel.m_SetSkillBonuses = new AosSkillBonuses(newItem, this.m_SetSkillBonuses);
            #endregion

            jewel.m_AosSkillBonuses = new AosSkillBonuses(newItem, this.m_AosSkillBonuses);
        }

        public virtual int ArtifactRarity
        {
            get
            {
                return 0;
            }
        }

        private Mobile m_Crafter;
        private ArmorQuality m_Quality;

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
        public ArmorQuality Quality
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

        public BaseJewel(int itemID, Layer layer)
            : base(itemID)
        {
            this.m_AosAttributes = new AosAttributes(this);
            this.m_AosResistances = new AosElementAttributes(this);
            this.m_AosSkillBonuses = new AosSkillBonuses(this);
            this.m_Resource = CraftResource.Iron;
            this.m_GemType = GemType.None;

            this.Layer = layer;

            this.m_HitPoints = this.m_MaxHitPoints = Utility.RandomMinMax(this.InitMinHits, this.InitMaxHits);

            this.m_SetAttributes = new AosAttributes(this);
            this.m_SetSkillBonuses = new AosSkillBonuses(this);
            this.m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this);
        }

        #region Stygian Abyss
        public override bool CanEquip(Mobile from)
        {
            if (this.BlessedBy != null && this.BlessedBy != from)
            {
                from.SendLocalizedMessage(1075277); // That item is blessed by another player.
                return false;
            }

            if (from.AccessLevel < AccessLevel.GameMaster)
            {
                if (from.Race == Race.Gargoyle && !this.CanBeWornByGargoyles)
                {
                    from.SendLocalizedMessage(1111708); // Gargoyles can't wear this.
                    return false;
                }
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
            }
		
            return base.CanEquip(from);
        }

        #endregion

        public override void OnAdded(object parent)
        {
            if (Core.AOS && parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                this.m_AosSkillBonuses.AddTo(from);

                int strBonus = this.m_AosAttributes.BonusStr;
                int dexBonus = this.m_AosAttributes.BonusDex;
                int intBonus = this.m_AosAttributes.BonusInt;

                if (strBonus != 0 || dexBonus != 0 || intBonus != 0)
                {
                    string modName = this.Serial.ToString();

                    if (strBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                    if (dexBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                    if (intBonus != 0)
                        from.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
                }

                from.CheckStatTimers();

                #region Mondain's Legacy Sets
                if (this.IsSetItem)
                {
                    this.m_SetEquipped = SetHelper.FullSetEquipped(from, this.SetID, this.Pieces);

                    if (this.m_SetEquipped)
                    {
                        this.m_LastEquipped = true;
                        SetHelper.AddSetBonus(from, this.SetID);
                    }
                }
                #endregion
            }

            if (parent is Mobile)
            {
                if (Server.Engines.XmlSpawner2.XmlAttach.CheckCanEquip(this, (Mobile)parent))
                    Server.Engines.XmlSpawner2.XmlAttach.CheckOnEquip(this, (Mobile)parent);
                else
                    ((Mobile)parent).AddToBackpack(this);
            }
        }

        public override void OnRemoved(object parent)
        {
            if (Core.AOS && parent is Mobile)
            {
                Mobile from = (Mobile)parent;

                this.m_AosSkillBonuses.Remove();

                string modName = this.Serial.ToString();

                from.RemoveStatMod(modName + "Str");
                from.RemoveStatMod(modName + "Dex");
                from.RemoveStatMod(modName + "Int");

                from.CheckStatTimers();

                #region Mondain's Legacy Sets
                if (this.IsSetItem && this.m_SetEquipped)
                    SetHelper.RemoveSetBonus(from, this.SetID, this);
                #endregion
            }

            Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);
        }

        public BaseJewel(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            #region Imbuing
            if (this.m_TimesImbued > 0)
                list.Add(1080418); // (Imbued)
            #endregion

            #region Mondain's Legacy
            if (this.m_Quality == ArmorQuality.Exceptional)
                list.Add(1063341); // exceptional

            if (this.m_Crafter != null)
                list.Add(1050043, this.m_Crafter.Name); // crafted by ~1_NAME~
            #endregion

            #region Mondain's Legacy Sets
            if (this.IsSetItem)
            {
                list.Add(1080240, this.Pieces.ToString()); // Part of a Jewelry Set (~1_val~ pieces)

                if (this.m_SetEquipped)
                {
                    list.Add(1080241); // Full Jewelry Set Present					
                    SetHelper.GetSetProperties(list, this);
                }
            }
            #endregion

            this.m_AosSkillBonuses.GetProperties(list);

            int prop;

            #region Stygian Abyss
            if (this.RequiredRace == Race.Elf)
                list.Add(1075086); // Elves Only
            else if (this.RequiredRace == Race.Gargoyle)
                list.Add(1111709); // Gargoyles Only
            #endregion

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

            if ((prop = this.m_AosAttributes.Luck) != 0)
                list.Add(1060436, prop.ToString()); // luck ~1_val~

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

            Server.Engines.XmlSpawner2.XmlAttach.AddAttachmentProperties(this, list);

            if (this.m_HitPoints >= 0 && this.m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", this.m_HitPoints, this.m_MaxHitPoints); // durability ~1_val~ / ~2_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(4); // version

            // Version 4
            writer.WriteEncodedInt((int)this.m_TimesImbued);
            this.m_SAAbsorptionAttributes.Serialize(writer);
            writer.Write((Mobile)this.m_BlessedBy);
            writer.Write((bool)this.m_LastEquipped);
            writer.Write((bool)this.m_SetEquipped);
            writer.WriteEncodedInt((int)this.m_SetHue);

            this.m_SetAttributes.Serialize(writer);
            this.m_SetSkillBonuses.Serialize(writer);

            writer.Write(this.m_Crafter);
            writer.Write((int)this.m_Quality);

            // Version 3
            writer.WriteEncodedInt((int)this.m_MaxHitPoints);
            writer.WriteEncodedInt((int)this.m_HitPoints);

            writer.WriteEncodedInt((int)this.m_Resource);
            writer.WriteEncodedInt((int)this.m_GemType);

            this.m_AosAttributes.Serialize(writer);
            this.m_AosResistances.Serialize(writer);
            this.m_AosSkillBonuses.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    {
                        this.m_TimesImbued = reader.ReadEncodedInt();
                        this.m_SAAbsorptionAttributes = new SAAbsorptionAttributes(this, reader);

                        this.m_BlessedBy = reader.ReadMobile();
                        this.m_LastEquipped = reader.ReadBool();
                        this.m_SetEquipped = reader.ReadBool();
                        this.m_SetHue = reader.ReadEncodedInt();

                        this.m_SetAttributes = new AosAttributes(this, reader);
                        this.m_SetSkillBonuses = new AosSkillBonuses(this, reader);

                        this.m_Crafter = reader.ReadMobile();
                        this.m_Quality = (ArmorQuality)reader.ReadInt();
                        goto case 3;
                    }
                case 3:
                    {
                        this.m_MaxHitPoints = reader.ReadEncodedInt();
                        this.m_HitPoints = reader.ReadEncodedInt();

                        goto case 2;
                    }
                case 2:
                    {
                        this.m_Resource = (CraftResource)reader.ReadEncodedInt();
                        this.m_GemType = (GemType)reader.ReadEncodedInt();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_AosAttributes = new AosAttributes(this, reader);
                        this.m_AosResistances = new AosElementAttributes(this, reader);
                        this.m_AosSkillBonuses = new AosSkillBonuses(this, reader);

                        if (Core.AOS && this.Parent is Mobile)
                            this.m_AosSkillBonuses.AddTo((Mobile)this.Parent);

                        int strBonus = this.m_AosAttributes.BonusStr;
                        int dexBonus = this.m_AosAttributes.BonusDex;
                        int intBonus = this.m_AosAttributes.BonusInt;

                        if (this.Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0))
                        {
                            Mobile m = (Mobile)this.Parent;

                            string modName = this.Serial.ToString();

                            if (strBonus != 0)
                                m.AddStatMod(new StatMod(StatType.Str, modName + "Str", strBonus, TimeSpan.Zero));

                            if (dexBonus != 0)
                                m.AddStatMod(new StatMod(StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero));

                            if (intBonus != 0)
                                m.AddStatMod(new StatMod(StatType.Int, modName + "Int", intBonus, TimeSpan.Zero));
                        }

                        if (this.Parent is Mobile)
                            ((Mobile)this.Parent).CheckStatTimers();

                        break;
                    }
                case 0:
                    {
                        this.m_AosAttributes = new AosAttributes(this);
                        this.m_AosResistances = new AosElementAttributes(this);
                        this.m_AosSkillBonuses = new AosSkillBonuses(this);

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
            {
                this.m_Resource = CraftResource.Iron;
                this.m_GemType = GemType.None;
            }
        }

        #region ICraftable Members

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            if (!craftItem.ForceNonExceptional)
                this.Resource = CraftResources.GetFromType(resourceType);

            CraftContext context = craftSystem.GetContext(from);

            if (context != null && context.DoNotColor)
                this.Hue = 0;

            if (1 < craftItem.Resources.Count)
            {
                resourceType = craftItem.Resources.GetAt(1).ItemType;

                if (resourceType == typeof(StarSapphire))
                    this.GemType = GemType.StarSapphire;
                else if (resourceType == typeof(Emerald))
                    this.GemType = GemType.Emerald;
                else if (resourceType == typeof(Sapphire))
                    this.GemType = GemType.Sapphire;
                else if (resourceType == typeof(Ruby))
                    this.GemType = GemType.Ruby;
                else if (resourceType == typeof(Citrine))
                    this.GemType = GemType.Citrine;
                else if (resourceType == typeof(Amethyst))
                    this.GemType = GemType.Amethyst;
                else if (resourceType == typeof(Tourmaline))
                    this.GemType = GemType.Tourmaline;
                else if (resourceType == typeof(Amber))
                    this.GemType = GemType.Amber;
                else if (resourceType == typeof(Diamond))
                    this.GemType = GemType.Diamond;
            }

            #region Mondain's Legacy
            this.m_Quality = (ArmorQuality)quality;

            if (makersMark)
                this.m_Crafter = from;
            #endregion

            return 1;
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
    }
}