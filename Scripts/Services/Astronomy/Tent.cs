namespace Server.Items
{
    public class AstronomyTent : BaseAddon
    {
        public override BaseAddonDeed Deed => null;

        [Constructable]
        public AstronomyTent()
        {
            // yellow walls
            int hue = 2721;

            AddonComponent comp = new AddonComponent(0x369)
            {
                Hue = hue
            };
            AddComponent(comp, -1, 3, 0);

            comp = new AddonComponent(0x369)
            {
                Hue = hue
            };
            AddComponent(comp, 2, 3, 0);

            comp = new AddonComponent(0x36A)
            {
                Hue = hue
            };
            AddComponent(comp, 5, 1, 0);

            comp = new AddonComponent(0x36A)
            {
                Hue = hue
            };
            AddComponent(comp, 5, -1, 0);

            comp = new AddonComponent(0x36F)
            {
                Hue = hue
            };
            AddComponent(comp, -4, -1, 0);

            comp = new AddonComponent(0x36F)
            {
                Hue = hue
            };
            AddComponent(comp, -4, 1, 0);

            comp = new AddonComponent(0x36E)
            {
                Hue = hue
            };
            AddComponent(comp, -2, -3, 0);

            comp = new AddonComponent(0x36E)
            {
                Hue = hue
            };
            AddComponent(comp, 2, -3, 0);

            // blue walls
            hue = 1156;

            comp = new AddonComponent(0x36C)
            {
                Hue = hue
            };
            AddComponent(comp, -4, 3, 0);

            comp = new AddonComponent(0x369)
            {
                Hue = hue
            };
            AddComponent(comp, -3, 3, 0);

            comp = new AddonComponent(0x369)
            {
                Hue = hue
            };
            AddComponent(comp, -2, 3, 0);

            comp = new AddonComponent(0x369)
            {
                Hue = hue
            };
            AddComponent(comp, 3, 3, 0);

            comp = new AddonComponent(0x369)
            {
                Hue = hue
            };
            AddComponent(comp, 4, 3, 0);

            comp = new AddonComponent(0x368)
            {
                Hue = hue
            };
            AddComponent(comp, 5, 3, 0);

            comp = new AddonComponent(0x36A)
            {
                Hue = hue
            };
            AddComponent(comp, 5, 2, 0);

            comp = new AddonComponent(0x36A)
            {
                Hue = hue
            };
            AddComponent(comp, 5, 0, 0);

            comp = new AddonComponent(0x36A)
            {
                Hue = hue
            };
            AddComponent(comp, 5, -2, 0);

            comp = new AddonComponent(0x36D)
            {
                Hue = hue
            };
            AddComponent(comp, 5, -3, 0);

            comp = new AddonComponent(0x36E)
            {
                Hue = hue
            };
            AddComponent(comp, 4, -3, 0);

            comp = new AddonComponent(0x36E)
            {
                Hue = hue
            };
            AddComponent(comp, 3, -3, 0);

            comp = new AddonComponent(0x36E)
            {
                Hue = hue
            };
            AddComponent(comp, -3, -3, 0);

            comp = new AddonComponent(0x36F)
            {
                Hue = hue
            };
            AddComponent(comp, -4, -2, 0);

            comp = new AddonComponent(0x36F)
            {
                Hue = hue
            };
            AddComponent(comp, -4, 0, 0);

            comp = new AddonComponent(0x36F)
            {
                Hue = hue
            };
            AddComponent(comp, -4, 2, 0);

            AddComponent(new AddonComponent(0x36B), -4, -3, 0);

            // South/East Corner
            comp = new AddonComponent(0x663)
            {
                Hue = hue
            };
            AddComponent(comp, 5, 3, 18);

            comp = new AddonComponent(0x663)
            {
                Hue = hue
            };
            AddComponent(comp, 4, 2, 21);

            comp = new AddonComponent(0x663)
            {
                Hue = hue
            };
            AddComponent(comp, 3, 1, 24);

            // North/East Corner
            comp = new AddonComponent(0x664)
            {
                Hue = hue
            };
            AddComponent(comp, 5, -3, 18);

            comp = new AddonComponent(0x664)
            {
                Hue = hue
            };
            AddComponent(comp, 4, -2, 21);

            comp = new AddonComponent(0x664)
            {
                Hue = hue
            };
            AddComponent(comp, 3, -1, 24);

            // South/West Corner
            comp = new AddonComponent(0x666)
            {
                Hue = hue
            };
            AddComponent(comp, -4, 3, 18);

            comp = new AddonComponent(0x666)
            {
                Hue = hue
            };
            AddComponent(comp, -3, 2, 21);

            comp = new AddonComponent(0x666)
            {
                Hue = hue
            };
            AddComponent(comp, -2, 1, 24);

            // North/West Corner
            comp = new AddonComponent(0x665)
            {
                Hue = hue
            };
            AddComponent(comp, -4, -3, 18);

            comp = new AddonComponent(0x665)
            {
                Hue = hue
            };
            AddComponent(comp, -3, -2, 21);

            comp = new AddonComponent(0x665)
            {
                Hue = hue
            };
            AddComponent(comp, -2, -1, 24);

            // Top - East to West
            comp = new AddonComponent(0x668)
            {
                Hue = hue
            };
            AddComponent(comp, 2, 0, 27);

            comp = new AddonComponent(0x662)
            {
                Hue = hue
            };
            AddComponent(comp, 1, 0, 27);

            comp = new AddonComponent(0x662)
            {
                Hue = hue
            };
            AddComponent(comp, 0, 0, 27);

            comp = new AddonComponent(0x669)
            {
                Hue = hue
            };
            AddComponent(comp, -1, 0, 27);

            // South Facing
            for (int x = -3; x <= 4; x++)
            {
                comp = new AddonComponent(0x601)
                {
                    Hue = hue
                };
                AddComponent(comp, x, 3, 18);
            }

            for (int x = -2; x <= 3; x++)
            {
                comp = new AddonComponent(0x601)
                {
                    Hue = hue
                };
                AddComponent(comp, x, 2, 21);
            }

            for (int x = -1; x <= 2; x++)
            {
                comp = new AddonComponent(0x601)
                {
                    Hue = hue
                };
                AddComponent(comp, x, 1, 24);
            }

            // North Facing
            for (int x = -3; x <= 4; x++)
            {
                comp = new AddonComponent(0x600)
                {
                    Hue = hue
                };
                AddComponent(comp, x, -3, 18);
            }

            for (int x = -2; x <= 3; x++)
            {
                comp = new AddonComponent(0x600)
                {
                    Hue = hue
                };
                AddComponent(comp, x, -2, 21);
            }

            for (int x = -1; x <= 2; x++)
            {
                comp = new AddonComponent(0x600)
                {
                    Hue = hue
                };
                AddComponent(comp, x, -1, 24);
            }

            // West Facing
            for (int y = -2; y <= 2; y++)
            {
                comp = new AddonComponent(0x5FF)
                {
                    Hue = hue
                };
                AddComponent(comp, -4, y, 18);
            }

            for (int y = -1; y <= 1; y++)
            {
                comp = new AddonComponent(0x5FF)
                {
                    Hue = hue
                };
                AddComponent(comp, -3, y, 21);
            }

            comp = new AddonComponent(0x5FF)
            {
                Hue = hue
            };
            AddComponent(comp, -2, 0, 24);

            // East Facing
            for (int y = -2; y <= 2; y++)
            {
                comp = new AddonComponent(0x602)
                {
                    Hue = hue
                };
                AddComponent(comp, 5, y, 18);
            }

            for (int y = -1; y <= 1; y++)
            {
                comp = new AddonComponent(0x602)
                {
                    Hue = hue
                };
                AddComponent(comp, 4, y, 21);
            }

            comp = new AddonComponent(0x602)
            {
                Hue = hue
            };
            AddComponent(comp, 3, 0, 24);

            AddComponent(new AddonComponent(0xB6B), 2, 0, 0);
            AddComponent(new AddonComponent(0xB6C), 2, -1, 0);
            AddComponent(new AddonComponent(0x9CEF), -1, 5, 0);
            AddComponent(new AddonComponent(0x9CEF), 3, 5, 0);
            // Center:4707, 1127, 0
        }

        public AstronomyTent(Serial serial)
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