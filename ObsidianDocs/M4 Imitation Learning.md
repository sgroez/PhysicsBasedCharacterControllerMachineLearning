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