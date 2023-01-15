using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FG
{
	public class SelectScreenManager : MonoBehaviour
	{
		public int numberOfPlayers = 1;
		public PlayerSelectInfo player1;
		public PlayerSelectInfo player2;
		public AvatarInfo[] portraitPrefabs;
		public int maxX;
		public int maxY;
		AvatarInfo[,] charGrid;
		public GameObject portraitCanvas;
		public bool bothPlayersSelected;

		bool isInCorutine;
		bool loadLevel;
		FighterMenu fighterMenu;

		public static SelectScreenManager instance;

		public static SelectScreenManager GetInstance()
		{
			return instance;
		}

		void Awake()
		{
			instance = this;
		}

		void Start()
		{
			fighterMenu = FighterMenu.GetInstance();
			numberOfPlayers = fighterMenu.numberOfUsers;

			if (numberOfPlayers == 1)
			{
				fighterMenu.player2.hasCharacter = true;
			}

			charGrid = new AvatarInfo[maxX, maxY];

			portraitPrefabs = portraitCanvas.GetComponentsInChildren<AvatarInfo>();

			int x = 0;
			int y = 0;
			for (int i = 0; i < fighterMenu.characterList.Count; i++)
			{
				portraitPrefabs[i].posX += x;
				portraitPrefabs[i].posY += y;

				charGrid[x, y] = portraitPrefabs[i];

				if (x < maxX - 1)
				{
					x++;
				}
				else
				{
					x = 0;
					y++;
				}
			}
		}

		void Update()
		{
			if (!loadLevel)
			{
				HandleUserChoice(fighterMenu.player1, player1);

				if (numberOfPlayers == 2)
				{
					HandleUserChoice(fighterMenu.player2, player2);
				}
			}
			if (bothPlayersSelected && !isInCorutine)
			{
				StartCoroutine(LoadLevel());
				loadLevel = true;
			}

			bothPlayersSelected = (fighterMenu.player1.hasCharacter && fighterMenu.player2.hasCharacter) ? true : false;
		}

		void HandleUserChoice(FighterStats player, PlayerSelectInfo p)
		{
			if (Input.GetButtonUp("Fire2" + player.inputId))
			{
				p.playerBase.hasCharacter = false;
			}
			if (!player.hasCharacter)
			{
				p.playerBase = player;
				SetSelectorPosition(p);
				HandleSelectScreenInput(p, player.inputId);
				HandleFighterPreview(p);
			}
		}

		void HandleSelectScreenInput(PlayerSelectInfo pl, string playerId)
		{
			#region Grid

			float toUp = Input.GetAxis("Vertical" + playerId);

			if (toUp != 0)
			{
				if (!pl.hitInputOnce)
				{
					if (toUp < 0)
					{
						pl.activeY = (pl.activeY > 0) ? pl.activeY - 1 : maxY - 1;
					}
					else
					{
						pl.activeY = (pl.activeY < maxY - 1) ? pl.activeY + 1 : 0;
					}
					pl.hitInputOnce = true;
				}
			}

			float toTheLeft = Input.GetAxis("Horizontal" + playerId);

			if (toTheLeft != 0)
			{
				if (!pl.hitInputOnce)
				{
					if (toTheLeft < 0)
					{
						pl.activeX = (pl.activeX > 0) ? pl.activeX - 1 : maxX - 1;
					}
					else
					{
						pl.activeX = (pl.activeX < maxX - 1) ? pl.activeX + 1 : 0;
					}
					pl.timetToReset = 0;
					pl.hitInputOnce = true;
				}
			}

			if (toUp == 0 && toTheLeft == 0)
			{
				pl.hitInputOnce = false;
			}

			if (pl.hitInputOnce)
			{
				pl.timetToReset += Time.deltaTime;

				if (pl.timetToReset > 0.8f)
				{
					pl.hitInputOnce = false;
					pl.timetToReset = 0;
				}
			}

			#endregion

			if (Input.GetButtonUp("Fire1" + playerId))
			{
				pl.createdFighter.GetComponent<FighterController>().PlayAnimation("Attack 1");
				pl.playerBase.hasCharacter = true;
				pl.playerBase.characterId = pl.activatePortrait.characterId;
			}
		}

		IEnumerator LoadLevel()
		{
			if (fighterMenu.player2.playerType == FighterStats.PlayerType.ai)
			{
				int numOfFighters = fighterMenu.characterList.Count;
				int ranValue = UnityEngine.Random.Range(0, numOfFighters);
				FighterData character = fighterMenu.getFighterWithID(fighterMenu.characterList[ranValue].charId);
				fighterMenu.player2.characterId = character.charId;
			}

			isInCorutine = true;
			yield return new WaitForSeconds(2);
			SceneManager.LoadScene("level", LoadSceneMode.Single);
		}

		void SetSelectorPosition(PlayerSelectInfo pl)
		{
			pl.selector.SetActive(true);

			pl.activatePortrait = charGrid[pl.activeX, pl.activeY];
			Vector2 selector = pl.activatePortrait.transform.localPosition;

			selector = selector +
				new Vector2(portraitCanvas.transform.localPosition.x, portraitCanvas.transform.localPosition.y);

			pl.selector.transform.localPosition = selector;
		}

		void HandleFighterPreview(PlayerSelectInfo pl)
		{
			if (pl.previewPortrait != pl.activatePortrait)
			{
				if (pl.createdFighter != null)
				{
					Destroy(pl.createdFighter);
				}

				GameObject temp = Instantiate(FighterMenu.GetInstance().getFighterWithID(
					pl.activatePortrait.characterId).prefab, pl.charVisPos.position,
					Quaternion.identity);

				pl.createdFighter = temp;

				pl.previewPortrait = pl.activatePortrait;

				FighterController prewCharacter = pl.createdFighter.GetComponent<FighterController>();
				prewCharacter.GetComponent<InputHandler>().enabled = false;

				if (!string.Equals(pl.playerBase.playerId, fighterMenu.player1.playerId))
				{
					prewCharacter.isLookingLeft = true;
				}
				else
				{
					prewCharacter.isLookingLeft = false;
				}
			}
		}

		[System.Serializable]
		public class PlayerSelectInfo
		{
			public AvatarInfo activatePortrait;
			public AvatarInfo previewPortrait;
			public GameObject selector;
			public Transform charVisPos;
			public GameObject createdFighter;

			public int activeX;
			public int activeY;

			public bool hitInputOnce;
			public float timetToReset;

			public FighterStats playerBase;

		}
	}
}