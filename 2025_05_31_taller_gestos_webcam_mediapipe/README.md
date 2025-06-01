# 🧪 Gestos con Cámara Web: Control Visual con MediaPipe

## 📅 Fecha
`2025_05_31`

---

## 🎯 Objetivo del Taller

Usar la webcam y la biblioteca MediaPipe para detectar gestos de manos y ejecutar acciones visuales en tiempo real. El propósito es explorar cómo las interfaces naturales pueden usarse para interactuar con la pantalla de forma intuitiva, sin hardware adicional.

---

## 🧠 Conceptos Aprendidos

- [X] Detección de gestos con MediaPipe.
- [X] Integración de gestos con aplicaciones visuales.
- [X] Uso de Python para procesamiento de imágenes y gestos.

---

## 🔧 Herramientas y Entornos

- Python (`opencv-python`, `mediapipe`, `tkinter`, `threading`)

---

## 📁 Estructura del Proyecto

```
2025_05_31_taller_gestos_webcam_mediapipe/
├── python/                # Python
├── resultados/            # capturas, métricas, gifs
├── README.md
```

---

## 🧪 Implementación


### 🔹 Etapas realizadas
1. Instalación de dependencias de Python.
2. Implementación de un detector de gestos con MediaPipe.
3. Integración de los gestos para controlar una aplicación visual.
4. Generación de GIFs y documentación del proceso.


### 🔹 Código relevante


#### Python

```python
def detectar_gestos():
    global estado_gesto
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
```


---
## 📊 Resultados Visuales



### Python
![Python](resultados/TestPython.gif)

---

## 🧩 Prompts Usados

### Python
```text
En un entorno Python como Colab, Jupyter Notebook o ejecución local, utiliza mediapipe, opencv-python y numpy para activar la cámara web y capturar video en tiempo real. Emplea MediaPipe Hands para detectar gestos de mano y medir condiciones como el número de dedos extendidos y la distancia entre dedos específicos (por ejemplo, índice y pulgar). A partir de estas señales, implementa una escena visual interactiva donde se desencadenen acciones en respuesta a los gestos, tales como cambiar el color de fondo, mover un objeto en pantalla o cambiar de escena al detectar una palma abierta.
```

---

## 💬 Reflexión Final

- ¿Qué aprendiste o reforzaste con este taller?
Aprendí a usar MediaPipe para la detección de gestos en tiempo real y cómo integrarlo con aplicaciones visuales en Python.
- ¿Qué parte fue más compleja o interesante?
La parte más interesante fue la integración de los gestos con acciones visuales, ya que permite una interacción más natural y fluida con la computadora.
- ¿Qué mejorarías o qué aplicarías en futuros proyectos?
Mejoraría la precisión de la detección de gestos y exploraría más acciones visuales que se puedan desencadenar con diferentes gestos.
---


## ✅ Checklist de Entrega

- [X] Carpeta `2025_05_31_taller_gestos_webcam_mediapipe`
- [X] Código limpio y funcional
- [X] GIF incluido con nombre descriptivo
- [X] Visualizaciones o métricas exportadas
- [X] README completo y claro
- [X] Commits descriptivos en inglés

---