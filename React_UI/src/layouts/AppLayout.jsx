// src/layouts/AppLayout.jsx
import { Outlet } from "react-router-dom";
import BottomBar from "../components/BottomBar";

export default function AppLayout() {
    return (
        <div className="app-shell">
            {/* 상단 헤더/사이드바가 필요하면 여기에 추가 */}
            <main className="page-content">
                <Outlet /> {/* 자식 페이지가 여기 렌더링 */}
            </main>

            {/* 공통 하단 바 */}
            <BottomBar />
        </div>
    );
}