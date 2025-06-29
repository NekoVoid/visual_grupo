import React, { useState } from 'react'
import { Canvas, useFrame } from '@react-three/fiber'
import { useControls } from 'leva'
import './App.css'


const Figure = () => {
    // Configuración de controles con Leva
    const {
        scale,              // Escala del objeto
        color,              // Color del material
        rotate,             // Activar/desactivar rotación
        position,           // Posición del objeto en el espacio 3D
        opacity,            // Opacidad del material
        wireframe,          // Modo wireframe del material
        shape               // Forma del objeto (caja, esfera, cilindro)
    } = useControls({
        scale: { value: 1, min: 0.5, max: 4, step: 0.1 },
        color: '#ffffff',
        rotate: false,
        position: { value: [0, 0, 0], step: 0.1 },
        opacity: { value: 1, min: 0, max: 1, step: 0.01 },
        wireframe: false,
        shape: { options: ['box', 'sphere', 'cylinder'] }
    })

    // Estado para la rotación del objeto
    const [rotationY, setRotationY] = useState(0)

    // Animación de rotación del objeto
    useFrame(() => {
        if (rotate) {
            setRotationY((prev) => prev + 0.01)
        }
    })

    // Selección de la geometría según la forma elegida
    let geometry = null
    if (shape === 'box') {
        geometry = <boxGeometry args={[scale, scale, scale]} />
    } else if (shape === 'sphere') {
        geometry = <sphereGeometry args={[scale / 2, 32, 32]} />
    } else if (shape === 'cylinder') {
        geometry = <cylinderGeometry args={[scale / 2, scale / 2, scale, 32]} />
    }

    // Renderizado del objeto 3D
    return (
        <mesh rotation={[0, rotationY, 0]} position={position}>
            {geometry}
            <meshStandardMaterial
                color={color}
                opacity={opacity}
                transparent={true}
                wireframe={wireframe}/>
        </mesh>
    )
};

// Componente principal de la aplicación
const App = () => {
    return (
        <Canvas shadows style={{width: '100vh', height: '100vh'}} dpr={[1, 2]}>
            <ambientLight intensity={0.5}/>
            <pointLight position={[10, 10, 10]}/>
            <Figure/>
        </Canvas>
    );
};

export default App
