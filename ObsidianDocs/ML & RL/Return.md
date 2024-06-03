### Beschreibung
Summe aller bisher erhaltenen Rewards

### Finite horizon undiscounted return
Beispiel Rewards=[0,0,1]
Formel:
__$R(\tau)=\sum_{t=0}^\tau r_t$__

### Infinite horizon discounted return
Beispiel Rewards=[1,2,0,5]
Formel:
__$R(\tau)=\sum_{t=0}^\infty r_t*\gamma^t$__
$\gamma$ discount factor immer im Bereich [0..1] meistens im Bereich [0.95..0.99]