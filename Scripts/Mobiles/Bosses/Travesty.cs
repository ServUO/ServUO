using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a travesty's corpse")]
    public class Travesty : BasePeerless
    {
        public override WeaponAbility GetWeaponAbility()
        {
            if (Weapon == null)
                return null;

            BaseWeapon weapon = Weapon as BaseWeapon;

            return Utility.RandomBool() ? weapon.PrimaryAbility : weapon.SecondaryAbility;
        }

        private DateTime m_NextBodyChange;
        private DateTime m_NextMirrorImage;
        private bool m_SpawnedHelpers;
        private Timer m_Timer;

        private bool _CanDiscord;
        private bool _CanPeace;
        private bool _CanProvoke;

        public override bool CanDiscord { get { return _CanDiscord; } }
        public override bool CanPeace { get { return _CanPeace; } }
        public override bool CanProvoke { get { return _CanProvoke; } }

        [Constructable]
        public Travesty()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Travesty";
            Body = 0x108;

            BaseSoundID = 0x46E;

            SetStr(900, 950);
            SetDex(900, 950);
            SetInt(900, 950);

            SetHits(35000);

            SetDamage(11, 18);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 70);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 50, 70);
            SetResistance(ResistanceType.Poison, 50, 70);
            SetResistance(ResistanceType.Energy, 50, 70);

            SetSkill(SkillName.Wrestling, 300.0, 320.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Anatomy, 100.0, 120.0);
            SetSkill(SkillName.Healing, 100.0, 120.0);
            SetSkill(SkillName.Poisoning, 100.0, 120.0);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Hiding, 100.0);
            SetSkill(SkillName.Parry, 100.0, 110.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Meditation, 100.0, 120.0);
            SetSkill(SkillName.Necromancy, 100.0, 120.0);
            SetSkill(SkillName.SpiritSpeak, 100.0, 120.0);
            SetSkill(SkillName.Focus, 100.0, 120.0);
            SetSkill(SkillName.Spellweaving, 100.0, 120.0);
            SetSkill(SkillName.Discordance, 100.0, 120.0);
            SetSkill(SkillName.Bushido, 100.0, 120.0);
            SetSkill(SkillName.Ninjitsu, 100.0, 120.0);
            SetSkill(SkillName.Chivalry, 100.0, 120.0);

            SetSkill(SkillName.Musicianship, 100.0, 120.0);
            SetSkill(SkillName.Discordance, 100.0, 120.0);
            SetSkill(SkillName.Provocation, 100.0, 120.0);
            SetSkill(SkillName.Peacemaking, 100.0, 120.0);

            Fame = 30000;
            Karma = -30000;

            VirtualArmor = 50;
            PackTalismans(5);
            PackResources(8);

            for (int i = 0; i < Utility.RandomMinMax(1, 6); i++)
            {
                PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public override bool ShowFameTitle { get { return false; } }

        public Travesty(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new EyeOfTheTravesty());
            c.DropItem(new OrdersFromMinax());

            switch (Utility.Random(3))
            {
                case 0:
                    c.DropItem(new TravestysSushiPreparations());
                    break;
                case 1:
                    c.DropItem(new TravestysFineTeakwoodTray());
                    break;
                case 2:
                    c.DropItem(new TravestysCollectionOfShells());
                    break;
            }

            if (Utility.RandomDouble() < 0.6)
                c.DropItem(new ParrotItem());

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new TragicRemainsOfTravesty());

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new ImprisonedDog());

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new MarkOfTravesty());

            if (Utility.RandomDouble() < 0.025)
            {
                c.DropItem(new MalekisHonor());
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (0.1 > Utility.RandomDouble() && m_NextMirrorImage < DateTime.UtcNow)
            {
                new Server.Spells.Ninjitsu.MirrorImage(this, null).Cast();

                m_NextMirrorImage = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 45));
            }

            if (0.25 > Utility.RandomDouble() && DateTime.UtcNow > m_NextBodyChange)
            {
                ChangeBody();
            }

            base.OnDamage(amount, from, willKill);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosSuperBoss, 8);
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

        public void ChangeBody()
        {
            List<Mobile> list = new List<Mobile>();

            IPooledEnumerable eable = Map.GetMobilesInRange(Location, 5);

            foreach (Mobile m in eable)
            {
                if (m.Player && m.AccessLevel == AccessLevel.Player && m.Alive)
                    list.Add(m);
            }

            eable.Free();

            if (list.Count == 0 || IsBodyMod)
            {
                return;
            }

            Mobile attacker = list[Utility.Random(list.Count)];

            BodyMod = attacker.Body;
            HueMod = attacker.Hue;
            NameMod = attacker.Name;
            Female = attacker.Female;
            Title = "(Travesty)";
            HairItemID = attacker.HairItemID;
            HairHue = attacker.HairHue;
            FacialHairItemID = attacker.FacialHairItemID;
            FacialHairHue = attacker.FacialHairHue;

            foreach (Item item in attacker.Items)
            {
                if (item.Layer < Layer.Mount &&
                    item.Layer != Layer.Backpack &&
                    item.Layer != Layer.Mount &&
                    item.Layer != Layer.Bank &&
                    item.Layer != Layer.Hair &&
                    item.Layer != Layer.Face &&
                    item.Layer != Layer.FacialHair)
                {
                    if (FindItemOnLayer(item.Layer) == null)
                    {
                        if (item is BaseRanged)
                        {
                            Item i = FindItemOnLayer(Layer.TwoHanded);

                            if (i != null)
                                i.Delete();

                            i = FindItemOnLayer(Layer.OneHanded);

                            if (i != null)
                                i.Delete();

                            AddItem(Loot.Construct(item.GetType()));
                        }
                        else
                        {
                            AddItem(new ClonedItem(item));
                        }
                    }
                }
            }

            if (attacker.Skills[SkillName.Swords].Value >= 50.0 || attacker.Skills[SkillName.Fencing].Value >= 50.0 || attacker.Skills[SkillName.Macing].Value >= 50.0)
                ChangeAIType(AIType.AI_Melee);

            if (attacker.Skills[SkillName.Archery].Value >= 50.0)
                ChangeAIType(AIType.AI_Archer);

            if (attacker.Skills[SkillName.Spellweaving].Value >= 50.0)
                ChangeAIType(AIType.AI_Spellweaving);

            if (attacker.Skills[SkillName.Mysticism].Value >= 50.0)
                ChangeAIType(AIType.AI_Mystic);

            if (attacker.Skills[SkillName.Magery].Value >= 50.0)
                ChangeAIType(AIType.AI_Mage);

            if (attacker.Skills[SkillName.Necromancy].Value >= 50.0)
                ChangeAIType(AIType.AI_Necro);

            if (attacker.Skills[SkillName.Ninjitsu].Value >= 50.0)
                ChangeAIType(AIType.AI_Ninja);

            if (attacker.Skills[SkillName.Bushido].Value >= 50.0)
                ChangeAIType(AIType.AI_Samurai);

            if (attacker.Skills[SkillName.Necromancy].Value >= 50.0 && attacker.Skills[SkillName.Magery].Value >= 50.0)
                ChangeAIType(AIType.AI_NecroMage);

            PlaySound(0x511);
            FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);

            m_NextBodyChange = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);

            if (attacker.Skills[SkillName.Healing].Base > 20)
            {
                SetSpecialAbility(SpecialAbility.Heal);
            }

            if (attacker.Skills[SkillName.Discordance].Base > 50)
            {
                _CanDiscord = true;
            }

            if (attacker.Skills[SkillName.Peacemaking].Base > 50)
            {
                _CanPeace = true;
            }

            if (attacker.Skills[SkillName.Provocation].Base > 50)
            {
                _CanProvoke = true;
            }

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), new TimerCallback(RestoreBody));
        }

        public void DeleteItems()
        {
            ColUtility.SafeDelete(Items, item => item is ClonedItem || item is BaseRanged);

            if (Backpack != null)
            {
                ColUtility.SafeDelete(Backpack.Items, item => item is ClonedItem || item is BaseRanged);
            }
        }

        public virtual void RestoreBody()
        {
            BodyMod = 0;
            HueMod = -1;
            NameMod = null;
            Female = false;
            Title = null;

            _CanDiscord = false;
            _CanPeace = false;
            _CanProvoke = false;

            if (HasAbility(SpecialAbility.Heal))
            {
                RemoveSpecialAbility(SpecialAbility.Heal);
            }

            DeleteItems();

            ChangeAIType(AIType.AI_Mage);

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public override bool OnBeforeDeath()
        {
            RestoreBody();

            return base.OnBeforeDeath();
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            base.OnAfterDelete();
        }

        #region Spawn Helpers
        public override bool CanSpawnHelpers { get { return true; } }
        public override int MaxHelpersWaves { get { return 1; } }

        public override bool CanSpawnWave()
        {
            if (Hits > 2000)
                m_SpawnedHelpers = false;

            return !m_SpawnedHelpers && Hits < 2000;
        }

        public override void SpawnHelpers()
        {
            m_SpawnedHelpers = true;

            SpawnNinjaGroup(new Point3D(80, 1964, 0));
            SpawnNinjaGroup(new Point3D(80, 1949, 0));
            SpawnNinjaGroup(new Point3D(92, 1948, 0));
            SpawnNinjaGroup(new Point3D(92, 1962, 0));

            if (Map != null && Map != Map.Internal && Region.IsPartOf("TheCitadel"))
            {
                var loc = _WarpLocs[Utility.Random(_WarpLocs.Length)];
                MoveToWorld(loc, Map);
            }
        }

        public void SpawnNinjaGroup(Point3D _location)
        {
            SpawnHelper(new DragonsFlameMage(), _location);
            SpawnHelper(new SerpentsFangAssassin(), _location);
            SpawnHelper(new TigersClawThief(), _location);
        }

        #endregion

        private Point3D[] _WarpLocs =
        {
            new Point3D(71, 1939, 0),
            new Point3D(71, 1955, 0),
            new Point3D(69, 1972, 0),
            new Point3D(86, 1971, 0),
            new Point3D(103, 1972, 0),
            new Point3D(86, 1939, 0),
            new Point3D(102, 1938, 0),
        };

        private class ClonedItem : Item
        {
            public ClonedItem(Item oItem)
                : base(oItem.ItemID)
            {
                Name = oItem.Name;
                Weight = oItem.Weight;
                Hue = oItem.Hue;
                Layer = oItem.Layer;
            }

            public override DeathMoveResult OnParentDeath(Mobile parent)
            {
                return DeathMoveResult.RemainEquiped;
            }

            public override DeathMoveResult OnInventoryDeath(Mobile parent)
            {
                Delete();
                return base.OnInventoryDeath(parent);
            }

            public ClonedItem(Serial serial)
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
}
