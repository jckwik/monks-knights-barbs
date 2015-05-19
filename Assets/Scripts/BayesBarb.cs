using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public struct Observation
{
	public int knights;			// degrees Farenheit
	public int monks;			// Relative humidity as %
	public int barbs;			// Windy or not
	public bool monastery;		// Play or not
	public bool aggro; 			// Aggro or Defensive
}

public class BayesBarb {

	double sqrt2PI = Math.Sqrt(2.0 * Math.PI);

	List<Observation> obsTab = new List<Observation> ();

	int [] knightSum = new int[2];			// Knights condition (continuous)
	double [] knightMean = new double[2];
	double [] knightStdDev = new double[2];
	int [] knightSumSq = new int[2];
	
	int [] monkSum = new int[2];				// Monks condition (continuous)
	double [] monkMean = new double[2];
	double [] monkStdDev = new double[2];
	int [] monkSumSq = new int[2];
	
	int [] barbSum = new int[2];				// Barbs condition (continuous)
	int [] barbSumSq = new int[2];
	double [] barbMean = new double[2];
	double [] barbStdDev = new double[2];
	
	int [,] monasteryCt = new int[2,2];				// monastery condition (Boolean)
	double [,] monasteryPrp = new double[2,2];
	
	int [] aggroCt = new int[2];					// Play action (Boolean) - aggro or defensive
	double [] aggroPrp = new double[2];

	public BayesBarb() {
	}

	public double CalcBayes(int knights, int monks, int barbs,
	                        bool monastery, bool aggro)
	{
		int playOff = aggro ? 0 : 1;
		double like = GauProb(knightMean[playOff], knightStdDev[playOff], knights) *
				GauProb(monkMean[playOff],monkStdDev[playOff],monks) *
				GauProb(barbMean[playOff],barbStdDev[playOff],barbs) *
				monasteryPrp[monastery?0:1,playOff] *
				aggroPrp[playOff];
		return like;
	}

	public void BuildStats()
	{
		// Accumulate all the counts
		foreach (Observation obs in obsTab)
		{
			// Do this once
			int aggroOff = obs.aggro ? 0 : 1;
			
			knightSum[aggroOff] += obs.knights;
			knightSumSq[aggroOff] += obs.knights*obs.knights;
			
			monkSum[aggroOff] += obs.monks;
			monkSumSq[aggroOff] += obs.monks*obs.monks;
			
			barbSum[aggroOff] += obs.barbs;
			barbSumSq[aggroOff] += obs.barbs*obs.barbs;
			
			monasteryCt[obs.monastery?0:1,aggroOff]++;
			
			aggroCt[aggroOff]++;
		}
		
		// Calculate the statistics

		knightMean[0] = Mean(knightSum[0],aggroCt[0]);
		knightMean[1] = Mean(knightSum[1],aggroCt[1]);
		knightStdDev[0] = StdDev(knightSumSq[0],knightSum[0],aggroCt[0]);
		knightStdDev[1] = StdDev(knightSumSq[1],knightSum[1],aggroCt[1]);

		monkMean[0] = Mean(monkSum[0],aggroCt[0]);
		monkMean[1] = Mean(monkSum[1],aggroCt[1]);
		monkStdDev[0] = StdDev(monkSumSq[0],monkSum[0],aggroCt[0]);
		monkStdDev[1] = StdDev(monkSumSq[1],monkSum[1],aggroCt[1]);
		
		barbMean[0] = Mean(barbSum[0],aggroCt[0]);
		barbMean[1] = Mean(barbSum[1],aggroCt[1]);
		barbStdDev[0] = StdDev(barbSumSq[0],barbSum[0],aggroCt[0]);
		barbStdDev[1] = StdDev(barbSumSq[1],barbSum[1],aggroCt[1]);
		
		CalcProps(monasteryCt, aggroCt, monasteryPrp);
		
		aggroPrp[0] = (double)aggroCt[0] / obsTab.Count;
		aggroPrp[1] = (double)aggroCt[1] / obsTab.Count;
	}

	public void ReadObsTab (string fName)
	{
		try {
			using (StreamReader rdr = new StreamReader (fName))
			{
				string lineBuf = null;
				while ((lineBuf = rdr.ReadLine ()) != null)
				{
					string[] lineAra = lineBuf.Split (' ');
					
					// Map strings to correct data types for conditions & action
					// and Add the observation to List obsTab
					AddObs(int.Parse (lineAra[0]), int.Parse(lineAra[1]),
					       int.Parse(lineAra[2]), (lineAra[3] == "True" ? true : false),
					       (lineAra[4] == "True" ? true : false) );
				}
			}
		} catch
		{
			Console.WriteLine ("Problem reading and/or parsing observation file");
			Environment.Exit (-1);
		}
	}

