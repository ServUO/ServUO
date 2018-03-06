using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Neville : BaseEscort
    {
        [Constructable]
        public Neville()
            : base()
        {
            Name = "Neville Brightwhistle";
        }

        public Neville(Serial serial)
            : base(serial)
        {
        }

        public override bool InitialInnocent
        {
            get
            {
                return true;
            }
        }
        public override bool IsInvulnerable
        {
            get
            {
                return true;
            }
        }
        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(EscortToDugan)
                };
            }
        }
        public override bool CanBeDamaged()
        {
            return false;
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();

            Frozen = true;
            Direction = Direction.North;
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new Shoes(1877));
            SetWearable(new LongPants(443));
            SetWearable(new FancyShirt(1425));
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

            Frozen = true;
            Direction = Direction.North;
        }
    }
}