using System;

namespace Server.Spells
{
    public class Initializer
    {
        public static void Initialize()
        {
            // First circle
            Register(00, typeof(First.ClumsySpell));
            Register(01, typeof(First.CreateFoodSpell));
            Register(02, typeof(First.FeeblemindSpell));
            Register(03, typeof(First.HealSpell));
            Register(04, typeof(First.MagicArrowSpell));
            Register(05, typeof(First.NightSightSpell));
            Register(06, typeof(First.ReactiveArmorSpell));
            Register(07, typeof(First.WeakenSpell));

            // Second circle
            Register(08, typeof(Second.AgilitySpell));
            Register(09, typeof(Second.CunningSpell));
            Register(10, typeof(Second.CureSpell));
            Register(11, typeof(Second.HarmSpell));
            Register(12, typeof(Second.MagicTrapSpell));
            Register(13, typeof(Second.RemoveTrapSpell));
            Register(14, typeof(Second.ProtectionSpell));
            Register(15, typeof(Second.StrengthSpell));

            // Third circle
            Register(16, typeof(Third.BlessSpell));
            Register(17, typeof(Third.FireballSpell));
            Register(18, typeof(Third.MagicLockSpell));
            Register(19, typeof(Third.PoisonSpell));
            Register(20, typeof(Third.TelekinesisSpell));
            Register(21, typeof(Third.TeleportSpell));
            Register(22, typeof(Third.UnlockSpell));
            Register(23, typeof(Third.WallOfStoneSpell));

            // Fourth circle
            Register(24, typeof(Fourth.ArchCureSpell));
            Register(25, typeof(Fourth.ArchProtectionSpell));
            Register(26, typeof(Fourth.CurseSpell));
            Register(27, typeof(Fourth.FireFieldSpell));
            Register(28, typeof(Fourth.GreaterHealSpell));
            Register(29, typeof(Fourth.LightningSpell));
            Register(30, typeof(Fourth.ManaDrainSpell));
            Register(31, typeof(Fourth.RecallSpell));

            // Fifth circle
            Register(32, typeof(Fifth.BladeSpiritsSpell));
            Register(33, typeof(Fifth.DispelFieldSpell));
            Register(34, typeof(Fifth.IncognitoSpell));
            Register(35, typeof(Fifth.MagicReflectSpell));
            Register(36, typeof(Fifth.MindBlastSpell));
            Register(37, typeof(Fifth.ParalyzeSpell));
            Register(38, typeof(Fifth.PoisonFieldSpell));
            Register(39, typeof(Fifth.SummonCreatureSpell));

            // Sixth circle
            Register(40, typeof(Sixth.DispelSpell));
            Register(41, typeof(Sixth.EnergyBoltSpell));
            Register(42, typeof(Sixth.ExplosionSpell));
            Register(43, typeof(Sixth.InvisibilitySpell));
            Register(44, typeof(Sixth.MarkSpell));
            Register(45, typeof(Sixth.MassCurseSpell));
            Register(46, typeof(Sixth.ParalyzeFieldSpell));
            Register(47, typeof(Sixth.RevealSpell));

            // Seventh circle
            Register(48, typeof(Seventh.ChainLightningSpell));
            Register(49, typeof(Seventh.EnergyFieldSpell));
            Register(50, typeof(Seventh.FlameStrikeSpell));
            Register(51, typeof(Seventh.GateTravelSpell));
            Register(52, typeof(Seventh.ManaVampireSpell));
            Register(53, typeof(Seventh.MassDispelSpell));
            Register(54, typeof(Seventh.MeteorSwarmSpell));
            Register(55, typeof(Seventh.PolymorphSpell));

            // Eighth circle
            Register(56, typeof(Eighth.EarthquakeSpell));
            Register(57, typeof(Eighth.EnergyVortexSpell));
            Register(58, typeof(Eighth.ResurrectionSpell));
            Register(59, typeof(Eighth.AirElementalSpell));
            Register(60, typeof(Eighth.SummonDaemonSpell));
            Register(61, typeof(Eighth.EarthElementalSpell));
            Register(62, typeof(Eighth.FireElementalSpell));
            Register(63, typeof(Eighth.WaterElementalSpell));

            if (Core.AOS)
            {
                // Necromancy spells
                Register(100, typeof(Necromancy.AnimateDeadSpell));
                Register(101, typeof(Necromancy.BloodOathSpell));
                Register(102, typeof(Necromancy.CorpseSkinSpell));
                Register(103, typeof(Necromancy.CurseWeaponSpell));
                Register(104, typeof(Necromancy.EvilOmenSpell));
                Register(105, typeof(Necromancy.HorrificBeastSpell));
                Register(106, typeof(Necromancy.LichFormSpell));
                Register(107, typeof(Necromancy.MindRotSpell));
                Register(108, typeof(Necromancy.PainSpikeSpell));
                Register(109, typeof(Necromancy.PoisonStrikeSpell));
                Register(110, typeof(Necromancy.StrangleSpell));
                Register(111, typeof(Necromancy.SummonFamiliarSpell));
                Register(112, typeof(Necromancy.VampiricEmbraceSpell));
                Register(113, typeof(Necromancy.VengefulSpiritSpell));
                Register(114, typeof(Necromancy.WitherSpell));
                Register(115, typeof(Necromancy.WraithFormSpell));

                if (Core.SE)
                    Register(116, typeof(Necromancy.ExorcismSpell));

                // Paladin abilities
                Register(200, typeof(Chivalry.CleanseByFireSpell));
                Register(201, typeof(Chivalry.CloseWoundsSpell));
                Register(202, typeof(Chivalry.ConsecrateWeaponSpell));
                Register(203, typeof(Chivalry.DispelEvilSpell));
                Register(204, typeof(Chivalry.DivineFurySpell));
                Register(205, typeof(Chivalry.EnemyOfOneSpell));
                Register(206, typeof(Chivalry.HolyLightSpell));
                Register(207, typeof(Chivalry.NobleSacrificeSpell));
                Register(208, typeof(Chivalry.RemoveCurseSpell));
                Register(209, typeof(Chivalry.SacredJourneySpell));

                if (Core.SE)
                {
                    // Samurai abilities
                    Register(400, typeof(Bushido.HonorableExecution));
                    Register(401, typeof(Bushido.Confidence));
                    Register(402, typeof(Bushido.Evasion));
                    Register(403, typeof(Bushido.CounterAttack));
                    Register(404, typeof(Bushido.LightningStrike));
                    Register(405, typeof(Bushido.MomentumStrike));

                    // Ninja abilities
                    Register(500, typeof(Ninjitsu.FocusAttack));
                    Register(501, typeof(Ninjitsu.DeathStrike));
                    Register(502, typeof(Ninjitsu.AnimalForm));
                    Register(503, typeof(Ninjitsu.KiAttack));
                    Register(504, typeof(Ninjitsu.SurpriseAttack));
                    Register(505, typeof(Ninjitsu.Backstab));
                    Register(506, typeof(Ninjitsu.Shadowjump));
                    Register(507, typeof(Ninjitsu.MirrorImage));
                }

                if (Core.ML)
                {
                    Register(600, typeof(Spellweaving.ArcaneCircleSpell));
                    Register(601, typeof(Spellweaving.GiftOfRenewalSpell));
                    Register(602, typeof(Spellweaving.ImmolatingWeaponSpell));
                    Register(603, typeof(Spellweaving.AttuneWeaponSpell));
                    Register(604, typeof(Spellweaving.ThunderstormSpell));
                    Register(605, typeof(Spellweaving.NatureFurySpell));
                    Register(606, typeof(Spellweaving.SummonFeySpell));
                    Register(607, typeof(Spellweaving.SummonFiendSpell));
                    Register(608, typeof(Spellweaving.ReaperFormSpell));
                    Register( 609, typeof( Spellweaving.WildfireSpell ) );
                    Register(610, typeof(Spellweaving.EssenceOfWindSpell));
                    Register( 611, typeof( Spellweaving.DryadAllureSpell ) );
                    Register(612, typeof(Spellweaving.EtherealVoyageSpell));
                    Register(613, typeof(Spellweaving.WordOfDeathSpell));
                    Register(614, typeof(Spellweaving.GiftOfLifeSpell));
                    Register( 615, typeof( Spellweaving.ArcaneEmpowermentSpell ) );
                }

                if (Core.SA)
                {
                    Register(677, typeof(Mystic.NetherBoltSpell));
                    Register(678, typeof(Mystic.HealingStoneSpell));
                    Register(679, typeof(Mystic.PurgeMagicSpell));
                    Register(680, typeof(Mystic.EnchantSpell));
                    Register(681, typeof(Mystic.SleepSpell));
                    Register(682, typeof(Mystic.EagleStrikeSpell));
                    Register(683, typeof(Mystic.AnimatedWeaponSpell));
                    Register(684, typeof(Mystic.StoneFormSpell));
                    Register(685, typeof(Mystic.SpellTriggerSpell));
                    Register(686, typeof(Mystic.MassSleepSpell));
                    Register(687, typeof(Mystic.CleansingWindsSpell));
                    Register(688, typeof(Mystic.BombardSpell));
                    Register(689, typeof(Mystic.SpellPlagueSpell));
                    Register(690, typeof(Mystic.HailStormSpell));
                    Register(691, typeof(Mystic.NetherCycloneSpell));
                    Register(692, typeof(Mystic.RisingColossusSpell));

                    Register(700, typeof(SkillMasteries.InspireSpell));
                    Register(701, typeof(SkillMasteries.InvigorateSpell));
                    Register(702, typeof(SkillMasteries.ResilienceSpell));
                    Register(703, typeof(SkillMasteries.PerseveranceSpell));
                    Register(704, typeof(SkillMasteries.TribulationSpell));
                    Register(705, typeof(SkillMasteries.DespairSpell));
                }

                if (Core.TOL)
                {
                    /*Register(706, typeof(SkillMasteries.DeathRaySpell));
                    Register(707, typeof(SkillMasteries.EtherealBurstSpell));
                    Register(708, typeof(SkillMasteries.NetherBlastSpell));
                    Register(709, typeof(SkillMasteries.MysticWeaponSpell));
                    Register(710, typeof(SkillMasteries.CommandUndeadSpell));
                    Register(711, typeof(SkillMasteries.ConduitSpell));
                    Register(712, typeof(SkillMasteries.ManaShieldSpell));
                    Register(713, typeof(SkillMasteries.SummonReaperSpell));
                    Register(714, typeof(SkillMasteries.PassiveMasterySpell));
                    Register(715, typeof(SkillMasteries.PassiveMasterySpell));
                    Register(716, typeof(SkillMasteries.WarcrySpell));
                    Register(717, typeof(SkillMasteries.PassiveMasterySpell));
                    Register(718, typeof(SkillMasteries.RejuvinateSpell));
                    Register(719, typeof(SkillMasteries.HolyFistSpell));
                    Register(720, typeof(SkillMasteries.ShadowSpell));
                    Register(721, typeof(SkillMasteries.WhiteTigerFormSpell));
                    Register(722, typeof(SkillMasteries.FlamingShotSpell));
                    Register(723, typeof(SkillMasteries.PlayingTheOddsSpell));
                    Register(724, typeof(SkillMasteries.ThrustSpell));
                    Register(725, typeof(SkillMasteries.PierceSpell));
                    Register(726, typeof(SkillMasteries.StaggerSpell));
                    Register(727, typeof(SkillMasteries.ToughnessSpell));
                    Register(728, typeof(SkillMasteries.OnslaughtSpell));
                    Register(729, typeof(SkillMasteries.FocusedEyeSpell));
                    Register(730, typeof(SkillMasteries.ElementalFurySpell));
                    Register(731, typeof(SkillMasteries.CalledShotSpell));
                    Register(732, typeof(SkillMasteries.PassiveMasterySpell));
                    Register(733, typeof(SkillMasteries.ShieldBashSpell));
                    Register(734, typeof(SkillMasteries.BodyGuardSpell));
                    Register(735, typeof(SkillMasteries.HeightenedSensesSpell));
                    Register(736, typeof(SkillMasteries.ToleranceSpell));
                    Register(737, typeof(SkillMasteries.InjectedStrikeSpell));
                    Register(738, typeof(SkillMasteries.PassiveMasterySpell));
                    Register(739, typeof(SkillMasteries.RampageSpell));
                    Register(740, typeof(SkillMasteries.FistsOfFurySpell));
                    Register(741, typeof(SkillMasteries.PassiveMasterySpell));
                    Register(742, typeof(SkillMasteries.WhisperingSpell));
                    Register(743, typeof(SkillMasteries.CombatTrainingSpell));*/
                }
            }
        }

        public static void Register(int spellID, Type type)
        {
            SpellRegistry.Register(spellID, type);
        }
    }
}