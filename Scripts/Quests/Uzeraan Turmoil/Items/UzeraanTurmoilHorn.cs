using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Haven
{
    public class UzeraanTurmoilHorn : HornOfRetreat
    {
        [Constructable]
        public UzeraanTurmoilHorn()
        {
            this.DestLoc = new Point3D(3597, 2582, 0);
            this.DestMap = Map.Trammel;
        }

        public UzeraanTurmoilHorn(Serial serial)
            : base(serial)
        {
        }

        public override bool ValidateUse(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            return (pm != null && pm.Quest is UzeraanTurmoilQuest);
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