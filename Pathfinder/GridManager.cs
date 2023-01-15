using System.Collections.Generic;
using UnityEngine;

namespace FG
{
	public class GridManager : MonoBehaviour
	{
		public float scale = 0.1f;
		public Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

		public static GridManager singleton;
		public LayerMask walkLayer;

		private void Awake()
		{
			singleton = this;
		}

		public Node GetNode(Vector3 p)
		{
			int x = Mathf.RoundToInt(p.x / scale);
			int y = Mathf.RoundToInt(p.y / scale);

			return GetNode(x, y);
		}

		public Node GetNode(int x, int y)
		{
			Vector2Int p = Vector2Int.zero;
			p.x = x;
			p.y = y;

			if (!grid.ContainsKey(p))
			{
				Node n = new Node();
				n.x = x;
				n.y = y;
				Vector3 targetPosition = Vector3.zero;
				targetPosition.x = x * scale;
				targetPosition.y = y * scale;
				n.worldPosition = targetPosition;
				n.isWalkable = GetValidPosition(targetPosition);
				grid.Add(p, n);
			}

			grid.TryGetValue(p, out Node retVal);

			return retVal;
		}

		bool GetValidPosition(Vector3 o)
		{
			bool retVal = false;
			Collider2D[] collders = Physics2D.OverlapPointAll(o, walkLayer);

			foreach (var coll in collders)
			{
				TWalklabe w = coll.transform.GetComponentInParent<TWalklabe>();
				if (w != null)
				{
					retVal = true;
				}
			}
			return retVal;
		}

		public List<Node> GetPath(Vector3 start, Vector3 end)
		{
			Pathfinder path = new Pathfinder(start, end);
			return path.FindPath();
		}
	}
}
