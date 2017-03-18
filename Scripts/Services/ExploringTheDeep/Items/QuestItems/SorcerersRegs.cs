using System;
using Server.Network;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Items
{
    public class PerfectBlackPearlDecor : Item
    {
        public override int LabelNumber { get { return 1154257; } } // Perfect Black Pearl

        [Constructable]
        public PerfectBlackPearlDecor()
            : base(0xF7A)
        {
            this.Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
                from.SendLocalizedMessage(500446); // That is too far away.

            if (!(from is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)from;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
            {
                Container pack = from.Backpack;
                pack.TryDropItem(from, new PerfectBlackPearl(), false);
            }
            else
            {
                from.PublicOverheadMessage(MessageType.Regular, 0x559, 1154274); // *You aren't quite sure what to do with this. If you spoke to the Salvage Master at the Sons of the Sea in Trinsic you might have a better understanding of its use...*
            }
        }

        public PerfectBlackPearlDecor(Serial serial)
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

    public class PerfectBlackPearl : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154257; } } // Perfect Black Pearl

        [Constructable]
        public PerfectBlackPearl()
            : base(0xF7A)
        {
            this.Stackable = false;
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public PerfectBlackPearl(Serial serial)
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

    public class BurstingBrimstoneDecor : Item
    {
        public override int LabelNumber { get { return 1154258; } } // Bursting Brimstone

        [Constructable]
        public BurstingBrimstoneDecor()
            : base(0xF7F)
        {
            this.Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
                from.SendLocalizedMessage(500446); // That is too far away.

            if (!(from is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)from;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
            {
                Container pack = from.Backpack;
                pack.TryDropItem(from, new BurstingBrimstone(), false);
            }
            else
            {
                from.PublicOverheadMessage(MessageType.Regular, 0x559, 1154274); // *You aren't quite sure what to do with this. If you spoke to the Salvage Master at the Sons of the Sea in Trinsic you might have a better understanding of its use...*
            }
        }

        public BurstingBrimstoneDecor(Serial serial)
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

    public class BurstingBrimstone : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154258; } } // Bursting Brimstone

        [Constructable]
        public BurstingBrimstone()
            : base(0xF7F)
        {
            this.Stackable = false;
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public BurstingBrimstone(Serial serial)
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

    public class BrightDaemonBloodDecor : Item
    {
        public override int LabelNumber { get { return 1154259; } } // Bright Daemon Blood

        [Constructable]
        public BrightDaemonBloodDecor()
            : base(0xF7D)
        {
            this.Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
                from.SendLocalizedMessage(500446); // That is too far away.

            if (!(from is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)from;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
            {
                Container pack = from.Backpack;
                pack.TryDropItem(from, new BrightDaemonBlood(), false);
            }
            else
            {
                from.PublicOverheadMessage(MessageType.Regular, 0x559, 1154274); // *You aren't quite sure what to do with this. If you spoke to the Salvage Master at the Sons of the Sea in Trinsic you might have a better understanding of its use...*
            }
        }

        public BrightDaemonBloodDecor(Serial serial)
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

    public class BrightDaemonBlood : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154259; } } // Bright Daemon Blood

        [Constructable]
        public BrightDaemonBlood()
            : base(0xF7D)
        {
            this.Stackable = false;
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public BrightDaemonBlood(Serial serial)
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

    public class MightyMandrakeDecor : Item
    {
        public override int LabelNumber { get { return 1154260; } } // Mighty Mandrake

        [Constructable]
        public MightyMandrakeDecor()
            : base(0xF86)
        {
            this.Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
                from.SendLocalizedMessage(500446); // That is too far away.

            if (!(from is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)from;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
            {
                Container pack = from.Backpack;
                pack.TryDropItem(from, new MightyMandrake(), false);
            }
            else
            {
                from.PublicOverheadMessage(MessageType.Regular, 0x559, 1154274); // *You aren't quite sure what to do with this. If you spoke to the Salvage Master at the Sons of the Sea in Trinsic you might have a better understanding of its use...*
            }
        }

        public MightyMandrakeDecor(Serial serial)
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

    public class MightyMandrake : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154260; } } // Mighty Mandrake

        [Constructable]
        public MightyMandrake()
            : base(0xF86)
        {
            this.Stackable = false;
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public MightyMandrake(Serial serial)
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

    public class BurlyBoneDecor : Item
    {
        public override int LabelNumber { get { return 1154261; } } // Burly Bone

        [Constructable]
        public BurlyBoneDecor()
            : base(0xF7E)
        {
            this.Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 2))
                from.SendLocalizedMessage(500446); // That is too far away.

            if (!(from is PlayerMobile))
                return;

            PlayerMobile pm = (PlayerMobile)from;

            if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.Sorcerers)
            {
                Container pack = from.Backpack;
                pack.TryDropItem(from, new BurlyBone(), false);
            }
            else
            {
                from.PublicOverheadMessage(MessageType.Regular, 0x559, 1154274); // *You aren't quite sure what to do with this. If you spoke to the Salvage Master at the Sons of the Sea in Trinsic you might have a better understanding of its use...*
            }
        }

        public BurlyBoneDecor(Serial serial)
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

    public class BurlyBone : BaseDecayingItem
    {
        public override int LabelNumber { get { return 1154261; } } // Burly Bone

        [Constructable]
        public BurlyBone()
            : base(0xF7E)
        {
            this.Stackable = false;
            this.LootType = LootType.Blessed;
            this.Weight = 1;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1072351); // Quest Item
        }

        public override int Lifespan { get { return 3600; } }
        public override bool UseSeconds { get { return false; } }

        public BurlyBone(Serial serial)
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