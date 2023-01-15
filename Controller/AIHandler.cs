using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

namespace FG
{
	public class AIHandler : MonoBehaviour
	{
		public FighterController fighterController;
		public FighterController enemy;

		public float attackRate = 1f;
		float timer = 0;
		public float attackDistance = 0.4f;
		public Vector3 targetOffset = new Vector3(0.35f, 0, 0);

		public bool isInteracting
		{
			get { return fighterController.isInteracting; }
		}

		private void Start()
		{

		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;

			if (isInteracting || fighterController.isDead || enemy.isDead)
			{
				fighterController.UseRootMotion(deltaTime);
				return;
			}

			if (CanHitEnemy(deltaTime))
			{
				PlayRandomActionFromHolder();
			}
			else
			{
				bool hasPosition = GetPositionCloseToPlayer(targetOffset);

				if (!hasPosition)
				{
					GetRandomPosition();
				}

				MoveToPosition(deltaTime);
				HandleAimingToEnemy(10);
			}

			return;
		}

		public void PlayRandomActionFromHolder()
		{
			int numOfAttacks = 0;
			for (int i = 0; i < fighterController.actionDataHolder.actions.Length; i++)
			{
				if (fighterController.actionDataHolder.actions[i].isDeterministic == true)
					numOfAttacks++;
			}
			int ranValue = UnityEngine.Random.Range(0, numOfAttacks);

			fighterController.PlayAction(fighterController.actionDataHolder.actions[ranValue]);
		}

		public float GetDistance(Vector3 p1, Vector3 p2)
		{
			p1.z = 0;
			p1.z = 0;

			return Vector3.Distance(p1, p2);
		}

		public float GetDistanceFromEnemy()
		{
			return GetDistance(transform.position, enemy.position);
		}

		public bool CanHitEnemy(float delta)
		{
			timer += delta;
			if (timer > attackRate)
			{
				timer = 0;
			}
			else
			{
				return false;
			}

			Vector3 temp1 = Vector3.zero;
			temp1.x = transform.position.x;

			Vector3 temp2 = Vector3.zero;
			temp2.x = enemy.position.x;

			Vector3 temp3 = Vector3.zero;
			temp3.y = transform.position.y;

			Vector3 temp4 = Vector3.zero;
			temp4.y = enemy.position.y;

			if (Vector3.Distance(temp1, temp2) < attackDistance)
			{
				if (Vector3.Distance(temp3, temp4) < 0.05)
					return true;
			}
			return false;
		}

		public void HandleAimingToEnemy(float rotateDistance)
		{
			float dis = GetDistanceFromEnemy();
			if (dis < rotateDistance)
			{
				Vector3 direction = enemy.position - transform.position;

				fighterController.HandleRotation(direction.x < 0);
			}
		}

		int randomPositonSteps;

		public void GetRandomPosition()
		{
			Vector3 randomPosition = Random.insideUnitCircle;
			Vector3 tp = enemy.position + randomPosition;

			bool result = isValidPosition(ref tp);

			if (result)
			{
				hasMovedPosition = true;
				currentPath = GridManager.singleton.GetPath(transform.position, tp);
			}
			else
			{
				if (randomPositonSteps < 5)
				{
					GetRandomPosition();
					randomPositonSteps++;
				}
			}
		}

		public bool isValidPosition(ref Vector3 tp)
		{
			bool result = false;

			Collider2D col = Physics2D.OverlapCircle(tp, fighterController.walkLayer);

			if (col != null)
			{
				TWalklabe w = col.gameObject.transform.GetComponentInParent<TWalklabe>();
				if (w != null)
				{
					result = true;
				}
				//Debug.DrawRay(tp, Vector3.up * 0.2f, Color.green, 10);
			}

			if (!result)
			{
				//Debug.DrawRay(tp, Vector3.up * 0.2f, Color.yellow, 10);
				col = Physics2D.OverlapCircle(tp, 5, fighterController.walkLayer);

				RaycastHit2D hit2D = Physics2D.Linecast(tp, col.transform.position, fighterController.walkLayer);
				if (hit2D.transform != null)
				{
					TWalklabe w = col.gameObject.transform.GetComponentInParent<TWalklabe>();
					if (w != null)
					{
						result = true;
					}

					tp = hit2D.point;
				}
			}

			Node n = GridManager.singleton.GetNode(tp);

			if (n.isWalkable)
			{
				result = true;
			}

			return result;
		}

		public bool GetPositionCloseToPlayer(Vector3 offset)
		{
			Vector3 dir = enemy.position - transform.position;
			if (dir.x > 0)
			{
				offset.x = -offset.x;
			}

			Vector3 tp = enemy.position;
			tp += offset;

			bool isValid = isValidPosition(ref tp);

			if (isValid)
			{
				currentPath = GridManager.singleton.GetPath(transform.position, tp);
				hasMovedPosition = true;
				return true;
			}
			return false;
		}

		List<Node> currentPath;
		Vector3 targetPosition;
		bool hasMovedPosition;

		public bool MoveToPosition(float delta)
		{
			if (currentPath == null || currentPath.Count == 0)
			{
				if (hasMovedPosition)
				{
					fighterController.TickPlayer(delta, Vector3.zero);
					hasMovedPosition = false;
				}
				return true;
			}

			targetPosition = currentPath[0].worldPosition;

			Vector3 p1 = targetPosition;
			p1.z = transform.position.z;

			Vector3 targetDirection = p1 - transform.position;
			targetDirection.Normalize();

			fighterController.TickPlayer(delta, targetDirection);

			float distanceToTarget = Vector2.Distance(transform.position, currentPath[0].worldPosition);
			if (distanceToTarget < targetOffset.x)
			{
				currentPath.RemoveAt(0);
			}

			return false;
		}
	}
}
