/*                                                             .---.
/  .  \
|\_/|   |
|   |  /|
.----------------------------------------------------------------' |
/  .-.                                                              |
|  /   \            Contribute To The Orbsydia SA Project            |
| |\_.  |                                                            |
|\|  | /|                        By Lotar84                          |
| `---' |                                                            |
|       |         (Orbanised by Orb SA Core Development Team)        | 
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
    public class Dugan : MondainQuester
    {
        [Constructable]
        public Dugan()
            : base("Elder Dugan", "the Prospector")
        {
        }

        public Dugan(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(Missing)
                };
            }
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = false;
            this.Race = Race.Human;
            this.Body = 0x190;

            this.Hue = 0x83EA;
            this.HairItemID = 0x203C;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes());
            this.AddItem(new LeatherArms());
            this.AddItem(new LeatherChest());
            this.AddItem(new LeatherLegs());
            this.AddItem(new LeatherGloves());
            this.AddItem(new GnarledStaff());
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