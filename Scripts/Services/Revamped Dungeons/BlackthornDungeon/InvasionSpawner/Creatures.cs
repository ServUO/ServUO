using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Engines.CityLoyalty;

namespace Server.Engines.Blackthorn
{
    public class Invader : BaseCreature
    {
        public static SkillName RandomSpecialty()
        {
            return _Specialties[Utility.Random(_Specialties.Length)];
        }

        private static SkillName[] _Specialties =
        {
            SkillName.Swords,
            SkillName.Fencing,
            SkillName.Macing,
            SkillName.Archery,
            SkillName.Magery,
            SkillName.Mysticism,
            SkillName.Spellweaving,
            SkillName.Bushido,
            SkillName.Bushido,
            SkillName.Ninjitsu,
            SkillName.Chivalry,
            SkillName.Necromancy,
            SkillName.Poisoning
        };

        private InvasionType _InvasionType;
        private SkillName _Specialty;

        private bool _Sampire;
        private DateTime _NextSpecial;

        public override bool AlwaysMurderer { get { return true; } }
        public override double HealChance { get { return AI == AIType.AI_Melee || AI == AIType.AI_Paladin ? 1.0 : 0.0; } }
        public override double WeaponAbilityChance { get { return AI == AIType.AI_Melee || AI == AIType.AI_Paladin ? 0.4 : 0.1; } }

        public override WeaponAbility GetWeaponAbility()
        {
            BaseWeapon wep = Weapon as BaseWeapon;

            if (wep != null)
            {
                return 0.6 > Utility.RandomDouble() ? wep.PrimaryAbility : wep.SecondaryAbility;
            }

            return null;
        }

        public override bool UseSmartAI { get { return true; } }

        public override bool HasAura { get { return true; } }
        public override TimeSpan AuraInterval { get { return TimeSpan.FromSeconds(2); } }
        public override int AuraRange { get { return 3; } }

        public override int AuraBaseDamage { get { return 25; } }
        public override int AuraEnergyDamage { get { return 100; } }

        public virtual bool CanDoSpecial { get { return SpellCaster; } }

        public override void AuraEffect(Mobile m)
        {
            if (m.NetState != null)
                m.SendLocalizedMessage(1151112, String.Format("{0}\t#1072073", this.Name)); // : The creature's aura of energy is damaging you!
        }

        public virtual double MinSkill { get { return 100.0; } }
        public virtual double MaxSkill { get { return 120.0; } }
        public virtual int MinResist { get { return 10; } }
        public virtual int MaxResist { get { return 20; } }

        public bool SpellCaster { get { return AI != AIType.AI_Melee && AI != AIType.AI_Ninja && AI != AIType.AI_Samurai && AI != AIType.AI_Paladin; } }

        [Constructable]
        public Invader(InvasionType type)
            : this(RandomSpecialty(), type)
        {
        }

        [Constructable]
        public Invader(SkillName specialty, InvasionType type)
            : base(GetAI(specialty), FightMode.Closest, 10, 1, .2, .1)
        {
            _Specialty = specialty;
            _InvasionType = type;

            if (_Specialty == SkillName.Bushido && Utility.RandomBool())
                _Sampire = true;

            if (Female = Utility.RandomBool())
            {
                //Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                //Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetBody();

            string title;

            if (_Sampire)
            {
                title = "the sampire";
            }
            else if (specialty == SkillName.Magery)
            {
                title = "the wizard";
            }
            else
            {
                title = String.Format("the {0}", Skills[specialty].Info.Title);
                if (Female && title.EndsWith("man"))
                    title = title.Substring(0, title.Length - 3) + "woman";
            }

            Title = title;

            SetStr(120, 170);
            SetDex(SpellCaster ? 75 : 150);
            SetInt(SpellCaster ? 1800 : 500);

            SetHits(800, 1250);

            if (AI == AIType.AI_Melee)
                SetDamage(15, 28);
            else if (!SpellCaster)
                SetDamage(12, 22);
            else
                SetDamage(8, 18);

            Fame = 8000;
            Karma = -8000;  

            SetResists();
            SetSkills();
            EquipSpecialty();

            _NextSpecial = DateTime.UtcNow;

            if (_Sampire)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                    {
                        VampiricEmbraceSpell spell = new VampiricEmbraceSpell(this, null);
                        spell.Cast();
                    });
            }
        }

        public virtual void SetBody()
        {
            switch (_Specialty)
            {
                default:
                    if (0.75 > Utility.RandomDouble())
                        Race = Race.Human;
                    else
                        Race = Race.Elf; break;
                case SkillName.Archery:
                case SkillName.Spellweaving: Race = Race.Elf; break;
            }

            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();

            FacialHairItemID = Race.RandomFacialHair(Female);
            FacialHairHue = Race.RandomHairHue();

            Hue = Race.RandomSkinHue();
        }

