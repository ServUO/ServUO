using Server;
using System;
using Server.Mobiles;
using Server.Engines.Despise;
using System.Collections.Generic;

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

            TeleportPets(m, p, map);

            bool sendEffect = (!m.Hidden || m.AccessLevel == AccessLevel.Player);

            if (SourceEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            m.MoveToWorld(p, map);

            if (DestEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            if (SoundID > 0 && sendEffect)
                Effects.PlaySound(m.Location, m.Map, SoundID);
        }

        public static void TeleportPets(Mobile master, Point3D loc, Map map)
        {
            var move = new List<Mobile>();

            foreach (Mobile m in master.GetMobilesInRange(3))
            {
                if (m is BaseCreature && !(m is DespiseCreature))
                {
                    BaseCreature pet = (BaseCreature)m;

                    if (pet.Controlled && pet.ControlMaster == master)
                    {
                        if (pet.ControlOrder == OrderType.Guard || pet.ControlOrder == OrderType.Follow ||
                            pet.ControlOrder == OrderType.Come)
                        {
                            move.Add(pet);
                        }
                    }
                }
            }

            foreach (Mobile m in move)
            {
                m.MoveToWorld(loc, map);
            }

            move.Clear();
            move.TrimExcess();
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