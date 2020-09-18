using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostsController : MonoBehaviour
{
    [SerializeField] private List<Transform> _boostsPosition;

    public void StartGame()
    {
        _boostsPosition.ForEach(x => x.gameObject.SetActive(true));
    }

    public List<Transform> GetBoosts()
    {
        return _boostsPosition;
    }

    public void BoostAchived(Transform boost)
    {
        boost.gameObject.SetActive(false);
    }
}
