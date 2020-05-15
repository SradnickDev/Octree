using System.Collections.Generic;
using Octree;
using UnityEngine;

public class WorldBounds : MonoBehaviour
{
	[SerializeField] private int m_maxPointsPerCell = 1;
	[SerializeField] private float m_size = 10;
	[SerializeField] private int m_maxSubdivisions = 10;
	[SerializeField] private List<OctreePoint> m_points = new List<OctreePoint>();
	[SerializeField] private bool m_drawOctree = false;
	[SerializeField] private bool m_drawBounds = true;

	private OctreeCell m_rootCell;

	private void Start()
	{
		m_rootCell = new OctreeCell(null, transform.position, m_size / 2f, m_points,
									m_maxSubdivisions);

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
		if (m_drawOctree)
		{
			m_rootCell?.DrawGizmos();
		}

		if (m_drawBounds)
		{
			Gizmos.DrawWireCube(transform.position, Vector3.one * m_size);
		}
	}
}