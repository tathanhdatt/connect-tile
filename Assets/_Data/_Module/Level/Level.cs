using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AudioService;
using Core.Service;
using Core.VibrationService;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Lean.Touch;
using MatchingPairAlgorithm;
using MessageEvent;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;
using Random = UnityEngine.Random;

public enum ChangeDirection
{
    None = -1,
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3,
}

public class Level : MonoBehaviour, ILevel
{
    [SerializeField] private CellView[] cellData;
    [SerializeField] private VectorObject2D drawer;
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private Transform starTarget;

    [Header("Change Direction")] [SerializeField]
    private ChangeDirection direction = ChangeDirection.None;

    [SerializeField, Range(0, 1)] private float changingPercent = 1;

    [Header("Time Counter")] [SerializeField]
    private Image timerCounter;

    [SerializeField] private float playingTime;

    private const int Col = 12;
    private const int Row = 8;

    private Algorithm algorithm;
    private readonly List<Point> selectedPoints = new List<Point>();
    private const int NumberMatchablePoints = 2;
    private Sprite[] icons;

    private readonly List<CellView> hintCells = new List<CellView>();
    private const float HintCoolDownTime = 120;


    private bool isCoolDownHint = false;

    private void Awake()
    {
        Messenger.AddListener<Point>(EventKey.AddCell, AddCellHandler);
        icons = Resources.LoadAll<Sprite>("CellIcon/");
        InitAlgorithm();
        ShuffleIcon();
        CountDown();
        CountDownHint();
        LeanTouch.OnFingerTap += OnFingerTapHandler;
    }

    private void OnDestroy()
    {
        LeanTouch.OnFingerTap -= OnFingerTapHandler;
        Messenger.RemoveListener<Point>(EventKey.AddCell, AddCellHandler);
    }

    private void OnFingerTapHandler(LeanFinger finger)
    {
        var fingerPosition = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
        fingerPosition.z = 0;
        var result = Physics2D.OverlapPoint(fingerPosition);
        if (result == null) return;
        var cell = result.GetComponent<CellView>();
        if (cell == null)
        {
            return;
        }

        cell.Selected();
        foreach (var cellHint in this.hintCells)
        {
            if (cell.EqualsByCoordinates(cellHint))
            {
                return;
            }
        }

        cell.SelectedScaleEffect();
    }

    private void CountDown()
    {
        DOTween.To(val => { timerCounter.fillAmount = val; }, 1, 0, playingTime)
            .SetEase(Ease.Linear)
            .OnComplete(() => { Messenger.Broadcast(EventKey.LevelLose); });
    }

    private Tweener coolDownHint;

    private void CountDownHint()
    {
        if (isCoolDownHint)
        {
            coolDownHint.Restart();
            return;
        }

        this.isCoolDownHint = true;
        this.coolDownHint = DOTween.To(val => { }, 1, 0, HintCoolDownTime)
            .OnComplete(ShowHint);
    }

    private void ShowHint()
    {
        if (this.hintCells.IsEmpty())
        {
            HasMatchablePairOfCell();
        }

        foreach (var cell in this.hintCells)
        {
            cell.transform.DOKill(true);
            cell.transform
                .DOScale(0.8f, 0.5f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }

        this.isCoolDownHint = false;
    }

    private async UniTask DrawLineFromListCell(List<int> cellIds)
    {
        var stars = new List<GameObject>();
        drawer.vectorLine.points2.Clear();
        drawer.vectorLine.active = true;
        foreach (var cellId in cellIds)
        {
            Vector2 pointPosition = this.cellData[cellId].transform.position;
            drawer.vectorLine.points2.Add(pointPosition);
            var go = Instantiate(starPrefab, this.transform, true);
            go.transform.localScale = Vector3.zero;
            go.transform.localPosition = pointPosition;
            go.transform.DOScale(new Vector3(0.4f, 0.4f, 0.4f), 0.1f)
                .SetEase(Ease.OutBack);
            stars.Add(go);
            // await UniTask.Delay(10);
        }

        drawer.vectorLine.Draw();
        await UniTask.Delay(100);
        drawer.vectorLine.active = false;
        foreach (var star in stars)
        {
            star.transform
                .DOMove(this.starTarget.position, 15)
                .SetEase(Ease.InBack)
                .SetSpeedBased(true)
                .OnComplete(() =>
                {
                    this.starTarget
                        .DOScale(new Vector3(0.011f, 0.011f, 0.011f), 0.2f)
                        .OnComplete(() => this.starTarget
                            .DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0.1f));
                    Destroy(star);
                });
            await UniTask.Delay(10);
        }
    }

    private void InitAlgorithm()
    {
        var intData = GetIntMatrix();
        this.algorithm = new Algorithm(new Graph(intData));
    }