        public virtual void SetResists()
        {
            SetResistance(ResistanceType.Physical, MinResist, MaxResist);
            SetResistance(ResistanceType.Fire, MinResist, MaxResist);
            SetResistance(ResistanceType.Cold, MinResist, MaxResist);
            SetResistance(ResistanceType.Poison, MinResist, MaxResist);
            SetResistance(ResistanceType.Energy, MinResist, MaxResist);
        }

        public virtual void SetSkills()
        {
            SetSkill(SkillName.Fencing, MinSkill, MaxSkill);
            SetSkill(SkillName.Macing, MinSkill, MaxSkill);
            SetSkill(SkillName.MagicResist, MinSkill, MaxSkill);
            SetSkill(SkillName.Swords, MinSkill, MaxSkill);
            SetSkill(SkillName.Tactics, MinSkill, MaxSkill);
            SetSkill(SkillName.Wrestling, MinSkill, MaxSkill);
            SetSkill(SkillName.Archery, MinSkill, MaxSkill);
            SetSkill(SkillName.Parry, MinSkill, MaxSkill);

            if (SpellCaster)
            {
                SetSkill(SkillName.Magery, MinSkill, MaxSkill);
                SetSkill(SkillName.EvalInt, MinSkill, MaxSkill);
            }

            if (Skills[_Specialty].Base < MinSkill)
                SetSkill(_Specialty, MinSkill, MaxSkill);

            if (_Sampire)
            {
                SetSkill(SkillName.Necromancy, MinSkill, MaxSkill);
            }

            if (_Sampire || _Specialty == SkillName.Necromancy)
            {
                SetSkill(SkillName.SpiritSpeak, MinSkill, MaxSkill);
            }
        }

        public virtual void EquipSpecialty()
        {
            if (AbilityProfile != null && AbilityProfile.HasAbility(SpecialAbility.Heal))
                PackItem(new Bandage(Utility.RandomMinMax(10, 25)));

            SetWearable(new ThighBoots());
            SetWearable(new BodySash(), Utility.RandomSlimeHue());

            switch (_Specialty)
            {
                case SkillName.Chivalry:
                    SetWearable(RandomSwordWeapon());
                    PaladinEquip();
                    break;
                case SkillName.Swords:
                    SetWearable(RandomSwordWeapon());
                    StandardMeleeEquip();
                    break;
                case SkillName.Fencing:
                    SetWearable(RandomFencingWeapon());
                    StandardMeleeEquip();
                    break;
                case SkillName.Macing:
                    SetWearable(RandomMaceWeapon());
                    StandardMeleeEquip();
                    break;
                case SkillName.Archery:
                    SetWearable(RandomArhceryWeapon());
                    StandardMeleeEquip();
                    break;
                case SkillName.Magery:
                    SetWearable(RandomMageWeapon());
                    StandardMageEquip();
                    break;
                case SkillName.Mysticism:
                    SetWearable(RandomMageWeapon());
                    StandardMageEquip();
                    break;
                case SkillName.Spellweaving:
                    SetWearable(RandomMageWeapon());
                    StandardMageEquip();
                    break;
                case SkillName.Necromancy:
                    SetWearable(RandomMageWeapon());
                    StandardMageEquip();
                    break;
                case SkillName.Bushido:
                    BaseWeapon w = RandomSamuraiWeapon() as BaseWeapon;
                    SetWearable(w);

                    SetWearable(new LeatherSuneate());
                    SetWearable(new LeatherJingasa());
                    SetWearable(new LeatherDo());
                    SetWearable(new LeatherHiroSode());
                    SetWearable(new SamuraiTabi(Utility.RandomNondyedHue()));

                    if (_Sampire)
                        w.WeaponAttributes.HitLeechHits = 100;

                    SetSkill(SkillName.Parry, 120);
                    break;
                case SkillName.Ninjitsu:
                    SetWearable(RandomNinjaWeapon());

                    LeatherNinjaBelt belt = new LeatherNinjaBelt();
                    belt.UsesRemaining = 20;
                    belt.Poison = Poison.Greater;
                    belt.PoisonCharges = 20;
                    SetWearable(belt);

                    for (int i = 0; i < 2; i++)
                    {
                        Fukiya f = new Fukiya();
                        f.UsesRemaining = 10;
                        f.Poison = Poison.Greater;
                        f.PoisonCharges = 10;
                        f.Movable = false;
                        PackItem(f);
                    }

                    SetWearable(new NinjaTabi());
                    SetWearable(new LeatherNinjaJacket());
                    SetWearable(new LeatherNinjaHood());
                    SetWearable(new LeatherNinjaPants());
                    SetWearable(new LeatherNinjaMitts());

                    break;
                case SkillName.Poisoning:
                    BaseWeapon wep = RandomAssassinWeapon() as BaseWeapon;
                    wep.Poison = Poison.Lethal;
                    wep.PoisonCharges = 100;
                    SetWearable(wep);

                    SetWearable(new LeatherChest());
                    SetWearable(new LeatherLegs());
                    SetWearable(new LeatherGloves());
                    SetWearable(new LeatherGorget());
                    break;
            }
        }

