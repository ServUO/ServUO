using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a horse corpse")]
    public class PackHorse : BaseCreature
    {
        [Constructable]
        public PackHorse()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a pack horse";
            this.Body = 291;
            this.BaseSoundID = 0xA8;

            this.SetStr(44, 120);
            this.SetDex(36, 55);
            this.SetInt(6, 10);

            this.SetHits(61, 80);
            this.SetStam(81, 100);
            this.SetMana(0);

            this.SetDamage(5, 11);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Cold, 20, 25);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 25.1, 30.0);
            this.SetSkill(SkillName.Tactics, 29.3, 44.0);
            this.SetSkill(SkillName.Wrestling, 29.3, 44.0);

            this.Fame = 0;
            this.Karma = 200;

            this.VirtualArmor = 16;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 11.1;

            Container pack = this.Backpack;

            if (pack != null)
                pack.Delete();

            pack = new StrongBackpack();
            pack.Movable = false;

            this.AddItem(pack);
        }

        public override int Meat
        {
            get
            {
                return 3;
            }
        }
        public override int Hides
        {
            get
            {
                return 10;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }

        public PackHorse(Serial serial)
            : base(serial)
        {
        }

        #region Pack Animal Methods
        public override bool OnBeforeDeath()
        {
            if (!base.OnBeforeDeath())
                return false;

            PackAnimal.CombineBackpacks(this);

            return true;
        }

        public override DeathMoveResult GetInventoryMoveResultFor(Item item)
        {
            return DeathMoveResult.MoveToCorpse;
        }

        public override bool IsSnoop(Mobile from)
        {
            if (PackAnimal.CheckAccess(this, from))
                return false;

            return base.IsSnoop(from);
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (this.CheckFeed(from, item))
                return true;

            if (PackAnimal.CheckAccess(this, from))
            {
                this.AddToBackpack(item);
                return true;
            }

            return base.OnDragDrop(from, item);
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            return PackAnimal.CheckAccess(this, from);
        }

        public override bool CheckNonlocalLift(Mobile from, Item item)
        {
            return PackAnimal.CheckAccess(this, from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            PackAnimal.TryPackOpen(this, from);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            PackAnimal.GetContextMenuEntries(this, from, list);
        }

        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PackAnimalBackpackEntry : ContextMenuEntry
    {
        private readonly BaseCreature m_Animal;
        private readonly Mobile m_From;

        public PackAnimalBackpackEntry(BaseCreature animal, Mobile from)
            : base(6145, 3)
        {
            this.m_Animal = animal;
            this.m_From = from;

            if (animal.IsDeadPet)
                this.Enabled = false;
        }

        public override void OnClick()
        {
            PackAnimal.TryPackOpen(this.m_Animal, this.m_From);
        }
    }

    public class PackAnimal
    {
        public static void GetContextMenuEntries(BaseCreature animal, Mobile from, List<ContextMenuEntry> list)
        {
            if (CheckAccess(animal, from))
                list.Add(new PackAnimalBackpackEntry(animal, from));
        }

        public static bool CheckAccess(BaseCreature animal, Mobile from)
        {
            if (from == animal || from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            if (from.Alive && animal.Controlled && !animal.IsDeadPet && (from == animal.ControlMaster || from == animal.SummonMaster || animal.IsPetFriend(from)))
                return true;

            return false;
        }

        public static void CombineBackpacks(BaseCreature animal)
        {
            if (Core.AOS)
                return;

            if (animal.IsBonded || animal.IsDeadPet)
                return;

            Container pack = animal.Backpack;

            if (pack != null)
            {
                Container newPack = new Backpack();

                for (int i = pack.Items.Count - 1; i >= 0; --i)
                {
                    if (i >= pack.Items.Count)
                        continue;

                    newPack.DropItem(pack.Items[i]);
                }

                pack.DropItem(newPack);
            }
        }

        public static void TryPackOpen(BaseCreature animal, Mobile from)
        {
            if (animal.IsDeadPet)
                return;

            Container item = animal.Backpack;

            if (item != null)
                from.Use(item);
        }
    }
}