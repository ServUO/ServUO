using System;
using Server.Items;

namespace Server.Mobiles
{
    public class TalkingSeekerOfAdventure : TalkingBaseEscortable
    {
        private static readonly string[] m_Dungeons = new string[]
        {
            "Covetous", "Deceit", "Despise",
            "Destard", "Hythloth", "Shame",
            "Wrong"
        };
        [Constructable]
        public TalkingSeekerOfAdventure()
        {
            this.Title = "the seeker of adventure";
        }

        public TalkingSeekerOfAdventure(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }// Do not display 'the seeker of adventure' when single-clicking
        public override string[] GetPossibleDestinations()
        {
            return m_Dungeons;
        }

        public override void InitOutfit()
        {
            if (this.Female)
                this.AddItem(new FancyDress(GetRandomHue()));
            else
                this.AddItem(new FancyShirt(GetRandomHue()));

            int lowHue = GetRandomHue();

            this.AddItem(new ShortPants(lowHue));

            if (this.Female)
                this.AddItem(new ThighBoots(lowHue));
            else
                this.AddItem(new Boots(lowHue));

            if (!this.Female)
                this.AddItem(new BodySash(lowHue));

            this.AddItem(new Cloak(GetRandomHue()));

            this.AddItem(new Longsword());

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

            this.PackGold(100, 150);
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