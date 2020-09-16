using Server.Items;
using Server.Regions;
using System;

namespace Server.Engines.Quests
{
    public class FreedomQuest : BaseQuest
    {
        public override bool DoneOnce => true;

        public FreedomQuest()
            : base()
        {
            AddObjective(new EscortObjective("Sanctuary Entrance"));

            AddReward(new BaseReward(typeof(StolenRing), "Lenley's Favorite Sparkly"));
        }

        /* Freedom! */
        public override object Title => 1072367;

        /*
         * Lenley isn't seen.  Why you see me? Lenley is sneaking.  Lenley runs away.
         * You help Lenley to not get dead?  We go out past pig-men orcs?  Yes? Yes? You say yes?
        */
        public override object Description => 1072552;

        /* You no like Lenley? No hurt Lenley!  No see Lenley.  Go 'way. */
        public override object Refuse => 1072553;

        /* Lenley not run away yet.  Go, go, Lenley not past pig-men orcs.  You go, Lenley go after you.  Go! */
        public override object Uncomplete => 1072554;

        /* Lenley so happy!  Lenley not get dead.  You have best Lenley shiny! */
        public override object Complete => 1072556;

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

    public class Lenley : BaseEscort
    {
        public override Type[] Quests => new Type[] { typeof(FreedomQuest) };

        public LenleyRegion _Region { get; set; }

        [Constructable]
        public Lenley()
            : base()
        {
            Name = "Lenley";
            Title = "the Snitch";
            Body = 0x2A;
            Hidden = true;
            CantWalk = true;

            SetStr(96, 120);
            SetDex(81, 100);
            SetInt(36, 60);

            SetHits(58, 72);

            SetDamage(4, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 30);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 35.1, 60.0);
            SetSkill(SkillName.Tactics, 50.1, 75.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);
            SetSkill(SkillName.Hiding, 75.0);

            Fame = 1500;
            Karma = 1500;
        }

        public Lenley(Serial serial)
            : base(serial)
        {
        }

        public override void RevealingAction()
        {
            if (_Region != null)
                _Region.Unregister();

            CantWalk = false;

            base.RevealingAction();
        }

        public override void OnDelete()
        {
            DeleteLenleyRegion();

            base.OnDelete();
        }

        public void DeleteLenleyRegion()
        {
            if (_Region != null)
                _Region.Unregister();
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            if (Deleted)
                return;

            UpdateLenleyRegion();
        }

        protected override void OnMapChange(Map oldMap)
        {
            if (Deleted)
                return;

            UpdateLenleyRegion();
        }

        public void UpdateLenleyRegion()
        {
            if (Hidden)
            {
                DeleteLenleyRegion();

                if (!Deleted && Map != Map.Internal)
                {
                    _Region = new LenleyRegion(this);
                    _Region.Register();
                }
            }
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

            if (Hidden)
            {
                Timer.DelayCall(TimeSpan.Zero, UpdateLenleyRegion);
            }
        }
    }

    public class LenleyRegion : BaseRegion
    {
        public LenleyRegion(Mobile lenley)
            : base(null, lenley.Map, Find(lenley.Location, lenley.Map), new Rectangle2D(lenley.Location.X - 2, lenley.Location.Y - 2, 5, 5))
        {
        }

        public override void OnEnter(Mobile m)
        {
            m.SendLocalizedMessage(1075014); // Psst!  Lenley isn't seen.  You help Lenley?
        }
    }
}
