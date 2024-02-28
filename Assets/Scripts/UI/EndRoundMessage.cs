using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;

/* a popup shown at the end of a game*/
public class EndRoundMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMessage;

    private StringBuilder _messageBuilder;

    private const string MESSAGE_PREFIX = " straight!";
    private const float POPOUP_IN_DURATION = 0.75f;
    private const float POPOUP_OUT_DURATION = 0.25f;

    private void Awake()
    {
        _messageBuilder = new StringBuilder();
    }

    public void DisplayMessage(int counter)
    {
        PrepareMessage(counter);
        AnimateMessage();        
    }

    private void PrepareMessage(int counter)
    {
        _messageBuilder.Clear();
        _messageBuilder.Append(counter);
        _messageBuilder.Append(MESSAGE_PREFIX);
        _textMessage.text = _messageBuilder.ToString();
    }

    private void AnimateMessage()
    {
        _textMessage.transform.localScale = Vector3.zero;
        _textMessage.enabled = true;

        var inTween = transform
            .DOScale(Vector3.one, POPOUP_IN_DURATION)
            .SetEase(Ease.OutElastic)
            .Pause();

        var outTween = transform
            .DOScale(Vector3.zero, POPOUP_OUT_DURATION)
            .Pause();

        var seq = DOTween.Sequence();
        seq.Append(inTween).Append(outTween);
    }
}
