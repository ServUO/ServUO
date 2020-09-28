using Server.Engines.VoidPool;
using Server.Items;
using System;

namespace Server.Mobiles
{
    public class CoraTheSorceress : BaseCreature, IElementalCreature, IAuraCreature
    {
        public ElementType ElementType => ElementType.Chaos;

        public DateTime NextManaDrain { get; set; }

        public TimeSpan ManaDrainInterval => TimeSpan.FromSeconds(Utility.RandomMinMax(15, 120));

        [Constructable]
        public CoraTheSorceress() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.1)
        {
            Body = 0x191;
            Name = "Cora";
            Title = "the sorceress";

            HairItemID = 0x2045;
            HairHue = 452;

            SetStr(909, 949);
            SetDex(125);
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

            AddAndEquip(new WildStaff(), 1971);
            AddAndEquip(new ThighBoots(), 1910);
            AddAndEquip(new ChainLegs(), 1936);
            AddAndEquip(new LeatherGloves(), 1910);
            AddAndEquip(new LeatherBustierArms(), 1947);

            SetSpecialAbility(SpecialAbility.DragonBreath);
            SetAreaEffect(AreaEffect.AuraDamage);
        }

        public override bool AlwaysMurderer => true;
        public override bool ClickTitle => false;
        public override bool ShowFameTitle => false;

        public void AuraEffect(Mobile m)
        {
            if (m is PlayerMobile && Services.TownCryer.TownCryerSystem.UnderMysteriousPotionEffects((PlayerMobile)m, true))
            {
                m.SayTo(m, 1158288, 1154); // *You resist Cora's attack!*
            }
            else
            {
                int mana = Utility.Random(1, m.Mana);
                m.Mana -= mana;
                m.SendLocalizedMessage(1153114, mana.ToString()); // Cora drains ~1_VAL~ points of your mana!
            }
        }

        public override bool TeleportsTo => true;
        public override TimeSpan TeleportDuration => TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
        public override double TeleportProb => 1.0;
        public override bool TeleportsPets => true;

        public override int GetDeathSound() { return 0x316; }

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
            Animate(AnimationType.Spell, 1);

            DoEffects(Direction.North);
            DoEffects(Direction.West);
            DoEffects(Direction.South);
            DoEffects(Direction.East);

            NextManaDrain = DateTime.UtcNow + ManaDrainInterval;
        }

        private bool DoEffects(Direction d)
        {
            int x = X;
            int y = Y;
            int z = Z;
            int range = 10;
            int offset = 8;

            switch (d)
            {
                case Direction.North:
                    x = X + Utility.RandomMinMax(-offset, offset);
                    y = Y - range;
                    break;
                case Direction.West:
                    x = X - range;
                    y = Y + Utility.RandomMinMax(-offset, offset);
                    break;
                case Direction.South:
                    x = X + Utility.RandomMinMax(-offset, offset);
                    y = Y + range;
                    break;
                case Direction.East:
                    x = X + range;
                    y = Y + Utility.RandomMinMax(-offset, offset);
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

                z = Map.GetAverageZ(x, y);
                Point3D p = new Point3D(x, y, z);

                if (Spells.SpellHelper.AdjustField(ref p, Map, 12, false))
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
            int x = X;
            int y = Y;

            for (int i = 0; i < path.Directions.Length; ++i)
            {
                Movement.Movement.Offset(path.Directions[i], ref x, ref y);
                IPoint3D p = new Point3D(x, y, Map.GetAverageZ(x, y)) as IPoint3D;

                Timer.DelayCall(TimeSpan.FromMilliseconds(time), new TimerStateCallback(ManaDrainEffects_Callback), new object[] { p, Map });

                time += 200;
            }
        }

        private void ManaDrainEffects_Callback(object o)
        {
            object[] objs = o as object[];
            IPoint3D p = objs[0] as IPoint3D;
            Map map = objs[1] as Map;

            ManaDrainItem item = new ManaDrainItem(Utility.RandomList(6913, 6915, 6917, 6919), this);
            Spells.SpellHelper.GetSurfaceTop(ref p);

            item.MoveToWorld(new Point3D(p), Map);
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
                Static = new Static(ItemID + 1);
                Static.MoveToWorld(Location, Map);

                IPooledEnumerable eable = Map.GetMobilesInRange(Location, 0);

                foreach (Mobile m in eable)
                {
                    OnMoveOver(m);
                }
                eable.Free();
            }

            public override bool OnMoveOver(Mobile m)
            {
                if ((m is PlayerMobile || (m is BaseCreature && !((BaseCreature)m).IsMonster)) && m.CanBeHarmful(Owner, false))
                {
                    if (m is PlayerMobile && Services.TownCryer.TownCryerSystem.UnderMysteriousPotionEffects((PlayerMobile)m, true))
                    {
                        m.SayTo(m, 1158288, 1154); // *You resist Cora's attack!*
                    }
                    else
                    {
                        m.FixedParticles(0x3779, 10, 25, 5002, EffectLayer.Head);
                        m.Mana = 0;
                    }
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
                Mobile m = RandomPlayerWithLootingRights();

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

        public override void OnKilledBy(Mobile mob)
        {
            base.OnKilledBy(mob);

            if (Siege.SiegeShard && mob is PlayerMobile)
            {
                int chance = Engines.Despise.DespiseBoss.ArtifactChance + Math.Min(10, ((PlayerMobile)mob).Luck / 180);

                if (chance >= Utility.Random(100))
                {
                    Type t = Engines.Despise.DespiseBoss.Artifacts[Utility.Random(Engines.Despise.DespiseBoss.Artifacts.Length)];

                    if (t != null)
                    {
                        Item arty = Loot.Construct(t);

                        if (arty != null)
                        {
                            Container pack = mob.Backpack;

                            if (pack == null || !pack.TryDropItem(mob, arty, false))
                            {
                                mob.BankBox.DropItem(arty);
                                mob.SendMessage("An artifact has been placed in your bankbox!");
                            }
                            else
                                mob.SendLocalizedMessage(1153440); // An artifact has been placed in your backpack!
                        }
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
