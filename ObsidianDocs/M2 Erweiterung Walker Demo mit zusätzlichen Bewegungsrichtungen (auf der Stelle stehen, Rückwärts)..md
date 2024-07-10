# Anforderungen
Bewegt sich mit targetWalkingSpeed in Richtung Target
Schaut in Richtung von lookAtTarget
Bewegt sich stabil (fällt kaum)

# Reward
positiver Reward für walkingSpeed ähnlich zu targetWalkingSpeed in Richtung Target
positiver Reward für geringe Abweichung von Blickrichtung zu lookAtTarget - Neigungsabweichung von horizontaler Blickrichtung
negativer Reward für fallen

# Targets
Target wechselt Position wenn erreicht
LookAtTarget wechselt Position wenn Avg lookAtTargetReward > Value bei Episodenende oder Target Position erreicht?
Oder
LookAtTarget unterschiedlicher Winkel bei allen Training Environments um gleichzeitig unterschiedliche Blickwinkel zu erlernen?

# Versuche
## Versuch 1
### Auf Stelle stehen
Wenn Distance(agent.position, target.position) == 0f target velocity auf 0 setzen.

### Rückwärts laufen
Wenn Quaternion.Angle(agent.rotation, target.rotation) > 160 target velocity negieren.
Look at target reward head direction umdrehen.

```
Vector3 GetGoalVelocity()
    {
        var cubeForward = m_OrientationCube.transform.forward;
        //add backwards walk (negative goal velocity)
        if (Quaternion.Angle(m_JdController.bodyPartsDict[hips].rb.transform.rotation, m_OrientationCube.transform.rotation) > 160f)
        {
            return -cubeForward * MTargetWalkingSpeed;
        }
        //add standing still (0, 0, 0 goal velocity)
        if (Vector3.Distance(m_JdController.bodyPartsDict[hips].rb.transform.position, target.position) < 0.1f)
        {
            return Vector3.zero;
        }
        return cubeForward * MTargetWalkingSpeed;
    }
    
Inside Fixed Update:
//add backwards walk (negating head forward)
        if (Quaternion.Angle(m_JdController.bodyPartsDict[hips].rb.transform.rotation, m_OrientationCube.transform.rotation) > 160f)
        {
            headForward = -headForward;
        }
```

!!! Denkfehler da negieren der target velocity dazu führt das der Agent sich von dem Ziel entfernt und nicht rückwärts zum Ziel läuft. Grund dafür ist das die Velocity die **Bewegungsrichtung in relation zum Szenenursprung * Geschwindigkeit** ist und somit nicht abhängig von der Körper Orientierung ist.
!!!

## Versuch 2
### Auf Stelle stehen
Wenn Distance < slowDownDistance dann je nach Distanz die Geschwindigkeit verringern

### Unterschiedliche Laufrichtungen
One hot encoded enum mit Laufrichtung (vorwärts, rechts, rückwärts, links) in observation hinzugefügt.
Laufrichtung ändert look direction und somit auch den look at target reward.

### Ergebnis
- Funktioniert teilweise training / model performance muss verbessert werden.
- Stehen bleiben führt teilweise dazu das model bei laufen zum ziel das ziel am ende in sehr geringem Abstand umrundet.
- Funktioniert nur in seperaten modeln (Model kann in curriculum nicht von einer Laufrichtung auf andere schließen und muss daher alles entlernen und neu lernen -> model kann nach 100mio steps weder vorwärts noch rückwärts laufen)

## Versuch 3
### Unterschiedliche Laufrichtungen
Agent bekommt in Observation look direction Vector (Position von extra look at target) der an einer zufälligen stelle positioniert wird sodass look direction und walk direction getrennt voneinander funktionieren

## Versuch 4
Hypernetwork mit Goal Observation nutzen

### Ergebnis
Trainiert etwa 6 mal langsamer (nicht sinnvoll ohne evtl. mögliche Optimierungen)

## Versuch 5
Versuch 3 mit curriculum erweitern (target spawn angle)
Target spawn angle noch von Agent position abhängig machen (bzw agent ausrichtung an target spawn angle anpassen)

# Ergebnis
## Seperate bewegungen
Walker lernt alle bewegungen seperat (vorwärts, seitwärts, rückwärts laufen oder auf der Stelle stehen)
Einige Bewegungen sind für den Walker Agent schwieriger wie Seitwärtslaufen, liegt evtl und joint limits?

## Bewegungen kombinieren
### Manuell zwischen Modellen wechseln
#### Vorteile: 
- Einfach im Code zu implementieren, aber Logik wann welches Modell verwendet wird muss manuell festgelegt werden.
#### Nachteile:
- Stabilitäts Probleme währen Wechsel zwischen Modellen.
### NN Model trainieren was je nach Ziel zwischen Modellen wechselt
#### Vorteile:
- Übergänge zwischen Modell wechsel kann mit Reward von NN optimiert werden.
#### Nachteile:
- Vermutlich komplexere Implementierung und unklar welche Observationen verwendet werden können um ein NN zu trainieren ohne das Observationen eindeutig auf Modell zu zuweisen sind (da sonst NN überflüssig).
### Hyper Network zum lernen von mehreren Bewegungen über GoalObservation Vektor
#### Vorteile:
- Von Ml-Agents unterstützt und bereits implementiert
- keine Übergänge zwischen Modellen
#### Nachteile:
- Traininszeit steigt um ein vielfaches
#### Optimierungsmöglichkeiten:
-NN Struktur vereinfachen (weniger Layer und hidden units)
### Alle Bewegungen zu einem Komplexen Goal zusammenführen
Schwer zu trainieren, evtl. mit besserer NN Hyperparameter Konfiguration möglich?
#### Vorteile:
- keine Übergänge zwischen Modellen
- Modell kann generalisieren zwischen gewünschten Bewegungen
#### Nachteile:
- Trainingskomplexität
- Trainingsdauer
#### Optimierungsmöglichkeiten:
- NN Struktur anpassen
- Hyperparameter tuning
- Curriculum learning für einfachere Probleme zum Start