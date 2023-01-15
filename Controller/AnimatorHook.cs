using UnityEngine;

namespace FG
{
	public class AnimatorHook : MonoBehaviour
	{
		Animator anim;

		public Vector3 deltaPosition;

		FighterController owner;

		public bool canEnableCombo
		{
			get { return anim.GetBool("canEnableCombo"); }
		}

		public bool isInteracting
		{
			get { return anim.GetBool("isInteracting"); }
		}

		private void Start()
		{
			anim = GetComponent<Animator>();
			owner = GetComponent<FighterController>();
		}
		public void SetAnimatorController(RuntimeAnimatorController controller)
		{
			anim.GetComponent<Animator>().runtimeAnimatorController = controller as RuntimeAnimatorController;
		}
		public void Tick(bool isMoving)
		{
			float v = (isMoving) ? 1 : 0;
			anim.SetFloat("move", v);
		}

		public void PlayAnimation(string animName)
		{
			anim.Play(animName);
			anim.SetBool("isInteracting", true);
		}

		private void OnAnimatorMove()
		{
			deltaPosition = anim.deltaPosition / Time.deltaTime;
		}

		public void SetIsDead()
		{
			anim.SetBool("isDead", true);
		}

		public void SetAlive()
		{
			anim.SetBool("isDead", false);
		}

		public void SetIsCombo()
		{
			anim.SetBool("isCombo", true);
		}

		public void SetIsInteracting(bool status)
		{
			anim.SetBool("isInteracting", status);
		}

		public void LoadActionData(string actionName)
		{
			owner.LoadActionData(actionName);
		}
	}
}
