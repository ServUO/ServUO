using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("an ant lion corpse")]
    public class AntLion : BaseCreature
    {
        private DateTime _NextTunnel;
        private Map _StartTunnelMap;
        private Point3D _StartTunnelLoc;
        private bool _Tunneling;

        [Constructable]
        public AntLion()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an ant lion";
            Body = 787;
            BaseSoundID = 1006;

            SetStr(296, 320);
            SetDex(81, 105);
            SetInt(36, 60);

            SetHits(151, 162);

            SetDamage(7, 21);

            SetDamageType(ResistanceType.Physical, 70);
            SetDamageType(ResistanceType.Poison, 30);

            SetResistance(ResistanceType.Physical, 45, 60);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 35);

            SetSkill(SkillName.MagicResist, 70.0);
            SetSkill(SkillName.Tactics, 90.0);
            SetSkill(SkillName.Wrestling, 90.0);

            Fame = 4500;
            Karma = -4500;
           
            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.PeculiarSeed3);
            AddLoot(LootPack.Bones);
            AddLoot(LootPack.LootItem<Bone>(100.0, 3, false, true));
            AddLoot(LootPack.LootItem<FertileDirt>(100.0, Utility.RandomMinMax(1, 5), false, true));

            AddLoot(LootPack.LootItemCallback(RandomOre, 100.0, Utility.RandomMinMax(1, 10), false, true));
            AddLoot(LootPack.LootItemCallback(RandomSkeleton, 7.0, 1, false, true));
        }

        private Item RandomOre(IEntity e)
        {
            Item orepile = null;

            switch (Utility.Random(4))
            {
                case 0:
                    orepile = new DullCopperOre();
                    break;
                case 1:
                    orepile = new ShadowIronOre();
                    break;
                case 2:
                    orepile = new CopperOre();
                    break;
                default:
                    orepile = new BronzeOre();
                    break;
            }

            orepile.ItemID = 0x19B9;

            return orepile;
        }

        private Item RandomSkeleton(IEntity e)
        {
            switch (Utility.Random(3))
            {
                default:
                case 0: return new UnknownBardSkeleton();
                case 1: return new UnknownMageSkeleton();
                case 2: return new UnknownRogueSkeleton();
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (!(Combatant is Mobile))
                return;

            Mobile combatant = Combatant as Mobile;

            if (_NextTunnel < DateTime.UtcNow && combatant.InRange(Location, 10))
            {
                _NextTunnel = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 40));
                DoTunnel(combatant);
            }
        }

        private void DoTunnel(Mobile combatant)
        {
            PublicOverheadMessage(Network.MessageType.Regular, 0x3B3, false, "* The ant lion begins tunneling into the ground *");
            Effects.SendTargetParticles(this, 0x36B0, 20, 10, 1734, 0, 5044, EffectLayer.Head, 0);

            Frozen = true;
            _Tunneling = true;
            _StartTunnelLoc = Location;
            _StartTunnelMap = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                {
                    if (_Tunneling)
                    {
                        Hidden = true;
                        Blessed = true;

                        Item item = new InternalItem(3892);
                        item.MoveToWorld(Location, Map);

                        item = new InternalItem(4967);
                        item.MoveToWorld(Location, Map);

                        Timer.DelayCall(TimeSpan.FromSeconds(3), () =>
                            {
                                Hidden = false;
                                Blessed = false;

                                if (!combatant.Alive || !combatant.InRange(_StartTunnelLoc, 20) || combatant.Map != _StartTunnelMap)
                                {
                                    MoveToWorld(_StartTunnelLoc, _StartTunnelMap);
                                }
                                else
                                {
                                    MoveToWorld(combatant.Location, combatant.Map);
                                    AOS.Damage(combatant, this, 25, 70, 0, 0, 30, 0);

                                    Item item2 = new InternalItem(3892);
                                    item2.MoveToWorld(Location, Map);

                                    item2 = new InternalItem(4967);
                                    item2.MoveToWorld(Location, Map);
                                }

                                _StartTunnelLoc = Point3D.Zero;
                                _StartTunnelMap = null;
                                _Tunneling = false;
                                Frozen = false;
                            });
                    }
                });
        }

        public override int Damage(int amount, Mobile from, bool informMount, bool checkDisrupt)
        {
            if (_Tunneling && !Hidden && 0.25 > Utility.RandomDouble())
            {
                PublicOverheadMessage(Network.MessageType.Regular, 0x3B3, false, "* You interrupt the ant lion's digging! *");

                Frozen = false;
                Hidden = false;
                Blessed = false;
                _Tunneling = false;
                _StartTunnelLoc = Point3D.Zero;
                _StartTunnelMap = null;
            }

            return base.Damage(amount, from, informMount, checkDisrupt);
        }

        public AntLion(Serial serial)
            : base(serial)
        {
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            if (attacker.Weapon is BaseRanged)
                BeginAcidBreath();

            base.OnGotMeleeAttack(attacker);
        }

        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);

            BeginAcidBreath();
        }

        #region Acid Breath
        private DateTime m_NextAcidBreath;

        public void BeginAcidBreath()
        {
            PlayerMobile m = Combatant as PlayerMobile;

            if (m == null || m.Deleted || !m.Alive || !Alive || m_NextAcidBreath > DateTime.Now || !CanBeHarmful(m))
                return;

            PlaySound(0x118);
            MovingEffect(m, 0x36D4, 1, 0, false, false, 0x3F, 0);

            TimeSpan delay = TimeSpan.FromSeconds(GetDistanceToSqrt(m) / 5.0);
            Timer.DelayCall(delay, new TimerStateCallback<Mobile>(EndAcidBreath), m);

            m_NextAcidBreath = DateTime.Now + TimeSpan.FromSeconds(5);
        }

        public void EndAcidBreath(Mobile m)
        {
            if (m == null || m.Deleted || !m.Alive || !Alive)
                return;

            if (0.2 >= Utility.RandomDouble())
                m.ApplyPoison(this, Poison.Greater);

            AOS.Damage(m, Utility.RandomMinMax(100, 120), 0, 0, 0, 100, 0);
        }
        #endregion

        public override int GetAngerSound() { return 0x5A; }
        public override int GetIdleSound() { return 0x5A; }
        public override int GetAttackSound() { return 0x164; }
        public override int GetHurtSound() { return 0x187; }
        public override int GetDeathSound() { return 0x1BA; }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Hidden = false;
            Blessed = false;
        }

        private class InternalItem : Item
        {
            public override int LabelNumber => 1027025;

            public InternalItem(int id)
                : base(id)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(10), Delete);
                Hue = 1;
            }

            public InternalItem(Serial serial)
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

                Delete();
            }
        }
    }
}
