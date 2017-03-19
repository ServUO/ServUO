using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class WrongPrisoner : BaseEscort
    {
        [Constructable]
        public WrongPrisoner()
            : base()
        {
            this.Title = "the prisoner";
            this.IsPrisoner = true;
            this.Female = false;
            this.Body = 0x190;
            this.Hue = 33802;
            this.Name = NameList.RandomName("male");

            this.SetWearable(new PlateChest());
            this.SetWearable(new PlateArms());
            this.SetWearable(new PlateGloves());
            this.SetWearable(new PlateLegs());
        }

        public WrongPrisoner(Serial serial)
            : base(serial)
        {
        }

        public override void InitOutfit() { }

        public override bool IsInvulnerable { get { return !this.Controlled; } }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(EscortToWrongEntrance)
                };
            }
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
