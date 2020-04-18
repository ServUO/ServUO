using System;

namespace Server.Items
{
    public enum SBType
    {
        DrunkWomans,
        DrunkMans,
        Bedlam,
        SosarianSteeds,
        BlueBoar
    }

    public class StoreSingingBall : SingingBall
    {
        public override int LabelNumber => 1152323 + (int)Type;

        public SBType Type { get; set; }

        [Constructable]
        public StoreSingingBall()
            : base(0x468A)
        {
            Array values = Enum.GetValues(typeof(SBType));
            Type = (SBType)values.GetValue(Utility.Random(values.Length));

            Weight = 1.0;
            LootType = LootType.Regular;
            SetHue();
        }

        private void SetHue()
        {
            if (Type == SBType.Bedlam)
                Hue = 2611;
            else if (Type == SBType.BlueBoar)
                Hue = 2514;
            else if (Type == SBType.DrunkMans)
                Hue = 2659;
            else if (Type == SBType.DrunkWomans)
                Hue = 2596;
            else
                Hue = 2554;
        }

        public override int SoundList()
        {
            int sound = 0;

            if (Type == SBType.Bedlam)
                sound = Utility.RandomList(897, 1005, 889, 1001, 1002, 1004, 1005, 894, 893, 889, 1003);
            else if (Type == SBType.BlueBoar)
                sound = Utility.RandomList(1073, 1085, 811, 799, 1066, 794, 801, 1075, 803, 811, 1071);
            else if (Type == SBType.DrunkMans)
                sound = Utility.RandomMinMax(1049, 1098);
            else if (Type == SBType.DrunkWomans)
                sound = Utility.RandomMinMax(778, 823);
            else
                sound = Utility.RandomList(1218, 751, 629, 1226, 1305, 1246, 1019, 1508, 674, 1241);

            return sound;
        }

        public StoreSingingBall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write((int)Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Type = (SBType)reader.ReadInt();
        }
    }
}
