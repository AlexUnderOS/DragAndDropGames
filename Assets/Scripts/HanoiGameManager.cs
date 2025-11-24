using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HanoiGameManager : MonoBehaviour
{
    public static HanoiGameManager Instance { get; private set; }
    [Header("Reward Button")]
    public Button rewardedButton;

    [Header("Настройки башни")]
    public int diskCount = 4;
    public float baseDiskWidth = 1.5f;
    public float diskWidthStep = 0.25f;

    [Header("Стержни (позиции внизу палок, без скейла)")]
    public Transform[] pegRoots = new Transform[3];

    [Header("Родитель для дисков (опционально)")]
    public Transform disksParent;

    [Header("Префаб диска")]
    public HanoiDisk diskPrefab;

    [Header("Цвета дисков (0 = самый маленький)")]
    public Color[] diskColors;

    [Header("UI")]
    public Text movesText;
    public Text minMovesText;
    public Text timerText;
    public Text modeText;

    public Text best15Text;
    public Text best30Text;
    public Text best60Text;

    public Button startButton;
    public Button mode15Button;
    public Button mode30Button;
    public Button mode60Button;

    private List<HanoiDisk>[] pegs = new List<HanoiDisk>[3];
    private List<HanoiDisk> allDisks = new List<HanoiDisk>();
    private int moveCount = 0;
    private float diskHeightWorld = 0f;

    private float timeLimitSeconds = 0f;
    private float timeRemaining = 0f;
    private bool timerRunning = false;

    private readonly int[] modeSeconds = new int[] { 15, 30, 60 };
    private int currentModeIndex = -1;

    private int[] bestMovesForMode = new int[3];
    private readonly string[] bestMovesKeys = new string[]
    {
        "HanoiBest_15",
        "HanoiBest_30",
        "HanoiBest_60"
    };

    public bool gameRunning { get; private set; } = false;

    private bool rewardUsedThisGame = false;

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
            pegs[i] = new List<HanoiDisk>();

        CreateDisksSolved();

        long minMoves = (1L << diskCount) - 1;
        if (minMovesText != null)
            minMovesText.text = $"Min: {minMoves}";

        LoadBestResults();
        UpdateBestTexts();

        UpdateMovesText();
        UpdateTimerText();

        if (startButton != null)
            startButton.interactable = true;

        SetModeButtonsInteractable(true);
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;
            gameRunning = false;
            UpdateTimerText();
            OnTimeExpired();
            return;
        }

        UpdateTimerText();
    }


    public void SelectMode15() => SelectMode(0);
    public void SelectMode30() => SelectMode(1);
    public void SelectMode60() => SelectMode(2);

    private void SelectMode(int modeIndex)
    {
        if (gameRunning)
            return;

        currentModeIndex = modeIndex;
        timeLimitSeconds = modeSeconds[modeIndex];
        timeRemaining = timeLimitSeconds;

        if (modeText != null)
            modeText.text = $"{modeSeconds[modeIndex]} s";

        UpdateTimerText();
        UpdateBestTexts();
    }

    public void StartGame()
    {

        if (rewardedButton != null)
    rewardedButton.interactable = true;
        if (currentModeIndex < 0)
        {
            SelectMode30();
        }

        moveCount = 0;
        UpdateMovesText();

        timeRemaining = timeLimitSeconds;
        timerRunning = true;
        gameRunning = true;
        rewardUsedThisGame = false;

        RandomizeDisks();

        if (startButton != null)
            startButton.interactable = false;

        SetModeButtonsInteractable(false);
    }

    private void OnTimeExpired()
    {
        Debug.Log("Time is up! Game over.");
        if (startButton != null)
            startButton.interactable = true;

        SetModeButtonsInteractable(true);

        rewardUsedThisGame = false;
        ResetTowerToInitial();
    }

    private void SetModeButtonsInteractable(bool value)
    {
        if (mode15Button != null) mode15Button.interactable = value;
        if (mode30Button != null) mode30Button.interactable = value;
        if (mode60Button != null) mode60Button.interactable = value;
    }


    private void ResetTowerToInitial()
    {
        foreach (var d in allDisks)
        {
            if (d != null)
                Destroy(d.gameObject);
        }

        allDisks.Clear();
        for (int i = 0; i < 3; i++)
            pegs[i].Clear();

        diskHeightWorld = 0f;
        CreateDisksSolved();
    }

    private void CreateDisksSolved()
    {
        if (diskPrefab == null)
        {
            Debug.LogError("HanoiGameManager: diskPrefab is not assigned!");
            return;
        }

        allDisks.Clear();
        for (int i = 0; i < 3; i++)
            pegs[i].Clear();

        for (int i = 0; i < diskCount; i++)
        {
            int sizeIndex = diskCount - 1 - i;

            HanoiDisk disk = Instantiate(diskPrefab);
            if (disksParent != null)
                disk.transform.SetParent(disksParent);

            disk.sizeIndex = sizeIndex;
            ApplyDiskColor(disk, sizeIndex);

            // ширина
            float width = baseDiskWidth - diskWidthStep * (diskCount - 1 - sizeIndex);
            Vector3 localScale = disk.transform.localScale;
            localScale.x = width;
            disk.transform.localScale = localScale;

            if (diskHeightWorld <= 0f)
            {
                var rend = disk.GetComponent<Renderer>();
                if (rend != null)
                {
                    diskHeightWorld = rend.bounds.size.y * 1.05f;
                }
                else
                {
                    diskHeightWorld = 0.3f;
                }
            }

            allDisks.Add(disk);
            pegs[0].Add(disk);
        }

        UpdatePegVisual(0);
    }

    private void ApplyDiskColor(HanoiDisk disk, int sizeIndex)
    {
        Color c = Color.white;
        if (diskColors != null && diskColors.Length > 0)
        {
            c = diskColors[Mathf.Clamp(sizeIndex, 0, diskColors.Length - 1)];
        }
        c.a = 1f;

        var sr = disk.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = c;
            sr.sortingOrder = 5 + sizeIndex;
            return;
        }

        var mr = disk.GetComponent<MeshRenderer>();
        if (mr != null && mr.material != null)
        {
            mr.material.color = c;
        }
    }

    private void RandomizeDisks()
    {
        for (int i = 0; i < 3; i++)
            pegs[i].Clear();

        List<HanoiDisk>[] temp = new List<HanoiDisk>[3];
        for (int i = 0; i < 3; i++)
            temp[i] = new List<HanoiDisk>();

        foreach (var disk in allDisks)
        {
            int pegIndex = Random.Range(0, 3);
            temp[pegIndex].Add(disk);
        }

        for (int i = 0; i < 3; i++)
        {
            temp[i].Sort((a, b) => b.sizeIndex.CompareTo(a.sizeIndex));
            pegs[i].AddRange(temp[i]);
        }

        if (pegs[0].Count == diskCount || pegs[1].Count == diskCount || pegs[2].Count == diskCount)
        {
            for (int i = 0; i < 3; i++)
                pegs[i].Clear();
            RandomizeDisks();
            return;
        }

        for (int i = 0; i < 3; i++)
            UpdatePegVisual(i);
    }

    private void UpdatePegVisual(int pegIndex)
    {
        var list = pegs[pegIndex];
        for (int i = 0; i < list.Count; i++)
        {
            HanoiDisk disk = list[i];
            if (disk == null) continue;

            Vector3 basePos = pegRoots[pegIndex].position;
            basePos.y += diskHeightWorld * 0.5f;
            basePos.y += i * diskHeightWorld;

            disk.transform.position = new Vector3(
                basePos.x,
                basePos.y,
                disk.transform.position.z
            );
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
        if (!gameRunning)
        {
            ResetAllVisuals();
            return;
        }

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

    private int GetTotalDiskCount()
    {
        int total = 0;
        for (int i = 0; i < pegs.Length; i++)
            total += pegs[i].Count;
        return total;
    }

    public void RemoveOneDiskReward()
    {
        if (!gameRunning)
        {
            Debug.Log("RemoveOneDiskReward: game is not running, skipping.");
            return;
        }

        if (rewardUsedThisGame)
        {
            Debug.Log("RemoveOneDiskReward: reward already used this game.");
            return;
        }

        int total = GetTotalDiskCount();
        if (total <= 1)
        {
            Debug.Log("RemoveOneDiskReward: not enough disks to remove.");
            return;
        }

        HanoiDisk target = null;
        int targetPeg = -1;
        int targetIndexOnPeg = -1;

        for (int i = 0; i < pegs.Length; i++)
        {
            var list = pegs[i];
            for (int j = 0; j < list.Count; j++)
            {
                var d = list[j];
                if (d == null) continue;

                if (target == null || d.sizeIndex < target.sizeIndex)
                {
                    target = d;
                    targetPeg = i;
                    targetIndexOnPeg = j;
                }
            }
        }

        if (target == null || targetPeg < 0)
        {
            Debug.LogWarning("RemoveOneDiskReward: no valid disk found.");
            return;
        }

        Debug.Log($"RemoveOneDiskReward: removing disk sizeIndex={target.sizeIndex} from peg {targetPeg}");

        var pegList = pegs[targetPeg];
        pegList.RemoveAt(targetIndexOnPeg);

        allDisks.Remove(target);
        Destroy(target.gameObject);

        UpdatePegVisual(targetPeg);

        rewardUsedThisGame = true;

        CheckWin();
    }


    private void UpdateMovesText()
    {
        if (movesText != null)
            movesText.text = $"Moves: {moveCount}";
        if (rewardedButton != null)
    rewardedButton.interactable = false;
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        if (timeLimitSeconds <= 0f)
        {
            timerText.text = "--:--";
            return;
        }

        int seconds = Mathf.CeilToInt(timeRemaining);
        if (seconds < 0) seconds = 0;
        int m = seconds / 60;
        int s = seconds % 60;

        timerText.text = $"{m:00}:{s:00}";
    }

    private void LoadBestResults()
    {
        for (int i = 0; i < bestMovesForMode.Length; i++)
        {
            bestMovesForMode[i] = PlayerPrefs.GetInt(bestMovesKeys[i], 0);
        }
    }

    private void SaveBestResult(int modeIndex)
    {
        PlayerPrefs.SetInt(bestMovesKeys[modeIndex], bestMovesForMode[modeIndex]);
        PlayerPrefs.Save();
    }

    private void UpdateBestTexts()
    {
        if (best15Text != null)
            best15Text.text = bestMovesForMode[0] > 0 ? bestMovesForMode[0].ToString() : "--";

        if (best30Text != null)
            best30Text.text = bestMovesForMode[1] > 0 ? bestMovesForMode[1].ToString() : "--";

        if (best60Text != null)
            best60Text.text = bestMovesForMode[2] > 0 ? bestMovesForMode[2].ToString() : "--";
    }

    private void CheckWin()
    {
        if (rewardedButton != null)
            rewardedButton.interactable = false;
        int totalDisks = GetTotalDiskCount();
        if (totalDisks == 0)
            return;

        int fullPegIndex = -1;
        for (int i = 0; i < pegs.Length; i++)
        {
            if (pegs[i].Count == totalDisks)
            {
                fullPegIndex = i;
                break;
            }
        }

        if (fullPegIndex == -1)
            return;

        Debug.Log($"Hanoi completed on peg {fullPegIndex}!");

        timerRunning = false;
        gameRunning = false;

        if (startButton != null)
            startButton.interactable = true;

        SetModeButtonsInteractable(true);

        if (currentModeIndex >= 0)
        {
            int best = bestMovesForMode[currentModeIndex];
            if (best == 0 || moveCount < best)
            {
                bestMovesForMode[currentModeIndex] = moveCount;
                SaveBestResult(currentModeIndex);
                UpdateBestTexts();
            }
        }

        rewardUsedThisGame = false;
        ResetTowerToInitial();
    }

    private void ResetAllVisuals()
    {
        for (int i = 0; i < 3; i++)
            UpdatePegVisual(i);
    }
}
