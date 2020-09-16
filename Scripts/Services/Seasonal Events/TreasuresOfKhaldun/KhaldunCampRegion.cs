using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Khaldun
{
    public class KhaldunCampRegion : BaseRegion
    {
        public static KhaldunCampRegion InstanceTram { get; set; }
        public static KhaldunCampRegion InstanceFel { get; set; }

        public KhaldunCampRegion(Map map)
            : base("Khaldun Camp Region", map, DefaultPriority, new Rectangle2D(5990, 3706, 52, 88))
        {
            Register();
        }

        private readonly Point3D[] _GuardLocs =
        {
            new Point3D(6024, 3713, 1), new Point3D(6035, 3752, 11),
            new Point3D(5999, 3726, 22), new Point3D(5992, 3752, 9),
            new Point3D(6023, 3777, 20)
        };

        private readonly Point3D[][] _BlockerLocs =
        {
            new Point3D[]
            {
                new Point3D(5995, 3727, 19), new Point3D(5995, 3726, 18), new Point3D(5996, 3726, 20), new Point3D(5996, 3725, 17), new Point3D(5997, 3725, 19),
                new Point3D(5997, 3724, 19), new Point3D(5998, 3724, 19), new Point3D(5998, 3723, 20), new Point3D(5999, 3723, 20), new Point3D(5999, 3722, 20),
            },

            new Point3D[]
            {
                new Point3D(6023, 3710, 0), new Point3D(6024, 3710, 1), new Point3D(6025, 3710, 2), new Point3D(6026, 3710, 1), new Point3D(6027, 3710, -2),
                new Point3D(6028, 3710, -2), new Point3D(6029, 3710, -1), new Point3D(6030, 3710, -1), new Point3D(6031, 3710, 0),
            },
            new Point3D[]
            {
                new Point3D(6036, 3748, 2), new Point3D(6037, 3748, 1), new Point3D(6038, 3748, 0), new Point3D(6039, 3748, 0), new Point3D(6040, 3748, 1), new Point3D(6041, 3748, -2),
            },

            new Point3D[]
            {
                new Point3D(6025, 3782, 22), new Point3D(6026, 3782, 22), new Point3D(6026, 3781, 22), new Point3D(6027, 3781, 19), new Point3D(6027, 3780, 22), new Point3D(6027, 3779, 18),
            },

            new Point3D[]
            {
                new Point3D(5991, 3755, 4), new Point3D(5991, 3754, 9), new Point3D(5991, 3753, 8), new Point3D(5991, 3752, 8), new Point3D(5991, 3751, 8), new Point3D(5991, 3750, 7), new Point3D(5991, 3749, 5),
            },

            new Point3D[]
            {
                new Point3D(5992, 3749, 9), new Point3D(5993, 3749, 10), new Point3D(5994, 3749, 10), new Point3D(5995, 3749, 10)
            }
        };

        public override void OnRegister()
        {
            foreach (Point3D p in _GuardLocs)
            {
                IPooledEnumerable eable = Map.GetMobilesInRange(p, 0);
                bool empty = true;

                foreach (object m in eable)
                {
                    if (m is KhaldunCampGuard)
                    {
                        empty = false;
                        break;
                    }
                }

                eable.Free();

                if (empty)
                {
                    KhaldunCampGuard guard = new KhaldunCampGuard();

                    guard.MoveToWorld(p, Map);
                }
            }

            for (int i = 0; i < _BlockerLocs.Length; i++)
            {
                foreach (Point3D p in _BlockerLocs[i])
                {
                    IPooledEnumerable eable = Map.GetItemsInRange(p, 0);
                    bool empty = true;

                    foreach (object m in eable)
                    {
                        if (m is KhaldunCampBlocker)
                        {
                            empty = false;
                            break;
                        }
                    }

                    eable.Free();

                    if (empty)
                    {
                        KhaldunCampBlocker blocker = new KhaldunCampBlocker(i);

                        blocker.MoveToWorld(p, Map);
                    }
                }
            }
        }

        public override void OnUnregister()
        {
            GetEnumeratedMobiles().OfType<KhaldunCampGuard>().IterateReverse(g => g.Delete());
            GetEnumeratedItems().OfType<KhaldunCampBlocker>().IterateReverse(b => b.Delete());
        }

        public override bool CheckTravel(Mobile traveller, Point3D p, TravelCheckType type)
        {
            return false;
        }
    }

    public class KhaldunCampGuard : BaseVendor
    {
        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override bool IsActiveVendor => false;

        public override void InitSBInfo()
        {
        }

        public KhaldunCampGuard()
            : base("the Guard")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = Utility.RandomBool();
            CantWalk = true;

            Name = NameList.RandomName(Female ? "female" : "male");

            Race = Race.Human;
            Hue = Race.RandomSkinHue();
            HairHue = Race.RandomHairHue();
            FacialHairHue = Race.RandomHairHue();
            HairItemID = Race.RandomHair(Female);
            FacialHairItemID = Race.RandomFacialHair(Female);
        }

        public override void InitOutfit()
        {
            SetWearable(new PlateChest());
            SetWearable(new PlateLegs());
            SetWearable(new PlateGorget());
            SetWearable(new PlateGloves());
            SetWearable(new Halberd());
            SetWearable(new PlateArms());
        }

        public KhaldunCampGuard(Serial serial)
            : base(serial)
        {
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

    public class LeadInvestigator : BaseVendor
    {
        protected readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override bool IsActiveVendor => false;

        public override void InitSBInfo()
        {
        }

        public static LeadInvestigator InstanceTram { get; set; }
        public static LeadInvestigator InstanceFel { get; set; }

        public LeadInvestigator()
            : base("the Lead Investigator")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = Utility.RandomBool();
            CantWalk = true;

            Name = NameList.RandomName(Female ? "female" : "male");

            Race = Race.Human;
            Hue = Race.RandomSkinHue();
            HairHue = Race.RandomHairHue();
            FacialHairHue = Race.RandomHairHue();
            HairItemID = Race.RandomHair(Female);
            FacialHairItemID = Race.RandomFacialHair(Female);
        }

        public override void InitOutfit()
        {
            SetWearable(new Shirt(), 1354);
            SetWearable(new Doublet(), 1504);
            SetWearable(new Kilt(), 1351);
            SetWearable(new LongPants(), 1743);
            SetWearable(new GoldRing());
            SetWearable(new GoldBracelet());
            SetWearable(new Shoes());
            SetWearable(new GoldNecklace());
            SetWearable(new GoldEarrings());
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (InRange(from.Location, 3) && from.FindItemOnLayer(Layer.Neck) is DetectiveCredentials)
            {
                Direction = Utility.GetDirection(this, from);

                from.SendGump(new BasicInfoGump(1158700, null, 300, 500));
                /*You must be the Gumshoe who cracked this case wide open? Wow, to think we would still be worrying about these cultist
                 * miscreants after all this time? No matter, thanks to your instincts we may stand a fighting chance against this prophecy.
                 * I have to admit, I was skeptical at first, but after hearing what Sage Humbolt's had to say and seeing this, what do 
                 * they call it - a meteorite? - was all the convincing I needed! Seems the good news is, despite numbers the cultist 
                 * armies have and the big bad evil pulling the strings we've been keeping them inside the dungeon. It seems that once 
                 * the meteorite impacted the lands here, it spread a material the smiths are calling "Caddellite" in everything - rocks,
                 * trees, fish - you name it. The artisans were clever enough to fashion weapons, spellbooks, and treats to get past the
                 * magics protecting the creatures in Khaldun. One thing though - seems the Caddellite is too strong for typical Britannian
                 * crafting tools save for those made by Krett the Tinker over there. Can't say I really blame him for making some coin 
                 * on the situation. Lucky dog!*/
            }
        }

        public LeadInvestigator(Serial serial)
            : base(serial)
        {
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

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }
        }
    }

    public class KhaldunCampBlocker : Item
    {
        public int Position { get; set; }

        public KhaldunCampBlocker(int position)
            : base(0x1BC3)
        {
            Position = position;

            Name = "Khaldun Camp Blocker";
            Movable = false;
            Visible = false;
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.AccessLevel > AccessLevel.Player)
            {
                return base.OnMoveOver(m);
            }

            if (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)
            {
                m = ((BaseCreature)m).GetMaster();
            }

            bool hasCreds = m.FindItemOnLayer(Layer.Neck) is DetectiveCredentials;
            bool canCross = hasCreds;

            if (!canCross)
            {
                IPooledEnumerable eable = m.Map.GetItemsInRange(m.Location, 0);

                foreach (Item item in eable)
                {
                    if (item is KhaldunCampBlocker)
                    {
                        eable.Free();
                        return base.OnMoveOver(m);
                    }
                }

                eable.Free();

                if (X == 5991 && Y == 3749)
                {
                    CheckGuard(m, false);
                    return false;
                }

                Direction moving = Utility.GetDirection(m, this);

                switch (Position)
                {
                    case 0:
                        canCross = moving == Direction.North || moving == Direction.Up || moving == Direction.West; break;
                    case 1:
                    case 2:
                        canCross = moving == Direction.North || moving == Direction.Up || moving == Direction.Right; break;
                    case 3:
                        canCross = moving == Direction.South || moving == Direction.Down || moving == Direction.East; break;
                    case 4:
                        canCross = moving == Direction.West || moving == Direction.Up || moving == Direction.Left; break;
                    case 5:
                        canCross = moving == Direction.North || moving == Direction.Right || moving == Direction.North; break;
                }
            }

            if (!canCross)
            {
                CheckGuard(m, false);
                return false;
            }

            if (hasCreds)
            {
                CheckGuard(m, true);
            }

            return base.OnMoveOver(m);
        }

        private void CheckGuard(Mobile m, bool pass)
        {
            IPooledEnumerable eable = Map.GetMobilesInRange(m.Location, 10);

            foreach (KhaldunCampGuard g in eable.OfType<KhaldunCampGuard>())
            {
                if (pass)
                {
                    g.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158716, m.NetState); // *nods* Please proceed, Detective
                }
                else
                {
                    g.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1158715, m.NetState); // Halt! Who goes there! Official RBG Detective personnel only!
                }

                break;
            }

            eable.Free();
        }

        public KhaldunCampBlocker(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(Position);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Position = reader.ReadInt();
        }
    }
}
