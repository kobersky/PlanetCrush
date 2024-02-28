using DG.Tweening;
using TMPro;
using UnityEngine;

public class ReshufflingMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMessage;

    private const float POPOUP_IN_DURATION = 1.25f;
    private const float POPOUP_OUT_DURATION = 0.5f;


    public void DisplayMessage()
    {
        _textMessage.transform.localScale = Vector3.zero;
        _textMessage.enabled = true;

        var inTween = transform
            .DOScale(Vector3.one, POPOUP_IN_DURATION)
            .SetEase(Ease.OutBack)
            .Pause();

        var outTween = transform
            .DOScale(Vector3.zero, POPOUP_OUT_DURATION)
            .Pause();

        var seq = DOTween.Sequence();
        seq
            .Append(inTween)
            .Append(outTween);
    }
}

