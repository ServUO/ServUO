using System;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseFruitTreeAddon : BaseAddon
    {
        private int m_Fruits;
        public BaseFruitTreeAddon()
            : base()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(5), new TimerCallback(Respawn));
        }

        public BaseFruitTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override abstract BaseAddonDeed Deed { get; }
        public abstract Item Fruit { get; }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Fruits
        {
            get
            {
                return this.m_Fruits;
            }
            set
            {
                if (value < 0)
                    this.m_Fruits = 0;
                else
                    this.m_Fruits = value;
            }
        }
        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(c.Location, 2))
            {
                if (this.m_Fruits > 0)
                {
                    Item fruit = this.Fruit;

                    if (fruit == null)
                        return;

                    if (!from.PlaceInBackpack(fruit))
                    {
                        fruit.Delete();
                        from.SendLocalizedMessage(501015); // There is no room in your backpack for the fruit.					
                    }
                    else
                    {
                        if (--this.m_Fruits == 0)
                            Timer.DelayCall(TimeSpan.FromMinutes(30), new TimerCallback(Respawn));

                        from.SendLocalizedMessage(501016); // You pick some fruit and put it in your backpack.
                    }
                }
                else
                    from.SendLocalizedMessage(501017); // There is no more fruit on this tree
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int)this.m_Fruits);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Fruits = reader.ReadInt();

            if (this.m_Fruits == 0)
                this.Respawn();
        }

        private void Respawn()
        {
            this.m_Fruits = Utility.RandomMinMax(1, 4);
        }
    }

    public class AppleTreeAddon : BaseFruitTreeAddon
    {
        [Constructable]
        public AppleTreeAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0xD98, 1076269), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x3124, 1076269), 0, 0, 0);
        }

        public AppleTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new AppleTreeDeed();
            }
        }
        public override Item Fruit
        {
            get
            {
                return new Apple();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class AppleTreeDeed : BaseAddonDeed
    {
        [Constructable]
        public AppleTreeDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public AppleTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new AppleTreeAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076269;
            }
        }// Apple Tree
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class PeachTreeAddon : BaseFruitTreeAddon
    {
        [Constructable]
        public PeachTreeAddon()
            : base()
        {
            this.AddComponent(new LocalizedAddonComponent(0xD9C, 1076270), 0, 0, 0);
            this.AddComponent(new LocalizedAddonComponent(0x3123, 1076270), 0, 0, 0);
        }

        public PeachTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new PeachTreeDeed();
            }
        }
        public override Item Fruit
        {
            get
            {
                return new Peach();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class PeachTreeDeed : BaseAddonDeed
    {
        [Constructable]
        public PeachTreeDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public PeachTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new PeachTreeAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1076270;
            }
        }// Peach Tree
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}