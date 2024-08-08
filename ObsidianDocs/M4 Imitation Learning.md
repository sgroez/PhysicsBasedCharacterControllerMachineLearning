## Versuch 1
Charakter lernt eine statische pose (alle Gelenke auf neutraler rotation / t pose) ohne rotation als input.

## Versuch 2
Charakter lernt animation zu imitieren anhand von Phase variable (Zeitpunkt innerhalb der animation), problem lernt nur einfachen teil der Bewegung

## Versuch 3
Charakter an zufällige punkte innerhalb der Animation initialisieren um alle Bewegungsabläufe gleich oft zu trainieren und alle posen schon direkt zu durchlaufen.

### Probleme
- Charakter lässt sich nicht so einfach in pose initialisieren da vom nn model direkt die Rotationen überschrieben werden (außer Hüfte da Hüfte nicht direkt über die actions gesteuert wird sondern über die anhängenden Körperteile und die Gelenke dazwischen)
- Problem war das animation erst im nächsten frame aktuallisiert wurde und daher der rl gesteuerte Charakter auf die alte position gesetzt wurde.
- Reward und imitations Abweichung Berechnung funktioniert noch nicht richtig
- walkingSpeed muss auch beachtet und auf animation angepasst werden.


/* public override void Heuristic(in ActionBuffers actionsOut)

{

var continuousActionsOut = actionsOut.ContinuousActions;

int i = 0;

for (int j = 0; j < bodyparts.Count; j++)

{

Bodypart bp = bodyparts[j];

if (bp.dof.sqrMagnitude <= 0) continue;

ReferenceBodypart rbp = referenceController.referenceBodyparts[j];

Vector3 targetRotation = ConfigurableJointExtensions.GetTargetRotation(bp.joint, rbp.transform.localRotation, bp.startingRotLocal, Space.Self).eulerAngles;

if (bp.dof.x == 1)

{

float x = Mathf.InverseLerp(bp.joint.lowAngularXLimit.limit, bp.joint.highAngularXLimit.limit, targetRotation.x);

continuousActionsOut[i++] = 2 * x - 1;

}

if (bp.dof.y == 1)

{

float y = Mathf.InverseLerp(-bp.joint.angularYLimit.limit, bp.joint.angularYLimit.limit, targetRotation.y);

continuousActionsOut[i++] = 2 * y - 1;

}

if (bp.dof.z == 1)

{

float z = Mathf.InverseLerp(-bp.joint.angularZLimit.limit, bp.joint.angularZLimit.limit, targetRotation.z);

continuousActionsOut[i++] = 2 * z - 1;

}

continuousActionsOut[i++] = 1f;

}

} */

  

public void SetReferenceMotion()

{

for (int j = 0; j < bodyparts.Count; j++)

{

Bodypart bp = bodyparts[j];

if (bp.dof.sqrMagnitude <= 0) continue;

ReferenceBodypart rbp = referenceController.referenceBodyparts[j];

ConfigurableJointExtensions.SetTargetRotationLocal(bp.joint, rbp.transform.localRotation, bp.startingRotLocal);

bp.SetJointStrength(0.8f);

}

}