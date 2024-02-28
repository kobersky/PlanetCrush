using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Linq;

/* Handles gameplay flow - aftermath of matches, gameplay ui update, etc. */
public class GamePlayManager : MonoBehaviour
{
    [SerializeField] Board _board;
    [SerializeField] PlanetObjectPool _planetPool;

    [SerializeField] InputManager _inputManager;
    [SerializeField] UIManager _uiManager;

    public static event Action<int> OnOutOfMoves;

    private BoardScanner _boardScanner;
    private BoardRearranger _boardRearranger;

    private int _totalMovesAllowed;

    private int _movesLeft;
    private int _score;

    public void Init(GameConfiguration gameConfiguration)
    {
        _totalMovesAllowed = gameConfiguration.TotalMoves;
        _planetPool.InitPool(gameConfiguration.PlanetDataList);
        _board.Init(_planetPool, gameConfiguration.NumOfRows, gameConfiguration.NumOfCols);
        _boardScanner = new BoardScanner();
        _boardRearranger = new BoardRearranger(_planetPool);
    }

    private void OnEnable()
    { 
        InputManager.OnClickablesChosenByPlayer += OnClickablesChosenByPlayer;
    }

    private void OnDisable()
    {
        InputManager.OnClickablesChosenByPlayer -= OnClickablesChosenByPlayer;
    }

    public void StartNewGame()
    {
        _inputManager.EnablePlayerInput();

        ResetGameplayParams();
        PopulateBoard();
    }

    /* reset score and moves left */
    private void ResetGameplayParams()
    {
        _movesLeft = _totalMovesAllowed;
        _score = 0;

        _uiManager.RefreshMovesLeftDisplay(_movesLeft);
        _uiManager.ResetScoreDisplay();
    }

    /* Populate board with planets*/
    private void PopulateBoard()
    {

        var successfullyPopulated = false;
        //repopulate until a valid population is found (statistically, few attempts are needed)
        while (!successfullyPopulated)
        {

            _board.RemoveAllPlanets();
            _board.PopulateTilesWithPlanets();
            var scanResult = _boardScanner.ScanBoard(_board);
            Debug.Log($"GamePlayManager: Populating, matches: {scanResult.MatchingSequences.Count}");

            successfullyPopulated = !scanResult.DoMatchingSequencesExist;
        }
    }

    /* Once detected intent of player to switch betwen 2 planets, proceed diong that */
    private void OnClickablesChosenByPlayer(IClickable originClickable, IClickable targetClickable)
    {
        var originTile = originClickable as Tile;
        var targetTile = targetClickable as Tile;
        bool areTilesAdjacent = _board.AreTilesAdjacent(originTile, targetTile);
        if (areTilesAdjacent) TrySwitchingAdjacentPlanets(originTile, targetTile).Forget();
    }



    /* switch between 2 planets, and process results */
    private async UniTask TrySwitchingAdjacentPlanets(Tile originTile, Tile targetTile)
    {
        _inputManager.DisablePlayerInput();

        await _boardRearranger.SwitchPlanets(_board, originTile, targetTile);
        var scanResult = _boardScanner.ScanBoard(_board);

        // start board processing if switch creates a match
        if (scanResult.DoMatchingSequencesExist)
        {
            await ProcessCreatedMatchConsequences(scanResult);
        }
        //switch planets back if there is no match
        else
        {
            await _boardRearranger.SwitchPlanets(_board, targetTile, originTile);
        }

        if (_movesLeft == 0)
        {
            OnOutOfMoves.Invoke(_score);
            _inputManager.DisablePlayerInput();
        }
        else 
        { 
            _inputManager.EnablePlayerInput();
        }
    }

    /* handle all aftermath resulted after a match is made*/
    private async Task ProcessCreatedMatchConsequences(ScanResult scanResult)
    {
        var totalCrushCounter = 0;

        /* after each match, more matches may appear. 
         * keep processing until no mathces are left */
        while (scanResult.DoMatchingSequencesExist)
        {
            HandleSpecialMatches(scanResult);
            UpdateScore(scanResult);

            totalCrushCounter += scanResult.MatchingSequences.Count;
            await _boardRearranger.ProcessBoardAfterMatch(_board, scanResult, true);

            scanResult = _boardScanner.ScanBoard(_board);
        }

        _uiManager.DisplayTotalRoundMessages(totalCrushCounter);

        UpdateMovesLeft();

        var areOptionsLeft = _boardScanner.DoesBoardEnableValidMoves(_board);
        var areMovesLeft = _movesLeft > 0;

        if (!areOptionsLeft && areMovesLeft) await ReshuffleBoard();
    }

    /* Handle special matches, i.e long matches and matches which cross between themselves */
    private void HandleSpecialMatches(ScanResult scanResult)
    {
        if (scanResult.DoCrossMatchesExist)
        {
            foreach (var crossMatch in scanResult.CrossMatches)
            {
                _uiManager.DisplaySpecialMatchMessage(crossMatch.transform.position, SpecialMatchType.Cross);
            }
        }
        if (scanResult.DoExtraLongMatechingSequencesExist)
        {
            foreach (var specialSequence in scanResult.LongMatchingSequences)
            {
                _uiManager.DisplaySpecialMatchMessage(specialSequence.First().transform.position, SpecialMatchType.Long);
            }
        }
    }

    /* Shuffle all planets, so that the new board arrangement will not have immediate matches,
     * yet will still allow the player to perform a meaningful move*/
    public async UniTask ReshuffleBoard()
    {
        _uiManager.DisplayReshufflingMessage();

        var validShuffle = false;
        var shuffleCounter = 0;
        //shuffle board until is valid (statistically, few attempts are needed)
        while (!validShuffle)
        {
            shuffleCounter++;
            _board.ShufflePlanetRefsBetweenTiles(_planetPool.GetAllActivePlanets());
            var scanResult = _boardScanner.ScanBoard(_board);

            var doValidOptionsExist = _boardScanner.DoesBoardEnableValidMoves(_board);
            validShuffle = !scanResult.DoMatchingSequencesExist && doValidOptionsExist;
            Debug.Log($"GamePlayManager: Shuffle no. {shuffleCounter} - doImmediateMatchesExist: {scanResult.DoMatchingSequencesExist}, doValidOptionsExist: {doValidOptionsExist}...");
        }
        await _boardRearranger.MoveAssignedPlanetsToTilePositions(_board, true, true);        
    }

    private void UpdateScore(ScanResult scanResult)
    {
        var beforeScore = _score;
        _score += scanResult.TotalMatchedTiles;
        _uiManager.RefreshScoreDisplay(beforeScore, _score);
    }

    private void UpdateMovesLeft()
    {
        _movesLeft--;
        _uiManager.RefreshMovesLeftDisplay(_movesLeft);
    }
}
