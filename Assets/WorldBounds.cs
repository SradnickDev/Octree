using System.Collections.Generic;
using Octree;
using UnityEngine;

public class WorldBounds : MonoBehaviour
{
	[SerializeField] private int m_maxPointsPerCell = 1;
	[SerializeField] private float Size = 10;
	[SerializeField] private List<OctreePoint> m_points = new List<OctreePoint>();

	private OctreeCell m_rootCell;

	private void Start()
	{
		m_rootCell = new OctreeCell(null, transform.position, Size / 2f, m_points);
		OctreeCell.MaxPoints = m_maxPointsPerCell;
		OctreeCell.Root = m_rootCell;
	}

	private void FixedUpdate() => UpdatePoints();

	private void UpdatePoints()
	{
		foreach (var point in m_points)
		{
			if (point.CanBeProcessed && m_rootCell.ContainsPoint(point.transform.position))
			{
				m_rootCell.ProcessPoint(point);
				point.UpdateContainingCells();
			}
		}
	}

	private void OnDrawGizmos()
	{
		m_rootCell?.DrawGizmos();
	}
}