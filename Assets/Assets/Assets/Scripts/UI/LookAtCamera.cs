using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode 
    {
        LookAt,
        LookAtInverted,
        CameraFoward,
        CameraFowardInverted,
    }

    [SerializeField] private Mode mode; 

    private void LateUpdate() // LateUpate é o Ultimo Update Chamado, então é o melhor momento para atualizar a posição de câmeras
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 dirFromCamera = this.transform.position - Camera.main.transform.position; //da a direcao de onde a camera esta apontando
                transform.LookAt(this.transform.position + dirFromCamera);
                break;
            case Mode.CameraFoward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraFowardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
            default:
                break;
        }
    }
}
