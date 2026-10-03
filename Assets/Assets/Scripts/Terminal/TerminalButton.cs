
using UnityEngine;
using UnityEngine.Events;

public class ComputerButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;

    public void Click() 
    {
        onClick?.Invoke();
    }
}