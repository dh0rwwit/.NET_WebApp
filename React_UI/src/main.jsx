import { StrictMode } from 'react'
//import { createRoot } from 'react-dom/client'
import { BrowserRouter, Routes, Route } from 'react-router-dom' // npm install react-router-dom
import TableRowAdd from './tableRowAdd'
//import './index.css'
import App from './App.jsx'
import ReactDOM from 'react-dom/client'

import Home from "./Home.jsx"

//ReactDOM.createRoot(document.getElementById('root')).render(
//    <BrowserRouter>
//        <Routes>
//            <Route path="/" element={<Home />} /> {/*맨 처음 보게 될 화면 App에서 Home으로 수정수정*/}
//            <Route path="/add-row" element={<TableRowAdd />} />
//            </Routes>
//    </BrowserRouter>
//)

// BrowserRouter 렌더링은 App.jsx에서 한 번 만한다.
ReactDOM.createRoot(document.getElementById('root')).render(
    <BrowserRouter>
    <App />
    </BrowserRouter >,
)
