using System;

namespace Server.Items
{
    public class ReptilianDeathSpellbook : Spellbook
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ReptilianDeathSpellbook() : base(ulong.MaxValue)
        {
            Name = "Tome of Reptilian Death";
            Hue = 1267;
            LootType = LootType.Blessed;
            Slayer = SlayerName.ReptilianDeath;
            Attributes.SpellDamage = 50;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;
            Attributes.RegenMana = 3;
        }

        public ReptilianDeathSpellbook(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class RepondSpellbook : Spellbook
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public RepondSpellbook() : base(ulong.MaxValue)
        {
            Name = "Tome of Repond";
            Hue = 1152;
            LootType = LootType.Blessed;
            Slayer = SlayerName.Repond;
            Attributes.SpellDamage = 50;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;
            Attributes.RegenMana = 3;
        }

        public RepondSpellbook(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class UndeadSpellbook : Spellbook
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public UndeadSpellbook() : base(ulong.MaxValue)
        {
            Name = "Tome of the Undead";
            Hue = 1150;
            LootType = LootType.Blessed;
            Slayer = SlayerName.Silver;
            Attributes.SpellDamage = 50;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;
            Attributes.RegenMana = 3;
        }

        public UndeadSpellbook(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class DemonSpellbook : Spellbook
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DemonSpellbook() : base(ulong.MaxValue)
        {
            Name = "Tome of Exorcism";
            Hue = 1157;
            LootType = LootType.Blessed;
            Slayer = SlayerName.Exorcism;
            Attributes.SpellDamage = 50;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;
            Attributes.RegenMana = 3;
        }

        public DemonSpellbook(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class FeySpellbook : Spellbook
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public FeySpellbook() : base(ulong.MaxValue)
        {
            Name = "Tome of the Fey";
            Hue = 1159;
            LootType = LootType.Blessed;
            Slayer = SlayerName.Fey;
            Attributes.SpellDamage = 50;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;
            Attributes.RegenMana = 3;
        }

        public FeySpellbook(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ArachnidDoomSpellbook : Spellbook
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ArachnidDoomSpellbook() : base(ulong.MaxValue)
        {
            Name = "Tome of Arachnid Doom";
            Hue = 1161;
            LootType = LootType.Blessed;
            Slayer = SlayerName.ArachnidDoom;
            Attributes.SpellDamage = 50;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;
            Attributes.RegenMana = 3;
        }

        public ArachnidDoomSpellbook(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }

    public class ElementalBanSpellbook : Spellbook
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ElementalBanSpellbook() : base(ulong.MaxValue)
        {
            Name = "Tome of Elemental Ban";
            Hue = 1160;
            LootType = LootType.Blessed;
            Slayer = SlayerName.ElementalBan;
            Attributes.SpellDamage = 50;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;
            Attributes.RegenMana = 3;
        }

        public ElementalBanSpellbook(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter w) { base.Serialize(w); w.Write(0); }
        public override void Deserialize(GenericReader r) { base.Deserialize(r); r.ReadInt(); }
    }
}
