using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	public class Star
    {
		/// <summary>
		/// Star classification type.
		/// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		/// Class,	Effective Temp.,	Vega-relative colour label,	Chromaticity,		Main-sequence mass,	Main-sequence radius,	Main-sequence luminosity,	Hydrogen lines,	Fraction of stars
		/// -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		/// O,		≥ 30,000 K,			blue,						blue,				≥ 16 M☉,			≥ 6.6 R☉,				≥ 30,000 L☉,				Weak,			~0.00003%
		/// B,		10,000–30,000 K,	blue white,					deep blue white,	2.1–16 M☉,			1.8–6.6 R☉,				25–30,000 L☉,				Medium,			0.13%
		/// A,		7,500–10,000 K,		white,						blue white,			1.4–2.1 M☉,			1.4–1.8 R☉,				5–25 L☉,					Strong,			0.6%
		/// F,		6,000–7,500 K,		yellow white,				white,				1.04–1.4 M☉,		1.15–1.4 R☉,			1.5–5 L☉,					Medium,			3%
		/// G,		5,200–6,000 K,		yellow,						yellowish white,	0.8–1.04 M☉,		0.96–1.15 R☉,			0.6–1.5 L☉,					Weak,			7.6%
		/// K,		3,700–5,200 K,		orange,						pale yellow orange,	0.45–0.8 M☉,		0.7–0.96 R☉,			0.08–0.6 L☉,				Very weak,		12.1%
		/// M,		2,400–3,700 K,		red,						light orange red,	0.08–0.45 M☉,		≤ 0.7 R☉,				≤ 0.08 L☉,					Very weak,		76.45%
		/// </summary>
		public enum Class
		{
			O,
			B,
			A,
			F,
			G,
			K,
			M,
		}

		/// <summary>
		/// Orbital distance in astronomical units from the system centre
		/// </summary>
		public double OrbitalDistance
		{
			get; set;
		}

		/// <summary>
		/// Name of the star
		/// </summary>
		public string Name
		{
			get; set;
		}

		/// <summary>
		/// Radius in solar radii (1 R☉ = 6.957 * 10^5 km)
		/// </summary>
		public double Radius
		{
			get; set;
		}

		/// <summary>
		/// Temperature of the star in degrees Kelvin
		/// </summary>
		public double Temperature
		{
			get; set;
		}

		/// <summary>
		/// Classification of the star type
		/// </summary>
		public Class Classification
		{
			get; set;
		}
	}
}
