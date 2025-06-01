// Importar la librería oscP5 para recibir mensajes OSC
import oscP5.*;
OscP5 osc;

// Variables de estado visual
color fondoColor = color(30);
float shapeX, shapeY;
float circuloRadio = 80;
boolean animar = false;
String ultimoComando = "Esperando...";
int forma = 0; // 0 = círculo, 1 = cuadrado, 2 = triángulo

void setup() {
  size(800, 600);
  background(30);

  // Iniciar servidor OSC en puerto 8000
  osc = new OscP5(this, 9124);

  // Posición inicial del círculo
  shapeX = width / 2;
  shapeY = height / 2;
}

// Función llamada automáticamente al recibir un mensaje OSC
void oscEvent(OscMessage theOscMessage) {
  if (theOscMessage.checkAddrPattern("/accion")) {
    String accion = theOscMessage.get(0).stringValue();
    println("Comando recibido: " + accion);
    ultimoComando = accion;

    if (accion.equals("iniciar_animacion")) {
      animar = true;
    } else if (accion.equals("cambiar_color")) {
      fondoColor = color(random(255), random(255), random(255));
    } else if (accion.equals("mover_objeto")) {
      shapeX = random(width);
      shapeY = random(height);
    } else if (accion.equals("cambiar_forma")) {
      forma = (forma + 1) % 3; // Cambiar entre formas
    } else if (accion.equals("detener_animacion")) {
      animar = false;
    }
  }
}

void draw() {
  background(fondoColor);

  // Dibujar objeto según forma seleccionada
  fill(255, 100);
  noStroke();

  pushMatrix();
  translate(shapeX, shapeY);

  if (forma == 0) {
    ellipse(0, 0, circuloRadio * 2, circuloRadio * 2);
  } else if (forma == 1) {
    rect(-circuloRadio, -circuloRadio, circuloRadio * 2, circuloRadio * 2);
  } else if (forma == 2) {
    triangle(-circuloRadio, circuloRadio,
             circuloRadio, circuloRadio,
             0, -circuloRadio);
  }

  popMatrix();

  // Animación opcional
  if (animar) {
    shapeX += sin(millis() * 0.001) * 0.5;
    shapeY += cos(millis() * 0.001) * 0.5;
  }

  // Mostrar último comando recibido
  fill(255);
  textSize(20);
  textAlign(CENTER);
  text("Último comando: " + ultimoComando, width / 2, 40);
}
