using System;
using System.Collections.Generic;
using UnityEngine;


namespace StarGeneration
{
	public class CumulativeDistributionFunction
	{
		private double m_min;
		private double m_max;
		private int m_steps;

		// Parameters for realistic star distribution
		private double m_I0;
		private double m_k;
		private double m_a;
		private double m_RBulge;

		private List<double> m_vM1;
		private List<double> m_vY1;
		private List<double> m_vX1;
		private List<double> m_vM2;
		private List<double> m_vY2;
		private List<double> m_vX2;


		public void SetupRealistic(double I0, double k, double a, double RBulge, double min, double max, int steps)
		{
			m_min = min;
			m_max = max;
			m_steps = steps;

			m_I0 = I0;
			m_k  = k;
			m_a  = a;
			m_RBulge = RBulge;

			// build the distribution function
			BuildCDF();
		}

		private void BuildCDF()
		{
			double h = (m_max - m_min) / m_steps;
			double x = 0, y = 0;

			m_vX1 = new List<double>();
			m_vY1 = new List<double>();
			m_vX2 = new List<double>();
			m_vY2 = new List<double>();
			m_vM1 = new List<double>();
			m_vM2 = new List<double>();

			// Simpson rule for integration of the distribution function
			m_vY1.Add(0.0);
			m_vX1.Add(0.0);
			for (int i = 0; i < m_steps; i += 2)
			{
				x = (i + 2) * h;
				y += h / 3 * (Intensity(m_min + i * h) + 4 * Intensity(m_min + (i + 1) * h) + Intensity(m_min + (i + 2) * h));

				m_vM1.Add((y - m_vY1[m_vY1.Count - 1]) / (2 * h));
				m_vX1.Add(x);
				m_vY1.Add(y);
			}
			m_vM1.Add(0.0);

			// All arrays must have the same length
			if ((m_vM1.Count != m_vX1.Count) || (m_vM1.Count != m_vY1.Count))
				throw new ArrayTypeMismatchException("Array size mismatch");

			// Normalise
			for (int i = 0; i < m_vY1.Count; ++i)
			{
				m_vY1[i] /= m_vY1[m_vY1.Count - 1];
				m_vM1[i] /= m_vY1[m_vY1.Count - 1];
			}

			m_vX2.Add(0.0);
			m_vY2.Add(0.0);
			double p = 0;
			h = 1.0 / m_steps;
			for (int i = 1, k = 0; i <m_steps; ++i)
			{
				p = (double)i * h;

				for (; m_vY1[k + 1] <= p; ++k) { }

				y = m_vX1[k] + (p - m_vY1[k]) / m_vM1[k];

				m_vM2.Add((y - m_vY2[m_vY2.Count - 1]) / h);
				m_vX2.Add(p);
				m_vY2.Add(y);
			}
			m_vM2.Add(0.0);

			// All arrays must have the same length
			if ((m_vM2.Count != m_vX2.Count) || (m_vM2.Count != m_vY2.Count))
				throw new ArrayTypeMismatchException("Array size mismatch");
		}

		public double ProbabilityFromValue(double value)
		{
			if ((value < m_min) || (value > m_max))
				throw new ArgumentOutOfRangeException("value", "Value is out of range. It should be between the provided min and max values (inclusive)");

			double h = 2 * ((m_max - m_min) / m_steps);
			int i = (int)((value - m_min) / h);
			double remainder = value - i * h;

			return (m_vY1[i] + m_vM1[i] * remainder);
		}

		public double ValueFromProbability(double probability)
		{
			if ((probability < 0) || (probability > 1))
				throw new ArgumentOutOfRangeException("probability", "Probability is out of range. It should be a probability between 0 and 1 (inclusive)");

			double h = 1.0 / (m_vY2.Count - 1);

			int i = (int)(probability / h);
			double remainder = probability - i * h;

			return (m_vY2[i] + m_vM2[i] * remainder);
		}

		private double IntensityBulge(double R, double I0, double k)
		{
			return I0 * Math.Exp(-k * Math.Pow(R, 0.25));
		}

		private double IntensityDisc(double R, double I0, double a)
		{
			return I0 * Math.Exp(-R / a);
		}

		private double Intensity(double x)
		{
			return (x<m_RBulge) ? IntensityBulge(x, m_I0, m_k) : IntensityDisc(x - m_RBulge, IntensityBulge(m_RBulge, m_I0, m_k), m_a);
		}
	}
}
