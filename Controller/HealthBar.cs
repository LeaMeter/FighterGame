using UnityEngine;
using UnityEngine.UI;

namespace FG
{
	public class HealthBar : MonoBehaviour
	{
		public Slider slider;
		public Gradient gradient;
		public Image fill;
		public int health;

		public void SetHealth(int health)
		{
			this.health = health;
			slider.value = health;

			fill.color = gradient.Evaluate(slider.normalizedValue);
		}

		public void SetMaxHealth(int health)
		{
			this.health = health;
			slider.maxValue = health;
			slider.value = health;

			fill.color = gradient.Evaluate(1f);
		}
	}
}
