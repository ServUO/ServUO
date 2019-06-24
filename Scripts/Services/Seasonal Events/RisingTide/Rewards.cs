using System;
using System.Linq;

using Server;
using Server.Gumps;
using Server.Multis;

namespace Server.Items
{
    public class DragonCannon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new DragonCannonDeed(); } }

        [Constructable]
        public DragonCannon()
            : this(DirectionType.South)
        {
        }

        [Constructable]
        public DragonCannon(DirectionType direction)
        {
            switch (direction)
            {
                case DirectionType.North:
                    AddComponent(new AddonComponent(0x44F4), 0, 0, 0);
                    AddComponent(new AddonComponent(0x44F3), 0, 1, 0);
                    AddComponent(new AddonComponent(0x44F5), 0, -1, 0);
                    break;
                case DirectionType.West:
                    AddComponent(new AddonComponent(0x424A), 0, 0, 0);
                    AddComponent(new AddonComponent(0x4223), 1, 0, 0);
                    AddComponent(new AddonComponent(0x418F), -1, 0, 0);
                    break;
                case DirectionType.South:
                    AddComponent(new AddonComponent(0x4221), 0, 0, 0);
                    AddComponent(new AddonComponent(0x4222), 0, 1, 0);
                    AddComponent(new AddonComponent(0x4220), 0, -1, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new AddonComponent(0x44F7), 0, 0, 0);
                    AddComponent(new AddonComponent(0x44F6), 1, 0, 0);
                    AddComponent(new AddonComponent(0x44F8), -1, 0, 0);
                    break;
            }
        }

        public DragonCannon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class DragonCannonDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber { get { return 1158926; } } // Decorative Dragon Cannon
        public override BaseAddon Addon { get { return new DragonCannon(_Direction); } }

        private DirectionType _Direction;

        [Constructable]
        public DragonCannonDeed()
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.North, 1075389); // North
            list.Add((int)DirectionType.West, 1075390); // West
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076783)); // Please select your shadow altar position
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public DragonCannonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0xA2CA, 0xA2CB)]
    public class ShoulderParrot : BaseOuterTorso
    {
        private DateTime _NextFly;
        private DateTime _FlyEnd;
        private Timer _Timer;
        private Mobile _LastShoulder;

        private string _MasterName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string MasterName { get { return _MasterName; } set { _MasterName = value; InvalidateProperties(); } }

        [Constructable]
        public ShoulderParrot()
            : base(0xA2CA)
        {
            LootType = LootType.Blessed;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (_MasterName != null)
            {
                list.Add(1158958, String.Format("{0}{1}", _MasterName, _MasterName.ToLower().EndsWith("s") || _MasterName.ToLower().EndsWith("z") ? "'" : "'s"));
            }
            else
            {
                list.Add(1158928); // Shoulder Parrot
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.FindItemOnLayer(Layer.OuterTorso) == this)
            {
                if (_NextFly > DateTime.UtcNow)
                {
                    m.SendLocalizedMessage(1158956); // Your parrot is too tired to fly right now.
                }
                else
                {
                    _Timer = Timer.DelayCall(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500), FlyOnTick);
                    _Timer.Start();

                    Movable = false;
                    _LastShoulder = m;
                    MoveToWorld(new Point3D(m.X, m.Y, m.Z + 15), m.Map);
                    ItemID = 0xA2CC;

                    _FlyEnd = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(3, 5));
                }
            }
            else
            {
                m.SendLocalizedMessage(1158957); // Your parrot can't fly here.
            }
        }

        private void FlyOnTick()
        {
            if (_FlyEnd < DateTime.UtcNow)
            {
                Movable = true;
                ItemID = 0xA2CA;

                if (_LastShoulder.FindItemOnLayer(Layer.OuterTorso) != null)
                {
                    _LastShoulder.Backpack.DropItem(this);
                }
                else
                {
                    _LastShoulder.AddItem(this);
                }

                _LastShoulder = null;
                _Timer.Stop();
                _NextFly = DateTime.UtcNow + TimeSpan.FromMinutes(2);
            }
        }

        public ShoulderParrot(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_MasterName);
            writer.Write(_LastShoulder);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _MasterName = reader.ReadString();
            Mobile m = reader.ReadMobile();

            if (m != null)
            {
                ItemID = 0xA2CA;

                Timer.DelayCall(() =>
                {
                    if (m.FindItemOnLayer(Layer.OuterTorso) != null)
                    {
                        m.Backpack.DropItem(this);
                    }
                    else
                    {
                        m.AddItem(this);
                    }
                });
            }
        }
    }

    [Flipable(0xA2C8, 0xA2C9)]
    public class PirateWallMap : Item
    {
        public override int LabelNumber { get { return 1158938; } } // Pirate Wall Map

        [Constructable]
        public PirateWallMap()
            : base(0xA2C8)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                var gump = new Gump(50, 50);
                gump.AddImage(0, 0, 0x9CE9);

                m.SendGump(gump);
            }
        }

        public PirateWallMap(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0xA2C6, 0xA2C7)]
    public class MysteriousStatue : Item
    {
        public override int LabelNumber { get { return 1158935; } } // Mysterious Statue

        [Constructable]
        public MysteriousStatue()
            : base(0xA2C6)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.InRange(GetWorldLocation(), 2))
            {
                m.SendGump(new BasicInfoGump(1158937));
                /*This mysterious statue towers above you. Even as skilled a mason as you are, the craftsmanship is uncanny, and unlike anything you have encountered before.
                 * The stone appears to be smooth and special attention was taken to sculpt the statue as a perfect likeness. According to the pirate you purchased the statue
                 * from, it was recovered somewhere at sea. The amount of marine growth seems to reinforce this claim, yet you cannot discern how long it may have been
                 * submerged and are thus unsure of its age. Whatever its origins, one thing is clear - the figure is one you hope you do not encounter anytime soon...*/
            }
        }

        /*public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1158936); // Purchased from a Pirate Merchant
        }*/

        public MysteriousStatue(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    [Flipable(0x4C26, 0x4C27)]
    public class DecorativeWoodCarving : Item
    {
        public string _ShipName;

        [CommandProperty(AccessLevel.GameMaster)]
        public string ShipName { get { return _ShipName; } set { _ShipName = value; InvalidateProperties(); } }

        [Constructable]
        public DecorativeWoodCarving()
            : base(0x4C26)
        {
            Hue = 2968;
        }

        public void AssignRandomName()
        {
            var list = BaseBoat.Boats.Where(b => !String.IsNullOrEmpty(b.ShipName)).Select(x => x.ShipName).ToList();

            if (list.Count > 0)
            {
                _ShipName = list[Utility.Random(list.Count)];
            }
            else
            {
                _ShipName = _ShipNames[Utility.Random(_ShipNames.Length)];
            }

            InvalidateProperties();
            ColUtility.Free(list);
        }

        private static string[] _ShipNames =
        {
            "Adventure Galley",
            "Queen Anne's Revenge",
            "Fancy",
            "Whydah",
            "Royal Fortune",
            "The Black Pearl",
            "Satisfaction",
            "The Golden Fleece",
            "Bachelor's Delight",
            "The Revenge",
            "The Flying Dragon",
            "The Gabriel",
            "Privateer's Death",
            "Kiss of Death",
            "Devil's Doom",
            "Monkeebutt",
            "Mourning Star",
            "Cursed Sea-Dog",
            "The Howling Lusty Wench",
            "Scourage of the Seven Seas",
            "Neptune's Plague",
            "Sea's Hellish Plague",
            "The Salty Bastard"
        };

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (String.IsNullOrEmpty(_ShipName))
            {
                list.Add(1158943); // Wood Carving of [Ship's Name]
            }
            else
            {
                list.Add(1158921, _ShipName); // Wood Carving of ~1_name~
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (String.IsNullOrEmpty(_ShipName))
            {
                list.Add(1158953); // Named with a random famous ship, or if yer lucky - named after you!
            }
        }

        public DecorativeWoodCarving(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_ShipName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _ShipName = reader.ReadString();
        }
    }

    public class QuartermasterRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1158951; } } // Quartermaster

        [Constructable]
        public QuartermasterRewardDeed()
        {
        }

        public QuartermasterRewardDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SailingMasterRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1158950; } } // Sailing Master

        [Constructable]
        public SailingMasterRewardDeed()
        {
        }

        public SailingMasterRewardDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BotswainRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1158949; } } // Botswain

        [Constructable]
        public BotswainRewardDeed()
        {
        }

        public BotswainRewardDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PowderMonkeyRewardDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title { get { return 1158948; } } // Powder Monkey

        [Constructable]
        public PowderMonkeyRewardDeed()
        {
        }

        public PowderMonkeyRewardDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SpikedWhipOfPlundering : SpikedWhip
    {
        public override int LabelNumber { get { return 1158925; } } // Spiked Whip of Plundering

        [Constructable]
        public SpikedWhipOfPlundering()
        {
            ExtendedWeaponAttributes.HitExplosion = 15;
            WeaponAttributes.HitLeechMana = 81;
            Attributes.SpellChanneling = 1;
            Attributes.Luck = 100;
            Attributes.WeaponDamage = 70;
        }

        public SpikedWhipOfPlundering(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BladedWhipOfPlundering : BladedWhip
    {
        public override int LabelNumber { get { return 1158924; } } // Bladed Whip of Plundering

        [Constructable]
        public BladedWhipOfPlundering()
        {
            ExtendedWeaponAttributes.HitExplosion = 15;
            WeaponAttributes.HitLeechMana = 81;
            Attributes.SpellChanneling = 1;
            Attributes.Luck = 100;
            Attributes.WeaponDamage = 70;
        }

        public BladedWhipOfPlundering(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BarbedWhipOfPlundering : BarbedWhip
    {
        public override int LabelNumber { get { return 1158923; } } // Barbed Whip of Plundering

        [Constructable]
        public BarbedWhipOfPlundering()
        {
            ExtendedWeaponAttributes.HitExplosion = 15;
            WeaponAttributes.HitLeechMana = 81;
            Attributes.SpellChanneling = 1;
            Attributes.Luck = 100;
            Attributes.WeaponDamage = 70;
        }

        public BarbedWhipOfPlundering(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
