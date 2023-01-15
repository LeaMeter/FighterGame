using UnityEngine;

namespace FG
{
	public class FighterController : MonoBehaviour
	{
		public int health = 100;
		public HealthBar healthBar;

		public AnimatorHook animatorHook;
		public Transform holder;
		FighterMenu fighterMenu;

		public float horizontalSpeed = .6f;
		public float verticalSpeed = .35f;
		public bool isPlayer1;
		public bool isPlayer2;
		public bool isAI;
		public bool isLookingLeft;
		public bool hasBackHit;
		public bool isDead;
		public LayerMask walkLayer;

		public string actionName;
		public ActionDataHolder actionDataHolder;
		public Vector3 position
		{
			get { return transform.position; }
		}

		public bool canDoCombo
		{
			get { return animatorHook.canEnableCombo; }
		}

		ActionData[] currentActionData
		{
			get { return actionDataHolder.GetActions(); }
		}

		public bool isInteracting
		{
			get
			{
				return animatorHook.isInteracting;
			}
		}

		private void Start()
		{
			fighterMenu = FighterMenu.GetInstance();

			animatorHook = GetComponentInChildren<AnimatorHook>();

			if (isPlayer1)
			{
				animatorHook.SetAnimatorController(fighterMenu.getFighterWithID(fighterMenu.player1.characterId).animController);
			}
			if (isPlayer2)
			{
				animatorHook.SetAnimatorController(fighterMenu.getFighterWithID(fighterMenu.player2.characterId).animController);
			}

			if (healthBar != null)
			{
				healthBar.SetMaxHealth(health);
			}

			HandleRotation(isLookingLeft);
		}

		public void TickPlayer(float delta, Vector3 direction)
		{
			direction.x *= horizontalSpeed * delta;
			direction.y *= verticalSpeed * delta;
			bool isMoving = direction.sqrMagnitude > 0;

			animatorHook.Tick(isMoving);

			Vector3 targetPosition = transform.position + direction;
			MoveOnPosition(targetPosition);
		}

		public void UseRootMotion(float delta)
		{
			Vector3 targetPosotion = transform.position + animatorHook.deltaPosition * delta;
			MoveOnPosition(targetPosotion);
		}

		void MoveOnPosition(Vector3 targetPosition)
		{
			Collider2D[] colliders = Physics2D.OverlapPointAll(targetPosition, walkLayer);
			bool isValid = false;

			foreach (var item in colliders)
			{
				TWalklabe w = item.GetComponent<TWalklabe>();
				if (w != null)
				{
					if (w.isPlayer)
					{
						isValid = true;
					}
				}
			}
			if ((isValid || isAI) && !isInteracting)
			{
				transform.position = targetPosition;
			}
		}

		public void HandleRotation(bool looksLeft)
		{
			Vector3 eulers = Vector3.zero;
			isLookingLeft = false;
			if (looksLeft)
			{
				eulers.y = 180;
				isLookingLeft = true;
			}
			holder.localEulerAngles = eulers;
		}

		ActionData storedAction;

		public ActionData getLastAction { get { return storedAction; } }

		public void DetectAction(InputHandler.InputFrame input)
		{
			if (input.attack1 == false && input.attack2 == false && input.attack3 == false && input.jump == false)
				return;

			foreach (var a in currentActionData)
			{
				if (a.isDeterministic)
				{
					if (a.inputs.attack1 == input.attack1 &&
					a.inputs.attack2 == input.attack2 &&
					a.inputs.attack3 == input.attack3 &&
					a.inputs.left == input.left &&
					a.inputs.right == input.right &&
					a.inputs.up == input.up &&
					a.inputs.down == input.down &&
					a.inputs.jump == input.jump)
					{
						PlayAction(a);
						break;
					}
				}
				else
				{
					if (a.inputs.jump == input.jump)
					{
						PlayAction(a);
						break;
					}
				}
			}
		}

		public void PlayAction(ActionData actionData)
		{
			PlayAnimation(actionData.actionAnimation);
			storedAction = actionData;
		}

		public void PlayAnimation(string animName)
		{
			animatorHook.PlayAnimation(animName);
		}

		public void SetIsDead()
		{
			animatorHook.SetIsDead();
			isDead = true;
		}

		public bool SetAlive()
		{
			if (health == 100)
				return true;
			health = 100;
			animatorHook.SetAlive();
			animatorHook.SetIsInteracting(false);
			isDead = false;
			animatorHook.Tick(false);
			return false;
		}
		public void OnHit(ActionData actionData, bool hitterLookingLeft)
		{
			if (isDead)
				return;

			bool isFromBehind = false;

			if (isLookingLeft && hitterLookingLeft
				|| !hitterLookingLeft && !isLookingLeft)
			{
				isFromBehind = true;
			}

			DamageType damageType = actionData.damageType;
			health -= actionData.damageAmount;
			if (health <= 0)
			{
				damageType = DamageType.heavy;
				SetIsDead();
			}

			if (!hasBackHit)
			{
				if (isFromBehind)
				{
					HandleRotation(!hitterLookingLeft);
				}
				isFromBehind = false;
			}
			switch (damageType)
			{
				case DamageType.light:
					if (isFromBehind)
					{
						PlayAnimation("Hurt_Back");
					}
					else
					{
						PlayAnimation("Hurt_Front");
					}
					break;
				case DamageType.heavy:
					if (isFromBehind)
					{
						PlayAnimation("Knockdown_Back");
					}
					else
					{
						PlayAnimation("Knockdown_Front");
					}
					break;
				default:
					break;
			}
			healthBar.SetHealth(health);
		}

		public void IsCombo()
		{
			animatorHook.SetIsCombo();
		}

		public void LoadActionData(string name)
		{
			actionName = name;
		}

		public void ResetActionData()
		{
			actionName = "";
		}
	}
}