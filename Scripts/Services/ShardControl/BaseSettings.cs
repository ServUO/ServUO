using Server;

namespace CustomsFramework.Systems.ShardControl
{
	[PropertyObject]
    public abstract class BaseSettings
    {
		public BaseSettings()
		{ }

		public BaseSettings(GenericReader reader)
		{
			Deserialize(reader);
		}

        public abstract void Serialize(GenericWriter writer);
        public abstract void Deserialize(GenericReader reader);

        public abstract override string ToString();
    }
}