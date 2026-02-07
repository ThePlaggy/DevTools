using System;
using UnityEngine;

namespace MirzaBeig.Shaders.ImageEffects;

[Serializable]
[ExecuteInEditMode]
public class IEBase : MonoBehaviour
{
	private Camera _camera;

	private Material _material;

	protected Material material
	{
		get
		{
			if (!_material)
			{
				_material = new Material(shader);
				_material.hideFlags = HideFlags.HideAndDontSave;
			}
			return _material;
		}
	}

	protected Shader shader { get; set; }

	protected Camera camera
	{
		get
		{
			if (!_camera)
			{
				_camera = GetComponent<Camera>();
			}
			return _camera;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
		if ((bool)_material)
		{
			UnityEngine.Object.DestroyImmediate(_material);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
	}

	protected void blit(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination, material);
	}
}
