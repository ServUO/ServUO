using System;
using Server.Items;

namespace Server.Mobiles
{
    public class BrideGroom : BaseEscortable
    {
        [Constructable]
        public BrideGroom()
        {
            if (this.Female)
                this.Title = "the bride";
            else
                this.Title = "the groom";			
        }

        public BrideGroom(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach
        {
            get
            {
                return true;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }// Do not display 'the groom' when single-clicking
        public override void InitOutfit()
        {
            if (this.Female)
				
                this.AddItem(new FancyDress());
            else
                this.AddItem(new FancyShirt());

            int lowHue = GetRandomHue();

            this.AddItem(new LongPants(lowHue));

            if (this.Female)
                this.AddItem(new Shoes(lowHue));
            else
                this.AddItem(new Boots(lowHue));

            if (Utility.RandomBool())
                this.HairItemID = 0x203B;
            else
                this.HairItemID = 0x203C;

            this.HairHue = this.Race.RandomHairHue();

            this.PackGold(200, 250);
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

        private static int GetRandomHue()
        {
            switch ( Utility.Random(6) )
            {
                default:
                case 0:
                    return 0;
                case 1:
                    return Utility.RandomBlueHue();
                case 2:
                    return Utility.RandomGreenHue();
                case 3:
                    return Utility.RandomRedHue();
                case 4:
                    return Utility.RandomYellowHue();
                case 5:
                    return Utility.RandomNeutralHue();
            }
        }
    }
}