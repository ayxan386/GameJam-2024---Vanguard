using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [Header("Ground detection")]
    [SerializeField] private Transform bottomLeft, bottomRight, topLeft;

    [SerializeField] private Vector2Int stepSize;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private bool showGizmos;

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

    private IEnumerator Start()
    {
        FindAllPoints();
        SpawnAllBeacons();

        yield return new WaitUntil(() => MainGameManager.Instance.CanMove);
        
        PlayerController.OnReachedBeacon += OnPlayerReachedBeacon; 

        StartCoroutine(GameLoop());
    }

    private void OnPlayerReachedBeacon(PlayerController obj)
    {
        print($"Player {obj.PlayerIndex} reached beacon");
        currentReached++;
                    
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
    }

    private IEnumerator GameLoop()
    {
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

    [ContextMenu("Find all points")]
    public void FindAllPoints()
    {
        emptyPoints = new List<Vector3>();
        for (int posY = 0; posY <= stepSize.y; posY++)
        {
            var bottomLeftPosition = bottomLeft.position;
            var offset = Vector3.Lerp(bottomLeftPosition, topLeft.position, 1f * posY / stepSize.y) -
                         bottomLeftPosition;
            for (int posX = 0; posX <= stepSize.x; posX++)
            {
                var pos = Vector3.Lerp(bottomLeftPosition, bottomRight.position, 1f * posX / stepSize.x) + offset;

                if (Physics.CheckSphere(pos, checkRadius, groundLayer))
                {
                    emptyPoints.Add(pos);
                }
            }
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

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.blue;

        foreach (var emptyPoint in emptyPoints)
        {
            Gizmos.DrawSphere(emptyPoint, checkRadius);
        }
    }
}

[Serializable]
public class GameStage
{
    // public int totalBeaconCount;
    public int selectedBeaconCount;
}