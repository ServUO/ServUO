using Server;
using System;
using Server.Mobiles;
using Server.Engines.Despise;

namespace Server.Items
{
    public class DespiseTeleporter : Teleporter
    {
        [Constructable]
        public DespiseTeleporter()
        {
        }

        public override bool CanTeleport(Mobile m)
        {
            if (m is DespiseCreature)
                return false;

            return base.CanTeleport(m);
        }

        public override void DoTeleport(Mobile m)
        {
            Map map = MapDest;

            if (map == null || map == Map.Internal)
                map = m.Map;

            Point3D p = PointDest;

            if (p == Point3D.Zero)
                p = m.Location;

            //Server.Mobiles.BaseCreature.TeleportPets(m, p, map);

            bool sendEffect = (!m.Hidden || m.AccessLevel == AccessLevel.Player);

            if (SourceEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            m.MoveToWorld(p, map);

            if (DestEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            if (SoundID > 0 && sendEffect)
                Effects.PlaySound(m.Location, m.Map, SoundID);
        }

        public DespiseTeleporter(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
    }
}