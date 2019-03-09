using System;
using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Items;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class KotlCityPuzzle : BaseAddon
    {
        public static KotlCityPuzzle Puzzle { get; set; }
        public override BaseAddonDeed Deed { get { return null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Next { get { return _Order == null || _Order.Count == 0 ? -1 : _Order[0]; } }

        private List<int> _Order;
        private int _Index;
        private bool _Complete;
        private int _Fails;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Complete
        { 
            get { return _Complete; } 
            set
            {
                foreach (var comp in Components.OfType<KotlCityPuzzleComponent>().Where(c => (value && c.Active) || (!value && !c.Active)))
                    comp.Active = !value;

                if (_Complete && !value)
                {
                    RandomizeOrder();
                }

                _Complete = value;
            }
        }

        public KotlCityPuzzle()
        {
            AddComponent(new KotlCityPuzzleComponent(), 0, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 1, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 2, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 3, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 4, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 5, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 6, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 7, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 8, 0, 0);
            AddComponent(new KotlCityPuzzleComponent(), 9, 0, 0);

            Puzzle = this;

            Reset();
        }

        private void RandomizeOrder()
        {
            _Order = new List<int>();
            var list = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int count = Utility.RandomMinMax(5, 10);

            int ran = 0;

            do
            {
                ran = list[Utility.Random(list.Count)];

                _Order.Add(ran);
                list.Remove(ran);
            }
            while (_Order.Count < count);

            ColUtility.Free(list);
        }

        private void Reset()
        {
            Complete = false;

            _Index = 0;
            _Fails = 0;
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            if (_Complete || !from.InRange(component, 2))
                return;

            if (_Order == null)
            {
                RandomizeOrder();
            }

            KotlCityPuzzleComponent comp = component as KotlCityPuzzleComponent;

            if (comp != null && comp.Active)
            {
                if (comp.Offset.X == _Order[_Index])
                {
                    comp.Active = false;
                   
                    _Fails = 0;
                    from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1157028, from.NetState); // *You activate the switch!*

                    if (_Order.Count - 1 == _Index)
                    {
                        Complete = true;

                        if (KotlDoor.Instance != null)
                        {
                            KotlDoor.Instance.Locked = false;
                            from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1157019, from.NetState); // *You hear a low hum as the door to the Time Room unseals...*
                            from.PlaySound(0x667);

                            Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
                                {
                                    KotlDoor.Instance.Locked = true;
                                    KotlDoor.Instance.KeyValue = Key.RandomValue();

                                    Reset();
                                });
                        }
                    }
                    else
                    {
                        _Index++;
                    }
                }
                else
                {
                    _Fails++;
                    AOS.Damage(from, Utility.RandomMinMax(100, 150), false, 0, 0, 0, 0, 100);

                    from.FixedParticles(0x3818, 1, 11, 0x13A8, 0, 0, EffectLayer.Waist);
                    from.PlaySound(0x665);
                    from.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1157029, from.NetState); // *The switch shorts out and electrocutes you! You are vulnerable to more energy damage in your shocked state!*

                    if (_Fails > 5 && _Fails > Utility.Random(15))
                    {
                        component.PrivateOverheadMessage(Server.Network.MessageType.Regular, 1154, 1157031, from.NetState); // *Circuit Fault! Generating new circuit sequence!*
                        Reset();
                    }
                }
            }
        }

        public KotlCityPuzzle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(Complete);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Puzzle = this;

            Complete = reader.ReadBool();

            if (!_Complete)
                Reset();
            else
            {
                Timer.DelayCall(TimeSpan.FromMinutes(5), () =>
                    {
                        if (KotlDoor.Instance != null)
                        {
                            KotlDoor.Instance.Locked = true;
                            KotlDoor.Instance.KeyValue = Key.RandomValue();
                        }

                        Reset();
                    });
            }
        }
    }

    public class KotlCityPuzzleComponent : AddonComponent
    {
        public override int LabelNumber { get { return 1124182; } }

        public bool _Active;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        { 
            get { return _Active; } 
            set 
            { 
                _Active = value;

                if (_Active && ItemID != 0x9CDE)
                {
                    Effects.PlaySound(this.Location, this.Map, 0x051);
                    ItemID = 0x9CDE;
                }
                else if (!_Active && ItemID != 0x9D0B)
                {
                    Effects.PlaySound(this.Location, this.Map, 0x051);
                    ItemID = 0x9D0B;
                }
            }
        }

        public KotlCityPuzzleComponent()
            : base(0x9CDE)
        {
            _Active = true;
        }

        public KotlCityPuzzleComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(_Active);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Active = reader.ReadBool();
        }
    }
}
