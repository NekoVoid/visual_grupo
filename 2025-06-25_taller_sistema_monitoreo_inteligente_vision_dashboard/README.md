# 🧪 Taller - Construcción de un Mini-Sistema de Monitoreo Inteligente

## 📅 Fecha
`2025-06-25`

---

## 🎯 Objetivo del Taller

Diseñar un sistema de monitoreo inteligente que integre visión por computador (detección de personas u objetos) y un panel visual en tiempo real que permita observar lo que ocurre frente a la cámara. Además, se implementará la capacidad de generar logs o capturas automáticas según eventos definidos.

---

## 🧠 Conceptos Aprendidos

- [x] Detección de objetos con YOLOv12 y Ultralytics.
- [x] Visualización de datos en tiempo real con Python.
- [x] Integración de sistemas de monitoreo con visión por computador.
- [x] Generación de logs y capturas automáticas.

---

## 🔧 Herramientas y Entornos


- Python (`opencv-python`, `torch`, `ultralytics`, `matplotlib`)

---

## 📁 Estructura del Proyecto

```
2025-06-25_taller_sistema_monitoreo_inteligente_vision_dashboard/
├── python/                # Python
├── datos/                 # Modelo
├── capturas/              # imágenes guardadas
├── logs/                  # registros de eventos
├── resultados/            # GIF
├── README.md
```

---

## 🧪 Implementación


### 🔹 Etapas realizadas
1. Instalación de dependencias y configuración del entorno.
2. Desarrollo del script en Python para captura de video y detección de objetos.
3. Implementación del sistema de conteo y visualización en tiempo real.
4. Generación de logs y capturas automáticas.
5. Creación de visualizaciones y métricas exportadas.
6. Documentación del proceso y resultados.

### 🔹 Código relevante


#### Python

```python
def procesar_detecciones(frame):
    """
    Procesa las detecciones en un frame y devuelve el conteo actual de objetos detectados.
    """
    results = model(frame, verbose=False)
    detecciones = results[0].boxes.data.cpu().numpy()
    current_conteo = {}
    persona_detectada = False
    conf_persona = 0.0

    for *box, conf, cls in detecciones:
        clase = model.names[int(cls)]
        current_conteo[clase] = current_conteo.get(clase, 0) + 1
        if clase == "person":
            persona_detectada = True
            conf_persona = conf
            x1, y1, x2, y2 = map(int, box)
            cv2.rectangle(frame, (x1, y1), (x2, y2), (0, 255, 0), 2)
            cv2.putText(frame, f"{clase} {conf:.2f}", (x1, y1 - 10),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 1)
    return current_conteo, persona_detectada, conf_persona
```

---
## 📊 Resultados Visuales

### Python
![Python](resultados/Python.gif)


---

## 🧩 Prompts Usados

### Python
```text
En Python, desarrolla una aplicación que capture video en tiempo real desde la webcam y utilice YOLOv12 (a través de Ultralytics) para detectar personas u objetos específicos. Implementa un sistema de conteo que identifique e incremente instancias por tipo de objeto y actualiza dinámicamente un contador visual en pantalla. Diseña un panel de información en tiempo real que muestre estadísticas de detección (tipo de objeto, cantidad), gráficos de barras o líneas que reflejen el flujo de detecciones, y el estado del sistema ('inactivo', 'alerta', 'grabando'). Además, registra automáticamente eventos: si se detecta una persona, guarda una imagen en la carpeta capturas/ y escribe un registro (log) en formato CSV dentro de logs/, incluyendo timestamp y descripción del evento.
```


---

## 💬 Reflexión Final

- ¿Qué aprendiste o reforzaste con este taller?
Aprendí a integrar visión por computador con sistemas de monitoreo en tiempo real, mejorando mis habilidades en Python y manejo de modelos de detección.

- ¿Qué parte fue más compleja o interesante?
La parte más interesante fue la integración del sistema de detección con la visualización en tiempo real, especialmente al manejar múltiples objetos y generar logs automáticamente.

- ¿Qué mejorarías o qué aplicarías en futuros proyectos?
Delimitaría mejor las capturas automáticas y el número de eventos registrados, para evitar saturar el sistema con demasiadas imágenes y logs.

---

## ✅ Checklist de Entrega

- [x] Carpeta `2025-06-25_taller_sistema_monitoreo_inteligente_vision_dashboard`
- [x] Código limpio y funcional
- [x] GIF incluido con nombre descriptivo
- [x] Visualizaciones o métricas exportadas
- [x] README completo y claro
- [x] Commits descriptivos en inglés

---