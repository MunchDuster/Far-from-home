using UnityEngine;
using UnityEngine.Events;

public class OnEnableEvent : MonoBehaviour
{
    public UnityEvent OnEnabled;
    private void OnEnable() 
    {
        OnEnabled.Invoke();
    }
}
