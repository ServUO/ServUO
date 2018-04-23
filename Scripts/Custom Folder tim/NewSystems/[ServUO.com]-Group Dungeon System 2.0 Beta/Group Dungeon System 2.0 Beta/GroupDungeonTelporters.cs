using System;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class GroupDungeonTeleporter : Teleporter
    {
        private GroupDungeonStone m_Stone;

        [CommandProperty(AccessLevel.GameMaster)]
        public GroupDungeonStone Stone
        {
            get { return m_Stone; }
            set { m_Stone = value; InvalidateProperties(); }
        }

        [Constructable]
		public GroupDungeonTeleporter() : this( new Point3D( 0, 0, 0 ), null, false )
		{
        }

		[Constructable]
		public GroupDungeonTeleporter( Point3D pointDest, Map mapDest ) : this( pointDest, mapDest, false )
		{
        }

        [Constructable]
		public GroupDungeonTeleporter( Point3D pointDest, Map mapDest, bool creatures ) : base( pointDest, mapDest, creatures)
		{
            Name = "an instance zone-in teleporter";
            Hue = 1157;
            Visible = true;
            base.MapDest = this.Map;
            base.PointDest = this.Location;
        }

        public GroupDungeonTeleporter(Serial serial) : base(serial)
		{
            Name = "an instance zone-in teleporter";
            Hue = 1157;
            Visible = true;
            base.MapDest = this.Map;
            base.PointDest = this.Location;
		}

        public override bool OnMoveOver(Mobile m)
        {
            if (m_Stone != null)
            {
                if (m_Stone.IRegion != null)
                {
                    if (m_Stone.IRegion.CanEnter(m))
                        return base.OnMoveOver(m);
                }
                else
                    m.SendMessage(34, "Teleporter not linked to a dungeon region. Contact staff.");
            }
            else
                m.SendMessage(34, "Teleporter not linked to a dungeon stone. Contact staff.");
            return true;
        }

        public override void DoTeleport(Mobile m)
        {
            Map map = base.MapDest;

            if (map == null || map == Map.Internal)
                map = m.Map;

            Point3D p = base.PointDest;

            if (p == Point3D.Zero)
                p = m.Location;

            // Check for AllowPets here
            if (!m_Stone.AllowPets && CountPets(m) > 0)
                m.SendMessage(34, "Pets are not allowed to enter {0}.", m_Stone.DungeonName);
            else
                Server.Mobiles.BaseCreature.TeleportPets(m, p, map);

            bool sendEffect = (!m.Hidden || m.AccessLevel == AccessLevel.Player);

            if (base.SourceEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            m.MoveToWorld(p, map);

            if (base.DestEffect && sendEffect)
                Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            if (base.SoundID > 0 && sendEffect)
                Effects.PlaySound(m.Location, m.Map, base.SoundID);
        }

        public int CountPets(Mobile master)
        {
            int count = 0;

            foreach (Mobile m in master.GetMobilesInRange(3))
            {
                if (m is BaseCreature)
                {
                    BaseCreature pet = (BaseCreature)m;

                    if (pet.Controlled && pet.ControlMaster == master)
                        count++;
                }
            }

            return count;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_Stone);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Stone = (GroupDungeonStone)reader.ReadItem();
                        break;
                    }
            }
        }
    }

    public class GroupDungeonExit : Teleporter
    {
        [Constructable]
        public GroupDungeonExit()
            : this(new Point3D(0, 0, 0), null, false)
        {
        }

        [Constructable]
        public GroupDungeonExit(Point3D pointDest, Map mapDest)
            : this(pointDest, mapDest, false)
        {
        }

        [Constructable]
        public GroupDungeonExit(Point3D pointDest, Map mapDest, bool creatures)
            : base(pointDest, mapDest, creatures)
        {
            Name = "an instance zone-out teleporter";
            Hue = 1155;
            Visible = false;
            base.MapDest = this.Map;
            base.PointDest = this.Location;
        }

        public GroupDungeonExit(Serial serial)
            : base(serial)
        {
            Name = "an instance zone-out teleporter";
            Hue = 1155;
            Visible = false;
            base.MapDest = this.Map;
            base.PointDest = this.Location;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.Alive && m is PlayerMobile) // Only allow living players to leave.
                return base.OnMoveOver(m);    // This is to wait for rez or wipe.
                    
            return true;
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

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }
        }
    }
}