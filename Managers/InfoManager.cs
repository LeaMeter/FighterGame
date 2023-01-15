using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FG
{
	public class InfoManager : MonoBehaviour
	{
		public FighterController player1;
		public FighterController player2;

		public InputHandler player1Input;
		public InputHandler player2Input;

		public AIHandler aiHandler;

		FighterMenu fighterMenu;
		InfoUI levelUI;

		public int maxTurns = 2;
		int currnetTurn = 1;

		public bool countdown;
		public int maxTurnTimer = 30;
		int currnetTimer;
		float internalTimer;
		Vector3 player1StartPos;
		Vector3 player2StartPos;


		void Start()
		{
			fighterMenu = FighterMenu.GetInstance();
			levelUI = InfoUI.GetInstance();
			player1StartPos = player1.transform.position;
			player2StartPos = player2.transform.position;

			levelUI.Announcer1.gameObject.SetActive(false);
			levelUI.Announcer2.gameObject.SetActive(false);

			StartCoroutine(StartGame());
		}

		void Update()
		{
			if (countdown)
			{
				HandleTurnTimer();
			}
		}

		void HandleTurnTimer()
		{
			levelUI.Timer.text = currnetTimer.ToString();

			internalTimer += Time.deltaTime;

			if (internalTimer > 1)
			{
				currnetTimer--;
				internalTimer = 0;
			}

			if (currnetTimer > 0 && (player1.health <= 0 || player2.health <= 0))
			{
				EndTurnFunction(false);
			}

			if (currnetTimer <= 0)
			{
				EndTurnFunction(true);
			}
		}

		IEnumerator InitTurn()
		{
			levelUI.Announcer1.gameObject.SetActive(false);
			levelUI.Announcer2.gameObject.SetActive(false);

			currnetTimer = maxTurnTimer;
			countdown = false;

			yield return EnableControl();
		}

		IEnumerator StartGame()
		{
			yield return InitTurn();
		}

		IEnumerator EnableControl()
		{
			levelUI.Announcer1.gameObject.SetActive(true);
			levelUI.Announcer1.text = "Round " + currnetTurn;
			levelUI.Announcer1.color = Color.white;
			yield return new WaitForSeconds(2);

			levelUI.Announcer1.text = "3";
			levelUI.Announcer1.color = Color.green;
			yield return new WaitForSeconds(1);
			levelUI.Announcer1.text = "2";
			levelUI.Announcer1.color = Color.yellow;
			yield return new WaitForSeconds(1);
			levelUI.Announcer1.text = "1";
			levelUI.Announcer1.color = Color.red;
			yield return new WaitForSeconds(1);
			levelUI.Announcer1.color = Color.red;
			levelUI.Announcer1.text = "FIGHT!";

			if (fighterMenu.numberOfUsers == 1)
			{
				player1Input.enabled = true;
				player2Input.enabled = false;
				player2.isAI = true;
				aiHandler.enabled = true;
			}
			else
			{
				player1Input.enabled = true;
				player2Input.enabled = true;
				player2.isAI = false;
				aiHandler.enabled = false;
			}

			yield return new WaitForSeconds(1);
			levelUI.Announcer1.gameObject.SetActive(false);
			countdown = true;
		}

		void DisableControl()
		{
			player1Input.enabled = false;
			player2Input.enabled = false;
			aiHandler.enabled = false;
			player1.animatorHook.Tick(false);
			player2.animatorHook.Tick(false);
		}

		public void EndTurnFunction(bool timeOut = false)
		{
			countdown = false;

			levelUI.Timer.text = maxTurnTimer.ToString();

			if (timeOut)
			{
				levelUI.Announcer1.gameObject.SetActive(true);
				levelUI.Announcer1.text = "Time Out!";
				levelUI.Announcer1.color = Color.cyan;
			}
			else
			{
				levelUI.Announcer1.gameObject.SetActive(true);
				levelUI.Announcer1.text = "K.O.";
				levelUI.Announcer1.color = Color.red;
			}

			DisableControl();

			StartCoroutine(EndTurn());
		}

		IEnumerator EndTurn()
		{
			yield return new WaitForSeconds(3);

			FighterController wPlayer;
			wPlayer = FindWinningPlayer();
			bool matchOver = isMatchOver();
			bool isFullHealth = false;

			if (!matchOver)
			{
				isFullHealth = player1.SetAlive();
				isFullHealth = player2.SetAlive();

				player1.transform.position = player1StartPos;
				player1.HandleRotation(false);

				player2.transform.position = player2StartPos;
				player2.HandleRotation(true);
			}

			if (wPlayer == null)
			{
				levelUI.Announcer1.text = "Draw";
				levelUI.Announcer1.color = Color.green;
			}
			else
			{
				string playerName = wPlayer.isPlayer1 ? fighterMenu.player1.characterId : fighterMenu.player2.characterId;
				levelUI.Announcer1.text = playerName + " Wins!";
				levelUI.Announcer1.color = Color.red;
				levelUI.Announcer1.transform.localScale *= 1.3f;
			}

			yield return new WaitForSeconds(3);

			if (wPlayer != null)
			{
				if (isFullHealth)
				{
					levelUI.Announcer2.gameObject.SetActive(true);
					levelUI.Announcer2.text = "Flawless Victory!";
				}
			}
			player1.healthBar.SetHealth(player1.health);
			player2.healthBar.SetHealth(player2.health);

			yield return new WaitForSeconds(3);

			currnetTurn++;

			if (!matchOver)
			{
				StartCoroutine(InitTurn());
			}
			else
			{

				fighterMenu.player1.score = 0;
				fighterMenu.player1.hasCharacter = false;
				fighterMenu.player2.score = 0;
				fighterMenu.player2.hasCharacter = false;
				
				SceneManager.LoadSceneAsync("intro");
			}
		}

		bool isMatchOver()
		{
			bool retVal = false;

			if (fighterMenu.player1.score >= maxTurns || fighterMenu.player2.score >= maxTurns)
			{
				retVal = true;
			}

			return retVal;
		}

		FighterController FindWinningPlayer()
		{
			FighterController winner = null;

			if (player1.health != player2.health)
			{
				if (player1.health > player2.health)
				{
					fighterMenu.player1.score++;
					winner = player1;
					levelUI.AddWinIndicator(0);
				}
				else
				{
					fighterMenu.player2.score++;
					winner = player2;
					levelUI.AddWinIndicator(1);
				}
			}
			return winner;
		}

		public static InfoManager instance;

		public static InfoManager GetInstance()
		{
			return instance;
		}

		void Awake()
		{
			instance = this;
		}

	}
}

