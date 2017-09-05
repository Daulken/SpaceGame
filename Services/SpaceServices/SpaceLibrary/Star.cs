using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Description of a star.
	/// <para>Note that while luminosity is not included, it can be calculated from its radius and temperature: L = 4π R^2 σ T^4.
	/// The constant, σ, is the Stefan-Boltzman radiation constant and it has a value of σ = 5.67 x 10^-5 ergs/(cm^2 sec deg^4).
	/// See https://spacemath.gsfc.nasa.gov/weekly/5Page44.pdf for details.</para>
	/// </summary>
	public class Star
    {
		/// <summary>
		/// Star classification type.
		/// </summary>
		public enum Class
		{
			/// <summary>
			/// Class O Star
			/// <para>Effective Temperature: ≥ 30,000 K</para>
			/// <para>Vega-relative Colour Label: Blue</para>
			/// <para>Chromaticity: Blue</para>
			/// <para>Main-sequence Mass: ≥ 16 M☉</para>
			/// <para>Main-sequence Radius: ≥ 6.6 R☉</para>
			/// <para>Main-sequence Luminosity: ≥ 30,000 L☉</para>
			/// <para>Hydrogen Lines: Weak</para>
			/// <para>Fraction of Main Sequence Stars In This Class: ~0.00003%</para>
			/// </summary>
			O,

			/// <summary>
			/// Class B Star
			/// <para>Effective Temperature: 10,000 – 30,000 K</para>
			/// <para>Vega-relative Colour Label: Blue White</para>
			/// <para>Chromaticity: Deep Blue White</para>
			/// <para>Main-sequence Mass: 2.1 – 16 M☉</para>
			/// <para>Main-sequence Radius: 1.8 – 6.6 R☉</para>
			/// <para>Main-sequence Luminosity: 25 – 30,000 L☉</para>
			/// <para>Hydrogen Lines: Medium</para>
			/// <para>Fraction of Main Sequence Stars In This Class: 0.13%</para>
			/// </summary>
			B,

			/// <summary>
			/// Class A Star
			/// <para>Effective Temperature: 7,500 – 10,000 K</para>
			/// <para>Vega-relative Colour Label: White</para>
			/// <para>Chromaticity: Blue White</para>
			/// <para>Main-sequence Mass: 1.4 – 2.1 M☉</para>
			/// <para>Main-sequence Radius: 1.4 – 1.8 R☉</para>
			/// <para>Main-sequence Luminosity: 5 – 25 L☉</para>
			/// <para>Hydrogen Lines: Strong</para>
			/// <para>Fraction of Main Sequence Stars In This Class: 0.6%</para>
			/// </summary>
			A,

			/// <summary>
			/// Class F Star
			/// <para>Effective Temperature: 6,000 – 7,500 K</para>
			/// <para>Vega-relative Colour Label: Yellow White</para>
			/// <para>Chromaticity: White</para>
			/// <para>Main-sequence Mass: 1.04 – 1.4 M☉</para>
			/// <para>Main-sequence Radius: 1.15 – 1.4 R☉</para>
			/// <para>Main-sequence Luminosity: 1.5 – 5 L☉</para>
			/// <para>Hydrogen Lines: Medium</para>
			/// <para>Fraction of Main Sequence Stars In This Class: 3%</para>
			/// </summary>
			F,

			/// <summary>
			/// Class G Star
			/// <para>Effective Temperature: 5,200 – 6,000 K</para>
			/// <para>Vega-relative Colour Label: Yellow</para>
			/// <para>Chromaticity: Yellowish White</para>
			/// <para>Main-sequence Mass: 0.8 – 1.04 M☉</para>
			/// <para>Main-sequence Radius: 0.96 – 1.15 R☉</para>
			/// <para>Main-sequence Luminosity: 0.6 – 1.5 L☉</para>
			/// <para>Hydrogen Lines: Weak</para>
			/// <para>Fraction of Main Sequence Stars In This Class: 7.6%</para>
			/// </summary>
			G,

			/// <summary>
			/// Class K Star
			/// <para>Effective Temperature: 3,700 – 5,200 K</para>
			/// <para>Vega-relative Colour Label: Orange</para>
			/// <para>Chromaticity: Pale Yellow Orange</para>
			/// <para>Main-sequence Mass: 0.45 – 0.8 M☉</para>
			/// <para>Main-sequence Radius: 0.7 – 0.96 R☉</para>
			/// <para>Main-sequence Luminosity: 0.08 – 0.6 L☉</para>
			/// <para>Hydrogen Lines: Very Weak</para>
			/// <para>Fraction of Main Sequence Stars In This Class: 12.1%</para>
			/// </summary>
			K,

			/// <summary>
			/// Class M Star
			/// <para>Effective Temperature: 2,400 – 3,700 K</para>
			/// <para>Vega-relative Colour Label: Red</para>
			/// <para>Chromaticity: Light Orange Red</para>
			/// <para>Main-sequence Mass: 0.08 – 0.45 M☉</para>
			/// <para>Main-sequence Radius: ≤ 0.7 R☉</para>
			/// <para>Main-sequence Luminosity: ≤ 0.08 L☉</para>
			/// <para>Hydrogen Lines: Very Weak</para>
			/// <para>Fraction of Main Sequence Stars In This Class: 76.45%</para>
			/// </summary>
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
