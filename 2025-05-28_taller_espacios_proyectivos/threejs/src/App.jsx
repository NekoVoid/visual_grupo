import * as THREE from "three";
import { Canvas } from "@react-three/fiber";
import { OrbitControls, OrthographicCamera, PerspectiveCamera } from "@react-three/drei";
import { useState } from "react";



function MyMesh() {


  return (
    <>
    <group rotation={[Math.PI/8,0,0]} position={[0,-3,0]}>
      <mesh position={[3,0,-1]} rotation={[Math.PI/8,Math.PI/8,0]}>
        <boxGeometry args={[1,1,1]}/>
        <meshStandardMaterial color="orange" />
      </mesh>
      <mesh position={[-4,0,-8]}>
        <coneGeometry args={[1,2]}/>
        <meshStandardMaterial color="orange" />
      </mesh>
      <mesh position={[0,0,-16]} rotation={[Math.PI/2,0,0]}>
        <torusGeometry args={[]}/>
        <meshStandardMaterial color="orange" />
      </mesh>
    </group>
    </>
  );
}

export default function App() {
  const [proj, setProj] = useState(true);

  return (
    <div id="root" style={{ width: "100vw", height: "100vh" }}>
      <div id="controls"
        style={{
          position: "absolute",
          left: 0,
          right: 0,
          zIndex: 1,
          padding: "0 1rem",
        }}
      >
        <h3>Camara</h3>
        <div style={{cursor: "pointer", userSelect: "none"}} onClick={() => setProj(!proj)}>
          <span style={{backgroundColor: proj ? "lightgray" : "transparent", padding: "0.2rem 0.4rem", borderRadius: "0.2rem"}}>
            Perspectiva
          </span> | <span style={{backgroundColor: !proj ? "lightgray" : "transparent", padding: "0.2rem 0.4rem", borderRadius: "0.2rem"}}>
            Ortografica
          </span>
        </div>
      </div>
      <div id="canvas-container" style={
        {
          display: "grid",
          width: "100%",
          height: "100%",

        }}
      >

        <Canvas>
          <MyMesh/>
          <ambientLight intensity={0.2} />
          <pointLight position={[0,0,-4]} intensity={15}/>
          <pointLight position={[0,0,-12]} intensity={15}/>
          <pointLight position={[0,0,0]}  intensity={15}/>
          {
            proj? <PerspectiveCamera makeDefault position={[0,0,10]}/>: <OrthographicCamera makeDefault zoom={65}/>
          }
          <OrbitControls makeDefault/>
        </Canvas>
      </div>
    </div>
  );
}
