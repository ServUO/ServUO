using System;
using Server.Items;

namespace Server.Mobiles
{
    public class GargishRefugee : BaseCreature
    {
        [Constructable]
        public GargishRefugee()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        {
            this.Name = "Refugee";
            if (this.Female = Utility.RandomBool())
            {
                this.Body = 667;
                this.HairItemID = 17067;
                this.HairHue = 1762;
                this.AddItem(new GargishClothChest());
                this.AddItem(new GargishClothKilt(Utility.RandomNeutralHue()));
            }
            else
            {
                this.Body = 666;
                this.HairItemID = 16987;
                this.HairHue = 1801;
                this.AddItem(new GargishClothChest());
                this.AddItem(new GargishClothKilt());
                this.AddItem(new GargishClothLegs(Utility.RandomNeutralHue()));
            }
        }

        public GargishRefugee(Serial serial)
            : base(serial)
        {
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