using Server.Items;

namespace Server.Services.ContainerSort.ItemIdentification.ItemIdentificationRules
{
	public class ArmorPositionIdentificationRule : IItemIdentificationRule
	{
		private readonly Layer Layer;

		public ArmorPositionIdentificationRule(Layer layer)
		{
			Layer = layer;
		}

		public bool DoesItemQualifyForSortFilter(Item item)
		{
			//shields share the layer with weapons so a type check is needed for this snowflake
			bool typeCheckPassed = true;
			if(Layer == Layer.TwoHanded)
			{
				typeCheckPassed = item is BaseShield;
			}

			return typeCheckPassed && item.Layer == Layer;
		}
	}
}