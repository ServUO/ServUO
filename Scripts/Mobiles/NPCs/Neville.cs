using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.Quests
{
    public class Neville : BaseEscort
    {
        public static void Initialize()
        {
            Spawn();
        }

        public static Point3D HomeLocation => new Point3D(1150, 964, -42);
        public static int HomeRange => 5;

        public override Type[] Quests => new Type[] { typeof(EscortToDugan) };

        private DateTime m_TalkTime;

        public static List<Neville> Instances { get; set; }

        readonly string[] NevilleSay = new string[]
        {
            "Save Us",
            "Murder is being done!",
            "Protect me!",
            "a scoundrel is committing murder!",
            "Where are the guards! Help!",
            "Make haste",
            "Tisawful! Death! Ah!"
        };

        [Constructable]
        public Neville()
            : base()
        {
            Name = "Neville Brightwhistle";

            SpeechHue = 0x3B2;

            if (Instances == null)
                Instances = new List<Neville>();

            Instances.Add(this);
        }

        public Neville(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public override void Advertise()
        {
            Say(1095004); // Please help me, where am I?
        }

        public override void OnThink()
        {
            if (DateTime.UtcNow >= m_TalkTime)
            {
                if (!Alive || Deleted || ControlMaster == null)
                {
                    return;
                }

                if (!ControlMaster.Hidden && ControlMaster.Aggressors.Count > 0)
                {
                    SayRandom(NevilleSay, this);

                    m_TalkTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30));
                }
            }

            base.OnThink();
        }

        private void SayRandom(string[] say, Mobile m)
        {
            m.Say(say[Utility.Random(say.Length)]);
        }

        public override void OnAfterDelete()
        {            
        }

        public override void OnDelete()
        {
            if (Instances != null && Instances.Contains(this))
                Instances.Remove(this);

            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
            {
                Spawn();
            });

            base.OnDelete();
        }

        public static void Spawn()
        {
            if (Instances != null && Instances.Count > 0)
                return;

            Neville creature = new Neville
            {
                Home = HomeLocation,
                RangeHome = HomeRange
            };

            creature.MoveToWorld(HomeLocation, Map.TerMur);
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;

            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new Shoes(0x70A));
            SetWearable(new LongPants(0x1BB));
            SetWearable(new FancyShirt(0x588));
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

            if (Instances == null)
                Instances = new List<Neville>();

            Instances.Add(this);
        }
    }
}
