import React, { useState } from 'react';


export default function TableRowAdd() {
    const [data, setdata] = useState([]);

    const handleLoad = async () => {
        try {
            const response = await fetch('/api/factorypgadonet/users') // 백엔드 GET API
            if (response.ok) {
                const json = await response.json();
                setdata(json) // 테이블에 반영
            }
            else {
                console.error('서버에러', await response.text())
            }
        }
        catch (err) {
            console.error('요청실패 : ', err);
        }
    };
    return (
        <div>
            <h2> sysuser테이블 사용자 목록 조회 </h2>
            <button onClick={handleLoad} > 조회  </button>

                <table border="1">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>이름</th>
                        <th>나이</th>
                    </tr>
                </thead>

                <tbody>
                    {data.map((row, idx) => (
                        <tr key={idx}>
                            <td>{row.id}</td> {/* DB컬럼 별칭과 맞춰준다.*/}
                            <td>{row.name}</td>
                            <td>{row.age}</td>
                        </tr>
                    ))}

                </tbody>
            </table>
        </div>
    )
}

//export default function TableRowAdd() {
//    return (
//        <div>
//            <h2>테이블 행 추가</h2>

//        </div>
//    );
//}