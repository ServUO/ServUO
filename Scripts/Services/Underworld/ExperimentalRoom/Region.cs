using Server.Items;
using Server.Mobiles;
using System.Xml;

namespace Server.Regions
{
    public class ExperimentalRoomRegion : DungeonRegion
    {
        public ExperimentalRoomRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        private static Rectangle2D m_Entrance = new Rectangle2D(994, 1114, 4, 4);

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            if (m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned))
            {
                foreach (Rectangle2D rec in m_RoomRecs)
                {
                    if (rec.Contains(m.Location))
                    {
                        KickToEntrance(m);
                        Mobile master = ((BaseCreature)m).GetMaster();

                        if (master != null && master.NetState != null)
                            master.SendLocalizedMessage(1113472); // Your pet has been kicked out of the room. This is not a stable!
                    }
                }
            }
            else if (m is PlayerMobile && m.AccessLevel == AccessLevel.Player)
            {
                if (m.Backpack == null)
                    KickToEntrance(m);
                else
                {
                    Item item = m.Backpack.FindItemByType(typeof(ExperimentalGem));

                    if (item == null)
                    {
                        foreach (Rectangle2D rec in m_RoomRecs)
                        {
                            if (rec.Contains(m.Location))
                                KickToEntrance(m);
                        }
                    }
                }
            }
        }

        public void KickToEntrance(Mobile from)
        {
            if (from == null || from.Map == null)
                return;

            int x = Utility.RandomMinMax(m_Entrance.X, m_Entrance.X + m_Entrance.Width);
            int y = Utility.RandomMinMax(m_Entrance.Y, m_Entrance.Y + m_Entrance.Height);
            int z = from.Map.GetAverageZ(x, y);

            Point3D loc = from.Location;
            Point3D p = new Point3D(x, y, z);

            if (from is PlayerMobile)
                BaseCreature.TeleportPets(from, p, Map.TerMur);

            from.MoveToWorld(p, Map.TerMur);

            Effects.SendLocationParticles(EffectItem.Create(loc, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
            Effects.SendLocationParticles(EffectItem.Create(p, from.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
        }

        private static readonly Rectangle2D[] m_RoomRecs = new Rectangle2D[]
        {
            new Rectangle2D(977, 1104, 15, 10), //RoomOne
            new Rectangle2D(977, 1092, 15, 9), //RoomTwo
            new Rectangle2D(977, 1074, 15, 10), //RoomThree
        };
    }
}