    private int[,] GetIntMatrix()
    {
        var intData = new int[Row, Col];
        for (var rowId = 0; rowId < Row; rowId++)
        {
            for (var colId = 0; colId < Col; colId++)
            {
                var iconId = rowId * Col + colId;
                var cell = this.cellData[iconId];
                cell.X = rowId;
                cell.Y = colId;
                intData[rowId, colId] = cell.gameObject.activeSelf ? 1 : 0;
            }
        }

        return intData;
    }

    private void ShuffleIcon()
    {
        ServiceLocator.GetService<IAudioService>().PlaySfx(AudioName.Shuffle);
        List<Sprite> randomIcons = new List<Sprite>();
        var numberCell = 0;
        foreach (var cell in this.cellData)
        {
            if (cell.gameObject.activeSelf)
            {
                numberCell++;
            }
        }

        for (var cellId = 0; cellId < numberCell - 1; cellId += 2)
        {
            var randomIcon = icons[Random.Range(0, icons.Length)];
            randomIcons.Add(randomIcon);
            randomIcons.Add(randomIcon);
        }

        randomIcons.Shuffle();
        var iconId = 0;
        foreach (var cell in this.cellData)
        {
            if (!cell.gameObject.activeSelf) continue;
            cell.SetIconSprite(randomIcons[iconId]);
            iconId++;
        }
    }

    private async void AddCellHandler(Point newPoint)
    {
        if (selectedPoints.Count > 0)
        {
            if (newPoint.Equals(selectedPoints[0]))
            {
                return;
            }
        }

        selectedPoints.Add(newPoint);
        if (selectedPoints.Count < NumberMatchablePoints)
        {
            return;
        }

        var firstCellData = this.cellData[selectedPoints[0].X * Col + selectedPoints[0].Y];
        var secondCellData = this.cellData[selectedPoints[1].X * Col + selectedPoints[1].Y];
        var matchable = firstCellData.IconName
            .Equals(secondCellData.IconName);

        if (matchable)
        {
            var points = this.algorithm
                .GetPathFromPointToPoint(selectedPoints[0], selectedPoints[1]);
            
            
            selectedPoints.RemoveAt(0);
            selectedPoints.RemoveAt(0);
            var hasPath = points != null;
            if (hasPath)
            {
                foreach (var cell in this.hintCells)
                {
                    var cellTransform = cell.transform;
                    cellTransform.DOKill(true);
                    cellTransform.localScale = Vector3.one;
                }

                CountDownHint();
                firstCellData.SetActiveFXSelected(false);
                secondCellData.SetActiveFXSelected(false);
                firstCellData.gameObject.SetActive(false);
                secondCellData.gameObject.SetActive(false);
                var cellIds = new List<int>();
                foreach (var point in points)
                {
                    cellIds.Add(point.X * Col + point.Y);
                }

                ServiceLocator.GetService<IAudioService>().PlaySfx(AudioName.Connect);
                await this.DrawLineFromListCell(cellIds);
                var hasActiveCell = false;
                foreach (var cell in cellData)
                {
                    if (!cell.gameObject.activeSelf) continue;
                    hasActiveCell = true;
                    break;
                }

                if (hasActiveCell)
                {
                    if (Random.Range(0f, 1f) <= this.changingPercent)
                    {
                        ChangeCellPositions();
                        this.algorithm.SetGraph(new Graph(GetIntMatrix()));
                    }

                    while (!HasMatchablePairOfCell())
                    {
                        ShuffleIcon();
                    }
                }
                else
                {
                    Messenger.Broadcast(EventKey.LevelWin);
                }
            }
            else
            {
                ServiceLocator.GetService<IVibrationService>().Vibrate();
                ServiceLocator.GetService<IAudioService>().PlaySfx(AudioName.ConnectFail);
                await Task.Delay(100);
                firstCellData.SetActiveFXSelected(false);
                secondCellData.SetActiveFXSelected(false);
            }


            if (selectedPoints.Count > 0)
            {
                foreach (var point in selectedPoints)
                {
                    Debug.LogError(point);
                }
            }
        }
        else
        {
            firstCellData.SetActiveFXSelected(false);
            selectedPoints.RemoveAt(0);
        }
    }

    private bool HasMatchablePairOfCell()
    {
        hintCells.Clear();
        var numberCell = this.cellData.Length;
        for (var i = 0; i < numberCell - 1; i++)
        {
            var p1 = this.cellData[i];
            if (!p1.gameObject.activeSelf) continue;
            for (var j = i + 1; j < numberCell; j++)
            {
                var p2 = this.cellData[j];
                if (!p2.gameObject.activeSelf) continue;
                var canMatch = p1.IconName.Equals(p2.IconName);
                if (!canMatch) continue;
                var hasPath = this.algorithm
                    .HasPath(new Point(p1.X, p1.Y), new Point(p2.X, p2.Y));
                if (hasPath)
                {
                    this.hintCells.Add(p1);
                    this.hintCells.Add(p2);
                    return true;
                }
            }
        }

        return false;
    }

