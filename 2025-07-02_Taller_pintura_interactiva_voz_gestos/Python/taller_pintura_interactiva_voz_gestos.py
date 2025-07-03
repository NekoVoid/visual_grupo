import cv2
import mediapipe as mp
import numpy as np
import threading
import speech_recognition as sr
import time
import os

# === CONFIG INICIAL ===
color = (0, 0, 255)  # Color inicial: rojo (en formato BGR)
canvas = np.ones((480, 640, 3), dtype=np.uint8) * 255  # Lienzo en blanco
drawing = False  # Indica si se está dibujando
x_prev, y_prev = 0, 0  # Posición anterior del dedo
pincel_grosor = 5  # Grosor inicial del pincel
modo_pincel = "normal"  # Modo del pincel: normal o difuso

# === MEDIA PIPE ===
mp_hands = mp.solutions.hands
mp_draw = mp.solutions.drawing_utils
hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.7)

# === COMANDOS DE VOZ ===
def escuchar_comando():
    global color, canvas
    r = sr.Recognizer()
    with sr.Microphone() as source:
        print("Escuchando comando...")
        r.adjust_for_ambient_noise(source)
        audio = r.listen(source)
    try:
        comando = r.recognize_google(audio, language='es-ES').lower()
        print("Comando detectado:", comando)

        # Cambios de color por voz
        if "rojo" in comando:
            color = (0, 0, 255)
        elif "verde" in comando:
            color = (0, 255, 0)
        elif "azul" in comando:
            color = (255, 0, 0)
        elif "amarillo" in comando:
            color = (0, 255, 255)
        elif "morado" in comando:
            color = (255, 0, 255)
        elif "naranja" in comando:
            color = (0, 128, 255)
        # Borrar lienzo
        elif "limpiar" in comando:
            canvas[:] = 255
        # Guardar imagen
        elif "guardar" in comando:
            carpeta_obras = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "obras"))
            os.makedirs(carpeta_obras, exist_ok=True)

            i = 1
            while os.path.exists(os.path.join(carpeta_obras, f"img{i}.png")):
                i += 1
            ruta = os.path.join(carpeta_obras, f"img{i}.png")

            cv2.imwrite(ruta, canvas)
            print("Imagen guardada como", ruta)
        else:
            print("Comando no reconocido.")
    except:
        print("No se entendió el comando.")

# === HILO PARA ESCUCHAR VOZ EN PARALELO ===
def escuchar_en_hilo():
    while True:
        escuchar_comando()

# Iniciar el hilo
threading.Thread(target=escuchar_en_hilo, daemon=True).start()

# === DETECTAR DEDOS ===
def esta_arriba(lm_tip, lm_base):
    # El dedo está arriba si su punta está más arriba (y menor) que su articulación base
    return (lm_tip.y < lm_base.y) and (abs(lm_tip.y - lm_base.y) > 0.04)

def dedos_arriba(hand_landmarks):
    dedos = []
    lm = hand_landmarks.landmark

    # Pulgar: comparando en el eje X
    dedos.append(1 if lm[4].x < lm[3].x else 0)

    # Otros dedos: comparar en eje Y
    dedos.append(1 if esta_arriba(lm[8], lm[6]) else 0)   # Índice
    dedos.append(1 if esta_arriba(lm[12], lm[10]) else 0) # Medio
    dedos.append(1 if esta_arriba(lm[16], lm[14]) else 0) # Anular
    dedos.append(1 if esta_arriba(lm[20], lm[18]) else 0) # Meñique

    print("Dedos detectados:", dedos, "| Total arriba:", sum(dedos))
    return dedos

# === LOOP PRINCIPAL ===
cap = cv2.VideoCapture(0)

while True:
    ret, frame = cap.read()
    if not ret:
        break

    frame = cv2.flip(frame, 1)  # Invertir imagen horizontalmente
    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    result = hands.process(rgb)

    if result.multi_hand_landmarks:
        for handLms in result.multi_hand_landmarks:
            mp_draw.draw_landmarks(frame, handLms, mp_hands.HAND_CONNECTIONS)
            dedos = dedos_arriba(handLms)

            h, w, _ = frame.shape
            x = int(handLms.landmark[8].x * w)  # Coordenadas del dedo índice
            y = int(handLms.landmark[8].y * h)

            # === CAMBIO DE GROSOR POR DEDOS ===
            if dedos[1] and dedos[2] and not dedos[3]:
                pincel_grosor = 20
            elif dedos[1] and dedos[2] and dedos[3] and not dedos[4]:
                pincel_grosor = 40

            # === INDICE Y MEÑIQUE ===
            if dedos[1] and dedos[4]:
                modo_pincel = "difuso"
            else:
                modo_pincel = "normal"

            # === ÍNDICE ESTÁ LEVANTADO ===
            if dedos[1]:
                # Suavizado de coordenadas
                x_suave = int((x + x_prev) / 2)
                y_suave = int((y + y_prev) / 2)

                if drawing:
                    if modo_pincel == "normal":
                        cv2.line(canvas, (x_prev, y_prev), (x_suave, y_suave), color, pincel_grosor)
                    elif modo_pincel == "difuso":
                        overlay = canvas.copy()
                        cv2.circle(overlay, (x_suave, y_suave), pincel_grosor * 2, color, -1)
                        alpha = 0.2  # Transparencia
                        cv2.addWeighted(overlay, alpha, canvas, 1 - alpha, 0, canvas)

                x_prev, y_prev = x_suave, y_suave
                drawing = True
            else:
                drawing = False
    else:
        drawing = False

    # Mostrar las ventanas de dibujo y cámara
    cv2.imshow("Lienzo", canvas)
    cv2.imshow("Cámara con mano", frame)

    # Salir con ESC
    if cv2.waitKey(1) & 0xFF == 27:
        break

cap.release()
cv2.destroyAllWindows()
