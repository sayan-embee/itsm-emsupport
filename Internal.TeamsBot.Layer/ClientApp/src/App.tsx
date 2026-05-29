import React, { useEffect, useState, createContext } from "react";
import { Provider, teamsTheme, teamsDarkTheme, teamsHighContrastTheme, ThemePrepared } from "@fluentui/react-northstar";
import { BrowserRouter, Route, Redirect, Switch } from "react-router-dom";
import { Routes } from "./router";
import { useTeams } from "msteams-react-base-component";
import * as microsoftTeams from "@microsoft/teams-js";
import "./App.scss";
import "../node_modules/primereact/resources/themes/saga-blue/theme.css";
import "../node_modules/primereact/resources/primereact.min.css";
import "../node_modules/primeicons/primeicons.css";
import { AuthProvider } from "./components/auth/AuthProvider";
import Home from "./components/teamsTab/Home";

export const ThemeContext = createContext<ThemePrepared<any> | null>(null);

function App() {
    const [{ inTeams, theme }] = useTeams();
    const [currentTheme, setCurrentTheme] = useState<ThemePrepared<any> | null>(null);

    useEffect(() => {
        if (inTeams) {
            microsoftTeams.appInitialization.notifySuccess();
        }
    }, [inTeams]);

    useEffect(() => {
        if (theme) {
            setCurrentTheme(theme);
        }
    }, [theme]);

    const fluentTheme =
        currentTheme === teamsDarkTheme
            ? teamsDarkTheme
            : currentTheme === teamsHighContrastTheme
                ? teamsHighContrastTheme
                : teamsTheme;

    return (
        <AuthProvider>
            <ThemeContext.Provider value={currentTheme}>
                <Provider theme={fluentTheme}>
                    <BrowserRouter 
                    //  basename="/TeamsApp"
                    >
                        <Route path="/" component={Home} />
                    </BrowserRouter>
                </Provider>
            </ThemeContext.Provider>
        </AuthProvider>
    );
}

export default App;