// src/components/BottomBar.jsx
import { useNavigate } from "react-router-dom";

export default function BottomBar() {
    const navigate = useNavigate();

    const handleBottomClick = (path) => {
        // (선택) 로그인 체크 후 가드 로직 넣을 자리
        // if (!isLoggedIn) { alert("로그인이 필요합니다."); return; }
        navigate(path); // 경로로 이동...path가 정확히 어떤 것인지 정리 필요
    };

    return (
        <div className="bottom-bar">
            <button onClick={() => handleBottomClick("/bottompages/div_plan")}>배당가정</button>
            <button onClick={() => handleBottomClick("/bottompages/net_deposit")}>종목별순입금매수액</button>
            <button onClick={() => handleBottomClick("/bottompages/div_result")}>배당내역</button>
            <button onClick={() => handleBottomClick("/bottompages/net_profit")}>손익합</button>
        </div>
    );
}