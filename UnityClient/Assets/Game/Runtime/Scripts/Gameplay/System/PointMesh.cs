using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PointMesh
{
	private Vector3[] m_positions;
	private Color[] m_colours;
	private Vector2[] m_uv;
	private Vector2[] m_offset;
	private int[] m_indices;

	private int m_vertexWriteOffset = 0;
	private int m_indexWriteOffset = 0;

	private MeshFilter m_mesh;

	public PointMesh(int noofPoints, MeshFilter templateMesh, string name)
	{
		m_mesh = MeshFilter.Instantiate(templateMesh, templateMesh.transform.parent) as MeshFilter;
		m_mesh.name = name;

		// Clamp vertices to a multiple of 4 under 65000
		int noofVerts = Mathf.Min(noofPoints * 4, 65000);

		// Clamp indices to a multiple of 6 under 65000
		int noofIndices = Mathf.Min(noofPoints * 6, 64998);

		// Set up a point cloud to render all of the dust
		m_positions = new Vector3[noofVerts];
		m_colours = new Color[noofVerts];
		m_uv = new Vector2[noofVerts];
		m_offset = new Vector2[noofVerts];
		m_indices = new int[noofIndices];
	}

	public bool CanAddPoint()
	{
		if ((m_vertexWriteOffset + 4) > 65000)
			return false;
		if ((m_indexWriteOffset + 6) > 64998)
			return false;
		return true;
	}

	public int AddPoint(Vector3 position, Color colour, float radius)
	{
		int vertexOffsetWrittenTo = m_vertexWriteOffset;

		// Write top-left
		m_positions[m_vertexWriteOffset + 0] = position;
		m_colours[m_vertexWriteOffset + 0] = colour;
		m_uv[m_vertexWriteOffset + 0] = new Vector2(0.0f, 1.0f);
		m_offset[m_vertexWriteOffset + 0] = new Vector2(radius, -radius);

		// Write top-right
		m_positions[m_vertexWriteOffset + 1] = position;
		m_colours[m_vertexWriteOffset + 1] = colour;
		m_uv[m_vertexWriteOffset + 1] = new Vector2(1.0f, 1.0f);
		m_offset[m_vertexWriteOffset + 1] = new Vector2(radius, radius);

		// Write bottom-left
		m_positions[m_vertexWriteOffset + 2] = position;
		m_colours[m_vertexWriteOffset + 2] = colour;
		m_uv[m_vertexWriteOffset + 2] = new Vector2(0.0f, 0.0f);
		m_offset[m_vertexWriteOffset + 2] = new Vector2(-radius, -radius);

		// Write bottom-right
		m_positions[m_vertexWriteOffset + 3] = position;
		m_colours[m_vertexWriteOffset + 3] = colour;
		m_uv[m_vertexWriteOffset + 3] = new Vector2(1.0f, 0.0f);
		m_offset[m_vertexWriteOffset + 3] = new Vector2(-radius, radius);

		// Write indices
		m_indices[m_indexWriteOffset + 0] = m_vertexWriteOffset + 0;
		m_indices[m_indexWriteOffset + 1] = m_vertexWriteOffset + 2;
		m_indices[m_indexWriteOffset + 2] = m_vertexWriteOffset + 1;
		m_indices[m_indexWriteOffset + 3] = m_vertexWriteOffset + 2;
		m_indices[m_indexWriteOffset + 4] = m_vertexWriteOffset + 3;
		m_indices[m_indexWriteOffset + 5] = m_vertexWriteOffset + 1;

		// Move to the next quad
		m_vertexWriteOffset += 4;
		m_indexWriteOffset += 6;

		// Return the vertex offset written to
		return vertexOffsetWrittenTo;
	}

	public void Finalise()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = m_positions;
		mesh.colors = m_colours;
		mesh.uv = m_uv;
		mesh.uv2 = m_offset;
		mesh.SetIndices(m_indices, MeshTopology.Triangles, 0);
		m_mesh.mesh = mesh;
	}

	public void UpdatePosition(int vertexOffset, Vector3 position)
	{
		// Write top-left
		m_positions[vertexOffset + 0] = position;

		// Write top-right
		m_positions[vertexOffset + 1] = position;

		// Write bottom-left
		m_positions[vertexOffset + 2] = position;

		// Write bottom-right
		m_positions[vertexOffset + 3] = position;
	}

	public void WritePositions()
	{
		Mesh mesh = m_mesh.mesh;
		mesh.vertices = m_positions;
	}
}
