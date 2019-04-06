using System;
using System.Collections.Generic;

using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.Quests
{
    public class Prugyilonus : MondainQuester
    {
        public static Prugyilonus Instance { get; set; }

        public static void Initialize()
        {
            if (Core.SA && Instance == null)
            {
                Instance = new Prugyilonus();
                Instance.MoveToWorld(new Point3D(750, 3344, 61), Map.TerMur);
            }
        }

        public override Type[] Quests { get { return new Type[] { ScalesOfADreamSerpentQuest }; } }

        public Prugyilonus()
            : base("Prugyilonus", "the Advisor to the Queen")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            Race = Race.Gargoyle;

            CantWalk = true;

            Hue = 34547;
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new GargishFancyRobe(), 1345);
        }

        public Prugyilonus(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenaricReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            Instance = this;
        }
    }

    public class Bexil : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { CatchMeIfYouCanQuest }; } }

        public static Bexil Instance { get; set; }

        public static void Initialize()
        {
            if(Core.SA && Instance == null)
            {
                Instance = new Bexil();
                Instnace.MoveToWorld(new Point3D(662, 3819, -43), Map.TerMur);
                Instance.Home = new Point3D(662, 3819, -43);
                Insance.RangeHome = 2;
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.Backpack.GetAmount(typeof(DreamSerpentScale)) == 0)
            {
                base.OnDoubleClick(m);
            }
            else
            {
                SayTo(m, 1151355, 0x3B2); // You may not obtain more than one of this item.
                SayTo(m, 1080107, 0x3B2); // I'm sorry, I have nothing for you at this time.
            }
        }

        public Bexil()
            : base("Bexil", "the Dream Serpent")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            Body = 0xCE;
            Hue = 2069;
        }

        public Bexil(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenaricReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            Instance = this;
        }
    }

    public class BexilPunchingBag : BaseCreature
    {
        public virtual bool InitialInnocent { get { return true; } }

        private Dictionary<Mobile, int> _Table = new Dictionary<Mobile, int>();
        private DateTime _NextTeleport;

        public BexilPunchingBag()
            : base("the Dream Serpent")
        {
            Name = "Bexil";
            Title = "the Dream Serpent";

            Body = 0xCE;
            Hue = 1976;
            BaseSoundID = 0x5A;

            CantWalk = true;

            SetHits(1000000);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant is Mobile && _NextTeleport < DateTime.UtcNow)
            {
                var map = Map;
                var c = (Mobile)Combatant;

                var rec = new Rectangle2D(X - RangePerception, Y - RangePerception, X + RangePerception, Y + RangePerception);
                Point3D = Point3D.Zero;

                do
                {
                    p = map.GetRandomSpawnPoint(rec);
                }
                while (p == Point3D.Zero || !map.CanSpawnMobile(p.X, p.Y, z));

                Effects.SendLocationParticles(EffectItem.Create(Location, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                MoveToWorld(new Point3D(px, py, Map.TerMur.GetAverageZ(px, py)));
                Effects.SendLocationParticles(EffectItem.Create(Location, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                _NextTeleport = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(1, 3));
            }
        }

        public override int Damage(int amount, Mobile from, bool informMount, bool checkDisrupt)
        {
            if (from is PlayerMobile)
            {
                var quest = QuestHelper.GetQuest<CatchMeIfYouCanQuest>((PlayerMobile)from);

                if (quest != null)
                {
                    quest.Update(this);

                    if (quest.Completed)
                    {
                        DreamSerpentCharm.CompleteQuest(m);
                    }
                }
            }

            return 0;
        }

        public override bool OnAfterDeath()
        {
            var bex = new BexilPunchingBag();
            bex.MoveToWorld(new Point3D(403, 3391, 38), Map.TerMur);

            return base.OnAfterDeath();
        }

        public BexilPunchingBag(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenaricReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            Delete();
        }
    }

    public class Grubbix : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { FilthyLifeStealersQuest }; } }

        public static Bexil Instance { get; set; }

        public static void Initialize()
        {
            if (Core.SA && Instance == null)
            {
                Instance = new Grubbix();
                Instnace.MoveToWorld(new Point3D(1106, 3138, -43), Map.TerMur);
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m.Backpack.GetAmount(typeof(DreamSerpentScale)) == 0)
            {
                base.OnDoubleClick(m);
            }
            else
            {
                SayTo(m, 1151355, 0x3B2); // You may not obtain more than one of this item.
                SayTo(m, 1080107, 0x3B2); // I'm sorry, I have nothing for you at this time.
            }
        }

        public Grubbix()
            : base("Grubbix", "the Soulbinder")
        {
        }

        public override void InitBody()
        {
            CantWalk = true;

            InitStats(100, 100, 25);
            Body = 0x100;
            Hue = 2076;
        }

        public Bexil(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenaricReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt(); // version

            Instance = this;
        }
    }


}
