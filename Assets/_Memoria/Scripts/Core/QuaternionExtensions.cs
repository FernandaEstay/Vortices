using System;
using UnityEngine;

namespace Memoria.Core
{
	public static class QuaternionExtensions
	{
		public static bool CompareQuaternion(this Quaternion quaternionA, Quaternion quaternionB, float tolerance)
		{
			if (Math.Abs(Math.Abs(quaternionA.x) - Math.Abs(quaternionB.x)) < tolerance &&
				Math.Abs(Math.Abs(quaternionA.y) - Math.Abs(quaternionB.y)) < tolerance &&
				Math.Abs(Math.Abs(quaternionA.z) - Math.Abs(quaternionB.z)) < tolerance)
			{
				return Math.Abs(quaternionA.w - quaternionB.w) < tolerance;
			}

			return false;
		}

		public static bool EqualOrMayorCompareQuaternion(this Quaternion quaternionA, Quaternion quaternionB, float tolerance)
		{
			if (Math.Abs(quaternionA.x) - Math.Abs(quaternionB.x) >= tolerance &&
				Math.Abs(quaternionA.y) - Math.Abs(quaternionB.y) >= tolerance &&
				Math.Abs(quaternionA.z) - Math.Abs(quaternionB.z) >= tolerance
				)
			{
				return Math.Abs(quaternionA.w) - Math.Abs(quaternionB.w) >= tolerance;
			}

			return false;
		}
	}
}
