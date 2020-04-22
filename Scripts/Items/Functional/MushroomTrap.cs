using Server.Regions;
using System;

namespace Server.Items
{
    public class MushroomTrap : BaseTrap
    {
        [Constructable]
        public MushroomTrap()
            : base(0x1125)
        {
        }

        public MushroomTrap(Serial serial)
            : base(serial)
        {
        }

        public override bool PassivelyTriggered => true;
        public override TimeSpan PassiveTriggerDelay => TimeSpan.Zero;
        public override int PassiveTriggerRange => 2;
        public override TimeSpan ResetDelay => TimeSpan.Zero;
        public override void OnTrigger(Mobile from)
        {
            if (!from.Alive || ItemID != 0x1125 || from.IsStaff())
                return;

            ItemID = 0x1126;
            Effects.PlaySound(Location, Map, 0x306);

            Spells.SpellHelper.Damage(TimeSpan.FromSeconds(0.5), from, from, Utility.Dice(2, 4, 0));

            Timer.DelayCall(TimeSpan.FromSeconds(2.0), OnMushroomReset);
        }

        public virtual void OnMushroomReset()
        {
            if (Region.Find(Location, Map).IsPartOf<DungeonRegion>())
                ItemID = 0x1125; // reset
            else
                Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (ItemID == 0x1126)
                OnMushroomReset();
        }
    }
}
