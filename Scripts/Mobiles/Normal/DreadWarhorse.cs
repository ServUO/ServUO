using System;

namespace Server.Mobiles
{
    [CorpseName("a dread warhorse corpse")]
    public class DreadWarhorse : BaseMount
    {
        private DateTime _NextTrick;

        [Constructable]
        public DreadWarhorse()
            : this("a dread warhorse")
        {
        }

        [Constructable]
        public DreadWarhorse(string name)
            : base(name, 0x74, 0x3EA7, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0xA8;
            BodyValue = 116;
            Hue = 1175;

            SetStr(500, 555);
            SetDex(89, 125);
            SetInt(100, 160);

            SetHits(555, 650);

            SetDamage(20, 26);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 20, 40);
            SetResistance(ResistanceType.Cold, 20, 40);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 15.2, 19.3);
            SetSkill(SkillName.Magery, 39.5, 49.5);
            SetSkill(SkillName.MagicResist, 91.4, 93.4);
            SetSkill(SkillName.Tactics, 108.1, 110.0);
            SetSkill(SkillName.Wrestling, 97.3, 98.2);

            Fame = 15000;
            Karma = -15000;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 108.0;
        }

        public DreadWarhorse(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 5;
        public override int Hides => 10;
        public override HideType HideType => HideType.Barbed;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override bool CanAngerOnTame => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LowScrolls);
            AddLoot(LootPack.Potions);
        }

        public override int GetAngerSound()
        {
            if (!Controlled)
                return 0x16A;

            return base.GetAngerSound();
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            if (_NextTrick > DateTime.UtcNow)
            {
                return;
            }

            var m = e.Mobile;

            if (GetMaster() == m && e.Speech.ToLower() == "trick" && !IsDeadBondedPet && !Deleted && Map != null)
            {
                _NextTrick = DateTime.UtcNow + TimeSpan.FromSeconds(10);

                e.Handled = true;
                Map myMap = Map;
                Point3D p = Location;

                Timer.DelayCall(TimeSpan.FromMilliseconds(350), () =>
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Timer.DelayCall(TimeSpan.FromMilliseconds(i * 250), idx =>
                        {
                            Misc.Geometry.Circle2D(p, myMap, idx, (pnt, map) =>
                            {
                                Effects.SendLocationEffect(pnt, map, Utility.RandomBool() ? 14000 : 14013, 14, 20, 2018, 0);
                            });

                            if (idx == 3)
                            {
                                var c = 0;

                                for (int j = idx; j > 0; j--)
                                {
                                    Timer.DelayCall(TimeSpan.FromMilliseconds(c * 250), idx2 =>
                                    {
                                        Misc.Geometry.Circle2D(p, myMap, idx2, (pnt, map) =>
                                        {
                                            Effects.SendLocationEffect(pnt, map, Utility.RandomBool() ? 14000 : 14013, 14, 20, 2018, 0);
                                        });
                                    }, j);

                                    c++;
                                }
                            }
                        }, i);
                    }
                });
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
