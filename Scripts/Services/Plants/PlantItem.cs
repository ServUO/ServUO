using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Plants
{
    public enum PlantStatus
    {
        BowlOfDirt = 0,
        Seed = 1,
        Sapling = 2,
        Plant = 4,
        FullGrownPlant = 7,
        DecorativePlant = 10,
        DeadTwigs = 11,

        Stage1 = 1,
        Stage2 = 2,
        Stage3 = 3,
        Stage4 = 4,
        Stage5 = 5,
        Stage6 = 6,
        Stage7 = 7,
        Stage8 = 8,
        Stage9 = 9
    }

    public class PlantItem : Item, ISecurable
    {
        /*
		 * Clients 7.0.12.0+ expect a container type in the plant label.
		 * To support older (and only older) clients, change this to false.
		 */
        private static readonly bool ShowContainerType = true;

        private PlantSystem m_PlantSystem;

        private PlantStatus m_PlantStatus;
        private PlantType m_PlantType;
        private PlantHue m_PlantHue;
        private bool m_ShowType;

        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public PlantSystem PlantSystem => m_PlantSystem;

        public override bool ForceShowProperties => true;

        #region Magincia/Raised Garden Plant Support
        public virtual bool RequiresUpkeep => true;
        public virtual bool MaginciaPlant => false;
        public virtual int BowlOfDirtID => 0x1602;
        public virtual int GreenBowlID => 0x1600;

        public virtual int ContainerLocalization => 1150435;
        public virtual int OnPlantLocalization => 1061922;
        public virtual int CantUseLocalization => 1061921;
        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantStatus PlantStatus
        {
            get { return m_PlantStatus; }
            set
            {
                if (m_PlantStatus == value || value < PlantStatus.BowlOfDirt || value > PlantStatus.DeadTwigs)
                    return;

                double ratio;
                if (m_PlantSystem != null)
                    ratio = (double)m_PlantSystem.Hits / m_PlantSystem.MaxHits;
                else
                    ratio = 1.0;

                m_PlantStatus = value;

                if (m_PlantStatus >= PlantStatus.DecorativePlant)
                {
                    m_PlantSystem = null;
                }
                else
                {
                    if (m_PlantSystem == null)
                        m_PlantSystem = new PlantSystem(this, false);

                    int hits = (int)(m_PlantSystem.MaxHits * ratio);

                    if (hits == 0 && m_PlantStatus > PlantStatus.BowlOfDirt)
                        m_PlantSystem.Hits = hits + 1;
                    else
                        m_PlantSystem.Hits = hits;
                }

                Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantType PlantType
        {
            get { return m_PlantType; }
            set
            {
                m_PlantType = value;
                Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantHue PlantHue
        {
            get { return m_PlantHue; }
            set
            {
                m_PlantHue = value;
                Update();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowType
        {
            get { return m_ShowType; }
            set
            {
                m_ShowType = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool ValidGrowthLocation
        {
            get
            {
                if (IsLockedDown && RootParent == null)
                    return true;


                Mobile owner = RootParent as Mobile;
                if (owner == null)
                    return false;

                if (owner.Backpack != null && IsChildOf(owner.Backpack))
                    return true;

                BankBox bank = owner.FindBankNoCreate();
                if (bank != null && IsChildOf(bank))
                    return true;

                return false;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsGrowable => m_PlantStatus >= PlantStatus.BowlOfDirt && m_PlantStatus <= PlantStatus.Stage9;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsCrossable => PlantHueInfo.IsCrossable(PlantHue) && PlantTypeInfo.IsCrossable(PlantType);

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Reproduces => PlantHueInfo.CanReproduce(PlantHue) && PlantTypeInfo.CanReproduce(PlantType);

        private static readonly ArrayList m_Instances = new ArrayList();

        public static ArrayList Plants => m_Instances;

        [Constructable]
        public PlantItem() : this(0x1602, false)
        {
        }

        [Constructable]
        public PlantItem(int itemID) : this(itemID, false)
        {
        }

        [Constructable]
        public PlantItem(bool fertileDirt) : this(0x1602, fertileDirt)
        {
        }

        [Constructable]
        public PlantItem(int itemID, bool fertileDirt) : base(itemID)
        {
            Weight = 1.0;

            m_PlantStatus = PlantStatus.BowlOfDirt;
            m_PlantSystem = new PlantSystem(this, fertileDirt);
            m_Level = SecureLevel.Owner;

            m_Instances.Add(this);
        }

        public PlantItem(Serial serial) : base(serial)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (m_PlantStatus != PlantStatus.DecorativePlant)
            {
                SetSecureLevelEntry.AddTo(from, this, list);
            }
        }

        public int GetLocalizedPlantStatus()
        {
            if (m_PlantStatus >= PlantStatus.Plant)
                return 1060812; // plant
            else if (m_PlantStatus >= PlantStatus.Sapling)
                return 1023305; // sapling
            else if (m_PlantStatus >= PlantStatus.Seed)
                return 1060810; // seed
            else
                return 1026951; // dirt
        }

        public int GetLocalizedContainerType()
        {
            return ContainerLocalization; // mound -or- bowl
        }

        private void Update()
        {
            if (m_PlantStatus >= PlantStatus.DeadTwigs)
            {
                ItemID = 0x1B9D;
                Hue = PlantHueInfo.GetInfo(m_PlantHue).Hue;
            }
            else if (m_PlantStatus >= PlantStatus.FullGrownPlant)
            {
                ItemID = PlantTypeInfo.GetInfo(m_PlantType).ItemID;
                Hue = PlantHueInfo.GetInfo(m_PlantHue).Hue;
            }
            else if (m_PlantStatus >= PlantStatus.Plant)
            {
                ItemID = GreenBowlID;
                Hue = 0;
            }
            else
            {
                ItemID = BowlOfDirtID;
                Hue = 0;
            }

            InvalidateProperties();
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (m_PlantStatus >= PlantStatus.DeadTwigs)
            {
                base.AddNameProperty(list);
            }
            else if (m_PlantStatus < PlantStatus.Seed)
            {
                string args;

                if (ShowContainerType)
                    args = string.Format("#{0}\t#{1}", GetLocalizedContainerType(), m_PlantSystem.GetLocalizedDirtStatus());
                else
                    args = string.Format("#{0}", m_PlantSystem.GetLocalizedDirtStatus());

                list.Add(1060830, args); // a ~1_val~ of ~2_val~ dirt
            }
            else
            {
                PlantTypeInfo typeInfo = PlantTypeInfo.GetInfo(m_PlantType);
                PlantHueInfo hueInfo = PlantHueInfo.GetInfo(m_PlantHue);

                if (m_PlantStatus >= PlantStatus.DecorativePlant)
                {
                    list.Add(typeInfo.GetPlantLabelDecorative(hueInfo), string.Format("#{0}\t#{1}", hueInfo.Name, typeInfo.Name));
                }
                else if (m_PlantStatus >= PlantStatus.FullGrownPlant)
                {
                    list.Add(typeInfo.GetPlantLabelFullGrown(hueInfo), string.Format("#{0}\t#{1}\t#{2}", m_PlantSystem.GetLocalizedHealth(), hueInfo.Name, typeInfo.Name));
                }
                else
                {
                    string args;

                    if (ShowContainerType)
                        args = string.Format("#{0}\t#{1}\t#{2}", GetLocalizedContainerType(), m_PlantSystem.GetLocalizedDirtStatus(), m_PlantSystem.GetLocalizedHealth());
                    else
                        args = string.Format("#{0}\t#{1}", m_PlantSystem.GetLocalizedDirtStatus(), m_PlantSystem.GetLocalizedHealth());

                    if (m_ShowType)
                    {
                        args += string.Format("\t#{0}\t#{1}\t#{2}", hueInfo.Name, typeInfo.Name, GetLocalizedPlantStatus());

                        if (m_PlantStatus == PlantStatus.Plant)
                            list.Add(typeInfo.GetPlantLabelPlant(hueInfo), args);
                        else
                            list.Add(typeInfo.GetPlantLabelSeed(hueInfo), args);
                    }
                    else
                    {
                        args += string.Format("\t#{0}\t#{1}", (typeInfo.PlantCategory == PlantCategory.Default) ? hueInfo.Name : (int)typeInfo.PlantCategory, GetLocalizedPlantStatus());

                        list.Add(hueInfo.IsBright() ? 1060832 : 1060831, args); // a ~1_val~ of ~2_val~ dirt with a ~3_val~ [bright] ~4_val~ ~5_val~
                    }
                }
            }
        }

        public virtual bool IsUsableBy(Mobile from)
        {
            Item root = RootParent as Item;
            return IsChildOf(from.Backpack) || IsChildOf(from.FindBankNoCreate()) || IsLockedDown && IsAccessibleTo(from) || root != null && root.IsSecure && root.IsAccessibleTo(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_PlantStatus >= PlantStatus.DecorativePlant)
                return;

            Point3D loc = GetWorldLocation();

            if (!from.InLOS(loc) || !from.InRange(loc, 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that.
                return;
            }

            if (!IsUsableBy(from))
            {
                LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                return;
            }

            from.SendGump(new MainPlantGump(this));
        }

        public virtual bool PlantSeed(Mobile from, Seed seed)
        {
            if (m_PlantStatus >= PlantStatus.FullGrownPlant)
            {
                LabelTo(from, 1061919); // You must use a seed on some prepared soil!
            }
            else if (!IsUsableBy(from))
            {
                LabelTo(from, CantUseLocalization); // The bowl of dirt must be in your pack, or you must lock it down.
            }
            else if (m_PlantStatus != PlantStatus.BowlOfDirt)
            {
                if (RequiresUpkeep && !MaginciaPlant)
                    from.SendLocalizedMessage(1080389, "#" + GetLocalizedPlantStatus().ToString()); // This bowl of dirt already has a ~1_val~ in it!
                else
                    from.SendLocalizedMessage(1150441); // This mound of dirt already has a seed in it!
            }
            else if (RequiresUpkeep && m_PlantSystem.Water < 2)
            {
                LabelTo(from, 1061920); // The dirt needs to be softened first.
            }
            else
            {
                m_PlantType = seed.PlantType;
                m_PlantHue = seed.PlantHue;
                m_ShowType = seed.ShowType;

                seed.Consume();

                PlantStatus = PlantStatus.Seed;

                m_PlantSystem.Reset(false);

                LabelTo(from, OnPlantLocalization); // You plant the seed in the bowl of dirt.
                return true;
            }

            return false;
        }

        public virtual void Die()
        {
            if (m_PlantStatus >= PlantStatus.FullGrownPlant)
            {
                PlantStatus = PlantStatus.DeadTwigs;
            }
            else
            {
                PlantStatus = PlantStatus.BowlOfDirt;
                m_PlantSystem.Reset(true);
            }
        }

        public void Pour(Mobile from, Item item)
        {
            if (m_PlantStatus >= PlantStatus.DeadTwigs)
                return;

            if (m_PlantStatus == PlantStatus.DecorativePlant)
            {
                LabelTo(from, 1053049); // This is a decorative plant, it does not need watering!
                return;
            }

            if (!RequiresUpkeep)
            {
                LabelTo(from, 1150619); // You don't need to water it.
                return;
            }

            if (!IsUsableBy(from))
            {
                LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                return;
            }

            if (item is BaseBeverage)
            {
                BaseBeverage beverage = (BaseBeverage)item;

                if (beverage.IsEmpty || !beverage.Pourable || beverage.Content != BeverageType.Water)
                {
                    LabelTo(from, 1053069); // You can't use that on a plant!
                    return;
                }

                if (!beverage.ValidateUse(from, true))
                    return;

                beverage.Quantity--;
                m_PlantSystem.Water++;

                from.PlaySound(0x4E);
                LabelTo(from, 1061858); // You soften the dirt with water.

                m_PlantSystem.NextGrowth = DateTime.UtcNow + PlantSystem.CheckDelay;
            }
            else if (item is BasePotion)
            {
                BasePotion potion = (BasePotion)item;

                int message;
                if (ApplyPotion(potion.PotionEffect, false, out message))
                {
                    potion.Consume();
                    from.PlaySound(0x240);
                    from.AddToBackpack(new Bottle());

                    m_PlantSystem.NextGrowth = DateTime.UtcNow + PlantSystem.CheckDelay;
                }
                LabelTo(from, message);
            }
            else if (item is PotionKeg)
            {
                PotionKeg keg = (PotionKeg)item;

                if (keg.Held <= 0)
                {
                    LabelTo(from, 1053069); // You can't use that on a plant!
                    return;
                }

                int message;
                if (ApplyPotion(keg.Type, false, out message))
                {
                    keg.Held--;
                    from.PlaySound(0x240);

                    m_PlantSystem.NextGrowth = DateTime.UtcNow + PlantSystem.CheckDelay;
                }
                LabelTo(from, message);
            }
            else
            {
                LabelTo(from, 1053069); // You can't use that on a plant!
            }
        }

        public bool ApplyPotion(PotionEffect effect, bool testOnly, out int message)
        {
            if (m_PlantStatus >= PlantStatus.DecorativePlant)
            {
                message = 1053049; // This is a decorative plant, it does not need watering!
                return false;
            }

            if (!RequiresUpkeep)
            {
                message = 1150619; // You don't need to water it.
                return false;
            }

            if (m_PlantStatus == PlantStatus.BowlOfDirt)
            {
                message = 1053066; // You should only pour potions on a plant or seed!
                return false;
            }

            bool full = false;

            if (effect == PotionEffect.PoisonGreater || effect == PotionEffect.PoisonDeadly)
            {
                if (m_PlantSystem.IsFullPoisonPotion)
                    full = true;
                else if (!testOnly)
                    m_PlantSystem.PoisonPotion++;
            }
            else if (effect == PotionEffect.CureGreater)
            {
                if (m_PlantSystem.IsFullCurePotion)
                    full = true;
                else if (!testOnly)
                    m_PlantSystem.CurePotion++;
            }
            else if (effect == PotionEffect.HealGreater)
            {
                if (m_PlantSystem.IsFullHealPotion)
                    full = true;
                else if (!testOnly)
                    m_PlantSystem.HealPotion++;
            }
            else if (effect == PotionEffect.StrengthGreater)
            {
                if (m_PlantSystem.IsFullStrengthPotion)
                    full = true;
                else if (!testOnly)
                    m_PlantSystem.StrengthPotion++;
            }
            else if (effect == PotionEffect.PoisonLesser || effect == PotionEffect.Poison || effect == PotionEffect.CureLesser || effect == PotionEffect.Cure ||
                effect == PotionEffect.HealLesser || effect == PotionEffect.Heal || effect == PotionEffect.Strength)
            {
                message = 1053068; // This potion is not powerful enough to use on a plant!
                return false;
            }
            else
            {
                message = 1053069; // You can't use that on a plant!
                return false;
            }

            if (full)
            {
                message = 1053065; // The plant is already soaked with this type of potion!
                return false;
            }
            else
            {
                message = 1053067; // You pour the potion over the plant.
                return true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            writer.Write((int)m_Level);

            writer.Write((int)m_PlantStatus);
            writer.Write((int)m_PlantType);
            writer.Write((int)m_PlantHue);
            writer.Write(m_ShowType);

            if (m_PlantStatus < PlantStatus.DecorativePlant)
                m_PlantSystem.Save(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                    {
                        m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            m_Level = SecureLevel.CoOwners;

                        m_PlantStatus = (PlantStatus)reader.ReadInt();
                        m_PlantType = (PlantType)reader.ReadInt();
                        m_PlantHue = (PlantHue)reader.ReadInt();
                        m_ShowType = reader.ReadBool();

                        if (m_PlantStatus < PlantStatus.DecorativePlant)
                            m_PlantSystem = new PlantSystem(this, reader);

                        if (version < 2 && PlantHueInfo.IsCrossable(m_PlantHue))
                            m_PlantHue |= PlantHue.Reproduces;

                        break;
                    }
            }

            m_Instances.Add(this);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            m_Instances.Remove(this);
        }
    }
}
