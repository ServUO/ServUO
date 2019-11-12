using System;
using System.Linq;
using Server;
using Server.Network;
using Server.Targeting;

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
        public override int LabelNumber { get { return 1154271; } } // Jack O' Lantern Carving Kit

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

                    from.PlaySound(0x249);

                    pumpkin.ItemID = pumpkin.PumpkinDefinition[Utility.Random(pumpkin.PumpkinDefinition.Length)].UnlitItemID;
                    pumpkin.CarvedBy = from.Name;

                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1154339); // *You carefully carve the pumpkin*

                    m_Item.Delete();
                    
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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BaseCarvablePumpkin : BaseLight
    {
        public override int LabelNumber { get { return CarvedBy == null ? 1123239 : 1096937; } } // Carvable Pumpkin | carved pumpkin

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

        public override int LitItemID { get { return PumpkinDefinition.FirstOrDefault(x => x.UnlitItemID == ItemID).LitItemID; } }
        public override int UnlitItemID { get { return PumpkinDefinition.FirstOrDefault(x => x.LitItemID == ItemID).UnlitItemID; ; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (CarvedBy != null)
                list.Add(1154350, CarvedBy); // Carved By ~1_NAME~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

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
        public override PumpkinDefinition[] PumpkinDefinition
        {
            get
            {
                return new PumpkinDefinition[]
                {
                     new PumpkinDefinition(0x9934, 0x9931),
                     new PumpkinDefinition(0x9935, 0x9936),
                     new PumpkinDefinition(0x9939, 0x993A),
                     new PumpkinDefinition(0x993D, 0x993E),
                     new PumpkinDefinition(0x9941, 0x9942),
                     new PumpkinDefinition(0x9945, 0x9946),
                     new PumpkinDefinition(0x9949, 0x994A),
                     new PumpkinDefinition(0x9951, 0x9952),
                };
            }
        }

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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CarvableGordPumpkinTall : BaseCarvablePumpkin
    {
        public override PumpkinDefinition[] PumpkinDefinition
        {
            get
            {
                return new PumpkinDefinition[]
                {
                     new PumpkinDefinition(0x9D23, 0x9D24),
                     new PumpkinDefinition(0x9D27, 0x9D28),
                };
            }
        }

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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CarvablePlainPumpkin : BaseCarvablePumpkin
    {
        public override PumpkinDefinition[] PumpkinDefinition
        {
            get
            {
                return new PumpkinDefinition[]
                {
                     new PumpkinDefinition(0x9F23, 0x9F24),
                     new PumpkinDefinition(0x9F27, 0x9F28),
                };
            }
        }

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
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
