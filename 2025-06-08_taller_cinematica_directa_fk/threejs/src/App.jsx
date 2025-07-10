import * as THREE from "three";
import { Canvas, useFrame } from "@react-three/fiber";
import { OrbitControls } from "@react-three/drei";
import { useControls } from "leva";
import { useEffect, useRef, useState } from "react";


function clamp(a, min, max){
  return Math.max(min, Math.min(max, a))
}

function Arm(props){
  const baseRef = useRef();
  const artARef = useRef();
  const artBRef = useRef();
  const tipRef = useRef();

  const lineRef = useRef();
  const linePoints = useRef([]);

  const lengthArtA = 4;
  const lengthArtB = 4;

  useFrame((state, delta) => {
    const t = state.clock.getElapsedTime();
    const {factor = {}, displacement = {}}= props;

    baseRef.current.rotation.y = Math.sin(t)*Math.PI*(factor.base ?? 1) + (displacement.base ?? 0);
    artARef.current.rotation.z = clamp(Math.sin(t)*Math.PI*0.25*(factor.artA ?? 1) + (displacement.artA ?? 0), -Math.PI/4, Math.PI/4);
    artBRef.current.rotation.z = clamp(Math.sin(t)*Math.PI*0.5*(factor.artB ?? 1) + (displacement.artB ?? 0), -Math.PI/2, Math.PI/2);

    if(t < 2){
      const worldPos = tipRef.current.localToWorld(new THREE.Vector3(0,1,0));
      linePoints.current.push(worldPos);
      lineRef.current.geometry = new THREE.BufferGeometry().setFromPoints(linePoints.current);
    }else{
      const worldPos = tipRef.current.localToWorld(new THREE.Vector3(0,1,0));

      linePoints.current.push(worldPos);
      linePoints.current.shift();

      lineRef.current.geometry = new THREE.BufferGeometry().setFromPoints(linePoints.current);
    }

  })

  return (
    <>
    <line ref={lineRef}>
      <lineBasicMaterial color={"orange"}/>
    </line>
    <group ref={baseRef}>
      <mesh position={[0,-0.5,0]}>
        <cylinderGeometry args={[2,2,1,8]}/>
        <meshStandardMaterial color="blue"/>
      </mesh>
      <group ref={artARef}>
        <mesh rotation={new THREE.Euler(Math.PI/2, 0, 0)}>
          <cylinderGeometry args={[0.6,0.6,,12]}/>
          <meshStandardMaterial color="green"/>
        </mesh>
        <mesh position={[0,lengthArtA/2,0]}>
          <boxGeometry args={[0.8,lengthArtA,0.9]}/>
          <meshStandardMaterial color={"red"}/>
        </mesh>
        <group ref={artBRef} position={[0,lengthArtA,0]}>
          <mesh rotation={new THREE.Euler(Math.PI/2, 0, 0)}>
            <cylinderGeometry args={[0.6,0.6,,12]}/>
            <meshStandardMaterial color="green"/>
          </mesh>
          <mesh position={[0,lengthArtB/2,0]}>
            <boxGeometry args={[0.8,lengthArtB,0.9]}/>
            <meshStandardMaterial color={"red"}/>
          </mesh>
          <group ref={tipRef} position={[0,lengthArtB+0.5,0]}>
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
  const {
    base_Factor, artA_Factor, artB_Factor,
    base_Displacement, artA_Displacement, artB_Displacement
   } = useControls({
    base_Factor: {value:0.5, min:0, max:1, step:0.05},
    artA_Factor: {value:1, min:0, max:1, step:0.05},
    artB_Factor: {value:1, min:0, max:1, step:0.05},
    base_Displacement: {value:0, min:-Math.PI, max:Math.PI, step:0.05},
    artA_Displacement: {value:0, min:-Math.PI/4, max:Math.PI/4, step:0.05},
    artB_Displacement: {value:0, min:-Math.PI/2, max:Math.PI/2, step:0.05}
  });

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
            <Arm factor={{base: base_Factor, artA: artA_Factor, artB: artB_Factor}} displacement={{base: base_Displacement, artA: artA_Displacement, artB: artB_Displacement}}/>
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
