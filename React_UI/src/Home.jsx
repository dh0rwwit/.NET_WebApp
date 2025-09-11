import { useRef, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./Home.css";


export default function Home() {
    const barRef = useRef(null);
    const navigate = useNavigate();
    const [form, setForm] = useState({ id: "", pw: "" });



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
    }


    return (
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
                        required
                    >
                    </input>

                    <label>password</label>
                    <input
                        id="login-pw"
                        name="pw"
                        type="password"
                        value={form.pw}
                        onChange={onChange}
                        className="auth-input"
                        placeholder="비밀번호를 입력하세요"
                        autoComplete="current-password"
                        required
                    >
                    </input>

                    <div className="auth-actions">
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
                    </div>
                </form>
            </aside>


            {/* 하단 고정 버튼바 */}
            <div className="bottom-bar" ref={barRef}>
                <button onClick={() => navigate("/bottompages/div_plan")}>배당가정</button>
                <button onClick={() => navigate("/bottompages/net_deposit")}>종목별순입금매수액</button>
                <button onClick={() => navigate("/bottompages/div_result")}>배당내역</button>
                <button onClick={() => navigate("/bottompages/net_profit")}>손익합</button>
            </div>

        </div>
    );
}


