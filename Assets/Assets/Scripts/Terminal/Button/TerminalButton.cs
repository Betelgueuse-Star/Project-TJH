
using UnityEngine;
using UnityEngine.Events;

public class TerminalButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;

    public void Click() 
    {
        onClick?.Invoke();
    }
}