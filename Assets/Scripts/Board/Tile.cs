using UnityEngine;

/* A tile is represented by a rectangle in a fixed position on the screen,
 * and is a resting place for a planet. Tiles may have it's planet
 * switched by another planet during gameplay */
public class Tile : MonoBehaviour, IClickable
{
    [SerializeField] GridObject _gridObject;
    [SerializeField] Transform _planetContainer;

    public Planet Planet { get; private set; }


    public void AssignPlanetToTile(Planet newPlanet)
    {
        newPlanet.transform.parent = _planetContainer.transform;
        Planet = newPlanet;
    }

    public void RemovePlanetRefFromTile()
    {
        Planet = null;
    }

    public Vector3 GetGridPosition()
    {
        return _gridObject.GridPosition;
    }

    public Vector3 GetPlanetContainerPosition()
    {
        return _planetContainer.position;
    }


}
