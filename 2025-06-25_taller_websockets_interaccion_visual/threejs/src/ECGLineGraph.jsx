import React, {useState, useEffect} from 'react'
import {Line} from '@react-three/drei'
import * as THREE from 'three'

export default function ECGLineGraph() {
    const [points, setPoints] = useState([])
    const bufferLength = 300        // Número máximo de puntos visibles
    const pointSpacing = 0.1        // Espaciado entre puntos en el eje X

    useEffect(() => {
        const ws = new WebSocket('ws://localhost:8800')

        ws.onmessage = (event) => {
            const data = JSON.parse(event.data)     // Recibe datos del WebSocket

            setPoints((prev) => {

                // Crea un nuevo punto basado en los datos recibidos
                const newPoint = new THREE.Vector3(
                    prev.length ? prev[prev.length - 1].x + pointSpacing : 0,
                    data.ecg * 2,
                    0
                )

                // Agrega el nuevo punto al array de puntos
                let updated = [...prev, newPoint]

                // Si el número de puntos excede el bufferLength, elimina los más antiguos
                if (updated.length > bufferLength) {
                    const shiftX = updated[1].x - updated[0].x
                    updated = updated.slice(1).map(p => new THREE.Vector3(p.x - shiftX, p.y, p.z))
                }

                return updated
            })
        }

        return () => {
            ws.close()
        }
    }, [])

    // Grafica de línea con los puntos recibidos
    return (
        <group>
            <Line
                points={points}
                color="red"
                lineWidth={2}
                segments
            />
        </group>
    )
}