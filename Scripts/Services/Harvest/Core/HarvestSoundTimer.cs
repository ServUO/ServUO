using System;

namespace Server.Engines.Harvest
{
    public class HarvestSoundTimer : Timer
    {
        private readonly Mobile m_From;
        private readonly Item m_Tool;
        private readonly HarvestSystem m_System;
        private readonly HarvestDefinition m_Definition;
        private readonly object m_ToHarvest;
        private readonly object m_Locked;
        private readonly bool m_Last;
        public HarvestSoundTimer(Mobile from, Item tool, HarvestSystem system, HarvestDefinition def, object toHarvest, object locked, bool last)
            : base(def.EffectSoundDelay)
        {
            this.m_From = from;
            this.m_Tool = tool;
            this.m_System = system;
            this.m_Definition = def;
            this.m_ToHarvest = toHarvest;
            this.m_Locked = locked;
            this.m_Last = last;
        }

        protected override void OnTick()
        {
            this.m_System.DoHarvestingSound(this.m_From, this.m_Tool, this.m_Definition, this.m_ToHarvest);

            if (this.m_Last)
                this.m_System.FinishHarvesting(this.m_From, this.m_Tool, this.m_Definition, this.m_ToHarvest, this.m_Locked);
        }
    }
}