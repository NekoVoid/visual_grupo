import pandas as pd
import numpy as np
from scipy.signal import butter, filtfilt
import pygame
from ucimlrepo import fetch_ucirepo

# === 1. Cargar el dataset desde ucimlrepo ===
eeg_eye_state = fetch_ucirepo(id=264)
X = eeg_eye_state.data.features  # señales EEG
df = X.copy()

# === 2. Elegir canal EEG y filtrar banda Alpha (8–12 Hz) ===
fs = 250  # Frecuencia de muestreo estimada
eeg_raw = df['O1'].values[:2000]  # usamos las primeras 2000 muestras

def butter_bandpass(lowcut, highcut, fs, order=5):
    nyq = 0.5 * fs
    low = lowcut / nyq
    high = highcut / nyq
    return butter(order, [low, high], btype='band')

def bandpass_filter(data, lowcut, highcut, fs, order=5):
    b, a = butter_bandpass(lowcut, highcut, fs, order=order)
    return filtfilt(b, a, data)

# Filtrar en banda Alpha
eeg_filtered = bandpass_filter(eeg_raw, 8, 12, fs)

# Normalizar y escalar a coordenadas de pantalla
eeg_norm = (eeg_filtered - np.min(eeg_filtered)) / (np.max(eeg_filtered) - np.min(eeg_filtered))
eeg_scaled = 400 - eeg_norm * 300  # Mapea a rango visual (de 100 a 400 px)

# === 3. Inicializar pygame ===
pygame.init()
width, height = 800, 500
screen = pygame.display.set_mode((width, height))
pygame.display.set_caption("Movimiento EEG - Banda Alpha (AF3)")
clock = pygame.time.Clock()

# === 4. Bucle de animación ===
index = 0
running = True
while running and index < len(eeg_scaled):
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

    y_pos = int(eeg_scaled[index])
    index += 1

    screen.fill((0, 0, 0))  # fondo negro
    pygame.draw.circle(screen, (0, 255, 0), (width // 2, y_pos), 30)

    pygame.display.flip()
    clock.tick(60)  # 60 cuadros por segundo

pygame.quit()
