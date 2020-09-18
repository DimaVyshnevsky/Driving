using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : BaseDrawBehavior
{
    [SerializeField] private float _pointsDistance = .5f;

    private Vector3 _previousPosition;
    private List<Vector3> _path = new List<Vector3>();

    public override void StartGame()
    {
        _state = true;
    }

    public override void ResetController()
    {
        base.ResetController();
        _path.Clear();
    }

    public List<Vector3> GetPath()
    {
        return _path;
    }

    protected override void Reaction()
    {
        Vector3 mouseClickPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (Vector3.Distance(_previousPosition, mouseClickPosition) > _pointsDistance)
        {
            _previousPosition = mouseClickPosition;
            _path.Add(mouseClickPosition);
        }
    }
}
