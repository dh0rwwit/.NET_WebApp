import { useState, useEffect } from 'react'
//import reactLogo from './assets/react.svg'
//import viteLogo from '/vite.svg'
import './App.css'
import TableRowAdd from './tableRowAdd';

import { Link } from 'react-router-dom';


function App() {
    const [data, setData] = useState('');
    const [showTableRowAdd, setShowTableRowAdd] = useState(false);

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
            <Counter />
            <button onClick= {() => setShowTableRowAdd(true)}> 테이블추가화면 여기에 보여주기 </button>
            { showTableRowAdd && <TableRowAdd/> }
            {/*<h2>*/}
            {/*    테이블 생성*/}
            {/*</h2>*/}
            <Link to="/add-row"> <button> 테이블 추가 페이지로 이동 </button> </Link>
            <DataTable />

            <n></n>
            <button onClick={DataTableFactory} > 조회  </button>
            <DataTableFactory />

        </div>
    );
}


function DataTable() {
    const [data, setData] = useState([]);

    useEffect(
        () => {
            fetch('/api/factorypgadonet/users') // 컨트롤러 지정
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

function DataTableFactory()
{
    const [data, setdata] = useState([]);
    const [key, setKey] = useState("A");
    const handleLoad = async (selectedKey) => {
        try {
            setKey(selectedKey); // A,B키 누를때마다 상태 업데이트, 

            const response = await fetch(`/api/factorypgadonet/userskey?key=${selectedKey}`)
            if (response.ok) // 버튼 클릭 이벤트 실행?
            {
                const json = await response.json();
                setdata(json); // select결과 넣기
            }
            else { console.error('서버에러', await response.text()) }
        }
        catch (err) { console.error("요청실패 : ",err)}
    };
    useEffect(
        () => {
            fetch(`/api/factorypgadonet/userskey?key=${key}`)
                .then(res => res.json())
                .then(json => setdata(json))
                .catch(err => console.error("api access error : ", err));
        }, [key]
    );
    return (
        <div>
            <div>
                <button onClick={() => handleLoad("A")}> factorypgadonetA 실행 </button>
                <button onClick={() => handleLoad("B")}> factorypgadonetB 실행 </button>
                <button onClick=
                    {() =>
                        {
                            setdata([]);
                            setKey("A");
                        }
                    }>테이블 초기화</button> { /* setdata[]하면 .map()에 들어갈 값 없음*/}
            </div>
            <table border='1'>
                <thead>
                    <tr>
                        <th> id </th>
                        <th> 이름 </th>
                        <th> 나이 </th>
                        <th> 이메일(B버튼) </th>
                    </tr>
                </thead>
                <tbody>
                    {
                        data.length === 0 ? (
                            <tr>
                                <td colSpan="3"> 데이터없음</td>
                            </tr>
                        ) : (
                                data.map(
                                    (person, index) => (
                                        <tr key={index}>
                                            <td> {person.id}</td>
                                            <td> {person.name}</td>
                                            <td> {person.age}</td>
                                            <td> {person.email}</td>
                                        </tr>
                                    )
                                )
                        )
                    }
                </tbody>
            </table>
        </div>
    );

    // 기존
/*
    const [data, setData] = useState([]);
    const [key, setKey] = useState("A");

    useEffect(
        () => {
            fetch("/api/factorypgadonet/userskey?key=${key}")
                .then(res => res.json())
                .then(json => setData(json))
                .catch(err => console.error("api access error : ", err));
        }, [key]
    );

    return (
        <div>
            <div>
                <button onClick={() => setKey("A")}> factorypgadonetA </button>
                <button onClick={() => setKey("B")}> factorypgadonetB </button>
            </div>
            <table border='1'>
                <thead>
                    <tr>
                        <th> 이름 </th>
                        <th> 나이 </th>
                    </tr>
                </thead>
                <tbody>
                    {
                        data.map(
                            (person, index) => (
                                <tr key={index}>
                                    <td> {person.name}</td>
                                    <td> {person.age}</td>
                                </tr>
                            )
                        )
                    }
                </tbody>
            </table>
        </div>
    );
*/
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