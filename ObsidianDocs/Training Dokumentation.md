<table>
<tr>
<th>Name</th><th>Beschreibung</th><th>Notizen</th>
</tr>
<tr><td>28_Benchmark</td><td>default Walker demo</td><td></td></tr>
<tr><td>27_Simplified</td><td>Walker demo code umstrukturiert / vereinfacht</td><td></td></tr>
<tr><td>30_SimplifiedMulti</td><td>Walker mit extra target (lookAtTarget)</td><td>lernt nicht</td></tr>
<tr><td>32_SimplifiedSlow</td><td>Max Walking speed von 5 anstatt 10</td><td>lernt schneller aber im Bereich des Zufalls (schlechter vergleich bei max steps von 10mil anstatt 30mil)</td></tr>
<tr><td>34_SimplifiedOptim</td><td>batch und buffer size verdoppelt um an verdoppelten env count anzupassen</td><td>lernt schlechter (schlechter vergleich bei max steps von 10mil anstatt 30mil)</td></tr>
<tr><td>35_SimplifiedOptim</td><td>num_epoch von 3 auf 6 erhöht</td><td>sollte schneller aber instabiler lernen, lernt ähnlich stabil aber langsamer? (schlechter vergleich bei max steps von 10mil anstatt 30mil)</td></tr>
<tr><td>36_SimplifiedOptim</td><td>num_epoch weiter auf 10 erhöht</td><td>sollte schneller aber instabiler lernen, lernt ähnlich stabil aber langsamer? (schlechter vergleich bei max steps von 10mil anstatt 30mil)</td></tr>
<tr><td>37_SimplifiedOptim</td><td>num_epoch zurück auf 3 gesetzt und batch, buffer size von 2048 auf 1024 gesetzt und max steps zurück auf 30mil gesetzt um learning rate nicht zu verfälschen</td><td></td></tr>
<tr><td>38_SimplifiedMulti</td><td>enum mit look direction als goal observation für hyper network genutzt</td><td>lernt extrem langsam vermutlich nur für sehr einfache nn und aufgaben einsetzbar</td></tr>
<tr><td>45_Base</td><td>(vereinfachte Variante) reward für hip velocity in Richtung zum Ziel</td><td>walker lernt schnell aber läuft rückwärts (da keine look direction belohnt wird)</td></tr>
<tr><td>46_Base</td><td>hip velocity reward + look direction reward / 2 </td><td>nutzt look direction reward aus und balanciert einfach auf der stelle vor und zurück</td></tr>
<tr><td>47_Base</td><td>reward range von 0..1 auf -1..1 geändert</td><td>agent versucht episode so schnell wie möglich zu beenden</td></tr>
<tr><td>54_Milestone4</td><td>Imitation von animation "kneeing soccer ball" mit phase variable</td><td>lernt einfache teile der Bewegung aber hebt das Bein nicht an</td></tr>
<tr><td>55_Milestone4</td><td>Imitation von animation "backflip" und zusätzlicher Initialisierung an zufälligen posen in der animation</td><td>Codefehler Initialisierung hat nicht funktioniert</td></tr>
<tr><td>56_Milestone4</td><td>Imitation von animation "backflip" und zusätzlicher Initialisierung an zufälligen posen in der animation</td><td>Codefehler behoben aber lernt backflip auch nicht ganz</td></tr>
<tr><td>57_Milestone4</td><td>Observations an Aufgabe angepasst</td><td>lernt besser aber kann immer noch kein backflip nach 100mil steps</td></tr>
<tr><td>58_Milestone4</td><td>Observations und rewards an Deep Mimic Paper angeglichen und animation zu "jump" geändert um es etwas zu erleichtern</td><td>springt nach 60mil steps immer noch nicht</td></tr>
<tr><td>59_Milestone4</td><td>Joints Winkelbegrenzungen entfernt (Alle Joints 3 dof außer Ellenbogen und Knie haben 1 dof und Schultern sind fixiert. Zudem spring, dampen und max strength für alle Joints verdoppelt</td><td>lernt auch nicht zu springen</td></tr>
<tr><td>61_Milestone4</td><td>Walker LookAtTarget & WalkingSpeed Reward mit Imitation reward von Deep Mimic Paper kombiniert und animation zu walking animation gewechselt (gerade aus laufen)</td><td>Denkfehler (kann nicht zufällige geschwindigkeit halten und gleichzeitig die selbe position wie referenz halten)</td></tr>
<tr><td>62_Milestone4</td><td>walkingSpeed an referenz Charakter angepasst</td><td></td></tr>
<tr><td>63_Milestone4</td><td>WalkingSpeed an Referenz Bodypart Geschwindigkeit angepasst / run um stats recorder zu testen</td><td></td></tr>
<tr><td>64_Milestone4</td><td>stats recorder stats unter environment/ gruppiert</td><td>fehler in code (vermutlich in reward function) oder reference velocity berechnung. Agent zittert nur auf der Stelle</td></tr>
<tr><td>65_Milestone2</td><td>lookDirectionTarget hinzugefügt welches zufällige positioniert wird. Random spawn winkel von target und lookDirectionTarget sowie agent root auf 30grad gesetzt</td><td></td></tr>
<tr><td>67_Milestone3</td><td>basic walker demo but with mixamo model</td><td>lernt sich auf das ziel zu zu bewegen aber kann nicht lange die balance halten</td></tr>
<tr><td>68_WalkerDemo</td><td>walker demo von unity unverändert (mit ausnahme des statsRecorders für details über die rewards)</td><td>ausversehen walking speed Änderungen in Szene (walkingSpeed ist konstant auf 1.7 statt variable in range 0.1..10). Lernt sehr schnell sehr gutes Verhalten</td></tr>
<tr><td>69_WalkerDemo</td><td>walker demo von unity unverändert (mit ausnahme des statsRecorders für details über die rewards)</td><td>lernt match velocity reward auch nur bis 0.4</td></tr>
<tr><td>71_Milestone1_Side</td><td>milestone 1 plus look at target reward mit head.right anstatt head.forward (um seitwärtslaufen zu testen), zusätlich walking speed fix auf 1.7</td><td>hat funktioniert, aber seitwärtslaufen ist nicht sehr stabil (läuft meist nicht mehr als 1-2 schritte bevor er fällt)</td></tr>
<tr><td>72_Milestone1_Backward</td><td>milestone 1 plus look at target reward mit -head.forward anstatt head.forward (um rückwärts laufen zu testen), zusätlich walking speed fix auf 1.7</td><td></td></tr>
<tr><td>73_Milestone3</td><td>mixamo character trainiert mit num layers  auf 4 und fester walkingSpeed von 2</td><td></td></tr>
<tr><td>74_Milestone3</td><td>mixamo character trainiert mit hidden units auf 512 anstatt 256 und fester walkingSpeed von 2</td><td>lernt schlechter braucht evtl mehr samples (größerer buffer time horizon etc)</td></tr>
<tr><td>75_Milestone1</td><td>milestone 1 mit neuen Kennzahlen zur Überwachung des Trainings (Distanz zurückgelegt in Richtung dem Ziel, Ziele Erreich in Episode)</td><td></td></tr>
<tr><td>77_Milestone4</td><td>imitations lernen mit reference bodypart observations und num layers auf 4</td><td></td></tr>
<tr><td>78_Milestone4</td><td>imitations lernen mit reference bodypart observations und num layers auf 4 und fester velocity von 1,72 (gemessen von animation)</td><td></td></tr>
<tr><td>79_Milestone4</td><td>imitations lernen. rotationen werden automatisch von reference animation übernommen und nn fügt dann joint strength und rotation offset hinzu um bewegung physikalisch möglich zu machen (unter unity und joint Restriktionen) und Körper zu auszubalancieren</td><td></td></tr>
<tr><td>81_Milestone3</td><td>Mixamo Charakter auf gleiche Anzahl und limits wie Walker Demo begrenzt mit default hyperparams und WalkerAgent1</td><td></td></tr>
<tr><td>82_Milestone3</td><td>Test fixed update auf 100hz und decision request auf 50hz, zusätzlich 50 anstatt 20 envs to test speedup</td><td></td></tr>
<tr><td>83_Milestone3</td><td>Target nur respawnen wenn es erreicht wurde und beim Plaform laden einmal</td><td></td></tr>
<tr><td>84_Milestone1</td><td>Performance Vergleich mit Walker Demo (Ziel komplett identisch)</td><td>Werte in Tensorboard identisch aber in inferenz in Unity nicht vergleichbar (schlechter)</td></tr>
<tr><td>85_Milestone1</td><td>Gleich wie 84 aber mit 10 envs anstatt 50</td><td>Performanz nach Tensorboard graphen identisch aber Inferenz in Unity nicht vergleichbar (schlechter)</td></tr>
<tr><td>86_Milestone1</td><td>Rewrite mit Modulen für Erweiterbarkeit und bis auf kleine Anpassungen identisch zu Walker demo code (test training und inferenz ob identisch oder immer noch Abweichungen)</td><td>zufällig sehr guter run?</td></tr>
<tr><td>87_Milestone1</td><td>Gleich wie 86 aber mit use compatibility true</td><td>Auch nicht besser in Inferenz (Reihenfolge ist nicht das Problem)</td></tr>
<tr><td>88_WalkerSimple</td><td>Walker Demo Scripts mit automatischer Konfiguration der Körperteile über GetComponentsInChildren und Anpassung des Actions codes um allgemeiner einsetzbar zu sein (für verschiedene Körperstrukturen). <b>Training / Inferenz Performance Test und Vergleich mit Walker Demo</b></td><td>Inferenz funktioniert wie erwartet (gut).</td></tr>
<tr><td>89_WalkerSimple</td><td>88_WalkerSimple mit weiteren Codeänderungen.
* Removed direction indicator
* Removed unused variable m_WorldDirToWalk
* Added automated orientationCube creation
* Changed from using JointDriveController to using BodypartSimple
* Changed OnActionReceived to ignore all bodyparts with dof (0,0,0)
*  Removed unused targetContact reference
* Removed jointDriveControllerReference
* Added physics config to BodypartSimple
* Changed BodypartSimple class to extend MonoBehaviour
* Moved SetupBodypart into BodypartSimple Awake function
* Removed JointDriveController
* Removed unnecessary Bodypart reference from Reset function
* Added degrees of freedom variable
* Merged Ground Contact into BodypartSimple</td><td>Inferenz funktioniert wie erwartet (gut).</td></tr>
<tr><td>90_WalkerSimple</td><td>89_WalkerSImple mit weiteren Codeänderungen
* Removed unused debug variables
* Moved base Walker code to WalkerAgentSimpleBase
* Moved Walker Move to target function to Module</td><td>Inferenz funktioniert wie erwartet (gut).</td></tr>
<tr><td>Name</td><td>beschreibung</td><td>notizen</td></tr>
</table>