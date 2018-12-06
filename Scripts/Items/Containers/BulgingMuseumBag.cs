using System;

using Server.SkillHandlers;

namespace Server.Items 
{
    public class BulgingMuseumBag : BaseRewardBag
    {
        [Constructable]
        public BulgingMuseumBag()
        {
            DropItem(new Gold(10000));

            for (int i = 0; i < 6; i++)
            {
                switch (Utility.Random(9))
                {
                    case 0:
                        DropItem(new Amber());
                        break;
                    case 1:
                        DropItem(new Amethyst());
                        break;
                    case 2:
                        DropItem(new Citrine());
                        break;
                    case 3:
                        DropItem(new Ruby());
                        break;
                    case 4:
                        DropItem(new Emerald());
                        break;
                    case 5:
                        DropItem(new Diamond());
                        break;
                    case 6:
                        DropItem(new Sapphire());
                        break;
                    case 7:
                        DropItem(new StarSapphire());
                        break;
                    case 8:
                        DropItem(new Tourmaline());
                        break;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                var type = Imbuing.IngredTypes[Utility.Random(Imbuing.IngredTypes.Length)];

                DropItem(Loot.Construct(type));
            }

            // TODO: Book http://www.uoguide.com/Ter_Mur_Quest_Reward_Books
        }

        public BulgingMuseumBag(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112995;
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