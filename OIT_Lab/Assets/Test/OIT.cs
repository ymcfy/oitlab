using UnityEngine;
using System.Collections;

namespace OIT {

	[ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class OIT : MonoBehaviour {
		public const string KW_PREMULT_ON = "PREMULTIPLIED_ALPHA_ON";
		public const string KW_PREMULT_OFF = "PREMULTIPLIED_ALPHA_OFF";

		public enum TransparentModeEnum { PremultAlpha = 0, NotPremultAlpha }
		public enum UIModeEnum { Hidden = 0, Visible }

		public TransparentModeEnum transparentMode;
		public UIModeEnum uiMode;
        /// <summary>
        /// 是否显示左上角UI
        /// </summary>
		public KeyCode uikey = KeyCode.T;
    	public Shader accumShader;
    	public Shader revealageShader;
    	public Shader postEffectShader;
        /// <summary>
        /// 开启OIT
        /// </summary>
    	public bool	oitEnabled;
        /// <summary>
        /// 开启WBOIT
        /// </summary>
    	public bool weightEnabled;
        /// <summary>
        /// 权重
        /// </summary>
		public float weight = -5f;
        /// <summary>
        /// 只渲染第0层，Default
        /// </summary>
		public LayerMask _layerOpaque = 1 << 0;
        /// <summary>
        /// 只渲染第1层，TransparentFX
        /// </summary>
		public LayerMask _layerTransparent = 1 << 1;

        /// <summary>
        /// 当前摄像机
        /// </summary>
        Camera _attachedCam;
        /// <summary>
        /// 主摄像机的子相机
        /// </summary>
    	Camera _renderCam;
        /// <summary>
        /// 子相机物体
        /// </summary>
    	GameObject _renderGO;

        /// <summary>
        /// 不透明纹理贴图
        /// </summary>
    	RenderTexture _opaqueTex;
    	RenderTexture _accumTex;
    	RenderTexture _revealageTex;

        /// <summary>
        /// 后处理材质球
        /// </summary>
    	Material _postEffectMat;
        public enum WeightFunction { Weight0 = 0, Weight1, Weight2 , Weight3 }
        public WeightFunction weightFunction = WeightFunction.Weight0;

        void Awake() {
            //当前摄像机
            _attachedCam = GetComponent<Camera>();
			if (_renderGO == null) {
	    		_renderGO = new GameObject();
                //对象不会保存到场景中，当一个新的场景创建的时候也不会被销毁~
                _renderGO.hideFlags = HideFlags.DontSave;
                //作为当前摄像机的子物体
				_renderGO.transform.SetParent(transform, false);
	    		_renderCam = _renderGO.AddComponent<Camera>();
                //使这个相机的设置与主相机相同。
                _renderCam.CopyFrom(_attachedCam);
                //关闭这个子物体
	    		_renderCam.enabled = false;
                //不会对屏幕(缓存帧)指定区域进行清除
                _renderCam.clearFlags = CameraClearFlags.Nothing;
			}
			if (_postEffectMat == null) {
                //新建一个材质球，带后处理shader
    			_postEffectMat = new Material(postEffectShader);
                //对象不会保存到场景中，当一个新的场景创建的时候也不会被销毁~
                _postEffectMat.hideFlags = HideFlags.DontSave;
			}
            //权重取0和指定值的最小值，即让权重始终小于等于0
			weight = Mathf.Min(0f, weight);
            //如果开启了权重，则传递权重值，如果未开启，传递0
            //在OITAccum Shader里用到
			Shader.SetGlobalFloat("_Weight", (weightEnabled ? weight : 0f));
    	}

		void OnDestroy() {
            //销毁子相机
			DestroyImmediate(_renderGO);
            //销毁后处理材质球
			DestroyImmediate(_postEffectMat);
		}

        /// <summary>
        /// OnPreRender在相机开始渲染场景之前调用
        /// </summary>
    	void OnPreRender() {
            //屏幕宽度
            var width = Screen.width;
            //屏幕高度
            var height = Screen.height;
            //分配一个临时的渲染纹理 不透明纹理
            _opaqueTex = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            //累积纹理
    		_accumTex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
            _revealageTex = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.RHalf, RenderTextureReadWrite.Linear);

            //判断当前PREMULTIPLIED_ALPHA打开还是关闭
            switch (transparentMode) {
			case TransparentModeEnum.NotPremultAlpha:
				Shader.DisableKeyword(KW_PREMULT_ON);
				Shader.EnableKeyword(KW_PREMULT_OFF);
				break;
			case TransparentModeEnum.PremultAlpha:
				Shader.DisableKeyword(KW_PREMULT_OFF);
				Shader.EnableKeyword(KW_PREMULT_ON);
				break;
			}

            //首先渲染不透明物体
    		_renderCam.targetTexture = _opaqueTex;
            //主摄像机背景色
            _renderCam.backgroundColor =_attachedCam.backgroundColor;
            //主摄像机的clearFlags
            _renderCam.clearFlags = _attachedCam.clearFlags;
            //只渲染第0层，Default
            _renderCam.cullingMask = ~(1 << LayerMask.NameToLayer("Transparent"));
            //进行渲染
    		_renderCam.Render();

            //目标纹理改为_accumTex
            _renderCam.targetTexture = _accumTex;
            //设置背景色
    		_renderCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
            //Camera 在渲染每一帧前，逐像素过程中会对每个像素进行深度值与颜色值的 Clear，
            //然后在将需要渲染的颜色值与深度值(如有需要)写入
            _renderCam.clearFlags = CameraClearFlags.SolidColor;
            //任何层都不渲染
            _renderCam.cullingMask = 0;
            //渲染
    		_renderCam.Render();
            //将摄像机设置为渲染到一个或多个 RenderTexture 的选定缓冲区
            _renderCam.SetTargetBuffers(_accumTex.colorBuffer, _opaqueTex.depthBuffer);
    		_renderCam.clearFlags = CameraClearFlags.Nothing;
            //只渲染透明层
    		_renderCam.cullingMask = _layerTransparent;
            //使用accumShader进行渲染
            _renderCam.RenderWithShader(accumShader, null);

    		_renderCam.targetTexture = _revealageTex;
    		_renderCam.backgroundColor = new Color(1f, 1f, 1f, 1f);
    		_renderCam.clearFlags = CameraClearFlags.SolidColor;
    		_renderCam.cullingMask = 0;
    		_renderCam.Render();

    		_renderCam.SetTargetBuffers(_revealageTex.colorBuffer, _opaqueTex.depthBuffer);
    		_renderCam.clearFlags = CameraClearFlags.Nothing;
    		_renderCam.cullingMask = _layerTransparent ;
    		_renderCam.RenderWithShader(revealageShader, null);
    	}
    	
    	void OnRenderImage(RenderTexture src, RenderTexture dst) {
    		if (!oitEnabled) {
    			Graphics.Blit(src, dst);
    			return;
    		}
            switch (weightFunction)
            {
                case WeightFunction.Weight0:
                    Shader.EnableKeyword("_WEIGHTED0");
                    Shader.DisableKeyword("_WEIGHTED1");
                    Shader.DisableKeyword("_WEIGHTED2");
                    Shader.DisableKeyword("_WEIGHTED3");
                    break;
                case WeightFunction.Weight1:
                    Shader.EnableKeyword("_WEIGHTED1");
                    Shader.DisableKeyword("_WEIGHTED0");
                    Shader.DisableKeyword("_WEIGHTED2");
                    Shader.DisableKeyword("_WEIGHTED3");
                    break;
                case WeightFunction.Weight2:
                    Shader.EnableKeyword("_WEIGHTED2");
                    Shader.DisableKeyword("_WEIGHTED0");
                    Shader.DisableKeyword("_WEIGHTED1");
                    Shader.DisableKeyword("_WEIGHTED3");
                    break;
                case WeightFunction.Weight3:
                    Shader.EnableKeyword("_WEIGHTED3");
                    Shader.DisableKeyword("_WEIGHTED0");
                    Shader.DisableKeyword("_WEIGHTED1");
                    Shader.DisableKeyword("_WEIGHTED2");
                    break;
            }
            _postEffectMat.SetTexture("_AccumTex", _accumTex);
    		_postEffectMat.SetTexture("_RevealageTex", _revealageTex);
    		Graphics.Blit(_opaqueTex, dst, _postEffectMat);
    	}

    	void OnPostRender() {
    		RenderTexture.ReleaseTemporary(_opaqueTex);
    		RenderTexture.ReleaseTemporary(_accumTex);
    		RenderTexture.ReleaseTemporary(_revealageTex);
    	}
		void Update() {
			if (Input.GetKeyDown(uikey)) {
				uiMode = (UIModeEnum)(((int)uiMode + 1) % 2);
			}
		}

    	void OnGUI() {
			//if (uiMode == UIModeEnum.Hidden)
			//	return;
			
   // 		GUILayout.BeginVertical();
   // 		oitEnabled = GUILayout.Button(string.Format("OIT {0}", (oitEnabled ? "Enabled" : "Disabled"))) == true ? !oitEnabled : oitEnabled;
   // 		weightEnabled = GUILayout.Button(string.Format("Weight {0}", (weightEnabled ? "Enabled" : "Disabled"))) == true ? !weightEnabled : weightEnabled;
   // 		weight = Mathf.Min(0f, weight);
			//Shader.SetGlobalFloat("_Weight", (weightEnabled ? weight : 0f));
   // 		GUILayout.EndVertical();
    	}
    }
}