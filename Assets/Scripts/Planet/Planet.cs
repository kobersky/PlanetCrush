using UnityEngine;

/* planet is the main gameplay object.
  Players need to match planets of the same type */
[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Animator _animator;

    public PlanetData PlanetData;

    public void Init(PlanetData planetData)
    {        
        PlanetData = planetData;
    }

    public PlanetType GetPlanetType()
    {
        return PlanetData.PlanetType;
    }

    private void Start()
    {
        _spriteRenderer.sprite = PlanetData.PlanetSprite;
    }

    private void OnEnable()
    {
        StartIdleAnimation();
    }

    //for testing/designing purposes, the sprite will match the type event while editing
    private void Update()
    {
#if UNITY_EDITOR
        _spriteRenderer.sprite = PlanetData.PlanetSprite;
#endif
    }

    private void StartIdleAnimation()
    {
        var randomIdleAnimation = UnityEngine.Random.Range(0, 2);

        var trigger = randomIdleAnimation == 0 ? AnimationKeys.IDLE1 : AnimationKeys.IDLE2;
        _animator.SetTrigger(trigger);
    }

    public void StartCrushAnimation()
    {        
        _animator.SetTrigger(AnimationKeys.CRUSH);
    }

}
