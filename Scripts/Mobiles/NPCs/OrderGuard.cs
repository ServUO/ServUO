using System;
using Server.Guilds;
using Server.Items;

namespace Server.Mobiles
{
    public class OrderGuard : BaseShieldGuard
    {
        [Constructable]
        public OrderGuard()
        {
        }

        public OrderGuard(Serial serial)
            : base(serial)
        {
        }

        public override int Keyword
        {
            get
            {
                return 0x21;
            }
        }// *order shield*
        public override BaseShield Shield
        {
            get
            {
                return new OrderShield();
            }
        }
        public override int SignupNumber
        {
            get
            {
                return 1007141;
            }
        }// Sign up with a guild of order if thou art interested.
        public override GuildType Type
        {
            get
            {
                return GuildType.Order;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return true;
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
}