import { BrowserRouter, Routes, Route } from "react-router-dom";
// import { HashRouter as Router, Routes, Route } from "react-router-dom";

import AppLayout from "./layouts/AppLayout";

import Home from "./Home";

import DivPlan from "./bottompages/div_plan";
import DivResult from "./bottompages/div_result";
import NetDeposit from "./bottompages/net_deposit";
import NetProfit from "./bottompages/net_profit";

import './App.css'
//import TableRowAdd from './tableRowAdd';

export default function App() {
    return (
        <Routes>
            <Route element={<AppLayout />}>
                {/*레이아웃이 감싸는 영역*/ }
                {/*한 화면이라도 정의되어 있지 않으면 런타임 에러*/}
                {/*메인*/}
                <Route path="/" element={<Home />} />

                {/*하단버튼*/}
                <Route path="/bottompages/div_plan" element={<DivPlan />} />
                <Route path="/bottompages/div_result" element={<DivResult />} />
                <Route path="/bottompages/net_deposit" element={<NetDeposit />} />
                <Route path="/bottompages/net_profit" element={<NetProfit />} />

                {/* 404 */}
                <Route path="*" element={<NotFound />} />
            </Route>
        </Routes>
    );
}

function NotFound() {
    return <div style={{ padding: 24 }}>페이지를 찾을 수 없습니다.</div>;
}
