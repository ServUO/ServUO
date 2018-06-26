using Server;
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Shadowguard
{
    public class ShadowguardPirate : BaseCreature
    {
        public ShadowguardPirate() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("male");
            Title = "the Pirate";

            Body = 0x190;
            Hue = Utility.RandomSkinHue();

            SetStr(386, 400);
            SetDex(151, 165);
            SetInt(161, 175);

            SetHits(1200);

            SetDamage(15, 21);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 25, 30);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 125.0);
            SetSkill(SkillName.Fencing, 46.0, 77.5);
            SetSkill(SkillName.Macing, 35.0, 57.5);
            SetSkill(SkillName.Poisoning, 60.0, 82.5);
            SetSkill(SkillName.MagicResist, 83.5, 92.5);
            SetSkill(SkillName.Swords, 125.0);
            SetSkill(SkillName.Tactics, 125.0);
            SetSkill(SkillName.Lumberjacking, 125.0);

            Fame = 1000;
            Karma = -1000;

            AddItem(new ExecutionersAxe());

            AddItem(new Boots(Utility.RandomNeutralHue()));
            AddItem(new ShortPants());
            AddItem(new FancyShirt());
            AddItem(new TricorneHat());

            Fame = 5000;
            Karma = -5000;

            Utility.AssignRandomHair(this);
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 3);
        }

        public override bool AlwaysMurderer { get { return true; } }

        public ShadowguardPirate(Serial serial) : base(serial)
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

    public class ShantyThePirate : ShadowguardPirate
    {
        [Constructable]
        public ShantyThePirate()
        {
            Name = "Shanty";

            SetHits(10000);

            SetSkill(SkillName.Fencing, 120.0);
            SetSkill(SkillName.Macing, 120.0);
            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Swords, 120.0);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 120.0);

            Fame = 15000;
            Karma = -15000;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
        }

        public ShantyThePirate(Serial serial) : base(serial)
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

    public class VileWaterElemental : WaterElemental
    {
        public override bool CanMoveOverObstacles { get { return false; } }

        [Constructable]
        public VileWaterElemental()
        {
            Name = "a vile water elemental";
            Hue = 1916;
            Body = 13;
        }

        public override bool DeleteCorpseOnDeath { get { return true; } }

        public override bool OnBeforeDeath()
        {
            FountainEncounter encounter = ShadowguardController.GetEncounter(this.Location, this.Map) as FountainEncounter;

            if (encounter != null)
            {
                var canal = new ShadowguardCanal();
                canal.MoveToWorld(this.Location, this.Map);
                encounter.AddShadowguardCanal(canal);
            }

            return base.OnBeforeDeath();
        }

        public VileWaterElemental(Serial serial) : base(serial)
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

    public class HurricaneElemental : VileWaterElemental
    {
        public override bool CanMoveOverObstacles { get { return false; } }

        [Constructable]
        public HurricaneElemental()
        {
            Name = "a hurricane elemental";
            Body = 16;

            SetStr(400, 500);
            SetDex(140, 250);
            SetInt(130, 150);

            SetHits(550, 700);
            SetMana(650, 750);

            SetDamage(14, 16);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.Wrestling, 95.0, 110.0);
            SetSkill(SkillName.Tactics, 95.0, 110.0);
            SetSkill(SkillName.Magery, 95.0, 110.0);
            SetSkill(SkillName.EvalInt, 95.0, 110.0);
            SetSkill(SkillName.Parry, 95.0, 110.0);
            SetSkill(SkillName.DetectHidden, 63.0);
        }

        public HurricaneElemental(Serial serial)
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

    public class VileTreefellow : FeralTreefellow
    {
        [Constructable]
        public VileTreefellow()
        {
            Name = "a vile treefellow";

            SetDamage(12, 16);

            //Missing Attr - Hit Point Reg: 35, Mana Reg: 20

            SetResistance(ResistanceType.Physical, 2);
            SetResistance(ResistanceType.Cold, 5);
            SetResistance(ResistanceType.Poison, 3);
            SetResistance(ResistanceType.Energy, 2);

            SetSkill(SkillName.MagicResist, 40.1, 55.0);
            SetSkill(SkillName.Tactics, 65.1, 90.0);
            SetSkill(SkillName.Wrestling, 65.1, 105.0);
            SetSkill(SkillName.Poisoning, 100.0);
            SetSkill(SkillName.DetectHidden, 40.0, 45.0);
            SetSkill(SkillName.Parry, 55.0, 60.0);

            SetWeaponAbility(WeaponAbility.Dismount);
            SetWeaponAbility(WeaponAbility.ForceOfNature);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            Paralyze(defender);
        }

        #region Paralyze
        private void Paralyze(Mobile defender)
        {
            defender.Paralyze(TimeSpan.FromSeconds(Utility.Random(3)));

            defender.FixedEffect(0x376A, 6, 1);
            defender.PlaySound(0x204);

            defender.SendLocalizedMessage(1060164); // The attack has temporarily paralyzed you!
        }
        #endregion

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 3);
        }

        public VileTreefellow(Serial serial) : base(serial)
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

    [CorpseName("a magical corpse")]
    public class EnsorcelledArmor : BaseCreature
    {
        public ArmoryEncounter Encounter { get; set; }

        [Constructable]
        public EnsorcelledArmor() : this(null)
        {
        }

        [Constructable]
        public EnsorcelledArmor(ArmoryEncounter encounter) : base(AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.2, 0.4)
        {
            Encounter = encounter;
            Name = "ensorcelled armor";
            BaseSoundID = 412;

            Body = 0x190;
            SetStr(386, 400);
            SetDex(151, 165);
            SetInt(161, 175);

            SetDamage(15, 21);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 25, 30);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 125.0);
            SetSkill(SkillName.Fencing, 46.0, 77.5);
            SetSkill(SkillName.Macing, 35.0, 57.5);
            SetSkill(SkillName.Poisoning, 60.0, 82.5);
            SetSkill(SkillName.MagicResist, 83.5, 92.5);
            SetSkill(SkillName.Swords, 125.0);
            SetSkill(SkillName.Tactics, 125.0);
            SetSkill(SkillName.Lumberjacking, 125.0);

            var helm = new CloseHelm();
            helm.Hue = 0x96D;
            AddItem(helm);

            var arms = new PlateArms();
            arms.Hue = 0x96D;
            AddItem(arms);

            var legs = new PlateLegs();
            legs.Hue = 0x96D;
            AddItem(legs);

            var tunic = new PlateChest();
            tunic.Hue = 0x96D;
            AddItem(tunic);

            var gorget = new PlateGorget();
            gorget.Hue = 0x96D;
            AddItem(gorget);

            var golves = new PlateGloves();
            golves.Hue = 0x96D;
            AddItem(golves);

            var halberd = new Halberd();
            halberd.Hue = 0x96D;
            AddItem(halberd);

            AddItem(new HalfApron(728));

            Fame = 8500;
            Karma = -8500;
        }

        public override bool AlwaysMurderer { get { return true; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (0.66 > Utility.RandomDouble() && Encounter != null)
                c.DropItem(new Phylactery());
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 3);
        }

        public EnsorcelledArmor(Serial serial) : base(serial)
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

    public class VileDrake : Drake
    {
        public VileDrake()
        {
            Name = "a vile drake";

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 90, 100);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.MagicResist, 65.0, 80.0);
            SetSkill(SkillName.Tactics, 65.0, 90.0);
            SetSkill(SkillName.Wrestling, 110.0, 130.0);
            SetSkill(SkillName.DetectHidden, 50.6);
            SetSkill(SkillName.Parry, 65.0, 75.0);

            SetWeaponAbility(WeaponAbility.DoubleStrike);
            SetWeaponAbility(WeaponAbility.TalonStrike);
        }

        public override void OnDeath(Container c)
        {
            BelfryEncounter encounter = ShadowguardController.GetEncounter(c.Location, c.Map) as BelfryEncounter;

            if (encounter != null)
            {
                c.DropItem(new MagicDrakeWing());
            }

            base.OnDeath(c);
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 3);
        }

        public VileDrake(Serial serial) : base(serial)
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

    public class ShadowguardGreaterDragon : GreaterDragon
    {
        public ShadowguardGreaterDragon()
        {
            Tamable = false;

            SetHits(9800, 10999);

            //Missing Attr - Hit Point Reg: 20, Stamina Reg: 43, Mana Reg: 43

            SetDamage(29, 38);

            SetSkill(SkillName.EvalInt, 110.0, 145.0);
            SetSkill(SkillName.Magery, 110.0, 145.0);
            SetSkill(SkillName.MagicResist, 110.0, 150.0);
            SetSkill(SkillName.Tactics, 110.0, 155.0);
            SetSkill(SkillName.Wrestling, 115.0, 155.0);
            SetSkill(SkillName.DetectHidden, 120.0);
            SetSkill(SkillName.Parry, 120.0);
        }

        public override double TeleportChance { get { return 0; } }

        public override void OnThink()
        {
            base.OnThink();

            BelfryEncounter encounter = ShadowguardController.GetEncounter(this.Location, this.Map) as BelfryEncounter;

            if (encounter != null && this.Z == -20)
            {
                Point3D p = encounter.SpawnPoints[0];
                encounter.ConvertOffset(ref p);

                this.MoveToWorld(p, this.Map);
            }
        }

        protected override bool OnMove(Direction d)
        {
            if (ShadowguardController.GetEncounter(this.Location, this.Map) != null)
            {
                int x = this.X;
                int y = this.Y;

                Movement.Movement.Offset(d, ref x, ref y);

                Point3D p = new Point3D(x, y, this.Map.GetAverageZ(x, y));
                int z = p.Z;

                IPooledEnumerable eable = this.Map.GetItemsInRange(p, 0);

                foreach (Item item in eable)
                {
                    if (item.Z + item.ItemData.CalcHeight > z)
                    {
                        z = item.Z + item.ItemData.CalcHeight;
                    }
                }

                StaticTile[] staticTiles = this.Map.Tiles.GetStaticTiles(x, y, true);

                foreach (StaticTile tile in staticTiles)
                {
                    ItemData itemData = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                    if (tile.Z + itemData.CalcHeight > z)
                        z = tile.Z + itemData.CalcHeight;
                }

                eable.Free();

                if (z < this.Z)
                    return false;
            }

            return base.OnMove(d);
        }

        public override void Damage(int amount, Mobile from, bool informmount, bool checkfizzle)
        {
            if (from == null || (ShadowguardController.GetEncounter(this.Location, this.Map) != null && this.Z == from.Z))
            {
                base.Damage(amount, from, informmount, checkfizzle);
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 8);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (this.Map != null && 0.25 > Utility.RandomDouble())
            {
                int pushRange = Utility.RandomMinMax(2, 4);

                Direction d = Utility.GetDirection(this, defender);
                int x = defender.X;
                int y = defender.Y;

                for (int i = 0; i < pushRange; i++)
                {
                    Movement.Movement.Offset(d, ref x, ref y);
                }

                int z = this.Map.GetAverageZ(x, y);

                IPooledEnumerable eable = this.Map.GetItemsInRange(new Point3D(x, y, z), 0);

                foreach (Item item in eable)
                {
                    if (item.Z + item.ItemData.CalcHeight > z)
                    {
                        z = item.Z + item.ItemData.CalcHeight;
                    }
                }

                eable.Free();

                StaticTile[] staticTiles = this.Map.Tiles.GetStaticTiles(x, y, true);

                foreach (StaticTile tile in staticTiles)
                {
                    ItemData itemData = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                    if (tile.Z + itemData.CalcHeight > z)
                        z = tile.Z + itemData.CalcHeight;
                }

                defender.MoveToWorld(new Point3D(x, y, Z), this.Map);
            }
        }

        public ShadowguardGreaterDragon(Serial serial)
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

    public class LadyMinax : BaseCreature
    {
        public LadyMinax()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Minax";
            Title = "the Enchantress";

            Body = 0x191;
            Hue = Race.RandomSkinHue();
            HairItemID = 0x203C;
            HairHue = Race.RandomHairHue();

            SetStr(386, 400);
            SetDex(151, 165);
            SetInt(161, 175);

            SetDamage(15, 21);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 25, 30);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Magery, 125.0);
            SetSkill(SkillName.EvalInt, 125.0);
            SetSkill(SkillName.Meditation, 125.0);
            SetSkill(SkillName.Anatomy, 125.0);
            SetSkill(SkillName.Fencing, 46.0, 77.5);
            SetSkill(SkillName.Macing, 35.0, 57.5);
            SetSkill(SkillName.Poisoning, 60.0, 82.5);
            SetSkill(SkillName.MagicResist, 83.5, 92.5);
            SetSkill(SkillName.Swords, 125.0);
            SetSkill(SkillName.Tactics, 125.0);
            SetSkill(SkillName.Lumberjacking, 125.0);

            SetWearable(new Cloak(), 1157);
            SetWearable(new Boots(), 1175);
            SetWearable(new FemaleStuddedChest(), 1175);
            SetWearable(new LeatherGloves(), 1157);
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override double TeleportChance { get { return 0; } }

        protected override bool OnMove(Direction d)
        {
            RoofEncounter encounter = ShadowguardController.GetEncounter(this.Location, this.Map) as RoofEncounter;

            if (encounter != null)
            {
                Point3D spawn = encounter.SpawnPoints[0];

                int x = this.X;
                int y = this.Y;

                Movement.Movement.Offset(d, ref x, ref y);

                Point3D p = new Point3D(x, y, this.Map.GetAverageZ(x, y));
                int z = p.Z;

                if (p.Y < spawn.Y - 5 || p.Y > spawn.Y + 4 || p.X > spawn.X + 4 || p.X < spawn.X - 5)
                    return false;

                IPooledEnumerable eable = this.Map.GetItemsInRange(p, 0);
                Item i = null;

                foreach (Item item in eable)
                {
                    if (item.Z + item.ItemData.CalcHeight > z)
                    {
                        i = item;
                        z = item.Z + item.ItemData.CalcHeight;
                    }
                }

                StaticTile[] staticTiles = this.Map.Tiles.GetStaticTiles(x, y, true);

                foreach (StaticTile tile in staticTiles)
                {
                    ItemData itemData = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                    if (tile.Z + itemData.CalcHeight > z)
                        z = tile.Z + itemData.CalcHeight;
                }

                eable.Free();

                if (z < this.Z)
                    return false;
            }

            return base.OnMove(d);
        }

        public override void OnThink()
        {
            base.OnThink();

            RoofEncounter encounter = ShadowguardController.GetEncounter(this.Location, this.Map) as RoofEncounter;

            if (encounter != null)
            {
                Point3D spawn = encounter.SpawnPoints[0];
                Point3D p = this.Location;
                encounter.ConvertOffset(ref spawn);

                if (this.Z < 30 || p.Y < spawn.Y - 5 || p.Y > spawn.Y + 4 || p.X > spawn.X + 4 || p.X < spawn.X - 5)
                {
                    this.MoveToWorld(spawn, Map.TerMur);
                }
            }
        }

        public override void Damage(int amount, Mobile from, bool informMount, bool checkfizzle)
        {
            RoofEncounter encounter = ShadowguardController.GetEncounter(this.Location, this.Map) as RoofEncounter;

            if (encounter != null && from != null)
            {
                from.SendLocalizedMessage(1156254); // Minax laughs as she deflects your puny attacks! Defeat her minions to close the Time Gate!
                return;
            }

            base.Damage(amount, from, informMount, checkfizzle);
        }

        public LadyMinax(Serial serial)
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
