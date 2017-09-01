﻿/*
 * The MIT License (MIT)
 * Copyright (c) 2014 Andrés Correa Casablanca
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/*
 * Contributors:
 *  - Andrés Correa Casablanca <castarco@gmail.com , castarco@litipk.com>
 */


namespace Litipk.ColorSharp
{
	namespace MatchingFunctions
	{
		/**
		 * This class will be a singleton
		 */
		sealed public class CIE1964XYZ1Nm10DegZ : RegularMatchingFunction
		{
			/**
			 * <summary>Component Z of CIE's 1964 10º matching functions (1nm of precision)</summary>
			 */
			public static readonly CIE1964XYZ1Nm10DegZ Instance = new CIE1964XYZ1Nm10DegZ (
				360.0, new [] {
					0.000000535027,
					0.000000810720,
					0.000001221200,
					0.000001828700,
					0.000002722200,
					0.000004028300,
					0.000005925700,
					0.000008665100,
					0.000012596000,
					0.000018201000,
					0.000026143700,
					0.000037330000,
					0.000052987000,
					0.000074764000,
					0.000104870000,
					0.000146220000,
					0.000202660000,
					0.000279230000,
					0.000382450000,
					0.000520720000,
					0.000704776000,
					0.000948230000,
					0.001268200000,
					0.001686100000,
					0.002228500000,
					0.002927800000,
					0.003823700000,
					0.004964200000,
					0.006406700000,
					0.008219300000,
					0.010482200000,
					0.013289000000,
					0.016747000000,
					0.020980000000,
					0.026127000000,
					0.032344000000,
					0.039802000000,
					0.048691000000,
					0.059210000000,
					0.071576000000,
					0.086010900000,
					0.102740000000,
					0.122000000000,
					0.144020000000,
					0.168990000000,
					0.197120000000,
					0.228570000000,
					0.263470000000,
					0.301900000000,
					0.343870000000,
					0.389366000000,
					0.437970000000,
					0.489220000000,
					0.542900000000,
					0.598810000000,
					0.656760000000,
					0.716580000000,
					0.778120000000,
					0.841310000000,
					0.906110000000,
					0.972542000000,
					1.038900000000,
					1.103100000000,
					1.165100000000,
					1.224900000000,
					1.282500000000,
					1.338200000000,
					1.392600000000,
					1.446100000000,
					1.499400000000,
					1.553480000000,
					1.607200000000,
					1.658900000000,
					1.708200000000,
					1.754800000000,
					1.798500000000,
					1.839200000000,
					1.876600000000,
					1.910500000000,
					1.940800000000,
					1.967280000000,
					1.989100000000,
					2.005700000000,
					2.017400000000,
					2.024400000000,
					2.027300000000,
					2.026400000000,
					2.022300000000,
					2.015300000000,
					2.006000000000,
					1.994800000000,
					1.981400000000,
					1.965300000000,
					1.946400000000,
					1.924800000000,
					1.900700000000,
					1.874100000000,
					1.845100000000,
					1.813900000000,
					1.780600000000,
					1.745370000000,
					1.709100000000,
					1.672300000000,
					1.634700000000,
					1.595600000000,
					1.554900000000,
					1.512200000000,
					1.467300000000,
					1.419900000000,
					1.370000000000,
					1.317560000000,
					1.262400000000,
					1.205000000000,
					1.146600000000,
					1.088000000000,
					1.030200000000,
					0.973830000000,
					0.919430000000,
					0.867460000000,
					0.818280000000,
					0.772125000000,
					0.728290000000,
					0.686040000000,
					0.645530000000,
					0.606850000000,
					0.570060000000,
					0.535220000000,
					0.502340000000,
					0.471400000000,
					0.442390000000,
					0.415254000000,
					0.390024000000,
					0.366399000000,
					0.344015000000,
					0.322689000000,
					0.302356000000,
					0.283036000000,
					0.264816000000,
					0.247848000000,
					0.232318000000,
					0.218502000000,
					0.205851000000,
					0.193596000000,
					0.181736000000,
					0.170281000000,
					0.159249000000,
					0.148673000000,
					0.138609000000,
					0.129096000000,
					0.120215000000,
					0.112044000000,
					0.104710000000,
					0.098196000000,
					0.092361000000,
					0.087088000000,
					0.082248000000,
					0.077744000000,
					0.073456000000,
					0.069268000000,
					0.065060000000,
					0.060709000000,
					0.056457000000,
					0.052609000000,
					0.049122000000,
					0.045954000000,
					0.043050000000,
					0.040368000000,
					0.037839000000,
					0.035384000000,
					0.032949000000,
					0.030451000000,
					0.028029000000,
					0.025862000000,
					0.023920000000,
					0.022174000000,
					0.020584000000,
					0.019127000000,
					0.017740000000,
					0.016403000000,
					0.015064000000,
					0.013676000000,
					0.012308000000,
					0.011056000000,
					0.009915000000,
					0.008872000000,
					0.007918000000,
					0.007030000000,
					0.006223000000,
					0.005453000000,
					0.004714000000,
					0.003988000000,
					0.003289000000,
					0.002646000000,
					0.002063000000,
					0.001533000000,
					0.001091000000,
					0.000711000000,
					0.000407000000,
					0.000184000000,
					0.000047000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000,
					0.000000000000
				}, 1
			);

			CIE1964XYZ1Nm10DegZ (double minWaveLength, double[] amplitudes, double nmPerStep) : base(
				minWaveLength, amplitudes, nmPerStep
			) {}
		}
	}
}

