namespace FG
{
	[System.Serializable]
	public class ActionData
	{
		public string actionAnimation;
		public DamageType damageType;
		public int damageAmount = 5;
		public bool isDeterministic;
		public InputHandler.InputFrame inputs;
	}

	public enum DamageType
	{
		light, heavy
	}
}
