using System.Collections;
using System.Collections.Generic;

using Accord.Extensions.Statistics.Filters;
using Accord.Math;

using UnityEngine;

public static class KalmanGenerator
{
	public static DiscreteKalmanFilter<ConstantVelocity3DModel, Vector3> Generate3DimensionalFilter(Vector3 initialPosition, int framerate, float noise, float reliability)
	{
		float frequency = 1f / framerate;

		ConstantVelocity3DModel initialPositionState = new ConstantVelocity3DModel { Position = initialPosition, Velocity = Vector3.zero };
		double[,] initialPositionStateError = ConstantVelocity3DModel.GetProcessNoise(0, frequency);

		var kalman = new DiscreteKalmanFilter<ConstantVelocity3DModel, Vector3>(
			initialPositionState,
			initialPositionStateError,
			3,
			0,
			ConstantVelocity3DModel.ToArray,
			ConstantVelocity3DModel.FromArray,
			x => new double[] { x.x, x.y, x.z });

		kalman.ProcessNoise = ConstantVelocity3DModel.GetProcessNoise(noise, frequency);
		kalman.MeasurementNoise = Matrix.Diagonal<double>(kalman.MeasurementVectorDimension, reliability);

		kalman.MeasurementMatrix = ConstantVelocity3DModel.GetPositionMeasurementMatrix();
		kalman.TransitionMatrix = ConstantVelocity3DModel.GetTransitionMatrix();

		return kalman;
	}
}
