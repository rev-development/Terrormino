using Mapster;
using UnityEngine;

namespace Helpers
{
	public abstract class DtoPatternSO<TDto, TIDto> : ScriptableObject
		where TDto : TIDto
	{
		public virtual void AssignData(TDto dto)
		{
			dto.Adapt(this);
		}
	}
}