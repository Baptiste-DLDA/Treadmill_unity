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

    [Header("Configuration Axe (Espace Parent)")]
    public Vector3 startPosLocal = new Vector3(0, -0.068f, 0); // Position fermée
    public Vector3 endPosLocal = new Vector3(0, 0.051f, 0); // Position tirée (déclencheur)

    [Header("Retour")]
    public float returnSpeed = 5.0f;

    // Variables internes
    private Vector3 axis;
    private float maxLength;
    private IXRSelectInteractor hand;
    private bool hasTriggered = false; // Pour ne pas déclencher 50 fois par seconde

    protected override void Awake()
    {
        base.Awake();
        // Calcul de l'axe et de la longueur max
        Vector3 diff = endPosLocal - startPosLocal;
        axis = diff.normalized;
        maxLength = diff.magnitude;

        // Placement initial
        transform.localPosition = startPosLocal;
    }

    // Quand on attrape la poignée
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        hand = args.interactorObject;
        StopAllCoroutines(); // On arrête le retour automatique si on l'attrape
    }

    // Quand on lâche la poignée
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        hand = null;
        hasTriggered = false; // On reset le déclencheur pour la prochaine fois
        StartCoroutine(ReturnToStart()); // Retour auto
    }

    // Boucle de mise à jour VR
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
        // 1. Calcul de la position de la main par rapport au parent de la poignée (référentiel fixe)
        Vector3 handLocal = transform.parent.InverseTransformPoint(hand.transform.position);
        Vector3 vectorToHand = handLocal - startPosLocal;

        // 2. Projection sur l'axe
        float projectedDist = Vector3.Dot(vectorToHand, axis);
        float clampedDist = Mathf.Clamp(projectedDist, 0, maxLength);

        // 3. Déplacement visuel
        transform.localPosition = startPosLocal + (axis * clampedDist);

        // 4. DÉTECTION DU DÉCLENCHEMENT
        // Si on est tiré à plus de 95% et qu'on n'a pas encore déclenché
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
            // Appelle la fonction qu'on a créée dans l'étape 1
            controller.TriggerEmergencyStop(pauseDuration);
        }

        // Optionnel : Jouer un son, faire vibrer la manette, changer une couleur...
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

    // Pour régler les points de départ/fin facilement dans l'éditeur
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