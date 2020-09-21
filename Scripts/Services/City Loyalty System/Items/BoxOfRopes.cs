using Server.Engines.CityLoyalty;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class BoxOfRopes : Container
    {
        public City City { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltySystem CitySystem { get { return CityLoyaltySystem.GetCityInstance(City); } set { } }

        public override int LabelNumber => 1152262;  // a box of ropes

        public BoxOfRopes(City city) : base(3650)
        {
            Hue = 1944;
            Movable = false;

            City = city;

            if (CitySystem != null && CitySystem.Captain != null)
                CitySystem.Captain.Box = this;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (CityLoyaltySystem.Enabled && CityLoyaltySystem.IsSetup() && from.InRange(Location, 3))
            {
                if (_Cooldown == null || !_Cooldown.ContainsKey(from) || _Cooldown[from] < DateTime.UtcNow)
                {
                    GuardsmansRope rope = new GuardsmansRope();
                    from.AddToBackpack(rope);

                    from.SendLocalizedMessage(1152263); // You take a rope from the chest. Use it to arrest rioters and subdued raiders.

                    if (_Cooldown == null)
                        _Cooldown = new Dictionary<Mobile, DateTime>();

                    _Cooldown[from] = DateTime.UtcNow + TimeSpan.FromSeconds(60);
                }
                else
                    from.SendLocalizedMessage(1152264); // You must wait a moment before taking another rope.
            }
            else
                from.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045, from.NetState); // I can't reach that.
        }

        public static Dictionary<Mobile, DateTime> _Cooldown { get; set; }

        public static void Defrag()
        {
            if (_Cooldown == null)
                return;

            var remove = new List<Mobile>();

            foreach (var kvp in _Cooldown)
            {
                if (kvp.Value < DateTime.UtcNow)
                {
                    remove.Add(kvp.Key);
                }
            }

            foreach (var m in remove)
            {
                _Cooldown.Remove(m);
            }

            ColUtility.Free(remove);
        }

        public BoxOfRopes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)City);

            Defrag();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            City = (City)reader.ReadInt();

            if (CitySystem != null && CitySystem.Captain != null)
                CitySystem.Captain.Box = this;
        }
    }
}
