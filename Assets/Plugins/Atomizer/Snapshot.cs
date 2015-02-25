using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Snapshot
{
	public Texture2D Image
	{ get; private set; }
	
	public Rect ScreenRect
	{ get; private set; }

	public Texture2D ColourData
	{ get; private set; }

	public bool ReflectOnYAxis
	{ get; private set; }
	
	public static Snapshot GetObjectSnapshot(GameObject target, Camera camera)
	{
		List<Renderer> renderers = target.GetAllComponents<Renderer>();
		if (renderers.Count == 0 )
		{
			Debug.LogWarning("Atomizer: Target does not have any active Renderer components.");
			return null;
		}
		
		Bounds bounds = GeometryHelper.GetRendererBounds(renderers);
		Rect screenRect = GeometryHelper.GetScreenRect(GeometryHelper.GetWorldPoints(bounds), camera);		
		
		if (screenRect.width < 1 || screenRect.height < 1)
		{
			Debug.LogWarning("Atomizer: Attempting to atomize an off-screen object.");
			return null;
		}

		Texture2D image = new Texture2D(Mathf.FloorToInt(screenRect.width), Mathf.FloorToInt(screenRect.height), TextureFormat.ARGB32, true);
		image.ReadPixels(screenRect, 0, 0);
		
		Snapshot snapshot = new Snapshot();
		snapshot.Image = image;
		snapshot.ScreenRect = screenRect;
		
		return snapshot;
	}

    public static Snapshot GetCanvasObjectSnapshot(GameObject target, Camera camera)
    {
        RectTransform rectTransform = target.GetComponentInChildren<RectTransform>();

        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        Rect screenRect = GeometryHelper.GetScreenRect(worldCorners, camera);

        if (screenRect.width < 1 || screenRect.height < 1)
        {
            Debug.LogWarning("Atomizer: Attempting to atomize an off-screen object.");
            return null;
        }

        Texture2D image = new Texture2D(Mathf.FloorToInt(screenRect.width), Mathf.FloorToInt(screenRect.height), TextureFormat.ARGB32, false);
        image.ReadPixels(screenRect, 0, 0);

        Sprite sprite = target.GetComponent<Image>().sprite;
        Texture2D croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.ARGB32, false);

        Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height);

        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();

        Snapshot snapshot = new Snapshot();
        snapshot.Image = image;
        if (sprite.rect.width > screenRect.width)
        {
            snapshot.ColourData = image;
        }
        else
        {
            snapshot.ColourData = croppedTexture;
        }
        snapshot.ScreenRect = screenRect;

        // Might need to reflect on the Y axis.
        snapshot.ReflectOnYAxis = (Mathf.Approximately(rectTransform.eulerAngles.y, 180.0f));

        return snapshot;
    }
}
