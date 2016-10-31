using System;

namespace Server.Items
{
    public class ExodusChest : LockableContainer
    {
        [Constructable]
        public ExodusChest() : base(0x2DF3)
        {
            this.Name = "Exodus Chest";
            this.Locked = true;
            this.LockLevel = 90;
            this.RequiredSkill = 90;
            this.Weight = 255.0;
            this.Hue = 0xA92;

            switch (Utility.RandomMinMax(0, 4))  // modify as necessary
            {
                case 0: TrapType = TrapType.None; break;
                case 1: TrapType = TrapType.MagicTrap; break;
                case 2: TrapType = TrapType.DartTrap; break;
                case 3: TrapType = TrapType.PoisonTrap; break;
                case 4: TrapType = TrapType.ExplosionTrap; break;
            }

            this.TrapPower =  100;

            this.DropItem(new Gold (2000));
            this.DropItem(new Diamond(5));
            this.DropItem(new MagicalResidue(3));

            if (Utility.RandomDouble() < 0.8)
            {
                switch (Utility.Random(4))
                {
                    case 0:
                        DropItem(new ExodusSummoningRite());
                        break;
                    case 1:
                        DropItem(new ExodusSacrificalDagger());
                        break;
                    case 2:
                        DropItem(new RobeofRite());
                        break;
                    case 3:
                        DropItem(new ExodusSummoningAlter());
                        break;
                }
            }
        }

        public ExodusChest(Serial serial) : base(serial)
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