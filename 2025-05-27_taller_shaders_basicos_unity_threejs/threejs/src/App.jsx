import * as THREE from "three";
import { Canvas } from "@react-three/fiber";
import { OrbitControls, shaderMaterial } from "@react-three/drei";

import { extend } from '@react-three/fiber'
import { useRef } from "react";

const ColorShiftMaterial = shaderMaterial(
  { time: 0, color: new THREE.Color(0.2, 0.0, 0.1)},
  // vertex shader
  /*glsl*/`
    void main() {
      gl_Position = projectionMatrix * modelViewMatrix * vec4(position, 1.0);
    }
  `,
  // fragment shader
  /*glsl*/`
    uniform vec3 color;
    void main() {
      gl_FragColor = vec4(color, 1.0);
    }
  `
)

extend({ ColorShiftMaterial })

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
          <mesh>
            <planeGeometry args={[10,10,80,80]}/>
            <colorShiftMaterial lightPosition={new THREE.Vector3(2,2,2)}/>
          </mesh>
          <OrbitControls makeDefault/>
        </Canvas>
      </div>
    </div>
  );
}
