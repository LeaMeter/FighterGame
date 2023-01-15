using UnityEngine;

namespace FG
{
	public class DamageCollider : MonoBehaviour
	{
		FighterController owner;

		private void Start()
		{
			owner = GetComponentInParent<FighterController>();
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			FighterController u = other.GetComponentInParent<FighterController>();
			if (u != null)
			{
				if(u != owner)
				{
					u.OnHit(owner.getLastAction, owner.isLookingLeft);
				}
			}
		}
	}
}
