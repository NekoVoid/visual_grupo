# UV Mapping: Texturas que Encajan

Exploración de mapeo UV y texturizado avanzado en modelos 3D usando Three.js y React Three Fiber.

## Three.js

Se carga un modelo 3D en formato OBJ y se le aplican dos texturas distintas usando grupos de materiales y mapeo UV. Se permite modificar los UVs de la geometría en tiempo real para observar cómo afectan la proyección de la textura sobre el modelo.

El modelo se compone de dos materiales: uno para la textura base y otro para los bigotes (con transparencia). Se pueden modificar los UVs de la geometría con un slider de fuerza y un botón para "rerollear" los offsets aleatorios de los UVs.

```jsx
function Mesh(props) {
  let obj = useLoader(OBJLoader, "maxwell.obj");
  let dingusTex = useLoader(THREE.TextureLoader, "dingus_nowhiskers.jpg");
  let whiskersTex = useLoader(THREE.TextureLoader, "dingus_whiskers.tga.png");
  // ...
  useEffect(() => {
    obj.traverse((child) => {
      if (child.isMesh && child.name === "dingus") {
        child.geometry.addGroup(0, 449, 0);
        child.geometry.addGroup(449, 9, 1);
        child.material = [
          new THREE.MeshStandardMaterial({ map: dingusTex }),
          new THREE.MeshStandardMaterial({ map: whiskersTex, transparent: true, side: THREE.DoubleSide })
        ];
        // ...
      }
    });
  }, []);
  // ...
  return (
    <primitive position={props.position} object={dingus}/>
  );
}
```

La interfaz permite modificar la fuerza del desplazamiento de los UVs y "rerollear" los offsets para ver el efecto en la textura:

```jsx
<input type="range" min="0" max="1" step="0.001" onChange={ev => setStrength(ev.target.valueAsNumber)}/>
<button onClick={() => setReroll(true)}>reroll</button>
```

demostración

![demostracion cambio de uvs](./threejs_anim.gif)

El código se encuentra en [App.jsx](./threejs/src/App.jsx) y se puede correr con los comandos:

```sh
cd threejs
npm install
npm run dev
```
