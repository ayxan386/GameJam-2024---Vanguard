using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<Transform> lightPoints;

    [Header("Game play")]
    [SerializeField] private Renderer beaconPrefab;

    [SerializeField] private Transform beaconParent;
    [SerializeField] private Color[] beaconColors;

    [SerializeField] private float waitBetweenStages;
    [SerializeField] private int selectedColor;
    [SerializeField] private List<GameStage> stages;

    private List<Vector3> emptyPoints;
    private List<Renderer> beacons;
    private int totalActiveAtStartOfStage;
    private int currentReached;
    private bool CanContinue { get; set; }

    public static event Action<Color> OnNextStageStarted;
    public static event Action<int, int> OnReachCountUpdate;
    public static event Action<int> OnPlayerVictory;

    private IEnumerator Start()
    {
        SetLights();
        SpawnAllBeacons();

        yield return new WaitUntil(() => MainGameManager.Instance.CanMove);

        PlayerController.OnReachedBeacon += OnPlayerReachedBeacon;

        StartCoroutine(GameLoop());
    }

    private void OnPlayerReachedBeacon(PlayerController obj)
    {
        currentReached++;
        currentReached = Mathf.Clamp(currentReached, 0, totalActiveAtStartOfStage);

        if (currentReached + 1 >= totalActiveAtStartOfStage)
        {
            foreach (var player in MainGameManager.Instance.Players)
            {
                if (!player.Reached)
                {
                    player.SetEliminated();
                }
            }

            CanContinue = true;
        }

        OnReachCountUpdate?.Invoke(currentReached, totalActiveAtStartOfStage);
    }

    private IEnumerator GameLoop()
    {
        if (beacons.Count <= 0) yield break;

        foreach (var stage in stages)
        {
            yield return new WaitForSeconds(waitBetweenStages);

            foreach (var beacon in beacons)
            {
                beacon.gameObject.SetActive(false);
            }

            totalActiveAtStartOfStage = 0;
            currentReached = 0;

            foreach (var playerController in MainGameManager.Instance.Players)
            {
                if (!playerController.IsEliminated) totalActiveAtStartOfStage++;
            }

            if (totalActiveAtStartOfStage == 1)
            {
                OnPlayerVictory?.Invoke(totalActiveAtStartOfStage);
                yield break;
            }

            OnReachCountUpdate?.Invoke(currentReached, totalActiveAtStartOfStage);

            for (int count = 0; count < stage.selectedBeaconCount; count++)
            {
                var randomBeacon = Random.Range(0, beacons.Count);
                var randomColor = Random.Range(0, beaconColors.Length);
                beacons[randomBeacon].gameObject.SetActive(true);
                beacons[randomBeacon].material.color = beaconColors[randomColor];

                selectedColor = randomColor;
            }

            OnNextStageStarted?.Invoke(beaconColors[selectedColor]);
            CanContinue = false;

            yield return new WaitUntil(() => CanContinue);
        }
    }

    public void SetLights()
    {
        emptyPoints = new List<Vector3>();
        foreach (var point in lightPoints)
        {
            emptyPoints.Add(point.position);
        }
    }

    private void SpawnAllBeacons()
    {
        beacons = new List<Renderer>();
        foreach (var emptyPoint in emptyPoints)
        {
            var beacon = Instantiate(beaconPrefab, emptyPoint, Quaternion.identity, beaconParent);
            beacon.gameObject.SetActive(false);
            beacons.Add(beacon);
        }
    }
}

[Serializable]
public class GameStage
{
    public int selectedBeaconCount;
}