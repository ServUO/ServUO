using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.Plants
{
    public enum PlantHealth
    {
        Dying,
        Wilted,
        Healthy,
        Vibrant
    }

    public enum PlantGrowthIndicator
    {
        None,
        InvalidLocation,
        NotHealthy,
        Delay,
        Grown,
        DoubleGrown
    }

    public class PlantSystem
    {
        public static readonly TimeSpan CheckDelay = TimeSpan.FromHours(23.0);
        private readonly PlantItem m_Plant;
        private bool m_FertileDirt;
        private DateTime m_NextGrowth;
        private PlantGrowthIndicator m_GrowthIndicator;
        private int m_Water;
        private int m_Hits;
        private int m_Infestation;
        private int m_Fungus;
        private int m_Poison;
        private int m_Disease;
        private int m_PoisonPotion;
        private int m_CurePotion;
        private int m_HealPotion;
        private int m_StrengthPotion;
        private bool m_Pollinated;
        private PlantType m_SeedType;
        private PlantHue m_SeedHue;
        private int m_AvailableSeeds;
        private int m_LeftSeeds;
        private int m_AvailableResources;
        private int m_LeftResources;
        public PlantSystem(PlantItem plant, bool fertileDirt)
        {
            this.m_Plant = plant;
            this.m_FertileDirt = fertileDirt;

            this.m_NextGrowth = DateTime.UtcNow + CheckDelay;
            this.m_GrowthIndicator = PlantGrowthIndicator.None;
            this.m_Hits = this.MaxHits;
            this.m_LeftSeeds = 8;
            this.m_LeftResources = 8;
        }

        public PlantSystem(PlantItem plant, GenericReader reader)
        {
            this.m_Plant = plant;

            int version = reader.ReadInt();

            this.m_FertileDirt = reader.ReadBool();

            if (version >= 1)
                this.m_NextGrowth = reader.ReadDateTime();
            else
                this.m_NextGrowth = reader.ReadDeltaTime();

            this.m_GrowthIndicator = (PlantGrowthIndicator)reader.ReadInt();

            this.m_Water = reader.ReadInt();

            this.m_Hits = reader.ReadInt();
            this.m_Infestation = reader.ReadInt();
            this.m_Fungus = reader.ReadInt();
            this.m_Poison = reader.ReadInt();
            this.m_Disease = reader.ReadInt();
            this.m_PoisonPotion = reader.ReadInt();
            this.m_CurePotion = reader.ReadInt();
            this.m_HealPotion = reader.ReadInt();
            this.m_StrengthPotion = reader.ReadInt();

            this.m_Pollinated = reader.ReadBool();
            this.m_SeedType = (PlantType)reader.ReadInt();
            this.m_SeedHue = (PlantHue)reader.ReadInt();
            this.m_AvailableSeeds = reader.ReadInt();
            this.m_LeftSeeds = reader.ReadInt();

            this.m_AvailableResources = reader.ReadInt();
            this.m_LeftResources = reader.ReadInt();
        }

        public PlantItem Plant
        {
            get
            {
                return this.m_Plant;
            }
        }
        public bool FertileDirt
        {
            get
            {
                return this.m_FertileDirt;
            }
            set
            {
                this.m_FertileDirt = value;
            }
        }
        public DateTime NextGrowth
        {
            get
            {
                return this.m_NextGrowth;
            }
        }
        public PlantGrowthIndicator GrowthIndicator
        {
            get
            {
                return this.m_GrowthIndicator;
            }
        }
        public bool IsFullWater
        {
            get
            {
                return this.m_Water >= 4;
            }
        }
        public int Water
        {
            get
            {
                return this.m_Water;
            }
            set
            {
                if (value < 0)
                    this.m_Water = 0;
                else if (value > 4)
                    this.m_Water = 4;
                else
                    this.m_Water = value;

                this.m_Plant.InvalidateProperties();
            }
        }
        public int Hits
        {
            get
            {
                return this.m_Hits;
            }
            set
            {
                if (this.m_Hits == value)
                    return;

                if (value < 0)
                    this.m_Hits = 0;
                else if (value > this.MaxHits)
                    this.m_Hits = this.MaxHits;
                else
                    this.m_Hits = value;

                if (this.m_Hits == 0)
                    this.m_Plant.Die();

                this.m_Plant.InvalidateProperties();
            }
        }
        public int MaxHits
        {
            get
            {
                return 10 + (int)this.m_Plant.PlantStatus * 2;
            }
        }
        public PlantHealth Health
        {
            get
            {
                int perc = this.m_Hits * 100 / this.MaxHits;

                if (perc < 33)
                    return PlantHealth.Dying;
                else if (perc < 66)
                    return PlantHealth.Wilted;
                else if (perc < 100)
                    return PlantHealth.Healthy;
                else
                    return PlantHealth.Vibrant;
            }
        }
        public int Infestation
        {
            get
            {
                return this.m_Infestation;
            }
            set
            {
                if (value < 0)
                    this.m_Infestation = 0;
                else if (value > 2)
                    this.m_Infestation = 2;
                else
                    this.m_Infestation = value;
            }
        }
        public int Fungus
        {
            get
            {
                return this.m_Fungus;
            }
            set
            {
                if (value < 0)
                    this.m_Fungus = 0;
                else if (value > 2)
                    this.m_Fungus = 2;
                else
                    this.m_Fungus = value;
            }
        }
        public int Poison
        {
            get
            {
                return this.m_Poison;
            }
            set
            {
                if (value < 0)
                    this.m_Poison = 0;
                else if (value > 2)
                    this.m_Poison = 2;
                else
                    this.m_Poison = value;
            }
        }
        public int Disease
        {
            get
            {
                return this.m_Disease;
            }
            set
            {
                if (value < 0)
                    this.m_Disease = 0;
                else if (value > 2)
                    this.m_Disease = 2;
                else
                    this.m_Disease = value;
            }
        }
        public bool IsFullPoisonPotion
        {
            get
            {
                return this.m_PoisonPotion >= 2;
            }
        }
        public int PoisonPotion
        {
            get
            {
                return this.m_PoisonPotion;
            }
            set
            {
                if (value < 0)
                    this.m_PoisonPotion = 0;
                else if (value > 2)
                    this.m_PoisonPotion = 2;
                else
                    this.m_PoisonPotion = value;
            }
        }
        public bool IsFullCurePotion
        {
            get
            {
                return this.m_CurePotion >= 2;
            }
        }
        public int CurePotion
        {
            get
            {
                return this.m_CurePotion;
            }
            set
            {
                if (value < 0)
                    this.m_CurePotion = 0;
                else if (value > 2)
                    this.m_CurePotion = 2;
                else
                    this.m_CurePotion = value;
            }
        }
        public bool IsFullHealPotion
        {
            get
            {
                return this.m_HealPotion >= 2;
            }
        }
        public int HealPotion
        {
            get
            {
                return this.m_HealPotion;
            }
            set
            {
                if (value < 0)
                    this.m_HealPotion = 0;
                else if (value > 2)
                    this.m_HealPotion = 2;
                else
                    this.m_HealPotion = value;
            }
        }
        public bool IsFullStrengthPotion
        {
            get
            {
                return this.m_StrengthPotion >= 2;
            }
        }
        public int StrengthPotion
        {
            get
            {
                return this.m_StrengthPotion;
            }
            set
            {
                if (value < 0)
                    this.m_StrengthPotion = 0;
                else if (value > 2)
                    this.m_StrengthPotion = 2;
                else
                    this.m_StrengthPotion = value;
            }
        }
        public bool HasMaladies
        {
            get
            {
                return this.Infestation > 0 || this.Fungus > 0 || this.Poison > 0 || this.Disease > 0 || this.Water != 2;
            }
        }
        public bool PollenProducing
        {
            get
            {
                return this.m_Plant.IsCrossable && this.m_Plant.PlantStatus >= PlantStatus.FullGrownPlant;
            }
        }
        public bool Pollinated
        {
            get
            {
                return this.m_Pollinated;
            }
            set
            {
                this.m_Pollinated = value;
            }
        }
        public PlantType SeedType
        {
            get
            {
                if (this.m_Pollinated)
                    return this.m_SeedType;
                else
                    return this.m_Plant.PlantType;
            }
            set
            {
                this.m_SeedType = value;
            }
        }
        public PlantHue SeedHue
        {
            get
            {
                if (this.m_Pollinated)
                    return this.m_SeedHue;
                else
                    return this.m_Plant.PlantHue;
            }
            set
            {
                this.m_SeedHue = value;
            }
        }
        public int AvailableSeeds
        {
            get
            {
                return this.m_AvailableSeeds;
            }
            set
            {
                if (value >= 0)
                    this.m_AvailableSeeds = value;
            }
        }
        public int LeftSeeds
        {
            get
            {
                return this.m_LeftSeeds;
            }
            set
            {
                if (value >= 0)
                    this.m_LeftSeeds = value;
            }
        }
        public int AvailableResources
        {
            get
            {
                return this.m_AvailableResources;
            }
            set
            {
                if (value >= 0)
                    this.m_AvailableResources = value;
            }
        }
        public int LeftResources
        {
            get
            {
                return this.m_LeftResources;
            }
            set
            {
                if (value >= 0)
                    this.m_LeftResources = value;
            }
        }
        public static void Configure()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler(EventSink_WorldLoad);

            if (!Misc.AutoRestart.Enabled)
                EventSink.WorldSave += new WorldSaveEventHandler(EventSink_WorldSave);

            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        public static void GrowAll()
        {
            ArrayList plants = PlantItem.Plants;
            DateTime now = DateTime.UtcNow;

            for (int i = plants.Count - 1; i >= 0; --i)
            {
                PlantItem plant = (PlantItem)plants[i];

                if (plant.IsGrowable && (plant.RootParent as Mobile) == null && now >= plant.PlantSystem.NextGrowth)
                    plant.PlantSystem.DoGrowthCheck();
            }
        }

        public void Reset(bool potions)
        {
            this.m_NextGrowth = DateTime.UtcNow + CheckDelay;
            this.m_GrowthIndicator = PlantGrowthIndicator.None;

            this.Hits = this.MaxHits;
            this.m_Infestation = 0;
            this.m_Fungus = 0;
            this.m_Poison = 0;
            this.m_Disease = 0;

            if (potions)
            {
                this.m_PoisonPotion = 0;
                this.m_CurePotion = 0;
                this.m_HealPotion = 0;
                this.m_StrengthPotion = 0;
            }

            this.m_Pollinated = false;
            this.m_AvailableSeeds = 0;
            this.m_LeftSeeds = 8;

            this.m_AvailableResources = 0;
            this.m_LeftResources = 8;
        }

        public int GetLocalizedDirtStatus()
        {
            if (this.Water <= 1)
                return 1060826; // hard
            else if (this.Water <= 2)
                return 1060827; // soft
            else if (this.Water <= 3)
                return 1060828; // squishy
            else
                return 1060829; // sopping wet
        }

        public int GetLocalizedHealth()
        {
            switch ( this.Health )
            {
                case PlantHealth.Dying:
                    return 1060825; // dying
                case PlantHealth.Wilted:
                    return 1060824; // wilted
                case PlantHealth.Healthy:
                    return 1060823; // healthy
                default:
                    return 1060822; // vibrant
            }
        }

        public void DoGrowthCheck()
        {
            if (!this.m_Plant.IsGrowable)
                return;

            if (DateTime.UtcNow < this.m_NextGrowth)
            {
                this.m_GrowthIndicator = PlantGrowthIndicator.Delay;
                return;
            }

            this.m_NextGrowth = DateTime.UtcNow + CheckDelay;

            if (!this.m_Plant.ValidGrowthLocation)
            {
                this.m_GrowthIndicator = PlantGrowthIndicator.InvalidLocation;
                return;
            }

            if (this.m_Plant.PlantStatus == PlantStatus.BowlOfDirt)
            {
                if (this.Water > 2 || Utility.RandomDouble() < 0.9)
                    this.Water--;
                return;
            }

            this.ApplyBeneficEffects();

            if (!this.ApplyMaladiesEffects()) // Dead
                return;

            this.Grow();

            this.UpdateMaladies();
        }

        public void Save(GenericWriter writer)
        {
            writer.Write((int)1); // version

            writer.Write((bool)this.m_FertileDirt);

            writer.Write((DateTime)this.m_NextGrowth);
            writer.Write((int)this.m_GrowthIndicator);

            writer.Write((int)this.m_Water);

            writer.Write((int)this.m_Hits);
            writer.Write((int)this.m_Infestation);
            writer.Write((int)this.m_Fungus);
            writer.Write((int)this.m_Poison);
            writer.Write((int)this.m_Disease);
            writer.Write((int)this.m_PoisonPotion);
            writer.Write((int)this.m_CurePotion);
            writer.Write((int)this.m_HealPotion);
            writer.Write((int)this.m_StrengthPotion);

            writer.Write((bool)this.m_Pollinated);
            writer.Write((int)this.m_SeedType);
            writer.Write((int)this.m_SeedHue);
            writer.Write((int)this.m_AvailableSeeds);
            writer.Write((int)this.m_LeftSeeds);

            writer.Write((int)this.m_AvailableResources);
            writer.Write((int)this.m_LeftResources);
        }

        private static void EventSink_Login(LoginEventArgs args)
        {
            Mobile from = args.Mobile;

            if (from.Backpack != null)
            {
                List<PlantItem> plants = from.Backpack.FindItemsByType<PlantItem>();

                foreach (PlantItem plant in plants)
                {
                    if (plant.IsGrowable)
                        plant.PlantSystem.DoGrowthCheck();
                }
            }

            BankBox bank = from.FindBankNoCreate();

            if (bank != null)
            {
                List<PlantItem> plants = bank.FindItemsByType<PlantItem>();

                foreach (PlantItem plant in plants)
                {
                    if (plant.IsGrowable)
                        plant.PlantSystem.DoGrowthCheck();
                }
            }
        }

        private static void EventSink_WorldLoad()
        {
            GrowAll();
        }

        private static void EventSink_WorldSave(WorldSaveEventArgs args)
        {
            GrowAll();
        }

        private void ApplyBeneficEffects()
        {
            if (this.PoisonPotion >= this.Infestation)
            {
                this.PoisonPotion -= this.Infestation;
                this.Infestation = 0;
            }
            else
            {
                this.Infestation -= this.PoisonPotion;
                this.PoisonPotion = 0;
            }

            if (this.CurePotion >= this.Fungus)
            {
                this.CurePotion -= this.Fungus;
                this.Fungus = 0;
            }
            else
            {
                this.Fungus -= this.CurePotion;
                this.CurePotion = 0;
            }

            if (this.HealPotion >= this.Poison)
            {
                this.HealPotion -= this.Poison;
                this.Poison = 0;
            }
            else
            {
                this.Poison -= this.HealPotion;
                this.HealPotion = 0;
            }

            if (this.HealPotion >= this.Disease)
            {
                this.HealPotion -= this.Disease;
                this.Disease = 0;
            }
            else
            {
                this.Disease -= this.HealPotion;
                this.HealPotion = 0;
            }

            if (!this.HasMaladies)
            {
                if (this.HealPotion > 0)
                    this.Hits += this.HealPotion * 7;
                else
                    this.Hits += 2;
            }

            this.HealPotion = 0;
        }

        private bool ApplyMaladiesEffects()
        {
            int damage = 0;

            if (this.Infestation > 0)
                damage += this.Infestation * Utility.RandomMinMax(3, 6);

            if (this.Fungus > 0)
                damage += this.Fungus * Utility.RandomMinMax(3, 6);

            if (this.Poison > 0)
                damage += this.Poison * Utility.RandomMinMax(3, 6);

            if (this.Disease > 0)
                damage += this.Disease * Utility.RandomMinMax(3, 6);

            if (this.Water > 2)
                damage += (this.Water - 2) * Utility.RandomMinMax(3, 6);
            else if (this.Water < 2)
                damage += (2 - this.Water) * Utility.RandomMinMax(3, 6);

            this.Hits -= damage;

            return this.m_Plant.IsGrowable && this.m_Plant.PlantStatus != PlantStatus.BowlOfDirt;
        }

        private void Grow()
        {
            if (this.Health < PlantHealth.Healthy)
            {
                this.m_GrowthIndicator = PlantGrowthIndicator.NotHealthy;
            }
            else if (this.m_FertileDirt && this.m_Plant.PlantStatus <= PlantStatus.Stage5 && Utility.RandomDouble() < 0.1)
            {
                int curStage = (int)this.m_Plant.PlantStatus;
                this.m_Plant.PlantStatus = (PlantStatus)(curStage + 2);

                this.m_GrowthIndicator = PlantGrowthIndicator.DoubleGrown;
            }
            else if (this.m_Plant.PlantStatus < PlantStatus.Stage9)
            {
                int curStage = (int)this.m_Plant.PlantStatus;
                this.m_Plant.PlantStatus = (PlantStatus)(curStage + 1);

                this.m_GrowthIndicator = PlantGrowthIndicator.Grown;
            }
            else
            {
                if (this.Pollinated && this.LeftSeeds > 0 && this.m_Plant.IsCrossable)
                {
                    this.LeftSeeds--;
                    this.AvailableSeeds++;
                }

                if (this.LeftResources > 0 && PlantResourceInfo.GetInfo(this.m_Plant.PlantType, this.m_Plant.PlantHue) != null)
                {
                    this.LeftResources--;
                    this.AvailableResources++;
                }

                this.m_GrowthIndicator = PlantGrowthIndicator.Grown;
            }

            if (this.m_Plant.PlantStatus >= PlantStatus.Stage9 && !this.Pollinated)
            {
                this.Pollinated = true;
                this.SeedType = this.m_Plant.PlantType;
                this.SeedHue = this.m_Plant.PlantHue;
            }
        }

        private void UpdateMaladies()
        {
            double infestationChance = 0.30 - this.StrengthPotion * 0.075 + (this.Water - 2) * 0.10;

            PlantTypeInfo typeInfo = PlantTypeInfo.GetInfo(this.m_Plant.PlantType);
            if (typeInfo.Flowery)
                infestationChance += 0.10;

            if (PlantHueInfo.IsBright(this.m_Plant.PlantHue))
                infestationChance += 0.10;

            if (Utility.RandomDouble() < infestationChance)
                this.Infestation++;

            double fungusChance = 0.15 - this.StrengthPotion * 0.075 + (this.Water - 2) * 0.10;

            if (Utility.RandomDouble() < fungusChance)
                this.Fungus++;

            if (this.Water > 2 || Utility.RandomDouble() < 0.9)
                this.Water--;

            if (this.PoisonPotion > 0)
            {
                this.Poison += this.PoisonPotion;
                this.PoisonPotion = 0;
            }

            if (this.CurePotion > 0)
            {
                this.Disease += this.CurePotion;
                this.CurePotion = 0;
            }

            this.StrengthPotion = 0;
        }
    }
}