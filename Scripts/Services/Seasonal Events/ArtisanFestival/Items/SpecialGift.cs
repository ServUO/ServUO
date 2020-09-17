using Server.Items;
using Server.Mobiles;

namespace Server.Engines.ArtisanFestival
{
    public class SpecialGift : BaseContainer
    {
        private Mobile _Owner;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get { return _Owner; } set { _Owner = value; InvalidateProperties(); } }

        public SpecialGift(Mobile m)
            : base(0x9E1A)
        {
            Owner = m;
            DropItem(RandomGift(m));
        }

        public SpecialGift(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1157163, _Owner != null ? _Owner.Name : "Somebody"); // A Special Gift for ~1_NAME~ 
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile pm && from.InRange(GetWorldLocation(), 3))
            {
                if (_Owner == null || pm == _Owner)
                {
                    base.OnDoubleClick(pm);
                }
                else
                {
                    pm.SendLocalizedMessage(1157162); // That's not your gift!
                }
            }
        }

        private Item RandomGift(Mobile m)
        {
            switch (Utility.Random(4))
            {
                default: return new RewardLantern(m);
                case 1: return new RewardPillow(m);
                case 2: return new RewardPainting(m);
                case 3: return new RewardSculpture(m);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Owner = reader.ReadMobile();
        }
    }

    public class FestivalGiftBox : Item
    {
        public FestivalGiftBox()
            : base(Utility.Random(0x46A2, 6))
        {
            Hue = Utility.RandomMinMax(1, 500);
        }

        public FestivalGiftBox(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                var gift = new SpecialGift(from)
                {
                    Hue = Hue
                };

                from.Backpack.DropItem(gift);
                Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
