using System;

namespace Server.Items
{
    public class FireColumnTrap : BaseTrap
    {
        [Constructable]
        public FireColumnTrap()
            : base(0x1B71)
        {
        }

        public override bool PassivelyTriggered { get { return true; } }
        public override TimeSpan PassiveTriggerDelay { get { return TimeSpan.FromSeconds(2.0); } }
        public override int PassiveTriggerRange { get { return 3; } }
        public override TimeSpan ResetDelay { get { return TimeSpan.FromSeconds(0.5); } }
        public override int MessageHue { get { return 0x66D; } }


        public override void OnTrigger(Mobile from)
        {
            if (!from.Player || !from.Alive || from.AccessLevel > AccessLevel.Player)
                return;
            //if ( !from.Alive || from.AccessLevel > AccessLevel.Player ||
            //	from is BaseCreature && !( (BaseCreature)from ).Controlled )
            //	return;
            //if ( from.AccessLevel > AccessLevel.Player )
            //	return;

            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
            Effects.PlaySound(Location, Map, 0x225);

            if (from.Alive && CheckRange(from.Location, 0))
            {
                Spells.SpellHelper.Damage(TimeSpan.FromSeconds(0.5), from, from, Utility.RandomMinMax(10, 40), 0, 100, 0, 0, 0);
            }
        }

        public FireColumnTrap(Serial serial)
            : base(serial)
        {
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