import * as THREE from "three";
import { Canvas, useFrame } from "@react-three/fiber";
import { OrbitControls, Line } from "@react-three/drei";
import { useControls } from "leva";
import { useEffect, useRef, useState } from "react";


function clamp(a, min, max){
  return Math.max(min, Math.min(max, a))
}

/**
 * returns a vector that if added to a would equal b
 * @param {THREE.Vector2 | THREE.Vector3} a 
 * @param {THREE.Vector2 | THREE.Vector3} b 
 */
function vAtoB(a, b){
  return b.clone().sub(a);
}

/**
 * @param {THREE.Vector2} target
 * @param {THREE.Vector2[]} positions 
 * @returns {THREE.Vector2[]}
 */
function FABRIK2DIter(target, positions){
  if(positions.length < 2) return [];

  let lTarget = target.clone();

  /**@type {THREE.Vector2}*/
  let lStart;

  let nPositions = [lTarget];
  for(let i = positions.length - 1; i > 0; i--){
    lStart = positions[i-1];

    const segLen = lStart.distanceTo(positions[i]);
    const toStart = lStart.clone().sub(lTarget)
    toStart.normalize()
    toStart.multiplyScalar(segLen);

    nPositions.push(toStart.add(lTarget));
    lTarget = toStart;
  }

  return nPositions;
}

/**
 * @param {THREE.Vector2} target 
 * @param {THREE.Vector2} start 
 * @param {THREE.Vector2[]} positions 
 * @returns {number[]}
 */
function FABRIK2D(target, start, positions){
  /** @type {THREE.Vector2[]} */
  let nPositions = Array.from(positions);
  for(let i = 0; i < 5; i++){
    nPositions = FABRIK2DIter(target, nPositions);
    nPositions = FABRIK2DIter(start, nPositions);
  }

  /** @type {number[]} */
  let angles = [];
  for(let i = 0; i < nPositions.length-1; i++){

    /** @type {THREE.Vector2} */
    let artAngle = vAtoB(nPositions[i], nPositions[i+1]).angle() - Math.PI/2;

    artAngle -= artAngle > Math.PI? (2*Math.PI): 0;

    angles.push(artAngle);
  }
  for(let i = angles.length-1; i > 0; i--){
    angles[i] -= angles[i-1];
  }

  return angles;
}

/**
 * @param {THREE.Vector3} target 
 * @param {THREE.Vector3} base
 * @returns {[number, THREE.Vector2]}
 */
function FABRIK3DTo2DTargletYL(target, base){
  const toTarget3D = target.clone().sub(base)
  const toTargetZX = new THREE.Vector2(toTarget3D.x, toTarget3D.z)
  
  // creates toTarget vector projected on a plane tha could be described as having a normal
  // ortogonal to both the toTarget and Y basis vectors
  // (you could use a cross product to get this normal, but why bother)
  const toTargetYL = new THREE.Vector2(toTargetZX.length(), toTarget3D.y);
  
  return [toTargetZX.angle(), toTargetYL]
}

/**
 * this function asumes you are giving initial conditions, base rotates around Y axis and their articualtions all rotate around the z axis
 * @param {THREE.Vector3} base 
 * @param {THREE.Vector3} target
 * @param {number[]} lengths
 * @returns {{base: number, angles: number[]}}
*/
function ArmFabrik(target, base, lengths){

  const [ZXangle, toTargetYL] = FABRIK3DTo2DTargletYL(target, base);

  const positions = [new THREE.Vector2(0,0)];
  for(let i = 0; i < lengths.length; i++){
    positions.push(new THREE.Vector2(0,positions[i].y+lengths[i]));
  }

  return {base: -ZXangle, angles: FABRIK2D(toTargetYL, positions[0], positions)};
}

/**
 * @param {{target: THREE.Vector3}} props
 */
function Arm(props){
  const lengths = [4,4,2];
  const limits = [[-Math.PI/4, Math.PI/4],
  [-Math.PI/2, Math.PI/2],
  [-Math.PI/2, Math.PI/2]];
  
  /** @type {React.RefObject<THREE.Group>[]} */
  const refs = Array.from({length: lengths.length+1},() => useRef());

  useFrame((state, delta) => {
    const t = state.clock.getElapsedTime();

    if(props.target){
      const nState = ArmFabrik(props.target, refs[0].current.position, lengths);

      refs[0].current.rotation.y = nState.base;

      for(let i = 0; i < nState.angles.length; i++){
        refs[i + 1].current.rotation.z = nState.angles[i];//clamp(nState.angles[i], limits[i][0], limits[i][1]);
      }
    }
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

          <group ref={refs[3]} position={[0,lengths[1],0]}>
            
            <mesh rotation={new THREE.Euler(Math.PI/2, 0, 0)}>
              <cylinderGeometry args={[0.6,0.6,,12]}/>
              <meshStandardMaterial color="green"/>
            </mesh>
            <mesh position={[0,lengths[2]/4,0]}>
              <boxGeometry args={[0.8,lengths[2]/2,0.9]}/>
              <meshStandardMaterial color={"red"}/>
            </mesh>
            <mesh position={[0,3*lengths[2]/4,0]}>
              <coneGeometry args={[0.6,lengths[2]/2]}/>
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
      height, dist, rot
     } = useControls({
      height: {value:5, min:0, max:10, step:0.1},
      dist: {value:5, min:0, max:10, step:0.1},
      rot: {value:0, min:0, max:2*Math.PI, step:0.1},
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
            <Arm target={new THREE.Vector3(dist*Math.cos(rot),height,dist*Math.sin(rot))}/>
            <Line points={[[0,0,0],[dist*Math.cos(rot),height,dist*Math.sin(rot)]]} color={"orange"} lineWidth={5}/>
            <mesh position={[dist*Math.cos(rot),height,dist*Math.sin(rot)]}>
              <sphereGeometry args={[0.2]}/>
              <meshBasicMaterial color={"orange"}/>
            </mesh>
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
