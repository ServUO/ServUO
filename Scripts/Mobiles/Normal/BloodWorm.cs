using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace Server.Mobiles
{
    public interface IBloodCreature
    {
    }

    [CorpseName("a bloodworm corpse")]
    public class BloodWorm : BaseCreature, IBloodCreature
    {
        [Constructable]
        public BloodWorm()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a bloodworm";
            this.Body = 287;

            this.SetStr(401, 473);
            this.SetDex(80);
            this.SetInt(18, 19);

            this.SetHits(374, 422);

            this.SetDamage(11, 17);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 52, 55);
            this.SetResistance(ResistanceType.Fire, 42, 50);
            this.SetResistance(ResistanceType.Cold, 29, 31);
            this.SetResistance(ResistanceType.Poison, 69, 75);
            this.SetResistance(ResistanceType.Energy, 26, 27);

            this.SetSkill(SkillName.MagicResist, 35.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.Fame = 10000;
            this.Karma = -10000;

            this.QLPoints = 15;
        }

        public override int GetAngerSound() { return 0x5DF; }
        public override int GetIdleSound() { return 0x5DF; }
        public override int GetAttackSound() { return 0x5DC; }
        public override int GetHurtSound() { return 0x5DE; }
        public override int GetDeathSound() { return 0x5DD; }

        public BloodWorm(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.02)
                c.DropItem(new LuckyCoin());
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomBool())
            {
                if (Rotworm.IsDiseased(defender))
                {
                    // * The bloodworm is repulsed by your diseased blood. *
                    defender.SendLocalizedMessage(1111668, "", 0x25);
                }
                else
                {
                    // *The bloodworm drains some of your blood to replenish its health.*
                    defender.SendLocalizedMessage(1111698, "", 0x25);

                    Hits += (defender.HitsMax - defender.Hits);
                }
            }

            if (0.1 > Utility.RandomDouble() && !IsAnemic(defender) && !FountainOfFortune.HasBlessing(defender, FountainOfFortune.BlessingType.Protection))
            {
                defender.SendLocalizedMessage(1111669); // The bloodworm's attack weakens you. You have become anemic.

                defender.AddStatMod(new StatMod(StatType.Str, "[Bloodworm] Str Malus", -40, TimeSpan.FromSeconds(15.0)));
                defender.AddStatMod(new StatMod(StatType.Dex, "[Bloodworm] Dex Malus", -40, TimeSpan.FromSeconds(15.0)));
                defender.AddStatMod(new StatMod(StatType.Int, "[Bloodworm] Int Malus", -40, TimeSpan.FromSeconds(15.0)));

                Effects.SendPacket(defender, defender.Map, new GraphicalEffect(EffectType.FixedFrom, defender.Serial, Serial.Zero, 0x375A, defender.Location, defender.Location, 9, 20, true, false));
                Effects.SendTargetParticles(defender, 0x373A, 1, 15, 0x26B9, EffectLayer.Head);
                Effects.SendLocationParticles(defender, 0x11A6, 9, 32, 0x253A);

                defender.PlaySound(0x1ED);

                Timer.DelayCall(TimeSpan.FromSeconds(15.0), new TimerCallback(
                    delegate
                    {
                        // You recover from your anemia.
                        defender.SendLocalizedMessage(1111670);

                        defender.RemoveStatMod("[Bloodworm] Str Malus");
                        defender.RemoveStatMod("[Bloodworm] Dex Malus");
                        defender.RemoveStatMod("[Bloodworm] Int Malus");
                    }
                ));
            }
        }

        public static bool IsAnemic(Mobile m)
        {
            return m.GetStatMod("[Bloodworm] Str Malus") != null;
        }

        public override void OnAfterMove(Point3D oldLocation)
        {
            base.OnAfterMove(oldLocation);

            if (Hits < HitsMax && 0.25 > Utility.RandomDouble())
            {
                Corpse toAbsorb = null;

                foreach (Item item in Map.GetItemsInRange(Location, 1))
                {
                    if (item is Corpse)
                    {
                        Corpse c = (Corpse)item;

                        if (c.ItemID == 0x2006)
                        {
                            toAbsorb = c;
                            break;
                        }
                    }
                }

                if (toAbsorb != null)
                {
                    toAbsorb.ProcessDelta();
                    toAbsorb.SendRemovePacket();
                    toAbsorb.ItemID = Utility.Random(0xECA, 9); // bone graphic
                    toAbsorb.Hue = 0;
                    toAbsorb.Direction = Direction.North;
                    toAbsorb.ProcessDelta();

                    Hits = HitsMax;

                    // * The bloodworm drains blood from a nearby corpse to heal itself. *
                    PublicOverheadMessage(MessageType.Regular, 0x3B2, 1111699);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}