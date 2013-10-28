using System;

namespace Server.Items
{
    public class JacobsPickaxe : Pickaxe
    {
        [Constructable]
        public JacobsPickaxe()
            : base()
        {
            this.SkillBonuses.SetValues(0, SkillName.Mining, 10.0);
            this.UsesRemaining = 20;

            Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), new TimerCallback(Tick_Callback));
        }

        public JacobsPickaxe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1077758;
            }
        }// Jacob's Pickaxe
        public override void OnDoubleClick(Mobile from)
        {
            if (this.HarvestSystem == null)
                return;

            if (this.IsChildOf(from.Backpack) || this.Parent == from)
            {
                if (this.UsesRemaining > 0)
                    this.HarvestSystem.BeginHarvesting(from, this);
                else 
                    from.SendLocalizedMessage(1072306); // You must wait a moment for it to recharge.
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), new TimerCallback(Tick_Callback));
        }

        private void Tick_Callback()
        { 
            int charge = this.UsesRemaining + 10 > 20 ? 20 - this.UsesRemaining : 10;
		
            if (charge > 0)
                this.UsesRemaining += charge;
				
            this.InvalidateProperties();
        }
    }
}