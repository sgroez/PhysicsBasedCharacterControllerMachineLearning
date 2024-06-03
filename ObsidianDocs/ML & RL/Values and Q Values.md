## Values

### Beschreibung
Erwarteter (durchschnittlicher) Return für alles Trajectories entnommen der Policie wo der Anfangszustand $S_0 = S$ ist.

### Formel
$V^\pi(S)=\mathbb{E}_{\tau\sim\pi}[R(\tau)|S_0=S]$

### Referenz
$V^\pi(S)=J(\pi)$ wenn S = Verteilung über Anfangszustände $P_0$.

### Optimal
$V^*(S)=\underset{\pi}{Max}\mathbb{E}_{\tau\sim\pi}[R(\tau)|S_0=S]$


## Q Values

### Beschreibung
Erwarteter (durchschnittlicher) Return für alles Trajectories entnommen der Policie wo der Anfangszustand $S_0=S$ und $a_0=0$ ist.

### Formel
$Q^\pi (S,a)=\mathbb{E}_{\tau\sim\pi}[R(\tau)|S_0=S, a_0=a]$

### Referenz
$Q^\pi(S,a)=V^\pi(S)$ wenn a entnommen ist aus der Policy (on policy) $a\sim\pi$.

### Optimal
$Q^*(S,a)=\underset{\pi}{Max}\mathbb{E}_{\tau\sim\pi}[R(\tau)|S_0=S,a_0=a]$