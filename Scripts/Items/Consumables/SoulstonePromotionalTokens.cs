namespace Server.Items
{
    public enum SoulstoneType
    {
        Green,
        Blue,
        Red,
        Violet,
        Orange,
        Yellow,
        White,
        Black
    }

    public class SoulstoneFragmentToken : PromotionalToken
    {
        [Constructable]
        public SoulstoneFragmentToken()
            : base()
        {
        }

        public SoulstoneFragmentToken(Serial serial)
            : base(serial)
        {
        }

        public override TextDefinition ItemGumpName => 1070999;// <center>Soulstone Fragment</center>
        public override TextDefinition ItemName => 1071000;//soulstone fragment
        public override TextDefinition ItemReceiveMessage => 1070976;// A soulstone fragment has been created in your bank box.
        public override Item CreateItemFor(Mobile from)
        {
            if (from != null && from.Account != null)

                return new SoulstoneFragment(from.Account.ToString());
            else
                return null;
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

    public class SoulstoneToken : PromotionalToken
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public SoulstoneType Type { get; set; }

        [Constructable]
        public SoulstoneToken()
            : this(SoulstoneType.Blue)
        {
        }

        [Constructable]
        public SoulstoneToken(SoulstoneType type)
            : base()
        {
            Type = type;
        }

        public SoulstoneToken(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                switch (Type)
                {
                    default: return 1070997;
                    case SoulstoneType.Green: return 1078831;
                    case SoulstoneType.Blue: return 1078832;
                    case SoulstoneType.Red: return 1078833;
                    case SoulstoneType.Violet: return 1158404;
                    case SoulstoneType.Orange: return 1158869;
                    case SoulstoneType.Yellow: return 1158870;
                    case SoulstoneType.White: return 1158868;
                    case SoulstoneType.Black: return 1158867;
                }
            }
        }

        public override TextDefinition ItemGumpName => 1030903;// <center>Soulstone</center>
        public override TextDefinition ItemName
        {
            get
            {
                switch (Type)
                {
                    default: return 1030899;
                    case SoulstoneType.Green: return 1078834;
                    case SoulstoneType.Blue: return 1078835;
                    case SoulstoneType.Red: return 1078836;
                    case SoulstoneType.Violet: return 1158404;
                    case SoulstoneType.Orange: return 1158869;
                    case SoulstoneType.Yellow: return 1158870;
                    case SoulstoneType.White: return 1158868;
                    case SoulstoneType.Black: return 1158867;
                }
            }
        }//soulstone
        public override TextDefinition ItemReceiveMessage => 1070743;// A soulstone has been created in your bank box.
        public override Item CreateItemFor(Mobile from)
        {
            if (from != null && from.Account != null)
            {
                switch (Type)
                {
                    case SoulstoneType.Green: return new SoulStone(from.Account.ToString()) { LastUserName = from.RawName };
                    case SoulstoneType.Blue: return new BlueSoulstone(from.Account.ToString()) { LastUserName = from.RawName };
                    case SoulstoneType.Red: return new RedSoulstone(from.Account.ToString()) { LastUserName = from.RawName };
                    case SoulstoneType.Violet: return new VioletSoulstone(from.Account.ToString()) { LastUserName = from.RawName };
                    case SoulstoneType.Orange: return new OrangeSoulstone(from.Account.ToString()) { LastUserName = from.RawName };
                    case SoulstoneType.Yellow: return new YellowSoulstone(from.Account.ToString()) { LastUserName = from.RawName };
                    case SoulstoneType.White: return new WhiteSoulstone(from.Account.ToString()) { LastUserName = from.RawName };
                    case SoulstoneType.Black: return new BlackSoulstone(from.Account.ToString()) { LastUserName = from.RawName };
                }
            }

            return null;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write((int)Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Type = (SoulstoneType)reader.ReadInt();
                    break;
            }
        }
    }
}
