using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRSpeedLever : XRBaseInteractable
{
    [Header("Références")]
    [Tooltip("L'objet qui pivote (le manche du levier)")]
    [SerializeField] Transform handle;
    [Tooltip("Le script qui gère les tapis roulants")]
    [SerializeField] TreadmillsController treadmillController;

    [Header("Paramètres du Levier")]
    [Tooltip("L'axe de rotation local")]
    [SerializeField] Vector3 rotationAxis = Vector3.right;
    [Tooltip("Angle minimum (ex: 0 ou -90)")]
    [SerializeField] float minAngle = -90f;
    [Tooltip("Angle maximum (ex: 180 ou 90)")]
    [SerializeField] float maxAngle = 90f;
    [Tooltip("Angle de départ")]
    [SerializeField] float moyAngle = 0f;

    [Header("Debug")]
    [Tooltip("on peut utiliser ce curseur en mode Play pour tester la vitesse sans le casque VR")]
    [Range(0f, 1f)]
    [SerializeField] private float debugSpeedRatio = 0.5f;

    private float currentAngle;
    private IXRSelectInteractor interactorHand;
    protected override void Awake()
    {
        base.Awake();
        currentAngle = moyAngle;
        ApplyRotation(currentAngle);
        debugSpeedRatio = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        interactorHand = args.interactorObject;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        interactorHand = null;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            // CAS 1 : Le levier est attrapé par la main VR
            if (isSelected && interactorHand != null)
            {
                UpdateLeverFromHand(interactorHand.transform.position);
            }
            // CAS 2 : Le levier est libre -> on écoute le curseur de l'Inspector
            else
            {
                UpdateLeverFromInspector();
            }
        }
    }

    public void UpdateLeverFromHand(Vector3 targetPosition)
    {
        Vector3 localTargetPos = transform.InverseTransformPoint(targetPosition);
        float targetAngle = 0f;

        if (rotationAxis == Vector3.right || rotationAxis == -Vector3.right)
        {
            targetAngle = Vector3.SignedAngle(Vector3.up, localTargetPos, Vector3.right);
        }
        else
        {
            Vector3 direction = localTargetPos.normalized;
            targetAngle = Vector3.Angle(Vector3.up, direction);
        }

        currentAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);
        debugSpeedRatio = Mathf.InverseLerp(minAngle, maxAngle, currentAngle);
        ApplyValues();
    }
    private void UpdateLeverFromInspector()
    {
        currentAngle = Mathf.Lerp(minAngle, maxAngle, debugSpeedRatio);
        ApplyValues();
    }
    private void ApplyValues()
    {
        ApplyRotation(currentAngle);
        if (treadmillController != null)
        {
            treadmillController.SetTargetSpeedRatio(debugSpeedRatio);
        }
    }

    private void ApplyRotation(float angle)
    {
        Quaternion targetRot = Quaternion.AngleAxis(angle, rotationAxis);
        handle.localRotation = targetRot;
    }

}