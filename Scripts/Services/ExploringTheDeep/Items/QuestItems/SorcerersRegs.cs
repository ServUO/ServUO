using Server.Engines.Quests;
using Server.Mobiles;

namespace Server.Items
{
    public class PerfectBlackPearlDecor : Item
    {
        public override int LabelNumber => 1154257;  // Perfect Black Pearl

        [Constructable]
        public PerfectBlackPearlDecor()
            : base(0xF7A)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }

            Item reg = from.Backpack.FindItemByType(typeof(PerfectBlackPearl));

            if (reg == null)
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
                {
                    Container pack = from.Backpack;
                    pack.TryDropItem(from, new PerfectBlackPearl(), false);
                    from.SendLocalizedMessage(1154489); // You received a Quest Item!
                }
                else
                {
                    from.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                }
            }
            else
            {
                from.SendLocalizedMessage(1154331); // You already have one of these. You don't need another.
            }
        }

        public PerfectBlackPearlDecor(Serial serial)
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

    public class PerfectBlackPearl : BaseDecayingItem
    {
        public override int LabelNumber => 1154257;  // Perfect Black Pearl

        [Constructable]
        public PerfectBlackPearl()
            : base(0xF7A)
        {
            Stackable = false;
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 3600;
        public override bool UseSeconds => false;

        public PerfectBlackPearl(Serial serial)
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

    public class BurstingBrimstoneDecor : Item
    {
        public override int LabelNumber => 1154258;  // Bursting Brimstone

        [Constructable]
        public BurstingBrimstoneDecor()
            : base(0xF7F)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }

            Item reg = from.Backpack.FindItemByType(typeof(BurstingBrimstone));

            if (reg == null)
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
                {
                    Container pack = from.Backpack;
                    pack.TryDropItem(from, new BurstingBrimstone(), false);
                    from.SendLocalizedMessage(1154489); // You received a Quest Item!
                }
                else
                {
                    from.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                }
            }
            else
            {
                from.SendLocalizedMessage(1154331); // You already have one of these. You don't need another.
            }
        }

        public BurstingBrimstoneDecor(Serial serial)
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

    public class BurstingBrimstone : BaseDecayingItem
    {
        public override int LabelNumber => 1154258;  // Bursting Brimstone

        [Constructable]
        public BurstingBrimstone()
            : base(0xF7F)
        {
            Stackable = false;
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 3600;
        public override bool UseSeconds => false;

        public BurstingBrimstone(Serial serial)
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

    public class BrightDaemonBloodDecor : Item
    {
        public override int LabelNumber => 1154259;  // Bright Daemon Blood

        [Constructable]
        public BrightDaemonBloodDecor()
            : base(0xF7D)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }

            Item reg = from.Backpack.FindItemByType(typeof(BrightDaemonBlood));

            if (reg == null)
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
                {
                    Container pack = from.Backpack;
                    pack.TryDropItem(from, new BrightDaemonBlood(), false);
                    from.SendLocalizedMessage(1154489); // You received a Quest Item!
                }
                else
                {
                    from.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                }
            }
            else
            {
                from.SendLocalizedMessage(1154331); // You already have one of these. You don't need another.
            }
        }

        public BrightDaemonBloodDecor(Serial serial)
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

    public class BrightDaemonBlood : BaseDecayingItem
    {
        public override int LabelNumber => 1154259;  // Bright Daemon Blood

        [Constructable]
        public BrightDaemonBlood()
            : base(0xF7D)
        {
            Stackable = false;
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 3600;
        public override bool UseSeconds => false;

        public BrightDaemonBlood(Serial serial)
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

    public class MightyMandrakeDecor : Item
    {
        public override int LabelNumber => 1154260;  // Mighty Mandrake

        [Constructable]
        public MightyMandrakeDecor()
            : base(0xF86)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }

            Item reg = from.Backpack.FindItemByType(typeof(MightyMandrake));

            if (reg == null)
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
                {
                    Container pack = from.Backpack;
                    pack.TryDropItem(from, new MightyMandrake(), false);
                    from.SendLocalizedMessage(1154489); // You received a Quest Item!
                }
                else
                {
                    from.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                }
            }
            else
            {
                from.SendLocalizedMessage(1154331); // You already have one of these. You don't need another.
            }
        }

        public MightyMandrakeDecor(Serial serial)
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

    public class MightyMandrake : BaseDecayingItem
    {
        public override int LabelNumber => 1154260;  // Mighty Mandrake

        [Constructable]
        public MightyMandrake()
            : base(0xF86)
        {
            Stackable = false;
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 3600;
        public override bool UseSeconds => false;

        public MightyMandrake(Serial serial)
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

    public class BurlyBoneDecor : Item
    {
        public override int LabelNumber => 1154261;  // Burly Bone

        [Constructable]
        public BurlyBoneDecor()
            : base(0xF7E)
        {
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }

            Item reg = from.Backpack.FindItemByType(typeof(BurlyBone));

            if (reg == null)
            {
                PlayerMobile pm = (PlayerMobile)from;

                if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
                {
                    Container pack = from.Backpack;
                    pack.TryDropItem(from, new BurlyBone(), false);
                    from.SendLocalizedMessage(1154489); // You received a Quest Item!
                }
                else
                {
                    from.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                }
            }
            else
            {
                from.SendLocalizedMessage(1154331); // You already have one of these. You don't need another.
            }
        }

        public BurlyBoneDecor(Serial serial)
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

    public class BurlyBone : BaseDecayingItem
    {
        public override int LabelNumber => 1154261;  // Burly Bone

        [Constructable]
        public BurlyBone()
            : base(0xF7E)
        {
            Stackable = false;
            LootType = LootType.Blessed;
            Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan => 3600;
        public override bool UseSeconds => false;

        public BurlyBone(Serial serial)
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