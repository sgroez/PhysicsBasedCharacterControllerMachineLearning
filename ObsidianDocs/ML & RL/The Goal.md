### Beschreibung
Ziel mit Reinforcement Learning mathematisch ausgedrückt

### Für die Policy der durchschnittliche Return Wert
$J(\pi)=\mathbb{E}_{\tau\sim\pi}[R(\tau)]$

$\mathbb{E}$: Erwartet (Durchschnitt)
$\tau\sim\pi$: Alle Trajectories entnommen von der policy
$R(\tau)$: Return für den Trajectory

### Ziel ist es den duchschnittlichen Return Wert zu maximieren
$\pi^*=\underset{\pi}{argmax} J(\pi)$

### durchschnittlicher Return Wert Formel spezifizieren
$J(\pi)=\int_\tau P(\tau|\pi)R(\tau)$
#### wahrscheinlichkeit für Trajectory mit Policie
$P(\tau|\pi)=P_0(S_0)\pi^{T-1}_{t=0}[\pi(a_t,S_t)*P(S_{t+1}|S_t, a_t)]$
$P_0(S_0)$: Chance das wir in $S_0$ starten
$\pi^{T-1}_{t=0}$ Summe nachfolgendes für alle timesteps
$\pi(a_t,S_t)$ Chance das wir von $S_t$ action $a_t$ wählen
$P(S_{t+1}|S_t, a_t)$ Chance das wir von $S_t$ mit action $a_t$ in $S_t+1$ landen