using DG.Tweening;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

/* BoardRearranger responsible for moving planets around the board*/
public class BoardRearranger 
{
    private PlanetObjectPool _planetPool;

    private const float DELAY_BEFORE_REMOVAL = 0.6f;
    private const float PLANET_SLOW_MOVE_DURATION = 2f;
    private const float PLANET_FAST_MOVE_DURATION = 0.4f;

    public BoardRearranger(PlanetObjectPool planetPool)
    {
        _planetPool = planetPool;
    }
    /* Handle removal of matching planets, 
     * moving planets above to empty tiles, 
     * and creating new planets to be assigned unto evacuated tiles*/
    public async UniTask ProcessBoardAfterMatch(Board board, ScanResult scanResult, bool shouldAnimate)
    {
        await RemovePlanetsInMatchingSequences(scanResult.MatchingSequences, shouldAnimate);
        AssignPlanetsToEmptyTiles(board);
        await MoveAssignedPlanetsToTilePositions(board, shouldAnimate);
    }

    /* remove matching planets from board */
    private async UniTask RemovePlanetsInMatchingSequences(List<List<Tile>> matchingSequences, bool shouldAnimate)
    {
        if (shouldAnimate)
        {
            foreach (var currentMatchingSequence in matchingSequences)
            {
                foreach (var currentTile in currentMatchingSequence)
                {
                    if (currentTile.Planet != null)
                    {
                        currentTile.Planet.StartCrushAnimation();
                    }
                }
            }
            await UniTask.Delay(TimeSpan.FromSeconds(DELAY_BEFORE_REMOVAL));
        }

        foreach (var currentMatchingSequence in matchingSequences)
        {
            foreach (var currentTile in currentMatchingSequence)
            {
                if (currentTile.Planet != null)
                {
                    _planetPool.ReturnPlanetToPool(currentTile.Planet.gameObject);
                    currentTile.RemovePlanetRefFromTile();
                }
            }
        }
    }

    /* For each column, calculate how many tiles became empty.
     * Move planets above empty tiles unto these empty tiles, 
     * and create new planets which will be placed unto evacuated tiles */
    private void AssignPlanetsToEmptyTiles(Board board)
    {
        for (var colIndex = 0; colIndex < board.NumOfCols; colIndex++)
        {
            var emptyTileCounter = 0;
            for (var rowIndex = 0; rowIndex < board.NumOfRows; rowIndex++)
            {
                var currentTile = board.Tile2dArray[rowIndex, colIndex];
                if (currentTile.Planet == null || currentTile.Planet.GetPlanetType() == PlanetType.None)
                {
                    emptyTileCounter++;
                }
                else if (emptyTileCounter > 0)
                {
                    var emptyTile = board.Tile2dArray[rowIndex - emptyTileCounter, colIndex];
                    emptyTile.AssignPlanetToTile(currentTile.Planet);
                    currentTile.RemovePlanetRefFromTile();
                }
            }
            GenerateNewPlanetsForColumn(board, colIndex, emptyTileCounter);
        }
    }

    /* For a given column, create new planets and assign them unto evacuated tiles*/
    private void GenerateNewPlanetsForColumn(Board board, int colIndex, int emptyTileCounter)
     {
        var firstEmptyRowIndex = board.NumOfRows - emptyTileCounter;
        for (int rowIndex = firstEmptyRowIndex; rowIndex < board.NumOfRows; rowIndex++)
        {
            var currentTile = board.Tile2dArray[rowIndex, colIndex];
            var newPlanet = _planetPool.GetRandomPlanet();
            newPlanet.transform.position =
                new Vector3(currentTile.GetPlanetContainerPosition().x,
                            currentTile.GetPlanetContainerPosition().y + emptyTileCounter,
                            currentTile.GetPlanetContainerPosition().z);

            currentTile.AssignPlanetToTile(newPlanet.GetComponent<Planet>());
        }
    }

    /* switch between refs of planet assignments of 2 tiles, 
     * and then physically move their positions */
    public async UniTask SwitchPlanets(Board board, Tile originTile, Tile targetTile)
    {
        board.SwitchBetweenTilePlanetRefs(originTile, targetTile);
        await MoveAssignedPlanetsToTilePositions(board, true);
    }

    /* Throughout the board, move planets assigned to tiles to position of these tiles */
    public async UniTask MoveAssignedPlanetsToTilePositions(Board board, bool shouldAnimate, bool IsSlowAnimation = false)
    {
        var completionSource = new UniTaskCompletionSource();

        if (shouldAnimate)
        {
            var seq = DOTween.Sequence();

            for (var colIndex = 0; colIndex < board.NumOfCols; colIndex++)
            {
                for (var rowIndex = 0; rowIndex < board.NumOfRows; rowIndex++)
                {
                    var currentTile = board.Tile2dArray[rowIndex, colIndex];
                    var targetPosition = currentTile.GetPlanetContainerPosition();
                    float duration = IsSlowAnimation ?
                       PLANET_SLOW_MOVE_DURATION :
                       PLANET_FAST_MOVE_DURATION;

                    var tween = currentTile.Planet.transform.DOMove(targetPosition, duration).SetEase(Ease.OutBack).Pause();

                    seq.Join(tween).Pause();
                }
            }

            seq.Play()
                .OnComplete(() => completionSource.TrySetResult());
        }
        else
        {
            for (var colIndex = 0; colIndex < board.NumOfCols; colIndex++)
            {
                for (var rowIndex = 0; rowIndex < board.NumOfRows; rowIndex++)
                {
                    var currentTile = board.Tile2dArray[rowIndex, colIndex];
                    currentTile.Planet.transform.position = currentTile.GetPlanetContainerPosition();
                }
            }

            completionSource.TrySetResult();
        }

        await completionSource.Task;
    }

}