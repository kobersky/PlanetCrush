using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

/* Board is composed of tiles,
 * and allows erforming basic operations on planets*/
public class Board : MonoBehaviour
{
    [SerializeField] Tile _tilePrefab;
    public Tile[,] Tile2dArray { get; private set; }

    public int NumOfRows { get; private set; }
    public int NumOfCols { get; private set; }

    private PlanetObjectPool _planetPool;

    public void Init(PlanetObjectPool planetPool, int numOfRows, int numOfCols)
    {
        NumOfRows = numOfRows;
        NumOfCols = numOfCols;
        _planetPool = planetPool;
        Tile2dArray = new Tile[NumOfRows, NumOfCols];

        for (int rowIndex = 0; rowIndex < NumOfRows; rowIndex++)
        {
            for (int colIndex = 0; colIndex < NumOfCols; colIndex++)
            {
                var newTile = Instantiate(_tilePrefab, transform);
                newTile.transform.position = new Vector3(colIndex - NumOfCols / 2, rowIndex - NumOfRows / 2, 0);
                Tile2dArray[rowIndex, colIndex] = newTile;
            }
        }
    }

    public void PopulateTilesWithPlanets()
    {
        for (int rowIndex = 0; rowIndex < NumOfRows; rowIndex++)
        {
            for (int colIndex = 0; colIndex < NumOfCols; colIndex++)
            {
                var currentTile = Tile2dArray[rowIndex, colIndex];
                var planetGO = _planetPool.GetRandomPlanet();
                var planet = planetGO.GetComponent<Planet>();
                currentTile.AssignPlanetToTile(planet);
                planet.transform.position = currentTile.GetPlanetContainerPosition();
            }
        }
    }

    public void ShufflePlanetRefsBetweenTiles(List<GameObject> planets)
    {
        for (int rowIndex = 0; rowIndex < NumOfRows; rowIndex++)
        {
            for (int colIndex = 0; colIndex < NumOfCols; colIndex++)
            {
                var currentTile = Tile2dArray[rowIndex, colIndex];
                var randomIndex = UnityEngine.Random.Range(0, planets.Count);
                var randomPlanet = planets[randomIndex];

                planets.Remove(randomPlanet);
                var planet = randomPlanet.GetComponent<Planet>();
                currentTile.AssignPlanetToTile(planet);
            }
        }
    }

    public void RemoveAllPlanets()
    {
        for (int rowIndex = 0; rowIndex < NumOfRows; rowIndex++)
        {
            for (int colIndex = 0; colIndex < NumOfCols; colIndex++)
            {
                if (Tile2dArray[rowIndex, colIndex].Planet != null)
                {
                    _planetPool.ReturnPlanetToPool(Tile2dArray[rowIndex, colIndex].Planet.gameObject);
                    Tile2dArray[rowIndex, colIndex].RemovePlanetRefFromTile();
                }
            }
        }
    }

    public void SwitchBetweenTilePlanetRefs(Tile firstTile, Tile secondTile)
    {
        var secondPlanet = secondTile.Planet;
        secondTile.AssignPlanetToTile(firstTile.Planet);
        firstTile.AssignPlanetToTile(secondPlanet);
    }

    public bool AreTilesAdjacent(Tile firstTile, Tile secondTile)
    {
        return (firstTile.GetGridPosition().x == secondTile.GetGridPosition().x - 1 && firstTile.GetGridPosition().y == secondTile.GetGridPosition().y) ||
               (firstTile.GetGridPosition().x == secondTile.GetGridPosition().x + 1 && firstTile.GetGridPosition().y == secondTile.GetGridPosition().y) ||
               (firstTile.GetGridPosition().y == secondTile.GetGridPosition().y - 1 && firstTile.GetGridPosition().x == secondTile.GetGridPosition().x) ||
               (firstTile.GetGridPosition().y == secondTile.GetGridPosition().y + 1 && firstTile.GetGridPosition().x == secondTile.GetGridPosition().x);
    }
}
