import React, { createContext, useContext } from "react";
import useTeamsAuth from "./useTeamsAuth";
import { ITeamsToken } from "../Interfaces";

interface IAuthContext {
    teamsSSOToken: ITeamsToken | null;
    teamsSSOError: any;
    teamsSSOUser: any;
}

const AuthContext = createContext<IAuthContext>({
    teamsSSOToken: null,
    teamsSSOError: null,
    teamsSSOUser: null
});

export const AuthProvider: React.FC = ({ children }) => {
    const { teamsSSOToken, teamsSSOError, teamsSSOUser } = useTeamsAuth();

    return (
        <AuthContext.Provider value={{ teamsSSOToken, teamsSSOError, teamsSSOUser }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);