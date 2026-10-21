using System.Collections;
using UnityEngine;
using System;

public class Elevator : MonoBehaviour
{
    public enum ElevatorState { Bottom, Top, Moving, Between }

    public event Action ReachedTop;
    public event Action ReachedBottom;

    [Header("Elevator")]
    [SerializeField] private Rigidbody elevatorRigidbody;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;

    [Header("Delivery")]
    [SerializeField] private Transform itemSpawnPoint;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    public ElevatorState State { get; private set; } = ElevatorState.Between;
    public bool IsAtTop => State == ElevatorState.Top;
    public bool IsAtBottom => State == ElevatorState.Bottom;
    public Transform ItemSpawnPoint => itemSpawnPoint;

    // tolerância de chegada ao quadrado (0.01 * 0.01).
    // Por quê: comparar sqrMagnitude evita a raiz quadrada do Vector3.Distance a cada passo
    // da física. O comportamento é o mesmo de antes (chegou a 1 cm do destino).
    private const float ArrivalToleranceSqr = 0.01f * 0.01f;

    private Coroutine movementCoroutine;

    // para onde o elevador está indo enquanto está em Moving.
    // Por quê: permite ignorar um pedido repetido para o mesmo destino (ex.: apertar
    // o botão de subir duas vezes) em vez de reiniciar a corrotina.
    private ElevatorState currentDestination;

    private bool isConfigured;

    private void Awake()
    {
        isConfigured =
            elevatorRigidbody != null &&
            bottomPoint != null &&
            topPoint != null;

        if (!isConfigured)
        {
            Debug.LogError($"{name}: referência faltando (Rigidbody, bottomPoint ou topPoint), o elevador não vai funcionar.", this);
            return;
        }

        // Com velocidade 0 o elevador nunca chegaria ao destino (a corrotina não terminaria).
        if (moveSpeed <= 0f)
        {
            Debug.LogError($"{name}: moveSpeed precisa ser maior que 0, o elevador não vai funcionar.", this);
            isConfigured = false;
        }

        // O elevador se move sem esse ponto, mas o carrinho não consegue entregar itens.
        // Por isso é erro, mas não desliga o elevador.
        if (itemSpawnPoint == null)
            Debug.LogError($"{name}: itemSpawnPoint faltando, o carrinho não vai conseguir entregar os itens.", this);
    }

    private void Start()
    {
        if (!isConfigured)
            return;

        UpdatePositionState();
    }

    public void MoveUp()
    {
        MoveTo(topPoint, ElevatorState.Top);
    }

    public void MoveDown()
    {
        MoveTo(bottomPoint, ElevatorState.Bottom);
    }

    private void MoveTo(Transform destination, ElevatorState destinationState)
    {
        if (!isConfigured)
            return;

        if (State == destinationState)
            return;

        if (State == ElevatorState.Moving && currentDestination == destinationState)
            return;

        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        State = ElevatorState.Moving;
        currentDestination = destinationState;

        movementCoroutine = StartCoroutine(
            MoveRoutine(destination.position, destinationState)
        );
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition, ElevatorState destinationState)
    {
        // sqrMagnitude em vez de Vector3.Distance (ver ArrivalToleranceSqr).
        while ((elevatorRigidbody.position - targetPosition).sqrMagnitude > ArrivalToleranceSqr)
        {
            elevatorRigidbody.MovePosition(
                Vector3.MoveTowards(
                    elevatorRigidbody.position,
                    targetPosition,
                    moveSpeed * Time.fixedDeltaTime
                )
            );

            yield return new WaitForFixedUpdate();
        }

        elevatorRigidbody.position = targetPosition;

        movementCoroutine = null;

        // o estado vem direto do destino, não é mais recalculado com Distance.
        // Ele é definido ANTES de disparar o evento, para que quem escuta (ex.: o carrinho)
        // já encontre IsAtTop / IsAtBottom corretos dentro do callback.
        State = destinationState;

        if (destinationState == ElevatorState.Top)
        {
            ReachedTop?.Invoke();
        }
        else
        {
            ReachedBottom?.Invoke();
        }
    }

    // só roda no Start, para descobrir em que ponto a cena começou.
    // Depois disso o estado é sempre definido pelo MoveRoutine.
    private void UpdatePositionState()
    {
        if (IsNear(topPoint.position))
        {
            State = ElevatorState.Top;
        }
        else if (IsNear(bottomPoint.position))
        {
            State = ElevatorState.Bottom;
        }
        else
        {
            State = ElevatorState.Between;
            Debug.LogWarning($"{name}: o elevador começou fora do topo e do fundo. Mande-o para um dos pontos.", this);
        }
    }

    private bool IsNear(Vector3 point)
    {
        return (elevatorRigidbody.position - point).sqrMagnitude <= ArrivalToleranceSqr;
    }
}