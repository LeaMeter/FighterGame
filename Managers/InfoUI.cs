using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FG
{
	public class InfoUI : MonoBehaviour
	{
		public Text Announcer1;
		public Text Announcer2;
		public Text Timer;

		public GameObject[] winIndicatorGrids;
		public GameObject winIndicator;

		public static InfoUI instance;
		public static InfoUI GetInstance()
		{
			return instance;
		}

		void Awake()
		{
			instance = this;
		}

		public void AddWinIndicator(int player)
		{
			GameObject go = Instantiate(winIndicator, transform.position, Quaternion.identity) as GameObject;
			go.transform.SetParent(winIndicatorGrids[player].transform);
		}
	}
}

