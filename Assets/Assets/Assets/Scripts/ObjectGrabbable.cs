using UnityEngine;
using System;
using ITISKIRUHERE;

public class ObjectGrabbable : MonoBehaviour
{

    [Header("Throw Settings")]
    [SerializeField] private bool canBeThrown = true;

   private GameObject player;
   
   private Rigidbody rb;
   private Transform ObjectGrabPointTransform;

   private const string PLAYER = "Player";

   public bool IsBeingHeld => ObjectGrabPointTransform != null;
   public bool CanBeThrown => canBeThrown;

   public event Action OnGrabbed;
   public event Action OnDropped;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag(PLAYER);
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (ObjectGrabPointTransform != null) //verifica se existe um ponto de agarrar antes de mover o objeto
        {
            float lerpSpeed = 10f; //velocidade de movimento do objeto
            //move de um lugar para o outro, por ultimo a velocidade do movimento
            Vector3 newPosition = Vector3.Lerp(this.transform.position, ObjectGrabPointTransform.position, Time.fixedDeltaTime * lerpSpeed);

            rb.MovePosition(newPosition); //moveposition pede um vetor3, então passamos a posição de onde queremos que ele va
            //se tiver travando, no rb do inspertor

            //rb.MoveRotation(ObjectGrabPointTransform.rotation);
        }
    }   

    private void SetPlayerCollision(bool ignore)
    {
       Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
       Collider[] objectColliders = GetComponentsInChildren<Collider>();

       foreach (Collider playerCollider in playerColliders)
       {

           foreach (Collider objectCollider in objectColliders)
          {
              Debug.Log($"{playerCollider.name} x {objectCollider.name} | Ignore: {ignore}");
               Physics.IgnoreCollision(playerCollider, objectCollider, ignore);
          }
       }
    }

    public void OnGrab(Transform grabPoint)
    {
        this.ObjectGrabPointTransform = grabPoint;
        rb.useGravity = false; //desativa a gravidade do objeto, pois caso ativa ele vai cair enquanto estiver segurando
        SetPlayerCollision(true);// desativa a collisao entre o objeto e o player

        OnGrabbed?.Invoke();
    }

    public void OnDrop()
    {
        this.ObjectGrabPointTransform = null;
        rb.useGravity = true;
        SetPlayerCollision(false); //reative a collisao entre o objeto e o player

        OnDropped?.Invoke();
    }

    public void Throw(Vector3 direction, float force)
    {
        if (!canBeThrown || !IsBeingHeld)
            return;

        OnDrop();

        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
    }
}
