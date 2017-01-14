using Server;
using System;
using Server.Items;
using System.Collections.Generic;
using Server.Engines.VoidPool;

 namespace Server.Mobiles
 {
	public class CoraTheSorceress : BaseCreature
	{
        public DateTime NextManaDrain { get; set; }

        public TimeSpan ManaDrainInterval { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(15, 120)); } }

        [Constructable]
        public CoraTheSorceress() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.1)
        {
            Body = 0x191;
            Name = "cora";
            Title = "the sorceress";

            HairItemID = 0x2045;
            HairHue = 452;

            SetStr(909, 949);
            SetDex(901, 948);
            SetInt(903, 947);

            SetHits(35000);

            SetDamage(17, 25);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 52, 67);
            SetResistance(ResistanceType.Fire, 51, 68);
            SetResistance(ResistanceType.Cold, 51, 69);
            SetResistance(ResistanceType.Poison, 51, 70);
            SetResistance(ResistanceType.Energy, 50, 68);

            SetSkill(SkillName.Wrestling, 100.1, 119.7);
            SetSkill(SkillName.Tactics, 102.3, 118.5);
            SetSkill(SkillName.MagicResist, 101.2, 119.6);
            SetSkill(SkillName.Anatomy, 100.1, 117.5);
            SetSkill(SkillName.Magery, 100.1, 117.5);
            SetSkill(SkillName.EvalInt, 100.1, 117.5);

            Fame = 32000;
            Karma = -32000;

            AddAndEquip(new WildStaff(), 2963);
            AddAndEquip(new ThighBoots(), 1150);
            AddAndEquip(new LongPants(), 2724);
            AddAndEquip(new LeatherGloves(), 2724);
            AddAndEquip(new LeatherBustierArms(), 2527);
        }

        public override int BreathChaosDamage { get { return 100; } }
        public override bool HasBreath { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override bool TeleportsTo { get { return true; } }
        public override TimeSpan TeleportDuration { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60)); } }
        public override double TeleportProb { get { return 1.0; } }
        public override bool TeleportsPets { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.SuperBoss, 3);
        }

        private void AddAndEquip(Item item, int hue = 0)
        {
            item.Movable = false;
            item.Hue = hue;
            AddItem(item);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (NextManaDrain < DateTime.UtcNow)
                DoManaDrain();
        }

        public void DoManaDrain()
        {
            DoEffects(Direction.North);
            DoEffects(Direction.West);
            DoEffects(Direction.South);
            DoEffects(Direction.East);

            NextManaDrain = DateTime.UtcNow + ManaDrainInterval;
        }

        private bool DoEffects(Direction d)
        {
            int x = this.X;
            int y = this.Y;
            int z = this.Z;
            int range = 10;
            int offset = 8;

            switch (d)
            {
                case Direction.North:
                    x = this.X + Utility.RandomMinMax(-offset, offset);
                    y = this.Y - range;
                    break;
                case Direction.West:
                    x = this.X - range;
                    y = this.Y + Utility.RandomMinMax(-offset, offset); 
                    break;
                case Direction.South:
                    x = this.X + Utility.RandomMinMax(-offset, offset);
                    y = this.Y + range;
                    break;
                case Direction.East:
                    x = this.X + range;
                    y = this.Y + Utility.RandomMinMax(-offset, offset); 
                    break;
            }

            for (int i = 0; i < range; i++)
            {
                switch (d)
                {
                    case Direction.North: y += i; break;
                    case Direction.West: x += i; break;
                    case Direction.South: y -= i; break;
                    case Direction.East: x -= i; break;
                }

                z = this.Map.GetAverageZ(x, y);
                Point3D p = new Point3D(x, y, z);

                if (Server.Spells.SpellHelper.AdjustField(ref p, this.Map, 12, false))/*this.Map.CanFit(x, y, z, 16, false, false, true))/*this.Map.CanSpawnMobile(x, y, z)*/
                {
                    MovementPath path = new MovementPath(this, p);

                    if (path.Success)
                    {
                        DropCrack(path);
                        return true;
                    }
                }
            }

            return false;
        }

        private void DropCrack(MovementPath path)
        {
            int time = 10;
            int x = this.X;
            int y = this.Y;

            for (int i = 0; i < path.Directions.Length; ++i)
            {
                Movement.Movement.Offset(path.Directions[i], ref x, ref y);
                IPoint3D p = new Point3D(x, y, this.Map.GetAverageZ(x, y)) as IPoint3D;

                Timer.DelayCall(TimeSpan.FromMilliseconds(time), new TimerStateCallback(ManaDrainEffects_Callback), new object[] { p, this.Map });

                time += 200;
            }
        }

        private void ManaDrainEffects_Callback(object o)
        {
            object[] objs = o as object[];
            IPoint3D p = objs[0] as IPoint3D;
            Map map = objs[1] as Map;

            var item = new ManaDrainItem(Utility.RandomList(6913, 6915, 6917, 6919), this);
            Spells.SpellHelper.GetSurfaceTop(ref p);

            item.MoveToWorld(new Point3D(p), this.Map);
        }

        private class ManaDrainItem : Item
        {
            public Item Static { get; private set; }
            public Mobile Owner { get; private set; }

            public ManaDrainItem(int id, Mobile owner) : base(id)
            {
                Owner = owner;

                Movable = false;
                Hue = 1152;
                Timer.DelayCall(TimeSpan.FromSeconds(5), ChangeHue);
            }

            private void ChangeHue()
            {
                Hue = 1153;
                Static.Hue = 1153;

                Timer.DelayCall(TimeSpan.FromSeconds(0.5), Delete);
            }

            public override void Delete()
            {
                if (Static != null)
                    Static.Delete();

                Static = null;
                base.Delete();
            }

            public override void OnLocationChange(Point3D oldLocation)
            {
                Static = new Static(this.ItemID + 1);
                Static.MoveToWorld(this.Location, this.Map);

                IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 0);

                foreach (Mobile m in eable)
                {
                    OnMoveOver(m);
                }
                eable.Free();
            }

            public override bool OnMoveOver(Mobile m)
            {
                if ((m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile)) && m.CanBeHarmful(Owner, false))
                {
                    m.SendLocalizedMessage(1153114, m.Mana.ToString()); // Cora drains ~1_VAL~ points of your mana!
                    m.Mana = 0;
                }

                return true;
            }

            public ManaDrainItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);

                writer.Write(Static);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                Static = reader.ReadItem();

                if (Static != null)
                    Static.Delete();

                Delete();
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (0.30 > Utility.RandomDouble())
            {            
                Mobile m = DemonKnight.FindRandomPlayer(this);

                if (m != null)
                {
                    Item artifact = VoidPoolRewards.DropRandomArtifact();

                    if (artifact != null)
                    {
                        Container pack = m.Backpack;

                        if (pack == null || !pack.TryDropItem(m, artifact, false))
                            m.BankBox.DropItem(artifact);

                        m.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
                    }
                }
            }
        }

        public CoraTheSorceress(Serial serial)
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