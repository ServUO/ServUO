using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class Vrulkax : BaseVendor
    {
        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override bool IsActiveVendor => false;
        public override bool IsInvulnerable => true;

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public Vrulkax()
            : base("the Exalted Artificer")
        {
            Name = "Vrulkax";
        }

        private readonly Type[][] _Table =
        {
            new Type[] { typeof(BritchesOfWarding), typeof(GargishBritchesOfWarding) },
            new Type[] { typeof(GlovesOfFeudalGrip), typeof(GargishKiltOfFeudalVise) },
            new Type[] { typeof(CuffsOfTheArchmage), typeof(GargishCuffsOfTheArchmage) },
            new Type[] { typeof(BritchesOfWarding), typeof(GargishBritchesOfWarding) },
            new Type[] { typeof(BowOfTheInfiniteSwarm), typeof(GlaiveOfTheInfiniteSwarm) }
        };

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            foreach (Type[] t in _Table)
            {
                if (dropped is IDurability && t[0] == dropped.GetType())
                {
                    IDurability dur = dropped as IDurability;

                    if (dur != null && dur.MaxHitPoints == 255 && dur.HitPoints == 255)
                    {
                        Item item = Loot.Construct(t[1]);

                        if (item != null)
                        {
                            from.AddToBackpack(item);
                            dropped.Delete();
                            return true;
                        }
                    }
                    else
                    {
                        SayTo(from, 1157368); // I can only work with artifacts that are in pristine condition.
                        return false;
                    }
                }
            }

            SayTo(from, 1157365); // I'm sorry, I cannot accept this item.
            return false;
        }

        private DateTime _NextSpeak;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (_NextSpeak < DateTime.UtcNow && m is PlayerMobile && InRange(m.Location, 5) && InLOS(m))
            {
                Say(1157369); // I am the Exalted Artificer. Hand me select human artifacts to convert them for gargoyle use.
                _NextSpeak = DateTime.UtcNow + TimeSpan.FromSeconds(25);
            }

            base.OnMovement(m, oldLocation);
        }

        public Vrulkax(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            CantWalk = true;
            Race = Race.Gargoyle;

            Hue = 34545;
            HairItemID = 0x4259;
            HairHue = 0x0;
        }

        public override void InitOutfit()
        {
            AddItem(new SerpentStoneStaff());
            AddItem(new GargishClothChest(902));
            AddItem(new GargishClothArms(902));
            AddItem(new GargishClothKilt(902));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}