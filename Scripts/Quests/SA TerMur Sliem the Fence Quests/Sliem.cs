/*                                                             .---.
/  .  \
|\_/|   |
|   |  /|
.----------------------------------------------------------------' |
/  .-.                                                              |
|  /   \         Contribute To The Orbsydia SA Project               |
| |\_.  |                                                            |
|\|  | /|                        By Lotar84                          |
| `---' |                                                            |
|       |       (Orbanised by Orb SA Core Development Team)          | 
|       |                                                           /
|       |----------------------------------------------------------'
\       |
\     /
`---'
*/
using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Sliem : MondainQuester
    {
        [Constructable]
        public Sliem()
            : base("Sliem", "the Fence")
        {
        }

        public Sliem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(UnusualGoods)
                };
            }
        }
        public override void InitBody()
        {
            this.Race = Race.Gargoyle;
            this.InitStats(100, 100, 25);
            this.Female = false;
            this.Body = 666;
            this.HairItemID = 16987;
            this.HairHue = 1801;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());

            this.AddItem(new GargishClothChest(Utility.RandomNeutralHue()));
            this.AddItem(new GargishClothKilt(Utility.RandomNeutralHue()));
            this.AddItem(new GargishClothLegs(Utility.RandomNeutralHue()));
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