using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.ClassSystem
{
    /// <summary>
    /// The Avenger – a dark paladin archetype that blends martial combat with
    /// unholy prayers.
    ///
    /// How to bootstrap
    /// ================
    /// Add a static <c>Initialize()</c> method call to the server startup or
    /// create a script that ServUO picks up automatically:
    ///
    ///   [CallPriority(10)]
    ///   public static void Initialize()
    ///   {
    ///       ClassRegistry.Register(new AvengerClass());
    ///   }
    ///
    /// (The CallPriority attribute ensures this runs after the ClassRegistry.)
    /// </summary>
    public sealed class AvengerClass : BasePlayerClass
    {
        // ── Singleton bootstrap ──────────────────────────────────────────────────

        public static void Register()
        {
            ClassRegistry.Register(new AvengerClass());
        }

        // ── Identity ─────────────────────────────────────────────────────────────

        public override ClassType ClassType  => ClassType.Avenger;
        public override string    DisplayName => "Avenger";

        // ── Base Stats ───────────────────────────────────────────────────────────
        // Flat values – no level scaling; players start at full power.

        public override int BaseStr => 95;
        public override int BaseDex => 67;
        public override int BaseInt => 43;

        // ── Skills ───────────────────────────────────────────────────────────────

        public override double SkillSwords      => 160.0;
        public override double SkillMacing      => 144.3;
        public override double SkillFencing     => 160.0;
        public override double SkillArchery     => 0.0;      // Avengers do not use ranged weapons
        public override double SkillWrestling   => 84.0;
        public override double SkillTactics     => 170.0;
        public override double SkillParrying    => 110.0;
        public override double SkillAnatomy     => 65.0;
        public override double SkillHiding      => 40.0;
        public override double SkillDetectHidden => 35.0;
        public override double SkillHealing     => 35.0;
        public override double SkillMeditation  => 71.0;
        public override double SkillMagicResist => 137.0;

        // ── Equipment ────────────────────────────────────────────────────────────

        public override IReadOnlyList<ArmorMaterialType> AllowedArmorMaterials { get; }
            = Array.AsReadOnly(new[]
            {
                ArmorMaterialType.Leather,
                ArmorMaterialType.Studded,
                ArmorMaterialType.Ringmail,
                ArmorMaterialType.Chainmail,
                ArmorMaterialType.Plate
            });

        /// <summary>Avengers cannot use ranged weapons.</summary>
        public override bool CanEquipWeapon(BaseWeapon weapon)
        {
            if (weapon == null)
                return false;

            // WeaponType.Ranged maps to Archery weapons in ServUO.
            return weapon.Skill != SkillName.Archery;
        }

        // ── Power command registration ────────────────────────────────────────────

        /// <summary>
        /// Registers the ".avenger &lt;number&gt;" command that dispatches all Avenger
        /// powers.  Called once at startup by <see cref="ClassRegistry"/>.
        /// </summary>
        public override void RegisterCommands()
        {
            CommandSystem.Register(
                "avenger",
                AccessLevel.Player,
                new CommandEventHandler(OnCommand));
        }

        // ── Lifecycle hooks ───────────────────────────────────────────────────────

        public override void OnClassAssigned(PlayerMobile player)
        {
            player.SendMessage(38, "The dark power of the Avenger flows through you.");
        }

        public override void OnClassRemoved(PlayerMobile player)
        {
            // Cancel any active Avenger effects.
            ClassPowerSystem.ExpireAllEffects(player);
            player.SendMessage(38, "You have lost your Avenger powers.");
        }

        // ── Power table ───────────────────────────────────────────────────────────

        /// <summary>
        /// Maps command index → power factory.
        /// Add a new entry here (and a new power class in AvengerPowers.cs) to add
        /// a new power without touching anything else.
        /// </summary>
        private static readonly Dictionary<int, Func<Mobile, BasePower>> PowerTable
            = new Dictionary<int, Func<Mobile, BasePower>>
        {
            // 1  → DetectGoodPower          (was IndividuaIlBene)
            { 1,  m => new DetectGoodPower(m)            },
            // 2  → InflictWoundsPower        (was InfliggiFeriteVendicatore)
            { 2,  m => new InflictWoundsPower(m)          },
            // 3  → TouchOfEvil              (was ToccoDelMale)
            { 3,  m => new TouchOfEvilPower(m)            },
            // 4  → AvengerLightPower        (was LuceVendicatore)
            { 4,  m => new AvengerLightPower(m)           },
            // 5  → HeatMetalPower           (was RiscaldaMetalloVendicatore)
            { 5,  m => new HeatMetalPower(m)              },
            // 6  → AvengerCursePower        (was MaledizioneVendicatore)
            { 6,  m => new AvengerCursePower(m)           },
            // 7  → AvengerDiseasePower      (was MalattiaIVendicatore)
            { 7,  m => new AvengerDiseasePower(m)         },
            // 8  → ChaosShieldPower         (was ScudoDelCaosVendicatore)
            { 8,  m => new ChaosShieldPower(m)            },
            // 9  → BlackFlamePower          (was FiammaNera)
            { 9,  m => new BlackFlamePower(m)             },
            // 10 → WeakenResistancePower    (was ResistenzaRidotta)
            { 10, m => new WeakenResistancePower(m)       },
            // 11 → InfernalSummonPower      (was EvocazioneInfernale)
            { 11, m => new InfernalSummonPower(m)         },
            // 12 → BloodThirstPower         (was SeteDiSangue)
            { 12, m => new BloodThirstPower(m)            },
            // 13 → PunishGoodPower          (was PunireIlBene)
            { 13, m => new PunishGoodPower(m)             },
            // 14 → ReturnFromAbyssPower     (was RitornoDallAbissoVendicatore)
            { 14, m => new ReturnFromAbyssPower(m)        },
            // 15 → BloodSabrePower          (was SciabolaSanguinaria)
            { 15, m => new BloodSabrePower(m)             },
            // 16 → BlasfemousSoulPower      (was AnimaBlasfema)
            { 16, m => new BlasphemousSoulPower(m)        },
            // 17 → UseBlasfemousSoulPower   (was UsaAnimaBlasfema)
            { 17, m => new UseBlasphemousSoulPower(m)     },
            // 18 → DiabolicIntentPower      (was IntentoDiabolico)
            { 18, m => new DiabolicIntentPower(m)         },
        };

        // ── Command handler ───────────────────────────────────────────────────────

        private static void OnCommand(CommandEventArgs e)
        {
            if (!(e.Mobile is PlayerMobile player))
                return;

            // Class check (GMs bypass).
            if (player.AccessLevel < AccessLevel.GameMaster
                && !player.ClassComponent.HasClass(ClassType.Avenger))
            {
                player.SendMessage("You are not an Avenger.");
                return;
            }

            if (e.Length != 1)
            {
                player.SendMessage("Usage: .avenger <power number>");
                return;
            }

            int index = e.GetInt32(0);

            Func<Mobile, BasePower> factory;
            if (!PowerTable.TryGetValue(index, out factory))
            {
                player.SendMessage($"Unknown power: {index}");
                return;
            }

            BasePower power = factory(player);
            ClassPowerSystem.Cast(power, ClassType.Avenger);
        }
    }
}
