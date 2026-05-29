import React, { useContext, useEffect, useState } from "react";
import { ThemeContext } from "../../App";
import { getDirectlineTokenAPI } from "../../apis/APIList";
import { DirectLine } from "botframework-directlinejs";
import ReactWebChat from "botframework-webchat";

const WebChatComponent: React.FC = () => {
    const theme = useContext(ThemeContext);

    const themeClass = theme?.siteVariables?.colors?.brand === "#333"
        ? 'dark-theme'
        : theme?.siteVariables?.colors?.brand === "#000"
            ? 'contrast-theme'
            : 'light-theme';

    console.log("theme: ", theme);
    console.log("themeClass: ", themeClass);

    //#region State
    const [directLine, setDirectLine] = useState<DirectLine | null>(null);
    const [tokenExpireTime, setTokenExpireTime] = useState<number | null>(null);
    //#endregion

    //#region API

    const getDirectlineToken = async (): Promise<void> => {
        try {
            const id = new Date().getTime().toString();
            const jsonBody = {
                "userId": "dl_" + id,
                "userName": id
            };
            console.log('jsonBody: ', jsonBody);

            const response = await getDirectlineTokenAPI(jsonBody);

            console.log("getDirectlineTokenAPI: ", response);
            if (response && response.data) {

                const { token, expires_in, conversationId } = response.data;

                setDirectLine(new DirectLine({ token }));

                // Dispose of the old DirectLine instance before creating a new one
                // setDirectLine((prevDirectLine) => {
                //     if (prevDirectLine) {
                //         prevDirectLine.end();
                //     }
                //     return new DirectLine({ token });
                // });

                // Calculate expiration time in milliseconds
                const expirationTime = new Date().getTime() + expires_in * 1000;
                setTokenExpireTime(expirationTime);

                console.log("DirectLine token set. Conversation ID:", conversationId);
            }
        } catch (error) {
            console.error("Error at getDirectlineToken():", error);
        }
    };

    const refreshDirectlineToken = async (): Promise<void> => {
        console.log("Refreshing Direct Line token...");
        await getDirectlineToken();
    };

    //#endregion

    //#region Effects

    useEffect(() => {
        getDirectlineToken();

        // return () => {
        //     // Cleanup DirectLine instance on component unmount
        //     setDirectLine((prevDirectLine) => {
        //         if (prevDirectLine) {
        //             prevDirectLine.end();
        //         }
        //         return null;
        //     });
        // };
    }, []);

    useEffect(() => {
        // Setup interval to refresh token before it expires
        if (tokenExpireTime) {
            const refreshInterval = setInterval(() => {
                const currentTime = new Date().getTime();

                // Refresh the token 60 seconds before it expires
                if (tokenExpireTime - currentTime <= 60 * 1000) {
                    refreshDirectlineToken();
                }
            }, 30 * 1000); // Check every 30 seconds

            return () => clearInterval(refreshInterval);
        }
    }, [tokenExpireTime]);

    //#endregion

    return (
        <div className={`webchat-container ${themeClass}`}>
            {directLine ? (
                <ReactWebChat
                    directLine={directLine}
                    styleOptions={{ backgroundColor: '#f3f3f3', hideUploadButton: true }}
                />
            ) : (
                <p>Loading chat...</p>
            )}
        </div>
    );
};

export default WebChatComponent;