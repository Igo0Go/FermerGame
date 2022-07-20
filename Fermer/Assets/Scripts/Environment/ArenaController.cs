using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaController : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;
    [SerializeField] private List<TranslateScript> movingCubes;

    public void AllToDefault()
    {
        foreach (var item in movingCubes)
        {
            item.ToDefaultPos();
        }
    }

    public void StartRandomMoving()
    {
        StartCoroutine(StartRandomMovingCoroutine());
    }

    private IEnumerator StartRandomMovingCoroutine()
    {
        foreach (var item in movingCubes)
        {
            item.loopMove = true;
            item.SetMovingTime(Random.Range(3f, 7f));
            item.ChangePosition();
            yield return null;
        }
    }
}
