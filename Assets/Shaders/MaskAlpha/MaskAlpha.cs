using System;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class MaskAlpha : BaseDrawBehavior
{
    [SerializeField]private GameObject _dust;
    [SerializeField]private Material _eraserMaterial;
    [SerializeField]private int _widthMask = 110;
    [SerializeField]private int _heightMask = 110;
    [SerializeField]private float _percentShow = 0.99f;

    private Camera _maskCamera;
    private bool _clearGL, _imageShown, _imageShowing;
    private Rect _screenRect;
    private Vector2? _newHolePosition;
    private float _showingTime = 1f;
    private float _currentTime;
    private RenderTexture _renderTexture;
    private Renderer _dustRenderer;

    public Action CleaningEnd;
    public Action OnMaskStart;

    void Awake()
    {
        _maskCamera = GetComponent<Camera>();
        _dustRenderer = _dust.GetComponent<Renderer>();
    }

    [ContextMenu("Start")]
    public override void StartGame()
    {
        _maskCamera.enabled = true;
        _renderTexture = _maskCamera.targetTexture;
        Bounds bounds = _dustRenderer.bounds;
        _screenRect.x = bounds.min.x;
        _screenRect.y = bounds.min.y;
        _screenRect.width = bounds.size.x;
        _screenRect.height = bounds.size.y;

        _dustRenderer.material.SetFloat("_Alpha", 1.0f);
        _dustRenderer.material.SetTexture("_MaskTex", _renderTexture);

        _imageShowing = false;
        _imageShown = false;
        _clearGL = true;
        _newHolePosition = null;
        _state = true;
    }

    public void Release()
    {
        _maskCamera.enabled = false;
        _imageShowing = false;
        _imageShown = false;
    }

    public void ShowImage()
    {
        _imageShowing = true;
        _imageShown = false;
        _currentTime = 0;
    }

    protected override void Reaction()
    {
        _newHolePosition = null;

        if (!_imageShowing && !_imageShown)
        {
            Vector2 v = _camera.ScreenToWorldPoint(Input.mousePosition);                                                   
            Rect worldRect = _screenRect;

            if (worldRect.Contains(v))
            {
                _newHolePosition = new Vector2(_widthMask * (v.x - worldRect.xMin) / worldRect.width, _heightMask * (v.y - worldRect.yMin) / worldRect.height);
            }
        }
    }

    public void Update()
    {
        if (_imageShowing)
        {
            _currentTime += Time.deltaTime;
            _dustRenderer.material.SetFloat("_Alpha", Mathf.Lerp(1.0f, 0.0f, _currentTime / _showingTime));

            if (_currentTime >= _showingTime)
            {
                _imageShowing = false;
                _imageShown = true;

                CleaningEnd?.Invoke();
            }
        }
    }

    private float CleaningPercent(Texture2D texture)
    {
        float p = 0;

        for (int i = 0; i < _widthMask; i++)
        {
            for (int j = 0; j < _heightMask; j++)
            {
                if (texture.GetPixel(i, j).a != 0)
                {
                    p++;
                }
            }
        }

        return p / (float)(_widthMask * _heightMask);
    }

    public void OnPostRender()
    {
        if (!_imageShown && !_imageShowing)
        {
            if (_clearGL)
            {
                _clearGL = false;
                GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
            }

            if (_newHolePosition != null)
            {
                Texture2D tex = CutHole(new Vector2(_widthMask, _heightMask), _newHolePosition.Value);

                if (CleaningPercent(tex) >= _percentShow)
                {
                    ShowImage();
                }

                Destroy(tex);
            }
        }
    }

    private Texture2D CutHole(Vector2 imageSize, Vector2 imageLocalPosition)
    {
        Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        Rect positionRect = new Rect(
            (imageLocalPosition.x - 0.5f * _eraserMaterial.mainTexture.width) / imageSize.x,
            (imageLocalPosition.y - 0.5f * _eraserMaterial.mainTexture.height) / imageSize.y,
            _eraserMaterial.mainTexture.width / imageSize.x,
            _eraserMaterial.mainTexture.height / imageSize.y
        );
        GL.PushMatrix();
        GL.LoadOrtho();

        for (int i = 0; i < _eraserMaterial.passCount; i++)
        {
            _eraserMaterial.SetPass(i);
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            GL.TexCoord2(textureRect.xMin, textureRect.yMax);
            GL.Vertex3(positionRect.xMin, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMax);
            GL.Vertex3(positionRect.xMax, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMin);
            GL.Vertex3(positionRect.xMax, positionRect.yMin, 0.0f);
            GL.TexCoord2(textureRect.xMin, textureRect.yMin);
            GL.Vertex3(positionRect.xMin, positionRect.yMin, 0.0f);

            GL.End();
        }

        GL.PopMatrix();
        RenderTexture.active = _renderTexture;
        Texture2D newTexture = new Texture2D(_widthMask, _heightMask);
        newTexture.ReadPixels(new Rect(0, 0, _widthMask, _heightMask), 0, 0);
        bool applyMipsmaps = false;
        newTexture.Apply(applyMipsmaps);
        bool highQuality = false;
        newTexture.Compress(highQuality);
        RenderTexture.active = null;

        return newTexture;
    }
}

