import { useState, useEffect } from 'react';
import reactLogo from './assets/react.svg';
import viteLogo from '/vite.svg';
import './App.css';

function App() {
    const [count, setCount] = useState(0);
    const [serverMessage, setServerMessage] = useState('');

    // count 변경될 때 서버로 전송
    useEffect(() => {
        fetch('http://localhost:5000/update-count', {
            method: 'POST',
            headers: { 'Content-Type': 'text/plain' },
            body: count.toString(),
        });
    }, [count]);

    // 서버에서 메시지 가져오기
    useEffect(() => {
        const fetchMessage = async () => {
            const response = await fetch('http://localhost:5000/api');
            const data = await response.text();
            setServerMessage(data);
        };

        fetchMessage();
    }, [count]); // count 변경 시마다 서버에서 가져옴

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
            <div className="card">
                <button onClick={() => setCount(count + 1)}>count is {count}</button>
                <p>Server Response: {serverMessage}</p>
            </div>
            <p className="read-the-docs">
                Click on the Vite and React logos to learn more
            </p>
        </>
    );
}

export default App;



/*import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom'; // React Router를 활용하여 URL 값 가져오기
import reactLogo from './assets/react.svg';
import viteLogo from '/vite.svg';
import './App.css';

function App() {
    const [count, setCount] = useState(0);
    const [serverMessage, setServerMessage] = useState('');
    const [searchParams, setSearchParams] = useSearchParams(); // URL 값 읽기

    // 서버에서 count 값 가져오기
    useEffect(() => {
        const fetchMessage = async () => {
            const response = await fetch('http://localhost:5000/api');
            const data = await response.text();
            setServerMessage(data);
        };

        fetchMessage();
    }, [count]); // count 변경 시마다 서버에서 가져옴

    // count가 변경될 때마다 서버로 업데이트
    useEffect(() => {
        fetch('http://localhost:5000/update-count', {
            method: 'POST',
            headers: { 'Content-Type': 'text/plain' },
            body: count.toString(),
        });
    }, [count]);

    // URL에서 count 값을 가져와 설정
    useEffect(() => {
        const countFromUrl = searchParams.get("count");
        if (countFromUrl && !isNaN(countFromUrl)) {
            setCount(parseInt(countFromUrl, 10));
        }
    }, [searchParams]);

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
            <div className="card">
                <button onClick={() => setCount(count + 1)}>count is {count}</button>
                <p>Server Response: {serverMessage}</p>
            </div>
            <p className="read-the-docs">
                Click on the Vite and React logos to learn more
            </p>
        </>
    );
}

export default App;


*/