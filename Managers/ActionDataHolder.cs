using UnityEngine;

namespace FG
{
	[CreateAssetMenu]
	public class ActionDataHolder : ScriptableObject
	{
		public ActionData[] actions;

		public ActionData GetAction(string animation)
		{
			for (int i = 0; i < actions.Length; i++)
			{
				if (actions[i].actionAnimation == animation)
				{
					return actions[i];
				}
			}
			return null;
		}
		public ActionData[] GetActions()
		{
			return actions;
		}
	}
}