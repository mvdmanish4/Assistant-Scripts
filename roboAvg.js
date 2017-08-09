while (true) {
  var new_health = health();
  
  while (loc_x() < 80 && speed() < 10) {
    swim(0, 80);
    helicopterShot();
  }
  stop();
  while (loc_y() > 20 && speed() < 10) {
    swim(270, 80);
    helicopterShot();
  }
  stop();
  while (loc_x() > 20 && speed() < 10) {
    swim(180, 80);
    helicopterShot();
  }
  stop();
  while (loc_y() < 80 && speed() < 10) {
    swim(90, 80);
    helicopterShot();
  }
  stop();
}

function helicopterShot() {
  for (var i = 0; i < 72; i++) {
    if (scan(i * 5) <= 70){
      cannon(i * 5, scan(i * 5));
    }
    if (scan(i * 5 + 90) <= 70){
     cannon(i * 5 + 90, scan(i * 5 + 90)); 
    }
    if (scan(i * 5 + 180) <= 70){
     cannon(i * 5 + 180, scan(i * 5 + 180)); 
    }
    if (scan(i * 5 + 270) <= 70){
      cannon(i * 5 + 270, scan(i * 5 + 270));
    }
  }
}