        private void PaladinEquip()
        {
            SetWearable(Loot.Construct(new Type[] { typeof(Bascinet), typeof(Helmet), typeof(PlateHelm) }), 1153);

            SetWearable(new PlateChest());
            SetWearable(new PlateLegs());
            SetWearable(new PlateGloves());
            SetWearable(new PlateGorget());
            SetWearable(new PlateArms());
            SetWearable(new MetalKiteShield());

            SetSkill(SkillName.Parry, 120);
        }

        private void StandardMeleeEquip()
        {
            SetWearable(Loot.Construct(new Type[] { typeof(Bascinet), typeof(Helmet), typeof(LeatherCap), typeof(RoyalCirclet) }));

            SetWearable(new ChainChest());
            SetWearable(new ChainLegs());
            SetWearable(new RingmailGloves());
            SetWearable(new LeatherGorget());
        }

        private void StandardMageEquip()
        {
            bool mage = AI == AIType.AI_Mage;

            SetWearable(new WizardsHat(), mage ? Utility.RandomBlueHue() : Utility.RandomRedHue());
            SetWearable(new Robe(), mage ? Utility.RandomBlueHue() : Utility.RandomRedHue());
            SetWearable(new LeatherGloves());
        }

        public static AIType GetAI(SkillName skill)
        {
            switch (skill)
            {
                default: return AIType.AI_Melee;
                case SkillName.Ninjitsu: return AIType.AI_Ninja;
                case SkillName.Bushido: return AIType.AI_Samurai;
                case SkillName.Chivalry: return AIType.AI_Paladin;
                case SkillName.Magery: return AIType.AI_Mage;
                case SkillName.Necromancy: return AIType.AI_NecroMage;
                case SkillName.Spellweaving: return AIType.AI_Spellweaving;
                case SkillName.Mysticism: return AIType.AI_Mystic;
            }
        }

        public Item RandomSwordWeapon()
        {
            if (Race == Race.Elf)
                return Loot.Construct(new Type[] { typeof(ElvenMachete), typeof(RadiantScimitar) });

            return Loot.Construct(new Type[] { typeof(Broadsword), typeof(Longsword), typeof(Katana), typeof(Halberd), typeof(Bardiche), typeof(VikingSword) });
        }

        public Item RandomFencingWeapon()
        {
            if(Race == Race.Elf)
                return Loot.Construct(new Type[] { typeof(Leafblade), typeof(WarCleaver), typeof(AssassinSpike) });

            return Loot.Construct(new Type[] { typeof(Kryss), typeof(Spear), typeof(ShortSpear), typeof(Lance), typeof(Pike) });
        }

        public Item RandomMaceWeapon()
        {
            return Loot.Construct(new Type[] { typeof(Mace), typeof(WarHammer), typeof(WarAxe), typeof(BlackStaff), typeof(QuarterStaff), typeof(WarMace), typeof(DiamondMace), typeof(Scepter)  });
        }

        public Item RandomArhceryWeapon()
        {
            if (Race == Race.Elf)
                return Loot.Construct(new Type[] { typeof(MagicalShortbow), typeof(ElvenCompositeLongbow) });

            return Loot.Construct(new Type[] { typeof(Bow), typeof(Crossbow), typeof(HeavyCrossbow), typeof(CompositeBow), typeof(RepeatingCrossbow) });
        }

        public Item RandomMageWeapon()
        {
            return Loot.Construct(new Type[] { typeof(Spellbook), typeof(GnarledStaff), typeof(BlackStaff), typeof(QuarterStaff), typeof(WildStaff) });
        }

        public Item RandomSamuraiWeapon()
        {
            return Loot.Construct(new Type[] { typeof(Lajatang), typeof(Wakizashi), typeof(NoDachi) });
        }

        public Item RandomNinjaWeapon()
        {
            return Loot.Construct(new Type[] { typeof(Wakizashi), typeof(Tessen), typeof(Nunchaku), typeof(Daisho), typeof(Sai), typeof(Tekagi), typeof(Kama), typeof(Katana) });
        }

