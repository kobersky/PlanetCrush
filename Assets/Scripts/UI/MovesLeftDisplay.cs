using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MovesLeftDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMessage;

    private StringBuilder _messageBuilder;

    private const string MESSAGE_PREFIX = "Moves Left: ";
    private const float ANIM_DURATION = 0.25f;

    private Vector3 _maxScaleSize = new Vector3(1.2f, 1.2f, 1.2f);

    private void Awake()
    {
        _messageBuilder = new StringBuilder();
    }

    public void UpdateDisplay(int counter)
    {
        PrepareMessage(counter);
        AnimateMessage();
    }

    private void PrepareMessage(int counter)
    {
        _messageBuilder.Clear();
        _messageBuilder.Append(MESSAGE_PREFIX);
        _messageBuilder.Append(counter);
        _textMessage.text = _messageBuilder.ToString();
    }

    private void AnimateMessage()
    {
        var inTween = transform
            .DOScale(_maxScaleSize, ANIM_DURATION)
            .SetEase(Ease.OutBack)
            .Pause();

        var outTween = transform
            .DOScale(Vector3.one, ANIM_DURATION)
            .SetEase(Ease.OutBack)
            .Pause();

        var seq = DOTween.Sequence();
        seq.Append(inTween).Append(outTween);
    }
}
