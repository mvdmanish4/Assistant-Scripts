function attack() {
  for (var i = 0; i < 72; i++) {
    if (scan(i * 5) <= 70) cannon(i * 5, scan(i * 5));
    if (scan(i * 5 + 90) <= 70) cannon(i * 5 + 90, scan(i * 5 + 90));
    if (scan(i * 5 + 180) <= 70) cannon(i * 5 + 180, scan(i * 5 + 180));
    if (scan(i * 5 + 270) <= 70) cannon(i * 5 + 270, scan(i * 5 + 270));
  }
}

function DoubleAttack() {
  for (var i = 0; i < 72; i++) {
    if (scan(i * 5) <= 70) cannon(i * 5, scan(i * 5));
    if (scan(i * 5 + 180) <= 70) cannon(i * 5 + 180, scan(i * 5 + 180));
  }
}

while (true) {
  while (loc_x() < 80) {
    swim(0);
    dualSweep();
  }
  stop();
  while (loc_y() > 20) {
    swim(270);
    dualSweep();
  }
  stop();
  while (loc_x() > 20) {
    swim(180);
    dualSweep();
  }
  stop();
  while (loc_y() < 80) {
    swim(90);
    dualSweep();
  }
  stop();
}