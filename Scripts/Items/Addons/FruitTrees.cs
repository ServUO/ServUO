using Server.Network;
using System;

namespace Server.Items
{
    public abstract class BaseFruitTreeAddon : BaseAddon
    {
        private int m_Fruit;

        public BaseFruitTreeAddon()
            : base()
        {
            Respawn();
        }

        public BaseFruitTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override abstract BaseAddonDeed Deed { get; }
        public abstract Item FruitItem { get; }

        public virtual int MaxFruit => 10;
        public virtual TimeSpan SpawnTime => TimeSpan.FromHours(24);

        [CommandProperty(AccessLevel.GameMaster)]
        public int Fruit
        {
            get
            {
                return m_Fruit;
            }
            set
            {
                m_Fruit = Math.Max(0, Math.Min(MaxFruit, value));
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSpawn { get; set; }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            if (from.InRange(c.Location, 2))
            {
                if (m_Fruit > 0)
                {
                    Item fruit = FruitItem;

                    if (fruit == null)
                        return;

                    if (!from.PlaceInBackpack(fruit))
                    {
                        fruit.Delete();
                        from.SendLocalizedMessage(501015); // There is no room in your backpack for the fruit.					
                    }
                    else
                    {
                        Fruit--;
                        from.SendLocalizedMessage(501016); // You pick some fruit and put it in your backpack.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(501017); // There is no more fruit on this tree
                }
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            if (NextSpawn < DateTime.UtcNow)
            {
                Respawn();
            }

            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version

            writer.Write(NextSpawn);
            writer.Write(m_Fruit);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    NextSpawn = reader.ReadDateTime();
                    goto case 0;
                case 0:
                    m_Fruit = reader.ReadInt();
                    break;
            }
        }

        private void Respawn()
        {
            Fruit++;

            NextSpawn = DateTime.UtcNow + SpawnTime;
        }
    }

    public class AppleTreeAddon : BaseFruitTreeAddon
    {
        [Constructable]
        public AppleTreeAddon()
            : base()
        {
            AddComponent(new LocalizedAddonComponent(0xD98, 1076269), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x3124, 1076269), 0, 0, 0);
        }

        public AppleTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new AppleTreeDeed();
        public override Item FruitItem => new Apple();
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
            LootType = LootType.Blessed;
        }

        public AppleTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new AppleTreeAddon();
        public override int LabelNumber => 1076269;// Apple Tree
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
            AddComponent(new LocalizedAddonComponent(0xD9C, 1076270), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x3123, 1076270), 0, 0, 0);
        }

        public PeachTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new PeachTreeDeed();
        public override Item FruitItem => new Peach();
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
            LootType = LootType.Blessed;
        }

        public PeachTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new PeachTreeAddon();
        public override int LabelNumber => 1076270;// Peach Tree
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
