using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform ObjectGrabPointTransform;

    private void Awake()
    {
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
    public void Grab(Transform grabPoint)
    {
        this.ObjectGrabPointTransform = grabPoint;
        rb.useGravity = false; //desativa a gravidade do objeto, pois caso ativa ele vai cair enquanto estiver segurando
    }

    public void Drop()
    {
        this.ObjectGrabPointTransform = null;
        rb.useGravity = true;
    }
}
