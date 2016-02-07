using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{ 
    public class BrigandCamp : BaseCamp
    {
        private Mobile m_Prisoner;
        [Constructable]
        public BrigandCamp()
            : base(0x10EE)// dummy garbage at center
        {
        }

        public BrigandCamp(Serial serial)
            : base(serial)
        {
        }

        public virtual Mobile Brigands
        {
            get
            {
                return new Brigand();
            }
        }
        public virtual Mobile Executioners
        {
            get
            {
                return new Executioner();
            }
        }
        public override void AddComponents()
        {
            BaseCreature bc;
            //BaseEscortable be;

            this.Visible = false;
            this.DecayDelay = TimeSpan.FromMinutes(5.0);

            this.AddItem(new Static(0x10ee), 0, 0, 0);
            this.AddItem(new Static(0xfac), 0, 7, 0);
			
            switch ( Utility.Random(3) )
            {
                case 0:
                    {
                        this.AddItem(new Item(0xDE3), 0, 7, 0); // Campfire
                        this.AddItem(new Item(0x974), 0, 7, 1); // Cauldron
                        break;
                    }
                case 1:
                    {
                        this.AddItem(new Item(0x1E95), 0, 7, 1); // Rabbit on a spit
                        break;
                    }
                default:
                    {
                        this.AddItem(new Item(0x1E94), 0, 7, 1); // Chicken on a spit
                        break;
                    }
            }

            this.AddCampChests();
			
            for (int i = 0; i < 4; i ++)
            { 
                this.AddMobile(this.Brigands, 6, Utility.RandomMinMax(-7, 7), Utility.RandomMinMax(-7, 7), 0);
            }
			
            switch ( Utility.Random(2) )
            {
                case 0:
                    this.m_Prisoner = new Noble();
                    break;
                default:
                    this.m_Prisoner = new SeekerOfAdventure();
                    break;
            }
			
            //be = (BaseEscortable)m_Prisoner;
            //be.m_Captive = true;
			
            bc = (BaseCreature)this.m_Prisoner;
            bc.IsPrisoner = true;
            bc.CantWalk = true;
			
            this.m_Prisoner.YellHue = Utility.RandomList(0x57, 0x67, 0x77, 0x87, 0x117);
            this.AddMobile(this.m_Prisoner, 2, Utility.RandomMinMax(-2, 2), Utility.RandomMinMax(-2, 2), 0);
        }

        // Don't refresh decay timer
        public override void OnEnter(Mobile m)
        {
            if (m.Player && this.m_Prisoner != null && this.m_Prisoner.CantWalk)
            {
                int number;

                switch ( Utility.Random(8) )
                {
                    case 0:
                        number = 502261;
                        break; // HELP!
                    case 1:
                        number = 502262;
                        break; // Help me!
                    case 2:
                        number = 502263;
                        break; // Canst thou aid me?!
                    case 3:
                        number = 502264;
                        break; // Help a poor prisoner!
                    case 4:
                        number = 502265;
                        break; // Help! Please!
                    case 5:
                        number = 502266;
                        break; // Aaah! Help me!
                    case 6:
                        number = 502267;
                        break; // Go and get some help!
                    default:
                        number = 502268;
                        break; // Quickly, I beg thee! Unlock my chains! If thou dost look at me close thou canst see them.	
                }
                this.m_Prisoner.Yell(number);
            }
        }

        // Don't refresh decay timer
        public override void OnExit(Mobile m)
        {
        }

        public override void AddItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            if (item != null)
                item.Movable = false;
				
            base.AddItem(item, xOffset, yOffset, zOffset);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Prisoner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Prisoner = reader.ReadMobile();
                        break;
                    }
            }
        }

        private void AddCampChests()
        {
            LockableContainer chest = null;

            switch (Utility.Random(3))
            {
                case 0:
                    chest = new MetalChest();
                    break;
                case 1:
                    chest = new MetalGoldenChest();
                    break;
                default:
                    chest = new WoodenChest();
                    break;
            }

            chest.LiftOverride = true;

            TreasureMapChest.Fill(chest, 1);

            this.AddItem(chest, -2, -2, 0);

            LockableContainer crates = null;

            switch (Utility.Random(4))
            {
                case 0:
                    crates = new SmallCrate();
                    break;
                case 1:
                    crates = new MediumCrate();
                    break;
                case 2:
                    crates = new LargeCrate();
                    break;
                default:
                    crates = new LockableBarrel();
                    break;
            }

            crates.TrapType = TrapType.ExplosionTrap;
            crates.TrapPower = Utility.RandomMinMax(30, 40);
            crates.TrapLevel = 2;

            crates.RequiredSkill = 76;
            crates.LockLevel = 66;
            crates.MaxLockLevel = 116;
            crates.Locked = true;

            crates.DropItem(new Gold(Utility.RandomMinMax(100, 400)));
            crates.DropItem(new Arrow(10));
            crates.DropItem(new Bolt(10));

            crates.LiftOverride = true;

            if (Utility.RandomDouble() < 0.8)
            {
                switch (Utility.Random(4))
                {
                    case 0:
                        crates.DropItem(new LesserCurePotion());
                        break;
                    case 1:
                        crates.DropItem(new LesserExplosionPotion());
                        break;
                    case 2:
                        crates.DropItem(new LesserHealPotion());
                        break;
                    default:
                        crates.DropItem(new LesserPoisonPotion());
                        break;
                }
            }

            this.AddItem(crates, 2, 2, 0);
        }
    }
}