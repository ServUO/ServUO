using Server.Engines.Fellowship;
using Server.Mobiles;

namespace Server.Items
{
    public class BlackthornDungeonTeleporter : Item
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Dest { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public FellowshipChain Chain { get; set; }

        [Constructable]
        public BlackthornDungeonTeleporter()
            : this(FellowshipChain.None)
        {
        }

        [Constructable]
        public BlackthornDungeonTeleporter(FellowshipChain chain)
            : base(0x1827)
        {
            Chain = chain;
            Visible = true;
            Movable = false;
        }

        public BlackthornDungeonTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is PlayerMobile)
            {
                if (Worker.FellowshipChainList.ContainsKey(m) && Worker.FellowshipChainList[m] >= Chain || Chain == FellowshipChain.None)
                {
                    Timer.DelayCall(DoTeleport, m);
                }
                else
                {
                    m.SendLocalizedMessage(1159235); // You may not pass.                    
                }
            }

            return base.OnMoveOver(m);
        }

        public virtual void DoTeleport(Mobile m)
        {
            Map map = Map;

            if (map == null || map == Map.Internal)
            {
                map = m.Map;
            }

            Point3D p = Dest;

            if (p == Point3D.Zero)
            {
                p = m.Location;
            }

            BaseCreature.TeleportPets(m, p, map);

            m.MoveToWorld(p, map);

            m.PlaySound(510);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(Dest);
            writer.Write((int)Chain);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Dest = reader.ReadPoint3D();
            Chain = (FellowshipChain)reader.ReadInt();
        }
    }
}
