/*using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
    public class HumanBrigandCamp : BaseCamp
    { 
        [Constructable]
        public HumanBrigandCamp()
            : base(0x1F6D)
        {
        }

        public HumanBrigandCamp(Serial serial)
            : base(serial)
        {
        }

        public virtual Mobile Camper
        {
            get
            {
                return new HumanBrigand();
            }
        }
        public override void AddComponents()
        {
            AddItem(new Item(0xFAC), 0, 0, 0); // fire pit
            AddItem(new Item(0xDE3), 0, 0, 0); // camp fire
            AddItem(new Item(0x974), 0, 0, 1); // cauldron
			
            for (int i = 0; i < 2; i ++)
            {
                LockableContainer cont = null;
				
                switch ( Utility.Random(3) )
                {
                    case 0:
                        cont = new MetalChest();
                        break;
                    case 1:
                        cont = new WoodenChest();
                        break;
                    case 2:
                        cont = new SmallCrate();
                        break;
                }
				
                cont.Movable = false;
                cont.Locked = true;
				
                cont.TrapType = TrapType.ExplosionTrap;
                cont.TrapPower = Utility.RandomMinMax(30, 40);
                cont.TrapLevel = 2;
                cont.RequiredSkill = 76;
                cont.LockLevel = 66;
                cont.MaxLockLevel = 116;
                cont.DropItem(new Gold(Utility.RandomMinMax(100, 400)));
                cont.DropItem(new Arrow(10));
                cont.DropItem(new Bolt(10));
				
                if (Utility.RandomDouble() < 0.8)
                {
                    switch ( Utility.Random(4) )
                    {
                        case 0:
                            cont.DropItem(new LesserCurePotion());
                            break;
                        case 1:
                            cont.DropItem(new LesserExplosionPotion());
                            break;
                        case 2:
                            cont.DropItem(new LesserHealPotion());
                            break;
                        case 3:
                            cont.DropItem(new LesserPoisonPotion());
                            break;
                    }
                }
				
                if (Utility.RandomDouble() < 0.5)
                {
                    Item item = Loot.RandomArmorOrShieldOrWeapon();					
					
                    if (item is BaseWeapon)
                        BaseRunicTool.ApplyAttributesTo((BaseWeapon)item, false, 0, Utility.RandomMinMax(1, 5), 10, 100);
                    else if (item is BaseArmor)
                        BaseRunicTool.ApplyAttributesTo((BaseArmor)item, false, 0, Utility.RandomMinMax(1, 5), 10, 100);
						
                    cont.DropItem(item);
                }
				
                Point3D loc = GetRandomSpawnPoint(3);
				
                AddItem(cont, loc.X, loc.Y, loc.Z);
                Treasure1 = cont;
            }
			
            switch ( Utility.Random(2) )
            {
                case 0:
                    Prisoner = new Noble();
                    break;
                case 1:
                    Prisoner = new SeekerOfAdventure();
                    break;
            }
			
            for (int i = 0; i < 4; i ++)
            {
                Point3D loc = GetRandomSpawnPoint(5);				
				
                AddMobile(Camper,loc.X, loc.Y, loc.Z);
            }

            Prisoner.IsPrisoner = true;
            Prisoner.CantWalk = true;

            Point3D p = GetRandomSpawnPoint(3);	
            AddMobile(Prisoner, p.X, p.Y, p.Z);
        }

        public override void AddItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            if (item != null)
                item.Movable = false;
				
            base.AddItem(item, xOffset, yOffset, zOffset);
        }

        public virtual Point3D GetRandomSpawnPoint(int range)
        { 
            Map map = Map;

            if (map == null)
                return Location;				

            // Try 10 times to find a Spawnable location.
            for (int i = 0; i < 10; i++)
            {
                int x = Location.X + (Utility.Random((range * 2) + 1) - range);
                int y = Location.Y + (Utility.Random((range * 2) + 1) - range);
                int z = Map.GetAverageZ(x, y);

                if (Map.CanSpawnMobile(new Point2D(x, y), Z))
                    return new Point3D(x, y, Z);
                else if (Map.CanSpawnMobile(new Point2D(x, y), z))
                    return new Point3D(x, y, z);
            }

            return Location;
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (m.Player && Prisoner != null)
            {
                Prisoner.Yell(Utility.RandomMinMax(502261, 502268));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1: break;
                case 0:
                    Prisoner = reader.ReadMobile() as BaseCreature;
                    break;
            }
        }
    }
}*/