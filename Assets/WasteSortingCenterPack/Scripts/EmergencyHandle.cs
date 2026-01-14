using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class EmergencyHandle : XRBaseInteractable
{
    [Header("Connexion")]
    public TreadmillsController controller;
    public float pauseDuration = 5.0f;

    [Header("Configuration Axe")]
    public Vector3 startPosLocal = new Vector3(0, -0.068f, 0); // Position fermée
    public Vector3 endPosLocal = new Vector3(0, 0.051f, 0); // Position tirée

    [Header("Retour")]
    public float returnSpeed = 5.0f;

    private Vector3 axis;
    private float maxLength;
    private IXRSelectInteractor hand;
    private bool hasTriggered = false; // Pour ne pas déclencher 50 fois par seconde

    protected override void Awake()
    {
        base.Awake();
        Vector3 diff = endPosLocal - startPosLocal;
        axis = diff.normalized;
        maxLength = diff.magnitude;
        transform.localPosition = startPosLocal;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        hand = args.interactorObject;
        StopAllCoroutines();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        hand = null;
        hasTriggered = false;
        StartCoroutine(ReturnToStart());
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected && hand != null)
        {
            MoveHandle();
        }
    }

    void MoveHandle()
    {
        Vector3 handLocal = transform.parent.InverseTransformPoint(hand.transform.position);
        Vector3 vectorToHand = handLocal - startPosLocal;

        float projectedDist = Vector3.Dot(vectorToHand, axis);
        float clampedDist = Mathf.Clamp(projectedDist, 0, maxLength);

        transform.localPosition = startPosLocal + (axis * clampedDist);

        if (clampedDist >= (maxLength * 0.95f) && !hasTriggered)
        {
            TriggerStop();
        }
    }

    void TriggerStop()
    {
        hasTriggered = true;
        if (controller != null)
        {
            controller.TriggerEmergencyStop(pauseDuration);
        }
    }

    IEnumerator ReturnToStart()
    {
        while (Vector3.Distance(transform.localPosition, startPosLocal) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosLocal, Time.deltaTime * returnSpeed);
            yield return null;
        }
        transform.localPosition = startPosLocal;
    }
    void OnDrawGizmosSelected()
    {
        if (transform.parent != null)
        {
            Gizmos.color = Color.red;
            Vector3 p1 = transform.parent.TransformPoint(startPosLocal);
            Vector3 p2 = transform.parent.TransformPoint(endPosLocal);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawWireSphere(p2, 0.03f);
        }
    }
}