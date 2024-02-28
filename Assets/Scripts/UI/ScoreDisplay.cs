using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMessage;

    private StringBuilder _messageBuilder;

    private const string MESSAGE_PREFIX = "Score: ";
    private const float ANIM_DURATION = 0.5f;

    private void Awake()
    {
        _messageBuilder = new StringBuilder();
    }

    public void UpdateDisplay(int score)
    {
        PrepareMessage(score);
    }

    public void UpdateDisplay(int prevScore, int newScore)
    {
       // PrepareMessage(counter);
        AnimatePopup(prevScore, newScore);
    }

/*    private void PrepareMessage(int counter)
    {
        _messageBuilder.Clear();
        _messageBuilder.Append(MESSAGE_PREFIX);
        _messageBuilder.Append(counter);
        _textMessage.text = _messageBuilder.ToString();
    }*/

    private void AnimatePopup(int prevScore, int newScore)
    {
        DOTween.To(() => prevScore, x => prevScore = x, newScore, ANIM_DURATION)
            .OnUpdate(() => PrepareMessage(prevScore));
    }

    void PrepareMessage(int currentValue)
    {
        _messageBuilder.Clear();
        _messageBuilder.Append(MESSAGE_PREFIX);
        _messageBuilder.Append(currentValue);
        _textMessage.text = _messageBuilder.ToString();
    }

    /*    private void AnimatePopup()
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
        }*/
}
