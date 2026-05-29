import React, { useEffect, useState, createContext } from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";

import "./App.scss";
import "../node_modules/primereact/resources/themes/saga-blue/theme.css";
import "../node_modules/primereact/resources/primereact.min.css";
import "../node_modules/primeicons/primeicons.css";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.min.js';
import 'jquery/dist/jquery';

import LoaderComponent from "./components/common/LoaderComponent";

import { HOME_ROUTES, ROUTE_PATH, ROUTES } from "./router";
import { useAuthStore } from "./store/authStore";


function App() {
    const hasHydrated = useAuthStore.persist.hasHydrated();
    const { isCaptchaVerified } = useAuthStore();


    if (!hasHydrated) {
        return (
            <div className="d-flex flex-column justify-content-center align-items-center vh-100">
                <LoaderComponent />
            </div>
        )
    }

    return (
        <Router>
            <Routes>
                {ROUTES.map((route, index) => {
                    if (route.isProtected) {
                        return (
                            <Route
                                key={index}
                                path={route.path}
                                element={
                                    isCaptchaVerified ? (
                                        route.component
                                    ) : (
                                        <Navigate to={ROUTE_PATH.SIGN_IN} />
                                    )
                                }
                            />
                        );
                    }

                    return (
                        <Route key={index} path={route.path} element={route.component} />
                    );
                })}

                <Route path="*" element={<Navigate to={ROUTE_PATH.SIGN_IN} />} />
            </Routes>
        </Router>
    );
}

export default App;