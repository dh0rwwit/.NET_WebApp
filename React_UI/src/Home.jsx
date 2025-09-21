import { useRef, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Home.css";


export default function Home() {
    const barRef = useRef(null);
    const navigate = useNavigate();
    const [form, setForm] = useState({ id: "", pw: "" });
    // 로그인 상태, 기본상태는 false로 로그인 안 된 상태
    const [isLoggedIn, setIsLoggedIn] = useState(false);

    // 로그인 상태 체크, 하단 바
    const handleBottomClick = (path) => {
        if (!isLoggedIn) {
            alert("로그인을 해주세요");
            return;
        }
        navigate(path);

    }



    useEffect(() => {

        const el = barRef.current;
        let isDown = false;
        let startX;
        let scrollLeft;

        const mouseDownHandler = (e) => {
            isDown = true;
            startX = e.pageX - el.offsetLeft;
            scrollLeft = el.scrollLeft;
        };
        const mouseLeaveHandler = () => {
            isDown = false;
        };
        const mouseUpHandler = () => {
            isDown = false;
        }
        const mouseMoveHandler = (e) => {
            if (!isDown) return;
            e.preventDefault();
            const x = e.pageX - el.offsetLeft;
            const walk = (x - startX) * 1; // 스크롤 속도
            el.scrollLeft = scrollLeft - walk;
        };

 

        el.addEventListener("mousedown", mouseDownHandler);
        el.addEventListener("mouseleave", mouseLeaveHandler);
        el.addEventListener("mouseup", mouseUpHandler);
        el.addEventListener("mousemove", mouseMoveHandler);

        return () => {
            el.removeEventListener("mousedown", mouseDownHandler);
            el.removeEventListener("mouseleave", mouseLeaveHandler);
            el.removeEventListener("mouseup", mouseUpHandler);
            el.removeEventListener("mousemove", mouseMoveHandler);
        };

    }, []);

    const onChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    }

    // 로그인 API
    // 로그인하면 하단 바 활성화
    const onLogin = (e) => {
        e.preventDefault();
        alert('로그인 : ${form.id}');
        setIsLoggedIn(true); // 폼 로그인 시 상단 상태 바 상태 갱신

    }

    return (
        // react는 최상위 노드 하나만 반환하므로 <> 써줌.
        <> 
        <div className="status-bar">
            {isLoggedIn ? (
                <div>
                <span>
                    {form.id} 님 로그인     
                </span>
                <button 
                    onClick={() => setIsLoggedIn(false)} // 로그아웃 : setIsLoggedIn을 false로 변경 -> state를 변경함.변경된 state는 react가 렌더링
                    className="status-btn">
                로그아웃    
                </button>
                </div>
                ):(
                <button
                    // onClick={() => setIsLoggedIn(true)} // setIsLoggedIn을 true로 변경 -> state를 변경함. 변경된 state는 react가 렌더링
                    className="status-btn"    
                > 로그인
                </button>
            )}
            </div>




        <div className="home">
            <h1>Main Widget</h1>

            { /* 우측 로그인 화면 */}
            <aside className="auth-card" aria-label="로그인">
                <h2 className="auth-title" htmlFor="login-id"> 로그인 </h2>
                <form onSubmit={onLogin} className="auth-form">

                    <label className="auth-label" htmlFor="login-id">ID</label>
                    <input
                        id="login-id"
                        name="id"
                        type="text"
                        value={form.id}
                        onChange={onChange}
                        className="auth-input"
                        placeholder="id를 입력하세요"
                        autoComplete="username"
                        required // 비어 있으면 입력 메세지
                    />

                    <label className="auth-label" htmlFor="login-pw">비밀번호</label>
                    <input
                        id="login-pw"
                        name="pw"
                        type="password"
                        value={form.pw}
                        onChange={onChange}
                        className="auth-input"
                        placeholder="비밀번호를 입력하세요"
                        autoComplete="current-password"
                        required  // 비어 있으면 입력 메세지
                    />

                    <button type="button" className="btn btn-outline"
                        onClick={() => navigate("auth/register")}
                    >
                    회원등록
                    </button>

                    <button type="submit" className="btn btn-primary">
                        로그인
                    </button>

                    <button type="button" className="btn btn-ghost"
                        onClick={() => navigate("/auth/password")}
                    >
                        비밀번호변경
                    </button>

                </form>
            </aside>



            {/* 하단 고정 버튼바 */}
            <div className="bottom-bar" ref={barRef}>
                <button onClick={() => handleBottomClick("/bottompages/div_plan")}>배당가정</button>
                <button onClick={() => handleBottomClick("/bottompages/net_deposit")}>종목별순입금매수액</button>
                <button onClick={() => handleBottomClick("/bottompages/div_result")}>배당내역</button>
                <button onClick={() => handleBottomClick("/bottompages/net_profit")}>손익합</button>
            </div>

        </div>

        </>
    );
}


