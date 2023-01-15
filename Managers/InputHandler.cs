using UnityEngine;

namespace FG
{
	public class InputHandler : MonoBehaviour
	{
		public FighterController unitController;

		public InputFrame inputFrame;

		private void Start()
		{
		}
		private void Update()
		{
			inputFrame.jump = false;
			inputFrame.left = false;
			inputFrame.right = false;
			inputFrame.up = false;
			inputFrame.down = false;
			inputFrame.attack1 = false;
			inputFrame.attack2 = false;
			inputFrame.attack3 = false;

			if (unitController.isDead)
			{
				return;
			}

			float h = 0f;
			float v = 0f;
			if (unitController.isPlayer1)
			{
				h = Input.GetAxisRaw("Horizontal");
				v = Input.GetAxisRaw("Vertical");
				inputFrame.attack1 = Input.GetButtonDown("Fire1"); //K
				inputFrame.attack2 = Input.GetButtonDown("Fire2"); //J
				inputFrame.attack3 = Input.GetButtonDown("Fire3"); //H 
				inputFrame.jump = Input.GetButtonDown("Jump"); // space
			}
			if (unitController.isPlayer2)
			{
				h = Input.GetAxisRaw("Horizontal1");
				v = Input.GetAxisRaw("Vertical1");
				inputFrame.attack1 = Input.GetButtonDown("Fire11"); //Q
				inputFrame.attack2 = Input.GetButtonDown("Fire21"); //E
				inputFrame.attack3 = Input.GetButtonDown("Fire31"); //F
				inputFrame.jump = Input.GetButtonDown("Jump1"); // V
			}

			if (h > 0.3f)
				inputFrame.right = true;
			if (h < -0.3f)
				inputFrame.left = true;
			if (v > 0.3f)
				inputFrame.up = true;
			if (v < -0.3f)
				inputFrame.down = true;


			Vector3 targetDirection = Vector3.zero;
			targetDirection.x = h;
			targetDirection.y = v;

			unitController.TickPlayer(Time.deltaTime, targetDirection);

			if (unitController.isInteracting)
			{
				if (unitController.canDoCombo)
				{
					if (inputFrame.attack1)
					{
						unitController.IsCombo();
					}
				}
				unitController.UseRootMotion(Time.deltaTime);
			}
			else
			{
				if (targetDirection.x != 0)
				{
					unitController.HandleRotation(targetDirection.x < 0);
				}

				unitController.TickPlayer(Time.deltaTime, targetDirection);

				unitController.DetectAction(inputFrame);
			}
		}

		[System.Serializable]
		public class InputFrame
		{
			public bool left;
			public bool right;
			public bool up;
			public bool down;
			public bool attack1;
			public bool attack2;
			public bool attack3;
			public bool jump;
		}
	}
}
