using System;
using Server.Items;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class Vrulkax : BaseVendor
    {
        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return this.m_SBInfos; } }

        public override bool IsActiveVendor { get { return false; } }
        public override bool IsInvulnerable { get { return true; } }

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public Vrulkax()
            : base("the Exalted Artificer")
        {
            this.SetSkill(SkillName.Imbuing, 60.0, 80.0);
        }

        private Type[][] _Table = 
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
                        var item = Loot.Construct(t[1]);

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
            InitStats(100, 100, 100);
            Body = 666;
            Race = Race.Gargoyle;
            HairItemID = 0x2044;//
            HairHue = 1153;
            Name = "Vrulkax";
            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();
            SpeechHue = 0x3B2;
            this.Blessed = true;
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new LeatherTalons());
            SetWearable(new GargishRobe(1645));
            SetWearable(new GargishClothWingArmor(1645));
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