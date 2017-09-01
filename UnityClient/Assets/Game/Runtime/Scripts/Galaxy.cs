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
	
	//private OpenSimplexNoise.OpenSimplexNoise m_noise = new OpenSimplexNoise.OpenSimplexNoise();

	// Use this for initialization
	protected void Start()
	{
		// Generate a galaxy using a fixed seed
		StarGeneration.Galaxy galaxy = StarGeneration.Galaxy.Generate(new StarGeneration.Galaxies.Spiral(), new System.Random(543554));
		SpaceLibrary.Star[] stars = galaxy.Stars.ToArray();

		// Set up a point cloud to render this
		m_galaxyMesh = new Mesh();
		GetComponent<MeshFilter>().mesh = m_galaxyMesh;
		Vector3[] points = new Vector3[stars.Length];
		Color[] colours = new Color[stars.Length];
		int[] indices = new int[stars.Length];
		for (int starIndex = 0; starIndex < stars.Length; ++starIndex)
		{
			Vector3 colour = stars[starIndex].TemperatureColour();

			indices[starIndex] = starIndex;
			points[starIndex] = stars[starIndex].Position.ToVector3();
			colours[starIndex] = new Color(colour.x, colour.y, colour.z);
		}
		m_galaxyMesh.vertices = points;
		m_galaxyMesh.colors = colours;
		m_galaxyMesh.SetIndices(indices, MeshTopology.Points, 0);
	}

	protected void Update()
	{

	}

}
