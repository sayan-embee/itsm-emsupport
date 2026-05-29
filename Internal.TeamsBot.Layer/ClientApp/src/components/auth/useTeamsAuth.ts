import { useState, useEffect } from "react";
import { app, authentication } from "@microsoft/teams-js";
import jwtDecode from "jwt-decode";
import { ITeamsToken } from "../Interfaces";
import { TeamsUserCredential, TeamsUserCredentialAuthConfig } from "@microsoft/teamsfx";
import { Client } from "@microsoft/microsoft-graph-client";
import { TokenCredentialAuthenticationProvider } from "@microsoft/microsoft-graph-client/authProviders/azureTokenCredentials";

const useTeamsAuth = () => {
    const [teamsSSOToken, setTeamsSSOToken] = useState<ITeamsToken | null>(null);
    const [teamsSSOError, setTeamsSSOError] = useState<string | null>(null);
    const [teamsSSOUser, setTeamsSSOUser] = useState<any | null>(null);

    useEffect(() => {
        const initializeTeamsApp = async () => {
            try {
                await app.initialize();

                authentication.getAuthToken({
                    silent: true,
                    successCallback: async (token: string) => {
                        console.log('initializeTeamsApp token: ', token);
                        try {
                            const decodedToken: any = jwtDecode(token);
                            setTeamsSSOToken(decodedToken);
                            console.log('initializeTeamsApp decodedToken: ', decodedToken);

                            console.log(`user name: ${decodedToken?.name}`);
                            console.log(`user email: ${decodedToken?.unique_name}`);

                        } catch (decodeError) {
                            console.error("Token decoding failed:", decodeError);
                            setTeamsSSOError("Failed to decode token.");
                        }

                        // await fetchUserDetailsUsingTeamsFx();
                    },
                    failureCallback: (error: string) => {
                        console.error("Authentication failed:", error);
                        setTeamsSSOError(`Authentication failed: ${error}`);
                    }
                });
            } catch (error) {
                console.error("Error initializing Teams App:", error);
                setTeamsSSOError("Teams SDK initialization failed.");
            }
        };

        initializeTeamsApp();

        return () => {

        };
    }, []);

    // const fetchUserDetailsUsingTeamsFx = async (): Promise<void> => {
    //     const authConfig: TeamsUserCredentialAuthConfig = {
    //         clientId: "d746e01d-b925-4c5f-903a-c80eff9785c4",
    //         initiateLoginEndpoint: "",
    //     };

    //     const teamsUserCredential = new TeamsUserCredential(authConfig);

    //     try {
    //         // Use the credential to acquire a Graph token silently
    //         const tokenResponse = await teamsUserCredential.getToken(["User.Read"]);

    //         if (!tokenResponse?.token) {
    //             throw new Error("Failed to acquire token from TeamsFx.");
    //         }

    //         // Create an authentication provider using the TeamsUserCredential
    //         const authProvider = new TokenCredentialAuthenticationProvider(teamsUserCredential, {
    //             scopes: ["User.Read"],
    //         });

    //         // Initialize the Microsoft Graph client
    //         const graphClient = Client.initWithMiddleware({ authProvider });

    //         // Fetch the user's profile
    //         const profile = await graphClient.api("/me").get();
    //         const user = profile.mail || profile.userPrincipalName;

    //         console.log("Fetched User Email:", user);
    //         setTeamsSSOUser(user);
    //     } catch (error) {
    //         console.error("Error fetching user email using TeamsFx:", error);
    //         setTeamsSSOError("Failed to fetch user email using TeamsFx.");
    //     }
    // };

    return { teamsSSOToken, teamsSSOError, teamsSSOUser };
};

export default useTeamsAuth;