using UnityEngine;

/* PlanetData is the data which identifies a planet*/
[CreateAssetMenu]
public class PlanetData : ScriptableObject
{
    public PlanetType PlanetType;
    public Sprite PlanetSprite;
}
