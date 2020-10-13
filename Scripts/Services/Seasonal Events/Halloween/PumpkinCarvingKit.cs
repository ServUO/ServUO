using Server.Network;
using Server.Targeting;
using System.Linq;

namespace Server.Items
{
    public class PumpkinDefinition
    {
        public int UnlitItemID { get; set; }
        public int LitItemID { get; set; }

        public PumpkinDefinition(int unlit, int lit)
        {
            UnlitItemID = unlit;
            LitItemID = lit;
        }
    }

    [Flipable(0x992D, 0x992E)]
    public class PumpkinCarvingKit : Item
    {
        public override int LabelNumber => 1154271;  // Jack O' Lantern Carving Kit

        [Constructable]
        public PumpkinCarvingKit()
            : base(0x992D)
        {
            Weight = 1.0;
            Hue = 1258;
        }

        public PumpkinCarvingKit(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1080058); // This must be in your backpack to use it.
            }
            else
            {
                from.SendLocalizedMessage(1154273); // What do you wish to carve?
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private readonly Item m_Item;

            public InternalTarget(Item item)
                : base(2, true, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCarvablePumpkin)
                {
                    BaseCarvablePumpkin pumpkin = (BaseCarvablePumpkin)targeted;

                    if (!pumpkin.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1045158); // You must have the item in your backpack to target it.
                    }
                    else if (pumpkin.CarvedBy == null)
                    {
                        from.PlaySound(0x249);

                        pumpkin.ItemID = pumpkin.PumpkinDefinition[Utility.Random(pumpkin.PumpkinDefinition.Length)].UnlitItemID;
                        pumpkin.CarvedBy = from.Name;
                        pumpkin.InvalidateProperties();

                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1154339); // *You carefully carve the pumpkin*

                        m_Item.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1154272); // That is not suitable for carving.
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BaseCarvablePumpkin : BaseLight
    {
        public override int LabelNumber => CarvedBy == null ? 1123239 : 1096937;  // Carvable Pumpkin | carved pumpkin

        [CommandProperty(AccessLevel.GameMaster)]
        public string CarvedBy { get; set; }

        public virtual PumpkinDefinition[] PumpkinDefinition { get; }

        [Constructable]
        public BaseCarvablePumpkin(int id)
            : base(id)
        {
        }

        public BaseCarvablePumpkin(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (CarvedBy == null)
                return;

            base.OnDoubleClick(from);
        }

        public override int LitItemID => PumpkinDefinition.FirstOrDefault(x => x.UnlitItemID == ItemID).LitItemID;
        public override int UnlitItemID { get { return PumpkinDefinition.FirstOrDefault(x => x.LitItemID == ItemID).UnlitItemID; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (CarvedBy != null)
                list.Add(1154350, CarvedBy); // Carved By ~1_NAME~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(CarvedBy);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            CarvedBy = reader.ReadString();
        }
    }

    public class CarvablePumpkinTall : BaseCarvablePumpkin
    {
        public override PumpkinDefinition[] PumpkinDefinition => new PumpkinDefinition[]
                {
                     new PumpkinDefinition(0x9934, 0x9931),
                     new PumpkinDefinition(0x9935, 0x9936),
                     new PumpkinDefinition(0x9939, 0x993A),
                     new PumpkinDefinition(0x993D, 0x993E),
                     new PumpkinDefinition(0x9941, 0x9942),
                     new PumpkinDefinition(0x9945, 0x9946),
                     new PumpkinDefinition(0x9949, 0x994A),
                     new PumpkinDefinition(0x9951, 0x9952),
                     new PumpkinDefinition(0xA39A, 0xA39B),
                     new PumpkinDefinition(0xA39E, 0xA39F),
                     new PumpkinDefinition(0x9BD5, 0x9BD6)
                };

        [Constructable]
        public CarvablePumpkinTall()
            : base(0x992F)
        {
            Weight = 16.0;
        }

        public CarvablePumpkinTall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CarvableGordPumpkinTall : BaseCarvablePumpkin
    {
        public override PumpkinDefinition[] PumpkinDefinition => new PumpkinDefinition[]
                {
                     new PumpkinDefinition(0x9D23, 0x9D24),
                     new PumpkinDefinition(0x9D27, 0x9D28),
                     new PumpkinDefinition(0xA139, 0xA13A),
                     new PumpkinDefinition(0xA13D, 0xA13E),
                     new PumpkinDefinition(0xA141, 0xA142),
                     new PumpkinDefinition(0xA145, 0xA146),
                     new PumpkinDefinition(0xA149, 0xA14A),
                     new PumpkinDefinition(0xA14D, 0xA14E),
                     new PumpkinDefinition(0xA151, 0xA152),
                     new PumpkinDefinition(0xA155, 0xA156),
                     new PumpkinDefinition(0xA159, 0xA15A),
                     new PumpkinDefinition(0xA15D, 0xA15E),
                     new PumpkinDefinition(0xA161, 0xA162)
                };

        [Constructable]
        public CarvableGordPumpkinTall()
            : base(0x9D22)
        {
            Weight = 18.0;
        }

        public CarvableGordPumpkinTall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CarvablePlainPumpkin : BaseCarvablePumpkin
    {
        public override bool ForceShowProperties => true;

        public override PumpkinDefinition[] PumpkinDefinition => new PumpkinDefinition[]
                {
                     new PumpkinDefinition(0x9F23, 0x9F24),
                     new PumpkinDefinition(0x9F27, 0x9F28),
                     new PumpkinDefinition(0xA396, 0xA397),
                     new PumpkinDefinition(0xA5E0, 0xA5E1),
                     new PumpkinDefinition(0xA5E4, 0xA5E5),
                     new PumpkinDefinition(0xA165, 0xA166),
                     new PumpkinDefinition(0xA169, 0xA16A),
                     new PumpkinDefinition(0xA16D, 0xA16E)
                };

        [Constructable]
        public CarvablePlainPumpkin()
            : base(0x9F51)
        {
            Weight = 18.0;
        }

        public CarvablePlainPumpkin(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
