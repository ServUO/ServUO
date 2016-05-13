using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;

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
        private static readonly ArrayList m_Instances = new ArrayList();
        private PlantSystem m_PlantSystem;
        private PlantStatus m_PlantStatus;
        private PlantType m_PlantType;
        private PlantHue m_PlantHue;
        private bool m_ShowType;
        private SecureLevel m_Level;
        [Constructable]
        public PlantItem()
            : this(false)
        {
        }

        [Constructable]
        public PlantItem(bool fertileDirt)
            : base(0x1602)
        {
            this.Weight = 1.0;

            this.m_PlantStatus = PlantStatus.BowlOfDirt;
            this.m_PlantSystem = new PlantSystem(this, fertileDirt);
            this.m_Level = SecureLevel.Owner;

            m_Instances.Add(this);
        }

        public PlantItem(Serial serial)
            : base(serial)
        {
        }

        public static ArrayList Plants
        {
            get
            {
                return m_Instances;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        public PlantSystem PlantSystem
        {
            get
            {
                return this.m_PlantSystem;
            }
        }
        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override int LabelNumber
        {
            get
            {
                if (this.m_PlantStatus >= PlantStatus.DeadTwigs)
                    return base.LabelNumber;
                else if (this.m_PlantStatus >= PlantStatus.DecorativePlant)
                    return 1061924; // a decorative plant
                else if (this.m_PlantStatus >= PlantStatus.FullGrownPlant)
                    return PlantTypeInfo.GetInfo(this.m_PlantType).Name;
                else
                    return 1029913; // plant bowl
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PlantStatus PlantStatus
        {
            get
            {
                return this.m_PlantStatus;
            }
            set
            {
                if (this.m_PlantStatus == value || value < PlantStatus.BowlOfDirt || value > PlantStatus.DeadTwigs)
                    return;

                double ratio;
                if (this.m_PlantSystem != null)
                    ratio = (double)this.m_PlantSystem.Hits / this.m_PlantSystem.MaxHits;
                else
                    ratio = 1.0;

                this.m_PlantStatus = value;

                if (this.m_PlantStatus >= PlantStatus.DecorativePlant)
                {
                    this.m_PlantSystem = null;
                }
                else
                {
                    if (this.m_PlantSystem == null)
                        this.m_PlantSystem = new PlantSystem(this, false);

                    int hits = (int)(this.m_PlantSystem.MaxHits * ratio);

                    if (hits == 0 && this.m_PlantStatus > PlantStatus.BowlOfDirt)
                        this.m_PlantSystem.Hits = hits + 1;
                    else
                        this.m_PlantSystem.Hits = hits;
                }

                this.Update();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PlantType PlantType
        {
            get
            {
                return this.m_PlantType;
            }
            set
            {
                this.m_PlantType = value;
                this.Update();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PlantHue PlantHue
        {
            get
            {
                return this.m_PlantHue;
            }
            set
            {
                this.m_PlantHue = value;
                this.Update();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowType
        {
            get
            {
                return this.m_ShowType;
            }
            set
            {
                this.m_ShowType = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ValidGrowthLocation
        {
            get
            {
                if (this.IsLockedDown && this.RootParent == null)
                    return true;

                Mobile owner = this.RootParent as Mobile;
                if (owner == null)
                    return false;

                if (owner.Backpack != null && this.IsChildOf(owner.Backpack))
                    return true;

                BankBox bank = owner.FindBankNoCreate();
                if (bank != null && this.IsChildOf(bank))
                    return true;

                return false;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsGrowable
        {
            get
            {
                return this.m_PlantStatus >= PlantStatus.BowlOfDirt && this.m_PlantStatus <= PlantStatus.Stage9;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsCrossable
        {
            get
            {
                return PlantHueInfo.IsCrossable(this.PlantHue) && PlantTypeInfo.IsCrossable(this.PlantType);
            }
        }
        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public int GetLocalizedPlantStatus()
        {
            if (this.m_PlantStatus >= PlantStatus.Plant)
                return 1060812; // plant
            else if (this.m_PlantStatus >= PlantStatus.Sapling)
                return 1023305; // sapling
            else if (this.m_PlantStatus >= PlantStatus.Seed)
                return 1060810; // seed
            else
                return 1026951; // dirt
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.m_PlantStatus >= PlantStatus.DeadTwigs)
            {
                base.AddNameProperty(list);
            }
            else if (this.m_PlantStatus >= PlantStatus.FullGrownPlant)
            {
                PlantHueInfo hueInfo = PlantHueInfo.GetInfo(this.m_PlantHue);
                PlantTypeInfo typeInfo = PlantTypeInfo.GetInfo(this.m_PlantType);

                int title = PlantTypeInfo.GetBonsaiTitle(this.m_PlantType);
                if (title == 0) // Not a bonsai
                    title = hueInfo.Name;
                if (this.m_PlantType == PlantType.CocoaTree)
                    title = 1080529;

                if (this.m_PlantStatus < PlantStatus.DecorativePlant)
                {
                    if (this.m_PlantType == PlantType.SugarCanes)
                    {
                        string args = string.Format("#{0}", this.m_PlantSystem.GetLocalizedHealth());
                        list.Add(1094702, args); // ~1_HEALTH~ Sugar Canes
                    }
                    else if (this.m_PlantType == PlantType.CocoaTree)
                    {
                        string args = string.Format("#{0}", this.m_PlantSystem.GetLocalizedHealth());
                        list.Add(1080534, args); // a ~1_HEALTH~ cocoa tree
                    }
                    else
                    {
                        string args = string.Format("#{0}\t#{1}\t#{2}", this.m_PlantSystem.GetLocalizedHealth(), title, typeInfo.Name);

                        if (typeInfo.ContainsPlant)
                        {
                            // a ~1_HEALTH~ [bright] ~2_COLOR~ ~3_NAME~
                            list.Add(hueInfo.IsBright() ? 1061891 : 1061889, args);
                        }
                        else
                        {
                            // a ~1_HEALTH~ [bright] ~2_COLOR~ ~3_NAME~ plant
                            list.Add(hueInfo.IsBright() ? 1061892 : 1061890, args);
                        }
                    }
                }
                else
                {
                    if (this.m_PlantType == PlantType.SugarCanes)
                        list.Add(1094703); // Decorative Sugar Canes
                    else if (this.m_PlantType == PlantType.CocoaTree)
                        list.Add(1080531); // a decorative cocoa tree
                    else if (title == 1080528)
                    // a decorative ~2_TYPE~
                        list.Add(1080539, string.Format("#{0}\t#{1}", title, typeInfo.Name));
                    else
                    // a decorative [bright] ~1_COLOR~ ~2_TYPE~
                        list.Add(hueInfo.IsBright() ? 1074267 : 1070973, string.Format("#{0}\t#{1}", title, typeInfo.Name));
                }
            }
            else if (this.m_PlantStatus >= PlantStatus.Seed)
            {
                PlantHueInfo hueInfo = PlantHueInfo.GetInfo(this.m_PlantHue);

                int title = PlantTypeInfo.GetBonsaiTitle(this.m_PlantType);
                if (title == 0) // Not a bonsai
                    title = hueInfo.Name;
                if (this.m_PlantType == PlantType.CocoaTree)
                    title = 1080532; // cocoa tree

                string args = string.Format("#{0}\t#{1}\t#{2}", this.m_PlantSystem.GetLocalizedDirtStatus(), this.m_PlantSystem.GetLocalizedHealth(), title);

                // 7.0.12.0 cliloc change
                args = String.Concat("#1150435\t", args); // TODO: Change to plant container type when implemented

                if (this.m_ShowType)
                {
                    PlantTypeInfo typeInfo = PlantTypeInfo.GetInfo(this.m_PlantType);
                    args += "\t#" + typeInfo.Name.ToString();

                    if (typeInfo.ContainsPlant && this.m_PlantStatus == PlantStatus.Plant)
                    {
                        // a ~1_val~ of ~2_val~ dirt with a ~3_val~ [bright] ~4_val~ ~5_val~
                        list.Add(hueInfo.IsBright() ? 1060832 : 1060831, args);
                    }
                    else if (this.m_PlantType == PlantType.CocoaTree)
                    {
                        if (this.m_PlantStatus == PlantStatus.Seed)
                        {
                            // a ~1_val~ of ~2_val~ dirt with a ~3_val~ cocoa tree seed
                            list.Add(1080536, args);
                        }
                        else
                        {
                            // a ~1_val~ of ~2_val~ dirt with a ~3_val~ ~4_val~ ~5_val~
                            list.Add(1080535, args + "\t#" + this.GetLocalizedPlantStatus().ToString());
                        }
                    }
                    else
                    {
                        // a ~1_val~ of ~2_val~ dirt with a ~3_val~ [bright] ~4_val~ ~5_val~ ~6_val~
                        list.Add(hueInfo.IsBright() ? 1061887 : 1061888, args + "\t#" + this.GetLocalizedPlantStatus().ToString());
                    }
                }
                else
                {
                    // a ~1_val~ of ~2_val~ dirt with a ~3_val~ [bright] ~4_val~ ~5_val~
                    list.Add(hueInfo.IsBright() ? 1060832 : 1060831, args + "\t#" + this.GetLocalizedPlantStatus().ToString());
                }
            }
            else
            {
                string args = "#" + this.m_PlantSystem.GetLocalizedDirtStatus();

                // 7.0.12.0 cliloc change
                args = String.Concat("#1150435\t", args); // TODO: Change to plant container type when implemented

                list.Add(1060830, args); // a ~1_val~ of ~2_val~ dirt
            }
        }

        public bool IsUsableBy(Mobile from)
        {
            Item root = this.RootParent as Item;
            return this.IsChildOf(from.Backpack) || this.IsChildOf(from.FindBankNoCreate()) || this.IsLockedDown && this.IsAccessibleTo(from) || root != null && root.IsSecure && root.IsAccessibleTo(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_PlantStatus >= PlantStatus.DecorativePlant)
                return;
			
            Point3D loc = this.GetWorldLocation();	
			
            if (!from.InLOS(loc) || !from.InRange(loc, 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3E9, 1019045); // I can't reach that.
                return;
            }

            if (!this.IsUsableBy(from))
            {
                this.LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                return;
            }

            from.SendGump(new MainPlantGump(this));
        }

        public void PlantSeed(Mobile from, Seed seed)
        {
            if (this.m_PlantStatus >= PlantStatus.FullGrownPlant)
            {
                this.LabelTo(from, 1061919); // You must use a seed on some prepared soil!
            }
            else if (!this.IsUsableBy(from))
            {
                this.LabelTo(from, 1061921); // The bowl of dirt must be in your pack, or you must lock it down.
            }
            else if (this.m_PlantStatus != PlantStatus.BowlOfDirt)
            {
                from.SendLocalizedMessage(1080389, "#" + this.GetLocalizedPlantStatus().ToString()); // This bowl of dirt already has a ~1_val~ in it!
            }
            else if (this.m_PlantSystem.Water < 2)
            {
                this.LabelTo(from, 1061920); // The dirt needs to be softened first.
            }
            else
            {
                this.m_PlantType = seed.PlantType;
                this.m_PlantHue = seed.PlantHue;
                this.m_ShowType = seed.ShowType;

                seed.Consume();

                this.PlantStatus = PlantStatus.Seed;

                this.m_PlantSystem.Reset(false);

                this.LabelTo(from, 1061922); // You plant the seed in the bowl of dirt.
            }
        }

        public void Die()
        {
            if (this.m_PlantStatus >= PlantStatus.FullGrownPlant)
            {
                this.PlantStatus = PlantStatus.DeadTwigs;
            }
            else
            {
                this.PlantStatus = PlantStatus.BowlOfDirt;
                this.m_PlantSystem.Reset(true);
            }
        }

        public void Pour(Mobile from, Item item)
        {
            if (this.m_PlantStatus >= PlantStatus.DeadTwigs)
                return;

            if (this.m_PlantStatus == PlantStatus.DecorativePlant)
            {
                this.LabelTo(from, 1053049); // This is a decorative plant, it does not need watering!
                return;
            }

            if (!this.IsUsableBy(from))
            {
                this.LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                return;
            }

            if (item is BaseBeverage)
            {
                BaseBeverage beverage = (BaseBeverage)item;

                if (beverage.IsEmpty || !beverage.Pourable || beverage.Content != BeverageType.Water)
                {
                    this.LabelTo(from, 1053069); // You can't use that on a plant!
                    return;
                }

                if (!beverage.ValidateUse(from, true))
                    return;

                beverage.Quantity--;
                this.m_PlantSystem.Water++;

                from.PlaySound(0x4E);
                this.LabelTo(from, 1061858); // You soften the dirt with water.
            }
            else if (item is BasePotion)
            {
                BasePotion potion = (BasePotion)item;

                int message;
                if (this.ApplyPotion(potion.PotionEffect, false, out message))
                {
                    potion.Consume();
                    from.PlaySound(0x240);
                    from.AddToBackpack(new Bottle());
                }
                this.LabelTo(from, message);
            }
            else if (item is PotionKeg)
            {
                PotionKeg keg = (PotionKeg)item;

                if (keg.Held <= 0)
                {
                    this.LabelTo(from, 1053069); // You can't use that on a plant!
                    return;
                }

                int message;
                if (this.ApplyPotion(keg.Type, false, out message))
                {
                    keg.Held--;
                    from.PlaySound(0x240);
                }
                this.LabelTo(from, message);
            }
            else
            {
                this.LabelTo(from, 1053069); // You can't use that on a plant!
            }
        }

        public bool ApplyPotion(PotionEffect effect, bool testOnly, out int message)
        {
            if (this.m_PlantStatus >= PlantStatus.DecorativePlant)
            {
                message = 1053049; // This is a decorative plant, it does not need watering!
                return false;
            }

            if (this.m_PlantStatus == PlantStatus.BowlOfDirt)
            {
                message = 1053066; // You should only pour potions on a plant or seed!
                return false;
            }

            bool full = false;

            if (effect == PotionEffect.PoisonGreater || effect == PotionEffect.PoisonDeadly)
            {
                if (this.m_PlantSystem.IsFullPoisonPotion)
                    full = true;
                else if (!testOnly)
                    this.m_PlantSystem.PoisonPotion++;
            }
            else if (effect == PotionEffect.CureGreater)
            {
                if (this.m_PlantSystem.IsFullCurePotion)
                    full = true;
                else if (!testOnly)
                    this.m_PlantSystem.CurePotion++;
            }
            else if (effect == PotionEffect.HealGreater)
            {
                if (this.m_PlantSystem.IsFullHealPotion)
                    full = true;
                else if (!testOnly)
                    this.m_PlantSystem.HealPotion++;
            }
            else if (effect == PotionEffect.StrengthGreater)
            {
                if (this.m_PlantSystem.IsFullStrengthPotion)
                    full = true;
                else if (!testOnly)
                    this.m_PlantSystem.StrengthPotion++;
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

            writer.Write((int)1); // version

            writer.Write((int)this.m_Level);

            writer.Write((int)this.m_PlantStatus);
            writer.Write((int)this.m_PlantType);
            writer.Write((int)this.m_PlantHue);
            writer.Write((bool)this.m_ShowType);

            if (this.m_PlantStatus < PlantStatus.DecorativePlant)
                this.m_PlantSystem.Save(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Level = (SecureLevel)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            this.m_Level = SecureLevel.CoOwners;

                        this.m_PlantStatus = (PlantStatus)reader.ReadInt();
                        this.m_PlantType = (PlantType)reader.ReadInt();
                        this.m_PlantHue = (PlantHue)reader.ReadInt();
                        this.m_ShowType = reader.ReadBool();

                        if (this.m_PlantStatus < PlantStatus.DecorativePlant)
                            this.m_PlantSystem = new PlantSystem(this, reader);

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

        private void Update()
        {
            if (this.m_PlantStatus >= PlantStatus.DeadTwigs)
            {
                this.ItemID = 0x1B9D;
                this.Hue = PlantHueInfo.GetInfo(this.m_PlantHue).Hue;
            }
            else if (this.m_PlantStatus >= PlantStatus.FullGrownPlant)
            {
                this.ItemID = PlantTypeInfo.GetInfo(this.m_PlantType).ItemID;
                this.Hue = PlantHueInfo.GetInfo(this.m_PlantHue).Hue;
            }
            else if (this.m_PlantStatus >= PlantStatus.Plant)
            {
                this.ItemID = 0x1600;
                this.Hue = 0;
            }
            else
            {
                this.ItemID = 0x1602;
                this.Hue = 0;
            }

            this.InvalidateProperties();
        }
    }
}