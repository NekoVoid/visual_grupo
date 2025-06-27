import React from "react";
import {Canvas} from "@react-three/fiber";
import {OrbitControls} from '@react-three/drei'
import './App.css'

const App = ({ Component, title }) => {
    return (
        <div style={{ width: '100%', height: '100%', display: 'flex', flexDirection: 'column' }}>
            <h2 style={{ textAlign: 'center', margin: 0, padding: '0.5em 0', flex: '0 0 auto' }}>{title}</h2>
            <div style={{ flex: '1 1 0', minHeight: 0 }}>
                <Canvas
                    shadows
                    style={{ width: '100%', height: '100%' }}
                    camera={{ position: [0, 0, 10], fov: 50 }}
                    dpr={[1, 2]}
                >
                    <ambientLight intensity={0.5} />
                    <pointLight position={[10, 10, 10]} />
                    <Component />
                    <OrbitControls />
                </Canvas>
            </div>
        </div>
    );
};
export default App
