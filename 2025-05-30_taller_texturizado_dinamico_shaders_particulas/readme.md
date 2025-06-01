# Texturizado Creativo: Materiales Dinámicos con Shaders y Datos

Exploración de técnicas avanzadas de texturizado y partículas usando shaders personalizados en Three.js y React Three Fiber.

## Three.js

Se implementa una escena con dos materiales personalizados mediante shaders GLSL:
- Un plano animado con desplazamiento y sombreado dinámico.
- Un sistema de partículas tipo "fuego" generado y animado completamente en el GPU, donde cada partícula es un plano orientado hacia la cámara y su color varía según su ciclo de vida.

El sistema de partículas utiliza atributos personalizados para la vida y velocidad de cada partícula, y el shader calcula la posición, orientación y color en tiempo real. El plano de fondo también utiliza un shader para simular una superficie ondulante e iluminada.


### plano animado
```jsx
const ColorShiftMaterial = shaderMaterial(
  { time: 0, color: new THREE.Color(0.2, 0.0, 0.1), lightPosition: new THREE.Vector3(0, 1, 0),
    initRotMatrix: new THREE.Matrix4(
      1,                    0,                   0, 0,
      0,  Math.cos(Math.PI/2), Math.sin(Math.PI/2), 0,
      0, -Math.sin(Math.PI/2), Math.cos(Math.PI/2), 0,
      0,                    0,                   0, 1
    )
  },
  // vertex shader
  /*glsl*/`
    uniform mat4 initRotMatrix;
    uniform float time;
    varying vec3 fragNormal;
    varying vec3 fragPos;

    void main() {
      vec3 initPos = (initRotMatrix * vec4(position, 1.0)).xyz;
      
      float xFreq = 0.2;
      float zFreq = 1.0;
      float xAmp = 2.0;
      float zAmp = 0.5;

      float displaceFactorX = xFreq * initPos.x + time;
      float displaceFactorZ = zFreq * initPos.z + time * 3.0;

      vec3 normalX = normalize(vec3(-xFreq * xAmp * cos(displaceFactorX), 1.0, 0.0));
      vec3 normalZ = normalize(vec3(0.0, 1.0, -zFreq * zAmp * cos(displaceFactorZ)));

      fragNormal = normalize(mix(normalX, normalZ, 0.5));
    

      initPos += vec3(0.0, xAmp*sin(displaceFactorX) + zAmp*sin(displaceFactorZ), 0.0);
      fragPos = initPos;
      gl_Position = projectionMatrix * modelViewMatrix * vec4(initPos, 1.0);
    }
  `,
  // fragment shader
  /*glsl*/`
    uniform vec3 color;
    uniform vec3 lightPosition;
    varying vec3 fragPos;
    varying vec3 fragNormal;
    void main() {
      vec3 lightDir = lightPosition - fragPos;

      float lDistSqr = dot(lightDir, lightDir) + 1e-6;
      float lightIntensity = clamp(90.0*dot(normalize(fragNormal), normalize(lightDir))/lDistSqr, 0.0, 1.0);

      gl_FragColor = vec4(mix(color, vec3(1.0), lightIntensity), 1.0);
    }
  `
)
```

### particulas de fuego
```jsx
const FireParticleMaterial = shaderMaterial(
  {
    sColor: new THREE.Color(1.0, 1.0, 0.0), eColor: new THREE.Color(1.0, 0.2, 0.0),
    time: 0, acc: new THREE.Vector3(0, 5, 0), drag: 0.22, particleCenter: new THREE.Vector3(0, 0, 0)
  },
  // vertex shader
  /*glsl*/`
    uniform float time;
    uniform float drag;
    uniform vec3 acc;
    uniform vec3 particleCenter;

    uniform vec3 sColor;
    uniform vec3 eColor;

    attribute float life;
    attribute vec3 speed;

    varying vec3 vColor;

    mat4 rotXMatrix(float angle) {
      return mat4(
        1.0, 0.0, 0.0, 0.0,
        0.0, cos(angle), -sin(angle), 0.0,
        0.0, sin(angle), cos(angle), 0.0,
        0.0, 0.0, 0.0, 1.0
      );
    }
    mat4 rotYMatrix(float angle) {
      return mat4(
        cos(angle), 0.0, sin(angle), 0.0,
        0.0, 1.0, 0.0, 0.0,
        -sin(angle), 0.0, cos(angle), 0.0,
        0.0, 0.0, 0.0, 1.0
      );
    }

    void main() {
      float t = mod(time, life);
      vec3 disp = (speed * t) + (acc * t * t * 0.5) - (drag * t);

      vec3 locCenter = particleCenter + disp;
      
      vec3 fvec = vec3(0.0, 0.0, 1.0);
      
      vec3 camDir = (cameraPosition - locCenter);
      
      vec3 camDXZ = normalize(vec3(camDir.x, 0.0, camDir.z));
      vec3 camDY = normalize(camDir);
      
      float signXZ = -sign(camDXZ.x);
      float signY = sign(camDY.y);
      
      float angleXZ = acos(dot(fvec, camDXZ)) * signXZ;
      float angleY = acos(dot(camDXZ, camDY)) * signY;
      
      
      mat4 rotX = rotXMatrix(angleY);
      mat4 rotY = rotYMatrix(angleXZ);

      float lifeStage = (t / life);
      vec3 pos = position * (1.0 - lifeStage);
      pos = (rotY * rotX * vec4(pos, 1.0)).xyz;

      pos = pos + disp;
      vColor = mix(sColor, eColor, lifeStage);

      gl_Position = projectionMatrix * modelViewMatrix * vec4(pos, 1.0);
    }
  `,
  // fragment shader
  /*glsl*/`
    uniform vec3 sColor;
    uniform vec3 eColor;
    uniform float time;

    varying vec3 vColor;

    void main() {
      gl_FragColor = vec4(vColor, 1.0);
    }
  `
)
```

### setup de escena (resumido)
```jsx
function ParticleScene() {
  // ...
  useFrame((state) => {
    const time = state.clock.getElapsedTime();
    // Actualiza posición de la "fuente" de partículas y uniformes
    materialRef.current.time = time;
    materialRef.current.lightPosition = newfirePos.clone();
    partMesh.current.material.time = time;
    setFirePos(newfirePos);
  });
  // ...
  return (
    <group>
      <mesh ref={partMesh} position={firePos}>
        <fireParticleMaterial side={THREE.DoubleSide} particleCenter={firePos}/>
      </mesh>
      <mesh>
        <planeGeometry args={[100, 100, 300, 300]} />
        <colorShiftMaterial ref={materialRef} side={THREE.DoubleSide} color={new THREE.Color(0.1,0.0,0.2)}/>
      </mesh>
    </group>
  );
}
```

Demostración

![animacion threejs](./threejs_anim.gif)

El código se encuentra en [App.jsx](./threejs/src/App.jsx) y se puede correr con los comandos:

```sh
cd threejs
npm install
npm run dev
```
