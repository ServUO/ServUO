using System;
using Server.Targeting;

namespace Server.Engines.Plants
{
    public class PollinateTarget : Target
    {
        private readonly PlantItem m_Plant;
        public PollinateTarget(PlantItem plant)
            : base(3, true, TargetFlags.None)
        {
            this.m_Plant = plant;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (!this.m_Plant.Deleted && this.m_Plant.PlantStatus < PlantStatus.DecorativePlant && from.InRange(this.m_Plant.GetWorldLocation(), 3))
            {
                if (!this.m_Plant.IsUsableBy(from))
                {
                    this.m_Plant.LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                }
                else if (!this.m_Plant.IsCrossable)
                {
                    this.m_Plant.LabelTo(from, 1053050); // You cannot gather pollen from a mutated plant!
                }
                else if (!this.m_Plant.PlantSystem.PollenProducing)
                {
                    this.m_Plant.LabelTo(from, 1053051); // You cannot gather pollen from a plant in this stage of development!
                }
                else if (this.m_Plant.PlantSystem.Health < PlantHealth.Healthy)
                {
                    this.m_Plant.LabelTo(from, 1053052); // You cannot gather pollen from an unhealthy plant!
                }
                else
                {
                    PlantItem targ = targeted as PlantItem;

                    if (targ == null || targ.PlantStatus >= PlantStatus.DecorativePlant || targ.PlantStatus <= PlantStatus.BowlOfDirt)
                    {
                        this.m_Plant.LabelTo(from, 1053070); // You can only pollinate other specially grown plants!
                    }
                    else if (!targ.IsUsableBy(from))
                    {
                        targ.LabelTo(from, 1061856); // You must have the item in your backpack or locked down in order to use it.
                    }
                    else if (!targ.IsCrossable)
                    {
                        targ.LabelTo(from, 1053073); // You cannot cross-pollinate with a mutated plant!
                    }
                    else if (!targ.PlantSystem.PollenProducing)
                    {
                        targ.LabelTo(from, 1053074); // This plant is not in the flowering stage. You cannot pollinate it!
                    }
                    else if (targ.PlantSystem.Health < PlantHealth.Healthy)
                    {
                        targ.LabelTo(from, 1053075); // You cannot pollinate an unhealthy plant!
                    }
                    else if (targ.PlantSystem.Pollinated)
                    {
                        targ.LabelTo(from, 1053072); // This plant has already been pollinated!
                    }
                    else if (targ == this.m_Plant)
                    {
                        targ.PlantSystem.Pollinated = true;
                        targ.PlantSystem.SeedType = this.m_Plant.PlantType;
                        targ.PlantSystem.SeedHue = this.m_Plant.PlantHue;

                        targ.LabelTo(from, 1053071); // You pollinate the plant with its own pollen.
                    }
                    else
                    {
                        targ.PlantSystem.Pollinated = true;
                        targ.PlantSystem.SeedType = PlantTypeInfo.Cross(this.m_Plant.PlantType, targ.PlantType);
                        targ.PlantSystem.SeedHue = PlantHueInfo.Cross(this.m_Plant.PlantHue, targ.PlantHue);

                        targ.LabelTo(from, 1053076); // You successfully cross-pollinate the plant.
                    }
                }
            }
        }

        protected override void OnTargetFinish(Mobile from)
        {
            if (!this.m_Plant.Deleted && this.m_Plant.PlantStatus < PlantStatus.DecorativePlant && this.m_Plant.PlantStatus != PlantStatus.BowlOfDirt && from.InRange(this.m_Plant.GetWorldLocation(), 3) && this.m_Plant.IsUsableBy(from))
            {
                from.SendGump(new ReproductionGump(this.m_Plant));
            }
        }
    }
}