- Besser generell (offen für verschiedene Ansätze) halten oder speziell einen Ansatz vertieft untersuchen? Bsp: Imitation mit Animationen oder Motion Capture -> offen gestalten
- Was wird am meisten bewertet? Ergebnisse oder Ansätze / Versuche oder Analyse Recherche Ansätze beschreiben nicht nur der letzte entschluss
- Aktuelle Literatur nicht zu tief einsteigen (nicht gleiches Level erwarten)
- Inference funktioniert nur mit gleicher Anzahl an Environments wie training? (Training mit PPO und SAC hat Probleme mit der Generalisierung wenn weniger environments genutzt werden als beim training.)
- Rigidbody & Configurable joint vs articulation body
- kinematica package
- module structure like in https://github.com/sergioabreu-g/active-ragdolls
- start animation at different timepoints to optimize learning with https://docs.unity3d.com/ScriptReference/Animator.Play.html
- EnvironmentParameters callback nutzen für spawn environments?
- test more degrees of freedom and higher angle limits on joints
- Build setup with config folder to automate learning with list of configs after another (for hyperparameter testing)
- train strengths and rotations seperate (using animation data as reference)
- check phase variable after animator.play
- check out deep mimic multiple clip reward structure
- deep mimic rewards use negative multiplication with exp function to get values from 0 to 1
  (high value _ -2 = high negative value with exp lower values get closer to 0, low value _ -2 = low negative value with exp higher values <= 0 values get closer to 1)
- deep mimic velocity reward uses finite difference to calculate velocity of kinematic reference (using earlier and or later position to calculate rate of change)
- check side channels in mlagents to create more graphs
- stats recorder hilft Fehler / Probleme zu entdecken während training auf server ohne grafik
- Walker Agent lernt matchingVelocityReward nur sehr wenig (vergleichen mit unveränderter Walker demo)
- research mixture of experts
- test different reward dropoffs using xbpeng reward scaling
- trägheit in unity physics
- leistung (kraft aufwand) in episode


ppo aus nutzersicht und referenz auf literatur
fließtext für zuweisung von abbildung. und table bei komponenten beschreibung oder in fließtext
codeausschnitte zusammen einfügen und zeilennummer referenzieren in erklärung
mlagents learning environment abbildungen eine überflüssig?
Unity physikkomponenten erklären in Grundlagen?