using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a lady melisande corpse")]
    public class LadyMelisande : BasePeerless
    {
        [Constructable]
        public LadyMelisande()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Lady Melisande";
            Body = 0x102;
            BaseSoundID = 451;

            SetStr(400, 1000);
            SetDex(300, 400);
            SetInt(1500, 1700);

            SetHits(100000);

            SetDamage(11, 18);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 40, 60);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 55, 65);
            SetResistance(ResistanceType.Poison, 70, 75);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.Wrestling, 100, 105);
            SetSkill(SkillName.Tactics, 100, 105);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Magery, 120);
            SetSkill(SkillName.EvalInt, 120);
            SetSkill(SkillName.Meditation, 120);
            SetSkill(SkillName.Necromancy, 120);
            SetSkill(SkillName.SpiritSpeak, 120);

            Timer.DelayCall(TimeSpan.FromSeconds(1), SpawnSatyrs);

            Fame = 25000;
            Karma = -25000;

            SetAreaEffect(AreaEffect.AuraOfNausea);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 8);
            AddLoot(LootPack.Parrot, 1);
            AddLoot(LootPack.ArcanistScrolls, Utility.RandomMinMax(1, 6));
            AddLoot(LootPack.PeerlessResource, 8);
            AddLoot(LootPack.Talisman, 5);
            AddLoot(LootPack.LootItem<DiseasedBark>());
            AddLoot(LootPack.LootItem<EternallyCorruptTree>());
            AddLoot(LootPack.LootItem<MelisandesFermentedWine>(4, 8));
            AddLoot(LootPack.LootItem<ParrotItem>(60.0));
            AddLoot(LootPack.RandomLootItem(new[] { typeof(MelisandesHairDye), typeof(MelisandesCorrodedHatchet), typeof(AlbinoSquirrelImprisonedInCrystal) }, 22.25, 1));
        }

        public override void OnThink()
        {
            base.OnThink();

            Mobile combatant = Combatant as Mobile;

            if (combatant != null)
            {
                if (CanTakeLife(combatant))
                    TakeLife(combatant);

                if (CanSmackTalk())
                    SmackTalk();
            }
        }

        public override void SetLocation(Point3D newLocation, bool isTeleport)
        {
            if (newLocation.Z > -10)
                base.SetLocation(newLocation, isTeleport);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (willKill)
            {
                SpawnHelper(new Reaper(), 6490, 948, 19);
                SpawnHelper(new InsaneDryad(), 6497, 946, 17);
                SpawnHelper(new StoneHarpy(), 6511, 946, 28);

                Say(1075118); // Noooooo!  You shall never defeat me.  Even if I should fall, my tree will sustain me and I will rise again.
            }

            base.OnDamage(amount, from, willKill);
        }

        public override bool GivesMLMinorArtifact => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override int TreasureMapLevel => 5;

        public LadyMelisande(Serial serial)
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

        #region Smack Talk
        private DateTime m_NextSmackTalk;

        public bool CanSmackTalk()
        {
            if (m_NextSmackTalk > DateTime.UtcNow)
                return false;

            if (Combatant == null)
                return false;

            return Hits > 0.5 * HitsMax;
        }

        public void SmackTalk()
        {
            Say(Utility.RandomMinMax(1075102, 1075115)); // Muahahahaha!  I'll feast on your flesh.

            m_NextSmackTalk = DateTime.UtcNow + TimeSpan.FromSeconds(2 + Utility.RandomDouble() * 3);
        }

        #endregion

        #region Take Life
        private DateTime m_NextTakeLife;

        public bool CanTakeLife(Mobile from)
        {
            if (m_NextTakeLife > DateTime.UtcNow)
                return false;

            if (!CanBeHarmful(from))
                return false;

            if (Hits > 0.1 * HitsMax || Hits < 0.025 * HitsMax)
                return false;

            return true;
        }

        public void TakeLife(Mobile from)
        {
            Hits += from.Hits / (from.Player ? 2 : 6);

            FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            PlaySound(0x1F2);

            Say(1075117);  // Muahahaha!  Your life essence is MINE!
            Say(1075120); // An unholy aura surrounds Lady Melisande as her wounds begin to close.

            m_NextTakeLife = DateTime.UtcNow + TimeSpan.FromSeconds(15 + Utility.RandomDouble() * 45);
        }

        #endregion

        #region Helpers
        public override bool CanSpawnHelpers => true;

        public override int MaxHelpersWaves => 1;

        public override void SpawnHelpers()
        {
            int count = 4;

            if (Altar != null)
            {
                count = Math.Min(Altar.Fighters.Count, 4);

                for (int i = 0; i < count; i++)
                {
                    Mobile fighter = Altar.Fighters[i];

                    if (CanBeHarmful(fighter))
                    {
                        EnslavedSatyr satyr = new EnslavedSatyr
                        {
                            FightMode = FightMode.Closest
                        };
                        SpawnHelper(satyr, GetSpawnPosition(fighter.Location, fighter.Map, 2));

                        satyr.Combatant = fighter;

                        fighter.SendLocalizedMessage(1075116); // A twisted satyr scrambles onto the branch beside you and attacks!
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                    SpawnHelper(new EnslavedSatyr(), 4);
            }
        }

        public void SpawnSatyrs()
        {
            SpawnHelper(new EnslavedSatyr(), 6485, 945, 19);
            SpawnHelper(new EnslavedSatyr(), 6486, 948, 22);
            SpawnHelper(new EnslavedSatyr(), 6487, 945, 17);
            SpawnHelper(new EnslavedSatyr(), 6488, 947, 23);
        }
        #endregion
    }
}
