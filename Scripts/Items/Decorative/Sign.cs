using System;

namespace Server.Items
{
    public enum SignFacing
    {
        North,
        West
    }

    public enum SignType
    {
        Library,
        DarkWoodenPost,
        LightWoodenPost,
        MetalPostC,
        MetalPostB,
        MetalPostA,
        MetalPost,
        Bakery,
        Tailor,
        Tinker,
        Butcher,
        Healer,
        Mage,
        Woodworker,
        Customs,
        Inn,
        Shipwright,
        Stables,
        BarberShop,
        Bard,
        Fletcher,
        Armourer,
        Jeweler,
        Tavern,
        ReagentShop,
        Blacksmith,
        Painter,
        Provisioner,
        Bowyer,
        WoodenSign,
        BrassSign,
        ArmamentsGuild,
        ArmourersGuild,
        BlacksmithsGuild,
        WeaponsGuild,
        BardicGuild,
        BartersGuild,
        ProvisionersGuild,
        TradersGuild,
        CooksGuild,
        HealersGuild,
        MagesGuild,
        SorcerersGuild,
        IllusionistGuild,
        MinersGuild,
        ArchersGuild,
        SeamensGuild,
        FishermensGuild,
        SailorsGuild,
        ShipwrightsGuild,
        TailorsGuild,
        ThievesGuild,
        RoguesGuild,
        AssassinsGuild,
        TinkersGuild,
        WarriorsGuild,
        CavalryGuild,
        FightersGuild,
        MerchantsGuild,
        Bank,
        Theatre
    }

    public class Sign : BaseSign
    {
        [Constructable]
        public Sign()
            : base(0xB95)
        {
        }

        [Constructable]
        public Sign(SignType type, SignFacing facing)
            : base((0xB95 + (2 * (int)type)) + (int)facing)
        {
        }

        [Constructable]
        public Sign(int itemID)
            : base(itemID)
        {
        }

        public Sign(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
