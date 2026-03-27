using System;

namespace Server.Items
{
    #region Helper
    public static class SlayerWeaponHelper
    {
        public static void ApplyCommonProperties(BaseWeapon weapon, SlayerName slayer, int damageType, AosWeaponAttribute hitArea)
        {
            weapon.Slayer = slayer;
            weapon.WeaponAttributes.HitManaDrain = 100;
            weapon.WeaponAttributes.HitLeechHits = 100;
            weapon.WeaponAttributes.HitLeechStam = 50;
            weapon.WeaponAttributes.HitLowerDefend = 50;
            weapon.Attributes.Brittle = 1;

            weapon.MaxHitPoints = 255;
            weapon.HitPoints = 255;

            // Set hit area
            switch (hitArea)
            {
                case AosWeaponAttribute.HitPoisonArea: weapon.WeaponAttributes.HitPoisonArea = 50; break;
                case AosWeaponAttribute.HitColdArea: weapon.WeaponAttributes.HitColdArea = 50; break;
                case AosWeaponAttribute.HitFireArea: weapon.WeaponAttributes.HitFireArea = 50; break;
                case AosWeaponAttribute.HitEnergyArea: weapon.WeaponAttributes.HitEnergyArea = 50; break;
            }

            // Set 100% damage conversion
            weapon.AosElementDamages.Physical = 0;
            switch (damageType)
            {
                case 0: weapon.AosElementDamages.Poison = 100; break; // poison
                case 1: weapon.AosElementDamages.Cold = 100; break;   // cold
                case 2: weapon.AosElementDamages.Fire = 100; break;   // fire
                case 3: weapon.AosElementDamages.Energy = 100; break; // energy
            }
        }
    }
    #endregion

    // ==================== REPTILE SLAYER ====================
    // Poison damage 100%, Hit Poison Area 50%

