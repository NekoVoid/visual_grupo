// Importamos la librería oscP5
import oscP5.*;
OscP5 osc;

// Variables de estado visual
float squareX, squareY;
float squareSize = 100;
color currentColor = color(255);
boolean isRotating = false;
boolean isTranslating = false;
float rotationAngle = 0;

// Velocidad de movimiento
float speedX = 3;
float speedY = 2;

void setup() {
  size(800, 600);
  background(0);

  // Inicia el servidor OSC en el puerto 8000
  osc = new OscP5(this, 9100);
  
  // Posición inicial del círculo
  squareX = width / 2;
  squareY = height / 2;
}

// Función que se ejecuta cuando llega un mensaje OSC
void oscEvent(OscMessage theOscMessage) {
  String comando = theOscMessage.get(0).stringValue();
  println("Comando recibido: " + comando);

  if (comando.equals("naranja")) {
    currentColor = color(255, 179, 0);
  } else if (comando.equals("verde")) {
    currentColor = color(138, 247, 0);
  } else if (comando.equals("azul")) {
    currentColor = color(72, 227, 217);
  } else if (comando.equals("iniciar")) {
    isRotating = true;
    isTranslating = true;
  } else if (comando.equals("girar")) {
    isRotating = !isRotating;        // Alternar rotación
  } else if (comando.equals("mover")) {
    isTranslating = !isTranslating;  // Alternar translación
  } else if (comando.equals("detener")) {
    reset();
  }
}

void draw() {
  background(0, 20); // Efecto de cola
  
  setTranslation();
  pushMatrix();            // Guardar sistema de coordenadas
  translate(squareX, squareY);
  
  
  setRotation();
  rotate(rotationAngle);


  drawSquare();
  popMatrix(); 

}

void setTranslation(){
  // Actualizar posición si debe moverse
  if (isTranslating) {
    squareX += speedX;
    squareY += speedY;

    // Rebote contra paredes
    if (squareX < squareSize/2 || squareX > width - squareSize/2) {
      speedX *= -1;
    }
    if (squareY < squareSize/2 || squareY > height - squareSize/2) {
      speedY *= -1;
    }
  }
}


void setRotation(){
  if (isRotating) {
    rotationAngle += 0.05;
  }
}

void drawSquare(){
  fill(currentColor);
  noStroke();
  rectMode(CENTER);
  rect(0, 0, squareSize, squareSize);
}

void reset(){
  isRotating = false;
  isTranslating = false;
  squareX = width / 2;
  squareY = height / 2;
  rotationAngle = 0;
  currentColor = color(255, 255, 255);
}
