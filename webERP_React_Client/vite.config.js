
// 5173포트가 실행되고 있으면 다른 포트를 찾는다.
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [react()],
    server: {
        port: 5173, // Vite 개발 서버 포트
        proxy: {
            "/api": {
                target: "https://localhost:5174", // ASP.NET Core API 서버
                changeOrigin: true,
                secure: false,
            },
            "/client1": {
                target: "https://localhost:5174", // YARP 프록시 서버 (React)
                changeOrigin: true,
                secure: false,
            },
        },
    },
});


// 5173포트가 실행되고 있으면 중지 됨.
//import { defineConfig } from 'vite';
//import react from '@vitejs/plugin-react';

//export default defineConfig({
//    plugins: [react()],
//    server: {
//        host: 'localhost',
//        port: 5173, // Vite 개발 서버 포트
//        strictPort: true, // 사용 불가능하면 오류 발생 (포트 자동 변경 방지)
//        proxy: {
//            '/client1': {
//                target: 'https://localhost:5174', // YARP 리버스 프록시 서버
//                changeOrigin: true,
//                secure: false, // SSL 인증서 검증 비활성화 (개발 환경)
//                rewrite: (path) => path.replace(/^\/client1/, ''), // 경로 수정
//            },
//        },
//    },
//});



