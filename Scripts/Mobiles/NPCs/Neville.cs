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
    public class Neville : BaseEscort
    {
        [Constructable]
        public Neville()
            : base()
        {
            this.Name = "Neville Brightwhistle";
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
            this.InitStats(100, 100, 25);

            this.Female = false;
            this.Race = Race.Human;

            this.Hue = 0x8412;
            this.HairItemID = 0x2047;
            this.HairHue = 0x465;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x1BB));
            this.AddItem(new LongPants(0x901));
            this.AddItem(new Tunic(0x70A));
            this.AddItem(new Cloak(0x675));
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