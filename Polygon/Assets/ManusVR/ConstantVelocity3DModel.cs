using System.Collections;
using System.Collections.Generic;

using Accord.Extensions.Math;
using Accord.Extensions.Statistics.Filters;
using Accord.Math;

using UnityEngine;

public class ConstantVelocity3DModel
{
	public const int Dimension = 4;
	public Vector3 Position;
	public Vector3 Velocity;

	public ConstantVelocity3DModel()
	{
		this.Position = new Vector3(0, 0, 0);
		this.Velocity = new Vector3(0, 0, 0);
	}

	public ConstantVelocity3DModel Evaluate(double[,] transitionMat)
	{
		return ConstantVelocity3DModel.FromArray(transitionMat.Multiply(ConstantVelocity3DModel.ToArray(this)));
	}

	public static double[,] GetTransitionMatrix(double timeInterval = 1.0)
	{
		double num = timeInterval;
		return new double[6, 6]
		{
		{
		  1.0,
		  num,
		  0.0,
		  0.0,
		  0.0,
		  0.0
		},
		{
		  0.0,
		  1.0,
		  0.0,
		  0.0,
		  0.0,
		  0.0
		},
		{
		  0.0,
		  0.0,
		  1.0,
		  num,
		  0.0,
		  0.0
		},
		{
		  0.0,
		  0.0,
		  0.0,
		  1.0,
		  0.0,
		  0.0
		},
		{
		  0.0,
		  0.0,
		  0.0,
		  0.0,
		  1.0,
		  num
		},
		{
		  0.0,
		  0.0,
		  0.0,
		  0.0,
		  0.0,
		  1.0
		}
		};
	}

	public static double[,] GetPositionMeasurementMatrix()
	{
		double[,] numArray = new double[3, 6];
		numArray[0, 0] = 1.0;
		numArray[1, 2] = 1.0;
		numArray[2, 4] = 1.0;

		return numArray;
	}

	public static double[,] GetProcessNoise(double accelerationNoise, double timeInterval = 1.0)
	{
		double num1 = timeInterval;
		double[,] numArray1 = new double[6, 3];
		double num2 = num1;
		numArray1[0, 0] = num2 * num2 / 2.0;
		numArray1[1, 0] = num1;
		double num3 = num1;
		numArray1[2, 1] = num3 * num3 / 2.0;
		numArray1[3, 1] = num1;
		double num4 = num1;
		numArray1[4, 2] = num4 * num4 / 2.0;
		numArray1[5, 2] = num1;
		double[,] numArray2 = numArray1;
		double[,] b = Matrix.Diagonal<double>(numArray2.ColumnCount<double>(), accelerationNoise);
		return numArray2.Multiply(b).Multiply(numArray2.Transpose<double>());
	}

	public static ConstantVelocity3DModel FromArray(double[] arr)
	{
		return new ConstantVelocity3DModel()
		{
			Position = new Vector3((float)arr[0], (float)arr[2], (float)arr[4]),
			Velocity = new Vector3((float)arr[1], (float)arr[3], (float)arr[5])
		};
	}

	public static double[] ToArray(ConstantVelocity3DModel modelState)
	{
		return new double[6]
		{
			modelState.Position.x,
			modelState.Velocity.x,
			modelState.Position.y,
			modelState.Velocity.y,
			modelState.Position.z,
			modelState.Velocity.z
		};
	}
}
