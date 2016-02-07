using System;
using Server.Targeting;

namespace Server.Engines.Plants
{
    public class PlantPourTarget : Target
    {
        private readonly PlantItem m_Plant;
        public PlantPourTarget(PlantItem plant)
            : base(3, true, TargetFlags.None)
        {
            this.m_Plant = plant;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!this.m_Plant.Deleted && from.InRange(this.m_Plant.GetWorldLocation(), 3) && targeted is Item)
            {
                this.m_Plant.Pour(from, (Item)targeted);
            }
        }

        protected override void OnTargetFinish(Mobile from)
        {
            if (!this.m_Plant.Deleted && this.m_Plant.PlantStatus < PlantStatus.DecorativePlant && from.InRange(this.m_Plant.GetWorldLocation(), 3) && this.m_Plant.IsUsableBy(from))
            {
                if (from.HasGump(typeof(MainPlantGump)))
                    from.CloseGump(typeof(MainPlantGump));

                from.SendGump(new MainPlantGump(this.m_Plant));
            }
        }
    }
}