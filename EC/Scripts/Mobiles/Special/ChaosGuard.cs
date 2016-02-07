using System;
using Server.Guilds;
using Server.Items;

namespace Server.Mobiles
{
    public class ChaosGuard : BaseShieldGuard
    {
        [Constructable]
        public ChaosGuard()
        {
        }

        public ChaosGuard(Serial serial)
            : base(serial)
        {
        }

        public override int Keyword
        {
            get
            {
                return 0x22;
            }
        }// *chaos shield*
        public override BaseShield Shield
        {
            get
            {
                return new ChaosShield();
            }
        }
        public override int SignupNumber
        {
            get
            {
                return 1007140;
            }
        }// Sign up with a guild of chaos if thou art interested.
        public override GuildType Type
        {
            get
            {
                return GuildType.Chaos;
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