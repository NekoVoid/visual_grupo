import * as THREE from "three";
import { Canvas, useFrame } from "@react-three/fiber";
import { OrbitControls, shaderMaterial } from "@react-three/drei";

import { extend } from '@react-three/fiber'
import { use, useRef } from "react";

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

      float lightIntensity = clamp(dot(normalize(fragNormal), normalize(lightDir))/(length(lightDir)/8.0 + 0.1e-6), 0.0, 1.0);

      gl_FragColor = vec4(mix(color, vec3(1.0), lightIntensity), 1.0);
    }
  `
)

extend({ ColorShiftMaterial });

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
          <Plane/>
          <OrbitControls makeDefault/>
        </Canvas>
      </div>
    </div>
  );
}
