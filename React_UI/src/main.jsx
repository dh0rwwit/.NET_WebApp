import { StrictMode } from 'react'
//import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from 'react-router-dom' // npm install react-router-dom
import TableRowAdd from './tableRowAdd'
import './index.css'
import App from './App.jsx'
import ReactDOM from 'react-dom/client'

ReactDOM.createRoot(document.getElementById('root')).render(
    <BrowserRouter>
        <Routes>
            <Route path="/" element={<App />} />
            <Route path="/add-row" element={<TableRowAdd />} />
            
                
            </Routes>
    </BrowserRouter>
)

// React에서 렌더링은 한 번만한다.
//createRoot(document.getElementById('root')).render(
//  <StrictMode>
//    <App />
//  </StrictMode>,
//)
