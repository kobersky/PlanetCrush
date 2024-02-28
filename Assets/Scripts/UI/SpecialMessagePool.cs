using System;
using System.Collections.Generic;
using UnityEngine;

/* a pool for managing special match messages*/
public class SpecialMessagePool : MonoBehaviour
{
    [SerializeField] GameObject _specialMatchMessagePrefab;

    private List<GameObject> _pool;

    private void Awake()
    {
        _pool = new List<GameObject>();
    }

    public void ReturnMessageToPool(SpecialMatchMessage message)
    {
        message.gameObject.SetActive(false);
    }
    

    public SpecialMatchMessage GetMessage()
    {
        GameObject messageGO = null;

        foreach (var currentMessage in _pool)
        {
            if (!currentMessage.activeInHierarchy)
            {
                messageGO = currentMessage;
                break;
            }
        }

        if (messageGO == null)
        {
            messageGO = Instantiate(_specialMatchMessagePrefab, transform);
            _pool.Add(messageGO);
        }

        messageGO.SetActive(true);
        return messageGO.GetComponent<SpecialMatchMessage>();
    }

    internal object GetMessage(object onDoneDisplayingSpecialMessage)
    {
        throw new NotImplementedException();
    }
}
