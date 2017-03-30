using System;
using UnityEngine;

namespace Memoria.Core
{
	public static class VectorExtensions
	{
		public static bool CompareVector(this Vector3 vectorA, Vector3 vectorB, float tolerance)
		{
			if (Math.Abs(vectorA.x - vectorB.x) < tolerance &&
				Math.Abs(vectorA.y - vectorB.y) < tolerance)
			{
				return Math.Abs(vectorA.z - vectorB.z) < tolerance;
			}

			return false;
		}

		public static bool EqualOrMayorCompareVector(this Vector3 vectorA, Vector3 vectorB, float tolerance)
		{
			if (Math.Abs(vectorA.x) - Math.Abs(vectorB.x) >= tolerance &&
				Math.Abs(vectorA.y) - Math.Abs(vectorB.y) >= tolerance
				)
			{
				return Math.Abs(vectorA.z) - Math.Abs(vectorB.z) >= tolerance;
			}

			return false;
		}

		public static bool EqualOrMinorCompareVector(this Vector3 vectorA, Vector3 vectorB, float tolerance)
		{
			if (Math.Abs(vectorA.x) - Math.Abs(vectorB.x) <= tolerance &&
				Math.Abs(vectorA.y) - Math.Abs(vectorB.y) <= tolerance
				)
			{
				return Math.Abs(vectorA.z) - Math.Abs(vectorB.z) <= tolerance;
			}

			return false;
		}
	}
}
