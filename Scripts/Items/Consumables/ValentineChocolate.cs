namespace Server.Items
{
    public class ValentineChocolate : Food
    {
        private static readonly int[] m_Labels = new int[]
        {
            1114827, // "Someone Likes You"
			1114834, // "You’re Sexy"
			1114836, // "You’re The Best"
			1114822, // "Be Mine"
			1114829, // "Always Together"
			1114839, // "Sweet Memories"
			1114833, // "Hot Stuff"
			1114830, // "Thinking Of You"
			1114838, // "*hug*"
			1114832, // "*wink*"
			1114826, // "You’re Sweet"
			1114818, // "Sweet Dreams"
			1114831, // "Kiss Me"
			1114825, // "Be My Valentine"
			1114835, // "Tasty!"
			1114840, // "How About A Date?"
			1114823, // "You’re Cute"
			1114841, // "Let’s Be Impulsive"
			1114821, // "Yours-4-Ever"
			1114824, // "Let’s Be Friends"
			1114837, // "Someone Loves You"
			1114828, // "True Love"
		};

        public override int LabelNumber => m_Title;

        private int m_Title, m_Label;

        [Constructable]
        public ValentineChocolate()
            : base(2538)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;

            switch (Utility.Random(3))
            {
                case 0:
                    Hue = 1125;
                    m_Title = 1079994; // Dark chocolate
                    break;
                case 1:
                    Hue = 1121;
                    m_Title = 1079995; // Milk chocolate
                    break;
                case 2:
                    Hue = 1150;
                    m_Title = 1079996; // White Chocolate
                    break;
            }

            m_Label = m_Labels[Utility.Random(m_Labels.Length)];
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(m_Label);
        }

        public ValentineChocolate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Title);
            writer.Write(m_Label);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Title = reader.ReadInt();
            m_Label = reader.ReadInt();
        }
    }
}
