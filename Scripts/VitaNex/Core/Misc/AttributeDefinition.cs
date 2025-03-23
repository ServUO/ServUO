#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
#endregion

namespace Server
{
	public class AttributeDefinition : AttributeFactors
	{
		[CommandProperty(AccessLevel.Administrator)]
		public TextDefinition Name { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public string NameString { get => Name.String; set => Name = new TextDefinition(Name.Number, value); }

		[CommandProperty(AccessLevel.Administrator)]
		public int NameNumber { get => Name.Number; set => Name = new TextDefinition(value, Name.String); }

		[CommandProperty(AccessLevel.Administrator)]
		public bool Percentage { get; set; }

		public AttributeDefinition(TextDefinition name = default, double weight = 1.0, int min = 0, int max = 1, int inc = 1, bool percentage = false)
			: base(weight, min, max, inc)
		{
			Name = name;
			Percentage = percentage;
		}

		public AttributeDefinition(AttributeDefinition def)
			: this(new TextDefinition(def.Name.Number, def.Name.String), def.Weight, def.Min, def.Max, def.Inc)
		{ }

		public AttributeDefinition(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.WriteTextDef(Name);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Name = reader.ReadTextDef();
		}
	}
}