    private void ChangeCellPositions()
    {
        switch (this.direction)
        {
            case ChangeDirection.Left:
                for (var rowId = 1; rowId < Row - 1; rowId++)
                {
                    var colIdOfActiveCell = 1;
                    for (var colId = 1; colId < Col - 1; colId++)
                    {
                        var currentCell = this.cellData[rowId * Col + colId];
                        var isActiveCell = currentCell.gameObject.activeSelf;
                        if (isActiveCell)
                        {
                            var newPositionCell = this.cellData[rowId * Col + colIdOfActiveCell];
                            (newPositionCell.X, currentCell.X) = (currentCell.X, newPositionCell.X);
                            (newPositionCell.Y, currentCell.Y) = (currentCell.Y, newPositionCell.Y);
                            this.cellData[rowId * Col + colIdOfActiveCell] = currentCell;
                            this.cellData[rowId * Col + colId] = newPositionCell;

                            var oldPosition = currentCell.transform.position;
                            var newPosition = newPositionCell.transform.position;
                            newPositionCell.transform.position = oldPosition;
                            currentCell.transform.DOMove(newPosition, 0.2f)
                                .SetEase(Ease.OutBack);

                            colIdOfActiveCell++;
                        }
                    }
                }

                break;
            case ChangeDirection.Up:
                for (var colId = 1; colId < Col - 1; colId++)
                {
                    var rowIdOfActiveCell = 1;
                    for (var rowId = 1; rowId < Row - 1; rowId++)
                    {
                        var currentCell = this.cellData[rowId * Col + colId];
                        var isActiveCell = currentCell.gameObject.activeSelf;
                        if (isActiveCell)
                        {
                            var newPositionCell = this.cellData[rowIdOfActiveCell * Col + colId];
                            (newPositionCell.X, currentCell.X) = (currentCell.X, newPositionCell.X);
                            (newPositionCell.Y, currentCell.Y) = (currentCell.Y, newPositionCell.Y);
                            this.cellData[rowIdOfActiveCell * Col + colId] = currentCell;
                            this.cellData[rowId * Col + colId] = newPositionCell;

                            var oldPosition = currentCell.transform.position;
                            var newPosition = newPositionCell.transform.position;
                            newPositionCell.transform.position = oldPosition;
                            currentCell.transform.DOMove(newPosition, 0.2f)
                                .SetEase(Ease.OutBack);

                            rowIdOfActiveCell++;
                        }
                    }
                }

                break;
            case ChangeDirection.Down:
                for (var colId = 1; colId < Col - 1; colId++)
                {
                    var rowIdOfActiveCell = Row - 2;
                    for (var rowId = Row - 2; rowId > 0; rowId--)
                    {
                        var currentCell = this.cellData[rowId * Col + colId];
                        var isActiveCell = currentCell.gameObject.activeSelf;
                        if (isActiveCell)
                        {
                            var newPositionCell = this.cellData[rowIdOfActiveCell * Col + colId];
                            (newPositionCell.X, currentCell.X) = (currentCell.X, newPositionCell.X);
                            (newPositionCell.Y, currentCell.Y) = (currentCell.Y, newPositionCell.Y);
                            this.cellData[rowIdOfActiveCell * Col + colId] = currentCell;
                            this.cellData[rowId * Col + colId] = newPositionCell;

                            var oldPosition = currentCell.transform.position;
                            var newPosition = newPositionCell.transform.position;
                            newPositionCell.transform.position = oldPosition;
                            currentCell.transform.DOMove(newPosition, 0.2f)
                                .SetEase(Ease.OutBack);

                            rowIdOfActiveCell--;
                        }
                    }
                }

                break;
            case ChangeDirection.Right:
                for (var rowId = Row - 2; rowId > 0; rowId--)
                {
                    var colIdOfActiveCell = Col - 2;
                    for (var colId = Col - 2; colId > 0; colId--)
                    {
                        var currentCell = this.cellData[rowId * Col + colId];
                        var isActiveCell = currentCell.gameObject.activeSelf;
                        if (isActiveCell)
                        {
                            var newPositionCell = this.cellData[rowId * Col + colIdOfActiveCell];
                            (newPositionCell.X, currentCell.X) = (currentCell.X, newPositionCell.X);
                            (newPositionCell.Y, currentCell.Y) = (currentCell.Y, newPositionCell.Y);
                            this.cellData[rowId * Col + colIdOfActiveCell] = currentCell;
                            this.cellData[rowId * Col + colId] = newPositionCell;

                            var oldPosition = currentCell.transform.position;
                            var newPosition = newPositionCell.transform.position;
                            newPositionCell.transform.position = oldPosition;
                            currentCell.transform.DOMove(newPosition, 0.2f)
                                .SetEase(Ease.OutBack);

                            colIdOfActiveCell--;
                        }
                    }
                }

                break;
            case ChangeDirection.None:
                break;
        }
    }
}