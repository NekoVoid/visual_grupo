import asyncio
from websockets import ServerConnection
from websockets.asyncio.server import serve
import cv2
import numpy as np
import mediapipe as mp


def get_finger_list(hand_landmarks, mp_hands):
    """
    Función para consultar la lista de gestos y determinar si se ha realizado alguno.
    """
    if not hand_landmarks:
        return "NINGUNO"

    lista_dedos = {
        "PULGAR": (mp_hands.HandLandmark.THUMB_TIP, mp_hands.HandLandmark.THUMB_IP),
        "INDICE": (mp_hands.HandLandmark.INDEX_FINGER_TIP, mp_hands.HandLandmark.INDEX_FINGER_PIP),
        "MEDIO": (mp_hands.HandLandmark.MIDDLE_FINGER_TIP, mp_hands.HandLandmark.MIDDLE_FINGER_PIP),
        "ANULAR": (mp_hands.HandLandmark.RING_FINGER_TIP, mp_hands.HandLandmark.RING_FINGER_PIP),
        "MEÑIQUE": (mp_hands.HandLandmark.PINKY_TIP, mp_hands.HandLandmark.PINKY_PIP)
    }

    extendidos = {}
    for nombre, (tip, pip) in lista_dedos.items():
        extendidos[nombre] = hand_landmarks.landmark[tip].y < hand_landmarks.landmark[pip].y

    return extendidos


def check_not_extended(extendidos, fingers):
    """
    Verifica si los dedos especificados no están extendidos.
    """
    return all(not extendidos[finger] for finger in fingers)


def check_extended(extendidos, fingers):
    """
    Verifica si los dedos especificados están extendidos.
    """
    return all(extendidos[finger] for finger in fingers)


def identify_gesture(hand_landmarks, mp_hands):
    """
    Función para determinar el gesto basado en los dedos extendidos.
    """
    extendidos = get_finger_list(hand_landmarks, mp_hands)

    if check_not_extended(extendidos, ["INDICE", "MEDIO", "ANULAR", "MEÑIQUE"]) and extendidos["PULGAR"]:
        return "ACEPTAR"
    elif check_extended(extendidos, ["PULGAR", "MEÑIQUE"]) and check_not_extended(extendidos,
                                                                                  ["INDICE", "MEDIO", "ANULAR"]):
        return "CANCELAR"
    elif check_extended(extendidos, ["MEÑIQUE", "INDICE", "PULGAR"]) and check_not_extended(extendidos,
                                                                                  ["MEDIO", "ANULAR"]):
        return "LANZAR"
    elif check_extended(extendidos, ["INDICE"]) and check_not_extended(extendidos,
                                                                       ["MEDIO", "ANULAR", "MEÑIQUE"]):
        return "ESCOGER_1"
    elif check_extended(extendidos, ["INDICE", "MEDIO"]) and check_not_extended(extendidos,
                                                                                ["ANULAR", "MEÑIQUE"]):
        return "ESCOGER_2"
    elif check_extended(extendidos, ["INDICE", "MEDIO", "ANULAR"]) and check_not_extended(extendidos,
                                                                                          ["MEÑIQUE"]):
        return "ESCOGER_3"
    else:
        return "NINGUNO"


def check_gesture(image):
    """
    Función para procesar la imagen y detectar el gesto de la mano.
    """
    mp_hands = mp.solutions.hands
    mp_dibujo = mp.solutions.drawing_utils
    with mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.7) as hands:
        result = hands.process(cv2.cvtColor(image, cv2.COLOR_BGR2RGB))

        if result.multi_hand_landmarks:
            mp_dibujo.draw_landmarks(
                image, result.multi_hand_landmarks[0], mp_hands.HAND_CONNECTIONS
            )
            return identify_gesture(result.multi_hand_landmarks[0], mp_hands), image
        return "NINGUNO", image


async def handle_client(websocket: ServerConnection):
    """ 
    Manejador de conexión del cliente WebSocket.
    """
    await websocket.send("Welcome to the WebSocket server!")
    try:
        width = 640
        height = 480

        async for message in websocket:
            if isinstance(message, bytes):
                print("Received image data")
                np_array = np.frombuffer(message, dtype=np.uint8)
                image = cv2.imdecode(np_array, cv2.IMREAD_COLOR)
                if image is not None:
                    # _ , image = cv2.threshold(image, 127, 255, cv2.THRESH_BINARY)
                    gesto, output = check_gesture(image)

                    _, data = cv2.imencode(".png", output)
                    print("Gesto detectado:", gesto)
                    await websocket.send(gesto)
                    await websocket.send(data.tobytes())


            elif isinstance(message, str):
                width, height = map(int, message.split(","))

    except Exception as e:
        print(f"Error: {e}")
    finally:
        await websocket.close()


async def main():
    async with serve(handle_client, "localhost", 3000) as server:
        print("Server started on ws://localhost:3000")
        await server.wait_closed()


if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        print("\nServer stopped!")
