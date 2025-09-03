import "./Home.css";

export default function Home() {
    return (
        <div className="home">
            <h1>Main Widget</h1>

            {/* 하단 고정 버튼바 */}
            <div className="bottom-bar">
                <button>배당가정</button>
                <button>배당내역</button>
                <button>종목별순입금매수액</button>
                <button>종목별평가</button>
                <button>손익합</button>
            </div>

        </div>
    );
}


