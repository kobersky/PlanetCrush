using System.Collections.Generic;
using System.Linq;

/* Holds all data needed about state of board (in terms of matches) after being scanned */
public class ScanResult 
{
    private const int MIN_MATCH_LENGTH = 3;
    private const int EXTRA_LONG_MATCH_LENGTH = 4;

    public List<List<Tile>> MatchingSequences {  get; private set; }
    public List<List<Tile>> LongMatchingSequences { get; private set; }
    public List<Tile> CrossMatches { get; private set; }

    public int TotalMatchedTiles { get; private set; }
    public bool DoMatchingSequencesExist => MatchingSequences.Count > 0;
    public bool DoExtraLongMatechingSequencesExist => LongMatchingSequences.Count > 0;
    public bool DoCrossMatchesExist => CrossMatches.Count > 0;

    public ScanResult(List<List<Tile>> allSequences)
    {
        FilterOutShortSequences(allSequences);
        CountMatchedTiles();
        FindLongSequences();
        FindCrossMatches();
    }

    private void FilterOutShortSequences(List<List<Tile>> allSequences)
    {
        MatchingSequences = allSequences
            .Where(currentSequence => currentSequence.Count >= MIN_MATCH_LENGTH)
            .ToList();
    }

    private void CountMatchedTiles()
    {
        TotalMatchedTiles = MatchingSequences
            .Sum(currentSequence => currentSequence.Count);
    }

    private void FindLongSequences()
    {
        LongMatchingSequences = MatchingSequences
            .Where(currentSequence => currentSequence.Count >= EXTRA_LONG_MATCH_LENGTH)
            .ToList();
    }

    private void FindCrossMatches()
    {
        var allMatches = new List<Tile>();
        CrossMatches = new List<Tile>();
        foreach (var sequence in MatchingSequences)
        {
            foreach (var tile in sequence)
            {
                if (allMatches.Contains(tile))
                {
                    CrossMatches.Add(tile);
                }
                else 
                { 
                    allMatches.Add(tile);
                }
            }
        }
    }
}
