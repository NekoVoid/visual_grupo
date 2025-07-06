# Taller Práctico #1 – Detección, Segmentación y Profundidad con YOLO + SAM + MiDaS

## 📅 Fecha
`2025-06-13`

---

## 🎯 Objetivo

Construir un pipeline completo de visión por computador que:

1. Detecta objetos en una imagen (YOLO).
2. Segmenta con precisión la forma del objeto (SAM).
3. Calcula la profundidad estimada por píxel (MiDaS).
4. Aplica análisis o efectos visuales basados en región y profundidad.

---

## 🧠 Temas Aplicados

- Detección de Objetos (YOLOv8)
- Segmentación de Instancia (SAM)
- Estimación de Profundidad Monocular (MiDaS)
- Manipulación de Imágenes y Visualización 2.5D
- Análisis Visual Avanzado

---

## 🔧 Herramientas y Entornos

- Python (`opencv-python`, `timm`, `torch`, `torchvision`, `ultralytics`)


---

## 📁 Estructura de la Práctica

```
2025-06-13_taller_yolo_sam_midas/
├── yolo_sam_midas_pipeline.ipynb       # Jupyter Notebook con el pipeline completo
├── images/                             # Imágenes de entrada
│   └── ejemplo1.jpg
├── outputs/                            # Resultados generados
│   ├── resultados.png                  # Imagen con resultados finales
│   ├── depth_maps/                     # Mapas de profundidad generados
│   ├── recortes/                       # Recortes de objetos detectados
│   └── segmentaciones/                 # Segmentaciones generadas
├── runs/                               # Resultados de entrenamiento y pruebas
│   └── segment/                        # Segmentaciones generadas por SAM
└── informe.md

```

---
## 📈 Capturas del proceso y resultados


### Por completar...

---

## Diagrama de Flujo del Pipeline

### Por completar...

---

## Breve explicación del uso de cada modelo

### Por completar...

---

## Análisis visual o cuantitativo

### Por completar...

---

## 💬 Reflexión sobre posibles usos de esta combinación

Esta combinación de modelos permite crear aplicaciones avanzadas de visión por computador que pueden ser utilizadas en diversas áreas como:
- **Seguridad y Vigilancia**: Detección de intrusos, segmentación de áreas críticas y análisis de profundidad para identificar amenazas.
- **Robótica y Navegación Autónoma**: Segmentación de obstáculos, estimación de distancias y navegación en entornos complejos.
- **Realidad Aumentada y Virtual**: Creación de experiencias inmersivas donde los objetos detectados pueden interactuar con el entorno virtual.
- **Análisis de Imágenes Médicas**: Segmentación de órganos o tumores, estimación de profundidad para análisis tridimensional.
- **Agricultura de Precisión**: Detección de cultivos, segmentación de áreas afectadas y análisis de salud de plantas mediante imágenes aéreas.
- **Interacción Humano-Computadora**: Mejora de interfaces mediante segmentación de gestos y análisis de profundidad para reconocimiento de acciones.
- **Arte Generativo**: Creación de obras de arte interactivas donde los objetos detectados influyen en la visualización y efectos generados.


