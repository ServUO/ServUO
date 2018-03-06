using Server;
using System;
using Server.Mobiles;
using Server.Engines.Despise;
using System.Collections.Generic;
using System.Linq;

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
            IPooledEnumerable eable = master.GetMobilesInRange(3);

            foreach (Mobile m in eable)
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

            eable.Free();

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

    public class GateTeleporter : Item
    {
        private Point3D _Destination;
        private Map _DestinationMap;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Destination
        {
            get { return _Destination; }
            set
            {
                if (_Destination != value)
                {
                    _Destination = value;
                    AssignDestination(value);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map DestinationMap
        {
            get { return _DestinationMap; }
            set
            {
                if (DestinationMap != value)
                {
                    _DestinationMap = value;
                    AssignMap(value);
                }
            }
        }

        public List<InternalTeleporter> Teleporters { get; set; }

        [Constructable]
        public GateTeleporter()
            : this(19343, 0, Point3D.Zero, null)
        {
        }

        public GateTeleporter(int id, int hue, Point3D destination, Map destinationMap)
            : base(id)
        {
            Hue = hue;

            Movable = false;

            _Destination = destination;
            _DestinationMap = destinationMap;

            AssignTeleporters();
        }

        private void AssignTeleporters()
        {
            if (Teleporters != null)
            {
                foreach (var tele in Teleporters.Where(t => t != null && !t.Deleted))
                {
                    tele.Delete();
                }

                ColUtility.Free(Teleporters);
            }

            Teleporters = new List<InternalTeleporter>();

            for (int i = 0; i <= 7; i++)
            {
                Direction offset = (Direction)i;

                var tele = new InternalTeleporter(this, _Destination, _DestinationMap);

                int x = this.X;
                int y = this.Y;
                int z = this.Z;

                Movement.Movement.Offset(offset, ref x, ref y);
                tele.MoveToWorld(new Point3D(x, y, z), this.Map);

                Teleporters.Add(tele);
            }
        }

        public void AssignDestination(Point3D p)
        {
            if (Teleporters == null)
            {
                AssignTeleporters();
            }
            else
            {
                Teleporters.ForEach(t => t.PointDest = p);
            }
        }

        public void AssignMap(Map map)
        {
            if (Teleporters == null)
            {
                AssignTeleporters();
            }
            else
            {
                Teleporters.ForEach(t => t.MapDest = map);
            }
        }

        public override void OnMapChange()
        {
            if (Teleporters != null)
            {
                Teleporters.ForEach(t => t.Map = Map);
            }
        }

        public override void OnLocationChange(Point3D old)
        {
            if (Teleporters != null)
            {
                Teleporters.ForEach(t =>
                    {
                        t.Location = new Point3D(X + (t.X - old.X), Y + (t.Y - old.Y), Z + (t.Z - old.Z));
                    });
            }
        }

        public override void Delete()
        {
            base.Delete();

            if (Teleporters != null)
            {
                Teleporters.ForEach(t => t.Delete());

                ColUtility.Free(Teleporters);
            }
        }

        public GateTeleporter(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

            writer.Write(_Destination);
            writer.Write(_DestinationMap);

            writer.Write(Teleporters == null ? 0 : Teleporters.Count);

            if (Teleporters != null)
            {
                Teleporters.ForEach(t => writer.Write(t));
            }
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();

            _Destination = reader.ReadPoint3D();
            _DestinationMap = reader.ReadMap();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                if (Teleporters == null)
                    Teleporters = new List<InternalTeleporter>();

                var tele = reader.ReadItem() as InternalTeleporter;

                if (tele != null)
                {
                    Teleporters.Add(tele);
                    tele.Master = this;
                }
            }
		}

        public class InternalTeleporter : Teleporter
        {
            [CommandProperty(AccessLevel.GameMaster)]
            public GateTeleporter Master { get; set; }

            public InternalTeleporter(GateTeleporter master, Point3D dest, Map destMap)
                : base(dest, destMap, true)
            {
                Master = master;
            }

            public override bool OnMoveOver(Mobile m)
            {
                return true;
            }

            public override bool HandlesOnMovement { get { return Master != null && Utility.InRange(Master.Location, Location, 1) && this.Map == Master.Map; } }
            
            public override void OnMovement(Mobile m, Point3D oldLocation)
            {
                if (Master == null || Master.Destination == Point3D.Zero || Master.Map == null || Master.Map == Map.Internal)
                    return;

                if (m.Location == Location)
                {
                    var eable = Map.GetItemsInRange(oldLocation, 0);

                    foreach (Item item in eable)
                    {
                        if (item is InternalTeleporter || item == Master)
                        {
                            eable.Free();
                            return;
                        }
                    }

                    base.OnMoveOver(m);
                }
            }

            public InternalTeleporter(Serial serial)
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
}