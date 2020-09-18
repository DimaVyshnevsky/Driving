using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private Transform _car;
    [SerializeField] private float _achiveGoalDistance = .5f;
    [SerializeField] private float _carSpeed = 3f;

    private Tween _movementTween;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private List<Transform> _boosts;
    private Vector3 _targetPosition;

    public Action<bool> DrivingResultEvent;
    public Action<Transform> AchiveBoostPositionEvent;

    private void Start()
    {
        _startPosition = _car.position;
        _startRotation = _car.rotation;
    }

    public void ResetController()
    {
        _car.position = _startPosition;
        _car.rotation = _startRotation;
    }

    public void Init(Transform targetPosition, List<Transform> boostsPositions)
    {
        _targetPosition = targetPosition.position;
        _boosts = new List<Transform>();

        foreach (var boost in boostsPositions)
        {
            _boosts.Add(boost);
        }
    }

    public void Move(Vector3[] path)
    {
        if (_movementTween != null && _movementTween.IsActive())
        {
            return;
        }

        if (path == null || path.Length == 0)
        {
            return;
        }

        for (int i = 0; i < path.Length; i++)
        {
            path[i].z = transform.position.z;
        }

        _movementTween = _car.DOPath(path, 2, PathType.CatmullRom)
        .SetSpeedBased()
        .SetLookAt(0.01f)
        .SetEase(Ease.Flash)
        .OnComplete(()=>
        {
            bool result = false;
            float distance = Vector2.Distance(transform.position, _targetPosition);

            if (distance <= _achiveGoalDistance)
            {
                result = true;
            }

            DrivingResultEvent?.Invoke(result);
        })
        .OnUpdate(()=>
        {
            int achiveBoostIndex = -1;
            for (int i = 0; i < _boosts.Count; i++)
            {
                float distance = Vector2.Distance(transform.position, _boosts[i].position);

                if (distance <= _achiveGoalDistance)
                {
                    AchiveBoostPositionEvent?.Invoke(_boosts[i]);
                    achiveBoostIndex = i;
                    break;
                }
            }

            if (achiveBoostIndex != -1)
            {
                _boosts.RemoveAt(achiveBoostIndex);
            }
        });
    }
}
