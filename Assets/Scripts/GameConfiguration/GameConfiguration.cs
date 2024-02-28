using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* a configuration of various gameplay parameters*/
[CreateAssetMenu]
public class GameConfiguration : ScriptableObject
{
    public List<PlanetData> PlanetDataList;
    public int NumOfRows;
    public int NumOfCols;
    public int TotalMoves;
}
