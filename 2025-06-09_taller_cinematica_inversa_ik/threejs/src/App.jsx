import * as THREE from "three";
import { Canvas, useFrame } from "@react-three/fiber";
import { OrbitControls } from "@react-three/drei";
import { useControls } from "leva";
import { useEffect, useRef, useState } from "react";


function clamp(a, min, max){
  return Math.max(min, Math.min(max, a))
}

function dist(a, b){
  const x = a.x - b.x, y = a.y - b.y;
  return Math.sqrt(x*x + y*y);
}

/**
 * 
 * @param {THREE} target 
 * @param {*} start 
 * @param {*} positions 
 * @returns 
 */
function FABRIK2D(target, start, positions){
  if(positions.length < 2) return [];

  let lTarget = target;
  let lStart = positions[positions.length - 2];
  for(let i = positions.length - 1; i > 0; i++){
    const len = dist(positions[i], lStart);
    let 
  }
}

function Arm(props){
  const refs = Array.from({length:4},() => useRef())
  const lengths = [4,4,1]

  useFrame((state, delta) => {
    const t = state.clock.getElapsedTime();


  })

  return (
    <>
    <group ref={refs[0]}>
      <mesh position={[0,-0.5,0]}>
        <cylinderGeometry args={[2,2,1,8]}/>
        <meshStandardMaterial color="blue"/>
      </mesh>

      <group ref={refs[1]}>
        <mesh rotation={new THREE.Euler(Math.PI/2, 0, 0)}>
          <cylinderGeometry args={[0.6,0.6,,12]}/>
          <meshStandardMaterial color="green"/>
        </mesh>
        <mesh position={[0,lengths[0]/2,0]}>
          <boxGeometry args={[0.8,lengths[0],0.9]}/>
          <meshStandardMaterial color={"red"}/>
        </mesh>

        <group ref={refs[2]} position={[0,lengths[0],0]}>
          <mesh rotation={new THREE.Euler(Math.PI/2, 0, 0)}>
            <cylinderGeometry args={[0.6,0.6,,12]}/>
            <meshStandardMaterial color="green"/>
          </mesh>
          <mesh position={[0,lengths[1]/2,0]}>
            <boxGeometry args={[0.8,lengths[1],0.9]}/>
            <meshStandardMaterial color={"red"}/>
          </mesh>

          <group ref={refs[3]} position={[0,lengths[1]+0.5,0]}>
            <mesh>
              <coneGeometry args={[1,2,6]}/>
              <meshStandardMaterial color={"purple"}/>
            </mesh>
          </group>

        </group>

      </group>

    </group>
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
          {/* Arm component */}
            <Arm/>
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
