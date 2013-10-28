using System;
using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// This is a test creature
    /// You can set its value in game
    /// It die after 5 minutes, so your test server stay clean
    /// Create a macro to help your creation "[add Dummy 1 15 7 -1 0.5 2"
    /// 
    /// A iTeam of negative will set a faction at random
    /// 
    /// Say Kill if you want them to die
    /// 
    /// </summary>
    public class DummyMace : Dummy
    {
        [Constructable]
        public DummyMace()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Macer
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(125, 125, 90);
            this.Skills[SkillName.Macing].Base = 120;
            this.Skills[SkillName.Anatomy].Base = 120;
            this.Skills[SkillName.Healing].Base = 120;
            this.Skills[SkillName.Tactics].Base = 120;

            // Name
            this.Name = "Macer";

            // Equip
            WarHammer war = new WarHammer();
            war.Movable = true;
            war.Crafter = this;
            war.Quality = WeaponQuality.Regular;
            this.AddItem(war);

            Boots bts = new Boots();
            bts.Hue = iHue;
            this.AddItem(bts);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Newbied;
            cht.Crafter = this;
            cht.Quality = ArmorQuality.Regular;
            this.AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Newbied;
            chl.Crafter = this;
            chl.Quality = ArmorQuality.Regular;
            this.AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Newbied;
            pla.Crafter = this;
            pla.Quality = ArmorQuality.Regular;
            this.AddItem(pla);

            Bandage band = new Bandage(50);
            this.AddToBackpack(band);
        }

        public DummyMace(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DummyFence : Dummy
    {
        [Constructable]
        public DummyFence()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Fencer
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(125, 125, 90);
            this.Skills[SkillName.Fencing].Base = 120;
            this.Skills[SkillName.Anatomy].Base = 120;
            this.Skills[SkillName.Healing].Base = 120;
            this.Skills[SkillName.Tactics].Base = 120;

            // Name
            this.Name = "Fencer";

            // Equip
            Spear ssp = new Spear();
            ssp.Movable = true;
            ssp.Crafter = this;
            ssp.Quality = WeaponQuality.Regular;
            this.AddItem(ssp);

            Boots snd = new Boots();
            snd.Hue = iHue;
            snd.LootType = LootType.Newbied;
            this.AddItem(snd);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Newbied;
            cht.Crafter = this;
            cht.Quality = ArmorQuality.Regular;
            this.AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Newbied;
            chl.Crafter = this;
            chl.Quality = ArmorQuality.Regular;
            this.AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Newbied;
            pla.Crafter = this;
            pla.Quality = ArmorQuality.Regular;
            this.AddItem(pla);

            Bandage band = new Bandage(50);
            this.AddToBackpack(band);
        }

        public DummyFence(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DummySword : Dummy
    {
        [Constructable]
        public DummySword()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Swordsman
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(125, 125, 90);
            this.Skills[SkillName.Swords].Base = 120;
            this.Skills[SkillName.Anatomy].Base = 120;
            this.Skills[SkillName.Healing].Base = 120;
            this.Skills[SkillName.Tactics].Base = 120;
            this.Skills[SkillName.Parry].Base = 120;

            // Name
            this.Name = "Swordsman";

            // Equip
            Katana kat = new Katana();
            kat.Crafter = this;
            kat.Movable = true;
            kat.Quality = WeaponQuality.Regular;
            this.AddItem(kat);

            Boots bts = new Boots();
            bts.Hue = iHue;
            this.AddItem(bts);

            ChainChest cht = new ChainChest();
            cht.Movable = false;
            cht.LootType = LootType.Newbied;
            cht.Crafter = this;
            cht.Quality = ArmorQuality.Regular;
            this.AddItem(cht);

            ChainLegs chl = new ChainLegs();
            chl.Movable = false;
            chl.LootType = LootType.Newbied;
            chl.Crafter = this;
            chl.Quality = ArmorQuality.Regular;
            this.AddItem(chl);

            PlateArms pla = new PlateArms();
            pla.Movable = false;
            pla.LootType = LootType.Newbied;
            pla.Crafter = this;
            pla.Quality = ArmorQuality.Regular;
            this.AddItem(pla);

            Bandage band = new Bandage(50);
            this.AddToBackpack(band);
        }

        public DummySword(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DummyNox : Dummy
    {
        [Constructable]
        public DummyNox()
            : base(AIType.AI_Mage, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Nox or Pure Mage
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(90, 90, 125);
            this.Skills[SkillName.Magery].Base = 120;
            this.Skills[SkillName.EvalInt].Base = 120;
            this.Skills[SkillName.Inscribe].Base = 100;
            this.Skills[SkillName.Wrestling].Base = 120;
            this.Skills[SkillName.Meditation].Base = 120;
            this.Skills[SkillName.Poisoning].Base = 100;

            // Name
            this.Name = "Nox Mage";

            // Equip
            Spellbook book = new Spellbook();
            book.Movable = false;
            book.LootType = LootType.Newbied;
            book.Content = 0xFFFFFFFFFFFFFFFF;
            this.AddItem(book);

            Kilt kilt = new Kilt();
            kilt.Hue = jHue;
            this.AddItem(kilt);

            Sandals snd = new Sandals();
            snd.Hue = iHue;
            snd.LootType = LootType.Newbied;
            this.AddItem(snd);

            SkullCap skc = new SkullCap();
            skc.Hue = iHue;
            this.AddItem(skc);

            // Spells
            this.AddSpellAttack(typeof(Spells.First.MagicArrowSpell));
            this.AddSpellAttack(typeof(Spells.First.WeakenSpell));
            this.AddSpellAttack(typeof(Spells.Third.FireballSpell));
            this.AddSpellDefense(typeof(Spells.Third.WallOfStoneSpell));
            this.AddSpellDefense(typeof(Spells.First.HealSpell));
        }

        public DummyNox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DummyStun : Dummy
    {
        [Constructable]
        public DummyStun()
            : base(AIType.AI_Mage, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Stun Mage
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(90, 90, 125);
            this.Skills[SkillName.Magery].Base = 100;
            this.Skills[SkillName.EvalInt].Base = 120;
            this.Skills[SkillName.Anatomy].Base = 80;
            this.Skills[SkillName.Wrestling].Base = 80;
            this.Skills[SkillName.Meditation].Base = 100;
            this.Skills[SkillName.Poisoning].Base = 100;

            // Name
            this.Name = "Stun Mage";

            // Equip
            Spellbook book = new Spellbook();
            book.Movable = false;
            book.LootType = LootType.Newbied;
            book.Content = 0xFFFFFFFFFFFFFFFF;
            this.AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Newbied;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Regular;
            this.AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Newbied;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Regular;
            this.AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Newbied;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Regular;
            this.AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Newbied;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Regular;
            this.AddItem(lel);

            Boots bts = new Boots();
            bts.Hue = iHue;
            this.AddItem(bts);

            Cap cap = new Cap();
            cap.Hue = iHue;
            this.AddItem(cap);

            // Spells
            this.AddSpellAttack(typeof(Spells.First.MagicArrowSpell));
            this.AddSpellAttack(typeof(Spells.First.WeakenSpell));
            this.AddSpellAttack(typeof(Spells.Third.FireballSpell));
            this.AddSpellDefense(typeof(Spells.Third.WallOfStoneSpell));
            this.AddSpellDefense(typeof(Spells.First.HealSpell));
        }

        public DummyStun(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DummySuper : Dummy
    {
        [Constructable]
        public DummySuper()
            : base(AIType.AI_Mage, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Super Mage
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(125, 125, 125);
            this.Skills[SkillName.Magery].Base = 120;
            this.Skills[SkillName.EvalInt].Base = 120;
            this.Skills[SkillName.Anatomy].Base = 120;
            this.Skills[SkillName.Wrestling].Base = 120;
            this.Skills[SkillName.Meditation].Base = 120;
            this.Skills[SkillName.Poisoning].Base = 100;
            this.Skills[SkillName.Inscribe].Base = 100;

            // Name
            this.Name = "Super Mage";

            // Equip
            Spellbook book = new Spellbook();
            book.Movable = false;
            book.LootType = LootType.Newbied;
            book.Content = 0xFFFFFFFFFFFFFFFF;
            this.AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Newbied;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Regular;
            this.AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Newbied;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Regular;
            this.AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Newbied;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Regular;
            this.AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Newbied;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Regular;
            this.AddItem(lel);

            Sandals snd = new Sandals();
            snd.Hue = iHue;
            snd.LootType = LootType.Newbied;
            this.AddItem(snd);

            JesterHat jhat = new JesterHat();
            jhat.Hue = iHue;
            this.AddItem(jhat);

            Doublet dblt = new Doublet();
            dblt.Hue = iHue;
            this.AddItem(dblt);

            // Spells
            this.AddSpellAttack(typeof(Spells.First.MagicArrowSpell));
            this.AddSpellAttack(typeof(Spells.First.WeakenSpell));
            this.AddSpellAttack(typeof(Spells.Third.FireballSpell));
            this.AddSpellDefense(typeof(Spells.Third.WallOfStoneSpell));
            this.AddSpellDefense(typeof(Spells.First.HealSpell));
        }

        public DummySuper(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DummyHealer : Dummy
    {
        [Constructable]
        public DummyHealer()
            : base(AIType.AI_Healer, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Healer Mage
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(125, 125, 125);
            this.Skills[SkillName.Magery].Base = 120;
            this.Skills[SkillName.EvalInt].Base = 120;
            this.Skills[SkillName.Anatomy].Base = 120;
            this.Skills[SkillName.Wrestling].Base = 120;
            this.Skills[SkillName.Meditation].Base = 120;
            this.Skills[SkillName.Healing].Base = 100;

            // Name
            this.Name = "Healer";

            // Equip
            Spellbook book = new Spellbook();
            book.Movable = false;
            book.LootType = LootType.Newbied;
            book.Content = 0xFFFFFFFFFFFFFFFF;
            this.AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Newbied;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Regular;
            this.AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Newbied;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Regular;
            this.AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Newbied;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Regular;
            this.AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Newbied;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Regular;
            this.AddItem(lel);

            Sandals snd = new Sandals();
            snd.Hue = iHue;
            snd.LootType = LootType.Newbied;
            this.AddItem(snd);

            Cap cap = new Cap();
            cap.Hue = iHue;
            this.AddItem(cap);

            Robe robe = new Robe();
            robe.Hue = iHue;
            this.AddItem(robe);
        }

        public DummyHealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DummyAssassin : Dummy
    {
        [Constructable]
        public DummyAssassin()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Hybrid Assassin
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(105, 105, 105);
            this.Skills[SkillName.Magery].Base = 120;
            this.Skills[SkillName.EvalInt].Base = 120;
            this.Skills[SkillName.Swords].Base = 120;
            this.Skills[SkillName.Tactics].Base = 120;
            this.Skills[SkillName.Meditation].Base = 120;
            this.Skills[SkillName.Poisoning].Base = 100;

            // Name
            this.Name = "Hybrid Assassin";

            // Equip
            Spellbook book = new Spellbook();
            book.Movable = false;
            book.LootType = LootType.Newbied;
            book.Content = 0xFFFFFFFFFFFFFFFF;
            this.AddToBackpack(book);

            Katana kat = new Katana();
            kat.Movable = false;
            kat.LootType = LootType.Newbied;
            kat.Crafter = this;
            kat.Poison = Poison.Deadly;
            kat.PoisonCharges = 12;
            kat.Quality = WeaponQuality.Regular;
            this.AddToBackpack(kat);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Newbied;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Regular;
            this.AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Newbied;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Regular;
            this.AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Newbied;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Regular;
            this.AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Newbied;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Regular;
            this.AddItem(lel);

            Sandals snd = new Sandals();
            snd.Hue = iHue;
            snd.LootType = LootType.Newbied;
            this.AddItem(snd);

            Cap cap = new Cap();
            cap.Hue = iHue;
            this.AddItem(cap);

            Robe robe = new Robe();
            robe.Hue = iHue;
            this.AddItem(robe);

            DeadlyPoisonPotion pota = new DeadlyPoisonPotion();
            pota.LootType = LootType.Newbied;
            this.AddToBackpack(pota);

            DeadlyPoisonPotion potb = new DeadlyPoisonPotion();
            potb.LootType = LootType.Newbied;
            this.AddToBackpack(potb);

            DeadlyPoisonPotion potc = new DeadlyPoisonPotion();
            potc.LootType = LootType.Newbied;
            this.AddToBackpack(potc);

            DeadlyPoisonPotion potd = new DeadlyPoisonPotion();
            potd.LootType = LootType.Newbied;
            this.AddToBackpack(potd);

            Bandage band = new Bandage(50);
            this.AddToBackpack(band);
        }

        public DummyAssassin(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [TypeAlias("Server.Mobiles.DummyTheif")]
    public class DummyThief : Dummy
    {
        [Constructable]
        public DummyThief()
            : base(AIType.AI_Thief, FightMode.Closest, 15, 1, 0.2, 0.6)
        {
            // A Dummy Hybrid Thief
            int iHue = 20 + this.Team * 40;
            int jHue = 25 + this.Team * 40;

            // Skills and Stats
            this.InitStats(105, 105, 105);
            this.Skills[SkillName.Healing].Base = 120;
            this.Skills[SkillName.Anatomy].Base = 120;
            this.Skills[SkillName.Stealing].Base = 120;
            this.Skills[SkillName.ArmsLore].Base = 100;
            this.Skills[SkillName.Meditation].Base = 120;
            this.Skills[SkillName.Wrestling].Base = 120;

            // Name
            this.Name = "Hybrid Thief";

            // Equip
            Spellbook book = new Spellbook();
            book.Movable = false;
            book.LootType = LootType.Newbied;
            book.Content = 0xFFFFFFFFFFFFFFFF;
            this.AddItem(book);

            LeatherArms lea = new LeatherArms();
            lea.Movable = false;
            lea.LootType = LootType.Newbied;
            lea.Crafter = this;
            lea.Quality = ArmorQuality.Regular;
            this.AddItem(lea);

            LeatherChest lec = new LeatherChest();
            lec.Movable = false;
            lec.LootType = LootType.Newbied;
            lec.Crafter = this;
            lec.Quality = ArmorQuality.Regular;
            this.AddItem(lec);

            LeatherGorget leg = new LeatherGorget();
            leg.Movable = false;
            leg.LootType = LootType.Newbied;
            leg.Crafter = this;
            leg.Quality = ArmorQuality.Regular;
            this.AddItem(leg);

            LeatherLegs lel = new LeatherLegs();
            lel.Movable = false;
            lel.LootType = LootType.Newbied;
            lel.Crafter = this;
            lel.Quality = ArmorQuality.Regular;
            this.AddItem(lel);

            Sandals snd = new Sandals();
            snd.Hue = iHue;
            snd.LootType = LootType.Newbied;
            this.AddItem(snd);

            Cap cap = new Cap();
            cap.Hue = iHue;
            this.AddItem(cap);

            Robe robe = new Robe();
            robe.Hue = iHue;
            this.AddItem(robe);

            Bandage band = new Bandage(50);
            this.AddToBackpack(band);
        }

        public DummyThief(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}