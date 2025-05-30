import * as THREE from "three";
import { Canvas, useLoader} from "@react-three/fiber";
import { OrbitControls } from "@react-three/drei";
import { OBJLoader } from "three-stdlib";
import { useEffect } from "react";
import { useState } from "react";

function Mesh(props) {
  

  let obj = useLoader(OBJLoader, "maxwell.obj");
  let dingusTex = useLoader(THREE.TextureLoader, "dingus_nowhiskers.jpg");
  let whiskersTex = useLoader(THREE.TextureLoader, "dingus_whiskers.tga.png");
  let [dingus, setDingus] = useState(new THREE.Mesh());
  let [ogUvs, setOgUvs] = useState();
  let [uvsOffset, setUvsOffset] = useState();

  useEffect(() => {
    obj.traverse((child) => {
      console.log(child);
      if (child.isMesh) {
        if( child.name === "dingus") {
          let newDingus = child;
  
          newDingus.geometry.addGroup(0, 449, 0);
          newDingus.geometry.addGroup(449, 9, 1);
  
          newDingus.material = [
            new THREE.MeshStandardMaterial({
              map: dingusTex,
              name: "dingus",
            }),
            new THREE.MeshStandardMaterial({
              map: whiskersTex,
              transparent: true,
              side: THREE.DoubleSide,
              name: "whiskers",
            })
          ];

          setOgUvs(newDingus.geometry.getAttribute("uv"));
          let offsets = newDingus.geometry.getAttribute("uv").array.map((v) => (Math.random() * 2 - 1));
          console.log(offsets);
          setUvsOffset(newDingus.geometry.getAttribute("uv").array.map((v) => v * 2));
          setDingus(newDingus);
        }
      }
    }
    );
  }, []);

  useEffect(() => {
    if (dingus && ogUvs) {
      let {moduvs = false, reroll = false, strength = 0.001} = props;
      if (moduvs) {
        let uvs = ogUvs.clone();
        let localOffsets = uvsOffset;
        if (reroll) {
          localOffsets = uvs.array.map((v) => (Math.random() * 2 - 1));
          setUvsOffset(localOffsets);
        }
        let newUvs = [];
        for (let i = 0; i < uvs.count; i++) {
          newUvs.push(ogUvs.array[i * 2] + uvsOffset[i] * strength);
          newUvs.push(ogUvs.array[i * 2 + 1] + uvsOffset[i] * strength);
        }
        let newDingus = dingus.clone();
        newDingus.geometry.setAttribute("uv", new THREE.Float32BufferAttribute(newUvs, 2));
        setDingus(newDingus);
      }
    }
  }
  , [props.moduvs, props.reroll, props.strength]);


  return (
    <>
      <primitive position={props.position} object={dingus ?? <mesh><boxGeometry/><meshBasicMaterial/></mesh>}/>
    </>
  );
};

export default function App() {

  const [strength, setStrength] = useState(0);
  const [reroll, setReroll] = useState(false);

  useEffect(() => {if(reroll){setTimeout(() => setReroll(false),0)}}, [reroll]);

  return (
    <div id="root" style={{ width: "100vw", height: "100vh" }}>
      <div id="controls"
        style={{
          position: "absolute",
          left: 0,
          right: 0,
          zIndex: 1,
          padding: "0 1rem"
        }}
      >
        <h3>uvs offset strength</h3>
        <input type="range" min="0" max="1" step="0.001" defaultValue="0" id="strength" onChange={ev => setStrength(ev.target.valueAsNumber)}/>
        <br/>
        <button onClick={(ev) => {ev.preventDefault();setReroll(true)}}>reroll</button>
      </div>
      <div id="canvas-container" style={
        {
          display: "grid",
          width: "100%",
          height: "100%",

        }}
      >

        <Canvas>  
          <ambientLight intensity={0.4}/>
          <Mesh moduvs={true} strength={strength} reroll={reroll} position={[0,-10,0]}/>
          <OrbitControls makeDefault/>
        </Canvas>
      </div>
    </div>
  );
}
