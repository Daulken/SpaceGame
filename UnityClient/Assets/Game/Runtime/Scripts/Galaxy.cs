using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using OpenSimplexNoise;
using SpaceLibrary;
using StarGeneration;


[AddComponentMenu("SPACEJAM/Galaxy")]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Galaxy : MonoBehaviour
{
	private Mesh m_galaxyMesh;

	// Use this for initialization
	protected void Start()
	{
		// Generate a galaxy using a fixed seed
		StarGeneration.Galaxy galaxy = StarGeneration.Galaxy.Generate(new StarGeneration.Galaxies.Spiral(), new System.Random(System.DateTime.Now.Millisecond));

		// Count the stars in the systems
		SpaceLibrary.StarSystem[] starSystems = galaxy.StarSystems.ToArray();
		int noofStars = 0;
		foreach (SpaceLibrary.StarSystem starSystem in starSystems)
			noofStars += starSystem.Stars.Length;

		// Set up a point cloud to render all of the stars
		m_galaxyMesh = new Mesh();
		GetComponent<MeshFilter>().mesh = m_galaxyMesh;
		Vector3[] points = new Vector3[noofStars * 4];
		Color[] colours = new Color[noofStars * 4];
		Vector2[] uv = new Vector2[noofStars * 4];
		Vector2[] offset = new Vector2[noofStars * 4];
		int[] indices = new int[noofStars * 6];

		int vertexOffset = 0;
		int indexOffset = 0;

		foreach (SpaceLibrary.StarSystem starSystem in starSystems)
		{
			Vector3 systemPosition = starSystem.Position.ToVector3();

			foreach (SpaceLibrary.Star star in starSystem.Stars)
			{
				Vector3 tempColour = star.TemperatureColour();
				Color colour = new Color(tempColour.x, tempColour.y, tempColour.z);
				float radius = (float)star.Radius * 6.957f;
				Vector3 position = systemPosition;		// Ignoring orbital radius for now

				// Write top-left
				points[vertexOffset + 0] = position;
				colours[vertexOffset + 0] = colour;
				uv[vertexOffset + 0] = new Vector2(0.0f, 1.0f);
				offset[vertexOffset + 0] = new Vector2(radius, -radius);

				// Write top-right
				points[vertexOffset + 1] = position;
				colours[vertexOffset + 1] = colour;
				uv[vertexOffset + 1] = new Vector2(1.0f, 1.0f);
				offset[vertexOffset + 1] = new Vector2(radius, radius);

				// Write bottom-left
				points[vertexOffset + 2] = position;
				colours[vertexOffset + 2] = colour;
				uv[vertexOffset + 2] = new Vector2(0.0f, 0.0f);
				offset[vertexOffset + 2] = new Vector2(-radius, -radius);

				// Write bottom-right
				points[vertexOffset + 3] = position;
				colours[vertexOffset + 3] = colour;
				uv[vertexOffset + 3] = new Vector2(1.0f, 0.0f);
				offset[vertexOffset + 3] = new Vector2(-radius, radius);

				// Write indices
				indices[indexOffset + 0] = vertexOffset + 0;
				indices[indexOffset + 1] = vertexOffset + 2;
				indices[indexOffset + 2] = vertexOffset + 1;
				indices[indexOffset + 3] = vertexOffset + 2;
				indices[indexOffset + 4] = vertexOffset + 3;
				indices[indexOffset + 5] = vertexOffset + 1;

				// Move to the next quad
				vertexOffset += 4;
				indexOffset += 6;
			}
		}

		m_galaxyMesh.vertices = points;
		m_galaxyMesh.colors = colours;
		m_galaxyMesh.uv = uv;
		m_galaxyMesh.uv2 = offset;
		m_galaxyMesh.SetIndices(indices, MeshTopology.Triangles, 0);
	}

	protected void Update()
	{

	}

}
