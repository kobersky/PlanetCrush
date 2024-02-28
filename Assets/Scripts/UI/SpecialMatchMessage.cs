using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SpecialMatchMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMessage;

    private StringBuilder _messageBuilder;

    private const string LONG_SEQUENCE_MESSAGE = "Long Sequence!";
    private const string CROSS_SEQUENCE_MESSAGE = "Crossing Match!";

    private const float ANIMATION_IN_DURATION = 1f;
    private const float ANIMATION_OUT_DURATION = 0.25f;
    private const int ANIMATION_FLOAT_HEIGHT = 50;

    private void Awake()
    {
        _messageBuilder = new StringBuilder();
    }

    private void OnEnable()
    {
        var textColor = _textMessage.color;

        textColor.a = 0f; 
        _textMessage.color = textColor;
    }

    public void DisplayMessage(Vector3 position, SpecialMatchType specialMatchType, System.Action<SpecialMatchMessage> onDoneDisplayingSpecialMessage)
    {
        transform.position = Camera.main.WorldToScreenPoint(position);
        PrepareMessage(specialMatchType);
        AnimateMessage(onDoneDisplayingSpecialMessage);
    }

    private void PrepareMessage(SpecialMatchType specialMatchType)
    {
        _messageBuilder.Clear();
        var text = specialMatchType == 
            SpecialMatchType.Long ?
            LONG_SEQUENCE_MESSAGE :
            CROSS_SEQUENCE_MESSAGE;

        _messageBuilder.Append(text);
        _textMessage.text = _messageBuilder.ToString();
    }

        private void AnimateMessage(System.Action<SpecialMatchMessage> onDoneDisplayingSpecialMessage)
        {
            var inTween = _textMessage
                .DOFade(1, ANIMATION_OUT_DURATION)
                .Pause();

            var targetY = transform.position.y + ANIMATION_FLOAT_HEIGHT;
            var inTween2 = transform
                .DOMoveY(targetY, ANIMATION_IN_DURATION)
                .Pause();

            var outTween = _textMessage
                .DOFade(0, ANIMATION_IN_DURATION)
                .Pause();

            var seq = DOTween.Sequence();
            seq
            .Append(inTween)
            .Append(inTween2)
            .Join(outTween)
            .OnComplete(() => onDoneDisplayingSpecialMessage.Invoke(this));
        }
}