    public class ReptileLeafblade : Leafblade
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileLeafblade() { Name = "Reptile Leafblade"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileLeafblade(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileWarAxe : WarAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileWarAxe() { Name = "Reptile War Axe"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileWarAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileBroadsword : Broadsword
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileBroadsword() { Name = "Reptile Broadsword"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileBroadsword(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileDoubleAxe : DoubleAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileDoubleAxe() { Name = "Reptile Double Axe"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileDoubleAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileWarHammer : WarHammer
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileWarHammer() { Name = "Reptile Paladin Hammer"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileWarHammer(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileMagicalShortbow : MagicalShortbow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileMagicalShortbow() { Name = "Reptile Magical Shortbow"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileMagicalShortbow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileCompositeBow : CompositeBow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileCompositeBow() { Name = "Reptile Composite Bow"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileSoulGlaive : SoulGlaive
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileSoulGlaive() { Name = "Reptile Soul Glaive"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileBoomerang : Boomerang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileBoomerang() { Name = "Reptile Boomerang"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileBoomerang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileGargishTalwar : GargishTalwar
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileGargishTalwar() { Name = "Reptile Gargish Talwar"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileGargishTalwar(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileGargishKatana : GargishKatana
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileGargishKatana() { Name = "Reptile Gargish Katana"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileGargishKatana(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ReptileLajatang : Lajatang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ReptileLajatang() { Name = "Reptile Lajatang"; Hue = 1267; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ReptilianDeath, 0, AosWeaponAttribute.HitPoisonArea); }
        public ReptileLajatang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    // ==================== REPOND SLAYER ====================
    // Cold damage 100%, Hit Cold Area 50%

    public class RepondLeafblade : Leafblade
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondLeafblade() { Name = "Repond Leafblade"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondLeafblade(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondWarAxe : WarAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondWarAxe() { Name = "Repond War Axe"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondWarAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondBroadsword : Broadsword
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondBroadsword() { Name = "Repond Broadsword"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondBroadsword(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondDoubleAxe : DoubleAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondDoubleAxe() { Name = "Repond Double Axe"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondDoubleAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondWarHammer : WarHammer
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondWarHammer() { Name = "Repond Paladin Hammer"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondWarHammer(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondMagicalShortbow : MagicalShortbow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondMagicalShortbow() { Name = "Repond Magical Shortbow"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondMagicalShortbow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondCompositeBow : CompositeBow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondCompositeBow() { Name = "Repond Composite Bow"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondSoulGlaive : SoulGlaive
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondSoulGlaive() { Name = "Repond Soul Glaive"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondBoomerang : Boomerang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondBoomerang() { Name = "Repond Boomerang"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondBoomerang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondGargishTalwar : GargishTalwar
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondGargishTalwar() { Name = "Repond Gargish Talwar"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondGargishTalwar(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondGargishKatana : GargishKatana
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondGargishKatana() { Name = "Repond Gargish Katana"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondGargishKatana(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondLajatang : Lajatang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RepondLajatang() { Name = "Repond Lajatang"; Hue = 1152; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Repond, 1, AosWeaponAttribute.HitColdArea); }
        public RepondLajatang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    // ==================== ARACHNID SLAYER ====================
    // Fire damage 100%, Hit Fire Area 50%

    public class ArachnidLeafblade : Leafblade
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidLeafblade() { Name = "Arachnid Leafblade"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidLeafblade(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidWarAxe : WarAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidWarAxe() { Name = "Arachnid War Axe"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidWarAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidBroadsword : Broadsword
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidBroadsword() { Name = "Arachnid Broadsword"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidBroadsword(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidDoubleAxe : DoubleAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidDoubleAxe() { Name = "Arachnid Double Axe"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidDoubleAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidWarHammer : WarHammer
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidWarHammer() { Name = "Arachnid Paladin Hammer"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidWarHammer(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidMagicalShortbow : MagicalShortbow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidMagicalShortbow() { Name = "Arachnid Magical Shortbow"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidMagicalShortbow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidCompositeBow : CompositeBow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidCompositeBow() { Name = "Arachnid Composite Bow"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidSoulGlaive : SoulGlaive
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidSoulGlaive() { Name = "Arachnid Soul Glaive"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidBoomerang : Boomerang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidBoomerang() { Name = "Arachnid Boomerang"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidBoomerang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidGargishTalwar : GargishTalwar
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidGargishTalwar() { Name = "Arachnid Gargish Talwar"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidGargishTalwar(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidGargishKatana : GargishKatana
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidGargishKatana() { Name = "Arachnid Gargish Katana"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidGargishKatana(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidLajatang : Lajatang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ArachnidLajatang() { Name = "Arachnid Lajatang"; Hue = 1161; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ArachnidDoom, 2, AosWeaponAttribute.HitFireArea); }
        public ArachnidLajatang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    // ==================== UNDEAD SLAYER (Silver) ====================
    // Fire damage 100%, Hit Fire Area 50%

    public class UndeadLeafblade : Leafblade
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadLeafblade() { Name = "Undead Leafblade"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadLeafblade(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadWarAxe : WarAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadWarAxe() { Name = "Undead War Axe"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadWarAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadBroadsword : Broadsword
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadBroadsword() { Name = "Undead Broadsword"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadBroadsword(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadDoubleAxe : DoubleAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadDoubleAxe() { Name = "Undead Double Axe"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadDoubleAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadWarHammer : WarHammer
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadWarHammer() { Name = "Undead Paladin Hammer"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadWarHammer(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadMagicalShortbow : MagicalShortbow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadMagicalShortbow() { Name = "Undead Magical Shortbow"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadMagicalShortbow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadCompositeBow : CompositeBow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadCompositeBow() { Name = "Undead Composite Bow"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadSoulGlaive : SoulGlaive
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadSoulGlaive() { Name = "Undead Soul Glaive"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadBoomerang : Boomerang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadBoomerang() { Name = "Undead Boomerang"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadBoomerang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadGargishTalwar : GargishTalwar
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadGargishTalwar() { Name = "Undead Gargish Talwar"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadGargishTalwar(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadGargishKatana : GargishKatana
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadGargishKatana() { Name = "Undead Gargish Katana"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadGargishKatana(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadLajatang : Lajatang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public UndeadLajatang() { Name = "Undead Lajatang"; Hue = 1150; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Silver, 2, AosWeaponAttribute.HitFireArea); }
        public UndeadLajatang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    // ==================== DEMON SLAYER (Exorcism) ====================
    // Cold damage 100%, Hit Cold Area 50%

    public class DemonLeafblade : Leafblade
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonLeafblade() { Name = "Demon Leafblade"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonLeafblade(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonWarAxe : WarAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonWarAxe() { Name = "Demon War Axe"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonWarAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonBroadsword : Broadsword
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonBroadsword() { Name = "Demon Broadsword"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonBroadsword(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonDoubleAxe : DoubleAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonDoubleAxe() { Name = "Demon Double Axe"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonDoubleAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonWarHammer : WarHammer
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonWarHammer() { Name = "Demon Paladin Hammer"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonWarHammer(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonMagicalShortbow : MagicalShortbow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonMagicalShortbow() { Name = "Demon Magical Shortbow"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonMagicalShortbow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonCompositeBow : CompositeBow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonCompositeBow() { Name = "Demon Composite Bow"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonSoulGlaive : SoulGlaive
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonSoulGlaive() { Name = "Demon Soul Glaive"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonBoomerang : Boomerang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonBoomerang() { Name = "Demon Boomerang"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonBoomerang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonGargishTalwar : GargishTalwar
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonGargishTalwar() { Name = "Demon Gargish Talwar"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonGargishTalwar(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonGargishKatana : GargishKatana
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonGargishKatana() { Name = "Demon Gargish Katana"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonGargishKatana(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonLajatang : Lajatang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DemonLajatang() { Name = "Demon Lajatang"; Hue = 1157; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Exorcism, 1, AosWeaponAttribute.HitColdArea); }
        public DemonLajatang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    // ==================== FEY SLAYER ====================
    // Fire damage 100%, Hit Fire Area 50%

    public class FeyLeafblade : Leafblade
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyLeafblade() { Name = "Fey Leafblade"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyLeafblade(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyWarAxe : WarAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyWarAxe() { Name = "Fey War Axe"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyWarAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyBroadsword : Broadsword
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyBroadsword() { Name = "Fey Broadsword"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyBroadsword(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyDoubleAxe : DoubleAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyDoubleAxe() { Name = "Fey Double Axe"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyDoubleAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyWarHammer : WarHammer
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyWarHammer() { Name = "Fey Paladin Hammer"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyWarHammer(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyMagicalShortbow : MagicalShortbow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyMagicalShortbow() { Name = "Fey Magical Shortbow"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyMagicalShortbow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyCompositeBow : CompositeBow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyCompositeBow() { Name = "Fey Composite Bow"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeySoulGlaive : SoulGlaive
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeySoulGlaive() { Name = "Fey Soul Glaive"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeySoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyBoomerang : Boomerang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyBoomerang() { Name = "Fey Boomerang"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyBoomerang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyGargishTalwar : GargishTalwar
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyGargishTalwar() { Name = "Fey Gargish Talwar"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyGargishTalwar(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyGargishKatana : GargishKatana
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyGargishKatana() { Name = "Fey Gargish Katana"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyGargishKatana(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeyLajatang : Lajatang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FeyLajatang() { Name = "Fey Lajatang"; Hue = 1159; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.Fey, 2, AosWeaponAttribute.HitFireArea); }
        public FeyLajatang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    // ==================== ELEMENTAL SLAYER ====================
    // Energy damage 100%, Hit Energy Area 50%

    public class ElementalLeafblade : Leafblade
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalLeafblade() { Name = "Elemental Leafblade"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalLeafblade(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalWarAxe : WarAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalWarAxe() { Name = "Elemental War Axe"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalWarAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalBroadsword : Broadsword
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalBroadsword() { Name = "Elemental Broadsword"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalBroadsword(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalDoubleAxe : DoubleAxe
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalDoubleAxe() { Name = "Elemental Double Axe"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalDoubleAxe(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalWarHammer : WarHammer
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalWarHammer() { Name = "Elemental Paladin Hammer"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalWarHammer(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalMagicalShortbow : MagicalShortbow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalMagicalShortbow() { Name = "Elemental Magical Shortbow"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalMagicalShortbow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalCompositeBow : CompositeBow
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalCompositeBow() { Name = "Elemental Composite Bow"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalCompositeBow(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalSoulGlaive : SoulGlaive
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalSoulGlaive() { Name = "Elemental Soul Glaive"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalSoulGlaive(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalBoomerang : Boomerang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalBoomerang() { Name = "Elemental Boomerang"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalBoomerang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalGargishTalwar : GargishTalwar
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalGargishTalwar() { Name = "Elemental Gargish Talwar"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalGargishTalwar(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalGargishKatana : GargishKatana
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalGargishKatana() { Name = "Elemental Gargish Katana"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalGargishKatana(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalLajatang : Lajatang
    {
        public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ElementalLajatang() { Name = "Elemental Lajatang"; Hue = 1160; SlayerWeaponHelper.ApplyCommonProperties(this, SlayerName.ElementalBan, 3, AosWeaponAttribute.HitEnergyArea); }
        public ElementalLajatang(Serial s) : base(s) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }
}
