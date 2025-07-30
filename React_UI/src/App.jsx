import { useState, useEffect } from 'react'
//import reactLogo from './assets/react.svg'
//import viteLogo from '/vite.svg'
import './App.css'


function App() {
    const [data, setData] = useState('');

    useEffect(() => {
        fetch('/api/test/hello') // 백엔드 API
            //.then(response => response.text())
            //.then(text => setData(text))
            //.catch(err => console.error('API 호출 실패', err));
            .then(response => response.json())
            .then(json => setData(json.message))
            .catch(err => console.error('API failure', err));

    }, []);

    return (
        <div>
            <h1>Vite + React</h1>
            <p>API 응답: {data}</p>

            <Counter/>

            <h2>
                테이블 생성
            </h2>
            <DataTable />

        </div>
    );
}


function DataTable() {
    const [data, setData] = useState([]);

    useEffect(
        () => {
            fetch('/api/pgAdoNet/users')
                .then(res => res.json())
                .then(json => setData(json))
                .catch(err => console.error('API 호출 실패 : ', err));
        }, []);

    return (
        <table border="1">
            <thread>
                <tr>
                    <th> 이름 </th>
                    <th> 나이 </th>
                </tr>
            </thread>
            <tbody>
                {
                    data.map((person, index) => (
                        <tr key={index}>
                            <td> {person.name} </td>
                            <td> {person.age} </td>
                        </tr>
                    ))}
            </tbody>
        </table>    
    )


    //const data = [
    //    { name: "khkim", age: 11 },
    //    { name: "dhkim1", age: 12 },
    //    { name: "dhkim2", age: 13 }
    //];

    //return (
    //    <table border="1">
    //        <thead>
    //            <tr>
    //                <th>이름</th>
    //                <th>나이</th>
    //            </tr>
    //        </thead>
    //        <tbody>
    //            {data.map((person, index) =>
    //            (
    //                <tr key={index}>
    //                    <td>{person.name}</td>
    //                    <td>{person.age}</td>
    //                </tr>
    //            ))}
    //        </tbody>
    //    </table>
    //);

}

function Counter()
{
    const [cnt, setCnt] = useState(0);

    return  (
        <div>
            <p> 현재 값: {cnt}</p>
            <button onClick = {() => setCnt(cnt+1)}> +1</button>
        </div>
    );
}





export default App;
/*function App() {
    const [count, setCount] = useState(0)
    const [message, setMessage] = useState('')

    useEffect(() => {
        // React 개발 서버에서 API 서버로 요청 보내기
        fetch('/api/test/hello') // Vite proxy 설정을 통해 ASP.NET Core로 프록시됨
            .then((res) => res.json())
            .then((data) => {
                setMessage(data.message)
            })
            .catch((err) => {
                console.error("API 호출 오류:", err)
            })
    }, [])

    return (
        <>
            <div>
                <a href="https://vite.dev" target="_blank">
                    <img src={viteLogo} className="logo" alt="Vite logo" />
                </a>
                <a href="https://react.dev" target="_blank">
                    <img src={reactLogo} className="logo react" alt="React logo" />
                </a>
            </div>
            <h1>Vite + React</h1>
            <h2>{message}</h2> { ASP.NET Core API에서 받은 메시지 표시 }
            <div className="card">
                <button onClick={() => setCount((count) => count + 1)}>
                    count is {count}
                </button>
                <p>
                    Edit <code>src/App.jsx</code> and save to test HMR
                </p>
            </div>
            <p className="read-the-docs">
                Click on the Vite and React logos to learn more
            </p>
        </>
    )
}*/