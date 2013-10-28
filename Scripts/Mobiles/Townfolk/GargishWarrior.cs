using System;
using Server.Items;

namespace Server.Mobiles
{
    public class GargishWarrior : BaseCreature
    {
        [Constructable]
        public GargishWarrior()
            : base(AIType.AI_Animal, FightMode.None, 10, 1, 0.2, 0.4)
        {
            this.Name = "Warrior";
            if (this.Female = Utility.RandomBool())
            {
                this.Body = 667;
                this.HairItemID = 17067;
                this.HairHue = 1762;
                this.AddItem(new FemaleGargishPlateChest());
                this.AddItem(new FemaleGargishPlateKilt());
                this.AddItem(new FemaleGargishPlateLegs());
                this.AddItem(new FemaleGargishPlateArms());
                this.AddItem(new PlateTalons());
               
                this.AddItem(new GlassSword());
            }
            else
            {
                this.Body = 666;
                this.HairItemID = 16987;
                this.HairHue = 1801;
                this.AddItem(new MaleGargishPlateChest());
                this.AddItem(new MaleGargishPlateKilt());
                this.AddItem(new MaleGargishPlateLegs());
                this.AddItem(new MaleGargishPlateArms());
                this.AddItem(new PlateTalons());
           
                this.AddItem(new GlassSword());
            }
        }

        public GargishWarrior(Serial serial)
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