using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FG
{
	public class FirstSceneManager : MonoBehaviour
	{
		public GameObject pressStartText;
		float timer;
		bool loadingLevel;
		bool isFirstScene;

		public bool activeOption;
		public GameObject menuObj;
		public ButtonRef[] menuOptions;
		public ButtonRef option1;
		public ButtonRef option2;

		void Start()
		{
			menuObj.SetActive(false);
			isFirstScene = true;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitGame();
			}
			if (isFirstScene)
			{
				timer += Time.deltaTime;
				if (timer > 0.5f)
				{
					timer = 0;
					pressStartText.SetActive(!pressStartText.activeInHierarchy);
				}

				if (Input.GetButtonUp("Jump"))
				{
					pressStartText.SetActive(false);
					menuObj.SetActive(true);
					isFirstScene = false;
				}
			}
			else
			{
				if (!loadingLevel)
				{
					if (!activeOption)
					{
						option1.selected = true;
						option2.selected = false;
					}

					if (activeOption)
					{
						option2.selected = true;
						option1.selected = false;
					}

					if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow))
					{
						activeOption = !activeOption;
					}

					if (Input.GetButtonUp("Jump"))
					{
						loadingLevel = true;
						StartCoroutine(LoadLevel());

						if (!activeOption)
						{
							option1.transform.localScale *= 1.3f;
						}

						if (activeOption)
						{
							option2.transform.localScale *= 1.3f;
						}

					}
				}
			}
		}
		void HandleSelectedOption()
		{
			if (!activeOption)
			{
				FighterMenu.GetInstance().numberOfUsers = 1;
				FighterMenu.GetInstance().player2.playerType = FighterStats.PlayerType.ai;
			}
			else
			{
				FighterMenu.GetInstance().numberOfUsers = 2;
				FighterMenu.GetInstance().player2.playerType = FighterStats.PlayerType.user;
			}
		}
		public void QuitGame()
		{
			UnityEditor.EditorApplication.isPlaying = false;
			//Application.Quit();
		}

		IEnumerator LoadLevel()
		{
			HandleSelectedOption();
			yield return new WaitForSeconds(1);
			SceneManager.LoadSceneAsync("select", LoadSceneMode.Single);
		}
	}
}
