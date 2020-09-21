using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Misc;
using Server.Network;

namespace Server.Engines.JollyRoger
{
    public enum MasterTitle
    {
        Swordsman,
        Fencer,
        Macer,
        Archer,
        Wizard,
        Sampire,
        Necromancer,
        Assassin,
        Bard,
        Rogue,
        Mystic
    }

    [CorpseName("a human corpse")]
    public class ShrineMaster : BaseCreature
    {
        public static MasterTitle RandomSpecialty()
        {
            var master = Enum.GetValues(typeof(MasterTitle));
            return (MasterTitle)master.GetValue(Utility.Random(master.Length));
        }

        private MasterType _Type;
        private MasterTitle _Specialty;

        private DateTime _NextSpecial;

        public override bool AlwaysMurderer => true;
        public override double HealChance => AI == AIType.AI_Melee || AI == AIType.AI_Paladin ? 1.0 : 0.0;
        public override double WeaponAbilityChance => AI == AIType.AI_Melee || AI == AIType.AI_Paladin ? 0.4 : 0.1;
        public override bool CanStealth => _Specialty == MasterTitle.Assassin || _Specialty == MasterTitle.Rogue;
        public override bool CanPeace => _Specialty == MasterTitle.Bard;
        public override bool AutoDispel => _Specialty == MasterTitle.Wizard;
        public override bool AlwaysAttackable => _Specialty == MasterTitle.Archer;
        public override Poison PoisonImmune => _Specialty == MasterTitle.Assassin ? Poison.Lethal : null;

        public override WeaponAbility GetWeaponAbility()
        {
            BaseWeapon wep = Weapon as BaseWeapon;

            if (wep != null)
            {
                return 0.6 > Utility.RandomDouble() ? wep.PrimaryAbility : wep.SecondaryAbility;
            }

            return null;
        }

        public override bool UseSmartAI => true;

        public virtual double MinSkill => 105.0;
        public virtual double MaxSkill => 130.0;
        public virtual int MinResist => 20;
        public virtual int MaxResist => 30;

        public bool SpellCaster => AI != AIType.AI_Melee && AI != AIType.AI_Ninja && AI != AIType.AI_Samurai && AI != AIType.AI_Paladin;

        [Constructable]
        public ShrineMaster(MasterType type, ShrineBattleController controller)
            : this(RandomSpecialty(), type, controller)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ShrineBattleController _Controller { get; set; }

        [Constructable]
        public ShrineMaster(MasterTitle specialty, MasterType type, ShrineBattleController controller)
            : base(GetAI(specialty), FightMode.Closest, 10, 1, .2, .1)
        {
            _Specialty = specialty;
            _Type = type;
            _Controller = controller;

            if (Female = Utility.RandomBool())
            {
                Name = NameList.RandomName("female");
            }
            else
            {
                Name = NameList.RandomName("male");
            }

            SetBody();

            Title = "the " + specialty.ToString();

            SetStr(250);
            SetDex(SpellCaster ? 150 : 200);
            SetInt(SpellCaster ? 1000 : 5000);

            SetHits(20000, 25000);

            if (AI == AIType.AI_Melee)
                SetDamage(22, 30);
            else if (!SpellCaster)
                SetDamage(20, 28);
            else
                SetDamage(10, 20);

            Fame = 48000;
            Karma = -48000;

            SetResists();
            SetSkills();
            EquipSpecialty();

            _NextSpecial = DateTime.UtcNow;

            if (specialty == MasterTitle.Sampire)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                    {
                        VampiricEmbraceSpell spell = new VampiricEmbraceSpell(this, null);
                        spell.Cast();
                    });
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Blessed)
            {
                from.SendLocalizedMessage(1071372); // It's covered with treasure guardian's magical power. To touch it, you need to beat them!
                return;
            }

