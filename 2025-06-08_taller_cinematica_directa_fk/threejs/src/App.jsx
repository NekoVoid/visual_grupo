import * as THREE from "three";
import { Canvas } from "@react-three/fiber";
import { OrbitControls } from "@react-three/drei";
import { useEffect, useRef, useState } from "react";


// This is a simple arm with two segments (shoulder and elbow) that can be rotated
// independently. The arm is made up of two boxes, one for the shoulder and one for the elbow
// The shoulder is the parent of the elbow, so when the shoulder rotates, the elbow follows.
function Arm(props){
  const shoulder = useRef();
  const elbow = useRef();

  // Original position, rotation and scale of the shoulder and elbow
  const ogShoulder = {
    position: new THREE.Vector3(0,0,0),
    rotation: new THREE.Euler(0,0,0, "XYZ"),
    scale: new THREE.Vector3(1,1,1),
  };
  const ogElbow = {
    position: new THREE.Vector3(0,-3,0),
    rotation: new THREE.Euler(0,0,0, "XYZ"),
    scale: new THREE.Vector3(1,1,1),
  };

  // This effect is used to set the position, rotation and scale of the shoulder and elbow, parameters passed as props
  useEffect(() => {
    if (props.shoulder){
      // if shoulder position prop is passed, add it to the original position, set it to the original otherwise
      if(props.shoulder.position) shoulder.current.position.addVectors(props.shoulder.position, ogShoulder.position);
      else shoulder.current.position.copy(ogShoulder.position);
      // if shoulder scale prop is passed, multiply it with the original scale, set it to the original otherwise
      if(props.shoulder.scale) shoulder.current.scale.multiplyVectors(props.shoulder.scale, ogShoulder.scale);
      else shoulder.current.scale.copy(ogShoulder.scale);
      // if shoulder rotation prop is passed, set it to the rotation prop, set it to the original otherwise
      if(props.shoulder.rotation) shoulder.current.rotation.copy(props.shoulder.rotation);
      else shoulder.current.rotation.copy(ogShoulder.rotation);
    }

    if(props.elbow){
      // same as with the shoulder for the elbow
      if(props.elbow.position) elbow.current.position.addVectors(props.elbow.position, ogElbow.position);
      else elbow.current.position.copy(ogElbow.position);
      if(props.elbow.scale) elbow.current.scale.multiplyVectors(props.elbow.scale, ogElbow.scale);
      else elbow.current.scale.copy(ogElbow.scale);
      if(props.elbow.rotation) elbow.current.rotation.copy(props.elbow.rotation);
      else elbow.current.rotation.copy(ogElbow.rotation);
    }
  }, [props.shoulder, props.elbow]);

  return (
    <group ref={shoulder}>
      <mesh position={[0, -1.5, 0]}>
        <boxGeometry args={[1, 3, 1]} />
        <meshStandardMaterial color="red" />
      </mesh>
      <group ref={elbow} position={[0, -3, 0]}>
        <mesh position={[0, -1.5, 0]}>
          <boxGeometry args={[1, 3, 1]} />
          <meshStandardMaterial color="blue" />
        </mesh>
      </group>
    </group>
  )
}

export default function App() {

  // articulations transformation state
  const [shoulder, setShoulder] = useState({
    position: new THREE.Vector3(0,2.5,0),
    rotation: new THREE.Euler(0,0,0, "XYZ"),
    scale: new THREE.Vector3(1,1,1),
  });
  const [elbow, setElbow] = useState({
    position: new THREE.Vector3(0,0,0),
    rotation: new THREE.Euler(0,0,0, "XYZ"),
    scale: new THREE.Vector3(1,1,1),
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
            <Arm elbow={elbow} shoulder={shoulder}/>
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
        {/* Elbow component rotation control */}
        <label>
          Elbow: <wbr/>
          <input type="range" min={0} max={5*Math.PI/6} step={0.01} defaultValue={0} onChange={e => {
            const value = parseFloat(e.target.value);
            setElbow(prev => ({...prev, rotation: new THREE.Euler(prev.rotation.x, prev.rotation.y, value)}));
          }}/>
        </label>
        
        {/* Shoulder component rotation control */}
        <label>
          Shoulder: <wbr/>
          <input type="range" min={-Math.PI/2} max={Math.PI/2} step={0.01} defaultValue={0} onChange={e => {
            const value = parseFloat(e.target.value);
            setShoulder(prev => ({...prev, rotation: new THREE.Euler(prev.rotation.x, prev.rotation.y, value)}));
          }}/>
        </label>
      </div>
    </div>
  );
}
