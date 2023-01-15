using System.Collections.Generic;
using UnityEngine;


namespace FG
{
	public class Pathfinder
	{
		Node startNode;
		Node targetNode;
		public Pathfinder(Vector3 start, Vector3 target)
		{
			startNode = GetNode(start);
			targetNode = GetNode(target);
		}

		public List<Node> FindPath()
		{
			List<Node> path = FindPathImpl();
			return path;
		}

		List<Node> FindPathImpl()
		{
			List<Node> foundPath = new List<Node>();

			List<Node> openSet = new List<Node>();
			HashSet<Node> closedSet = new HashSet<Node>();

			openSet.Add(startNode);

			while (openSet.Count > 0)
			{
				Node currentNode = openSet[0];

				for (int i = 0; i < openSet.Count; i++)
				{
					if (openSet[i].fCost < currentNode.fCost ||
							(openSet[i].fCost == currentNode.fCost))
					{
						if (openSet[i].hCost < currentNode.hCost)
						{
							if (!currentNode.Equals(openSet[i]))
								currentNode = openSet[i];
						}
					}
				}
				openSet.Remove(currentNode);
				closedSet.Add(currentNode);

				if (currentNode.Equals(targetNode))
				{
					foundPath = RetracePath(startNode, currentNode);
					break;
				}

				foreach (Node neighbour in GetNeighbours(currentNode))
				{
					if (!neighbour.isWalkable || closedSet.Contains(neighbour))
					{
						continue;
					}

					float newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

					if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
					{
						neighbour.gCost = newCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parentNode = currentNode;

						if (!openSet.Contains(neighbour))
						{
							openSet.Add(neighbour);
						}
					}
				}
			}
			return foundPath;
		}

		private List<Node> GetNeighbours(Node node)
		{
			List<Node> retList = new List<Node>();

			for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0)
					{
						//00 is the current node
					}
					else
					{
						int _x = node.x + x;
						int _y = node.y + y;

						Node targetNode = GetNode(_x, _y);
						if (targetNode.isWalkable)
							retList.Add(targetNode);
					}
				}
			}
			return retList;
		}

		private List<Node> RetracePath(Node startNode, Node endNode)
		{
			List<Node> path = new List<Node>();
			Node currentNode = endNode;

			while (currentNode != startNode)
			{
				path.Add(currentNode);
				currentNode = currentNode.parentNode;
			}
			path.Reverse();

			return path;
		}

		private int GetDistance(Node posA, Node posB)
		{
			int distX = Mathf.Abs(posA.x - posB.x);
			int distY = Mathf.Abs(posA.y - posB.y);

			return 14 * distX + 10 * (distY - distX) + 10 * distY;
		}

		Node GetNode(Vector3 position)
		{
			return GridManager.singleton.GetNode(position);
		}

		Node GetNode(int x, int y)
		{
			return GridManager.singleton.GetNode(x, y);
		}
	}

	public class Node
	{
		public int x;
		public int y;
		public int step;

		public float hCost;
		public float gCost;

		public float fCost
		{
			get { return gCost + hCost; }
		}

		public Node parentNode;
		public bool isWalkable;
		public Vector3 worldPosition;
	}
}
