using System;
using Server.Items;

namespace Server.Mobiles
{
    public class Messenger : BaseEscortable
    {
        [Constructable]
        public Messenger()
        {
            this.Title = "the messenger";
        }

        public Messenger(Serial serial)
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
        }// Do not display 'the messenger' when single-clicking
        public override void InitOutfit()
        {
            if (this.Female)
                this.AddItem(new PlainDress());
            else
                this.AddItem(new Shirt(GetRandomHue()));

            int lowHue = GetRandomHue();

            this.AddItem(new ShortPants(lowHue));

            if (this.Female)
                this.AddItem(new Boots(lowHue));
            else
                this.AddItem(new Shoes(lowHue));

            //if ( !Female )
            //AddItem( new BodySash( lowHue ) );

            //AddItem( new Cloak( GetRandomHue() ) );

            //if ( !Female )
            //AddItem( new Longsword() );

            switch ( Utility.Random(4) )
            {
                case 0:
                    this.AddItem(new ShortHair(Utility.RandomHairHue()));
                    break;
                case 1:
                    this.AddItem(new TwoPigTails(Utility.RandomHairHue()));
                    break;
                case 2:
                    this.AddItem(new ReceedingHair(Utility.RandomHairHue()));
                    break;
                case 3:
                    this.AddItem(new KrisnaHair(Utility.RandomHairHue()));
                    break;
            }

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