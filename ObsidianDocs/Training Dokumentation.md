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
* Changed from using JointDriveController to using Bodypart
* Changed OnActionReceived to ignore all bodyparts with dof (0,0,0)
*  Removed unused targetContact reference
* Removed jointDriveControllerReference
* Added physics config to Bodypart
* Changed Bodypart class to extend MonoBehaviour
* Moved SetupBodypart into Bodypart Awake function
* Removed JointDriveController
* Removed unnecessary Bodypart reference from Reset function
* Added degrees of freedom variable
* Merged Ground Contact into Bodypart</td><td>Inferenz funktioniert wie erwartet (gut).</td></tr>
<tr><td>90_WalkerSimple</td><td>89_WalkerSImple mit weiteren Codeänderungen
* Removed unused debug variables
* Moved base Walker code to WalkerAgent1Base
* Moved Walker Move to target function to Module</td><td>Inferenz funktioniert wie erwartet (gut).</td></tr>
<tr><td>91_WalkerSimpleLong</td><td>Test mit langer Laufbahn und nur gerade aus laufen. Versuch ob schneller besser lernt und als nächsten Schritt ob gelerntes dann zu schnellerem / besserem lernen für weitere Laufmuster mit sich bringt</td><td>lernt schnell und guten Reward aber fällt oft in Inferenz</td></tr>
<tr><td>92_WalkerSimpleLong</td><td>91_WalkerSimpleLong mit extra ordering der bodyparts Liste um gleiche Reihenfolge zu gewährleisten und mit nur 10mil steps</td><td></td></tr>
<tr><td>93_WalkerSimpleLong</td><td>Gleiches Environment (gleicher build wie 92) aber mit 30mil steps um training zu vergleichen. Test von step count auf epsilon und damit training Stabilität</td><td>ist das ppo training unterschiedlich je nach bodypart Reihenfolge in Observation und Action? Oder gibt es noch Reihenfolge abhängigen code im Agent / Bodypart script?</td></tr>
<tr><td>94_WalkerSimpleLong</td><td>Gleiche wie 92 und 93 aber ohne bodypart Sortierung (standart Reihenfolge von GetComponentsInChildren) und wieder mit 10 mil max steps um kürzere trainings dauer zu testen</td><td>lernt in 10 mil steps gut, aber inferenz nur bei einem von 20 envs gut</td></tr>
<tr><td>95_Milestone2</td><td>WalkerSimpleLong nach Milestone2 verschoben und fixedTimeStep auf 60hz umgestellt und decision period von 5 auf 1 timestep verkürzt</td><td>läuft nicht so stabil (weit) aber sieht natürlicher aus?</td></tr>
<tr><td>96_Milestone2</td><td>DistributedStrengthReward hinzugefügt (nähert sich 1 wenn alle Gelenkte wenig Kraft benutzen und nähert sich 0 wenn min 1 Gelenk große Kraft aufwendet)</td><td>training mit 3 Rewards multipliziert (Gesamt Reward nur hoch wenn alle 3 Teilrewards hoch sind) ist zu schwer / komplex</td></tr>
<tr><td>97_Milestone2</td><td>Gleich wie 96 aber Rewards mit weights zusammen addiert anstatt multipliziert</td><td>endet in lokalem maximum (maximiert nur look at target reward) und optimiert bis zur maximalen episode length</td></tr>
<tr><td>98_Milestone2</td><td>Gleich wie 97 aber mit sac als training algorithmus</td><td>lernt nicht zu laufen (nutzt LookAtReward aus)</td></tr>
<tr><td>99_Milestone2</td><td>Gleich wie 97 mit neuen Reward weights um LookAtReward zu reduzieren</td><td>lernt DistributedStrength und LookAtTarget Reward (vermutlich vorherigen Versuche zu früh abgebrochen)</td></tr>
<tr><td>100_Milestone2</td><td>Look At Target mit Winkelabweichung von agent zu target implementiert und Winkel wird über curriculum learning langsam vergößert</td><td>env parameter callback funktioniert nicht (hat Winkel bei neuer lesson nicht angepasst)</td></tr>
<tr><td>103_Milestone2</td><td>Gleich wie 100 aber mit parameter setzen beim start einer neuen Episode</td><td>ignoriert look at target, läuft zum ziel schauen gerade aus. Evtl Ziel für LookAtTarget implementieren das erst position gewechselt wird wenn &gt; 10sek auf LookAtTarget geschaut</td></tr>
<tr><td>104_Milestone2</td><td>Gleich wie 103 aber ohne curriculum learning. Look At Target min max angle ist fest auf 90 gestellt</td><td>passt look Richtung leicht an</td></tr>
<tr><td>105_Milestone2</td><td>gleich wie 104 mit min max angle fest auf 180</td><td>funktioniert leicht aber umgeht look at reward mit nach unten schauen (verkleinert Winkel zu look at targets hinter agent)</td></tr>
<tr><td>106_Milestone3</td><td>Foot Collider Fehler behoben und Nutzung von neuem Walker Agent mit Bodypart components</td><td>funktioniert ziemlich gut aber springt anstatt zu laufen (behält ein bein vorne und eins hinten und galopiert)</td></tr>
<tr><td>108_Milestone3</td><td>Negativer Reward hinzugefügt wenn Agent nicht alle 2s das vordere Bein wechselt um galopieren zu verhindern</td><td>hat nicht funktioniert galopiert immer noch</td></tr>
<tr><td>109_Milestone3</td><td>Interval in dem Bein gewechselt werden soll auf 1.2s reduziert und Negativer Reward linearer anstatt quadratischer anstieg</td><td>Funktioniert (galopiert nicht mehr) aber lernt nicht so einen guten Reward wie ohne bein wechsel reward</td></tr>
<tr><td>110_Milestone3</td><td>Wie 109 mit zusätzlichem Power Save Reward</td><td>funktioniert nur arme sind sehr nah und starr am Körper</td></tr>
<tr><td>112_Milestone2</td><td>Wie 105 aber mit head tilt reward um ausnutzen von look target reward zu vermeiden</td><td>läuft nur rückwärts ? Vermutlich weil die meisten looktargets > 90 grad winkel zum target stehen somit rückwärts am öftesten gute Rewards bringt</td></tr>
<tr><td>113_Milestone2</td><td>LookTarget position jedes fixed update gesetzt um winkel gleich bleiben zu machen und LookTarget winkel erst ändern wenn looktarget reward in episode duchschnittlich größer als 0.7 war</td><td>läuft nur noch seitwärts. Evtl extremeren dropoff um zu verhindern das eine Lösung für alle Blickrichtungen gesucht wird (aktuell Mittlewert von 0.4). Zusätzlich testen Blickrichtung alle x Schritte zu ändern um zu startes Overfitting an eine Blickrichtung zu vermeiden</td></tr>
<tr><td>114_Milestone2</td><td>Wie 113 aber mit exp ansteigendem reward für look at target anstatt linear. Zudem Zielwert für LookAtTargetReward verringert um overfitting auf eine Blickrichtung zu vermeiden</td><td>schaut relativ zuverlässig zum lookAtTarget und läuft zum target aber ziemlich instabil und galopiert in sehr kleinen Schritten</td></tr>
<tr><td>115_Milestone2</td><td>Wie 114 aber Targets werden nur neu gesetzt wenn beim erreichen des Targets der avgLookAtTargetReward über 0.6 liegt um erst fortzufahren wenn Szenario gemeistert ist</td><td></td></tr>
<tr><td>116_Milestone1</td><td>Code umstrukturiert Bodypart als nerven / Muskeln, agent als gehirn und target controller rein zum setzen der position des targets und test mit 100 envs</td><td></td></tr>
<tr><td>117_Milestone2</td><td>Look Target wird neugesetzt wenn Agent min 3 sek auf target schaut am Stück (implementiert mit SphereCast)</td><td>funktioniert aber relativ instabil vmtl. zu kurzes training (da training reward und episoden länge immer noch steigend waren am ende des trainings)</td></tr>
<tr><td>118_Milestone2</td><td>gleich wie 117 aber mit 2sek look at time und doppelter trainings dauer (120mil steps)</td><td>abgebrochen für 119</td></tr>
<tr><td>119_Milestone2</td><td>gleich wie 118 aber mit reached look goal Statisik in tensorboard</td><td></td></tr>
<tr><td>120_Milestone2</td><td>gleich wie 119 aber mit 4 hidden layers</td><td></td></tr>
<tr><td>121_Milestone2</td><td>gleich wie 120 aber für 1 milliarde steps als langzeittest</td><td>Endet bei 50mil steps in lokalem maximum, ändert sich nicht mehr</td></tr>
<tr><td>122_Milestone2</td><td>gleich wie 121 aber mit beta = 0.01 um mehr zufällige Aktionen auszuprobieren (Entropy Regularization)</td><td>0.01 ist zu hoch entropy (randomness) steigt im Verlauf des trainings</td></tr>
<tr><td>123_Milestone2</td><td>gleich wie 122 aber mit beta = 0.0075 als mittelwert zwischen 0.005 und 0.01</td><td></td></tr>
<tr><td>124_Milestone2Standing</td><td>Versuch einfaches auf der Stelle stehen trainieren</td><td>abgebrochen</td></tr>
<tr><td>125_Milestone2Standing</td><td>Versuch einfaches auf der Stelle stehen mit Target auf Startpunkt und Geschwindigkeit von 0.0001</td><td>abgebrochen</td></tr>
<tr><td>126_Milestone2Standing</td><td>Versuch auf der Stelle stehen mit neuer MatchingVelocityReward Funktion</td><td>funktioniert, vermutlich in kombi mit Leistungsminimierung noch besser</td></tr>
<tr><td>127_Milestone1</td><td>Milestone 1 Walker Umgebung mit neuer MatchingVelocityReward Funktion</td><td>lernt sehr langsam, walker demo belohnungsfunktion verändert sensitivität je nach laufgeschwindigkeit</td></tr>
<tr><td>128_Milestone2Standing</td><td>Nutzt wieder original MatchingVelocityReward Funktion aber mit einem Limit auf dem Teiler sodass 0 Geschwindigkeit akzeptiert wird</td><td>funktioniert</td></tr>
<tr><td>129_Milestone2Backward</td><td>Richtungs enum implementiert um Blickrichtung zwischen Forwärts, Rechts, Links, Rückwärts zu wechseln, dieses Training nur Rückwärts</td><td>Konfigfehler</td></tr>
<tr><td>130_Milestone2Backward</td><td>gleich zu 129 aber Fehler behoben</td><td>funktioniert</td></tr>
<tr><td>131_Milestone2Right</td><td>gleich zu 130 aber mit Blickrichtung rechts</td><td>funktioniert</td></tr>
<tr><td>132_Milestone2Left</td><td>gleich zu 131 aber mit Blickrichtung links</td><td>Ergebnis (Belohnung) halb so gut wie 131_Milestone2Right ???</td></tr>
<tr><td>Name</td><td>beschreibung</td><td>notizen</td></tr>
</table>
