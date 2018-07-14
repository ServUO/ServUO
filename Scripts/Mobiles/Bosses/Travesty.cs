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
        private bool m_SpawnedHelpers;
        private Timer m_Timer;

        [Constructable]
        public Travesty()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
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

            if (Utility.RandomDouble() < 0.10)
                c.DropItem(new HumanFeyLeggings());

            if (Utility.RandomDouble() < 0.6)
                c.DropItem(new ParrotItem());

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new TragicRemainsOfTravesty());

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new ImprisonedDog());

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new MarkOfTravesty());

            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrimsonCincture());

            if (Utility.RandomDouble() < 0.025)
            {
                switch (Utility.Random(7))
                {
                    case 0:
                        c.DropItem(new AssassinLegs());
                        break;
                    case 1:
                        c.DropItem(new AssassinArms());
                        break;
                    case 2:
                        c.DropItem(new AssassinGloves());
                        break;
                    case 3:
                        c.DropItem(new MalekisHonor());
                        break;
                    case 4:
                        c.DropItem(new JusticeBreastplate());
                        break;
                    case 5:
                        c.DropItem(new CompassionArms());
                        break;
                    case 6:
                        c.DropItem(new ValorGauntlets());
                        break;
                }
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Utility.RandomBool() && from != null)
            {
                Clone clone = new Clone(this);
                clone.MoveToWorld(Location, Map);

                FixedParticles(0x376A, 1, 14, 0x13B5, 0, 0, EffectLayer.Waist);
                PlaySound(0x511);

                from.Combatant = clone;

                from.SendLocalizedMessage(1063141); // Your attack has been diverted to a nearby mirror image of your target!
            }

            if (0.25 > Utility.RandomDouble() && DateTime.UtcNow > m_NextBodyChange)
                ChangeBody();

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

            if (list.Count <= 0)
            {
                if (Body != 0x108)
                    RestoreBody();

                return;
            }

            Mobile attacker = (Mobile)list[Utility.Random(list.Count - 1)];

            Body = attacker.Body;
            Hue = attacker.Hue;
            Name = attacker.Name;
            Female = attacker.Female;
            Title = "(Travesty)";
            HairItemID = attacker.HairItemID;
            HairHue = attacker.HairHue;
            FacialHairItemID = attacker.FacialHairItemID;
            FacialHairHue = attacker.FacialHairHue;

            foreach (Item item in attacker.Items)
            {
                if (item.Layer != Layer.Backpack && item.Layer != Layer.Mount && item.Layer != Layer.Bank)
                {
                    if (FindItemOnLayer(item.Layer) == null)
                    {
                        AddItem(new ClonedItem(item));
                    }
                }
            }

            if (attacker.Skills[SkillName.Swords].Value >= 50.0 || attacker.Skills[SkillName.Fencing].Value >= 50.0 || attacker.Skills[SkillName.Macing].Value >= 50.0)
                ChangeAIType(AIType.AI_Melee);

            if (attacker.Skills[SkillName.Archery].Value >= 50.0)
                ChangeAIType(AIType.AI_Archer);

            if (attacker.Skills[SkillName.Spellweaving].Value >= 50.0)
                ChangeAIType(AIType.AI_Spellweaving);

            if (attacker.Skills[SkillName.Magery].Value >= 50.0)
                ChangeAIType(AIType.AI_Mage);

            if (attacker.Skills[SkillName.Necromancy].Value >= 50.0)
                ChangeAIType(AIType.AI_Necro);

            if (attacker.Skills[SkillName.Necromancy].Value >= 50.0 && attacker.Skills[SkillName.Magery].Value >= 50.0)
                ChangeAIType(AIType.AI_NecroMage);

            PlaySound(0x511);
            FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);

            m_NextBodyChange = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);

            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), new TimerCallback(RestoreBody));
        }

        public void DeleteItems()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
                if (Items[i] is ClonedItem)
                    Items[i].Delete();

            if (Backpack != null)
            {
                for (int i = Backpack.Items.Count - 1; i >= 0; i--)
                    if (Backpack.Items[i] is ClonedItem)
                        Backpack.Items[i].Delete();
            }
        }

        public virtual void RestoreBody()
        {
            Name = "Travesty";
            Title = null;
            Body = 264;
            Hue = 0;

            DeleteItems();

            ChangeAIType(AIType.AI_Melee);

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public override bool OnBeforeDeath()
        {
            if (m_Timer != null)
                m_Timer.Stop();

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
        }

        public static void SpawnNinjaGroup(Point3D _location)
        {
            BaseCreature ninja = new BlackOrderAssassin();
            ninja.MoveToWorld(_location, Map.Malas);

            ninja = new BlackOrderThief();
            ninja.MoveToWorld(_location, Map.Malas);

            ninja = new BlackOrderMage();
            ninja.MoveToWorld(_location, Map.Malas);
        }

        #endregion

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
