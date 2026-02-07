using UnityEngine;
using UnityEngine.Rendering;

[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class NGSS_ContactShadows : MonoBehaviour
{
	public Light mainDirectionalLight;

	public Shader contactShadowsShader;

	[Header("SHADOWS SETTINGS")]
	[Tooltip("Poisson Noise. Randomize samples to remove repeated patterns.")]
	public bool m_noiseFilter;

	[Tooltip("Tweak this value to remove soft-shadows leaking around edges.")]
	[Range(0.01f, 1f)]
	public float m_shadowsEdgeTolerance = 0.25f;

	[Tooltip("Overall softness of the shadows.")]
	[Range(0.01f, 1f)]
	public float m_shadowsSoftness = 0.25f;

	[Tooltip("Overall distance of the shadows.")]
	[Range(1f, 4f)]
	public float m_shadowsDistance = 1f;

	[Tooltip("The distance where shadows start to fade.")]
	[Range(0.1f, 4f)]
	public float m_shadowsFade = 1f;

	[Tooltip("Tweak this value if your objects display backface shadows.")]
	[Range(0f, 2f)]
	public float m_shadowsOffset = 0.325f;

	[Header("RAY SETTINGS")]
	[Tooltip("The higher the value, the ticker the shadows will look.")]
	[Range(0f, 1f)]
	public float m_rayWidth = 0.1f;

	[Tooltip("Number of samplers between each step. The higher values produces less gaps between shadows. Keep this value as low as you can!")]
	[Range(16f, 128f)]
	public int m_raySamples = 64;

	[Tooltip("Samplers scale over distance. Lower this value if you want to speed things up by doing less sampling on far away objects.")]
	[Range(0f, 1f)]
	public float m_raySamplesScale = 1f;

	private Camera _mCamera;

	private Material _mMaterial;

	private CommandBuffer blendShadowsCB;

	private CommandBuffer computeShadowsCB;

	private bool isInitialized;

	private Camera mCamera
	{
		get
		{
			if (_mCamera == null)
			{
				_mCamera = GetComponent<Camera>();
				if (_mCamera == null)
				{
					_mCamera = Camera.main;
				}
				if (_mCamera == null)
				{
					Debug.LogError("NGSS Error: No MainCamera found, please provide one.", this);
				}
				else
				{
					_mCamera.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
			return _mCamera;
		}
	}

	private Material mMaterial
	{
		get
		{
			if (_mMaterial == null)
			{
				if (contactShadowsShader == null)
				{
					Shader.Find("Hidden/NGSS_ContactShadows");
				}
				_mMaterial = new Material(contactShadowsShader);
				if (_mMaterial == null)
				{
					Debug.LogWarning("NGSS Warning: can't find NGSS_ContactShadows shader, make sure it's on your project.", this);
					base.enabled = false;
					return null;
				}
			}
			return _mMaterial;
		}
	}

	private void OnEnable()
	{
		if (IsNotSupported())
		{
			Debug.LogWarning("Unsupported graphics API, NGSS requires at least SM3.0 or higher and DX9 is not supported.", this);
			base.enabled = false;
		}
		else
		{
			Init();
		}
	}

	private void OnDisable()
	{
		if (isInitialized)
		{
			RemoveCommandBuffers();
		}
	}

	private void OnApplicationQuit()
	{
		if (isInitialized)
		{
			RemoveCommandBuffers();
		}
	}

	private void OnPreRender()
	{
		Init();
		if (isInitialized && !(mainDirectionalLight == null))
		{
			mMaterial.SetVector("LightDir", mCamera.transform.InverseTransformDirection(mainDirectionalLight.transform.forward));
			mMaterial.SetFloat("ShadowsOpacity", 1f - mainDirectionalLight.shadowStrength);
			mMaterial.SetFloat("ShadowsEdgeTolerance", m_shadowsEdgeTolerance * 0.075f);
			mMaterial.SetFloat("ShadowsSoftness", m_shadowsSoftness * 4f);
			mMaterial.SetFloat("ShadowsDistance", m_shadowsDistance);
			mMaterial.SetFloat("ShadowsFade", m_shadowsFade);
			mMaterial.SetFloat("ShadowsBias", m_shadowsOffset * 0.02f);
			mMaterial.SetFloat("RayWidth", m_rayWidth);
			mMaterial.SetFloat("RaySamples", m_raySamples);
			mMaterial.SetFloat("RaySamplesScale", m_raySamplesScale);
			if (m_noiseFilter)
			{
				mMaterial.EnableKeyword("NGSS_CONTACT_SHADOWS_USE_NOISE");
			}
			else
			{
				mMaterial.DisableKeyword("NGSS_CONTACT_SHADOWS_USE_NOISE");
			}
		}
	}

	private void AddCommandBuffers()
	{
		computeShadowsCB = new CommandBuffer
		{
			name = "NGSS ContactShadows: Compute"
		};
		blendShadowsCB = new CommandBuffer
		{
			name = "NGSS ContactShadows: Mix"
		};
		bool flag = mCamera.actualRenderingPath == RenderingPath.Forward;
		if ((bool)mCamera)
		{
			CommandBuffer[] commandBuffers = mCamera.GetCommandBuffers(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting);
			foreach (CommandBuffer commandBuffer in commandBuffers)
			{
				if (commandBuffer.name == computeShadowsCB.name)
				{
					return;
				}
			}
			mCamera.AddCommandBuffer(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting, computeShadowsCB);
		}
		if (!mainDirectionalLight)
		{
			return;
		}
		CommandBuffer[] commandBuffers2 = mainDirectionalLight.GetCommandBuffers(LightEvent.AfterScreenspaceMask);
		foreach (CommandBuffer commandBuffer2 in commandBuffers2)
		{
			if (commandBuffer2.name == blendShadowsCB.name)
			{
				return;
			}
		}
		mainDirectionalLight.AddCommandBuffer(LightEvent.AfterScreenspaceMask, blendShadowsCB);
	}

	private void RemoveCommandBuffers()
	{
		_mMaterial = null;
		bool flag = mCamera.actualRenderingPath == RenderingPath.Forward;
		if ((bool)mCamera)
		{
			mCamera.RemoveCommandBuffer(flag ? CameraEvent.AfterDepthTexture : CameraEvent.BeforeLighting, computeShadowsCB);
		}
		if ((bool)mainDirectionalLight)
		{
			mainDirectionalLight.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, blendShadowsCB);
		}
		isInitialized = false;
	}

	private void Init()
	{
		if (!isInitialized && !(mainDirectionalLight == null))
		{
			if (mCamera.actualRenderingPath == RenderingPath.VertexLit)
			{
				Debug.LogWarning("Vertex Lit Rendering Path is not supported by NGSS Contact Shadows. Please set the Rendering Path in your game camera or Graphics Settings to something else than Vertex Lit.", this);
				base.enabled = false;
				return;
			}
			AddCommandBuffers();
			int num = Shader.PropertyToID("NGSS_ContactShadowRT");
			int num2 = Shader.PropertyToID("NGSS_ContactShadowRT2");
			int num3 = Shader.PropertyToID("NGSS_DepthSourceRT");
			computeShadowsCB.GetTemporaryRT(num, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
			computeShadowsCB.GetTemporaryRT(num2, -1, -1, 0, FilterMode.Bilinear, RenderTextureFormat.R8);
			computeShadowsCB.GetTemporaryRT(num3, -1, -1, 0, FilterMode.Point, RenderTextureFormat.RFloat);
			computeShadowsCB.Blit(num, num3, mMaterial, 0);
			computeShadowsCB.Blit(num3, num, mMaterial, 1);
			computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(0f, 1f));
			computeShadowsCB.Blit(num, num2, mMaterial, 2);
			computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(1f, 0f));
			computeShadowsCB.Blit(num2, num, mMaterial, 2);
			computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(0f, 2f));
			computeShadowsCB.Blit(num, num2, mMaterial, 2);
			computeShadowsCB.SetGlobalVector("ShadowsKernel", new Vector2(2f, 0f));
			computeShadowsCB.Blit(num2, num, mMaterial, 2);
			computeShadowsCB.SetGlobalTexture("NGSS_ContactShadowsTexture", num);
			blendShadowsCB.Blit(BuiltinRenderTextureType.CurrentActive, BuiltinRenderTextureType.CurrentActive, mMaterial, 3);
			isInitialized = true;
		}
	}

	private bool IsNotSupported()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStationVita || SystemInfo.graphicsDeviceType == GraphicsDeviceType.N3DS;
	}
}
