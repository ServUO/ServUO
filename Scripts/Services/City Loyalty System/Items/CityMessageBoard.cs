using Server.Engines.CityLoyalty;

namespace Server.Items
{
    public class CityMessageBoard : BasePlayerBB
    {
        public City City { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltySystem CitySystem { get => CityLoyaltySystem.GetCityInstance(City); set { } }

        public override int LabelNumber => 1027774;  // bulletin board
        public override bool Public => true;
        public override bool ForceShowProperties => true;

        [Constructable]
        public CityMessageBoard(City city, int id) : base(id)
        {
            Movable = false;
            City = city;
        }

        public override bool CanPostGreeting(Multis.BaseHouse house, Mobile m)
        {
            CityLoyaltySystem sys = CitySystem;

            return sys != null && (m.AccessLevel >= AccessLevel.GameMaster || sys.Governor == m);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!CityLoyaltySystem.Enabled || CitySystem == null)
                return;

            if (CitySystem.IsCitizen(from))
            {
                if (from.InRange(Location, 3))
                {
                    from.SendGump(new PlayerBBGump(from, null, this, 0));
                }
                else
                {
                    from.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045, from.NetState);
                }
            }
            else
            {
                from.SendLocalizedMessage(1154275); // Only Citizens of this City may use this. 
            }
        }

        public CityMessageBoard(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)City);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            City = (City)reader.ReadInt();
            CitySystem.Board = this;
        }
    }
}