            base.OnDoubleClick(from);
        }

        public virtual void SetSkills()
        {
            SetSkill(SkillName.Wrestling, MinSkill, MaxSkill);
            SetSkill(SkillName.Tactics, MinSkill, MaxSkill);
            SetSkill(SkillName.Anatomy, MinSkill, MaxSkill);

            switch (_Specialty)
            {
                case MasterTitle.Swordsman: // Swordsman
                    SetSkill(SkillName.Swords, MinSkill, MaxSkill);
                    SetSkill(SkillName.Tactics, MinSkill, MaxSkill);
                    SetSkill(SkillName.Parry, MinSkill, MaxSkill);
                    SetSkill(SkillName.Bushido, MinSkill, MaxSkill);
                    SetSpecialAbility(SpecialAbility.AngryFire);
                    SetSpecialAbility(SpecialAbility.SearingWounds);
                    SetWeaponAbility(WeaponAbility.FrenziedWhirlwind);
                    break;
                case MasterTitle.Fencer: // Fencer
                    SetSkill(SkillName.Fencing, MinSkill, MaxSkill);
                    SetSkill(SkillName.Tactics, MinSkill, MaxSkill);
                    SetSkill(SkillName.Parry, MinSkill, MaxSkill);
                    SetSkill(SkillName.Ninjitsu, MinSkill, MaxSkill);
                    SetWeaponAbility(WeaponAbility.Feint);
                    SetWeaponAbility(WeaponAbility.ArmorIgnore);
                    break;
                case MasterTitle.Macer: // Macer
                    SetSkill(SkillName.Macing, MinSkill, MaxSkill);
                    SetSkill(SkillName.Tactics, MinSkill, MaxSkill);
                    SetSkill(SkillName.Parry, MinSkill, MaxSkill);
                    SetWeaponAbility(WeaponAbility.CrushingBlow);
                    break;
                case MasterTitle.Archer: // Archer
                    SetSkill(SkillName.Archery, MinSkill, MaxSkill);
                    SetSkill(SkillName.MagicResist, MinSkill, MaxSkill);
                    SetSkill(SkillName.Swords, MinSkill, MaxSkill);
                    SetSkill(SkillName.Tactics, MinSkill, MaxSkill);
                    SetWeaponAbility(WeaponAbility.PsychicAttack);
                    SetWeaponAbility(WeaponAbility.ForceArrow);
                    SetWeaponAbility(WeaponAbility.ParalyzingBlow);
                    break;
                case MasterTitle.Wizard: // Wizard
                    SetSkill(SkillName.Magery, MinSkill, MaxSkill);
                    SetSkill(SkillName.EvalInt, MinSkill, MaxSkill);
                    SetSkill(SkillName.Meditation, MinSkill, MaxSkill);
                    SetSkill(SkillName.MagicResist, MinSkill, MaxSkill);
                    SetAreaEffect(AreaEffect.AuraDamage);
                    break;
                case MasterTitle.Mystic: // Mystic
                    SetSkill(SkillName.Mysticism, MinSkill, MaxSkill);
                    SetSkill(SkillName.Focus, MinSkill, MaxSkill);
                    SetSkill(SkillName.Meditation, MinSkill, MaxSkill);
                    SetSkill(SkillName.MagicResist, MinSkill, MaxSkill);
                    break;
                case MasterTitle.Necromancer: // Necromancer
                    SetSkill(SkillName.Necromancy, MinSkill, MaxSkill);
                    SetSkill(SkillName.SpiritSpeak, MinSkill, MaxSkill);
                    SetSkill(SkillName.MagicResist, MinSkill, MaxSkill);
                    SetSpecialAbility(SpecialAbility.ManaDrain);
                    SetSpecialAbility(SpecialAbility.LifeLeech);
                    break;
                case MasterTitle.Sampire: // Sampire
                    SetSkill(SkillName.Chivalry, MinSkill, MaxSkill);
                    SetSkill(SkillName.Bushido, MinSkill, MaxSkill);
                    SetSkill(SkillName.Necromancy, MinSkill, MaxSkill);
                    SetSkill(SkillName.SpiritSpeak, MinSkill, MaxSkill);
                    SetSkill(SkillName.Swords, MinSkill, MaxSkill);
                    SetSkill(SkillName.Parry, MinSkill, MaxSkill);
                    break;
                case MasterTitle.Assassin: // Assassin
                    SetSkill(SkillName.Hiding, MinSkill, MaxSkill);
                    SetSkill(SkillName.Stealth, MinSkill, MaxSkill);
                    SetSkill(SkillName.Poisoning, MinSkill, MaxSkill);
                    SetSkill(SkillName.Swords, MinSkill, MaxSkill);
                    SetSkill(SkillName.Fencing, MinSkill, MaxSkill);
                    SetSkill(SkillName.Ninjitsu, MinSkill, MaxSkill);
                    SetAreaEffect(AreaEffect.PoisonBreath);
                    break;
                case MasterTitle.Bard: // Bard
                    SetSkill(SkillName.Musicianship, MinSkill, MaxSkill);
                    SetSkill(SkillName.Peacemaking, MinSkill, MaxSkill);
                    SetSpecialAbility(SpecialAbility.HowlOfCacophony);
                    break;
                case MasterTitle.Rogue: // Rogue
                    SetSkill(SkillName.Archery, MinSkill, MaxSkill);
                    SetSkill(SkillName.MagicResist, MinSkill, MaxSkill);
                    SetSkill(SkillName.Swords, MinSkill, MaxSkill);
                    SetSkill(SkillName.Tactics, MinSkill, MaxSkill);
                    SetSkill(SkillName.Poisoning, MinSkill, MaxSkill);
                    SetSkill(SkillName.Hiding, MinSkill, MaxSkill);
                    SetSkill(SkillName.Stealth, MinSkill, MaxSkill);
                    SetAreaEffect(AreaEffect.EssenceOfDisease);
                    break;
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
                case MasterTitle.Mystic: Race = Race.Gargoyle;
                    break;
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

        public virtual void EquipSpecialty()
        {
            Item item;

            switch (_Specialty)
            {
                case MasterTitle.Bard:
                    SetWearable(new QuarterStaff());
                    SetWearable(new FeatheredHat(1308));

                    if (Female)
                    {
                        SetWearable(new HideFemaleChest());
                    }
                    else
                    {
                        SetWearable(new HideChest());
                    }

                    SetWearable(new HidePauldrons());
                    SetWearable(new HideGorget());
                    SetWearable(new LeafGloves());
                    SetWearable(new LeatherSkirt());
                    SetWearable(new ThighBoots());
                    SetWearable(new BodySash(1308));
                    break;
                case MasterTitle.Macer:
                    SetWearable(new WarMace());
                    SetWearable(new PlateArms());
                    SetWearable(new PlateLegs());
                    SetWearable(new PlateGloves());
                    SetWearable(new PlateHelm());
                    SetWearable(new PlateChest());
                    SetWearable(new HeaterShield());
                    SetWearable(new Surcoat());
                    break;
                case MasterTitle.Rogue:
                    SetWearable(new RepeatingCrossbow());
                    SetWearable(new LeatherGloves());
                    SetWearable(new Doublet());
                    SetWearable(new Cap());
                    SetWearable(new QuiverOfInfinity());
                    SetWearable(new LeatherArms());
                    SetWearable(new LeatherLegs());
                    SetWearable(new Sandals());
                    break;
                case MasterTitle.Archer:
                    SetWearable(new MagicalShortbow());
                    SetWearable(new RingmailGloves());
                    SetWearable(new ChainChest());
                    SetWearable(new BodySash(1193));
                    SetWearable(new ThighBoots());
                    SetWearable(new Cap());
                    SetWearable(new QuiverOfInfinity());
                    break;
                case MasterTitle.Wizard:
                    SetWearable(new Kilt(1156));

                    if (Female)
                    {
                        SetWearable(new LeatherBustierArms(), 2498);
                    }
                    else
                    {
                        SetWearable(new LeatherArms(), 2498);
                    }

                    SetWearable(new LeatherGloves(), 2498);
                    SetWearable(new BodySash(1156));
                    SetWearable(new WizardsHat(1156));
                    SetWearable(new Sandals(2498));
                    SetWearable(new Spellbook());
                    break;
                case MasterTitle.Mystic:
                    SetWearable(new MysticBook((uint)0xFFF));
                    SetWearable(new GargishFancyRobe());
                    SetWearable(new GargishEarrings());
                    SetWearable(new GargishClothWingArmor());
                    SetWearable(new LeatherTalons());
                    break;
                case MasterTitle.Sampire:
                    SetWearable(new DoubleAxe(), 1858);
                    SetWearable(new Epaulette(), 1192);
                    SetWearable(new ChainChest());
                    SetWearable(new ChainLegs());
                    SetWearable(new RingmailGloves(), 1863);
                    SetWearable(new ElvenBoots());
                    SetWearable(new BodySash(1866));
                    SetWearable(new RoyalCirclet());
                    break;
                case MasterTitle.Necromancer:
                    SetWearable(new NecromancerSpellbook());
                    SetWearable(new BoneChest(), 1910);
                    SetWearable(new BoneLegs(), 1910);
                    SetWearable(new LeatherGloves(), 1175);
                    SetWearable(new Sandals());
                    SetWearable(new Cloak(), 1157);
                    SetWearable(new GoldNecklace());
                    break;
                case MasterTitle.Assassin:
                    SetWearable(new AssassinSpike());
                    SetWearable(new LeatherChest(), 2051);
                    SetWearable(new LeatherLegs(), 2051);
                    SetWearable(new LeatherGloves());
                    SetWearable(new LeatherGorget(), 2051);
                    SetWearable(new Sandals());
                    SetWearable(new LeatherNinjaBelt());
                    SetWearable(new Doublet(), 2051);
                    SetWearable(new LeatherNinjaHood(), 2051);
                    break;
                case MasterTitle.Swordsman:
                    item = new PaladinSword
                    {
                        LootType = LootType.Blessed
                    };
                    SetWearable(item);

                    item = new Daisho
                    {
                        LootType = LootType.Blessed
                    };
                    PackItem(item);

                    item = new Wakizashi
                    {
                        LootType = LootType.Blessed
                    };
                    PackItem(item);

                    SetWearable(new NorseHelm(), 2406);
                    SetWearable(new PlateArms(), 2406);
                    SetWearable(new DragonGloves(), 2406);
                    SetWearable(new DragonChest(), 2406);
                    SetWearable(new PlateLegs(), 2406);
                    SetWearable(new LeatherNinjaBelt(), 2726);
                    SetWearable(new BodySash(2726));
                    SetWearable(new Cloak(), 2726);
                    break;
                case MasterTitle.Fencer:
                    item = new Lajatang
                    {
                        LootType = LootType.Blessed
                    };
                    SetWearable(item);

                    item = new Sai
                    {
                        LootType = LootType.Blessed
                    };
                    PackItem(item);

                    item = new Tekagi
                    {
                        LootType = LootType.Blessed
                    };
                    PackItem(item);

                    if (Female)
                    {
                        SetWearable(new LeatherBustierArms());
                        SetWearable(new LeatherSkirt());
                    }
                    else
                    {
                        SetWearable(new LeatherChest());
                        SetWearable(new LeatherLegs());
                    }

                    SetWearable(new BoneArms());
                    SetWearable(new ThighBoots());
                    break;
            }
        }

        public static AIType GetAI(MasterTitle title)
        {
            switch (title)
            {
                default: return AIType.AI_Melee;
                case MasterTitle.Swordsman: return AIType.AI_Samurai;
                case MasterTitle.Fencer: return AIType.AI_Ninja;
                case MasterTitle.Sampire: return AIType.AI_Samurai;
                case MasterTitle.Macer: return AIType.AI_Melee;
                case MasterTitle.Rogue:
                case MasterTitle.Archer: return AIType.AI_Archer;
                case MasterTitle.Wizard: return AIType.AI_Mage;
                case MasterTitle.Mystic: return AIType.AI_Mystic;
                case MasterTitle.Necromancer: return AIType.AI_Necro;
                case MasterTitle.Assassin: return AIType.AI_Ninja;
                case MasterTitle.Bard: return AIType.AI_Melee;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1154196 + (int)_Type);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Blessed && _Controller != null && _Controller.MasterBlessCheck(this))
            {
                for (int i = 1; i < 4; i++)
                {
                    Timer.DelayCall(TimeSpan.FromMilliseconds(i * 50), o =>
                    {
                        Geometry.Circle2D(Location, Map, o, (pnt, map) =>
                        {
                            Effects.SendLocationEffect(pnt, map, 0x36B0, 14, 14, 2053, 0);
                        });
                    }, i);
                }

                Blessed = false;
            }

            if (!Hidden && 0.1 > Utility.RandomDouble() && (_Specialty == MasterTitle.Rogue || _Specialty == MasterTitle.Assassin))
            {
                HideSelf();
            }

            if (Combatant == null)
                return;

            if (!Utility.InRange(Location, Home, 20))
            {
                Timer.DelayCall(TimeSpan.FromSeconds(5), () => { Location = Home; });
            }

            if (0.3 > Utility.RandomDouble() && _NextSpecial < DateTime.UtcNow)
            {
                switch (_Specialty)
                {
                    case MasterTitle.Macer:
                        Teleport();
                        break;
                    case MasterTitle.Fencer:
                    case MasterTitle.Swordsman:
                        ChangeWeapon();
                        break;
                    case MasterTitle.Wizard:
                        if (0.5 > Utility.RandomDouble())
                            DoNuke(Location);
                        else
                            DoDismount((Mobile)Combatant);
                        break;

                }

                _NextSpecial = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));
            }
            else if (_Specialty == MasterTitle.Sampire)
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

        private void ChangeWeapon()
        {
            if (Backpack == null)
                return;

            Item item = FindItemOnLayer(Layer.OneHanded);

            if (item == null)
                item = FindItemOnLayer(Layer.TwoHanded);

            List<BaseWeapon> weapons = new List<BaseWeapon>();

            foreach (Item i in Backpack.Items)
            {
                if (i is BaseWeapon && i != item)
                    weapons.Add((BaseWeapon)i);
            }

            if (weapons.Count > 0)
            {
                if (item != null)
                    Backpack.DropItem(item);

                AddItem(weapons[Utility.Random(weapons.Count)]);
            }

            ColUtility.Free(weapons);
        }

        private void Teleport()
        {
            var toTele = SpellHelper.AcquireIndirectTargets(this, Location, Map, 12).OfType<PlayerMobile>().ToList();

            if (toTele.Count > 0)
            {
                PlayerMobile from = toTele[Utility.Random(toTele.Count)];

                if (from != null)
                {
                    Combatant = from;

                    from.MoveToWorld(GetSpawnPosition(1), Map);
                    from.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                    from.PlaySound(0x1FE);
                }
            }

            ColUtility.Free(toTele);
        }

        public void DoNuke(Point3D p)
        {
            if (!Alive || Map == null)
            {
                return;
            }

            var range = 8;

            //Flame Columns
            for (var i = 0; i < 2; i++)
            {
                Geometry.Circle2D(Location, Map, i, (pnt, map) =>
                {
                    Effects.SendLocationParticles(EffectItem.Create(pnt, map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                });
            }

            //Flash then boom
            Timer.DelayCall(TimeSpan.FromSeconds(1.5), () =>
            {
                if (Alive && Map != null)
                {
                    var flash = ScreenLightFlash.Instance;
                    IPooledEnumerable e = Map.GetClientsInRange(p, range * 4 + 5);

                    foreach (NetState ns in e)
                    {
                        if (ns.Mobile != null)
                        {
                            ns.Mobile.Send(flash);
                        }
                    }

                    e.Free();

                    for (var i = 0; i < range; i++)
                    {
                        Geometry.Circle2D(Location, Map, i, (pnt, map) =>
                        {
                            Effects.SendLocationEffect(pnt, map, 14000, 14, 10, Utility.RandomMinMax(2497, 2499), 2);
                        });
                    }
                }
            });

            IPooledEnumerable eable = GetMobilesInRange(range);

            foreach (Mobile m in eable)
            {
                if ((m is PlayerMobile || m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile) && CanBeHarmful(m))
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(1.75), new TimerStateCallback(DoDamage_Callback), m);
                }
            }

            eable.Free();
        }

        private void DoDamage_Callback(object o)
        {
            var m = o as Mobile;
            var map = Map;

            if (m != null && map != null)
            {
                DoHarmful(m);
                AOS.Damage(m, this, Utility.RandomMinMax(100, 150), 50, 50, 0, 0, 0);

                var d = Utility.GetDirection(this, m);
                var range = 0;
                var x = m.X;
                var y = m.Y;
                var orx = x;
                var ory = y;

                while (range < 12)
                {
                    range++;
                    var lastx = x;
                    var lasty = y;

                    Movement.Movement.Offset(d, ref x, ref y);

                    if (!map.CanSpawnMobile(x, y, map.GetAverageZ(x, y)))
                    {
                        m.MoveToWorld(new Point3D(lastx, lasty, map.GetAverageZ(lastx, lasty)), Map);
                        break;
                    }

                    if (range >= 12 && (orx != x || ory != y))
                    {
                        m.MoveToWorld(new Point3D(x, y, map.GetAverageZ(x, y)), Map);
                    }
                }

                m.Paralyze(TimeSpan.FromSeconds(3));
            }
        }

        public void DoDismount(Mobile m)
        {
            MovingParticles(m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
            PlaySound(0x15E);

            var range = m.GetDistanceToSqrt(this);

            Timer.DelayCall(TimeSpan.FromMilliseconds(250 * range), () =>
            {
                var mount = m.Mount;

                if (mount != null)
                {
                    if (m is PlayerMobile pm)
                    {
                        pm.SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                    }
                    else
                    {
                        mount.Rider = null;
                    }
                }
                else if (m.Flying)
                {
                    ((PlayerMobile)m).SetMountBlock(BlockMountType.Dazed, TimeSpan.FromSeconds(10), true);
                }

                AOS.Damage(m, this, Utility.RandomMinMax(15, 25), 100, 0, 0, 0, 0);
            });
        }

        private void HideSelf()
        {
            if (Core.TickCount >= NextSkillTime)
            {
                Effects.SendLocationParticles(
                    EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

                PlaySound(0x22F);
                Hidden = true;

                UseSkill(SkillName.Stealth);
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            List<DamageStore> rights = GetLootingRights();
            rights.Sort();

            List<Mobile> list = rights.Select(x => x.m_Mobile).Where(m => m.InRange(c.Location, 20)).ToList();

            if (list.Count > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Mobile drop;
                    Item item = ShrineBattleController.CreateItem(list[0]);

                    if (list.Count == 1 || i >= list.Count)
                        drop = list[0];
                    else
                        drop = list[i];

                    if (_Controller != null)
                    {
                        JollyRogerData.AddMasterKill(drop, _Controller.Shrine);
                        _Controller.OnMasterDestroyed();
                    }

                    drop.SendLocalizedMessage(1159318); // You notice the Fellowship Insignia on your fallen foe's equipment and decide it may be of some value...

                    if (drop.Backpack == null || !drop.Backpack.TryDropItem(drop, item, false))
                    {
                        drop.BankBox.DropItem(item);
                        drop.SendLocalizedMessage(1079730); // The item has been placed into your bank box.
                    }
                }
            }

            ColUtility.Free(list);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.LootItemCallback(CheckAbilityLootItem, 100.0, Utility.RandomMinMax(10, 25), false, true));
        }

        protected Item CheckAbilityLootItem(IEntity e)
        {
            if (AbilityProfile != null && AbilityProfile.HasAbility(SpecialAbility.Heal))
            {
                return new Bandage();
            }

            return null;
        }

        public ShrineMaster(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)_Specialty);
            writer.Write((int)_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Specialty = (MasterTitle)reader.ReadInt();
            _Type = (MasterType)reader.ReadInt();

            _NextSpecial = DateTime.UtcNow;
        }
    }
}
