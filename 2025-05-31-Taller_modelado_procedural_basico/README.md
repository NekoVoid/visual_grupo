# 🧪 Taller: Modelado Procedural Básico – Unity

## 🎯 Objetivo
Explorar el modelado procedural en Unity generando geometría 3D desde código sin usar herramientas de modelado manual, aprovechando programación con C# para construir estructuras dinámicas y reutilizables.

---

## 🔹 ¿Qué es el modelado procedural?
El modelado procedural es una técnica que permite crear modelos 3D mediante algoritmos en lugar de modelarlos a mano. Esto facilita la generación de estructuras repetitivas, fractales, terrenos, objetos aleatorios o animaciones complejas con muy poco esfuerzo manual.

---

## 🧱 Estructuras generadas

### 🟩 1. Cuadrícula de cubos
Se generó una rejilla usando dos bucles `for` y `GameObject.CreatePrimitive()`, colocando cubos en una grilla regular sobre el plano XZ.

```csharp
for (int x = 0; x < gridSize; x++)
{
    for (int z = 0; z < gridSize; z++)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(x, 0, z);
    }
}

for (int i = 0; i < spiralSteps; i++)
{
    float angle = i * 0.3f;
    float x = Mathf.Cos(angle) * spiralRadius;
    float z = Mathf.Sin(angle) * spiralRadius;
    float y = i * spiralHeight;

    GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
    cylinder.transform.position = new Vector3(x, y, z);
}
```
---

### 🟩 2. Espiral de cilindros
Usando trigonometría (Mathf.Sin, Mathf.Cos), se ubicaron cilindros en forma de espiral, variando altura y ángulo en cada paso del bucle.

```csharp
for (int i = 0; i < spiralSteps; i++)
{
    float angle = i * 0.3f;
    float x = Mathf.Cos(angle) * spiralRadius;
    float z = Mathf.Sin(angle) * spiralRadius;
    float y = i * spiralHeight;

    GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
    cylinder.transform.position = new Vector3(x, y, z);
}
```
---

### 🟩 3. Fractal de esferas (recursivo)
Se implementó un árbol fractal simple usando recursividad. Cada esfera genera copias más pequeñas en seis direcciones hasta alcanzar cierta profundidad.

```csharp
void GenerateFractal(Vector3 position, int depth)
{
    if (depth == 0) return;

    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    sphere.transform.position = position;
    float scale = 1f / depth;
    sphere.transform.localScale = Vector3.one * scale;

    Vector3[] directions = {
        Vector3.up, Vector3.down,
        Vector3.left, Vector3.right,
        Vector3.forward, Vector3.back
    };

    foreach (Vector3 dir in directions)
    {
        GenerateFractal(position + dir * 1.5f * scale, depth - 1);
    }
}
```
---

### 🟩 4. Cubo desde cero (malla personalizada)
En lugar de usar primitivas, se creó un cubo manualmente definiendo vértices y triángulos con Mesh, mostrando control total sobre la geometría.

```csharp
Vector3[] vertices = new Vector3[]
{
    new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(0, 1, 0),
    new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1)
};

int[] triangles = new int[]
{
    0,2,1, 0,3,2, 1,2,6, 1,6,5, 5,6,7, 5,7,4,
    4,7,3, 4,3,0, 0,1,5, 0,5,4, 3,6,2, 3,7,6
};
```
---

## 💻 Código

Todo el código se encuentra en el archivo GeometryGenerator.cs ubicado en la carpeta unity/.

---

## 🔑 Resultados

En el siguiente gif se ve la generación de las figuras y el mesh a partir del script GeometryGenerator.cs
![GifDesarrollo](Animation.gif)

---

## 🧩 Reflexión final

El modelado procedural, aunque es más abstracto que el proceso de modelar manualmente, permite automatizar la creación de estructuras repetitivas, generar variedad infinita y experimentar con formas complejas rápidamente. Es ideal para videojuegos, simulaciones, fractales o también generación de contenido aleatorio. Y aunque requiere lógica y planificación, el control que ofrece es muy potente y ahorra muchísimo tiempo en ciertos contextos.