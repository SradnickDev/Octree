using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Octree
{
	public class OctreeCell
	{
		public static int MaxPoints = 1;
		public static OctreeCell Root;

		public OctreeCell Parent;
		public List<OctreePoint> ContainedItems = new List<OctreePoint>();
		public OctreeCell[] ChildCells => m_childCells;
		private OctreeCell[] m_childCells = new OctreeCell[8];

		private Vector3 m_position;
		private Bounds m_bounds;
		private float m_halfDimension;

		public OctreeCell(OctreeCell parent,
						  Vector3 childPos,
						  float halfDimension,
						  List<OctreePoint> potentialItems)
		{
			Parent = parent;
			m_position = childPos;
			m_halfDimension = halfDimension;

			m_bounds = new Bounds(m_position, Vector3.one * halfDimension * 2f);

			foreach (var item in potentialItems)
			{
				ProcessPoint(item);
			}
		}

		public bool ProcessPoint(OctreePoint point)
		{
			if (ContainsPoint(point.transform.position))
			{
				if (m_childCells[0] == null)
				{
					PushItem(point);
				}
				else
				{
					foreach (var childNode in m_childCells)
					{
						if (childNode.ProcessPoint(point))
						{
							return true;
						}
					}
				}
			}

			return false;
		}

		private void PushItem(OctreePoint point)
		{
			if (!ContainedItems.Contains(point))
			{
				ContainedItems.Add(point);
				point.Cells.Add(this);
			}

			if (ContainedItems.Count > MaxPoints)
			{
				Split();
			}
		}

		private void Split()
		{
			foreach (var containedItem in ContainedItems)
			{
				containedItem.Cells.Remove(this);
			}

			//create upper cells
			var extents = m_bounds.extents / 2f;
			for (var i = 0; i < 4; i++)
			{
				var childPosition = m_position + extents;
				m_childCells[i] =
					new OctreeCell(this, childPosition, m_halfDimension / 2, ContainedItems);
				extents = Quaternion.Euler(0f, -90f, 0f) * extents;
			}

			//create lower cells
			extents = m_bounds.extents / 2f;
			extents.y = -extents.y;

			for (var i = 4; i < 8; i++)
			{
				var childPosition = m_position + extents;
				m_childCells[i] =
					new OctreeCell(this, childPosition, m_halfDimension / 2, ContainedItems);
				extents = Quaternion.Euler(0f, -90f, 0f) * extents;
			}

			ContainedItems.Clear();
		}

		public bool ContainsPoint(Vector3 position)
		{
			if (position.x > m_position.x + m_halfDimension
			 || position.x < m_position.x - m_halfDimension)
				return false;
			if (position.y > m_position.y + m_halfDimension
			 || position.y < m_position.y - m_halfDimension)
				return false;
			if (position.z > m_position.z + m_halfDimension
			 || position.z < m_position.z - m_halfDimension)
				return false;
			return true;
		}

		public void TryReduceSubdivision(OctreePoint point)
		{
			if (this != Root && !SiblingNodesContainsToManyPoints())
			{
				foreach (var cell in Parent.m_childCells)
				{
					cell.Delete(Parent.ChildCells.Where(x => !ReferenceEquals(x, this)).ToArray());
				}

				Parent.ClearChildCells();
			}
			else
			{
				ContainedItems.Remove(point);
				point.Cells.Remove(this);
			}
		}

		private bool SiblingNodesContainsToManyPoints()
		{
			var containingItems = new List<OctreePoint>();

			foreach (var sibling in Parent.ChildCells)
			{
				if (sibling.ChildCells[0] != null) return true;

				containingItems.AddRange(sibling.ContainedItems.Where(x => !containingItems
																		  .Contains(x)));
			}

			return containingItems.Count > MaxPoints + 1;
		}

		private void Delete(OctreeCell[] obsoleteCell)
		{
			foreach (var item in ContainedItems)
			{
				item.Cells = item.Cells.Except(obsoleteCell).ToList();
				item.Cells.Remove(this);

				item.Cells.Add(Parent);
				Parent.ContainedItems.Add(item);
			}
		}

		private void ClearChildCells()
		{
			m_childCells = new OctreeCell[8];
		}

		public void DrawGizmos(bool onlyWire = true)
		{
			if (onlyWire)
			{
				Gizmos.DrawWireCube(m_position,
									Vector3.one * 2 * m_halfDimension);
			}
			else
			{
				Gizmos.DrawCube(m_position,
								Vector3.one * 2 * m_halfDimension);
			}

			foreach (var cell in ChildCells)
			{
				cell?.DrawGizmos();
			}
		}
	}
}