using System.Collections.Generic;
using UnityEngine;

namespace Octree
{
	public class OctreePoint : MonoBehaviour
	{
		public bool CanBeProcessed
		{
			get
			{
				if (m_canBeProcessed)
				{
					m_canBeProcessed = false;
					return true;
				}

				return false;
			}
		}

		private bool m_canBeProcessed;

		public List<OctreeCell> Cells = new List<OctreeCell>();
		private Vector3 m_previousPosition;

		public void FixedUpdate() => UpdatePositionChange();

		private void UpdatePositionChange()
		{
			if (m_previousPosition != transform.position)
			{
				m_canBeProcessed = true;
				m_previousPosition = transform.position;
			}
		}

		public void UpdateContainingCells()
		{
			var validCells = new List<OctreeCell>();
			var obsoleteCells = new List<OctreeCell>();

			foreach (var node in Cells)
			{
				if (!node.ContainsPoint(transform.position))
				{
					obsoleteCells.Add(node);
				}
				else
				{
					validCells.Add(node);
				}
			}

			Cells = validCells;
			
			foreach (var cell in obsoleteCells)
			{
				cell.TryReduceSubdivisions(this);
			}
		}

		private void OnDrawGizmos()
		{
			foreach (var node in Cells)
			{
				Gizmos.color = new Color(0f, 0.18f, 1f, 0.21f);
				node.DrawGizmos(false);
			}
		}
	}
}