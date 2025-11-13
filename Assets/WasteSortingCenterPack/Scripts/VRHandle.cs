using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRHandle : XRBaseInteractable
{
    [Header("Configuration de l'Axe (Espace Parent)")]
    [Tooltip("Position locale de départ (0) par rapport au parent")]
    public Vector3 startPositionLocal = Vector3.zero;

    [Tooltip("Position locale de fin (1) par rapport au parent")]
    public Vector3 endPositionLocal = new Vector3(0, 0, 0.5f);

    [Header("Retour")]
    public float returnSpeed = 5.0f;

    // Variables de calcul
    private Vector3 axisDirection;
    private float axisLength;
    private IXRSelectInteractor currentHand;
    private Coroutine returnCoroutine;

    protected override void Awake()
    {
        base.Awake();
        // On calcule le vecteur de l'axe et sa longueur une seule fois
        Vector3 diff = endPositionLocal - startPositionLocal;
        axisDirection = diff.normalized;
        axisLength = diff.magnitude;

        // On place la poignée au début par défaut
        transform.localPosition = startPositionLocal;
    }

    // --- 1. INTERACTION DÉBUT (GRAB) ---
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        currentHand = args.interactorObject;

        // On coupe le retour automatique si on attrape la poignée
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
    }

    // --- 2. INTERACTION FIN (RELEASE) ---
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        currentHand = null;

        // On lance le retour automatique
        returnCoroutine = StartCoroutine(ReturnToStart());
    }

    // --- 3. BOUCLE DE CALCUL (UPDATE VR) ---
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        // On ne calcule que pendant la phase dynamique et si on tient la poignée
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && isSelected && currentHand != null)
        {
            ProjectHandOnAxis();
        }
    }

    void ProjectHandOnAxis()
    {
        // ÉTAPE A : Changement de référentiel (Monde -> Local du Parent)
        // On utilise transform.parent pour avoir un repère fixe
        Vector3 handPosLocal = transform.parent.InverseTransformPoint(currentHand.transform.position);

        // ÉTAPE B : Projection sur l'axe
        // On calcule le vecteur entre le début de l'axe et la main
        Vector3 handToStart = handPosLocal - startPositionLocal;

        // On projette ce vecteur sur la direction de l'axe
        float projectedDistance = Vector3.Dot(handToStart, axisDirection);

        // ÉTAPE C : Limites (Clamp)
        // On empêche d'aller avant 0 ou après la fin de l'axe
        float clampedDistance = Mathf.Clamp(projectedDistance, 0f, axisLength);

        // ÉTAPE D : Application
        // Nouvelle Position = Départ + (Direction * Distance)
        Vector3 newLocalPos = startPositionLocal + (axisDirection * clampedDistance);

        transform.localPosition = newLocalPos;
    }

    // --- 4. COROUTINE DE RETOUR ---
    IEnumerator ReturnToStart()
    {
        while (Vector3.Distance(transform.localPosition, startPositionLocal) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPositionLocal, Time.deltaTime * returnSpeed);
            yield return null;
        }
        transform.localPosition = startPositionLocal;
    }

    // --- 5. VISUEL DANS L'ÉDITEUR (GIZMOS) ---
    void OnDrawGizmosSelected()
    {
        // Affiche la ligne verte seulement si on a un parent
        if (transform.parent != null)
        {
            Gizmos.color = Color.green;
            // On convertit les points locaux en position monde pour l'affichage
            Vector3 p1 = transform.parent.TransformPoint(startPositionLocal);
            Vector3 p2 = transform.parent.TransformPoint(endPositionLocal);

            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawWireSphere(p1, 0.02f); // Début
            Gizmos.DrawWireSphere(p2, 0.02f); // Fin
        }
    }
}