	public void WriteObsTab (string fName)
	{
		try {
			using (StreamWriter rdr = new StreamWriter (fName, false))
			{
				foreach (Observation obs in obsTab) {
					rdr.Write (obs.knights);
					rdr.Write (" " + obs.monks);
					rdr.Write (" " + obs.barbs);
					rdr.Write (" " + obs.monastery);
					rdr.WriteLine (" " + obs.aggro);
				}
			}
		} catch
		{
			Console.WriteLine ("Problem writing and/or parsing observation file");
			Environment.Exit (-1);
		}
	}

	public void AddObs(int knights, int monks, int barbs,
	                   bool monastery, bool aggro)
	{
		// Build an Observation struct
		Observation obs;
		obs.knights = knights;
		obs.monks = monks;
		obs.barbs = barbs;
		obs.monastery = monastery;
		obs.aggro = aggro;
		
		// Add it to the List
		obsTab.Add (obs);
	}

	public void DumpTab ()
	{
		foreach (Observation obs in obsTab)
		{
			Console.Write (obs.knights);
			Console.Write (" " + obs.monks);
			Console.Write (" " + obs.barbs);
			Console.Write (" " + obs.monastery);
			Console.WriteLine (" " + obs.aggro);
		}
	}

	public void CalcProps (int[,] counts, int[] n, double[,] props)
	{
		for (int i = 0; i < counts.GetLength(0); i++)
			for (int j = 0; j < counts.GetLength(1); j++)
				// Detects and corrects a 0 count by assigning a proportion
				// that is 1/10 the size of a proportion for a count of 1
				if (counts[i,j] == 0)
					props[i,j] = 0.1d/aggroCt[j];	// Can't have 0
		else
			props[i,j] = (double)counts[i,j] / n[j];
	}
	
	public double Mean (int sum, int n)
	{
		return (double)sum / n;
	}
	
	public double StdDev(int sumSq, int sum, int n)
	{
		return Math.Sqrt((sumSq - (sum*sum)/(double)n) / (n-1));
	}
	
	// Calculates probability of x in a normal distribution of
	// mean and stdDev.  This corrects a mistake in the pseudo-code,
	// used a power function instead of an exponential.
	public double GauProb (double mean, double stdDev, int x)
	{
		double xMinusMean = x - mean;
		return (1.0d / (stdDev*sqrt2PI)) * 
			Math.Exp(-1.0d*xMinusMean*xMinusMean / (2.0d*stdDev*stdDev));
	}

	public void DumpStats()
	{
		Console.Write ("Knights ");
		Console.Write (knightSum[0]+" "+knightSum[1]+" ");
		Console.Write (knightSumSq[0]+" "+knightSumSq[1]+" ");
		Console.Write (knightMean[0]+" "+knightMean[1]+" ");
		Console.WriteLine (knightStdDev[0]+" "+knightStdDev[1]);
		
		Console.Write ("Monks ");
		Console.Write (monkSum[0]+" "+monkSum[1]+" ");
		Console.Write (monkSumSq[0]+" "+monkSumSq[1]+" ");
		Console.Write (monkMean[0]+" "+monkMean[1]+" ");
		Console.WriteLine (monkStdDev[0]+" "+monkStdDev[1]);
		
		Console.Write ("Barbs ");
		Console.Write (barbSum[0]+" "+barbSum[1]+" ");
		Console.Write (barbSumSq[0]+" "+barbSumSq[1]+" ");
		Console.Write (barbMean[0]+" "+barbMean[1]+" ");
		Console.WriteLine (barbStdDev[0]+" "+barbStdDev[1]);
		
		Console.Write ("Monastery ");
		for (int i = 0; i < 2; i++)
			for (int j = 0; j < 2; j++)
				Console.Write(monasteryCt[i,j]+" "+monasteryPrp[i,j]+" ");
		Console.WriteLine ();
		
		Console.Write ("Aggro ");
		Console.Write (aggroCt[0]+" "+aggroPrp[0]+" ");
		Console.WriteLine (aggroCt[1]+" "+aggroPrp[1]);
	}

}
