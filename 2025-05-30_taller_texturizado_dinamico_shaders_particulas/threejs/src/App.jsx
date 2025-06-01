import * as THREE from "three";
import { Canvas, useFrame } from "@react-three/fiber";
import { OrbitControls, shaderMaterial } from "@react-three/drei";

import { extend } from '@react-three/fiber'
import { useEffect, useRef, useState } from "react";
import { randInt } from "three/src/math/MathUtils.js";

function addPlaneToGeometry(geometry = new THREE.BufferGeometry()) {
  const positionArray = geometry.getAttribute("position")?.array;
  const normalArray = geometry.getAttribute("normal")?.array;
  const uvArray = geometry.getAttribute("uv")?.array;

  geometry.setAttribute("position", new THREE.BufferAttribute(new Float32Array([...(positionArray ?? []),
    -0.5,  0.5, 0,
     0.5,  0.5, 0,
    -0.5, -0.5, 0]), 3));
  geometry.setAttribute("normal", new THREE.BufferAttribute(new Float32Array([...(normalArray ?? []),
    0,0,1, 0,0,1, 0,0,1]), 3));
  geometry.setAttribute("uv", new THREE.BufferAttribute(new Float32Array([...(uvArray ?? []),
    0,1, 1,1, 0,0]), 2));

  const index = geometry.getIndex();
  const iLen = index ? index.array.length : 0;
  geometry.setIndex([...(index ? index.array : []), iLen, iLen + 2, iLen + 1]);
}

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

      float invLifeStage = 1.0 - (t / life);
      vec3 pos = position * invLifeStage;
      pos = (rotY * rotX * vec4(pos, 1.0)).xyz;

      pos = pos + disp;
      vColor = mix(sColor, eColor, invLifeStage);

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

extend({ ColorShiftMaterial });
extend({ FireParticleMaterial });

function ParticleScene() {
  const materialRef = useRef();
  const partMesh = useRef();
  const [firePos, setFirePos] = useState(new THREE.Vector3(0,0,0));

  const particleCount = 1000;
  const particleMaxLife = 5;
  const particleGeometry = ((count)=>{
  const geometry = new THREE.BufferGeometry()
    for(let i = 0; i < count; i++) {
      addPlaneToGeometry(geometry);
    }
    return geometry;
  });
  
  useFrame((state) => {
    const time = state.clock.getElapsedTime();

    const newfirePos = new THREE.Vector3(20*Math.cos(time), -10*Math.sin(time/3)+15, 20*Math.sin(time));

    materialRef.current.time = time;
    materialRef.current.lightPosition = newfirePos.clone();

    partMesh.current.material.time = time;

    setFirePos(newfirePos);
  });


  useEffect(() => {
    if (partMesh.current) {
      partMesh.current.geometry = particleGeometry(particleCount);

      const ones = Array(particleCount).fill(1);

      partMesh.current.geometry.setAttribute("life", new THREE.BufferAttribute(
        new Float32Array(
          ones.map(() => Array(3).fill(Math.pow(particleMaxLife, Math.random()))).flat()
        ), 1));
      partMesh.current.geometry.setAttribute("speed", new THREE.BufferAttribute(
        new Float32Array(ones.map(() => {
          const speed = new THREE.Vector3();
          speed.setFromCylindricalCoords(
            2*Math.random(),
            Math.random()*Math.PI*2,
            -Math.random()
          );
          const arr = speed.toArray();
          return [arr,arr,arr].flat();
        }
      ).flat()), 3));

      partMesh.current.geometry.computeBoundingSphere();
      partMesh.current.geometry.computeBoundingBox();
    }
    console.log(partMesh.current);
  }, []);
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

export default function App() {

  return (
    <div id="root" style={{ width: "100vw", height: "100vh" }}>
      <div id="controls"
        style={{
          position: "absolute",
          left: 0,
          right: 0,
        }}
      >
      </div>
      <div id="canvas-container" style={
        {
          display: "grid",
          width: "100%",
          height: "100%",

        }}
      >

        <Canvas>
          <ParticleScene/>
          <OrbitControls makeDefault/>
        </Canvas>
      </div>
    </div>
  );
}
