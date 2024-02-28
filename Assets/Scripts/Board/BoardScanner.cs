using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* BoardScanner responsible for scanning the board for matches */
public class BoardScanner
{
    /*scans the board and returns all scan-related data ina ScanResult object */
    public ScanResult ScanBoard(Board board)
    {
        var matchingSequences = new List<List<Tile>>();

        ScanForHorizontalSequences(board, ref matchingSequences);
        ScanForVerticalSequences(board, ref matchingSequences);

        return new ScanResult(matchingSequences);
    }

    private void ScanForVerticalSequences(Board board, ref List<List<Tile>> matchingSequences)
    {
        for (int colIndex = 0; colIndex < board.NumOfCols; colIndex++)
        {
            for (int rowIndex = 0; rowIndex < board.NumOfRows; rowIndex++)
            {
                UpdateMatchingSequencesForCurrentTile(board, rowIndex, colIndex, ScanType.Vertical, ref matchingSequences);
            }
        }
    }

    private void ScanForHorizontalSequences(Board board, ref List<List<Tile>> matchingSequences)
    {
        for (int rowIndex = 0; rowIndex < board.NumOfRows; rowIndex++)
        {
            for (int colIndex = 0; colIndex < board.NumOfCols; colIndex++)
            {
                UpdateMatchingSequencesForCurrentTile(board, rowIndex, colIndex, ScanType.Horizontal, ref matchingSequences);
            }
        }
    }

    /* either adds current tile's planet to the last match sequence,
     * or creates a new sequence which includes this planet*/ 

    private void UpdateMatchingSequencesForCurrentTile(Board board, int rowIndex, int colIndex, ScanType scanType, ref List<List<Tile>> matchingSequences)
    {
        var currentTile = board.Tile2dArray[rowIndex, colIndex];
        var (previousTileRowIndex, previousTileColIndex) = GetPreviousTileIndices(scanType, rowIndex, colIndex);

        if (IsPrevTileBeyondScope(scanType, previousTileRowIndex, previousTileColIndex))
        {
            matchingSequences.Add(new List<Tile>() { currentTile });
        }
        else if (currentTile.Planet?.GetPlanetType() != PlanetType.None)
        {
            var prevTile = board.Tile2dArray[previousTileRowIndex, previousTileColIndex];
            if(prevTile.Planet?.GetPlanetType() == currentTile.Planet?.GetPlanetType())
            {
                matchingSequences.Last().Add(currentTile);
            }
            else
            {
                matchingSequences.Add(new List<Tile>() { currentTile });
            }
        }
    }

    /* Checks if there are possible options that create matches after a single move */
    public bool DoesBoardEnableValidMoves(Board board)
    {
        for (int colIndex = 0; colIndex < board.NumOfCols; colIndex++)
        {
            for (int rowIndex = 0; rowIndex < board.NumOfRows; rowIndex++)
            {
                var isOneStepAwayFromValidMove = DoesMoveCreateMatch(board, rowIndex, colIndex, ScanType.Vertical);
                if (isOneStepAwayFromValidMove)
                {
                    Debug.Log($"Sequence will be created by switching between:[{rowIndex},{colIndex}] and [{rowIndex - 1},{colIndex}]");
                    return true;
                }
            }
        }

        for (int rowIndex = 0; rowIndex < board.NumOfRows; rowIndex++)
        {
            for (int colIndex = 0; colIndex < board.NumOfCols; colIndex++)
            {
                var isOneStepAwayFromValidMove = DoesMoveCreateMatch(board, rowIndex, colIndex, ScanType.Horizontal);
                if (isOneStepAwayFromValidMove)
                {
                    Debug.Log($"Sequence will be created by switching between:[{rowIndex},{colIndex}] and [{rowIndex},{colIndex - 1}]");
                    return true;
                }
            }
        }

        return false;
    }

    /* Checks if switching between planets of current and previous tile creates a match */
    private bool DoesMoveCreateMatch(Board board, int rowIndex, int colIndex, ScanType scanType)
    {
        var currentTile = board.Tile2dArray[rowIndex, colIndex];
        var (previousTileRowIndex, previousTileColIndex) = GetPreviousTileIndices(scanType, rowIndex, colIndex);

        if (!IsPrevTileBeyondScope(scanType, previousTileRowIndex, previousTileColIndex))
        {
            var prevTile = board.Tile2dArray[previousTileRowIndex, previousTileColIndex];
            board.SwitchBetweenTilePlanetRefs(currentTile, prevTile);
            var scanResult = ScanBoard(board);
            board.SwitchBetweenTilePlanetRefs(prevTile, currentTile);
            return scanResult.DoMatchingSequencesExist;
        }

        return false;
    }

    /* returns row and col indices of previous tile */
    private (int, int) GetPreviousTileIndices(ScanType scanType, int rowIndex, int colIndex)
    {
        var previousTileRowIndex = scanType == ScanType.Horizontal
            ? rowIndex
            : rowIndex - 1;

        var previousTileColIndex = scanType == ScanType.Horizontal
            ? colIndex - 1
            : colIndex;

        return (previousTileRowIndex, previousTileColIndex);
    }

    /* checks validity of col/row index */
    private bool IsPrevTileBeyondScope(ScanType scanType, int previousTileRowIndex, int previousTileColIndex)
    {
        return (scanType == ScanType.Horizontal && previousTileColIndex < 0) ||
            (scanType == ScanType.Vertical && previousTileRowIndex < 0);
    }
}
