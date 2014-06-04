using UnityEngine;
using System.Collections;
using System;

public class Ease
{
	#region Circular	
	public static float CircEaseOut( float curr, float start, float length)
	{
		float t = curr - 1;
		return length * Mathf.Sqrt( 1 - t * t ) + start;
	}
	public static float CircEaseIn( float curr, float start, float length)
	{
		return -length * ( Mathf.Sqrt( 1 - curr * curr ) - 1 ) + start;
	}
	#endregion
	
	#region Quad
	public static float QuadEaseOut( float curr, float start, float length)
	{
		return -length * curr * ( curr - 2 ) + start;
	}
	public static float QuadEaseIn( float curr, float start, float length)
	{
		return length * curr * curr + start;
	}
	#endregion
	
	#region Cubic
	public static float CubicEaseOut( float curr, float start, float length)
	{
		float t = curr - 1;
		return length * ( t * t * t + 1 ) + start;
	}
	public static float CubicEaseIn( float curr, float start, float length)
	{
		return length * curr * curr * curr + start;
	}
	#endregion
	
	#region Quartic
	public static float QuartEaseIn( float curr, float start, float length)
	{
		return length * curr * curr * curr * curr + start;
	}
	public static float QuartEaseOut( float curr, float start, float length)
	{
		float t = curr - 1;
		return -length * ( t * t * t * t - 1 ) + start;
	}
	#endregion
	
	#region Expo
	public static float ExpoEaseOut(float curr, float start, float length)
	{
		return ( curr == 1.0f ) ? start + length : length * ( -Mathf.Pow( 2, -10 * curr ) + 1 ) + start;
	}
	public static float ExpoEaseIn(float curr, float start, float length)
	{
		return ( curr == 0 ) ? start : length * Mathf.Pow( 2, 10 * ( curr - 1 ) ) + start;
	}
	#endregion
}