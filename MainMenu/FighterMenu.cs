using System.Collections.Generic;
using UnityEngine;

namespace FG
{
	public class FighterMenu : MonoBehaviour
	{
		public int numberOfUsers;
		public FighterStats player1;
		public FighterStats player2;

		public List<FighterData> characterList = new List<FighterData>();

		public FighterData getFighterWithID(string id)
		{
			FighterData retVal = null;

			for (int i = 0; i < characterList.Count; i++)
			{
				if (string.Equals(characterList[i].charId, id))
				{
					retVal = characterList[i];
					break;
				}
			}
			return retVal;
		}

		public static FighterMenu instance;
		public static FighterMenu GetInstance()
		{
			return instance;
		}
		void Awake()
		{
			if(instance == null)
			{
				instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
		}
	}

	[System.Serializable]
	public class FighterData
	{
		public string charId;
		public GameObject prefab;
		public RuntimeAnimatorController animController;
	}

	[System.Serializable]
	public class FighterStats
	{
		public string playerId;
		public string inputId;
		public PlayerType playerType;
		public bool hasCharacter;

		public string characterId;
		public int score;

		public enum PlayerType
		{
			user,
			ai
		}
	}
}
