using System;

namespace Server.Items
{
    public class GiantSpikeTrap : BaseTrap
    {
        [Constructable]
        public GiantSpikeTrap()
            : base(1)
        {
        }

        public GiantSpikeTrap(Serial serial)
            : base(serial)
        {
        }

        public override bool PassivelyTriggered
        {
            get
            {
                return true;
            }
        }
        public override TimeSpan PassiveTriggerDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public override int PassiveTriggerRange
        {
            get
            {
                return 3;
            }
        }
        public override TimeSpan ResetDelay
        {
            get
            {
                return TimeSpan.FromSeconds(0.0);
            }
        }
        public override void OnTrigger(Mobile from)
        {
            if (from.IsStaff())
                return;

            Effects.SendLocationEffect(this.Location, this.Map, 0x1D99, 48, 2, this.GetEffectHue(), 0);

            if (from.Alive && this.CheckRange(from.Location, 0))
                Spells.SpellHelper.Damage(TimeSpan.FromTicks(1), from, from, Utility.Dice(10, 7, 0));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}