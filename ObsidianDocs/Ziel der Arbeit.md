### Kurz
Mindestens:
M1 Ausführung und Analyse Unity ML Agents Walker Demo.
M2 Erweiterung Walker Demo mit zusätzlichen Bewegungsrichtungen (auf der Stelle stehen, Rückwärts).
M2.1 Erweiterung Walker Demo mit Nutzergesteuertem Target (WASD setzt Target in entsprechender Richtung relativ zum Charakter).
M3 Mixamo Charakter mit Rigidbodies und Joints als active ragdoll konfigurieren.
M3.1 Walker Demo für Humanoid Charakter mit mehreren Gelenken erweitern (Mixamo Charakter).
Optional:
M4 Erweiterung Walker Demo mit Imitation learning (Reward für Ähnlichkeit der Bewegung) Gelenkrotation vergleichen mit bestehender traditioneller animation.
M5 Prototyp für verschiedene Bewegungen testen (unterschiedliche Animationen imitieren).
M6 Wechsel zwischen unterschiedlicher Bewegungen analysieren und implementieren mit State input (Sprinting, Injured).

### Lang
#### Mindestens:

##### M1 Ausführung und Analyse der Unity ML Agents Walker Demo
Bei der Walker Demo handelt es sich um eine von Unity entwickelte Testumgebung, in welcher ein physikbasierter humanoider Charakter mit Festkörpern und Gelenken simuliert wird. Der Charakter hat insgesamt 26 Freiheitsgrade, welche sich aus den unterschiedlichen Rotationen der Gelenke zusammensetzen. Die Demo zeigt, wie die Ml Agents Library genutzt werden kann, um einen solchen Charakter mithilfe von aktuellen Reinforcement Learning Algorithmen zu trainieren, um sich mit einer bestimmten Geschwindigkeit zu einem statischen Ziel zu bewegen.

##### M2 Erweiterung Walker Demo mit zusätzlichen Bewegungsrichtungen
In diesem Schritt soll das erarbeitete Wissen der vorherigen Analyse angewandt werden, um die Bewegung des Charakters zu erweitern. Der Charakter soll trainiert werden rückwärts, seitwärts zu laufen und auf der Stelle zu stehen. Zudem soll er auch darauf trainiert werden, einem bewegten Ziel zu folgen.

##### M2.1 Erweiterung Walker Demo mit Nutzergesteuertem Target
Um die Ergebnisse aus den vorherigen Schritten auch in Spielen verwenden zu können, soll zusätzlich ein Mechanismus entwickelt werden, um den trainierten Charakter live über die Tastatur zu steuern.

##### M3 Physik Simulation für Mixamo Charakter mit Festkörpern und Gelenken konfigurieren
Realistische Charaktermodelle haben in der Regel mehr Körperteile und Gelenke, daher soll in diesem Kapitel die Simulation auf ein menschliches Charaktermodell von Mixamo, einer Library aus Charakteren und Animationen von Adobe, übertragen werden. Dafür soll das erlangte Wissen aus Meilenstein 1 sowie weitere Quellen verwendet werden, um Parameter wie Gewichtsverteilung im Körper und Einschränkungen der Gelenke möglichst realitätsnah zu konfigurieren.

##### M3.1 Walker Demo für Humanoiden Charakter mit mehreren Gelenken (Mixamo Charakter) erweitern
Für diesen Meilenstein sollen die Ml Agent Komponenten der Walker Demo erweitert werden, sodass die vorhergehenden Systeme auch auf den in Meilenstein 3 simulierten Charakter angewendet werden können. Zudem soll hier auch der Einfluss von komplexeren Körperstrukturen auf die Trainingskomplexität / Trainingsdauer sowohl als auch die Qualität des Ergebnisses erforscht werden.

#### Optimal:

##### M4 Erweiterung Walker Demo mit Imitation learning
In den bisherigen Ansätzen ist die Realitätsnähe abhängig von der Limitierung der Beweglichkeit als auch von der Optimierungsfunktion des Reinforcement Learning Algorithmus. Um die Trainingsdauer zu verkürzen und die Qualität der Bewegungen weiter zu steigern, werden in diesem Meilenstein bestehende Bewegungen aus Animationen oder aufgezeichneten Bewegungsdaten als Referenz genutzt. Somit kann der Agent abhängig von der Differenz zwischen Referenz und simulierter Bewegung belohnt werden.

##### M5 Prototyp für verschiedene Bewegungen testen
Um die behandelten Konzepte und das daraus resultierende System für unterschiedliche Spiele nutzen zu können, ist das einfache Bewegen in die vier Himmelsrichtungen nicht ausreichend. Für verschiedene Bewegungen sind unterschiedliche Observationen und auch Belohnungen erforderlich. In diesem Kapitel sollen die Anpassungsmöglichkeiten der entwickelten Skripte auf gängige Bewegungsabläufe in Videospielen getestet werden.

##### M6 Wechsel zwischen unterschiedlichen Bewegungen analysieren und implementieren
Das letzte Kapitel befasst sich mit dem Wechsel zwischen unterschiedlichen bereits antrainierten Bewegungen. Hierzu wird entweder zwischen Trainingsmodellen gewechselt oder es wird ein Konzept entwickelt, um ein Modell auf mehrere Bewegungen zu trainieren, indem spezielle Eigenschaften als Input in das Training mit einfließen.