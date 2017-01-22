using System;
using Server;
using Server.Mobiles;
using Server.Engines.MyrmidexInvasion;
using Server.Engines.Quests;

namespace Server.Items
{
    public class MyrmidexPitTeleporter : Teleporter
    {
        public Allegiance Allegiance { get; set; }

        [Constructable]
        public MyrmidexPitTeleporter(Allegiance allegiance, Point3D dest, Map map)
            : base(dest, map)
        {
            Allegiance = allegiance;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (MyrmidexInvasionSystem.IsAlliedWith(m, Allegiance) || m.AccessLevel > AccessLevel.Player)
                return base.OnMoveOver(m);

            m.SendLocalizedMessage(Allegiance == Allegiance.Myrmidex ? 1156838 : 1156839); // You must ally yourself to the Eodonians to enter.

            return false;
        }

        public MyrmidexPitTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)Allegiance);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            Allegiance = (Allegiance)reader.ReadInt();
        }
    }

    public class MyrmidexQueenTeleporter : Teleporter
    {
        [Constructable]
        public MyrmidexQueenTeleporter(Point3D p, Map map)
            : base(p, map)
        {
        }

        public override bool OnMoveOver(Mobile from)
        {
            if ((from is PlayerMobile && QuestHelper.GetQuest((PlayerMobile)from, typeof(InsecticideAndRegicideQuest)) != null) || from.AccessLevel > AccessLevel.Player)
                return base.OnMoveOver(from);

            from.SendLocalizedMessage(1156840); // You must be on the "Insecticide & Regicide" quest to enter.  Visit Professor Rafkin in Sir Geoffrey's camp to the East.

            return false;
        }

        public MyrmidexQueenTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}