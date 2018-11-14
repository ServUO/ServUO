using System;

namespace Server.Engines.Harvest
{
    public class HarvestTimer : Timer
    {
        private readonly Mobile m_From;
        private readonly Item m_Tool;
        private readonly HarvestSystem m_System;
        private readonly HarvestDefinition m_Definition;
        private readonly object m_ToHarvest;
        private readonly object m_Locked;
        private readonly int m_Count;
        private int m_Index;
        public HarvestTimer(Mobile from, Item tool, HarvestSystem system, HarvestDefinition def, object toHarvest, object locked)
            : base(TimeSpan.Zero, def.EffectDelay)
        {
            this.m_From = from;
            this.m_Tool = tool;
            this.m_System = system;
            this.m_Definition = def;
            this.m_ToHarvest = toHarvest;
            this.m_Locked = locked;
            this.m_Count = Utility.RandomList(def.EffectCounts);
        }

        protected override void OnTick()
        {
            if (!this.m_System.OnHarvesting(this.m_From, this.m_Tool, this.m_Definition, this.m_ToHarvest, this.m_Locked, ++this.m_Index == this.m_Count))
                this.Stop();
        }
    }
}