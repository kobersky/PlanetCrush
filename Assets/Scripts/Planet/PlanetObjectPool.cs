using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* an object pool for planets*/
public class PlanetObjectPool : MonoBehaviour
{
    private const int INITAL_PLANETS_PER_TYPE = 5;

    [SerializeField] GameObject _planetPrefab;
    [SerializeField] Transform _planetContainer;

    private List<PlanetData> _planetDataList;
    private Dictionary<PlanetType, List<GameObject>> _planetPool = new Dictionary<PlanetType, List<GameObject>>();

    public void InitPool(List<PlanetData> planetDataList)
    {       
        _planetDataList = planetDataList;

        foreach (var planetData in _planetDataList)
        {
            var planetsOfCurrentType = new List<GameObject>();
            for (int i = 0; i < INITAL_PLANETS_PER_TYPE; i++)
            {
                var planetGO = InstantiateNewPlanet(planetData);
                planetsOfCurrentType.Add(planetGO);
            }

            _planetPool.Add(planetData.PlanetType, planetsOfCurrentType);
        }
    }
    
    public void ReturnPlanetToPool(GameObject planet)
    {
        planet.transform.parent = _planetContainer;
        planet.gameObject.SetActive(false);
    }

    public List<GameObject> GetAllActivePlanets()
    { 
        List<GameObject> planetList = new List<GameObject>();
        foreach (var kvp in _planetPool) 
        {
            foreach (var planet in kvp.Value)
            {
                if (planet.activeInHierarchy)
                {
                    planetList.Add(planet);
                }
            }
        }
            
        return planetList;
    }


    public GameObject GetRandomPlanet()
    {
        var planetIndex = Random.Range(0, _planetDataList.Count);
        return GetPlanet(_planetDataList[planetIndex].PlanetType);
    }

    public GameObject GetPlanet(PlanetType planetType) 
   {
        var matchingPlanets = _planetPool[planetType];

        GameObject planetGO = null;

        foreach (var currentPlanet in matchingPlanets)
        {
            if (!currentPlanet.activeInHierarchy)
            {
                planetGO = currentPlanet;
                break;
            }
        }

        if (planetGO == null)
        { 
            var matchingPlanetData = _planetDataList.Find(currentPlanetData => currentPlanetData.PlanetType == planetType);
            planetGO = InstantiateNewPlanet(matchingPlanetData);
            matchingPlanets.Add(planetGO);
        }

        planetGO.SetActive(true);
        return planetGO;
    }

    
    private GameObject InstantiateNewPlanet(PlanetData planetData)
    {
        var planetGO = Instantiate(_planetPrefab);
        planetGO.GetComponent<Planet>().Init(planetData);
        planetGO.transform.SetParent(_planetContainer);
        planetGO.SetActive(false);

        return planetGO;
    }
}
