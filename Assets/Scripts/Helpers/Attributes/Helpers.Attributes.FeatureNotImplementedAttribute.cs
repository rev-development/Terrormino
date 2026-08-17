using System;
using JetBrains.Annotations;

namespace Helpers.Attributes
{
	[AttributeUsage(AttributeTargets.All)]
	[MeansImplicitUse(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.WithMembers)]
	public class FeatureNotImplementedAttribute : Attribute
	{
		public FeatureNotImplementedAttribute(string note = null) => Note = note;

		public string Note { get; }
	}
}