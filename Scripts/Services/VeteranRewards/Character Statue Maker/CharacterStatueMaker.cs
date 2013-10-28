using System;
using Server.Engines.VeteranRewards;
using Server.Mobiles;

namespace Server.Items
{
    public class CharacterStatueMaker : Item, IRewardItem
    {
        private bool m_IsRewardItem;
        private StatueType m_Type;
        public CharacterStatueMaker(StatueType type)
            : base(0x32F0)
        {
            this.m_Type = type;

            this.InvalidateHue();

            this.LootType = LootType.Blessed;
            this.Weight = 5.0;
        }

        public CharacterStatueMaker(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076173;
            }
        }// Character Statue Maker
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public StatueType StatueType
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateHue();
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, new object[] { this.m_Type }))
                return;

            if (this.IsChildOf(from.Backpack))
            {
                if (!from.IsBodyMod)
                {
                    from.SendLocalizedMessage(1076194); // Select a place where you would like to put your statue.
                    from.Target = new CharacterStatueTarget(this, this.m_Type);
                }
                else
                    from.SendLocalizedMessage(1073648); // You may only proceed while in your original state...
            }
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_IsRewardItem)
                list.Add(1076222); // 6th Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_IsRewardItem);
            writer.Write((int)this.m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_IsRewardItem = reader.ReadBool();
            this.m_Type = (StatueType)reader.ReadInt();
        }

        public void InvalidateHue()
        {
            this.Hue = 0xB8F + (int)this.m_Type * 4;
        }
    }

    public class MarbleStatueMaker : CharacterStatueMaker
    {
        [Constructable]
        public MarbleStatueMaker()
            : base(StatueType.Marble)
        {
        }

        public MarbleStatueMaker(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class JadeStatueMaker : CharacterStatueMaker
    {
        [Constructable]
        public JadeStatueMaker()
            : base(StatueType.Jade)
        {
        }

        public JadeStatueMaker(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class BronzeStatueMaker : CharacterStatueMaker
    {
        [Constructable]
        public BronzeStatueMaker()
            : base(StatueType.Bronze)
        {
        }

        public BronzeStatueMaker(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}