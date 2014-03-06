using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class TreasureMapChest : LockableContainer
    {
        private static readonly Type[] m_Artifacts = new Type[]
        {
            typeof(CandelabraOfSouls), typeof(GoldBricks), typeof(PhillipsWoodenSteed),
            typeof(ArcticDeathDealer), typeof(BlazeOfDeath), typeof(BurglarsBandana),
            typeof(CavortingClub), typeof(DreadPirateHat),
            typeof(EnchantedTitanLegBone), typeof(GwennosHarp), typeof(IolosLute),
            typeof(LunaLance), typeof(NightsKiss), typeof(NoxRangersHeavyCrossbow),
            typeof(PolarBearMask), typeof(VioletCourage), typeof(HeartOfTheLion),
            typeof(ColdBlood), typeof(AlchemistsBauble), typeof(CaptainQuacklebushsCutlass),
			typeof(ForgedPardon), typeof(ShieldOfInvulnerability), typeof(AncientShipModelOfTheHMSCape),
			typeof(AdmiralHeartyRum)
        };
        private int m_Level;
        private DateTime m_DeleteTime;
        private Timer m_Timer;
        private Mobile m_Owner;
        private bool m_Temporary;
        private List<Mobile> m_Guardians;
        private List<Item> m_Lifted = new List<Item>();
        [Constructable]
        public TreasureMapChest(int level)
            : this(null, level, false)
        {
        }

        public TreasureMapChest(Mobile owner, int level, bool temporary)
            : base(0xE40)
        {
            this.m_Owner = owner;
            this.m_Level = level;
            this.m_DeleteTime = DateTime.UtcNow + TimeSpan.FromHours(3.0);

            this.m_Temporary = temporary;
            this.m_Guardians = new List<Mobile>();

            this.m_Timer = new DeleteTimer(this, this.m_DeleteTime);
            this.m_Timer.Start();

            Fill(this, level);
        }

        public TreasureMapChest(Serial serial)
            : base(serial)
        {
        }

        public static Type[] Artifacts
        {
            get
            {
                return m_Artifacts;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 3000541;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeleteTime
        {
            get
            {
                return this.m_DeleteTime;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Temporary
        {
            get
            {
                return this.m_Temporary;
            }
            set
            {
                this.m_Temporary = value;
            }
        }
        public List<Mobile> Guardians
        {
            get
            {
                return this.m_Guardians;
            }
        }
        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
        }
        public static void Fill(LockableContainer cont, int level)
        {
            cont.Movable = false;
            cont.Locked = true;
            int numberItems;
			
            if (level == 0)
            {
                cont.LockLevel = 0; // Can't be unlocked

                cont.DropItem(new Gold(Utility.RandomMinMax(50, 100)));

                if (Utility.RandomDouble() < 0.75)
                    cont.DropItem(new TreasureMap(0, Map.Trammel));
            }
            else
            {
                cont.TrapType = TrapType.ExplosionTrap;
                cont.TrapPower = level * 25;
                cont.TrapLevel = level;

                switch ( level )
                {
                    case 1:
                        cont.RequiredSkill = 36;
                        break;
                    case 2:
                        cont.RequiredSkill = 76;
                        break;
                    case 3:
                        cont.RequiredSkill = 84;
                        break;
                    case 4:
                        cont.RequiredSkill = 92;
                        break;
                    case 5:
                        cont.RequiredSkill = 100;
                        break;
                    case 6:
                        cont.RequiredSkill = 100;
                        break;
                }

                cont.LockLevel = cont.RequiredSkill - 10;
                cont.MaxLockLevel = cont.RequiredSkill + 40;
				
                //Publish 67 gold change
                //if ( Core.SA )
                //	cont.DropItem( new Gold( level * 5000 ) );
                //else					
                cont.DropItem(new Gold(level * 1000));

                for (int i = 0; i < level * 5; ++i)
                    cont.DropItem(Loot.RandomScroll(0, 63, SpellbookType.Regular));

                if (Core.SE)
                {
                    switch ( level )
                    {
                        case 1:
                            numberItems = 5;
                            break;
                        case 2:
                            numberItems = 10;
                            break;
                        case 3:
                            numberItems = 15;
                            break;
                        case 4:
                            numberItems = 38;
                            break;
                        case 5:
                            numberItems = 50;
                            break;
                        case 6:
                            numberItems = 60;
                            break;
                        default:
                            numberItems = 0;
                            break;
                    }
                }
                else
                    numberItems = level * 6;
				
                for (int i = 0; i < numberItems; ++i)
                {
                    Item item;

                    if (Core.AOS)
                        item = Loot.RandomArmorOrShieldOrWeaponOrJewelry();
                    else
                        item = Loot.RandomArmorOrShieldOrWeapon();

                    if (item is BaseWeapon)
                    {
                        BaseWeapon weapon = (BaseWeapon)item;

                        if (Core.AOS)
                        {
                            int attributeCount;
                            int min, max;

                            GetRandomAOSStats(out attributeCount, out min, out max);

                            BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);
                        }
                        else
                        {
                            weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(6);
                            weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                            weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                        }

                        cont.DropItem(item);
                    }
                    else if (item is BaseArmor)
                    {
                        BaseArmor armor = (BaseArmor)item;

                        if (Core.AOS)
                        {
                            int attributeCount;
                            int min, max;

                            GetRandomAOSStats(out attributeCount, out min, out max);

                            BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);
                        }
                        else
                        {
                            armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(6);
                            armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                        }

                        cont.DropItem(item);
                    }
                    else if (item is BaseHat)
                    {
                        BaseHat hat = (BaseHat)item;

                        if (Core.AOS)
                        {
                            int attributeCount;
                            int min, max;

                            GetRandomAOSStats(out attributeCount, out min, out max);

                            BaseRunicTool.ApplyAttributesTo(hat, attributeCount, min, max);
                        }

                        cont.DropItem(item);
                    }
                    else if (item is BaseJewel)
                    {
                        int attributeCount;
                        int min, max;

                        GetRandomAOSStats(out attributeCount, out min, out max);

                        BaseRunicTool.ApplyAttributesTo((BaseJewel)item, attributeCount, min, max);

                        cont.DropItem(item);
                    }
                }
            }

            int reagents;
            if (level == 0)
                reagents = 12;
            else
                reagents = level * 3;

            for (int i = 0; i < reagents; i++)
            {
                Item item = Loot.RandomPossibleReagent();
                item.Amount = Utility.RandomMinMax(40, 60);
                cont.DropItem(item);
            }

            int gems;
            if (level == 0)
                gems = 2;
            else
                gems = level * 3;

            for (int i = 0; i < gems; i++)
            {
                Item item = Loot.RandomGem();
                cont.DropItem(item);
            }

            if (level == 6 && Core.AOS)
                cont.DropItem((Item)Activator.CreateInstance(m_Artifacts[Utility.Random(m_Artifacts.Length)]));
        }

        public override bool CheckLocked(Mobile from)
        {
            if (!this.Locked)
                return false;

            if (this.Level == 0 && from.AccessLevel < AccessLevel.GameMaster)
            {
                foreach (Mobile m in this.Guardians)
                {
                    if (m.Alive)
                    {
                        from.SendLocalizedMessage(1046448); // You must first kill the guardians before you may open this chest.
                        return true;
                    }
                }

                this.LockPick(from);
                return false;
            }
            else
            {
                return base.CheckLocked(from);
            }
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            return this.CheckLoot(from, item != this) && base.CheckItemUse(from, item);
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            return this.CheckLoot(from, true) && base.CheckLift(from, item, ref reject);
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            bool notYetLifted = !this.m_Lifted.Contains(item);

            from.RevealingAction();

            if (notYetLifted)
            {
                this.m_Lifted.Add(item);

                if (0.1 >= Utility.RandomDouble()) // 10% chance to spawn a new monster
                    TreasureMap.Spawn(this.m_Level, this.GetWorldLocation(), this.Map, from, false);
            }

            base.OnItemLifted(from, item);
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (m.AccessLevel < AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1048122, "", 0x8A5); // The chest refuses to be filled with treasure again.
                return false;
            }

            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            writer.Write(this.m_Guardians, true);
            writer.Write((bool)this.m_Temporary);

            writer.Write(this.m_Owner);

            writer.Write((int)this.m_Level);
            writer.WriteDeltaTime(this.m_DeleteTime);
            writer.Write(this.m_Lifted, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                    {
                        this.m_Guardians = reader.ReadStrongMobileList();
                        this.m_Temporary = reader.ReadBool();

                        goto case 1;
                    }
                case 1:
                    {
                        this.m_Owner = reader.ReadMobile();

                        goto case 0;
                    }
                case 0:
                    {
                        this.m_Level = reader.ReadInt();
                        this.m_DeleteTime = reader.ReadDeltaTime();
                        this.m_Lifted = reader.ReadStrongItemList();

                        if (version < 2)
                            this.m_Guardians = new List<Mobile>();

                        break;
                    }
            }

            if (!this.m_Temporary)
            {
                this.m_Timer = new DeleteTimer(this, this.m_DeleteTime);
                this.m_Timer.Start();
            }
            else
            {
                this.Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            base.OnAfterDelete();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from.Alive)
                list.Add(new RemoveEntry(from, this));
        }

        public void BeginRemove(Mobile from)
        {
            if (!from.Alive)
                return;

            from.CloseGump(typeof(RemoveGump));
            from.SendGump(new RemoveGump(from, this));
        }

        public void EndRemove(Mobile from)
        {
            if (this.Deleted || from != this.m_Owner || !from.InRange(this.GetWorldLocation(), 3))
                return;

            from.SendLocalizedMessage(1048124, "", 0x8A5); // The old, rusted chest crumbles when you hit it.
            this.Delete();
        }

        private static void GetRandomAOSStats(out int attributeCount, out int min, out int max)
        {
            int rnd = Utility.Random(15);
			
            if (Core.SE)
            {
                if (rnd < 1)
                {
                    attributeCount = Utility.RandomMinMax(3, 5);
                    min = 50;
                    max = 100;
                }
                else if (rnd < 3)
                {
                    attributeCount = Utility.RandomMinMax(2, 5);
                    min = 40;
                    max = 80;
                }
                else if (rnd < 6)
                {
                    attributeCount = Utility.RandomMinMax(2, 4);
                    min = 30;
                    max = 60;
                }
                else if (rnd < 10)
                {
                    attributeCount = Utility.RandomMinMax(1, 3);
                    min = 20;
                    max = 40;
                }
                else
                {
                    attributeCount = 1;
                    min = 10;
                    max = 20;
                }
            }
            else
            {
                if (rnd < 1)
                {
                    attributeCount = Utility.RandomMinMax(2, 5);
                    min = 20;
                    max = 70;
                }
                else if (rnd < 3)
                {
                    attributeCount = Utility.RandomMinMax(2, 4);
                    min = 20;
                    max = 50;
                }
                else if (rnd < 6)
                {
                    attributeCount = Utility.RandomMinMax(2, 3);
                    min = 20;
                    max = 40;
                }
                else if (rnd < 10)
                {
                    attributeCount = Utility.RandomMinMax(1, 2);
                    min = 10;
                    max = 30;
                }
                else
                {
                    attributeCount = 1;
                    min = 10;
                    max = 20;
                }
            }
        }

        private bool CheckLoot(Mobile m, bool criminalAction)
        {
            if (this.m_Temporary)
                return false;

            if (m.AccessLevel >= AccessLevel.GameMaster || this.m_Owner == null || m == this.m_Owner)
                return true;

            Party p = Party.Get(this.m_Owner);

            if (p != null && p.Contains(m))
                return true;

            Map map = this.Map;

            if (map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0)
            {
                if (criminalAction)
                    m.CriminalAction(true);
                else
                    m.SendLocalizedMessage(1010630); // Taking someone else's treasure is a criminal offense!

                return true;
            }

            m.SendLocalizedMessage(1010631); // You did not discover this chest!
            return false;
        }

        private class RemoveGump : Gump
        {
            private readonly Mobile m_From;
            private readonly TreasureMapChest m_Chest;
            public RemoveGump(Mobile from, TreasureMapChest chest)
                : base(15, 15)
            {
                this.m_From = from;
                this.m_Chest = chest;

                this.Closable = false;
                this.Disposable = false;

                this.AddPage(0);

                this.AddBackground(30, 0, 240, 240, 2620);

                this.AddHtmlLocalized(45, 15, 200, 80, 1048125, 0xFFFFFF, false, false); // When this treasure chest is removed, any items still inside of it will be lost.
                this.AddHtmlLocalized(45, 95, 200, 60, 1048126, 0xFFFFFF, false, false); // Are you certain you're ready to remove this chest?

                this.AddButton(40, 153, 4005, 4007, 1, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(75, 155, 180, 40, 1048127, 0xFFFFFF, false, false); // Remove the Treasure Chest

                this.AddButton(40, 195, 4005, 4007, 2, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(75, 197, 180, 35, 1006045, 0xFFFFFF, false, false); // Cancel
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 1)
                    this.m_Chest.EndRemove(this.m_From);
            }
        }

        private class RemoveEntry : ContextMenuEntry
        {
            private readonly Mobile m_From;
            private readonly TreasureMapChest m_Chest;
            public RemoveEntry(Mobile from, TreasureMapChest chest)
                : base(6149, 3)
            {
                this.m_From = from;
                this.m_Chest = chest;

                this.Enabled = (from == chest.Owner);
            }

            public override void OnClick()
            {
                if (this.m_Chest.Deleted || this.m_From != this.m_Chest.Owner || !this.m_From.CheckAlive())
                    return;

                this.m_Chest.BeginRemove(this.m_From);
            }
        }

        private class DeleteTimer : Timer
        {
            private readonly Item m_Item;
            public DeleteTimer(Item item, DateTime time)
                : base(time - DateTime.UtcNow)
            {
                this.m_Item = item;
                this.Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                this.m_Item.Delete();
            }
        }
    }
}