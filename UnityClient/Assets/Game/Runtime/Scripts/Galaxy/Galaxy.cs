using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using OpenSimplexNoise;
using SpaceLibrary;
using StarGeneration;


[AddComponentMenu("SPACEJAM/Galaxy/Galaxy")]
public class Galaxy : MonoBehaviour
{
	private StarGeneration.GalaxyGenerator m_galaxy;

	// Templates for rendering different types of celestial object
	public MeshFilter m_dustMesh;
	public MeshFilter m_h2Mesh;
	public MeshFilter m_starMesh;

	private class ObjectIndexAndWriteOffsetPair
	{
		public int m_objectIndex;
		public int m_writeOffset;

		public ObjectIndexAndWriteOffsetPair(int objectIndex, int writeOffset)
		{
			m_objectIndex = objectIndex;
			m_writeOffset = writeOffset;
		}
	}

	private class PointCloud
	{
		public PointMesh m_pointMesh;
		public List<ObjectIndexAndWriteOffsetPair> m_renderData;

		public PointCloud(int noofPoints, MeshFilter templateMesh, string name)
		{
			m_pointMesh = new PointMesh(noofPoints, templateMesh, name);
			m_renderData = new List<ObjectIndexAndWriteOffsetPair>();
		}
	}

	List<PointCloud> m_dustPointClouds = new List<PointCloud>();
	List<PointCloud> m_h2PointClouds = new List<PointCloud>();
	List<PointCloud> m_starPointClouds = new List<PointCloud>();

	// Multiplier for point sprite sizes to account for less than ideal number of stars. Reduce as more stars are added
	private static float ms_sizeMultiplier = 4.0f;

