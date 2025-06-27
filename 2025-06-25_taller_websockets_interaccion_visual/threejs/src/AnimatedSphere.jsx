import React, {useState, useEffect, useRef} from 'react'
import {useFrame} from '@react-three/fiber'


export default function AnimatedSphere() {
    const meshRef = useRef(null)        // Referencia al mesh de la esfera
    const [position, setPosition] = useState({x: 0, y: 0})   // Posición inicial de la esfera
    const [color, setColor] = useState('#ffffff')   // Color inicial de la esfera

    useEffect(() => {
        const ws = new WebSocket('ws://localhost:8700')

        // Establece el WebSocket y maneja los mensajes entrantes
        ws.onmessage = (event) => {
            const data = JSON.parse(event.data)
            setPosition({x: data.x, y: data.y})
            setColor(data.color)
        }

        ws.onerror = () => {
            console.error('WebSocket error')
        }

        return () => {
            ws.close()
        }
    }, [])

    // Actualiza posición del objeto usando useFrame para suavidad
    useFrame(() => {
        if (meshRef.current) {
            meshRef.current.position.x += (position.x - meshRef.current.position.x) * 0.1
            meshRef.current.position.y += (position.y - meshRef.current.position.y) * 0.1
            meshRef.current.material.color.set(color)
        }
    })

    // Renderiza la esfera con geometría y material
    return (
        <mesh ref={meshRef}>
            <sphereGeometry args={[1, 32, 32]}/>
            <meshStandardMaterial/>
        </mesh>
    )
}


