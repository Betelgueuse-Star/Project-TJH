using UnityEngine;

public class ObjectInteractable : MonoBehaviour, IInteractable
{
    private Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }


    public void Interact(Player player)
    {
        Debug.Log("Interacted with " + gameObject.name);
        // Add your interaction logic here, such as opening a door, picking up an item, etc.
        myAnimator.SetTrigger("Interact");
    }

    
}
