using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a leviathan corpse")]
    public class Leviathan : BaseCreature
    {
        private static readonly Type[] m_Artifacts = new Type[]
        {
            // Decorations
            typeof(CandelabraOfSouls),
            typeof(GhostShipAnchor),
            typeof(GoldBricks),
            typeof(PhillipsWoodenSteed),
            typeof(SeahorseStatuette),
            typeof(ShipModelOfTheHMSCape),
            typeof(AdmiralsHeartyRum),

            // Equipment
            typeof(AlchemistsBauble),
            typeof(ArcticDeathDealer),
            typeof(BlazeOfDeath),
            typeof(BurglarsBandana),
            typeof(CaptainQuacklebushsCutlass),
            typeof(CavortingClub),
            typeof(DreadPirateHat),
            typeof(EnchantedTitanLegBone),
            typeof(GwennosHarp),
            typeof(IolosLute),
            typeof(LunaLance),
            typeof(NightsKiss),
            typeof(NoxRangersHeavyCrossbow),
            typeof(PolarBearMask),
            typeof(VioletCourage)
        };

        private Mobile m_Fisher;
        private DateTime m_NextWaterBall;

        [Constructable]
        public Leviathan()
            : this(null)
        {
        }

        [Constructable]
        public Leviathan(Mobile fisher)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.m_Fisher = fisher;
            this.m_NextWaterBall = DateTime.UtcNow;

            // May not be OSI accurate; mostly copied from krakens
            this.Name = "a leviathan";
            this.Body = 77;
            this.BaseSoundID = 353;

            this.Hue = 0x481;

            this.SetStr(1000);
            this.SetDex(501, 520);
            this.SetInt(501, 515);

            this.SetHits(1500);

            this.SetDamage(25, 33);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Cold, 30);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 45, 55);
            this.SetResistance(ResistanceType.Cold, 45, 55);
            this.SetResistance(ResistanceType.Poison, 35, 45);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.EvalInt, 97.6, 107.5);
            this.SetSkill(SkillName.Magery, 97.6, 107.5);
            this.SetSkill(SkillName.MagicResist, 97.6, 107.5);
            this.SetSkill(SkillName.Meditation, 97.6, 107.5);
            this.SetSkill(SkillName.Tactics, 97.6, 107.5);
            this.SetSkill(SkillName.Wrestling, 97.6, 107.5);

            this.Fame = 22500;
            this.Karma = -22500;

            this.VirtualArmor = 50;

            this.CanSwim = true;
            this.CantWalk = true;

            this.PackItem(new MessageInABottle());

            Rope rope = new Rope();
            rope.ItemID = 0x14F8;
            this.PackItem(rope);

            rope = new Rope();
            rope.ItemID = 0x14FA;
            this.PackItem(rope);
        }

        public Leviathan(Serial serial)
            : base(serial)
        {
        }

        public static Type[] Artifacts { get { return m_Artifacts; } }

        public Mobile Fisher
        {
            get { return this.m_Fisher; }
            set { this.m_Fisher = value; }
        }

        public override int DefaultHitsRegen
        {
            get
            {
                int regen = base.DefaultHitsRegen;

                return IsParagon ? regen : regen += 40;
            }
        }

        public override int DefaultStamRegen
        {
            get
            {
                int regen = base.DefaultStamRegen;

                return IsParagon ? regen : regen += 40;
            }
        }

        public override int DefaultManaRegen
        {
            get
            {
                int regen = base.DefaultManaRegen;

                return IsParagon ? regen : regen += 40;
            }
        }

        public override bool HasBreath { get { return true; } }
        public override int BreathPhysicalDamage { get { return 70; } }
        public override int BreathColdDamage { get { return 30; } }
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathEffectHue { get { return 0x1ED; } }
        public override double BreathDamageScalar { get { return 0.05; } }
        public override double BreathMinDelay { get { return 5.0; } }
        public override double BreathMaxDelay { get { return 7.5; } }
        public override double TreasureMapChance { get { return 0.25; } }
        public override int TreasureMapLevel { get { return 5; } }

        public override void OnActionCombat()
        {
            Mobile combatant = this.Combatant as Mobile;

            if (combatant == null || combatant.Deleted || combatant.Map != this.Map || !this.InRange(combatant, 12) || !this.CanBeHarmful(combatant) || !this.InLOS(combatant))
                return;

            if (DateTime.UtcNow >= this.m_NextWaterBall)
            {
                double damage = combatant.Hits * 0.3;

                if (damage < 10.0)
                    damage = 10.0;
                else if (damage > 40.0)
                    damage = 40.0;

                this.DoHarmful(combatant);
                this.MovingParticles(combatant, 0x36D4, 5, 0, false, false, 195, 0, 9502, 3006, 0, 0, 0);
                AOS.Damage(combatant, this, (int)damage, 100, 0, 0, 0, 0);

                if (combatant is PlayerMobile && combatant.Mount != null)
                {
                    (combatant as PlayerMobile).SetMountBlock(BlockMountType.DismountRecovery, TimeSpan.FromSeconds(10), true);
                }

                m_NextWaterBall = DateTime.UtcNow + TimeSpan.FromMinutes(1);
            }
        }

        public static void GiveArtifactTo(Mobile m)
        {
            Item item = Loot.Construct(m_Artifacts);

            if (item == null)
                return;

            // TODO: Confirm messages
            if (m.AddToBackpack(item))
                m.SendMessage("As a reward for slaying the mighty leviathan, an artifact has been placed in your backpack.");
            else
                m.SendMessage("As your backpack is full, your reward for destroying the legendary leviathan has been placed at your feet.");
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 5);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextWaterBall = DateTime.UtcNow;
        }

        public override void OnKilledBy(Mobile mob)
        {
            base.OnKilledBy(mob);

            if (Paragon.CheckArtifactChance(mob, this))
            {
                GiveArtifactTo(mob);

                if (mob == this.m_Fisher)
                    this.m_Fisher = null;
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (this.m_Fisher != null && 25 > Utility.Random(100))
                GiveArtifactTo(this.m_Fisher);

            this.m_Fisher = null;
        }
    }
}