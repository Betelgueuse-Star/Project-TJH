using System.Collections;
using UnityEngine;
using System;

public class Elevator : MonoBehaviour
{
    public event Action ReachedTop;
    public event Action ReachedBottom;

    [Header("Elevator")]
    [SerializeField] private Rigidbody elevatorRigidbody;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private Transform topPoint;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;

    public bool IsAtTop { get; private set; }
    public bool IsAtBottom { get; private set; }

    private Coroutine movementCoroutine;

    private void Start()
    {
        UpdatePositionState();
    }

    public void MoveUp()
    {
        MoveTo(topPoint.position);
    }

    public void MoveDown()
    {
        MoveTo(bottomPoint.position);
    }

    private void MoveTo(Vector3 targetPosition)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        IsAtTop = false;
        IsAtBottom = false;

        movementCoroutine = StartCoroutine(
            MoveRoutine(targetPosition)
        );
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition)
    {
        while (Vector3.Distance(
            elevatorRigidbody.position, targetPosition) > 0.01f)
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

        UpdatePositionState();
        movementCoroutine = null;

        if (IsAtTop)
        {
            ReachedTop?.Invoke();
        }
        else if (IsAtBottom)
        {
            ReachedBottom?.Invoke();
        }
    }

    private void UpdatePositionState()
    {
        IsAtTop = Vector3.Distance(
            elevatorRigidbody.position,
            topPoint.position
        ) <= 0.01f;

        IsAtBottom = Vector3.Distance(
            elevatorRigidbody.position,
            bottomPoint.position
        ) <= 0.01f;
    }
}
