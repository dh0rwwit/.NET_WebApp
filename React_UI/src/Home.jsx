import { useRef, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./Home.css";


export default function Home() {
    const barRef = useRef(null);
    const navigate = useNavigate();


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


    return (
        <div className="home">
            <h1>Main Widget</h1>

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


