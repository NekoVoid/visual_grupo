import * as THREE from "three";
import { Canvas, useFrame } from "@react-three/fiber";
import { Line, OrbitControls } from "@react-three/drei";
import { useControls } from "leva";
import React, { useEffect, useRef, useState } from "react";


function clamp(a, min, max){
  return Math.max(min, Math.min(max, a))
}

/**
 * @param {THREE.Vector3} v 
 * @param {THREE.Vector3} vn 
 */
function setVec(v, vn){
  v.x = vn.x;
  v.y = vn.y;
  v.z = vn.z;
}

/**
 * @param {{
 *   stateA: {color: THREE.Color, rot: THREE.Quaternion}
 *   stateB: {color: THREE.Color, rot: THREE.Quaternion}
 *   bezier: [THREE.Vector3,THREE.Vector3,THREE.Vector3,THREE.Vector3]
 *   lerpT: number
 *   ghosts: number
 *   lineSegments?: number}} props 
 */
function Scene(props){

  const {
    stateA = {color: "#0000FF", rot:  new THREE.Quaternion()},
    stateB = {color: "#FF0000", rot:  new THREE.Quaternion()},
    bezier = [new THREE.Vector3(),new THREE.Vector3(),new THREE.Vector3(),new THREE.Vector3()],
    lerpT = 0, ghosts = 2, lineSegments = 50} = props;
  const bezierCurve = new THREE.CubicBezierCurve3(bezier[0],bezier[1],bezier[2],bezier[3]);

  /** @type {React.RefObject<THREE.Mesh>} */
  const meshRef = useRef();


  useFrame((state, delta) => {
    const t = state.clock.getElapsedTime();
    if(bezier) meshRef.current.position.copy(bezierCurve.getPoint(lerpT));
    if(stateA && stateB){
      if(stateA.color && stateB.color) meshRef.current.material.color = new THREE.Color().lerpColors(stateA.color, stateB.color, lerpT);
      if(stateA.rot && stateB.rot) meshRef.current.quaternion.copy(new THREE.Quaternion().slerpQuaternions(stateA.rot, stateB.rot, lerpT));
    }
  })


  return (
    <>
    <mesh ref={meshRef}>
      <boxGeometry/>
      <meshStandardMaterial color={"red"}/>
    </mesh>
    {
      Array.from({length:ghosts}).map((_,i) => {
        const t = i/(ghosts - 1);
        return (
        <mesh key={i} position={bezierCurve.getPoint(t)} quaternion={new THREE.Quaternion().slerpQuaternions(stateA.rot, stateB.rot, t)}>
          <boxGeometry/>
          <meshStandardMaterial color={new THREE.Color().lerpColors(stateA.color, stateB.color, t)} transparent opacity={0.5}/>
        </mesh>);
      })
    }
    <Line points={bezierCurve.getPoints(lineSegments)}
      vertexColors={Array.from({length:lineSegments}).map(
        (_, i) => new THREE.Color().lerpColors(stateA.color, stateB.color, i/(lineSegments - 1))
      )}
      lineWidth={2}
    />
    </>
  )
}

export default function App() {

  const {lerpT, ghosts} = useControls({
    lerpT: {value: 0.5, min: 0, max: 1, step: 0.01},
    ghosts: {value: 2, min: 2, max: 10, step: 1}
  })

  return (
    <div style={{ width: "100vw", height: "100vh", display: "grid" }}>
      <div id="canvas-container" style={{ display: "grid", width: "100%", height: "100%" }}>
        <Canvas>  
          <ambientLight intensity={0.4}/>
          <directionalLight position={[0, 0.5, 1]} intensity={1}/>
          <directionalLight position={[0, 1, 0]} intensity={1}/>
          <Scene
            stateA={{
              color: new THREE.Color("#0000FF"),
              rot: new THREE.Quaternion().setFromAxisAngle(new THREE.Vector3(1,1,1).normalize(), 3*Math.PI/4)
            }}
            stateB={{
              color: new THREE.Color("#FF0000"),
              rot: new THREE.Quaternion().setFromAxisAngle(new THREE.Vector3(0,1,0).normalize(), Math.PI/4)
            }}
            bezier={[new THREE.Vector3(-2.5,0,0),new THREE.Vector3(-2.5,-5,4),new THREE.Vector3(2.5,3,-3),new THREE.Vector3(2.5,0,0)]}
            lerpT={lerpT} ghosts={ghosts}
          />
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
