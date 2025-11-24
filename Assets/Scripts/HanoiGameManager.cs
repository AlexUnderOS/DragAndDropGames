using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HanoiGameManager : MonoBehaviour
{
    public static HanoiGameManager Instance { get; private set; }

    [Header("Настройки")]
    public int diskCount = 4;
    public float diskHeight = 0.3f;  
    public float baseDiskWidth = 1.5f;  
    public float diskWidthStep = 0.25f;  

    [Header("Стержни")]
    public Transform[] pegRoots = new Transform[3]; // Peg0, Peg1, Peg2

    [Header("Префаб диска")]
    public HanoiDisk diskPrefab;

    [Header("UI")]
    public Text movesText;
    public Text minMovesText;

    private List<HanoiDisk>[] pegs = new List<HanoiDisk>[3];
    private int moveCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            pegs[i] = new List<HanoiDisk>();
        }

        CreateDisks();

        long minMoves = (1L << diskCount) - 1; // 2^n - 1
        if (minMovesText != null)
            minMovesText.text = $"Min: {minMoves}";

        UpdateMovesText();
    }

    private void CreateDisks()
    {
        if (diskPrefab == null)
        {
            Debug.LogError("HanoiGameManager: diskPrefab is not assigned!");
            return;
        }

        for (int i = 0; i < diskCount; i++)
        {
            int sizeIndex = diskCount - 1 - i;

            HanoiDisk disk = Instantiate(diskPrefab, pegRoots[0]);
            disk.sizeIndex = sizeIndex;

            float width = baseDiskWidth - diskWidthStep * (diskCount - 1 - sizeIndex);
            Vector3 localScale = disk.transform.localScale;
            localScale.x = width;
            disk.transform.localScale = localScale;

            pegs[0].Add(disk);
        }

        UpdatePegVisual(0);
    }

    private void UpdatePegVisual(int pegIndex)
    {
        var list = pegs[pegIndex];
        for (int i = 0; i < list.Count; i++)
        {
            HanoiDisk disk = list[i];
            if (disk == null) continue;

            Vector3 basePos = pegRoots[pegIndex].position;
            basePos.y += i * diskHeight;
            disk.transform.position = basePos;
        }
    }

    public bool IsTopDisk(HanoiDisk disk, out int pegIndex)
    {
        for (int i = 0; i < pegs.Length; i++)
        {
            var list = pegs[i];
            if (list.Count == 0)
                continue;

            if (list[list.Count - 1] == disk)
            {
                pegIndex = i;
                return true;
            }

            if (list.Contains(disk))
            {
                pegIndex = i;
                return false;
            }
        }

        pegIndex = -1;
        return false;
    }

    public void TryDropDisk(HanoiDisk disk, Vector3 worldPos)
    {
        int fromPeg = -1;
        for (int i = 0; i < pegs.Length; i++)
        {
            if (pegs[i].Contains(disk))
            {
                fromPeg = i;
                break;
            }
        }

        if (fromPeg == -1)
        {
            Debug.LogWarning("Disk is not registered on any peg!");
            return;
        }

        int toPeg = GetClosestPeg(worldPos);

        if (toPeg == fromPeg)
        {
            UpdatePegVisual(fromPeg);
            return;
        }

        var fromList = pegs[fromPeg];
        var toList = pegs[toPeg];

        if (fromList.Count == 0 || fromList[fromList.Count - 1] != disk)
        {
            Debug.Log("Move rejected: disk is not top on source peg.");
            UpdatePegVisual(fromPeg);
            return;
        }

        if (toList.Count > 0)
        {
            var topTargetDisk = toList[toList.Count - 1];
            if (disk.sizeIndex > topTargetDisk.sizeIndex)
            {
                Debug.Log("Invalid move: larger disk on smaller disk!");
                UpdatePegVisual(fromPeg);
                return;
            }
        }

        fromList.RemoveAt(fromList.Count - 1);
        toList.Add(disk);

        UpdatePegVisual(fromPeg);
        UpdatePegVisual(toPeg);

        moveCount++;
        UpdateMovesText();
        CheckWin();
    }

    private int GetClosestPeg(Vector3 worldPos)
    {
        int bestIndex = 0;
        float bestDist = float.MaxValue;

        for (int i = 0; i < pegRoots.Length; i++)
        {
            float d = Mathf.Abs(worldPos.x - pegRoots[i].position.x);
            if (d < bestDist)
            {
                bestDist = d;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private void UpdateMovesText()
    {
        if (movesText != null)
            movesText.text = $"Moves: {moveCount}";
    }

    private void CheckWin()
    {
        if (pegs[2].Count == diskCount)
        {
            Debug.Log("Hanoi completed!");
        }
    }
}
