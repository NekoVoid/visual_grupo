# 🧪 Taller - Interfaces Multimodales: Uniendo Voz y Gestos

## 📅 Fecha
`2025-05-31`

---

## 🎯 Objetivo del Taller
Fusionar gestos (detectados con MediaPipe) y comandos de voz para realizar acciones compuestas dentro de una interfaz visual. Este taller introduce los fundamentos de los sistemas de interacción multimodal, combinando dos formas de entrada humana para enriquecer la experiencia de control.

---

## 🧠 Conceptos Aprendidos

- [X] Interacción multimodal: combinación de voz y gestos.
- [X] Detección de gestos con MediaPipe.
- [X] Reconocimiento de voz con SpeechRecognition y PyAudio.
- [X] Comunicación entre Python y Processing usando OSC.
- [X] Sincronización de entradas de voz y gestos para acciones compuestas.

---

## 🔧 Herramientas y Entornos


- Python (`opencv-python`, `mediapipe`, `speech_recognition`, `pyaudio`, `tkinter`, `threading`)
- Processing (`oscP5`)

---

## 📁 Estructura del Proyecto

```
2025-05-31_taller_interfaces_multimodales_voz_gestos/
├── processing/            # Processing
├── python/                # Python
├── resultados/            # capturas, métricas, gifs
├── README.md
```

---

## 🧪 Implementación


### 🔹 Etapas realizadas
1. Configuración del entorno de Python con las librerías necesarias.
2. Implementación de la captura de video y detección de gestos con MediaPipe.
3. Implementación del reconocimiento de voz con SpeechRecognition y PyAudio.
4. Configuración de la comunicación OSC entre Python y Processing.
5. Implementación de la lógica para combinar gestos y comandos de voz.
6. Creación de una interfaz visual en Processing que responda a los comandos recibidos.
7. Pruebas y ajustes para asegurar una interacción fluida y en tiempo real.


### 🔹 Código relevante


#### Python

```python
def escuchar_voz(app):
    r = sr.Recognizer()
    with sr.Microphone() as source:
        while True:
            try:
                audio = r.listen(source, timeout=5, phrase_time_limit=3)
                texto = r.recognize_google(audio, language="es-CO").lower()
```

```python
def detectar_gesto_mano(hand_landmarks, mp_hands):
    dedos = {
        "PULGAR": (mp_hands.HandLandmark.THUMB_TIP, mp_hands.HandLandmark.THUMB_IP),
        "INDICE": (mp_hands.HandLandmark.INDEX_FINGER_TIP, mp_hands.HandLandmark.INDEX_FINGER_PIP),
        "MEDIO": (mp_hands.HandLandmark.MIDDLE_FINGER_TIP, mp_hands.HandLandmark.MIDDLE_FINGER_PIP),
        "ANULAR": (mp_hands.HandLandmark.RING_FINGER_TIP, mp_hands.HandLandmark.RING_FINGER_PIP),
        "MENIQUE": (mp_hands.HandLandmark.PINKY_TIP, mp_hands.HandLandmark.PINKY_PIP),
    }
    extendidos = {}
    for nombre, (tip, pip) in dedos.items():
        extendidos[nombre] = hand_landmarks.landmark[tip].y < hand_landmarks.landmark[pip].y


    if all(extendidos.values()):
        return "mano_abierta"
    ...
```
        

```python
def detectar_gestos():
    mp_hands = mp.solutions.hands
    mp_dibujo = mp.solutions.drawing_utils
    hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.7)
    cap = cv2.VideoCapture(0)

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        result = hands.process(rgb_frame)

        if result.multi_hand_landmarks:
            for hand_landmarks in result.multi_hand_landmarks:
                mp_dibujo.draw_landmarks(
                    frame, hand_landmarks, mp_hands.HAND_CONNECTIONS
                )
            
                estado_gesto = detectar_gesto_mano(hand_landmarks, mp_hands)
```

#### Processing

```java
osc = new OscP5(this, 9124);
```

```java
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
```



---
## 📊 Resultados Visuales


### Python + Processing
![Test](resultados/test.gif)


---

## 🧩 Prompts Usados

### Processing
```text
En Processing, crea una escena visual interactiva que reciba comandos desde una aplicación en Python a través de mensajes OSC. Utiliza la librería oscP5 para escuchar los mensajes entrantes y vincula cada tipo de mensaje a una acción específica: cambiar el color o forma de objetos en pantalla, iniciar o detener animaciones, y mostrar texto dinámico que indique el último comando recibido. Asegura una comunicación fluida y en tiempo real entre ambos entornos para una respuesta visual inmediata a las entradas del usuario.
```

### Python
```text
En un entorno Python (Colab o local), utiliza mediapipe y opencv-python para capturar video en tiempo real desde una webcam y detectar gestos de mano, mientras empleas speech_recognition y pyaudio para reconocer comandos de voz simples como 'azul', 'rotar' o 'mostrar'. Procesa simultáneamente ambas señales de entrada utilizando hilos o bucles coordinados, e implementa lógica condicional que combine ambos tipos de entrada. Por ejemplo: cambiar el color solo si se dice 'cambiar' y la mano está abierta, o mover un objeto solo si se pronuncia 'mover' y se muestra un gesto de dos dedos. Finalmente, diseña una escena visual interactiva con tkinter, que responda dinámicamente a estas combinaciones de entrada.
```



---

## 💬 Reflexión Final

- ¿Qué aprendiste o reforzaste con este taller?
Aprendí a integrar gestos con manos con MediaPipe y openCV, y a combinarlo con comandos de voz usando SpeechRecognition. La comunicación entre Python y Processing a través de OSC fue un desafío interesante que mejoró mi comprensión de la interacción multimodal.
- ¿Qué parte fue más compleja o interesante?
La parte más compleja fue la sincronización de las entradas de voz y gestos, especialmente al manejar múltiples condiciones para activar acciones específicas. 
- ¿Qué mejorarías o qué aplicarías en futuros proyectos?
Mejoraría la robustez del sistema para manejar errores de reconocimiento de voz y gestos, y exploraría más formas de interacción multimodal. También consideraría optimizar la latencia en la comunicación entre Python y Processing para una experiencia más fluida.

---

## ✅ Checklist de Entrega

- [X] Carpeta `2025-05-31_taller_interfaces_multimodales_voz_gestos`
- [X] Código limpio y funcional
- [X] GIF incluido con nombre descriptivo
- [X] README completo y claro
- [X] Commits descriptivos en inglés

---