	// Use this for initialization
	protected void Start()
	{
		// Generate a galaxy
		m_galaxy = new StarGeneration.GalaxyGenerator(
				new System.Random(System.DateTime.Now.Millisecond),
				13000,      // Radius of the galaxy
				4000,       // Radius of the core
				0.0004,     // Angular offset of the density wave per parsec of radius
				0.85,       // Eccentricity at the edge of the core
				0.95,       // Eccentricity at the edge of the disk
				80000,      // Total number of stars
				2,          // Perturbations per full ellipse (number of spiral arms / 2)
				80          // Amplitude damping factor of perturbation (width of arms: smaller = tight, larger = spread)
			);


		//double rangeOfInterest = m_galaxy.GetFarFieldRadius() * 1.3;

		// ---------------------------------------------------------------------

		// Count the dust in the systems
		StarGeneration.StarGenerator[] dustClouds = m_galaxy.GetDust();
		int noofDustClouds = dustClouds.Length;

		// Set up a point cloud to render all of the dust
		PointCloud dustPointCloud = new PointCloud(noofDustClouds, m_dustMesh, "Mesh_Dust");
		m_dustPointClouds.Add(dustPointCloud);

		// Add point sprites for each dust cloud
		for (int dustIndex = 0; dustIndex < dustClouds.Length; ++dustIndex)
		{
			StarGeneration.StarGenerator dustCloud = dustClouds[dustIndex];
			Vector3 dustColourVector = dustCloud.TemperatureColour() * (float)dustCloud.m_brightnessMagnitude;
			Color colour = new Color((float)(dustColourVector.x), (float)(dustColourVector.y), (float)(dustColourVector.z));
			float radius = 400 * ms_sizeMultiplier;

			if (!dustPointCloud.m_pointMesh.CanAddPoint())
			{
				dustPointCloud = new PointCloud(noofDustClouds, m_dustMesh, "Mesh_Dust");
				m_dustPointClouds.Add(dustPointCloud);
			}

			dustPointCloud.m_renderData.Add(new ObjectIndexAndWriteOffsetPair(dustIndex, dustPointCloud.m_pointMesh.AddPoint(dustCloud.m_position, colour, radius)));
		}
		foreach (PointCloud pointCloud in m_dustPointClouds)
			pointCloud.m_pointMesh.Finalise();

		// ---------------------------------------------------------------------

		// Count the H2 stellar nurseries in the systems
		StarGeneration.StarGenerator[] h2s = m_galaxy.GetH2();

		// Set up a point cloud to render all of the H2 stellar nurseries
		PointCloud h2PointCloud = new PointCloud(h2s.Length / 2, m_h2Mesh, "Mesh_H2");
		m_h2PointClouds.Add(h2PointCloud);

		// Add point sprites for each H2 star
		for (int h2Index = 0; h2Index < (h2s.Length / 2); ++h2Index)
		{
			int k1 = 2 * h2Index;
			int k2 = (2 * h2Index) + 1;

			Vector3 p1 = h2s[k1].m_position;
			Vector3 p2 = h2s[k2].m_position;

			// Calculate the size of the nursery, and ignore those below a certain size
			double dst = Mathf.Sqrt((p1.x-p2.x)*(p1.x-p2.x) + (p1.y-p2.y)*(p1.y-p2.y));
			double size = ((1000-dst) / 10) - 50;
			if (size < 1)
				continue;
			float radius = (float)(size * 2 * ms_sizeMultiplier);

			Vector3 h2ColourVector = h2s[k1].TemperatureColour() * (float)h2s[h2Index].m_brightnessMagnitude;
			Color colour = new Color((float)(h2ColourVector.x * 2), (float)(h2ColourVector.y * 0.5), (float)(h2ColourVector.z * 0.5));

			if (!h2PointCloud.m_pointMesh.CanAddPoint())
			{
				h2PointCloud = new PointCloud(h2s.Length / 2, m_h2Mesh, "Mesh_H2");
				m_h2PointClouds.Add(h2PointCloud);
			}

			h2PointCloud.m_renderData.Add(new ObjectIndexAndWriteOffsetPair(h2Index, h2PointCloud.m_pointMesh.AddPoint(p1, colour, radius)));
		}
		foreach (PointCloud pointCloud in m_h2PointClouds)
			pointCloud.m_pointMesh.Finalise();

		// ---------------------------------------------------------------------

		// Count the stars in the systems
		StarGeneration.StarGenerator[] stars = m_galaxy.GetStars();
		int noofStars = stars.Length;

		// Set up a point cloud to render all of the stars
		PointCloud starPointCloud = new PointCloud(noofStars, m_starMesh, "Mesh_Star");
		m_starPointClouds.Add(starPointCloud);

		// Add point sprites for each star
		for (int starIndex = 0; starIndex < stars.Length; ++starIndex)
		{
			StarGeneration.StarGenerator star = stars[starIndex];
			Vector3 starColourVector = star.TemperatureColour() * (float)star.m_brightnessMagnitude;
			Color colour = new Color((float)(starColourVector.x), (float)(starColourVector.y), (float)(starColourVector.z));
			float radius = (float)(star.m_radius * 12 * ms_sizeMultiplier);

			if (!starPointCloud.m_pointMesh.CanAddPoint())
			{
				starPointCloud = new PointCloud(noofStars, m_starMesh, "Mesh_Star");
				m_starPointClouds.Add(starPointCloud);
			}

			starPointCloud.m_renderData.Add(new ObjectIndexAndWriteOffsetPair(starIndex, starPointCloud.m_pointMesh.AddPoint(star.m_position, colour, radius)));
		}
		foreach (PointCloud pointCloud in m_starPointClouds)
			pointCloud.m_pointMesh.Finalise();
	}

	protected void Update()
	{
		// Time in years
		double speed = 50000;
		m_galaxy.SingleTimeStep(speed);

		// Update dust positions
		StarGeneration.StarGenerator[] dustClouds = m_galaxy.GetDust();
		foreach (PointCloud pointCloud in m_dustPointClouds)
		{
			foreach (ObjectIndexAndWriteOffsetPair renderData in pointCloud.m_renderData)
				pointCloud.m_pointMesh.UpdatePosition(renderData.m_writeOffset, dustClouds[renderData.m_objectIndex].m_position);
			pointCloud.m_pointMesh.WritePositions();
		}

		// Update H2 positions
		StarGeneration.StarGenerator[] h2s = m_galaxy.GetH2();
		foreach (PointCloud pointCloud in m_h2PointClouds)
		{
			foreach (ObjectIndexAndWriteOffsetPair renderData in pointCloud.m_renderData)
			{
				Vector3 position = h2s[2 * renderData.m_objectIndex].m_position;
				pointCloud.m_pointMesh.UpdatePosition(renderData.m_writeOffset, position);
			}
			pointCloud.m_pointMesh.WritePositions();
		}

		// Update star positions
		StarGeneration.StarGenerator[] stars = m_galaxy.GetStars();
		foreach (PointCloud pointCloud in m_starPointClouds)
		{
			foreach (ObjectIndexAndWriteOffsetPair renderData in pointCloud.m_renderData)
				pointCloud.m_pointMesh.UpdatePosition(renderData.m_writeOffset, stars[renderData.m_objectIndex].m_position);
			pointCloud.m_pointMesh.WritePositions();
		}
	}

}
