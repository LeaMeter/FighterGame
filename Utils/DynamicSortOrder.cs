using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FG
{
	public class DynamicSortOrder : MonoBehaviour
	{
		public SpriteRenderer spriteRenderer;
		public Transform origin;

		private void Update()
		{
			Vector3 position = transform.position;
			if (origin != null)
			{
				position = origin.position;
			}

			int sortOrder = Mathf.RoundToInt(position.y * 100) + 10;
			spriteRenderer.sortingOrder = -sortOrder;
		}
	}
}
