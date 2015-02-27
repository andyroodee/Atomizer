using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class Atomizer : MonoBehaviour
{
	[SerializeField] 
    private int renderLayer;	

	void Awake()
	{
		previousLayer = 0;	
		camera.enabled = false;
	}

	private void UpdateRenderCamera()
	{
		camera.CopyFrom(Camera.main);
		camera.targetTexture = null;
		camera.backgroundColor = Color.clear;
		camera.clearFlags = CameraClearFlags.Color;
		camera.cullingMask = 1 << renderLayer;
		camera.renderingPath = RenderingPath.Forward;
	}

	private void EffectBuildComplete()
	{
		effectGroup.OnFinish += onFinished;
		effectGroup.Play();
	}

	public static AtomizerEffectGroup CreateEffectGroup()
	{
		GameObject effectPrefab = Resources.Load<GameObject>("ManualAtomizerEffect");

		GameObject effectGo = GameObject.Instantiate(effectPrefab) as GameObject;
		effectGo.transform.position = Vector3.zero;		
		
		AtomizerEffectGroup effect = effectGo.GetComponent<AtomizerEffectGroup>();
		effect.Initialize();

		return effect;
    }

	public static void Atomize(GameObject target, AtomizerEffectGroup effectGroup, Action OnFinished)
	{
        GameObject atomizerPrefab = Resources.Load<GameObject>("Atomizer");

        Atomizer atomizer = atomizerPrefab.GetComponent<Atomizer>();

        atomizer.effectGroup = effectGroup;
        atomizer.onFinished = OnFinished;
        atomizer.target = target;
        atomizer.UpdateRenderCamera();
        atomizer.previousLayer = target.layer;
        LayerHelper.SetLayer(target, atomizer.renderLayer);
        atomizer.camera.Render();
	}
	
	private void CleanupOnFailure()
	{
		Destroy(effectGroup);
		LayerHelper.SetLayer(target, previousLayer);	
	}
			
	private void BuildEffect()
	{
		 Snapshot snapshot = null;

        CanvasRenderer canvasRenderer = target.GetComponentInChildren<CanvasRenderer>();
        if (canvasRenderer != null)
        {
            snapshot = Snapshot.GetCanvasObjectSnapshot(target, camera);
        }
        else
        {
		    snapshot = Snapshot.GetObjectSnapshot(target, camera);
        }
		if (snapshot == null)
		{
			CleanupOnFailure();
			if (onFinished != null)
			{
				onFinished();
			}
			return;
		}
		
		effectGroup.Build(snapshot, camera, target);
		
		LayerHelper.SetLayer(target, previousLayer);	
		LayerHelper.SetLayer(effectGroup.gameObject, Camera.main.gameObject.layer);
		
		EffectBuildComplete();
	}

	private	IEnumerator OnPostRender()
	{
		CanvasRenderer canvasRenderer = target.GetComponentInChildren<CanvasRenderer>();
		if (canvasRenderer != null)
		{
			yield return new WaitForEndOfFrame();
		}
		BuildEffect();
		yield return null;
	}

    private Action onFinished;
    private AtomizerEffectGroup effectGroup;
    private int previousLayer;
    private GameObject target;
}