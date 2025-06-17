import * as THREE from "three";
import { Canvas, useFrame } from "@react-three/fiber";
import { OrbitControls } from "@react-three/drei";
import { useControls } from "leva";
import { useEffect, useRef, useState } from "react";


function clamp(a, min, max){
  return Math.max(min, Math.min(max, a))
}

function Scene(props){

  useFrame((state, delta) => {
    const t = state.clock.getElapsedTime();
  })

  return (
    <>
    </>
  )
}

export default function App() {

  return (
    <div style={{ width: "100vw", height: "100vh", display: "grid" }}>
      <div id="canvas-container" style={
        {
          display: "grid",
          width: "100%",
          height: "100%",
          
        }}
      >

        <Canvas>  
          <ambientLight intensity={0.4}/>
          <directionalLight position={[0, 0.5, 1]} intensity={1}/>
          <directionalLight position={[0, 1, 0]} intensity={1}/>
            <Scene/>
          <OrbitControls makeDefault/>
        </Canvas>
      </div>
      <div id="controls"
        style={{
          position: "absolute",
          left: 0,
          right: 0,
          padding: "1rem",
          display: "grid"
        }}
      >
      </div>
    </div>
  );
}
