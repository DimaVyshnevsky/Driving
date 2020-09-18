using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Drawer : BaseDrawBehavior
{
    [SerializeField] private Color _paintColor;
    [SerializeField] private int _radius;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private float _drawRectangleDistance;

    private SpriteRenderer _spriteRenderer;
    private Texture2D _texture;
    private Vector2 _size, _previousPosition;
    private Collider2D _collider2D;
    private int _x, _y, _startX, _startY, _finishX, _finishY;
    private float _width, _height, _left, _right, _top, _bot;
    private Color[] _startTexture;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        Texture2D texture = _spriteRenderer.sprite.texture;
        _startTexture = texture.GetPixels();
    }

    [ContextMenu("Start")]
    public override void StartGame()
    {
        CreateNewTexture();
        SpriteSize();
        _state = true;
    }

    [ContextMenu("Reset")]
    public override void ResetController()
    {
        base.ResetController();
        _texture.SetPixels(_startTexture);
        _texture.Apply();
    }

    private void CreateNewTexture()
    {
        Texture2D texture = _spriteRenderer.sprite.texture;
        _texture = new Texture2D(texture.width, texture.height);
        _texture.SetPixels(texture.GetPixels());
        _size = new Vector2(texture.width, texture.height);
        _spriteRenderer.sprite = Sprite.Create(_texture, new Rect(Vector2.zero, new Vector2(_size.x, _size.y)), new Vector2(0.5f, 0.5f));

        _texture.Apply();
    }

    private void SpriteSize()
    {
        _width = _collider2D.bounds.size.x * transform.localScale.x;// * transform.parent.localScale.x;
        _height = _collider2D.bounds.size.y * transform.localScale.y;// * transform.parent.localScale.y;
        _left = _collider2D.bounds.center.x - _width / 2f;
        _right = _collider2D.bounds.center.x + _width / 2f;
        _top = _collider2D.bounds.center.y + _height / 2f;
        _bot = _collider2D.bounds.center.y - _height / 2f;
    }

    private bool ChekColorOfGroupPixels()
    {
        _startX = _x - _radius;
        _startY = _y - _radius;
        _finishX = _x + _radius;
        _finishY = _y + _radius;
       
        if (_startX < 0) 
            _startX = 0;
        
        if (_startY < 0) 
            _startY = 0;
       
        if (_finishX > _texture.width)
            _finishX = _texture.width - 1;
        
        if (_finishY > _texture.height) 
            _finishY = _texture.height - 1;
        
        Color _color;

        for (int i = _startX; i < _finishX; i++)
        {
            for (int j = _startY; j < _finishY; j++)
            {
                _color = _texture.GetPixel(i, j);

                if (_color.a > 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void ChangeGroupPixels()
    {
        for (int i = _startX; i < _finishX; i++)
        {
            for (int j = _startY; j < _finishY; j++)
            {
                if ((i - _x) * (i - _x) + (j - _y) * (j - _y) <= _radius * _radius)
                {
                    if (ChekColorOfPixel(_texture.GetPixel(i, j)))
                    {
                        _texture.SetPixel(i, j, _paintColor);
                    }
                }
            }
        }

        _texture.Apply();
    }

    private bool ChekColorOfPixel(Color _color)
    {
        if (_color.a > 0)
        {
            return true;
        }

        return false;
    }


    protected override void Reaction()
    {
        Vector2 mouseClickPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        _x = Mathf.RoundToInt(_texture.width / (_right - _left) * (mouseClickPosition.x - _left)) - 1;
        _y = Mathf.RoundToInt(_texture.height / (_top - _bot) * (mouseClickPosition.y - _bot)) - 1;
       
        _x += (int)_offset.x;
        _y += (int)_offset.y;
       
        if (_x < 0) 
            _x = 0;
       
        if (_y < 0)
            _y = 0;
        
        if (_x >= _texture.width) 
            _x = _texture.width - 1;
       
        if (_y >= _texture.height) 
            _y = _texture.height - 1;

        if (ChekColorOfGroupPixels())
        {
            ChangeGroupPixels();
        }
    }
}