        public Item RandomAssassinWeapon()
        {
            return Loot.Construct(new Type[] { typeof(Cleaver), typeof(ButcherKnife), typeof(Kryss), typeof(Dagger) });
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1154196 + (int)_InvasionType);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant == null)
                return;

            if (CanDoSpecial && InRange(Combatant, 4) && 0.1 > Utility.RandomDouble() && _NextSpecial < DateTime.UtcNow)
            {
                DoSpecial();

                _NextSpecial = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
            }
            else if (_Sampire)
            {
                if (0.1 > Utility.RandomDouble() && Weapon is BaseWeapon && !CurseWeaponSpell.IsCursed(this, (BaseWeapon)Weapon))
                {
                    CurseWeaponSpell spell = new CurseWeaponSpell(this, null);
                    spell.Cast();
                }
                else if (!TransformationSpellHelper.UnderTransformation(this, typeof(VampiricEmbraceSpell)))
                {
                    VampiricEmbraceSpell spell = new VampiricEmbraceSpell(this, null);
                    spell.Cast();
                }
            }
        }

        private void DoSpecial()
        {
            if (this.Map == null || this.Map == Map.Internal)
                return;

            Map m = this.Map;

            for (int i = 0; i < 4; i++)
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(i * 50), o =>
                {
                    Server.Misc.Geometry.Circle2D(this.Location, m, (int)o, (pnt, map) =>
                    {
                        Effects.SendLocationEffect(pnt, map, Utility.RandomBool() ? 14000 : 14013, 14, 20, 2018, 0);
                    });
                }, i);
            }

            Timer.DelayCall(TimeSpan.FromMilliseconds(200), () =>
                {
                    if (m != null)
                    {
                        List<Mobile> list = new List<Mobile>();
                        IPooledEnumerable eable = m.GetMobilesInRange(this.Location, 4);

                        foreach (Mobile mob in eable)
                        {
                            if (mob.AccessLevel > AccessLevel.Player)
                                continue;

                            if (mob is PlayerMobile || (mob is BaseCreature && ((BaseCreature)mob).GetMaster() is PlayerMobile) && CanBeHarmful(mob))
                                list.Add(mob);
                        }

                        list.ForEach(mob =>
                            {
                                AOS.Damage(mob, this, Utility.RandomMinMax(80, 90), 0, 0, 0, 0, 0, 100, 0);
                            });

                        list.Clear();
                        list.TrimExcess();
                    }
                });
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
        }

        public Invader(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)_Specialty);
            writer.Write((int)_InvasionType);
            writer.Write(_Sampire);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Specialty = (SkillName)reader.ReadInt();
            _InvasionType = (InvasionType)reader.ReadInt();
            _Sampire = reader.ReadBool();

            _NextSpecial = DateTime.UtcNow;
        }
    }

    public class InvaderCaptain : Invader
    {
        public override double MinSkill { get { return 105.0; } }
        public override double MaxSkill { get { return 130.0; } }
        public override int MinResist { get { return 20; } }
        public override int MaxResist { get { return 30; } }

        [Constructable]
        public InvaderCaptain(InvasionType type) : base(type)
        {
            SetStr(250);
            SetDex(SpellCaster ? 150 : 200);
            SetInt(SpellCaster ? 1000 : 5000);

            SetHits(8000, 12000);

            if (AI == AIType.AI_Melee)
                SetDamage(22, 30);
            else if (!SpellCaster)
                SetDamage(20, 28);
            else
                SetDamage(10, 20);

            Fame = 48000;
            Karma = -48000;  
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            var rights = GetLootingRights();
            rights.Sort();

            List<Mobile> list = rights.Select(x => x.m_Mobile).Where(m => m.InRange(c.Location, 20)).ToList();

            if(list.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Mobile drop;
                    Item item = InvasionController.CreateItem(list[0]);

                    if (list.Count == 1 || i >= list.Count)
                        drop = list[0];
                    else
                        drop = list[i];

                    drop.SendLocalizedMessage(1154530); // You notice the crest of Minax on your fallen foe's equipment and decide it may be of some value...

                    if (drop.Backpack == null || !drop.Backpack.TryDropItem(drop, item, false))
                    {
                        drop.BankBox.DropItem(item);
                        drop.SendLocalizedMessage(1079730); // // The item has been placed into your bank box.
                    }
                }
            }

            ColUtility.Free(list);
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
            this.AddLoot(LootPack.SuperBoss, 1);
        }

        public InvaderCaptain(Serial serial)
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