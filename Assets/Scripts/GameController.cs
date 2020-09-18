using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    enum GameState
    { 
        None,
        DrawPath,
        Driving,
        Finish
    }

    [SerializeField] private BaseDrawBehavior _drawer;
    [SerializeField] private PathController _pathController;
    [SerializeField] private BoostsController _boostsController;
    [SerializeField] private Car _car;
    [SerializeField] private Transform _targetPosition;
    [SerializeField] private UnityEvent _startGameEvent;
    [SerializeField] private UnityEvent _gameOverWinEvent;
    [SerializeField] private UnityEvent _gameOverLoseEvent;

    private GameState gameState;

    public void Start()
    {
        _car.Init(_targetPosition, _boostsController.GetBoosts());
        _car.DrivingResultEvent += FinishDrivingHandler;
        _car.AchiveBoostPositionEvent += AchiveBoostPositionHandler;
    }

    private void OnDisable()
    {
        _car.DrivingResultEvent -= FinishDrivingHandler;
        _car.AchiveBoostPositionEvent -= AchiveBoostPositionHandler;
    }

    void OnMouseUp()
    {
        if (gameState == GameState.DrawPath && _drawer.FirstPress)
        {
            TryStartDriving();
        }
    }

    public void StartGame()
    {
        _drawer.StartGame();
        _pathController.StartGame();
        _boostsController.StartGame();
        _startGameEvent?.Invoke();

        gameState = GameState.DrawPath;
    }

    public void Restart()
    {
        ResetController();
        StartGame();
    }

    private void ResetController()
    {
        _drawer.ResetController();
        _pathController.ResetController();
        _car.ResetController();
        _car.Init(_targetPosition, _boostsController.GetBoosts());

        gameState = GameState.DrawPath;
    }

    private void TryStartDriving()
    {
        _pathController.Switch(false);
        _drawer.Switch(false);
        _car.Move(_pathController.GetPath().ToArray());

        gameState = GameState.Driving;
    }

    private void FinishDrivingHandler(bool result)
    {
        gameState = GameState.Finish;

        if (result)
        {
            _gameOverWinEvent?.Invoke();
        }
        else
        { 
            _gameOverLoseEvent?.Invoke();
        }
    }

    private void AchiveBoostPositionHandler(Transform boost)
    {
        _boostsController.BoostAchived(boost);
    }
}