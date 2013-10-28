using System;

namespace Server.Items
{
    public class GaramonsBook1 : BrownBook
    {
        public static readonly BookContent Content = new BookContent(
            "A Journal", "Tyball",
            new BookPageInfo(
                "He speaks to me...",
                "I can hear him. The",
                "beast... He knows of",
                "our plan. But how?",
                "Such wonders, such",
                "powers and such wealth",
                "he promises. But at what"),
            new BookPageInfo(
                "cost? That it could ask",
                "me to sacrifice my",
                "brother I cannot. I will",
                "not. No knowledge, no",
                "greatness could warrant",
                "such infamy.",
                "But the seed of a doubt"),
            new BookPageInfo(
                "is gnawing at me. Did",
                "the Slasher make the",
                "same offer to my",
                "brother? Is he playing",
                "us against the other?",
                "Would Garamon even enter",
                "tain the thought? No, not",
                "my virtuous brother if he"),
            new BookPageInfo(
                "did, I would need to",
                "strike first, only to",
                "protect myself of course.",
                "I must banish the doubts",
                "from my mind. They are",
                "like poison. We cannot let",
                "this fiend divide us."));
        [Constructable]
        public GaramonsBook1()
            : base(false)
        {
        }

        public GaramonsBook1(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent
        {
            get
            {
                return Content;
            }
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