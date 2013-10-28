using System;

namespace Server.Engines.Harvest
{
    public class HarvestBank
    {
        private readonly int m_Maximum;
        readonly HarvestDefinition m_Definition;
        private int m_Current;
        private DateTime m_NextRespawn;
        private HarvestVein m_Vein, m_DefaultVein;
        public HarvestBank(HarvestDefinition def, HarvestVein defaultVein)
        {
            this.m_Maximum = Utility.RandomMinMax(def.MinTotal, def.MaxTotal);
            this.m_Current = this.m_Maximum;
            this.m_DefaultVein = defaultVein;
            this.m_Vein = this.m_DefaultVein;

            this.m_Definition = def;
        }

        public HarvestDefinition Definition
        {
            get
            {
                return this.m_Definition;
            }
        }
        public int Current
        {
            get
            {
                this.CheckRespawn();
                return this.m_Current;
            }
        }
        public HarvestVein Vein
        {
            get
            {
                this.CheckRespawn();
                return this.m_Vein;
            }
            set
            {
                this.m_Vein = value;
            }
        }
        public HarvestVein DefaultVein
        {
            get
            {
                this.CheckRespawn();
                return this.m_DefaultVein;
            }
        }
        public void CheckRespawn()
        {
            if (this.m_Current == this.m_Maximum || this.m_NextRespawn > DateTime.UtcNow)
                return;

            this.m_Current = this.m_Maximum;

            if (this.m_Definition.RandomizeVeins)
            {
                this.m_DefaultVein = this.m_Definition.GetVeinFrom(Utility.RandomDouble());
            }

            this.m_Vein = this.m_DefaultVein;
        }

        public void Consume(int amount, Mobile from)
        {
            this.CheckRespawn();

            if (this.m_Current == this.m_Maximum)
            {
                double min = this.m_Definition.MinRespawn.TotalMinutes;
                double max = this.m_Definition.MaxRespawn.TotalMinutes;
                double rnd = Utility.RandomDouble();

                this.m_Current = this.m_Maximum - amount;

                double minutes = min + (rnd * (max - min));
                if (this.m_Definition.RaceBonus && from.Race == Race.Elf)	//def.RaceBonus = Core.ML
                    minutes *= .75;	//25% off the time.  

                this.m_NextRespawn = DateTime.UtcNow + TimeSpan.FromMinutes(minutes);
            }
            else
            {
                this.m_Current -= amount;
            }

            if (this.m_Current < 0)
                this.m_Current = 0;
        }
    }
}