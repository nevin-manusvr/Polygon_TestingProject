using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace Manus.Polygon
{

	public class ErrorHandler : MonoBehaviour
	{
		public static void LogError(ErrorMessage message, [CallerFilePathAttribute] string file = null, [CallerMemberName] string method = null, [CallerLineNumber] int lineNumber = 0)
		{
			char[] split = { '\\' };
			string[] splittedString = file.Split(split);
			string fileName = splittedString[splittedString.Length - 1];

			switch (message)
			{
				case ErrorMessage.NoTrackerData:
					Debug.LogError($"Tracker data not found at {fileName} in {method}() on line {lineNumber}");
					break;
				case ErrorMessage.NoRequiredData:
					Debug.LogError($"Required data not available at {fileName} in {method}() on line {lineNumber}");
					break;
				case ErrorMessage.NotImplemented:
					Debug.LogError($"Something is not implemented at {fileName} in {method}() on line {lineNumber}");
					break;
				default:
					LogError(ErrorMessage.NotImplemented);
					break;
			}
		}
	}

	public enum ErrorMessage
	{
		NoTrackerData,
		NoRequiredData,
		NotImplemented
	}
}