using UnityEngine;

public abstract class BaseDrawBehavior : MonoBehaviour
{
    [SerializeField] private Transform _startPosition;
    [SerializeField] private float _radiusStartPosition = 1f;
    [SerializeField] protected Camera _camera;

    protected bool _state;
    protected bool _firstPress;

    public bool FirstPress => _firstPress;

    private void OnMouseDown()
    {
        if (!_firstPress)
        {
            _firstPress = CheckFirstPress();
        }

        if (_state && _firstPress)
            Reaction();
    }

    void OnMouseDrag()
    {
        if (!_firstPress)
        {
            _firstPress = CheckFirstPress();
        }

        if (_state && _firstPress)
            Reaction();
    }

    public void Switch(bool state)
    {
        _state = state;
    }

    private bool CheckFirstPress()
    {
        Vector2 mouseClickPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(_startPosition.position, mouseClickPosition);

        if (distance <= _radiusStartPosition)
        {
            return true;
        }

        return false;
    }

    public virtual void ResetController()
    {
        _firstPress = false;
    }

    protected abstract void Reaction();

    public abstract void StartGame();
}
