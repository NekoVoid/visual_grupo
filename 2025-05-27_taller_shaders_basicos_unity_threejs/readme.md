# Sombras Personalizadas: Primeros Shaders en Unity y Three.js

Exploración de shaders personalizados para simular efectos de iluminación y deformación en superficies, usando Three.js y React Three Fiber.

## Three.js

Se crea una escena con un plano que utiliza un material personalizado (`shaderMaterial`) para deformar su superficie y calcular la iluminación de manera manual en el shader. El plano se deforma dinámicamente en el eje Y usando funciones seno y coseno, y la iluminación se calcula en el fragment shader a partir de la posición de una luz que se mueve en el tiempo.

```jsx
<>
  <Canvas>
    <Plane/>
    <OrbitControls makeDefault/>
  </Canvas>
</>
```

El componente `Plane` utiliza un material de shader personalizado que recibe como uniformes el tiempo (`time`), el color base y la posición de la luz. El shader de vértices deforma la malla y calcula normales, mientras que el fragment shader mezcla el color base con blanco según la intensidad de la luz.

```jsx
const ColorShiftMaterial = shaderMaterial(
  { time: 0, color: new THREE.Color(0.2, 0.0, 0.1), lightPosition: new THREE.Vector3(0, 1, 0), ... },
  // vertex shader
  `...`,
  // fragment shader
  `...`
);

function Plane() {
  const materialRef = useRef();
  useFrame((state) => {
    const time = state.clock.getElapsedTime();
    materialRef.current.time = time;
    materialRef.current.lightPosition = new THREE.Vector3(48*Math.cos(time), -20*Math.sin(time/3)+25, 48*Math.sin(time));
  });
  return (
    <mesh>
      <planeGeometry args={[100, 100, 300, 300]} />
      <colorShiftMaterial ref={materialRef} side={THREE.DoubleSide} />
    </mesh>
  );
}
```
resultado

![Animacion](./threejs_anim.gif)

El código se encuentra en [App.jsx](./threejs/src/App.jsx) y se puede correr con los comandos:

```sh
cd threejs
npm install
npm run dev
```
