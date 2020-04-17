/*
snicker7
Released: 03/26/06
*/

namespace Server.Mobiles
{
    public class GMEthereal : EtherealMount
    {
        public override int FollowerSlots => 0;

        private static readonly EtherealInfo[] EthyItemTypes = new EtherealInfo[]
        {
            new EtherealInfo(0x20DD, 0x3EAA), //Horse
            new EtherealInfo(0x20F6, 0x3EAB), //Llama
            new EtherealInfo(0x2135, 0x3EAC), //Ostard
            new EtherealInfo(8501, 16035), //DesertOstard
            new EtherealInfo(8502, 16036), //FrenziedOstard
            new EtherealInfo(0x2615, 0x3E9A), //Ridgeback
            new EtherealInfo(0x25CE, 0x3E9B), //Unicorn
            new EtherealInfo(0x260F, 0x3E97), //Beetle
            new EtherealInfo(0x25A0, 0x3E9C), //Kirin
            new EtherealInfo(0x2619, 0x3E98), //SwampDragon
            new EtherealInfo(9751, 16059), //SkeletalMount
            new EtherealInfo(10090, 16020), //Hiryu
            new EtherealInfo(11676, 16018), //ChargerOfTheFallen
            new EtherealInfo(9658, 16051), //SeaHorse
            new EtherealInfo(11669, 16016), //Chimera
            new EtherealInfo(11670, 16017), //CuSidhe
            new EtherealInfo(8417, 16069), //PolarBear
            new EtherealInfo(0x46f8, 0x3EC6)
        };
        private EtherealTypes m_EthyType;
        [Constructable]
        public GMEthereal()
            : this(EtherealTypes.Horse)
        {
        }

        [Constructable]
        public GMEthereal(EtherealTypes type)
            : base(0, 0, 0)
        {
            EthyType = type;
            LootType = LootType.Blessed;
            Name = "Staff Ethereal Steed";
        }

        public GMEthereal(Serial serial)
            : base(serial)
        {
        }

        public enum EtherealTypes
        {
            Horse,
            Llama,
            Ostard,
            OstardDesert,
            OstardFrenzied,
            Ridgeback,
            Unicorn,
            Beetle,
            Kirin,
            SwampDragon,
            SkeletalHorse,
            Hiryu,
            ChargerOfTheFallen,
            SeaHorse,
            Chimera,
            CuSidhe,
            PolarBear,
            Boura
        }

        [CommandProperty(AccessLevel.Counselor)]
        public EtherealTypes EthyType
        {
            get
            {
                return m_EthyType;
            }
            set
            {
                if ((int)value > EthyItemTypes.Length)
                    return;
                m_EthyType = value;

                TransparentMountedID = EthyItemTypes[(int)value].MountedID;
                NonTransparentMountedID = TransparentMountedID;
                StatueID = EthyItemTypes[(int)value].RegularID;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.IsStaff())
            {
                if (from.Mounted)
                    from.SendLocalizedMessage(1005583); // Please dismount first.
                else if (from.Race == Race.Gargoyle)
                    from.SendLocalizedMessage(1112281); // gargs can't mount
                else if (from.HasTrade)
                    from.SendLocalizedMessage(1042317, "", 0x41); // You may not ride at this time
                else if (Multis.DesignContext.Check(from))
                {
                    if (!Deleted && Rider == null && IsChildOf(from.Backpack))
                    {
                        Rider = from;
                        if (MountedID == 16051)
                            Rider.CanSwim = true;
                    }
                }
            }
            else
            {
                from.SendMessage("This item is to only be used by staff members.");
                Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
            writer.Write((int)m_EthyType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            EthyType = (EtherealTypes)reader.ReadInt();
        }

        public struct EtherealInfo
        {
            public int RegularID;
            public int MountedID;
            public EtherealInfo(int id, int mid)
            {
                RegularID = id;
                MountedID = mid;
            }
        }
    }

    public class GMEthVirtual : EtherealMount
    {
        public GMEthVirtual(int id, int mid)
            : base(id, mid, 0)
        {
        }

        public GMEthVirtual(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            Delete();
        }
    }
}
