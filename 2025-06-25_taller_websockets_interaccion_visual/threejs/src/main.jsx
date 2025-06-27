// main.jsx
import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import './main.css'
import App from './App.jsx'
import ECGLineGraph from "./ECGLineGraph.jsx";
import AnimatedSphere from "./AnimatedSphere.jsx";

// Se importa el componente App y los componentes específicos para cada visualización
createRoot(document.getElementById('root')).render(
    <StrictMode>
        <div className="columns">
            <App Component={AnimatedSphere} title="Esfera Animada"/>
            <App Component={ECGLineGraph} title="ECG (Electrocardiograma)"/>
        </div>
    </StrictMode>,
)