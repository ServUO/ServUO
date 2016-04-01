using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
    public class DarkTidesHorn : HornOfRetreat
    {
        [Constructable]
        public DarkTidesHorn()
        {
            this.DestLoc = new Point3D(2103, 1319, -68);
            this.DestMap = Map.Malas;
        }

        public DarkTidesHorn(Serial serial)
            : base(serial)
        {
        }

        public override bool ValidateUse(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            return (pm != null && pm.Quest is DarkTidesQuest);
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