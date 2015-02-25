using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class GeometryHelper
{
	public static Bounds GetRendererBounds(List<Renderer> renderers)
	{								
		if (renderers.Count == 0)
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}
		
		Bounds inclusiveBounds = renderers[0].bounds;
		
		for (int i = 1; i < renderers.Count; i++)
		{
			inclusiveBounds.Encapsulate(renderers[i].bounds);
		}			
		
		return inclusiveBounds;
	}
	
	private static Rect BuildScreenRect(Vector2[] screenPoints)
	{
		List<Vector2> sp = screenPoints.ToList();
		float minX = sp.Min(point => point.x);
		float maxX = sp.Max(point => point.x);
		float minY = sp.Min(point => point.y);
		float maxY = sp.Max(point => point.y);
		
		Rect screenRect = new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
		
		screenRect.xMin = Mathf.Clamp(screenRect.xMin, 1.0f, Screen.width - 1);
		screenRect.xMax = Mathf.Clamp(screenRect.xMax, 1.0f, Screen.width - 1);
		screenRect.yMin = Mathf.Clamp(screenRect.yMin, 1.0f, Screen.height - 1);
		screenRect.yMax = Mathf.Clamp(screenRect.yMax, 1.0f, Screen.height - 1);
		
		return screenRect;
	}
	
	public static Rect GetScreenRect(Vector3[] worldPoints, Camera camera)
	{
		Vector2[] screenPoints = new Vector2[worldPoints.Length];
		for (int i = 0; i < worldPoints.Length; i++)
		{
			screenPoints[i] = camera.WorldToScreenPoint(worldPoints[i]);
		}
		
		return BuildScreenRect(screenPoints);
	}
	
	public static Rect GetScreenRect(Vector2[] worldPoints, Camera camera)
	{
		Vector2[] screenPoints = new Vector2[worldPoints.Length];
		for (int i = 0; i < worldPoints.Length; i++)
		{
			screenPoints[i] = camera.WorldToScreenPoint(worldPoints[i]);
		}
		
		return BuildScreenRect(screenPoints);
	}
	
	public static Vector3[] GetWorldPoints(Bounds bounds)
	{
		Vector3[] worldPoints = new Vector3[8];
		
		Vector3 boundsCenter = bounds.center;
		Vector3 boundsExtents = bounds.extents;
		
		float plusX  = boundsCenter.x + boundsExtents.x;
		float minusX = boundsCenter.x - boundsExtents.x;
		float plusY  = boundsCenter.y + boundsExtents.y;
		float minusY = boundsCenter.y - boundsExtents.y;
		float plusZ  = boundsCenter.z + boundsExtents.z;
		float minusZ = boundsCenter.z - boundsExtents.z;			
		
		worldPoints[0] = new Vector3(plusX,  plusY,  plusZ);
		worldPoints[1] = new Vector3(minusX, plusY,  plusZ);
		worldPoints[2] = new Vector3(plusX,  minusY, plusZ);
		worldPoints[3] = new Vector3(minusX, minusY, plusZ);
		worldPoints[4] = new Vector3(plusX,  plusY,  minusZ);
		worldPoints[5] = new Vector3(minusX, plusY,  minusZ);
		worldPoints[6] = new Vector3(plusX,  minusY, minusZ);
		worldPoints[7] = new Vector3(minusX, minusY, minusZ);
		
		return worldPoints;
	}
}
