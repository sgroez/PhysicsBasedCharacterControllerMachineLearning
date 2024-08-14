
stehen bleiben:
wenn Distance < slowDownDistance dann je nach Distanz die Geschwindigkeit verringern -> agent bleibt vor ziel stehen bzw. kreist um ziel

extra Laufrichtungen:
-getrennte Modelle und Modell live wechseln problem mit seitwärts laufen und vermutlich schlechter Übergang bei Wechsel
-ein Modell mit Laufrichtung als one hot encoding in Beobachtung mit Lektion für verschiedene Laufrichtung -> kann nicht von vorwärtslaufen auf andere Richtung generalisieren bzw. anpassen (vergisst vorheriges verhalten) schränkt Bewegung auf feste Bewegungsrichtungen ein (vorwärts, rechts, links, rückwärts)
-extra Ziel für Blickrichtung:
  -Ziel zufällig gesetzt mit Winkel Begrenzung von agent zu ziel -> Winkel ändert sich bei Bewegung
  -Blickziel setzen bei Episodenwechsel oder Ziel erreicht -> ändert sich zu häufig das Agent verhalten nicht lernt
  -Blickziel und Laufziel nur neu setzen wenn Durchschnittliche Blickbelohnung > Grenzwert -> zu schwer bzw. dauert zu lange, Agent veralten schon zu sehr vertieft um es groß zu ändern
  -Walker lernt auf Boden zu schauen da Blickrichtung nach unten näher an Blickrichtung Ziel ist wenn sich das Ziel hinter dem Walker befindet
  -Extra Belohnung für aufrechte Blickrichtung
  -Blickziel wird jedes Physikupdate neu gesetzt um Winkel gleich zu behalten
  -Blickziel neu setzen wenn bestimmte Zeit auf Ziel geschaut (mit Spherecast) -> funktioniert nicht schlecht aber bei längerem training hört der Agent auf das Blickziel zu erreichen


Zu beginn werden unterschiedliche Bewegungsabläufe in einzelnen Modellen trainiert um zu prüfen ob die Limits des Läufers das erlernen dieser erlauben. Anschließend werden unterschiedliche Ansätze getestet um die Bewegungsabläufe in einem System zu kombinieren.
